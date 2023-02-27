using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Tools
    {
        public static class Mesh_Helper
        {
            // ------------------------------------------------------------------------------------------------
            // Get Unique Material List from MeshRenderer Array
            // ------------------------------------------------------------------------------------------------
            public static List<Material> GetUniqueMaterialListFromMeshRendererArray(MeshRenderer[] sourceMeshRenderers)
            {
                
                // Create a List containing all unique Materials in the GameObjects
                List<Material> uniqueMats = new List<Material>();
                
                bool foundMat;
                
                // Loop through each MeshRenderer
                for (int i = 0; i < sourceMeshRenderers.Length; i++)
                {
                    // Loop through each MeshRenderer's SharedMaterials
                    for (int j = 0; j < sourceMeshRenderers[i].sharedMaterials.Length; j++)
                    {
                        // Set Found Material flag to "False"
                        foundMat = false;
                        
                        // Loop through all Materials in the Unique Material list
                        for (int k = 0; k < uniqueMats.Count; k++)
                        {
                            // If Material was found, set the Found Material flag to "True"
                            if (sourceMeshRenderers[i].sharedMaterials[j].name == uniqueMats[k].name)
                            {
                                foundMat = true;
                            }
                        }
                        
                        // If Found Material flag is "True", add the Material to the Unique Material Array
                        if (!foundMat)
                        {
                            uniqueMats.Add (sourceMeshRenderers[i].sharedMaterials[j]);
                        }
                    }
                }
                
                return uniqueMats;
            }
            
            // ----------------------------------------------------------------------------------------------------
            // Combine Meshes that include multiple Materials
            // ----------------------------------------------------------------------------------------------------
            // In:      List<Material>      matsInCombine           Materials to include in the Combine operation.
            //                                                      Any Submesh using a Material not in this List
            //                                                      will be ignored.
            //
            //          MeshFilter[]        sourceMeshFilters       MeshFilters containing all Meshes to combine
            //
            //          MeshRenderer[]      sourceMeshRenderers     MeshRenderers containing all Mesh Materials.
            //                                                      This will line up with [sourceMeshFilters]. 
            // ----------------------------------------------------------------------------------------------------
            // Out:     GameObject containing all Meshes combined
            // ----------------------------------------------------------------------------------------------------
            private class MeshCombine
            {
                public List<CombineInstance> combineList;
            }
            
            public static GameObject CombineAllMeshesInGameObject(List<Material> matsInCombine, MeshFilter[] sourceMeshFilters, MeshRenderer[] sourceMeshRenderers)
            {
                // ---------------------------------------------------------------------------
                // Extract Meshes into Separate CombineInstances based on Material
                // ---------------------------------------------------------------------------
                
                // Create a MeshCombine Class Array the size of the uniqueMats List and initialize
                MeshCombine[] uniqueMatMeshCombine = new MeshCombine[matsInCombine.Count];
                for (int i = 0; i < matsInCombine.Count; i++)
                {
                    uniqueMatMeshCombine[i] = new MeshCombine();
                    uniqueMatMeshCombine[i].combineList = new List<CombineInstance>();
                }
                
                // Prepare variables
                CombineInstance combineInstance;
                
                // Loop through each MeshRenderer in sourceMeshRenderers
                for (int i = 0; i < sourceMeshRenderers.Length; i++)
                {
                    // Loop through each Material in each MeshRenderer
                    for (int j = 0; j < sourceMeshRenderers[i].sharedMaterials.Length; j++)
                    {
                        // Loop through each Material in the uniqueMats List
                        for (int k = 0; k < matsInCombine.Count; k++)
                        {
                            // If this Material matches the Material in the uniqueMats List
                            if (sourceMeshRenderers[i].sharedMaterials[j] == matsInCombine[k])
                            {
                                // Initialize a Combine Instance
                                combineInstance = new CombineInstance();
                                
                                // Copy this mesh to the Combine Instance
                                combineInstance.mesh = sourceMeshFilters[i].sharedMesh;
                                
                                // Set it to only include the Mesh with the specified material
                                combineInstance.subMeshIndex = j;
                                
                                // Transform to world matrix
                                combineInstance.transform = sourceMeshFilters[i].transform.localToWorldMatrix;
                                
                                // Add this CombineInstance to the appropriate CombineInstance List (by Material)
                                uniqueMatMeshCombine[k].combineList.Add(combineInstance);
                            }
                        }
                    }
                }
                
                // ---------------------------------------------------------------------------
                // Combine all Mesh Instances into a single GameObject
                // ---------------------------------------------------------------------------
                
                // Disable all Source GameObjects
                for (int i = 0; i < sourceMeshFilters.Length; i++)
                {
                    sourceMeshFilters[i].gameObject.SetActive(false);
                }
                
                // Create the final GameObject that will hold all the other GameObjects
                GameObject finalGameObject = new GameObject("Merged Meshes");
                
                // Create a new GameObject Array the size of the All Materials List
                GameObject[] singleMatGameObject = new GameObject[matsInCombine.Count];
                
                // Combine meshes for each singleMatGameObject into one mesh
                CombineInstance[] finalCombineInstance = new CombineInstance[matsInCombine.Count];
                
                // Enable all Source GameObjects
                for (int i = 0; i < sourceMeshFilters.Length; i++)
                {
                    sourceMeshFilters[i].gameObject.SetActive(true);
                }
                
                // Prepare mesh filter and mesh renderer arrays for the final combine
                MeshFilter[] meshFilter = new MeshFilter[matsInCombine.Count];
                MeshRenderer[] meshRenderer = new MeshRenderer[matsInCombine.Count];
                
                // Loop through each Material in the uniqueMats List
                for (int i = 0; i < matsInCombine.Count; i++)
                {
                    // Initialize GameObject
                    singleMatGameObject[i] = new GameObject();
                    
                    singleMatGameObject[i].name = matsInCombine[i].name;
                    
                    // Add a MeshRender and set the Material
                    meshRenderer[i] = singleMatGameObject[i].AddComponent<MeshRenderer>();
                    meshRenderer[i].sharedMaterial = matsInCombine[i];
                    
                    // Add a MeshFilter and add the Combined Meshes with this Material
                    meshFilter[i] = singleMatGameObject[i].AddComponent<MeshFilter>();
                    meshFilter[i].sharedMesh = new Mesh();
                    meshFilter[i].sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                    meshFilter[i].sharedMesh.CombineMeshes(uniqueMatMeshCombine[i].combineList.ToArray());
                    
                    // Weld duplicate vertices
                    //meshFilter[i].sharedMesh = WeldVertices(meshFilter[i].sharedMesh, 0.001f);
                    
                    // Add this Mesh to the final Mesh Combine
                    finalCombineInstance[i].mesh = meshFilter[i].sharedMesh;
                    finalCombineInstance[i].transform = meshFilter[i].transform.localToWorldMatrix;
                    
                    // Hide the GameObject
                    meshFilter[i].gameObject.SetActive(false);
                    
                    GameObject.DestroyImmediate(singleMatGameObject[i]);
                }
                
                // Combine the Mesh, optimize it, and recalculate the bounds
                Mesh mesh = new Mesh();
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                mesh.CombineMeshes(finalCombineInstance, false, false);
                MeshUtility.Optimize(mesh);
                mesh.RecalculateBounds();
                
                // Add MeshFilter to final GameObject and Combine all Meshes
                MeshFilter finalMeshFilter = finalGameObject.AddComponent<MeshFilter>();        
                finalMeshFilter.sharedMesh = new Mesh();
                finalMeshFilter.sharedMesh = mesh;
                
                // Add MeshRenderer to final GameObject Attach Materials
                MeshRenderer finalMeshRenderer = finalGameObject.AddComponent<MeshRenderer>();
                finalMeshRenderer.sharedMaterials = matsInCombine.ToArray();
                
                return finalGameObject;
            }
            
            // ------------------------------------------------------------------------------------------------
            // Weld vertices in mesh ***** Not implemented yet due to bugs! *****
            // ------------------------------------------------------------------------------------------------
            public static Mesh WeldVertices(Mesh aMesh, float aMaxDelta = 0.001f)
            {
                aMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                
                Vector3[] verts = aMesh.vertices;
                Vector3[] normals = aMesh.normals;
                Vector2[] uvs = aMesh.uv;
                List<int> newVerts = new List<int>();
                int[] map = new int[verts.Length];
                
                // Create mapping and filter duplicates
                for(int i = 0; i < verts.Length; i++)
                {
                    Vector3 p = verts[i];
                    Vector3 n = normals[i];
                    Vector2 uv = uvs[i];
                    bool duplicate = false;
                    
                    for(int i2 = 0; i2 < newVerts.Count; i2++)
                    {
                        int a = newVerts[i2];
                        
                        if (
                            (verts[a] - p).sqrMagnitude <= aMaxDelta && // compare position
                            Vector3.Angle(normals[a], n) <= aMaxDelta && // compare normal
                            (uvs[a] - uv).sqrMagnitude <= aMaxDelta // compare first uv coordinate
                            )
                        {
                            map[i] = i2;
                            duplicate = true;
                            break;
                        }
                    }
                    
                    if (!duplicate)
                    {
                        map[i] = newVerts.Count;
                        newVerts.Add(i);
                    }
                }
                
                // Create new vertices
                Vector3[] verts2 = new Vector3[newVerts.Count];
                Vector3[] normals2 = new Vector3[newVerts.Count];
                Vector2[] uvs2 = new Vector2[newVerts.Count];
                
                for(int i = 0; i < newVerts.Count; i++)
                {
                    int a = newVerts[i];
                    verts2[i] = verts[a];
                    normals2[i] = normals[a];
                    uvs2[i] = uvs[a];
                }
                
                // Map the triangle to the new vertices
                var tris = aMesh.triangles;
                
                for(int i = 0; i < tris.Length; i++)
                {
                    tris[i] = map[tris[i]];
                }
                
                aMesh.vertices = verts2;
                aMesh.normals = normals2;
                aMesh.uv = uvs2;
                aMesh.triangles = tris;
                
                return aMesh;
            }
        }
    }
}

#endif