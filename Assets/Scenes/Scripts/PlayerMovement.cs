using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    [Header("Variables")]
    public float speed = 10f;
    public float sprintMultiplier = 1.2f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;
    float percievedGravity;
    void Start()
    {
        percievedGravity = gravity/9.81f;
    }
    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = percievedGravity;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if(Input.GetKey(KeyCode.LeftShift)){
            controller.Move(move * speed * sprintMultiplier * Time.deltaTime);
        } else if(!Input.GetKey(KeyCode.LeftShift)){
            controller.Move(move * speed * Time.deltaTime);
        }

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = jumpHeight/100;
        }

        velocity.y += percievedGravity * Time.deltaTime;

        controller.Move(velocity);
    }
}
