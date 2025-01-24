using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private bool _isOpen = false;//ドアが開いているかどうか
    private bool _isGameStart = false;
    private float _openSpeed = 3.0f;
    private Vector3[] _closedPosition = new Vector3[2];
    private Vector3[] _openPosition = new Vector3[2];
    private List<GameObject> _door = new List<GameObject>();
    public bool IsOpen { get => _isOpen; set => _isOpen = value; }
    public bool IsGameStart { get => _isGameStart; set => _isGameStart = value; }

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            _door.Add(child.gameObject);
        }
        _closedPosition[0] = _door[0].transform.position;
        _openPosition[0] = _closedPosition[0] + new Vector3(-0.6f, 0, 0);//左のドア
        _openPosition[0].y = _door[0].transform.position.y;

        _closedPosition[1] = _door[1].transform.position;
        _openPosition[1] = _closedPosition[1] + new Vector3(0.6f, 0, 0);//右のドア
        _openPosition[1].y = _door[1].transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isGameStart)
        {
            MoveDoors(_isOpen);
        }
    }
    /// <summary>
    /// ドアを動かす
    /// </summary>
    /// <param name="isOpen">に応じて、true開ける、folse閉じる</param>
    private void MoveDoors(bool isOpen)
    {
        Vector3 targetPositionLeft = isOpen ? _openPosition[0] : _closedPosition[0];
        Vector3 targetPositionRight = isOpen ? _openPosition[1] : _closedPosition[1];

        _door[0].transform.position = Vector3.Lerp(_door[0].transform.position, targetPositionLeft, _openSpeed * Time.deltaTime);
        _door[1].transform.position = Vector3.Lerp(_door[1].transform.position, targetPositionRight, _openSpeed * Time.deltaTime);
    }
    /// <summary>
    /// ドアを開ける
    /// </summary>
    public void OpenDoors()
    {
        _door[0].transform.position = new Vector3(_door[0].transform.position.x - 0.6f, _door[0].transform.position.y, _door[0].transform.position.z);
        _door[1].transform.position = new Vector3(_door[1].transform.position.x + 0.6f, _door[1].transform.position.y, _door[1].transform.position.z);

    }
}
