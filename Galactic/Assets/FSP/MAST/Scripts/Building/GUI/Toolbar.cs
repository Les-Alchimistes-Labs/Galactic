using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)
namespace MAST
{
    namespace Building
    {
        namespace GUI
        {
            public static class Toolbar
            {
                // ------------------------------
                // Image Variables
                // ------------------------------
                private static Texture2D iconGridToggle;
                private static Texture2D iconGridUp;
                private static Texture2D iconGridDown;
                private static Texture2D iconGridSnap;
                private static GUIContent[] guiContentDrawTool;
                private static Texture2D iconRotate;
                private static Texture2D iconFlip;
                private static Texture2D iconAxisX;
                private static Texture2D iconAxisY;
                private static Texture2D iconAxisZ;
                private static Texture2D iconLoadFromFolder;
                private static Texture2D iconSettings;
                private static Texture2D iconTools;
                
            // ---------------------------------------------------------------------------
            #region Load Images
            // ---------------------------------------------------------------------------
                private static void LoadImages()
                {
                    iconGridToggle = MAST.LoadingHelper.GetImage("Grid_Toggle.png");
                    iconGridUp = MAST.LoadingHelper.GetImage("Grid_Up.png");
                    iconGridDown = MAST.LoadingHelper.GetImage("Grid_Down.png");
                    
                    iconGridSnap = MAST.LoadingHelper.GetImage("Grid_Snap.png");
                    
                    guiContentDrawTool = new GUIContent[5];
                    guiContentDrawTool[0] = new GUIContent(MAST.LoadingHelper.GetImage("Pencil.png"), "Draw Single Tool");
                    guiContentDrawTool[1] = new GUIContent(MAST.LoadingHelper.GetImage("Paint_Roller.png"), "Draw Continuous Tool");
                    guiContentDrawTool[2] = new GUIContent(MAST.LoadingHelper.GetImage("Paint_Bucket.png"), "Paint Area Tool");
                    guiContentDrawTool[3] = new GUIContent(MAST.LoadingHelper.GetImage("Randomizer.png"), "Randomizer Tool");
                    guiContentDrawTool[4] = new GUIContent(MAST.LoadingHelper.GetImage("Eraser.png"), "Eraser Tool");
                    
                    iconRotate = MAST.LoadingHelper.GetImage("Rotate.png");
                    iconFlip = MAST.LoadingHelper.GetImage("Flip.png");
                    iconAxisX = MAST.LoadingHelper.GetImage("Axis_X.png");
                    iconAxisY = MAST.LoadingHelper.GetImage("Axis_Y.png");
                    iconAxisZ = MAST.LoadingHelper.GetImage("Axis_Z.png");
                    iconLoadFromFolder = MAST.LoadingHelper.GetImage("Load_From_Folder.png");
                    iconSettings = MAST.LoadingHelper.GetImage("Settings.png");
                    iconTools = MAST.LoadingHelper.GetImage("Tools.png");
                }
            #endregion
                
            #region Toolbar GUI
            // ---------------------------------------------------------------------------
                public static void DisplayToolbarGUI(float toolBarIconSize)
                {
                    if (iconGridToggle == null)
                    {
                        LoadImages();
                    }
                    
                    GUILayout.Space(toolBarIconSize / 10);
                    
                    GUILayout.BeginVertical();
                    
                    GUILayout.Space(toolBarIconSize / 7.5f);
                    
                    // --------------------------------------------------------------------------------------------
                    // Grid Section
                    // --------------------------------------------------------------------------------------------
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    
                    // ------------------------------------
                    // Grid Up Button
                    // ------------------------------------
                    if (GUILayout.Button(new GUIContent(iconGridUp, "Move Grid Up to " + (MAST.Settings.Data.gui.grid.gridHeight + 1)),
                        "MAST Half Button", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize / 2)))
                    {
                        MAST.Building.GridManager.MoveGridUp();
                    }
                    
                    // ------------------------------------
                    // Toggle Grid Button
                    // ------------------------------------
                    EditorGUI.BeginChangeCheck ();
                    
                    // OnScene Enable/Disable Randomizer Icon Button
                    MAST.Building.GridManager.gridExists = GUILayout.Toggle(
                        MAST.Building.GridManager.gridExists,
                        new GUIContent(iconGridToggle, "Toggle Scene Grid - Current Level " + MAST.Settings.Data.gui.grid.gridHeight),
                        "MAST Toggle", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize));
                    
