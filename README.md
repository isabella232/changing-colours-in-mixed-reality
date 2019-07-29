# Rather than augment on top, could we subtract (on top)?

This is an open repository for an experiment made in collaboration between Molamil and **SPACE10**. It explores, using exisiting technology, how perception can be changed at realtime conscious or subconsicious to a user.
With AR technology we usually add to the world. We wanted to use it to play with how it feels to subtract from the world.

Want to try the experiment yourself? Download the source files here and get started below.

## Overview
The project actually has 2 apps:
1. The Magic Leap experience, where a live camera feed is overlayed on both eyes at almost realtime. Using the ML controller you can modify the cameras color spectrum, select the part of the colors to modify and then only paint thos modified colors in front of the eye.
2. An iPhone /Apple Watch app, that streams heart rate data to the Magic Leap, as an external factor: Modifying the users perception based on their own body input, but without the users' awareness. The heart rate shouldn't be considered a really relevant factor though. We chose to use it, as it's easily available. It's really unlikely, though, that the heart rate can be used to say anything meaningful about the users' current mood (unless it's 0).

## Setup app 1: Magic Leap

Build in Unity 2019.1.6f1. Note, if you plan to update to 2019.2, there seems to be some adjustments to the Magic Leap integration, [Read more](https://creator.magicleap.com/learn/guides/unity-setup)

The project includes [Tween](https://assetstore.unity.com/packages/tools/animation/tween-55983) by [Jeff Johnson](https://www.digitalruby.com/unity-plugins/) and Magic Leap integration.

We also used [OSCsimpl](https://assetstore.unity.com/packages/tools/input-management/osc-simpl-53710) to sync data between the iPhone (Apple Watch heart rate) and the ML, but you need to buy and install it if you want that part to work.

To run the project, open the scene: `Assets/_project/main`


Though most of the Magic Leap integration should be done, you should still follow through the setup described here: [Unity Setup](https://creator.magicleap.com/learn/guides/unity-setup)

Especially, you'll need to generate and add your own ML certificate

### Usage

1. Launch the app in the Magic Leap
2. Accept the permission requests
3. Notice that there should be blue colors painted in AR where there are red colors in the real world.
4. Tap the top button on the back of the controller to toggle full video feed on/off. A full feed looks better, but it's further away from "the future" we're simulating. In reality the goal is to only paint on the part of the world we want to modify/subtract. Sadly we are limited with the ML only streaming from the top left camera, so we don't have stereo video and we don't have video from the pupil of the eye either. ...ok let's continue :)
5. Look at the remote, it shows 2 labels "Selection" and "Hue/Sat". The small round button under the touchpad is the "back" button.
  - Selection
    1. Tap close to the "Selection" label on the touchpad.
    2. **scrub** sideways on the touchpad to change the selection. Basically you are choosing which color in the real world to target. It can help to google "Hue Gradient" on a desktop and see an actual color spectrum in front of you while you play with this.
    3. **Scrub** upwards or downwards to widen / narrow the color selection. You should also be able to see the effect reflected in the UI above the controller.
6. Tap the small round button beneath the touchpad to go a step back
  - Hue/Sat
    1. Tap close to the "Hue/Sat" label on the touchpad.
    2. **scrub** sideways on the touchpad to "hue". You'll see that the selection you did before stays, but now the colors within it are being changed.
    3. **Scrub** upwards or downwards to oversaturate / desaturate the colors
7. You can go back and forth between Selection and Hue/Sat as much as you like. You'll get the hang of it :)
  

## Setup app 2: iPhone / Apple Watch
If you don't do app 2, the only thing you're missing out on, is our Apple Watch integration (and to be fair, it's not the most crazy experience, when it's set up. At its current state it simply changes the opacity of the hued video depending on your heart rate).

You need OSCsimpl + an extra iPhone + an Apple Watch

We aren't going to go into too much details, with the overall app. Hint, buy this unity package: [Apple Watch Kit](https://assetstore.unity.com/packages/templates/systems/apple-watch-kit-88245) ... done

### OSCsimpl

We aren't including OSCsimpl itself, but our setup is included, so if you buy and install it, then you should be able to work with our integration.

On the iPhone/Apple Watch side you'd want to first of all create an app that reads the heart rate. The Unity [Apple Watch Kit](https://assetstore.unity.com/packages/templates/systems/apple-watch-kit-88245) got us started really quick, though it comes with a price. Then on the iPhone side you integrate OSC as well and simply broadcast the raw heart rate values to the IP adress of the Magic Leap with the OSC adresss "/biodata".

If OSCsimpl is installed on the Magic Leap you should already be able to see the IP if you look at the ML controller while inside the ML experience.

### Usage

The Magic Leap will be waiting for data that's broadcasted to it's local IP (written on the controller) +  the OSC adress "/biodata". As soon as your iPhone app starts broadcasting the Magic Leap will start using the data. The heart rate is also displayed on the controller for debuggins sake.

## Shaders

The project relies on 2 shaders:
- A heavily modified version of the rainbow shader described [here](https://forum.unity.com/threads/solved-gradient-rainbow-shader.449080/) for the UI on the ML controller.
- A heavily modified version of the [HSLRangeShader](https://github.com/greggman/hsva-unity/blob/master/Assets/Shaders/HSLRangeShader.shader) used to modify the color spectrum of the live camera feed.