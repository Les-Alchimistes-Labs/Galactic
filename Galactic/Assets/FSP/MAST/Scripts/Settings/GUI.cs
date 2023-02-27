using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)
namespace MAST
{
    namespace Settings
    {
        public static class GUI
        {
            // Have preferences changed?
            [SerializeField] private static bool prefChanged = false;
            
            // GameObject reference for target parent of all placed prefeabs
            //[SerializeField] private static GameObject targetParent = null;
            
            private static int tab = 0;
            
            [SerializeField] private static Vector2 placementScrollPos = new Vector2();
            [SerializeField] private static Vector2 gridScrollPos = new Vector2();
            [SerializeField] private static Vector2 hotkeyScrollPos = new Vector2();
            
            private static bool placementOffsetFoldout = true;
            private static bool placementRotationFoldout = true;
            private static bool placementRaycastFoldout = true;
            private static bool placementRandomizerFoldout = true;
            
            private static bool guiToolbarFoldout = true;
            private static bool guiPaletteFoldout = true;
            private static bool guiGridFoldout = true;
            
            // ---------------------------------------------------------------------------
            // Initialize
            // ---------------------------------------------------------------------------
            public static void Initialize()
            {
                // Get reference to target parent, or create a new one
                MAST.Building.Placement.ReferenceTargetParent();
            }
            
        // ---------------------------------------------------------------------------
        #region Preferences Interface
        // ---------------------------------------------------------------------------
            public static void DisplaySettingsGUI(GUISkin guiSkin)
            {
                // If target parent is deleted, create a new one
                if (MAST.Building.Placement.targetParent == null)
                    Initialize();
                
                GUILayout.BeginVertical("MAST Toolbar BG");  // Begin entire window vertical layout
                
                GUILayout.Space(5f);
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                // Start polling for changes
                EditorGUI.BeginChangeCheck ();
                
                string[] tabCaptions = {"Placement", "GUI", "Hotkeys"};
                
                tab = GUILayout.Toolbar (tab, tabCaptions);
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                switch (tab) {
                    case 0:
                        PlacementGUI();
                        break;
                    case 1:
                        GUIGUI(guiSkin);
                        break;
                    case 2:
                        HotkeyGUI();
                        break;
                }
                
                GUILayout.EndVertical();
                
                // If changes to UI value ocurred, update
                if (EditorGUI.EndChangeCheck ()) {
                    prefChanged = true;
                    PreferencesChanged();
                }
            }
        #endregion
        // ---------------------------------------------------------------------------

