using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Building
    {
        namespace Palette
        {
            public class IO
            {
                // Top-level folder path for all prefabs
                private string rootPrefabPath;
                
                // List of folder paths containing prefabs "possibly top-level folder + all subfolders with prefabs"
                private List<string> prefabFolderPaths;
                
                // Folder name for saving thumbnails relative to the prefab folder
                private string thumbnailFolderName;
                
                // GameObject array containing all active prefabs in the palette
                private GameObject[] activePrefabs;
                
                // Texture2D array containing thumbnails for all active prefabs in the palette
                private Texture2D[] activePrefabThumbnails;
                
                #region Public Methods
                
                // ------------------------------------------------------------------------------------------------
                // Get an array of subfolders located inside the specified path excluding thumbnail folders
                // ------------------------------------------------------------------------------------------------
                public void Initialize(string prefabPath, bool generateThumbnails, bool recreateAllThumbnails)
                {
                    // Set the thumbnail folder name
                    // *** Will move this later to the MAST_Const class ***
                    thumbnailFolderName = "_MAST_Thumbnails";
                    
                    // Save prefab path, converting full path to project local path "Assets/..."
                    rootPrefabPath = prefabPath.Replace(Application.dataPath, "Assets");
                    
                    // Create a list of all prefab paths
                    prefabFolderPaths = new List<string>();
                    CreatePrefabPathList();
                    
                    // Exit if no prefab folders were found
                    if (prefabFolderPaths.Count == 0)
                        return;
                    
                    // Make sure each prefab has a thumbnail
                    if (generateThumbnails)
                        PrepareThumbnails(recreateAllThumbnails);
                    
                    // Default to the first prefab folder, loading prefabs and thumbnails
                    ChangeActivePrefabFolder(0);
                }
                
                // ------------------------------------------------------------------------------------------------
                // Get an array containing just the prefab folder names, without the root prefab path
                // ------------------------------------------------------------------------------------------------
                public string[] GetFolderNames()
                {
                    // Initialize the Folder Array
                    string[] folders = new string[prefabFolderPaths.Count + 1];
                    
                    // Make the first option "All Prefabs"
                    folders[0] = "All Prefabs";
                    
                    // Loop through each Prefab Path
                    for (int i = 0; i < prefabFolderPaths.Count; i++)
                    {
                        // If this is the root prefab path, name this folder "Root"
                        if (prefabFolderPaths[i] == rootPrefabPath)
                        {
                            folders[i + 1] = "Root";
                        }
                        
                        // If this is not the root prefab path, remove the root prefab path, leaving only the folder and subfolder names
                        else
                        {
                            folders[i + 1] = prefabFolderPaths[i].Replace(rootPrefabPath + "/", "");
                        }
                    }
                    
                    // Return with the Folder Array
                    return folders;
                }
                
                // ------------------------------------------------------------------------------------------------
                // Load prefabs and thumbnails from the specified folder
                // ------------------------------------------------------------------------------------------------
                public void ChangeActivePrefabFolder(int prefabFolderIndex)
                {
                    // If a specific folder was chosen
                    if (prefabFolderIndex > -1)
                    {
                        // Get a string array containing all prefab GUIDs in this folder
                        string[] prefabGUIDs = AssetDatabase.FindAssets("t:prefab", new[] { prefabFolderPaths[prefabFolderIndex] });
                        
                        // Reinitialize prefab and thumbnail arrays
                        activePrefabs = new GameObject[prefabGUIDs.Length];
                        activePrefabThumbnails = new Texture2D[prefabGUIDs.Length];
                        
                        // Loop through each prefab GUID in the folder
                        for (int i = 0; i < prefabGUIDs.Length; i++)
                        {
                            // Get the path of the prefab
                            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUIDs[i]);
                            
                            string prefabFolderPath = Path.GetDirectoryName(prefabPath).Replace("\\", "/");
                            
                            string thumbnailPath = GetThumbnailPathFromPrefabPath(prefabPath);
                            
                            // If this prefab is located in this specific folder "not a subfolder" or loading all folders
                            if (prefabFolderPath == prefabFolderPaths[prefabFolderIndex])
                            {
                                // Load prefabs
                                activePrefabs[i] = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                                
                                // Load thumbnails
                                activePrefabThumbnails[i] = (Texture2D)AssetDatabase.LoadAssetAtPath(thumbnailPath, typeof(Texture2D));
                            }
                        }
                    }
                    
                    // If all folders were chosen
                    else
                    {
                        // Get a string array containing all prefab GUIDs in this folder
                        string[] prefabGUIDs = AssetDatabase.FindAssets("t:prefab", new[] { rootPrefabPath });
                        
                        // Reinitialize prefab and thumbnail arrays
                        activePrefabs = new GameObject[prefabGUIDs.Length];
                        activePrefabThumbnails = new Texture2D[prefabGUIDs.Length];
                        
                        // Loop through each prefab GUID in the folder
                        for (int i = 0; i < prefabGUIDs.Length; i++)
                        {
                            // Get the path of the prefab
                            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUIDs[i]);
                            
                            string prefabFolderPath = Path.GetDirectoryName(prefabPath).Replace("\\", "/");
                            
                            string thumbnailPath = GetThumbnailPathFromPrefabPath(prefabPath);
                            
                            // Load prefabs
                            activePrefabs[i] = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                            
                            // Load thumbnails
                            activePrefabThumbnails[i] = (Texture2D)AssetDatabase.LoadAssetAtPath(thumbnailPath, typeof(Texture2D));
                        }
                    }
                }
                
                public GameObject[] GetActivePrefabs()
                {
                    return activePrefabs;
                }
                
                public Texture2D[] GetActiveThumbnails()
                {
                    return activePrefabThumbnails;
                }
                
                #endregion
                
                #region Create Prefab Path List
                
                // ------------------------------------------------------------------------------------------------
                // Used by Initialize:  Find all folders containing prefabs and save the paths to a list
                // ------------------------------------------------------------------------------------------------
                private void CreatePrefabPathList()
                {
                    // Get an array containing all paths unfiltered
                    List<string> unfilteredPaths = new List<string>();
                    unfilteredPaths.Add(rootPrefabPath);
                    List<string> unfilteredSubPaths = GetSubfoldersRecursively(unfilteredPaths, rootPrefabPath);
                    unfilteredPaths.AddRange(unfilteredSubPaths);
                    
                    // Loop through each path in the unfiltered path array
                    foreach (string path in unfilteredPaths)
                    {
                        // Get a string array containing all prefab GUIDs in this folder
                        string[] prefabGUIDs = AssetDatabase.FindAssets("t:prefab", new[] { path });
                        
                        // Loop through each prefab GUID in the folder
                        foreach (string prefabGUID in prefabGUIDs)
                        {
                            // Get the path of the prefab
                            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUID);
                            prefabPath = Path.GetDirectoryName(prefabPath).Replace("\\", "/");
                            
                            // If this prefab is located in this folder "not a subfolder"
                            if (prefabPath == path)
                            {
                                // Add the path to the prefab path list and exit the loop
                                prefabFolderPaths.Add(path);
                                break;
                            }
                        }
                    }
                }
                
                // ------------------------------------------------------------------------------------------------
                // Used by CreatePrefabPathList:  Recursively find all subfolders at the specified path
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
                // Create thumbnails for prefabs
                // ------------------------------------------------------------------------------------------------
                private void PrepareThumbnails(bool recreateAllThumbnails)
                {
                    // Get a list containing paths of any prefabs missing thumbnails
                    List<string> newPrefabPaths = GetPathsOfPrefabsMissingThumbnails(recreateAllThumbnails);
                    
                    // Create folders where necessary
                    CreateMissingThumbnailFolders();
                    
                    // Create thumbnails for any new prefabs "no thumbnail found for them"
                    CreateAndSaveThumbnails(newPrefabPaths);
                }
                
                // ------------------------------------------------------------------------------------------------
                // Used by InitializeThumbnails:  Get a list of prefab paths that are missing thumbnails
                // ------------------------------------------------------------------------------------------------
                private List<string> GetPathsOfPrefabsMissingThumbnails(bool recreateAllThumbnails)
                {
                    // Initialize the missing thumbnail prefab path list
                    List<string> missingThumbnailPrefabPaths = new List<string>();
                    
                    // Loop through each prefab folder
                    foreach (string path in prefabFolderPaths)
                    {
                        // Flag whether this prefab folder has a thumbnail subfolder
                        bool hasThumbnailFolder = AssetDatabase.IsValidFolder(path + "/" + thumbnailFolderName);
                        
                        // Get a string array containing all prefab GUIDs in this folder
                        string[] prefabGUIDs = AssetDatabase.FindAssets("t:prefab", new[] { path });
                        
                        // Loop through each prefab GUID in the folder
                        foreach (string prefabGUID in prefabGUIDs)
                        {
                            // Get the path of the prefab
                            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUID);
                            
                            string prefabFolderPath = Path.GetDirectoryName(prefabPath).Replace("\\", "/");
                            
                            // If this prefab is located in this folder "not a subfolder"
                            if (prefabFolderPath == path)
                            {
                                // If a thumbnail folder exists
                                if (hasThumbnailFolder && !recreateAllThumbnails)
                                {
                                    // Create the expected prefab thumbnail path
                                    string thumbnailPath = GetThumbnailPathFromPrefabPath(prefabPath);
                                    
                                    // If no thumbnail exists for this prefab, add the prefab's path to the missing thumbnail list
                                    if (AssetDatabase.LoadAssetAtPath(thumbnailPath, typeof(Texture2D)) == null)
                                    {
                                        missingThumbnailPrefabPaths.Add(prefabPath);
                                    }
                                }
                                
                                // If no thumbnail folder exists or user specified to overwrite existing thumbnails
                                else
                                {
                                    // Add the prefab's path to the missing thumbnail list
                                    missingThumbnailPrefabPaths.Add(prefabPath);
                                }
                            }
                        }
                    }
                    return missingThumbnailPrefabPaths;
                }
                
                // ------------------------------------------------------------------------------------------------
                // Used by InitializeThumbnails:  Create any missing thumbnail folders
                // ------------------------------------------------------------------------------------------------
                private void CreateMissingThumbnailFolders()
                {
                    // Loop through each prefab folder
                    foreach (string path in prefabFolderPaths)
                    {
                        // If the the thumbnail folder is missing for this prefab folder, create it
                        if(!AssetDatabase.IsValidFolder(path + "/" + thumbnailFolderName))
                            AssetDatabase.CreateFolder(path, thumbnailFolderName);
                    }
                }
                
                // ------------------------------------------------------------------------------------------------
                // Used by InitializeThumbnails:  Get a list of prefab paths that are missing thumbnails
                // ------------------------------------------------------------------------------------------------
                private void CreateAndSaveThumbnails(List<string> prefabPaths)
                {
                    // Initialize a prefab array
                    GameObject[] prefabs = new GameObject[prefabPaths.Count];
                    
                    // Loop through all prefab paths in the list
                    for (int i = 0; i < prefabPaths.Count; i++)
                    {
                        // Load prefab at this path and save it to the prefab array
                        prefabs[i] = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPaths[i], typeof(GameObject));
                    }
                    
                    // Initialize thumbnail image array
                    Texture2D[] thumbnailImages;
                    
                    // Generate images of all prefabs and save to a Texture2D array
                    thumbnailImages = MAST.LoadingHelper.GetThumbnailCamera()
                        .GetComponent<MAST.Component.ThumbnailCamera>()
                        .GetPrefabThumbnails(prefabs);
                    
                    // Save the thumbnails to their correct folders
                    for (int i = 0; i < prefabPaths.Count; i++)
                    {
                        // Encode this thumbnail image to PNG
                        byte[] bytes = thumbnailImages[i].EncodeToPNG();
                        
                        // Save this thumbnail to correct folder
                        string thumbnailPath = GetThumbnailPathFromPrefabPath(prefabPaths[i]);
                        File.WriteAllBytes(thumbnailPath, bytes);
                    }
                    
                    // Refresh the assetdatabase so the new thumbnail images can be accessed
                    AssetDatabase.Refresh();
                    
                }
                
                #endregion
                
                // ------------------------------------------------------------------------------------------------
                // Used by GetPathsOfPrefabsMissingThumbnails and CreateAndSaveThumbnails:
                // Get the expected thumbnail path based on the specifed prefab path
                // ------------------------------------------------------------------------------------------------
                private string GetThumbnailPathFromPrefabPath(string prefabPath)
                {
                    // Get the prefab filename without extension
                    string prefabName = Path.GetFileNameWithoutExtension(prefabPath);
                    
                    // Get the prefab path without the filename
                    string prefabPathWithoutFilename = Path.GetDirectoryName(prefabPath).Replace("\\", "/");
                    
                    // Return this prefab's expected thumbnail path
                    return prefabPathWithoutFilename + "/" + thumbnailFolderName + "/" + prefabName + ".png";
                }
                
            }
        }
    }
}
#endif