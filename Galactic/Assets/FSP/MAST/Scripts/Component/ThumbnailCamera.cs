using UnityEngine;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Component
    {
        [ExecuteInEditMode]
        public class ThumbnailCamera : MonoBehaviour
        {
            
            const int prefabLayer = 2;
            
            [SerializeField] private Camera snapshotCamera;
            
            // ---------------------------------------------------------------------------
            // Get thumbnail array from Prefabs
            // ---------------------------------------------------------------------------
            public Texture2D[] GetPrefabThumbnails(GameObject[] prefabs)
            {
                // Set orientation for entire camera rig based on pitch and yaw
                gameObject.transform.eulerAngles = new Vector3(
                    MAST.Settings.Data.gui.palette.snapshotCameraYaw,
                    MAST.Settings.Data.gui.palette.snapshotCameraPitch,
                    0f);
                
                // Initialize thumbnail array
                Texture2D[] thumbnails = new Texture2D[prefabs.Length];
                
                // Prep camera
                if (snapshotCamera == null)
                    snapshotCamera = gameObject.GetComponentInChildren<Camera>();
                
                snapshotCamera.clearFlags = CameraClearFlags.SolidColor;
                snapshotCamera.backgroundColor = new Color(0f, 0f, 0f, 0.25f);
                
                snapshotCamera.cullingMask = 1 << prefabLayer;
                
                // Take a snapshot of each prefab and add to thumbnail array
                for (int i = 0; i < prefabs.Length; i++)
                {
                    // Instantiate prefab
                    GameObject subject = GameObject.Instantiate(prefabs[i]);
                    
                    thumbnails[i] = TakeSnapshot(subject);
                    
                    GameObject.DestroyImmediate(subject);
                }
                
                // Return thumbnail array
                return thumbnails;
            }
            
            // ---------------------------------------------------------------------------
            // Get thumbnail array from Materials
            // ---------------------------------------------------------------------------
            public Texture2D[] GetMaterialThumbnails(Material[] materials)
            {
                // Set orientation for entire camera rig based on pitch and yaw
                gameObject.transform.eulerAngles = new Vector3(
                    MAST.Settings.Data.gui.palette.snapshotCameraYaw,
                    MAST.Settings.Data.gui.palette.snapshotCameraPitch,
                    0f);
                
                // Initialize thumbnail array
                Texture2D[] thumbnails = new Texture2D[materials.Length];
                
                // Prep camera
                if (snapshotCamera == null)
                    snapshotCamera = gameObject.GetComponentInChildren<Camera>();
                
                snapshotCamera.clearFlags = CameraClearFlags.SolidColor;
                snapshotCamera.backgroundColor = new Color(0f, 0f, 0f, 0.25f);
                
                snapshotCamera.cullingMask = 1 << prefabLayer;
                
                // Instantiate prefab
                GameObject subject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                
                // Take a snapshot of each material and add to thumbnail array
                for (int i = 0; i < materials.Length; i++)
                {
                    subject.GetComponent<MeshRenderer>().sharedMaterial = materials[i];
                    
                    thumbnails[i] = TakeSnapshot(subject);
                }
                
                GameObject.DestroyImmediate(subject);
                
                // Return thumbnail array
                return thumbnails;
            }
            
            // ---------------------------------------------------------------------------
            // Instantiate gameobject and take a snapshot - returned as a Texture2D
            // ---------------------------------------------------------------------------
            private Texture2D TakeSnapshot(GameObject subject)
            {
                // Set prefab parent and child layers
                SetLayerRecursively(subject.GetComponent<Transform>(), prefabLayer);
                
                // ----------------------------------
                // Calculate Prefab Bounds
                // ----------------------------------
                Bounds bounds;
                Renderer[] renderers = subject.GetComponentsInChildren<Renderer>();
                
                // If at least 1 mesh renderer was found
                if (renderers.Length > 0)
                {
                    // Get the bounds of the first mesh renderer
                    bounds = renderers[0].bounds;
                    
                    // Encapsulate "enlarge" the bounds to include all mesh renderers found 
                    for (int i = 1; i < renderers.Length; ++i)
                        bounds.Encapsulate(renderers[i].bounds);
                }
                
                // If no mesh renderers are found!?
                else
                {
                    // Create a fake bounds at world center
                    bounds = new Bounds(new Vector3(0f, 0f, 0f), Vector3.zero);
                }
                
                // ----------------------------------
                // Fit Camera to GameObject
                // ----------------------------------
                float cameraDistance = 2.5f; // Constant factor
                Vector3 objectSizes = bounds.max - bounds.min;
                float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
                float cameraView = 3.5f * Mathf.Tan(0.5f * Mathf.Deg2Rad * snapshotCamera.fieldOfView); // Visible height 1 meter in front
                float distance = cameraDistance * objectSize / cameraView; // Combined wanted distance from the object
                distance += 0.5f * objectSize; // Estimated offset from the center to the outside of the object
                snapshotCamera.transform.position = bounds.center - distance * snapshotCamera.transform.forward;
                
                // ----------------------------------
                // Take snapshot of GameObject
                // ----------------------------------
                var renderTarget = RenderTexture.GetTemporary(128, 128);
                
                snapshotCamera.targetTexture = renderTarget;
                snapshotCamera.Render();
                
                RenderTexture.active = renderTarget;
                Texture2D snapshot = new Texture2D(renderTarget.width, renderTarget.height, TextureFormat.RGBA32, false);
                snapshot.ReadPixels(new Rect(0, 0, renderTarget.width, renderTarget.height), 0, 0);
                
                // Destroy instantiated prefab
                //Object.DestroyImmediate(subject);
                
                snapshot.Apply();
                
                RenderTexture.active = null;
                snapshotCamera.targetTexture = null;
                
                return snapshot;
            }
            
            // ---------------------------------------------------------------------------
            // Recursively loop through all children and set their layers
            // ---------------------------------------------------------------------------
            private void SetLayerRecursively(Transform transform, int layer)
            {
                transform.gameObject.layer = layer;
                
                foreach (Transform childTransform in transform)
                    SetLayerRecursively(childTransform, layer);
            }
            
        }
    }
}
#endif