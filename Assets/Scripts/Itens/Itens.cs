using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Itens : MonoBehaviour
{
    private float distanceItem;
    private GameObject itemTake;
    private GameObject item;
    public Button takeButton;
    public Texture imgNormal, imgTaked;

    public float GetDistanceItem()
    {
        return distanceItem;
    }

    public GameObject GetItemTake()
    {
        return itemTake;
    }

    public void ShowTxt()
    {
        //takeButton.GetComponent<RawImage>().texture = imgTaked;
    }

    public void HideTxt()
    {
        //takeButton.GetComponent<RawImage>().texture = imgNormal;
    }

    // Start is called before the first frame update
    void Start()
    {
        HideTxt();
    }

    // Update is called once per frame
    void Update()
    {
        // To execute each 5 frames
        if (Time.frameCount % 5 == 0)
        {
            itemTake = null;

            RaycastHit hit;

            if (Physics.SphereCast(transform.position, 0.1f, transform.TransformDirection(Vector3.forward), out hit, 5))
            {
                distanceItem = hit.distance;

                if (item != null && hit.transform.gameObject != item)
                {
                    item.GetComponent<Outline>().OutlineWidth = 0f;
                    item = null;

                    takeButton.GetComponent<RawImage>().texture = imgNormal;
                }


                if (hit.transform.gameObject.tag == "Take")
                {
                    itemTake = hit.transform.gameObject;
                    item = itemTake;
                }

                if (item != null)
                {
                    item.GetComponent<Outline>().OutlineWidth = 5f;
                    takeButton.GetComponent<RawImage>().texture = imgTaked;
                }
            }
            else
            {
                if (item != null)
                {
                    item.GetComponent<Outline>().OutlineWidth = 0f;
                    item = null;

                    CleanTxt();
                }
            }
        }
    }

    public void CleanTxt()
    {
        takeButton.GetComponent<RawImage>().texture = imgNormal;
    }

}
