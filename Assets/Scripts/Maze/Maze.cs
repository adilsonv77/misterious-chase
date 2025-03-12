using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Maze : MonoBehaviour
{
    public TMP_Text txtTimer;
    public float time;

    // To show messages
    public GameObject mazeInfo;
    public TMP_Text txtInfo;

    // To pause the game
    private bool pausedGame;

    // To control the dust bar
    public int dustLevel;
    public Slider dustSlider;

    // To the light walls
    private GameObject[] lightWalls;

    // To control input types
    public Texture btnOn, btnOff;
    public Button accelButton;

    // To send API requests
    private API apiClient;

    // Our global functions
    private Utils utils;

    // Only for tests
    public GameObject btnGodMode;

    // Start is called before the first frame update
    void Start()
    {
        // For tests
        //PlayerPrefs.SetInt("level", 4);
        CheckGodMode();

        // To access global functions
        utils = GetComponent<Utils>();

        // To send requests
        apiClient = GetComponent<API>();

        // To prevent the cutscene to be showed again
        PlayerPrefs.SetInt("alreadyTried", PlayerPrefs.GetInt("alreadyTried") + 1);
        PlayerPrefs.SetInt("puzzleFailed", 0);

        // Check if accelerometer is not available to hide button
        if (!SystemInfo.supportsAccelerometer)
        {
            accelButton.gameObject.SetActive(false);
        }

        // Att the accelerometer image
        AttInputImg();

        /* Define the level according to the prefs
         * 2 - little maze with dust | without power-ups
         * 4 - little maze with dust | initial power-ups
         * 6 - large maze with dust | all power-ups
         * 8 - large maze with dust | all power-ups 
         */
        if (PlayerPrefs.GetInt("level") == 2)
        {
            // Disable the other level
            GameObject.Find("BlackHole lvl2").SetActive(false);
            GameObject.Find("Ball lvl2").SetActive(false);

            // Hide power ups
            DisableBonus();
        }

        if (PlayerPrefs.GetInt("level") == 4)
        {
            // Disable the other level
            GameObject.Find("BlackHole lvl2").SetActive(false);
            GameObject.Find("Ball lvl2").SetActive(false);
        }

        if (PlayerPrefs.GetInt("level") == 6 || PlayerPrefs.GetInt("level") == 8)
        {
            // Disable the other level
            GameObject.Find("BlackHole lvl1").SetActive(false);
            GameObject.Find("Ball lvl1").SetActive(false);

            // To choose a random group of power ups
            EnableBonus();
        }

        // Load light walls to can hide them
        lightWalls = GameObject.FindGameObjectsWithTag("LightWall");
        TurnLightWalls(false);

        // Verify if player already failed by dust to set quantity
        SetDustLevel();

        // Initiate the counter
        StartTimer();

        // To calc the level time
        // Verify if the pref is already set to keep the time even with retry
        if (string.IsNullOrEmpty(PlayerPrefs.GetString(utils.GetLvlParam())))
        {
            PlayerPrefs.SetString(utils.GetLvlParam(), DateTime.Now.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If the timer ends
        if (time == 0f)
        {
            PuzzleFailed(1);
        }

        // If the dust get full
        if (dustLevel >= 50)
        {
            PuzzleFailed(2);
        }
    }

    // If the player failed
    public void PuzzleFailed(int situation)
    {
        /* The game can fail for some reasons
         * 1 = failed by time
         * 2 = failed by dust excess
         */
        if (situation == 1)
        {
            // Put in prefs to take in other scene
            PlayerPrefs.SetInt("puzzleFailed", 1);

            SceneManager.LoadScene(4);
        } else if (situation == 2)
        {
            // Put in prefs to take in other scene
            PlayerPrefs.SetInt("puzzleFailed", 2);

            SceneManager.LoadScene(4);
        }
    }

    // If the player won
    public void PuzzleWon()
    {
        // Send the number of tries
        // The param need to be triesMazeLvlX
        string triesParam = "triesMazeLvl" + PlayerPrefs.GetInt("level");
        apiClient.SendDataToAPI(triesParam, PlayerPrefs.GetInt("alreadyTried"));

        // Send the time to complete
        apiClient.SendDataToAPI(utils.GetLvlParam(), utils.GetTimeInterval(PlayerPrefs.GetString(utils.GetLvlParam())));

        // Put in prefs to take in other scene
        // Increase the level and reset the alreadyTried for show cutscene in next level 
        PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") + 1);
        PlayerPrefs.SetInt("alreadyTried", 0);

        SceneManager.LoadScene(4);
    }

    void StartTimer()
    {
        ShowTime(time);

        // To execute repeatedly
        InvokeRepeating("DecreaseTime", 1f, 1f);
    }

    public void StopTimer()
    {
        CancelInvoke("DecreaseTime");
    }

    // To decrease the time
    void DecreaseTime()
    {
        if (time < 0f)
        {
            return;
        }

        if (time > 0f)
        {
            time--;
        }
        else
        {
            time = 0;
        }

        ShowTime(time);
    }

    // To show the time
    void ShowTime(float timeToDisplay)
    {
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        txtTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // To get a bonus time
    public void BonusTime(int bonusTime) {
        time += bonusTime;
        ShowTime(time);
    }

    // To show a dialogue message
    // The game is paused while showing messages
    public IEnumerator ShowMessage(string msg)
    {
        PauseGame();

        mazeInfo.SetActive(true);
        txtInfo.SetText(msg);

        // Wait 10 seconds
        yield return new WaitForSecondsRealtime(10f);

        mazeInfo.SetActive(false);

        ResumeGame();
    }

    // To pause game while showing messages
    public void PauseGame()
    {
        Time.timeScale = 0f;
        pausedGame = true;
    }

    // To resume game after showing messages
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pausedGame = false;
    }

    public bool GetPaused()
    {
        return pausedGame;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(2);

        // In case of being paused
        ResumeGame();
    }
    public void AddDust(int dust)
    {
        dustLevel += dust;

        dustSlider.value = dustLevel;
    }

    // To hide power ups
    private void DisableBonus()
    {
        GameObject[] bonuses = GameObject.FindGameObjectsWithTag("Bonus");

        foreach (GameObject bonus in bonuses)
        {
            bonus.SetActive(false);
        }
    }

    // To choose a random group of power ups
    private void EnableBonus()
    {
        GameObject[] bonusGroups = GameObject.FindGameObjectsWithTag("BonusGroup");

        foreach (GameObject group in bonusGroups)
        {
            group.SetActive(false);
        }

        bonusGroups[UnityEngine.Random.Range(0, bonusGroups.Length)].SetActive(true);
    }

    // To turn on or off the lightWalls
    public void TurnLightWalls(bool opt)
    {
        foreach (GameObject wall in lightWalls)
        {
            wall.SetActive(opt);
        }
    }

    //To change the input type
    public void ChangeInput()
    {
        if (PlayerPrefs.GetInt("controlType") == 1)
        {
            PlayerPrefs.SetInt("controlType", 2);
        }
        else
        {
            PlayerPrefs.SetInt("controlType", 1);
        }

        AttInputImg();
    }

    private void AttInputImg()
    {
        if (PlayerPrefs.GetInt("controlType") == 1)
        {
            accelButton.GetComponent<RawImage>().texture = btnOn;
        }
        else
        {
            accelButton.GetComponent<RawImage>().texture = btnOff;
        }
    }

    // To set the dust level
    // If the player already failed by dust, decrease the quantity
    private void SetDustLevel()
    {
        if (PlayerPrefs.GetInt("failedByDust") == 0)
        {
            GameObject.FindGameObjectWithTag("DustEasy").SetActive(false);
        }
        else
        {
            GameObject.FindGameObjectWithTag("DustHard").SetActive(false);
        }
    }

    // To send bonus info to api
    public void SendBonusInfo(string bonus)
    {
        apiClient.SendDataToAPI(bonus, 1);
    }

    // Only for tests
    public void CheckGodMode()
    {
        if (PlayerPrefs.GetInt("godMode") == 1)
        {
            btnGodMode.SetActive(true);
        }
    }
}