                    // If randomizer enabled value changed, process the change
                    if (EditorGUI.EndChangeCheck ())
                    {
                        MAST.Building.GridManager.ChangeGridVisibility();
                    }
                    
                    // ------------------------------------
                    // Grid Down Button
                    // ------------------------------------
                    if (GUILayout.Button(new GUIContent(iconGridDown, "Move Grid Down to " + (MAST.Settings.Data.gui.grid.gridHeight - 1)),
                        "MAST Half Button", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize / 2)))
                    {
                        MAST.Building.GridManager.MoveGridDown();
                    }
                    
                    GUILayout.EndVertical();
                    
                    GUILayout.Space(toolBarIconSize / 5);
                    
                    // --------------------------------------------------------------------------------------------
                    // Grid Snap Section
                    // --------------------------------------------------------------------------------------------
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    
                    // Create Grid Snap button tooltip
                    string tooltipGridSnap;
                    if (MAST.Settings.Data.placement.snapToGrid)
                        tooltipGridSnap = "Turn OFF Grid Snap";
                    else
                        tooltipGridSnap = "Turn ON Grid Snap";
                    
                    // OnScene Enable/Disable Randomizer Icon Button
                    MAST.Settings.Data.placement.snapToGrid = GUILayout.Toggle(
                        MAST.Settings.Data.placement.snapToGrid,
                        new GUIContent(iconGridSnap, tooltipGridSnap),
                        "MAST Toggle", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize));
                    
                    GUILayout.EndVertical();
                    
                    GUILayout.Space(toolBarIconSize / 5);
                    
                    // --------------------------------------------------------------------------------------------
                    // Draw Tool Section
                    // --------------------------------------------------------------------------------------------
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    
                    // ------------------------------------
                    // Add Draw Tool Toggle Group
                    // ------------------------------------
                    EditorGUI.BeginChangeCheck ();
                    
                    // Create drawtools SelectionGrid
                    int newSelectedDrawToolIndex = GUILayout.SelectionGrid(
                        MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex, 
                        guiContentDrawTool, 1, "MAST Toggle",
                        GUILayout.Width(toolBarIconSize), 
                        GUILayout.Height(toolBarIconSize * 5));
                    
                    // If the draw tool was changed
                    if (EditorGUI.EndChangeCheck ()) {
                        
                        // If the draw tool was clicked again, deselect it
                        if (newSelectedDrawToolIndex == MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex)
                        {
                            MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex = -1;
                            
                            MAST.Building.Interface.ChangePlacementMode(BuildMode.None);
                            
                            MAST.GUI.DataManager.state.previousBuildToolIndex = -1;
                        }
                        
                        // If a different draw tool was clicked, change to it
                        else
                        {
                            MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex = newSelectedDrawToolIndex;
                            
                            switch (MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex)
                            {
                                // Draw Single Tool selected
                                case 0:
                                    MAST.Building.Interface.ChangePlacementMode(BuildMode.DrawSingle);
                                    break;
                                
                                // Draw Continuous Tool selected
                                case 1:
                                    MAST.Building.Interface.ChangePlacementMode(BuildMode.DrawContinuous);
                                    break;
                                
                                // Flood Fill Tool selected
                                case 2:
                                    MAST.Building.Interface.ChangePlacementMode(BuildMode.PaintArea);
                                    break;
                                
                                // Randomizer Tool selected
                                case 3:
                                    MAST.Building.Interface.ChangePlacementMode(BuildMode.Randomize);
                                    SceneView.RepaintAll();
                                    break;
                                
                                // Eraser Tool selected
                                case 4:
                                    MAST.Building.Interface.ChangePlacementMode(BuildMode.Erase);
                                    SceneView.RepaintAll();
                                    break;
                            }
                            
                        }
                    }
                    
                    GUILayout.EndVertical();
                    
                    GUILayout.Space(toolBarIconSize / 5);
                    
