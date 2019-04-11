# Introduction

The Hub Training Module is an additional module of the Innoactive Hub that allows you to train your employees safely and cost-efficiently around the globe. Unlike other VR training building solutions, our product considers the complete lifecycle of virtual trainings, including the creation, the maintainability, and the delivery of the applications. It's designed for large enterprises who need to train complex human-machine-interaction processes

There are two user roles: a template developer and a training designer. Training designer is a person who has no deep technical knowledge. Training module is very powerful and flexible tool, but it requires adjustments to meet designer's needs. These adjustments are done by a template developer and are shipped in a form of a training template.

Templates hide the complexity from training designers. All they should work with are basic Unity concepts and Workflow Editor / Step Inspector, which are GUI tools for creating a training.

This project is an Innoactive template. In the following chapters it will be described to how to create a similar one. You may use this project as a reference to validate your progress through this tutorial.

# Initial setup

## Setup the Hub SDK

Hub SDK is a collection of useful modules for the development of multi-user VR applications based on the Innoactive Hub. Please, check the [pre-requisites](http://docs.hub.innoactive.de/articles/sdk-setup.html#prerequisites) and follow the [instructions](http://docs.hub.innoactive.de/articles/sdk-setup.html#importing-the-hub-sdk) to install it.

## Install VR SDKs

In addition, install the SDKs of the VR headsets you're working with. For example, [SteamVR](https://github.com/Innoactive/SteamVR).

## Import the Training Module

Latest stable version can be found at [Innoactive Hub Developer Portal](http://developers.innoactive.de/components/).

# What is a training

The training is a linear sequence of steps that are executed one after another and cannot be skipped.

Every step consists of a collection of behaviors and conditions.

Behaviors are actions that are executed independently from the trainee. For example; a behavior can play an audio or move an object from one point to another.

A condition checks that a trainee has performed a certain action. With conditions you define actions that a trainee must perform. Once a condition is completed, it can't become incomplete again.

Behaviors and conditions communicate with objects on a scene through their training properties. A training property is an abstraction that hides a concrete implementation from the training: e. g. `GrabbedCondition` is only concerned if its target object is currently grabbed, but it doesn't care how exactly `GrabbableProperty` determines it.

Conditions and behaviors use `TrainingObjectReference` and `TrainingPropertyReference<TProperty>` classes to reference training objects and their properties. References locate training objects by their unique name, preventing tight coupling between trainings and scenes.

A step is completed when all of its behaviors have finished their execution and all its conditions are met.

# Create a simplest training template

A simplest template would consist of a single scene. This scene would contain Innoactive Hub SDK configuration, preconfigured VR headset, and an object with a single script that would load a training.
All that a training designer would have to do only two steps:

1) Copy the scene and populate it with training objects he needs.
2) Create a training, save it anywhere in `Assets` folder, and reference the saved file from the scene.

## Setup a scene

### Create a scene

Create a new Unity scene in which you want a VR training to be executed.

### Setup the scene as a Hub Scene

