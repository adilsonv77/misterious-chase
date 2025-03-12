using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System;
using TMPro;

public class MainMenu : MonoBehaviour
{
    // To control the about us panel
    public GameObject info; 
    private bool showInfo = false;

    // To start a new game
    public void StartGame() 
    {
        // Clean the previous prefs
        PlayerPrefs.DeleteAll();

        // Check if accelerometer is available
        // 1 - Accelerometer
        // 2 - Touch control
        if (SystemInfo.supportsAccelerometer)
        {
            PlayerPrefs.SetInt("controlType", 1);
        }
        else 
        {
            PlayerPrefs.SetInt("controlType", 2);
        }

        // Reset all the prefs
        // Set the level as 1
        // Create a random key to the player
        // Set the initial time as now
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("level", 1);
        PlayerPrefs.SetString("player", "Player_" + GenerateRandomIntID());
        PlayerPrefs.SetString("startTime", DateTime.Now.ToString());

        // Reset the alreadyTried, puzzleFailed and failedByDust
        PlayerPrefs.SetInt("alreadyTried", 0);
        PlayerPrefs.SetInt("puzzleFailed", 0);
        PlayerPrefs.SetInt("failedByDust", 0);

        /* To prevent send data while testing
         * 1 - to send
         * 0 - to not send
         */
        PlayerPrefs.SetInt("sendData", 1);

        // For tests
        PlayerPrefs.SetInt("godMode", 0);
        //API apiClient = GetComponent<API>();
        //apiClient.SendDataToAPI("test", 666);

        SceneManager.LoadScene(4);
    }

    // To load a previous game
    public void LoadGame() 
    {
        // Go to cutscenes scene to redirect for the correct level according prefs
        // Reset the puzzleFailed in case of the player quit in middle of the cutscene
        PlayerPrefs.SetInt("puzzleFailed", 0);
        SceneManager.LoadScene(4);
    }

    // To quit game
    public void QuitGame()
    {
        Application.Quit();
    }

    // To show the dev info
    public void ShowInfo()
    {
        if (!showInfo)
        {
            info.SetActive(true);
        }
        else
        {
            info.SetActive(false);
        }

        showInfo = !showInfo;
    }

    // To get a random and exclusive id
    private int GenerateRandomIntID()
    {
        // Get a seed using the clock
        int seed = (int)DateTime.Now.Ticks;

        // Create a random object with the seed
        System.Random random = new System.Random(seed);

        // Generate a random number
        return random.Next(0, int.MaxValue);
    }

}
