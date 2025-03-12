using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCube : MonoBehaviour
{

    public int x, y;

    // To play audios
    private AudioSource audioSrc;
    public AudioClip[] clips; // 0 - Piece fliped

    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began) {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        rotate();

                        // Keeps the value to verify if the puzzle was solved
                        int degree = (int)transform.rotation.eulerAngles.y;

                        GameObject.FindWithTag("Puzzle").GetComponent<Puzzle>().AttPos(x, y, degree);
                    }
                }
            }            
        }
    }

    void rotate() {
        audioSrc.clip = clips[0];
        audioSrc.Play();

        transform.Rotate(new Vector3(x: 0, y: 90, z: 0));
    }

}
