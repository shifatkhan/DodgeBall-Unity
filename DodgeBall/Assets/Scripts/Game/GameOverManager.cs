using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour {

    public GameObject gameoverUI;
    private Text winnerText;


    public void ShowGameOverInfo(int winner)
    {
        Text winnerText = GameObject.Find("PlayerWonText").GetComponent<Text>();
        winnerText.text = "Player " + winner + " won!";
    }

    public void PlayAgain()
    {
        //Reload current scene to play again
        Scene currentLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentLevel.buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
