using UnityEngine;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Settings
    {
        namespace ScriptObj
        {
            public class Core : ScriptableObject
            {
                [SerializeField] public string guiSettingsPath =
                    MAST.LoadingHelper.GetMASTRootFolder() + "/Settings/GUI.asset";
                    
                [SerializeField] public string placementSettingsPath =
                    MAST.LoadingHelper.GetMASTRootFolder() + "/Settings/Placement.asset";
                    
                [SerializeField] public string hotkeySettingsPath =
                    MAST.LoadingHelper.GetMASTRootFolder() + "/Settings/Hotkey.asset";
            }
        }
    }
}
#endif