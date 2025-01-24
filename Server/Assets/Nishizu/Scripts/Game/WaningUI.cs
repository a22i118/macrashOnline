using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningUI : MonoBehaviour
{
    [SerializeField] GameObject _warningPrefab;
    [SerializeField] private float _generationTimeInterval = 1.285f;
    private bool _isWarning = false;
    private bool _isNearingEnd = false;
    private bool _isExecuteOnce = false;//一回だけ実行する
    private float _timer = 0.0f;
    private float _generationInterval;
    private Vector2 _spawnPosition = new Vector2(1320f, 500f);

    // Start is called before the first frame update
    void Start()
    {
        _generationInterval = _warningPrefab.GetComponent<RectTransform>().sizeDelta.x * _warningPrefab.GetComponent<RectTransform>().localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isWarning)
        {
            _timer += Time.deltaTime;

            if (_timer >= _generationTimeInterval && !_isExecuteOnce)
            {
                if (!_isNearingEnd)
                {
                    GeneratesWarning(_isNearingEnd);
                }
                else
                {
                    _isExecuteOnce = true;
                    GeneratesWarning(_isNearingEnd);
                }

                _timer = 0.0f;
            }
        }
    }
    private void GeneratesWarning(bool isNearingEnd)
    {
        if (!isNearingEnd)
        {
            GameObject warningTop = Instantiate(_warningPrefab, _spawnPosition, Quaternion.identity);
            GameObject warningUnder = Instantiate(_warningPrefab, -_spawnPosition, Quaternion.identity);
            warningTop.transform.SetParent(transform, false);
            warningUnder.transform.SetParent(transform, false);
            warningUnder.GetComponent<WarningMove>().Direction = false;
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject warningTop = Instantiate(_warningPrefab, _spawnPosition + new Vector2(_generationInterval * i, 0), Quaternion.identity);
                GameObject warningUnder = Instantiate(_warningPrefab, -_spawnPosition - new Vector2(_generationInterval * i, 0), Quaternion.identity);
                warningTop.transform.SetParent(transform, false);
                warningUnder.transform.SetParent(transform, false);
                warningTop.GetComponent<WarningMove>().StartCoroutine(warningTop.GetComponent<WarningMove>().FadeOutCoroutine());
                warningUnder.GetComponent<WarningMove>().StartCoroutine(warningUnder.GetComponent<WarningMove>().FadeOutCoroutine());
                warningUnder.GetComponent<WarningMove>().Direction = false;
            }
        }
    }
    public void Init()
    {
        _isWarning = true;
        _isNearingEnd = false;
        _isExecuteOnce = false;
        StartCoroutine(WarningCoroutine());
        for (int i = 0; i < 4; i++)
        {
            GameObject warningTop = Instantiate(_warningPrefab, _spawnPosition - new Vector2(_generationInterval * i, 0), Quaternion.identity);
            GameObject warningUnder = Instantiate(_warningPrefab, -_spawnPosition + new Vector2(_generationInterval * i, 0), Quaternion.identity);
            warningTop.GetComponent<WarningMove>().IsTransparent = true;
            warningUnder.GetComponent<WarningMove>().IsTransparent = true;
            warningTop.transform.SetParent(transform, false);
            warningUnder.transform.SetParent(transform, false);
            warningTop.GetComponent<WarningMove>().StartCoroutine(warningTop.GetComponent<WarningMove>().FadeInCoroutine());
            warningUnder.GetComponent<WarningMove>().StartCoroutine(warningUnder.GetComponent<WarningMove>().FadeInCoroutine());

            warningUnder.GetComponent<WarningMove>().Direction = false;
        }
    }
    private IEnumerator WarningCoroutine()
    {
        yield return new WaitForSeconds(1.0f);
        _isNearingEnd = true;
    }
}
