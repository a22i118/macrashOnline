using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorMarker : MonoBehaviour
{
    private Action _onDisable;
    private float _accumulate; //経過時間
    //セットとなるメテオの取得
    private MakuraMeteor _meteor;
    public MakuraMeteor MarkerMeteor { set =>_meteor = value; }
    void Update()
    {
        //時間経過で消去
        _accumulate += Time.deltaTime;

        if (! _meteor.gameObject.activeSelf)
        {
            _onDisable?.Invoke();
            gameObject.SetActive(false);

        }
    }
    //オブジェクトの初期化
    public void Initialize(Action onDisable)
    {
        //非アクティブ化の際に実行されるコールバックの設定
        _onDisable = onDisable;
        _accumulate = 0;
    }


}
