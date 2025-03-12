using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorButton : MonoBehaviour, ITake
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

        GameObject.FindWithTag("Elevator").GetComponent<Elevator>().CallElevator();
    }
}
