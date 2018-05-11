using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayerSoundController : MonoBehaviour {

    private const string soundFxDirectory = "FX/";

    private AudioClip catchSound, throwSound, jumpSound, dashSound;
    private AudioSource audioSource;

	// Use this for initialization
	void Start () {
        catchSound = Resources.Load<AudioClip>(soundFxDirectory + "catch");
        throwSound = Resources.Load<AudioClip>(soundFxDirectory + "throw");
        jumpSound = Resources.Load<AudioClip>(soundFxDirectory + "jump");
        dashSound = Resources.Load<AudioClip>(soundFxDirectory + "dash");

        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlaySound(string filename)
    {
        switch (filename)
        {
            case "catch":
                audioSource.PlayOneShot(catchSound);
                break;
            case "throw":
                audioSource.PlayOneShot(throwSound);
                break;
            case "jump":
                audioSource.PlayOneShot(jumpSound);
                break;
            case "dash":
                audioSource.PlayOneShot(dashSound);
                break;
            default:
                Debug.LogError("Unexpected player sound fx filename requested.");
                break;
        }
    }
}
