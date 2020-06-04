using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        Debug.Assert(animator != null, "Cannot find Animator component for TurretController");
    }

    void Update()
    {
        if ( animator != null )
        {
            if ( animator.GetCurrentAnimatorStateInfo(0).IsName("idle") )
            {
                animator.Play("shoot");
            }
        }
    }
}
