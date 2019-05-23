# Introduction

The Hub Training Module is a part of the Innoactive Hub SDK. It's designed for large enterprises who train people to work with complex machinery. With it, you can train employees remotely, safely, and cost-efficiently. Our solution covers the complete lifecycle of virtual training applications, including their creation, maintenance, and distribution to the final user.

There are two user roles: a template developer and a training designer. Template developers have to be programmers, but training designers do not have to possess a deep technical knowledge. At first, a template developer performs initial configuration of the Training Module to meet the company's needs, creating a template. Then, training designers create training applications based on this template.

This document is intended for template developers.
 
This project is an example of a template. The following chapters describe to how to create a similar one. Use this project as a reference to validate your progress through this tutorial. Feel free to modify or discard it to create your own: this template is not meant to be extended.

# What is a training

The training is a linear sequence of chapters. Each chapter starts where the previous ends: if a trainee has to drill a hole in a wall in a first chapter, the hole will be there when you load the second chapter. You can start a training from any chapter.

Each chapter consists of steps that are connected to each other via transitions. Every step consists of a collection of behaviors and transitions. Behaviors are actions that execute independently from the trainee. For example, a behavior can play an audio or move an object from one point to another.

A transition may contain multiple conditions. With conditions, designers decide what trainees have to do to progress through the training. When all conditions are completed, the next step (defined in the transition) starts.

Behaviors and conditions communicate with objects on a scene through their training properties. A training property defines what actions are available for its scene object: for example, `GrabbableProperty` will inform `GrabbedCondition` that a trainee has grabbed the object. Conditions and behaviors use `TrainingObjectReference` and `TrainingPropertyReference<TProperty>` classes to reference training objects and their properties. Never use Unity game objects directly.

# Initial setup

## Setup the Hub SDK

Hub SDK is used for the development of Innoactive Hub VR applications. Check the [pre-requisites](http://docs.hub.innoactive.de/articles/sdk-setup.html#prerequisites) and follow the [instructions](http://docs.hub.innoactive.de/articles/sdk-setup.html#importing-the-hub-sdk) to set it up.

## Install VR SDKs

Install the SDKs of the VR headsets you're working with. For example, [SteamVR SDK](https://github.com/Innoactive/SteamVR).

## Import the Training Module

You can find the latest stable version at [Innoactive Hub Developer Portal](http://developers.innoactive.de/components/#training-module).

# The simplest training template

The simplest template possible consists of a single scene that contains a Innoactive Hub SDK and Training Module configuration, preconfigured VRTK headset, and an object with a single script that would load a training.

All that a training designer would have to do only two steps:

1) Copy the scene and populate it with training objects he needs.
2) Create a training, save it anywhere in `Assets` folder, and reference the saved file from the scene.

## Create a scene

Create a new Unity scene in which you want a VR training to be executed.

## Setup the scene as a Training Scene

Select the following option in the Unity editor's toolbar: `Innoactive > Training > Setup Scene as a Training Scene`.

## Create a script

Create a new C# script named TrainingLoader and replace its contents with the following code snippet:

```c#
   using System.Collections;
   using Innoactive.Hub.Training.Utils.Serialization;
   using UnityEngine;
   
   namespace Innoactive.Hub.Training.Template
   {
       public class TrainingLoader : MonoBehaviour
       {
           [SerializeField]
           [Tooltip("Text asset with saved training.")]
           private TextAsset serializedTraining;
   
           private IEnumerator Start()
           {
               // Skip the first two frames to give VRTK time to initialize.
               yield return null;
               yield return null;
   		
               // Load a training from the text asset
               ITraining training = JsonTrainingSerializer.Deserialize(serializedTraining.text);
   
               // Start the training execution
               training.Activate();
           }
       }
   }
```

## Add a training loader to the scene

1) Add a new empty game object to the scene.
2) Add the `Trainer Loader` component to it.

## The complete example

To verify the achieved result, you can compare it to the scene under the following path: `[Path to Innoactive training template]/Scenes/Simple`.

# Advanced topics

Cheers! You've covered the basics.

The following chapters describe available Training Module features and explain how to use them to create more complex templates. You can tune both training designer tools and a running application logic with Training module configurations, or you can extend it with your own behaviors and conditions.

 We recommend you to explore this project while you are reading this tutorial.

