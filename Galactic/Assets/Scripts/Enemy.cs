using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using personnage_class.Personage;
using System;
using Photon.Pun;
using Random = UnityEngine.Random;
using Photon.Realtime;
using personnage_class.Personage.Monsters;

public class Enemy : MonoBehaviour
{
    public EnumMonster Monster ;

    private Personnage _monstre;
    public GameObject _active_monster;
    private List<Player2> Players;
    private List<GameObject> PlayersG;
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
        level = 0;
        waita = false;
        switch (Monster)
        {
            case EnumMonster.LittelMonster :
                _monstre = new LittelMonster("a");
                break;
            case EnumMonster.BossIntermediate :
                _monstre = new BossIntermediate("a");
                break;
            case EnumMonster.BossFinal :
                _monstre = new BossFinal("a");
                break;
        }
        
        
    }
    
    
    IEnumerator wait(int temp)
    {
        print(Time.time);
        yield return new WaitForSeconds(5);
        waita = false;
        print(Time.time);
    }
    
    
    // Update is called once per frame
    void Update()
    {
        if (_monstre != null)
        {
            if (_monstre.level < level)
            {
                _monstre.level = level;
            }
            if (Players.Count != 0 && _monstre.IsAlive())
            {
                transform.transform.LookAt(Players[0].transform);
                if (_pos == Players.Count)
                {
                    if (!waita)
                    {
                        int temp = Random.Range(0, Players.Count);
                        waita = true;
                        StartCoroutine(wait(temp));
                        _monstre.Attack(Players[temp].Personnage);
                        _pos = 0;
                        if (!Players[temp].Personnage.IsAlive())
                        {
                            Debug.Log("player is dead");
                            PlayersG[temp].transform.position = new Vector3(0, 0, 0);
                            
                            Players.RemoveAt(temp);
                        }
                    }

                }
                else if (Players[_pos].Choice != EnumChoice.None)
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
                            Players[_pos].Personnage.Use(Players[_pos].Personnage.PosInv);
                            break;


                    }

                    Players[_pos].Choice = EnumChoice.None;
                    
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
    [PunRPC]
    void update_Enemy(int pos ,int RemainLife )
    {
        _pos += pos + 1;
        _monstre.Remove_Life(- (RemainLife -_monstre.Getlife));
    }


    private void OnTriggerEnter(Collider other)
    {
        _photonView.RPC("update_Player", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);

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
            Players.Add(other.gameObject.GetComponent<Player2>());
            PlayersG.Add(other.gameObject);
        }
    }

    [PunRPC]
    void update_CanMove()
    {
        foreach (var player in Players)
        {
            player.Personnage.canMove = true;
            player.Choice = EnumChoice.None;
        }
    }
    
}
