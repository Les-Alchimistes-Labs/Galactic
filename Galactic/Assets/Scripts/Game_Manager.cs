using personnage_class.Personage;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Random = System.Random;


public class Game_Manager : MonoBehaviourPunCallbacks
{
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
    
    public void Select_Soldat()
    {
        test = PhotonNetwork.Instantiate(Soldat_prefab.name, new Vector3(0, 1, 2), Quaternion.identity, 0);
        GUI.SetActive(false);
        PlayerUI.SetActive(true);
    }
    public void Select_Sniper()
    {
        test =PhotonNetwork.Instantiate(Sniper_prefab.name, new Vector3(0, 1, 2), Quaternion.identity, 0);
        GUI.SetActive(false);
        PlayerUI.SetActive(true);
    }
    public void Select_Canonnier()
    {
        test =PhotonNetwork.Instantiate(Canonnier_prefab.name, new Vector3(0, 1, 2), Quaternion.identity, 0);
        GUI.SetActive(false);
        PlayerUI.SetActive(true);
        
    }
    public void Select_Hacker()
    {
        test =PhotonNetwork.Instantiate(Hacker_prefab.name, new Vector3(0, 1, 2), Quaternion.identity, 0);
        GUI.SetActive(false);
        PlayerUI.SetActive(true);
        
    }
    
    
    void Update()
    {
        if (Player_UI.fight && !Player_UI.move)
        {
            Choice_Canvas.SetActive(true);
        }
        else
        {
            Choice_Canvas.SetActive(false);
        }
        
        GetInventory();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
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
}