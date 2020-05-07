﻿using TMPro;
using UnityEngine;
using UnityEngine.XR;
using System;
using System.Collections.Generic;
using Innoactive.Creator.Core;
using Innoactive.Creator.Core.Configuration;

namespace Innoactive.Creator.BasicTemplate
{
    public class StandaloneMenuHandler : MonoBehaviour
    {
        [Tooltip("Initial distance between this controller and the trainee.")] [SerializeField]
        protected float appearanceDistance = 2f;

        private float s_DefaultPressThreshold = 0.1f;
        private Canvas canvas;
        private Transform trainee;
        private bool lastMenuState;
        private readonly List<InputDevice> controllers = new List<InputDevice>();
        private readonly List<TMP_Dropdown> dropdownsList = new List<TMP_Dropdown>();

        [SerializeField, HideInInspector] private string buttonTypeName = "bool";
        [SerializeField, HideInInspector] private string buttonName = "MenuButton";

        private Type buttonType;

        private void OnValidate()
        {
            buttonType = Type.GetType(buttonTypeName);
        }

        private void Start()
        {
            try
            {
                trainee = RuntimeConfigurator.Configuration.Trainee.GameObject.transform;
                canvas = GetComponentInChildren<Canvas>();
                canvas.worldCamera = Camera.main;

                canvas.enabled = CourseRunner.Current != null;
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"{exception.GetType().Name} while initializing {GetType().Name}.\n{exception.StackTrace}",
                    gameObject);
            }

            Vector3 position = trainee.position + (trainee.forward * appearanceDistance);
            Quaternion rotation = new Quaternion(0.0f, trainee.rotation.y, 0.0f, trainee.rotation.w);
            position.y = trainee.position.y;

            transform.SetPositionAndRotation(position, rotation);
            dropdownsList.AddRange(GetComponentsInChildren<TMP_Dropdown>());

            OnValidate();
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
            if (IsActionButtonPressDown())
            {
                ToggleCourseControllerMenu();
            }
        }

        private void RegisterDevice(InputDevice connectedDevice)
        {
            if (connectedDevice.isValid)
            {
                controllers.Add(connectedDevice);
            }
        }

        private void ToggleCourseControllerMenu()
        {
            canvas.enabled = !canvas.enabled;

            if (canvas.enabled)
            {
                Vector3 position = trainee.position + (trainee.forward * appearanceDistance);
                Quaternion rotation = new Quaternion(0.0f, trainee.rotation.y, 0.0f, trainee.rotation.w);
                position.y = trainee.position.y;

                transform.SetPositionAndRotation(position, rotation);
            }
            else
            {
                HideDropdowns();
            }
        }

        private void HideDropdowns()
        {
            foreach (TMP_Dropdown dropdown in dropdownsList)
            {
                if (dropdown.IsExpanded)
                {
                    dropdown.Hide();
                }
            }
        }

        private bool IsActionButtonPressDown(float pressThreshold = -1.0f)
        {
            IsActionButtonPress(out bool isButtonPress);
            bool wasPressed = lastMenuState == false && isButtonPress;

            lastMenuState = isButtonPress;

            return wasPressed;
        }

        private bool IsActionButtonPress(out bool isPressed, float pressThreshold = -1.0f)
        {
            foreach (InputDevice controller in controllers)
            {
                if (controller.isValid == false)
                {
                    return isPressed = false;
                }

                if (buttonType == typeof(bool))
                {
                    if (controller.TryGetFeatureValue(new InputFeatureUsage<bool>(buttonName), out bool value))
                    {
                        isPressed = value;
                        return true;
                    }
                }
                else if (buttonType == typeof(float))
                {
                    if (controller.TryGetFeatureValue(new InputFeatureUsage<float>(buttonName), out float value))
                    {
                        float threshold = (pressThreshold >= 0.0f) ? pressThreshold : s_DefaultPressThreshold;
                        isPressed = value >= threshold;
                        return true;
                    }
                }
                else if (buttonType == typeof(Vector2))
                {
                    if (controller.TryGetFeatureValue(new InputFeatureUsage<Vector2>(buttonName), out Vector2 value))
                    {
                        float threshold = (pressThreshold >= 0.0f) ? pressThreshold : s_DefaultPressThreshold;
                        isPressed = value.x >= threshold;
                        return true;
                    }
                }
            }

            return isPressed = false;
        }
    }
}