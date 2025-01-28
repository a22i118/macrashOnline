using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Net.Security;
using PlayerCS;
namespace PlayerCS
{
    public class Player
    {
        // 表示モデルの種類
        public enum eKind
        {
            take_Idol,
            okada_Idol,
        }
    }
    public partial class PlayerController : MonoBehaviour
    {
        [SerializeField] private LayerMask _groundLayers;
        [SerializeField] private LayerMask _hutonLayer;
        [SerializeField] private LayerMask _wallLayer;
        [SerializeField] private GameObject _showMakura;
        [SerializeField] private GameObject _alterEgoMakura;
        [SerializeField] private GameObject _playerTagUI;
        [SerializeField] private GameObject _spGageUI;

        private bool _isSleep = false;//寝ているか
        private bool _isCanSleep = false;
        private bool _isHitStop = false;//止まっているか
        private bool _isJumping = false;//ジャンプ中か
        private bool _isThrowChargeTime = false;//ため攻撃中か
        private bool _isSleepChargeTime = false;
        private bool _isThrowCoolTime = false;
        private bool _isCanCatch = false;//ジャストキャッチ可能か
        private bool _isVibrating = false;
        private bool _isHitCoolTime = false;
        private bool _isCounterAttackTime = false;
        private bool _isThrowKeyPushed = false;
        private bool _isPickUpKeyPushed = false;
        private bool _isGameStart = false;
        private bool _isGameStartCheck = false;
        private bool _isGameEnd = false;
        private bool _isResultEnd = false;
        private bool _isGameEndCheck = false;
        private bool _isSpeedUp = false;
        private bool _isTeacherMakuraHit = false;
        private int _playerIndex;
        private const float C_gravity = -25.0f;
        private float _speed = 5.0f;//プレイヤーの移動速度
        private float _groundCheckRadius = 0.01f;//足元が地面か判定する球の半径
        private float _pickUpDistance = 1.0f;//まくらを拾うことができる距離
        private float _playerSerchDistance = 5.0f;//敵プレイヤーの捜索範囲
        private float _throwKeyHoldTime;//長押ししている時間
        private float _pickUpKeyHoldTime;
        private float _throwKeyLongPressTime = 0.5f;//ため攻撃にかかる時間
        private float _rotationSpeed = 200.0f;//持っているまくらの回転速度
        private float _showRadius = 0.6f;//プレイヤーからのまくらの距離
        private float _rotationAngle;
        private float _vibrationStrength = 0.3f;//振動の強さ
        private float _vibrationTime = 0.1f;//振動する時間
        private float _jumpHoldTime = 0f;//ジャンプキーが押されている時間
        private float _maxJumpHoldTime = 0.2f;//最大ジャンプの押す時間
        private float _minJumpForce = 6.5f;//最小ジャンプ力
        private float _maxJumpForce = 9.0f;//最大ジャンプ力
        private GameObject _currentMakuraDisplay;
        private GameObject[] _currentMakuraDisplays = new GameObject[2];
        private GameObject _playerTagUIInstance;
        private Transform _playerTransform;
        private Rigidbody _rb;
        private Animator _animator;
        private CapsuleCollider _col;
        private List<GameObject> _currentMakuras = new List<GameObject>();
        private GameObject _thrownMakura;//投げられたまくら
        private Vector3 _targetPosition;//敵プレイヤーの位置
        private Quaternion _beforeSleepRotation;//布団で寝る前の向き
        private Quaternion _lastDirection;//移動入力の最後に向いている向き
        private HutonController _currentHutonController;//布団のスクリプト
        private Transform _currentHuton;
        private MakuraController _makuraController;//まくらのスクリプト
        private PlayerStatus _playerStatus;//プレイヤーのスクリプト
        private List<ShowMakuraController> _showMakuraControllers = new List<ShowMakuraController>();
        private Transform _huton;
        private Vector3 _movement;
        private Vector3 _nomalColliderCenter;
        private Vector3 _sleepColliderCenter;
        private GameObject _spGageInstance;
        public bool IsHitCoolTime { get => _isHitCoolTime; set => _isHitCoolTime = value; }
        public bool IsCanSleep { get => _isCanSleep; set => _isCanSleep = value; }
        public bool IsSleep { get => _isSleep; }
        public bool IsGameStart { get => _isGameStart; set => _isGameStart = value; }
        public bool IsGameStartCheck { get => _isGameStartCheck; }
        public bool IsGameEnd { get => _isGameEnd; set => _isGameEnd = value; }
        public bool IsResultEnd { get => _isResultEnd; set => _isResultEnd = value; }
        public bool IsGameEndCheck { get => _isGameEndCheck; set => _isGameEndCheck = value; }
        public int PlayerIndex { get => _playerIndex; set => _playerIndex = value; }
        public GameObject CurrentMakuraDisplay { get => _currentMakuraDisplay; set => _currentMakuraDisplay = value; }
        public GameObject SpGageInstance { get => _spGageInstance; set => _spGageInstance = value; }
        public GameObject PlayerTagUIInstance { get => _playerTagUIInstance; set => _playerTagUIInstance = value; }
        public GameObject[] CurrentMakuraDisplays { get => _currentMakuraDisplays; set => _currentMakuraDisplays = value; }



