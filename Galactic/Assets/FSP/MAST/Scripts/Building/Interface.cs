using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Building
    {
        public static class Interface
        {
            
            [SerializeField] public static BuildMode placementMode = BuildMode.None;
            
        // ---------------------------------------------------------------------------
        #region Change Placement Mode
        // ---------------------------------------------------------------------------
            public static void ChangePlacementMode(BuildMode newPlacementMode)
            {
                // Get new selected Draw Tool
                placementMode = newPlacementMode;
                
                // Remove any previous visualizer
                Visualizer.RemoveVisualizer();
                
                // --------------------------------
                // Create Visualizer
                // --------------------------------
                
                // If changed tool to Nothing or Eraser
                if (placementMode == BuildMode.None || placementMode == BuildMode.Erase)
                {
                    // If changed tool to Eraser, create eraser visualizer
                    if (placementMode == BuildMode.Erase)
                        ChangePrefabToEraser();
                }
                
                // If changed tool to Draw Single, Draw Continuous, Paint Area, or Randomizer
                else
                {
                    // If a palette item is selected
                    if (Palette.Manager.selectedItemIndex != -1)
                    {
                        // Create visualizer from selected item in the palette
                        GUI.Palette.ChangePrefabSelection();
                        
                        // If changed tool to Randomizer
                        if (placementMode == BuildMode.Randomize)
                        {
                            // Make a new random seed
                            Randomizer.GenerateNewRandomSeed();
                        }
                    }
                }
                
                // If Draw Continuous nor Paint Area tools are selected
                if (placementMode != BuildMode.DrawContinuous &&
                    placementMode != BuildMode.PaintArea)
                {
                    // Delete last saved position
                    Placement.lastPosition = Vector3.positiveInfinity;
                    
                    // Remove any paint area visualization
                    PaintArea.DeletePaintArea();
                }
                
            }
        #endregion
            
            // Change visualizer prefab when a new item is selected in the palette menu
            public static void ChangeSelectedPrefab()
            {
                // Get reference to the MAST script attached to the GameObject or null if no MAST script attached
                Helper.mastScript = 
                    Palette.Manager.GetSelectedPrefab().GetComponent<Component.MASTPrefabSettings>();
                
                // Remove any existing visualizer
                Visualizer.RemoveVisualizer();
                
                // Create a new visualizer
                Visualizer.CreateVisualizer(Palette.Manager.GetSelectedPrefab());
            }
            
            // Change visualizer to eraser
            public static void ChangePrefabToEraser()
            {
                // Remove any existing visualizer
                Visualizer.RemoveVisualizer();
                
                // Create a new visualizer with eraser
                Visualizer.CreateVisualizer(LoadingHelper.GetEraserPrefab());
            }
            
            public static void ErasePrefab()
            {
                // Get array containing all Colliders within eraser
                Collider[] colliders =
                    Physics.OverlapBox(
                        Visualizer.GetGameObject().transform.position +
                        new Vector3(0f, 0.35f, 0f), new Vector3(0.4f, 0.4f, 0.4f));
                
                // Loop through each GameObject inside or colliding with this OverlapBox
                foreach (Collider collider in colliders)
                {
                    // Use try/catch, incase this collider's GameObject is already destroyed
                    try
                    {
                        // If the nearby GameObject has a parent
                        if (collider.gameObject.transform.parent != null)
                        {
                            // Get Parent GameObject for the GameObject containing this Collider
                            GameObject objectToDelete = GetPrefabParent(collider.gameObject.transform).gameObject;
                            
                            // If a GameObject placed with MAST was found
                            if (objectToDelete != null)
                            {
                                // If near GameObject is not the visualizer itself
                                if (objectToDelete.name != "MAST_Visualizer" &&
                                    objectToDelete.name != Const.Grid.defaultName &&
                                    objectToDelete.name != Const.Grid.defaultParentName)
                                {
                                    // Erase it, but allow an undo
                                    Undo.DestroyObjectImmediate(objectToDelete);
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Do nothing since this collider's GameObject was already destroyed
                    }
                }
            }
            
            // Get Prefab parent of provided transform
            private static Transform GetPrefabParent(Transform transform)
            {
                // If this GameObject doesn't have a MAST_Prefab_Component script
                if (transform.gameObject.GetComponent<Component.MASTPrefabSettings>() == null)
                {
                    // Get result from GameObject parent or if at the top-level, return null
                    try { return GetPrefabParent(transform.parent); }
                    catch { return null; }
                }
                
                // If this GameObject has a MAST_Prefab_Component script, return it's transform
                else
                    return transform;
            }
        }
    }
}

#endif