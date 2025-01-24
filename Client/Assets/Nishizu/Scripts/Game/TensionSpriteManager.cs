using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TensionSpriteManager : MonoBehaviour
{
    private bool _isHeartBeat = false;
    private bool _isLoop = false;
    private float _startAlpha;
    private float _changeDuration = 0.1f;
    private float[] _alphaValues = { 0.0f, 1.0f, 0.6f, 1.0f, 0.0f };
    private Image _image;
    public bool IsLoop { get => _isLoop; set => _isLoop = value; }

    // Start is called before the first frame update
    void Start()
    {
        _image = GetComponent<Image>();
        _startAlpha = _image.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isHeartBeat)
        {
            _isHeartBeat = false;
            StartCoroutine(ChangeAlpha());
        }
    }
    private IEnumerator ChangeAlpha()
    {
        for (int i = 0; i < _alphaValues.Length; i++)
        {
            float targetAlpha = _alphaValues[i];

            float elapsedTime = 0f;
            _startAlpha = _image.color.a;

            while (elapsedTime < _changeDuration)
            {
                float alpha = Mathf.Lerp(_startAlpha, targetAlpha, elapsedTime / _changeDuration);
                Color color = _image.color;
                color.a = alpha;
                _image.color = color;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, targetAlpha);

            yield return new WaitForSeconds(_changeDuration);
        }
        yield return new WaitForSeconds(1.0f);
        if (_isLoop)
        {
            StartCoroutine(ChangeAlpha());
        }

    }
    public void Init()
    {
        _isLoop = true;
        _isHeartBeat = true;
    }
}