        // プレイヤーモデルの種類
        private Player.eKind _kind = Player.eKind.take_Idol;
        // public bool IsGround { get { return IsGround(); } }

        public Player.eKind Kind { get { return _kind; } set { _kind = value; } }

        public enum ThrowType
        {
            Nomal,
            Charge
        }
        void Awake()
        {
            // Init();
            _rb = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
            _col = GetComponent<CapsuleCollider>();
            _groundLayers |= _hutonLayer;
            _groundLayers |= _wallLayer;
        }

        public void OnGameStartCheck(InputValue value)
        {
            if (value.isPressed)
            {
                _isGameStartCheck = true;
            }
            else
            {
                _isGameStartCheck = false;
            }
        }
        public void OnResultEnd(InputValue value)
        {
            if (_isResultEnd && value.isPressed)
            {
                _isGameEndCheck = true;
            }
            else
            {
                _isGameEndCheck = false;
            }
        }
        void Update()
        {
            if (_playerTagUIInstance != null)
            {
                _playerTagUIInstance.transform.position = _playerTransform.position + new Vector3(-1.375f, 1.5f, -0.3f);
            }
            if (!_isGameEnd)
            {
                IsCheckPlayer();
                MakuraDisplayColorChange();
                // if (IsGround())
                // {
                //     _animator.SetBool("OnGround", true);
                // }
                // else
                // {
                //     _animator.SetBool("OnGround", false);
                // }
                // if (_isSleep)
                // {
                //     Vector3 offset = _huton.position - transform.position;
                //     transform.position = new Vector3(transform.position.x, transform.position.y + offset.y, transform.position.z);
                //     _animator.SetBool("Sleep", true);
                // }
                // else
                // {
                //     _animator.SetBool("Sleep", false);
                // }

                // if (_currentMakuras.Count > 0 && !_isSleep)
                // {
                //     RotateShowMakura();
                // }
                // if (_currentMakuras.Count == 0 || _isSleep)
                // {
                //     _currentMakuraDisplays[0].SetActive(false);
                //     _currentMakuraDisplays[0].transform.GetChild(0).gameObject.SetActive(true);
                //     _currentMakuraDisplays[1].SetActive(false);
                // }
                // else if (_currentMakuras.Count == 1)
                // {
                //     _currentMakuraDisplays[0].SetActive(true);
                //     _currentMakuraDisplays[0].transform.GetChild(0).gameObject.SetActive(true);
                //     _currentMakuraDisplays[1].SetActive(false);
                // }
                // else if (_currentMakuras.Count == 2)
                // {
                //     _currentMakuraDisplays[0].SetActive(true);
                //     _currentMakuraDisplays[0].transform.GetChild(0).gameObject.SetActive(true);
                //     _currentMakuraDisplays[1].SetActive(true);
                //     _currentMakuraDisplays[1].transform.GetChild(0).gameObject.SetActive(false);
                // }
                if (IsHuton() || _currentMakuras.Count > 0 && _isThrowKeyPushed && _currentMakuras[0].GetComponent<MakuraController>().CurrentColorType == ColorChanger.ColorType.Nomal)
                {
                    if (_isSpeedUp)
                    {
                        _speed = 4.0f;
                    }
                    else
                    {
                        _speed = 2.0f;
                    }
                }
                else
                {
                    if (_isSpeedUp)
                    {
                        _speed = 6.0f;
                    }
                    else
                    {
                        _speed = 4.0f;
                    }
                }
                if (!_isHitStop && !_isVibrating && !_isTeacherMakuraHit)
                {
                    if (!_isSleep)
                    {
                        Move();
                        Jump();
                        if (_isGameStart)
                        {
                            MakuraThrow();
                        }
                    }
                    Sleep_WakeUp();
                }
            }
        }
        private void Init()
        {

            _playerStatus = GetComponent<PlayerStatus>();
            _nomalColliderCenter = _col.center;
            _sleepColliderCenter = _nomalColliderCenter + new Vector3(0, -0.2f, 1);
            if (_showMakura != null)
            {
                _currentMakuraDisplays[0] = Instantiate(_showMakura);
                _currentMakuraDisplays[1] = Instantiate(_showMakura);
                foreach (var currentmakuraDisplay in _currentMakuraDisplays)
                {
                    _showMakuraControllers.Add(currentmakuraDisplay.GetComponent<ShowMakuraController>());
                }
            }

            _rb.useGravity = false;

            Canvas[] canvases = FindObjectsOfType<Canvas>();
            Color color;
            if (_playerIndex == 0)
            {
                color = Color.red;
            }
            else if (_playerIndex == 1)
            {
                color = Color.blue;
            }
            else if (_playerIndex == 2)
            {
                color = Color.yellow;
            }
            else
            {
                color = Color.green;
            }
            color.a = 0.5f;
            foreach (var canvas in canvases)
            {
                if (canvas.renderMode == RenderMode.WorldSpace)
                {
                    _playerTransform = transform;
                    _playerTagUIInstance = Instantiate(_playerTagUI, _playerTransform.position, Quaternion.identity);
                    TextMeshProUGUI text = _playerTagUIInstance.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                    text.text = _playerIndex + 1 + " P";

                    _playerTagUIInstance.transform.GetChild(0).GetComponent<Image>().color = color;
                    _playerTagUIInstance.transform.SetParent(_playerTransform);

                    Vector3 directionToCamera = Camera.main.transform.position - _playerTagUIInstance.transform.position;
                    directionToCamera.y = 0;
                    _playerTagUIInstance.transform.rotation = Quaternion.LookRotation(directionToCamera);
                    _playerTagUIInstance.transform.Rotate(0, 180, 0);
                    _playerTagUIInstance.transform.SetParent(canvas.transform, false);

                }
                else
                {
                    _spGageInstance = Instantiate(_spGageUI, new Vector2(640.0f + 260.0f * _playerIndex, 140.0f), Quaternion.identity);
                    TextMeshProUGUI text = _spGageInstance.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                    text.text = _playerIndex + 1 + " P";

                    _spGageInstance.GetComponent<Image>().color = color;
                    _spGageInstance.transform.SetParent(canvas.transform, false);

                    Slider _slider = _spGageInstance.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<Slider>();
                    _playerStatus.SpBar = _slider;

                    ScoreManager scoreManager = canvas.transform.GetChild(1).GetComponent<ScoreManager>();
                    scoreManager.Scores.Add(_spGageInstance.transform.GetChild(1).GetComponent<TextMeshProUGUI>());
                    scoreManager.ScoreCrown.Add(_spGageInstance.transform.GetChild(3).gameObject);
                    scoreManager.PlayerTagCrown.Add(_playerTagUIInstance.transform.GetChild(2).gameObject);
                }
            }
        }

