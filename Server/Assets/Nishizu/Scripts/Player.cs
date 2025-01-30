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
    private bool _isJumping = false;
    private float _jumpHoldTime = 0.0f;
    private const float c_minJumpForce = 6.5f;
    private const float c_maxJumpForce = 9.0f;
    private const float c_maxJumpHoldTime = 0.2f;

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
    private PacketData.eInputMask _lastInputMask = 0;
    // 状態を表すマスク
    protected PacketData.eStateMask _stateMask = 0;

    public IPEndPoint EndPoint { get { return _endPoint; } }
    public GameObject Obj { get { return _obj; } set { _obj = value; } }

    public bool IsFire { get { return (_inputMask & PacketData.eInputMask.Throw) != 0; } }
    public bool IsJump { get { return (_inputMask & PacketData.eInputMask.Jump) != 0; } }
    public bool IsGround { get { return _obj.GetComponent<PlayerController>().IsGround(); } }

    public PacketData.eStateMask SendState
    {
        get
        {
            PacketData.eStateMask stateMask = 0;
            if (IsGround) { stateMask |= PacketData.eStateMask.Ground; }
            return stateMask;
        }
    }

    public Player(IPEndPoint iPEndPoint) { _endPoint = iPEndPoint; }

    public void OnDestroy() { GameObject.Destroy(_obj); }

    public void Update(GameObject prefab, Transform parent)
    {
        Vector3 force = new Vector3();
        Vector3 velocity = new Vector3();
        PacketData.eInputMask inputMask = _lastInputMask;


        lock (_lockObject)
        {
            byte timer;

            for (timer = _execTimer; timer != _receiveTimer; timer++)
            {
                PacketData p = _packets.Find(packet => timer == packet.Timer);
                if (p != null)
                {
                    _execTimer = timer;
                    velocity = new Vector3(p.Movement.x, 0, p.Movement.y);
                    inputMask = p.InputMask;
                    _packets.Remove(p);
                }
            }

            // for (byte i = 0; i < 3; i++)
            // {
            //     PacketData p = _packets.Find(packet => (timer + i) == packet.Timer);
            //     if (p != null)
            //     {
            //         _execTimer = (byte)(timer + i);
            //         velocity = new Vector3(p.Movement.x, 0, p.Movement.y);
            //         inputMask |= p.InputMask;
            //         _packets.Remove(p);
            //     }
            // }
        }

        _inputMask = inputMask;

        _stateMask = 0;

        if (_obj == null)
        {
            _obj = GameObject.Instantiate(prefab);
            _obj.transform.parent = parent;
            _playerController = _obj.GetComponent<PlayerController>();
        }

        Jump();
        Move(velocity);

        _lastInputMask = _inputMask;
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
    private void Jump()
    {
        if (IsJump)
        {
            if (IsGround)
            {
                _jumpHoldTime = 0.0f;
                _isJumping = true;
            }
        }
        else
        {
            _isJumping = false;
        }

        if (_isJumping)
        {
            if (_jumpHoldTime < c_maxJumpHoldTime)
            {
                _jumpHoldTime += Time.deltaTime;
            }
            else
            {
                _jumpHoldTime = c_maxJumpHoldTime;
                _isJumping = false;
            }

            float jumpForce = Mathf.Lerp(c_minJumpForce, c_maxJumpForce, _jumpHoldTime / c_maxJumpHoldTime);
            _obj.GetComponent<Rigidbody>().velocity = new Vector3(_obj.GetComponent<Rigidbody>().velocity.x, jumpForce, _obj.GetComponent<Rigidbody>().velocity.z);
        }
    }
    private void Move(Vector3 velocity)
    {
        Vector3 dir = _obj.transform.position - _lastPos;

        dir.y = 0.0f;

        if (dir != Vector3.zero)
        {
            _obj.transform.rotation = Quaternion.Slerp(_lastDir, Quaternion.LookRotation(dir), Mathf.Clamp(0.0f, 1.0f, dir.magnitude));
        }

        _lastPos = _obj.transform.position;
        _lastDir = _obj.transform.rotation;
        _obj.GetComponent<Rigidbody>().velocity = new Vector3(velocity.x * 5, _obj.GetComponent<Rigidbody>().velocity.y, velocity.z * 5);
    }
}