# Template configuration

You can either modify training designer tools with editor configuration, or you can define how a built training application would look and behave like. 

## Editor configuration

To change the editor configuration, implement `Innoactive.Hub.Training.Editors.Configuration.IDefinition` interface. The Training Module will automatically locate it if you do so, and will use the `DefaultDefinition` otherwise. If there are more than one custom editor configuration definitions, an undefined behavior occurs.

 > This is the main reason why we recommend to build a template from scratch instead of extending this project.

The `DefaultDefinition` configures the Training Module in a basic way. Inherit from it to not implement everything from scratch. Take a look at `InnoactiveDefinition` to see how to implement complex things like `Audio Hint` menu option.

With editor configuration, you can limit or allow training designers to use certain behaviors and conditions. For example, you might add a custom behavior that highlights its target object in a special way, and hide the default `Highlight` behavior.

1. `BehaviorsMenuContent` property defines a dropdown menu which is shown when a training designer clicks on `Add Behavior` button.
2. `ConditionsMenuContent` property defines a dropdown menu which is shown when a training designer clicks on `Add Condition` button.

Finally, you define what happens when someone clicks at `Innoactive > Training > Setup Scene` menu option in the `SetupTrainingScene()` method. By default, it set ups the Hub SDK and creates a training configuration object.

## Training configuration

You can use the training configuration to adjust the way the training application executes in a runtime.
 
There should be one and only one training configuration scene object in a scene. This object is just a container for the configuration definition, which actually customizes the template. You can assign the definition either programmatically, or with a game object inspector. By default, the `Innoactive.Hub.Training.Configuration.DefaultDefinition` is used. Extend it to create your own, and then assign it to the scene object.
 
The definition has the following properties and methods:

1. `TrainingObjectRegistry` provides the access to all training objects and properties of the current scene.
2. `Trainee` is a shortcut to a trainee's headset.
3. `InstructionPlayer` is a default audio source for all `PlayAudioBehavior` behaviors.
4. `TextToSpeechConfig` defines a TTS engine, voice, and language to use to generate audio.
1. `SetMode(index)` sets current mode to the one at provided `index` in the collection of available modes.
2. `GetCurrentMode()` returns the current mode.
3. `GetCurentModeIndex()` returns the current mode's index.
4. `AvailableModes` returns a collection of all modes available. Normally, this is a single modes-related class member you want to override.

The next chapters explain the TTS configuration and training modes in detail.

## TTS configuration

The text-to-speech config defines the TTS engine, voice, and language to use. By default, the training configuration loads TTS config from `[YOUR_PROJECT_ROOT_FOLDER]/Config/text-to-speech-config.json` file (if there is any). A different TTS config can be set at runtime, but it will have no effect until you reload the current training.

The `Provider` property contains the name of a TTS provider class. For example, `MicrosoftSapiTextToSpeechProvider` uses the Windows TTS engine (works offline), and `WatsonTextToSpeechProvider` uses the Watson TTS (requires internet connection). 

The acceptable values for the `Voice` and the `Language` properties differ from provider to provider. For online TTS engines, the `Auth` property may be required, as well.

### Using the offline Windows TTS

Windows 10 has a built-in speech synthesizer. It doesn't require an internet connection but a respective Language Pack has to be installed at end user's system (`Windows Settings > Time and Language > Language > Add a language`).

