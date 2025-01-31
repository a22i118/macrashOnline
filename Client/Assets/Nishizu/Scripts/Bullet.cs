using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet
{
    // 弾丸GameObject
    private GameObject _obj = null;
    // 弾丸操作クラス
    private BulletController _bulletController = null;

    public GameObject Obj { get { return _obj; } set { _obj = value; } }

    public Bullet(GameObject prefab, Transform parent, bool isSleep)
    {
        // PrefabからGameObjectを作成
        _obj = GameObject.Instantiate(prefab);
        // parentの子にする
        _obj.transform.parent = parent.transform;
        // コンポーネント
        _bulletController = _obj.GetComponent<BulletController>();

        // ネットワークプレイのときはSleepする
        if (isSleep) { _bulletController.Sleep(); }
    }

    // 受信したbyte配列からデータを復元する
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

        //  衝突判定用レイヤー
        _obj.layer = getByte[offset]; offset += sizeof(byte);

        return offset;
    }
}
