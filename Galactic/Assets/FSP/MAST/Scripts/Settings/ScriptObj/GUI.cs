using System;
using UnityEngine;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Settings
    {
        namespace ScriptObj
        {
            [Serializable]
            public class GUI : ScriptableObject
            {
                [SerializeField] public Grid grid;
                [Serializable] public class Grid
                {
                    [SerializeField] public float xzUnitSize = 1f;
                    [SerializeField] public float yUnitSize = 1f;
                    
                    [SerializeField] public int cellCount = 50;
                    
                    [SerializeField] public int gridHeight = 0;
                    
                    [SerializeField] public Color tintColor = new Color32(255, 255, 255, 127);
                    
                    [SerializeField] public float yPos()
                    {
                        return gridHeight * yUnitSize;
                    }
                }
                
                [SerializeField] public Palette palette;
                [Serializable] public class Palette
                {
                    [SerializeField] public PaleteBGColor bgColor = PaleteBGColor.Dark;
                    [SerializeField] public float snapshotCameraPitch = 225f;
                    [SerializeField] public float snapshotCameraYaw = 30f;
                    [SerializeField] public bool overwriteThumbnails = false;
                }
                
                [SerializeField] public Toolbar toolbar;
                [Serializable] public class Toolbar
                {
                    [SerializeField] public int selectedDrawToolIndex = -1;
                    [SerializeField] public int selectedPaintToolIndex = -1;
                    [SerializeField] public ToolbarPos position = ToolbarPos.Left;
                    [SerializeField] public float scale = 1f;
                }
            }
        }
    }
}
#endif