using System;
using System.IO;
using personnage_class.Personage;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = System.Random;


public class Game_Manager : MonoBehaviourPunCallbacks
{
    public bool use_old_inventory = true;
    public static bool attack;
    public static bool heal_Boost;
    public static bool changeGun;

    public GameObject Choice_Canvas;
    public GameObject MyInventory;
    bool TouchButton;
    public GameObject Soldat_prefab;
    public GameObject Sniper_prefab;
    public GameObject Canonnier_prefab;
    public GameObject Hacker_prefab;
    public GameObject GUI;
    private GameObject test;
    public GameObject PlayerUI;
    public int Level;
    public GameObject PauseMenu;
    
    public GameObject MenuButton;
    private bool touch_MenuButton;

    public static int desactivateAccessObjectFinalLevel = 0;
    
    void Start()
    {
        Choice_Canvas.SetActive(false);
        attack = false;
        heal_Boost = false;
        changeGun = false;
        Level = 0;
        TouchButton = false;
        touch_MenuButton = false;
    }
    
    
    public void OpenMenu()
    {
        touch_MenuButton = true;
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<PhotonView>().IsMine)
            {
                player.GetComponent<Player2>().Personnage.canMove = false;
            }
        }
    }

    public void OpenInventory()
    {
        TouchButton = true;
    }

    public void CloseInventory()
    {
        TouchButton = false;
    }

    void GetInventory()
    {
        if (TouchButton)
        {
            MyInventory.SetActive(true);
        }
        else
        {
            MyInventory.SetActive(false);
        }
    }

    public void SelectAttack()
    {
        attack = true;
    }

    public void SelectChangeGun()
    {
        changeGun = true;
    }

    public void SelectHealOrBoost()
    {
        heal_Boost = true;
    }

    




public void Select_Soldat()
    {
        test = PhotonNetwork.Instantiate(Soldat_prefab.name, new Vector3(0, 1, -10), Quaternion.identity, 0); 
        GUI.SetActive(false);
        PlayerUI.SetActive(true);

    }
    public void Select_Sniper()
    {
        test =PhotonNetwork.Instantiate(Sniper_prefab.name, new Vector3(0, 1, -10), Quaternion.identity, 0);
        GUI.SetActive(false);
        PlayerUI.SetActive(true);
    }
    public void Select_Canonnier()
    {
        test =PhotonNetwork.Instantiate(Canonnier_prefab.name, new Vector3(0, 1, -10), Quaternion.identity, 0);
        GUI.SetActive(false);
        PlayerUI.SetActive(true);
        
    }
    public void Select_Hacker()
    {
        test =PhotonNetwork.Instantiate(Hacker_prefab.name, new Vector3(0, 1, -10), Quaternion.identity, 0);
        GUI.SetActive(false);
        PlayerUI.SetActive(true);
    }
    
    
    void Update()
    {
        if (touch_MenuButton)
        {
            PauseMenu.SetActive(true);
        }
        else
        {
            PauseMenu.SetActive(false);
        }

        desactivateAccessObjectFinalLevel = GameObject.FindGameObjectsWithTag("intermidateboss").Length % 4; 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //QuitApplication();
            if (PauseMenu.activeSelf)
            {
                //PauseMenu.SetActive(false);
                touch_MenuButton = false;
                foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
                {
                    if (player.GetComponent<PhotonView>().IsMine)
                    {
                        player.GetComponent<Player2>().Personnage.canMove = true;
                    }
                }
            }
            else
            {
                //PauseMenu.SetActive(true);
                touch_MenuButton = true;
                foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
                {
                    if (player.GetComponent<PhotonView>().IsMine)
                    {
                        player.GetComponent<Player2>().Personnage.canMove = false;
                    }
                } 
            }

        }

        if (Player_UI.fight && !Player_UI.move)
        {
            Choice_Canvas.SetActive(true);
        }
        else
        {
            Choice_Canvas.SetActive(false);
        }
        
        GetInventory();
    }




    public void PlayerEnterRoom(Player player)
    {
        float rd = new Random().Next(1,3);
        
        if (rd <= 2)
        {
            print(player.name + " appears suddenly in the room .");
        }
        else
        {
            print(player.name +": Hello !");
        }
    }

    public void OnPlayerLeftRoom(Player player)
    {
        float rd = new Random().Next(1, 3);
        
        if (rd <= 2)
        {
            print(player.name + " disappeared from the room .");
        }
        else
        {
            print(player.name + ": Bye bye .");
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    
    public void QuitRoom()
    {
        touch_MenuButton = false;
        //PauseMenu.SetActive(false);
        //PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("Connection");
        
    }
    
    public void QuitGame()
    {
        touch_MenuButton = false;
        //PauseMenu.SetActive(false);
        PhotonNetwork.LeaveRoom();
        UnityEngine.Device.Application.Quit();
    }
    
    public void QuitRoomToMenu()
    {
        touch_MenuButton = false;
        //PauseMenu.SetActive(false);
        string path = Application.dataPath;

        Player2 p = null;
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<PhotonView>().IsMine)
            {
                p = player.GetComponent<Player2>();
            }
        }
        if (!File.Exists(path + $"{p.Player}.txt"))
        {
            // Créer un nouveau fichier
            using (FileStream fileStream = File.Create(path + $@"\{p.Player}.txt"))
            {
            }
        }
        if (! (p is null))
        {
            StreamWriter sw = new StreamWriter( path + @$"\{p.Player}.txt");
            sw.Write($"type:{p.Player}\n");
            sw.Write($"name:{p.Personnage.name}\n");
            if (p.Personnage.pricipale_Weapon != null)
            {
                sw.Write($"Principal_Weapon:{p.Personnage.pricipale_Weapon.Name}\n");
            }
            if ( p.Personnage.level != 0)
                sw.Write($"level:{p.Personnage.level}\n");
            if (p.Personnage.GetXP() != 0)
                sw.Write($"xp:{p.Personnage.GetXP()}\n");
            if (p.Personnage.Getlife != 0)
                sw.Write($"life:{p.Personnage.Getlife}\n");
            string inventory = "";
            foreach (var item in p.Personnage.Get_Inventory())
            {
                if (!(item is null))
                    inventory +=  " " + item.Name ;
            }
            if (inventory != "")
            {
                sw.Write("inventory:"+inventory + "\n");
            }
            sw.Close();
        }

        //PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("Connection");
    }
    
    public void Resume()
    {
        touch_MenuButton = false;
        //PauseMenu.SetActive(false);
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<PhotonView>().IsMine)
            {
                player.GetComponent<Player2>().Personnage.canMove = true;
            }
        }
        
    }
    
}
