using System.Collections;
using System.Collections.Generic;
using personnage_class.Personage;
using UnityEditor;
using UnityEngine;

public class ItemInGame : MonoBehaviour
{
    public Item Item;

    public GameObject gameObject;
    // Start is called before the first frame update
    void Start()
    {
        switch (gameObject.name)
        {
            case "Boss_Weapon":
                Item = new Boss_Weapon(1,1,1);
                break;
            case "Banana Variant":
                Item = new Food(1,1);
                break;
            case "Cheese Variant":
                Item = new Food(1,1);
                break;
            case "Hamburger Variant":
                Item = new Food(1,1);
                break;
            case "Gun":
                Item = new Gun(1,1,1);
                break;
            case "Kit_Heal":
                Item = new Kit_Heal(1,1);
                break;
            case "Ordinateur_Kali_Linux":
                Item = new Ordinateur_Kali_Linux(1,1);
                break;
            case "Potion_Boost":
                Item = new Potion_Boost(1,true,1,EnumsItem.Boost);
                break;
            case "Sniper":
                Item = new Sniper_a(1,1,1);
                break;
            case "":
                Item = new Boss_Weapon(1,1,1);
                break;



        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
