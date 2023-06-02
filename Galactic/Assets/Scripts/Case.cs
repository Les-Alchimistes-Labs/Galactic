using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using personnage_class.Personage;

namespace DefaultNamespace
{
    public class Case 
    {
        public enum EnumType
        {
            Ground1,
            Ground2,
            Ground3,
            Ground4,
            GroundfinalBoss,
            Tree1,
            Tree2,
            Tree3,
            Tree4,
            Wall,
            River,
            Item,
            Rock,
            AccessObjectFinalLevel,
            Empty
        }

        public (EnumType, EnumsItem)[,] ConvertMatrix(int[,] matrix)
        {
            (EnumType, EnumsItem)[,] newMatrix = new (EnumType, EnumsItem)[matrix.GetLength(0), matrix.GetLength(1)];
            for (int indiceY = 0; indiceY < matrix.GetLength(0); indiceY++)
            {
                for(int indiceX = 0; indiceX < matrix.GetLength(1); indiceX++)
                {
                    switch (matrix[indiceY, indiceX])
                    {
                        case 0:
                            newMatrix[indiceY, indiceX] = (EnumType.Ground1, EnumsItem.Empty);
                            break;
                        case 1:
                            newMatrix[indiceY, indiceX] = (EnumType.Ground2, EnumsItem.Empty);
                            break;
                        case 2:
                            newMatrix[indiceY, indiceX] = (EnumType.Ground3, EnumsItem.Empty);
                            break;
                        case 3:
                            newMatrix[indiceY, indiceX] = (EnumType.Ground4, EnumsItem.Empty);
                            break;
                        case 4:
                            newMatrix[indiceY, indiceX] = (EnumType.Tree1, EnumsItem.Empty);
                            break;
                        case 5:
                            newMatrix[indiceY, indiceX] = (EnumType.Tree2, EnumsItem.Empty);
                            break;
                        case 6:
                            newMatrix[indiceY, indiceX] = (EnumType.Tree3, EnumsItem.Empty);
                            break;
                        case 7:
                            newMatrix[indiceY, indiceX] = (EnumType.Tree4, EnumsItem.Empty);
                            break;
                        case 8:
                            newMatrix[indiceY, indiceX] = (EnumType.Wall, EnumsItem.Empty);
                            break;
                        case 9:
                            newMatrix[indiceY, indiceX] = (EnumType.River, EnumsItem.Empty);
                            break;
                        case 10:
                            newMatrix[indiceY, indiceX] = (EnumType.Item, EnumsItem.Empty);
                            break;
                        case 11:
                            newMatrix[indiceY, indiceX] = (EnumType.Rock, EnumsItem.Empty);
                            break;
                        case 12:
                            newMatrix[indiceY, indiceX] = (EnumType.GroundfinalBoss, EnumsItem.Empty);
                            break;
                        case 13:
                            newMatrix[indiceY, indiceX] = (EnumType.AccessObjectFinalLevel, EnumsItem.Empty);
                            break;
                        default:
                            newMatrix[indiceY, indiceX] = (EnumType.Empty, EnumsItem.Empty);
                            break;
                    }
                }
            }

            return newMatrix;
        }
        
        public EnumType Type { get; set; }

    }
}
