using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


public class MovePlayer : MonoBehaviour
{
    // attributs modifiables
    
    public float rotate_speed = 160f;
    public float move_speed = 50f;
    private PhotonView _photonView;


    void Start()
    {
        _photonView = GetComponent<PhotonView>();
    }


    // Z -> déplacer vers le haut
    // S -> déplacer vers le bas
    // D -> déplacer vers la droite
    // Q -> déplacer vers la gauche
    // Flèche droite -> tourner vers la droite
    // Flèche gauche -> tourner vers la gauche
    
    void Update()
    {
        if (_photonView.IsMine)
        {
            if (Input.GetKey(KeyCode.Z))
            {
                transform.Translate(Vector3.forward * (Time.deltaTime * move_speed));
            }


            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(Vector3.back * (move_speed * Time.deltaTime));
            }


            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * (move_speed * Time.deltaTime));
            }


            if (Input.GetKey(KeyCode.Q))
            {
                transform.Translate(Vector3.left * (move_speed * Time.deltaTime));
            }


            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(new Vector3(0, 1, 0) * (rotate_speed * Time.deltaTime));
            }


            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(new Vector3(0, -1, 0) * (rotate_speed * Time.deltaTime));
            }
        }
    }
}