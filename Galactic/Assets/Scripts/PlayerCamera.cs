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
		private Vector3 offset = new Vector3(0f,6f,-4f);
		private Camera _camera;
		public GameObject poscam;

		

			
		void Start ()
		{
			_camera = Camera.main;
			_photonView = GetComponent<PhotonView>();

		}

		void LateUpdate ()
		{
			_photonView = GetComponent<PhotonView>();
			if ( _photonView.IsMine )
			{
				_camera.transform.position = poscam.transform.position;
				_camera.transform.LookAt(player.transform);
			}

		}
}