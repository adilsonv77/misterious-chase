using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour, ITake
{
    public bool locked;

    // To play audios
    private AudioSource audioSrc;
    public AudioClip[] clips; // 0 - Locked | 1 - Open

    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    public void Take()
    {
        if (locked)
        {
            audioSrc.clip = clips[0];
            audioSrc.Play();
        }
        else 
        {
            audioSrc.clip = clips[1];
            audioSrc.Play();
        }
    }
}
