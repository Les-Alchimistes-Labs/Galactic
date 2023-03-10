using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Building
    {
        public static class Helper
        {
            // Layer that the MAST grid is set to
            [SerializeField] private static int theLayerTheGridIsOn = 1 << Const.Grid.gridLayer;
            
            // MAST script component attached the GameObjects
            [SerializeField] public static Component.MASTPrefabSettings mastScript;
            
        // ---------------------------------------------------------------------------
        #region Get Mouse Position on Grid (with or without snap)
        // ---------------------------------------------------------------------------
            // Converts a position on the grid object into a position snapped to the grid
            public static Vector3 GetPositionOnGridClosestToMousePointer()
            {
                Physics.queriesHitBackfaces = true;
                
                // Create a ray starting from the current point the mouse is
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                
                // Raycast to grid layer
                Visualizer.visualizerOnGrid =
                    Physics.Raycast(ray.origin,
                    ray.direction,
                    out RaycastHit hit,
                    Mathf.Infinity,
                    theLayerTheGridIsOn);
                
                // Calculate closest grid position to hit
                float xPos, zPos;
                if (Settings.Data.placement.snapToGrid)
                {
                    xPos = RoundToNearestGridCenter(hit.point.x);
                    zPos = RoundToNearestGridCenter(hit.point.z);
                }
                // Return true position hit
                else
                {
                    xPos = hit.point.x;
                    zPos = hit.point.z;
                }
                
                // If raycast reached infinity *** Not sure if this will ever happen ***
                if (xPos == Mathf.Infinity || zPos == Mathf.Infinity)
                {
                    // Set position to x=0 z=0
                    xPos = 0;
                    zPos = 0;
                }
                
                // Calculate current placement position on the grid
                Vector3 placementPosition = new Vector3(xPos, Settings.Data.gui.grid.gridHeight * Settings.Data.gui.grid.yUnitSize, zPos);
                
                // Calculate placement position by raycast from current placement position
                if (PlacementRaycast.GetUseRaycast())
                    placementPosition = GetRaycastPosition(placementPosition);
                    
                // Return the closest grid position
                return placementPosition;
            }
            
            // Calculate closest position to the grid - offset to grid center
            private static float RoundToNearestGridCenter(float positionOnAxis)
            {
                return (Mathf.Floor(positionOnAxis / Settings.Data.gui.grid.xzUnitSize) + 0.5f) * Settings.Data.gui.grid.xzUnitSize;
                
                //return (Mathf.Round((positionOnAxis + (MAST_Settings.gui.grid.xzUnitSize / 2f))
                //    / MAST_Settings.gui.grid.xzUnitSize) - 0.5f) * MAST_Settings.gui.grid.xzUnitSize;
            }
            
            // Get placement position based on raycast operation
            private static Vector3 GetRaycastPosition(Vector3 placementPosition)
            {
                // Get Raycast Direction
                Vector3 raycastDirection = new Vector3();
                switch (PlacementRaycast.GetDirection())
                {
                    case DirectionVector.Down:      raycastDirection = Vector3.down;    break;
                    case DirectionVector.Up:        raycastDirection = Vector3.up;      break;
                    case DirectionVector.Left:      raycastDirection = Vector3.left;    break;
                    case DirectionVector.Right:     raycastDirection = Vector3.right;   break;
                    case DirectionVector.Forward:   raycastDirection = Vector3.forward; break;
                    case DirectionVector.Back:      raycastDirection = Vector3.back;    break;
                }
                
                // Create a layer mask that excludes the grid
                int rayCastLayers = ~(1 << Const.Grid.gridLayer | 1 << Const.Placement.visualizerLayer);
                
                // If Raycast from current placement position is successful
                if(Physics.Raycast(placementPosition + PlacementRaycast.GetStartOffset(),
                    raycastDirection, out RaycastHit hit, Mathf.Infinity, rayCastLayers))
                    {
                        // If the raycast distance did not reach infinity *** Not sure if this will ever happen ***
                        if (hit.point.x != Mathf.Infinity && hit.point.y != Mathf.Infinity && hit.point.z != Mathf.Infinity)
                        {
                            // Get Placement Position from the raycast hit point
                            placementPosition = hit.point;
                        }
                    }
                
                // Return with the Placement Position
                return placementPosition;
            }
            
        #endregion
        // ---------------------------------------------------------------------------
            
        // ---------------------------------------------------------------------------
        #region Get Values from Prefab or Settings
        // ---------------------------------------------------------------------------
            // Get offset position
            public static Vector3 GetOffsetPosition()
            {
                if (Settings.Data.placement.overridePrefabOffset)
                    return Settings.Data.placement.offset.pos;
                else
                    try { return mastScript.offsetPosition; }
                    catch { return Settings.Data.placement.offset.pos; }
            }
            
            // Get rotation step
            public static Vector3 GetRotationStep()
            {
                if (Settings.Data.placement.overridePrefabRotation)
                    return Settings.Data.placement.rotation.step;
                else
                    try { return mastScript.rotationStep; }
                    catch { return Settings.Data.placement.rotation.step; }
            }  
            
            // ---------------------------------------------------------------------------
            // Placement Raycast
            // ---------------------------------------------------------------------------
            public class PlacementRaycast
            {
                // Does this Prefab use the Placement Raycast?
                public static bool GetUseRaycast()
                {
                    if (Settings.Data.placement.overridePrefabRaycast)
                        return Settings.Data.placement.placementRaycast.useRaycast;
                    else
                        try { return mastScript.placementRaycast.useRaycast; }
                        catch { return Settings.Data.placement.placementRaycast.useRaycast; }
                }
                
                // Get Raycast Direction
                public static DirectionVector GetDirection()
                {
                    try { return mastScript.placementRaycast.direction; }
                    catch { return Settings.Data.placement.placementRaycast.direction; }
                }
                
                // Get Raycast Start Offset
                public static Vector3 GetStartOffset()
                {
                    try { return mastScript.placementRaycast.startOffset; }
                    catch { return Settings.Data.placement.placementRaycast.startOffset; }
                }
            }
            
            // ---------------------------------------------------------------------------
            // Randomizer
            // ---------------------------------------------------------------------------
            public class Randomizer
            {
                // Does this Prefab use the Randomizer?
                public static bool GetUseRandomizer()
                {
                    if (Settings.Data.placement.overridePrefabRandomizer)
                        return Settings.Data.placement.randomizer.useRandomizer;
                    else
                        try { return mastScript.randomizer.useRandomizer; }
                        catch { return Settings.Data.placement.randomizer.useRandomizer; }
                }
                
                // Randomize rotation
                public class Replace
                {
                    // Get replaceable flag
                    public static bool GetReplaceable()
                    {
                        try { return mastScript.randomizer.allowReplacement; }
                        catch { return false; }
                    }
                    
                    // Get replace ID int
                    public static int GetReplaceID()
                    {
                        try { return mastScript.randomizer.replacementID; }
                        catch { return 0; }
                    }
                }
                
                // Randomize rotation
                public class Rotation
                {
                    public static Vector3 GetStep()
                    {
                        try { return mastScript.randomizer.rotateStep; }
                        catch { return Settings.Data.placement.randomizer.rotateStep; }
                    }
                    public static Vector3 GetMin()
                    {
                        try { return mastScript.randomizer.rotateMin; }
                        catch { return Settings.Data.placement.randomizer.rotateMin; }
                    }
                    public static Vector3 GetMax()
                    {
                        try { return mastScript.randomizer.rotateMax; }
                        catch { return Settings.Data.placement.randomizer.rotateMax; }
                    }
                }
                
                // Randomize scale
                public class Scale
                {
                    public static Vector3 GetMin()
                    {
                        try { return mastScript.randomizer.scaleMin; }
                        catch { return Settings.Data.placement.randomizer.scaleMin; }
                    }
                    public static Vector3 GetMax()
                    {
                        try { return mastScript.randomizer.scaleMax; }
                        catch { return Settings.Data.placement.randomizer.scaleMax; }
                    }
                    public static ScaleAxisLock GetLock()
                    {
                        try { return (ScaleAxisLock)(int)mastScript.randomizer.scaleLock; }
                        catch { return Settings.Data.placement.randomizer.scaleLock; }
                    }
                }
                
                // Randomize position
                public class Position
                {
                    public static Vector3 GetMin()
                    {
                        try { return mastScript.randomizer.posMin; }
                        catch { return Settings.Data.placement.randomizer.posMin; }
                    }
                    public static Vector3 GetMax()
                    {
                        try { return mastScript.randomizer.posMax; }
                        catch { return Settings.Data.placement.randomizer.posMax; }
                    }
                }
                
                // Randomize flip
                public class Flip
                {
                    public static bool GetX()
                    {
                        try { return mastScript.randomizer.flipX; }
                        catch { return Settings.Data.placement.randomizer.flipX; }
                    }
                    public static bool GetY()
                    {
                        try { return mastScript.randomizer.flipY; }
                        catch { return Settings.Data.placement.randomizer.flipY; }
                    }
                    public static bool GetZ()
                    {
                        try { return mastScript.randomizer.flipZ; }
                        catch { return Settings.Data.placement.randomizer.flipZ; }
                    }
                }
            }
            
            // Can prefab be placed inside others?
            public static bool GetAllowOverlap()
            {
                try { return mastScript.allowOverlap; }
                catch { return true; }
            }
            
            // Can prefab be scaled?
            public static bool GetPaintAreaStretch()
            {
                try { return mastScript.paintAreaStretch; }
                catch { return true; }
            }
        #endregion
        // ---------------------------------------------------------------------------

        }
    }
}

#endif