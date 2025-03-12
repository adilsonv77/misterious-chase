using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEndPuzzle : MonoBehaviour
{
    //To collide for end the puzzle
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ball")
        {
            GameObject.FindWithTag("Puzzle").GetComponent<Maze>().PuzzleWon();
        }
    }
}
