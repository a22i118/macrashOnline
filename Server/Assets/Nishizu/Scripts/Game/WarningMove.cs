using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WarningMove : MonoBehaviour
{
    private bool _isDirection = true;
    private bool _isTransparent = false;
    private float _speed = 500.0f;
    private float _destroyPosition_x = -1320.0f;
    private float _startAlpha;
    private RectTransform rectTransform;
    private Image _image;
    public bool Direction { get => _isDirection; set => _isDirection = value; }
    public bool IsTransparent { get => _isTransparent; set => _isTransparent = value; }

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _startAlpha = _image.color.a;
        if (_isTransparent)
        {
            TransparencyUpdate(0.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.anchoredPosition += (_isDirection ? Vector2.left : Vector2.right) * _speed * Time.deltaTime;
        if (_isDirection)
        {
            if (rectTransform.anchoredPosition.x <= _destroyPosition_x)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (rectTransform.anchoredPosition.x >= -_destroyPosition_x)
            {
                Destroy(gameObject);
            }
        }
        if (_isTransparent)
        {
            TransparencyUpdate(0.0f);
        }
        else
        {
            TransparencyUpdate(_startAlpha);
        }
    }

    private void TransparencyUpdate(float alpha)
    {
        if (_image != null)
        {
            Color color = _image.color;
            color.a = alpha;//透明度
            _image.color = color;
        }
    }

    public IEnumerator FadeOutCoroutine()
    {
        yield return new WaitForSeconds(4.0f);
        float changeTime = 1.5f;
        float timeElapsed = 0.0f;

        while (timeElapsed < changeTime)
        {
            float alpha = Mathf.Lerp(_startAlpha, 0.0f, timeElapsed / changeTime);//透明度を滑らかに更新
            TransparencyUpdate(alpha);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        _isTransparent = true;
        TransparencyUpdate(0.0f);
    }
    public IEnumerator FadeInCoroutine()
    {
        float changeTime = 1.0f;
        float timeElapsed = 0.0f;

        while (timeElapsed < changeTime)
        {
            float alpha = Mathf.Lerp(0.0f, _startAlpha, timeElapsed / changeTime);//透明度を滑らかに更新
            TransparencyUpdate(alpha);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        _isTransparent = false;
        TransparencyUpdate(_startAlpha);
    }
}
