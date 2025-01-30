using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static Player;
using PlayerCS;

// オフライン用Player、ネットワーク用Playerの基底クラス
public abstract class PlayerBase
{
    // プレイヤーID（サーバーが割り当てる）
    protected byte _id = byte.MaxValue;
    // プレイヤーGameObject
    protected GameObject _obj = null;
    // プレイヤー操作クラス
    protected PlayerController _playerController = null;
    // 前フレームの位置
    protected Vector3 _lastPos = Vector3.zero;
    // 前フレームの姿勢
    protected Quaternion _lastDir = Quaternion.identity;
    // 移動速度
    //protected Vector3 _force = Vector3.zero;
    // アナログレバー入力の値
    protected Vector2 _inputMovement;
    // ボタン入力をマスクにしたもの
    protected PacketData.eInputMask _inputMask = 0;
    // 状態を表すマスク
    protected PacketData.eStateMask _stateMask = 0;
    // eStateMaskが参照されたらtrueになるマスク
    protected bool _isStateUsed = true;

    public byte Id { get { return _id; } set { _id = value; } }
    public GameObject Obj { get { return _obj; } set { _obj = value; } }
    public Vector2 Movement { get { return _playerController.Movement; } }
    public Vector3 InputMovement { get { return _inputMovement; } }
    public PacketData.eInputMask InputMask { get { return _inputMask; } }
    public bool IsJump { get { return (_inputMask & PacketData.eInputMask.Jump) != 0; } }
    public bool IsThrow { get { return (_inputMask & PacketData.eInputMask.Throw) != 0; } }
    public bool IsPickUp { get { return (_inputMask & PacketData.eInputMask.PickUp) != 0; } }
    public bool IsGround { get { _isStateUsed = true; return (_stateMask & PacketData.eStateMask.Ground) != 0; } }

    public PlayerBase(GameObject prefab, Transform parent)
    {
        // PrefabからGameObjectを作成
        _obj = GameObject.Instantiate(prefab);
        // parentの子にする
        _obj.transform.parent = parent.transform;
        // コンポーネント
        _playerController = _obj.GetComponent<PlayerController>();
    }

    // このプレイヤーの有効と無効を切り替える
    public void SetActive(bool flag) { if (_obj) { _obj.SetActive(flag); } }

    // サーバーから送られてきたデータからサーバー上で計算されたプレイヤーの状態を復元する
    public virtual int ReadByte(byte[] getByte, int offset) { return 0; }

    // 更新
    public virtual void Update(PlayerInput input)
    {
        // アナログレバーの値
        _inputMovement = input.InputMovement;

        // PlayerInputから入力のマスクを作成
        _inputMask = 0;

        // if (input.IsMenu) { _inputMask |= PacketData.eInputMask.Menu; }
        if (input.IsPickUp) { _inputMask |= PacketData.eInputMask.PickUp; }
        if (input.IsThrow) { _inputMask |= PacketData.eInputMask.Throw; }
        if (input.IsJump) { _inputMask |= PacketData.eInputMask.Jump; }
        // 使い終わったPlayerInputの状態は初期化
        // input.Reset();

        // forceを計算する
        _playerController.UpdateForce(this);
    }
}

// オフライン用Player
public class Player : PlayerBase
{
    // 表示モデルの種類
    public enum eKind
    {
        take_Idol,
        okada_Idol,
    }

    public Player(GameObject prefab, Transform parent)
        : base(prefab, parent)
    {
    }

    public override void Update(PlayerInput input)
    {
        // 基底クラスのUpdate
        base.Update(input);

        _stateMask = 0;

        // 移動速度（y方向は含めない）
        Rigidbody rigidbody = _obj.GetComponent<Rigidbody>();
        _playerController.Speed = Mathf.Abs(Vector3.Dot(new Vector3(1.0f, 0.0f, 1.0f), rigidbody.velocity));

        // 角度を更新（移動量で補間する）
        Vector3 d = _obj.transform.position - _lastPos;
        d.y = 0.0f;
        if (d != Vector3.zero)
        {
            _obj.transform.rotation = Quaternion.Slerp(_lastDir, Quaternion.LookRotation(d), Mathf.Clamp(0.0f, 1.0f, d.magnitude));
        }

        // 位置と姿勢を保存
        _lastPos = _obj.transform.position;
        _lastDir = _obj.transform.rotation;

        // フォースを加える
        rigidbody.AddForce(Movement);
    }
}

// ネットワーク用Player
public class NetPlayer : PlayerBase
{
    public NetPlayer(GameObject prefab, Transform parent)
        : base(prefab, parent)
    {
        // 受信したデータに従うため、プレイヤー操作は停止させる
        _playerController.Sleep();
    }

    // 受信したbyte配列からデータを復元する
    public override int ReadByte(byte[] getByte, int offset)
    {
        // 位置
        float px = System.BitConverter.ToSingle(getByte, offset); offset += sizeof(float);
        float py = System.BitConverter.ToSingle(getByte, offset); offset += sizeof(float);
        float pz = System.BitConverter.ToSingle(getByte, offset); offset += sizeof(float);
        _obj.transform.position = new Vector3(px, py, pz);

        // 移動速度
        _playerController.Speed = System.BitConverter.ToSingle(getByte, offset); offset += sizeof(float);

        // 姿勢
        float rx = System.BitConverter.ToSingle(getByte, offset); offset += sizeof(float);
        float ry = System.BitConverter.ToSingle(getByte, offset); offset += sizeof(float);
        float rz = System.BitConverter.ToSingle(getByte, offset); offset += sizeof(float);
        float rw = System.BitConverter.ToSingle(getByte, offset); offset += sizeof(float);
        _obj.transform.rotation = new Quaternion(rx, ry, rz, rw);

        // ID
        _id = getByte[offset]; offset += sizeof(byte);

        //  表示モデルの種類
        _playerController.Kind = (eKind)getByte[offset]; offset += sizeof(byte);

        // 入力マスクをクリア
        //_inputMask = 0;

        // 状態マスクは参照済（すでに使われていたら）上書き、未参照（まだつかわれていなければ）ORを取ることで前の状態も残す
        if (_isStateUsed) { _stateMask = (PacketData.eStateMask)getByte[offset]; offset += sizeof(byte); }
        else { _stateMask |= (PacketData.eStateMask)getByte[offset]; offset += sizeof(byte); }

        // 位置と姿勢を保存
        _lastPos = _obj.transform.position;
        _lastDir = _obj.transform.rotation;

        return offset;
    }
}
