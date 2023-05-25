using System;
using UnityEngine;
using Photon.Realtime;
using System.Collections;
using personnage_class.Personage;
using Photon.Pun;
using Random = UnityEngine.Random;
using System;
using System.IO;


public class Player2 : MonoBehaviour
{	
		
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
		public KeyCode[] bind = new KeyCode[] { KeyCode.W,KeyCode.S,KeyCode.A,KeyCode.D,KeyCode.Q,KeyCode.E};
		public GameObject Enemy;


		
		void Start ()
		{
			
			controller = GetComponent <CharacterController>();
			anim = gameObject.GetComponentInChildren<Animator>();
			Spwan = new Vector3(10, 2, -10);
			
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
			Update_Player();
		}

		void Update (){
			if (_photonView.IsMine   )
			{
				if (Personnage.canMove)
				{
					if (Input.GetKey (bind[0])) {
						anim.SetInteger ("AnimationPar", 1);
					}  else {
						anim.SetInteger ("AnimationPar", 0);
					}

				 
					if(controller.isGrounded){
						moveDirection = transform.forward * Input.GetAxis("Vertical")* speed;
					}

					float turn = Input.GetAxis("Horizontal");
					transform.Rotate(0, turn * turnSpeed * Time.deltaTime, 0);
					moveDirection.y -= gravity * Time.deltaTime; 
					controller.Move(moveDirection * Time.deltaTime);
					if ((moveDirection.x!= 0 || moveDirection.z!= 0) && !Personnage.InSafeZone )
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
						Personnage.Add_Life(Personnage.MaxLife);
						transform.position = Spwan;
					}
				}
				else if (Personnage.inFight && Enemy == null)
				{
					Personnage.canMove = true;
					Personnage.inFight = false;
					
				}
				else if (Choice == EnumChoice.None) // to change with gui choice
				{
					if (Input.GetKeyDown("f") || Game_Manager.attack)
					{
						Choice = EnumChoice.Attack;
						Game_Manager.attack = false;
						SoundEffects.attacksound = true;
					}
					if (Input.GetKeyDown("r") || Game_Manager.heal_Boost)
					{
						Choice = EnumChoice.HealorBoost;
						Game_Manager.heal_Boost = false;
						SoundEffects.healsound = true;
					}
					if (Input.GetKeyDown("c") || Game_Manager.changeGun)
					{
						Choice = EnumChoice.ChangeGun;
						Game_Manager.changeGun = false;
						SoundEffects.chanGunsound = true;
					}

				}




			}

		}
		
		
	private void Update_Player()
		{
			
        string path = Application.dataPath;
        
        if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<Game_Manager>().use_old_inventory && File.Exists(path + $@"\{Player.ToString()}.txt"))
        {
            int nbligne = 0;

            using (StreamReader sr = new StreamReader(path + $@"\{Player.ToString()}.txt"))
            {
                string line;
                long lineNumber = 0;

                // Parcourir chaque ligne du fichier
                while ((line = sr.ReadLine()) != null)
                {
                    nbligne++;
                    while (line != null && line != "type:" + Player.ToString())
                    {
                        line = sr.ReadLine();
                        nbligne++;
                    }

                    Item item = null;
                    while ((line = sr.ReadLine()) != null && !line.StartsWith("type:"))
                    {
                        nbligne++;
                        if (line.StartsWith("inventory:"))
                        {
                            Personnage.Reset_Inventory();
                            string element = "";
                            int i = 11;
                            while (i < line.Length)
                            {
                                while (i < line.Length && line[i] != ' ' && line[i] != '\n' )
                                {
                                    element += line[i];
                                    i++;
                                }


                                item = null;

                                switch (element)
                                {
                                    case "Boss_Weapon":
                                        item = new Boss_Weapon(1,9,1);
                                        break;
                                    case "Banana":
                                        item = new Food("Banana",1,3);
                                        break;
                                    case "Cheese":
                                        item = new Food("Cheese",1,5);
                                        break;
                                    case "Hamburger":
                                        item = new Food("Hamburger",1,10);
                                        break;
                                    case "Gun":
                                        item = new Gun(1,3,1);
                                        break;
                                    case "Kit_Heal":
                                        item = new Kit_Heal(1,9);
                                        break;
                                    case "Ordinateur_Kali_Linux":
                                        item = new Ordinateur_Kali_Linux(4,1);
                                        break;
                                    case "Potion_Boost":
                                        item = new Potion_Boost("Potion_Mana",1,true,1,EnumsItem.Boost);
                                        break;
                                    case "Sniper":
                                        item = new Sniper_a(5,1,1);
                                        break;
                                }

                                element = "";
                                i++;
                                Personnage.Took(item);
                            }
                        }
                        else if (line.StartsWith("Principal_Weapon:"))
                        {
                            line = line.Remove(0, 17);
                            Personnage.name = line;
                        }
                        else if (line.StartsWith("name:"))
                        {
                            line = line.Remove(0, 5);
                            Personnage.name = line;
                        }
                        else if (line.StartsWith("level:"))
                        {
                            line = line.Remove(0, 6);
                            Personnage.level = int.Parse(line);
                        }
                        else if (line.StartsWith("xp:"))
                        {
                            line = line.Remove(0, 3);
                            Personnage.Add_Xp(int.Parse(line));
                        }
                        else if (line.StartsWith("life:"))
                        {
                            line = line.Remove(0, 5);
                            Personnage.Remove_Life(Personnage.Getlife - int.Parse(line));
                        }
                    }
                }
                sr.Close();
            }
        }
		}
		
	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			Enemy = other.gameObject;
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

		if ((other.tag == "Equipement" && (Input.GetKey("e") || Input.GetKeyDown("e"))&& !other.gameObject.GetComponent<ItemInGame>().took))
		{
			Item item = other.gameObject.GetComponent<ItemInGame>().Item;
			bool take =Personnage.Took(item);
			if (take  && item != null)
			{
				other.gameObject.GetComponent<ItemInGame>().took = true;
				_photonView.RPC("DestroyGameObject", RpcTarget.All, other.gameObject.GetComponent<PhotonView>().ViewID);
			}


		}
			



	}			
	public void OnTriggerStay(Collider other)
	{
		if ((other.tag == "Equipement" && (Input.GetKey("e") || Input.GetKeyDown("e"))&& !other.gameObject.GetComponent<ItemInGame>().took))
		{

			Item item = other.gameObject.GetComponent<ItemInGame>().Item;
			bool take =Personnage.Took(item);
			if (take  && item != null)
			{
				other.gameObject.GetComponent<ItemInGame>().took = true;
				_photonView.RPC("DestroyGameObject", RpcTarget.All, other.gameObject.GetComponent<PhotonView>().ViewID);
			}


		}
		else if (other.tag == "Enemy")
		{
			Personnage.canMove = false;
			Personnage.inFight = true;
			anim.SetInteger ("AnimationPar", 0);
				
		}
			



	}
		

	[PunRPC]
	public void DestroyGameObject(int viewID)
	{
		PhotonView targetPhotonView = PhotonView.Find(viewID);

		if (targetPhotonView != null && targetPhotonView.IsMine)
		{
			PhotonNetwork.Destroy(targetPhotonView);
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
	
	/*

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
				if (!other.GetComponent<PhotonView>().IsMine)
				{
					// Transférez la propriété au client local
					other.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
				}

// Supprimez le GameObject en tant que propriétaire ou MasterClient
				PhotonNetwork.Destroy(other.gameObject);
				//_photonView.RPC("delobj", RpcTarget.MasterClient,other.gameObject.GetComponent<PhotonView>().ViewID);

			}

		}
			



	}
		
		
	public void OnTriggerStay(Collider other)
	{
		if ((other.tag == "Equipement" && (Input.GetKey("e") || Input.GetKeyDown("e"))))
		{

			Item item = other.gameObject.GetComponent<ItemInGame>().Item;
			bool take =Personnage.Took(item);
			if (take && item!=null)
			{
				itemOnWorld.AddNewItem(item.Name);
				if (!other.gameObject.GetComponent<PhotonView>().IsMine)
				{
					// Transférez la propriété au client local
					other.gameObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
				}

// Supprimez le GameObject en tant que propriétaire ou MasterClient
				PhotonNetwork.Destroy(other.gameObject);
				
				//_photonView.RPC("delobj", RpcTarget.MasterClient,other.gameObject.GetComponent<PhotonView>().ViewID);

			}


		}
		else if (other.tag == "Enemy")
		{
			Personnage.canMove = false;
			Personnage.inFight = true;
			anim.SetInteger ("AnimationPar", 0);
				
		}
			



	}

	[PunRPC]
	void delobj(int id)
	{
		PhotonNetwork.Destroy(PhotonView.Find(id));
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

*/
}