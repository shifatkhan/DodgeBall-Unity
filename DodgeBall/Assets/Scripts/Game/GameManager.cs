using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public MyPlayer p1;
    public MyPlayer p2;

    public GameObject healthScript;
    public GameObject pauseScript;
    public GameObject gameoverScript;

    public GameObject gameoverScreen;

    private BgMusicController bgMusicController;

    private bool gameIsPaused;
    private bool gameIsOver;
    private bool pauseDisabled;

    private bool musicTensedUp;

    void Awake()
    {
        SceneManager.sceneUnloaded += UnPauseGame;
    }

    // Use this for initialization
    void Start () {
        Time.timeScale = 1f;
        gameIsOver = false;
        gameIsPaused = false;
        pauseDisabled = false;
        musicTensedUp = false;
        bgMusicController = GameObject.Find("BGMusic").GetComponent<BgMusicController>();
        bgMusicController.PlayGameMusic();
	}

    // Update is called once per frame
    void Update() {

        //Check Pause
        CheckForPause();

        //Check health
        CheckHealth();
        
	}

    private void CheckForPause()
    {
        //Fixes thing where user can still bring up pause screen on gameover screen
        if (!pauseDisabled)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!gameIsPaused)
                {
                    pauseScript.GetComponent<PauseManager>().PauseGame();
                }
                else
                {
                    pauseScript.GetComponent<PauseManager>().ResumeGame();
                }
            }
        }
    }

    private void CheckHealth()
    {
        //Updates the heart ui 
        healthScript.gameObject.GetComponent<HealthManager>().UpdateHealthUI(p1.health, p2.health);

        if(!musicTensedUp && (p1.health == 1 || p2.health == 1))
        {
            musicTensedUp = true;
            bgMusicController.PlayTenseMusic();
        }
        else if (p1.health == 0 || p2.health == 0)
        {
            //If health is 0, game ends
            pauseDisabled = true;
            int winner = (p2.health == 0) ? 1 : 2;
            StartCoroutine(FinishGame(winner));
        }
    }

    private void EndGame(int winner)
    {
        //end game
        //Time.timeScale = 0f;
        gameoverScreen.SetActive(true);

        //Send winning info
        gameoverScript.gameObject.GetComponent<GameOverManager>().ShowGameOverInfo(winner);
    }

    IEnumerator FinishGame(int winner)
    {
        yield return new WaitForSeconds(2f);

        if (!gameIsOver)
        {
            Time.timeScale = 0f;

            gameoverScreen.SetActive(true);
            gameoverScript.gameObject.GetComponent<GameOverManager>().ShowGameOverInfo(winner);
            gameIsOver = true;
        }
        
        yield return 0;
    }

    /// <summary>
    /// Un-freeze the game so the other scenes are not frozen.
    /// </summary>
    /// <typeparam name="Scene"></typeparam>
    /// <param name="scene"></param>
    void UnPauseGame<Scene> (Scene scene)
    {
        Time.timeScale = 1f;
    }
}
