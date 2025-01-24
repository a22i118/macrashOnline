using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
namespace PlayerCS
{
    public class PlayerStatus : MonoBehaviour
    {
        private bool _IsChargeMax = false;
        private bool _isGameStart = false;
        private bool _isPlayerSet = true;
        private const int _maxSP = 100000;
        private int _currentSP = 0;
        private Slider _spBar;
        private Image _fillImage;
        private Color _normalColor;
        private Color _maxColor = Color.red;
        public bool IsChargeMax { get => _IsChargeMax; set => _IsChargeMax = value; }
        public bool IsGameStart { get => _isGameStart; set => _isGameStart = value; }
        public int CurrentSP { get => _currentSP; set => _currentSP = value; }
        public Slider SpBar { get => _spBar; set => _spBar = value; }

        private void Start()
        {

        }

        private void Update()
        {
            if (_isGameStart)
            {
                if (_isPlayerSet)
                {
                    _currentSP = 0;
                    _spBar.maxValue = _maxSP;
                    _spBar.value = _currentSP;

                    _fillImage = _spBar.fillRect.GetComponent<Image>();
                    _normalColor = _fillImage.color;
                    _isPlayerSet = false;
                }
                if (_currentSP < _maxSP)
                {
                    _IsChargeMax = false;
                    _currentSP += 20;
                    _currentSP = Mathf.Min(_currentSP, _maxSP);
                    _spBar.value = _currentSP;

                    ChangeSPBarColor();
                }
                else
                {
                    _IsChargeMax = true;
                }
            }

        }

        public void SpUp()
        {
            if (_currentSP < _maxSP)
            {
                _currentSP += 10000;
                _currentSP = Mathf.Min(_currentSP, _maxSP);
                _spBar.value = _currentSP;

                ChangeSPBarColor();
            }
        }

        private void ChangeSPBarColor()
        {
            if (_fillImage != null)
            {
                _fillImage.color = _currentSP >= _maxSP ? _maxColor : _normalColor;
            }
        }
    }

}
