using UnityEngine;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace GUI
    {
        public class Hotkeys
        {
            public bool ProcessHotkeys()
            {
                // Set change made to false
                bool changeMade = false;
                
                // Get current event
                Event currentEvent = Event.current;
                
                // Get the control's ID
                int controlID = GUIUtility.GetControlID(FocusType.Passive);
                
                // If a key is pressed
                if (Event.current.GetTypeForControl(controlID) == EventType.KeyDown)
                {
                    // Toggle grid visibility
                    if (KeysPressed(currentEvent,
                        MAST.Settings.Data.hotkey.toggleGridKey,
                        MAST.Settings.Data.hotkey.toggleGridMod))
                    {
                        MAST.Building.GridManager.gridExists = !MAST.Building.GridManager.gridExists;
                        MAST.Building.GridManager.ChangeGridVisibility();
                        changeMade = true;
                    }
                    
                    // Move grid up
                    if (KeysPressed(currentEvent,
                        MAST.Settings.Data.hotkey.moveGridUpKey,
                        MAST.Settings.Data.hotkey.moveGridUpMod))
                    {
                        MAST.Building.GridManager.MoveGridUp();
                        changeMade = true;
                    }
                    
                    // Move grid down
                    if (KeysPressed(currentEvent,
                        MAST.Settings.Data.hotkey.moveGridDownKey,
                        MAST.Settings.Data.hotkey.moveGridDownMod))
                    {
                        MAST.Building.GridManager.MoveGridDown();
                        changeMade = true;
                    }
                    
                    // Deselect prefab in palette or draw tool
                    if (KeysPressed(currentEvent,
                        MAST.Settings.Data.hotkey.deselectPrefabKey,
                        MAST.Settings.Data.hotkey.deselectPrefabMod))
                    {
                        switch (MAST.GUI.DataManager.state.selectedInterfaceTab)
                        {
                            // Build tab
                            case 0:
                                // Deselect prefab in palette and draw tool
                                MAST.Building.GUI.Palette.RemovePrefabSelection();
                                changeMade = true;
                                break;
                                
                            // Paint tab
                            case 1:
                                // Deselect material in palette and paint tool
                                MAST.Painting.Palette.Manager.selectedItemIndex = -1;
                                MAST.Settings.Data.gui.toolbar.selectedPaintToolIndex = -1;
                                MAST.Painting.Painter.ClearCurrentMaterialPaintPreview();
                                changeMade = true;
                                break;
                        }
                    }
                    
                    // Draw single
                    if (KeysPressed(currentEvent,
                        MAST.Settings.Data.hotkey.drawSingleKey,
                        MAST.Settings.Data.hotkey.drawSingleMod))
                    {
                        // If Draw Single isn't selected, select it
                        if (MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex != 0)
                        {
                            MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex = 0;
                            MAST.Building.Interface.ChangePlacementMode(BuildMode.DrawSingle);
                        }
                        else
                        {
                            // If Draw Single was selected, deselect it
                            if(MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex == 0)
                            {
                                MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex = -1;
                                MAST.Building.Interface.ChangePlacementMode(BuildMode.None);
                            }
                        }
                        changeMade = true;
                    }
                    
                    // Draw Continuous
                    if (KeysPressed(currentEvent,
                        MAST.Settings.Data.hotkey.drawContinuousKey,
                        MAST.Settings.Data.hotkey.drawContinuousMod))
                    {
                        // If Draw Continuous isn't selected, select it
                        if (MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex != 1)
                        {
                            MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex = 1;
                            MAST.Building.Interface.ChangePlacementMode(BuildMode.DrawContinuous);
                        }
                        else
                        {
                            // If Draw continuous was selected, deselect it
                            if(MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex == 1)
                            {
                                MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex = -1;
                                MAST.Building.Interface.ChangePlacementMode(BuildMode.None);
                            }
                        }
                        changeMade = true;
                    }
                    
                    // Paint square
                    if (KeysPressed(currentEvent,
                        MAST.Settings.Data.hotkey.paintSquareKey,
                        MAST.Settings.Data.hotkey.paintSquareMod))
                    {
                        // If Paint Square isn't selected, select it
                        if (MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex != 2)
                        {
                            MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex = 2;
                            MAST.Building.Interface.ChangePlacementMode(BuildMode.PaintArea);
                        }
                        else
                        {
                            // If Paint Square was selected, deselect it
                            if(MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex == 2)
                            {
                                MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex = -1;
                                MAST.Building.Interface.ChangePlacementMode(BuildMode.None);
                            }
                        }
                        changeMade = true;
                    }
                    
                    // Toggle randomizer
                    if (KeysPressed(currentEvent,
                        MAST.Settings.Data.hotkey.randomizerKey,
                        MAST.Settings.Data.hotkey.randomizerMod))
                    {
                        // If Randomizer isn't selected, select it
                        if (MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex != 3)
                        {
                            MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex = 3;
                            MAST.Building.Interface.ChangePlacementMode(BuildMode.Randomize);
                        }
                        else
                        {
                            // If Randomizer was selected, deselect it
                            if(MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex == 3)
                            {
                                MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex = -1;
                                MAST.Building.Interface.ChangePlacementMode(BuildMode.None);
                            }
                        }
                        changeMade = true;
                    }
                    
                    // Toggle erase
                    if (KeysPressed(currentEvent,
                        MAST.Settings.Data.hotkey.eraseKey,
                        MAST.Settings.Data.hotkey.eraseMod))
                    {
                        // If Erase isn't selected, select it
                        if (MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex != 4)
                        {
                            MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex = 4;
                            MAST.Building.Interface.ChangePlacementMode(BuildMode.Erase);
                        }
                        else
                        {
                            // If Erase was selected, deselect it
                            if(MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex == 4)
                            {
                                MAST.Settings.Data.gui.toolbar.selectedDrawToolIndex = -1;
                                MAST.Building.Interface.ChangePlacementMode(BuildMode.None);
                            }
                        }
                        changeMade = true;
                    }
                    
                    // New random seed
                    if (KeysPressed(currentEvent,
                        MAST.Settings.Data.hotkey.newRandomSeedKey,
                        MAST.Settings.Data.hotkey.newRandomSeedMod))
                    {
                        MAST.Building.Randomizer.GenerateNewRandomSeed();
                        changeMade = true;
                    }
                    
                    // Rotate prefab
                    if (KeysPressed(currentEvent,
                        MAST.Settings.Data.hotkey.rotatePrefabKey,
                        MAST.Settings.Data.hotkey.rotatePrefabMod))
                    {
                        MAST.Building.Manipulate.RotateObject();
                        changeMade = true;
                    }
                    
                    // Flip prefab
                    if (KeysPressed(currentEvent,
                        MAST.Settings.Data.hotkey.flipPrefabKey,
                        MAST.Settings.Data.hotkey.flipPrefabMod))
                    {
                        MAST.Building.Manipulate.FlipObject();
                        changeMade = true;
                    }
                    
                    // Toggle paint material
                    if (KeysPressed(currentEvent,
                        MAST.Settings.Data.hotkey.paintMaterialKey,
                        MAST.Settings.Data.hotkey.paintMaterialMod))
                    {
                        // If Paint Material isn't selected, select it
                        if (MAST.Settings.Data.gui.toolbar.selectedPaintToolIndex != 0)
                        {
                            MAST.Settings.Data.gui.toolbar.selectedPaintToolIndex = 0;
                        }
                        else
                        {
                            // If Paint Material was selected, deselect it
                            if(MAST.Settings.Data.gui.toolbar.selectedPaintToolIndex == 0)
                            {
                                MAST.Settings.Data.gui.toolbar.selectedPaintToolIndex = -1;
                                MAST.Painting.Painter.ClearCurrentMaterialPaintPreview();
                            }
                        }
                        changeMade = true;
                    }
                    
                    // Toggle restore material
                    if (KeysPressed(currentEvent,
                        MAST.Settings.Data.hotkey.restoreMaterialKey,
                        MAST.Settings.Data.hotkey.restoreMaterialMod))
                    {
                        // If Restore Material isn't selected, select it
                        if (MAST.Settings.Data.gui.toolbar.selectedPaintToolIndex != 1)
                        {
                            MAST.Settings.Data.gui.toolbar.selectedPaintToolIndex = 1;
                        }
                        else
                        {
                            // If Restore Material was selected, deselect it
                            if(MAST.Settings.Data.gui.toolbar.selectedPaintToolIndex == 1)
                            {
                                MAST.Settings.Data.gui.toolbar.selectedPaintToolIndex = -1;
                                MAST.Painting.Painter.ClearCurrentMaterialPaintPreview();
                            }
                        }
                        changeMade = true;
                    }
                }
                
                return changeMade;
            }
            
            // All these key methods could be grouped up a lot nicer later using delegates and refs
            private bool KeysPressed(Event currentEvent, KeyCode key, HotkeyModifier mod)
            {
                // If correct key was pressed for the first time
                if (currentEvent.keyCode == key)
                {
                    // If the correct modifier was held down
                    if (IsModifierPressed(currentEvent, mod))
                        return true;
                }
                
                return false;
            }
            
            // Return true if the correct modifier key is held down
            private bool IsModifierPressed(Event currentEvent, HotkeyModifier modifier)
            {
                // If SHIFT is needed and is held down, return true
                if (modifier == HotkeyModifier.SHIFT && currentEvent.shift)
                    return true;
                
                // If SHIFT is not needed and SHIFT is not held down, return true
                else if (modifier == HotkeyModifier.NONE && !currentEvent.shift)
                    return true;
                
                return false;
            }
        }
    }
}
#endif