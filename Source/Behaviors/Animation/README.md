# Animation Behavior
The _Animation Behavior_ allows you to trigger animations which are setup through [Unity's Animation System](https://docs.unity3d.com/Manual/AnimationOverview.html). Therefore, you can start animations within a step to e.g. animate avatars or move objects around.

# Usage
To use the _Animation Behavior_, do as follows:

## Setup Objects
1. The object that will be animated and has the [_Animator_](https://docs.unity3d.com/Manual/class-Animator.html) component attached needs an additional _AnimationEvents_ component
2. The _TrainingSceneObject_ needs an _AnimationProperty_ with a reference to the _AnimationEvents_

![Training Scene Object setup](.Docu/AnimationObjects.png)

## Setup Animations
1. Only triggers can be conditions of transitions within the [_Animator Controller_](https://docs.unity3d.com/Manual/class-AnimatorController.html), and they should be named the same as the target state.

![Animator Controller](.Docu/AnimatorSetup.PNG)

2. Every [_AnimationClip_](https://docs.unity3d.com/Manual/AnimationClips.html) must have an event in the beginning and when it is done. The called function of the event in the beginning must be set to _AnimationClipStarted_ of the _AnimationEvents_ component and the string parameter must be set to the string of the Trigger parameter. Do the same for the event at the end of the clip but use the _AnimationClipDone_ function.

![Animation Clip](.Docu/AnimationClip.png)

## Setup Behavior
Reference the _AnimationProperty_ in the animation behavior. Set the string parameter to the same value as in the _Animator Controller_.

![Animation Clip](.Docu/AnimationBehavior.PNG)