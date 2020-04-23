using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Innoactive.Creator.BasicTemplate;
using UnityEditor;
using UnityEngine;

namespace Innoactive.CreatorEditor.BasicTemplate
{
    [CustomEditor(typeof(CourseControllerSetup))]
    public class CourseControllerSetupEditor : Editor
    {

        private SerializedProperty courseModeProperty;
        private SerializedProperty prefabProperty;
        
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

            if (prefab == null)
            {
                int courseMode = courseModeProperty.enumValueIndex;

                switch (courseMode)
                {
                    case 0:
                        SetDefaultCourseController();
                        break;
                    case 1:
                        SetStandaloneCourseController();
                        break;
                }
            }
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