using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{

    public GameObject alert;
    public TextMeshProUGUI txtAlert;

    public GameObject message;
    public TextMeshProUGUI txtMessage;

    // To play audios
    private AudioSource audioSrc;
    public AudioClip[] clips; // 0 - Message received

    // To control if that are a message being showed
    private bool messageIncoming = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // To show a message n in screen
    // 1 for alert
    // 2 formessage from Stephen
    public IEnumerator ShowMessage(string msg, int type)
    {
        messageIncoming = true;

        audioSrc.clip = clips[0];
        audioSrc.Play();

        if (type == 1)
        {
            txtAlert.text = msg;
            alert.SetActive(true);
        }
        else
        {
            txtMessage.text = msg;
            message.SetActive(true);
        }

        yield return new WaitForSeconds(5f);

        txtAlert.text = "";
        alert.SetActive(false);
        txtMessage.text = "";
        message.SetActive(false);

        messageIncoming = false;
    }

}
