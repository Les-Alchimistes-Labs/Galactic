using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Tools
    {
        namespace GUI
        {
            public class PrefabCreator : EditorWindow
            {
                [SerializeField] private GUISkin guiSkin;
                
                [SerializeField] private MAST.Tools.Prefab_Creator PrepModelsClass;
                private MAST.Tools.Prefab_Creator PrepModels
                {
                    get
                    {
                        // Initialize Prep Models Class if needed and return PrepModelsClass
                        if(PrepModelsClass == null)
                            PrepModelsClass = new MAST.Tools.Prefab_Creator();
                        
                        return PrepModelsClass;
                    }
                }
                
                // ---------------------------------------------------------------------------
                // Overall window settings
                // ---------------------------------------------------------------------------
                [SerializeField] private int processStep = 0;           // Current step of the interface (0-4)
                [SerializeField] private float horizontalWidth;         // Width of the PrepModels window - adjusted for border
                
                // ---------------------------------------------------------------------------
                // Select Model Folder Section
                // ---------------------------------------------------------------------------
                [SerializeField] private string modelPath = "";         // Path containing models
                [SerializeField] private Vector2 modelScrollPos;        // Scroll position for Consolidate Mats Selected Mats selection grid
                [SerializeField] private string[] modelSelGridData;     // Used for displaying the Found Mats selection grid.
                
                // ---------------------------------------------------------------------------
                // Find Materials Section
                // ---------------------------------------------------------------------------
                
                [SerializeField] private List<Material> sourceMat;      // A unique list containing the all materials found in the models
                [SerializeField] private Vector2 findMatScrollPos;      // Scroll position for Find Mats selection grid
                [SerializeField] private string[] findMatSelGridData;   // Used for displaying the Found Mats selection grid.
                
                // ---------------------------------------------------------------------------
                // Organize Materials Section
                // ---------------------------------------------------------------------------
                
                [SerializeField] private string[] sourceMatName;        // A unique array containing only the names of the original materials
                [SerializeField] private string[] sourceMatNewName;     // An array containing the new names of the original materials
                
                [SerializeField] private Vector2 orgMatScrollPos;       // Scroll position for Organize Mats selection grid
                [SerializeField] private string[] orgMatSelGridData;    // Used for displaying the Organize Mats selection grid.  (sourceMatName > sourceMatNewName)
                [SerializeField] private int orgMatSelection;           // Currently highlighted item in Organize Mats selection grid
                
                [SerializeField] private Vector2 selOrgMatScrollPos;    // Scroll position for Organize Mats Selected Mats selection grid
                [SerializeField] private int[] selOrgMatPointer;        // Contains an array of indexes use to point to specific data in sourceMatNewName
                [SerializeField] private string[] selOrgMatGridData;    // Used for displaying the Organize Mats Selected Mats selection grid.  (sourceMatName > sourceMatNewName)
                [SerializeField] private int selOrgMatSelection;        // Currently highlighted item in Organize Mats Selected Mats selection grid
                
                [SerializeField] private string newMaterialName;        // New name for materials in Organize Mats Selected Mats "Selection Grid"
                
                // ---------------------------------------------------------------------------
                // Link Materials Section
                // ---------------------------------------------------------------------------
                [SerializeField] private Vector2 conMatScrollPos;       // Scroll position for Consolidate Mats Selected Mats selection grid
                
                [SerializeField] private int[] conMatPointer;           // Points from original to consolidated material array
                
                [SerializeField] private Material[] conMat;             // Consolidated material array
                [SerializeField] private string[] conMatName;           // Consolidated material name array
                
                [SerializeField] private string[] conMatPath;           // Material path array
                [SerializeField] private string[] linkMatSelGridData;   // Link material Selection Grid display data
                [SerializeField] private int linkMatSelection;          // Highlighted item in Link material "Selection Grid"
                
                [SerializeField] private string lastFolderSelected;     // Returns to the last folder selected for Substitute Materials
                
                // ---------------------------------------------------------------------------
                // Create Prefabs Section
                // ---------------------------------------------------------------------------
                [SerializeField] private bool flagAddMeshCollider = true;
                [SerializeField] private bool flagAddEmptyParent = true;
                [SerializeField] private MAST.Tools.Prefab_Creator.MergeMethod mergeMethod = MAST.Tools.Prefab_Creator.MergeMethod.MergeChildren;
                //[SerializeField] private bool flagPreserveModelHierarchy = true;
                
                void OnFocus() {}
                
                void OnDestroy() {}
                
                void OnGUI()
                {
                    // Load custom gui styles
                    if (guiSkin == null)
                        guiSkin = MAST.LoadingHelper.GetGUISkin();
                    
                    UnityEngine.GUI.skin = guiSkin;
                    
                    GUILayout.BeginVertical("MAST Toolbar BG");  // Begin entire window vertical layout
                    
                    // --------------------------------------------------------------------
                    // Top Toolbar Section
                    // --------------------------------------------------------------------
                    EditorGUILayout.BeginHorizontal();
                    
                    GUIToolBar();
                    
                    EditorGUILayout.EndHorizontal();
                    
                    // Get window width - used for sizing Selection Grids
                    float testWidth = GUILayoutUtility.GetLastRect().width;
                    
                    if (testWidth > 30)
                        horizontalWidth = testWidth - 58;
                    
                    // --------------------------------------------------------------------
                    // Main Section
                    // --------------------------------------------------------------------
                    
                    // Draw GUI depending on step
                    switch (processStep)
                    {
                        case 0:  // Select Models Section
                            GUISelectModelFolder();
                            break;
                            
                        case 1:  // Organize Materials Section
                            GUIOrganizeMaterials();
                            break;
                            
                        case 2:  // Link Materials Section
                            GUILinkMaterials();
                            break;
                            
                        case 3:  // Create Prefabs Section
                            GUICreatePrefabs();
                            break;
                    }
                    
                    EditorGUILayout.EndVertical();
                    
                }
                
                // ---------------------------------------------------------------------------
                // Toolbar
                // ---------------------------------------------------------------------------
                private void GUIToolBar()
                {
                    if (GUILayout.Button(new GUIContent("Step 1:  Find Models", "Return to Step 1")))
                        processStep = 0;
                        
                    GUILayout.Space(5f);
                    
                    UnityEngine.GUI.enabled = (processStep > 0);
                    if (GUILayout.Button(new GUIContent("Step 2:  Organize Materials", "Return to Step 3")))
                        processStep = 1;
                    
                    GUILayout.Space(5f);
                    
                    UnityEngine.GUI.enabled = (processStep > 1);
                    if (GUILayout.Button(new GUIContent("Step 3:  Link Materials", "Return to Step 4")))
                        processStep = 2;
                    
                    GUILayout.Space(5f);
                    
                    UnityEngine.GUI.enabled = (processStep > 2);
                    if (GUILayout.Button(new GUIContent("Step 4:  Create Prefabs", "")))
                        processStep = 3;
                    
                    UnityEngine.GUI.enabled = true;
                }
                
                // ---------------------------------------------------------------------------
                // Choose Model Folder
                // ---------------------------------------------------------------------------
                private void GUISelectModelFolder()
                {
                    // -------------------------------------
                    // Section header
                    // -------------------------------------
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    GUILayout.Space(5f);
                    EditorGUILayout.LabelField("Step 1:  Find Models", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Choose folder containing models");
                    GUILayout.Space(5f);
                    EditorGUILayout.EndVertical();
                    
                    GUILayout.Space(5f);
                    
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    GUILayout.Space(5f);
                    
                    // Select model folder button
                    if (GUILayout.Button(new GUIContent("Select Model Folder",
                        "Choose the Folder that Contains your Models to Convert.")))
                    {
                        // Show choose folder dialog
                        modelPath = EditorUtility.OpenFolderPanel("Choose the Folder that Contains your Models to Convert",
                            Application.dataPath, "");
                        
                        // If a folder was chosen (Cancel was not clicked) convert to project local path "Assets/..."
                        if (modelPath != "")
                        {
                            modelPath = modelPath.Replace(Application.dataPath, "Assets");
                            
                            string[] modelPaths = PrepModels.GetPathOfModelsInFolder(modelPath);
                            
                            modelSelGridData = new string[modelPaths.Length];
                            
                            for (int i = 0; i < modelPaths.Length; i++)
                            {
                                modelSelGridData[i] = modelPaths[i].Replace(modelPath + "/", "");
                            }
                        }
                    }
                    
                    GUILayout.Space(5f);
                    
                    // Display selected path
                    EditorGUILayout.LabelField("Selected Path: [" + modelPath + "]", EditorStyles.wordWrappedLabel);
                    
                    GUILayout.Space(5f);
                    EditorGUILayout.EndVertical();
                    
                    GUILayout.Space(5f);
                    
                    // -------------------------------------
                    // List - Models in folder
                    // -------------------------------------
                    
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    GUILayout.Space(5f);
                    
                    EditorGUILayout.LabelField("Models Found");
                    
                    if (modelSelGridData != null)
                    {
                        modelScrollPos = EditorGUILayout.BeginScrollView(modelScrollPos);
                        
                        // Display a Selection grid of all Source Material Names
                        GUILayout.SelectionGrid(-1, modelSelGridData, 1);
                        
                        EditorGUILayout.EndScrollView();
                    }
                    
                    GUILayout.Space(5f);
                    EditorGUILayout.EndVertical();
                    
                    GUILayout.Space(5f);
                    
                    GUILayout.FlexibleSpace();
                    
                    // -------------------------------------
                    // Section footer
                    // -------------------------------------
                    
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    GUILayout.Space(5f);
                    
                    // Done with Substituting Materials button
                    if (GUILayout.Button(new GUIContent("Done with Selecting Folder", "Go to next step")))
                    {
                        // Get materials from all models in the specified path
                        sourceMat = PrepModels.StripMaterials(modelPath);
                        
                        // If at least 1 material was found
                        if (sourceMat != null)
                        {
                            // Initialize the string arrays used to populate the Organize Material listbox
                            sourceMatName = new string[sourceMat.Count];
                            sourceMatNewName = new string[sourceMat.Count];
                            
                            // Populate the arrays used in the Organize Materials step
                            for (int i = 0; i < sourceMat.Count; i++)
                            {
                                // Populate the source and renamed material name arrays with the names of each material
                                sourceMatName[i] = sourceMat[i].name;
                                sourceMatNewName[i] = sourceMatName[i];
                            }
                            
                            // Create a new array containing the source and renamed material name combined for each item
                            orgMatSelGridData = CombineStringsWithinArrays(sourceMatName, sourceMatNewName, " > ");
                            
                            // Initialize the selection variables for organize materials
                            orgMatSelection = -1;
                            selOrgMatSelection = -1;
                            selOrgMatPointer = new int[0];
                            selOrgMatGridData = new string[0];
                            
                            // GoTo next step
                            processStep = 1;
                            
                        }
                        
                        // If no materials were found in the models, display an error popup
                        else
                        {
                            EditorUtility.DisplayDialog("No Materials found!", "The Prefab Creator requires materials to be attached to the models.", "Understood");
                        }
                        
                    }
                    
                    GUILayout.Space(5f);
                    EditorGUILayout.EndVertical();
                    
                }
                
                // ---------------------------------------------------------------------------
                // Organize Materials Section
                // ---------------------------------------------------------------------------
                private void GUIOrganizeMaterials()
                {
                    #region Error Handling
                    // If the material stripping process not complete
                    try
                    {
                        if (sourceMat.Count == 0)
                        {
                            // Go back to previous step
                            processStep = 1;
                            return;
                        }
                    }
                    catch
                    {
                        // Go back to previous step
                        processStep = 1;
                        return;
                    }
                    #endregion
                    
                    // -------------------------------------
                    // Section header
                    // -------------------------------------
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    GUILayout.Space(5f);
                    EditorGUILayout.LabelField("Step 3:  Organize Materials", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Double-click to select materials to be renamed.  Multiple materials sharing the same new name will be consolidated.");
                    GUILayout.Space(5f);
                    EditorGUILayout.EndVertical();
                    
                    GUILayout.Space(5f);
                    
                    EditorGUILayout.BeginHorizontal();
                    
                    GUILayout.Space(5f);
                    
                    // -------------------------------------
                    // Main Materials Selection Grid
                    // -------------------------------------
                    EditorGUILayout.BeginVertical();
                    
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    
                    GUILayout.Space(5f);
                    
                    EditorGUILayout.LabelField("All Materials (Old > New) - Double-click to Add/Remove");
                    
                    orgMatScrollPos = EditorGUILayout.BeginScrollView(orgMatScrollPos);
                    
                    EditorGUI.BeginChangeCheck ();
                    
                    int newOrgMatSelection = GUILayout.SelectionGrid(orgMatSelection, orgMatSelGridData, 1, GUILayout.Width(horizontalWidth / 2));
                    
                    // If an item in the Selection Grid was clicked
                    if (EditorGUI.EndChangeCheck ())
                    {
                        // If selected item was clicked again "double-clicked"
                        if (newOrgMatSelection == orgMatSelection)
                        {
                            // Add/remove item to/from the selection list
                            selOrgMatPointer = AddOrDeleteIntFromOrderedIntArray(selOrgMatPointer, orgMatSelection);
                            
                            // Update the selected materials array for the selection organized materials Selection Grid
                            selOrgMatGridData = CreateStringArrayFromSelectionArray(orgMatSelGridData, selOrgMatPointer);
                            
                            // Unselect item
                            orgMatSelection = -1;
                        }
                        
                        // If a differet item was clicked
                        else
                        {
                            // Select it
                            orgMatSelection = newOrgMatSelection;
                        }
                    }
                    
                    EditorGUILayout.EndScrollView();
                    
                    GUILayout.Space(5f);
                    EditorGUILayout.EndVertical();
                    GUILayout.Space(5f);
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    GUILayout.Space(5f);
                    
                    // Remove Blender Material Duplicates Button
                    if (GUILayout.Button(new GUIContent("Merge Blender Material Duplicates",
                        "Merge all Blender material duplicates, removing the .001, .002, etc, leaving the base material name")))
                    {
                        // Remove Blender Material duplicates
                        sourceMatNewName = MergeBlenderMaterialDuplicates(sourceMatNewName);
                        
                        // Recreate the organize material array containing the source and renamed material name combined for each item
                        orgMatSelGridData = CombineStringsWithinArrays(sourceMatName, sourceMatNewName, " > ");
                        
                        // Reset the Selected Materials in Organize Materials
                        selOrgMatSelection = -1;
                        selOrgMatPointer = new int[0];
                        selOrgMatGridData = new string[0];
                    }
                    
                    GUILayout.Space(5f);
                    EditorGUILayout.EndVertical();
                    
                    EditorGUILayout.EndVertical();
                    
                    GUILayout.Space(5f);
                    
                    // -------------------------------------
                    // Selected Materials Selection Grid
                    // -------------------------------------
                    EditorGUILayout.BeginVertical();
                    
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    GUILayout.Space(5f);
                    
                    EditorGUILayout.LabelField("Selected Materials (Old > New) - Double-click to Remove");
                    
                    selOrgMatScrollPos = EditorGUILayout.BeginScrollView(selOrgMatScrollPos);
                    
                    EditorGUI.BeginChangeCheck ();
                    
                    int newSelOrgMatSelection = GUILayout.SelectionGrid(selOrgMatSelection, selOrgMatGridData, 1, GUILayout.Width(horizontalWidth / 2));
                    //EditorStyles.radioButton
                    
                    // If an item in the Selection Grid was clicked
                    if (EditorGUI.EndChangeCheck ())
                    {
                        // If selected item was clicked again "double-clicked"
                        if (newSelOrgMatSelection == selOrgMatSelection)
                        {
                            // Add/remove item to/from the selection list
                            selOrgMatPointer = AddOrDeleteIntFromOrderedIntArray(selOrgMatPointer, selOrgMatPointer[selOrgMatSelection]);
                            
                            // Update the selected materials array for the selection organized materials Selection Grid
                            selOrgMatGridData = CreateStringArrayFromSelectionArray(orgMatSelGridData, selOrgMatPointer);
                            
                            // Unselect item
                            selOrgMatSelection = -1;
                        }
                        
                        // If a differet item was clicked
                        else
                        {
                            // Select it
                            selOrgMatSelection = newSelOrgMatSelection;
                        }
                    }
                    
                    EditorGUILayout.EndScrollView();
                    
                    GUILayout.Space(5f);
                    EditorGUILayout.EndVertical();
                    GUILayout.Space(5f);
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    GUILayout.Space(5f);
                    
                    // New Material Name TextField
                    EditorGUIUtility.labelWidth = 68;
                    newMaterialName = EditorGUILayout.TextField(new GUIContent("New Name", "New name for material(s)"), newMaterialName);
                    EditorGUIUtility.labelWidth = 0;
                    
                    GUILayout.Space(5f);
                    
                    if (GUILayout.Button(new GUIContent("Rename / Merge Selected Materials", "Rename all selected materials")))
                    {
                        if (newMaterialName != "")
                        {
                            // Loop through each selected material
                            for (int i = 0; i < selOrgMatPointer.Length; i++)
                            {
                                // Use the pointer to change the name of the material in the renamed material list
                                sourceMatNewName[selOrgMatPointer[i]] = newMaterialName;
                            }
                            
                            // Recreate the organize material array containing the source and renamed material name combined for each item
                            orgMatSelGridData = CombineStringsWithinArrays(sourceMatName, sourceMatNewName, " > ");
                            
                            // Reset the Selected Materials in Organize Materials
                            selOrgMatSelection = -1;
                            selOrgMatPointer = new int[0];
                            selOrgMatGridData = new string[0];
                            
                            // Reset the material name
                            newMaterialName = "";
                        }
                    }
                    
                    GUILayout.Space(5f);
                    EditorGUILayout.EndVertical();
                    
                    EditorGUILayout.EndVertical();
                    
                    EditorGUILayout.EndHorizontal();
                    
                    GUILayout.Space(5f);
                    
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    GUILayout.Space(5f);
                    
                    // Done with Organizing Materials button
                    if (GUILayout.Button(new GUIContent("Done with Organizing  |  Extract Materials", "Extract materials then go to Link Materials section")))
                    {
                        
                        // If the "/Materials" subfolder doesn't exist
                        if (!AssetDatabase.IsValidFolder(modelPath + "/Materials"))
                        {
                            // Create the "/Materials" subfolder
                            AssetDatabase.CreateFolder(modelPath, "Materials");
                        }
                        
                        // If the "/Materials" subfolder already exists
                        else
                        {
                            // Display a warning.  If the user clicks "Continue"
                            if (EditorUtility.DisplayDialog("Erase existing Material folder?",
                                "A material folder already exists here.  Continuing will erase all previous materials in the folder.",
                                "Continue", "Cancel"))
                            {
                                // Delete the existing folder and create a new one
                                Directory.Delete(modelPath + "/Materials", true);
                                AssetDatabase.Refresh();
                                AssetDatabase.CreateFolder(modelPath, "Materials");
                                AssetDatabase.Refresh();
                            }
                            
                            // If the user clicks "Cancel", exit here
                            else return;
                        }
                        
                        // Get a consolidated list of all new material names
                        // This removes duplicates created from consolidation
                        conMatName = sourceMatNewName.Distinct().ToArray();
                        
                        // Define size of consolidated material array and material path array
                        conMat = new Material[conMatName.Length];
                        conMatPath = new string[conMatName.Length];
                        
                        // Get each consolidated material from the master sourceMat array
                        int matIndexInSource;
                        for (int i = 0; i < conMatName.Length; i++)
                        {
                            // Get index of first occurence of this material name in the source material list
                            matIndexInSource = Array.IndexOf(sourceMatNewName, conMatName[i]);
                            
                            // Get Material from the source material list using the index
                            conMat[i] = sourceMat[matIndexInSource];
                            
                            // Create path for each material
                            conMatPath[i] = modelPath + "/Materials/" + conMatName[i] + ".mat";
                            
                            // Create the file asset for each material
                            AssetDatabase.CreateAsset(UnityEngine.Object.Instantiate(conMat[i]), conMatPath[i]);
                        }
                        
                        // Save Materials and refesh the AssetsDatabase so it can be referenced again
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        
                        // Get a reference to each saved material
                        for (int i = 0; i < conMatPath.Length; i++)
                        {
                            conMat[i] = (Material)AssetDatabase.LoadAssetAtPath(conMatPath[i], typeof(Material));
                        }
                        
                        // Prepare Link Material selection grid
                        linkMatSelGridData = CombineStringsWithinArrays(conMatName, conMatPath, " > ");
                        linkMatSelection = -1;
                        
                        // Create the new material "with combined/renamed materials" pointer array
                        conMatPointer = new int[sourceMatName.Length];
                        
                        // Create a list from the subMatName array for searching
                        List<string> subMatNameList = new List<string>(conMatName);
                        
                        // Loop through each item in link material pointer arrays
                        for (int i = 0; i < conMatPointer.Length; i++)
                        {
                            // Create pointer from original mat list to the substitute material array
                            conMatPointer[i] = subMatNameList.IndexOf(sourceMatNewName[i]);
                        }
                        
                        // Set Substitute Material default folder to main project folder
                        lastFolderSelected = Application.dataPath;
                        
                        // GoTo next step
                        processStep = 3;
                    }
                    
                    GUILayout.Space(5f);
                    
                    EditorGUILayout.EndVertical();
                    
                }
                
                // ---------------------------------------------------------------------------
                // Link Materials Section
                // ---------------------------------------------------------------------------
                private void GUILinkMaterials()
                {
                    #region Error Handling
                    // If the material organization process not complete
                    try
                    {
                        if (conMatName.Length == 0)
                        {
                            // Go back a step
                            processStep = 2;
                            return;
                        }
                    }
                    catch
                    {
                        // Go back a step
                        processStep = 2;
                        return;
                    }
                    #endregion
                    
                    // -------------------------------------
                    // Section header
                    // -------------------------------------
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    GUILayout.Space(5f);
                    EditorGUILayout.LabelField("Step 4:  Link Materials", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Link model materials to saved materials in project");
                    GUILayout.Space(5f);
                    EditorGUILayout.EndVertical();
                    
                    GUILayout.Space(5f);
                    
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    GUILayout.Space(5f);
                    
                    EditorGUILayout.LabelField("Consolidated Materials (Name > Path)");
                    
                    conMatScrollPos = EditorGUILayout.BeginScrollView(conMatScrollPos);
                    
                    linkMatSelection = GUILayout.SelectionGrid(linkMatSelection, linkMatSelGridData, 1);
                    
                    EditorGUILayout.EndScrollView();
                    
                    GUILayout.Space(5f);
                    EditorGUILayout.EndVertical();
                    GUILayout.Space(5f);
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    GUILayout.Space(5f);
                    
                    // Substitute Selected Material button
                    if (GUILayout.Button(new GUIContent("Substitute Selected Material",
                        "Choose a Material to subtitute for the selected material")))
                    {
                        // Show choose Material file dialog
                        string matPath =
                            EditorUtility.OpenFilePanelWithFilters("Choose substitute Material for " + conMatName[linkMatSelection],
                            lastFolderSelected, new[] { "Material files", "mat"});
                        
                        // If a material was chosen (Cancel was not clicked)
                        if (matPath != "")
                        {
                            // Convert to project local path "Assets/..."
                            matPath = matPath.Replace(Application.dataPath, "Assets");
                            
                            // Add matpath to array
                            conMatPath[linkMatSelection] = matPath;
                            
                            // Link to new asset
                            conMat[linkMatSelection] = (Material)AssetDatabase.LoadAssetAtPath(conMatPath[linkMatSelection], typeof(Material));
                            
                            // Recreate selection grid data
                            linkMatSelGridData = CombineStringsWithinArrays(conMatName, this.conMatPath, " > ");
                            
                            // Remember this folder for next time
                            lastFolderSelected = matPath;
                        }
                    }
                    
                    GUILayout.Space(5f);
                    EditorGUILayout.EndVertical();
                    GUILayout.Space(5f);
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    GUILayout.Space(5f);
                    
                    // Done with Substituting Materials button
                    if (GUILayout.Button(new GUIContent("Done with Substituting Materials", "Go to next step")))
                    {
                        // GoTo next step
                        processStep = 4;
                        
                        // Reset "Create Prefab" section options
                        flagAddMeshCollider = true;
                        flagAddEmptyParent = true;
                        mergeMethod = MAST.Tools.Prefab_Creator.MergeMethod.MergeChildren;
                        //flagPreserveModelHierarchy = false;
                    }
                    
                    GUILayout.Space(5f);
                    EditorGUILayout.EndVertical();
                    
                }
                
                // ---------------------------------------------------------------------------
                // Create Prefabs Section
                // ---------------------------------------------------------------------------
                private void GUICreatePrefabs()
                {
                    // -------------------------------------
                    // Section header
                    // -------------------------------------
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    GUILayout.Space(5f);
                    EditorGUILayout.LabelField("Step 5:  Create Prefabs", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Extract meshes, then generate and save prefabs.");
                    GUILayout.Space(5f);
                    EditorGUILayout.EndVertical();
                    
                    GUILayout.Space(5f);
                    
                    EditorGUILayout.BeginVertical();
                    
                    EditorGUILayout.BeginHorizontal();
                    
                    // "Add Mesh Collider" checkbox
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    GUILayout.Space(5f);
                    flagAddMeshCollider = GUILayout.Toggle(flagAddMeshCollider, "Add Mesh Collider");
                    GUILayout.Space(5f);
                    EditorGUILayout.EndVertical();
                    
                    // "Add Empty Parent" checkbox
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    GUILayout.Space(5f);
                    flagAddEmptyParent = GUILayout.Toggle(flagAddEmptyParent, "Add Empty Parent");
                    GUILayout.Space(5f);
                    EditorGUILayout.EndVertical();
                    
                    // "Split Mesh by Material" checkbox
                    //GUI.enabled = false;
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    GUILayout.Space(5f);
                    
                    //MAST_PrepModels.MergeMethod mergeMethod
                    mergeMethod = (MAST.Tools.Prefab_Creator.MergeMethod)EditorGUILayout.EnumPopup(mergeMethod);
                    
                    //flagPreserveModelHierarchy = GUILayout.Toggle(flagPreserveModelHierarchy,
                    //    new GUIContent("Preserve Model Hierarchy", "Create a Prefab that has the same parent/child relationships as the Model"));
                    GUILayout.Space(5f);
                    EditorGUILayout.EndVertical();
                    //GUI.enabled = true;
                    
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.EndVertical();
                    
                    GUILayout.Space(5f);
                    
                    GUILayout.BeginVertical("MAST Toolbar BG Inset");
                    GUILayout.Space(5f);
                    
                    // "Create Prefabs" button
                    if (GUILayout.Button(new GUIContent("Create Prefabs", "Create prefabs")))
                    {
                        PrepModels.CreatePrefabs(modelPath, sourceMat, new List<string>(sourceMatName),
                                                conMatPointer, conMatName, conMat,
                                                flagAddMeshCollider, flagAddEmptyParent, mergeMethod);
                    }
                    
                    GUILayout.Space(5f);
                    EditorGUILayout.EndVertical();
                }
                
                // ---------------------------------------------------------------------------
                // Merge Blender Material Duplicates
                //
                // Used by Organize Materials section
                // ---------------------------------------------------------------------------
                private string[] MergeBlenderMaterialDuplicates(string[] materialNames)
                {
                    bool foundBlenderMaterialDuplicate = false;
                    
                    for (int i = 0; i < materialNames.Length; i++)
                    {
                        foundBlenderMaterialDuplicate = false;
                        
                        // If material name is long enough to contain a blender duplication
                        if (materialNames[i].Length > 4)
                        {
                            // If material has the expected "." 4 characters from the end of the string
                            if (materialNames[i].Substring(materialNames[i].Length - 4, 1) == ".")
                            {
                                // Try the follow - could throw an error if value isn't an integer
                                try
                                {
                                    // If final 3 character is a value
                                    if (int.Parse(materialNames[i].Substring(materialNames[i].Length - 3, 3)) > 0)
                                    {
                                        // Set flag to true
                                        foundBlenderMaterialDuplicate = true;
                                    }
                                }
                                catch { } // Value wasn't an integer.  Nothing to do.
                            }
                        }
                        
                        // If Blender duplicate found, remove trailing characters leaving only the base material name
                        if (foundBlenderMaterialDuplicate)
                        {
                            materialNames[i] = materialNames[i].Substring(0, materialNames[i].Length - 4);
                        }
                    }
                    
                    return materialNames;
                }
                
                // ---------------------------------------------------------------------------
                // Merge Blender Material Duplicates
                //
                // Used by Organize Materials section
                // ---------------------------------------------------------------------------
                private string[] CreateStringArrayFromSelectionArray(string[] sourceArray, int[] selectionArray)
                {
                    string[] newStringArray = new string[selectionArray.Length];
                    
                    for (int i = 0; i < selectionArray.Length; i++)
                    {
                        newStringArray[i] = sourceArray[selectionArray[i]];
                    }
                    
                    return newStringArray;
                }
                
                // ---------------------------------------------------------------------------
                // Add or Delete "if it already exists" an int from an orderered int array
                //
                // Used by Organize Materials section
                // ---------------------------------------------------------------------------
                private int[] AddOrDeleteIntFromOrderedIntArray(int[] intArray, int newInt)
                {
                    List<int> intList = new List<int>(intArray);
                    
                    int intIndex = intList.IndexOf(newInt);
                    
                    // If the list doesn't contain this item
                    if (intIndex == -1)
                    {
                        // Add Item to List
                        intList.Add(newInt);
                        
                        // Sort the list
                        intList.Sort();
                    }
                    
                    // If the list already contains this item
                    else
                    {
                        // Delete Item from List
                        intList.RemoveAt(intIndex);
                    }
                    
                    // Return the list as an array
                    return intList.ToArray();
                }
                
                // ---------------------------------------------------------------------------
                // Combine 2 string arrays where each item in the 2nd is appended to the 1st
                //
                // Used by Strip Materials and Organize Materials sections
                // ---------------------------------------------------------------------------
                private string[] CombineStringsWithinArrays (string[] stringArray, string[] secondStringArray, string separator)
                {
                    string[] newStringArray = new string[stringArray.Length];
                    
                    for (int i = 0; i < newStringArray.Length; i++)
                    {
                        newStringArray[i] = stringArray[i] + separator + secondStringArray[i];
                    }
                    
                    return newStringArray;
                }
                // ---------------------------------------------------------------------------
            }
        }
    }
}
#endif