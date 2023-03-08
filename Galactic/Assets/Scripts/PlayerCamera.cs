using System;
using UnityEngine;
using Photon.Realtime;
using System.Collections;
using personnage_class.Personage;
using Photon.Pun;


public class PlayerCamera : MonoBehaviour {

		private CharacterController controller;
		
		private PhotonView _photonView;
		public GameObject player;
		private Vector3 offset = new Vector3(0.25f,0.3f,-0.5f);


		void Start () 
		{
			
			controller = GetComponent <CharacterController>();

		}

		void LateUpdate ()
		{
			
			_photonView = GetComponent<PhotonView>();
			if ( _photonView.IsMine )
			{
				transform.position = player.transform.position + offset;
			}
			
		}
}
