using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tatami : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Makura") || collision.gameObject.CompareTag("Obstacles") || collision.gameObject.CompareTag("Player"))
        {
            // ��ɐG�ꂽ�I�u�W�F�N�g���q�ɐݒ�
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Makura"))
        {
            if (collision.transform.parent != null)
            {
                // �􂩂痣�ꂽ�I�u�W�F�N�g�����ɖ߂�
                collision.transform.SetParent(null);
            }
        }
    }
}
