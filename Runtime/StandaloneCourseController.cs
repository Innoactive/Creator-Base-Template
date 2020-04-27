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
        
        [Tooltip("Size of the font used")]
        [SerializeField]
        protected float distance = 2f;

        private Vector3 originalForward;
        private Transform trainee;
        private InputDevice controller;

        private bool lastMenuState;

        private void Start()
        {
            try
            {
                trainee = RuntimeConfigurator.Configuration.Trainee.GameObject.transform;
            }
            catch (NullReferenceException)
            {
                //TODO
            }
            
            SetFont();
            SetMainCamera();
            ShowCourseControllerMenu();
        }

        private void OnEnable()
        {
            InputDevices.deviceConnected += RegisterDevice;
            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevices(devices);

            foreach (InputDevice device in devices)
            {
                RegisterDevice(device);
            }
        }
        
        private void OnDisable()
        {
            InputDevices.deviceConnected -= RegisterDevice;
        }

        private void Update()
        {
            controller.TryGetFeatureValue(CommonUsages.menuButton, out bool isMenuActive);
            
            ShowCourseControllerMenu();
            
            if (lastMenuState != isMenuActive)
            {
                if (isMenuActive)
                {
                    originalForward = trainee.forward;
                }
                else
                {
                    
                }
            }

            lastMenuState = isMenuActive;
        }

        private void ShowCourseControllerMenu()
        {
            Quaternion traineeRotation = trainee.rotation;
            Vector3 position = trainee.position + (originalForward * distance);
            Quaternion rotation = new Quaternion(0.0f, -traineeRotation.y, 0.0f, traineeRotation.w);
            
            transform.SetPositionAndRotation(position, rotation);
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
        
        private void RegisterDevice(InputDevice connectedDevice)
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