#if UNITY_EDITOR

using System;
using System.Collections;
using Innoactive.Hub.Training;
using Innoactive.Hub.Training.Behaviors;
using Innoactive.Hub.Training.SceneObjects;
using Innoactive.Hub.Training.Template;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Innoactive.Hub.Unity.Tests.Training.Template.Behaviors
{
    public class ConfettiBehaviorTests : RuntimeTests
    {
        private const string pathToPrefab = "Confetti/Prefabs/InnoactiveConfettiMachine";
        private const string pathToMockPrefab = "Confetti/Prefabs/MockConfettiMachine";
        private const string positionProviderName = "Target Position";
        private const float duration = 0.15f;
        private const float areaRadius = 11f;

        [UnityTest]
        public IEnumerator CreateByReference()
        {
            // Given the path to the confetti machine prefab, the position provider name, the duration, the bool isAboveTrainee, the area radius, and the activation mode,
            GameObject target = new GameObject(positionProviderName);
            TrainingSceneObject positionProvider = target.AddComponent<TrainingSceneObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            BehaviorExecutionStages mode = BehaviorExecutionStages.ActivationAndDeactivation;

            // When we create ConfettiBehavior and pass training objects by reference,
            ConfettiBehavior confettiBehavior = new ConfettiBehavior(false, positionProvider, pathToPrefab, areaRadius, duration, mode);

            // Then all properties of the ConfettiBehavior are properly assigned
            Assert.AreEqual(false, confettiBehavior.Data.IsAboveTrainee);
            Assert.AreEqual(positionProvider, confettiBehavior.Data.PositionProvider.Value);
            Assert.AreEqual(pathToPrefab,confettiBehavior.Data.ConfettiMachinePrefabPath);
            Assert.AreEqual(areaRadius, confettiBehavior.Data.AreaRadius);
            Assert.AreEqual(duration, confettiBehavior.Data.Duration);
            Assert.AreEqual(mode, confettiBehavior.Data.ExecutionStages);

            yield break;
        }

        [UnityTest]
        public IEnumerator CreateByName()
        {
            // Given the path to the confetti machine prefab, the position provider name, the duration, the bool isAboveTrainee, the area radius, and the activation mode,
            GameObject target = new GameObject(positionProviderName);
            TrainingSceneObject positionProvider = target.AddComponent<TrainingSceneObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            BehaviorExecutionStages mode = BehaviorExecutionStages.ActivationAndDeactivation;

            // When we create ConfettiBehavior and pass training objects by their unique name,
            ConfettiBehavior confettiBehavior = new ConfettiBehavior(false, positionProviderName, pathToMockPrefab, areaRadius, duration, mode);

            // Then all properties of the MoveObjectBehavior are properly assigned.
            Assert.AreEqual(false, confettiBehavior.Data.IsAboveTrainee);
            Assert.AreEqual(positionProvider, confettiBehavior.Data.PositionProvider.Value);
            Assert.AreEqual(pathToMockPrefab,confettiBehavior.Data.ConfettiMachinePrefabPath);
            Assert.AreEqual(areaRadius, confettiBehavior.Data.AreaRadius);
            Assert.AreEqual(duration, confettiBehavior.Data.Duration);
            Assert.AreEqual(mode, confettiBehavior.Data.ExecutionStages);

            yield break;
        }

        [UnityTest]
        public IEnumerator ActivationWithSpawnedMachine()
        {
            // Given a positive duration, a position provider, some valid default settings, and the activation mode = Activation,
            GameObject target = new GameObject(positionProviderName);
            TrainingSceneObject positionProvider = target.AddComponent<TrainingSceneObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            ConfettiBehavior behavior = new ConfettiBehavior(false, positionProvider, pathToPrefab, areaRadius, duration, BehaviorExecutionStages.Activation);

            // When I activate that behavior and wait for one frame,
            behavior.LifeCycle.Activate();
            yield return null;

            string prefabName = "Behavior" + pathToPrefab.Substring(pathToPrefab.LastIndexOf("/", StringComparison.Ordinal) + 1);
            GameObject machine = GameObject.Find(prefabName);

            // Then the activation state of the behavior is "activating" and the ConfettiMachine exists in the scene.
            Assert.AreEqual(Stage.Activating, behavior.LifeCycle.Stage);
            Assert.IsTrue(machine != null);
        }

        [UnityTest]
        public IEnumerator RemovedMachineAfterPositiveDuration()
        {
            // Given a positive duration, a position provider, some valid default settings, and the activation mode = Activation,
            GameObject target = new GameObject(positionProviderName);
            TrainingSceneObject positionProvider = target.AddComponent<TrainingSceneObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            ConfettiBehavior behavior = new ConfettiBehavior(false, positionProvider, pathToPrefab, areaRadius, duration, BehaviorExecutionStages.Activation);

            // When I activate that behavior and wait for one frame,
            behavior.LifeCycle.Activate();
            yield return null;

            // And wait duration seconds,
            yield return new WaitForSeconds(duration + 0.1f);

            // Then behavior activation is completed, and the confetti machine should be deleted.
            Assert.AreEqual(Stage.Active, behavior.LifeCycle.Stage);
            string prefabName = "Behavior" + pathToPrefab.Substring(pathToPrefab.LastIndexOf("/", StringComparison.Ordinal) + 1);
            Assert.IsTrue(GameObject.Find(prefabName) == null);
        }

        [UnityTest]
        public IEnumerator NegativeDuration()
        {
            // Given a negative duration, a position provider, some valid default settings, and the activation mode = Activation,
            float newDuration = -0.25f;

            GameObject target = new GameObject(positionProviderName);
            TrainingSceneObject positionProvider = target.AddComponent<TrainingSceneObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            ConfettiBehavior behavior = new ConfettiBehavior(false, positionProvider, pathToPrefab, areaRadius, newDuration, BehaviorExecutionStages.Activation);

            // When I activate that behavior,
            behavior.LifeCycle.Activate();
            yield return null;

            // Then behavior activation is immediately completed, and the confetti machine should be deleted.
            string prefabName = "Behavior" + pathToPrefab.Substring(pathToPrefab.LastIndexOf("/", StringComparison.Ordinal) + 1);
            GameObject machine = GameObject.Find(prefabName);

            Assert.AreEqual(Stage.Active, behavior.LifeCycle.Stage);
            Assert.IsTrue(machine == null);
        }

        [UnityTest]
        public IEnumerator ZeroDuration()
        {
            // Given a duration equals zero, a position provider, some valid default settings, and the activation mode = Activation,
            float newDuration = 0f;

            GameObject target = new GameObject(positionProviderName);
            TrainingSceneObject positionProvider = target.AddComponent<TrainingSceneObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            ConfettiBehavior behavior = new ConfettiBehavior(false, positionProvider, pathToPrefab, areaRadius, newDuration, BehaviorExecutionStages.Activation);

            // When I activate that behavior,
            behavior.LifeCycle.Activate();
            yield return null;

            // Then behavior activation is immediately completed, and the confetti machine should be in the scene.
            string prefabName = "Behavior" + pathToPrefab.Substring(pathToPrefab.LastIndexOf("/", StringComparison.Ordinal) + 1);
            GameObject machine = GameObject.Find(prefabName);

            Assert.AreEqual(Stage.Active, behavior.LifeCycle.Stage);
            Assert.IsTrue(machine == null);

            // Cleanup created game objects.
            Object.DestroyImmediate(target);
        }

        [UnityTest]
        public IEnumerator SpawnAtPositionProvider()
        {
            // Given the position provider training object, some valid default settings, and the activation mode = Activation,
            GameObject target = new GameObject(positionProviderName);
            target.transform.position = new Vector3(5, 10, 20);
            TrainingSceneObject positionProvider = target.AddComponent<TrainingSceneObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            ConfettiBehavior behavior = new ConfettiBehavior(false, positionProvider, pathToPrefab, areaRadius, duration, BehaviorExecutionStages.Activation);

            // When we activate the behavior,
            behavior.LifeCycle.Activate();

            // Then the confetti machine is at the same position as the position provider
            string prefabName = "Behavior" + pathToPrefab.Substring(pathToPrefab.LastIndexOf("/", StringComparison.Ordinal) + 1);
            GameObject machine = GameObject.Find(prefabName);

            Assert.IsFalse(machine == null);
            Assert.IsTrue(machine.transform.position == target.transform.position);

            yield break;
        }

        [UnityTest]
        public IEnumerator SpawnAboveTrainee()
        {
            // TODO: VRTK_DeviceFinder does not work in test scenes. So it is not possible to actually spawn the confetti machine above the trainee that way.

            yield return null;
        }

        [UnityTest]
        public IEnumerator StillActivatingWhenBlocking()
        {
            // Given the position provider training object, some valid default settings, and the activation mode = Activation,
            GameObject target = new GameObject(positionProviderName);
            TrainingSceneObject positionProvider = target.AddComponent<TrainingSceneObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            ConfettiBehavior behavior = new ConfettiBehavior(false, positionProvider, pathToPrefab, areaRadius, duration, BehaviorExecutionStages.Activation);

            // When I activate that behavior and wait for one frame,
            behavior.LifeCycle.Activate();
            yield return null;

            // Then the activation state of the behavior is "deactivating".
            Assert.AreEqual(Stage.Activating, behavior.LifeCycle.Stage);
        }

        [UnityTest]
        public IEnumerator IsActiveAfterBlocking()
        {
            // Given the position provider training object, some valid default settings, and the activation mode = Activation,
            GameObject target = new GameObject(positionProviderName);
            TrainingSceneObject positionProvider = target.AddComponent<TrainingSceneObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            ConfettiBehavior behavior = new ConfettiBehavior(false, positionProvider, pathToPrefab, areaRadius, duration, BehaviorExecutionStages.Activation);

            // When I activate that behavior and wait,
            behavior.LifeCycle.Activate();
            yield return new WaitForSeconds(duration + 0.1f);

            // Then the activation state of the behavior is "active".
            Assert.AreEqual(Stage.Active, behavior.LifeCycle.Stage);
        }

        [UnityTest]
        public IEnumerator StillDeactivatingWhenBlocking()
        {
            // Given the position provider training object, some valid default settings, and the activation mode = Deactivation,
            GameObject target = new GameObject(positionProviderName);
            TrainingSceneObject positionProvider = target.AddComponent<TrainingSceneObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            ConfettiBehavior behavior = new ConfettiBehavior(false, positionProvider, pathToPrefab, areaRadius, duration, BehaviorExecutionStages.Deactivation);

            // When I activate, then immediately deactivate that behavior, and wait for one frame,
            behavior.LifeCycle.Activate();
            behavior.LifeCycle.Deactivate();
            yield return null;

            // Then the activation state of the behavior is "deactivating".
            Assert.AreEqual(Stage.Deactivating, behavior.LifeCycle.Stage);
        }

        [UnityTest]
        public IEnumerator IsDeactivatedAfterBlocking()
        {
            // Given the position provider training object, some valid default settings, and the activation mode = Deactivation,
            GameObject target = new GameObject(positionProviderName);
            TrainingSceneObject positionProvider = target.AddComponent<TrainingSceneObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            ConfettiBehavior behavior = new ConfettiBehavior(false, positionProvider, pathToPrefab, areaRadius, duration, BehaviorExecutionStages.Deactivation);

            // When I activate, then immediately deactivate that behavior, and wait for duration seconds,
            behavior.LifeCycle.Activate();
            behavior.LifeCycle.Deactivate();

            yield return new WaitForSeconds(duration + 0.1f);

            // Then the activation state of the behavior is "deactivated".
            Assert.AreEqual(Stage.Inactive, behavior.LifeCycle.Stage);
        }

        [UnityTest]
        public IEnumerator NotExistingPrefab()
        {
            // Given the position provider training object, an invalid path to a not existing prefab, some valid default settings, and the activation mode = Activation,
            GameObject target = new GameObject(positionProviderName);
            TrainingSceneObject positionProvider = target.AddComponent<TrainingSceneObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            ConfettiBehavior behavior = new ConfettiBehavior(false, positionProvider, pathToMockPrefab, areaRadius, duration, BehaviorExecutionStages.Activation);

            // When I activate it,
            behavior.LifeCycle.Activate();

            string prefabName = "Behavior" + pathToMockPrefab.Substring(pathToMockPrefab.LastIndexOf("/", StringComparison.Ordinal) + 1);
            GameObject machine = GameObject.Find(prefabName);

            // Then the activation state of the behavior is "active" and there is no confetti machine in the scene.
            Assert.AreEqual(Stage.Active, behavior.LifeCycle.Stage);
            Assert.AreEqual(null, machine);

            yield return new WaitForSeconds(duration + 0.1f);
        }

        [UnityTest]
        public IEnumerator FastForwardInactiveBehavior()
        {
            // Given a ConfettiBehavior with activation mode "Activation",
            GameObject target = new GameObject(positionProviderName);
            TrainingSceneObject positionProvider = target.AddComponent<TrainingSceneObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            ConfettiBehavior behavior = new ConfettiBehavior(false, positionProvider, pathToPrefab, areaRadius, duration, BehaviorExecutionStages.Activation);

            // When we mark it to fast-forward,
            behavior.LifeCycle.MarkToFastForward();

            // Then it doesn't autocomplete because it hasn't been activated yet.
            Assert.AreEqual(Stage.Inactive, behavior.LifeCycle.Stage);

            yield break;
        }

        [UnityTest]
        public IEnumerator FastForwardInactiveBehaviorAndActivateIt()
        {
            // Given a ConfettiBehavior with activation mode "Activation",
            GameObject target = new GameObject(positionProviderName);
            TrainingSceneObject positionProvider = target.AddComponent<TrainingSceneObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            ConfettiBehavior behavior = new ConfettiBehavior(false, positionProvider, pathToPrefab, areaRadius, duration, BehaviorExecutionStages.Activation);

            // When we mark it to fast-forward and activate it,
            behavior.LifeCycle.MarkToFastForward();
            behavior.LifeCycle.Activate();

            // Then it autocompletes immediately.
            Assert.AreEqual(Stage.Active, behavior.LifeCycle.Stage);

            yield break;
        }

        [UnityTest]
        public IEnumerator FastForwardInactiveBehaviorAndDeactivateIt()
        {
            // Given a ConfettiBehavior with activation mode "Deactivation",
            GameObject target = new GameObject(positionProviderName);
            TrainingSceneObject positionProvider = target.AddComponent<TrainingSceneObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            ConfettiBehavior behavior = new ConfettiBehavior(false, positionProvider, pathToPrefab, areaRadius, duration, BehaviorExecutionStages.Deactivation);

            // When we mark it to fast-forward, activate and immediately deactivate it,
            behavior.LifeCycle.MarkToFastForward();
            behavior.LifeCycle.Activate();

            yield return new WaitUntil(() => behavior.LifeCycle.Stage == Stage.Active);

            behavior.LifeCycle.Deactivate();

            // Then it autocompletes immediately.
            Assert.AreEqual(Stage.Inactive, behavior.LifeCycle.Stage);
        }

        [UnityTest]
        public IEnumerator FastForwardActivatingBehavior()
        {
            // Given an active ConfettiBehavior with activation mode "Activation",
            GameObject target = new GameObject(positionProviderName);
            TrainingSceneObject positionProvider = target.AddComponent<TrainingSceneObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            ConfettiBehavior behavior = new ConfettiBehavior(false, positionProvider, pathToPrefab, areaRadius, duration, BehaviorExecutionStages.Activation);

            behavior.LifeCycle.Activate();

            // When we mark it to fast-forward,
            behavior.LifeCycle.MarkToFastForward();

            // Then it autocompletes immediately.
            Assert.AreEqual(Stage.Active, behavior.LifeCycle.Stage);

            yield break;
        }

        [UnityTest]
        public IEnumerator FastForwardDeactivatingBehavior()
        {
            // Given an active ConfettiBehavior with activation mode "Deactivation",
            GameObject target = new GameObject(positionProviderName);
            TrainingSceneObject positionProvider = target.AddComponent<TrainingSceneObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            ConfettiBehavior behavior = new ConfettiBehavior(false, positionProvider, pathToPrefab, areaRadius, duration, BehaviorExecutionStages.Deactivation);

            behavior.LifeCycle.Activate();
            behavior.LifeCycle.Deactivate();

            // When we mark it to fast-forward,
            behavior.LifeCycle.MarkToFastForward();

            // Then it autocompletes immediately.
            Assert.AreEqual(Stage.Inactive, behavior.LifeCycle.Stage);

            yield break;
        }
    }
}

#endif
