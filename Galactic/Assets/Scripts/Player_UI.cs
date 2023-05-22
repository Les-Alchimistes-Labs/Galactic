using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player_UI : MonoBehaviour
{
    private PhotonView view;
    private Player2 _player2;
    public Canvas canvas;
    private PlayerInformation_UI UI;
    
    public static float actual_hp;
    public static float Max_hp;
    public static float actual_exp;
    public static float Max_exp;
    public static string Name="";
    public static float life;
    public static float exp;
    public static int lv;
    public static bool fight;
    public static bool move;
    
    
    void Start()
    {
        view = GetComponent<PhotonView>();
        _player2 = GetComponent<Player2>(); 
        UI= canvas.GetComponent<PlayerInformation_UI>();
        Name = _player2.Personnage.name;
        life = 1;
        exp = 0;
        fight = false;
        move = false;
    }

    
    void Update()
    {
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
