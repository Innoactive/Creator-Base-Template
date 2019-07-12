using System.Collections.Generic;
using System.Collections.ObjectModel;
using Innoactive.Hub.Training.Audio;
using Innoactive.Hub.Training.Behaviors;
using Innoactive.Hub.Training.Conditions;
using Innoactive.Hub.Training.Editors.Configuration;
using UnityEngine;

namespace Innoactive.Hub.Training.Template.Editors.Configuration
{
    public class InnoactiveEditorConfiguration : DefaultEditorConfiguration
    {
        private class Confetti : Menu.Item<IBehavior>
        {
            public override GUIContent DisplayedName
            {
                get { return new GUIContent("Innoactive/Spawn Confetti"); }
            }

            public override IBehavior GetNewItem()
            {
                return new ConfettiBehavior();
            }
        }

        private class AudioHint : Menu.Item<IBehavior>
        {
            public override GUIContent DisplayedName
            {
                get { return new GUIContent("Innoactive/Audio Hint"); }
            }

            public override IBehavior GetNewItem()
            {
                return new ActivationBlockingBehavior(
                    new BehaviorSequence(true,
                        new List<IBehavior>
                        {
                            new DelayBehavior(5f) { Name = "Wait for" },
                            new PlayAudioBehavior(new TextToSpeechAudio(new LocalizedString()), BehaviorActivationMode.Activation) { Name = "Play Audio" }
                        }) { Name = "Audio Hint" }, false);
            }
        }

        private class Point : Menu.Item<ICondition>
        {
            public override GUIContent DisplayedName
            {
                get { return new GUIContent("Innoactive/Point Object"); }
            }

            public override ICondition GetNewItem()
            {
                return new PointedCondition();
            }
        }

        private readonly IList<Menu.Option<IBehavior>> defaultBehaviors;
        private readonly IList<Menu.Option<ICondition>> defaultConditions;

        public InnoactiveEditorConfiguration()
        {
            defaultBehaviors = new List<Menu.Option<IBehavior>>
            {
                new Menu.DefaultBehaviors.BehaviorSequence(),
                new Menu.Separator<IBehavior>(),
                new Menu.DefaultBehaviors.Disable(),
                new Menu.DefaultBehaviors.Enable(),
                new Menu.Separator<IBehavior>(),
                new Menu.DefaultBehaviors.Lock(),
                new Menu.DefaultBehaviors.Unlock(),
                new Menu.Separator<IBehavior>(),
                new Menu.DefaultBehaviors.Delay(),
                new Menu.DefaultBehaviors.MoveObject(),
                new Menu.DefaultBehaviors.Tts(),
                new Menu.DefaultBehaviors.ResourceAudio(),
                new Confetti(),
                new AudioHint()
            };

            defaultConditions = new List<Menu.Option<ICondition>>
            {
                new Menu.DefaultConditions.Touch(),
                new Menu.Separator<ICondition>(),
                new Menu.DefaultConditions.Grab(),
                new Menu.DefaultConditions.Release(),
                new Menu.Separator<ICondition>(),
                new Menu.DefaultConditions.Use(),
                new Menu.DefaultConditions.Snap(),
                new Menu.Separator<ICondition>(),
                new Menu.DefaultConditions.MoveIntoCollider(),
                new Menu.DefaultConditions.ObjectNearby(),
                new Menu.Separator<ICondition>(),
                new Menu.DefaultConditions.Timeout(),
                new Point()
            };
        }

        public override ReadOnlyCollection<Menu.Option<IBehavior>> BehaviorsMenuContent
        {
            get { return new ReadOnlyCollection<Menu.Option<IBehavior>>(defaultBehaviors); }
        }

        public override ReadOnlyCollection<Menu.Option<ICondition>> ConditionsMenuContent
        {
            get { return new ReadOnlyCollection<Menu.Option<ICondition>>(defaultConditions); }
        }
    }
}