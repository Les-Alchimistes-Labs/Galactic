using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)
namespace MAST
{
    namespace Building
    {
        namespace GUI
        {
            public static class Palette
            {
                private static Vector2 paletteScrollPos = new Vector2();  // Current scroll position
                const int scrollBarWidth = 19;  // Subtracted from scroll area width or height when calculating visible area
                
            // ---------------------------------------------------------------------------
            #region Palette GUI
            // ---------------------------------------------------------------------------
                public static void DisplayPaletteGUI(float toolBarIconSize)
                {
                    // Only draw prefab palette if it is ready
                    if (MAST.Building.Palette.Manager.IsReady())
                        DisplayPaletteGUIPopulated(toolBarIconSize);
                    else
                        DisplayPaletteGUIPlaceholder();
                }
                
                private static void DisplayPaletteGUIPlaceholder()
                {
                    GUILayout.BeginVertical("MAST Toolbar BG");
                    GUILayout.Space(4f);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(5f);
                    EditorGUILayout.LabelField(
                        "No prefabs to display!  Select your prefabs folder and click Load Prefabs",
                        EditorStyles.wordWrappedLabel);
                    GUILayout.Space(5f);
                    GUILayout.EndHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.EndVertical();
                }
                
                // ---------------------------------------------------------------------------
                // GUI to display the prefab palette
                // ---------------------------------------------------------------------------
                private static void DisplayPaletteGUIPopulated(float toolBarIconSize)
                {
                    GUILayout.BeginVertical("MAST Toolbar BG");  // Begin toolbar vertical layout
                    
                    // ---------------------------------------------
                    // Display Palette folder selection PopUp
                    // ---------------------------------------------
                    
                    // Show prefab folder popup
                    int newSelectedFolder = EditorGUILayout.Popup(MAST.Building.Palette.Manager.selectedFolderIndex, MAST.Building.Palette.Manager.GetFolderNameArray());
                    
                    // Remove focus from all controls.  Otherwise pressing SPACE will trigger this PopUp, even if clicking an item in the palette
                    UnityEngine.GUI.FocusControl(null);
                    
                    // If the palette folder was changed, change the palette items as well
                    if (newSelectedFolder != MAST.Building.Palette.Manager.selectedFolderIndex)
                    {
                        RemovePrefabSelection();
                        MAST.Building.Palette.Manager.ChangeActivePaletteFolder(newSelectedFolder);
                    }
                    
                    GUILayout.BeginHorizontal();
                    
                    // ---------------------------------------------
                    // Calculate Palette SelectionGrid size
                    // ---------------------------------------------
                    
                    // Vertical scroll view for palette items
                    paletteScrollPos = EditorGUILayout.BeginScrollView(paletteScrollPos);
                    
                    // Get scrollview width and height of scrollview if is resized
                    float scrollViewWidth = EditorGUIUtility.currentViewWidth - scrollBarWidth - toolBarIconSize - 20;
                    
                    int rowCount = Mathf.CeilToInt(MAST.Building.Palette.Manager.GetGUIContentArray().Length / (float)MAST.GUI.DataManager.state.prefabPaletteColumnCount);
                    float scrollViewHeight = rowCount * ((scrollViewWidth) / MAST.GUI.DataManager.state.prefabPaletteColumnCount);
                    
                    // ---------------------------------------------
                    // Get palette background image
                    // ---------------------------------------------
                    string paletteGUISkin = null;
                    
                    switch (MAST.Settings.Data.gui.palette.bgColor)
                    {
                        case PaleteBGColor.Dark:
                            paletteGUISkin = "MAST Palette Item Dark";
                            break;
                        case PaleteBGColor.Gray:
                            paletteGUISkin = "MAST Palette Item Gray";
                            break;
                        case PaleteBGColor.Light:
                            paletteGUISkin = "MAST Palette Item Light";
                            break;
                    }
                    
                    EditorGUI.BeginChangeCheck ();
                    
                    // ---------------------------------------------
                    // Draw Palette SelectionGrid
                    // ---------------------------------------------
                    
                    int newSelectedPaletteItemIndex = GUILayout.SelectionGrid(
                        MAST.Building.Palette.Manager.selectedItemIndex,
                        MAST.Building.Palette.Manager.GetGUIContentArray(),
                        MAST.GUI.DataManager.state.prefabPaletteColumnCount,
                        paletteGUISkin,
                        GUILayout.Width((float)scrollViewWidth),
                        GUILayout.Height(scrollViewHeight)
                        );
                    
                    // If the user clicked the prefab palette SelectionGrid
                    if (EditorGUI.EndChangeCheck ()) {
                        
                        // If palette item was deselected by being clicked again, remove the selection
                        if (newSelectedPaletteItemIndex == MAST.Building.Palette.Manager.selectedItemIndex)
                            RemovePrefabSelection();
                        
                        // If palette item selection has changed
                        else
                        {
                            // Process the prefab selection
                            ChangePrefabSelection(newSelectedPaletteItemIndex);
                        }
                    }
                    
                    EditorGUILayout.EndScrollView();
                    
                    GUILayout.EndHorizontal();
                    
                    // Palette Column Count Slider
                    MAST.GUI.DataManager.state.prefabPaletteColumnCount =
                        (int)GUILayout.HorizontalSlider(MAST.GUI.DataManager.state.prefabPaletteColumnCount, 1, 10);
                    
                    GUILayout.Space(toolBarIconSize / 10);
                    
                    GUILayout.EndVertical();
                }
                
                // ---------------------------------------------------------------------------
                // Change prefab selection and handle events
                // ---------------------------------------------------------------------------
                public static void ChangePrefabSelection(int index = -2)
                {
                    // Change the index if it was supplied
                    if (index != -2)
                        MAST.Building.Palette.Manager.selectedItemIndex = index;
                    
                    // If no draw tool or eraser is selected
                    if (MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex == -1
                        || MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex == 4)
                    {
                        // If grid is off, turn it on
                        if (!Building.GridManager.DoesGridExist())
                        {
                            Building.GridManager.gridExists = true;
                            Building.GridManager.ChangeGridVisibility();
                        }
                        
                        // Select the draw single tool
                        MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex = 0;
                        MAST.Building.Interface.ChangePlacementMode(BuildMode.DrawSingle);
                    }
                    
                    // If erase draw tool isn't selected
                    if (MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex != 4)
                    {
                        // If randomizer is selected, roll a new random seed for the new prefab without replacing
                        if (MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex == 3)
                            Randomizer.GenerateNewRandomSeed(true);
                        
                        // Change the visualizer prefab
                        MAST.Building.Interface.ChangeSelectedPrefab();
                    }
                }
                
                // ---------------------------------------------------------------------------
                // Remove any active prefab selection and handle events
                // ---------------------------------------------------------------------------
                public static void RemovePrefabSelection()
                {
                    // Deselect any prefab
                    MAST.Building.Palette.Manager.selectedItemIndex = -1;
                    
                    // Deselect any draw tool
                    MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex = -1;
                    
                    // Made sure build mode is set to None
                    MAST.Building.Interface.ChangePlacementMode(BuildMode.None);
                    
                    // Remove any existing visualizer
                    MAST.Building.Visualizer.RemoveVisualizer();
                }
                
                

            #endregion
            }
        }
    }
}
#endif