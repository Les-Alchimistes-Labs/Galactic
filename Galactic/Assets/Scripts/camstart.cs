using System;
using UnityEngine;
using Photon.Realtime;
using System.Collections;
using personnage_class.Personage;
using Photon.Pun;


public class CamStart : MonoBehaviour {
    
    
   public Camera Camera;




   void Start ()
    {

    }

    void LateUpdate ()
    {
        Camera.transform.Rotate(0,0.01f,0);

    }
}