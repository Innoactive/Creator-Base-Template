﻿using System.Collections;
using System.Runtime.Serialization;
using Innoactive.Hub.Training.Attributes;
using Innoactive.Hub.Training.Behaviors;
using Innoactive.Hub.Training.Configuration;
using Innoactive.Hub.Training.SceneObjects;
using Innoactive.Hub.Training.Utils;
using Newtonsoft.Json;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Innoactive.Hub.Training.Template
{
    /// <summary>
    /// This behavior causes confetti to rain.
    /// </summary>
    [DataContract(IsReference = true)]
    public class ConfettiBehavior : Behavior<ConfettiBehavior.EntityData>
    {
        [DisplayName("Spawn Confetti")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            /// <summary>
            /// Bool to check whether the confetti machine should spawn above the trainee or at the position of the position provider.
            /// </summary>
            [DataMember]
            [DisplayName("Spawn Above Trainee")]
            public bool IsAboveTrainee { get; set; }

            /// <summary>
            /// Name of the training object where to spawn the confetti machine.
            /// Only needed if "Spawn Above Trainee" is not checked.
            /// </summary>
            [DataMember]
            [DisplayName("Position Provider")]
            public SceneObjectReference PositionProvider { get; set; }

            /// <summary>
            /// Path to the desired confetti machine prefab.
            /// </summary>
            [DataMember]
            [DisplayName("Confetti Machine Path")]
            public string ConfettiMachinePrefabPath { get; set; }

            /// <summary>
            /// Radius of the spawning area.
            /// </summary>
            [DataMember]
            [DisplayName("Area Radius")]
            public float AreaRadius { get; set; }

            /// <summary>
            /// Duration of the animation in seconds.
            /// </summary>
            [DataMember]
            [DisplayName("Duration")]
            public float Duration { get; set; }

            /// <summary>
            /// Activation mode of this behavior.
            /// </summary>
            [DataMember]
            public BehaviorExecutionStages ExecutionStages { get; set; }

            public GameObject ConfettiMachine { get; set; }

            public Metadata Metadata { get; set; }
            public string Name { get; set; }
        }

        private static readonly Common.Logging.ILog logger = Logging.LogManager.GetLogger<ConfettiBehavior>();

        private const float defaultDuration = 15f;
        private const string defaultPathConfettiPrefab = "Confetti/Prefabs/InnoactiveConfettiMachine";
        private const float defaultRadius = 1f;
        private const float distanceAboveTrainee = 3f;

        [JsonConstructor]
        public ConfettiBehavior() : this(true, "", defaultPathConfettiPrefab, defaultRadius, defaultDuration, BehaviorExecutionStages.Activation)
        {
        }

        public ConfettiBehavior(bool isAboveTrainee, ISceneObject positionProvider, string confettiMachinePrefabPath, float radius, float duration, BehaviorExecutionStages executionStages)
            : this(isAboveTrainee, TrainingReferenceUtils.GetNameFrom(positionProvider), confettiMachinePrefabPath, radius, duration, executionStages)
        {
        }

        public ConfettiBehavior(bool isAboveTrainee, string positionProviderSceneObjectName, string confettiMachinePrefabPath, float radius, float duration, BehaviorExecutionStages executionStages)
        {
            Data = new EntityData
            {
                IsAboveTrainee = isAboveTrainee,
                PositionProvider = new SceneObjectReference(positionProviderSceneObjectName),
                ConfettiMachinePrefabPath = confettiMachinePrefabPath,
                AreaRadius = radius,
                Duration = duration,
                ExecutionStages = executionStages
            };
        }

        private class EmitConfettiProcess : IStageProcess<EntityData>
        {
            private readonly BehaviorExecutionStages stages;
            private float timeStarted;
            private GameObject confettiPrefab;

            public void Start(EntityData data)
            {
                if (ShouldExecuteCurrentStage(data) == false)
                {
                    return;
                }

                // Load the given prefab and stop the coroutine if not possible.
                confettiPrefab = Resources.Load<GameObject>(data.ConfettiMachinePrefabPath);

                if (confettiPrefab == null)
                {
                    logger.Warn("No valid prefab path provided.");
                    return;
                }

                // If the confetti rain should spawn above the player, get the position of the player's headset and raise the y coordinate a bit.
                // Otherwise, use the position of the position provider.
                Vector3 spawnPosition;

                if (data.IsAboveTrainee)
                {
                    spawnPosition = RuntimeConfigurator.Configuration.Trainee.GameObject.transform.position;
                    spawnPosition.y += distanceAboveTrainee;
                }
                else
                {
                    spawnPosition = data.PositionProvider.Value.GameObject.transform.position;
                }

                // Spawn the machine and check if it has the interface IParticleMachine
                data.ConfettiMachine = Object.Instantiate(confettiPrefab, spawnPosition, Quaternion.Euler(90, 0, 0));

                if (data.ConfettiMachine == null)
                {
                    logger.Warn("The provided prefab is missing.");
                    return;
                }

                data.ConfettiMachine.name = "Behavior" + confettiPrefab.name;

                if (data.ConfettiMachine.GetComponent(typeof(IParticleMachine)) == null)
                {
                    logger.Warn("The provided prefab does not have any component of type \"IParticleMachine\".");
                    return;
                }

                // Change the settings and activate the machine
                IParticleMachine particleMachine = data.ConfettiMachine.GetComponent<IParticleMachine>();
                particleMachine.Activate(data.AreaRadius, data.Duration);

                if (data.Duration > 0f)
                {
                    timeStarted = Time.time;
                }
            }

            public IEnumerator Update(EntityData data)
            {
                if (ShouldExecuteCurrentStage(data) == false)
                {
                    yield break;
                }

                if (confettiPrefab == null || data.ConfettiMachine == null || data.ConfettiMachine.GetComponent(typeof(IParticleMachine)) == null)
                {
                    yield break;
                }

                if (data.Duration > 0)
                {
                    while (Time.time - timeStarted < data.Duration)
                    {
                        yield return null;
                    }
                }
            }

            public void End(EntityData data)
            {
                if (ShouldExecuteCurrentStage(data) && data.ConfettiMachine != null && data.ConfettiMachine.Equals(null) == false)
                {
                    Object.Destroy(data.ConfettiMachine);
                    data.ConfettiMachine = null;
                }
            }

            public void FastForward(EntityData data)
            {
            }

            public EmitConfettiProcess(BehaviorExecutionStages stages)
            {
                this.stages = stages;
            }

            private bool ShouldExecuteCurrentStage(EntityData data)
            {
                return (data.ExecutionStages & stages) > 0;
            }
        }

        private readonly IProcess<EntityData> process = new Process<EntityData>(new EmitConfettiProcess(BehaviorExecutionStages.Activation), new EmptyStageProcess<EntityData>(), new EmitConfettiProcess(BehaviorExecutionStages.Deactivation));

        protected override IProcess<EntityData> Process
        {
            get
            {
                return process;
            }
        }
    }
}
