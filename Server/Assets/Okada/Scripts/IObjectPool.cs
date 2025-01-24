using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class IObjectPool<T> : MonoBehaviour where T : MonoBehaviour
{
    // Start is called before the first frame update
    protected T _prefab; // 複製するプレハブ
    [SerializeField] private int _poolsize = 10; // プールのサイズ
    protected Transform _init_position;
    protected ObjectPool<T> _pool;

    protected virtual void Awake()
    {
        _pool = new ObjectPool<T>(OnCreatePooledObject, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, true, _poolsize, _poolsize);
    }

    protected virtual T OnCreatePooledObject()
    {

        return Instantiate(_prefab, _init_position.position, Quaternion.identity);
    }

    protected virtual void OnGetFromPool(T obj)
    {

        obj.gameObject.SetActive(true);
    }

    private void OnReleaseToPool(T obj)
    {
        obj.gameObject.SetActive(false);
    }

    private void OnDestroyPooledObject(T obj)
    {
        Destroy(obj);
    }

    public T GetGameObject()
    {
        return _pool.Get();
    }

    public void ReleaseGameObject(T obj)
    {
        _pool.Release(obj);
    }
}
