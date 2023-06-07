using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;

public class RoomListManager : MonoBehaviourPunCallbacks
{
    public GameObject roomNamePrefab;
    public Transform gridLayout;


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for (int i = 0; i < gridLayout.childCount; i++)
        {
            if (gridLayout.GetChild(i).gameObject.GetComponentInChildren<Text>().text == roomList[i].Name)
            {
                Destroy(gridLayout.GetChild(i).gameObject);

                if (roomList[i].PlayerCount == 0)
                {
                    roomList.Remove(roomList[i]);
                }
            }
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            for (int j =  i +1; j < roomList.Count; j++)
            {
                if (roomList[i].Name == roomList[j].Name)
                {
                    roomList.Remove(roomList[j]);
                }
            }
        }
        
        foreach (var room in roomList)
        {
            GameObject newroom = Instantiate(roomNamePrefab, gridLayout);

            newroom.GetComponentInChildren<Text>().text = room.Name;
            
            newroom.transform.SetParent(gridLayout);
        }
    }
}
