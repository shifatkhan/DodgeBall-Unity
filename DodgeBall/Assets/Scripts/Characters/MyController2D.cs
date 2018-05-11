using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyController2D : MyRaycastController {

	private float maxClimbAngle = 60f;
	private float maxDescendAngle = 60f;

	public CollisionInfo collisions;

	public Vector2 playerInput;

	public int currentSide = 1;

	// Use this for initialization
	void Start () {
		base.Start ();
		collisions.faceDir = 1;
	}
	
	public void Move(Vector2 moveAmount, bool standingOnMovingPlatform = false)
	{
		Move (moveAmount, Vector2.zero, standingOnMovingPlatform);
	}

	public void Move(Vector2 moveAmount, Vector2 input, bool standingOnMovingPlatform = false)
	{
		UpdateRaycastOrigins ();
		collisions.Reset ();
		collisions.moveAmountOld = moveAmount;
		playerInput = input;

		//Determine facing direction
		if (moveAmount.x != 0) {
			collisions.faceDir = (int)Mathf.Sign (moveAmount.x);
		}

		if (moveAmount.y < 0) {
			DescendSlope (ref moveAmount);
		}

		CheckHorizontalCollisions(ref moveAmount);

		if (moveAmount.y != 0) {
			CheckVerticalCollisions(ref moveAmount);
		}

		transform.Translate (moveAmount);

		//Debug.Log ("standing: " + standingOnMovingPlatform);

		if (standingOnMovingPlatform) {
			print ("STANDING");
			collisions.below = true;
		}
	}

	private void CheckHorizontalCollisions(ref Vector2 moveAmount)
	{
		float directionX = collisions.faceDir;
		float rayLength = Mathf.Abs (moveAmount.x) + skinWidth;
		Vector2 rayDirection = Vector2.right * directionX;

		if (Mathf.Abs (moveAmount.x) < skinWidth) {
			rayLength = 2 * skinWidth;
		}

		for (int i = 0; i < horizontalRayCount; i++) {
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, rayDirection, rayLength, collisionMask);

			Debug.DrawRay (rayOrigin, rayDirection, Color.red);

			if (hit) {
				if (hit.distance == 0) {
					continue;
				}

				//Handle slope climb
				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
				if (i == 0 && slopeAngle <= maxClimbAngle) {
					//When going down a slope and immediately going up on another one.
					if (collisions.descendingSlope) {
						collisions.descendingSlope = false;
						moveAmount = collisions.moveAmountOld;
					}

					float distanceToSlopeStart = 0f;

					//When we collide with another slope of different angle;
					if (slopeAngle != collisions.slopeAngleOld) {
						distanceToSlopeStart = hit.distance - skinWidth;
						moveAmount.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope (ref moveAmount, slopeAngle);
					moveAmount.x += distanceToSlopeStart * directionX;
				}

				//If not climbing a slope, if hitting something like a wall.
				if (!collisions.climbingSlope || slopeAngle > maxClimbAngle) {
					moveAmount.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					//Calculate new velocity y when climbing new slope
					if (collisions.climbingSlope) {
						moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
					}

					//set which side has collided
					collisions.left = (directionX == -1);
					collisions.right = (directionX == 1);
				}
			}
		}
	}

	private void ClimbSlope(ref Vector2 moveAmount, float slopeAngle)
	{
		float moveDistance = Mathf.Abs (moveAmount.x);
		float climbMoveAmountY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (moveAmount.y <= climbMoveAmountY) {
			moveAmount.y = climbMoveAmountY;
			moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		}
	}

	private void DescendSlope(ref Vector2 moveAmount)
	{
		float directionX = Mathf.Sign (moveAmount.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if (hit) {
			float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
			if (slopeAngle != 0 && slopeAngle <= maxDescendAngle) 
			{
				if (Mathf.Sign (hit.normal.x) == directionX) 
				{
					if (hit.distance - skinWidth <= Mathf.Tan (slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (moveAmount.x)) 
					{
						float moveDistance = Mathf.Abs (moveAmount.x);
						float descendMoveAmountY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
						moveAmount.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (moveAmount.x);
						moveAmount.y -= descendMoveAmountY;

						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
					}
				}
			}
		}
	}

	private void CheckVerticalCollisions(ref Vector2 moveAmount)
	{
		float directionY = Mathf.Sign (moveAmount.y);
		float rayLength = Mathf.Abs (moveAmount.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++) {
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			Debug.DrawRay (rayOrigin, Vector2.up * directionY, Color.red);

			if (hit) {
				//Allows jumping through platforms
				if (hit.collider.tag == "Through") {
					if (directionY == -1) {
						collisions.standingOnPlatform = true;
						Debug.Log ("Standing on platform");
					}
					if (directionY == 1 || hit.distance == 0) {
						Debug.Log ("Jumping through platform");
						continue;
					}
					if (collisions.fallingThroughPlatform) {
						Debug.Log ("Going through platform");
						Invoke ("ResetFallingThroughPlatform", 0.1f);
						continue;
					}
				}

				//restrict vertical movement to distance until collision
				moveAmount.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				//calculates the horizontal movement when going up a slope. using trigonometry
				if (collisions.climbingSlope) {
					moveAmount.x = moveAmount.y / Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign (moveAmount.x);
				}

				collisions.below = (directionY == -1);
				collisions.above = (directionY == 1);
			}
		}

		//if we're currently climbing a slope and encounter another slope of different angle
		//change the angle.
		if (collisions.climbingSlope)
		{
			float directionX = Mathf.Sign(moveAmount.x);
			rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			if (hit)
			{
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				if (slopeAngle != collisions.slopeAngle)
				{
					moveAmount.x = (hit.distance * skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
				}
			}
		}
	}

	private void ResetFallingThroughPlatform(){
		collisions.fallingThroughPlatform = false;
		Debug.Log ("Reset fall through");
	}

	public struct CollisionInfo
	{
		public bool above, below;
		public bool left, right;

		public bool climbingSlope;
		public bool descendingSlope;
		public float slopeAngle, slopeAngleOld;
		public Vector2 moveAmountOld;
		public int faceDir;
		public bool fallingThroughPlatform;
		public bool standingOnPlatform;

		public void Reset()
		{
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0f;
		}
	}
}