        // ---------------------------------------------------------------------------
        #region Placement Tab GUI
        // ---------------------------------------------------------------------------
            private static void PlacementGUI()
            {
                // Verical scroll view for palette items
                placementScrollPos = EditorGUILayout.BeginScrollView(placementScrollPos);
                
                // ----------------------------------
                // Placement Destination
                // ----------------------------------
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                MAST.Building.Placement.targetParent = (GameObject)EditorGUILayout.ObjectField(
                    new GUIContent("Placement Destination",
                    "Drag a GameObject from the Hierarchy into this field.  It will be used as the parent of new placed models"),
                    MAST.Building.Placement.targetParent, typeof(GameObject), true);
                
                // Wrap accessing the tag in a try/catch, since if the tag was just added, it may not be available yet
                try
                {
                    // If target parent was changed
                    if (MAST.Building.Placement.targetParent.tag != MAST.Const.Placement.defaultTargetParentTag)
                    {
                        // Remove defaultMASTParentTag from every GameObject with the tag
                        GameObject[] taggedGameObjects = GameObject.FindGameObjectsWithTag(MAST.Const.Placement.defaultTargetParentTag);
                        if (taggedGameObjects != null)
                        {
                            foreach (GameObject taggedGameObject in taggedGameObjects)
                                taggedGameObject.tag = "Untagged";
                        }
                        
                        // Add target parent tag to this GameObject
                        MAST.Building.Placement.targetParent.tag = MAST.Const.Placement.defaultTargetParentTag;
                    }
                }
                catch
                {
                    // Create Tag for target parent if it doesn't already exist
                    MAST.TagsAndLayers.AddTag(MAST.Const.Placement.defaultTargetParentTag);
                }
                
                //MAST.Settings.Data.placement.targetParentInstanceID = targetParent.GetInstanceID();
                //MAST.Settings.Data.placement.targetParentName = targetParent.name;
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                // ----------------------------------
                // Offset
                // ----------------------------------
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                placementOffsetFoldout = EditorGUILayout.Foldout(placementOffsetFoldout, "Offset Settings");
                if (placementOffsetFoldout) 
                {
                    EditorGUILayout.Space();
                    
                    MAST.Settings.Data.placement.overridePrefabOffset =
                        GUILayout.Toggle(MAST.Settings.Data.placement.overridePrefabOffset, "Override Prefab Component");
                    
                    EditorGUILayout.Space();
                    
                    MAST.Settings.Data.placement.offset.pos =
                        EditorGUILayout.Vector3Field("Position Offset", MAST.Settings.Data.placement.offset.pos);
                }
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                // ----------------------------------
                // Rotation
                // ----------------------------------
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                placementRotationFoldout = EditorGUILayout.Foldout(placementRotationFoldout, "Rotation Settings");
                if (placementRotationFoldout)
                {
                    EditorGUILayout.Space();
                    
                    MAST.Settings.Data.placement.overridePrefabRotation =
                        GUILayout.Toggle(MAST.Settings.Data.placement.overridePrefabRotation, "Override Prefab Component");
                    
                    EditorGUILayout.Space();
                    
                    MAST.Settings.Data.placement.rotation.step =
                        EditorGUILayout.Vector3Field("Rotation Step", MAST.Settings.Data.placement.rotation.step);
                }
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                // ----------------------------------
                // Placement Raycast
                // ----------------------------------
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                placementRaycastFoldout = EditorGUILayout.Foldout(placementRandomizerFoldout, "Raycast Settings");
                if (placementRaycastFoldout) 
                {
                    EditorGUILayout.Space();
                    
                    MAST.Settings.Data.placement.overridePrefabRaycast =
                        GUILayout.Toggle(MAST.Settings.Data.placement.overridePrefabRaycast, "Override Prefab Component");
                    
                    MAST.Settings.Data.placement.placementRaycast.useRaycast =
                        GUILayout.Toggle(MAST.Settings.Data.placement.placementRaycast.useRaycast, "Use Placement Raycast");
                    
                    EditorGUILayout.Space();
                    
                    MAST.Settings.Data.placement.placementRaycast.direction =
                        (DirectionVector)EditorGUILayout.EnumPopup(
                        "Direction Vector", MAST.Settings.Data.placement.placementRaycast.direction);
                    
                    MAST.Settings.Data.placement.placementRaycast.startOffset =
                        EditorGUILayout.Vector3Field("Raycast Start Offset", MAST.Settings.Data.placement.placementRaycast.startOffset);
                }
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                // ----------------------------------
                // Randomizer
                // ----------------------------------
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                placementRandomizerFoldout = EditorGUILayout.Foldout(placementRandomizerFoldout, "Randomizer Settings");
                if (placementRandomizerFoldout) 
                {
                    EditorGUILayout.Space();
                    
                    MAST.Settings.Data.placement.overridePrefabRandomizer =
                        GUILayout.Toggle(MAST.Settings.Data.placement.overridePrefabRandomizer, "Override Prefab Component");
                    
                    MAST.Settings.Data.placement.randomizer.useRandomizer =
                        GUILayout.Toggle(MAST.Settings.Data.placement.randomizer.useRandomizer, "Use Randomizer");
                    
                    EditorGUILayout.Space();
                    
                    EditorGUILayout.LabelField("Rotation", EditorStyles.boldLabel);
                    
                    MAST.Settings.Data.placement.randomizer.rotateMin =
                        EditorGUILayout.Vector3Field("Minimum", MAST.Settings.Data.placement.randomizer.rotateMin);
                    MAST.Settings.Data.placement.randomizer.rotateMax =
                        EditorGUILayout.Vector3Field("Maximum", MAST.Settings.Data.placement.randomizer.rotateMax);            
                    MAST.Settings.Data.placement.randomizer.rotateStep =
                        EditorGUILayout.Vector3Field("Step (Degree increments)", MAST.Settings.Data.placement.randomizer.rotateStep);
                    
                    EditorGUILayout.Space();
                    
                    EditorGUILayout.LabelField("Scale (1 = no scaling)", EditorStyles.boldLabel);
                    
                    MAST.Settings.Data.placement.randomizer.scaleLock =
                        (ScaleAxisLock)EditorGUILayout.EnumPopup(
                        "Lock", MAST.Settings.Data.placement.randomizer.scaleLock);
                    
                    MAST.Settings.Data.placement.randomizer.scaleMin =
                        EditorGUILayout.Vector3Field("Minimum", MAST.Settings.Data.placement.randomizer.scaleMin);
                    MAST.Settings.Data.placement.randomizer.scaleMax =
                        EditorGUILayout.Vector3Field("Maximum", MAST.Settings.Data.placement.randomizer.scaleMax);
                    
                    EditorGUILayout.Space();
                    
                    EditorGUILayout.LabelField("Position (0 = no offset)", EditorStyles.boldLabel);
                    
                    MAST.Settings.Data.placement.randomizer.posMin =
                        EditorGUILayout.Vector3Field("Minimum", MAST.Settings.Data.placement.randomizer.posMin);
                    MAST.Settings.Data.placement.randomizer.posMax =
                        EditorGUILayout.Vector3Field("Maximum", MAST.Settings.Data.placement.randomizer.posMax);
                    
                    EditorGUILayout.Space();
                    
                    EditorGUILayout.LabelField("Flip", EditorStyles.boldLabel);
                    
                    MAST.Settings.Data.placement.randomizer.flipX = GUILayout.Toggle(MAST.Settings.Data.placement.randomizer.flipX, "X");
                    MAST.Settings.Data.placement.randomizer.flipY = GUILayout.Toggle(MAST.Settings.Data.placement.randomizer.flipY, "Y");
                    MAST.Settings.Data.placement.randomizer.flipZ = GUILayout.Toggle(MAST.Settings.Data.placement.randomizer.flipZ, "Z");
                }
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                // Get placement settings object path from selected item in project view
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                if (GUILayout.Button(
                    new GUIContent("Load Placement Settings from selected file in Project",
                    "Use the selected (Placement Settings) file in project")))
                {
                    string path = MAST.LoadingHelper.GetPathOfSelectedObjectTypeOf(typeof(MAST.Settings.ScriptObj.Placement));
                    
                    if (path != "")
                    {
                        MAST.Settings.Data.core.placementSettingsPath = path;
                        MAST.Settings.Data.Load_Placement_Settings();
                    }
                }
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                EditorGUILayout.EndScrollView();
            }
        #endregion
        // ---------------------------------------------------------------------------

