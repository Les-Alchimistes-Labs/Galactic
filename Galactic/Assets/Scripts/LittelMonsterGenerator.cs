using System;
using Photon.Pun;
using Random = UnityEngine.Random;
using UnityEngine;
using Photon.Realtime;


namespace DefaultNamespace
{
    public class LittelMonsterGenerator
    {
        private static GameObject _littelmonsterprefab;
        
        
        public static void tryspawmmonster(GameObject littelmonsterprefab, Transform me)
        {
            _littelmonsterprefab = littelmonsterprefab;
            if ( Random.Range(0, 1001) == 10)
            {
                PhotonNetwork.Instantiate(_littelmonsterprefab.name,new Vector3(me.position.x + 5 ,me.position.y,me.position.z + 5) , me.rotation, 0);
                
            }


        }
    }
}