#if (UNITY_EDITOR)

namespace MAST
{
    public static class Const
    {
        // Grid
        public static class Grid
        {
            public static string defaultName = "MAST_Grid";
            public static string defaultParentName = "MAST_Grid_Parent";
            public static float yOffsetToAvoidTearing = -0.001f;
            public static int gridLayer = 4;
        }
        //public static Grid_Class grid = new Grid_Class();
        
        // Placement
        public static class Placement
        {
            public static string defaultTargetParentName = "MAST_Holder";
            public static string defaultTargetParentTag = "MAST_Holder";
            public static int visualizerLayer = 2;
        }
        //public static Placement_Class placement = new Placement_Class();
        
        
    }
}
#endif