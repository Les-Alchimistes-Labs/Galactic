using System;
using static Game_Manager;
using System.Collections.Generic;
using DefaultNamespace;
using personnage_class.Personage;
using Photon.Pun; using UnityEngine;
using Random = UnityEngine.Random;
using Photon.Pun;
using Photon.Realtime;

public class MapGenerator : MonoBehaviour { 
    public GameObject Biome1Prefab; 
    public GameObject Biome2Prefab; 
    public GameObject Biome3Prefab; 
    public GameObject Biome4Prefab;

    public GameObject tree1Prefab; 
    public GameObject tree2Prefab; 
    public GameObject tree3Prefab; 
    public GameObject tree4Prefab; 
    public GameObject wallPrefab; 
    public GameObject riverPrefab; 
    public GameObject itemPrefab; 
    public GameObject rockPrefab; 
    public GameObject IntermediateMonster;
    public GameObject BossFinalMonster;
    public GameObject AccessObjectFinalLevel;
    public bool[] biomeUse; 
    public int level;
    public int diffx = 100;

    private static int[,] matrixLevel;
    private static int[,] finalMonsterRoom;

    private void Awake()
    {
        diffx =200/2;
        matrixLevel = new int[200,200];
        finalMonsterRoom = new int[25,25];
        _photonView = GetComponent<PhotonView>();
        
        
        if (!_photonView.IsMine)
        {
            
            // Retrieve the value from the master client and assign it to the variable
            //_photonView.RPC("RequestArray", RpcTarget.MasterClient);
        }
        GenerateMatrixLevel(matrixLevel);

    }
    
    public int[,] getmatrixLevel
    {
        get { return matrixLevel; }
    }
    
    
    private static (int, string)[,] GenerateTupleMatrixOfRiverCube(int[,] matrixLevel)
    {
        (int, string)[,] matrixRiver = new (int, string)[matrixLevel.GetLength(0), matrixLevel.GetLength(1)];

        int indiceX = 1;
        int indiceY;

        int randomStartPoint = Random.Range(1, (matrixLevel.GetLength(0) - 1)/2);
        List<string> list = new List<string>() { "beside", "right"};
        int randomDirection = Random.Range(0, list.Count);
        matrixRiver[indiceX, randomStartPoint] = (9, list[randomDirection]);
        indiceY = randomStartPoint;
        indiceX += 1;

        while (indiceY < matrixLevel.GetLength(1))
        {
            if (indiceX - 1 > 0 && indiceY + 3 < matrixLevel.GetLength(1) && matrixRiver[indiceX - 1, indiceY].Item2 == "beside")
            {
                randomDirection = Random.Range(0, list.Count);
                matrixRiver[indiceX, indiceY] = (9, list[randomDirection]);
                matrixRiver[indiceX, indiceY + 1] = (9, list[randomDirection]);
                matrixRiver[indiceX, indiceY + 2] = (9, list[randomDirection]);
                matrixRiver[indiceX, indiceY + 3] = (9, "null");
                indiceY += 1;
                indiceX += 1;
            }
            else
            {
                if (indiceY + 3 < matrixLevel.GetLength(1) && indiceX + 3 < matrixLevel.GetLength(0))
                {
                    randomDirection = Random.Range(0, list.Count);
                    matrixRiver[indiceX, indiceY] = (9, list[randomDirection]);
                    matrixRiver[indiceX, indiceY + 1] = (9, list[randomDirection]);
                    matrixRiver[indiceX, indiceY + 2] = (9, list[randomDirection]);
                    matrixRiver[indiceX, indiceY + 3] = (9, list[randomDirection]);
                }

                indiceX += 1;
            }

            if (indiceX >= matrixLevel.GetLength(0) - 1)
                break;
        }

        return matrixRiver;
    }



