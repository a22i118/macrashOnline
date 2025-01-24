using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeaherMakuraController : MonoBehaviour
{
    private float _speed = 10f;
    private float _rotationSpeed = 5f;
    private Transform _target;
    private GameObject _targetPlayer;
    public Transform Target { get => _target; set => _target = value; }
    public GameObject TargetPlayer { get => _targetPlayer; set => _targetPlayer = value; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_target != null)
        {
            Vector3 direction = _target.position - transform.position;

            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _rotationSpeed * Time.deltaTime);

            transform.Translate(Vector3.forward * _speed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == TargetPlayer && other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject, 0.2f);
        }
    }
}
