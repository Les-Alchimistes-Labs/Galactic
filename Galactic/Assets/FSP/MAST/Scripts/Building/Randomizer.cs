using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if (UNITY_EDITOR)

namespace MAST
{
    namespace Building
    {
        public static class Randomizer
        {
            private static Vector3 position;
            private static Vector3 rotation;
            private static Vector3 scale;
            
            private static List<int> replacePrefabIndexList;
            private static List<int> replaceWeightList;
            
            private static int activeReplaceID;
            private static int lastUsedReplacePrefabIndex;
            
            // -----------------------------------------------------------------------
            // Generate new random values for placement
            // -----------------------------------------------------------------------
            public static void GenerateNewRandomSeed(bool skipReplacement = false)
            {
                CalculateRandomPosition();
                CalculateRandomRotation();
                CalculateRandomScale();
                CalculateRandomFlip();
                if (!skipReplacement)
                    CalculateReplacement();
            }
            
            // -----------------------------------------------------------------------
            // Randomize Position
            // -----------------------------------------------------------------------
            private static void CalculateRandomPosition()
            {
                // Get position randomizer values
                Vector3 posMin = Helper.Randomizer.Position.GetMin();
                Vector3 posMax = Helper.Randomizer.Position.GetMax();
                
                // Calculate position
                position.x = Random.Range(posMin.x, posMax.x);
                position.y = Random.Range(posMin.y, posMax.y);
                position.z = Random.Range(posMin.z, posMax.z);
            }
            
            // -----------------------------------------------------------------------
            // Randomize Rotation
            // -----------------------------------------------------------------------
            private static void CalculateRandomRotation()
            {
                // Get rotation randomizer values
                Vector3 rotStep = Helper.Randomizer.Rotation.GetStep();
                Vector3 rotMin = Helper.Randomizer.Rotation.GetMin();
                Vector3 rotMax = Helper.Randomizer.Rotation.GetMax();
                
                // Calculate rotation
                rotation.x = (rotStep.x == 0f) ? 0f : Mathf.Floor(Random.Range(rotMin.x, rotMax.x) / rotStep.x) * rotStep.x;
                rotation.y = (rotStep.y == 0f) ? 0f : Mathf.Floor(Random.Range(rotMin.y, rotMax.y) / rotStep.y) * rotStep.y;
                rotation.z = (rotStep.z == 0f) ? 0f : Mathf.Floor(Random.Range(rotMin.z, rotMax.z) / rotStep.z) * rotStep.z;
            }
            
            // -----------------------------------------------------------------------
            // Randomize Scale
            // -----------------------------------------------------------------------
            private static void CalculateRandomScale()
            {
                // Get position randomizer values
                Vector3 scaleMin = Helper.Randomizer.Scale.GetMin();
                Vector3 scaleMax = Helper.Randomizer.Scale.GetMax();
                ScaleAxisLock axisLock = Helper.Randomizer.Scale.GetLock();
                
                // Calculate scale
                scale.x = Random.Range(scaleMin.x, scaleMax.x);
                scale.y = (axisLock == ScaleAxisLock.XYZ) ? scale.x : Random.Range(scaleMin.y, scaleMax.y);
                scale.z = (axisLock != ScaleAxisLock.NONE) ? scale.x : Random.Range(scaleMin.z, scaleMax.z);
            }
            
            // -----------------------------------------------------------------------
            // Randomize Flip
            // -----------------------------------------------------------------------
            private static void CalculateRandomFlip()
            {
                // Calculate flip on X axis
                if (Helper.Randomizer.Flip.GetX())
                    if (Random.value < 0.5f)
                        scale.x = -scale.x;
                
                // Calculate flip on Y axis
                if (Helper.Randomizer.Flip.GetY())
                    if (Random.value < 0.5f)
                        scale.y = -scale.y;
                
                // Calculate flip on Z axis
                if (Helper.Randomizer.Flip.GetZ())
                    if (Random.value < 0.5f)
                        scale.z = -scale.z;
            }
            
