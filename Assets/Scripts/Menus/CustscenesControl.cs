using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustscenesControl : MonoBehaviour
{
    private int lastLevel = 8;

    private float fadeTime = 1f;
    private float transitionTime = 10f;
    public RawImage[] images;

    // To send API requests
    private API apiClient;

    // Our global functions
    private Utils utils;

    // To control cutscenes
    public GameObject skipButton;
    private int imgAtual = 0;
    private int startImage;
    private int lastImage;
    private bool firstTimeShowing = true;
    private bool showing = false;

    // Only for tests
    public GameObject btnGodMode;

    // Start is called before the first frame update
    void Start()
    {
        // For tests
        //PlayerPrefs.SetInt("level", 1);
        //PlayerPrefs.SetInt("alreadyTried", 0);
        CheckGodMode();

        // To access global functions
        utils = GetComponent<Utils>();

        // To send requests
        apiClient = GetComponent<API>();

        // Will verify the level and if it´s the first time trying
        // Control if need to show cutscenes or directly pass the player to the level

        // Level 1
        if (PlayerPrefs.GetInt("level") == 1)
        {
            if (PlayerPrefs.GetInt("puzzleFailed") == 0)
            {
                if (PlayerPrefs.GetInt("alreadyTried") > 0)
                {
                    SceneManager.LoadScene(5);
                }
                else
                {
                    showing = true;
                    startImage = 0;
                    lastImage = 8;

                    StartCoroutine(ShowImage());
                }
            }
        }

        // Level 2
        if (PlayerPrefs.GetInt("level") == 2)
        {
            if (PlayerPrefs.GetInt("puzzleFailed") == 0)
            {
                if (PlayerPrefs.GetInt("alreadyTried") > 0)
                {
                    SceneManager.LoadScene(2);
                }
                else
                {
                    showing = true;
                    startImage = 9;
                    lastImage = 11;

                    StartCoroutine(ShowImage());
                }
            }
        }

        // Level 4
        if (PlayerPrefs.GetInt("level") == 4)
        {
            if (PlayerPrefs.GetInt("puzzleFailed") == 0)
            {
                if (PlayerPrefs.GetInt("alreadyTried") > 0)
                {
                    SceneManager.LoadScene(2);
                }
                else
                {
                    showing = true;
                    startImage = 12;
                    lastImage = 14;

                    StartCoroutine(ShowImage());
                }
            }
        }

        // Level 3, 5 e 7
        if (PlayerPrefs.GetInt("level") == 3 || PlayerPrefs.GetInt("level") == 5 || PlayerPrefs.GetInt("level") == 7)
        {
            // When failed so continue
            if (PlayerPrefs.GetInt("puzzleFailed") == 0)
            {
                SceneManager.LoadScene(5);
            }
        }

        // Level 6 e 8
        if (PlayerPrefs.GetInt("level") == 6 || PlayerPrefs.GetInt("level") == 8)
        {
            // When failed so continue
            if (PlayerPrefs.GetInt("puzzleFailed") == 0)
            {
                SceneManager.LoadScene(2);
            }
        }

        // The end
        if (PlayerPrefs.GetInt("level") > lastLevel)
        {
            // Send the total time
            apiClient.SendDataToAPI("totalTime", utils.GetTimeInterval(PlayerPrefs.GetString("startTime")));

            showing = true;
            startImage = 17;
            lastImage = 19;

            StartCoroutine(ShowImage());
        }

        // Time Limit
        if (PlayerPrefs.GetInt("puzzleFailed") == 1)
        {
            showing = true;
            startImage = 16;
            lastImage = 16;

            StartCoroutine(ShowImage());
        }

        // Quasar explosion
        if (PlayerPrefs.GetInt("puzzleFailed") == 2)
        {
            showing = true;
            startImage = 15;
            lastImage = 15;

            StartCoroutine(ShowImage());

            // Set prefs to indicate the player has already failed by dust
            PlayerPrefs.SetInt("failedByDust", 1);

            // Send if failed by dust
            apiClient.SendDataToAPI("quasarExplosion", 1);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        //Verification for redirect the player after play the cutscenes

        // Follow The Way levels
        if (PlayerPrefs.GetInt("level") % 2 != 0 && PlayerPrefs.GetInt("level") <= lastLevel)
        {
            if (!showing) 
            {
                SceneManager.LoadScene(5);
            }            
        }

        // Maze levels
        if (PlayerPrefs.GetInt("level") % 2 == 0 && PlayerPrefs.GetInt("level") <= lastLevel)
        {
            if (!showing)
            {
                SceneManager.LoadScene(2);
            }
        }

        // The end
        if (PlayerPrefs.GetInt("level") > lastLevel)
        {
            if (!showing)
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    // Show some images in transition
    private IEnumerator ShowImage()
    {
        // Only in first time
        if (firstTimeShowing)
        {
            // Hide loading Screen
            GameObject.FindWithTag("LoadingScreen").SetActive(false);

            imgAtual = startImage;

            // Hide images before the first
            for (int i = 0; i < startImage; i++)
            {
                images[i].GetComponentInParent<CanvasGroup>().alpha = 0f;
            }

            firstTimeShowing = false;
        }

        // Wait the transition time
        yield return new WaitForSeconds(transitionTime);

        // Enable skip button
        skipButton.SetActive(true);
    }

    // To progress the cutscenes
    private IEnumerator NextCutscene() {
        // It´s the last image
        if (imgAtual == lastImage)
        {
            firstTimeShowing = true;
            showing = false;
        }
        else 
        {
            // Hide the button
            skipButton.SetActive(false);

            // Start fade-out
            yield return StartCoroutine(Fade(false));

            // Next Image
            imgAtual = (imgAtual + 1) % images.Length;

            StartCoroutine(ShowImage());
        }
    }

    // To the skip button
    public void HandleSkipButton() 
    {
        StartCoroutine(NextCutscene());
    }

    // To apply a fade effect
    private IEnumerator Fade(bool fadeIn)
    {
        float tempoInicial = Time.time;
        float alphaInicial = fadeIn ? 0f : 1f;
        float alphaFinal = fadeIn ? 1f : 0f;

        CanvasGroup canvas = images[imgAtual].GetComponentInParent<CanvasGroup>();

        canvas.alpha = alphaInicial;

        while (Time.time < tempoInicial + fadeTime)
        {
            float tempoDecorrido = Time.time - tempoInicial;
            float fracao = tempoDecorrido / fadeTime;
            float alpha = Mathf.Lerp(alphaInicial, alphaFinal, fracao);

            canvas.alpha = alpha;
            yield return null;
        }

        canvas.alpha = alphaFinal;
    }

    // Only for tests
    public void SkipCutscene() 
    {
        showing = false;
    }

    public void CheckGodMode() 
    {
        if (PlayerPrefs.GetInt("godMode") == 1) 
        {
            btnGodMode.SetActive(true);
        }
    }

}
