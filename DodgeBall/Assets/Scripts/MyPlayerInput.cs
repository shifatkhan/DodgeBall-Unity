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
		//Move
		input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		player.SetDirectionalInput (input);

		//jump
		if (Input.GetKeyDown ("j")) {
			player.OnJumpInputDown ();
		}

		if (Input.GetKeyUp ("j")) {
			player.OnJumpInputUp();
		}

		//dash
		if (Input.GetKeyDown ("k")) {
			player.OnDashInputDown ();
		}
	}
}
