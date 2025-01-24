using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    protected Renderer _renderer;
    protected Renderer _blurRenderer;
    protected Material _blurMaterial;
    protected ColorType _currentColorType = ColorType.Nomal;

    public ColorType CurrentColorType
    {
        get => _currentColorType;
        set
        {
            _currentColorType = value;
            ColorChange(_currentColorType);
        }
    }
    protected virtual void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (transform.childCount > 0)
        {
            _blurRenderer = transform.GetChild(0).GetComponent<Renderer>();
            _blurMaterial = _blurRenderer.material;
        }
        ColorChange(_currentColorType);
    }
    /// <summary>
    /// 色を変える
    /// </summary>
    /// <param name="type">に応じて色を変える</param>
    protected void ColorChange(ColorType type)
    {
        if (_renderer != null)
        {
            if (type == ColorType.Nomal)
            {
                _renderer.material.color = Color.white;
                if (transform.childCount > 0)
                {
                    _blurRenderer.material.color = new Color(1.0f, 1.0f, 1.0f, 0.01f);
                }
            }
            else if (type == ColorType.Red)
            {
                _renderer.material.color = Color.red;
                if (transform.childCount > 0)
                {
                    _blurRenderer.material.color = new Color(1.0f, 0.0f, 0.0f, 0.01f);
                }
            }
            else if (type == ColorType.Green)
            {
                _renderer.material.color = Color.green;
                if (transform.childCount > 0)
                {
                    _blurRenderer.material.color = new Color(0.0f, 1f, 0.0f, 0.01f);
                }
            }
            else if (type == ColorType.Blue)
            {
                _renderer.material.color = Color.blue;
                if (transform.childCount > 0)
                {
                    _blurRenderer.material.color = new Color(0.0f, 0.0f, 1.0f, 0.01f);
                }
            }
            else if (type == ColorType.Black)
            {
                _renderer.material.color = Color.black;
                if (transform.childCount > 0)
                {
                    _blurRenderer.material.color = new Color(0.0f, 0.0f, 0.0f, 0.01f);
                }
                // _renderer.material.color.a = 0.5f;これできない！！
                // Color color = _renderer.material.color;
                // color.a = 0f;//透明度
                // _renderer.material.color = color;
            }
        }
    }

    public enum ColorType
    {
        Nomal,
        Red,
        Blue,
        Green,
        Black
    }
}
