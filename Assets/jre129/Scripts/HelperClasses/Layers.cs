using UnityEditor;
using UnityEngine;

namespace jre129.Scripts.HelperClasses
{
    public class Layers
    {
        #region Static Fields

        private static int _maxTags = 10000;
        private static int _maxLayers = 31;

        #endregion
        

        public static bool CreateLayer(string layerName)
        {
#if UNITY_EDITOR
            SerializedObject tagManager =
                new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layersProp = tagManager.FindProperty("layers");

            if (!PropertyExists(layersProp, 0, layersProp.arraySize, layerName))
            {
                SerializedProperty serializedProperty;
                // Start at 8 as unity reserves Layers 0-7
                for (int layerIndex = 8; layerIndex < layersProp.arraySize; layerIndex++)
                {
                    serializedProperty = layersProp.GetArrayElementAtIndex(layerIndex);

                    if (serializedProperty.stringValue == "")
                    {
                        serializedProperty.stringValue = layerName;
                        Debug.Log("Layer: " + layerName + " has been added");
                        tagManager.ApplyModifiedProperties();
                        return true;
                    }

                    if (layerIndex == layersProp.arraySize)
                    {
                        Debug.Log("Maximum number of layers reached");
                    }
                }
            }
#endif
            return false;
        }

        /// <summary>
        /// Checks to see whether the key within the SerializedProperty exists.
        /// </summary>
        /// <param name="property">The property we are searching</param>
        /// <param name="begin">Index to begin the search</param>
        /// <param name="target">Index to end the search</param>
        /// <param name="key">They key we are looking for</param>
        /// <returns><c>true</c> if the property exists. <c>false</c> if it doesnt</returns>
        private static bool PropertyExists(SerializedProperty property, int begin, int target, string key)
        {
            for (int propertyIndex = begin; propertyIndex < target; propertyIndex++)
            {
                SerializedProperty currentProperty = property.GetArrayElementAtIndex(propertyIndex);
                if (currentProperty.stringValue.Equals(key))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to create a layer from the given name
        /// </summary>
        /// <param name="name">The name of the layer</param>
        /// <returns><c>true</c> if the given layer exists, or was successfully created <c>false</c> otherwise</returns>
        public static bool TryCreateLayerFromName(string name)
        {
            return LayerMask.NameToLayer(name) != -1 ||
                   // Layer Found
                   CreateLayer(name);
        }
    }
}