        // ---------------------------------------------------------------------------
        #region GUI Tab GUI
        // ---------------------------------------------------------------------------
            private static void GUIGUI(GUISkin guiSkin)
            {
                // Verical scroll view for palette items
                gridScrollPos = EditorGUILayout.BeginScrollView(gridScrollPos);
                
                
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                guiToolbarFoldout = EditorGUILayout.Foldout(guiToolbarFoldout, "Toolbar Settings");
                if (guiToolbarFoldout)
                {
                    MAST.Settings.Data.gui.toolbar.position =
                        (ToolbarPos)EditorGUILayout.EnumPopup(
                        "Position", MAST.Settings.Data.gui.toolbar.position);
                    
                    MAST.Settings.Data.gui.toolbar.scale = EditorGUILayout.Slider(
                        "Scale", MAST.Settings.Data.gui.toolbar.scale, 0.5f, 1f);
                }
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                guiPaletteFoldout = EditorGUILayout.Foldout(guiPaletteFoldout, "Palette Settings");
                if (guiPaletteFoldout)
                {
                    MAST.Settings.Data.gui.palette.bgColor =
                        (PaleteBGColor)EditorGUILayout.EnumPopup(
                        "Background Color", MAST.Settings.Data.gui.palette.bgColor);
                    
                    EditorGUILayout.Space();
                    
                    MAST.Settings.Data.gui.palette.snapshotCameraPitch =
                        Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent(
                        "Thumbnail Pitch (0-360)", "Rotation around the Y axis"),
                        MAST.Settings.Data.gui.palette.snapshotCameraPitch), 0f, 360f);
                    
