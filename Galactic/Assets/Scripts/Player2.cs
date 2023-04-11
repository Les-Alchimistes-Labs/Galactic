using System;
using UnityEngine;
using Photon.Realtime;
using System.Collections;
using personnage_class.Personage;
using Photon.Pun;
using DefaultNamespace;
using ExitGames.Client.Photon.StructWrapping;
using Random = UnityEngine.Random;


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
		public EnumChoice Choice;



		void Start () {
			controller = GetComponent <CharacterController>();
			anim = gameObject.GetComponentInChildren<Animator>();
			Personnage = new Soldat("test");
			_photonView = GetComponent<PhotonView>();
			Choice = EnumChoice.None;

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

				float turn = Input.GetAxis("Horizontal");
				transform.Rotate(0, turn * turnSpeed * Time.deltaTime, 0);
				
				controller.Move(moveDirection * Time.deltaTime);
				moveDirection.y -= gravity * Time.deltaTime;
				if ((moveDirection.x != 0 || moveDirection.z != 0) && !Personnage.InSafeZone )
				{
					if (Random.Range(0, 1001) == 10)
					{
						EnemyGenerator.EnemyGeneratore(EnumMonster.LittelMonster, littelMonster, transform,Personnage.level );
					}

					// LittelMonsterGenerator.tryspawmmonster(littelMonster, this.transform);
				}
				else if (Input.GetKeyDown("q"))
				{
					Item old = Personnage.Trow(Personnage.PosInv);
					if (old != null)
					{
						GameObject temp;
						temp = PhotonNetwork.Instantiate(old.Name,transform.position,transform.rotation,0);
						temp.GetComponent<ItemInGame>().Item = old;
					}
				}



			}
			else if (Choice == EnumChoice.None) // to change with gui choice
			{
				Choice = EnumChoice.Attack;
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

			else if (other.tag == "Equipement" && Input.GetKeyDown("e"))
			{

				Item item = other.gameObject.GetComponent<ItemInGame>().Item;
				bool take =Personnage.Took(item);
				if (take)
					PhotonNetwork.Destroy(other.gameObject);
				
			}
			



		}
		
		
		public void OnTriggerStay(Collider other)
		{
			if (other.tag == "Equipement" && Input.GetKeyDown("e"))
			{

				Item item = other.gameObject.GetComponent<ItemInGame>().Item;
				bool take =Personnage.Took(item);
				if (take)
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
