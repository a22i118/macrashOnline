using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    // 弾の寿命
    private float _lifeTime = 3.0f;
    public float LifeTime { get { return _lifeTime; } }

    void Start()
    {
    }

    void Update()
    {
        // 寿命が残っていたら経過時間を引く
        if (_lifeTime > 0.0f)
        {
            _lifeTime -= Time.deltaTime;
        }
    }

    // Sleep（サーバーで衝突判定やシミュレーションを行う状態）
    public void Sleep()
    {
        // Colliderを無効化
        GetComponent<SphereCollider>().enabled = false;
        // Rigidbodyを無効化（位置を姿勢指定のみで動かす）
        GetComponent<Rigidbody>().isKinematic = true;
    }

    // WakeUp（ローカルで衝突判定やシミュレーションを行う状態）
    public void WakeUp()
    {
        // Colliderを有効化
        GetComponent<SphereCollider>().enabled = true;
        // Rigidbodyを有効化
        GetComponent<Rigidbody>().isKinematic = false;
    }
}