Follow the [instructions](http://docs.hub.innoactive.de/articles/sdk-setup.html#setting-up-a-scene) to setup the scene.

### Disable unused HUB SDK components

The following components are not required for the Training SDK and may be disabled within the scene:

1. `[HUB-MULTIUSER]`
2. `[HUB-PERSISTENCE]`
3. `[HUB-LOGIN-CHECK]`
5. `[HUB-VR-LAUNCHER-CLIENT]`
4. `[HUB-MENU-SETUP]`

If you are working with the Hub-SDK for the first time it is recommended to disable them for the sake of simplicity. You can always turn them back on again later.

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

You can verify the achieved result with the scene is located under the following path: `[Path to Innoactive training template]/Scenes/Simple`.

# Advanced topics

Cheers! You've covered the basics.

The following chapters are explaining how to create more sophisticated templates, providing detailed overview of the Training Module features.

It is recommended to explore this project at the same time you read this tutorial.

# Template configuration

If you want to tweak Training Module a bit, you might want to look at `EditorConfiguration` and `TrainingConfiguration` classes first.

## Editor configuration

Editor configuration affects designer's tools in Unity Editor. 

1. `BehaviorsMenuContent` property defines a dropdown menu which is shown when a training designer clicks on `Add Behavior` button.
2. `ConditionsMenuContent` property defines a dropdown menu which is shown when a training designer clicks on `Add Condition` button.

By changing these properties you may control which conditions and behaviors are available for a designer.

To change the configuration, either implement `IDefinition` or inherit from DefaultDefinition in `Innoactive.Hub.Training.Editors.Configuration` namespace.

The configuration will automatically detect new definition class via reflection, so you can name it as you like and keep it where you want to.

Take a look at `DefaultDefinition` to see how basic options could be implemented. Take a look at `InnoactiveDefinition` to see how to implement complex things like `Audio Hint` menu option.

## Training configuration

Training configuration is used to adjust the way the training application executes in a runtime.
 
There should be one and only one training configuration game object in a training scene. To create one, you can use `Innoactive > Training > Setup Scene` menu option.

A training configuration instance is just a container for its definition, which provides the actual means for template customization.
 
The definition has the following properties and methods:

1. `TrainingObjectRegistry` provides the access to all training objects and properties of the current scene.
2. `Trainee` is a shortcut to a trainee's headset.
3. `InstructionPlayer` is a default audio source for all `PlayAudioBehavior` behaviors.
4. `TextToSpeechConfig` defines a TTS engine, voice, and language to use to generate audio.
1. `SetMode(index)` sets current mode to the one at provided `index` in the collection of available modes.
2. `GetCurrentMode()` returns the current mode.
3. `GetCurentModeIndex()` returns the current mode's index, which is useful if you want to iterate over available modes.
4. `AvailableModes` returns a collection of all modes available. Normally, this is a single modes-related class member you want to override.

TTS config and training modes are described in detail in the following chapters.

Unless you explicitly assign the `TrainingConfiguration.Definition` property, the `DefaultDefinition` is used.

You can modify the training configuration definition in the same way as the editor configuration, using `Innoactive.Hub.Training.Configuration` namespace types instead.

It is recommended to inherit from `DefaultDefinition` rather than implement `IDefinition` directly, as it already provides some basic functionality.

# TTS configuration

Text to Speech config defines the TTS engine to use, and voice and language to use.

By default, TTS config is loaded from `[YOUR_PROJECT_ROOT_FOLDER]/Config/text-to-speech-config.json` file (if there is any). New TTS config can be set at runtime, but it will have no effect on already loaded training.

The TTS engine is defined by its provider class name. For example, `MicrosoftSapiTextToSpeechProvider` is a provider for the Windows TTS, and `WatsonTextToSpeechProvider` is used for Watson TTS.

The acceptable values for the `Voice` and the `Language` properties differ from TTS provider to provider. For online TTS engines, the `Auth` property may be required, as well.

If you would like to use offline TTS engine, there is a speech synthesizer that is built-in Windows 10. 

It doesn't require an internet connection but a respective Language Pack has to be installed at end user's system (`Windows Settings > Time and Language > Language > Add a language`).

Its config takes either `Male` or `Female` value as a voice and either natural language name, and a two-letter ISO language code as a `Language` (if there is not a two-letter ISO language code, it is the three-letter ISO language code). You can look up for the language codes [here](https://msdn.microsoft.com/en-us/library/cc233982.aspx). 

For example, that how would the config look like for a female voice with english pronunciation:

```c#
new TextToSpeechConfig()
{
    // Define which TTS provider is used.
    Provider = typeof(MicrosoftSapiTextToSpeechProvider).Name,
    
    Voice = "Female",
    
    Language = "EN"
};
```

# Localization

The Training SDK uses the `Localization` class from the Hub SDK. It's basically a wrapper around a dictionary of strings with a convenient API. 

## Set the localization

To define current localization, you could either assign `entries` property directly, or load it from a JSON file with `LoadLocalization(string path)` method. The JSON file contains the following key-value structure:

```json
{
  "translated_text": "Ein übersetzter Text.",
  "example": "Ein Beispiel." 
}
```

Note that `Localization` class itself is not concerned about current language or which localization file should be loaded. This should be handled by the template.

## Use the localization

Use `LocalizedString` whenever you want to use some localized text. `Value` properties looks up for a localization entry by `Key` and returns its contents. If `Key` isn't specified or the entry is missing, `DefaultText` is used instead. 

# Instruction player

A `PlayAudioBehavior` uses `TrainingConguration.InstructionPlayer` property to determine which audio source it should use. By default, it is an audio source automatically attached to the trainee's headset.

# Training modes

A training mode is defined by its activation policy and the parameters.

An activation policy determines which behaviors and conditions are enabled during a training execution. If a behavior or condition is disabled, it's simply skipped.

The training mode parameters is a string-to-object dictionary which can be used by any behavior or condition which was designed to be configurable (see the `Extend Training SDK` chapter).

There is only one training mode that is defined in the default training definition. It allows any condition or behavior, and has no parameters.

To define your own training modes, override the `AvailableModes` property. Define a `Mode` when invoking its constructor. You will not be able to do it later, as modes are immutable by design.

To switch between modes, call `SetMode(index)` method.

# Extend Training SDK

The Training SDK has default behaviors and conditions. They are sufficient for most of your trainings, we suggest that you write your own to handle very specific cases (e. g. "Is that sheep shaved?" condition). 

In this chapter we will create the following

* A custom condition which is triggered when a given pointer points at a given object.
* A custom behavior which will change the scale of a target object. 

## Custom training property

Behaviors and conditions communicate with objects on a scene through their properties. Property hides Unity-dependent details of realization from the training.

Create new C# script named `PointingProperty` and set its contents to the following:

```c#
using System;
using UnityEngine;
using VRTK;

namespace Innoactive.Hub.Training.Examples
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
* It exposes an event so a training condition could use it, and it ensures that the event is fired only when pointer points at a training object with a collider.

Now it can be attached to a game object on a scene. To save time on making your own pointer tool, you could copy `Your Hub SDK Directory\SDK\Tools\Presenter\Resources\Presenter` and attach the property to its `Pointer` child object.

> Note that we expect the `ColliderWithTriggerProperty` to be attached to the training object we want to point at. VRTK_Pointer expects an object to have a collider with a trigger, and the `ColliderWithTriggerProperty` ensures that its owner object has one.

## Custom condition

Create new C# script named `PointedCondition` and change its contents to the following:

```c#
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Innoactive.Hub.Training.Examples
{
    // Condition which is completed when Pointer points at Target.
    public class PointedCondition : Condition
    {
        // Reference to a pointer property.
        public TrainingPropertyReference<PointingProperty> Pointer { get; private set; }

        // Reference to a target property.
        public TrainingPropertyReference<ColliderWithTriggerProperty> Target { get; private set; }

        // Make sure that references are initialized.
        public PointedCondition()
        {
            Pointer = new TrainingPropertyReference<PointingProperty>();
            Target = new TrainingPropertyReference<ColliderWithTriggerProperty>();
        }

        // This method is called when the step with that condition has completed activation of its behaviors.
        public override void OnActivate()
        {
            Pointer.Value.PointerEnter += OnPointerEnter;
        }

        // This method is called at deactivation of the step, after every behavior has completed its deactivation.
        public override void OnDeactivate()
        {
            Pointer.Value.PointerEnter -= OnPointerEnter;
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

This condition subscribes to a `PointerEnter` event of a referenced `PointingProperty`. When the Pointer points at the Target, the condition detects it and marks itself as complete.

A condition determines if it is completed by itself. The single limitation is that you should check for a completion of the condition only when it is activating or active. The best place to handle that is `OnActivation()` method which is called when condition begins its activation, and `OnDeactivation()`, which is called when the condition is not active anymore.

## Serialization

The Training SDK uses Newtonsoft.Json for serialization.

With the current implementation of PointedCondition, it can't be saved to a training file. To fix that, add required attributes as shown: 

```c#
// Specifies that the type defines or implements a data contract and is serializable by a serializer.
// IsReference property indicates that object reference data should be preserved.
[DataContract(IsReference = true)]
public class PointedCondition : Condition
{
    // When applied to the member of a type, specifies that the member is part of a data contract and is serializable by the serializer.
    [DataMember]
    public TrainingPropertyReference<ExamplePointingProperty> Pointer { get; private set; }

    // When applied to the member of a type, specifies that the member is part of a data contract and is serializable by the serializer.
    [DataMember]
    public TrainingPropertyReference<ColliderWithTriggerProperty> Target { get; private set; }

    // Specifies parameterless constructor which should be used by the serializer.
    [JsonConstructor]
    public PointedCondition()
    {
        Pointer = new TrainingPropertyReference<ExamplePointingProperty>();
        Target = new TrainingPropertyReference<ColliderWithTriggerProperty>();
    }
    // The methods are unchanged.
    // [...]
}
```

## Mode parameters

All conditions and behaviors can be enabled or disabled by a training mode activation policy, but sometimes you need it to be more customizable than that.

To achieve that, you could use `ModeParameter` class. An instance of `ModeParameter` automatically fetches the training mode parameter of a given type by its `key`. If the current mode does not define the parameter, it uses default value instead. You can subscribe to the `ParameterModified` event to handle the training mode change.

You can use the following code snippet as an example:

```c#
// Declare the property.
public ModeParameter<bool> IsShowingHighlight { get; private set; }

[...]

// Initialise the property (usually in constructor, at Awake() or OnEnable()).
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

## Custom behavior

Create new C# script named `ScalingBehavior` and change its contents to the following:

```c#
using System.Collections;
using System.Runtime.Serialization;
using Innoactive.Hub.Threading;
using Newtonsoft.Json;
using UnityEngine;

namespace Innoactive.Hub.Training.Examples
{
    // This behaviors linearly changes scale of a Target object over Duration seconds, until it matches TargetScale.
    [DataContract(IsReference = true)]
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

        // Handle data initialization in the constructor.
        [JsonConstructor]
        protected ScalingBehavior()
        {
            Target = new TrainingObjectReference();
            TargetScale = Vector3.one;
            Duration = 0f;
        }

        // Called on activation of the training entity. Define activation logic here.
        // You have to call `SignalActivationStarted()` at the start
        // and `SignalActivationFinished()` after you've done everything you wanted to do during the activation.
        public override void PerformActivation()
        {
            SignalActivationStarted();

            // Start coroutine which will scale our object.
            CoroutineDispatcher.Instance.StartCoroutine(ScaleTarget());
        }

        // Called on deactivation of the training entity. Define deactivation logic here.
        // You have to call `SignalDeactivationStarted()` at the start
        // and `SignalDeactivationFinished()` after you've done everything you wanted to do during the deactivation.
        public override void PerformDeactivation()
        {
            SignalDeactivationStarted();
            SignalDeactivationFinished();
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

Behaviors should inherit from the `Behavior` class. They should implement two methods: `PerformActivation()`, which is called when behavior's activation begins, and `PerformDeactivation()`, which is called when behavior's deactivation begins.

It should call `SignalActivationStarted()` at the beginning of the activation, and `SignalActivationFinished()` at the end of the activation. The same applies for `SignalDeactivationStarted()` and `SignalDeactivationFinished()`.

If you need to execute some logic every frame, make a coroutine for that and invoke it with `CoroutineDispatcher.Instance.StartCoroutine(coroutine)`.

If you want this behavior available to the user, you have to create a class that inherits from `InstantiationOption<IBehavior>`, similarly to the example given in section 7.4.

## Training object reference

Note that conditions and behaviors never reference training objects or properties directly: they use instances of `TrainingObjectReference` and `TrainingPropertyReference<TProperty>` classes instead. References locate training objects by their unique name, completely separating trainings from scenes.

## Custom drawers

You can create your own training drawers.

To make a new drawer, you have to implement the `ITrainingDrawer` interface and mark your drawer with one of two attributes:
* If you want to use the drawer as a default drawer for all data members of a type, use `[DefaultTrainingDrawer(Type type)]` attribute. For an example, see `BehaviorDrawer` class.
* If you want to use the drawer as a custom training drawer for explicitly marked data members, use `[CustomTrainingDrawerAttribute]` instead. To reference it, use `[UsesCustomTrainingDrawer(string type)]` attribute (see above). For an example, see `ListFloatColorDrawer` class.

Keep the following in mind:
* The drawer has to be stateless, as there is only one shared instance.
* Inherit from an `AbstractDrawer` instead of `ITrainingDrawer`, as it properly implements `ChangeValue` method and one of the `Draw` overloads.
* Pass the value assignment logic via `changeValueCallback` parameter of the `Draw` method.
* Never invoke that callback directly, either pass it to a child drawer, or call a `ChangeValue` with it as a parameter.
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

In the scene `Advanced` we use the prefab `AdvancedTrainerCamera` located in `IA-Training-Template/Resources/CustomCamera/Prefabs`. Its child uses the `AdvancedTrainingController` script to manage the overlay and that uses the `MicrosoftSapiTextToSpeechProvider` for Text to Speech.  

Using this custom overlay, you can see the current training status and step but you can also start, reset, and mute the training. Besides, it allows to change the language and training mode.  

For this prefab the training file to be loaded must be located in the folder `[YOUR_PROJECT_ROOT_FOLDER]/Assets/StreamingAssets/Training/DefaultTraining` and has to be named `DefaultTraining.json`. 

> Note that you can only have one StreamingAssets folder and it must be placed in the root of the project directly within the Assets folder. 

The localization files must be named like the two-letter ISO language code (like `en.json` or `de.json`) of the concerning languages (if there is no two-letter ISO code, it is the three-letter ISO code). They have to be located in `[YOUR_PROJECT_ROOT_FOLDER]/Assets/StreamingAssets/Training/DefaultTraining/Localization`. The script automatically loads all available valid languages from that folder and provides them in the language dropdown menu of the custom overlay. Make sure, you have all the desired languages installed on your system as described in `TTS configuration` section.