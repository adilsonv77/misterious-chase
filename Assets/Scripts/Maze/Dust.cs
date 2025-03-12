using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dust : MonoBehaviour
{

    // To play audios
    private AudioSource audioSrc;
    public AudioClip[] clips; // 0 - Dust taken

    // To prevent double dust
    public bool takingDust = false;

    /* Taken dust will progress a bar randomly.
     * The game will fail if the bar get full
     */

    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //To collide take dust and progress the bar
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ball")
        {
            if (!takingDust) 
            {
                StartCoroutine(TakeDust());
            }
            
        }
    }

    IEnumerator TakeDust() 
    {
        takingDust = true;

        audioSrc.clip = clips[0];
        audioSrc.Play();

        // Progress the dust bar with a random value between 1 and 10
        int dust = Random.Range(1, 10 + 1);

        GameObject.FindWithTag("Puzzle").GetComponent<Maze>().AddDust(dust);

        // Time to play the sound
        yield return new WaitForSeconds(0.3f);

        // Destroy the bonus item
        Destroy(gameObject);
    }

}
