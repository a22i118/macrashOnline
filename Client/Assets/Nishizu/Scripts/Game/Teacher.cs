using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Teacher : MonoBehaviour
{
    [SerializeField] private GameObject _objMakura;
    [SerializeField] private GameObject _teacherGuide;
    private bool _isGameStart = false;
    private Animator _animator;
    private TextMeshProUGUI _teacherComent;
    public bool IsGameStart { get => _isGameStart; set => _isGameStart = value; }

    private bool _isRotateOnce = false;
    private bool _isCanMove = false;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        transform.position = new Vector3(0.0f, 0.0f, 6.5f);
        _teacherComent = _teacherGuide.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isGameStart)
        {
            _animator.SetBool("Walk", true);
            if (!_isRotateOnce)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                _isRotateOnce = true;
            }

            if (_isRotateOnce && !_isCanMove)
            {
                Vector3 targetPosition = new Vector3(0.0f, 0.0f, 10.0f);
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);

                if (Vector3.Distance(transform.position, targetPosition) < 1.0f)
                {
                    _isCanMove = true;
                }
            }

            if (_isCanMove)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                _isGameStart = false;
                _animator.SetBool("Walk", false);
            }
        }
    }
    public void Angry(Transform targetTransform)
    {
        _animator.SetTrigger("Attack");
        StartCoroutine(AngryText());
        GameObject makura = Instantiate(_objMakura, transform.position + new Vector3(0.0f, 0.0f, -3.0f), Quaternion.identity);
        makura.GetComponent<TeaherMakuraController>().Target = targetTransform;
        makura.GetComponent<TeaherMakuraController>().TargetPlayer = targetTransform.gameObject;
    }
    private IEnumerator AngryText()
    {
        _teacherGuide.SetActive(true);
        _teacherComent.text = "何で起きているんだ!!";
        yield return new WaitForSeconds(3.0f);
        _teacherGuide.SetActive(false);
    }
}
