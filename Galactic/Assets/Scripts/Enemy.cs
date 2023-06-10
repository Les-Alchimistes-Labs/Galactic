using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using personnage_class.Personage;
using System;
using Photon.Pun;
using Random = UnityEngine.Random;
using Photon.Realtime;
using personnage_class.Personage.Monsters;
using UnityEngine.EventSystems;
using static Game_Manager;

public class Enemy : MonoBehaviour
{
    public EnumMonster Monster ;

    public Personnage _monstre;
    public GameObject _active_monster;
    private List<Player2> Players;
    private List<GameObject> PlayersG;
    private List<Personnage> PPlayers;
    private bool waita;
    private int _pos;
    public int level;
    private PhotonView _photonView;

    // Start is called before the first frame update
    void Start()
    {
        
        _photonView = GetComponent<PhotonView>();
        _pos = 0;
        PlayersG = new List<GameObject>();
        Players = new List<Player2>();
        PPlayers = new List<Personnage>();
        level = 2;
        waita = false;
        switch (Monster)
        {
            case EnumMonster.LittelMonster :
                _monstre = new LittelMonster("a");
                ((Monster)_monstre).Improve( 3);
                break;
            case EnumMonster.BossIntermediate :
                _monstre = new BossIntermediate("a");
                ((Monster)_monstre).Improve( 30);
                break;
            case EnumMonster.BossFinal :
                _monstre = new BossFinal("a");
                ((Monster)_monstre).Improve( 70);
                break;
        }
        if ( !PhotonNetwork.IsMasterClient)
            _photonView.RPC("SendList", RpcTarget.MasterClient);

        
    }
    
    
    IEnumerator wait(int temp)
    {
        Game_Manager.healOrder = "";
        yield return new WaitForSeconds(3);
        waita = false;
    }

    
    
    
    // Update is called once per frame
    void Update()
    {
        if (_monstre != null)
        {
            if (Players.Count != 0 && _monstre.IsAlive())
            {
                transform.transform.LookAt(Players[0].transform);
                if (_pos >= Players.Count && _photonView.IsMine )
                {
                    if (!waita)
                    {
                        int temp = Random.Range(0, 100);
                        waita = true;
                        StartCoroutine(wait(temp));
                        int posH = -1;
                        if (PlayersG.Count >= 2 ) // mettre à 2
                        {
                            posH = Target(PPlayers);
                            
                        }
                        int posa;// =((Monster)_monstre).Target(PPlayers,temp);
                        posa = Random.Range(0, PlayersG.Count);
                        _photonView.RPC("reset_pos", RpcTarget.All,temp, posH,posa);
                        

                    }

                }
                else if (!CheckCollision(PlayersG[_pos]) || PlayersG[_pos] is null )
                {
                    _photonView.RPC("playerkill", RpcTarget.All,_pos);
                }
                else if (Players[_pos].Choice != EnumChoice.None &&  PlayersG[_pos].GetComponent<PhotonView>().IsMine)
                {
                    switch (Players[_pos].Choice)
                    {
                        case EnumChoice.Attack:
                            Players[_pos].Personnage.Attack(_monstre);
                            break;
                        case EnumChoice.ChangeGun:
                            Players[_pos].Personnage.Change_Weapon_Equipped(Players[_pos].Personnage.PosInv);
                            break;
                        case EnumChoice.HealorBoost:
                            int pos = Players[_pos].Personnage.better_healorboost().pos;
                            if (pos != -1)
                            {
                                Players[_pos].Personnage.Use(pos);
                            }
                            break;
                    }

                    Players[_pos].Choice = EnumChoice.None;
                    if (_pos < PlayersG.Count && PlayersG[_pos].GetComponent<PhotonView>().IsMine)
                        GameObject.FindGameObjectWithTag("GameManager").GetComponent<Game_Manager>().unactiveturn();
                    _photonView.RPC("update_Enemy", RpcTarget.All, _pos,_monstre.Getlife);
                    
                }
            }
            else if ( !_monstre.IsAlive())
            {
                Debug.Log("monster is dead");
                for (int i = 0; i < _monstre.Get_Inventory().Length; i++)
                {
                    Item old = _monstre.Trow(i);
                    if (old != null)
                    {
                        GameObject temp;
                        temp = PhotonNetwork.Instantiate(old.Name,transform.position,transform.rotation,0);
                        temp.GetComponent<ItemInGame>().Item = old;
                    }
                }
                
                _photonView.RPC("update_CanMove", RpcTarget.All);
                PhotonNetwork.Destroy(_active_monster);

               

                
            }
            
        }
        
    }

    
    
