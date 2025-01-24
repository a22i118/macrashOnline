using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HutonController : MonoBehaviour
{
    private GameObject _makura;
    public GameObject Makura { get => _makura; set => _makura = value; }
    /// <summary>
    /// 布団のポジションを返す（自分自身）
    /// </summary>
    /// <returns></returns>
    public Vector3 GetCenterPosition()
    {
        return GetComponent<Collider>().bounds.center;
    }
    /// <summary>
    /// 布団の向きを返す（自分自身）
    /// </summary>
    /// <returns></returns>
    public Quaternion GetRotation()
    {
        return transform.rotation;
    }

    // Start is called before the first frame update
    void Start()
    {
        _makura = transform.GetChild(0).gameObject;
        _makura.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Makura") || collision.gameObject.CompareTag("Obstacles"))
        {
            collision.transform.SetParent(transform.parent);
        }
    }
}
