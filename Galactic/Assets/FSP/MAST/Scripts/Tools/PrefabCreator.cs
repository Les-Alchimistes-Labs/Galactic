using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Tools
    {
        public class Prefab_Creator
        {
            public enum MergeMethod { DoNotMerge, MergeChildren, MergeAll }
            
            // Selected folder
            private string pathSelected;
            private string pathMeshes;
            private string pathMaterials;
            private string pathPrefabs;
            
            // Original model files
            private GameObject[] sourceModels;
            
        #region StripMaterials
            
            // ---------------------------------------------------------------------------
            // Desc:    Strip Materials from all models in the specified folder
            //
            // In:      String path for models
            //
            // Out:     List containing all unique materials
            // ---------------------------------------------------------------------------
            public List<Material> StripMaterials(string targetPath)
            {
                // Return empty if no path was selected
                if (targetPath == "")
                    return null;
                
                // Get Models as a GameObject array
                GameObject[] models = GetModelsInFolder(targetPath);
                
                // If Models were found
                if (models != null)
                {
                    List<Material> mats = new List<Material>();
                    
                    // Loop through each model
                    for (int i = 0; i < models.Length; i++)
                    {
                        // Begin a recursive algorithm that iterates through all children on down
                        // Returns a Material List containing only unique Materials
                        mats = AppendUniqueMaterials(models[i].transform, mats);
                    }
                    
                    // Sort material list by material names
                    mats = mats.OrderBy(Material=>Material.name).ToList();
                    
                    return mats;
                }
                
                // NO Models were found
                return null;
            }
            
            // ---------------------------------------------------------------------------
            // Desc:    Get a GameObject array from the specific folder
            //
            // Used by StripMaterials and CreatePrefabs.
            // ---------------------------------------------------------------------------
            // In:      String folder path for models
            //
            // Out:     GameObject array containing all "model" GameObjects in the folder
            // ---------------------------------------------------------------------------
            private GameObject[] GetModelsInFolder(string targetPath)
            {
                // Get all GameObject GUID's in the specified path and any subfolders of it
                string[] modelPath = GetPathOfModelsInFolder(targetPath);
                
                // If models were found
                if (modelPath != null)
                {
                    // Create array to store the gameObjects
                    GameObject[] modelGameObject = new GameObject[modelPath.Length];
                    
                    // Loop through each GameObject in the folder
                    for (int i = 0; i < modelPath.Length; i++)
                    {
                        // Get gameObject at path
                        modelGameObject[i] = (GameObject)AssetDatabase.LoadMainAssetAtPath(modelPath[i]);
                    }
                    
                    return modelGameObject;
                }
                
                return null;
            }
            
            public string[] GetPathOfModelsInFolder(string targetPath)
            {
                // Get all GameObject GUID's in the specified path and any subfolders of it
                string[] GUIDOfAllGameObjectsInFolder = AssetDatabase.FindAssets("t:gameobject", new[] { targetPath });
                
                // If any models were found
                if (GUIDOfAllGameObjectsInFolder.Length > 0)
                {
                    // Create string array to store the model paths
                    string[] modelPath = new string[GUIDOfAllGameObjectsInFolder.Length];
                    
                    // Loop through each GameObject in the folder
                    for (int i = GUIDOfAllGameObjectsInFolder.Length - 1; i >= 0; i--)
                    {
                        // Convert GUID at current index to path string
                        modelPath[i] = AssetDatabase.GUIDToAssetPath(GUIDOfAllGameObjectsInFolder[i]);
                    }
                    
                    return modelPath;
                }
                
                return null;
            }
            
            // ---------------------------------------------------------------------------
            // Desc:    Recursive search that returns a list of unique materials and
            //          appends any new materials to the supplied list and returns it.
            //
            // Used by StripMaterials.
            // ---------------------------------------------------------------------------
            // In:      Transform
            //
            // Out:     Material List containing unique materials
            // ---------------------------------------------------------------------------
            private List<Material> AppendUniqueMaterials(Transform transform, List<Material> mats)
            {
                // Get this GameObject's MeshRenderer
                MeshRenderer meshRenderer = transform.gameObject.GetComponent<MeshRenderer>();
                
                // If MeshRenderer is found
                if (meshRenderer)
                {
                    // Get Materials (array) in this MeshRenderer
                    Material[] tempMats = meshRenderer.sharedMaterials;
                    
                    // Flag to show if material was found
                    bool foundMat;
                    
                    // Loop through each Material
                    for (int t = 0; t < tempMats.Length; t++)
                    {
                        
                        // Set found material flag back to false
                        foundMat = false;
                        
                        // Loop through each material in the Material list
                        foreach (Material material in mats)
                        {
                            // If material names match set found material flag to true
                            if (tempMats[t].name == material.name)
                                foundMat = true;
                        }
                        
                        // If the Material doesn't already exist in the unique list, add it
                        if (!foundMat)
                        {
                            mats.Add(tempMats[t]);
                        }
                    }
                }
                
                // Run this method for all child transforms
                foreach (Transform childTransform in transform)
                {
                    mats = AppendUniqueMaterials(childTransform, mats);
                }
                
                // Return with the current Material List
                return mats;
            }
            
        #endregion
            
            // ------------------------------------------------------------------------------------------------
            // Desc:    Look for a Material in a Material List
            // ------------------------------------------------------------------------------------------------
            // In:      string          targetPath      Path containing models
            //          List<Material>  sourceMat       Original Materials stripped from the models.
            //          string[]        sourceMatName   Original Material names.  Used to find the object's
            //                                            material in the primary material list.
            //          int[]           conMatPointer   Pointers linking the source material list to new materials
            //          string[]        conMatName      New Material names "shorter array with combined materials"
            //                                            used to rename the material.
            //          Material[]      conMat          Consolidated material array.  Already contains subsituted
            //                                            materials.
            //          bool            flagAddMeshCollider     Should a MeshCollider be added for each mesh
            //          bool            flagAddEmptyParent      Should an empty parent GameObject be created
            //          bool            flagPreserveModelHierarchy      Should prefab use same parent/child hierarchy as model
            //
            // Out:     Bool whether process was successful
            // ------------------------------------------------------------------------------------------------
            public bool CreatePrefabs(string targetPath, List<Material> sourceMat, List<string> sourceMatName,
                                    int[] conMatPointer, string[] conMatName, Material[] conMat,
                                    bool flagAddMeshCollider, bool flagAddEmptyParent, MergeMethod mergeMethod)
            {
                // Create a new "/Prefabs" folder, if one doesn't exist
                if (!AssetDatabase.IsValidFolder(targetPath + "/Prefabs"))
                    AssetDatabase.CreateFolder(targetPath, "Prefabs");
                
                AssetDatabase.SaveAssets();
                
                // Convert source Material name array to List for easy searching
                List<string> searchMatName = new List<string>(sourceMatName);
                
                // Loop through each source Material and give each a reference to the extracted or substituted Material
                for (int i = 0; i < sourceMat.Count; i++)
                {
                    sourceMat[i] = conMat[conMatPointer[i]];
                }
                
                // Get all models in the target folder
                GameObject[] model = GetModelsInFolder(targetPath);
                
                // Create new Prefab and Prefab child GameObjects
                GameObject finalGameObject = null;
                
                // Loop through each model
                for (int i = 0; i < model.Length; i++)
                {
                    // ------------------------------------------------------------
                    // Create new GameObject based on merge method chosen
                    // ------------------------------------------------------------
                    
                    switch (mergeMethod)
                    {
                        // If merging children only, create a GameObject with a semi-preserved hierarchy, only merging meshes
                        //     in GameObjects that are at the end of the hierarchy and shared by the same parent GameObject
                        case MergeMethod.MergeChildren:
                            finalGameObject = CreateGameObjectWithMergedChildren(model[i].transform, searchMatName, sourceMat, model[i].name);
                            break;
                        
                        // If not performing a merge, create a GameObject with a fully preserved hierarchy
                        case MergeMethod.DoNotMerge:
                            finalGameObject = CreateGameObjectWithPreservedHierarchy(model[i].transform, searchMatName, sourceMat, model[i].name);
                            break;
                        
                        // If merging everything, create a GameObject with a single MeshFilter containing all Meshes combined
                        case MergeMethod.MergeAll:
                            finalGameObject = CreateGameObjectWithSingleMesh(model[i].transform, searchMatName, sourceMat);
                            break;
                    }
                    
                    // ------------------------------------------------------------
                    // Add parent GameObject if requested and needed
                    // ------------------------------------------------------------
                    
                    GameObject prefabGameObject;
                    
                    // If adding an empty parent GameObject and the GameObject doesn't already have an empty parent GameObject
                    if (flagAddEmptyParent && finalGameObject.GetComponent<MeshRenderer>())
                    {
                        // Create an empty parent with model's name and attach the gameobject as a child
                        prefabGameObject = new GameObject(model[i].name);
                        finalGameObject.transform.parent = prefabGameObject.transform;
                    }
                    
                    // If not adding an empty parent GameObject or an empty parent GameObject already exists
                    else
                    {
                        // Create an empty parent with model's name
                        prefabGameObject = new GameObject(model[i].name);
                        
                        // If the final GameObject has a MeshRenderer at its top level
                        if (finalGameObject.GetComponent<MeshRenderer>())
                        {
                            // Copy the MeshRender and MeshFilter to the Prefab
                            MeshRenderer meshRenderer = prefabGameObject.AddComponent<MeshRenderer>();
                            meshRenderer = finalGameObject.GetComponent<MeshRenderer>();
                            
                            MeshFilter meshFilter = prefabGameObject.AddComponent<MeshFilter>();
                            meshFilter = finalGameObject.GetComponent<MeshFilter>();
                        }
                        
                        // Move each child of the final GameObject to the Prefab manually, iterate through children in reverse since they will disappear
                        for (int j = finalGameObject.transform.childCount - 1; j > -1 ; j--)
                        {
                            finalGameObject.transform.GetChild(j).parent = prefabGameObject.transform;
                        }
                        
                        // Delete the the empty final GameObject
                        GameObject.DestroyImmediate(finalGameObject);
                    }
                    
                    // If user wants to add a MeshCollider to the Prefab
                    if (flagAddMeshCollider)
                    {
                        // Get all MeshRenderers in the Prefab
                        MeshRenderer[] meshRenderers = prefabGameObject.GetComponentsInChildren<MeshRenderer>();
                        
                        // Add a MeshCollider to each GameObject with a MeshRenderer
                        foreach (MeshRenderer childMeshRenderer in meshRenderers)
                            childMeshRenderer.gameObject.AddComponent<MeshCollider>();
                    }
                    
                    // ------------------------------------------------------------
                    // Save Prefab along with any Meshes it uses
                    // ------------------------------------------------------------
                    
                    // Create a Prefab from the new GameObject, saving it as the model's name
                    GameObject prefab = PrefabUtility.SaveAsPrefabAsset(prefabGameObject, targetPath + "/Prefabs/" + model[i].name + ".prefab");
                    
                    // Save all Meshes with the GameObject
                    AttachAllMeshesInGameObjectToPrefab(prefabGameObject.transform, prefab.transform, prefab);
                    
                    // Delete the Prefab from the scene/Hierarchy
                    GameObject.DestroyImmediate(prefabGameObject);
                }
                
                // Save all changes
                AssetDatabase.SaveAssets();
                
                // Display a warning.  If the user clicks "Continue"
                if (EditorUtility.DisplayDialog("Prefab Creation Complete",
                    "Successfully created " + model.Length + " prefabs!  They are located in the (" + targetPath + "/Prefabs"
                    + ") folder.  The extracted Materials are located in (" + targetPath + "/Materials" + ") and the Prefab Meshes are located in ("
                    + targetPath + "/Meshes).  The original models are no longer required for the prefabs.",
                    "Got It!"))
                {
                    
                }
                
                return true;
            }
            
            // ------------------------------------------------------------------------------------------------
            // Desc:    Save all meshes in the GameObject to the Prefab and link the Meshes (Recursive)
            // ------------------------------------------------------------------------------------------------
            private GameObject AttachAllMeshesInGameObjectToPrefab(Transform sourceTransform, Transform prefabTransform, GameObject prefab)
            {
                // Find MeshFilter at this level
                MeshFilter meshFilter = sourceTransform.GetComponent<MeshFilter>();
                
                // If a MeshFilter was found, attach it to the Prefab and link it to the Prefab's MeshFilter
                if (meshFilter)
                {
                    AssetDatabase.AddObjectToAsset(meshFilter.sharedMesh, prefab);
                    prefabTransform.GetComponent<MeshFilter>().sharedMesh = meshFilter.sharedMesh;
                    prefabTransform.GetComponent<MeshCollider>().sharedMesh = meshFilter.sharedMesh;
                }
                
                // Run this method for all child transforms
                for (int i = 0; i < sourceTransform.transform.childCount; i++)
                {
                    prefab = AttachAllMeshesInGameObjectToPrefab(sourceTransform.transform.GetChild(i), prefabTransform.transform.GetChild(i), prefab);
                }
                
                // Return this new GameObject
                return prefab;
            }
            
            // ------------------------------------------------------------------------------------------------
            // Desc:    Create a GameObject from a model, merging everything into a single mesh
            // ------------------------------------------------------------------------------------------------
            // In:      Transform       modelTransform          Transform of the model being searched through
            //          List<string>    searchMatName           Original Material names.  Used to find the object's
            //          Material[]      savedMat                Direct reference to the saved mat files
            //          string          targetPath              Location of the models
            //
            // Out:     GameObject      newGameObject   GameObject being created
            // ------------------------------------------------------------------------------------------------
            
            private GameObject CreateGameObjectWithSingleMesh(Transform sourceTransform, List<string> searchMatName, List<Material> savedMat)
            {
                // Create a new GameObject that will become the Prefab of this Model, and name it after the Model
                GameObject newGameObject = new GameObject(sourceTransform.gameObject.name);
                
                // Get all MeshFilters and MeshRenderers from the source GameObject, including all children
                MeshFilter[] modelMeshFilters = sourceTransform.gameObject.GetComponentsInChildren<MeshFilter>();
                MeshRenderer[] modelMeshRenderers = sourceTransform.gameObject.GetComponentsInChildren<MeshRenderer>();
                
                // Create a List containing all unique Materials found in the Model
                List<Material> uniqueMats = MAST.Tools.Mesh_Helper.GetUniqueMaterialListFromMeshRendererArray(modelMeshRenderers);
                
                // Create a GameObject with all SubMeshes combined into a single Mesh
                GameObject combineMeshGameObject = MAST.Tools.Mesh_Helper.CombineAllMeshesInGameObject(uniqueMats, modelMeshFilters, modelMeshRenderers);
                
                // Create MeshFilter on new GameObject and copy the Mesh from the source
                MeshFilter gameObjectMeshFilter = newGameObject.AddComponent<MeshFilter>();
                gameObjectMeshFilter.sharedMesh = (Mesh)GameObject.Instantiate(combineMeshGameObject.GetComponent<MeshFilter>().sharedMesh);
                gameObjectMeshFilter.sharedMesh.name = sourceTransform.gameObject.name + "_mesh";
                
                // Create MeshRenderer component in the final GameObject and copy all Materials from the combined GameObject
                MeshRenderer finalMeshRenderer = newGameObject.AddComponent<MeshRenderer>();
                
                // Prepare Material List for the final GameObject "may have subsituted materials"
                List<Material> finalMats = new List<Material>();
                
                // Get the MeshRenderer from the combined Mesh GameObject
                MeshRenderer combineMeshRenderer = combineMeshGameObject.transform.GetComponent<MeshRenderer>();
                
                // ------------------------------------------------------------
                // Replace the Model's Materials with any chosen substitutions
                // ------------------------------------------------------------
                // Loop through each material in this MeshRenderer's sharedMaterials array
                foreach (Material material in combineMeshRenderer.sharedMaterials)
                {
                    // Search the searchMatName List for this Material's name and add the savedMat with the same index
                    finalMats.Add(savedMat[searchMatName.IndexOf(material.name)]);
                }
                
                // Destroy temporary Combine GameObject
                GameObject.DestroyImmediate(combineMeshGameObject);
                
                // Save the final Material array "including substitutions"
                finalMeshRenderer.sharedMaterials = finalMats.ToArray();
                // ------------------------------------------------------------
                
                // Return with the new GameObject
                return newGameObject;
            }
            
            
            
            // ------------------------------------------------------------------------------------------------
            // Desc:    Recursive search to create a GameObject from the components of a model
            // ------------------------------------------------------------------------------------------------
            // In:      Transform       modelTransform  Transform of the model being searched through
            //          List<string>    searchMatName   Original Material names.  Used to find the object's
            //          Material[]      savedMat        Direct reference to the saved mat files
            //          string          targetPath      Location of the models
            //          string          saveName        Name of the model
            //          int             saveIndex       0 for first child in array, 1 for 2nd child in array
            //
            // Out:     GameObject      newGameObject   GameObject being created
            // ------------------------------------------------------------------------------------------------
            
            private GameObject CreateGameObjectWithPreservedHierarchy(Transform sourceTransform, List<string> searchMatName,
                                                        List<Material> savedMat, string saveName, int saveIndex = 0)
            {
                // Add to the current saved name with this child's index
                saveName = saveName + "_" + saveIndex;
                
                // Create a new GameObject to hold this model's data from this child level
                GameObject newGameObject = new GameObject();
                
                // Rename the GameObject to the model's name at this child level
                newGameObject.name = sourceTransform.gameObject.name;
                
                // --------------------------
                // Mesh
                // --------------------------
                
                // Get this model's MeshRenderer
                MeshRenderer modelMeshRenderer = sourceTransform.gameObject.GetComponent<MeshRenderer>();
                
                // If MeshRenderer is found
                if (modelMeshRenderer)
                {
                    // Create MeshFilter on new GameObject and copy the Mesh from the source
                    MeshFilter gameObjectMeshFilter = newGameObject.AddComponent<MeshFilter>();
                    gameObjectMeshFilter.sharedMesh = (Mesh)GameObject.Instantiate(sourceTransform.gameObject.GetComponent<MeshFilter>().sharedMesh);
                    gameObjectMeshFilter.sharedMesh.name = saveName + "_mesh";
                    
                    // --------------------------
                    // Materials
                    // --------------------------
                    
                    // Add MeshRenderer component to the GameObject
                    MeshRenderer gameObjectMeshRenderer = newGameObject.AddComponent<MeshRenderer>();
                    
                    // Create Material List for the GameObject
                    List<Material> gameObjectMats = new List<Material>();
                    
                    // Loop through each material in the Material list
                    foreach (Material material in modelMeshRenderer.sharedMaterials)
                    {
                        // Search the searchMatName List for this Material's name and add the savedMat with the same index
                        gameObjectMats.Add(savedMat[searchMatName.IndexOf(material.name)]);
                    }
                    
                    // Add material array to the GameObject's MeshRenderer
                    gameObjectMeshRenderer.sharedMaterials = gameObjectMats.ToArray();
                }
                
                // --------------------------
                // Transforms
                // --------------------------
                
                // Apply transforms to the GameObject
                newGameObject.transform.position = sourceTransform.position;
                newGameObject.transform.rotation = sourceTransform.rotation;
                newGameObject.transform.localScale = sourceTransform.lossyScale;
                
                // --------------------------
                // Children
                // --------------------------
                
                // Run this method for all child transforms
                for (int i = 0; i < sourceTransform.childCount; i++)
                {
                    // Create a new GameObject for this child GameObject in the model
                    GameObject newChildGameObject = CreateGameObjectWithPreservedHierarchy(sourceTransform.GetChild(i),
                        searchMatName, savedMat, saveName, i);
                    
                    // Attach it as a child of this new GameObject
                    newChildGameObject.transform.parent = newGameObject.transform;
                }
                
                // Return this new GameObject
                return newGameObject;
            }
            
            
            
            // ------------------------------------------------------------------------------------------------
            // Desc:    Recursive search to create a GameObject from the components of a model
            //          with all children merged.
            // ------------------------------------------------------------------------------------------------
            // In:      Transform       modelTransform  Transform of the model being searched through
            //          List<string>    searchMatName   Original Material names.  Used to find the object's
            //          Material[]      savedMat        Direct reference to the saved mat files
            //          string          saveName        Name of the model
            //          int             saveIndex       0 for first child in array, 1 for 2nd child in array
            //
            // Out:     GameObject      newGameObject   GameObject being created
            // ------------------------------------------------------------------------------------------------
            
            private GameObject CreateGameObjectWithMergedChildren(Transform sourceTransform, List<string> searchMatName,
                                                        List<Material> savedMat, string saveName, int saveIndex = 0)
            {
                // Add to the current saved name with this child's index
                saveName = saveName + "_" + saveIndex;
                
                // Create a new GameObject to hold this model's data from this child level
                GameObject newGameObject = new GameObject();
                
                // Rename the GameObject to the model's name at this child level
                newGameObject.name = sourceTransform.gameObject.name;
                
                // Get this model's MeshRenderer
                MeshRenderer modelMeshRenderer = sourceTransform.gameObject.GetComponent<MeshRenderer>();
                
                // If MeshRenderer is found
                if (modelMeshRenderer)
                {
                    // ------------------------------------------
                    // Copy mesh from source to new GameObject
                    // ------------------------------------------
                    
                    // Create MeshFilter on new GameObject and copy the Mesh from the source
                    MeshFilter gameObjectMeshFilter = newGameObject.AddComponent<MeshFilter>();
                    gameObjectMeshFilter.sharedMesh = (Mesh)GameObject.Instantiate(sourceTransform.gameObject.GetComponent<MeshFilter>().sharedMesh);
                    
                    // ------------------------------------------
                    // Materials
                    // ------------------------------------------
                    
                    // Add MeshRenderer component to the GameObject
                    MeshRenderer gameObjectMeshRenderer = newGameObject.AddComponent<MeshRenderer>();
                    
                    // Create Material List for the GameObject
                    List<Material> gameObjectMats = new List<Material>();
                    
                    // Loop through each material in the Material list
                    foreach (Material material in modelMeshRenderer.sharedMaterials)
                    {
                        // Search the searchMatName List for this Material's name and add the savedMat with the same index
                        gameObjectMats.Add(savedMat[searchMatName.IndexOf(material.name)]);
                    }
                    
                    // Add material array to the GameObject's MeshRenderer
                    gameObjectMeshRenderer.sharedMaterials = gameObjectMats.ToArray();
                }
                
                // --------------------------
                // Transforms
                // --------------------------
                
                // Apply transforms to the GameObject
                newGameObject.transform.position = sourceTransform.position;
                newGameObject.transform.rotation = sourceTransform.rotation;
                newGameObject.transform.localScale = sourceTransform.lossyScale;
                
                // --------------------------
                // Children
                // --------------------------
                
                List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
                List<MeshFilter> meshFilters = new List<MeshFilter>();
                
                int childIndex = 0;
                
                // Run this method for all child transforms
                for (int i = 0; i < sourceTransform.childCount; i++)
                {
                    // If this child GameObject has its own children
                    if (sourceTransform.GetChild(i).childCount > 0)
                    {
                        // Increase child index
                        childIndex += 1;
                        
                        // Create a new GameObject for this child GameObject in the model
                        GameObject newChildGameObject = CreateGameObjectWithMergedChildren(sourceTransform.GetChild(i),
                            searchMatName, savedMat, saveName, childIndex);
                        
                        // Attach it as a child of this new GameObject
                        newChildGameObject.transform.parent = newGameObject.transform;
                    }
                    
                    // If this child GameObject has no children
                    else
                    {
                        // Add the child's MeshRender and MeshFilter to the combine lists
                        meshRenderers.Add(sourceTransform.GetChild(i).GetComponent<MeshRenderer>());
                        meshFilters.Add(sourceTransform.GetChild(i).GetComponent<MeshFilter>());
                        
                    }
                }
                
                // If any meshes were found to combine
                if (meshRenderers.Count > 0)
                {
                    
                    // Get list of mats in this GameObject
                    List<Material> matList = MAST.Tools.Mesh_Helper.GetUniqueMaterialListFromMeshRendererArray(meshRenderers.ToArray());
                    
                    // Create a GameObject with all SubMeshes combined into a single Mesh
                    GameObject combineMeshGameObject = MAST.Tools.Mesh_Helper.CombineAllMeshesInGameObject(matList, meshFilters.ToArray(), meshRenderers.ToArray());
                    
                    matList.Clear();
                    
                    // Loop through each material in this MeshRenderer's sharedMaterials array
                    foreach (Material material in combineMeshGameObject.GetComponent<MeshRenderer>().sharedMaterials)
                    {
                        // Search the searchMatName List for this Material's name and add the savedMat with the same index
                        matList.Add(savedMat[searchMatName.IndexOf(material.name)]);
                    }
                    
                    // Save the final Material array "including substitutions"
                    combineMeshGameObject.GetComponent<MeshRenderer>().sharedMaterials = matList.ToArray();
                    
                    // Name GameObject and Mesh and make it a child of the current GameObject
                    combineMeshGameObject.name = saveName + "_0";
                    combineMeshGameObject.GetComponent<MeshFilter>().sharedMesh.name = saveName + "_0_mesh";
                    combineMeshGameObject.transform.parent = newGameObject.transform;
                }
                
                // Return this new GameObject
                return newGameObject;
            }
            
            
        }
    }
}
#endif