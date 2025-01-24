using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultCameraController : MonoBehaviour
{
    [SerializeField] private Camera _resultCamera;
    private bool _isGameEnd = false;
    private bool _isCameraSet = true;
    private bool _isCanMove = false;
    private bool _isUISet = false;
    private float _targetPosition = 2.2f;
    private float _targetRotation = 180f;
    private float _transitionTime = 2.0f;
    private float _elapsedTime = 0.0f;

    private bool _isMoving = false;
    public bool IsGameEnd { get => _isGameEnd; set => _isGameEnd = value; }
    public bool IsUISet { get => _isUISet; set => _isUISet = value; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        if (_isGameEnd)
        {
            if (_isCameraSet)
            {
                _isCameraSet = false;
                StartCoroutine(CameraSetDerey());
            }
            if (_isCanMove && !_isMoving)
            {
                _isMoving = true;
                StartCoroutine(MoveAndRotateCamera());
            }
            if (_resultCamera.transform.position.y == 2.2f)
            {
                StartCoroutine(UISetDerey());
            }
        }
    }
    private IEnumerator MoveAndRotateCamera()
    {
        Vector3 startPosition = _resultCamera.transform.position;
        Quaternion startRotation = _resultCamera.transform.rotation;

        float startPosition_Y = startPosition.y;
        float startRotation_Y = startRotation.eulerAngles.y;

        while (_elapsedTime < _transitionTime)
        {
            _elapsedTime += Time.deltaTime;

            float time = Mathf.Clamp01(_elapsedTime / _transitionTime);
            float current_Y = Mathf.Lerp(startPosition_Y, _targetPosition, time);
            _resultCamera.transform.position = new Vector3(startPosition.x, current_Y, startPosition.z);

            float currentRotation_Y = Mathf.LerpAngle(startRotation_Y, -_targetRotation, time);
            _resultCamera.transform.rotation = Quaternion.Euler(90f, currentRotation_Y, 0.0f);

            yield return null;
        }

        _resultCamera.transform.position = new Vector3(startPosition.x, _targetPosition, startPosition.z);
        _resultCamera.transform.rotation = Quaternion.Euler(90.0f, _targetRotation, 0.0f);

        _elapsedTime = 0.0f;
        _isMoving = false;
    }
    private IEnumerator CameraSetDerey()
    {
        yield return new WaitForSeconds(0.5f);
        _isCanMove = true;
    }
    private IEnumerator UISetDerey()
    {
        yield return new WaitForSeconds(1.0f);
        _isUISet = true;
    }
}
