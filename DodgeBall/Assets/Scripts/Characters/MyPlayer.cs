using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MyController2D))]
[RequireComponent(typeof(MyPlayerInput))]
public class MyPlayer : MonoBehaviour
{
    // Identity for player 1 or player 2.
    public int playerID = 1;
    public int health = 3;
    // Delta time in which the player must catch the ball.
    public readonly float CATCH_TIME = 0.5f;
    // Ball Catching is when the ball initially touches the player.
    // Ball caught depicts whether the player is holding the ball or not.
    public bool ballCatching = false;
    public bool ballCaught = false;
    public bool canThrow = false;
    public float ballHitPushBack = 4f;
    public float throwForce = 4000f;
    public GameObject ball = null;
    public GameObject ballPrefab;
    private Vector2 ballBounceDirection;
    
    public bool invincible = false;
    public float invulnerableTime = 2f;

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
    public float timeToReachDashVelocity = .05f;

    public bool isWallSliding;
    public bool isWallJumping;
    public int wallSlideDirection; // sliding left or right
    public float wallSlideSpeed = 2f;
    public Vector2 wallJumpDistance;
    
    public Vector2 directionalInput;

    private MyController2D rc;
    public Animator animator;
    private SpriteRenderer render;

    public MyPlayerSoundController playerSoundController;

    public AudioSource footstepsSound;

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

    private void Update()
    {
        if (rc.collisions.below && Mathf.Abs(velocity.x) > 0.5f && !footstepsSound.isPlaying)
        {
            footstepsSound.enabled = true;
            footstepsSound.loop = true;
            animator.SetBool("running", true);
        }
        else if((!rc.collisions.below || Mathf.Abs(velocity.x) < 0.5f) && footstepsSound.isPlaying)
        {
            footstepsSound.enabled = false;
            footstepsSound.loop = false;
            animator.SetBool("running", false);
        }
    }

    void FixedUpdate()
    {
        if (!ballCatching)
        {
            CalculateVelocity();
        }

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

        //Apparantly, Color isn't something you can modify like transform.position
        //Reduce transparency by half when hurt.
        Color c = render.color;
        if (invincible)
        {
            c.a = 0.5f;
        }
        else
        {
            c.a = 1f;
        }
        render.color = c;

        //Check if should wall slide
        CheckWallSlide();

        CheckGrounded();
    }

