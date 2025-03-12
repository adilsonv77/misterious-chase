using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accelerometer : MonoBehaviour {
    
    public bool isFlat = true;
    private Rigidbody rigid;

    void Start() {
        rigid = GetComponent<Rigidbody>();
    }

    void Update() {
        Vector3 tilt = Input.acceleration;

        if (isFlat)
        {
            tilt = Quaternion.Euler(90, 0, 0) * tilt;
        }

        rigid.AddForce(tilt);

        Debug.Log(transform.position.x);
        Debug.Log(tilt.x);
    }

}
