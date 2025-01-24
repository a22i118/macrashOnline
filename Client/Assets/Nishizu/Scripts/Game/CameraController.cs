using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    private Vector3 _offset;
    // Start is called before the first frame update
    void Start()
    {
        // _offset = transform.position - _player.transform.position;
        _offset = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _player.transform.position + _offset;
    }
}
