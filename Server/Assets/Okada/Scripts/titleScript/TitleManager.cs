using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public FadeScene fadescene;
    [SerializeField] private GameObject ConfigPanel;

    public void StartButton()
    {
        fadescene.FadeToScene("MenuScene");
    }
}
