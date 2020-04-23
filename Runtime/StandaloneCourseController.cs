using System;
using UnityEngine;
using UnityEngine.UI;
using Innoactive.Creator.Core.Configuration;

namespace Innoactive.Creator.BasicTemplate
{
    public class StandaloneCourseController : MonoBehaviour
    {
        [Tooltip("The font used in the spectator view.")]
        [SerializeField]
        protected Font font;
        
        [Tooltip("Size of the font used")]
        [SerializeField]
        protected int fontSize = 30;
        
        private void Start()
        {
            SetFont();
            SetMainCamera();
            PositionCourseMenu();
        }

        private void PositionCourseMenu()
        {
            try
            {
                Transform trainee = RuntimeConfigurator.Configuration.Trainee.GameObject.transform;
                
                transform.SetPositionAndRotation(trainee.position + trainee.forward, trainee.rotation);
                transform.SetParent(trainee);
            }
            catch (NullReferenceException)
            {
            }
        }

        private void SetMainCamera()
        {
            if (Camera.main != null)
            {
                Canvas canvas = GetComponentInChildren<Canvas>();
                canvas.worldCamera = Camera.main;
            }
        }
        
        private void SetFont()
        {
            foreach (Text text in GetComponentsInChildren<Text>(true))
            {
                text.font = font;
                text.fontSize = fontSize;
            }
        }
    }
}