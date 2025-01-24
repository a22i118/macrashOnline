using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowMakuraDemo : MonoBehaviour
{
    [SerializeField] private GameObject _alterEgoMakura;
    private bool _isVibrating = false;
    private bool _isHitCoolTime = false;
    private bool _isPickUpCoolTime = false;
    private bool _isExecuteOnce = false;
    private float _pickUpDistance = 3.0f;
    private float _vibrationStrength = 0.3f;//振動の強さ
    private float _vibrationTime = 0.3f;//振動する時間
    private GameObject _currentMakura = null;
    private Rigidbody _rb;
    private Vector3 _targetPosition;
    private MakuraController _makuraController;
    public bool IsHitCoolTime { get => _isHitCoolTime; set => _isHitCoolTime = value; }

    public enum ThrowType
    {
        Nomal,
        Charge
    }

    // Start is called before the first frame update
    void Start()
    {
        _targetPosition = new Vector3(transform.position.x + -4, transform.position.y, transform.position.z);
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentMakura != null && !_isExecuteOnce)
        {
            _isPickUpCoolTime = true;
            _isExecuteOnce = true;
            StartCoroutine(ThrowMakuraNomal());
        }
        if (_currentMakura == null && !_isPickUpCoolTime)
        {
            PickUpMakura();
            _isExecuteOnce = false;
        }

    }
    private void PickUpMakura()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _pickUpDistance);
        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Makura") && !collider.GetComponent<MakuraController>().IsThrow && !collider.GetComponent<MakuraController>().IsAlterEgo)
            {
                _currentMakura = collider.gameObject;

                _currentMakura.SetActive(false);
                break;
            }
        }
    }
    private void ThrowMakura(ThrowType throwType)
    {
        if (_currentMakura != null)
        {
            Rigidbody rb = _currentMakura.GetComponent<Rigidbody>();
            _makuraController = _currentMakura.GetComponent<MakuraController>();
            if (rb.velocity != Vector3.zero)
            {
                rb.velocity = Vector3.zero;
            }
            rb.isKinematic = false;


            Vector3 throwDirection;
            if (_targetPosition != Vector3.zero)
            {
                Vector3 targetDirection = _targetPosition - transform.position;
                targetDirection.y = 0;
                throwDirection = targetDirection.normalized;
                transform.rotation = Quaternion.LookRotation(throwDirection);
            }
            else
            {
                throwDirection = transform.forward;
            }
            float forwardForce = 0.0f;
            float upwardForce = 0.0f;
            float throwDistance = 0.0f;
            float throwHeight = 0.0f;
            if (_makuraController.CurrentColorType == ColorChanger.ColorType.Nomal)
            {
                switch (throwType)
                {
                    case ThrowType.Nomal:
                        forwardForce = 900.0f;
                        upwardForce = 200.0f;
                        throwDistance = 1.3f;
                        throwHeight = 1.0f;
                        // Debug.Log("通常");
                        break;
                    case ThrowType.Charge:
                        forwardForce = 300.0f;
                        upwardForce = 700.0f;
                        throwDistance = 0.5f;
                        throwHeight = 2.0f;
                        // Debug.Log("くらえ！爆発まくら");
                        break;
                }
            }
            else if (_makuraController.CurrentColorType == ColorChanger.ColorType.Red)
            {
                rb.useGravity = false;
                forwardForce = 500.0f;
                throwDistance = 1.3f;
                throwHeight = 1.0f;
            }
            else if (_makuraController.CurrentColorType == ColorChanger.ColorType.Blue)
            {
                rb.useGravity = false;
                forwardForce = 300.0f;
                throwDistance = 1.3f;
                throwHeight = 1.0f;
                if (_alterEgoMakura != null)
                {
                    _alterEgoMakura.GetComponent<MakuraController>().CurrentColorType = ColorChanger.ColorType.Blue;
                }
                Vector3[] throwAngles = new Vector3[]
                {
                        Quaternion.Euler(0, 45, 0) * transform.forward,
                        Quaternion.Euler(0, -45, 0) * transform.forward
                };
                foreach (var angle in throwAngles)
                {
                    Vector3 throwPositionBlue = transform.position + angle.normalized * 1.7f + Vector3.up * throwHeight;

                    GameObject clone = Instantiate(_alterEgoMakura, throwPositionBlue, Quaternion.identity);

                    MakuraController cloneMC = clone.GetComponent<MakuraController>();
                    Rigidbody cloneRb = clone.GetComponent<Rigidbody>();

                    cloneMC.CurrentColorType = ColorChanger.ColorType.Blue;
                    cloneMC.IsAlterEgo = true;
                    cloneMC.IsThrow = true;
                    cloneMC.Thrower = gameObject;
                    cloneRb.useGravity = false;

                    cloneRb.AddForce(angle.normalized * forwardForce);
                    cloneRb.maxAngularVelocity = 100;
                    cloneRb.AddTorque(Vector3.up * 120.0f);
                }
            }
            else if (_makuraController.CurrentColorType == ColorChanger.ColorType.Green)
            {
                rb.useGravity = false;
                forwardForce = 300.0f;
                throwDistance = 1.3f;
                throwHeight = 1.0f;
                if (_alterEgoMakura != null)
                {
                    _alterEgoMakura.GetComponent<MakuraController>().CurrentColorType = ColorChanger.ColorType.Blue;
                }
                Vector3 upDirection = transform.up;

                Vector3[] throwAngles = new Vector3[]
                {
                        Quaternion.AngleAxis(60, transform.right) * upDirection,
                        Quaternion.AngleAxis(75, transform.right) * upDirection
                };

                foreach (var angle in throwAngles)
                {
                    Vector3 throwPositionGreen = transform.position + angle.normalized * throwDistance + Vector3.up * throwHeight;

                    GameObject clone = Instantiate(_alterEgoMakura, throwPositionGreen, Quaternion.identity);

                    MakuraController cloneMC = clone.GetComponent<MakuraController>();
                    Rigidbody cloneRb = clone.GetComponent<Rigidbody>();

                    cloneMC.CurrentColorType = ColorChanger.ColorType.Green;
                    cloneMC.IsAlterEgo = true;
                    cloneMC.IsThrow = true;
                    cloneMC.Thrower = gameObject;
                    cloneRb.useGravity = false;

                    cloneRb.AddForce(angle.normalized * forwardForce);
                    cloneRb.maxAngularVelocity = 100;
                    cloneRb.AddTorque(Vector3.up * 120.0f);
                }
            }
            else if (_makuraController.CurrentColorType == ColorChanger.ColorType.Black)
            {
                Collider col = _currentMakura.GetComponent<Collider>();
                col.isTrigger = true;
                rb.useGravity = false;
                forwardForce = 500.0f;
                throwDistance = 1.3f;
                throwHeight = 1.0f;
            }

            Vector3 throwPosition = transform.position + throwDirection * throwDistance + Vector3.up * throwHeight;

            _currentMakura.transform.position = throwPosition;
            _currentMakura.SetActive(true);
            _makuraController.IsThrow = true;
            _makuraController.Thrower = gameObject;

            rb.AddForce(throwDirection * forwardForce + Vector3.up * upwardForce);
            rb.maxAngularVelocity = 100;
            rb.AddTorque(Vector3.up * 120.0f);

            _currentMakura = null;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        MakuraController makuraController = collision.gameObject.GetComponent<MakuraController>();

        if (collision.gameObject.CompareTag("Makura") && makuraController.Thrower != gameObject && makuraController.IsThrow && !_isHitCoolTime)
        {
            _isHitCoolTime = true;
            HitMotion();
            if (!_isVibrating)
            {
                StartCoroutine(HitStopVibration(makuraController.IsCounterAttack));
            }
        }
    }
    // private IEnumerator HitStopVibration()
    // {
    //     _isVibrating = true;
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

    //     _isVibrating = false;
    //     _isHitCoolTime = false;
    // }
    private IEnumerator HitStopVibration(bool isCounterAttack)
    {
        _isVibrating = true;
        Vector3 hitPosition = transform.position;

        float elapsedTime = 0.0f;

        while (elapsedTime < _vibrationTime)
        {
            float strength = Mathf.Lerp(_vibrationStrength * (isCounterAttack ? 2.0f : 1.0f), 0, elapsedTime / _vibrationTime);

            Vector3 randomOffset = new Vector3(
                UnityEngine.Random.Range(-strength, strength),
                0,
                UnityEngine.Random.Range(-strength, strength)
            );

            transform.position = Vector3.Lerp(transform.position, hitPosition + randomOffset, Time.deltaTime * 100);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = hitPosition;

        _isVibrating = false;
        _isHitCoolTime = false;
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Explosion"))
        {
            ExplosionRange explosionRangeScript = collider.GetComponent<ExplosionRange>();
            if (explosionRangeScript.Thrower != gameObject)
            {
                _isHitCoolTime = true;
                HitMotion();
                if (!_isVibrating)
                {
                    StartCoroutine(HitStopVibration(true));
                }
            }
        }
    }
    private void HitMotion()
    {

        _rb.velocity = Vector3.zero;

        StartCoroutine(HitStopCoroutine());
    }
    private IEnumerator HitStopCoroutine()
    {
        yield return new WaitForSeconds(1.0f);
    }

    private IEnumerator ThrowMakuraNomal()
    {
        yield return new WaitForSeconds(3.0f);
        _isPickUpCoolTime = false;
        ThrowMakura(ThrowType.Nomal);
    }
}
