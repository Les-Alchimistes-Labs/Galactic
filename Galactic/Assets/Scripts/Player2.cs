using System;
using UnityEngine;
using Photon.Realtime;
using System.Collections;
using personnage_class.Personage;
using Photon.Pun;
using DefaultNamespace;


public class Player2 : MonoBehaviour {

		private Animator anim;
		private CharacterController controller;

		public float speed = 600.0f;
		public float turnSpeed = 400.0f;
		private Vector3 moveDirection = Vector3.zero;
		public float gravity = 20.0f;
		public Personnage Personnage;
		public GameObject littelMonster;
		private PhotonView _photonView;



		void Start () {
			controller = GetComponent <CharacterController>();
			anim = gameObject.GetComponentInChildren<Animator>();
			Personnage = new Soldat("test");
			_photonView = GetComponent<PhotonView>();

		}

		void Update (){
			if (Personnage.canMove && _photonView.IsMine  )
			{
				if (Input.GetKey ("w")) {
					anim.SetInteger ("AnimationPar", 1);
				}  else {
					anim.SetInteger ("AnimationPar", 0);
				}

				if(controller.isGrounded){
					moveDirection = transform.forward * Input.GetAxis("Vertical") * speed;
				}

				Personnage.Took(new Kit_Heal(5,0));
				Personnage.Took(new Kit_Heal(5,0));
				Console.WriteLine(Personnage.Get_Inventory()[0]);
				Personnage.Remove_Life(10);
				Personnage.Use(0);
				Personnage.Trow(1);
				Console.WriteLine(Personnage.Get_Inventory()[0]);
				
				float turn = Input.GetAxis("Horizontal");
				transform.Rotate(0, turn * turnSpeed * Time.deltaTime, 0);
				
				controller.Move(moveDirection * Time.deltaTime);
				moveDirection.y -= gravity * Time.deltaTime;
				if ((moveDirection.x != 0 || moveDirection.z != 0) && !Personnage.InSafeZone )
				{ 
					LittelMonsterGenerator.tryspawmmonster(littelMonster, this.transform);
				}



			}
			
		}

		
		
		public void OnTriggerEnter(Collider other)
		{
			if (other.tag == "Enemy")
			{
				Personnage.canMove = false;
				Debug.Log($"in and {Personnage.canMove}");
				Personnage.inFight = true;
				anim.SetInteger ("AnimationPar", 0);
				
			}
			else if (other.tag == "Respawn" && Personnage != null)
			{
				Personnage.InSafeZone = true;
				Debug.Log($"in and {Personnage.InSafeZone}");
			}

			else if (other.tag == "Equipement" && Input.GetKey("e"))
			{

				Item item = other.GetComponent<ItemInGame>().Item;
				Personnage.Took(item);
				PhotonNetwork.Destroy(other.gameObject);
				
			}
			



		}


		public void OnTriggerExit(Collider other)
		{
			if (other.tag == "Enemy")
			{
				Personnage.canMove = true;
				Personnage.inFight = false;
				Debug.Log($"out and {Personnage.canMove}");
			}
			if (other.tag == "Respawn")
			{
				
				Personnage.InSafeZone = false;
				Debug.Log($"out and {Personnage.InSafeZone}");
			}

		}


}
