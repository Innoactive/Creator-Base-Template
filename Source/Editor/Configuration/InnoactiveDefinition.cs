using System.Collections.Generic;
using System.Collections.ObjectModel;
using Innoactive.Hub.Training.Editors.Configuration;
using UnityEngine;

namespace Innoactive.Hub.Training.Template.Editors.Configuration
{
    public class InnoactiveDefinition : DefaultDefinition
    {
        private class Confetti : Menu.Item<IBehavior>
        {
            public override GUIContent DisplayedName
            {
                get
                {
                    return new GUIContent("Innoactive/Spawn Confetti");
                }
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
                get
                {
                    return new GUIContent("Innoactive/Audio Hint");
                }
            }

            public override IBehavior GetNewItem()
            {
                return new ActivationBlockingBehavior(
                    new BehaviorSequence(true,
                        new List<IBehavior>
                        {
                            new ActivationBlockingBehavior(new DelayBehavior(5f) {Name = "Wait for"}, false),
                            new ActivationBlockingBehavior(new PlayAudioBehavior(new TextToSpeechAudio(new LocalizedString()), BehaviorActivationMode.Activation) {Name = "Play Audio"}, false)
                        }) {Name = "Audio Hint"}, false);
            }
        }

        private readonly IList<Menu.Option<IBehavior>> defaultBehaviors;

        public InnoactiveDefinition()
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
                new AudioHint(),
            };
        }

        public override ReadOnlyCollection<Menu.Option<IBehavior>> BehaviorsMenuContent
        {
            get
            {
                return new ReadOnlyCollection<Menu.Option<IBehavior>>(defaultBehaviors);
            }
        }
    }
}