    public static int[,] GenerateMatrixRiver((int, string)[,] matrixRiver)
    {
        int[,] matrixRiverCube = new int[matrixLevel.GetLength(0), matrixLevel.GetLength(1)];
        
        for (int indiceX = 0; indiceX < matrixRiver.GetLength(0); indiceX++)
        {
            for (int indiceY = 0; indiceY < matrixRiver.GetLength(1); indiceY++)
            {
                if(matrixRiver[indiceX, indiceY].Item1 == 9) 
                    matrixRiverCube[indiceX, indiceY] = 9; 
                else 
                    matrixRiverCube[indiceX, indiceY] = -1;
            }
        } 
        return matrixRiverCube;
    }
    
    public static int[,] GenerateMatrixLevel(int[,] matrixLevel)
    {
        int[,] matrixRiverCube = GenerateMatrixRiver(GenerateTupleMatrixOfRiverCube(matrixLevel));
        for (int i = 0; i < matrixLevel.GetLength(0); i++)
        {
            for (int j = 0; j < matrixLevel.GetLength(1); j++)
            {
                if ((i == 0 || i == matrixRiverCube.GetLength(0)-1 || j == 0 || j == matrixRiverCube.GetLength(1)-1))
                {
                    if (i <= matrixLevel.GetLength(0)/2 && j <= matrixLevel.GetLength(1)/2)
                        matrixLevel[i, j] = 4;
                    else if (i <= matrixLevel.GetLength(0)/2 && j > matrixLevel.GetLength(1)/2)
                        matrixLevel[i, j] = 5;
                    else if (i > matrixLevel.GetLength(0)/2 && j <= matrixLevel.GetLength(1)/2)
                        matrixLevel[i, j] = 6;
                    else if (i > 3 * matrixLevel.GetLength(0) / 4 && i < (3 * matrixLevel.GetLength(0) / 4) + 10)
                        matrixLevel[i, j] = 11;
                    else
                        matrixLevel[i, j] = 7;
                }
                
                else if ((i == matrixLevel.GetLength(0)/2 || j == matrixLevel.GetLength(1)/2) && matrixRiverCube[i,j] != 9)
                {
                    if ((i > matrixLevel.GetLength(0)/4 && i < (matrixLevel.GetLength(0)/4)+10)
                        || (i > 3*matrixLevel.GetLength(0)/4 && i < (3*matrixLevel.GetLength(0)/4)+10)
                        || (j > matrixLevel.GetLength(1)/4 && j < (matrixLevel.GetLength(1)/4)+10)
                        || (j > 3*matrixLevel.GetLength(0)/4 && j < (3*matrixLevel.GetLength(0)/4)+10))
                    {
                        matrixLevel[i, j] = 11;
                    }
                    else
                    {
                        matrixLevel[i, j] = 8;
                    }
                }

                else
                {
                    if(matrixRiverCube[i,j] == 9)
                    {
                        matrixLevel[i,j] = 9;
                    }
                    else
                    {
                        int randomNumber = Random.Range(0, matrixLevel.GetLength(0)/2);
                        if (randomNumber < 2 && matrixRiverCube[i,j] != 9)
                        {
                            if (i <= matrixLevel.GetLength(0)/2 && j <= matrixLevel.GetLength(1)/2)
                                matrixLevel[i, j] = 4;
                            else if (i <= matrixLevel.GetLength(0)/2 && j > matrixLevel.GetLength(1)/2)
                                matrixLevel[i, j] = 5;
                            else if (i > matrixLevel.GetLength(0)/2 && j <= matrixLevel.GetLength(1)/2)
                                matrixLevel[i, j] = 6;
                            else
                                matrixLevel[i, j] = 7;
                        }
                        else if(randomNumber > 5 && matrixRiverCube[i,j] != 9)
                        {
                            if (i <= matrixLevel.GetLength(0)/2 && j <= matrixLevel.GetLength(1)/2)
                                matrixLevel[i, j] = 0;
                            else if (i <= matrixLevel.GetLength(0)/2 && j > matrixLevel.GetLength(1)/2)
                                matrixLevel[i, j] = 1;
                            else if (i > matrixLevel.GetLength(0)/2 && j <= matrixLevel.GetLength(1)/2)
                                matrixLevel[i, j] = 2;
                            else
                                matrixLevel[i, j] = 3;
                        }
                        else
                        {
                            if (i <= matrixLevel.GetLength(0)/2 && j <= matrixLevel.GetLength(1)/2)
                                matrixLevel[i, j] = 0;
                            else if (i <= matrixLevel.GetLength(0)/2 && j > matrixLevel.GetLength(1)/2)
                                matrixLevel[i, j] = 1;
                            else if (i > matrixLevel.GetLength(0)/2 && j <= matrixLevel.GetLength(1)/2)
                                matrixLevel[i, j] = 2;
                            else
                                matrixLevel[i, j] = 3;
                        }
                    }
                }
            }
        }

        return matrixLevel;
    }

