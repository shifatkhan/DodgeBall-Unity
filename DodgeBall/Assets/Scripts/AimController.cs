using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimController : MonoBehaviour {

    public bool aiming = false;
    public Transform aimPoint;
    public float speed = 2f;
    private Vector3 zAxis = new Vector3(0, 0, 1);
    private Vector3 originalRotation;

    private SpriteRenderer sprite;

    // Use this for initialization
    void Start () {
		if(aimPoint == null)
        {
            aimPoint = this.gameObject.transform.GetChild(0);
        }

        originalRotation = transform.rotation.eulerAngles;
        sprite = GetComponent<SpriteRenderer>();
	}

    private void FixedUpdate()
    {
        if (aiming)
        {
            if (transform.localRotation.z > 0.45)
            {
                transform.localPosition = new Vector3(0.833f, -0.588f, 0);
                transform.localRotation = new Quaternion(0, 0, -0.45f, 1);
            }

            transform.RotateAround(aimPoint.position, zAxis, speed);
        }
    }

    public void MakeSpriteVisible(bool visible)
    {
        if (!visible)
        {
            transform.localPosition = new Vector3(0.833f, -0.588f, 0);
            transform.localRotation = new Quaternion(0, 0, -0.45f, 1);
        }
        sprite.enabled = visible;
    }
}
