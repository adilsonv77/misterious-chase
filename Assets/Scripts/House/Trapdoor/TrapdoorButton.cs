using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class TrapdoorButton : MonoBehaviour, ITake
{
    // To play audios
    private AudioSource audioSrc;
    public AudioClip[] clips; // 0 - Press Button

    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    public void Take()
    {
        audioSrc.clip = clips[0];
        audioSrc.Play();

        if (PlayerPrefs.GetInt("puzzlesTaked") < 3)
        {
            StartCoroutine(GameObject.FindWithTag("Player").GetComponent<MainController>().ShowMessage("O que você está fazendo ai? Ah, sim, eu sei que é impressionante um alçapão hitech no meio da casa. Nós conversamos sobre isso depois." ,2));
        }
        else 
        {
            GameObject.FindWithTag("Trapdoor").GetComponent<Trapdoor>().OpenDoor();
        }    
    }
}
