using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class MyLauncher : MonoBehaviourPunCallbacks
{
    public Button button_log;
    public Text feedbackText;
    private byte maxPlayersPerRoom = 4;

    static bool isConnecting;
    
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    
    void Log_feedback(string message)
    {
        if (feedbackText is null)
        {
            return;
        }
        
        feedbackText.text += System.Environment.NewLine + message;
    }
    
    public void Connect()
    {
        feedbackText.text = "";
        isConnecting = true;
        button_log.interactable = false;

        if (PhotonNetwork.IsConnected)
        {
            Log_feedback("Joining room. Wait a moment.");
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            Log_feedback("Connecting. Wait a moment.");
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            Log_feedback("Try to join random room");
            PhotonNetwork.JoinRandomRoom();
        }
    }
    
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Log_feedback("Failed: no room to join. Creating a new room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom});
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public override void OnDisconnected(DisconnectCause cause)
    {
        Log_feedback("Disconnected: "+cause);
        
        isConnecting = false;
        button_log.interactable = true;

    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public override void OnJoinedRoom()
    {
        Log_feedback("Joined a room with "+PhotonNetwork.CurrentRoom.PlayerCount+" Player(s)");

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.LoadLevel("SampleScene");
        }
    }

    public static bool isConnect()
    {
        return isConnecting;
    }
}