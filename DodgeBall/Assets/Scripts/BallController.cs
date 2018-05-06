using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{

    private Rigidbody2D rigidBody;

    public float throwForce = 3500f;

    // Used to determine how to bounce off a player.
    public Vector2 throwDirection;

    // Number that tells us if player 1 or player 2 threw the ball.
    // It's equal to -1 if it hit the wall.
    public int throwerId = 2;
    
	void Start ()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        //rigidBody.AddForce(transform.forward * throwForce);
    }

    void FixedUpdate ()
    {
        /*// Test code that controls the ball with the mouse.
		var mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		var mouseDir = mousePos - gameObject.transform.position;
		mouseDir.z = 0.0f;
		mouseDir = mouseDir.normalized;

		if (Input.GetMouseButtonDown (0))
        {
            this.throwDirection = mouseDir * throwForce;
            rigidBody.AddForce(throwDirection);
        }
        */
	}
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Reset the ball's thrower id.
        if (collision.gameObject.tag == "Arena")
        {
            throwerId = -1;
        }
    }
}
