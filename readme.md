# Innoactive Training Template

## 1. Content

This template contains the **Hub SDK**, **SteamVR**, and the **Hub Training Module** in the `Extensions` folder. It also contains a simple scene with a simple training loader which are ready to use (see `SimpleTrainingLoader.cs` in `Source/TrainingLoader`).
Besides, new behaviors named `Spawn Confetti` and `Audio Hint` are added to the behaviors list in the Training Step Inspector. This is done via custom editor configuration definition which is located in the `Source/Editor/Configuration/` folder. The first behavior causes confetti to rain at a desired position or down on the player and the second one repeats the voice describing the task of the trainee.

## 2. Initial Setup

### 2.1. Setup the template

Download the Unity Package of this template and import it into your Unity project: `Assets > Import Package > Custom Package...`.

Alternatively, clone the template's repository into your project via git: 
`git clone --recurse-submodules -j8 <URL>`

### 2.2. Setup the scene and training

Simply use the provided scene and create and save a new training (see the description in the documentation of the **Hub Training Module Examples**) or import an existing training. In the scene, add the training to the `Simple Training Loader` component of the game object `Training Loader` in the scene hierarchy.