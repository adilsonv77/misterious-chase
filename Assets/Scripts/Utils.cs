using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    // To get the diference in seconds between now and an specific time
    public int GetTimeInterval(string initialTime)
    {
        DateTime startTime = DateTime.Parse(initialTime);

        // Get the interval in seconds
        TimeSpan duration = DateTime.Now - startTime;
        int durationInSeconds = (int)duration.TotalSeconds;

        return durationInSeconds;
    }

    // To save the level time
    // The param need to be lvlXTime
    public string GetLvlParam()
    {
        return "lvl" + PlayerPrefs.GetInt("level") + "Time";
    }
}
