using UnityEngine;
using System.Collections.Generic;
using Innoactive.Hub.Training.Audio;
using Innoactive.Creator.Internationalization;
using Innoactive.Hub.Training.Behaviors;
using Innoactive.Hub.Training.Editors.Configuration;

namespace Innoactive.Hub.Training.Template.Editors
{
    public class AudioHintMenuItem : Menu.Item<IBehavior>
    {
        public override GUIContent DisplayedName
        {
            get { return new GUIContent("Innoactive/Audio Hint"); }
        }

        public override IBehavior GetNewItem()
        {
            DelayBehavior delayBehavior = new DelayBehavior(5f);
            delayBehavior.Data.Name = "Wait for";

            PlayAudioBehavior audioBehavior = new PlayAudioBehavior(new TextToSpeechAudio(new LocalizedString()), BehaviorExecutionStages.Activation);
            audioBehavior.Data.Name = "Play Audio";

            BehaviorSequence behaviorSequence = new BehaviorSequence(true, new List<IBehavior>
            {
                delayBehavior,
                audioBehavior
            });
            behaviorSequence.Data.Name = "Audio Hint";

            behaviorSequence.Data.IsBlocking = false;

            return behaviorSequence;
        }
    }
}
