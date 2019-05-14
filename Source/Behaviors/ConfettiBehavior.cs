using System;
using System.Collections;
using System.Runtime.Serialization;
using Innoactive.Hub.Threading;
using Innoactive.Hub.Training.Utils;
using Newtonsoft.Json;
using UnityEngine;
using VRTK;
using Object = UnityEngine.Object;

namespace Innoactive.Hub.Training.Template
{
    /// <summary>
    /// This behavior causes confetti to rain.
    /// </summary>
    [DisplayName("Spawn Confetti")]
    [DataContract(IsReference = true)]
    public class ConfettiBehavior : Behavior
    {
        private static readonly Common.Logging.ILog logger = Logging.LogManager.GetLogger<ConfettiBehavior>();

        public class SpawnConfettiEventArg : EventArgs
        {
        }

        public event EventHandler<SpawnConfettiEventArg> ConfettiStarted;
        public event EventHandler<SpawnConfettiEventArg> ConfettiFinished;

        /// <summary>
        /// Bool to check whether the confetti machine should spawn above the trainee or at the position of the position provider.
        /// </summary>
        [DataMember]
        [DisplayName("Spawn Above Trainee")]
        public bool IsAboveTrainee { get; private set; }

        /// <summary>
        /// Name of the training object where to spawn the confetti machine.
        /// Only needed if "Spawn Above Trainee" is not checked.
        /// </summary>
        [DataMember]
        [DisplayName("Position Provider")]
        public TrainingObjectReference PositionProvider { get; private set; }

        /// <summary>
        /// Path to the desired confetti machine prefab.
        /// </summary>
        [DataMember]
        [DisplayName("Confetti Machine Path")]
        public string ConfettiMachinePrefabPath { get; private set; }

        /// <summary>
        /// Radius of the spawning area.
        /// </summary>
        [DataMember]
        [DisplayName("Area Radius")]
        public float AreaRadius { get; private set; }

        /// <summary>
        /// Duration of the animation in seconds.
        /// </summary>
        [DataMember]
        [DisplayName("Duration")]
        public float Duration { get; private set; }

        /// <summary>
        /// Activation mode of this behavior.
        /// </summary>
        [DataMember]
        public BehaviorActivationMode ActivationMode { get; private set; }

        private const float defaultDuration = 15f;
        private const string defaultPathConfettiPrefab = "Confetti/Prefabs/InnoactiveConfettiMachine";
        private const float defaultRadius = 1f;
        private const float distanceAboveTrainee = 3f;

        private IEnumerator coroutine;
        private GameObject confettiMachine;
        
        [JsonConstructor]
        public ConfettiBehavior() : this(true, "", defaultPathConfettiPrefab, defaultRadius, defaultDuration, BehaviorActivationMode.Activation)
        {
        }

        public ConfettiBehavior(bool isAboveTrainee, ITrainingObject positionProvider, string confettiMachinePrefabPath, float radius, float duration, BehaviorActivationMode activationMode)
            : this(isAboveTrainee, TrainingReferenceUtils.GetNameFrom(positionProvider), confettiMachinePrefabPath, radius, duration, activationMode)
        {
        }

        public ConfettiBehavior(bool isAboveTrainee, string positionProviderTrainingObjectName, string confettiMachinePrefabPath, float radius, float duration, BehaviorActivationMode activationMode)
        {
            IsAboveTrainee = isAboveTrainee;
            PositionProvider = new TrainingObjectReference(positionProviderTrainingObjectName);
            ConfettiMachinePrefabPath = confettiMachinePrefabPath;
            AreaRadius = radius;
            Duration = duration;
            ActivationMode = activationMode;
        }

        protected override void FastForwardActivating()
        {
            CoroutineDispatcher.Instance.StopCoroutine(coroutine);
            Object.Destroy(confettiMachine);
            EmitConfettiFinished();
        }

        protected override void FastForwardActive()
        {
        }

        protected override void FastForwardDeactivating()
        {
            CoroutineDispatcher.Instance.StopCoroutine(coroutine);
            Object.Destroy(confettiMachine);
            EmitConfettiFinished();
        }
        
