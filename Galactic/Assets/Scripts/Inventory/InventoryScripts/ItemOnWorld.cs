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
    private PhotonView view;
    private Player2 _player2;
    
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


    private void Start()
    {
        view = GetComponent<PhotonView>();
        _player2 = GetComponent<Player2>();
        MyBag = new Inventory();
    }

    void Update()
    {
        if (MyBag.items.Count != 0 && MyBag.items.Count < _player2.Personnage.Get_Inventory().Length)
        {
            var item = _player2.Personnage.Get_Inventory()[_player2.Personnage.Get_Inventory().Length - 1];
            AddNewItem(item.Name);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void AddNewItem(string itemStr)
    { 
        switch (itemStr)
        {
            case "Boss_Weapon":
                MyBag.items.Add(("Boss_Weapon", MyBag.items.Count, imageBoss_Weapon));
                InventoryManager.CreateItems(imageBoss_Weapon);
                break;
            case "Banana":
                MyBag.items.Add(("Banana", MyBag.items.Count, imageBanana));
                InventoryManager.CreateItems(imageBanana);
                break;
            case "Cheese":
                MyBag.items.Add(("Cheese", MyBag.items.Count, imageCheese));
                InventoryManager.CreateItems(imageCheese);
                break;
            case "Hamburger":
                MyBag.items.Add(("Hamburger", MyBag.items.Count, imageHamburger));
                InventoryManager.CreateItems(imageHamburger);
                break;
            case "Gun":
                MyBag.items.Add(("Gun", MyBag.items.Count, imageGun));
                InventoryManager.CreateItems(imageGun);
                break;
            case "Kit_Heal":
                MyBag.items.Add(("Kit_Heal", MyBag.items.Count, imageKit_Heal));
                InventoryManager.CreateItems(imageKit_Heal);
                break;
            case "Ordinateur_Kali_Linux":
                MyBag.items.Add(("Ordinateur_Kali_Linux", MyBag.items.Count, imageKahli_Computer));
                InventoryManager.CreateItems(imageKahli_Computer);
                break;
            case "Potion_Boost":
                MyBag.items.Add(("Potion_Boost", MyBag.items.Count, imagePotion_Mana));
                InventoryManager.CreateItems(imagePotion_Mana);
                break;
            case "Sniper":
                MyBag.items.Add(("Sniper", MyBag.items.Count, imageSniper));
                InventoryManager.CreateItems(imageSniper);
                break;
            default:
                MyBag.items.Add(("Banana", MyBag.items.Count, imageBanana));
                InventoryManager.CreateItems(imageBanana);
                break;
        }
    }
    
    
    public static void RemoveSprite(int i)
    {
        if (MyBag.items[i-1].Item1 is not null && MyBag.items.Count < i)
        {
            MyBag.items.Remove(MyBag.items[i-1]);
        }
    }
}
