using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour {

    public int hp_p1;
    public int hp_p2;

    private GameObject heart1_p1;
    private GameObject heart2_p1;
    private GameObject heart3_p1;

    private GameObject heart1_p2;
    private GameObject heart2_p2;
    private GameObject heart3_p2;

    [SerializeField]
    private Sprite filled;
    [SerializeField]
    private Sprite empty;

    // Use this for initialization
    void Start()
    {
        hp_p1 = 3;
        hp_p2 = 3;

        heart1_p1 = GameObject.Find("heart1");
        heart2_p1 = GameObject.Find("heart2");
        heart3_p1 = GameObject.Find("heart3");

        heart1_p2 = GameObject.Find("heart4");
        heart2_p2 = GameObject.Find("heart5");
        heart3_p2 = GameObject.Find("heart6");
    }

    public void UpdateHealthUI(int player, int remHealth)
    {
        if(player == 1)
        {
            hp_p1--;

            //Update UI
            if (remHealth == 2)
            {
                heart3_p1.GetComponent<Image>().sprite = empty;
            }
            else if (remHealth == 1)
            {
                heart2_p1.GetComponent<Image>().sprite = empty;
            }
            else if (remHealth == 0)
            {
                heart1_p1.GetComponent<Image>().sprite = empty;
            }
        }
    }
}
