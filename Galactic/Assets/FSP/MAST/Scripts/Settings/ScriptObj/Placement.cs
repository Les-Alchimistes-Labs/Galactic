using System;
using UnityEngine;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Settings
    {
        namespace ScriptObj
        {
            public class Placement : ScriptableObject
            {
                //[SerializeField] public string targetParentName = MAST.Const.Placement.defaultTargetParentName;
                //[SerializeField] public int targetParentInstanceID = 0;
                
                [SerializeField] public bool snapToGrid = true;
                
                [SerializeField] public bool overridePrefabOffset = false;
                [SerializeField] public Offset offset;
                [Serializable] public class Offset
                {
                    [SerializeField] public Vector3 pos = new Vector3(0.0f, 0.0f, 0.0f);
                }
                
                [SerializeField] public bool overridePrefabRotation = false;
                [SerializeField] public Rotation rotation;
                [Serializable] public class Rotation
                {
                    [SerializeField] public Vector3 step = new Vector3(0.0f, 90.0f, 0.0f);
                }
                
                [SerializeField] public bool overridePrefabRaycast = false;
                [SerializeField] public MAST.DataClass.PlacementRaycast placementRaycast;
                
                [SerializeField] public bool overridePrefabRandomizer = false;
                [SerializeField] public MAST.DataClass.Randomizer randomizer;
            }
        }
    }
}
#endif