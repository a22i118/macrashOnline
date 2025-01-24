using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerCS
{
    public partial class PlayerController : MonoBehaviour
    {
        private void ThrowMakura(ThrowType throwType)
        {
            if (_currentMakuras[0] != null)
            {
                if (IsWallThrowCheck())
                {
                    Debug.Log("壁が近いから投げられないぜ！");
                    return;
                }
                Rigidbody rb = _currentMakuras[0].GetComponent<Rigidbody>();
                _makuraController = _currentMakuras[0].GetComponent<MakuraController>();
                if (rb.velocity != Vector3.zero)
                {
                    rb.velocity = Vector3.zero;
                }
                rb.isKinematic = true;
                rb.isKinematic = false;
                if (_isCounterAttackTime)
                {
                    _targetPosition = TargetPosition(_makuraController.Thrower);
                }

                Vector3 throwDirection;
                if (_targetPosition != Vector3.zero)
                {
                    Vector3 targetDirection = _targetPosition - transform.position;
                    throwDirection = targetDirection.normalized;
                    targetDirection.y = 0.0f;
                    transform.rotation = Quaternion.LookRotation(targetDirection);
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
                            forwardForce = _isCounterAttackTime ? 1600.0f : 800.0f;
                            upwardForce = 200.0f;
                            throwDistance = 1.3f;
                            throwHeight = 1.0f;
                            break;
                        case ThrowType.Charge:
                            forwardForce = 300.0f;
                            upwardForce = 700.0f;
                            throwDistance = 0.5f;
                            throwHeight = 2.0f;
                            _makuraController.IsCharge = true;
                            break;
                    }
                }
                else if (_makuraController.CurrentColorType == ColorChanger.ColorType.Red)
                {
                    rb.useGravity = false;
                    forwardForce = _isCounterAttackTime ? 1000.0f : 500.0f;
                    throwDistance = 1.3f;
                    throwHeight = 1.0f;
                }
                else if (_makuraController.CurrentColorType == ColorChanger.ColorType.Blue)
                {
                    rb.useGravity = false;
                    forwardForce = _isCounterAttackTime ? 600.0f : 300.0f;
                    throwDistance = 1.7f;
                    throwHeight = 1.0f;
                    Vector3[] throwAngles = new Vector3[]
                    {
                        Quaternion.Euler(0, 45, 0) * transform.forward,
                        Quaternion.Euler(0, -45, 0) * transform.forward
                    };
                    CloneMakuraSpawn(ColorChanger.ColorType.Blue, throwAngles, forwardForce, throwDistance, throwHeight);

                }
                else if (_makuraController.CurrentColorType == ColorChanger.ColorType.Green)
                {
                    rb.useGravity = false;
                    forwardForce = _isCounterAttackTime ? 600.0f : 300.0f;
                    throwDistance = 1.3f;
                    throwHeight = 1.0f;
                    Vector3[] throwAngles = new Vector3[]
                    {
                        Quaternion.AngleAxis(60, transform.right) * transform.up,
                        Quaternion.AngleAxis(75, transform.right) * transform.up
                    };
                    CloneMakuraSpawn(ColorChanger.ColorType.Green, throwAngles, forwardForce, throwDistance, throwHeight);
                }
                else if (_makuraController.CurrentColorType == ColorChanger.ColorType.Black)
                {
                    Collider col = _currentMakuras[0].GetComponent<Collider>();
                    col.isTrigger = true;
                    rb.useGravity = false;
                    forwardForce = _isCounterAttackTime ? 1000.0f : 500.0f;
                    throwDistance = 1.3f;
                    throwHeight = 1.0f;
                }

                if (_makuraController.CurrentScaleType == MakuraController.ScaleType.Second || _makuraController.CurrentScaleType == MakuraController.ScaleType.First)
                {
                    throwDistance += 1.5f;

                    if (throwType == ThrowType.Charge)
                    {
                        throwHeight = 2.0f;
                    }
                    else
                    {
                        throwHeight = 0.5f;
                    }
                    if (_makuraController.CurrentColorType == ColorChanger.ColorType.Blue)
                    {
                        throwHeight = 1.0f;
                    }
                }
                Vector3 throwPosition = transform.position + throwDirection * throwDistance + Vector3.up * throwHeight;


                _currentMakuras[0].transform.position = throwPosition;
                _currentMakuras[0].SetActive(true);
                _makuraController.IsThrow = true;
                _makuraController.IsCounterAttack = _isCounterAttackTime ? true : false;
                _makuraController.Thrower = gameObject;

                rb.AddForce(throwDirection * forwardForce + Vector3.up * upwardForce);
                rb.maxAngularVelocity = 100;
                rb.AddTorque(Vector3.up * 120.0f);

                _currentMakuras.RemoveAt(0);
            }
        }
        private void CloneMakuraSpawn(ColorChanger.ColorType colorType, Vector3[] throwAngles, float forwardForce, float throwDistance, float throwHeight)
        {
            if (_alterEgoMakura != null)
            {
                _alterEgoMakura.GetComponent<MakuraController>().CurrentColorType = colorType;
            }
            foreach (var angle in throwAngles)
            {
                Vector3 throwPosition = transform.position + angle.normalized * throwDistance + Vector3.up * throwHeight;

                GameObject clone = Instantiate(_alterEgoMakura, throwPosition, Quaternion.identity);

                MakuraController cloneMC = clone.GetComponent<MakuraController>();
                Rigidbody cloneRb = clone.GetComponent<Rigidbody>();

                cloneMC.CurrentColorType = colorType;
                cloneMC.IsAlterEgo = true;
                cloneMC.IsThrow = true;
                cloneMC.IsCounterAttack = _isCounterAttackTime ? true : false;
                cloneMC.Thrower = gameObject;
                cloneRb.useGravity = false;

                cloneRb.AddForce(angle.normalized * forwardForce);
                cloneRb.maxAngularVelocity = 100;
                cloneRb.AddTorque(Vector3.up * 120.0f);
            }
        }
    }

}
