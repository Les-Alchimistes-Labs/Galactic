using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkLauncher : MonoBehaviourPunCallbacks
{
    public InputField roomName;
    public Text feedbackText;


    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        roomName.text = "";
    }

    public override void OnConnectedToMaster()
    {
        Log_feedback("Connected to the Master.");
        PhotonNetwork.JoinLobby();
    }

    void Log_feedback(string message)
    {
        if (feedbackText is null)
        {
            return;
        }
        
        feedbackText.text += System.Environment.NewLine + message;
    }


    public void JoinOrCreateRoomButton()
    {
        if (roomName.text.Length <= 2)
        {
            Log_feedback("The room name length should be greater than 2");
            return;
        }
        
        Log_feedback("Joining room. Wait a moment.");
        RoomOptions options = new RoomOptions { MaxPlayers = 4};
        PhotonNetwork.JoinOrCreateRoom(roomName.text, options, default);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount < 4)
        {
            PhotonNetwork.LoadLevel(1);
        }
        else
        {
            Log_feedback("Failed : the number of players in this room is full");
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Log_feedback("Join failed. Maybe this room is full.");
    }

    public void QuitGame()
    {
        PhotonNetwork.LeaveRoom();
        UnityEngine.Device.Application.Quit();
    }
}
