using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GiveDirection : MonoBehaviour
{
    public LayerMask layer;
    public float distance = 1000f;

    // To check if the player was seeing the trail
    private bool seeing = false;

    // To check if it´s complete
    private bool finish = false;

    // To start searching the trail
    private bool searching = false;
    public Button actionButton;

    public Texture imgNormal, imgTaked;
    public float indicatorTimer = 0f;
    public float increaseRadius = 0.05f;

    // To play audios
    private AudioSource audioSrc;
    public AudioClip[] clips; // 0 - Interference

    // To show messages
    public GameObject puzzleInfo;
    public TMP_Text puzzleTxt;
    private bool showingMsg;

    // To show hint
    private bool showingHint;

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
        //PlayerPrefs.SetInt("level", 1);
        CheckGodMode();

        audioSrc = GetComponent<AudioSource>();

        // To access global functions
        utils = GetComponent<Utils>();

        // To send requests
        apiClient = GetComponent<API>();

        GameObject.FindWithTag("Radar").GetComponent<Image>().enabled = false;

        // Choose randomly one of the points to be the trail
        ChoosseTrail();

        // In some levels will show messages for the player
        if (PlayerPrefs.GetInt("level") == 3) 
        {
            string[] messages = {
                "O que estamos fazendo aqui? Bom, A radiação Hawking resulta da criação de partículas no horizonte de eventos do buraco negro.... Ah, você não estava perguntando literalmente? Desculpe, garoto, me empolguei hahaha",
                "Um momento para apreciar, por favor... Certo, temos trabalho a fazer, use seu radar e localize outro Buraco Negro para seguirmos nossa jornada."
            };


            StartCoroutine(ShowMessages(messages));
        }

        if (PlayerPrefs.GetInt("level") == 5)
        {
            string[] messages = {
                "Vê os planetas lá na frente? Olhar para eles me faz pensar sobre a gravidade e como ela é magnífica... não é algo lindo de se pensar?!",
                "Sabe, garoto, há muito tempo Newton propôs que a gravidade fosse como uma força de atração direta entre massas. Uma coisa maior que atrai uma menor, basicamente.",
                "Mestre Einstein contrapôs essa teoria, ele explica que a gravidade resulta da curvatura do espaço-tempo causada pela presença de massa e energia. Como uma bola caindo e deformando a água."
            };

            StartCoroutine(ShowMessages(messages));
        }

        if (PlayerPrefs.GetInt("level") == 7)
        {
            string[] messages = {
                "Você sabia que a Radiação Hawking é única e exclusiva para Buracos Negros? Esse sinal característico emitido por eles não possui nenhuma outra utilidade conhecida por nós, pelo menos não ainda...",
                "Sinto que nossa jornada está próxima de chegar ao seu fim, ou quem sabe apenas a um hiato passageiro e relativo. Bom, você sabe o que fazer... Vá caçar alguns buracos negros, rapaz!",
            };


            StartCoroutine(ShowMessages(messages));
        }

        // To prevent the cutscene to be showed again
        PlayerPrefs.SetInt("alreadyTried", 1);

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
        // Detect if the player was looking for the right direction
        // The Radar in HUD will change
        if (searching) 
        {
            Vector3 originRay = Camera.main.transform.position;
            Vector3 directionRay = Camera.main.transform.forward;

            RaycastHit hitInfo;

            if (Physics.Raycast(originRay, directionRay, out hitInfo, distance, layer))
            {
                if (hitInfo.collider.CompareTag("Trail"))
                {
                    // If wasn´t seeing yet
                    // Change the radar in HUD and play the sound in loop
                    if (!seeing)
                    {
                        seeing = true;

                        GameObject.FindWithTag("Radar").GetComponent<Image>().enabled = true;

                        audioSrc.clip = clips[0];
                        audioSrc.loop = true;
                        audioSrc.Play();
                    }

                    // Increase the image radius
                    indicatorTimer += (Time.deltaTime / 10);
                    GameObject.FindWithTag("Radar").GetComponent<Image>().fillAmount = indicatorTimer;

                    if (indicatorTimer >= 1 && !finish)
                    {
                        EndCircle();
                    }
                }
                else
                {
                    // Seing something, but not the trail
                    StopSeeing();
                }
            }
            else
            {
                // Not seing anything
                StopSeeing();
            }
        } 
        else
        {
            // Button not pressed
            StopSeeing();
        }
    }

    // To define if the player is searching
    public void StartSearch() {
        if (searching) 
        {
            actionButton.GetComponent<RawImage>().texture = imgNormal;

            GameObject.FindWithTag("Radar").GetComponent<Image>().enabled = false;
            GameObject.FindWithTag("RadarExterno").GetComponent<Image>().enabled = false;
        } 
        else 
        {
            actionButton.GetComponent<RawImage>().texture = imgTaked;

            GameObject.FindWithTag("Radar").GetComponent<Image>().enabled = true;
            GameObject.FindWithTag("RadarExterno").GetComponent<Image>().enabled = true;
        }

        searching = !searching;
    }

    // When stop seeing
    public void StopSeeing()
    {
        seeing = false;

        indicatorTimer -= (Time.deltaTime / 10);
        GameObject.FindWithTag("Radar").GetComponent<Image>().fillAmount = indicatorTimer;

        if (indicatorTimer <= 0)
        {
            indicatorTimer = 0;
            GameObject.FindWithTag("Radar").GetComponent<Image>().fillAmount = indicatorTimer;
            GameObject.FindWithTag("Radar").GetComponent<Image>().enabled = false;
        }

        // Check if there isn´t other sounds playing
        if (!GameObject.FindWithTag("Player").GetComponent<Movements>().getIsPlayingSound()) 
        {
            audioSrc.loop = false;
            audioSrc.Stop();
        }
    }

    // When complete the circle
    // Register the level and go to cutscene menu
    public void EndCircle() 
    {
        // To avoid multiple calls
        finish = true;

        // Send the time to complete
        apiClient.SendDataToAPI(utils.GetLvlParam(), utils.GetTimeInterval(PlayerPrefs.GetString(utils.GetLvlParam())));

        // Put in prefs to take in other scene
        // Increase the level and reset the alreadyTried for show cutscene in next level 
        PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") + 1);
        PlayerPrefs.SetInt("alreadyTried", 0);

        SceneManager.LoadScene(4);
    }

    // To show multiple messages
    public IEnumerator ShowMessages(string[] messages)
    {
        showingMsg = true;

        foreach (string msg in messages) 
        {
            puzzleTxt.text = msg;
            puzzleInfo.SetActive(true);

            // Wait 10 seconds
            yield return new WaitForSecondsRealtime(10f);
        }

        puzzleInfo.SetActive(false);
        showingMsg = false;
    }

    // To choose randomly one of the points to be the trail
    private void ChoosseTrail() 
    {
        GameObject[] cubes = GameObject.FindGameObjectsWithTag("Trail");

        foreach (GameObject cube in cubes)
        {
            cube.SetActive(false);
        }

        cubes[UnityEngine.Random.Range(0, cubes.Length)].SetActive(true);
    }

    public void ShowHint() {
        // If the player ask the hint while showing some message
        if (showingMsg)
        {
            return;
        }

        if (!showingHint)
        {
            puzzleTxt.text = "Ative seu sensor pressionando o botão no canto inferior direito e olhe ao redor, o som da estática indicará que você está olhando na direção correta. Mantenha sua visão fixa no Buraco Negro até a barra ser preenchida.";
            puzzleInfo.SetActive(true);
        }
        else
        {
            puzzleInfo.SetActive(false);
        }

        showingHint = !showingHint;
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