    public static int[,] GenerateMatrixFinalMonsterRoom(int[,] matrixLevel)
    {
        int[,] matrixRiverCube = GenerateMatrixRiver(GenerateTupleMatrixOfRiverCube(matrixLevel));
        for (int i = 0; i < matrixLevel.GetLength(0); i++)
        {
            for (int j = 0; j < matrixLevel.GetLength(1); j++)
            {
                if (i == 0 || i == matrixLevel.GetLength(0) - 1 || j == 0 || j == matrixLevel.GetLength(1) - 1)
                {
                    if (j == 0)
                        matrixLevel[i, j] = 13;
                    else
                        matrixLevel[i, j] = 8;
                }
                else
                {
                    matrixLevel[i, j] = 12;
                }
            }
        }

        return matrixLevel;
    }


    public (Case.EnumType, EnumsItem)[,] matrixCase = new (Case.EnumType, EnumsItem)[200, 200];

    

    public void GenerateMap(int[,] matrixLevel)
    {
       
        matrixCase = new Case().ConvertMatrix(matrixLevel);

        for (int indiceX = 0; indiceX < matrixCase.GetLength(0); indiceX++)
        {
            for (int indiceY = 0; indiceY < matrixCase.GetLength(1); indiceY++)
            {
                GameObject ground;

                if (indiceY == 0 && (indiceX >= 1 +diffx && indiceX <= 9 +diffx ))
                {
                    ground = Instantiate(Biome1Prefab, new Vector3(indiceX - diffx, 0, indiceY), Quaternion.identity);
                    ground.transform.SetParent(transform);
                }
                else
                {
                    switch (matrixCase[indiceX, indiceY])
                    {
                        case (Case.EnumType.Ground1, EnumsItem.Empty):
                            ground = Instantiate(Biome1Prefab, new Vector3(indiceX - diffx , 0, indiceY), Quaternion.identity);
                            ground.transform.SetParent(transform);
                            if (Random.Range(0, 100*20) == 1 && !biomeUse[0] && indiceY > 10 && PhotonNetwork.IsMasterClient)
                            {
                                biomeUse[0] = true;
                                EnemyGenerator.EnemyGeneratore(EnumMonster.BossIntermediate, IntermediateMonster, null, level, indiceX - diffx, indiceY);
                            }
                            break;
                        case (Case.EnumType.Ground2, EnumsItem.Empty):
                            ground = Instantiate(Biome2Prefab, new Vector3(indiceX - diffx, 0, indiceY), Quaternion.identity);
                            ground.transform.SetParent(transform);
                            if (Random.Range(0, 100*20) == 1 && !biomeUse[1] && indiceY > 10 && PhotonNetwork.IsMasterClient)
                            {
                                biomeUse[1] = true;
                                EnemyGenerator.EnemyGeneratore(EnumMonster.BossIntermediate, IntermediateMonster, null, level, indiceX - diffx, indiceY);
                            }
                            break;
                        case (Case.EnumType.Ground3, EnumsItem.Empty):
                            ground = Instantiate(Biome3Prefab, new Vector3(indiceX - diffx, 0, indiceY), Quaternion.identity);
                            ground.transform.SetParent(transform);
                            if (Random.Range(0, 100*20) == 1 && !biomeUse[2] && indiceY > 10 && PhotonNetwork.IsMasterClient)
                            {
                                biomeUse[2] = true;
                                EnemyGenerator.EnemyGeneratore(EnumMonster.BossIntermediate, IntermediateMonster, null, level, indiceX - diffx, indiceY);
                            }
                            break;
                        case (Case.EnumType.Ground4, EnumsItem.Empty):
                            ground = Instantiate(Biome4Prefab, new Vector3(indiceX - diffx, 0, indiceY), Quaternion.identity);
                            ground.transform.SetParent(transform);
                            if (Random.Range(0, 100*20) == 1 && !biomeUse[3] && indiceY > 10 && PhotonNetwork.IsMasterClient)
                            {
                                biomeUse[3] = true;
                                EnemyGenerator.EnemyGeneratore(EnumMonster.BossIntermediate, IntermediateMonster, null, level, indiceX - diffx, indiceY);
                            }
                            break;
                        case (Case.EnumType.Tree1, EnumsItem.Empty):
                            GameObject tree1 = Instantiate(tree1Prefab, new Vector3(indiceX - diffx, 0, indiceY), Quaternion.identity);
                            tree1.transform.SetParent(transform);
                            break;
                        case (Case.EnumType.Tree2, EnumsItem.Empty):
                            GameObject tree2 = Instantiate(tree2Prefab, new Vector3(indiceX - diffx, 0, indiceY), Quaternion.identity);
                            tree2.transform.SetParent(transform);
                            ground = Instantiate(Biome2Prefab, new Vector3(indiceX - diffx, 0, indiceY), Quaternion.identity);
                            ground.transform.SetParent(transform);
                            BoxCollider tree2Collider = tree2.AddComponent<BoxCollider>();
                            tree2Collider.size = new Vector3(1f, 3f, 1f);
                            tree2Collider.center = new Vector3(0f, 1.5f, 0f);
                            break;
                        case (Case.EnumType.Tree3, EnumsItem.Empty):
                            GameObject tree3 = Instantiate(tree3Prefab, new Vector3(indiceX - diffx, 0, indiceY), Quaternion.identity);
                            tree3.transform.SetParent(transform);
                            ground = Instantiate(Biome3Prefab, new Vector3(indiceX - diffx, 0, indiceY), Quaternion.identity);
                            ground.transform.SetParent(transform);
                            break;
                        case (Case.EnumType.Tree4, EnumsItem.Empty):
                            ground = Instantiate(Biome4Prefab, new Vector3(indiceX - diffx, 0, indiceY), Quaternion.identity);
                            ground.transform.SetParent(transform);
                            GameObject tree4 = Instantiate(tree4Prefab, new Vector3(indiceX - diffx, 0, indiceY), Quaternion.identity);
                            tree4.transform.SetParent(transform);
                            break;

                        case (Case.EnumType.Wall, EnumsItem.Empty):
                            GameObject wall = Instantiate(wallPrefab, new Vector3(indiceX - diffx, 1.5f, indiceY), Quaternion.identity);
                            wall.transform.SetParent(transform);
                            break;
                        case (Case.EnumType.River, EnumsItem.Empty):
                            GameObject river = Instantiate(riverPrefab, new Vector3(indiceX - diffx, 0, indiceY), Quaternion.identity);
                            river.transform.SetParent(transform); 
                            break;
                        case (Case.EnumType.Item, EnumsItem.Empty):
                            GameObject item = Instantiate(itemPrefab, new Vector3(indiceX- diffx, 0, indiceY), Quaternion.identity);
                            item.transform.SetParent(transform);
                            break;
                        case (Case.EnumType.Rock, EnumsItem.Empty):
                            GameObject rock = Instantiate(rockPrefab, new Vector3(indiceX- diffx, 0.5f, indiceY), Quaternion.identity);
                            rock.transform.SetParent(transform);
                            break;
                    }
                }
            }
        }

        var monsterRoomMatrix = GenerateMatrixFinalMonsterRoom(finalMonsterRoom);
        
        matrixCase = new Case().ConvertMatrix(monsterRoomMatrix);
        
        for(int indiceX = 0; indiceX < monsterRoomMatrix.GetLength(0); indiceX++)
        {
            for (int indiceY = 0; indiceY < monsterRoomMatrix.GetLength(1); indiceY++)
            {
                GameObject ground;
                GameObject accessObjectFinalLevel;
                switch (matrixCase[indiceX, indiceY])
                {
                    case (Case.EnumType.GroundfinalBoss, EnumsItem.Empty):
                        ground = Instantiate(BossFinalMonster,
                            new Vector3(indiceX + 145 - diffx, 0, indiceY + 199),
                            Quaternion.identity);
                        ground.transform.SetParent(transform);
                        if (Random.Range(0, 200) == 1 && !biomeUse[0] && indiceY > 10 &&
                            PhotonNetwork.IsMasterClient)
                        {
                            biomeUse[0] = true;
                        }

                        break;
                    case (Case.EnumType.Wall, EnumsItem.Empty):
                        GameObject wall = Instantiate(wallPrefab,
                            new Vector3(indiceX + 145 - diffx, 1.5f, indiceY + 199), Quaternion.identity);
                        wall.transform.SetParent(transform);
                        break;
                    case (Case.EnumType.Item, EnumsItem.Empty):
                        GameObject item = Instantiate(itemPrefab,
                            new Vector3(indiceX + 145 - diffx, 0, indiceY + 199),
                            Quaternion.identity);
                        item.transform.SetParent(transform);
                        break;
                    case (Case.EnumType.Rock, EnumsItem.Empty):
                        GameObject rock = Instantiate(rockPrefab,
                            new Vector3(indiceX + 145 - diffx, 0, indiceY + 199), Quaternion.identity);
                        rock.transform.SetParent(transform);
                        break;
                    case (Case.EnumType.AccessObjectFinalLevel, EnumsItem.Empty):
                        if(desactivateAccessObjectFinalLevel < 3)
                        {
                            accessObjectFinalLevel = Instantiate(AccessObjectFinalLevel,
                                new Vector3(indiceX + 145 - diffx, 1.5f, indiceY + 199), Quaternion.identity);
                            accessObjectFinalLevel.transform.SetParent(transform);
                            Quaternion rotation = Quaternion.Euler(0, 90, 0);
                            accessObjectFinalLevel.transform.rotation = rotation;
                        }
                        else
                        {
                            rock = Instantiate(rockPrefab,
                                new Vector3(indiceX + 145 - diffx, 0, indiceY + 199), Quaternion.identity);
                            rock.transform.SetParent(transform);
                        }
                        break;

                
                }
            }
        }
    }

    private PhotonView _photonView;
    void Start()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        level = 0;
        _photonView = GetComponent<PhotonView>();
        biomeUse = new bool[] { false, false, false, false };
        int[,] matrixLevelWithRiver = GenerateMatrixLevel(matrixLevel);
        if (!PhotonNetwork.IsMasterClient)
        {
            // Retrieve the value from the master client and assign it to the variable
           // _photonView.RPC("RequestArray", RpcTarget.MasterClient);
        }
        GenerateMap(matrixLevelWithRiver);
        
        


    }

    [PunRPC]
    private void RequestArray(PhotonMessageInfo info)
    {
        _photonView.RPC("SendArray", info.Sender, matrixLevel);
    }

    [PunRPC]
    private void SendArray(int[,] array)
    {
        // Récupère le tableau envoyé par le "master"
        matrixLevel = array;
        
    }
    
    
}

