using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour {

    public Animator transitionAnim;

    public string sceneName;

	// Update is called once per frame
	void Update () {
        /*if (Input.GetMouseButtonDown(0))
        {
            StartLoad();
        }*/
	}

    public void StartLoad()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        while (true)
        {
            transitionAnim.SetTrigger("FadeOut");
            yield return new WaitForSeconds(1.5f);
            SceneManager.LoadScene(sceneName);
        }
    }
}
