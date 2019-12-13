using System.Collections;
using System.Runtime.Serialization;
using Innoactive.Hub.Training.Attributes;
using Innoactive.Hub.Training.Behaviors;
using Innoactive.Hub.Training.SceneObjects;
using Innoactive.Hub.Training.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace Innoactive.Hub.Training.Template.Animation
{
    [DataContract(IsReference = true)]
    public class AnimationBehavior : Behavior<AnimationBehavior.EntityData>
    {
        [DisplayName("Animate Object")]
        public class EntityData : IBehaviorData
        {
            [DataMember]
            [DisplayNameAttribute("Object To Animate")]
            public ScenePropertyReference<AnimationProperty> AnimationProperty { get; set; }

            [DataMember]
            [DisplayNameAttribute("Animation Trigger Parameter")]
            public string AnimationTriggerParameter { get; set; }

            /// <summary>
            /// A property that determines if the audio should be played at activation or deactivation (or both).
            /// </summary>
            [DataMember]
            public BehaviorExecutionStages ExecutionStage { get; set; }

            public Metadata Metadata { get; set; }

            public string Name { get; set; }
        }

        [JsonConstructor]
        public AnimationBehavior() : this("", "", BehaviorExecutionStages.None)
        {
        }

        public AnimationBehavior(AnimationProperty target, string animationTriggerParameter, BehaviorExecutionStages executionStage, string name = null) : this(TrainingReferenceUtils.GetNameFrom(target), animationTriggerParameter, executionStage, name)
        {
        }

        public AnimationBehavior(string target, string animationTriggerParameter, BehaviorExecutionStages executionStage, string name = "Animate Object")
        {
            Data = new EntityData()
            {
                AnimationProperty = new ScenePropertyReference<AnimationProperty>(target),
                AnimationTriggerParameter = animationTriggerParameter,
                ExecutionStage = executionStage,
                Name = name
            };
        }

        private class AnimationProcess : IStageProcess<EntityData>
        {
            private readonly BehaviorExecutionStages executionStage;
            private string animationTriggerParameter;
            private bool isAnimationRunning;
            private bool hasAnimationStarted;
            private Animator animator;

            public AnimationProcess(BehaviorExecutionStages executionStage)
            {
                this.executionStage = executionStage;
            }

            public void Start(EntityData data)
            {
                animationTriggerParameter = data.AnimationTriggerParameter;
                animator = data.AnimationProperty.Value.AnimationEvents.Animator;

                if ((data.ExecutionStage & executionStage) > 0)
                {
                    data.AnimationProperty.Value.AnimationEvents.AnimationDone += OnAnimationFinished;
                    data.AnimationProperty.Value.AnimationEvents.AnimationStarted += OnAnimationStarted;
                    animator.SetTrigger(data.AnimationTriggerParameter);
                }
            }

            public IEnumerator Update(EntityData data)
            {
                if ((data.ExecutionStage & executionStage) > 0)
                {
                    while (hasAnimationStarted == false)
                    {
                        yield return null;
                    }

                    while (isAnimationRunning)
                    {
                        yield return null;
                    }
                }
            }

            public void End(EntityData data)
            {
                if ((data.ExecutionStage & executionStage) > 0)
                {
                    animator.SetTrigger(data.AnimationTriggerParameter);
                }
                else
                {
                    animator.ResetTrigger(data.AnimationTriggerParameter);
                }
            }

            public void FastForward(EntityData data)
            {
                animator.Play(data.AnimationTriggerParameter, 0, 1);
                animator.ResetTrigger(data.AnimationTriggerParameter);
            }

            public void OnAnimationFinished(object sender, AnimationEvents.AnimationEventArgs animationEventArgs)
            {
                if (animationEventArgs.AnimationClipName == animationTriggerParameter)
                {
                    isAnimationRunning = false;
                }
            }

            private void OnAnimationStarted(object sender, AnimationEvents.AnimationEventArgs animationEventArgs)
            {
                if (animationEventArgs.AnimationClipName == animationTriggerParameter)
                {
                    hasAnimationStarted = true;
                    isAnimationRunning = true;
                }
            }
        }

        private IProcess<EntityData> process = new Process<EntityData>(new AnimationProcess(BehaviorExecutionStages.Activation), new EmptyStageProcess<EntityData>(), new AnimationProcess(BehaviorExecutionStages.Deactivation));

        protected override IProcess<EntityData> Process
        {
            get { return process; }
        }
    }

}

