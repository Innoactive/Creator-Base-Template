using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Innoactive.Creator.Core.Configuration;
using UnityEngine.XR;

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

        private InputDevice controller;

        private void Start()
        {
            SetFont();
            SetMainCamera();
            ShowCourseControllerMenu();
        }

        private void OnEnable()
        {
            InputDevices.deviceConnected += RegisterDevices;
            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevices(devices);

            foreach (InputDevice device in devices)
            {
                RegisterDevices(device);
            }
        }
        
        private void OnDisable()
        {
            InputDevices.deviceConnected -= RegisterDevices;
        }

        private void Update()
        {
            controller.TryGetFeatureValue(CommonUsages.menuButton, out bool onOnMenu);
            
            if (onOnMenu)
            {
                ShowCourseControllerMenu();
            }
        }

        private void ShowCourseControllerMenu()
        {
            try
            {
                Transform trainee = RuntimeConfigurator.Configuration.Trainee.GameObject.transform;
                
                transform.SetPositionAndRotation(trainee.position + trainee.forward, trainee.rotation);
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
        
        private void RegisterDevices(InputDevice connectedDevice)
        {
            if (connectedDevice.isValid)
            {
                if ((connectedDevice.characteristics & InputDeviceCharacteristics.Left) != 0)
                {
                    controller = connectedDevice;
                }
            }
        }
    }
}