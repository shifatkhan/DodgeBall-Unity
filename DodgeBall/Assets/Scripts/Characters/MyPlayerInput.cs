using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MyPlayer))]
public class MyPlayerInput : MonoBehaviour {

	private MyPlayer player;
	private Vector2 input;
    private AimController aimController;

    public AudioSource aimingSound;

	// Use this for initialization
	void Start () {
		player = GetComponent<MyPlayer> ();
        aimController = transform.GetChild(0).GetComponent<AimController>();
        player.playerSoundController = GetComponent<MyPlayerSoundController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!player.ballCatching) {

            if (player.playerID == 1) {
                //Move
                input = new Vector2(Input.GetAxisRaw("Horizontal_P1"), Input.GetAxisRaw("Vertical_P1"));
                player.SetDirectionalInput(input);

                //Jump
                if (Input.GetButtonDown("Jump_P1"))
                {
                    player.OnJumpInputDown();
                }

                if (Input.GetButtonUp("Jump_P1"))
                {
                    player.OnJumpInputUp();
                }

                //dash
                if (Input.GetButtonDown("Dash_P1")) {
                    player.OnDashInputDown();
                }
            }
            else if(player.playerID == 2)
            {
                input = new Vector2(Input.GetAxisRaw("Horizontal_P2"), Input.GetAxisRaw("Vertical_P2"));
                player.SetDirectionalInput(input);

                if (Input.GetButtonDown("Jump_P2"))
                {
                    player.OnJumpInputDown();
                }
                if (Input.GetButtonUp("Jump_P2"))
                {
                    player.OnJumpInputUp();
                }

                //dash
                if (Input.GetButtonDown("Dash_P2"))
                {
                    player.OnDashInputDown();
                }
            }
        }
        if (player.PlayerID == 1)
        {
            // Catch the ball.
            if (Input.GetButtonDown("ThrowCatch_P1") && player.ballCatching)
            {
                Debug.Log("Player 1 caught the ball");
                player.CatchTheBall();
            }

            // Aim the ball.
            if (Input.GetButton("ThrowCatch_P1") && player.ballCaught && !player.ballCatching && player.canThrow)
            {
                aimController.MakeSpriteVisible(true);
                aimController.aiming = true;

                player.animator.SetBool("aiming", aimController.aiming);
                player.animator.Play("aiming");
            }

            // Throw the ball at the aimed direction.
            if (Input.GetButtonUp("ThrowCatch_P1") && player.ballCaught && !player.ballCatching)
            {
                player.canThrow = true;

                if (aimController.aiming)
                {
                    player.throwBall();
                    
                    aimController.aiming = false;
                    aimController.MakeSpriteVisible(false);
                    player.animator.SetBool("aiming", aimController.aiming);
                }
            }
        }
        else if(player.PlayerID == 2)
        {
            // Catch the ball.
            if (Input.GetButtonDown("ThrowCatch_P2") && player.ballCatching)
            {
                Debug.Log("Player 2 caught the ball");
                player.CatchTheBall();
            }

            // Aim the ball.
            if (Input.GetButton("ThrowCatch_P2") && player.ballCaught && !player.ballCatching && player.canThrow)
            {
                aimController.MakeSpriteVisible(true);
                aimController.aiming = true;

                player.animator.SetBool("aiming", aimController.aiming);
                player.animator.Play("aiming");
            }

            // Throw the ball at the aimed direction.
            if (Input.GetButtonUp("ThrowCatch_P2") && player.ballCaught && !player.ballCatching)
            {
                player.canThrow = true;

                if (aimController.aiming)
                {
                    player.throwBall();

                    aimController.aiming = false;
                    aimController.MakeSpriteVisible(false);
                    player.animator.SetBool("aiming", aimController.aiming);
                }
            }
        }
    }
}
