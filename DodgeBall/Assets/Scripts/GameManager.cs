using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public MyPlayer p1;
    public MyPlayer p2;

    public GameObject HealthManagerScript;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        HealthManagerScript.gameObject.GetComponent<HealthManager>().UpdateHealthUI(p1.health, p2.health);
        if (p1.health == 0 || p2.health == 0)
        {
            EndGame();
        }
	}

    void EndGame()
    {
        //end game
        print("done");
    }
}
