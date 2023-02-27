using UnityEngine;

#if (UNITY_EDITOR)
namespace MAST
{
    namespace Building
    {
        namespace Palette
        {
            public class Manager
            {
                // Persistent access to the MAST_Prefab_Palette_IO Class
                private static MAST.Building.Palette.IO PaletteIOClass;
                private static MAST.Building.Palette.IO PaletteIO
                {
                    get
                    {
                        if(PaletteIOClass == null)
                            PaletteIOClass = new MAST.Building.Palette.IO();
                        return PaletteIOClass;
                    }
                }
                
                // Folder names
                
                private static GameObject[] prefabs;
                private static Texture2D[] texture2D;
                private static string[] tooltip;
                private static GUIContent[] guiContent;
                
                public static int selectedItemIndex = -1;
                public static int selectedFolderIndex = 0;
                
                // ---------------------------------------------------------------------------
                // Prefab Palette
                // ---------------------------------------------------------------------------
                
                private static string[] paths;
                private static string[] folderNames;
                
                public static void GenerateThumbnailsAndLoadMaterials(string defaultPath, int newFolderIndex, bool recreateAllThumbnails)
                {
                    // Initialize the PaletteIO Class so it's ready for use
                    PaletteIO.Initialize(defaultPath, true, recreateAllThumbnails);
                    
                    // Get list of subfolders, filtering out the existing thumbnail folders
                    folderNames = PaletteIO.GetFolderNames();
                    
                    // Create palette items from the prefabs
                    ChangeActivePaletteFolder(newFolderIndex);
                }
                
                public static void LoadPrefabs(string defaultPath, int newFolderIndex)
                {
                    //Debug.Log("Loading Prefabs");
                    
                    // Initialize the PaletteIO Class so it's ready for use
                    PaletteIO.Initialize(defaultPath, false, false);
                    
                    // Get list of subfolders, filtering out the existing thumbnail folders
                    folderNames = PaletteIO.GetFolderNames();
                    
                    // Create palette items from the prefabs
                    ChangeActivePaletteFolder(newFolderIndex);
                }
                
                public static void ChangeActivePaletteFolder(int newFolderIndex)
                {
                    // Save the new prefab folder index
                    selectedFolderIndex = newFolderIndex;
                    
                    // Prepare prefabs and thumbnails in folder
                    PaletteIO.ChangeActivePrefabFolder(selectedFolderIndex - 1);
                    
                    // Create palette items from the prefabs
                    CreatePaletteItems();
                }
                
                private static void CreatePaletteItems()
                {
                    // Get prefabs and thumbnails
                    prefabs = PaletteIO.GetActivePrefabs();
                    texture2D = PaletteIO.GetActiveThumbnails();
                    
                    // Initialize guiContent and tooltip arrays
                    guiContent = new GUIContent[prefabs.Length];
                    tooltip = new string[prefabs.Length];
                    
                    // Create GUI Content from Images and Prefab names
                    for (int i = 0; i < prefabs.Length; i++)
                    {
                        // Create tooltip from object name
                        tooltip[i] = prefabs[i].name.Replace("_", "\n").Replace(" ", "\n");
                        
                        // Make sure texture has a transparent background
                        if (texture2D[i] != null)
                            texture2D[i].alphaIsTransparency = true;
                        
                        // Create GUIContent from texture and tooltip
                        guiContent[i] = new GUIContent(texture2D[i], tooltip[i]);
                    }
                }
                
                public static string[] GetFolderNameArray()
                {
                    return PaletteIO.GetFolderNames();
                }
                
                // Does the Palette contain prefabs?
                public static bool IsReady()
                {
                    return (guiContent != null && guiContent.Length > 0);
                }
                
                // Were palette images lost
                public static bool ArePaletteImagesLost()
                {
                    // If array is empty, return true
                    if (texture2D == null)
                        return true;
                    
                    // If image in array is empty, return true
                    return (texture2D[0] == null);
                }
                
                // Get the Palette Prefab (GameObject) array
                public static GameObject[] GetPrefabArray()
                {
                    return prefabs;
                }
                
                // Get the Palette Texture2D array
                public static Texture2D[] GetTexture2DArray()
                {
                    return texture2D;
                }
                
                // Get the Palette GUIContent array for display
                public static GUIContent[] GetGUIContentArray()
                {
                    return guiContent;
                }
                
                // Return currently selected prefab in the palette
                public static GameObject GetSelectedPrefab()
                {
                    return prefabs[selectedItemIndex];
                }
                
            }
        }
    }
}
#endif