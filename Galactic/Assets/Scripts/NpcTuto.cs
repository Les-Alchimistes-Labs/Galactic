using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Globalization;
using TMPro;
using UnityEngine.UI;


public class NpcTuto : MonoBehaviour
{
    private int x = 0;
    private int y=0;
    private string[] dialogue = new string[]
    {
        "press 't' for the next hint",
        "press 'e' to take loot","press 'a' to drop item in inventory",
        "you can attack monster with others peoples", "you can save your player",
        
    };
    public GameObject Text;
    public GameObject npc;
    // Start is called before the first frame update
    void Start()
    {
        y = DateTime.Now.Second;
        Text.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Text.SetActive(true);
            Text.GetComponent<TextMeshPro>().text =dialogue[x]; 
            transform.LookAt(other.transform);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        transform.LookAt(other.transform);
        //Text.transform.LookAt(other.transform);
        if (Input.GetKey(KeyCode.T) && y!= DateTime.Now.Second)
        {
            x++;
            y = DateTime.Now.Second;
            if (x >= dialogue.Length)
            {
                x =0;
                
            }
            Text.GetComponent<TextMeshPro>().text =dialogue[x];
            Debug.Log(Text.GetComponent<TextMeshPro>().text);
            Text.GetComponent<TextMeshPro>().ForceMeshUpdate();
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Text.SetActive(false);
    }
}
