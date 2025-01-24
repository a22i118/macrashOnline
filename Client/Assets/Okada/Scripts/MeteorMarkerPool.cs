using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MeteorMarkerPool : MonoBehaviour
{
    [SerializeField] private MeteorMarker _markerPrefab;
    [SerializeField] private Transform _markerInitposition;
    private int _poolsize = 10; // プールのサイズ
    private ObjectPool<MeteorMarker> _pool;

    protected virtual void Awake()
    {
        _pool = new ObjectPool<MeteorMarker>(OnCreatePooledObject, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, true, _poolsize, _poolsize*2);
    }

    protected virtual MeteorMarker OnCreatePooledObject()
    {

        return Instantiate(_markerPrefab, _markerInitposition.position, Quaternion.identity);
    }

    private void OnGetFromPool(MeteorMarker obj)
    {
        obj.Initialize(() => _pool.Release(obj));
        obj.gameObject.SetActive(true);
    }

    private void OnReleaseToPool(MeteorMarker obj)
    {
        obj.gameObject.SetActive(false);
    }

    private void OnDestroyPooledObject(MeteorMarker obj)
    {
        Destroy(obj);
    }

    public MeteorMarker GetGameObject()
    {
        return _pool.Get();
    }

    public void ReleaseGameObject(MeteorMarker obj)
    {
        _pool.Release(obj);
    }
}
