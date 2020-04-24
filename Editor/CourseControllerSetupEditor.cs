using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Innoactive.Creator.BasicTemplate;

namespace Innoactive.CreatorEditor.BasicTemplate
{
    [CustomEditor(typeof(CourseControllerSetup))]
    public class CourseControllerSetupEditor : Editor
    {
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
            SetPrefab();

            serializedObject.ApplyModifiedProperties();
        }

        private void SetPrefab()
        {
            GameObject prefab = prefabProperty.objectReferenceValue as GameObject;
            int currentMode = courseModeProperty.enumValueIndex;
            
            if (prefab == null)
            {
                switch (currentMode)
                {
                    case 0:
                        SetDefaultCourseController();
                        break;
                    case 1:
                        SetStandaloneCourseController();
                        break;
                }
            }
            else
            {
                if (lastMode != currentMode)
                {
                    prefabProperty.objectReferenceValue = null;
                }
            }

            lastMode = currentMode;
        }

        private void SetDefaultCourseController()
        {
            GameObject prefab = GetPrefab("DefaultCourseController");
            prefabProperty.objectReferenceValue = prefab;
        }
        
        private void SetStandaloneCourseController()
        {
            GameObject prefab = GetPrefab("StandaloneCourseController");
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