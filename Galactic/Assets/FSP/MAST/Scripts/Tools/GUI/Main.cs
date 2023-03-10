using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Tools
    {
        namespace GUI
        {
            public static class Main
            {  
                [SerializeField] private static MAST.Tools.CombineMeshes MergeMeshesClass;
                private static MAST.Tools.CombineMeshes MergeMeshes
                {
                    get
                    {
                        // Initialize MergeMeshes Class if needed and return MergeMeshesClass
                        if(MergeMeshesClass == null)
                            MergeMeshesClass = new MAST.Tools.CombineMeshes();
                        
                        return MergeMeshesClass;
                    }
                }
                
                [SerializeField] private static MAST.Tools.GUI.PrefabCreator PrefabCreator;
                
                [SerializeField] private static MAST.Tools.GUI.AssemblyCreator AssemblyCreator;
                
                [SerializeField] private static Vector2 scrollPos;
                
                // ---------------------------------------------------------------------------
                #region Preferences Interface
                // ---------------------------------------------------------------------------
                public static void DisplayToolsGUI()
                {
                    GUILayout.BeginVertical("MAST Toolbar BG");  // Begin entire window vertical layout
                    
                    // Verical scroll view for palette items
                    scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                    
                    // ------------------------------------
                    // Open PrefabCreator Window Button
                    // ------------------------------------
                    GUILayout.Space(5f);
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    
                    EditorGUILayout.LabelField("Generate Prefabs from your own models.  Substitute and consolidate materials used during the process.", EditorStyles.wordWrappedLabel);
                    
                    if (GUILayout.Button(new GUIContent("Open Prefab Creator window", "Open Prefab Creator window")))
                    {
                        // If PrefabCreator window is closed, show and initialize it
                        if (PrefabCreator == null)
                        {
                            PrefabCreator = (MAST.Tools.GUI.PrefabCreator)EditorWindow.GetWindow(
                                typeof(MAST.Tools.GUI.PrefabCreator),
                                false, "MAST Prefab Creator");
                            
                            
                            PrefabCreator.minSize = new Vector2(800, 250);
                        }
                        
                        // If PrefabCreator window is open, close it
                        else
                        {
                            EditorWindow.GetWindow(typeof(MAST.Tools.GUI.PrefabCreator)).Close();
                        }
                    }
                    
                    GUILayout.EndVertical();
                    GUILayout.Space(5f);
                    
                    // ----------------------------------
                    // Add MAST Script to Prefabs
                    // ----------------------------------
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    
                    EditorGUILayout.LabelField("This will add a MAST script to each prefab.  The script is used to describe the type of object to the MAST editor.", EditorStyles.wordWrappedLabel);
                    
                    if (GUILayout.Button(new GUIContent("Add MAST Script to Prefabs",
                        "Create Prefabs from all models in the selected folder.")))
                    {
                        // Show choose folder dialog
                        string chosenPath = EditorUtility.OpenFolderPanel("Choose the Folder that Contains your Prefabs",
                            MAST.LoadingHelper.ConvertProjectPathToAbsolutePath(MAST.GUI.DataManager.state.prefabPath), "");
                        
                        // If a folder was chosen "Cancel was not clicked"
                        if (chosenPath != "")
                        {
                            // Convert to project local path "Assets/..."
                            chosenPath = MAST.LoadingHelper.ConvertAbsolutePathToProjectPath(chosenPath);
                            
                            // Loop through each Prefab in folder
                            foreach (GameObject prefab in MAST.LoadingHelper.GetPrefabsInFolder(chosenPath))
                            {
                                // Add MAST Prefab script if not already added
                                if (!prefab.GetComponent<MAST.Component.MASTPrefabSettings>())
                                    prefab.AddComponent<MAST.Component.MASTPrefabSettings>();
                            }
                        }
                    }
                    
                    GUILayout.EndVertical();
                    GUILayout.Space(5f);
                    
                    // ----------------------------------------
                    // Create Assembly Button
                    // ----------------------------------------
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    
                    EditorGUILayout.LabelField("Create a Prefab (Assembly) from the current scene selection.", EditorStyles.wordWrappedLabel);
                    
                    if (GUILayout.Button(new GUIContent("Open Create Assembly Window",
                        "Create a Prefab from the selection.  The final prefab will be moved to and anchored at 0,0,0")))
                    {
                        // If SelectionToPrefab window is closed, show and initialize it
                        if (AssemblyCreator == null)
                        {
                            AssemblyCreator = (MAST.Tools.GUI.AssemblyCreator)EditorWindow.GetWindow(
                                typeof(MAST.Tools.GUI.AssemblyCreator),
                                false, "MAST Assembly Creator");
                            
                            
                            AssemblyCreator.minSize = new Vector2(400, 400);
                        }
                        
                        // If SelectionToPrefab window is open, close it
                        else
                        {
                            EditorWindow.GetWindow(typeof(MAST.Tools.GUI.AssemblyCreator)).Close();
                        }
                    }
                    
                    GUILayout.EndVertical();
                    GUILayout.Space(5f);
                    
                    // ----------------------------------
                    // Remove MAST Components Button
                    // ----------------------------------
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    
                    EditorGUILayout.LabelField("Remove all MAST Components that were attached to the children of the selected GameObject during placement.", EditorStyles.wordWrappedLabel);
                    
                    if (GUILayout.Button(new GUIContent("Remove MAST Components",
                        "Remove any MAST Component code attached to gameobjects during placement")))
                    {
                        if (EditorUtility.DisplayDialog("Are you sure?",
                            "This will remove all MAST components attached to '" + Selection.activeGameObject.name + "'",
                            "Remove MAST Components", "Cancel"))
                        {
                            // Loop through all top-level children of targetParent
                            foreach (MAST.Component.MASTPrefabSettings prefabComponent
                                in Selection.activeGameObject.transform.GetComponentsInChildren<MAST.Component.MASTPrefabSettings>())
                            {
                                // Remove the SMACK_Prefab_Component script
                                GameObject.DestroyImmediate(prefabComponent);
                            }
                        }
                    }
                    
                    GUILayout.EndVertical();
                    GUILayout.Space(5f);
                    
                    // ----------------------------------
                    // Merge Meshes by Material Button
                    // ----------------------------------
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    
                    EditorGUILayout.LabelField("Merge all meshes in the selected GameObject, and place them in a new GameObject.", EditorStyles.wordWrappedLabel);
                    
                    if (GUILayout.Button(new GUIContent("Merge Meshes",
                        "Merge all meshes by material name, resulting in one mesh for each material")))
                    {
                        if (EditorUtility.DisplayDialog("Are you sure?",
                            "This will combine all meshes in '" + Selection.activeGameObject.name +
                            "' and save them to a new GameObject.  The original GameObject will not be affected.",
                            "Merge Meshes", "Cancel"))
                        {
                            
                            GameObject targetParent = MergeMeshes.MergeMeshes(Selection.activeGameObject);
                            targetParent.name = Selection.activeGameObject.name + "_Merged";
                        }
                    }
                    
                    GUILayout.EndVertical();
                    GUILayout.Space(5f);
                    
                    EditorGUILayout.EndScrollView();
                    
                    GUILayout.EndVertical();
                }
                #endregion
                // ---------------------------------------------------------------------------
            }
        }
    }
}
#endif