    public void Heal(Personnage heros)
    { 
        int heal = (int)(heros.Getlife * 0.2);
        while (heros.Add_Life(heal) == false)
            heal -= 1;
        //Game_Manager.healOrder = "the divinity of this planet healed " + str;
        heros.Add_Life(heal);
    
    }
    
    
    public int Target(List<Personnage> heros)
    {
        int target = 0;
        for (int i = 1; i < heros.Count; i++)
        {
            if (heros[target].IsAlive())
            {
                if (heros[i].IsAlive())
                {
                    if (heros[i].Getlife > heros[target].Getlife)
                        target = i;
                }
            }
        }

        return target;
        
    }
    
    
    public bool CheckCollision( GameObject object1)
    {
        // Get the colliders of both GameObjects
        Collider collider1 = this.gameObject.GetComponent<Collider>();
        Collider collider2 = object1.GetComponent<Collider>();

        // Check if both GameObjects have colliders
        if (collider1 != null && collider2 != null)
        {
            // Check if the bounding boxes of the colliders intersect
            if (collider1.bounds.Intersects(collider2.bounds))
            {
                return true; // Collision detected
            }
        }

        return false; // No collision detected
    }

    

    [PunRPC]
    void reset_pos(int temp, int posH,int posa)
    {
        
        if (  posa != -1 && PlayersG[posa].GetComponent<PhotonView>().IsMine)
            _monstre.Attack(PPlayers[posa]);
        _pos = 0;

        for (int i=0 ; i<PPlayers.Count;i++)
        {
            if (!Players[i].Personnage.IsAlive())
            {
                Debug.Log("player is dead");
                PlayersG[i].transform.position =Players[i].Spwan ;
                            
                _photonView.RPC("playerkill", RpcTarget.All,i);

            }
            
        }
        if (_pos < Players.Count)
        {
            Players[_pos].Choice = EnumChoice.None;
            if (posH != -1 && PlayersG[posH].GetComponent<PhotonView>().IsMine)
            {
                Heal(PPlayers[posH]);
            }
        }
        
        if (_pos < PlayersG.Count && PlayersG[_pos].GetComponent<PhotonView>().IsMine)
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<Game_Manager>().activeturn();


        


    }
    
    [PunRPC]
    void update_Enemy(int pos ,int RemainLife )
    {
        _pos += pos + 1;
        if (_pos < Players.Count)
        {
            Players[_pos].Choice = EnumChoice.None;
        }
        _monstre.Remove_Life(- (RemainLife -_monstre.Getlife));
        if (_pos < PlayersG.Count && PlayersG[_pos].GetComponent<PhotonView>().IsMine)
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<Game_Manager>().activeturn();

    }

    
    private bool MedecinSpawn = false;
    
    

    private void OnTriggerEnter(Collider other)
    {
        _photonView.RPC("update_Player", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);

    }
    
    
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Respawn")
        {
            Destroy(_active_monster);
        }
    }
    

    [PunRPC]
    void update_Player(int viewID )
    {
        GameObject other = PhotonView.Find(viewID).gameObject;
        bool find = false;
        foreach (var player in PlayersG)
        {
            if (player == other)
            {
                find = true;
                break;
            }
        }

        if (!find)
        {
            PPlayers.Add(other.gameObject.GetComponent<Player2>().Personnage);
            Players.Add(other.gameObject.GetComponent<Player2>());
            PlayersG.Add(other.gameObject);
            if (_pos < PlayersG.Count && PlayersG[_pos].GetComponent<PhotonView>().IsMine)
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<Game_Manager>().activeturn();

        }
    }

    [PunRPC]
    void update_CanMove()
    {
        if (_monstre.Type() == EnumType.IntermediateMonster)
            desactivateAccessObjectFinalLevel++;
        if (desactivateAccessObjectFinalLevel >= 3)
        {
            // delete all door
            GameObject[] doors = GameObject.FindGameObjectsWithTag("door");
            foreach (GameObject door in doors)
            {
                Destroy(door);
            }
        }
        foreach (var player in Players)
        {
            if (player.Personnage != null)
            {
                player.Personnage.inFight = false;
                player.Personnage.canMove = true;
            }
                
            
        }
    }
    
    [PunRPC]
    void playerkill(int i)
    {
        Game_Manager.attack = false;
        Game_Manager.heal_Boost = false;
        Game_Manager.changeGun = false;
        Debug.Log($"Player {PPlayers[i]} leave fight");
            PPlayers.RemoveAt(i);
            Players.RemoveAt(i);
            PlayersG.RemoveAt(i);
            string deb = "";
            foreach (var v in PlayersG)
            {
                if (v != null)
                {
                    deb += v.name +" ";
                }
            }
        Debug.Log("list player " + deb);
    }


    [PunRPC]
    void SendList(PhotonMessageInfo info)
    {
        _photonView.RPC("RequestList", info.Sender,PlayersG,PPlayers,Players);
 
    }

    [PunRPC]
    void RequestList(List<GameObject> pg,List<Personnage> pp, List<Player2> p)
    {
        PlayersG = pg;
        PPlayers = pp;
        Players = p;
    }

}
