#if UNITY_EDITOR

using System;
using System.Collections;
using Innoactive.Hub.Training;
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
        public IEnumerator CreateByReferenceTest()
        {
            // Given the path to the confetti machine prefab, the position provider name, the duration, the bool isAboveTrainee, the area radius, and the activation mode,
            GameObject target = new GameObject(positionProviderName);
            TrainingObject positionProvider = target.AddComponent<TrainingObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            BehaviorActivationMode mode = BehaviorActivationMode.ActivationAndDeactivation;

            // When we create ConfettiBehavior and pass training objects by reference,
            ConfettiBehavior confettiBehavior = new ConfettiBehavior(false, positionProvider, pathToPrefab, areaRadius, duration, mode);

            // Then all properties of the ConfettiBehavior are properly assigned
            Assert.AreEqual(false, confettiBehavior.IsAboveTrainee);
            Assert.AreEqual(positionProvider, confettiBehavior.PositionProvider.Value);
            Assert.AreEqual(pathToPrefab,confettiBehavior.ConfettiMachinePrefabPath);
            Assert.AreEqual(areaRadius, confettiBehavior.AreaRadius);
            Assert.AreEqual(duration, confettiBehavior.Duration);
            Assert.AreEqual(mode, confettiBehavior.ActivationMode);

            // Cleanup created game objects.
            Object.DestroyImmediate(target);

            yield return null;
        }

        [UnityTest]
        public IEnumerator CreateByNameTest()
        {
            // Given the path to the confetti machine prefab, the position provider name, the duration, the bool isAboveTrainee, the area radius, and the activation mode,
            GameObject target = new GameObject(positionProviderName);
            TrainingObject positionProvider = target.AddComponent<TrainingObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            BehaviorActivationMode mode = BehaviorActivationMode.ActivationAndDeactivation;

            // When we create ConfettiBehavior and pass training objects by their unique name,
            ConfettiBehavior confettiBehavior = new ConfettiBehavior(false, positionProviderName, pathToMockPrefab, areaRadius, duration, mode);

            // Then all properties of the MoveObjectBehavior are properly assigned.
            Assert.AreEqual(false, confettiBehavior.IsAboveTrainee);
            Assert.AreEqual(positionProvider, confettiBehavior.PositionProvider.Value);
            Assert.AreEqual(pathToMockPrefab,confettiBehavior.ConfettiMachinePrefabPath);
            Assert.AreEqual(areaRadius, confettiBehavior.AreaRadius);
            Assert.AreEqual(duration, confettiBehavior.Duration);
            Assert.AreEqual(mode, confettiBehavior.ActivationMode);

            // Cleanup created game objects.
            Object.DestroyImmediate(target);

            yield return null;
        }

        [UnityTest]
        public IEnumerator ActivationWithSpawnedMachineTest()
        {
            // Given a positive duration, a position provider, some valid default settings, and the activation mode = Activation,
            GameObject target = new GameObject(positionProviderName);
            TrainingObject positionProvider = target.AddComponent<TrainingObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            ConfettiBehavior behavior = new ConfettiBehavior(false, positionProvider, pathToPrefab, areaRadius, duration, BehaviorActivationMode.Activation);

            // When I activate that behavior and wait for one frame,
            behavior.Activate();
            yield return null;

            string prefabName = "Behavior" + pathToPrefab.Substring(pathToPrefab.LastIndexOf("/", StringComparison.Ordinal) + 1);
            GameObject machine = GameObject.Find(prefabName);

            // Then the activation state of the behavior is "activating" and the ConfettiMachine exists in the scene.
            Assert.AreEqual(ActivationState.Activating, behavior.ActivationState);
            Assert.IsTrue(machine != null);

            yield return new WaitForSeconds(duration + 0.1f);

            // Cleanup created game objects.
            Object.DestroyImmediate(target);

            yield return null;
        }

        [UnityTest]
        public IEnumerator RemovedMachineAfterPositiveDurationTest()
        {
            // Given a positive duration, a position provider, some valid default settings, and the activation mode = Activation,
            GameObject target = new GameObject(positionProviderName);
            TrainingObject positionProvider = target.AddComponent<TrainingObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            ConfettiBehavior behavior = new ConfettiBehavior(false, positionProvider, pathToPrefab, areaRadius, duration, BehaviorActivationMode.Activation);

            // When I activate that behavior and wait for one frame,
            behavior.Activate();
            yield return null;

            // And wait duration seconds,
            yield return new WaitForSeconds(duration + 0.1f);

            // Then behavior activation is completed, and the confetti machine should be deleted.
            Assert.AreEqual(ActivationState.Active, behavior.ActivationState);
            string prefabName = "Behavior" + pathToPrefab.Substring(pathToPrefab.LastIndexOf("/", StringComparison.Ordinal) + 1);
            Assert.IsTrue(GameObject.Find(prefabName) == null);

            // Cleanup created game objects.
            Object.DestroyImmediate(target);

            yield return null;
        }

        [UnityTest]
        public IEnumerator NegativeDurationTest()
        {
            // Given a negative duration, a position provider, some valid default settings, and the activation mode = Activation,
            float newDuration = -0.25f;

            GameObject target = new GameObject(positionProviderName);
            TrainingObject positionProvider = target.AddComponent<TrainingObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            ConfettiBehavior behavior = new ConfettiBehavior(false, positionProvider, pathToPrefab, areaRadius, newDuration, BehaviorActivationMode.Activation);

            // When I activate that behavior,
            behavior.Activate();
            yield return null;

            // Then behavior activation is immediately completed, and the confetti machine should be deleted.
            string prefabName = "Behavior" + pathToPrefab.Substring(pathToPrefab.LastIndexOf("/", StringComparison.Ordinal) + 1);
            GameObject machine = GameObject.Find(prefabName);

            Assert.AreEqual(ActivationState.Active, behavior.ActivationState);
            Assert.IsTrue(machine == null);

            // Cleanup created game objects.
            Object.DestroyImmediate(target);

            yield return null;
        }

        [UnityTest]
        public IEnumerator ZeroDurationTest()
        {
            // Given a duration equals zero, a position provider, some valid default settings, and the activation mode = Activation,
            float newDuration = 0f;

            GameObject target = new GameObject(positionProviderName);
            TrainingObject positionProvider = target.AddComponent<TrainingObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            ConfettiBehavior behavior = new ConfettiBehavior(false, positionProvider, pathToPrefab, areaRadius, newDuration, BehaviorActivationMode.Activation);

            // When I activate that behavior,
            behavior.Activate();
            yield return null;

            // Then behavior activation is immediately completed, and the confetti machine should be in the scene.
            string prefabName = "Behavior" + pathToPrefab.Substring(pathToPrefab.LastIndexOf("/", StringComparison.Ordinal) + 1);
            GameObject machine = GameObject.Find(prefabName);

            Assert.AreEqual(ActivationState.Active, behavior.ActivationState);
            Assert.IsTrue(machine == null);

            // Cleanup created game objects.
            Object.DestroyImmediate(target);

            yield return null;
        }

        [UnityTest]
        public IEnumerator SpawnAtPositionProviderTest()
        {
            // Given the position provider training object, some valid default settings, and the activation mode = Activation,
            GameObject target = new GameObject(positionProviderName);
            target.transform.position = new Vector3(5, 10, 20);
            TrainingObject positionProvider = target.AddComponent<TrainingObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            ConfettiBehavior behavior = new ConfettiBehavior(false, positionProvider, pathToPrefab, areaRadius, duration, BehaviorActivationMode.Activation);

            // When we activate the behavior,
            behavior.Activate();

            // Then the confetti machine is at the same position as the position provider
            string prefabName = "Behavior" + pathToPrefab.Substring(pathToPrefab.LastIndexOf("/", StringComparison.Ordinal) + 1);
            GameObject machine = GameObject.Find(prefabName);

            Assert.IsFalse(machine == null);
            Assert.IsTrue(machine.transform.position == target.transform.position);

            // Cleanup created game objects.
            Object.DestroyImmediate(target);
            Object.DestroyImmediate(machine);

            yield return null;
        }

        [UnityTest]
        public IEnumerator SpawnAboveTraineeTest()
        {
            // TODO: VRTK_DeviceFinder does not work in test scenes. So it is not possible to actually spawn the confetti machine above the trainee that way.

            yield return null;
        }

        [UnityTest]
        public IEnumerator StillActivatingWhenBlockingTest()
        {
            // Given the position provider training object, some valid default settings, and the activation mode = Activation,
            GameObject target = new GameObject(positionProviderName);
            TrainingObject positionProvider = target.AddComponent<TrainingObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            ConfettiBehavior behavior = new ConfettiBehavior(false, positionProvider, pathToPrefab, areaRadius, duration, BehaviorActivationMode.Activation);

            // When I activate that behavior and wait for one frame,
            behavior.Activate();
            yield return null;

            // Then the activation state of the behavior is "deactivating".
            Assert.AreEqual(ActivationState.Activating, behavior.ActivationState);

            // Cleanup created game objects.
            yield return new WaitForSeconds(duration + 0.1f);
            Object.DestroyImmediate(target);

            yield return null;
        }

        [UnityTest]
        public IEnumerator IsActiveAfterBlockingTest()
        {
            // Given the position provider training object, some valid default settings, and the activation mode = Activation,
            GameObject target = new GameObject(positionProviderName);
            TrainingObject positionProvider = target.AddComponent<TrainingObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            ConfettiBehavior behavior = new ConfettiBehavior(false, positionProvider, pathToPrefab, areaRadius, duration, BehaviorActivationMode.Activation);

            // When I activate that behavior and wait,
            behavior.Activate();
            yield return new WaitForSeconds(duration + 0.1f);

            // Then the activation state of the behavior is "active".
            Assert.AreEqual(ActivationState.Active, behavior.ActivationState);

            // Cleanup created game objects.
            Object.DestroyImmediate(target);

            yield return null;
        }

        [UnityTest]
        public IEnumerator StillDeactivatingWhenBlockingTest()
        {
            // Given the position provider training object, some valid default settings, and the activation mode = Deactivation,
            GameObject target = new GameObject(positionProviderName);
            TrainingObject positionProvider = target.AddComponent<TrainingObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            ConfettiBehavior behavior = new ConfettiBehavior(false, positionProvider, pathToPrefab, areaRadius, duration, BehaviorActivationMode.Deactivation);

            // When I activate, then immediately deactivate that behavior, and wait for one frame,
            behavior.Activate();
            behavior.Deactivate();
            yield return null;

            // Then the activation state of the behavior is "deactivating".
            Assert.AreEqual(ActivationState.Deactivating, behavior.ActivationState);

            // Cleanup created game objects.
            yield return new WaitForSeconds(duration + 0.1f);
            Object.DestroyImmediate(target);

            yield return null;
        }

        [UnityTest]
        public IEnumerator IsDeactivatedAfterBlockingTest()
        {
            // Given the position provider training object, some valid default settings, and the activation mode = Deactivation,
            GameObject target = new GameObject(positionProviderName);
            TrainingObject positionProvider = target.AddComponent<TrainingObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            ConfettiBehavior behavior = new ConfettiBehavior(false, positionProvider, pathToPrefab, areaRadius, duration, BehaviorActivationMode.Deactivation);

            // When I activate, then immediately deactivate that behavior, and wait for duration seconds,
            behavior.Activate();
            behavior.Deactivate();

            yield return new WaitForSeconds(duration + 0.1f);

            // Then the activation state of the behavior is "deactivated".
            Assert.AreEqual(ActivationState.Deactivated, behavior.ActivationState);

            // Cleanup created game objects.
            Object.DestroyImmediate(target);

            yield return null;
        }

        [UnityTest]
        public IEnumerator NotExistingPrefabTest()
        {
            // Given the position provider training object, an invalid path to a not existing prefab, some valid default settings, and the activation mode = Activation,
            GameObject target = new GameObject(positionProviderName);
            TrainingObject positionProvider = target.AddComponent<TrainingObject>();
            positionProvider.ChangeUniqueName(positionProviderName);

            ConfettiBehavior behavior = new ConfettiBehavior(false, positionProvider, pathToMockPrefab, areaRadius, duration, BehaviorActivationMode.Activation);

            // When I activate it,
            behavior.Activate();

            string prefabName = "Behavior" + pathToMockPrefab.Substring(pathToMockPrefab.LastIndexOf("/", StringComparison.Ordinal) + 1);
            GameObject machine = GameObject.Find(prefabName);

            // Then the activation state of the behavior is "active" and there is no confetti machine in the scene.
            Assert.AreEqual(ActivationState.Active, behavior.ActivationState);
            Assert.AreEqual(null, machine);

            yield return new WaitForSeconds(duration + 0.1f);

            // Cleanup created game objects.
            Object.DestroyImmediate(target);

            yield return null;
        }
    }
}

#endif
