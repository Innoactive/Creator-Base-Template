using System;
using UnityEngine;

namespace Innoactive.Hub.Training.Template.Animation
{
    /// <summary>
    /// Class that provides events for Animation Clips to get notified when a clip started and finished.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimationEvents : MonoBehaviour
    {
        /// <summary>
        /// Event fired when the animation clip has started.
        /// </summary>
        public event EventHandler<AnimationEventArgs> AnimationStarted;

        /// <summary>
        /// Event fired when the animation clip has finished.
        /// </summary>
        public event EventHandler<AnimationEventArgs> AnimationDone;

        private Animator animator;

        /// <summary>
        /// Animator related to the animations.
        /// </summary>
        public Animator Animator
        {
            get { return animator; }
        }

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public class AnimationEventArgs : EventArgs
        {
            public readonly string AnimationClipName;

            public AnimationEventArgs(string animationClipName)
            {
                AnimationClipName = animationClipName;
            }
        }

        public void AnimationClipDone(string animationClipName)
        {
            if (AnimationDone != null)
            {
                AnimationDone.Invoke(this, new AnimationEventArgs(animationClipName));
            }
        }

        public void AnimationClipStarted(string animationClipName)
        {
            if (AnimationStarted != null)
            {
                AnimationStarted.Invoke(this, new AnimationEventArgs(animationClipName));
            }
        }
    }
}
