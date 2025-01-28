using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using UnityEditor.PackageManager;
using PlayerCS;
public class Server : MonoBehaviour
{
    public GameObject _playerPrefab;
    // public GameObject _bulletPrefab;

    // UDP通信のためのクラス
    private UdpClient _udpClient = null;
    // 使用するPort(Server)
    private int _portServer = 53724;
    // ロックオブジェクト（プレイヤーリスト）
    private readonly object _lockObject = new object();
    // 空いているIDのリスト
    private List<byte> _freeIds = new List<byte>(Byte.MaxValue - 1);
    // IDをKeyにしたPlayerのDictionary
    private Dictionary<byte, Player> _players = new Dictionary<byte, Player>();
    // 弾丸のリスト
    // private List<GameObject> _bullets = new List<GameObject>();

    void OnDestroy()
    {
        _players.Clear();

        if (_udpClient != null)
        {
            _udpClient.Close();
            _udpClient.Dispose();
            _udpClient = null;
        }
    }

    void Start()
    {
        // 空いているIDのリストに0～255を順に詰めておく
        for (byte i = 0; i < _freeIds.Capacity; i++)
        {
            _freeIds.Add(i);
        }

        // 待ち受けするためのUdpClientを作成
        _udpClient = new UdpClient(AddressFamily.InterNetworkV6);
        _udpClient.Client.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, 0);
        IPEndPoint localEP = new IPEndPoint(IPAddress.IPv6Any, _portServer);
        _udpClient.Client.Bind(localEP);

        // 待ち受け開始（受信があったときOnReceivedが呼ばれる）
        _udpClient.BeginReceive(OnReceived, _udpClient);
    }

    // 一定の間隔でループさせるためにUpdateではなくFixedUpdateを使う
    void FixedUpdate()
    {
        lock (_lockObject)
        {
            update();
        }
    }

    void update()
    {
        foreach (KeyValuePair<byte, Player> player in _players)
        {
            player.Value.Update(_playerPrefab, transform);

            // if (player.Value.IsFire && _bullets.Count < 255)
            // {
            //     // 弾丸を作成
            //     GameObject bullet = GameObject.Instantiate(_bulletPrefab);
            //     {
            //         // 衝突判定用レイヤーをプレイヤーをプレイヤーと同じにする
            //         bullet.layer = player.Value.Obj.layer;
            //         // 位置と姿勢はプレイヤーからコピー
            //         bullet.transform.position = player.Value.Obj.transform.position;
            //         bullet.transform.rotation = player.Value.Obj.transform.rotation;
            //         // 初速度を与える
            //         Vector3 force = bullet.transform.rotation * new Vector3(0, 0, 1000);
            //         bullet.GetComponent<Rigidbody>().AddForce(force);
            //     }
            //     // 弾丸管理リストに登録
            //     _bullets.Add(bullet);
            // }
        }
        // {
        //     // 削除リスト
        //     List<GameObject> listDelete = new List<GameObject>();

        //     // 全ての弾丸
        //     foreach (var bullet in _bullets)
        //     {
        //         BulletController bc = bullet.GetComponent<BulletController>();
        //         // 弾丸の寿命が尽きていたら削除リストに登録する
        //         if (bc.LifeTime < 0.0f)
        //         {
        //             listDelete.Add(bullet);
        //         }
        //     }

        //     // 削除リストに詰まれた弾丸を削除する
        //     foreach (var item in listDelete)
        //     {
        //         Destroy(item.gameObject);
        //         _bullets.Remove(item);
        //     }
        // }
        {
            List<byte> listRemove = new List<byte>();

            foreach (KeyValuePair<byte, Player> player in _players)
            {
                // 通信がタイムアウトしたプレイヤーを削除リストに詰む
                if (player.Value.DecTimeout())
                {
                    player.Value.OnDestroy();
                    listRemove.Add(player.Key);
                }
            }

            // 削除リストのプレイヤーを削除
            foreach (byte key in listRemove)
            {
                Debug.Log($"disconnect {key}");
                _players.Remove(key);
                _freeIds.Add(key);
            }
        }
        {
            // 送信用リスト
            List<byte> list = new List<byte>();

            // プレイヤー数
            list.Add((byte)_players.Count);
            // 弾丸数
            // list.Add((byte)_bullets.Count);

            // プレイヤー情報をリストに詰む
            foreach (KeyValuePair<byte, Player> player in _players)
                list.AddRange(player.Value.GetBytes(player.Key));

            // 弾丸情報をリストに詰む
            // foreach (GameObject bullet in _bullets)
            //     list.AddRange(bullet.GetComponent<BulletController>().GetBytes());

            // 全プレイヤーに送信
            for (byte i = 0; i < _players.Count; i++)
                _udpClient.Send(list.ToArray(), list.Count, _players[i].EndPoint);
        }
    }

    private void OnReceived(IAsyncResult result)
    {
        UdpClient getUdp = (UdpClient)result.AsyncState;
        IPEndPoint ipEnd = null;

        if (getUdp.Client == null)
        {
            return;
        }

        if (_freeIds.Count <= 0)
        {
            // 空きIDが無い
            goto labelEnd;
        }

        byte[] getByte;
        try
        {
            // getByteに受け取ったデータを読み出す
            getByte = getUdp.EndReceive(result, ref ipEnd);
        }
        catch
        {
            // 読み取りエラー
            goto labelEnd;
        }

        // 最初の接続（1byte以下のデータ）なら
        if (getByte.Length <= 1)
        {
            byte id;

            // Player作成
            Player player = new Player(ipEnd);

            // idの抜き差し中に参照しないようにロックする
            lock (_lockObject)
            {
                // 空きidリストから割り振る
                id = _freeIds[0];
                // 割り振ったidをリストから抜く
                _freeIds.RemoveAt(0);
                // Playerをidから取得できるようにDictionaryに登録
                _players.Add(id, player);
            }

            Debug.Log($"ip {ipEnd.Address.ToString()}, port {ipEnd.Port.ToString()}, id {id}");

            // このプレイヤーに割り当てられたidを返信する
            byte[] data = new byte[1] { id };
            _udpClient.Send(data, data.Length, ipEnd);
        }
        else
        {
            int offset = 1;
            byte userId = getByte[0];

            lock (_lockObject)
            {
                if (_players.ContainsKey(userId))
                {
                    PacketData packet0 = new PacketData();
                    PacketData packet1 = new PacketData();
                    PacketData packet2 = new PacketData();

                    offset = packet0.ReadBytes(getByte, offset);
                    offset = packet1.ReadBytes(getByte, offset);
                    offset = packet2.ReadBytes(getByte, offset);

                    _players[userId].ResetTimeout();
                    _players[userId].Push(packet0);
                    _players[userId].Push(packet1);
                    _players[userId].Push(packet2);
                }
            }
        }

    labelEnd:
        getUdp.BeginReceive(OnReceived, getUdp);
    }
    private void Dispose()
    {
        _udpClient.Dispose();
    }
}
