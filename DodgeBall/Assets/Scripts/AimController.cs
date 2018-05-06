using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimController : MonoBehaviour {

    public bool aiming = false;
    public Transform aimPoint;
    public float speed = 2f;
    private Vector3 zAxis;
    private Quaternion originalRotation;
    private Vector3 originalPosition;

    private SpriteRenderer sprite;

    private int playerID;

    // Use this for initialization
    void Start () {
		if(aimPoint == null)
        {
            aimPoint = this.gameObject.transform.GetChild(0);
        }

        originalRotation = transform.rotation;
        originalPosition = transform.localPosition;
        sprite = GetComponent<SpriteRenderer>();

        playerID = transform.parent.GetComponent<MyPlayer>().PlayerID;

        if(playerID == 1)
        {
            zAxis = new Vector3(0, 0, 1);
        }
        else
        {
            zAxis = new Vector3(0, 0, -1);
        }
	}

    private void FixedUpdate()
    {
        if (aiming)
        {
            Debug.Log("z: " + transform.localRotation.z);
            if (playerID == 1 && transform.localRotation.z > 0.45)
            {
                ResetArrow();
            }
            else if(playerID == 2 && transform.localRotation.z > -0.90)
            {
                ResetArrow();
            }

            transform.RotateAround(aimPoint.position, zAxis, speed);
        }
    }

    public void MakeSpriteVisible(bool visible)
    {
        if (!visible)
        {
            ResetArrow();
        }
        sprite.enabled = visible;
    }

    private void ResetArrow()
    {
        transform.localPosition = originalPosition;//new Vector3(0.833f, -0.588f, 0);
        transform.localRotation = originalRotation;//new Quaternion(0, 0, -0.45f, 1);
    }
}
