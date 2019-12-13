# Animation Behavior
The _Animation Behavior_ allows you to trigger animations which are setup through [Unity's Animation System](https://docs.unity3d.com/Manual/AnimationOverview.html). Therefore, you can start animations within a step to e.g. animate avatars or move objects around.

# Usage
To make use of the _Animation Behavior_ some important steps in the setup of objects have to be considered.

## Setup Objects
1. The object that will be animated and has the [_Animator_](https://docs.unity3d.com/Manual/class-Animator.html) attached needs an additional _AnimationEvents_ component
2. The _TrainingSceneObject_ needs an _AnimationProperty_ with a reference to the _AnimationEvents_

![Training Scene Object setup](.Docu/AnimationObjects.png)

## Setup Animations
1. Conditions of transitions within the [_Animator Controller_](https://docs.unity3d.com/Manual/class-AnimatorController.html) must be triggers and have the same name as the state that is transitioned to

![Animator Controller](.Docu/AnimatorSetup.PNG)

2. Every [_AnimationClip_](https://docs.unity3d.com/Manual/AnimationClips.html) must have an event in the beginning and when it is done. The called function of the event in the beginning must be set to _AnimationClipStarted_ of the _AnimationEvents_ component and the string parameter must be set to the string of the Trigger parameter. Do the same for the event at the end of the clip but use the _AnimationClipDone_ function.

![Animation Clip](.Docu/AnimationClip.png)

## Setup Behavior
Setting up the Behavior itself within the workflow editor requires a reference to the _AnimationProperty_. Additionally, add the string parameter you want to trigger which was created in the _Animator Controller_.

![Animation Clip](.Docu/AnimationBehavior.PNG)