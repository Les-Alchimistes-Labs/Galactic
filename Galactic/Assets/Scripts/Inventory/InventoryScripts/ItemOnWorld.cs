using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using personnage_class.Personage;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ItemOnWorld : MonoBehaviour
{
    public Sprite imageBanana;
    public Sprite imageCheese;
    public Sprite imageBoss_Weapon;
    public Sprite imageHamburger;
    public Sprite imageGun;
    public Sprite imageKit_Heal;
    public Sprite imageKahli_Computer;
    public Sprite imagePotion_Mana;
    public Sprite imageSniper;
    
    public static Inventory MyBag;
    private Sprite player;

    
    private void Start()
    {
        player = GetComponent<InventoryManager>().slotPrefab.slotImage.sprite;
        MyBag = new Inventory();
    }

    void Update()
    {
        PrintSprite();
    }
    
    public void AddNewItem(string itemName)
    {
        switch (itemName)
        {
            case "Boss_Weapon":
                MyBag.items.Add((itemName, MyBag.items.Count, imageBoss_Weapon));
                break;
            case "Banana":
                MyBag.items.Add((itemName, MyBag.items.Count, imageBanana));
                //InventoryManager.CreateNewItem(imageBanana);
                break;
            case "Cheese":
                MyBag.items.Add((itemName,MyBag.items.Count, imageCheese));
                //InventoryManager.CreateNewItem(imageCheese);
                break;
            case "Hamburger":
                MyBag.items.Add((itemName,MyBag.items.Count,imageHamburger));
                break;
            case "Gun":
                MyBag.items.Add((itemName,MyBag.items.Count,imageGun));
                break;
            case "Kit_Heal":
                MyBag.items.Add((itemName,MyBag.items.Count,imageKit_Heal));
                break;
            case "Ordinateur_Kali_Linux":
                MyBag.items.Add((itemName,MyBag.items.Count,imageKahli_Computer));
                break;
            case "Potion_Boost":
                MyBag.items.Add((itemName,MyBag.items.Count,imagePotion_Mana));
                break;
            case "Sniper":
                MyBag.items.Add((itemName,MyBag.items.Count,imageSniper));
                break;
        }
    }

    public void PrintSprite()
    {
        if (MyBag.items.Count > 0)
        {
            for (int i = 0; i < MyBag.items.Count; i++)
            {
                player = MyBag.items[i].Item3;
            }
        }
    }


    public static void RemoveSprite(int i)
    {
        if (MyBag.items.Count < i)
        {
            MyBag.items.Remove(MyBag.items[i]);
        }
    }
}
