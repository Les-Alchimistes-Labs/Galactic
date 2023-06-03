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
    public Text Information;
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
        Information.text = "";
        for (int i = 0; i < 8; i++)
        {
            everyItems[i] = Instantiate(Empty_image, here);
            everyItems[i].gameObject.SetActive(true);
        }
    }


    void Update()
    {
        Invent = Player_UI.inv;
        RefreshInv();
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
                            everyItems[i].gameObject.SetActive(true);
                            break;
                        case "Cheese":
                            if (everyItems[i] is not null)
                            {
                                Destroy(everyItems[i].gameObject);
                            }
                            everyItems[i] = Instantiate(Cheese_image, here);
                            everyItems[i].gameObject.SetActive(true);
                            break;
                        case "Hamburger":
                            if (everyItems[i] is not null)
                            {
                                Destroy(everyItems[i].gameObject);
                            }
                            everyItems[i] = Instantiate(Hamburger_image, here);
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
                            everyItems[i].gameObject.SetActive(true);
                            break;
                        case Kit_Heal:
                            if (everyItems[i] is not null)
                            {
                                Destroy(everyItems[i].gameObject);
                            }
                            everyItems[i] = Instantiate(Kit_Heal_image, here);
                            everyItems[i].gameObject.SetActive(true);
                            break;
                        case Gun:
                            if (everyItems[i] is not null)
                            {
                                Destroy(everyItems[i].gameObject);
                            }
                            everyItems[i] = Instantiate(Gun_image, here);
                            everyItems[i].gameObject.SetActive(true);
                            break;
                        case Ordinateur_Kali_Linux:
                            if (everyItems[i] is not null)
                            {
                                Destroy(everyItems[i].gameObject);
                            }
                            everyItems[i] = Instantiate(Ordinateur_Kali_Linux_image, here);
                            everyItems[i].gameObject.SetActive(true);
                            break;
                        case Sniper_a:
                            if (everyItems[i] is not null)
                            {
                                Destroy(everyItems[i].gameObject);
                            }
                            everyItems[i] = Instantiate(Sniper_image, here);
                            everyItems[i].gameObject.SetActive(true);
                            break;
                        case Potion_Boost:
                            if (everyItems[i] is not null)
                            {
                                Destroy(everyItems[i].gameObject);
                            }
                            everyItems[i] = Instantiate(Potion_Boost_image, here);
                            everyItems[i].gameObject.SetActive(true);
                            break;
                    }
                }
            }
        }
    }
    
    
    public void EmptyInformation()
    {
        Information.text = "There is nothing";
    }

    public void BananaInformation()
    {
        Information.text = "Banana? Banana!!!";
    }
    
    public void CheeseInformation()
    {
        Information.text = "I love Cheese";
    }
    
    public void HamburgerInformation()
    {
        Information.text = "Hamburger, My Favorite!!";
    }
    
    public void BossWeaponInformation()
    {
        Information.text = "Weapon of a boss !";
    }
    
    public void KitHealInformation()
    {
        Information.text = "Heal! Heal!! Heal!!!";
    }
    
    public void GunInformation()
    {
        Information.text = "A gun, dangerous and powerful weapon";
    }
    
    public void ComputerInformation()
    {
        Information.text = "Banana!!! Very Delicious";
    }
    
    public void SniperInformation()
    {
        Information.text = "Use with care, you can't imagine how power full it can be";
    }
    
    public void PotionInformation()
    {
        Information.text = "A potion. It can save your life";
    }


}
