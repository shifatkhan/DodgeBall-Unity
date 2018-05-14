using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    // Used to determine how to bounce off a player.
    public Vector2 throwDirection;

    public AudioSource bounceSound;

    private Rigidbody2D rb;

    // Number that tells us if player 1 or player 2 threw the ball.
    // It's equal to -1 if it hit the wall.
    public int throwerId = 2;
    public float speedBeforeReset = 4f;

    private void Start()
    {
        bounceSound = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (throwerId != -1 && Mathf.Abs(rb.velocity.x) < speedBeforeReset 
            && Mathf.Abs(rb.velocity.y) < speedBeforeReset)
        {
            throwerId = -1;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Reset the ball's thrower id.
        if (collision.gameObject.tag == "Arena" || collision.gameObject.tag == "Ball")
        {
            throwerId = -1;
            bounceSound.Play();
        }
    }
}
