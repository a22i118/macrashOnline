using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Makura
{
    protected byte _id = byte.MaxValue;
    // MakuraのGameObject
    protected GameObject _obj = null;
    protected MakuraController _makuraController = null;
    // 状態を表すマスク
    protected PacketData.eStateMask _stateMask = 0;
    // eStateMaskが参照されたらtrueになるマスク
    protected bool _isStateUsed = true;
    public byte Id { get { return _id; } set { _id = value; } }
    public GameObject Obj { get { return _obj; } set { _obj = value; } }
    public Makura(GameObject prefab, bool isSleep)
    {
        // PrefabからGameObjectを作成
        _obj = GameObject.Instantiate(prefab);
        // コンポーネント
        _makuraController = _obj.GetComponent<MakuraController>();

        // ネットワークプレイのときはSleepする
        if (isSleep) { _makuraController.Sleep(); }
    }
    public int ReadByte(byte[] getByte, int offset)
    {
        // 位置
        float px = System.BitConverter.ToSingle(getByte, offset); offset += sizeof(float);
        float py = System.BitConverter.ToSingle(getByte, offset); offset += sizeof(float);
        float pz = System.BitConverter.ToSingle(getByte, offset); offset += sizeof(float);
        _obj.transform.position = new Vector3(px, py, pz);

        // 移動速度
        float speed = System.BitConverter.ToSingle(getByte, offset); offset += sizeof(float);

        // 姿勢
        float rx = System.BitConverter.ToSingle(getByte, offset); offset += sizeof(float);
        float ry = System.BitConverter.ToSingle(getByte, offset); offset += sizeof(float);
        float rz = System.BitConverter.ToSingle(getByte, offset); offset += sizeof(float);
        float rw = System.BitConverter.ToSingle(getByte, offset); offset += sizeof(float);
        _obj.transform.rotation = new Quaternion(rx, ry, rz, rw);

        // 状態マスクは参照済（すでに使われていたら）上書き、未参照（まだつかわれていなければ）ORを取ることで前の状態も残す
        if (_isStateUsed) { _stateMask = (PacketData.eStateMask)getByte[offset]; offset += sizeof(byte); }
        else { _stateMask |= (PacketData.eStateMask)getByte[offset]; offset += sizeof(byte); }

        if ((_stateMask & PacketData.eStateMask.SetActive) != 0)
        {
            _obj.SetActive(true);
        }
        else
        {
            _obj.SetActive(false);
        }
        return offset;
    }
}