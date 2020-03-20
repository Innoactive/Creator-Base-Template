using System.Collections;
using System.Runtime.Serialization;
using Innoactive.Creator.Core;
using Innoactive.Creator.Core.Attributes;
using Innoactive.Creator.Core.Behaviors;
using Innoactive.Creator.Core.SceneObjects;
using Newtonsoft.Json;
using UnityEngine;

namespace Innoactive.Hub.Training.Template
{
    // This behavior linearly changes scale of a Target object over Duration seconds, until it matches TargetScale.
    [DataContract(IsReference = true)]
    public class ScalingBehavior : Behavior<ScalingBehavior.EntityData>
    {
        [DisplayName("Scale Object")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            // Training object to scale.
            [DataMember]
            public SceneObjectReference Target { get; set; }

            // Target scale.
            [DataMember]
            [DisplayName("Target Scale")]
            public Vector3 TargetScale { get; set; }

            // Duration of the animation in seconds.
            [DataMember]
            [DisplayName("Animation Duration")]
            public float Duration { get; set; }

            public Metadata Metadata { get; set; }
            public string Name { get; set; }
        }

        // Handle data initialization in the constructor.
        [JsonConstructor]
        public ScalingBehavior() : this(new SceneObjectReference(), Vector3.one, 0f)
        {
        }

        public ScalingBehavior(SceneObjectReference target, Vector3 targetScale, float duration)
        {
            Data = new EntityData()
            {
                Target = target,
                TargetScale = targetScale,
                Duration = duration,
            };
        }

        public class ActivatingProcess : IStageProcess<EntityData>
        {
            public void Start(EntityData data)
            {
            }

            public IEnumerator Update(EntityData data)
            {
                float startedAt = Time.time;

                Transform scaledTransform = data.Target.Value.GameObject.transform;

                Vector3 initialScale = scaledTransform.localScale;

                while (Time.time - startedAt < data.Duration)
                {
                    float progress = (Time.time - startedAt) / data.Duration;

                    scaledTransform.localScale = Vector3.Lerp(initialScale, data.TargetScale, progress);
                    yield return null;
                }
            }

            public void End(EntityData data)
            {
                Transform scaledTransform = data.Target.Value.GameObject.transform;
                scaledTransform.localScale = data.TargetScale;
            }

            public void FastForward(EntityData data)
            {
            }
        }

        private readonly IProcess<EntityData> process = new Process<EntityData>(new ActivatingProcess(), new EmptyStageProcess<EntityData>(), new EmptyStageProcess<EntityData>());

        protected override IProcess<EntityData> Process
        {
            get
            {
                return process;
            }
        }
    }
}
