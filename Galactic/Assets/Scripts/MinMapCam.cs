using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MinMapCam : MonoBehaviour
{
    private CharacterController controller;
    public GameObject player;
    private Vector3 offset = new Vector3(0f,10f,0);
    public Camera _camera;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] joueur = GameObject.FindGameObjectsWithTag("Player");
        foreach (var val in joueur)
        {
            if (val.GetComponent<PhotonView>().IsMine)
            {
                player = val;
            }
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player == null)
        {
            GameObject[] joueur = GameObject.FindGameObjectsWithTag("Player");
            foreach (var val in joueur)
            {
                if (val.GetComponent<PhotonView>().IsMine)
                {
                    player = val;
                }
            } 
        }
        else
        {
            _camera.transform.position = player.transform.position + offset ;
        }
        
    }
}
