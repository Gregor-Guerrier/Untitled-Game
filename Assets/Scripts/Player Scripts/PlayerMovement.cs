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
    public bool isGrounded;
    float percievedGravity;
    public bool isAiming;
    public float adsSpeedMultiplyer;
    public float shootingSpeedMultiplyer;

    private GameManager gm;
    private Vector2 movement;
    private PlayerManager playerManager;
    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        gm = GameObject.FindObjectOfType<GameManager>();
        percievedGravity = gravity/9.81f;
    }
    // Update is called once per frame
    void Update()
    {
        //Check if the player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        //If the player is grounded then give it gravity
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = percievedGravity;
        }

        //Movement
        if(Input.GetKey(gm.keybindManager.forward.primaryBind) || Input.GetKey(gm.keybindManager.forward.altBind)) {movement.y += .5f;}
        if(Input.GetKey(gm.keybindManager.backward.primaryBind) || Input.GetKey(gm.keybindManager.backward.altBind)) {movement.y -= .5f;}
        if(Input.GetKey(gm.keybindManager.right.primaryBind) || Input.GetKey(gm.keybindManager.right.altBind)) {movement.x += .5f;}
        if(Input.GetKey(gm.keybindManager.left.primaryBind) || Input.GetKey(gm.keybindManager.left.altBind)) {movement.x -= .5f;}
        if(!Input.GetKey(gm.keybindManager.forward.primaryBind) && !Input.GetKey(gm.keybindManager.forward.altBind) && !Input.GetKey(gm.keybindManager.backward.primaryBind) && !Input.GetKey(gm.keybindManager.backward.primaryBind)){movement.y = 0;}
        if(!Input.GetKey(gm.keybindManager.right.primaryBind) && !Input.GetKey(gm.keybindManager.right.altBind) && !Input.GetKey(gm.keybindManager.left.primaryBind) && !Input.GetKey(gm.keybindManager.left.altBind)){movement.x = 0;}
        
        //Restrictor
        if(movement.x > 1){movement.x = 1;}
        if(movement.x < -1){movement.x = -1;}
        if(movement.y > 1){movement.y = 1;}
        if(movement.y < -1){movement.y = -1;}
        Vector3 move = transform.right * movement.x + transform.forward * movement.y;

        //Sprinting
        if(Input.GetKey(gm.keybindManager.sprint.primaryBind) && isAiming == false && playerManager.canSprint == true || Input.GetKey(gm.keybindManager.sprint.altBind) && isAiming == false && playerManager.canSprint == true)
        {
            playerManager.Sprinting();
            playerManager.isSprinting = true;
            controller.Move(move * speed * sprintMultiplier * Time.deltaTime);
        } else if(!Input.GetKey(gm.keybindManager.sprint.primaryBind) && isAiming == false || !Input.GetKey(gm.keybindManager.sprint.altBind) && isAiming == false)
        {
            playerManager.isSprinting = false;
            controller.Move(move * speed * Time.deltaTime);
        } else if(isAiming == true)
        {
            playerManager.isSprinting = false;
            controller.Move(move * speed * adsSpeedMultiplyer * Time.deltaTime);
        }

        if(Input.GetKeyDown(gm.keybindManager.jump.primaryBind) && isGrounded || Input.GetKey(gm.keybindManager.jump.altBind) && isGrounded)
        {
            if(playerManager.stamina - 20 * playerManager.staminaReductionModifier > 0)
            {
                velocity.y = jumpHeight/100;
                playerManager.Jumping();
            }
        } 

        velocity.y += percievedGravity * Time.deltaTime;

        controller.Move(velocity);
    }
}
