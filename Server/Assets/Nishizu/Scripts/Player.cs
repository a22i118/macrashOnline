using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.Windows;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;
using PlayerCS;


public class Player
{
    public enum eKind
    {
        take_Idol,
        okada_Idol,
    }
    private bool _isJumping = false;
    private bool _isJumpReset = true;
    private bool _isThrowChargeTime = false;//ため攻撃中か
    private bool _isThrowCoolTime = false;
    private float _jumpHoldTime = 0.0f;
    private float _throwKeyHoldTime = 0.0f;//長押ししている時間

    private const float c_minJumpForce = 6.5f;
    private const float c_maxJumpForce = 9.0f;
    private const float c_maxJumpHoldTime = 0.2f;
    private const float c_pickUpDistance = 1.0f;
    private const float c_throwKeyLongPressTime = 0.5f;//ため攻撃にかかる時間
    private const int c_timeout = 600;    // タイムアウトまでのフレーム数
    private int _timeout = c_timeout;
    private byte _receiveTimer = 0;    // 受信済みフレーム
    private byte _execTimer = 0;    // 処理済みフレーム
    private readonly object _lockObject = new object();
    private List<PacketData> _packets = new List<PacketData>();
    private List<GameObject> _currentMakuras = new List<GameObject>();

    private IPEndPoint _endPoint;
    private GameObject _obj = null;    // プレイヤーGameObject
    private PlayerController _playerController = null;    // プレイヤー操作クラス
    protected Vector3 _lastPos = Vector3.zero;    // 前フレームの位置
    protected Quaternion _lastDir = Quaternion.identity;    // 前フレームの姿勢
    private PacketData.eInputMask _inputMask = 0;    // ボタン入力をマスクにしたもの
    private PacketData.eInputMask _lastInputMask = 0;
    protected PacketData.eStateMask _stateMask = 0;    // 状態を表すマスク

    public IPEndPoint EndPoint { get { return _endPoint; } }
    public GameObject Obj { get { return _obj; } set { _obj = value; } }

    public bool IsThrow { get { return (_inputMask & PacketData.eInputMask.Throw) != 0; } }
    public bool IsJump { get { return (_inputMask & PacketData.eInputMask.Jump) != 0; } }
    public bool IsPickUp { get { return (_inputMask & PacketData.eInputMask.PickUp) != 0; } }

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
        if (IsCheckMakura() && IsPickUp)
        {
            PickUpMakura();
        }
        {
            // if (!_isThrowCoolTime)
            {
                if (_currentMakuras.Count > 0)
                {
                    if (IsThrow)
                    {
                        if (!_isThrowChargeTime)
                        {
                            _throwKeyHoldTime = Time.time;
                            _isThrowChargeTime = true;
                        }
                    }
                    else if (_isThrowChargeTime)
                    {
                        float holdTime = Time.time - _throwKeyHoldTime;

                        float forwardForce = 0.0f;
                        float upwardForce = 0.0f;
                        float throwDistance = 0.0f;
                        float throwHeight = 0.0f;
                        if (holdTime < c_throwKeyLongPressTime)
                        {
                            //if (_makuraController.CurrentColorType == ColorChanger.ColorType.Nomal)
                            {
                                forwardForce = 800.0f;
                                upwardForce = 200.0f;
                                throwDistance = 1.3f;
                                throwHeight = 1.0f;

                            }

                        }
                        else
                        {
                            forwardForce = 300.0f;
                            upwardForce = 700.0f;
                            throwDistance = 0.5f;
                            throwHeight = 2.0f;
                        }
                        Rigidbody rb = _currentMakuras[0].GetComponent<Rigidbody>();
                        MakuraController _makuraController = _currentMakuras[0].GetComponent<MakuraController>();
                        rb.isKinematic = true;
                        rb.isKinematic = false;

                        Vector3 throwDirection = _obj.transform.forward;
                        Vector3 throwPosition = _obj.transform.position + throwDirection * throwDistance + Vector3.up * throwHeight;


                        _currentMakuras[0].transform.position = throwPosition;
                        _currentMakuras[0].SetActive(true);
                        _makuraController.IsThrow = true;
                        _makuraController.IsSetActive = true;
                        _makuraController.Thrower = _obj.gameObject;

                        rb.AddForce(throwDirection * forwardForce + Vector3.up * upwardForce);
                        rb.maxAngularVelocity = 100;
                        rb.AddTorque(Vector3.up * 120.0f);
                        _currentMakuras.RemoveAt(0);
                        _isThrowChargeTime = false;
                    }
                }
                else
                {
                    if (IsThrow)
                    {
                        // StartCoroutine(ThrowCoolTimeCorountine());
                    }
                }
            }
        }

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
            if (_isJumpReset && IsGround)
            {
                _jumpHoldTime = 0.0f;
                _isJumping = true;
                _isJumpReset = false;
            }
        }
        else
        {
            _isJumping = false;
            _isJumpReset = true;
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
        Rigidbody rb = _obj.GetComponent<Rigidbody>();
        rb.velocity = new Vector3(velocity.x * 5, rb.velocity.y, velocity.z * 5);
    }
    private bool IsCheckMakura()
    {
        Vector3 playerPosition = _obj.transform.position;
        Collider[] hitColliders = Physics.OverlapSphere(playerPosition, c_pickUpDistance);
        foreach (var makura in hitColliders)
        {
            if (makura.CompareTag("Makura"))
            {
                // Debug.Log("近くに枕があるぜ！");
                return true;
            }
        }
        return false;
    }
    private void PickUpMakura()
    {
        Collider[] hitColliders = Physics.OverlapSphere(_obj.transform.position, c_pickUpDistance);
        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Makura") && !collider.GetComponent<MakuraController>().IsThrow)
            {
                GameObject currentMakura;

                currentMakura = collider.gameObject;
                currentMakura.transform.SetParent(null);
                currentMakura.GetComponent<MakuraController>().IsSetActive = false;

                currentMakura.SetActive(false);

                _currentMakuras.Add(currentMakura);
                break;
            }
        }
    }
    private IEnumerator ThrowCoolTimeCorountine()
    {
        _isThrowCoolTime = true;
        yield return new WaitForSeconds(0.5f);
        _isThrowCoolTime = false;
    }

}
