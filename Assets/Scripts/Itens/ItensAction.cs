using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItensAction : MonoBehaviour
{
    private Itens idItens;
    public Button actionButton;
    private bool clicked;

    // Start is called before the first frame update
    void Start()
    {
        idItens = GetComponent<Itens>();
     
        clicked = false;
        actionButton.onClick.AddListener(OnClickAction);
    }

    // Update is called once per frame
    void Update()
    {
        float distance = idItens.GetDistanceItem();

        // Only take objects that were 3 meters from player
        if (distance < 3)
        {
            idItens.ShowTxt();
                        
            // Verifica se o botão de pegar foi clicado
            if (clicked && idItens.GetItemTake() != null)
            {
                Take();
                clicked = false;
            }
        }
        else
        {
            idItens.HideTxt();
        }
    }

    private void Take()
    {
        ITake item = idItens.GetItemTake().GetComponent<ITake>();
        item.Take();

        //Destroy(idItens.GetItemTake());
        idItens.CleanTxt();
    }

    void OnClickAction() 
    {
        clicked = true;
    }

}
