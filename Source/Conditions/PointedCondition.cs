﻿using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Innoactive.Hub.Training.Template
{
    [DataContract(IsReference = true)]
    [DisplayName("Point at Object")]
    // Condition which is completed when Pointer points at Target.
    public class PointedCondition : Condition
    {
        [DataMember]
        // Reference to a pointer property.
        public TrainingPropertyReference<PointingProperty> Pointer { get; private set; }

        [DisplayName("Target with a collider")]
        [DataMember]
        // Reference to a target property.
        public TrainingPropertyReference<ColliderWithTriggerProperty> Target { get; private set; }

        [JsonConstructor]
        // Make sure that references are initialized.
        public PointedCondition() : this(new TrainingPropertyReference<PointingProperty>(), new TrainingPropertyReference<ColliderWithTriggerProperty>())
        {
        }

        public PointedCondition(TrainingPropertyReference<PointingProperty> pointer, TrainingPropertyReference<ColliderWithTriggerProperty> target)
        {
            Pointer = pointer;
            Target = target;
        }

        // This method is called when the step with that condition has completed activation of its behaviors.
        protected override void PerformActivation()
        {
            Pointer.Value.PointerEnter += OnPointerEnter;
            SignalActivationFinished();
        }

        // This method is called at deactivation of the step, after every behavior has completed its deactivation.
        protected override void PerformDeactivation()
        {
            Pointer.Value.PointerEnter -= OnPointerEnter;
            SignalDeactivationFinished();
        }

        protected override void FastForwardActivating()
        {
        }

        protected override void FastForwardActive()
        {
            Pointer.Value.FastForwardPoint(Target);
        }

        protected override void FastForwardDeactivating()
        {
        }

        // When PointerProperty points at something,
        private void OnPointerEnter(ColliderWithTriggerProperty pointed)
        {
            // Ignore it if this condition is already fulfilled.
            if (IsCompleted)
            {
                return;
            }

            // Else, if Target references the pointed object, complete the condition.
            if (Target.Value == pointed)
            {
                MarkAsCompleted();
            }
        }
    }
}