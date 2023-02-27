using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Building
    {
        public static class Placement
        {
            // GameObject reference for target parent of all placed prefeabs
            [SerializeField] public static GameObject targetParent = null;
            
            // Placement
            [SerializeField] public static Vector3 lastPosition;
            
            public static GameObject PlacePrefabInScene()
            {
                // Make sure target parent is referenced
                ReferenceTargetParent();
                
                if (Visualizer.visualizerOnGrid && Visualizer.GetGameObject() != null &&
                    Visualizer.GetGameObject() != null)
                {
                    // If drawing continuous and an object was just drawn here, exit without drawing
                    if (Interface.placementMode == BuildMode.DrawContinuous)
                        if (lastPosition == Visualizer.GetGameObject().transform.position)
                            return null;
                    
                    // If GameObject already exists here, exit without placing the block
                    if (GameObjectAlreadyHere())
                        return null;
                    
                    // Instantiate the prefab
                    //GameObject newPrefab = GameObject.Instantiate(MAST_Palette.GetSelectedPrefab());
                    GameObject newPrefab = (GameObject) PrefabUtility.InstantiatePrefab(Palette.Manager.GetSelectedPrefab());
                    
                    // Correct GameObject transform and name "to remove (clone)"
                    newPrefab.transform.rotation = Visualizer.GetGameObject().transform.rotation;
                    newPrefab.transform.localScale = Visualizer.GetGameObject().transform.localScale;
                    newPrefab.transform.position = Visualizer.GetGameObject().transform.position;
                    newPrefab.name = Palette.Manager.GetSelectedPrefab().name;
                    
                    // Make new prefab child of the target parent
                    newPrefab.transform.parent = targetParent.transform;
                    
                    // Randomize the seed after placement
                    if (Settings.Data.gui.toolbar.selectedDrawToolIndex == 3)
                        Randomizer.GenerateNewRandomSeed();
                    
                    // Save this GameObject position
                    lastPosition = Visualizer.GetGameObject().transform.position;
                    
                    // Make this an Undo point, just after placing the prefab
                    Undo.RegisterCreatedObjectUndo(newPrefab, "Placed new Prefab");
                    
                    // Return with newly created GameObject
                    return newPrefab;
                }
                
                return null;
            }
            
            // Check if a GameObject already exists here and if neither can be placed inside others
            private static bool GameObjectAlreadyHere()
            {
                // Get array containing all Colliders within 1.5f square box at placement position
                Collider[] colliders =
                    Physics.OverlapBox(
                    Visualizer.GetGameObject().transform.position,
                    new Vector3(0.75f, 0.75f, 0.75f));
                
                // Loop through each GameObject inside or colliding with this OverlapBox
                foreach (Collider collider in colliders)
                {
                    // If the nearby GameObject has a parent
                    if (collider.gameObject.transform.parent != null)
                    {
                        // Get Parent GameObject for the GameObject containing this Collider
                        GameObject nearObject = collider.gameObject.transform.parent.gameObject;
                        
                        // If near GameObject is not the visualizer itself
                        if (nearObject.name != "MAST_Visualizer")
                        {
                            // If the placed GameObject shares the same Position as the near GameObject
                            if (nearObject.transform.position ==
                                Visualizer.GetGameObject().transform.position)
                            {
                                //TODO:  Add rotation check as well
                                
                                // Get the MAST Component script of the near GameObject
                                Component.MASTPrefabSettings nearComponent = nearObject.GetComponent<Component.MASTPrefabSettings>();
                                
                                // If neither the GameObject nor placed GameObject can be placed inside others 
                                if (!Helper.GetAllowOverlap())
                                    if (!nearComponent.allowOverlap)
                                    {
                                        // Return true "Not safe to place"
                                        return true;
                                    }
                            }
                        }
                    }
                }
                // Return false "Safe to place"
                return false;
            }
            
            // Make sure target parent is referenced
            public static void ReferenceTargetParent()
            {
                // Create Tag for target parent if it doesn't already exist
                MAST.TagsAndLayers.AddTag(MAST.Const.Placement.defaultTargetParentTag);
                
                // Try to find a GameObject with the MAST_Holder tag
                GameObject taggedGameObject = GameObject.FindGameObjectWithTag(MAST.Const.Placement.defaultTargetParentTag);
                
                // If GameObject with MAST_Holder tag exists, then use it
                if (taggedGameObject != null)
                {
                    targetParent = taggedGameObject;
                }
                
                // If GameObject with MAST_Holder tag does not exist, then create a new GameObject
                else
                {
                    targetParent = new GameObject();
                    targetParent.transform.position = new Vector3(0, 0, 0);
                    targetParent.name = Const.Placement.defaultTargetParentName;
                    targetParent.tag = MAST.Const.Placement.defaultTargetParentTag;
                }
                
                // Get target parent from saved target parent name
                //targetParent = GameObject.Find(Settings.Data.placement.targetParentName);
                //targetParent = (GameObject)EditorUtility.InstanceIDToObject(Settings.Data.placement.targetParentInstanceID);
                
                // If target parent doesn't exist, create it and named it "MAST_Holder"
                //if (!targetParent)
                //{
                //    targetParent = new GameObject();
                //    targetParent.transform.position = new Vector3(0, 0, 0);
                //    Settings.Data.placement.targetParentInstanceID = targetParent.GetInstanceID();
                //    //Settings.Data.placement.targetParentName = Const.placement.defaultTargetParentName;
                //    //targetParent.name = Settings.Data.placement.targetParentName;
                //    targetParent.name = Const.Placement.defaultTargetParentName;
                //}
            }
            
        }
    }
}

#endif