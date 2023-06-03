using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using personnage_class.Personage;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class PrincipleWeapon : MonoBehaviour
{

    public Transform here;
    public Inventory_inside Empty_image;
    public Inventory_inside Boss_Weapon_image;
    public Inventory_inside Gun_image;
    public Inventory_inside Sniper_image;

    public static Item? thePrincipleWeapon;
    private Inventory_inside?  Weapon;
    
    void Update()
    {
        thePrincipleWeapon = Player_UI.PrincipleWeapon;
        ShowPrincipleWeapon();
    }

    private void ShowPrincipleWeapon()
    {
        if (thePrincipleWeapon is null)
        {
            if (Weapon is not null)
            {
                Destroy(Weapon.gameObject);
            }
            Weapon = Instantiate(Empty_image,here);
            Weapon.gameObject.SetActive(true);
        }
        else
        {
            switch (thePrincipleWeapon)
            {
                case Boss_Weapon:
                    if (Weapon is not null)
                    {
                        Destroy(Weapon.gameObject);
                    }
                    Weapon = Instantiate(Boss_Weapon_image,here);
                    Weapon.gameObject.SetActive(true);
                    break;
                case Gun:
                    if (Weapon is not null)
                    {
                        Destroy(Weapon.gameObject);
                    }
                    Weapon = Instantiate(Gun_image,here);
                    Weapon.gameObject.SetActive(true);
                    break;
                case Sniper_a:
                    if (Weapon is not null)
                    {
                        Destroy(Weapon.gameObject);
                    }
                    Weapon = Instantiate(Sniper_image,here);
                    Weapon.gameObject.SetActive(true);
                    break;
            }
        }
    }
}
