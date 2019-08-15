# Changing Colours in Mixed Reality: Challenges and Insights

![Hero image/GIF for the experiment](http://www.panix.com/~mshaw/images/Photos-12/placeholder.jpg)

This is an open repository for an experiment made in collaboration between MolaLAB and **SPACE10** using the Magic Leap One headset. With augmented- and mixed reality we usually add stuff to the world. With this experiment, we wanted to playfully investigate how we can use AR to simply manipulate or subtract elements in the world.

Want to try the experiment yourself? Download the source files [here](https://github.com/space10-community/subtractor-experiment/archive/master.zip) and Get Started below.

## Overview
The experiment has two components to it:

1. The primary Magic Leap experience, where a live camera feed from the device is overlayed on both eyes at almost real-time. Using the Magic Leap One Control you're able to modify the camera's color spectrum, sample colors spectrums to modify and then only paint those modified colors in front of the eye.

2. A companion iPhone/Apple Watch app that streams heart rate data to the Magic Leap One as passive parameter for modifying the color spectrums instead of manually doing so with the Control. We did this to explore how pure physiological inputs could potentially passively alter everyday experiences of the world without people's being conscious of it.

## Setup: Magic Leap App

Build in Unity 2019.1.6f1.
*Note, if you plan to update to 2019.2, you likely have to do some [adjustments to the Magic Leap integration](https://creator.magicleap.com/learn/guides/unity-setup).*

The project includes [Tween](https://assetstore.unity.com/packages/tools/animation/tween-55983) by [Jeff Johnson](https://www.digitalruby.com/unity-plugins/) and Magic Leap integration.

To run the project, open the scene: `Assets/_project/01Scenes/main`

Though most of the Magic Leap integration should be done, you should still follow through the setup described in the general [Unity Setup](https://creator.magicleap.com/learn/guides/unity-setup) guidelines.

Particularly, it's important that you generate and add your own Magic Leap Developer Certificate.

### Tutorial

1. Launch the app in the Magic Leap One.
2. Accept the permission requests.
3. At this point, you might notice that blue colors painted in AR have replaced red colors in the real world.
4. Press the top button on the back of the Magic Leap One Control to toggle full video feed on/off. A full feed looks better, but we're compromissing on precision.
5. Hold the the Magic Leap One Control up in front of you. It shows two labels "Selection" and "Hue/Sat". The small button below the touchpad is how you navigate "Back".
6. Try tapping close to the "Selection" label on the touchpad. You are now choosing what colors in the world you want to target.
    - Swiping horisontally changes the selection.
    - Swiping vertically expands or narrows the color range you're selecting.
    - The selection changes are reflected in the UI above the Magic Leap One Control.
    - When you have chosen what color and the range you want to target, press "Back".
7. Now tap close to the "Hue/Sat" label on the touchpad. You are now choosing what colors in the world your recently selected range should be changed to!
    - Swiping horisontally changes the hue of the selected color range.
    - Swiping vertically changes the saturation of the selected color range.
8. You can go back and forth between Selection and Hue/Sat as much as you like. You'll get the hang of it :)

## Setup: iPhone/Apple Watch App
This part of the experiment connects the main experience with the wearer's body. At its current state, this side component simply changes the opacity of the overlaid video feed depending on your heart rate.

### Requirements
- [OSCsimpl](https://assetstore.unity.com/packages/tools/input-management/osc-simpl-53710) for Unity
- [Apple Watch Kit](https://assetstore.unity.com/packages/templates/systems/apple-watch-kit-88245) for Unity
- An iPhone (any model compatible with an Apple Watch)
- An Apple Watch (any model)

OSCsimpl is used to sync data between the iPhone and the Magic Leap One. You need to buy and install it if you want this part to work.

### Setup: OSCsimpl

1. Import the OSCsimpl plugin into this repository project
2. Uncomment the commented out chunk of code inside `OscSender.cs` and `OscReceiver.cs`
3. Add the OSCin.cs script to the empty (and disabled) `OSCin` gameObject (it's nested inside the `OSC` gameObject)
4. In the inspector in the OSCin script component fold out the "Mappings" area and click "Add".
5. Name it "/biodata" and set the mapping to `int`
6. Click the plus button at the bottom of the mapping and drag the parent gameObject `OSC` into the slot.
7. In the function dropdown select OscReceiver.ReceiveInt

Your Magic Leap is now configured to receive OSC data as integers on its local IP on port 7000 with the "/biodata" mapping.

On the iPhone/Apple Watch you now need to create an app that reads the heart rate. The Apple Watch Kit got us started really quick, but other kits might work as well. Integrate the OSC here as well and start broadcasting the raw heart rate values to the IP adress of the Magic Leap One with the OSC adresss "/biodata".

Check out the `OscSender.cs` script for a simple test example that sends random integers every second. If you don't have an extra iPhone and/or apple watch, you could run this script from your computer as well, just to see it working.

### Usage

The Magic Leap One will be waiting for data that's broadcasted to it's local IP.

You can find the Magic Leaps IP if you launch the app on the Magic Leap Ine and look at the Control. As soon as your iPhone app starts broadcasting to the "/biodata" mapping, the Magic Leap will start using the data. The heart rate is also displayed on the Control.

## Shaders

The experiment relies on two shaders:
- A heavily modified version of the rainbow shader described [here](https://forum.unity.com/threads/solved-gradient-rainbow-shader.449080/) for the UI on the Magic Leap One Control.
- A heavily modified version of the [HSLRangeShader](https://github.com/greggman/hsva-unity/blob/master/Assets/Shaders/HSLRangeShader.shader) used to modify the color spectrum of the live camera feed.
