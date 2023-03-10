using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ThirdPersonCamera : MonoBehaviour
{
    private CharacterController controller;
		
    private PhotonView _photonView;
    public GameObject player;
    private Vector3 offset = new Vector3(0f,6f,-4f);
    private Camera _camera;




    void Start ()
    {
        _camera = Camera.main;

    }

    void LateUpdate ()
    {
        _photonView = GetComponent<PhotonView>();
        _camera.transform.position = player.transform.position + offset ;

    }
}
