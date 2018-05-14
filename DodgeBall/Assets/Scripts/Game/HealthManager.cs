using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{

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
        heart1_p1 = GameObject.Find("heart_1");
        heart2_p1 = GameObject.Find("heart_2");
        heart3_p1 = GameObject.Find("heart_3");

        heart1_p2 = GameObject.Find("heart_4");
        heart2_p2 = GameObject.Find("heart_5");
        heart3_p2 = GameObject.Find("heart_6");
    }

    public void UpdateHealthUI(int p1Health, int p2Health)
    {
        //Update UI
        switch (p1Health)
        {
            case 2:
                heart3_p1.GetComponent<Image>().sprite = empty;
                break;
            case 1:
                heart2_p1.GetComponent<Image>().sprite = empty;
                break;
            case 0:
                heart1_p1.GetComponent<Image>().sprite = empty;
                break;
        }

        switch (p2Health)
        {
            case 2:
                heart3_p2.GetComponent<Image>().sprite = empty;
                break;
            case 1:
                heart2_p2.GetComponent<Image>().sprite = empty;
                break;
            case 0:
                heart1_p2.GetComponent<Image>().sprite = empty;
                break;
        }
    }
}