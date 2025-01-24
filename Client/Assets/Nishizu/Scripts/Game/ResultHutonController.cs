using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultHutonController : MonoBehaviour
{
    private int _rank = -1;
    private Color _defaultColor;
    private Renderer _childRenderer;
    public int Rank { get => _rank; set => _rank = value; }

    // Start is called before the first frame update
    void Start()
    {
        Transform firstChild = transform.GetChild(0);

        _childRenderer = firstChild.GetComponent<Renderer>();
        _defaultColor = _childRenderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        ColorChange();
    }
    private void ColorChange()
    {
        if (_childRenderer != null)
        {
            Color color = _defaultColor;
            Material material = new Material(Shader.Find("Standard"));

            if (_rank == 0)
            {
                color = new Color(1.0f, 0.843f, 0.0f);
                material.SetColor("_Color", color);
                material.SetFloat("_Metallic", 1f);
                material.SetFloat("_Smoothness", 0.9f);
                _childRenderer.material = material;
            }
            else if (_rank == 1)
            {
                color = new Color(0.75f, 0.75f, 0.75f);
                material.SetColor("_Color", color);
                material.SetFloat("_Metallic", 1f);
                material.SetFloat("_Smoothness", 0.9f);
                _childRenderer.material = material;
            }
            else if (_rank == 2)
            {
                color = new Color(0.72f, 0.45f, 0.2f);
                material.SetColor("_Color", color);
                material.SetFloat("_Metallic", 1f);
                material.SetFloat("_Smoothness", 0.9f);
                _childRenderer.material = material;
            }
            _childRenderer.material.color = color;
            // Color(1.0f, 0.843f, 0.0f)//金
            // Color(0.75f, 0.75f, 0.75f)//銀
            // Color(0.72f, 0.45f, 0.2f)//銅
        }
    }
    public Vector3 GetCenterPosition()
    {
        return GetComponent<Collider>().bounds.center;
    }
    public Quaternion GetRotation()
    {
        return transform.rotation;
    }

}
