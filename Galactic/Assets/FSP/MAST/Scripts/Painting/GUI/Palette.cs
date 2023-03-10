using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Painting
    {
        namespace GUI
        {
            public static class Palette
            {
                private static Vector2 paletteScrollPos = new Vector2();  // Current scroll position
                const int scrollBarWidth = 19;  // Subtracted from scroll area width or height when calculating visible area
                
                public static void DisplayPaletteGUI(float toolBarIconSize)
                {
                    // Only draw material palette if it is ready
                    if (MAST.Painting.Palette.Manager.IsReady())
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
                        "No materials to display!  Select your materials folder and click Load Materials",
                        EditorStyles.wordWrappedLabel);
                    GUILayout.Space(5f);
                    GUILayout.EndHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.EndVertical();
                }
                
                // ---------------------------------------------------------------------------
                // GUI to display the material palette
                // ---------------------------------------------------------------------------
                private static void DisplayPaletteGUIPopulated(float toolBarIconSize)
                {
                    GUILayout.BeginVertical("MAST Toolbar BG");  // Begin toolbar vertical layout
                    
                    // ---------------------------------------------
                    // Display Palette folder selection PopUp
                    // ---------------------------------------------
                    
                    // Show material folder popup
                    int newSelectedFolder = EditorGUILayout.Popup(MAST.Painting.Palette.Manager.selectedFolderIndex, MAST.Painting.Palette.Manager.GetFolderNameArray());
                    
                    // Remove focus from all controls.  Otherwise pressing SPACE will trigger this PopUp, even if clicking an item in the palette
                    UnityEngine.GUI.FocusControl(null);
                    
                    // If the palette folder was changed, change the palette items as well
                    if (newSelectedFolder != MAST.Painting.Palette.Manager.selectedFolderIndex)
                    {
                        RemoveMaterialSelection();
                        MAST.Painting.Palette.Manager.ChangeActivePaletteFolder(newSelectedFolder);
                    }
                    
                    GUILayout.BeginHorizontal();
                    
                    // ---------------------------------------------
                    // Calculate Palette SelectionGrid size
                    // ---------------------------------------------
                    
                    // Vertical scroll view for palette items
                    paletteScrollPos = EditorGUILayout.BeginScrollView(paletteScrollPos);
                    
                    // Get scrollview width and height of scrollview if is resized
                    float scrollViewWidth = EditorGUIUtility.currentViewWidth - scrollBarWidth - toolBarIconSize - 20;
                    
                    int rowCount = Mathf.CeilToInt(MAST.Painting.Palette.Manager.GetGUIContentArray().Length / (float)MAST.GUI.DataManager.state.materialPaletteColumnCount);
                    float scrollViewHeight = rowCount * ((scrollViewWidth) / MAST.GUI.DataManager.state.materialPaletteColumnCount);
                    
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
                        MAST.Painting.Palette.Manager.selectedItemIndex,
                        MAST.Painting.Palette.Manager.GetGUIContentArray(),
                        MAST.GUI.DataManager.state.materialPaletteColumnCount,
                        paletteGUISkin,
                        GUILayout.Width((float)scrollViewWidth),
                        GUILayout.Height(scrollViewHeight)
                        );
                    
                    // If changes to UI value ocurred, update the grid
                    if (EditorGUI.EndChangeCheck ()) {
                        
                        // If palette item was deselected by being clicked again
                        if (newSelectedPaletteItemIndex == MAST.Painting.Palette.Manager.selectedItemIndex)
                        {
                            RemoveMaterialSelection();
                        }
                        
                        // If palette item selection has changed
                        else
                        {
                            // Select the material in the material palette by index
                            MAST.Painting.Palette.Manager.selectedItemIndex = newSelectedPaletteItemIndex;
                            
                            // Make sure the paint material tool is selected
                            MAST.Settings.Data.gui.toolbar.selectedPaintToolIndex = 0;
                        }
                    }
                    
                    EditorGUILayout.EndScrollView();
                    
                    GUILayout.EndHorizontal();
                    
                    // Palette Column Count Slider
                    MAST.GUI.DataManager.state.materialPaletteColumnCount =
                        (int)GUILayout.HorizontalSlider(MAST.GUI.DataManager.state.materialPaletteColumnCount, 1, 10);
                    
                    GUILayout.Space(toolBarIconSize / 10);
                    
                    GUILayout.EndVertical();
                }
                
                // ---------------------------------------------------------------------------
                // Remove any active material selection and handle events
                // ---------------------------------------------------------------------------
                public static void RemoveMaterialSelection()
                {
                    // Deselect any prefab
                    if (MAST.Painting.Palette.Manager.selectedItemIndex != -1)
                        MAST.Painting.Palette.Manager.selectedItemIndex = -1;
                    
                    // If the paint tool is selected, then clear the paint preview and deselect the paint tool
                    if (MAST.Settings.Data.gui.toolbar.selectedPaintToolIndex == 0)
                    {
                        MAST.Painting.Painter.ClearCurrentMaterialPaintPreview();
                        MAST.Settings.Data.gui.toolbar.selectedPaintToolIndex = -1;
                    }
                }
            }
        }
    }
}
#endif