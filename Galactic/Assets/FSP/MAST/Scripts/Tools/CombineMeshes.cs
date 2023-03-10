using System.Collections.Generic;
using UnityEngine;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Tools
    {
        public class CombineMeshes
        {
            
            public GameObject MergeMeshes(GameObject source)
            {
                // Instantiate a new copy of the source GameObject so the original is not changed
                GameObject sourceParent = GameObject.Instantiate(source);
                
                // ---------------------------------------------------------------------------
                // Remove GameObjects to Exclude from the Merge and move them back later
                // ---------------------------------------------------------------------------
                
                // Create a GameObject to be the Parent of all models excluded from the Combine operation
                GameObject excludeFromMergeParent = new GameObject("Not Merged");
                
                // Get all child Transforms in the Source GameObject
                Transform[] sourceTransforms = sourceParent.GetComponentsInChildren<Transform>();
                
                // Move any GameObjects not included in the merge to a separate Parent GameObject
                for (int i = sourceTransforms.Length - 1; i >= 0; i--)
                {
                    if (!IncludeInMerge(sourceTransforms[i].gameObject))
                        sourceTransforms[i].parent = excludeFromMergeParent.transform;
                }
                // ---------------------------------------------------------------------------
                
                // Get all MeshFilters and MeshRenderers from the source GameObject's children
                MeshFilter[] sourceMeshFilters = sourceParent.GetComponentsInChildren<MeshFilter>();
                MeshRenderer[] sourceMeshRenderers = sourceParent.GetComponentsInChildren<MeshRenderer>();
                
                // Create a List containing all unique Materials in the GameObjects
                List<Material> uniqueMats = MAST.Tools.Mesh_Helper.GetUniqueMaterialListFromMeshRendererArray(sourceMeshRenderers);
                
                // Create a GameObject with all SubMeshes combined into a single Mesh
                GameObject finalGameObject = MAST.Tools.Mesh_Helper.CombineAllMeshesInGameObject(uniqueMats, sourceMeshFilters, sourceMeshRenderers);
                
                // Name finalGameObject and make it the child of an empty parent
                GameObject finalGameObjectParent = new GameObject(sourceParent.name + " Merged");
                finalGameObject.transform.parent = finalGameObjectParent.transform;
                
                // If the Unmerged GameObject holder is isn't empty, make it a child of the empty parent
                if (excludeFromMergeParent.transform.childCount > 0)
                    excludeFromMergeParent.transform.parent = finalGameObjectParent.transform;
                
                // If the Unmerged GameObject holder is empty, delete it
                else
                    GameObject.DestroyImmediate(excludeFromMergeParent);
                
                // Delete unneeded GameObjects
                GameObject.DestroyImmediate(sourceParent);
                
                // Return the complete GameObject
                return finalGameObjectParent;
            }
            
            // Used by [MergeMeshes] to determine if a GameObject should be included in the Combine operation
            private bool IncludeInMerge(GameObject go)
            {
                // If prefab is not supposed to be included in the merge, don't include its material name
                MAST.Component.MASTPrefabSettings prefabComponent = go.GetComponent<MAST.Component.MASTPrefabSettings>();
                if (prefabComponent != null)
                    return prefabComponent.includeInMerge;
                
                // If no MAST prefab component was attached, include it in the merge
                return true;
            }
            
        }
    }
}

#endif