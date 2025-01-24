using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
public class ScoreManager : MonoBehaviour
{
    private bool _isGameStart = false;
    private bool _isGameEnd = false;
    private bool _isPlayerSet = true;
    private Dictionary<int, int> _scoreNum = new Dictionary<int, int>();
    private List<TextMeshProUGUI> _scores = new List<TextMeshProUGUI>();
    private List<GameObject> _scoreCrown = new List<GameObject>();
    private List<GameObject> _playerTagCrown = new List<GameObject>();
    public List<TextMeshProUGUI> Scores { get => _scores; set => _scores = value; }
    public bool IsGameStart { get => _isGameStart; set => _isGameStart = value; }
    public bool IsGameEnd { get => _isGameEnd; set => _isGameEnd = value; }
    public Dictionary<int, int> ScoreNum { get => _scoreNum; set => _scoreNum = value; }
    public List<GameObject> ScoreCrown { get => _scoreCrown; set => _scoreCrown = value; }
    public List<GameObject> PlayerTagCrown { get => _playerTagCrown; set => _playerTagCrown = value; }

    void Start()
    {

    }
    void Update()
    {
        if (_isGameStart)
        {
            if (_isPlayerSet)
            {
                InitializeScore();
                _isPlayerSet = false;
            }
            for (int i = 0; i < _scores.Count; i++)
            {
                if (IsValueMax(i))
                {
                    _scoreCrown[i].SetActive(true);
                    _playerTagCrown[i].SetActive(true);
                }
                else
                {
                    _scoreCrown[i].SetActive(false);
                    _playerTagCrown[i].SetActive(false);
                }
            }
        }
    }
    private void InitializeScore()
    {
        for (int i = 0; i < _scores.Count; i++)
        {
            _scoreNum.Add(i, 0);
            _scores[i].text = _scoreNum[i].ToString();
        }
    }
    public void UpdateScore(string player, string hitPlayer, bool isSleep)
    {
        int playerIndex = -1;
        int hitPlayerIndex = -1;
        switch (player)
        {
            case "Player (1)":
                playerIndex = 0;
                break;
            case "Player (2)":
                playerIndex = 1;
                break;
            case "Player (3)":
                playerIndex = 2;
                break;
            case "Player (4)":
                playerIndex = 3;
                break;
        }
        switch (hitPlayer)
        {
            case "Player (1)":
                hitPlayerIndex = 0;
                break;
            case "Player (2)":
                hitPlayerIndex = 1;
                break;
            case "Player (3)":
                hitPlayerIndex = 2;
                break;
            case "Player (4)":
                hitPlayerIndex = 3;
                break;
        }
        if (playerIndex >= 0 && playerIndex < _scoreNum.Count)
        {
            if (_scoreNum.ContainsKey(playerIndex))
            {
                if (IsValueMax(hitPlayerIndex))
                {
                    _scoreNum[playerIndex] += 2;
                }
                else
                {
                    _scoreNum[playerIndex]++;
                }
                if (isSleep)
                {
                    _scoreNum[playerIndex]++;
                }
                _scores[playerIndex].text = _scoreNum[playerIndex].ToString();
            }
        }
    }
    private bool IsValueMax(int key)
    {
        if (_scoreNum.ContainsKey(key))
        {
            int value = _scoreNum[key];
            int maxValue = _scoreNum.Values.Max();
            if (maxValue == 0)
            {
                return false;
            }
            return value == maxValue;
        }

        return false;
    }
}
