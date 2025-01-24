using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuUIManager : MonoBehaviour
{
    private enum UItype
    {
        VS,
        Practice,
        Config,
        Exit,
        Local,
        Internet,
        Null

    }

    Animator anim;
    UItype _currenttype;
    [SerializeField] private GameObject _makura;
    [SerializeField] private GameObject _VSmenu;
    [SerializeField] private GameObject _Firstmenu;
    [SerializeField] private GameObject _LocalVSmenu;
    [SerializeField] private GameObject _Configmenu;
    [SerializeField] private GameObject _Exitmenu;
    [SerializeField] private TextMeshProUGUI _uitext;
    LayerMask _uilayer;
    Rigidbody _rb;
    [SerializeField] float _throwforce;
    Vector3 _throwposition;
    private bool _isray = true;

    private void Start()
    {
        anim = GetComponent<Animator>();
        _throwposition = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - 1, Camera.main.transform.position.z);
        _uilayer = LayerMask.GetMask("UI");
    }

    private void Update()
    {
        if (_isray)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 30, _uilayer))
            {

                CurrentUI(hit.collider.gameObject.name);
                UIText();
                if (Input.GetMouseButtonDown(0))
                {
                    StartCoroutine(ThrowMakura(hit.collider.gameObject.transform.position));
                    _isray = false;
                    StartCoroutine(MoveMenu());

                }
            }
            else
            {
                CurrentUI("Null");
                UIText();
            }

        }



    }

    private void CurrentUI(string UIname)
    {
        switch (UIname)
        {
            case "VSSymbol":
                _currenttype = UItype.VS;
                break;

            case "TutorialSymbol":
                _currenttype = UItype.Practice;
                break;

            case "ConfigSymbol":
                _currenttype = UItype.Config;
                break;

            case "ExitSymbol":
                _currenttype = UItype.Exit;
                break;

            case "Local":
                _currenttype = UItype.Local;
                break;

            case "Internet":
                _currenttype = UItype.Internet;
                break;

            case "Null":
                _currenttype = UItype.Null;
                break;
        }
    }



    private IEnumerator ThrowMakura(Vector3 position)
    {
        Vector3 _throwdirection = position - _throwposition;
        _throwdirection.Normalize();

        GameObject _throwmakura = Instantiate(_makura, _throwposition, Quaternion.identity);
        _rb = _throwmakura.GetComponent<Rigidbody>();

        if (_rb != null)
        {
            _rb.AddForce(_throwdirection * _throwforce, ForceMode.VelocityChange);

        }

        yield return new WaitForSeconds(1.7f);
        Destroy(_throwmakura);
    }

    private IEnumerator MoveMenu()
    {
        yield return new WaitForSeconds(1.5f);
        switch (_currenttype)
        {
            case UItype.VS:
                _VSmenu.SetActive(true);
                _Firstmenu.SetActive(false);
                break;

            case UItype.Practice:
                break;

            case UItype.Config:
                _Configmenu.SetActive(true);
                _Firstmenu.SetActive(false);
                break;

            case UItype.Exit:
                _Exitmenu.SetActive(true);
                _Firstmenu.SetActive(false);
                break;
        }
        _isray = true;
    }

    private void UIText()
    {
        GameObject _uiPanel = _uitext.transform.parent.gameObject;

        switch (_currenttype)
        {

            case UItype.VS:
                _uiPanel.SetActive(true);
                _uitext.text = "対戦モード";
                break;

            case UItype.Practice:
                _uiPanel.SetActive(true);
                _uitext.text = "チュートリアルモード";
                break;

            case UItype.Config:
                _uiPanel.SetActive(true);
                _uitext.text = "設定画面";
                break;

            case UItype.Exit:
                _uiPanel.SetActive(true);
                _uitext.text = "ゲーム終了";
                break;

            case UItype.Null:
                _uiPanel.SetActive(false);
                _uitext.text = "";
                break;
        }
    }
    public void LocalVSMenu()
    {
        _LocalVSmenu.SetActive(true);
        _VSmenu.SetActive(false);
    }
}
