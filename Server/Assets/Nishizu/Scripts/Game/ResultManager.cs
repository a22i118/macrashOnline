using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using PlayerCS;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private bool _isGameEnd = false;
    [SerializeField] private GameObject _resultGuide;
    [SerializeField] private List<GameObject> _resultHutons = new List<GameObject>();
    [SerializeField] private List<GameObject> _resultScores = new List<GameObject>();
    private bool _isSceneSwith = false;
    private ResultCameraController _resultCameraController;
    private List<ResultHutonController> _resultHutonControllers = new List<ResultHutonController>();
    private List<PlayerController> _playerControllers = new List<PlayerController>();
    private Dictionary<int, int> _scoreDic = new Dictionary<int, int>();
    public bool IsGameEnd { get => _isGameEnd; set => _isGameEnd = value; }
    public List<PlayerController> PlayerControllers { get => _playerControllers; set => _playerControllers = value; }
    public List<ResultHutonController> ResultHutonControllers { get => _resultHutonControllers; set => _resultHutonControllers = value; }
    public Dictionary<int, int> ScoreDic { get => _scoreDic; set => _scoreDic = value; }

    // Start is called before the first frame update
    void Start()
    {
        _resultGuide.SetActive(false);
        foreach (var resultHuton in _resultHutons)
        {
            _resultHutonControllers.Add(resultHuton.GetComponent<ResultHutonController>());
        }
        foreach (var score in _resultScores)
        {
            score.SetActive(false);
        }
        _resultCameraController = this.gameObject.GetComponent<ResultCameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isGameEnd)
        {
            _resultCameraController.IsGameEnd = true;
        }
        if (_resultCameraController.IsUISet)
        {
            for (int i = 0; i < _scoreDic.Count; i++)
            {
                _resultScores[i].SetActive(true);

                TextMeshProUGUI text = _resultScores[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                text.text = _scoreDic[i].ToString();
            }
            StartCoroutine(ScenesSwitch());
            _resultCameraController.IsUISet = false;
        }
        if (_isSceneSwith)
        {
            if (_playerControllers.Count != 0)
            {
                foreach (var playerController in _playerControllers)
                {
                    playerController.IsResultEnd = true;
                    if (playerController.IsGameEndCheck)
                    {
                        SceneManager.LoadScene("MenuScene");
                        // Debug.Log("切り替わるはずや");
                    }
                }
            }

        }
    }
    private IEnumerator ScenesSwitch()
    {
        yield return new WaitForSeconds(3.0f);
        _isSceneSwith = true;
        yield return new WaitForSeconds(2.0f);
        _resultGuide.SetActive(true);
    }
}
