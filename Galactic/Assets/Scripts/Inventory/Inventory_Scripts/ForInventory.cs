using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using JetBrains.Annotations;
using personnage_class.Personage;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ForInventory : MonoBehaviour
{
    public Transform here;
    public Inventory_inside Empty_image;
    public Inventory_inside Banana_image;
    public Inventory_inside Cheese_image;
    public Inventory_inside Hamburger_image;
    public Inventory_inside Boss_Weapon_image;
    public Inventory_inside Gun_image;
    public Inventory_inside Kit_Heal_image;
    public Inventory_inside Ordinateur_Kali_Linux_image;
    public Inventory_inside Potion_Boost_image;
    public Inventory_inside Sniper_image;
    
    private Inventory_inside[]? everyItems = new Inventory_inside[8];
    public static Item[]? Invent = new Item[8];

    private void Start()
    {
        for (int i = 0; i < 8; i++)
        {
            everyItems[i] = Instantiate(Empty_image, here);
            everyItems[i].gameObject.SetActive(true);
        }
    }

    public int Index { get; set; }


    private void ChangePosInv()
    {
        Player_UI.pos = Index;
    }

    void Update()
    {
        Invent = Player_UI.inv;
        RefreshInv();
        ChangePosInv();
    }

    private void RefreshInv()
    {
        for (int i = 0; i < 8; i++)
        {
            if (Invent[i] is null)
            {
                if (everyItems[i] is not null)
                {
                    Destroy(everyItems[i].gameObject);
                }
                everyItems[i] = Instantiate(Empty_image, here);
                if (everyItems[i].touch)
                {
                    Index = i + 1;
                    everyItems[i].touch = false;
                }
                everyItems[i].gameObject.SetActive(true);
            }
            else
            {
                if (Invent[i] is Food)
                {
                    switch (Invent[i].Name)
                    {
                        case "Banana":
                            if (everyItems[i] is not null)
                            {
                                Destroy(everyItems[i].gameObject);
                            }
                            everyItems[i] = Instantiate(Banana_image, here);
                            if (everyItems[i].touch)
                            {
                                Index = i + 1;
                                everyItems[i].touch = false;
                            }
                            everyItems[i].gameObject.SetActive(true);
                            break;
                        case "Cheese":
                            if (everyItems[i] is not null)
                            {
                                Destroy(everyItems[i].gameObject);
                            }
                            everyItems[i] = Instantiate(Cheese_image, here);
                            if (everyItems[i].touch)
                            {
                                Index = i + 1;
                                everyItems[i].touch = false;
                            }
                            everyItems[i].gameObject.SetActive(true);
                            break;
                        case "Hamburger":
                            if (everyItems[i] is not null)
                            {
                                Destroy(everyItems[i].gameObject);
                            }
                            everyItems[i] = Instantiate(Hamburger_image, here);
                            if (everyItems[i].touch)
                            {
                                Index = i + 1;
                                everyItems[i].touch = false;
                            }
                            everyItems[i].gameObject.SetActive(true);
                            break;
                    }
                }
                else
                {
                    switch (Invent[i])
                    {
                        case Boss_Weapon:
                            if (everyItems[i] is not null)
                            {
                                Destroy(everyItems[i].gameObject);
                            }
                            everyItems[i] = Instantiate(Boss_Weapon_image, here);
                            if (everyItems[i].touch)
                            {
                                Index = i + 1;
                                everyItems[i].touch = false;
                            }
                            everyItems[i].gameObject.SetActive(true);
                            break;
                        case Kit_Heal:
                            if (everyItems[i] is not null)
                            {
                                Destroy(everyItems[i].gameObject);
                            }
                            everyItems[i] = Instantiate(Kit_Heal_image, here);
                            if (everyItems[i].touch)
                            {
                                Index = i + 1;
                                everyItems[i].touch = false;
                            }
                            everyItems[i].gameObject.SetActive(true);
                            break;
                        case Gun:
                            if (everyItems[i] is not null)
                            {
                                Destroy(everyItems[i].gameObject);
                            }
                            everyItems[i] = Instantiate(Gun_image, here);
                            if (everyItems[i].touch)
                            {
                                Index = i + 1;
                                everyItems[i].touch = false;
                            }
                            everyItems[i].gameObject.SetActive(true);
                            break;
                        case Ordinateur_Kali_Linux:
                            if (everyItems[i] is not null)
                            {
                                Destroy(everyItems[i].gameObject);
                            }
                            everyItems[i] = Instantiate(Ordinateur_Kali_Linux_image, here);
                            if (everyItems[i].touch)
                            {
                                Index = i + 1;
                                everyItems[i].touch = false;
                            }
                            everyItems[i].gameObject.SetActive(true);
                            break;
                        case Sniper_a:
                            if (everyItems[i] is not null)
                            {
                                Destroy(everyItems[i].gameObject);
                            }
                            everyItems[i] = Instantiate(Sniper_image, here);
                            if (everyItems[i].touch)
                            {
                                Index = i + 1;
                                everyItems[i].touch = false;
                            }
                            everyItems[i].gameObject.SetActive(true);
                            break;
                        case Potion_Boost:
                            if (everyItems[i] is not null)
                            {
                                Destroy(everyItems[i].gameObject);
                            }
                            everyItems[i] = Instantiate(Potion_Boost_image, here);
                            if (everyItems[i].touch)
                            {
                                Index = i + 1;
                                everyItems[i].touch = false;
                            }
                            everyItems[i].gameObject.SetActive(true);
                            break;
                    }
                }
            }
        }
    }
    
}
