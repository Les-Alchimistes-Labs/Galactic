using UnityEngine;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Building
    {
        public static class PaintArea
        {
            [SerializeField] private static bool paintingArea = false;
            [SerializeField] private static Vector3 paintAreaStart = new Vector3(0f, 0f, 0f);
            [SerializeField] private static GameObject paintAreaVisualizer;
            [SerializeField] private static Material paintAreaMaterial;
            
            // Start paint area
            public static void StartPaintArea()
            {
                if (Visualizer.GetGameObject() != null)
                {
                    // Set painting area to true
                    paintingArea = true;
                    
                    // Record paint area start location
                    paintAreaStart = Visualizer.GetGameObject().transform.position;
                    paintAreaStart.y = Settings.Data.gui.grid.gridHeight *
                        Settings.Data.gui.grid.yUnitSize + Const.Grid.yOffsetToAvoidTearing;
                    
                    // Create new Paint Area Visualizer
                    paintAreaVisualizer = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    paintAreaVisualizer.transform.position = new Vector3(0f, 0f, 0f);
                    paintAreaVisualizer.name = "MAST_Paint_Area_Visualizer";
                    
                    // Configure Paint Area Visualizer MeshRenderer
                    MeshRenderer paintAreaMeshRenderer = paintAreaVisualizer.GetComponent<MeshRenderer>();
                    paintAreaMeshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                    paintAreaMeshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                    paintAreaMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    paintAreaMeshRenderer.receiveShadows = false;
                    
                    // Configure Paint Area Visualizer Material
                    if (paintAreaMaterial == null)
                        paintAreaMaterial = LoadingHelper.GetPaintAreaMaterial();
                    paintAreaMeshRenderer.material = paintAreaMaterial;
                    
                    // Hide the Paint Area Visualizer in the hierarchy
                    paintAreaVisualizer.hideFlags = HideFlags.HideInHierarchy;
                    
                    // Update the paint area
                    UpdatePaintArea();
                }
            }
            
            // Update paint area
            public static void UpdatePaintArea()
            {
                // If painting area
                if (paintingArea)
                {
                    // Get current mouse position on grid
                    Vector3 paintAreaEnd = Helper.GetPositionOnGridClosestToMousePointer();
                    paintAreaEnd.y = Settings.Data.gui.grid.gridHeight *
                        Settings.Data.gui.grid.yUnitSize + Const.Grid.yOffsetToAvoidTearing;
                    
                    // Make sure paint area start is at the current grid height, incase the grid was moved
                    paintAreaStart.y = Settings.Data.gui.grid.gridHeight *
                        Settings.Data.gui.grid.yUnitSize + Const.Grid.yOffsetToAvoidTearing;
                    
                    // Get dimensions of paint area
                    Vector3 scale = new Vector3(
                        Mathf.Abs((paintAreaStart.x - paintAreaEnd.x) / 10f) + (Settings.Data.gui.grid.xzUnitSize / 10),
                        1,
                        Mathf.Abs((paintAreaStart.z - paintAreaEnd.z) / 10f) + (Settings.Data.gui.grid.xzUnitSize / 10));
                    
                    // Update paint area visualizer position to be between the start and end points
                    paintAreaVisualizer.transform.position = (paintAreaStart + paintAreaEnd) / 2;
                    
                    // Update paint area visualizer x and z scale
                    paintAreaVisualizer.transform.localScale = scale;
                    
                }
            }
            
            // Complete paint area
            public static void CompletePaintArea()
            {
                // Get current mouse position on grid
                Vector3 paintAreaEnd = Helper.GetPositionOnGridClosestToMousePointer();
                paintAreaEnd.y = Settings.Data.gui.grid.gridHeight *
                    Settings.Data.gui.grid.yUnitSize + Const.Grid.yOffsetToAvoidTearing;
                
                // If selected Prefab can be scaled
                if (Helper.GetPaintAreaStretch())
                {
                    // Place the prefab
                    GameObject placedPrefab = Placement.PlacePrefabInScene();
                    
                    // Move prefab centerpoint between both paint areas
                    placedPrefab.transform.position = (new Vector3(paintAreaStart.x, Settings.Data.gui.grid.gridHeight * Settings.Data.gui.grid.yUnitSize, paintAreaStart.z)
                                                    + new Vector3(paintAreaEnd.x, Settings.Data.gui.grid.gridHeight * Settings.Data.gui.grid.yUnitSize, paintAreaEnd.z))
                                                    / 2;
                    
                    // Apply position offset
                    placedPrefab.transform.position += Helper.GetOffsetPosition();
                    
                    //----------------------------------------------
                    // Scale prefab X and Z to match paint area
                    //----------------------------------------------
                    Vector3 scale;
                    
                    // Get Y angle of Visualizer, divide by 90, and round.  Result will be 0-3 instead of 0-360
                    int prefabRotationQuadrant = Mathf.RoundToInt(Visualizer.GetGameObject().transform.rotation.eulerAngles.y / 90f);
                    
                    // If prefab is rotated 0 or 180 degrees, then scale normally
                    if (prefabRotationQuadrant == 0 || prefabRotationQuadrant == 2)
                    {
                        scale = new Vector3(
                            Mathf.Abs(paintAreaStart.x - paintAreaEnd.x) + Settings.Data.gui.grid.xzUnitSize,
                            1,
                            Mathf.Abs(paintAreaStart.z - paintAreaEnd.z) + Settings.Data.gui.grid.xzUnitSize);
                    }
                    // If prefab is rotated 90 or 270 degrees, then swap X and Z for the scale
                    else
                    {
                        scale = new Vector3(
                            Mathf.Abs(paintAreaStart.z - paintAreaEnd.z) + Settings.Data.gui.grid.xzUnitSize,
                            1,
                            Mathf.Abs(paintAreaStart.x - paintAreaEnd.x) + Settings.Data.gui.grid.xzUnitSize);
                    }
                    
                    // Apply calculate scale
                    placedPrefab.transform.localScale = scale;
                    
                    // Apply rotation from the Visualizer
                    placedPrefab.transform.rotation = Visualizer.GetGameObject().transform.rotation;
                    
                }
                
                // If selected Prefab cannot be scaled
                else
                {
                    // Get base of rows and columns "lowest value"
                    float xBase = paintAreaStart.x < paintAreaEnd.x ? paintAreaStart.x : paintAreaEnd.x;
                    float zBase = paintAreaStart.z < paintAreaEnd.z ? paintAreaStart.z : paintAreaEnd.z;
                    
                    // Get count of rows and columns in paint area
                    int xCount = (int)(Mathf.Abs(paintAreaStart.x - paintAreaEnd.x) / Settings.Data.gui.grid.xzUnitSize);
                    int zCount = (int)(Mathf.Abs(paintAreaStart.z - paintAreaEnd.z) / Settings.Data.gui.grid.xzUnitSize);
                    
                    // Loop through each grid space in the area
                    for (int x = 0; x <= xCount; x++)
                    {
                        for (int z = 0; z <= zCount; z++)
                        {
                            // Set visualizer position
                            Visualizer.GetGameObject().transform.position =
                                new Vector3(xBase + (x * Settings.Data.gui.grid.xzUnitSize),
                                Settings.Data.gui.grid.gridHeight * Settings.Data.gui.grid.yUnitSize,
                                zBase + (z * Settings.Data.gui.grid.xzUnitSize)) + Helper.GetOffsetPosition();
                            
                            // Add Prefab to scene
                            Placement.PlacePrefabInScene();
                        }
                    }
                }
                
                // Delete painting area
                DeletePaintArea();
            }
            
            // Delete paint area
            public static void DeletePaintArea()
            {
                // Set painting area to false
                paintingArea = false;
                
                // Find existing paint area and delete it - even if disabled
                foreach (GameObject go in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                {
                    if (go.name == "MAST_Paint_Area_Visualizer")
                        GameObject.DestroyImmediate(go);
                }
            }
        }
    }
}

#endif