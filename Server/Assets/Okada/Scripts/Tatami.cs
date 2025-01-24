using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tatami : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Makura") || collision.gameObject.CompareTag("Obstacles") || collision.gameObject.CompareTag("Player"))
        {
            // 畳に触れたオブジェクトを子に設定
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Makura"))
        {
            if (collision.transform.parent != null)
            {
                // 畳から離れたオブジェクトを元に戻す
                collision.transform.SetParent(null);
            }
        }
    }
}
