using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MakuraMeteor : MonoBehaviour
{
    private Rigidbody _rb;
    private Action _onDisable;
    private float _accumulate; //経過時間

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.maxAngularVelocity = 10;
        // _rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        //消えなかったときに時間経過でプールに返す
        _accumulate += Time.deltaTime;

        if (_accumulate >= 5)
        {
            _onDisable?.Invoke();
            gameObject.SetActive(false);

        }


        _rb.AddTorque(Vector3.up * 100);
    }
    //オブジェクトの初期化
    public void Initialize(Action onDisable)
    {
        //非アクティブ化の際に実行されるコールバックの設定
        _onDisable = onDisable;
        _accumulate = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //ものに当たったらプールに返す
        if (gameObject.activeSelf)
        {
            _onDisable?.Invoke();
            gameObject.SetActive(false);
            // StartCoroutine(DestroyMeteorCoroutine());
        }

    }
    private void OnDisable()
    {
        _rb.velocity = Vector3.zero;
    }
    private IEnumerator DestroyMeteorCoroutine()
    {

        yield return new WaitForSeconds(0.5f);
        _onDisable?.Invoke();
        gameObject.SetActive(false);
    }
}