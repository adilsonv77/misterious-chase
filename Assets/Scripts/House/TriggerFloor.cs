using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TriggerFloor : MonoBehaviour
{

    public string floor;
    public TextMeshProUGUI txtFloor;

    // For first time in the laboratory
    private bool visitLab = false;

    //To show the floor in screen
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            txtFloor.text = floor;
        }

        // Message for first time in the laboratory
        if (txtFloor.text == "Laboratório" && !visitLab) 
        {
            StartCoroutine(GameObject.FindWithTag("Player").GetComponent<MainController>().ShowMessage("Laboratório maneiro né, eu sei, só não me pergunte como eu fiz para construir ele. Tenho pesadelos até hoje com os malditos alvarás para laboratórios super perigosos.", 2));
            visitLab = true;
        }
    }
}
