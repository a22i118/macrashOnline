using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// 参考にしたサイト
// see https://huchat-gamedev.net/explanation-object-pool/
public class MeteorPool : MonoBehaviour
{
    [SerializeField] private MakuraMeteor _meteorPrefab;
    [SerializeField] private Transform _meteorInitposition;
    private int _poolsize = 10; // プールのサイズ
    private ObjectPool<MakuraMeteor> _pool;

    protected virtual void Awake()
    {
        _pool = new ObjectPool<MakuraMeteor>(OnCreatePooledObject, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, true, _poolsize, _poolsize*2);
    }

    protected virtual MakuraMeteor OnCreatePooledObject()
    {

        return Instantiate(_meteorPrefab, _meteorInitposition.position, Quaternion.identity);
    }

    private void OnGetFromPool(MakuraMeteor obj)
    {
        obj.Initialize(() => _pool.Release(obj));
        obj.gameObject.SetActive(true);
    }

    private void OnReleaseToPool(MakuraMeteor obj)
    {
        obj.gameObject.SetActive(false);
    }

    private void OnDestroyPooledObject(MakuraMeteor obj)
    {
        Destroy(obj);
    }

    public MakuraMeteor GetGameObject()
    {
        return _pool.Get();
    }

    public void ReleaseGameObject(MakuraMeteor obj)
    {
        _pool.Release(obj);
    }
}
