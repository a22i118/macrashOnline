using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMakuraController : ColorChanger
{
    // Start is called before the first frame update
    void Start()
    {
        ColorChange(_currentColorType);
    }

    // Update is called once per frame
    void Update()
    {
        ColorChange(_currentColorType);
    }
}
