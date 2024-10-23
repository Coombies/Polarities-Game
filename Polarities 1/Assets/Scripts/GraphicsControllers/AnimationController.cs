using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Controls which animations are played when.
/// </summary>
public class AnimationController : MonoBehaviour
{
    private float xMovement;

    [SerializeField] private Animator anim;

    /// <summary>
    /// Changes IsWalking from true to false and vice versa
    /// depending on the players actions.
    /// </summary>
    void Update()
    {
        xMovement = Mathf.Abs(Input.GetAxisRaw("Horizontal"));

        if (xMovement > 0f)
            anim.SetBool("IsWalking", true);
        else
            anim.SetBool("IsWalking", false);
    }
}
