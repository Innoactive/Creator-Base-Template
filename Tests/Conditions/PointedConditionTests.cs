using System.Collections;
using Innoactive.Creator.Core.Tests.Utils;
using Innoactive.Hub.Training;
using Innoactive.Hub.Training.Configuration.Modes;
using Innoactive.Hub.Training.SceneObjects;
using Innoactive.Hub.Training.SceneObjects.Properties;
using Innoactive.Hub.Training.Template;
using Innoactive.Hub.Training.Utils;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Innoactive.Creator.Template.Tests.Conditions
{
    public class PointedConditionTests : RuntimeTests
    {
        private readonly IMode defaultMode = new Mode("Default", new WhitelistTypeRule<IOptional>());

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
            condition.Configure(defaultMode);

            // When we activate the condition and wait one update cycle,
            condition.LifeCycle.Activate();

            while (condition.LifeCycle.Stage != Stage.Active)
            {
                yield return null;
                condition.Update();
            }

            yield return null;
            condition.Update();

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
            condition.Configure(defaultMode);

            // When we activate the condition and raise the event,
            condition.LifeCycle.Activate();

            while (condition.LifeCycle.Stage != Stage.Active)
            {
                yield return null;
                condition.Update();
            }

            mockedProperty.FastForwardPoint(trigger);

            yield return null;
            condition.Update();

            // Then the condition is now completed.
            Assert.IsTrue(condition.IsCompleted);
        }

        [UnityTest]
        public IEnumerator AutocompleteActive()
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
            condition.Configure(defaultMode);

            // When we activate and then autocomplete it,
            condition.LifeCycle.Activate();

            while (condition.LifeCycle.Stage != Stage.Active)
            {
                yield return null;
                condition.Update();
            }

            condition.Autocomplete();

            // Then it is completed.
            Assert.AreEqual(Stage.Active, condition.LifeCycle.Stage);
            Assert.IsTrue(condition.IsCompleted);
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
            condition.Configure(defaultMode);

            // When we activate and then fast-forward it,
            condition.LifeCycle.Activate();

            while (condition.LifeCycle.Stage != Stage.Active)
            {
                yield return null;
                condition.Update();
            }

            condition.LifeCycle.MarkToFastForward();

            // Then it is not completed and the stage is still active.
            Assert.IsFalse(condition.IsCompleted);
            Assert.AreEqual(Stage.Active, condition.LifeCycle.Stage);
        }
    }
}
