using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Painting
    {
        public static class Painter
        {
            // Does a paint preview exist?
            private static bool previewExists = false;
            
            // Gameobject, meshrenderer, and sharematerial index of current paint preview
            private static GameObject previewGameObject;
            private static int previewGameObjectInstanceID;
            private static MeshRenderer previewMeshRenderer;
            private static int previewSharedMaterialIndex;
            
            // Backup of the gameobject's prefab modifications before the paint preview was applied
            private static Material originalMaterial;
            private static PropertyModification[] prefabModsBackup;
            
            // Is a painting session active?
            private static bool painting = false;
            
            private static bool paintingFirstMaterial = false;
            
            // Index of the under group for each painting session
            private static int undoGroupIndex;
            
            // ---------------------------------------------------------------------------
            // Show a preview of the material on the face under the mouse pointer
            // ---------------------------------------------------------------------------
            public static void PreviewPaint(Material material)
            {
                // Process the mouse pointer raycast
                ProcessMousePointerRaycast();
                
                // If no material was provided and no material was found on the prefab of whatever is hovered "restoring original material"
                if (material == null)
                {
                    // If a gameobject was hit
                    if (previewExists)
                    {
                        // Get material from this gameobject's source prefab
                        material = GetSourcePrefabMaterial();
                        
                        // If gameobject has no source prefab, exit here
                        if (material == null)
                            return;
                    }
                }
                
                // If a paint preview exists
                if (previewExists)
                {
                    // If this material is not already applied to the hovered material
                    if (previewMeshRenderer.sharedMaterials[previewSharedMaterialIndex] != material)
                    {
                        // If painting
                        if (painting)
                        {
                            // Add this gameobject's meshrenderer to the undo group
                            Undo.RegisterCompleteObjectUndo(previewMeshRenderer, "");
                        }
                        
                        // Get all materials on the gameobject, change the hovered material, then apply them back to the gameobject
                        ApplyMaterial(material);
                        
                        // If painting, remove the paintpreviewexists flag, making the change permanent
                        if (painting)
                            previewExists = false;
                    }
                    
                    // If this material is already applied to the hovered material
                    else
                    {
                        // If this is the first material being painted
                        if (paintingFirstMaterial)
                        {
                            // Restore the original material
                            ApplyMaterial(originalMaterial);
                            
                            // Add this gameobject's meshrenderer to the undo group
                            Undo.RegisterCompleteObjectUndo(previewMeshRenderer, "");
                            
                            // Add the painted material preview back
                            ApplyMaterial(material);
                            
                            // Mark as done painting the first material
                            paintingFirstMaterial = false;
                        }
                    }
                }
            }
            
            // ---------------------------------------------------------------------------
            // Start a new painting session
            // ---------------------------------------------------------------------------
            public static void StartPainting()
            {
                // Start painting
                painting = true;
                paintingFirstMaterial = true;
                
                // Start undo group that will contain all material changes
                Undo.IncrementCurrentGroup();
                Undo.SetCurrentGroupName("Painted Material(s)");
                undoGroupIndex = Undo.GetCurrentGroup();
            }
            
            // ---------------------------------------------------------------------------
            // Stop the current painting session
            // ---------------------------------------------------------------------------
            public static void StopPainting()
            {
                Undo.CollapseUndoOperations(undoGroupIndex);
                
                painting = false;
                
                // Make sure the last preview is permanent
                previewExists = false;
            }
            
            private static void ApplyMaterial(Material material)
            {
                Material[] sharedMaterials = previewMeshRenderer.sharedMaterials;
                sharedMaterials[previewSharedMaterialIndex] = material;
                previewMeshRenderer.sharedMaterials = sharedMaterials;
            }
            
            // ---------------------------------------------------------------------------
            // Remove material paint preview from whatever gameobject it's applied to
            // ---------------------------------------------------------------------------
            public static void ClearCurrentMaterialPaintPreview()
            {
                // If not painting and a preview exists, remove the preview by restoring previous material
                if (!painting)
                    if (previewExists)
                    {
                        ApplyMaterial(originalMaterial);
                        previewExists = false;
                    }
            }
            
            // --------------------------------------------------------------------------------
            // Used by the (Restore Material) paint tool:
            // Get material on the source prefab for this gameobject and shared material index
            // --------------------------------------------------------------------------------
            private static Material GetSourcePrefabMaterial()
            {
                // Get the source prefab for this gameobject
                GameObject sourcePrefab = PrefabUtility.GetCorrespondingObjectFromSource(previewGameObject);
                
                // If a source prefab was found
                if (sourcePrefab != null)
                {
                    // Return the material at the corresponding sharematerial index
                    MeshRenderer sourceMeshRenderer = sourcePrefab.GetComponent<MeshRenderer>();
                    Material[] sharedMaterials = sourceMeshRenderer.sharedMaterials;
                    return sharedMaterials[previewSharedMaterialIndex];
                }
                
                // If not source prefab was found, return null
                else
                    return null;
            }
            
            // ---------------------------------------------------------------------------
            // Raycast to the mouse pointer to find and process the material hit
            // ---------------------------------------------------------------------------
            private static void ProcessMousePointerRaycast()
            {
                // Create a ray starting from the current point the mouse is
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hit;
                
                // If raycast to mouse pointer 
                if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
                {
                    // Get hit gameobject
                    GameObject hitGameObject = hit.transform.gameObject;
                    
                    // If this is the MAST grid, remove the last material paint preview and exit
                    if (hitGameObject.name == MAST.Const.Grid.defaultName)
                    {
                        ClearCurrentMaterialPaintPreview();
                        return;
                    }
                    
                    // Get the mesh of the hit gameobject
                    Mesh mesh = GetMesh(hitGameObject);
                    
                    // If a mesh was found on this GameObject
                    if (mesh != null)
                    {
                        // Get int[3] triangle index array from the hit face
                        int[] hittedTriangle = new int[] 
                        {
                            mesh.triangles[hit.triangleIndex * 3], 
                            mesh.triangles[hit.triangleIndex * 3 + 1], 
                            mesh.triangles[hit.triangleIndex * 3 + 2] 
                        };
                        
                        // Loop through all submeshes
                        for (int i = 0; i < mesh.subMeshCount; i++)
                        {
                            // Get triangle from mesh at this index
                            int[] subMeshTris = mesh.GetTriangles(i);
                            
                            // Loop through all triangles in the submesh
                            for (int j = 0; j < subMeshTris.Length; j += 3)
                            {
                                // If this triangle matches the hit triangle
                                if (subMeshTris[j] == hittedTriangle[0])
                                    if (subMeshTris[j + 1] == hittedTriangle[1])
                                        if (subMeshTris[j + 2] == hittedTriangle[2])
                                        {
                                            // If a new material or gameobject is hovered, or no paint preview already exists
                                            if ((previewSharedMaterialIndex != i || previewGameObjectInstanceID != hitGameObject.GetInstanceID())
                                                || !previewExists)
                                            {
                                                // Clear the current material paint preview, so it's not permanently applied
                                                ClearCurrentMaterialPaintPreview();
                                                
                                                // Prep the new material paint preview
                                                PrepNextPaintPreview(hitGameObject, i);
                                            }
                                        }
                            }
                        }
                    }
                }
                
                // If nothing was hit, remove the last material paint preview and exit
                else
                {
                    ClearCurrentMaterialPaintPreview();
                }
            }
            
            // Set the next gameobject and shared material index to apply the current material to
            private static void PrepNextPaintPreview(GameObject gameObject, int sharedMaterialIndex)
            {
                // If this material is already being previewed, exit before saving it again
                if (previewMeshRenderer == gameObject.GetComponent<MeshRenderer>())
                    if (previewSharedMaterialIndex == sharedMaterialIndex)
                    {
                        previewExists = false;
                        return;
                    }
                
                
                // Save this gameobject, gameobject instance id, meshrenderer, and sharedmaterial index
                previewGameObject = gameObject;
                previewGameObjectInstanceID = gameObject.GetInstanceID();
                previewMeshRenderer = previewGameObject.GetComponent<MeshRenderer>();
                previewSharedMaterialIndex = sharedMaterialIndex;
                
                // Backup current material
                originalMaterial = previewMeshRenderer.sharedMaterials[previewSharedMaterialIndex];
                
                previewExists = true;
            }
            
            // Safely get mesh from the selected GameObject
            private static Mesh GetMesh(GameObject testGameObject)
            {
                // If gameobject exists
                if (testGameObject != null)
                {
                    // Get meshfilter for gameobject
                    MeshFilter meshFilter = testGameObject.GetComponent<MeshFilter>();
                    
                    // If meshfilter exists
                    if (meshFilter != null)
                    {
                        // Get sharedmesh
                        Mesh mesh = meshFilter.sharedMesh;
                        
                        // If sharedmesh not found, try to get as regular mesh
                        if (!mesh) { mesh = meshFilter.mesh; }
                        
                        // If mesh exists
                        if (mesh != null)
                        {
                            // Return with mesh
                            return mesh;
                        }
                    }
                }
                
                // Return null
                return (Mesh)null;
            }
        }
    }
}
#endif