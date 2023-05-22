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
    public static bool use_old_inventory = true;
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

    void Start()
    {
        Choice_Canvas.SetActive(false);
        attack = false;
        heal_Boost = false;
        changeGun = false;
        Level = 0;
        TouchButton = false;
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

    private void Update_Player(Player2 p, EnumPlayer player)
    {
        string path = Application.dataPath;
        switch (player)
        {
            case EnumPlayer.Soldat:
                p.Personnage = new Soldat("test");
                break;
            case EnumPlayer.Sniper:
                p.Personnage = new Sniper("test");
                break;
            case EnumPlayer.Canonnier:
                p.Personnage = new Canonnier("test");
                break;
            case EnumPlayer.Hacker:
                p.Personnage = new Hacker("test");
                break;

        }
        if (use_old_inventory && File.Exists(path + $@"\{p.Player}.txt"))
        {
            int nbligne = 0;

            using (StreamReader sr = new StreamReader(path + $@"\{p.Player}.txt"))
            {
                string line;
                long lineNumber = 0;

                // Parcourir chaque ligne du fichier
                while ((line = sr.ReadLine()) != null)
                {
                    nbligne++;
                    while (line != null && line != "type:" + p.Player)
                    {
                        line = sr.ReadLine();
                        nbligne++;
                    }

                    Item item = null;
                    while ((line = sr.ReadLine()) != null && !line.StartsWith("type:"))
                    {
                        nbligne++;
                        if (line.StartsWith("inventory:"))
                        {
                            p.Personnage.Reset_Inventory();
                            string element = "";
                            int i = 11;
                            while (i < line.Length)
                            {
                                while (i < line.Length && line[i] != ' ' && line[i] != '\n' )
                                {
                                    element += line[i];
                                    i++;
                                }


                                item = null;

                                switch (element)
                                {
                                    case "Boss_Weapon":
                                        item = new Boss_Weapon(1,9,1);
                                        break;
                                    case "Banana":
                                        item = new Food("Banana",1,3);
                                        break;
                                    case "Cheese":
                                        item = new Food("Cheese",1,5);
                                        break;
                                    case "Hamburger":
                                        item = new Food("Hamburger",1,10);
                                        break;
                                    case "Gun":
                                        item = new Gun(1,3,1);
                                        break;
                                    case "Kit_Heal":
                                        item = new Kit_Heal(1,9);
                                        break;
                                    case "Ordinateur_Kali_Linux":
                                        item = new Ordinateur_Kali_Linux(4,1);
                                        break;
                                    case "Potion_Boost":
                                        item = new Potion_Boost("Potion_Mana",1,true,1,EnumsItem.Boost);
                                        break;
                                    case "Sniper":
                                        item = new Sniper_a(5,1,1);
                                        break;
                                }

                                element = "";
                                i++;
                                p.Personnage.Took(item);
                            }
                        }
                        else if (line.StartsWith("Principal_Weapon:"))
                        {
                            line = line.Remove(0, 17);
                            p.Personnage.name = line;
                        }
                        else if (line.StartsWith("name:"))
                        {
                            line = line.Remove(0, 5);
                            p.Personnage.name = line;
                        }
                        else if (line.StartsWith("level:"))
                        {
                            line = line.Remove(0, 6);
                            p.Personnage.level = int.Parse(line);
                        }
                        else if (line.StartsWith("xp:"))
                        {
                            line = line.Remove(0, 3);
                            p.Personnage.Add_Xp(int.Parse(line));
                        }
                        else if (line.StartsWith("life:"))
                        {
                            line = line.Remove(0, 5);
                            p.Personnage.Remove_Life(p.Personnage.Getlife - int.Parse(line));
                        }
                    }
                }
                sr.Close();
            }
        }
    }




public void Select_Soldat()
    {
        test = PhotonNetwork.Instantiate(Soldat_prefab.name, new Vector3(0, 1, -10), Quaternion.identity, 0);
        GUI.SetActive(false);
        PlayerUI.SetActive(true);
        Update_Player(test.GetComponent<Player2>(),EnumPlayer.Soldat);


    }
    public void Select_Sniper()
    {
        test =PhotonNetwork.Instantiate(Sniper_prefab.name, new Vector3(0, 1, -10), Quaternion.identity, 0);
        GUI.SetActive(false);
        PlayerUI.SetActive(true);
        Update_Player(test.GetComponent<Player2>(),EnumPlayer.Sniper);
    }
    public void Select_Canonnier()
    {
        test =PhotonNetwork.Instantiate(Canonnier_prefab.name, new Vector3(0, 1, -10), Quaternion.identity, 0);
        GUI.SetActive(false);
        PlayerUI.SetActive(true);
        Update_Player(test.GetComponent<Player2>(),EnumPlayer.Canonnier);
        
    }
    public void Select_Hacker()
    {
        test =PhotonNetwork.Instantiate(Hacker_prefab.name, new Vector3(0, 1, -10), Quaternion.identity, 0);
        GUI.SetActive(false);
        PlayerUI.SetActive(true);
        Update_Player(test.GetComponent<Player2>(),EnumPlayer.Hacker);
        
    }
    
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //QuitApplication();
            if (PauseMenu.activeSelf)
            {
                PauseMenu.SetActive(false);
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
                PauseMenu.SetActive(true);
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
        PauseMenu.SetActive(false);
        //PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("Connection");
        
    }
    
    public void QuitGame()
    {
        PauseMenu.SetActive(false);
        PhotonNetwork.LeaveRoom();
        UnityEngine.Device.Application.Quit();
    }
    
    public void QuitRoomToMenu()
    {
        PauseMenu.SetActive(false);
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
        PauseMenu.SetActive(false);
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<PhotonView>().IsMine)
            {
                player.GetComponent<Player2>().Personnage.canMove = true;
            }
        }
        
    }
    
    
    
}