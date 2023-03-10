using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Photon.Pun;

public class MiddelMonsterGenerator : MonoBehaviour
{
    private static GameObject _middelmonsterprefab;
        
        
    public static void spawmmonster(GameObject middelmonsterprefab, int minx,int maxx, int minz,int maxz, int sizeblock=1)
    {
        _middelmonsterprefab = middelmonsterprefab;
        int x = Random.Range(minx, maxx);
        int z = Random.Range(minz, maxz);
        
        
        PhotonNetwork.Instantiate(_middelmonsterprefab.name,new Vector3(x*sizeblock ,5,z*sizeblock) , Random.rotation, 0);

    }
}
