using System;
using UnityEngine;

#if (UNITY_EDITOR)

namespace MAST
{
    public class DataClass
    {
        [Serializable] public class PlacementRaycast
        {
            [Space(5)]
            
            [Tooltip("Use a raycast to determine the placement position")]
            public bool useRaycast = false;
            [Tooltip("Direction to raycast")]
            public DirectionVector direction = DirectionVector.Down;
            [Tooltip("Start position offset for raycast")]
            public Vector3 startOffset = Vector3.zero;
        }
        
        [Serializable] public class Randomizer
        {
            [Space(5)]
            [Tooltip("Use Randomizer with this prefab?")]
            public bool useRandomizer = false;
            
            [Space(10)]
            [Header("Replacement Settings")]
            
            [Tooltip("Allow this prefab to be replaced by another linked prefab?")]
            public bool allowReplacement = false;
            [Tooltip("Links this prefab to other prefabs for random replacement")]
            public int replacementID = 0;
            [Tooltip("Likelihood that this prefab will be chosen instead of other linked prefabs")]
            public int replacementWeight = 0;
            [Tooltip("Allow this prefab to be chosen twice in a row?")]
            public bool allowSequentialRepeat = false;
            
            [Space(10)]
            [Header("Rotation Settings")]
            
            [Tooltip("Angle variation during randomization - counterclockwise from starting angle")]
            public Vector3 rotateMin = new Vector3(0f, -180.0f, 0f);
            [Tooltip("Angle variation during randomization - clockwise from starting angle")]
            public Vector3 rotateMax = new Vector3(0f, 180.0f, 0f);
            [Tooltip("Angle step during randomization - 0 for no rotation, 90 for (0, 90, 180, 270)")]
            public Vector3 rotateStep = new Vector3(1.0f, 1.0f, 1.0f);
            
            [Space(10)]
            [Header("Scale Settings")]
            
            [Tooltip("Minimum scale during randomization")]
            public Vector3 scaleMin = new Vector3(0.75f, 0.75f, 0.75f);
            [Tooltip("Maximum scale during randomization")]
            public Vector3 scaleMax = new Vector3(1.25f, 1.25f, 1.25f);
            [Tooltip("Lock axis together during randomization?")]
            public ScaleAxisLock scaleLock = ScaleAxisLock.XZ;
            
            [Space(10)]
            [Header("Flip Settings")]
            
            [Tooltip("Can this prefab be flipped on the X axis during randomization?")]
            public bool flipX = false;
            [Tooltip("Can this prefab be flipped on the Y axis during randomization?")]
            public bool flipY = false;
            [Tooltip("Can this prefab be flipped on the Z axis during randomization?")]
            public bool flipZ = false;
            
            [Space(10)]
            [Header("Position Settings")]
            
            [Tooltip("Minimum position offset during randomization")]
            public Vector3 posMin = new Vector3(-0.5f, -0.1f, -0.5f);
            [Tooltip("Maximum position offset during randomization")]
            public Vector3 posMax = new Vector3(0.5f, 0.1f, 0.5f);
        }
    }
}

#endif