The `Language` field of a config takes either natural name of the language or [its two-letter ISO language code](https://msdn.microsoft.com/en-us/library/cc233982.aspx). Valid values of a `Voice` field are `Male`, `Female`, and `Neutral`.

> Despite the name, some two-letter ISO codes are three letters long.

An example of a proper config:

```c#
new TextToSpeechConfig()
{
    // Define which TTS provider is used.
    Provider = typeof(MicrosoftSapiTextToSpeechProvider).Name,
    
    Voice = "Female",
    
    Language = "EN"
};
```

## Localization

The Training SDK uses the `Localization` class from the Hub SDK. It's a wrapper around a dictionary of strings with convenient API. To define a current localization, either assign `entries` property directly, or load it from a JSON file with the `LoadLocalization(string path)` method. The valid JSON file complies to the following key-value structure:

```json
{
  "translated_text": "Ein übersetzter Text.",
  "example": "Ein Beispiel." 
}
```

> The `Localization` class is not concerned about current language or which localization file should be loaded. You have to manage it in the template.

Whenever you want to localize a `string` value, replace it with a `LocalizedString`. When `Value` propertiy is invoked, it searches for a localization entry by the `Key` and returns the result. If `Key` isn't specified or the entry is missing, it uses the `DefaultText` instead. 

## Instruction player

A `PlayAudioBehavior` uses the value of the `TrainingConguration.InstructionPlayer` property as its audio source. By default, the property attaches an audio source to the trainee's headset.

## Training modes

Some conditions and behaviors reference the current training mode for custom parameters. For example, parameters can define in which color the object has to be highlighted. The training mode parameters is a string-to-object dictionary. Additionally, the current mode defines which behaviors and conditions should be entirely skipped.

The default training definition has only one available mode. It allows any condition or behavior, and has no parameters. To define your own training modes, override the `AvailableModes` property in your training configuration definition. To switch between modes, call `SetMode(index)` method. Use the `Mode` class constructor to create a mode.

# Extend the Training Module

The default behaviors and conditions are sufficient for most of the trainings, but you might have to write your own to handle very specific cases.

> To name one, we do not provide the "Is that sheep shaved?" condition. 

In this chapter we will create a behavior that changes the scale of a target object, and a condition that triggers when a trainee points at a given object with a laser pointer.

## Custom training property

Behaviors and conditions communicate with objects on the scene through their properties. To create a pointing condition, we need to create a property for a pointer object first. Create new C# script named `PointingProperty` and set its contents to the following:

```c#
using System;
using UnityEngine;
using VRTK;

namespace Innoactive.Hub.Training.Template
{
    // PointingProperty requires VRTK_Pointer to work.
    [RequireComponent(typeof(VRTK_Pointer))]
    // Training object with that property can point at other training objects.
    // Any property should inherit from TrainingObjectProperty class.
    public class PointingProperty : TrainingObjectProperty
    {
        // Event that is invoked every time when the object points at something.
        public event Action<ColliderWithTriggerProperty> PointerEnter;

        // Reference to attached VRTK_Pointer.
        private VRTK_Pointer pointer;

        // Fake the pointing at target. Used when you fast-forward PointedCondition.
        public virtual void FastForwardPoint(ColliderWithTriggerProperty target)
        {
            if (target != null && PointerEnter != null)
            {
                PointerEnter(target);
            }
        }

        // Unity callback method
        protected override void OnEnable()
        {
            // Training object property handle their initialization at OnEnable().
            base.OnEnable();

            // Find attached VRTK_Pointer.
            pointer = GetComponent<VRTK_Pointer>();

            // Subscribe to VRTK_Pointer's event which is raised when it hits any collider.
            pointer.DestinationMarkerEnter += PointerOnDestinationMarkerEnter;
        }

        // Unity callback method
        private void OnDisable()
        {
            // Unsubscribe from VRTK_Pointer's event.
            pointer.DestinationMarkerEnter -= PointerOnDestinationMarkerEnter;
        }

        // VRTK_Pointer.DestinationMarkerEnter handler.
        private void PointerOnDestinationMarkerEnter(object sender, DestinationMarkerEventArgs e)
        {
            // If target is ColliderWithTriggerProperty, raise PointerEnter event.
            ColliderWithTriggerProperty target = e.target.GetComponent<ColliderWithTriggerProperty>();
            if (target != null && PointerEnter != null)
            {
                PointerEnter(target);
            }
        }
    }
}
```

This property does the following:

* It ensures that a scene object has all required components attached.
* It encapsulates the VRTK_Pointer event handling.
* It exposes an event so a training condition could use it, and it ensures that the event is fired only when the pointer points at a training object with a collider or the condition was fast-forwarded.

Now it can be attached to a game object on a scene. To save time on making your own pointer tool, you copy `Your Hub SDK Directory\SDK\Tools\Presenter\Resources\Presenter` and attach the property to its `Pointer` child object.

> Note that we expect the `ColliderWithTriggerProperty` to be attached to the training object we point at. VRTK_Pointer expects an object to have a collider with a trigger, and the `ColliderWithTriggerProperty` ensures that its owner object has one.

## Custom condition

Create new C# script named `PointedCondition` and change its contents to the following:

```c#
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Innoactive.Hub.Training.Template
{
    [DataContract(IsReference = true)]
    [DisplayName("Point at Object")]
    // Condition which is completed when Pointer points at Target.
    public class PointedCondition : Condition
    {
        [DataMember]
        // Reference to a pointer property.
        public TrainingPropertyReference<PointingProperty> Pointer { get; private set; }

        [DisplayName("Target with a collider")]
        [DataMember]
        // Reference to a target property.
        public TrainingPropertyReference<ColliderWithTriggerProperty> Target { get; private set; }

        [JsonConstructor]
        // Make sure that references are initialized.
        public PointedCondition() : this(new TrainingPropertyReference<PointingProperty>(), new TrainingPropertyReference<ColliderWithTriggerProperty>())
        {
        }

        public PointedCondition(TrainingPropertyReference<PointingProperty> pointer, TrainingPropertyReference<ColliderWithTriggerProperty> target)
        {
            Pointer = pointer;
            Target = target;
        }

        // This method is called when the step with that condition has completed activation of its behaviors.
        protected override void PerformActivation()
        {
            Pointer.Value.PointerEnter += OnPointerEnter;
            SignalActivationFinished();
        }

        // This method is called at deactivation of the step, after every behavior has completed its deactivation.
        protected override void PerformDeactivation()
        {
            Pointer.Value.PointerEnter -= OnPointerEnter;
            SignalDeactivationFinished();
        }

        // When a condition or behavior is fast-forwarded, the activation has to complete immediately.
        // This method should handle it, but since the activation is instanteneous,
        // It doesn't require any additional actions.
        protected override void FastForwardActivating()
        {
        }

        // When fast-forwarded, a conditions should complete immediately.
        // For that, the pointer fakes that it pointed at the target.
        protected override void FastForwardActive()
        {
            Pointer.Value.FastForwardPoint(Target);
        }

        // When a condition or behavior is fast-forwarded, the deactivation has to complete immediately.
        // This method should handle it, but since the deactivation is instanteneous,
        // It doesn't require any additional actions.
        protected override void FastForwardDeactivating()
        {
        }

        // When PointerProperty points at something,
        private void OnPointerEnter(ColliderWithTriggerProperty pointed)
        {
            // Ignore it if this condition is already fulfilled.
            if (IsCompleted)
            {
                return;
            }

            // Else, if Target references the pointed object, complete the condition.
            if (Target.Value == pointed)
            {
                MarkAsCompleted();
            }
        }
    }
}
```

All conditions should inherit from the  `Condition` abstract class. To initialize a condition, implement the `PerformActivation()` method. This condition subscribes to a `PointerEnter` event of a referenced `PointingProperty`. When the Pointer points at the target, the condition will mark itself as complete. To deinitialize, implement the `PerformDeactivation()` method. In both methods, you have to call `SignalActivationFinished()` and `SignalDeactivationFinished()`, respectively.

Every condition should be able to complete immediately if `FastForwardActive()` method is called. In this case, we fake that the target was actually pointed at. To do so, we call the `FastForwardPoint()` method that we implemented in the previous chapter. Fast-forwarding allows us to load chapters, skip steps, and change modes.

## Custom behavior

Create new C# script named `ScalingBehavior` and change its contents to the following:

```
using System.Collections;
using System.Runtime.Serialization;
using Innoactive.Hub.Threading;
using Newtonsoft.Json;
using UnityEngine;

namespace Innoactive.Hub.Training.Template
{
    // This behavior linearly changes scale of a Target object over Duration seconds, until it matches TargetScale.
    [DataContract(IsReference = true)]
    [DisplayName("Scale Object")]
    public class ScalingBehavior : Behavior
    {
        // Training object to scale.
        [DataMember]
        public TrainingObjectReference Target { get; private set; }

        // Target scale.
        [DataMember]
        [DisplayName("Target Scale")]
        public Vector3 TargetScale { get; private set; }

        // Duration of the animation in seconds.
        [DataMember]
        [DisplayName("Animation Duration")]
        public float Duration { get; private set; }

        // A coroutine responsible for scaling the target.
        private IEnumerator coroutine;
        
        // Handle data initialization in the constructor.
        [JsonConstructor]
        public ScalingBehavior() : this(new TrainingObjectReference(), Vector3.one, 0f)
        {
        }

        public ScalingBehavior(TrainingObjectReference target, Vector3 targetScale, float duration)
        {
            Target = target;
            TargetScale = targetScale;
            Duration = duration;
        }
        
        // Called on activation of the training entity. Define activation logic here.
        // You have to call `SignalActivationStarted()` at the start
        // and `SignalActivationFinished()` after you've done everything you wanted to do during the activation.
        protected override void PerformActivation()
        {
            // Start coroutine which will scale our object.
            coroutine = ScaleTarget();
            CoroutineDispatcher.Instance.StartCoroutine(coroutine);
        }

        // Called on deactivation of the training entity. Define deactivation logic here.
        // You have to call `SignalDeactivationStarted()` at the start
        // and `SignalDeactivationFinished()` after you've done everything you wanted to do during the deactivation.
        protected override void PerformDeactivation()
        {
            SignalDeactivationFinished();
        }

        // This method is called when the activation has to be interrupted and completed immediately.
        protected override void FastForwardActivating()
        {
            // If the scaling behavior is currently activating (running),
            if (ActivationState == ActivationState.Activating)
            {
                // Stop the scaling coroutine,
                CoroutineDispatcher.Instance.StopCoroutine(coroutine);

                // Scale the target manually,
                Target.Value.GameObject.transform.localScale = TargetScale;

                // And signal that activation is finished.
                SignalActivationFinished();
            }
        }
        
        // It requires no additional action.
        protected override void FastForwardActive()
        {
        }

        // Deactivation is instanteneous.
        // It requires no additional action.
        protected override void FastForwardDeactivating()
        {
        }
        
        // Coroutine which scales the target transform over time and then finished the activation.
        private IEnumerator ScaleTarget()
        {
            float startedAt = Time.time;

            Transform scaledTransform = Target.Value.GameObject.transform;

            Vector3 initialScale = scaledTransform.localScale;

            while (Time.time - startedAt < Duration)
            {
                float progress = (Time.time - startedAt) / Duration;

                scaledTransform.localScale = Vector3.Lerp(initialScale, TargetScale, progress);
                yield return null;
            }

            scaledTransform.localScale = TargetScale;

            SignalActivationFinished();
        }
    }
}
```

All behaviors should inherit from the `Behavior` class. Similarly to [conditions](#custom-condition), they should implement `PerformActivation()` and `PerformDeactivation()` methods, as well as means to fast-forward it. 

Note the major difference from the [condition](#custom-condition) example: instead of activating immediately, this behavior starts a coroutine and calls the `SignalActivationFinished()` method only at the end of it. It allows to create behaviors and conditions that take some time to activate or deactivate. 

> Use `CoroutineDispatcher.Instance.StartCoroutine(coroutine)` to start coroutines.

The other difference is that behaviors are simply idling when they are active. The actual work happens either at activation or deactivation.


## Shared considerations for behaviors and conditions

* The Training Module uses Newtonsoft.Json to serialize and preserve trainings. Note the `DataContract`, `DataMember`, and `JsonConstructor` attributes: they denote which properties of the condition are serialized and thus saved.
* If you make a behavior or condition to implement an `IOptional` interface, you will be able to skip it with training modes.
* If you want this condition to available to training designers, you have to adjust the [editor configuration](#editor-configuration) accordingly. The same is true for behaviors.
* Conditions and behaviors never reference training objects or properties directly: they use instances of `TrainingObjectReference` and `TrainingPropertyReference<TProperty>` classes instead. It makes trainings independent from scenes. References locate training objects by their unique names.

## Mode parameters

To customize a behavior or condition with mode parameters, declare a property of a `ModeParameter` type. It automatically fetches the training mode parameter by its `key`. If the current mode does not define the parameter, it uses the default value instead. You can subscribe to the `ParameterModified` event to handle the training mode change.

For example, you might create a giant glowing arrow and change its color depending on the current training mode.

See the following code snippet for the reference:

```c#
// Declare the property.
public ModeParameter<bool> IsShowingHighlight { get; private set; }

// Initialise the property (typically in constructor, at Awake() or OnEnable()).
protected void Initialise()
{
    // Create a new mode parameter that binds to a training mode entry with a key `ShowSnapzoneHighlight`. 
    // It expects the value to be a bool, and if it isn't defined, it uses `true` as a default value.
    IsShowingHighlight = new ModeParameter<bool>("ShowSnapzoneHighlight", true);
    
    // Perform necessary changes 
    IsShowingHighlight.ParameterModified += (sender, args) =>
    {
        HighlightObject.SetActive(IsShowingHighlight.Value);
    };
}
```

## Custom drawers

You can customize the way behaviors and conditions are displayed. To make a new drawer, you have to implement the `ITrainingDrawer` interface. If you want to use the drawer as a default drawer for all data members of a type, use `[DefaultTrainingDrawer(Type type)]` attribute. 

For an example, see `BehaviorDrawer` class.

Keep the following in mind:

* Drawers use Unity Editor IMGUI.
* Only one instance per drawer type is created. Do not store any information that is related to a specific object.
* Inherit from an `AbstractDrawer` instead of `ITrainingDrawer`, as it properly implements `ChangeValue` method and one of the `Draw` overloads.
* Pass the value assignment logic via `changeValueCallback` parameter of the `Draw` method.
* Never invoke that callback directly: either pass it to a child drawer, or call a `ChangeValue` with it as a parameter.
* Call `ChangeValue` only when the *current* member has changed, not one of its children.
* Call `ChangeValue` only when the current member *has* changed, because you will clutter Undo stack otherwise.

For example, that's how `ListDrawer` handles a new entry being added to it:

```c# 
// When user clicks "Add new item" button...
if (GUI.Button(rect, "Add"))
{
    // Define function that results in a list with a new element added.
    Func<object> getListWithAddedElement = () =>
    {
        InsertIntoList(ref list, list.Count, ReflectionUtils.GetDefault(entryDeclaredType));
        return list;
    };

    // Define function that results in a previous version of a list (with freshly added element removed back).
    Func<object> getListWithRemovedElement = () =>
    {
        RemoveFromList(ref list, list.Count - 1);
        return list;
    };

    // changeValueCallback contains the logic required to assign a value to the drawn property or field.
    // It takes a value that has to be assigned as its argument.
    // ChangeValue will create a new undoable command.
    // When it is performed, it invokes changeValueCallback with the result of getListWithAddedElement.
    // When it is reverted, it invokes changeValueCallback with the result of GetListWithRemovedElement.
    ChangeValue(getListWithAddedElement, getListWithRemovedElement, changeValueCallback);
}
```

And that's how it draws the elements of the list:

```c#
entry = listBeingDrawn[index];

// Define the action that has to be performed if that entry's value changes.
Action<object> entryValueChangedCallback = newValue =>
{
    // Assign new value to the entry.
    listBeingDrawn[index] = newValue;
    
    // Invoke the assignment logic of a parent drawer.
    changeValueCallback(list);
};

ITrainingDrawer entryDrawer = // Determine a drawer for the entry [out of scope of this example].

Rect entryRect = // Determine a rect for the entry [out of scope of this example].

// Use index as a label of the entry's view.
string label = "#" + index;

// Draw entry.
entryDrawer.Draw(entryRect, entry, entryValueChangedCallback, label);
```

## Custom overlay

To make your own controls define your own `Spectator Cam Prefab Overload` in [`[HUB-PLAYER-SETUP-MANAGER]`](http://docs.hub.innoactive.de/api/Innoactive.Hub.PlayerSetup.PlayerSetupManager.html) scene object. 

For reference, find the the prefab `AdvancedTrainerCamera` located in `IA-Training-Template/Resources/CustomCamera/Prefabs`. It replaces the default spectator camera in the `Advanced` scene. The child of this prefab is a custom overlay with `AdvancedTrainingController` script attached. Using this overlay, a trainer is able to see the current training status, start, reset, and mute the training, pick a chapter and skip a step, choose a language and the training mode to use.

This training controller loads a training at the following path: `[YOUR_PROJECT_ROOT_FOLDER]/Assets/StreamingAssets/Training/DefaultTraining/DefaultTraining.json`. 

The localization files must be named by the two-letter ISO code of the respective language (for example, `en.json` or `de.json`). They have to be located at `[YOUR_PROJECT_ROOT_FOLDER]/Assets/StreamingAssets/Training/DefaultTraining/Localization`. The script automatically loads all available localizations and displays them in the language dropdown menu. If there is no [respective language pack](#using-the-offline-windows-tts), the localization file is ignored. 