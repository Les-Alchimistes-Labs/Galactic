using System;
using UnityEngine;
using Photon.Realtime;
using System.Collections;
using personnage_class.Personage;
using Photon.Pun;
using ExitGames.Client.Photon.StructWrapping;
using Random = UnityEngine.Random;


public class Player2 : MonoBehaviour
{	
		private ItemOnWorld itemOnWorld;
		private Animator anim;
		private CharacterController controller;

		public float speed = 600.0f;
		public float turnSpeed = 400.0f;
		private Vector3 moveDirection = Vector3.zero;
		public float gravity = 20.0f;
		public Personnage Personnage;
		public GameObject littelMonster;
		//public GameObject FinalBoss;
		private PhotonView _photonView;
		public EnumChoice Choice;
		public EnumPlayer Player;
		public Vector3 Spwan;
		public GameObject Map;
		


		
		void Start ()
		{
			
			itemOnWorld = GetComponent<ItemOnWorld>();
			controller = GetComponent <CharacterController>();
			anim = gameObject.GetComponentInChildren<Animator>();
			Spwan = new Vector3(0, 2, 0);
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
			//EnemyGenerator.EnemyGeneratore(EnumMonster.BossFinal, FinalBoss,transform ,Personnage.level,0,0 );

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
						int x = (int) (transform.position.x + Random.Range(-7, 7)), z = (int) (transform.position.z + Random.Range(-7, 7));
						if ( x>0 && z>0 && Map.GetComponent<MapGenerator>().matrixCase.GetLength(0)>x &&Map.GetComponent<MapGenerator>().matrixCase.GetLength(1)>z && Map.GetComponent<MapGenerator>().matrixCase[x,z].Item2 == EnumsItem.Empty  )
							EnemyGenerator.EnemyGeneratore(EnumMonster.LittelMonster, littelMonster, transform,Personnage.level,x,z );
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
					ItemOnWorld.RemoveSprite(1);
				}
				if (Input.GetKeyDown("2"))
				{
					Personnage.PosInv = 2;
					ItemOnWorld.RemoveSprite(2);
				}
				if (Input.GetKeyDown("3"))
				{
					Personnage.PosInv = 3;
					ItemOnWorld.RemoveSprite(3);
				}
				if (Input.GetKeyDown("4"))
				{
					Personnage.PosInv = 4;
					ItemOnWorld.RemoveSprite(4);
				}

				if (!Personnage.IsAlive() || transform.position.y < -5)
				{
					Personnage.Add_Life(Personnage.GetMaxLife());
					transform.position = Spwan;
				}



			}
			else if (Choice == EnumChoice.None && _photonView.IsMine) // to change with gui choice
			{
				if (Input.GetKeyDown("f") || Game_Manager.attack)
				{
					Choice = EnumChoice.Attack;
					Game_Manager.attack = false;
				}
				if (Input.GetKeyDown("r") || Game_Manager.heal_Boost)
				{
					Choice = EnumChoice.HealorBoost;
					Game_Manager.heal_Boost = false;
				}
				if (Input.GetKeyDown("c") || Game_Manager.changeGun)
				{
					Choice = EnumChoice.ChangeGun;
					Game_Manager.changeGun = false;
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

			else if ((other.tag == "Equipement" && (Input.GetKey("e") || Input.GetKeyDown("e"))))
			{

				Item item = other.gameObject.GetComponent<ItemInGame>().Item;
				bool take =Personnage.Took(item);
				if (take)
				{
					itemOnWorld.AddNewItem(item.Name);
					PhotonNetwork.Destroy(other.gameObject);
				}

			}
			



		}
		
		
		public void OnTriggerStay(Collider other)
		{
			if ((other.tag == "Equipement" && (Input.GetKey("e") || Input.GetKeyDown("e"))))
			{

				Item item = other.gameObject.GetComponent<ItemInGame>().Item;
				bool take =Personnage.Took(item);
				if (take)
				{
					itemOnWorld.AddNewItem(item.Name);
					PhotonNetwork.Destroy(other.gameObject);
				}


			}
			else if (other.tag == "Enemy")
			{
				Personnage.canMove = false;
				Debug.Log($"in and {Personnage.canMove}");
				Personnage.inFight = true;
				anim.SetInteger ("AnimationPar", 0);
				
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