using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCS;
using Unity.VisualScripting;

public class MakuraController : ColorChanger
{
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _hutonLayer;
    [SerializeField] private GameObject _explosionRange;
    private bool _isThrow = false;
    private bool _isAlterEgo = false;
    private bool _isTouching = false;
    private bool _isGameStart = false;
    private bool _isCharge = false;
    private bool _isCounterAttack = false;
    private bool _isHitCoolTimeOne = false;
    private float _vibrationStrength = 0.05f;
    private float _vibrationTime = 0.2f;
    private float _throwedTime = 3.0f;
    // private float _trailRate = 2.0f;
    private Rigidbody _rb;
    private Collider _col;
    private Vector3 _trailPos;
    private Quaternion _initialRotation;
    private GameObject _thrower;
    private ExplosionRange _explosionRangeScript;
    private ScoreManager _scoreManager;
    private ScaleType _currentScaleType = ScaleType.Nomal;
    private static readonly int PROPERTY_TRAIL_DIR = Shader.PropertyToID("_TrailDir");
    public bool IsThrow { get => _isThrow; set => _isThrow = value; }
    public bool IsAlterEgo { get => _isAlterEgo; set => _isAlterEgo = value; }
    public bool IsCharge { get => _isCharge; set => _isCharge = value; }
    public bool IsCounterAttack { get => _isCounterAttack; set => _isCounterAttack = value; }
    public bool IsGameStart { get => _isGameStart; set => _isGameStart = value; }
    public float ThrowedTime { get => _throwedTime; set => _throwedTime = value; }
    public GameObject Thrower { get => _thrower; set => _thrower = value; }
    public ScaleType CurrentScaleType { get => _currentScaleType; set => _currentScaleType = value; }

    public enum ScaleType
    {
        Nomal,
        First,
        Second
    }
    public void Sleep()
    {
        // Colliderを無効化
        _col.enabled = false;
        // Rigidbodyを無効化（位置を姿勢指定のみで動かす）
        _rb.isKinematic = true;
    }

