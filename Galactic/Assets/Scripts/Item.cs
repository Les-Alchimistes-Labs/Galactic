using System;
using System.Collections;
using System.Collections.Generic;
using personnage_class.Personage;
using Photon.Pun;
using UnityEditor;
using UnityEngine;

public class ItemInGame : MonoBehaviour
{
    public Item Item;
    public EnumsItems EnumsIteme;

    public GameObject gameObject;

    public bool took = false;
    // Start is called before the first frame update
    void Start()
    {
        switch (EnumsIteme)
        {
            case EnumsItems.Boss_Weapon:
                Item = new Boss_Weapon(1,9,1);
                break;
            case EnumsItems.Banana:
                Item = new Food("Banana",1,3);
                break;
            case EnumsItems.Cheese:
                Item = new Food("Cheese",1,5);
                break;
            case EnumsItems.Hamburger:
                Item = new Food("Hamburger",1,10);
                break;
            case EnumsItems.Gun:
                Item = new Gun(1,3,1);
                break;
            case EnumsItems.Kit_Heal:
                Item = new Kit_Heal(1,9);
                break;
            case EnumsItems.Ordinateur_Kali_Linux:
                Item = new Ordinateur_Kali_Linux(4,1);
                break;
            case EnumsItems.Potion_Boost:
                Item = new Potion_Boost("Potion_Mana",1,true,1,EnumsItem.Boost);
                break;
            case EnumsItems.Sniper:
                Item = new Sniper_a(5,1,1);
                break;
        }


    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
