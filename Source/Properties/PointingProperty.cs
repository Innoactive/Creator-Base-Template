using System;
using Innoactive.Hub.Training.SceneObjects.Properties;
using UnityEngine;
using VRTK;

namespace Innoactive.Hub.Training.Template
{
    // PointingProperty requires VRTK_Pointer to work.
    [RequireComponent(typeof(VRTK_Pointer))]
    // Training object with that property can point at other training objects.
    // Any property should inherit from SceneObjectProperty class.
    public class PointingProperty : SceneObjectProperty
    {
        // Event that is invoked every time when the object points at something.
        public event Action<ColliderWithTriggerProperty> PointerEnter;

        // Reference to attached VRTK_Pointer.
        private VRTK_Pointer pointer;

        // Fake the pointing at target. Used when you fast-forward PointedCondition.
        public virtual void FastForwardPoint(ColliderWithTriggerProperty target)
        {
            if (target != null && PointerEnter != null)
            {
                PointerEnter(target);
            }
        }

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