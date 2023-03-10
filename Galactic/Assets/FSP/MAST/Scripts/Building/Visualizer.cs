using UnityEngine;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Building
    {
        public static class Visualizer
        {
            [SerializeField] public static GameObject visualizerGameObject = null;
            [SerializeField] public static bool visualizerOnGrid = false;
            
            // Is the mouse pointer in the sceneview
            [SerializeField] public static bool pointerInSceneview = false;
            
            public static GameObject GetGameObject()
            {
                return visualizerGameObject;
            }
            public static void SetGameObject(GameObject newVisualizer)
            {
                visualizerGameObject = newVisualizer;
            }
            
            // Create the visualizer GameObject
            public static void CreateVisualizer(GameObject selectedPrefab)
            {
                // Exit without creating, if no Prefab is selected in the palette
                if (selectedPrefab == null)
                    return;
                
                // Create a new visualizer
                visualizerGameObject = GameObject.Instantiate(selectedPrefab);
                SetLayerRecursively(visualizerGameObject.transform, Const.Placement.visualizerLayer);
                
                // Name it "MAST_Visualizer" incase it needs to be found later for deletion
                visualizerGameObject.name = "MAST_Visualizer";
                
                // If not selecting the Eraser
                if (Settings.Data.gui.toolbar.selectedDrawToolIndex != 4)
                {
                    // If saved rotation is valid for the visualizer object, then apply the rotation to it
                    if (IsSavedRotationValidForVisualizer())
                        visualizerGameObject.transform.rotation = Manipulate.GetCurrentRotation();
                }
                
                // Set the visualizer and all it's children to be unselectable and not shown in the hierarchy
                visualizerGameObject.hideFlags = HideFlags.HideInHierarchy;
            }
            
            // ---------------------------------------------------------------------------
            // Recursively loop through all children and set their layers
            // ---------------------------------------------------------------------------
            private static void SetLayerRecursively(Transform transform, int layer)
            {
                transform.gameObject.layer = layer;
                
                foreach (Transform childTransform in transform)
                    SetLayerRecursively(childTransform, layer);
            }
            
            // See if new selected Prefab will allow the saved rotation
            private static bool IsSavedRotationValidForVisualizer()
            {
                // If there is a saved rotation
                if (Manipulate.GetCurrentRotation() != null)
                {
                    // If the current saved rotation is allowed by the prefab
                    //   (If allowed rotation divides evenly into current rotation)
                    //       or (Both allowed and current rotations are set to 0)
                    if (((int)(Manipulate.GetCurrentRotation().eulerAngles.x % Helper.GetRotationStep().x) == 0)
                    || (int)Manipulate.GetCurrentRotation().eulerAngles.x == 0 && (int)Helper.GetRotationStep().x == 0)
                        if (((int)(Manipulate.GetCurrentRotation().eulerAngles.y % Helper.GetRotationStep().y) == 0)
                        || (int)Manipulate.GetCurrentRotation().eulerAngles.y == 0 && (int)Helper.GetRotationStep().y == 0)
                            if (((int)(Manipulate.GetCurrentRotation().eulerAngles.z % Helper.GetRotationStep().z) == 0)
                            || (int)Manipulate.GetCurrentRotation().eulerAngles.z == 0 && (int)Helper.GetRotationStep().z == 0)
                            {
                                // Return true, since saved rotation is allowed
                                return true;
                            }
                }
                // Return false, since saved rotation is not allowed
                return false;
            }
            
            // Destroy current visualizer GameObject
            public static void RemoveVisualizer()
            {
                // Find existing visualizer prefab by name and delete
                // using this method to find disabled visualizer prefabs
                foreach (GameObject gameObject in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                    if (gameObject.name == "MAST_Visualizer")
                        GameObject.DestroyImmediate(gameObject);
            }
            
            // Change visualizer prefab visibility
            // Make prefab visible or invisible if pointer is in scene view on grid
            public static void SetVisualizerVisibility(bool visible)
            {
                pointerInSceneview = visible;
                
                if (visualizerGameObject != null)
                    visualizerGameObject.SetActive(visible);
            }
            
            // Moves the visualizer prefab to a position based on the current mouse position
            public static void UpdateVisualizerPosition()
            {
                // If a tool is selected
                if (Interface.placementMode != BuildMode.None)
                    
                    // If visualizer exists
                    if (visualizerGameObject != null)
                    {
                        // Update visualizer position from pointer location on grid
                        visualizerGameObject.transform.position =
                            Helper.GetPositionOnGridClosestToMousePointer();
                        
                        // If Eraser tool is not selected
                        if (Settings.Data.gui.toolbar.selectedDrawToolIndex != 4)
                        {
                            // Apply position offset
                            visualizerGameObject.transform.position += Helper.GetOffsetPosition();
                            
                            // If Randomizer is selected
                            if (Settings.Data.gui.toolbar.selectedDrawToolIndex == 3)
                                // If Prefab in randomizable, apply Randomizer to transform
                                if (Helper.Randomizer.GetUseRandomizer())
                                    visualizerGameObject = Randomizer.ApplyRandomizerToTransform(
                                        visualizerGameObject, Manipulate.GetCurrentRotation());
                        }
                        
                        // Set visualizer visibility based on if mouse over grid
                        if (pointerInSceneview)
                            visualizerGameObject.SetActive(visualizerOnGrid);
                    }
            }
        }
    }
}

#endif