using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimController : MonoBehaviour {

    public Transform aimPoint;
    public float speed = 2f;
    private Vector3 zAxis = new Vector3(0, 0, 1);

    // Use this for initialization
    void Start () {
		if(aimPoint == null)
        {
            aimPoint = this.gameObject.transform.GetChild(0);
        }
	}

    private void FixedUpdate()
    {
        if(transform.rotation.z > 0.70)
        {
            transform.rotation = new Quaternion(0,0,-0.7f,1);
        }

        transform.RotateAround(aimPoint.position, zAxis, speed);
    }
}