    // WakeUp（ローカルで衝突判定やシミュレーションを行う状態）
    public void WakeUp()
    {
        // Colliderを有効化
        _col.enabled = true;
        // Rigidbodyを有効化
        _rb.isKinematic = false;
    }
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _trailPos = transform.position;
        _col = GetComponent<Collider>();
        _initialRotation = transform.rotation;
        _scoreManager = FindObjectOfType<ScoreManager>();
        _groundLayer |= _hutonLayer;
    }
    void Start()
    {

    }
    // void Update()
    // {
    // // _trailPos = Vector3.Lerp(_trailPos, transform.position, Mathf.Clamp01(Time.deltaTime * _trailRate));
    // _trailPos = Vector3.Lerp(_trailPos, transform.position, Time.deltaTime);
    // Vector3 direction = transform.InverseTransformDirection(_trailPos - transform.position);
    // _blurMaterial.SetVector(PROPERTY_TRAIL_DIR, direction);
    // if (_isGameStart)
    // {
    //     ColorChange(_currentColorType);
    // }
    // ScaleChange(_currentScaleType);
    // BlackMakuraPositionUpdate();
    // if (_isThrow)
    // {
    //     StartCoroutine(AutoUseGrabity());
    //     _throwedTime -= Time.deltaTime;
    // }
    // else
    // {
    //     _throwedTime = 3.0f;
    // }
    // if (transform.position.y >= 9.5f)
    // {
    //     _rb.useGravity = true;
    // }
    // if (_currentColorType == ColorType.Black && _isThrow)
    // {
    //     if (_rb.useGravity && !_isTouching)
    //     {
    //         _col.isTrigger = false;
    //     }
    // }
    // if (_currentColorType != ColorType.Black)
    // {
    //     PositionReset();
    // }
    // }
    /// <summary>
    /// スケールを変える
    /// </summary>
    /// <param name="type"></param>
    // private void ScaleChange(ScaleType type)
    // {
    //     if (type == ScaleType.Second)
    //     {
    //         transform.localScale = new Vector3(6.0f, 6.0f, 6.0f);
    //     }
    //     else if (type == ScaleType.First)
    //     {
    //         transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
    //         StartCoroutine(ScaleChangeCoolTime());
    //     }
    //     else if (type == ScaleType.Nomal)
    //     {
    //         transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    //     }
    // }

    // private void OnCollisionEnter(Collision collision)
    // {
    // _isTouching = true;

    // if (!_rb.isKinematic)
    // {
    //     if (CurrentColorType != ColorType.Red)
    //     {
    //         if (_rb.velocity != Vector3.zero && collision.gameObject != _thrower)
    //         {
    //             _rb.useGravity = true;
    //             _rb.velocity = Vector3.zero;
    //         }
    //     }

    //     if (collision.gameObject.CompareTag("Player") && _isThrow && collision.gameObject != _thrower)
    //     {
    //         PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
    //         if (playerController == null)
    //         {
    //             ThrowMakuraDemo throwMakuraDemo = collision.gameObject.GetComponent<ThrowMakuraDemo>();
    //             if (!throwMakuraDemo.IsHitCoolTime)
    //             {
    //                 StartCoroutine(ScaleResetDeray());
    //                 _rb.useGravity = true;
    //                 _rb.isKinematic = true;
    //                 _rb.isKinematic = false;
    //                 _rb.velocity = Vector3.zero;
    //                 StartCoroutine(HitStopVibration());
    //                 StartCoroutine(HitCoolTime());
    //                 _scoreManager.UpdateScore(_thrower.name, collision.gameObject.name, false);
    //             }
    //         }
    //         else if (!playerController.IsHitCoolTime && !_isHitCoolTimeOne)
    //         {
    //             _isHitCoolTimeOne = true;
    //             StartCoroutine(HitCoolTimeDelay());
    //             StartCoroutine(ScaleResetDeray());
    //             _rb.useGravity = true;
    //             _rb.isKinematic = true;
    //             _rb.isKinematic = false;
    //             _rb.velocity = Vector3.zero;
    //             StartCoroutine(HitStopVibration());
    //             // StartCoroutine(IgnoreCollisionTime(collision.gameObject.GetComponent<CapsuleCollider>()));
    //             StartCoroutine(HitCoolTime());
    //             _scoreManager.UpdateScore(_thrower.name, collision.gameObject.name, playerController.IsSleep);
    //         }
    //     }
    //     if (collision.gameObject.CompareTag("Makura"))
    //     {
    //         _isThrow = false;
    //         StartCoroutine(ScaleResetDeray());
    //         _rb.useGravity = true;
    //         _rb.velocity = Vector3.zero;
    //         transform.rotation = Quaternion.Euler(_initialRotation.eulerAngles.x, transform.rotation.eulerAngles.y, _initialRotation.eulerAngles.z);
    //     }
    // }
    // if (_isCharge)
    // {
    //     HitSpawn();
    //     _isCharge = false;
    // }
    // if ((_groundLayer & (1 << collision.gameObject.layer)) != 0)
    // {
    //     _isThrow = false;
    //     _isCounterAttack = false;
    //     StartCoroutine(ScaleResetDeray());
    //     transform.rotation = Quaternion.Euler(_initialRotation.eulerAngles.x, transform.rotation.eulerAngles.y, _initialRotation.eulerAngles.z);
    //     if (!_rb.isKinematic)
    //     {
    //         _rb.velocity = Vector3.zero;
    //     }
    //     _rb.isKinematic = true;
    //     _currentColorType = GetRandomColor();
    // }

    // if (_isAlterEgo)
    // {
    //     Destroy(gameObject, 0.4f);
    // }
    // }
    /// <summary>
    /// 爆発（の当たり判定）を生成する
    /// </summary>
    // private void HitSpawn()
    // {
    //     GameObject spawnedObject = Instantiate(_explosionRange, transform.position, Quaternion.identity);
    //     if (_explosionRange != null)
    //     {
    //         _explosionRangeScript = spawnedObject.GetComponent<ExplosionRange>();
    //     }
    //     if (CurrentScaleType == ScaleType.Second)
    //     {
    //         spawnedObject.transform.localScale = new Vector3(10, 10, 10);
    //     }
    //     _explosionRangeScript.Thrower = _thrower;
    //     Destroy(spawnedObject, 2.0f);
    // }
    // private void OnTriggerEnter(Collider collider)
    // {
    //     _isTouching = true;
    //     if (_col.isTrigger && _isThrow && collider.gameObject != _thrower)
    //     {
    //         if (collider.gameObject.CompareTag("Player") && collider is CapsuleCollider)
    //         {
    //             PlayerController playerController = collider.gameObject.GetComponent<PlayerController>();
    //             if (playerController == null)
    //             {
    //                 ThrowMakuraDemo throwMakuraDemo = collider.gameObject.GetComponent<ThrowMakuraDemo>();
    //                 if (!throwMakuraDemo.IsHitCoolTime)
    //                 {
    //                     StartCoroutine(BlackMakuraHit());
    //                     StartCoroutine(HitCoolTime());
    //                 }
    //             }
    //             else if (!playerController.IsHitCoolTime)
    //             {
    //                 StartCoroutine(BlackMakuraHit());
    //                 StartCoroutine(HitCoolTime());
    //             }
    //         }
    //         if (collider.gameObject.CompareTag("Makura"))
    //         {
    //             if (collider.gameObject.GetComponent<MakuraController>()._currentColorType == ColorType.Black && collider.gameObject.GetComponent<MakuraController>().Thrower != _thrower)
    //             {
    //                 StartCoroutine(BlackMakuraHit());
    //                 Rigidbody rb = collider.gameObject.GetComponent<Rigidbody>();
    //                 if (rb != null)
    //                 {
    //                     rb.useGravity = true;
    //                     rb.velocity = Vector3.zero;
    //                 }
    //             }

    //         }
    //     }
    // }
    // private void OnTriggerExit()
    // {
    //     _isTouching = false;
    // }
    /// <summary>
    /// 黒枕があたったときの処理
    /// </summary>
    // private void BlackMakuraPositionUpdate()
    // {
    //     float xMin = -9.0f;
    //     float xMax = 9.0f;
    //     float yMin = 0.0f;
    //     float yMax = 6.0f;
    //     float zMin = -4.0f;
    //     float zMax = 8.0f;
    //     if (CurrentColorType == ColorType.Black && _isThrow)
    //     {
    //         Vector3 position = transform.position;

    //         if (position.x > xMax)
    //         {
    //             position.x = xMin;
    //         }
    //         else if (position.x < xMin)
    //         {
    //             position.x = xMax;
    //         }

    //         if (position.y > yMax)
    //         {
    //             position.y = yMin;
    //         }
    //         else if (position.y < yMin)
    //         {
    //             position.y = yMax;
    //         }

    //         if (position.z > zMax)
    //         {
    //             position.z = zMin;
    //         }
    //         else if (position.z < zMin)
    //         {
    //             position.z = zMax;
    //         }

    //         transform.position = position;
    //     }
    // }
    // private void PositionReset()
    // {
    //     float minX = -9.0f;
    //     float maxX = 9.0f;
    //     float minY = -0.5f;
    //     float maxY = 15.0f;
    //     float minZ = -4.0f;
    //     float maxZ = 8.0f;
    //     bool _isOutStage = false;
    //     Vector3 currentPosition = transform.position;

    //     if (currentPosition.x < minX)
    //     {
    //         currentPosition.x = minX + 2f;
    //         _isOutStage = true;
    //     }
    //     else if (currentPosition.x > maxX)
    //     {
    //         currentPosition.x = maxX - 2f;
    //         _isOutStage = true;
    //     }
    //     if (currentPosition.y < minY)
    //     {
    //         currentPosition.y = 5f;
    //         _isOutStage = true;
    //     }
    //     else if (currentPosition.y > maxY)
    //     {
    //         currentPosition.y = 5f;
    //         _isOutStage = true;
    //     }
    //     if (currentPosition.z < minZ)
    //     {
    //         currentPosition.z = minZ + 2f;
    //         _isOutStage = true;
    //     }
    //     else if (currentPosition.z > maxZ)
    //     {
    //         currentPosition.z = maxZ - 2f;

    //         _isOutStage = true;
    //     }
    //     if (!_rb.isKinematic && _isOutStage)
    //     {
    //         _rb.useGravity = true;
    //         _rb.velocity = Vector3.zero;
    //         _isThrow = false;
    //         transform.position = currentPosition;
    //     }
    // }
    /// <summary>
    /// ランダムで色を変える
    /// </summary>
    /// <returns></returns>
    // private ColorType GetRandomColor()
    // {
    //     const int nomal = 30;
    //     const int blue = 20;
    //     const int green = 20;
    //     const int red = 15;
    //     const int black = 15;
    //     int all = nomal + red + blue + green + black;
    //     float randomValue = UnityEngine.Random.Range(0, all);

    //     if (randomValue < nomal)
    //     {
    //         return ColorType.Nomal;
    //     }
    //     else if (randomValue < nomal + blue)
    //     {
    //         return ColorType.Blue;
    //     }
    //     else if (randomValue < nomal + blue + green)
    //     {
    //         return ColorType.Green;
    //     }
    //     else if (randomValue < nomal + blue + green + red)
    //     {
    //         return ColorType.Red;
    //     }
    //     else
    //     {
    //         return ColorType.Black;
    //     }
    // }
    /// <summary>
    /// ヒット時のクールタイム
    /// </summary>
    /// <returns></returns>
    // private IEnumerator HitCoolTime()
    // {
    //     yield return new WaitForSeconds(1.0f);
    //     _isThrow = false;
    // }
    /// <summary>
    /// スケールを変える際のクールタイム
    /// </summary>
    /// <returns></returns>
    // private IEnumerator ScaleChangeCoolTime()
    // {
    //     yield return new WaitForSeconds(1.2f);
    //     _currentScaleType = ScaleType.Second;
    // }
    /// <summary>
    /// 重力が自動でかかる時間
    /// </summary>
    /// <returns></returns>
    // private IEnumerator AutoUseGrabity()
    // {
    //     yield return new WaitForSeconds(11.0f);
    //     _rb.useGravity = true;
    // }
    /// <summary>
    /// 敵や枕に当たったときの揺れ
    /// </summary>
    /// <returns></returns>
    // public IEnumerator HitStopVibration()
    // {
    //     Vector3 hitPosition = transform.position;

    //     float elapsedTime = 0.0f;
    //     while (elapsedTime < _vibrationTime)
    //     {
    //         Vector3 randomOffset = new Vector3(
    //             UnityEngine.Random.Range(-_vibrationStrength, _vibrationStrength),
    //             0,
    //             UnityEngine.Random.Range(-_vibrationStrength, _vibrationStrength)
    //         );

    //         transform.position = hitPosition + randomOffset;

    //         elapsedTime += 0.05f;
    //         yield return new WaitForSeconds(0.05f);
    //     }

    //     transform.position = hitPosition;

    //     _isThrow = false;
    // }
    /// <summary>
    /// 黒枕が当たったときの処理
    /// </summary>
    /// <returns></returns>
    // private IEnumerator BlackMakuraHit()
    // {
    //     yield return new WaitForSeconds(0.01f);
    //     _col.isTrigger = false;

    //     _currentScaleType = ScaleType.Nomal;
    //     _rb.useGravity = true;
    //     _rb.isKinematic = true;
    //     _rb.isKinematic = false;
    //     StartCoroutine(HitStopVibration());
    // }
    /// <summary>
    /// ヒットクールを発動させるのに少し遅らせる
    /// </summary>
    /// <returns></returns>
    // private IEnumerator HitCoolTimeDelay()
    // {
    //     yield return new WaitForSeconds(0.01f);
    //     _isHitCoolTimeOne = false;
    // }
    // private IEnumerator ScaleResetDeray()
    // {
    //     yield return new WaitForSeconds(0.01f);
    //     _currentScaleType = ScaleType.Nomal;
    // }
    // private IEnumerator IgnoreCollisionTime(Collider collider)
    // {
    //     Physics.IgnoreCollision(collider, _col, true);
    //     yield return new WaitForSeconds(2.1f);
    //     Physics.IgnoreCollision(collider, _col, false);
    // }
}
