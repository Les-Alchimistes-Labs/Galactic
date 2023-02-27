using UnityEngine;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Settings
    {
        namespace ScriptObj
        {
            [System.Serializable]
            public class Hotkey : ScriptableObject
            {
                [SerializeField] public KeyCode drawSingleKey = KeyCode.D;
                [SerializeField] public HotkeyModifier drawSingleMod = HotkeyModifier.SHIFT;
                
                [SerializeField] public KeyCode drawContinuousKey = KeyCode.C;
                [SerializeField] public HotkeyModifier drawContinuousMod = HotkeyModifier.SHIFT;
                
                [SerializeField] public KeyCode paintSquareKey = KeyCode.P;
                [SerializeField] public HotkeyModifier paintSquareMod = HotkeyModifier.SHIFT;
                
                [SerializeField] public KeyCode randomizerKey = KeyCode.X;
                [SerializeField] public HotkeyModifier randomizerMod = HotkeyModifier.SHIFT;
                
                [SerializeField] public KeyCode eraseKey = KeyCode.E;
                [SerializeField] public HotkeyModifier eraseMod = HotkeyModifier.SHIFT;
                
                [SerializeField] public KeyCode newRandomSeedKey = KeyCode.X;
                [SerializeField] public HotkeyModifier newRandomSeedMod = HotkeyModifier.NONE;
                
                [SerializeField] public KeyCode toggleGridKey = KeyCode.G;
                [SerializeField] public HotkeyModifier toggleGridMod = HotkeyModifier.NONE;
                
                [SerializeField] public KeyCode moveGridUpKey = KeyCode.W;
                [SerializeField] public HotkeyModifier moveGridUpMod = HotkeyModifier.SHIFT;
                
                [SerializeField] public KeyCode moveGridDownKey = KeyCode.S;
                [SerializeField] public HotkeyModifier moveGridDownMod = HotkeyModifier.SHIFT;
                
                [SerializeField] public KeyCode deselectPrefabKey = KeyCode.Escape;
                [SerializeField] public HotkeyModifier deselectPrefabMod = HotkeyModifier.NONE;
                
                [SerializeField] public KeyCode rotatePrefabKey = KeyCode.Space;
                [SerializeField] public HotkeyModifier rotatePrefabMod = HotkeyModifier.NONE;
                
                [SerializeField] public KeyCode flipPrefabKey = KeyCode.F;
                [SerializeField] public HotkeyModifier flipPrefabMod = HotkeyModifier.NONE;
                
                [SerializeField] public KeyCode paintMaterialKey = KeyCode.M;
                [SerializeField] public HotkeyModifier paintMaterialMod = HotkeyModifier.SHIFT;
                
                [SerializeField] public KeyCode restoreMaterialKey = KeyCode.R;
                [SerializeField] public HotkeyModifier restoreMaterialMod = HotkeyModifier.SHIFT;
            }
        }
    }
}
#endif