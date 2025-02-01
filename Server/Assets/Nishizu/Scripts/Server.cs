using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
public class Server : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _makuraPrefab;
    private UdpClient _udpClient = null;    // UDP通信のためのクラス
    private int _portServer = 53724;    // 使用するPort(Server)
    private readonly object _lockObject = new object();    // ロックオブジェクト（プレイヤーリスト）
    private List<byte> _freeIds = new List<byte>(Byte.MaxValue - 1);    // 空いているIDのリスト
    private Dictionary<byte, Player> _players = new Dictionary<byte, Player>();    // IDをKeyにしたPlayerのDictionary
    private List<GameObject> _makuras = new List<GameObject>();

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
        //テストのためマクラを一つ生成
        _makuras.Add(Instantiate(_makuraPrefab, new Vector3(0, 0, 4), Quaternion.identity));
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
        }
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
            // マクラ数
            list.Add((byte)_makuras.Count);

            // プレイヤー情報をリストに詰む
            foreach (KeyValuePair<byte, Player> player in _players)
                list.AddRange(player.Value.GetBytes(player.Key));

            // マクラ情報をリストに詰む
            foreach (GameObject makura in _makuras)
                list.AddRange(makura.GetComponent<MakuraController>().GetBytes());

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
}
