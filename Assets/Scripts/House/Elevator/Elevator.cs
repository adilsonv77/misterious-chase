using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Elevator : MonoBehaviour
{
    private float distance = 5f;
    private float speed = 1f;

    Vector3 posInit;

    bool move = false;

    // To play audios
    private AudioSource audioSrc;
    public AudioClip[] clips; // 0 - Open trapdoor

    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();

        posInit = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (move) 
        {
            Vector3 posTarget = posInit + new Vector3(0f, distance, 0f);

            transform.position = Vector3.Lerp(transform.position, posTarget, speed * Time.deltaTime);

            // When in the top, change positions to get back down
            if (Vector3.Distance(transform.position, posTarget) < 0.1f)
            {
                posInit = transform.position;
                distance = distance * -1;

                move = false;
            }
        }
    }

    // To ativate the elevator
    public void CallElevator() 
    {
        move = true;

        audioSrc.clip = clips[0];
        audioSrc.Play();
    }

}
