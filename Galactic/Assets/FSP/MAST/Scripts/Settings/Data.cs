using System;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Settings
    {
        [Serializable]
        public static class Data
        { 
            [SerializeField] public static MAST.Settings.ScriptObj.Core core;
            [SerializeField] public static MAST.Settings.ScriptObj.GUI gui;
            [SerializeField] public static MAST.Settings.ScriptObj.Placement placement;
            [SerializeField] public static MAST.Settings.ScriptObj.Hotkey hotkey;
            
            // ---------------------------------------------------------------------------
            // When class is enabled
            // ---------------------------------------------------------------------------
            public static void Initialize()
            {
                // Load settings from scriptable object if it was lost
                if (core == null)
                    Load_Settings();
            }
            
            static void OnDisable() {}
            
            static void OnFocus() {}
            
            static void OnDestroy() {}
            
            // ---------------------------------------------------------------------------
            #region Manage Settings Scriptable Object
            // ---------------------------------------------------------------------------
            public static void Load_Settings()
            {
                Load_Core_Settings();
                Load_GUI_Settings();
                Load_Placement_Settings();
                Load_Hotkey_Settings();
                
                // Save the scriptable object
                //AssetDatabase.SaveAssets();
            }
            
            public static void Load_Core_Settings()
            {
                // Get MAST Core path
                string corePath = MAST.LoadingHelper.GetMASTRootFolder() + "/Etc/CoreSettings.asset";
                
                // Load the MAST Core scriptable object
                core = AssetDatabase.LoadAssetAtPath<MAST.Settings.ScriptObj.Core>(corePath);
                
                // If the Core scriptable object isn't found, create a new default one
                if (core == null)
                {
                    core = ScriptableObject.CreateInstance<MAST.Settings.ScriptObj.Core>();
                    AssetDatabase.CreateAsset(core, corePath);
                }
            }
            
            public static void Load_GUI_Settings()
            {
                // Get/create GUI scriptable object
                gui = AssetDatabase.LoadAssetAtPath<MAST.Settings.ScriptObj.GUI>(core.guiSettingsPath);
                
                // If scripable object doesn't exist, create a default scripable object
                if (gui == null)
                {
                    gui = ScriptableObject.CreateInstance<MAST.Settings.ScriptObj.GUI>();
                    AssetDatabase.CreateAsset(gui, core.guiSettingsPath);
                }
            }
            
            public static void Load_Placement_Settings()
            {
                // Get/create Placement scriptable object
                placement = AssetDatabase.LoadAssetAtPath<MAST.Settings.ScriptObj.Placement>(core.placementSettingsPath);
                
                // If scripable object doesn't exist, create a default scripable object
                if (placement == null)
                {
                    placement = ScriptableObject.CreateInstance<MAST.Settings.ScriptObj.Placement>();
                    AssetDatabase.CreateAsset(placement, core.placementSettingsPath);
                }
            }
            
            public static void Load_Hotkey_Settings()
            {
                // Get/create Hotkey scriptable object
                hotkey = AssetDatabase.LoadAssetAtPath<MAST.Settings.ScriptObj.Hotkey>(core.hotkeySettingsPath);
                
                // If scripable object doesn't exist, create a default scripable object
                if (hotkey == null)
                {
                    hotkey = ScriptableObject.CreateInstance<MAST.Settings.ScriptObj.Hotkey>();
                    AssetDatabase.CreateAsset(hotkey, core.hotkeySettingsPath);
                }
            }
            
            // Save preferences to a scriptable object
            public static void Save_Settings()
            {
                //AssetDatabase.SaveAssets();
                //AssetDatabase.Refresh();
                
                EditorUtility.SetDirty(core);
                EditorUtility.SetDirty(gui);
                EditorUtility.SetDirty(placement);
                EditorUtility.SetDirty(hotkey);
            }
            
            #endregion
            // ---------------------------------------------------------------------------
        }
    }
}
#endif