using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using personnage_class.Personage;
using System;
using System.Xml.Linq;
using Photon.Pun;
using Random = UnityEngine.Random;
using Photon.Realtime;
using personnage_class.Personage.Monsters;
public class EnemyGenerator : MonoBehaviour
{
    



    public static void EnemyGeneratore(EnumMonster monster, GameObject prefab,Transform _transform, int level , float x , float z)
    {
        EnumMonster Monster= monster;
        GameObject _active_monster;
        GameObject _png;
        int _pos;
        int _level;
        _level = level;
        _png = prefab;
        GameObject gamemanager = GameObject.Find("GameManager");
        if (Game_Manager.Level> level)
        {
            _level = Game_Manager.Level;
        }
        switch (Monster)
        {
            case EnumMonster.LittelMonster :
                _active_monster =  PhotonNetwork.Instantiate(_png.name,new Vector3(x  ,_transform.position.y, z  ) , _transform.rotation, 0);
                _active_monster.GetComponent<Enemy>().level = _level;
                break;
            case EnumMonster.BossIntermediate :
                _active_monster =  PhotonNetwork.Instantiate(_png.name,new Vector3(x ,1,z) , new Quaternion(1,1,1,1), 0);
                _active_monster.GetComponent<Enemy>().level = _level;
                break;
            case EnumMonster.BossFinal :
                _active_monster =  PhotonNetwork.Instantiate(_png.name,new Vector3(x ,2,z),Quaternion.identity, 0); //new Vector3(_transform.position.x ,_transform.position.y,_transform.position.z) , _transform.rotation, 0);
                _active_monster.GetComponent<Enemy>().level = _level;
                break;
            default:
                _active_monster =  PhotonNetwork.Instantiate(_png.name,new Vector3(x ,2,z),Quaternion.identity, 0); //new Vector3(_transform.position.x ,_transform.position.y,_transform.position.z) , _transform.rotation, 0);
                _active_monster.GetComponent<Enemy>().level = _level;
                break;
        }

    }
    
}
