using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{

    Animator anim;
    MeshRenderer mesh;

    void Start()
    {
        anim = GetComponent<Animator>();
    }
    void OnMouseEnter()
    {
        anim.SetBool("_isBig", true);
    }

    void OnMouseExit()
    {
        anim.SetBool("_isBig", false);
    }


}
