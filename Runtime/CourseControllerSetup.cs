﻿using System.IO;
using UnityEngine;

namespace Innoactive.Creator.BasicTemplate
{
    public class CourseControllerSetup : MonoBehaviour
    {
        private enum CourseMode
        {
            Default = 0,
            Standalone = 1
        }
        
        [SerializeField, HideInInspector]
        private CourseMode courseMode;
        
        [SerializeField]
        private GameObject courseControllerPrefab = null;

        private GameObject currentControllerInstance = null;

        protected virtual void Start()
        {
            InstantiateSpectator();
        }

        private void InstantiateSpectator()
        {
            if (courseControllerPrefab == null)
            {
                throw new FileNotFoundException($"No course controller prefabs set." );
            }
            
            if (currentControllerInstance != null)
            {
                Destroy(currentControllerInstance);
            }
            
            currentControllerInstance = Instantiate(courseControllerPrefab);
        }
    }
}
