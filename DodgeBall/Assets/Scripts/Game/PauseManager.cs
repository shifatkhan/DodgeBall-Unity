using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour {

    public bool gameIsPaused;

    public GameObject pauseUI;
    public GameObject optionsUI;

    void Start()
    {
        //Make sure game is NOT paused when you start game
        //Fixes issue when going from game to mmenu and back to game which keeps
        //GameIsPaused to true
        gameIsPaused = false;
    }

	// Update is called once per frame
	void Update () {
        
    }

    public void CloseOptionsMenu()
    {
        optionsUI.SetActive(false);
        pauseUI.SetActive(true);
    }

    public void PauseGame()
    {
        pauseUI.SetActive(true);

        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void ResumeGame()
    {
        pauseUI.SetActive(false);

        //Restore normal game speed.
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