                    // --------------------------------------------------------------------------------------------
                    // Manipulate Section
                    // --------------------------------------------------------------------------------------------
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    
                    // ------------------------------------
                    // Rotate Button
                    // ------------------------------------
                    if (GUILayout.Button(new GUIContent(iconRotate, "Rotate Prefab/Selection"),
                        "MAST Button", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize)))
                    {
                        MAST.Building.Manipulate.RotateObject();
                    }
                    
                    // OnScene Change Rotate Axis Icon Button
                    switch (MAST.Building.Manipulate.GetCurrentRotateAxis())
                    {
                        case Axis.X:
                            if (GUILayout.Button(new GUIContent(iconAxisX, "Change Rotate Axis"),
                                "MAST Half Button", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize / 2)))
                                MAST.Building.Manipulate.ToggleRotateAxis();
                            break;
                        case Axis.Y:
                            if (GUILayout.Button(new GUIContent(iconAxisY, "Change Rotate Axis"),
                                "MAST Half Button", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize / 2)))
                                MAST.Building.Manipulate.ToggleRotateAxis();
                            break;
                        case Axis.Z:
                            if (GUILayout.Button(new GUIContent(iconAxisZ, "Change Rotate Axis"),
                                "MAST Half Button", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize / 2)))
                                MAST.Building.Manipulate.ToggleRotateAxis();
                            break;
                    }
                    
                    GUILayout.Space(toolBarIconSize / 10);
                    
                    // ------------------------------------
                    // Flip Button
                    // ------------------------------------
                    if (GUILayout.Button(new GUIContent(iconFlip, "Flip Prefab/Selection"), 
                        "MAST Button", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize)))
                    {
                        MAST.Building.Manipulate.FlipObject();
                    }
                    
                    // OnScene Change Flip Axis Icon Button
                    switch (MAST.Building.Manipulate.GetCurrentFlipAxis())
                    {
                        case Axis.X:
                            if (GUILayout.Button(new GUIContent(iconAxisX, "Change Flip Axis"),
                                "MAST Half Button", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize / 2)))
                                MAST.Building.Manipulate.ToggleFlipAxis();
                            break;
                        case Axis.Y:
                            if (GUILayout.Button(new GUIContent(iconAxisY, "Change Flip Axis"),
                                "MAST Half Button", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize / 2)))
                                MAST.Building.Manipulate.ToggleFlipAxis();
                            break;
                        case Axis.Z:
                            if (GUILayout.Button(new GUIContent(iconAxisZ, "Change Flip Axis"),
                                "MAST Half Button", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize / 2)))
                                MAST.Building.Manipulate.ToggleFlipAxis();
                            break;
                    }
                    
                    GUILayout.EndVertical();
                    
                    GUILayout.Space(toolBarIconSize / 5);
                    
                    // --------------------------------------------------------------------------------------------
                    // Misc Section
                    // --------------------------------------------------------------------------------------------
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    
                    // ------------------------------------
                    // Load Palette Button
                    // ------------------------------------
                    if (GUILayout.Button(new GUIContent(iconLoadFromFolder, "Load prefabs from a project folder"),
                        "MAST Button", GUILayout.Width(toolBarIconSize), GUILayout.Height(toolBarIconSize)))
                    {
                        
                        // Show choose folder dialog
                        string chosenPath =
                            EditorUtility.OpenFolderPanel("Choose the Folder that Contains your Prefabs",
                            MAST.LoadingHelper.ConvertProjectPathToAbsolutePath(MAST.GUI.DataManager.state.prefabPath), "");
                        
                        // If a folder was chosen "Cancel was not clicked"
                        if (chosenPath != "")
                        {
                            // Generate thumbnails, load prefabs, and select the first folder found (0)
                            MAST.Building.Palette.Manager.GenerateThumbnailsAndLoadMaterials(chosenPath, 0, MAST.Settings.Data.gui.palette.overwriteThumbnails);
                            
                            // Refresh the AssetDatabase incase any new thumbnails were created
                            AssetDatabase.Refresh();
                            
                            // Save the prefab path and currently selected prefab folder index
                            MAST.GUI.DataManager.state.prefabPath = MAST.LoadingHelper.ConvertAbsolutePathToProjectPath(chosenPath);
                            MAST.GUI.DataManager.state.selectedPrefabPaletteFolderIndex = MAST.Building.Palette.Manager.selectedFolderIndex;
                            
                            //MAST_Interface_Data_Manager.Save_Palette_Items(true);
                            MAST.GUI.DataManager.Save_Changes_To_Disk();
                        }
                    }
                    
                    GUILayout.EndVertical();
                    
                    GUILayout.EndVertical();
                }
            #endregion
            }
        }
    }
}
#endif