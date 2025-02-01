using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private GameObject _firstmenu;
    [SerializeField] private GameObject _config;
    [SerializeField] private GameObject _basicsetting;
    [SerializeField] private GameObject _keysetting;
    [SerializeField] private GameObject _exitmenu;
    public void ConfigChange(int i)
    {
        if (i == 0)
        {
            _basicsetting.SetActive(true);
            _keysetting.SetActive(false);
        }
        else if (i == 1)
        {
            _basicsetting.SetActive(false);
            _keysetting.SetActive(true);
        }
        else if (i == 2)
        {
            _config.SetActive(false);
            _firstmenu.SetActive(true);
        }

    }

    public void StartLocal()
    {
        // SceneManager.LoadScene("GameScene");
    }

    public void StartServer()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ExitGame(int i)
    {
        if (i == 0)
        {
            Application.Quit(); //ゲーム終了

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; //エディタ終了
#endif
        }
        else
        {
            _exitmenu.SetActive(false);
            _firstmenu.SetActive(true);
        }
    }

}
