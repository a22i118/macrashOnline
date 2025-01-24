using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEditor.Sprites;
using UnityEditor.Presets;
using UnityEngine.Windows;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;
using UnityEditor.PackageManager;
using PlayerCS;
public class Player
{
    public enum eKind
    {
        take_Idol,
        okada_Idol,
    }

    // タイムアウトまでのフレーム数
    private const int c_timeout = 600;
    private int _timeout = c_timeout;
    // 受信済みフレーム
    private byte _receiveTimer = 0;
    // 処理済みフレーム
    private byte _execTimer = 0;
    private readonly object _lockObject = new object();
    private List<PacketData> _packets = new List<PacketData>();
    private IPEndPoint _endPoint;
    // プレイヤーGameObject
    private GameObject _obj = null;
    // プレイヤー操作クラス
    private PlayerController _playerController = null;
    // 前フレームの位置
    protected Vector3 _lastPos = Vector3.zero;
    // 前フレームの姿勢
    protected Quaternion _lastDir = Quaternion.identity;
    // ボタン入力をマスクにしたもの
    private PacketData.eInputMask _inputMask = 0;
    // 状態を表すマスク
    protected PacketData.eStateMask _stateMask = 0;

    public IPEndPoint EndPoint { get { return _endPoint; } }
    public GameObject Obj { get { return _obj; } set { _obj = value; } }

    public bool IsFire { get { return (_inputMask & PacketData.eInputMask.Throw) != 0; } }
    public bool IsJump { get { return (_inputMask & PacketData.eInputMask.Jump) != 0; } }
    // public bool IsChangeCharacter { get { return (_inputMask & PacketData.eInputMask.ChangeCharacter) != 0; } }
    public bool IsGround { get { return _obj.GetComponent<PlayerController>().IsGround(); } }
    public bool IsReset { get { return (_stateMask & PacketData.eStateMask.Reset) != 0; } }

    public PacketData.eStateMask SendState
    {
        get
        {
            PacketData.eStateMask stateMask = 0;
            if (IsGround) { stateMask |= PacketData.eStateMask.Ground; }
            if (IsReset) { stateMask |= PacketData.eStateMask.Reset; }
            return stateMask;
        }
    }

    public Player(IPEndPoint iPEndPoint) { _endPoint = iPEndPoint; }

    public void OnDestroy() { GameObject.Destroy(_obj); }

    public void Update(GameObject prefab, Transform parent)
    {
        Vector3 force = new Vector3();
        PacketData.eInputMask inputMask = 0;

        lock (_lockObject)
        {
            byte timer;
            for (timer = _execTimer; timer != _receiveTimer; timer++)
            {
                PacketData p = _packets.Find(packet => timer == packet.Timer);
                if (p != null)
                {
                    _execTimer = timer;
                    force += p.Force;
                    inputMask |= p.InputMask;
                    _packets.Remove(p);
                }
            }
            for (byte i = 0; i < 3; i++)
            {
                PacketData p = _packets.Find(packet => (timer + i) == packet.Timer);
                if (p != null)
                {
                    _execTimer = (byte)(timer + i);
                    force += p.Force;
                    inputMask |= p.InputMask;
                    _packets.Remove(p);
                }
            }
        }

        _inputMask = inputMask;
        _stateMask = 0;

        if (_obj == null)
        {
            _obj = GameObject.Instantiate(prefab);
            _obj.transform.parent = parent;
            _playerController = _obj.GetComponent<PlayerController>();
            _stateMask |= PacketData.eStateMask.Reset;
        }

        Vector3 d = _obj.transform.position - _lastPos;

        d.y = 0.0f;

        if (d != Vector3.zero)
        {
            _obj.transform.rotation = Quaternion.Slerp(_lastDir, Quaternion.LookRotation(d), Mathf.Clamp(0.0f, 1.0f, d.magnitude));
        }

        bool isReset = false;

        // if (IsChangeCharacter)
        // {
        //     _playerController.Kind = _playerController.Kind == eKind.police ? eKind.zombie : eKind.police;
        //     _obj.layer = LayerMask.NameToLayer("Player1") + (int)_playerController.Kind;
        //     isReset = true;
        // }

        if (_obj.transform.position.y < -8.0f)
        {
            isReset = true;
        }

        if (isReset)
        {
            _playerController.Restart();
            _stateMask |= PacketData.eStateMask.Reset;
        }

        _lastPos = _obj.transform.position;
        _lastDir = _obj.transform.rotation;

        if (IsGround && IsJump)
        {
            force.y += 24.0f * 8.0f;
        }
        _obj.GetComponent<Rigidbody>().AddForce(force);
    }

    public void ResetTimeout() { _timeout = c_timeout; }

    public bool DecTimeout() { return _timeout <= 0 ? true : (--_timeout <= 0); }

    public void Push(PacketData data)
    {
        // 処理済みのパケットは無視する
        if (data.Timer == _execTimer || data.Timer == _execTimer - 1 || data.Timer == _execTimer - 2)
        {
            return;
        }

        lock (_lockObject)
        {
            int indx = _packets.FindIndex(i => data.Timer == i.Timer);

            if (indx < 0)
            {
                _packets.Add(data);
                _receiveTimer = data.Timer;
            }
        }
    }

    public byte[] GetBytes(byte key)
    {

        Vector3 v = Vector3.zero;
        Quaternion q = Quaternion.identity;
        float speed = 0.0f;

        if (_obj != null)
        {
            v = _obj.transform.position;
            q = _obj.transform.rotation;
            speed = Mathf.Abs(Vector3.Dot(new Vector3(1.0f, 0.0f, 1.0f), _obj.GetComponent<Rigidbody>().velocity));
        }

        List<byte> list = new List<byte>();

        list.AddRange(BitConverter.GetBytes(v.x));
        list.AddRange(BitConverter.GetBytes(v.y));
        list.AddRange(BitConverter.GetBytes(v.z));
        list.AddRange(BitConverter.GetBytes(speed));
        list.AddRange(BitConverter.GetBytes(q.x));
        list.AddRange(BitConverter.GetBytes(q.y));
        list.AddRange(BitConverter.GetBytes(q.z));
        list.AddRange(BitConverter.GetBytes(q.w));
        list.Add(key);
        list.Add((byte)_playerController.Kind);
        list.Add((byte)SendState);

        return list.ToArray();
    }
}
