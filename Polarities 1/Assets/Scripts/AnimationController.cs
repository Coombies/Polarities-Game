using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private float xMovement;

    [SerializeField] private Animator anim;

    // Update is called once per frame
    void Update()
    {
        xMovement = Mathf.Abs(Input.GetAxisRaw("Horizontal"));

        if (xMovement > 0f)
            anim.SetBool("IsWalking", true);
        else
            anim.SetBool("IsWalking", false);

        Debug.Log(xMovement);
    }
}
