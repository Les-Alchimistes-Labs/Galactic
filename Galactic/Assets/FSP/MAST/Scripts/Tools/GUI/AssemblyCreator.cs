using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Tools
    {
        namespace GUI
        {
            public class AssemblyCreator : EditorWindow
            {
                [SerializeField] private GUISkin guiSkin;
                
                [SerializeField] private GameObject[] selection;
                [SerializeField] private string[] selectionNames;
                
                [SerializeField] Vector2 selScrollPos;
                [SerializeField] int selSelection, newSelSelection;
                
                [SerializeField] string savePath = "";
                
                [SerializeField] string prefabName = "New Assembly";
                
                public void OnInspectorUpdate()
                {
                    // This will only get called 10 times per second.
                    Repaint();
                }
                
                void OnGUI()
                {
                    // Load custom gui styles
                    if (guiSkin == null)
                        guiSkin = MAST.LoadingHelper.GetGUISkin();
                    
                    UnityEngine.GUI.skin = guiSkin;
                    
                    GUILayout.BeginVertical("MAST Toolbar BG");  // Begin entire window vertical layout
                    
                    // Step 1:  Select all objects in the scene/hierarchy to include in this new prefab, then click "Add Selected Items to Prefab"
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    
                    GUILayout.Space(5f);
                    EditorGUILayout.LabelField("Step 1:  Choose items to include in Assembly", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Select all objects in the scene/hierarchy to include in this new Assembly (prefab)");
                    GUILayout.Space(5f);
                    
                    // Add Selected Items to Prefab button
                    if (GUILayout.Button(new GUIContent("Add Selected Items to Assembly", "Add Selected Items to Assembly")))
                    {
                        // If at least 1 GameObject is selected
                        if (Selection.activeGameObject)
                        {
                            // Get all GameObjects in selection
                            GameObject[] unfilteredSelection = Selection.gameObjects;
                            
                            //----------------------------------------------------------------------------------
                            // If a Parent and Child of that Parent are both selected, don't include the Child
                            //----------------------------------------------------------------------------------
                            bool[] ignoreChild = new bool[unfilteredSelection.Length];
                            
                            // Loop through all GameObjects
                            for (int i = 0; i < unfilteredSelection.Length; i++)
                            {
                                for (int j = 0; j < unfilteredSelection.Length; j++)
                                {
                                    // If this GameObject is the parent of another GameObject in the selection, tag the child to be ignored "cruel I know!"
                                    if (unfilteredSelection[i].transform == unfilteredSelection[j].transform.parent)
                                        ignoreChild[j] = true;
                                }
                            }
                            
                            // Create a List containing all parent GameObjects in selection
                            List<GameObject> filteredSelection = new List<GameObject>();
                            
                            // Loop through all GameObjects
                            for (int i = 0; i < unfilteredSelection.Length; i++)
                            {
                                // If not ignoring child, add the the filtered selection List
                                if (!ignoreChild[i])
                                    filteredSelection.Add(unfilteredSelection[i]);
                            }
                            
                            // Save filtered GameObject list to array and get names as an array for the SelectionGrid
                            selection = filteredSelection.ToArray();
                            selectionNames = new string[selection.Length];
                            for (int i = 0; i < selection.Length; i++)
                                selectionNames[i] = selection[i].name;
                            
                            // Select first item in the SelectionGrid and the Scene
                            selSelection = 0;
                            Selection.activeGameObject = selection[0];
                        }
                    }
                    
                    EditorGUILayout.EndVertical();
                    
                    GUILayout.Space(5f);
                    
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    
                    GUILayout.Space(5f);
                    EditorGUILayout.LabelField("Step 2:  Choose GameObject to use as the Anchor", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("In the final Assembly, this will be moved to 0,0,0");
                    
                    GUILayout.Space(5f);
                    
                    // Begin Selection GameObjects scrollview
                    selScrollPos = EditorGUILayout.BeginScrollView(selScrollPos);
                    
                    // If a GameObject selection has been completed
                    if (selection != null)
                    {
                        // Loop through all items in the GameObject selection
                        for (int i = 0; i < selection.Length; i++)
                        {
                            // If the user selects a GameObject in the scene and it's in the GameObject selection, select it in the SelectionGrid
                            if (Selection.activeGameObject == selection[i])
                                selSelection = i;
                        }
                        
                        // Draw the SelectionGrid
                        newSelSelection = GUILayout.SelectionGrid(selSelection, selectionNames, 1);
                        
                        // If the selected item in the SelectionGrid has changed
                        if (newSelSelection != selSelection)
                        {
                            // Save the selection change and select the GameObject in the Hierarchy and Scene
                            selSelection = newSelSelection;
                            Selection.activeGameObject = selection[selSelection];
                            
                        }
                    }
                    
                    EditorGUILayout.EndScrollView();
                    
                    GUILayout.Space(5f);
                    
                    EditorGUILayout.EndVertical();
                    
                    GUILayout.Space(5f);
                    
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    
                    GUILayout.Space(5f);
                    
                    EditorGUILayout.LabelField("Step 3:  Choose Destination Folder for Created Assembly", EditorStyles.boldLabel);
                    //EditorGUILayout.LabelField("");
                    
                    GUILayout.Space(5f);
                    
                    // Select model folder button
                    if (GUILayout.Button(new GUIContent("Select Destination Folder",
                        "Choose which Folder to Save the Assembly to.")))
                    {
                        // Show choose folder dialog
                        savePath = EditorUtility.OpenFolderPanel("Choose which Folder to Save the Assembly to",
                            Application.dataPath, "");
                        
                        // If a folder was chosen (Cancel was not clicked) convert to project local path "Assets/..."
                        if (savePath != "")
                        {
                            savePath = savePath.Replace(Application.dataPath, "Assets");
                        }
                    }
                    
                    GUILayout.Space(5f);
                    
                    // Display selected path
                    EditorGUILayout.LabelField("Save Path: [" + savePath + "]", EditorStyles.wordWrappedLabel);
                    
                    GUILayout.Space(5f);
                    EditorGUILayout.EndVertical();
                    
                    GUILayout.Space(5f);
                    
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    
                    GUILayout.Space(5f);
                    
                    EditorGUILayout.LabelField("Step 4:  Choose Name for Assembly", EditorStyles.boldLabel);
                    GUILayout.Space(5f);
                    
                    prefabName = EditorGUILayout.TextField("Assembly Name", prefabName);
                    
                    GUILayout.Space(5f);
                    EditorGUILayout.EndVertical();
                    
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    
                    GUILayout.Space(5f);
                    
                    // Disable final button if nothing has been selected or save path was never chosen
                    if (savePath =="" || selection == null)
                        UnityEngine.GUI.enabled = false;
                    
                    // Add Selected Items to Prefab button
                    if (GUILayout.Button(new GUIContent("Create Final Assembly", "Create Final Assembly")))
                    {
                        // Create new empty Prefab parent
                        GameObject newPrefab = new GameObject();
                        newPrefab.name = prefabName;
                        
                        GameObject[] newPrefabChildren = new GameObject[selection.Length];
                        
                        // Get the Position offset for moving all the final GameObjects
                        Vector3 positionOffset = selection[selSelection].transform.position;
                        
                        for (int i = 0; i < selection.Length; i++)
                        {
                            // Get the source Prefab for this selected GameObject
                            Object prefab = PrefabUtility.GetCorrespondingObjectFromSource(selection[i]);
                            
                            // If the selected GameObject is a Prefab
                            if (prefab != null)
                            {
                                // Copy any modifications made to the Prefab
                                PropertyModification[] prefabMods = PrefabUtility.GetPropertyModifications(selection[i]);
                                
                                // Instantiate a new GameObject directly from the prefab
                                newPrefabChildren[i] = (GameObject) PrefabUtility.InstantiatePrefab(prefab);
                                
                                // Copy Prefabs to the new GameObject
                                PrefabUtility.SetPropertyModifications(newPrefabChildren[i], prefabMods);
                            }
                            
                            // If the selected GameObject isn't a Prefab
                            else
                            {
                                // May not do anything.  Possibly rename the entire thing to Combine Prefabs
                                
                            }
                            
                            // Move GameObject relative to the Anchor
                            newPrefabChildren[i].transform.position -= positionOffset;
                            
                            // Make this GameObject a child of the new Prefab GameObject
                            newPrefabChildren[i].transform.parent = newPrefab.transform;
                        }
                        
                        // Save the Prefab
                        PrefabUtility.SaveAsPrefabAsset(newPrefab, savePath + "/" + prefabName + ".prefab");
                        
                        // Destroy the temporary copy of the Prefab from the scene
                        GameObject.DestroyImmediate(newPrefab);
                        
                    }
                    
                    UnityEngine.GUI.enabled = true;
                    
                    GUILayout.Space(5f);
                    EditorGUILayout.EndVertical();
                    
                    EditorGUILayout.EndVertical();
                    
                }
            }
        }
    }
}
#endif