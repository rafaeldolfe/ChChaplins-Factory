using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 2;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 input;
    private CharacterController charController;
    public float turning = 1;
    public float gravity = .1f;
    private float fallSpeed = 0f;
    public float jumpForce = 10f;
    public bool isJumping = true;
    public bool gameOver = false;
    private Animator playerAnim;

    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
        playerAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        Fall();
    }

    private void MovePlayer()
    {
        if(!gameOver)
        {
            horizontalInput = -Input.GetAxis("Horizontal");
            verticalInput = -Input.GetAxis("Vertical");

            input = new Vector3(-horizontalInput, 0f, -verticalInput);
            input = input.normalized * Time.deltaTime * speed;

            playerAnim.SetFloat("speed", Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

            CharMovement(input);
            CharRotation(input);
        }
    }

    private void CharMovement(Vector3 input)
    {
        charController.Move(input);
        charController.Move(new Vector3(0, fallSpeed, 0));
    }

    private void CharRotation(Vector3 input)
    {
        if (input != new Vector3(0,0,0))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(input),
                                                  turning);
        }
    }

    // private void VerticalMovement()
    // {
    //     if(!charController.isGrounded && Input.GetKey(KeyCode.Space))
    //     {
    //         fallSpeed += jumpForce;
    //     }
    // }

    private void Fall()
    {
        if(!charController.isGrounded)
        {
            fallSpeed -= gravity * Time.deltaTime;
        }
        else
        {
            fallSpeed = 0;
        }
    }
}
