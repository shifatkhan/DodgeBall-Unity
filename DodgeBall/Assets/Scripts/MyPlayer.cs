using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MyController2D))]
[RequireComponent(typeof(MyPlayerInput))]
public class MyPlayer : MonoBehaviour
{

    public float maxJumpHeight = 2.5f;
    public float minJumpHeight = 1f;
    public float timeToJumpApex = .32f;
    private float accelerationTimeAirborne = .2f;
    private float accelerationTimeGrounded = .1f;
    public float moveSpeed = 7f;

    public float gravity;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    public Vector3 velocity;
    private float velocityXSmoothing;
    public bool isDoubleJumping;

    public bool isDashing;
    public float dashDistance = 21f;
    private float timeToReachDashVelocity = .8f;

    public bool isWallSliding;
    public bool isWallJumping;
    public int wallSlideDirection; // sliding left or right
    public float wallSlideSpeed = 2f;
    public Vector2 wallJumpDistance;

    public Vector2 directionalInput;

    private MyController2D rc;
    private Animator animator;
    private SpriteRenderer render;

    // Use this for initialization
    void Start()
    {
        rc = GetComponent<MyController2D>();
        animator = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();

        //Kinematic formula, solving for acceleration.
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        wallJumpDistance = new Vector2(7, 15);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CalculateVelocity();

        rc.Move(velocity * Time.deltaTime, directionalInput);
        flipSprite();

        checkState();
    }

    private void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (rc.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne));
        velocity.y += gravity * Time.deltaTime;

        if (Mathf.Abs(velocity.x) < 0.1)
        {
            velocity.x = 0;
        }
    }

    private void checkState()
    {
        //Prevent velocity from accumulating when standing on platform or hitting a ceiling
        if (rc.collisions.above || rc.collisions.below)
        {
            velocity.y = 0f;
        }

        //When airborne and player collides horizontally with/without a dash, reset velocity to 0; prevents bouncing
        if ((rc.collisions.left || rc.collisions.right) && !rc.collisions.below && !isDashing)
        {
            velocity.x = (rc.collisions.faceDir == 1) ? 0 : -0.001f;
        }

        //Check if should wall slide
        CheckWallSlide();

        CheckGrounded();
    }

    private void CheckWallSlide()
    {
        wallSlideDirection = (rc.collisions.left) ? -1 : 1;
        isWallSliding = false;
        if ((rc.collisions.left || rc.collisions.right) && !rc.collisions.below && velocity.y < 0 && directionalInput.x == wallSlideDirection)
        {
            isWallSliding = true;
            isDoubleJumping = false;
            //slows down descent when sliding on wall
            if (velocity.y < -wallSlideSpeed)
            {
                velocity.y = -wallSlideSpeed;
                Debug.Log("Wall Slide");
            }
        }
    }

    private void CheckGrounded()
    {
        //When landing, reset double jump
        if (rc.collisions.below)
        {
            animator.SetBool("grounded", true);
            isDoubleJumping = false;
        }
        else
        {
            animator.SetBool("grounded", false);
            //If in any situation where player isn't grounded, then he is not standing in a platform;
            rc.collisions.standingOnPlatform = false;
        }
    }

    private void flipSprite()
    {
        bool flipSprite = render.flipX ? (directionalInput.x > 0) : (directionalInput.x < 0);
        if (flipSprite)
        {
            render.flipX = !render.flipX;
        }
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {

        if (isWallSliding)
        {
            velocity.x = -wallSlideDirection * wallJumpDistance.x;
            velocity.y = wallJumpDistance.y;
            isDoubleJumping = false;
            isWallJumping = true;
            Invoke("resetWallJumping", 0.2f);
            Debug.Log("Wall Jump");
        }

        //Going down through platforms
        if (rc.collisions.standingOnPlatform && directionalInput.y == -1)
        {
            rc.collisions.fallingThroughPlatform = true;
            Debug.Log("Going down platform");
        }
        else if (rc.collisions.below)
        {
            Debug.Log("Jump");
            velocity.y = maxJumpVelocity;
            isDoubleJumping = false;
        }

        //Double jump
        if (!isDoubleJumping && !rc.collisions.below && !isWallSliding && !rc.collisions.standingOnPlatform)
        {
            velocity.y = maxJumpVelocity * 0.75f;
            isDoubleJumping = true;
            Debug.Log("Double jump");
        }
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
            Debug.Log("Short Jump");
        }
    }

    public void OnDashInputDown()
    {
        if (!isDashing && rc.collisions.below)
        {
            velocity.x = (rc.collisions.faceDir * dashDistance) / timeToReachDashVelocity;
            isDashing = true;
            Debug.Log("Dash");
        }
    }

    private void resetWallJumping()
    {
        isWallJumping = false;
    }

}
