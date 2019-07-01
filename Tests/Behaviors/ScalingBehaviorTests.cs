#if UNITY_EDITOR

using Innoactive.Hub.Training;
using System.Collections;
using Innoactive.Hub.Training.Behaviors;
using Innoactive.Hub.Training.SceneObjects;
using Innoactive.Hub.Training.Template;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Innoactive.Hub.Unity.Tests.Training.Template.Behaviors
{
    public class ScalingBehaviorTests : RuntimeTests
    {
        private const string targetName = "TestReference";
        private readonly Vector3 newScale = new Vector3(15, 10, 7.5f);
        
        [UnityTest]
        public IEnumerator DoneAfterTime()
        {
            // Given a complete scaling behavior with a positive duration,
            const float duration = 0.05f;
            
            GameObject target = new GameObject(targetName);
            SceneObject positionProvider = target.AddComponent<SceneObject>();
            positionProvider.ChangeUniqueName(targetName);

            Vector3 endScale = target.transform.localScale + newScale;

            Behavior behavior = new ScalingBehavior(new SceneObjectReference(targetName), endScale, duration);

            // When we activate the behavior and wait for it's delay time,
            behavior.Activate();
            yield return new WaitForSeconds(duration);
            yield return null;

            // Then the behavior should be active and the object is scaled correctly.
            Assert.AreEqual(ActivationState.Active, behavior.ActivationState);
            Assert.IsTrue(target.transform.localScale == endScale);
        }

        [UnityTest]
        public IEnumerator RunsInstantlyWhenDelayTimeIsZero()
        {
            // Given a complete scaling behavior with duration time == 0,
            const float duration = 0f;
            
            GameObject target = new GameObject(targetName);
            SceneObject positionProvider = target.AddComponent<SceneObject>();
            positionProvider.ChangeUniqueName(targetName);

            Vector3 endScale = target.transform.localScale + newScale;

            Behavior behavior = new ScalingBehavior(new SceneObjectReference(targetName), endScale, duration);

            // When we activate it,
            behavior.Activate();

            // Then the behavior is activated immediately and the object is scaled correctly.
            Assert.AreEqual(ActivationState.Active, behavior.ActivationState);
            Assert.IsTrue(target.transform.localScale == endScale);

            yield break;
        }

        [UnityTest]
        public IEnumerator NegativeTimeCompletesImmediately()
        {
            // Given a complete scaling behavior with negative duration time,
            const float duration = -0.05f;
            
            GameObject target = new GameObject(targetName);
            SceneObject positionProvider = target.AddComponent<SceneObject>();
            positionProvider.ChangeUniqueName(targetName);

            Vector3 endScale = target.transform.localScale + newScale;

            Behavior behavior = new ScalingBehavior(new SceneObjectReference(targetName), endScale, duration);

            // When we activate it,
            behavior.Activate();

            // Then the behavior is activated immediately and the object is scaled correctly.
            Assert.AreEqual(ActivationState.Active, behavior.ActivationState);
            Assert.IsTrue(target.transform.localScale == endScale);

            yield break;
        }
        
        [UnityTest]
        public IEnumerator ZeroScaleCompletes()
        {
            // Given a complete scaling behavior with duration time == 0 and scale == (0, 0, 0),
            const float duration = 0f;
            
            GameObject target = new GameObject(targetName);
            SceneObject positionProvider = target.AddComponent<SceneObject>();
            positionProvider.ChangeUniqueName(targetName);

            Vector3 endScale = Vector3.zero;

            Behavior behavior = new ScalingBehavior(new SceneObjectReference(targetName), endScale, duration);

            // When we activate it,
            behavior.Activate();

            // Then the behavior is activated immediately and the object is scaled correctly.
            Assert.AreEqual(ActivationState.Active, behavior.ActivationState);
            Assert.IsTrue(target.transform.localScale == endScale);

            yield break;
        }
        
        [UnityTest]
        public IEnumerator NegativeScaleCompletes()
        {
            // Given a complete scaling behavior with duration time == 0 and scale == (-1, -1, -1),
            const float duration = 0f;
            
            GameObject target = new GameObject(targetName);
            SceneObject positionProvider = target.AddComponent<SceneObject>();
            positionProvider.ChangeUniqueName(targetName);

            Vector3 endScale = new Vector3(-1, -1, -1);

            Behavior behavior = new ScalingBehavior(new SceneObjectReference(targetName), endScale, duration);

            // When we activate it,
            behavior.Activate();

            // Then the behavior is activated immediately and the object is scaled correctly.
            Assert.AreEqual(ActivationState.Active, behavior.ActivationState);
            Assert.IsTrue(target.transform.localScale == endScale);

            yield break;
        }

        [UnityTest]
        public IEnumerator FastForwardInactiveBehavior()
        {
            // Given a complete scaling behavior with a positive duration,
            const float duration = 0.05f;
            
            GameObject target = new GameObject(targetName);
            SceneObject positionProvider = target.AddComponent<SceneObject>();
            positionProvider.ChangeUniqueName(targetName);

            Vector3 endScale = target.transform.localScale + newScale;

            Behavior behavior = new ScalingBehavior(new SceneObjectReference(targetName), endScale, duration);

            // When we mark it to fast-forward,
            behavior.MarkToFastForward();

            // Then it doesn't autocomplete because it hasn't been activated yet.
            Assert.AreEqual(ActivationState.Inactive, behavior.ActivationState);
            Assert.IsFalse(target.transform.localScale == endScale);

            yield break;
        }

        [UnityTest]
        public IEnumerator FastForwardInactiveBehaviorAndActivateIt()
        {
            // Given a complete scaling behavior with a positive duration,
            const float duration = 0.05f;
            
            GameObject target = new GameObject(targetName);
            SceneObject positionProvider = target.AddComponent<SceneObject>();
            positionProvider.ChangeUniqueName(targetName);

            Vector3 endScale = target.transform.localScale + newScale;

            Behavior behavior = new ScalingBehavior(new SceneObjectReference(targetName), endScale, duration);

            // When we mark it to fast-forward and activate it,
            behavior.MarkToFastForward();
            behavior.Activate();

            // Then the behavior is activated immediately and the object is scaled correctly.
            Assert.AreEqual(ActivationState.Active, behavior.ActivationState);
            Assert.IsTrue(target.transform.localScale == endScale);

            yield break;
        }

        [UnityTest]
        public IEnumerator FastForwardActivatingBehavior()
        {
            // Given an active and complete scaling behavior with a positive duration,
            const float duration = 0.05f;
            
            GameObject target = new GameObject(targetName);
            SceneObject positionProvider = target.AddComponent<SceneObject>();
            positionProvider.ChangeUniqueName(targetName);

            Vector3 endScale = target.transform.localScale + newScale;

            Behavior behavior = new ScalingBehavior(new SceneObjectReference(targetName), endScale, duration);

            behavior.Activate();

            // When we mark it to fast-forward,
            behavior.MarkToFastForward();

            // Then the behavior is activated immediately and the object is scaled correctly.
            Assert.AreEqual(ActivationState.Active, behavior.ActivationState);
            Assert.IsTrue(target.transform.localScale == endScale);

            yield break;
        }
    }
}

#endif