using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BgMusicController : MonoBehaviour {

	public AudioSource titleIntro;
	public AudioSource titleLoop;

    public AudioSource gameIntro;
	public AudioSource gameLoop;

    public AudioSource tenseIntro;
    public AudioSource tenseLoop;

	private void Awake() {
		// Check if there was already a background music. If so, destroy it
		// to remove duplicate background music.
		GameObject[] objs = GameObject.FindGameObjectsWithTag ("Music");
		if (objs.Length > 1)
			Destroy (this.gameObject);

		// Keep the music playing after pressing "PLAY" and changing to 
		// the Game scene.
		DontDestroyOnLoad (transform.gameObject);
	}

    // Use this for initialization
    private void Start () {
        
	}

    private void OnEnable() {
		// Tell our 'OnLevelFinishedLoading' function to start listening
		// for a scene change as soon as this script is enabled.
		SceneManager.sceneLoaded += OnSceneChange;
	}

    private void OnDisable() {
		//Tell our 'OnLevelFinishedLoading' function to stop listening 
		// for a scene change as soon as this script is disabled. 
		// Remember to always have an unsubscription for every delegate you subscribe to!
		SceneManager.sceneLoaded -= OnSceneChange;
	}

    /// <summary>
    /// Speeds up the background music.
    /// </summary>
    public void SpeedUpMusic()
    {
        gameLoop.pitch = 1.05f;
    }

    /// <summary>
    /// Switch music to the title music.
    /// </summary>
    public void PlayTitleMusic()
    {
        if (!titleIntro.isPlaying && !titleLoop.isPlaying)
        {
            gameIntro.Stop();
            gameLoop.Stop();
            tenseIntro.Stop();
            tenseLoop.Stop();

            titleIntro.PlayScheduled(AudioSettings.dspTime + 0.25f);
            titleLoop.PlayScheduled(AudioSettings.dspTime + 0.25f + titleIntro.clip.length);
        }
    }

    /// <summary>
    /// Switch music to the game music.
    /// </summary>
    public void PlayGameMusic()
    {
        if(!gameIntro.isPlaying && !gameLoop.isPlaying)
        {
            titleIntro.Stop();
            titleLoop.Stop();
            tenseIntro.Stop();
            tenseLoop.Stop();

            gameIntro.PlayScheduled(AudioSettings.dspTime + 0.25f);
            gameLoop.PlayScheduled(AudioSettings.dspTime + 0.25f + gameIntro.clip.length);
        }
    }

    /// <summary>
    /// Switch music to a tense music. Played when both players are low health.
    /// </summary>
    public void PlayTenseMusic()
    {
        if (!tenseIntro.isPlaying && !tenseLoop.isPlaying)
        {
            titleIntro.Stop();
            titleLoop.Stop();
            gameIntro.Stop();
            gameLoop.Stop();

            tenseIntro.PlayScheduled(AudioSettings.dspTime + 0.25f);
            tenseLoop.PlayScheduled(AudioSettings.dspTime + 0.25f + tenseIntro.clip.length);
        }
    }

	/// <summary>
	/// Raises the scene change event. Switches music accordingly.
	/// </summary>
	/// <param name="scene">Scene.</param>
	/// <param name="mode">Mode.</param>
	void OnSceneChange(Scene scene, LoadSceneMode mode) {
		if (scene.name.Equals ("WilliamScene")) {
			if (titleIntro.isPlaying) {
                // Play the gameLoop after the intro finishes.
				gameLoop.PlayScheduled (AudioSettings.dspTime + (titleIntro.clip.length - titleIntro.time) - 0.03f);
                titleLoop.Stop ();
			} else {
                // Plays a different intro then the gameLoop. Might change this to playing the gameLoop right off the bat.
                PlayGameMusic();
			}
		}
        else if(scene.name.Equals("Menu"))
        {
            Debug.Log("In OnSceneChange to Menu");
            PlayTitleMusic();
		}
	}
}
