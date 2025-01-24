using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MeteorEvent : MonoBehaviour
{
    [SerializeField] private Vector3 _center; // ステージの中心位置
    [SerializeField] private float _interval; // 発射の間隔
    private float _accumulate; //経過時間
    private bool _isFall = false;
    private MeteorPool _meteorPool;
    private MeteorMarkerPool _markerPool;
   [SerializeField] private LayerMask _groundLayer;

    void Start()
    {
        _meteorPool = FindObjectOfType<MeteorPool>();
        _markerPool = FindObjectOfType<MeteorMarkerPool>();
        // 確認用のコード
        // _isFall = true;
        // StartCoroutine(FallCoroutine());
    }

    //このイベント開始時に呼び出される関数
    public void Init()
    {
        _isFall = !_isFall;
        if (_isFall)
        {
            StartCoroutine(FallCoroutine());
        }
    }

    private IEnumerator FallCoroutine()
    {
        while (_isFall)
        {
            _accumulate += Time.deltaTime;
            if (_accumulate >= _interval)
            {
                //経過時間をランダムに短縮
                _accumulate = Random.Range(0.1f, 1.7f);


                // ランダムな生成位置を計算
                float _finalposition_x = _center.x + Random.Range(-7.5f, 7.5f);
                float _finalposition_z = _center.z + Random.Range(-4.5f, 4.5f);

                // マクラメテオとマーカーの生成
                MakuraMeteor _meteor = _meteorPool.GetGameObject();
                _meteor.transform.position = new Vector3(_finalposition_x, 15f, _finalposition_z);
                MeteorMarker _marker = _markerPool.GetGameObject();
                _marker.MarkerMeteor = _meteor;
                Physics.Raycast(_meteor.transform.position + Vector3.up * 10, Vector3.down, out RaycastHit hit, 30, _groundLayer);
                _marker.transform.position = new Vector3(_finalposition_x, hit.point.y + 0.01f, _finalposition_z);
            }

            yield return null;
        }
    }





}
