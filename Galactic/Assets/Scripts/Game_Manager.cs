using personnage_class.Personage;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Random = System.Random;


public class Game_Manager : MonoBehaviourPunCallbacks
{
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
        Level = 0;
        
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