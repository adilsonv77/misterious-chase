using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroundControl : MonoBehaviour
{
    /* We have two differents input types
     * 1 - Accelerometer
     * 2 - Touch control
     */

    AccelerationManager accelerationManager;

    private Dictionary<int, Vector2> activeTouches = new Dictionary<int, Vector2>();
    Vector3 touchVector = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        accelerationManager = FindObjectOfType<AccelerationManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check for paused game
        if (!GameObject.FindWithTag("Puzzle").GetComponent<Maze>().GetPaused()) {
            // Check the input type
            if (PlayerPrefs.GetInt("controlType") == 1)
            {
                Vector3 accel = accelerationManager.acceleration;
                
                // Adjusts in axes for improve gameplay
                accel.z = -accel.y;
                accel.y = 0;

                transform.eulerAngles = accel * 45;
            }
            // If using touch control
            else
            {
                // Read all touches from the user
                foreach (Touch touch in Input.touches)
                {
                    // If just press the screen
                    if (touch.phase == TouchPhase.Began)
                    {
                        activeTouches.Add(touch.fingerId, touch.position);
                    }
                    // If remove the finger off the screen
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        if (activeTouches.ContainsKey(touch.fingerId))
                        {
                            activeTouches.Remove(touch.fingerId);
                        }
                    }
                    // If the finger is moving on
                    else 
                    {
                        // Apply a dead zone
                        float magnitude = 0;
                        Vector3 deltaPosition = touch.deltaPosition;
                        magnitude = deltaPosition.magnitude / 300;
                        deltaPosition = deltaPosition.normalized * magnitude;

                        // Invert axes to keep in the same orientation of the screen
                        // Adjusts for improve gameplay
                        deltaPosition.z = -deltaPosition.x;
                        deltaPosition.x = deltaPosition.y;
                        deltaPosition.y = 0;

                        touchVector += deltaPosition;
                    }
                }

                transform.eulerAngles = touchVector * 2;
            }
        }
    }
}
