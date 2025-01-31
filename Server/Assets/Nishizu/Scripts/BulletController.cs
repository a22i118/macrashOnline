using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    // ’e‚ÌŽõ–½
    private float _lifeTime = 3.0f;
    public float LifeTime { get { return _lifeTime; } }

    void Update()
    {
        // Žõ–½‚ªŽc‚Á‚Ä‚¢‚½‚çŒo‰ßŽžŠÔ‚ðˆø‚­
        if (_lifeTime > 0.0f)
        {
            _lifeTime -= Time.deltaTime;
        }
    }

    public byte[] GetBytes()
    {
        Vector3 v = transform.position;
        Quaternion q = transform.rotation;
        float speed = Mathf.Abs(Vector3.Dot(new Vector3(1.0f, 0.0f, 1.0f), GetComponent<Rigidbody>().velocity));

        List<byte> list = new List<byte>();

        list.AddRange(BitConverter.GetBytes(v.x));
        list.AddRange(BitConverter.GetBytes(v.y));
        list.AddRange(BitConverter.GetBytes(v.z));
        list.AddRange(BitConverter.GetBytes(speed));
        list.AddRange(BitConverter.GetBytes(q.x));
        list.AddRange(BitConverter.GetBytes(q.y));
        list.AddRange(BitConverter.GetBytes(q.z));
        list.AddRange(BitConverter.GetBytes(q.w));
        list.Add((byte)gameObject.layer);

        return list.ToArray();
    }
}