            // -----------------------------------------------------------------------
            // Replace prefab with another in same category
            // -----------------------------------------------------------------------
            private static void CalculateReplacement()
            {
                int replaceID = 0;
                
                // Get replaceID
                if (Helper.Randomizer.Replace.GetReplaceable())
                    replaceID = Helper.Randomizer.Replace.GetReplaceID();
                
                // If no replaceID was found, then exit without choosing another prefab
                if (replaceID == 0)
                    return;
                
                // If this is a different replace ID, get replacement prefabs
                if (replaceID != activeReplaceID)
                    GetReplacementPrefabs(replaceID);
                
                // If replace prefab index list is empty, exit without choosing another prefab
                if (replacePrefabIndexList == null || replacePrefabIndexList.Count == 0)
                    return;
                
                // Get random number between 1 and the total sum of replace weights
                float randomReplaceSeed = Random.Range(1, replaceWeightList.Sum());
                float replaceWeightRunningTotal = 0;
                
                // Loop through each replace weight
                for (int i = 0; i < replaceWeightList.Count; i++)
                {
                    // Add this replace weight to the running total
                    replaceWeightRunningTotal += replaceWeightList[i];
                    
                    // If the random replace number is less than this new running total
                    if (randomReplaceSeed < replaceWeightRunningTotal)
                    {
                        // Select this prefab and stop looking
                        GUI.Palette.ChangePrefabSelection(replacePrefabIndexList[i]);
                        lastUsedReplacePrefabIndex = replacePrefabIndexList[i];
                        break;
                    }
                }
            }
            
            // -----------------------------------------------------------------------
            // Used by CalculateReplacement: 
            //    Get replacement prefab indexes and replace weights
            // -----------------------------------------------------------------------
            private static void GetReplacementPrefabs(int replaceID)
            {
                // Reinitialize replacement prefab index list and replace weight list
                replacePrefabIndexList = new List<int>();
                replaceWeightList = new List<int>();
                
                // Define MAST component script variable outside the foreach loop
                Component.MASTPrefabSettings mastScript;
                
                // Loop through each prefab in the palette
                for (int prefabIndex = 0; prefabIndex < Palette.Manager.GetPrefabArray().Length; prefabIndex++)
                {
                    // Get the MAST component script attached to this prefab
                    mastScript = Palette.Manager.GetPrefabArray()[prefabIndex].GetComponent<Component.MASTPrefabSettings>();
                    
                    // If a MAST component script was attached to this prefab
                    if (mastScript)
                    {
                        // If prefab is replaceable and prefab category ID matches
                        if (mastScript.randomizer.allowReplacement)
                            if (mastScript.randomizer.replacementID == replaceID)
                            {
                                // If this prefab wasn't used last, or prefab was used last but can be repeated
                                if ((prefabIndex != lastUsedReplacePrefabIndex) ||
                                    (prefabIndex == lastUsedReplacePrefabIndex && mastScript.randomizer.allowSequentialRepeat))
                                {
                                    replacePrefabIndexList.Add(prefabIndex);
                                    replaceWeightList.Add(mastScript.randomizer.replacementWeight);
                                }
                            }
                    }
                }
                
                // Save this replace ID
                activeReplaceID = replaceID;
            }
            
            
            
            // ---------------------------------------------------------------------------
            // Apply Randomizer values to GameObject Transform
            // ---------------------------------------------------------------------------
            public static GameObject ApplyRandomizerToTransform(GameObject gameObject, Quaternion defaultRotation)
            {
                // Move ghost based on Randomizer values
                gameObject.transform.position += position;
                
                // Rotate gameobject based on Randomizer values
                gameObject.transform.rotation = defaultRotation;
                gameObject.transform.Rotate(rotation.x, rotation.y, rotation.z);
                
                // Scale ghost based on Randomizer values
                gameObject.transform.localScale = new Vector3(
                    scale.x, scale.y, scale.z);
                
                return gameObject;
            }
        }
    }
}

#endif