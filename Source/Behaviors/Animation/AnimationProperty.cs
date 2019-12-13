using Innoactive.Hub.Training.SceneObjects.Properties;
using UnityEngine;

namespace Innoactive.Hub.Training.Template.Animation
{
    public class AnimationProperty : TrainingSceneObjectProperty
    {
        [SerializeField]
        private AnimationEvents animationEvents;

        public AnimationEvents AnimationEvents
        {
            get { return animationEvents; }
        }
    }
}
