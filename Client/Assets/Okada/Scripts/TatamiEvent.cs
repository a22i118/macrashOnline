using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class TatamiEvent : MonoBehaviour
{
    [SerializeField] private List<GameObject> _tatami = new List<GameObject>();
    private Vector3[] _startPosition = new Vector3[4]; // 初期位置
    private Vector3[] _upPosition = new Vector3[4]; // 最終位置
    int[] _upscale = new int[4]; // 畳のせり上がり段階
    [SerializeField] private float _upSpeed;
    private bool _isUP = false;

    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            _startPosition[i] = _tatami[i].transform.position;
        }
    }
    // Update is called once per frame
    void Update()
    {
        MoveTatami(_isUP);
    }

    public void Init()
    {
        InitializePosition();
        _isUP = !_isUP;
    }

    // 最終位置の設定
    private void InitializePosition()
    {

        for (int i = 0; i < 4; i++)
        {
            if (i % 2 == 1)
            {
                // 手前側の畳
                _upscale[i] = Random.Range(0, 2);
            }
            else
            {
                // 奥側の畳
                _upscale[i] = Random.Range(0, 3);
            }

            _upPosition[i] = _startPosition[i] + new Vector3(0, 1.5f * _upscale[i], 0);
        }

        if (_upscale.All(x => x == 0))
        {
            // どれも上がらなかった場合
            for (int i = 0; i < 4; i++)
            {
                _upPosition[i] = _startPosition[i] + new Vector3(0, 1.5f, 0);
            }
        }

        // 奥だけ上がってる場合の調整
        if (_upscale[0] == 2 && _upscale[1] == 0)
        {
            _upPosition[1].y += 1.5f;
        }
        if (_upscale[2] == 2 && _upscale[3] == 0)
        {
            _upPosition[3].y += 1.5f;
        }

    }

    private void MoveTatami(bool isUP)
    {
        Vector3 _targetPosition0 = isUP ? _upPosition[0] : _startPosition[0];
        Vector3 _targetPosition1 = isUP ? _upPosition[1] : _startPosition[1];
        Vector3 _targetPosition2 = isUP ? _upPosition[2] : _startPosition[2];
        Vector3 _targetPosition3 = isUP ? _upPosition[3] : _startPosition[3];

        _tatami[0].transform.position = Vector3.Lerp(_tatami[0].transform.position, _targetPosition0, _upSpeed * Time.deltaTime);
        _tatami[1].transform.position = Vector3.Lerp(_tatami[1].transform.position, _targetPosition1, _upSpeed * Time.deltaTime);
        _tatami[2].transform.position = Vector3.Lerp(_tatami[2].transform.position, _targetPosition2, _upSpeed * Time.deltaTime);
        _tatami[3].transform.position = Vector3.Lerp(_tatami[3].transform.position, _targetPosition3, _upSpeed * Time.deltaTime);
    }
}
