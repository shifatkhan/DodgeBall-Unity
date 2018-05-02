using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{

    private Rigidbody2D rigidBody;

    public float throwForce = 3500f;
    
	void Start ()
    {
        rigidBody = GetComponent<Rigidbody2D>();

    }

	void FixedUpdate ()
    {
		var mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		var mouseDir = mousePos - gameObject.transform.position;
		mouseDir.z = 0.0f;
		mouseDir = mouseDir.normalized;

		if (Input.GetMouseButtonDown (0))
        {
            rigidBody.AddForce(mouseDir * throwForce);
		}
	}

    /// <summary>
    /// This method should make it so the ball doesn't interact with the MiddleWall.
    /// However, it collides with it the first time, and then it's fine. We want it
    /// to never collide (not even on the first time).
    /// 
    /// This method has been replaced by:
    /// Edit > Project Settings > Physics2D > Layer Collision Matrix
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MiddleWall")
        {
            Debug.Log("Ball collided with MiddleWall");
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }
}
