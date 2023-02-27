using System;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace GUI
    {
        [Serializable]
        public class Main : EditorWindow
        {
            // ---------------------------------------------------------------------------
            // Add menu named "Open MAST Palette" to the Window menu
            // ---------------------------------------------------------------------------
            [MenuItem("Tools/MAST/Open MAST Window", false, 16)]
            private static void ShowWindow()
            {
                // Get existing open window or if none, make a new one:
                EditorWindow.GetWindow(typeof(GUI.Main), false, "MAST").Show();
            }
            
        // ---------------------------------------------------------------------------
        #region Variable Declaration
        // ---------------------------------------------------------------------------
            
            // ------------------------------
            // Persisent Class Variables
            // ------------------------------
            
            // Initialize Hotkeys Class if needed and return HotKeysClass
            [SerializeField] private MAST.GUI.Hotkeys HotKeysClass;
            private MAST.GUI.Hotkeys HotKeys
            {
                get
                {
                    if(HotKeysClass == null)
                        HotKeysClass = new MAST.GUI.Hotkeys();
                    return HotKeysClass;
                }
            }
            
            // ------------------------------
            // Editor Window Variables
            // ------------------------------
            [SerializeField] private bool inPlayMode = false;
            [SerializeField] private bool isCleanedUp = false;
            
            // ------------------------------
            // GUIStyle Variables
            // ------------------------------
            [SerializeField] private GUISkin guiSkin;
            
            // ------------------------------
            // Draw Tool Variables
            // ------------------------------
            [SerializeField] private float toolBarIconSize;
            
        #endregion
            
            // ---------------------------------------------------------------------------
            // Perform Initializations
            // ---------------------------------------------------------------------------
            void Awake() // This runs on the first time open
            {
                //Debug.Log("Interface - Awake");
            }
            
            void OnFocus()
            {
                //Debug.Log("Interface - On Focus");
            }
            
            // ---------------------------------------------------------------------------
            // MAST Window is Enabled
            // ---------------------------------------------------------------------------
            private void OnEnable()
            {
                
                //Debug.Log("Interface - On Enable");
                
                // Initialize Preference Manager
                MAST.Settings.Data.Initialize();
                
                // Initialize the MAST State Manager
                MAST.GUI.DataManager.Initialize();
                
                // Set up deletegates so that OnScene is called automatically
                #if UNITY_2019_1_OR_NEWER
                    SceneView.duringSceneGui -= this.OnScene;
                    SceneView.duringSceneGui += this.OnScene;
                #else
                    SceneView.onSceneGUIDelegate -= this.OnScene;
                    SceneView.onSceneGUIDelegate += this.OnScene;
                #endif
                
                // Set up deletegates for exiting editor mode and returning to editor mode from play mode
                MAST.GUI.PlayModeStateListener.onExitEditMode -= this.ExitEditMode;
                MAST.GUI.PlayModeStateListener.onExitEditMode += this.ExitEditMode;
                MAST.GUI.PlayModeStateListener.onEnterEditMode -= this.EnterEditMode;
                MAST.GUI.PlayModeStateListener.onEnterEditMode += this.EnterEditMode;
                
                // Set scene to be updated by mousemovement
                wantsMouseMove = true;
                
                // If Enabled in Editor Mode
                if (!inPlayMode)
                {
                    // Load custom gui styles
                    if (guiSkin == null)
                        guiSkin = MAST.LoadingHelper.GetGUISkin();
                    
                    // Load interface data back from saved state
                    MAST.GUI.DataManager.Load_Interface_State();
                    
                    // Create a new grid if needed
                    if (MAST.GUI.DataManager.state.gridExists)
                    {
                        MAST.Building.GridManager.gridExists = true;
                        MAST.Building.GridManager.ChangeGridVisibility();
                    }
                    
                    // Change placement mode back to what was saved
                    MAST.Building.Interface.ChangePlacementMode(
                        (BuildMode)MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex);
                    
                    MAST.GUI.DataManager.Restore_Palette_Items();
                }
                
                // If Enabled in Run Mode
                else
                {
                    // Nothing so far, because everything is being triggered in ExitEditMode event method
                }
            }
            
            // ---------------------------------------------------------------------------
            // Save and Restore MAST Interface variables to keep state on play
            // ---------------------------------------------------------------------------
            private void ExitEditMode()
            {
                //Debug.Log("Interface - Exit Edit Mode");
                
                // Don't allow this method to run twice
                if (inPlayMode)
                    return;
                
                // If the grid exists
                if (MAST.Building.GridManager.gridExists)
                {
                    // Destroy the grid so it doesn't show while playing
                    MAST.Building.GridManager.DestroyGrid();
                    
                    // Make sure the grid is restored after returning to editor
                    MAST.Building.GridManager.gridExists = true;
                }
                
                inPlayMode = true;
                isCleanedUp = false;
            }
            
            private void EnterEditMode()
            {
                //Debug.Log("Interface - Enter Edit Mode");
                
                // Don't allow this method to run twice
                if (!inPlayMode)
                    return;
                
                // Load interface data back from saved state
                MAST.GUI.DataManager.Load_Interface_State();
                MAST.Building.GridManager.ChangeGridVisibility();
                
                // Restore the interface saved state
                MAST.GUI.DataManager.Restore_Palette_Items();
                
                // Change placement mode back to what was saved
                MAST.Building.Interface.ChangePlacementMode(
                    (BuildMode)MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex);
                
                // Repaint all views
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                
                inPlayMode = false;
            }
            
            // ---------------------------------------------------------------------------
            // Perform Cleanup when MAST Window is Disabled
            // ---------------------------------------------------------------------------
            private void OnDisable()
            {
                //Debug.Log("Interface - On Disable");
                
                // Save MAST Settings to Scriptable Objects
                MAST.Settings.Data.Save_Settings();
                
                // If OnDisable triggered by going fullscreen, closing MAST, or changing scenes
                if (!inPlayMode)
                    MAST.GUI.DataManager.Save_Interface_State();
                
                // If OnDisable is triggered by the user hitting play button
                else
                {
                    // If cleanup hasn't already ocurred
                    if (!isCleanedUp)
                    {
                        // Load interface and palette data state
                        MAST.GUI.DataManager.Save_Interface_State();
                        MAST.GUI.DataManager.Save_Palette_Items();
                        
                        CleanUpInterface();
                        
                        isCleanedUp = true;
                    }
                }
                
                // Remove SceneView delegate
                #if UNITY_2019_1_OR_NEWER
                    SceneView.duringSceneGui -= this.OnScene;
                #else
                    SceneView.onSceneGUIDelegate -= this.OnScene;
                #endif
            }
            
            // ---------------------------------------------------------------------------
            // Perform Cleanup when MAST Window is Closed
            // ---------------------------------------------------------------------------
            private void OnDestroy()
            {
                //Debug.Log("Interface - On Destroy");
            }
            
            // ---------------------------------------------------------------------------
            // Clean-up
            // ---------------------------------------------------------------------------
            private void CleanUpInterface()
            {
                //Debug.Log("Cleaning Up Interface");
                
                // Delete placement grid
                MAST.Building.GridManager.DestroyGrid();
                
                // Deselect palette item and delete visualizer
                MAST.Building.GUI.Palette.RemovePrefabSelection();
                
                // Deselect draw tool and change placement mode to none
                MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex = -1;
                MAST.Building.Interface.ChangePlacementMode(BuildMode.None);
                
                MAST.Settings.Data.gui.toolbar.selectedPaintToolIndex = -1;
                MAST.Painting.GUI.Palette.RemoveMaterialSelection();
                
                // Cancel any drawing or painting
                MAST.GUI.DataManager.state.previousBuildToolIndex = -1;
                MAST.GUI.DataManager.state.previousPaintToolIndex = -1;
                MAST.Building.PaintArea.DeletePaintArea();
            }
            
            // ---------------------------------------------------------------------------
            // Runs every frame
            // ---------------------------------------------------------------------------
            private void Update()
            {
                
            }
            
        // ---------------------------------------------------------------------------
        #region SceneView
        // ---------------------------------------------------------------------------
            private void OnScene(SceneView sceneView)
            {
                // Exit now if game is playing
                //if (inPlayMode)
                //     return;
                
                // Handle SceneView GUI
                SceneviewGUI(sceneView);
                
                // Handle view focus
                ProcessMouseEnterLeaveSceneview();
                
                // Process HotKeys and repaint all views if any changes were made
                if (HotKeys.ProcessHotkeys())
                {
                    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                }
                
                switch (MAST.GUI.DataManager.state.selectedInterfaceTab)
                {
                    // Build tab selected
                    case 0:
                        // If draw tool was deselected by a hotkey
                        if (MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex != 1)
                            // Stop any drawing
                            if (MAST.GUI.DataManager.state.previousBuildToolIndex == 1)
                                MAST.GUI.DataManager.state.previousBuildToolIndex = -1;
                        
                        // If paint area tool was deselected by a hotkey
                        if (MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex != 2)
                            // Stop any painting
                            if (MAST.GUI.DataManager.state.previousBuildToolIndex == 2)
                            {
                                MAST.Building.PaintArea.DeletePaintArea();
                                MAST.GUI.DataManager.state.previousBuildToolIndex = -1;
                            }
                        
                        // If erase tool was deselected by a hotkey
                        if (MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex != 4)
                            // Stop any erasing
                            if (MAST.GUI.DataManager.state.previousBuildToolIndex == 4)
                                MAST.GUI.DataManager.state.previousBuildToolIndex = -1;
                        
                        // If a palette item is selected or erase tool is selected, handle object placement
                        if (MAST.Building.Palette.Manager.selectedItemIndex > -1 ||
                            MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex == 4)
                            ObjectPlacement();
                        break;
                    
                    // Paint tab selected
                    case 1:
                        // If paint material tool selected
                        if (MAST.Settings.Data.gui.toolbar.selectedPaintToolIndex != -1)
                        {
                            MaterialPaint();
                        }
                        break;
                }
            }
            
            // Handle SceneView GUI
            private void SceneviewGUI(SceneView sceneView)
            {
                bool scrollWheelUsed = false;
                
                // If SHIFT key is held down
                if (Event.current.shift)
                {
                    // If mouse scrollwheel was used
                    if (Event.current.type == EventType.ScrollWheel)
                    {
                        // If scrolling wheel down
                        if (Event.current.delta.y > 0)
                        {
                            // Select next prefab and cycle back to top of prefabs if needed
                            MAST.Building.Palette.Manager.selectedItemIndex++;
                            if (MAST.Building.Palette.Manager.selectedItemIndex >= MAST.Building.Palette.Manager.GetPrefabArray().Length)
                                MAST.Building.Palette.Manager.selectedItemIndex = 0;
                            
                            scrollWheelUsed = true;
                        }
                        
                        // If scrolling wheel up
                        else if (Event.current.delta.y < 0)
                        {
                            // Select previous prefab and cycle back to bottom of prefabs if needed
                            MAST.Building.Palette.Manager.selectedItemIndex--;
                            if (MAST.Building.Palette.Manager.selectedItemIndex < 0)
                                MAST.Building.Palette.Manager.selectedItemIndex = MAST.Building.Palette.Manager.GetPrefabArray().Length - 1;
                            
                            scrollWheelUsed = true;
                        }
                    }
                }
                
                // If successfully scrolled wheel
                if (scrollWheelUsed)
                {
                    // If no draw tool is selected, then select the draw single tool
                    if (MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex == -1)
                    {
                        MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex = 0;
                        MAST.Building.Interface.ChangePlacementMode(BuildMode.DrawSingle);
                    }
                    
                    // If erase draw tool isn't selected, change the visualizer prefab
                    if (MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex != 4)
                        MAST.Building.Interface.ChangeSelectedPrefab();
                    
                    // Repaint all views
                    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                    
                    // Keep mouseclick from selecting other objects
                    GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                    Event.current.Use();
                }
                
                
            }
            
            // Handle events when mouse point enter or leaves the SceneView
            void ProcessMouseEnterLeaveSceneview()
            {
                // If mouse enters SceneView window, show visualizer
                if (Event.current.type == EventType.MouseEnterWindow)
                    MAST.Building.Visualizer.SetVisualizerVisibility(true);
                
                // If mouse leaves SceneView window
                else if (Event.current.type == EventType.MouseLeaveWindow)
                {
                    // Hide visualizer
                    MAST.Building.Visualizer.SetVisualizerVisibility(false);
                    
                    // Stop any drawing single
                    if (MAST.GUI.DataManager.state.previousBuildToolIndex == 1)
                        MAST.GUI.DataManager.state.previousBuildToolIndex = -1;
                    
                    // Stop any drawing continuous
                    if (MAST.GUI.DataManager.state.previousBuildToolIndex == 2)
                    {
                        MAST.Building.PaintArea.DeletePaintArea();
                        MAST.GUI.DataManager.state.previousBuildToolIndex = -1;
                    }
                    
                    // Stop any erasing
                    if (MAST.GUI.DataManager.state.previousBuildToolIndex == 4)
                        MAST.GUI.DataManager.state.previousBuildToolIndex = -1;
                }
            }
            
            void MaterialPaint()
            {
                // Get mouse events for object placement when in the Scene View
                Event currentEvent = Event.current;
                
                switch (MAST.Settings.Data.gui.toolbar.selectedPaintToolIndex)
                {
                    // Paint material tool
                    case 0:
                        // If no material is selected, exit without attempting to paint
                        if (MAST.Painting.Palette.Manager.selectedItemIndex == -1)
                            return;
                        
                        // Preview the material paint
                        MAST.Painting.Painter.PreviewPaint(MAST.Painting.Palette.Manager.GetSelectedMaterial());
                        
                        // If not already painting and the left mouse button pressed
                        if (MAST.GUI.DataManager.state.previousPaintToolIndex != 1)
                            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
                            {
                                // Start painting
                                MAST.GUI.DataManager.state.previousPaintToolIndex = 1;
                                
                                MAST.Painting.Painter.StartPainting();
                                
                                // Keep mouseclick from selecting other objects
                                GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                                Event.current.Use();
                            }
                        
                        // If painting and left mouse button not released
                        if (MAST.GUI.DataManager.state.previousPaintToolIndex == 1)
                        {
                            
                            // If left mouse button released, stop painting
                            if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0)
                            {
                                MAST.Painting.Painter.StopPainting();
                                MAST.GUI.DataManager.state.previousPaintToolIndex = -1;
                            }
                        }
                        break;
                    
                    // Restore material tool
                    case 1:
                        // If any material in the palette was selected, deselect it
                        if (MAST.Painting.Palette.Manager.selectedItemIndex != -1)
                            MAST.Painting.Palette.Manager.selectedItemIndex = -1;
                        
                        // Preview the material restore "providing a null material triggers restore mode"
                        MAST.Painting.Painter.PreviewPaint((Material)null);
                        
                        // If not already painting and the left mouse button pressed
                        if (MAST.GUI.DataManager.state.previousPaintToolIndex != 2)
                            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
                            {
                                // Start restoring
                                MAST.GUI.DataManager.state.previousPaintToolIndex = 2;
                                
                                MAST.Painting.Painter.StartPainting();
                                
                                // Keep mouseclick from selecting other objects
                                GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                                Event.current.Use();
                            }
                        
                        // If painting and left mouse button not released
                        if (MAST.GUI.DataManager.state.previousPaintToolIndex == 2)
                        {
                            // If left mouse button released, stop painting
                            if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0)
                            {
                                MAST.Painting.Painter.StopPainting();
                                MAST.GUI.DataManager.state.previousPaintToolIndex = -1;
                            }
                        }
                        break;
                }
                
                
            }
            
            // Handle object placement
            private void ObjectPlacement()
            {
                // Get mouse events for object placement when in the Scene View
                Event currentEvent = Event.current;
                
                // Change position of visualizer
                MAST.Building.Visualizer.UpdateVisualizerPosition();
                
                switch (MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex)
                {
                    // Draw single tool
                    case 0:
                        
                    // Draw continuous tool
                    case 1:
                        // If not already drawing and the left mouse button pressed
                        if (MAST.GUI.DataManager.state.previousBuildToolIndex != 1)
                            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
                            {
                                // Start drawing
                                MAST.GUI.DataManager.state.previousBuildToolIndex = 1;
                                
                                // Keep mouseclick from selecting other objects
                                GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                                Event.current.Use();
                            }
                        
                        // If drawing and left mouse button not released
                        if (MAST.GUI.DataManager.state.previousBuildToolIndex == 1)
                        {
                            // Place selected prefab on grid
                            MAST.Building.Placement.PlacePrefabInScene();
                            
                            // If left mouse button released
                            if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0)
                                MAST.GUI.DataManager.state.previousBuildToolIndex = -1;
                        }
                        break;
                        
                    // Paint area tool
                    case 2:
                        // If not already painting and the left mouse button pressed
                        if (MAST.GUI.DataManager.state.previousBuildToolIndex != 2)
                            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
                            {
                                // Start drawing
                                MAST.GUI.DataManager.state.previousBuildToolIndex = 2;
                                
                                // Start paint area at current mouse location
                                MAST.Building.PaintArea.StartPaintArea();
                                
                                // Keep mouseclick from selecting other objects
                                GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                                Event.current.Use();
                            }
                        
                        // If drawing and left mouse button not released
                        if (MAST.GUI.DataManager.state.previousBuildToolIndex == 2)
                        {
                            // Update the paint area as the mouse moves
                            MAST.Building.PaintArea.UpdatePaintArea();
                            
                            // If left mouse button released
                            if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0)
                            {
                                MAST.Building.PaintArea.CompletePaintArea();
                                MAST.GUI.DataManager.state.previousBuildToolIndex = -1;
                            }
                        }
                        break;
                    
                    // Randomizer tool
                    case 3:
                        // If left mouse button was clicked
                        if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
                        {
                            // Keep mouseclick from selecting other objects
                            GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                            Event.current.Use();
                            
                            // Place selected prefab on grid
                            MAST.Building.Placement.PlacePrefabInScene();
                        }
                        break;
                    
                    // Erase tool
                    case 4:
                        // If not already erasing and the left mouse button pressed
                        if (MAST.GUI.DataManager.state.previousBuildToolIndex != 4)
                            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
                            {
                                // Start drawing
                                MAST.GUI.DataManager.state.previousBuildToolIndex = 4;
                                
                                // Keep mouseclick from selecting other objects
                                GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                                Event.current.Use();
                            }
                        
                        // If erasing and left mouse button not released
                        if (MAST.GUI.DataManager.state.previousBuildToolIndex == 4)
                        {
                            // Place selected prefab on grid
                            MAST.Building.Interface.ErasePrefab();
                            
                            // If left mouse button released
                            if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0)
                                MAST.GUI.DataManager.state.previousBuildToolIndex = -1;
                        }
                        break;
                }
                
            }
        #endregion
            
        // ---------------------------------------------------------------------------
        #region Custom Editor Window Interface
        // ---------------------------------------------------------------------------
            void OnGUI()
            {
                // Exit now if game is playing
                //if (inPlayMode)
                //    return;
                
                // Load custom skin
                UnityEngine.GUI.skin = guiSkin;
                
                string[] tabCaptions = {"Build", "Paint", "Settings", "Tools"};
                
                // MAST interface tabs
                MAST.GUI.DataManager.state.selectedInterfaceTab
                    = GUILayout.Toolbar (MAST.GUI.DataManager.state.selectedInterfaceTab, tabCaptions);
                
                // Depending on tab selected, show the appropriate interface
                switch (MAST.GUI.DataManager.state.selectedInterfaceTab) {
                    case 0:
                        DisplayStagingGUI();
                        break;
                    case 1:
                        DisplayPaintingGUI();
                        break;
                    case 2:
                        MAST.Settings.GUI.DisplaySettingsGUI(guiSkin);
                        break;
                    case 3:
                        MAST.Tools.GUI.Main.DisplayToolsGUI();
                        break;
                }
                
                // ----------------------------------------------
                // Redraw MAST window if mouse is moved
                // ----------------------------------------------
                if (Event.current.type == EventType.MouseMove)
                    Repaint();
                
                // ----------------------------------------------
                // Process Hotkeys
                // ----------------------------------------------
                if (HotKeys.ProcessHotkeys())
                    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
            
            private void DisplayStagingGUI()
            {
                GUILayout.BeginHorizontal("MAST Toolbar BG");  // Begin entire window horizontal layout
                
                // Calculate toolbar icon size
                toolBarIconSize = (position.height / 15.3f) * MAST.Settings.Data.gui.toolbar.scale;
                
                // If toolbar is on the left
                if (MAST.Settings.Data.gui.toolbar.position == ToolbarPos.Left)
                {
                    MAST.Building.GUI.Toolbar.DisplayToolbarGUI(toolBarIconSize);
                    MAST.Building.GUI.Palette.DisplayPaletteGUI(toolBarIconSize);
                }
                
                // If toolbar is on the right
                else
                {
                    MAST.Building.GUI.Palette.DisplayPaletteGUI(toolBarIconSize);
                    MAST.Building.GUI.Toolbar.DisplayToolbarGUI(toolBarIconSize);
                }
                
                GUILayout.EndHorizontal(); // End of entire window horizontal layout
            }
            
            private void DisplayPaintingGUI()
            {
                GUILayout.BeginHorizontal("MAST Toolbar BG");  // Begin entire window horizontal layout
                
                // Calculate toolbar icon size
                toolBarIconSize = (position.height / 15.3f) * MAST.Settings.Data.gui.toolbar.scale;
                
                // If toolbar is on the left
                if (MAST.Settings.Data.gui.toolbar.position == ToolbarPos.Left)
                {
                    MAST.Painting.GUI.Toolbar.DisplayToolbarGUI(toolBarIconSize);
                    MAST.Painting.GUI.Palette.DisplayPaletteGUI(toolBarIconSize);
                }
                
                // If toolbar is on the right
                else
                {
                    MAST.Painting.GUI.Palette.DisplayPaletteGUI(toolBarIconSize);
                    MAST.Painting.GUI.Toolbar.DisplayToolbarGUI(toolBarIconSize);
                }
                
                GUILayout.EndHorizontal(); // End of entire window horizontal layout
            }
        #endregion
        }
    }
}
#endif