    private void CheckWallSlide()
    {
        wallSlideDirection = (rc.collisions.left) ? -1 : 1;
        isWallSliding = false;
        animator.SetBool("wallSliding", isWallSliding);

        if ((rc.collisions.left || rc.collisions.right) && !rc.collisions.below && velocity.y < 0 && directionalInput.x == wallSlideDirection)
        {
            isWallSliding = true;
            animator.SetBool("wallSliding", isWallSliding);
            
            if (ballCaught)
            {
                animator.Play("wallSliding_wBall");

            }
            else
            {
                animator.Play("wallSliding");
            }
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
        if (rc.collisions.below || isWallSliding)
        {
            //animator.SetBool("grounded", true);
            isDoubleJumping = false;
            animator.SetBool("jumping", false);
            animator.SetBool("falling", false);
        }
        else if (!rc.collisions.below && velocity.y < 0)
        {
            animator.SetBool("jumping", false);
            animator.SetBool("falling", true);
        }
        else
        {
            //animator.SetBool("grounded", false);
            //If in any situation where player isn't grounded, then he is not standing in a platform;
            rc.collisions.standingOnPlatform = false;
            animator.SetBool("jumping", true);
            animator.SetBool("falling", false);
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
            Invoke("ResetWallJumping", 0.2f);
            Debug.Log("Wall Jump");
            playerSoundController.PlaySound("jump");
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
            playerSoundController.PlaySound("jump");
        }

        //Double jump
        if (!isDoubleJumping && !rc.collisions.below && !isWallSliding && !rc.collisions.standingOnPlatform)
        {
            velocity.y = maxJumpVelocity * 0.75f;
            isDoubleJumping = true;
            Debug.Log("Double jump");
            playerSoundController.PlaySound("jump");

            if (ballCaught)
                animator.Play("jumping_wBall", -1, 0f);
            else
                animator.Play("jumping", -1, 0f);
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
        if (!isDashing)
        {
            //if you're not holding any direction while dashign, default to facing direction.
            //Ternary operator to avoid dashing in wrong direction if you don't hold left/right
            float xdir = (directionalInput.x == 0 && directionalInput.y == 0) ? rc.collisions.faceDir : directionalInput.x;            
            
            //keeps direction so that you can't change direction while dashing
            Vector2 dir = new Vector2(xdir, directionalInput.y);

            playerSoundController.PlaySound("dash");

            StartCoroutine("Dashing", dir);
        }
    }

    private void ResetDashing()
    {
        isDashing = false;
    }
    private void ResetWallJumping()
    {
        isWallJumping = false;
    }

    /// <summary>
	/// Resets the invincble boolean. Used by DetermineIfBallNotCaught, to return player to vulnerable state 
	/// after slight moment of invincibility.
	/// </summary>
	private void ResetInvincible()
    {
        this.invincible = false;
    }

    private void DetermineIfBallNotCaught()
    {
        if (!ballCaught && ballCatching)
        {
            
            ReceiveDamage();
        }

        this.ballCatching = false;
        animator.SetBool("catching", ballCatching);
    }

    public void ReceiveDamage()
    {
        // Player got hit (didn't catch the ball).
        Debug.Log("Player " + this.playerID + " got HIT!");

        //Reduce their health points.
        health--;

        // Bounce the ball off the player appropriately
        this.ballBounceDirection = this.ball.GetComponent<BallController>().throwDirection;
        this.ballBounceDirection.x *= -1;
        this.ball.GetComponent<Rigidbody2D>().AddForce(this.ballBounceDirection);

        // Reset the ball's thrower ID
        this.ball.GetComponent<BallController>().throwerId = -1;

        switch (playerID)
        {
            case 1:
                velocity.x = -ballHitPushBack;
                break;
            case 2:
                velocity.x = ballHitPushBack;
                break;
            default:
                Debug.LogError("Hit player with unknown playerID!");
                break;
        }

        this.ball.GetComponent<BallController>().playerIsCatchingTheBall = false;
        this.ball = null;

        //Become invulnerable for 2 seconds
        invincible = true;
        
        Invoke("ResetInvincible", invulnerableTime);

        //Play animation of getting hit
        if (health > 0)
        {
            if (ballCaught)
            {
                animator.Play("hit_wBall");
            }
            else
            {
                animator.Play("hit");
            }
        }
        else
        {
            animator.Play("death");
        }

        
    }

    public void throwBall()
    {
        if (ballCaught && !ballCatching)
        {
           
            this.ballPrefab.GetComponent<BallController>().throwerId = this.playerID;

            // Instantiate the ball with the rotation of the Aim. The first child object is the Aim gameObject.
            GameObject clone = Instantiate(this.ballPrefab, transform.GetChild(0).position, transform.GetChild(0).rotation);

            // Remember initial throw direction.
            clone.GetComponent<BallController>().throwDirection = clone.transform.right * throwForce;

            // Give the ball a force to simulate a throw.
            clone.GetComponent<Rigidbody2D>().AddForce(clone.transform.right * throwForce);
            
            canThrow = false;
            ballCaught = false;
            animator.SetBool("ballCaught", ballCaught);

            animator.Play("throwing");
            playerSoundController.PlaySound("throw");
        }
    }

    public void CatchTheBall()
    {
        Destroy(this.ball);

        ballCaught = true;
        animator.SetBool("ballCaught", ballCaught);
        animator.Play("catching");

        ballCatching = false;
        animator.SetBool("catching", ballCatching);
        playerSoundController.PlaySound("catch");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            // Collided with a ball.

            if(collision.gameObject.GetComponent<BallController>().throwerId != this.playerID
            && collision.gameObject.GetComponent<BallController>().throwerId != -1)
            {
                // The ball is thrown directly from the enemy.

                if (invincible)
                {
                    collision.gameObject.GetComponent<BallController>().throwerId = -1;
                }
                else if (!invincible && !ballCatching)
                {
                    if (!ballCaught)
                    {
                        // Player got hit by other player.

                        this.ball = collision.gameObject;
                        this.ball.GetComponent<BallController>().playerIsCatchingTheBall = true;

                        // Stop movement of the ball.
                        this.ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                        // Stop movement of the player.
                        this.velocity = Vector2.zero;
                        SetDirectionalInput(Vector2.zero);

                        this.ballCatching = true;
                        animator.SetBool("catching", ballCatching);
                        animator.Play("catching");
                        Invoke("DetermineIfBallNotCaught", CATCH_TIME);
                    }
                    else
                    {
                        // Player got hit by other player while holding a ball.

                        this.ball = collision.gameObject;

                        // Stop movement of the ball.
                        this.ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                        // Stop movement of the player.
                        this.velocity = Vector2.zero;
                        SetDirectionalInput(Vector2.zero);

                        animator.Play("hit_wBall");
                        ReceiveDamage();
                    }

                }
            }
            else if (collision.gameObject.GetComponent<BallController>().throwerId == -1 && !ballCaught && !ballCatching)
            {
                this.ball = collision.gameObject;
                CatchTheBall();
                canThrow = true;
            }
        }
    }

    /*
     * 
     * PROPERTIES
     * 
     * 
     * */

    public int PlayerID
    {
        get
        {
            return this.playerID;
        }

        set
        {
            if (value < 1 || value > 2)
            {
                throw new System.Exception("Player's ID must be within 1 or 2.");
            }
            this.playerID = value;
        }
    }

    
    /**
     * 
     * COROUTINES
     * 
     */
    IEnumerator Dashing(Vector2 direction)
    {
        float targetX = (dashDistance * direction.x) / timeToReachDashVelocity;
        float targetY = (dashDistance * direction.y) / timeToReachDashVelocity;

        if(Mathf.Abs(direction.x) > 0 && Mathf.Abs(direction.y) > 0)
        {
            targetX /= 1.50f;
            targetY /= 1.25f;
        }

        isDashing = true;
        float timer = 0; //Timer to check duration of dash
        
        PlayDashingAnimation(direction);
        while (0.2f > timer) //how long to dash for i.e dashing for 0.1 sec
        {
           
            timer += Time.deltaTime;
            velocity.x = targetX;
            velocity.y = targetY;
            yield return 0; //go to next frame
        }
        

        yield return new WaitForSeconds(1f); //cooldown for dashing
        isDashing = false;
    }

    private void PlayDashingAnimation(Vector2 direction)
    {
        string wBall = "";

        if (ballCaught)
        {
            wBall = "_wBall";
        }
        //Dash Straight up
        if(direction.x == 0 && direction.y > 0)
        {
            animator.Play("dash_up" + wBall);
        }

        //Dash diagonal up
        else if(direction.x != 0 && direction.y > 0)
        {
            animator.Play("dash_diag_up" + wBall);
        }

        //Dash diagonal down
        else if (direction.x != 0 && direction.y < 0 && !rc.collisions.below)
        {
            animator.Play("dash_diag_down" + wBall);
        }

        //Straight down
        else if (direction.x == 0 && direction.y < 0 && !rc.collisions.below)
        {
            animator.Play("dash_down" + wBall);
        }

        //Dash straight ahead
        else if (direction.x != 0 && direction.y == 0)
        {
            if (rc.collisions.below)
            {
                animator.Play("dash_ground" + wBall);
            }
            else
            {
                animator.Play("dash_air" + wBall);
            }
        }
    }
}
