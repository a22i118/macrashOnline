using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCS;
public class ExplosionRange : MonoBehaviour
{
    private GameObject _thrower;//投げたプレイヤー
    private ScoreManager _scoreManager;
    public GameObject Thrower { get => _thrower; set => _thrower = value; }

    // Start is called before the first frame update
    void Start()
    {
        _scoreManager = FindObjectOfType<ScoreManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject != _thrower)
        {
            if (collider.gameObject.CompareTag("Player") && collider is CapsuleCollider)
            {
                PlayerController playerController = collider.gameObject.GetComponent<PlayerController>();
                if (playerController == null)
                {
                    ThrowMakuraDemo throwMakuraDemo = collider.GetComponent<ThrowMakuraDemo>();
                    if (!throwMakuraDemo.IsHitCoolTime)
                    {
                        _scoreManager.UpdateScore(_thrower.name, collider.gameObject.name, false);
                    }
                }
                else if (!playerController.IsHitCoolTime)
                {
                    _scoreManager.UpdateScore(_thrower.name, collider.gameObject.name, playerController.IsSleep);
                }
            }
            if (collider.gameObject.CompareTag("Makura"))
            {
                Rigidbody rb = collider.gameObject.GetComponent<Rigidbody>();
                if (rb != null && !rb.isKinematic)
                {
                    rb.useGravity = true;
                    rb.velocity = Vector3.zero;
                }
            }
        }
    }
}
