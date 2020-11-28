using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController2D controller;
    float horizontalMove = 0f;
    float verticalMove = 0f;
    private float runSpeed = 40f;
    private bool jump = false, crouch = false;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        animator.SetFloat("speed", Mathf.Abs(horizontalMove));

        if(Input.GetButton("Jump"))
        {
            jump = true;
            animator.SetBool("jump", true);
        }
        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
            animator.SetBool("crouch", true);
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
            animator.SetBool("crouch", false);
        }

        verticalMove = Input.GetAxisRaw("Vertical");

    }

    public void OnLanding()
    {
        animator.SetBool("jump", false);
    }

    /*
    public void OnCrouching(bool crouch)
    {
        animator.SetBool("crouch", crouch);
    }*/

    private void FixedUpdate()
    {
        //Move our character.
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        jump = false;

    }
}
