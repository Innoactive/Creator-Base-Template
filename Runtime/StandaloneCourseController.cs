using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Innoactive.Creator.Core.Configuration;

namespace Innoactive.Creator.BasicTemplate
{
    public class StandaloneCourseController : MonoBehaviour
    {
        [Tooltip("Size of the font used")]
        [SerializeField]
        protected float distance = 2f;

        private Canvas canvas;
        private Transform trainee;
        private bool lastMenuState;
        private InputDevice controller;

        private void Start()
        {
            try
            {
                trainee = RuntimeConfigurator.Configuration.Trainee.GameObject.transform;
                canvas = GetComponentInChildren<Canvas>();
                canvas.worldCamera = Camera.main;
                canvas.enabled = false;
            }
            catch (Exception exception)
            {
                Debug.LogErrorFormat("{0} while initializing {1}.\n{2}", exception.GetType().Name, GetType().Name, exception.StackTrace);
            }
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
            if (IsMenuButtonDown())
            {
                ToggleCourseControllerMenu();
            }
        }

        private void ToggleCourseControllerMenu()
        {
            canvas.enabled = !canvas.enabled;

            if (canvas.enabled)
            {
                Vector3 position = trainee.position + (trainee.forward * distance);
                Quaternion rotation = new Quaternion(0.0f, trainee.rotation.y, 0.0f, trainee.rotation.w);

                transform.SetPositionAndRotation(position, rotation);
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

        private bool IsMenuButtonDown()
        {
            controller.TryGetFeatureValue(CommonUsages.menuButton, out bool isMenuActive);
            bool wasPressed = lastMenuState == false && isMenuActive;

            lastMenuState = isMenuActive;

            return wasPressed;
        }
    }
}