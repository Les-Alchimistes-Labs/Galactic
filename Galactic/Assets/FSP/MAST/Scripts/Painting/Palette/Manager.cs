using UnityEngine;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Painting
    {
        namespace Palette
        {
            public class Manager
            {
                // Persistent access to the MAST_Material_Palette_IO Class
                private static MAST.Painting.Palette.IO PaletteIOClass;
                private static MAST.Painting.Palette.IO PaletteIO
                {
                    get
                    {
                        if(PaletteIOClass == null)
                            PaletteIOClass = new MAST.Painting.Palette.IO();
                        return PaletteIOClass;
                    }
                }
                
                // Folder names
                
                private static Material[] materials;
                private static Texture2D[] texture2D;
                private static string[] tooltip;
                private static GUIContent[] guiContent;
                
                public static int selectedItemIndex = -1;
                public static int selectedFolderIndex = 0;
                
                // ---------------------------------------------------------------------------
                // Material Palette
                // ---------------------------------------------------------------------------
                
                private static string[] paths;
                private static string[] folderNames;
                
                public static void GenerateThumbnailsAndLoadMaterials(string defaultPath, int newFolderIndex, bool recreateAllThumbnails)
                {
                    // Initialize the PaletteIO Class so it's ready for use
                    PaletteIO.Initialize(defaultPath, true, recreateAllThumbnails);
                    
                    // Get list of subfolders, filtering out the existing thumbnail folders
                    folderNames = PaletteIO.GetFolderNames();
                    
                    // Create palette items from the materials
                    ChangeActivePaletteFolder(newFolderIndex);
                }
                
                public static void LoadMaterials(string defaultPath, int newFolderIndex)
                {
                    //Debug.Log("Loading Materials");
                    
                    // Initialize the PaletteIO Class so it's ready for use
                    PaletteIO.Initialize(defaultPath, false, false);
                    
                    // Get list of subfolders, filtering out the existing thumbnail folders
                    folderNames = PaletteIO.GetFolderNames();
                    
                    // Create palette items from the materials
                    ChangeActivePaletteFolder(newFolderIndex);
                }
                
                public static void ChangeActivePaletteFolder(int newFolderIndex)
                {
                    // Save the new material folder index
                    selectedFolderIndex = newFolderIndex;
                    
                    // Prepare materials and thumbnails in folder
                    PaletteIO.ChangeActiveMaterialFolder(selectedFolderIndex - 1);
                    
                    // Create palette items from the materials
                    CreatePaletteItems();
                }
                
                private static void CreatePaletteItems()
                {
                    // Get materials and thumbnails
                    materials = PaletteIO.GetActiveMaterials();
                    texture2D = PaletteIO.GetActiveThumbnails();
                    
                    // Initialize guiContent and tooltip arrays
                    guiContent = new GUIContent[materials.Length];
                    tooltip = new string[materials.Length];
                    
                    // Create GUI Content from Images and Material names
                    for (int i = 0; i < materials.Length; i++)
                    {
                        // Create tooltip from object name
                        tooltip[i] = materials[i].name.Replace("_", "\n").Replace(" ", "\n");
                        
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
                
                // Does the Palette contain materials?
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
                
                // Get the Palette Material array
                public static Material[] GetMaterialArray()
                {
                    return materials;
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
                
                // Return currently selected material in the palette
                public static Material GetSelectedMaterial()
                {
                    return materials[selectedItemIndex];
                }
            }
        }
    }
}
#endif