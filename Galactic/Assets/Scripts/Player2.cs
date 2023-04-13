using System;
using UnityEngine;
using Photon.Realtime;
using System.Collections;
using personnage_class.Personage;
using Photon.Pun;
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
		public GameObject FinalBoss;
		private PhotonView _photonView;
		public EnumChoice Choice;
		public EnumPlayer Player;


		
		void Start () {
			controller = GetComponent <CharacterController>();
			anim = gameObject.GetComponentInChildren<Animator>();
			
			switch (Player)
				{
					case EnumPlayer.Soldat:
						Personnage = new Soldat("test");
						break;
					case EnumPlayer.Sniper:
						Personnage = new Sniper("test");
						break;
					case EnumPlayer.Canonnier:
						Personnage = new Canonnier("test");
						break;
					case EnumPlayer.Hacker:
						Personnage = new Hacker("test");
						break;
                
				}

			_photonView = GetComponent<PhotonView>();
			Choice = EnumChoice.None;
			EnemyGenerator.EnemyGeneratore(EnumMonster.BossFinal, FinalBoss,transform ,Personnage.level );

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

				}
				if (Input.GetKeyDown("q"))
				{
					Item old = Personnage.Trow(Personnage.PosInv);
					if (old != null)
					{
						GameObject temp;
						temp = PhotonNetwork.Instantiate(old.Name,transform.position,transform.rotation,0);
						temp.GetComponent<ItemInGame>().Item = old;
					}
				}

				if (Input.GetKeyDown("1"))
				{
					Personnage.PosInv = 1;
				}
				if (Input.GetKeyDown("2"))
				{
					Personnage.PosInv = 2;
				}
				if (Input.GetKeyDown("3"))
				{
					Personnage.PosInv = 3;
				}
				if (Input.GetKeyDown("4"))
				{
					Personnage.PosInv = 4;
				}

				if (!Personnage.IsAlive() || transform.position.y < -5)
				{
					Personnage.Add_Life(Personnage.GetMaxLife());
					transform.position = new Vector3(0, 1, 0);
				}



			}
			else if (Choice == EnumChoice.None && _photonView.IsMine) // to change with gui choice
			{
				if (Input.GetKeyDown("f"))
				{
					Choice = EnumChoice.Attack;
				}
				if (Input.GetKeyDown("r"))
				{
					Choice = EnumChoice.HealorBoost;
				}
				if (Input.GetKeyDown("c"))
				{
					Choice = EnumChoice.ChangeGun;
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
