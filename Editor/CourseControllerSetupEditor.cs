using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Innoactive.Creator.BasicTemplate;

namespace Innoactive.CreatorEditor.BasicTemplate
{
    /// <summary>
    /// Editor drawer for <see cref="CourseControllerSetup"/>.
    /// </summary>
    [CustomEditor(typeof(CourseControllerSetup))]
    internal class CourseControllerSetupEditor : Editor
    {
        private const string DefaultPrefab = "DefaultCourseController";
        private const string StandalonePrefab = "StandaloneCourseController";
        
        private SerializedProperty courseModeProperty;
        private SerializedProperty prefabProperty;
        private int lastMode;
        
        private void OnEnable()
        {
            courseModeProperty = serializedObject.FindProperty("courseMode");
            prefabProperty = serializedObject.FindProperty("courseControllerPrefab");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ValidateControllerModeSelection();

            serializedObject.ApplyModifiedProperties();
        }

        private void ValidateControllerModeSelection()
        {
            GameObject prefab = prefabProperty.objectReferenceValue as GameObject;
            int currentMode = courseModeProperty.enumValueIndex;
            
            if (prefab == null)
            {
                SetPrefab(currentMode);
            }
            else if (lastMode != currentMode)
            {
                prefabProperty.objectReferenceValue = null;
            }

            lastMode = currentMode;
        }

        private void SetPrefab(int currentMode)
        {
            GameObject prefab;
            
            // Default = 0
            // Standalone = 1
            prefab = GetPrefab(currentMode == 0 ? DefaultPrefab : StandalonePrefab);
            prefabProperty.objectReferenceValue = prefab;
        }

        private GameObject GetPrefab(string prefab)
        {
            string filter = $"{prefab} t:Prefab";
            string[] prefabsGUIDs = AssetDatabase.FindAssets(filter, null);

            if (prefabsGUIDs.Any() == false)
            {
                throw new FileNotFoundException($"No prefabs found that match \"{prefab}\"." );
            }

            string assetPath = AssetDatabase.GUIDToAssetPath(prefabsGUIDs.First());
            return AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        }
    }
}