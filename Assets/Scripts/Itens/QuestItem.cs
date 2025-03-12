using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestItem : MonoBehaviour, ITake
{
    // To play audios
    private AudioSource audioSrc;
    public AudioClip[] clips; // 0 - Pick

    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    public void Take() 
    {
        audioSrc.clip = clips[0];
        audioSrc.Play();

        // Indicate to controller that the iten has been taked
        //GameObject.FindWithTag("Player").GetComponent<MainController>().ItenTaked();
    }

}
