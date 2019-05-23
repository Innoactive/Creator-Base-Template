using System.Collections;
using System.Runtime.Serialization;
using Innoactive.Hub.Threading;
using Newtonsoft.Json;
using UnityEngine;

namespace Innoactive.Hub.Training.Template
{
    // This behavior linearly changes scale of a Target object over Duration seconds, until it matches TargetScale.
    [DataContract(IsReference = true)]
    [DisplayName("Scale Object")]
    public class ScalingBehavior : Behavior
    {
        // Training object to scale.
        [DataMember]
        public TrainingObjectReference Target { get; private set; }

        // Target scale.
        [DataMember]
        [DisplayName("Target Scale")]
        public Vector3 TargetScale { get; private set; }

        // Duration of the animation in seconds.
        [DataMember]
        [DisplayName("Animation Duration")]
        public float Duration { get; private set; }

        // A coroutine responsible for scaling the target.
        private IEnumerator coroutine;
        
        // Handle data initialization in the constructor.
        [JsonConstructor]
        public ScalingBehavior() : this(new TrainingObjectReference(), Vector3.one, 0f)
        {
        }

        public ScalingBehavior(TrainingObjectReference target, Vector3 targetScale, float duration)
        {
            Target = target;
            TargetScale = targetScale;
            Duration = duration;
        }
        
        // Called on activation of the training entity. Define activation logic here.
        // You have to call `SignalActivationStarted()` at the start
        // and `SignalActivationFinished()` after you've done everything you wanted to do during the activation.
        protected override void PerformActivation()
        {
            // Start coroutine which will scale our object.
            coroutine = ScaleTarget();
            CoroutineDispatcher.Instance.StartCoroutine(coroutine);
        }

        // Called on deactivation of the training entity. Define deactivation logic here.
        // You have to call `SignalDeactivationStarted()` at the start
        // and `SignalDeactivationFinished()` after you've done everything you wanted to do during the deactivation.
        protected override void PerformDeactivation()
        {
            SignalDeactivationFinished();
        }

        // This method is called when the activation has to be interrupted and completed immediately.
        protected override void FastForwardActivating()
        {
            // If the scaling behavior is currently activating (running),
            if (ActivationState == ActivationState.Activating)
            {
                // Stop the scaling coroutine,
                CoroutineDispatcher.Instance.StopCoroutine(coroutine);

                // Scale the target manually,
                Target.Value.GameObject.transform.localScale = TargetScale;

                // And signal that activation is finished.
                SignalActivationFinished();
            }
        }
        
        // It requires no additional action.
        protected override void FastForwardActive()
        {
        }

        // Deactivation is instanteneous.
        // It requires no additional action.
        protected override void FastForwardDeactivating()
        {
        }
        
        // Coroutine which scales the target transform over time and then finished the activation.
        private IEnumerator ScaleTarget()
        {
            float startedAt = Time.time;

            Transform scaledTransform = Target.Value.GameObject.transform;

            Vector3 initialScale = scaledTransform.localScale;

            while (Time.time - startedAt < Duration)
            {
                float progress = (Time.time - startedAt) / Duration;

                scaledTransform.localScale = Vector3.Lerp(initialScale, TargetScale, progress);
                yield return null;
            }

            scaledTransform.localScale = TargetScale;

            SignalActivationFinished();
        }
    }
}
