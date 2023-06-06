using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using personnage_class.Personage;
using UnityEngine;
using Photon.Pun;

public class Player_UI : MonoBehaviour
{
    private PhotonView view;
    private Player2 _player2;
    //public Canvas canvas;
    //private PlayerInformation_UI UI;
    
    //Player Information
    public static float actual_hp;
    public static float Max_hp;
    public static float actual_exp;
    public static float Max_exp;
    public static string Name = "";
    public static float life;
    public static float exp;
    public static int lv;
    public static bool fight;
    public static bool move;

    // About items
    public static Item[]? inv = new Item[8];
    public static Item? PrincipleWeapon;
    
    
    
    void Start()
    {
        //UI= canvas.GetComponent<PlayerInformation_UI>();
        life = 1;
        exp = 0;
        fight = false;
        move = true;
        view = GetComponent<PhotonView>();
        _player2 = GetComponent<Player2>(); 
    }

    
    void Update()
    {
        if (_player2 is not null)
        {
            switch (_player2.Personnage)
            {
                case Canonnier:
                    Name = "Cannonier";
                    break;
                case Sniper:
                    Name = "Sniper";
                    break;
                case Soldat:
                    Name = "Soldat";
                    break;
                case Hacker:
                    Name = "Hacker";
                    break;
                default:
                    Name = "Who am I ?";
                    break;
            }
                    
            inv = _player2.Personnage.Get_Inventory();
            PrincipleWeapon = _player2.Personnage.pricipale_Weapon;
        }


        
        if (view.IsMine)
        {
            fight = _player2.Personnage.inFight;
            move = _player2.Personnage.canMove;
            lv = _player2.Personnage.level;
            actual_hp = _player2.Personnage.Getlife;
            Max_hp = _player2.Personnage.MaxLife;
            actual_exp = _player2.Personnage.GetXP();
            Max_exp = _player2.Personnage.Maxxp;
            life = actual_hp / Max_hp;
            exp = actual_exp / Max_exp;
            //canvas.GetComponent<PlayerInformation_UI>().level = _player2.Personnage.level;
        }
    }
}