        // 位置と向きをリセットする
        public void Restart()
        {
            // ランダムに決定する
            float x = UnityEngine.Random.value * 10.0f - 5.0f;
            float y = 4;
            float z = UnityEngine.Random.value * 10.0f + 10.0f;

            // 表示モデルの種類によってz座標の符号を決める（向きも同様）
            if (_kind == 0)
            {
                z = -z;
            }

            transform.position = new Vector3(x, y, z);
            transform.rotation = Quaternion.Euler(0.0f, _kind == 0 ? 0.0f : 180.0f, 0.0f);

            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
        private void OnSpecialAttack(InputValue value)
        {
            if (value.isPressed)
            {
                if (_currentMakuras.Count > 0 && !_isSleep && _playerStatus.IsChargeMax)
                // if (_currentMakuras.Count > 0 && !_isSleep)//デバッグ用
                {
                    _playerStatus.CurrentSP = 0;
                    _currentMakuras[0].GetComponent<MakuraController>().CurrentScaleType = MakuraController.ScaleType.Second;
                }
            }
        }
        private void Sleep_WakeUp()
        {
            if (_isPickUpKeyPushed && IsHuton())
            {
                if (!_isSleepChargeTime)
                {
                    _pickUpKeyHoldTime = Time.time;
                    _isSleepChargeTime = true;
                }

                float holdTime = Time.time - _pickUpKeyHoldTime;

                if (holdTime > 1.0f)
                {
                    if (!_isSleep && !_isHitStop && _currentMakuras.Count > 0 && _isCanSleep || !_isGameStart && !_isSleep && _currentMakuras.Count > 0)
                    {
                        transform.SetParent(_currentHuton);
                        SleepOnHuton();
                    }
                    _isSleepChargeTime = false;
                }
            }
            else
            {
                _isSleepChargeTime = false;
            }
        }

        private void OnThrow(InputValue value)
        {
            if (!_isSleep && !_isHitStop)
            {
                if (value.isPressed)
                {
                    _isThrowKeyPushed = true;
                }
                else
                {
                    _isThrowKeyPushed = false;
                }
            }
        }
        private void MakuraThrow()
        {
            if (!_isThrowCoolTime)
            {
                if (_currentMakuras.Count > 0)
                {
                    if (_isThrowKeyPushed)
                    {
                        if (!_isThrowChargeTime)
                        {
                            _throwKeyHoldTime = Time.time;
                            _isThrowChargeTime = true;
                        }
                    }
                    else if (_isThrowChargeTime && !_isThrowKeyPushed)
                    {
                        float holdTime = Time.time - _throwKeyHoldTime;

                        if (holdTime < _throwKeyLongPressTime)
                        {
                            ThrowMakura(ThrowType.Nomal);
                            _animator.SetTrigger("Throw");
                        }
                        else
                        {
                            ThrowMakura(ThrowType.Charge);
                            _animator.SetTrigger("Throw");
                        }

                        _isThrowChargeTime = false;
                    }
                }
                else
                {
                    if (_isThrowKeyPushed)
                    {
                        _animator.SetTrigger("Throw");
                        StartCoroutine(ThrowCoolTimeCorountine());
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            Vector3 gravityForce = new Vector3(0, C_gravity, 0);
            // _rb.AddForce(gravityForce, ForceMode.Acceleration);
        }

        private void OnMove(InputValue value)
        {
            Vector2 movementInput = value.Get<Vector2>();

            _movement = new Vector3(movementInput.x, 0, movementInput.y);
        }
        private void Move()
        {
            if (!_rb.isKinematic)
            {
                _rb.velocity = new Vector3(_movement.x * _speed, _rb.velocity.y, _movement.z * _speed);

                if (_movement.magnitude > 0.1f)
                {
                    // if (_isSpeedUp)
                    // {
                    //     _animator.SetBool("Run", true);
                    //     _animator.SetBool("Walk", false);
                    // }
                    // else
                    // {
                    //     _animator.SetBool("Walk", true);
                    //     _animator.SetBool("Run", false);
                    // }
                    transform.rotation = Quaternion.LookRotation(_movement);
                    _lastDirection = transform.rotation;
                }
                else
                {
                    // if (_isSpeedUp)
                    // {
                    //     _animator.SetBool("Run", false);
                    // }
                    // else
                    // {
                    //     _animator.SetBool("Walk", false);
                    // }
                    transform.rotation = _lastDirection;
                }
            }
        }

        /// <summary>
        /// 足元がGroundかどうか
        /// </summary>
        /// <returns>足元がGroundならtrueを返す</returns>
        public bool IsGround()
        {
            Vector3 groundCheckPosition = new Vector3(_col.bounds.center.x, _col.bounds.min.y, _col.bounds.center.z);
            return Physics.CheckSphere(groundCheckPosition, _groundCheckRadius, _groundLayers, QueryTriggerInteraction.Collide);
        }
        /// <summary>
        /// 足元がHutonかどうか
        /// </summary>
        /// <returns>足元がHutonならtrueを返す</returns>
        private bool IsHuton()
        {
            Vector3 groundCheckPosition = new Vector3(_col.bounds.center.x, _col.bounds.min.y, _col.bounds.center.z);
            return Physics.CheckSphere(groundCheckPosition, _groundCheckRadius, _hutonLayer);
        }
        private void OnJump(InputValue value)
        {
            if (value.isPressed)
            {
                if (!_isHitStop && !_isVibrating)
                {
                    if (IsGround())
                    {
                        _jumpHoldTime = 0.0f;
                        _isJumping = true;
                        _animator.SetTrigger("Jump");
                    }
                }
            }
            else
            {
                if (_isJumping)
                {
                    _isJumping = false;
                }
            }
        }

        private void Jump()
        {
            if (_isJumping)
            {
                if (_jumpHoldTime < _maxJumpHoldTime)
                {
                    _jumpHoldTime += Time.deltaTime;
                }
                else
                {
                    _jumpHoldTime = _maxJumpHoldTime;
                    _isJumping = false;
                }
                JumpForce();
            }
        }

        private void JumpForce()
        {
            if (_isJumping)
            {
                float jumpForce = Mathf.Lerp(_minJumpForce, _maxJumpForce, _jumpHoldTime / _maxJumpHoldTime);
                _rb.velocity = new Vector3(_rb.velocity.x, jumpForce, _rb.velocity.z);
            }
        }
        private void RotateShowMakura()
        {
            _rotationAngle += _rotationSpeed * Time.deltaTime;

            Vector3 offset1 = new Vector3(Mathf.Cos(_rotationAngle * Mathf.Deg2Rad) * _showRadius, 1.0f, Mathf.Sin(_rotationAngle * Mathf.Deg2Rad) * _showRadius);
            _currentMakuraDisplays[0].transform.position = transform.position + offset1;

            Vector3 offset2 = new Vector3(Mathf.Cos((_rotationAngle + 180f) * Mathf.Deg2Rad) * _showRadius, 1.0f, Mathf.Sin((_rotationAngle + 180f) * Mathf.Deg2Rad) * _showRadius);
            _currentMakuraDisplays[1].transform.position = transform.position + offset2;

            _currentMakuraDisplays[0].transform.LookAt(transform.position);

            _currentMakuraDisplays[1].transform.LookAt(transform.position);
        }
        private void MakuraDisplayColorChange()
        {
            if (_currentMakuras.Count > 0)
            {
                MakuraController[] makuraControllers = new MakuraController[2];
                makuraControllers[0] = _currentMakuras[0].GetComponent<MakuraController>();
                if (_currentMakuras.Count == 2)
                {
                    makuraControllers[1] = _currentMakuras[1].GetComponent<MakuraController>();
                }
                for (int i = 0; i < ((_currentMakuras.Count == 2) ? 2 : 1); i++)
                {
                    _showMakuraControllers[i].CurrentColorType = makuraControllers[i].CurrentColorType;
                }
            }
        }
        /// <summary>
        /// 近くに枕があることを返す
        /// </summary>
        /// <returns>近くに枕があればtrueを返す</returns>
        private bool CheckMakura()
        {
            Vector3 playerPosition = transform.position;
            Collider[] hitColliders = Physics.OverlapSphere(playerPosition, _pickUpDistance);
            foreach (var makura in hitColliders)
            {
                if (makura.CompareTag("Makura"))
                {
                    // Debug.Log("近くに枕があるぜ！");
                    return true;
                }
            }
            return false;
        }

        private void OnPickUp_Catch_WakeUp(InputValue value)
        {
            if (value.isPressed)
            {
                _isPickUpKeyPushed = true;
                if (!_isSleep && !_isHitStop && _currentMakuras.Count < 2)
                {
                    if (_thrownMakura != null && _isCanCatch)
                    {
                        CatchMakura();
                    }
                    else if (CheckMakura())
                    {
                        PickUpMakura();
                    }
                }
                if (_isSleep && _isCanSleep)
                {
                    WakeUpFromHuton();
                    transform.SetParent(null);
                }
            }
            else
            {
                _isPickUpKeyPushed = false;
            }
        }
        /// <summary>
        /// 近くの枕を拾う
        /// </summary>
        private void PickUpMakura()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, _pickUpDistance);
            foreach (var collider in hitColliders)
            {
                if (collider.CompareTag("Makura") && !collider.GetComponent<MakuraController>().IsThrow)
                {
                    GameObject currentMakura;

                    currentMakura = collider.gameObject;
                    currentMakura.transform.SetParent(null);
                    currentMakura.SetActive(false);

                    _currentMakuras.Add(currentMakura);
                    break;
                }
            }
        }
        private bool IsWallThrowCheck()
        {
            Vector3 throwDirection = transform.forward;
            if (_currentMakuras.Count > 0)
            {
                _makuraController = _currentMakuras[0].GetComponent<MakuraController>();

                float throwDistance = _makuraController.CurrentScaleType == MakuraController.ScaleType.Second ? 5.0f : 1.5f;

                return Physics.Raycast(transform.position, throwDirection, throwDistance, _wallLayer);
            }
            return true;
        }
        /// <summary>
        /// まくらをジャストキャッチする
        /// </summary>
        private void CatchMakura()
        {
            if (_thrownMakura != null)
            {
                _currentMakuras.Insert(0, _thrownMakura);
                _thrownMakura.SetActive(false);
                _thrownMakura = null;
                _isCanCatch = false;
                _isHitCoolTime = true;
                _playerStatus.CurrentSP += 5000;
                StartCoroutine(CounterAttackCoroutine());
                StartCoroutine(JustChachMakuraInvincibilityTime());
                // Debug.Log("枕をキャッチした！");
            }
        }
        /// <summary>
        /// ジャストキャッチ後0.3秒無敵
        /// </summary>
        /// <returns>0.3秒後解除</returns>
        private IEnumerator JustChachMakuraInvincibilityTime()
        {
            yield return new WaitForSeconds(0.3f);
            _isHitCoolTime = false;
        }
        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Makura"))
            {
                MakuraController makuraController = collider.GetComponent<MakuraController>();
                if (makuraController != null)
                {
                    if (makuraController.IsThrow && makuraController.Thrower != gameObject && makuraController.CurrentScaleType == MakuraController.ScaleType.Nomal && !makuraController.IsAlterEgo)
                    {
                        _isCanCatch = true;
                        _thrownMakura = collider.gameObject;
                        // Debug.Log("情報を記憶");
                    }
                }
            }
            if (collider.CompareTag("Explosion"))
            {
                ExplosionRange explosionRangeScript = collider.GetComponent<ExplosionRange>();
                if (explosionRangeScript.Thrower != gameObject)
                {
                    StartCoroutine(HitCoolTimeDelay());
                    _animator.SetBool("Walk", false);
                    // Debug.Log("う、動けない！");
                    HitMotion(false);
                    if (!_isVibrating)
                    {
                        StartCoroutine(HitStopVibration(true, 0.0f, null));
                    }
                }
            }
            if (collider.CompareTag("TeaherMakura"))
            {
                if (collider.gameObject.GetComponent<TeaherMakuraController>().TargetPlayer == gameObject)
                {
                    _animator.SetBool("Walk", false);
                    HitMotion(true);
                    if (!_isVibrating)
                    {
                        StartCoroutine(TeacherMakuraHit());
                    }
                }
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (collider.CompareTag("Makura"))
            {
                _isCanCatch = false;
                _thrownMakura = null;
            }
        }
        /// <summary>
        /// 布団の上で寝る
        /// </summary>
        private void SleepOnHuton()
        {
            _animator.SetBool("Walk", false);
            _animator.SetBool("Run", false);
            _animator.SetBool("Sleep", true);
            _rb.isKinematic = true;
            _isSleep = true;

            _col.center = _sleepColliderCenter;

            Vector3 hutonPosition = _currentHutonController.GetCenterPosition();
            transform.position = new Vector3(hutonPosition.x, hutonPosition.y, hutonPosition.z - 0.8f);
            _huton = _currentHutonController.transform;
            if (_currentHutonController != null)
            {
                _currentHutonController.Makura.SetActive(true);
            }

            transform.rotation = _currentHutonController.GetRotation();

            Quaternion additionalRotation = Quaternion.AngleAxis(-81.0f, transform.right);
            transform.rotation = additionalRotation * transform.rotation;
        }
        public void ResultSleep(ResultHutonController resultHutonController)
        {
            _animator.SetBool("Walk", false);
            _animator.SetBool("Run", false);
            _animator.SetBool("Sleep", true);
            _rb.isKinematic = true;

            transform.rotation = resultHutonController.GetRotation();
            float offset = 0;
            if (transform.rotation.y == 0.0f)
            {
                offset = 1.5f;
            }
            Vector3 hutonPosition = resultHutonController.GetCenterPosition();
            transform.position = new Vector3(hutonPosition.x, hutonPosition.y, hutonPosition.z - 0.8f + offset);

            Quaternion additionalRotation = Quaternion.AngleAxis(-81.0f, transform.right);
            transform.rotation = additionalRotation * transform.rotation;
        }
        /// <summary>
        /// 布団から起きる
        /// </summary>
        public void WakeUpFromHuton()
        {
            if (_currentHutonController != null)
            {
                _rb.isKinematic = true;
                _col.enabled = false;
                _col.center = _nomalColliderCenter;
                _currentHutonController.Makura.SetActive(false);
                Vector3 hutonPosition = _currentHutonController.GetCenterPosition();
                transform.position = new Vector3(hutonPosition.x, hutonPosition.y + 0.04f, hutonPosition.z);

                StartCoroutine(PhysicsAndColliderDelay());
            }
        }
        /// <summary>
        /// プレイヤーの物理とコライダーの無効化
        /// </summary>
        /// <returns>0.1秒後に有効化</returns>
        private IEnumerator PhysicsAndColliderDelay()
        {
            transform.rotation = _beforeSleepRotation;
            yield return new WaitForSeconds(0.1f);

            _rb.isKinematic = false;
            _col.enabled = true;

            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _isSleep = false;
        }
        /// <summary>
        /// 近くにプレイヤーがいるか
        /// </summary>
        /// <returns>近くにプレイヤーがいたらtrueを返す</returns>
        private bool IsCheckPlayer()
        {
            Vector3 playerPosition = transform.position;
            Collider[] hitColliders = Physics.OverlapSphere(playerPosition, _playerSerchDistance);
            bool foundOpponent = false;

            foreach (var collider in hitColliders)
            {
                if (collider.CompareTag("Player") && collider.gameObject != gameObject)
                {
                    _targetPosition = collider.gameObject.transform.position;
                    foundOpponent = true;
                    return true;
                }
            }

            if (!foundOpponent && _targetPosition != Vector3.zero)
            {
                _targetPosition = Vector3.zero;
            }

            return false;
        }
        private Vector3 TargetPosition(GameObject targetPlayer)
        {
            Vector3 playerPosition = transform.position;
            Collider[] hitColliders = Physics.OverlapSphere(playerPosition, 10.0f);

            foreach (var collider in hitColliders)
            {
                if (collider.CompareTag("Player") && collider.gameObject == targetPlayer)
                {
                    return collider.gameObject.transform.position;
                }
            }
            return Vector3.zero;
        }
        /// <summary>
        /// 接触したオブジェクトの情報をもとに操作
        /// </summary>
        /// <param name="collision">接触したオブジェクトの情報</param>
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Huton"))
            {
                _currentHuton = collision.gameObject.transform;
                _currentHutonController = collision.gameObject.GetComponent<HutonController>();
            }
            MakuraController makuraController = collision.gameObject.GetComponent<MakuraController>();

            if (makuraController != null && collision.gameObject.CompareTag("Makura") && makuraController.Thrower != gameObject && makuraController.IsThrow && !_isHitCoolTime)
            {
                _isCanCatch = false;
                StartCoroutine(HitCoolTimeDelay());
                _animator.SetBool("Walk", false);
                HitMotion(false);
                if (!_isVibrating)
                {
                    StartCoroutine(HitStopVibration(makuraController.IsCounterAttack, makuraController.ThrowedTime, collision.gameObject.transform));
                }
            }
            if (collision.gameObject.CompareTag("Meteor"))
            {
                _isCanCatch = false;
                _isHitCoolTime = true;
                _animator.SetBool("Walk", false);
                HitMotion(false);
                if (!_isVibrating)
                {
                    StartCoroutine(HitStopVibration(false, 0.0f, null));
                }
            }
        }
        private IEnumerator HitStopVibration(bool isCounterAttack, float throwedTime, Transform makuratransform)
        {
            _isVibrating = true;
            Vector3 hitPosition = transform.position;

            float elapsedTime = 0.0f;

            while (elapsedTime < _vibrationTime)
            {
                float strength = Mathf.Lerp(_vibrationStrength * (isCounterAttack ? 2.0f : 1.0f), 0, elapsedTime / _vibrationTime);
                Vector3 randomOffset = new Vector3(
                    UnityEngine.Random.Range(-strength, strength),
                    IsGround() ? 0 : UnityEngine.Random.Range(-strength, strength),
                    UnityEngine.Random.Range(-strength, strength)
                );

                transform.position = Vector3.Lerp(transform.position, hitPosition + randomOffset, Time.deltaTime * 100);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = hitPosition;
            if (makuratransform != null)
            {
                Vector3 direction = transform.position - makuratransform.position;
                if (direction.y <= 0)
                {
                    direction = new Vector3(direction.x, -direction.y, direction.z);
                }
                float lerpPercentage = 1 - Mathf.InverseLerp(0.1f, 0.5f, direction.y);
                float upForce = Mathf.Lerp(20, 50, lerpPercentage);

                _rb.AddForce(direction.normalized * 100 + Vector3.up * upForce * throwedTime, ForceMode.Impulse);
            }

            yield return new WaitForSeconds(0.5f);
            _isVibrating = false;
            _isHitCoolTime = false;
        }
        /// <summary>
        /// 枕が当たったときのモーション
        /// </summary>
        private void HitMotion(bool teacher)
        {
            if (!_rb.isKinematic)
            {
                _rb.velocity = Vector3.zero;
            }
            if (!teacher)
            {
                _isHitStop = true;
                _playerStatus.SpUp();
                StartCoroutine(HitStopCoroutine());
            }
        }
        /// <summary>
        /// 枕を当てられると1秒制止する
        /// </summary>
        /// <returns>1秒後に解除</returns>
        private IEnumerator HitStopCoroutine()
        {
            yield return new WaitForSeconds(0.5f);
            _isHitStop = false;
            _isHitCoolTime = false;
        }
        private IEnumerator HitCoolTimeDelay()
        {
            yield return new WaitForSeconds(0.01f);
            _isHitCoolTime = true;
        }

        private IEnumerator CounterAttackCoroutine()
        {
            _isCounterAttackTime = true;
            yield return new WaitForSeconds(1.0f);
            _isCounterAttackTime = false;
        }
        private IEnumerator ThrowCoolTimeCorountine()
        {
            _isThrowCoolTime = true;
            yield return new WaitForSeconds(0.5f);
            _isThrowCoolTime = false;
        }
        private IEnumerator TeacherMakuraHit()
        {
            bool makuraHit = false;
            _isTeacherMakuraHit = true;
            Vector3 hitPosition = new Vector3(transform.position.x, 0, transform.position.z);

            float elapsedTime = 0.0f;

            while (elapsedTime < 8.0f)
            {
                Vector3 randomOffset = new Vector3(
                    UnityEngine.Random.Range(-0.1f, 0.1f),
                    0,
                    UnityEngine.Random.Range(-0.1f, 0.1f)
                );

                transform.position = Vector3.Lerp(transform.position, new Vector3(hitPosition.x, transform.position.y, hitPosition.z) + randomOffset, Time.deltaTime * 100);

                elapsedTime += Time.deltaTime;
                if (_isHitStop)
                {
                    _isTeacherMakuraHit = false;
                    makuraHit = true;
                    break;
                }
                yield return null;
            }
            _isTeacherMakuraHit = false;
            if (!makuraHit)
            {
                transform.position = hitPosition;
            }
        }

        public IEnumerator SpeedUpCoroutine()
        {
            _isSpeedUp = true;
            yield return new WaitForSeconds(8.0f);
            _isSpeedUp = false;
        }
    }
}
