using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class Game_Manager : MonoBehaviourPunCallbacks
{
    public GameObject player_prefab;

    void Start()
    {
        PhotonNetwork.Instantiate(player_prefab.name, new Vector3(0, 1, 0), Quaternion.identity, 0);
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