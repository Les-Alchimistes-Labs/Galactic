#if (UNITY_EDITOR)
namespace MAST
{
    // Palette background
    public enum PaleteBGColor { Dark = 0, Gray = 1 , Light = 2 }

    // Toolbar position
    public enum ToolbarPos { Left, Right } // Later add top and bottom for horizontal palette

    // Modifier used in conjunction with each hotkey
    public enum HotkeyModifier { NONE = 0, SHIFT = 1 }

    // Draw Tool Selection
    public enum BuildMode { None, DrawSingle, DrawContinuous, PaintArea, Randomize, Erase }

    // Axis for rotating and flipping Prefabs
    public enum Axis { X = 0, Y = 1, Z = 2 }

    // Randomizer scale axis lock
    public enum ScaleAxisLock { NONE = 0, XZ = 1, XYZ = 2 }

    // Raycast direction for Prefab placement
    public enum DirectionVector { Up = 0, Down = 1, Left = 2, Right = 3, Forward = 4, Back = 5 }
}
#endif