                    MAST.Settings.Data.gui.palette.snapshotCameraYaw =
                        Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent(
                        "Thumbnail Yaw (0-90)", "Rotation around the X axis"),
                        MAST.Settings.Data.gui.palette.snapshotCameraYaw), 0f, 90f);
                        
                    EditorGUILayout.Space();
                    MAST.Settings.Data.gui.palette.overwriteThumbnails = GUILayout.Toggle(MAST.Settings.Data.gui.palette.overwriteThumbnails, "Recreate Thumbnails when Loading from Folder");
                }
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                guiGridFoldout = EditorGUILayout.Foldout(guiGridFoldout, "Grid Settings");
                if (guiGridFoldout)
                {
                    EditorGUILayout.LabelField("Grid Dimensions", EditorStyles.boldLabel);
                    
                    MAST.Settings.Data.gui.grid.xzUnitSize =
                        EditorGUILayout.FloatField(new GUIContent(
                        "X/Z Unit Size", "Size of an individual grid square for snapping"),
                        MAST.Settings.Data.gui.grid.xzUnitSize);
                    MAST.Settings.Data.gui.grid.yUnitSize =
                        EditorGUILayout.FloatField(new GUIContent(
                        "Y Unit Size", "Y step for grid raising/lowering"),
                        MAST.Settings.Data.gui.grid.yUnitSize);
                    MAST.Settings.Data.gui.grid.cellCount =
                        EditorGUILayout.IntField(new GUIContent(
                        "Count (Center to Edge)", "Count of squares from center to each edge"),
                        MAST.Settings.Data.gui.grid.cellCount);
                    
                    EditorGUILayout.Space();
                    
                    EditorGUILayout.LabelField("Grid Cosmetics", EditorStyles.boldLabel);
                    
                    UnityEngine.GUI.skin = null;
                    
                    MAST.Settings.Data.gui.grid.tintColor =
                        EditorGUILayout.ColorField("Tint Color", MAST.Settings.Data.gui.grid.tintColor);
                    
                    UnityEngine.GUI.skin = guiSkin;
                }
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                // Get grid settings object path from selected item in project view
                if (GUILayout.Button(
                    new GUIContent("Load GUI Settings from selected file in Project",
                    "Use the selected (GUI Settings) file in project")))
                {
                    string path = MAST.LoadingHelper.GetPathOfSelectedObjectTypeOf(typeof(MAST.Settings.ScriptObj.GUI));
                    
                    if (path != "")
                    {
                        MAST.Settings.Data.core.guiSettingsPath = path;
                        MAST.Settings.Data.Load_GUI_Settings();
                    }
                }
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                EditorGUILayout.EndScrollView();
            }
        #endregion
        // ---------------------------------------------------------------------------

        // ---------------------------------------------------------------------------
        #region Hotkey Tab GUI
        // ---------------------------------------------------------------------------
            private static void HotkeyGUI()
            {
                // Verical scroll view for palette items
                hotkeyScrollPos = EditorGUILayout.BeginScrollView(hotkeyScrollPos);
                
                // ----------------------------------
                // Toggle grid On/Off
                // ----------------------------------
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                EditorGUILayout.LabelField("Toggle Grid On/Off", EditorStyles.boldLabel);
                
                MAST.Settings.Data.hotkey.toggleGridKey =
                    (KeyCode)EditorGUILayout.EnumPopup(
                    "Key", MAST.Settings.Data.hotkey.toggleGridKey);
                MAST.Settings.Data.hotkey.toggleGridMod =
                    (HotkeyModifier)EditorGUILayout.EnumPopup(
                    "Modifier", MAST.Settings.Data.hotkey.toggleGridMod);
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                // ----------------------------------
                // Move grid up
                // ----------------------------------
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                EditorGUILayout.LabelField("Move Grid Up", EditorStyles.boldLabel);
                
                MAST.Settings.Data.hotkey.moveGridUpKey =
                    (KeyCode)EditorGUILayout.EnumPopup(
                    "Key", MAST.Settings.Data.hotkey.moveGridUpKey);
                MAST.Settings.Data.hotkey.moveGridUpMod =
                    (HotkeyModifier)EditorGUILayout.EnumPopup(
                    "Modifier", MAST.Settings.Data.hotkey.moveGridUpMod);
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                // ----------------------------------
                // Move grid down
                // ----------------------------------
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                EditorGUILayout.LabelField("Move Grid Down", EditorStyles.boldLabel);
                
                MAST.Settings.Data.hotkey.moveGridDownKey =
                    (KeyCode)EditorGUILayout.EnumPopup(
                    "Key", MAST.Settings.Data.hotkey.moveGridDownKey);
                MAST.Settings.Data.hotkey.moveGridDownMod =
                    (HotkeyModifier)EditorGUILayout.EnumPopup(
                    "Modifier", MAST.Settings.Data.hotkey.moveGridDownMod);
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                // ----------------------------------
                // Deselect prefab in palette
                // ----------------------------------
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                EditorGUILayout.LabelField("Deselect Draw Tool and Palette Selection", EditorStyles.boldLabel);
                
                MAST.Settings.Data.hotkey.deselectPrefabKey =
                    (KeyCode)EditorGUILayout.EnumPopup(
                    "Key", MAST.Settings.Data.hotkey.deselectPrefabKey);
                MAST.Settings.Data.hotkey.deselectPrefabMod =
                    (HotkeyModifier)EditorGUILayout.EnumPopup(
                    "Modifier", MAST.Settings.Data.hotkey.deselectPrefabMod);
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                // ----------------------------------
                // Draw single
                // ----------------------------------
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                EditorGUILayout.LabelField("Select Draw Single Tool", EditorStyles.boldLabel);
                
                MAST.Settings.Data.hotkey.drawSingleKey =
                    (KeyCode)EditorGUILayout.EnumPopup(
                    "Key", MAST.Settings.Data.hotkey.drawSingleKey);
                MAST.Settings.Data.hotkey.drawSingleMod =
                    (HotkeyModifier)EditorGUILayout.EnumPopup(
                    "Modifier", MAST.Settings.Data.hotkey.drawSingleMod);
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                // ----------------------------------
                // Draw continuous
                // ----------------------------------
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                EditorGUILayout.LabelField("Select Draw Continuous Tool", EditorStyles.boldLabel);
                
                MAST.Settings.Data.hotkey.drawContinuousKey =
                    (KeyCode)EditorGUILayout.EnumPopup(
                    "Key", MAST.Settings.Data.hotkey.drawContinuousKey);
                MAST.Settings.Data.hotkey.drawContinuousMod =
                    (HotkeyModifier)EditorGUILayout.EnumPopup(
                    "Modifier", MAST.Settings.Data.hotkey.drawContinuousMod);
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                // ----------------------------------
                // Paint square
                // ----------------------------------
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                EditorGUILayout.LabelField("Select Paint Square Tool", EditorStyles.boldLabel);
                
                MAST.Settings.Data.hotkey.paintSquareKey =
                    (KeyCode)EditorGUILayout.EnumPopup(
                    "Key", MAST.Settings.Data.hotkey.paintSquareKey);
                MAST.Settings.Data.hotkey.paintSquareMod =
                    (HotkeyModifier)EditorGUILayout.EnumPopup(
                    "Modifier", MAST.Settings.Data.hotkey.paintSquareMod);
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                // ----------------------------------
                // Randomizer
                // ----------------------------------
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                EditorGUILayout.LabelField("Select Randomizer Tool", EditorStyles.boldLabel);
                
                MAST.Settings.Data.hotkey.randomizerKey =
                    (KeyCode)EditorGUILayout.EnumPopup(
                    "Key", MAST.Settings.Data.hotkey.randomizerKey);
                MAST.Settings.Data.hotkey.randomizerMod =
                    (HotkeyModifier)EditorGUILayout.EnumPopup(
                    "Modifier", MAST.Settings.Data.hotkey.randomizerMod);
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                // ----------------------------------
                // Erase
                // ----------------------------------
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                EditorGUILayout.LabelField("Select Erase Tool", EditorStyles.boldLabel);
                
                MAST.Settings.Data.hotkey.eraseKey =
                    (KeyCode)EditorGUILayout.EnumPopup(
                    "Key", MAST.Settings.Data.hotkey.eraseKey);
                MAST.Settings.Data.hotkey.eraseMod =
                    (HotkeyModifier)EditorGUILayout.EnumPopup(
                    "Modifier", MAST.Settings.Data.hotkey.eraseMod);
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                // ----------------------------------
                // New random seed
                // ----------------------------------
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                EditorGUILayout.LabelField("Generate New Random(izer) Seed", EditorStyles.boldLabel);
                
                MAST.Settings.Data.hotkey.newRandomSeedKey =
                    (KeyCode)EditorGUILayout.EnumPopup(
                    "Key", MAST.Settings.Data.hotkey.newRandomSeedKey);
                MAST.Settings.Data.hotkey.newRandomSeedMod =
                    (HotkeyModifier)EditorGUILayout.EnumPopup(
                    "Modifier", MAST.Settings.Data.hotkey.newRandomSeedMod);
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                // ----------------------------------
                // Rotate prefab
                // ----------------------------------
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                EditorGUILayout.LabelField("Rotate Prefab", EditorStyles.boldLabel);
                
                MAST.Settings.Data.hotkey.rotatePrefabKey =
                    (KeyCode)EditorGUILayout.EnumPopup(
                    "Key", MAST.Settings.Data.hotkey.rotatePrefabKey);
                MAST.Settings.Data.hotkey.rotatePrefabMod =
                    (HotkeyModifier)EditorGUILayout.EnumPopup(
                    "Modifier", MAST.Settings.Data.hotkey.rotatePrefabMod);
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                // ----------------------------------
                // Flip prefab
                // ----------------------------------
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                EditorGUILayout.LabelField("Flip Prefab", EditorStyles.boldLabel);
                
                MAST.Settings.Data.hotkey.flipPrefabKey =
                    (KeyCode)EditorGUILayout.EnumPopup(
                    "Key", MAST.Settings.Data.hotkey.flipPrefabKey);
                MAST.Settings.Data.hotkey.flipPrefabMod =
                    (HotkeyModifier)EditorGUILayout.EnumPopup(
                    "Modifier", MAST.Settings.Data.hotkey.flipPrefabMod);
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                // ----------------------------------
                // Paint material
                // ----------------------------------
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                EditorGUILayout.LabelField("Paint Material", EditorStyles.boldLabel);
                
                MAST.Settings.Data.hotkey.paintMaterialKey =
                    (KeyCode)EditorGUILayout.EnumPopup(
                    "Key", MAST.Settings.Data.hotkey.paintMaterialKey);
                MAST.Settings.Data.hotkey.paintMaterialMod =
                    (HotkeyModifier)EditorGUILayout.EnumPopup(
                    "Modifier", MAST.Settings.Data.hotkey.paintMaterialMod);
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                // ----------------------------------
                // Restore material
                // ----------------------------------
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                EditorGUILayout.LabelField("Restore Material", EditorStyles.boldLabel);
                
                MAST.Settings.Data.hotkey.restoreMaterialKey =
                    (KeyCode)EditorGUILayout.EnumPopup(
                    "Key", MAST.Settings.Data.hotkey.restoreMaterialKey);
                MAST.Settings.Data.hotkey.restoreMaterialMod =
                    (HotkeyModifier)EditorGUILayout.EnumPopup(
                    "Modifier", MAST.Settings.Data.hotkey.restoreMaterialMod);
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                // ----------------------------------
                // Change scriptable object
                // ----------------------------------
                GUILayout.BeginVertical("MAST Toolbar BG Inset");
                
                // Get hotkey settings object path from selected item in project view
                if (GUILayout.Button(
                    new GUIContent("Load Hotkey Settings from selected file in Project",
                    "Use the selected (Hotkey Settings) file in project")))
                {
                    string path = MAST.LoadingHelper.GetPathOfSelectedObjectTypeOf(typeof(MAST.Settings.ScriptObj.Hotkey));
                    
                    if (path != "")
                    {
                        MAST.Settings.Data.core.hotkeySettingsPath = path;
                        MAST.Settings.Data.Load_Hotkey_Settings();
                    }
                }
                
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                
                EditorGUILayout.EndScrollView();
            }
        #endregion
        // ---------------------------------------------------------------------------
            
            // Return whether preferences have changed and set "preferences changed" back to false
            public static bool PreferencesChanged()
            {
                if (prefChanged)
                {
                    MAST.Building.GridManager.UpdateGridSettings();
                    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                    
                    prefChanged = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
#endif