﻿using System;
using UnityEngine;
using VRTK;

namespace Innoactive.Hub.Training.Template
{
    // PointingProperty requires VRTK_Pointer to work.
    [RequireComponent(typeof(VRTK_Pointer))]
    // Training object with that property can point at other training objects.
    // Any property should inherit from TrainingObjectProperty class.
    public class PointingProperty : TrainingObjectProperty
    {
        // Event that is invoked every time when the object points at something.
        public event Action<ColliderWithTriggerProperty> PointerEnter;

        // Reference to attached VRTK_Pointer.
        private VRTK_Pointer pointer;

        // Unity callback method
        protected override void OnEnable()
        {
            // Training object property handle their initialization at OnEnable().
            base.OnEnable();

            // Find attached VRTK_Pointer.
            pointer = GetComponent<VRTK_Pointer>();

            // Subscribe to VRTK_Pointer's event which is raised when it hits any collider.
            pointer.DestinationMarkerEnter += PointerOnDestinationMarkerEnter;
        }

        // Unity callback method
        private void OnDisable()
        {
            // Unsubscribe from VRTK_Pointer's event.
            pointer.DestinationMarkerEnter -= PointerOnDestinationMarkerEnter;
        }

        // VRTK_Pointer.DestinationMarkerEnter handler.
        private void PointerOnDestinationMarkerEnter(object sender, DestinationMarkerEventArgs e)
        {
            // If target is ColliderWithTriggerProperty, raise PointerEnter event.
            ColliderWithTriggerProperty target = e.target.GetComponent<ColliderWithTriggerProperty>();
            if (target != null && PointerEnter != null)
            {
                PointerEnter(target);
            }
        }
    }
}