        /// <inheritdoc />
        protected override void PerformActivation()
        {
            if ((ActivationMode & BehaviorActivationMode.Activation) > 0)
            {
                ConfettiFinished += OnConfettiFinishedOnActivation;
                coroutine = RainConfetti();
                CoroutineDispatcher.Instance.StartCoroutine(coroutine);
            }
            else
            {
                SignalActivationFinished();
            }
        }

        /// <inheritdoc />
        protected override void PerformDeactivation()
        {
            if ((ActivationMode & BehaviorActivationMode.Deactivation) > 0)
            {
                ConfettiFinished += OnConfettiFinishedOnDeactivation;
                coroutine = RainConfetti();
                CoroutineDispatcher.Instance.StartCoroutine(coroutine);
            }
            else
            {
                SignalDeactivationFinished();
            }
        }

        private void OnConfettiFinishedOnActivation(object sender, SpawnConfettiEventArg args)
        {
            ConfettiFinished -= OnConfettiFinishedOnActivation;
            SignalActivationFinished();
        }

        private void OnConfettiFinishedOnDeactivation(object sender, SpawnConfettiEventArg args)
        {
            ConfettiFinished -= OnConfettiFinishedOnDeactivation;
            SignalDeactivationFinished();
        }

        private void EmitConfettiStarted()
        {
            if (ConfettiStarted != null)
            {
                ConfettiStarted.Invoke(this, new SpawnConfettiEventArg());
            }
        }

        private void EmitConfettiFinished()
        {
            if (ConfettiFinished != null)
            {
                ConfettiFinished.Invoke(this, new SpawnConfettiEventArg());
            }
        }

        /// <summary>
        /// Coroutine which lets the confetti rain over time and then finishes the activation.
        /// </summary>
        private IEnumerator RainConfetti()
        {
            // Load the given prefab and stop the coroutine if not possible.
            GameObject confettiPrefab = Resources.Load<GameObject>(ConfettiMachinePrefabPath);

            if (confettiPrefab == null)
            {
                logger.Warn("No valid prefab path provided.");
                EmitConfettiFinished();
                yield break;
            }

            // If the confetti rain should spawn above the player, get the position of the player's headset and raise the y coordinate a bit.
            // Otherwise, use the position of the position provider.
            Vector3 spawnPosition;

            if (IsAboveTrainee)
            {
                // VRTK_DeviceFinder.HeadsetTransform throws an exception if you launch the training with an ExampleSimplestTrainingLoader.
                // Looks like it needs two frames to setup (during the first frame, the headset is enabled. During the second, it is actually registered).
                // yield return null twice for now, we'll refactor that later.
                yield return null;
                yield return null;
                spawnPosition = VRTK_DeviceFinder.HeadsetTransform().position;
                spawnPosition.y += distanceAboveTrainee;
            }
            else
            {
                spawnPosition = PositionProvider.Value.GameObject.transform.position;
            }

            // Spawn the machine and check if it has the interface IParticleMachine
            confettiMachine = Object.Instantiate(confettiPrefab, spawnPosition, Quaternion.Euler(90, 0, 0));

            if (confettiMachine == null)
            {
                logger.Warn("The provided prefab is missing.");
                EmitConfettiFinished();
                yield break;
            }

            confettiMachine.name = "Behavior" + confettiPrefab.name;

            if (confettiMachine.GetComponent(typeof(IParticleMachine)) == null)
            {
                logger.Warn("The provided prefab does not have any component of type \"IParticleMachine\".");
                Object.Destroy(confettiMachine);
                EmitConfettiFinished();
                yield break;
            }

            // Change the settings and activate the machine
            IParticleMachine particleMachine = confettiMachine.GetComponent<IParticleMachine>();
            particleMachine.Activate(AreaRadius, Duration);
            
            EmitConfettiStarted();

            if (Duration > 0f)
            {
                yield return new WaitForSeconds(Duration);
            }

            EmitConfettiFinished();
            UnityEngine.Object.Destroy(confettiMachine);
        }
    }
}
