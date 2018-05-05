using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MyPlayer))]
public class MyPlayerInput : MonoBehaviour {

	private MyPlayer player;
	private Vector2 input;

	// Use this for initialization
	void Start () {
		player = GetComponent<MyPlayer> ();
	}
	
	// Update is called once per frame
	void Update () {
        if (!player.ballTouch) {

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
            }
        }

        // Catch the ball.
        if (Input.GetKeyDown("l") && player.ballTouch)
        {
            Destroy(player.ball);
            player.ballCaught = true;
            player.ballTouch = false;
        }
    }
}
