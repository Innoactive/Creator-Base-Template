#if UNITY_EDITOR

using System.Collections;
using Innoactive.Hub.Training;
using Innoactive.Hub.Training.SceneObjects;
using Innoactive.Hub.Training.SceneObjects.Properties;
using Innoactive.Hub.Training.Template;
using Innoactive.Hub.Training.Utils;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Innoactive.Hub.Unity.Tests.Training.Template.Conditions
{
    public class PointedConditionTests : RuntimeTests
    {
        [UnityTest]
        public IEnumerator NotCompleteWithoutEvent()
        {
            // Given object with mocked pointing property,
            GameObject property = new GameObject("Property");
            GameObject target = new GameObject("Target");
            target.AddComponent<BoxCollider>().isTrigger = true;
            PointingProperty mockedProperty = property.AddComponent<PointingProperty>();
            ColliderWithTriggerProperty trigger = target.AddComponent<ColliderWithTriggerProperty>();

            PointedCondition condition = new PointedCondition(
                new ScenePropertyReference<PointingProperty>(TrainingReferenceUtils.GetNameFrom(mockedProperty)),
                new ScenePropertyReference<ColliderWithTriggerProperty>(TrainingReferenceUtils.GetNameFrom(trigger))
            );

            // When we activate the condition,
            condition.LifeCycle.Activate();

            yield return new WaitForFixedUpdate();

            // Then the condition is not completed.
            Assert.IsFalse(condition.IsCompleted);
        }

        [UnityTest]
        public IEnumerator CompleteOnEvent()
        {
            // Given object with mocked pointing property,
            GameObject property = new GameObject("Property");
            GameObject target = new GameObject("Target");
            target.AddComponent<BoxCollider>().isTrigger = true;
            PointingProperty mockedProperty = property.AddComponent<PointingProperty>();
            ColliderWithTriggerProperty trigger = target.AddComponent<ColliderWithTriggerProperty>();

            PointedCondition condition = new PointedCondition(
                new ScenePropertyReference<PointingProperty>(TrainingReferenceUtils.GetNameFrom(mockedProperty)),
                new ScenePropertyReference<ColliderWithTriggerProperty>(TrainingReferenceUtils.GetNameFrom(trigger))
            );

            // When we activate the condition and raise the event,
            condition.LifeCycle.Activate();
            mockedProperty.FastForwardPoint(trigger);

            // Then the condition is now completed.
            Assert.IsTrue(condition.IsCompleted);

            yield break;
        }

        [UnityTest]
        public IEnumerator FastForwardInactive()
        {
            // Given object with mocked pointing property,
            GameObject property = new GameObject("Property");
            GameObject target = new GameObject("Target");
            target.AddComponent<BoxCollider>().isTrigger = true;
            PointingProperty mockedProperty = property.AddComponent<PointingProperty>();
            ColliderWithTriggerProperty trigger = target.AddComponent<ColliderWithTriggerProperty>();

            PointedCondition condition = new PointedCondition(
                new ScenePropertyReference<PointingProperty>(TrainingReferenceUtils.GetNameFrom(mockedProperty)),
                new ScenePropertyReference<ColliderWithTriggerProperty>(TrainingReferenceUtils.GetNameFrom(trigger))
            );

            // When we fast-forward it,
            condition.LifeCycle.MarkToFastForward();

            // Then it doesn't activate by itself.
            Assert.AreEqual(Stage.Inactive, condition.LifeCycle.Stage);

            yield break;
        }

        [UnityTest]
        public IEnumerator FastForwardInactiveAndActivate()
        {
            // Given object with mocked pointing property,
            GameObject property = new GameObject("Property");
            GameObject target = new GameObject("Target");
            target.AddComponent<BoxCollider>().isTrigger = true;
            PointingProperty mockedProperty = property.AddComponent<PointingProperty>();
            ColliderWithTriggerProperty trigger = target.AddComponent<ColliderWithTriggerProperty>();

            PointedCondition condition = new PointedCondition(
                new ScenePropertyReference<PointingProperty>(TrainingReferenceUtils.GetNameFrom(mockedProperty)),
                new ScenePropertyReference<ColliderWithTriggerProperty>(TrainingReferenceUtils.GetNameFrom(trigger))
            );

            // When we fast-forward and activate it,
            condition.LifeCycle.MarkToFastForward();
            condition.LifeCycle.Activate();

            // Then it is deactivated and completed.
            Assert.AreEqual(Stage.Inactive, condition.LifeCycle.Stage);
            Assert.IsTrue(condition.IsCompleted);

            yield break;
        }

        [UnityTest]
        public IEnumerator FastForwardActive()
        {
            // Given object with mocked pointing property,
            GameObject property = new GameObject("Property");
            GameObject target = new GameObject("Target");
            target.AddComponent<BoxCollider>().isTrigger = true;
            PointingProperty mockedProperty = property.AddComponent<PointingProperty>();
            ColliderWithTriggerProperty trigger = target.AddComponent<ColliderWithTriggerProperty>();

            PointedCondition condition = new PointedCondition(
                new ScenePropertyReference<PointingProperty>(TrainingReferenceUtils.GetNameFrom(mockedProperty)),
                new ScenePropertyReference<ColliderWithTriggerProperty>(TrainingReferenceUtils.GetNameFrom(trigger))
            );

            // When we activate and then fast-forward it,
            condition.LifeCycle.Activate();
            condition.LifeCycle.MarkToFastForward();

            // Then it is deactivated and completed.
            Assert.AreEqual(Stage.Inactive, condition.LifeCycle.Stage);
            Assert.IsTrue(condition.IsCompleted);

            yield break;
        }
    }
}

#endif
