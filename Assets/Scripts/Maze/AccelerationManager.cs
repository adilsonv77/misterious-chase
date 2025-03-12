using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationManager : MonoBehaviour
{
    private Vector3 accel;
    private Vector3 prev;
    private const float kFilterFactor = 0.05f;
    private Matrix4x4 calibrationMatrix;
    private Vector3 accelerationSnapshot;
    public Vector2 acceleration;

    // Start is called before the first frame update
    public void Start()
    {
        // Wait a little to keep the dispositive position
        StartCoroutine(Calibrating());
    }

    // Update is called once per frame
    public void Update()
    {
        accel = Input.acceleration.normalized * kFilterFactor + (1.0f - kFilterFactor) * prev;
        prev = accel;

        // Invert axes to keep in the same orientation of the screen
        acceleration.y = accel.x;
        acceleration.x = calibrationMatrix.MultiplyVector(accel).y;
    }

    // To adjust according the actual position of the device
    public void Calibrate()
    {
        accel = Input.acceleration.normalized;
        accelerationSnapshot = accel;
        Quaternion rotateQuaternion = Quaternion.FromToRotation(new Vector3(0.0f, 0.0f, -1.0f), accelerationSnapshot);
        Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, rotateQuaternion, new Vector3(1.0f, 1.0f, 1.0f));
        calibrationMatrix = matrix.inverse;
    }

    public IEnumerator Calibrating() 
    {
        yield return new WaitForSeconds(0.5f);

        Calibrate();
    }
}
