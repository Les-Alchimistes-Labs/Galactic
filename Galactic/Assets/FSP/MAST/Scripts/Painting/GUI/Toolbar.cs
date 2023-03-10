using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Painting
    {
        namespace GUI
        {
            public static class Toolbar
            {
                // ------------------------------
                // Image Variables
                // ------------------------------
                private static GUIContent[] guiContentDrawTool;
                private static Texture2D iconLoadFromFolder;
                
                // ---------------------------------------------------------------------------
                private static void LoadImages()
                {
                    guiContentDrawTool = new GUIContent[2];
                    guiContentDrawTool[0] = new GUIContent(MAST.LoadingHelper.GetImage("Paint_Brush.png"), "Paint Material Tool");
                    guiContentDrawTool[1] = new GUIContent(MAST.LoadingHelper.GetImage("Cleaning_Brush.png"), "Restore Material Tool");
                    
                    iconLoadFromFolder = MAST.LoadingHelper.GetImage("Load_From_Folder.png");
                }
                
                // ---------------------------------------------------------------------------
                public static void DisplayToolbarGUI(float toolBarIconSize)
                {
                    if (iconLoadFromFolder == null)
                    {
                        LoadImages();
                    }
                    
                    GUILayout.Space(toolBarIconSize / 10);
                    
                    GUILayout.BeginVertical();
                    
                    GUILayout.Space(toolBarIconSize / 7.5f);
                    
                    // ----------------------------------------------
                    // Draw Tool Section
                    // ----------------------------------------------
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    
                    // ------------------------------------
                    // Add Draw Tool Toggle Group
                    // ------------------------------------
                    EditorGUI.BeginChangeCheck ();
                    
                    // Create drawtools SelectionGrid
                    int newSelectedPaintToolIndex = GUILayout.SelectionGrid(
                        MAST.Settings.Data.gui.toolbar.selectedPaintToolIndex, 
                        guiContentDrawTool, 1, "MAST Toggle",
                        GUILayout.Width(toolBarIconSize), 
                        GUILayout.Height(toolBarIconSize * 2));
                    
                    // If the draw tool was changed
                    if (EditorGUI.EndChangeCheck ()) {
                        
                        // If the paint tool was clicked again, deselect it
                        if (newSelectedPaintToolIndex == MAST.Settings.Data.gui.toolbar.selectedPaintToolIndex)
                        {
                            MAST.Settings.Data.gui.toolbar.selectedPaintToolIndex = -1;
                            
                            MAST.GUI.DataManager.state.previousPaintToolIndex = -1;
                            
                            MAST.Painting.Painter.ClearCurrentMaterialPaintPreview();
                        }
                        
                        // If a different paint tool was selected, change it
                        else
                        {
                            MAST.Settings.Data.gui.toolbar.selectedPaintToolIndex = newSelectedPaintToolIndex;
                        }
                    }
                    
                    GUILayout.EndVertical();
                    
                    GUILayout.Space(toolBarIconSize / 5);
                    
                    // ----------------------------------------------
                    // Misc Section
                    // ----------------------------------------------
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    
                    // ------------------------------------
                    // Load Material Palette Button
                    // ------------------------------------
                    if (GUILayout.Button(new GUIContent(iconLoadFromFolder, "Load materials from a project folder"),
                        "MAST Button", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize)))
                    {
                        // Show choose folder dialog
                        string chosenPath =
                            EditorUtility.OpenFolderPanel("Choose the Folder that Contains your Materials",
                            MAST.LoadingHelper.ConvertProjectPathToAbsolutePath(MAST.GUI.DataManager.state.materialPath), "");
                        
                        // If a folder was chosen "Cancel was not clicked"
                        if (chosenPath != "")
                        {
                            // Generate thumbnails, load materials, and select the first folder found (0)
                            MAST.Painting.Palette.Manager.GenerateThumbnailsAndLoadMaterials(chosenPath, 0, MAST.Settings.Data.gui.palette.overwriteThumbnails);
                            
                            // Refresh the AssetDatabase incase any new thumbnails were created
                            AssetDatabase.Refresh();
                            
                            // Save the material path and currently selected material folder index
                            MAST.GUI.DataManager.state.materialPath = MAST.LoadingHelper.ConvertAbsolutePathToProjectPath(chosenPath);
                            MAST.GUI.DataManager.state.selectedMaterialPaletteFolderIndex = MAST.Painting.Palette.Manager.selectedFolderIndex;
                            
                            //MAST_Interface_Data_Manager.Save_Palette_Items(true);
                            MAST.GUI.DataManager.Save_Changes_To_Disk();
                        }
                    }
                    
                    GUILayout.EndVertical();
                    
                    GUILayout.EndVertical();
                }
            }
        }
    }
}
#endif