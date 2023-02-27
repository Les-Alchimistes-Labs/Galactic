using System;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Building
    {
        [Serializable]
        public static class GridManager
        {
            // ---------------------------------------------------------------------------
            #region Variable Declaration
            // ---------------------------------------------------------------------------
            
            // Grid Appearance
            [SerializeField] public static bool gridExists = false;
            
            // Grid in Scene
            [SerializeField] private static GameObject gridGameObject;
            [SerializeField] private static Material gridMaterial;
            //[SerializeField] private static GameObject gridParent; // hidden in inspector with child grid left visible so it still draws gizmolines
            
            #endregion
            // ---------------------------------------------------------------------------
            
            // ---------------------------------------------------------------------------
            // Initialize
            // ---------------------------------------------------------------------------
            public static void Initialize()
            {
                
            }
            
            // ---------------------------------------------------------------------------
            #region Grid Location
            // ---------------------------------------------------------------------------
            public static void MoveGridUp()
            {
                if (gridExists)
                {
                    // Move Grid Up
                    Settings.Data.gui.grid.gridHeight += 1;
                    MoveGridToNewHeight();
                }
            }
            
            public static void MoveGridDown()
            {
                if (gridExists)
                {
                    // Move Grid Up
                    Settings.Data.gui.grid.gridHeight -= 1;
                    MoveGridToNewHeight();
                }
            }
            
            private static void MoveGridToNewHeight()
            {
                // Calculate new grid height
                float gridY = Settings.Data.gui.grid.gridHeight * Settings.Data.gui.grid.yUnitSize + Const.Grid.yOffsetToAvoidTearing;
                gridGameObject.transform.position =
                    new Vector3(gridGameObject.transform.position.x, gridY, gridGameObject.transform.position.z);
            }
            #endregion
            // ---------------------------------------------------------------------------
            
            // ---------------------------------------------------------------------------
            #region Create/Destroy Grid
            // ---------------------------------------------------------------------------
            
            // Return if grid reference exists
            public static bool DoesGridExist()
            {
                return gridExists;
            }
            
            // Change grid visibility
            public static void ChangeGridVisibility()
            {
                if (gridExists)
                {
                    CreateGrid();
                }
                else
                {
                    DestroyGrid();
                    
                    // Deselect draw tool and palette item and destroy any active prefab visualizer
                    GUI.Palette.RemovePrefabSelection();
                }
            }
            
            // Destroy any existing grid
            public static void DestroyGrid()
            {
                // Find existing grid(s) and delete them - even if disabled
                foreach (GameObject go in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                {
                    if (go.name == Const.Grid.defaultName)
                    {
                        GameObject.DestroyImmediate(go);
                    }
                }

                // Remove locked layer
                UnityEditor.Tools.lockedLayers &= ~(1 << Const.Grid.gridLayer);
                
                gridExists = false;
            }
            
            // Create grid
            public static void CreateGrid()
            {
                CreateLinkToGrid();

                // Lock the layer the grid is on
                UnityEditor.Tools.lockedLayers = 1 << Const.Grid.gridLayer;
                
                gridExists = true;
            }
            
            // Create link to any grid that exists, or create a new grid
            private static void CreateLinkToGrid()
            {
                gridGameObject = GameObject.Find(Const.Grid.defaultName);
                
                DestroyGrid();
                CreateNewGrid();
            }
            
            // ---------------------------------------------------------------------------
            // Create a New Grid in the Hierarchy from the Grid Prefab
            // ---------------------------------------------------------------------------
            static void CreateNewGrid()
            {
                // Create new Grid GameObject
                gridGameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
                gridGameObject.transform.position = new Vector3(0f, 0f, 0f);
                gridGameObject.name = Const.Grid.defaultName;
                gridGameObject.layer = Const.Grid.gridLayer;
                
                // Configure Grid GameObject MeshRenderer
                MeshRenderer gridMeshRenderer = gridGameObject.GetComponent<MeshRenderer>();
                gridMeshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                gridMeshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                gridMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                gridMeshRenderer.receiveShadows = false;
                
                // Configure Grid GameObject Material
                if (gridMaterial == null)
                {
                    gridMaterial = LoadingHelper.GetGridMaterial();
                }
                gridMaterial.SetColor("_Color", Settings.Data.gui.grid.tintColor);
                gridMeshRenderer.material = gridMaterial;
                
                // Add MAST_Grid_Component script to grid and pass grid preferences to it
                UpdateGridSettings();
                
                // Return the grid to its last saved height
                MoveGridToNewHeight();
                
                // Hide the grid in the hierarchy
                gridGameObject.hideFlags = HideFlags.HideInHierarchy;
            }
            #endregion
            // ---------------------------------------------------------------------------
            
            // ---------------------------------------------------------------------------
            #region Grid Settings
            // ---------------------------------------------------------------------------
            public static void UpdateGridSettings()
            {
                if (gridGameObject != null)
                {
                    // Scale plane and texture to match new grid size
                    float cellCount = Settings.Data.gui.grid.cellCount;
                    gridGameObject.transform.localScale =
                        new Vector3(cellCount * Settings.Data.gui.grid.xzUnitSize/ 5f,
                        1f,
                        cellCount * Settings.Data.gui.grid.xzUnitSize / 5f);
                    gridMaterial.SetTextureScale("_GridTexture", new Vector2(cellCount / 2f, cellCount / 2f));
                    
                    // Update grid color tint
                    gridMaterial.SetColor("_Tint", Settings.Data.gui.grid.tintColor);
                    
                    // Apply updated grid material
                    MeshRenderer gridMeshRenderer = gridGameObject.GetComponent<MeshRenderer>();
                    gridMeshRenderer.sharedMaterial = gridMaterial;
                }
            }
            #endregion
            // ---------------------------------------------------------------------------
        }
    }
}

#endif