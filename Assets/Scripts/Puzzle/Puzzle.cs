using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Puzzle : MonoBehaviour
{
    private int[,] puzzle;
    private string[][] paths;

    // The puzzle has 2 levels, defined in the Start method
    private int lvl;

    // Which solution the player made
    private int solution;

    // To move the ball
    private bool moveBall = false;
    private int atualPos = 0;

    // The timer
    public TMP_Text txtTimer;
    public float time;


    /* The puzzle is a two dimension array
        Movements line:
          [90, 270]   - Vertical     
          [360, 180]  - Horizontal

        Movements connection:
          [90]  - Up Right     
          [180] - Down Right    
          [270] - Down Left    
          [360] - Up Left      

          ? - Any position
        
        Correct solution for lvl 1 is:
           ?      ?      ?      ?
           ?     180    270   90/270
          180     0     090     0
           ?      ?      ?      ?

        Corrects solutions for lvl 2 are:
           ?      ?      ?      ?    0/180   270
           ?      ?      ?      ?      ?    90/270
           ?      ?      ?      ?     180     0
           ?      ?      ?      ?    90/270   ?
          180    270     ?     180     0      ?
           ?     090   0/180    0      ?      ?

           ?      ?      ?      ?    0/180   270
           ?      ?     180   0/180   270   90/270
           ?      ?     090    270    090     0
           ?     180   0/180    0      ?      ?
          180     0      ?      ?      ?      ?
           ?      ?      ?      ?      ?      ?

           ?      ?      ?      ?    0/180   270
           ?      ?      ?      ?      ?    90/270
           ?      ?      ?      ?     180     0
           ?     180   0/180   270   90/270   ?
          180     0      ?     090     0      ?
           ?      ?      ?      ?      ?      ?
     */

    // Start is called before the first frame update
    void Start()
    {
        // Define the level according to the number of puzzles taked
        // 1 - level easy
        // 2 - level hard
        if (PlayerPrefs.GetInt("puzzlesTaked") == 1)
        {
            lvl = 1;

            // Disable the other level
            GameObject.FindWithTag("lvl2").SetActive(false);
        } else 
        {
            lvl = 2;

            // Disable the other level
            GameObject.FindWithTag("lvl1").SetActive(false);
        }

        // Initialize the grid of the puzzle and the paths for the ball pass
        if (lvl == 1)
        {
            puzzle = new int[4, 4];
            paths = new string[][] {
                new string[] { "130", "120", "121", "111", "112", "122", "123", "113", "103", "Base 2" },
            };
        }
        else 
        {
            puzzle = new int[6, 6];
            paths = new string[][] {
                new string[] { "250", "240", "241", "231", "232", "233", "223", "222", "212", "213", "214", "224", "225", "215", "205", "204", "203", "Base 2" },
                new string[] { "250", "240", "241", "251", "252", "253", "243", "244", "234", "224", "225", "215", "205", "204", "203", "Base 2" },
                new string[] { "250", "240", "241", "231", "232", "233", "243", "244", "234", "224", "225", "215", "205", "204", "203", "Base 2" },
            };
        }

        StartTimer();
    }

    // Update is called once per frame
    void Update()
    {
        // If the timer ends
        if (time == 0f)
        {
            PuzzleFailed();
        }

        // If i solve the puzzle
        if (CheckSolution())
        {
            StopTimer();
            moveBall = true;
        }

        if (moveBall) 
        {
            // Move the ball for the puzzle
            Transform ball = GameObject.FindWithTag("Ball").transform;
            Transform dest = GameObject.Find(paths[solution][atualPos]).transform;

            Vector3 dir = (dest.position - ball.position).normalized;

            float distance = Vector3.Distance(ball.position, dest.position);

            if (distance > 0.1f)
            {
                ball.position += dir * 8f * Time.deltaTime;
            }
            else {
                atualPos++;
                
                if (atualPos == paths[solution].Length)
                {
                    moveBall = false;
                    PuzzleWon();
                }
            }
        }
    }

    // If the player failed
    public void PuzzleFailed()
    {
        // Put in prefs to take in other scene
        PlayerPrefs.SetInt("alreadyTried", 1);

        SceneManager.LoadScene(1);
    }

    // If the player won
    public void PuzzleWon()
    {
        // Put in prefs to take in other scene
        PlayerPrefs.SetInt("alreadyTried", 0);
        PlayerPrefs.SetInt("puzzlesTaked", PlayerPrefs.GetInt("puzzlesTaked") + 1);

        SceneManager.LoadScene(1);
    }

    public void AttPos(int x, int y, int degree) 
    {
        puzzle[x, y] = degree;
    }

    bool CheckSolution() 
    {
        //Check the level
        if (lvl == 1)
        {
            // in the first level there only one solution
            if (puzzle[1, 1] == 180 &&
                puzzle[1, 2] == 270 &&
                (puzzle[1, 3] == 90 || puzzle[1, 3] == 270) &&
                puzzle[2, 0] == 180 &&
                puzzle[2, 1] == 0 &&
                puzzle[2, 2] == 90 &&
                puzzle[2, 3] == 0)
            {
                solution = 0;

                return true;
            }
        }
        else {
            // in the second level there are 3 solutions
            if ((puzzle[0, 4] == 0 || puzzle[0, 4] == 180) &&
                (puzzle[0, 5] == 270 || puzzle[0, 5] == -90) &&
                puzzle[1, 2] == 180 &&
                (puzzle[1, 3] == 0 || puzzle[1, 3] == 180) &&
                (puzzle[1, 4] == 270 || puzzle[1, 4] == -90) &&
                (puzzle[1, 5] == 90 || puzzle[1, 5] == -90 || puzzle[1, 5] == 270) &&
                puzzle[2, 2] == 90 &&
                (puzzle[2, 3] == 270 || puzzle[2, 3] == -90) &&
                puzzle[2, 4] == 90 &&
                puzzle[2, 5] == 0 &&
                puzzle[3, 1] == 180 &&
                (puzzle[3, 2] == 0 || puzzle[3, 2] == 180) &&
                puzzle[3, 3] == 0 &&
                puzzle[4, 0] == 180 &&
                puzzle[4, 1] == 0)
            {
                solution = 0;

                return true;
            }
            else if ((puzzle[0, 4] == 0 || puzzle[0, 4] == 180) &&
                    (puzzle[4, 1] == 270 || puzzle[4, 1] == -90) &&
                    (puzzle[1, 5] == 90 || puzzle[1, 5] == -90 || puzzle[1, 5] == 270) &&
                    puzzle[2, 4] == 180 &&
                    puzzle[2, 5] == 0 &&
                    (puzzle[3, 4] == 90 || puzzle[3, 4] == -90 || puzzle[3, 4] == 270) &&
                    puzzle[4, 0] == 180 &&
                    (puzzle[4, 1] == 270 || puzzle[4, 1] == -90) &&
                    puzzle[4, 3] == 180 &&
                    puzzle[4, 4] == 0 &&
                    puzzle[5, 1] == 90 &&
                    (puzzle[5, 2] == 0 || puzzle[5, 2] == 180) &&
                    puzzle[5, 3] == 0)
            {
                solution = 1;

                return true;
            }
            else if ((puzzle[0, 4] == 0 || puzzle[0, 4] == 180) &&
                    (puzzle[0, 5] == 270 || puzzle[0, 5] == -90) &&
                    (puzzle[1, 5] == 90 || puzzle[1, 5] == -90 || puzzle[1, 5] == 270) &&
                    puzzle[2, 4] == 180 &&
                    puzzle[2, 5] == 0 &&
                    puzzle[3, 1] == 180 &&
                    (puzzle[3, 2] == 0 || puzzle[3, 2] == 180) &&
                    (puzzle[3, 3] == 270 || puzzle[3, 3] == -90) &&
                    (puzzle[3, 4] == 90 || puzzle[3, 4] == -90 || puzzle[3, 4] == 270) &&
                    puzzle[4, 0] == 180 &&
                    puzzle[4, 1] == 0 &&
                    puzzle[4, 3] == 90 &&
                    puzzle[4, 4] == 0)
            {
                solution = 2;

                return true;
            }
        }

        return false;
    }

    void StartTimer() {
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

}
