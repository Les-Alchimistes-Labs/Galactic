using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_inside : MonoBehaviour
{
    public GameObject Show;
    public bool touch = false;

    // Update is called once per frame
    void Update()
    {
        Show.SetActive(true);    
    }

    public void TouchThisItem()
    {
        touch = true;
    }
}
