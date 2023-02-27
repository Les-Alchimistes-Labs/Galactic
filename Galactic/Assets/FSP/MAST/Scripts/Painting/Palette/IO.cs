using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Painting
    {
        namespace Palette
        {
            public class IO
            {
                // Top-level folder path for all materials
                private string rootMaterialPath;
                
                // List of folder paths containing materials "possibly top-level folder + all subfolders with materials"
                private List<string> materialFolderPaths;
                
                // Folder name for saving thumbnails relative to the material folder
                private string thumbnailFolderName;
                
                // GameObject array containing all active materials in the palette
                private Material[] activeMaterials;
                
                // Texture2D array containing thumbnails for all active materials in the palette
                private Texture2D[] activeMaterialsThumbnails;
                
                #region Public Methods
                
                // ------------------------------------------------------------------------------------------------
                // Get an array of subfolders located inside the specified path excluding thumbnail folders
                // ------------------------------------------------------------------------------------------------
                public void Initialize(string materialPath, bool generateThumbnails, bool recreateAllThumbnails)
                {
                    // Set the thumbnail folder name
                    // *** Will move this later to the MAST_Const class ***
                    thumbnailFolderName = "_MAST_Thumbnails";
                    
                    // Save material path, converting full path to project local path "Assets/..."
                    rootMaterialPath = materialPath.Replace(Application.dataPath, "Assets");
                    
                    // Create a list of all material paths
                    materialFolderPaths = new List<string>();
                    CreateMaterialPathList();
                    
                    // Exit if no material folders were found
                    if (materialFolderPaths.Count == 0)
                        return;
                    
                    // Make sure each material has a thumbnail
                    if (generateThumbnails)
                        PrepareThumbnails(recreateAllThumbnails);
                    
                    // Default to the first material folder, loading materials and thumbnails
                    ChangeActiveMaterialFolder(0);
                }
                
                // ------------------------------------------------------------------------------------------------
                // Get an array containing just the material folder names, without the root material path
                // ------------------------------------------------------------------------------------------------
                public string[] GetFolderNames()
                {
                    // Initialize the Folder Array
                    string[] folders = new string[materialFolderPaths.Count + 1];
                    
                    // Make the first option "All Materials"
                    folders[0] = "All Materials";
                    
                    // Loop through each Material Path
                    for (int i = 0; i < materialFolderPaths.Count; i++)
                    {
                        // If this is the root material path, name this folder "Root"
                        if (materialFolderPaths[i] == rootMaterialPath)
                        {
                            folders[i + 1] = "Root";
                        }
                        
                        // If this is not the root material path, remove the root material path, leaving only the folder and subfolder names
                        else
                        {
                            folders[i + 1] = materialFolderPaths[i].Replace(rootMaterialPath + "/", "");
                        }
                    }
                    
                    // Return with the Folder Array
                    return folders;
                }
                
                // ------------------------------------------------------------------------------------------------
                // Load materials and thumbnails from the specified folder
                // ------------------------------------------------------------------------------------------------
                public void ChangeActiveMaterialFolder(int materialFolderIndex)
                {
                    // If a specific folder was chosen
                    if (materialFolderIndex > -1)
                    {
                        // Get a string array containing all material GUIDs in this folder
                        string[] materialGUIDs = AssetDatabase.FindAssets("t:material", new[] { materialFolderPaths[materialFolderIndex] });
                        
                        // Reinitialize material and thumbnail arrays
                        activeMaterials = new Material[materialGUIDs.Length];
                        activeMaterialsThumbnails = new Texture2D[materialGUIDs.Length];
                        
                        // Loop through each material GUID in the folder
                        for (int i = 0; i < materialGUIDs.Length; i++)
                        {
                            // Get the path of the material
                            string materialPath = AssetDatabase.GUIDToAssetPath(materialGUIDs[i]);
                            
                            string materialFolderPath = Path.GetDirectoryName(materialPath).Replace("\\", "/");
                            
                            string thumbnailPath = GetThumbnailPathFromMaterialPath(materialPath);
                            
                            // If this material is located in this specific folder "not a subfolder" or loading all folders
                            if (materialFolderPath == materialFolderPaths[materialFolderIndex])
                            {
                                // Load materials
                                //AssetDatabase.LoadAssetAtPath("Assets/MyMaterialNew.mat", typeof(Material)) as Material
                                activeMaterials[i] = AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material)) as Material;
                                //activeMaterials[i] = (GameObject)AssetDatabase.LoadAssetAtPath(materialPath, typeof(GameObject));
                                
                                // Load thumbnails
                                activeMaterialsThumbnails[i] = (Texture2D)AssetDatabase.LoadAssetAtPath(thumbnailPath, typeof(Texture2D));
                            }
                        }
                    }
                    
                    // If all folders were chosen
                    else
                    {
                        // Get a string array containing all material GUIDs in this folder
                        string[] materialGUIDs = AssetDatabase.FindAssets("t:material", new[] { rootMaterialPath });
                        
                        // Reinitialize material and thumbnail arrays
                        activeMaterials = new Material[materialGUIDs.Length];
                        activeMaterialsThumbnails = new Texture2D[materialGUIDs.Length];
                        
                        // Loop through each material GUID in the folder
                        for (int i = 0; i < materialGUIDs.Length; i++)
                        {
                            // Get the path of the material
                            string materialPath = AssetDatabase.GUIDToAssetPath(materialGUIDs[i]);
                            
                            string materialFolderPath = Path.GetDirectoryName(materialPath).Replace("\\", "/");
                            
                            string thumbnailPath = GetThumbnailPathFromMaterialPath(materialPath);
                            
                            // Load materials
                            activeMaterials[i] = AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material)) as Material;
                            //activeMaterials[i] = (Material)AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material));
                            
                            // Load thumbnails
                            activeMaterialsThumbnails[i] = (Texture2D)AssetDatabase.LoadAssetAtPath(thumbnailPath, typeof(Texture2D));
                        }
                    }
                }
                
                public Material[] GetActiveMaterials()
                {
                    return activeMaterials;
                }
                
                public Texture2D[] GetActiveThumbnails()
                {
                    return activeMaterialsThumbnails;
                }
                
                #endregion
                
                #region Create Material Path List
                
                // ------------------------------------------------------------------------------------------------
                // Used by Initialize:  Find all folders containing materials and save the paths to a list
                // ------------------------------------------------------------------------------------------------
                private void CreateMaterialPathList()
                {
                    // Get an array containing all paths unfiltered
                    List<string> unfilteredPaths = new List<string>();
                    unfilteredPaths.Add(rootMaterialPath);
                    List<string> unfilteredSubPaths = GetSubfoldersRecursively(unfilteredPaths, rootMaterialPath);
                    unfilteredPaths.AddRange(unfilteredSubPaths);
                    
                    // Loop through each path in the unfiltered path array
                    foreach (string path in unfilteredPaths)
                    {
                        // Get a string array containing all material GUIDs in this folder
                        string[] materialGUIDs = AssetDatabase.FindAssets("t:material", new[] { path });
                        
                        // Loop through each material GUID in the folder
                        foreach (string materialGUID in materialGUIDs)
                        {
                            // Get the path of the material
                            string materialPath = AssetDatabase.GUIDToAssetPath(materialGUID);
                            materialPath = Path.GetDirectoryName(materialPath).Replace("\\", "/");
                            
                            // If this material is located in this folder "not a subfolder"
                            if (materialPath == path)
                            {
                                // Add the path to the material path list and exit the loop
                                materialFolderPaths.Add(path);
                                break;
                            }
                        }
                    }
                }
                
                // ------------------------------------------------------------------------------------------------
                // Used by CreateMaterialPathList:  Recursively find all subfolders at the specified path
                // ------------------------------------------------------------------------------------------------
                private List<string> GetSubfoldersRecursively(List<string> pathList, string searchPath)
                {
                    // Get an array containing all paths
                    string[] paths = AssetDatabase.GetSubFolders(searchPath);
                    
                    // Loop through each path in the path array
                    foreach (string path in paths)
                    {
                        // Add this path to the path list
                        pathList.Add(path);
                        
                        // Run this method for this path to look for more subfolders
                        GetSubfoldersRecursively(pathList, path);
                    }
                    
                    // Return the path list
                    return pathList;
                }
                
                #endregion
                
                #region Prepare Thumbnails
                
                // ------------------------------------------------------------------------------------------------
                // Create thumbnails for materials
                // ------------------------------------------------------------------------------------------------
                private void PrepareThumbnails(bool recreateAllThumbnails)
                {
                    // Get a list containing paths of any materials missing thumbnails
                    List<string> newMaterialPaths = GetPathsOfMaterialsMissingThumbnails(recreateAllThumbnails);
                    
                    // Create folders where necessary
                    CreateMissingThumbnailFolders();
                    
                    // Create thumbnails for any new materials "no thumbnail found for them"
                    CreateAndSaveThumbnails(newMaterialPaths);
                }
                
                // ------------------------------------------------------------------------------------------------
                // Used by InitializeThumbnails:  Get a list of material paths that are missing thumbnails
                // ------------------------------------------------------------------------------------------------
                private List<string> GetPathsOfMaterialsMissingThumbnails(bool recreateAllThumbnails)
                {
                    // Initialize the missing thumbnail material path list
                    List<string> missingThumbnailMaterialPaths = new List<string>();
                    
                    // Loop through each material folder
                    foreach (string path in materialFolderPaths)
                    {
                        // Flag whether this material folder has a thumbnail subfolder
                        bool hasThumbnailFolder = AssetDatabase.IsValidFolder(path + "/" + thumbnailFolderName);
                        
                        // Get a string array containing all material GUIDs in this folder
                        string[] materialGUIDs = AssetDatabase.FindAssets("t:material", new[] { path });
                        
                        // Loop through each material GUID in the folder
                        foreach (string materialGUID in materialGUIDs)
                        {
                            // Get the path of the material
                            string materialPath = AssetDatabase.GUIDToAssetPath(materialGUID);
                            
                            string materialFolderPath = Path.GetDirectoryName(materialPath).Replace("\\", "/");
                            
                            // If this material is located in this folder "not a subfolder"
                            if (materialFolderPath == path)
                            {
                                // If a thumbnail folder exists
                                if (hasThumbnailFolder && !recreateAllThumbnails)
                                {
                                    // Create the expected material thumbnail path
                                    string thumbnailPath = GetThumbnailPathFromMaterialPath(materialPath);
                                    
                                    // If no thumbnail exists for this material, add the material's path to the missing thumbnail list
                                    if (AssetDatabase.LoadAssetAtPath(thumbnailPath, typeof(Texture2D)) == null)
                                    {
                                        missingThumbnailMaterialPaths.Add(materialPath);
                                    }
                                }
                                
                                // If no thumbnail folder exists or user specified to overwrite existing thumbnails
                                else
                                {
                                    // Add the material's path to the missing thumbnail list
                                    missingThumbnailMaterialPaths.Add(materialPath);
                                }
                            }
                        }
                    }
                    return missingThumbnailMaterialPaths;
                }
                
                // ------------------------------------------------------------------------------------------------
                // Used by InitializeThumbnails:  Create any missing thumbnail folders
                // ------------------------------------------------------------------------------------------------
                private void CreateMissingThumbnailFolders()
                {
                    // Loop through each material folder
                    foreach (string path in materialFolderPaths)
                    {
                        // If the the thumbnail folder is missing for this material folder, create it
                        if(!AssetDatabase.IsValidFolder(path + "/" + thumbnailFolderName))
                            AssetDatabase.CreateFolder(path, thumbnailFolderName);
                    }
                }
                
                // ------------------------------------------------------------------------------------------------
                // Used by InitializeThumbnails:  Get a list of material paths that are missing thumbnails
                // ------------------------------------------------------------------------------------------------
                private void CreateAndSaveThumbnails(List<string> materialPaths)
                {
                    // Initialize a material array
                    Material[] materials = new Material[materialPaths.Count];
                    
                    // Loop through all material paths in the list
                    for (int i = 0; i < materialPaths.Count; i++)
                    {
                        // Load material at this path and save it to the material array
                        materials[i] = AssetDatabase.LoadAssetAtPath(materialPaths[i], typeof(Material)) as Material;
                    }
                    
                    // Initialize thumbnail image array
                    Texture2D[] thumbnailImages;
                    
                    // Generate images of all materials and save to a Texture2D array
                    thumbnailImages = MAST.LoadingHelper.GetThumbnailCamera()
                        .GetComponent<MAST.Component.ThumbnailCamera>()
                        .GetMaterialThumbnails(materials);
                    
                    // Save the thumbnails to their correct folders
                    for (int i = 0; i < materialPaths.Count; i++)
                    {
                        // Encode this thumbnail image to PNG
                        byte[] bytes = thumbnailImages[i].EncodeToPNG();
                        
                        // Save this thumbnail to correct folder
                        string thumbnailPath = GetThumbnailPathFromMaterialPath(materialPaths[i]);
                        File.WriteAllBytes(thumbnailPath, bytes);
                    }
                    
                    // Refresh the assetdatabase so the new thumbnail images can be accessed
                    AssetDatabase.Refresh();
                    
                }
                
                #endregion
                
                // ------------------------------------------------------------------------------------------------
                // Used by GetPathsOfMaterialsMissingThumbnails and CreateAndSaveThumbnails:
                // Get the expected thumbnail path based on the specifed material path
                // ------------------------------------------------------------------------------------------------
                private string GetThumbnailPathFromMaterialPath(string materialPath)
                {
                    // Get the material filename without extension
                    string materialName = Path.GetFileNameWithoutExtension(materialPath);
                    
                    // Get the material path without the filename
                    string materialPathWithoutFilename = Path.GetDirectoryName(materialPath).Replace("\\", "/");
                    
                    // Return this material's expected thumbnail path
                    return materialPathWithoutFilename + "/" + thumbnailFolderName + "/" + materialName + ".png";
                }
            }
        }
    }
}
#endif