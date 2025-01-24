using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HappeningBall : MonoBehaviour
{
    private bool _isOutbreak = false;
    private float hue;
    private Renderer _renderer;
    private GameObject _starter = null;
    public bool Outbreak { get => _isOutbreak; set => _isOutbreak = value; }
    public GameObject Starter { get => _starter; set => _starter = value; }

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        ColorChange();
        if (_starter != null)
        {
            _isOutbreak = true;
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// 虹色に光らせる
    /// </summary>
    private void ColorChange()
    {
        hue = Mathf.Repeat(Time.time, 0.5f);

        Color color = Color.HSVToRGB(hue, 1.0f, 1.0f);
        color.a = 0.8f;

        _renderer.material.color = color;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _starter = collision.gameObject;
        }
        else if (collision.gameObject.CompareTag("Makura"))
        {
            _starter = collision.gameObject.GetComponent<MakuraController>().Thrower;
        }
    }
}
