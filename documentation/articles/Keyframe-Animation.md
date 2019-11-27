  > ❌ **Outdated**

# Introduction
The FUSEE Keyframe Animation, is a basic animation system that uses keyframes and interpolates the values betwenn 2 of them. We provide a linear interpolation, but it's possible to add other "Lerp functions"(interpolation functions). In this guide you'll see how to use this Animatiion System. We will use "KeyframeAnimationTest" to demonstrate the functionality.

## Using the KeyframeAnimation

### Step 1
First of all make sure to say that you want to use the KyframeAnimation. This is simply done typing it on the top of your file. The correct Namespace is shown in the picture below.

![](https://raw.github.com/wiki/FUSEEProjectTeam/Fusee/Images/Keyframe-Animation/using_Keyframe.jpg)

### Step 2
The secon thing to do is: Create the channels that are are capable of containing several keyframes, and create an animation that is capable of containing one or several channels. The order you do this isn't important.

![](https://raw.github.com/wiki/FUSEEProjectTeam/Fusee/Images/Keyframe-Animation/Animation_anlegen.jpg)

The 0 in the constructor is the Animation mode. There are 2 of them: **loop = 1 and repeat = 0**.

![](https://raw.github.com/wiki/FUSEEProjectTeam/Fusee/Images/Keyframe-Animation/Channel_anlegen.jpg)

If youre Creating a channel it is important to say what kind of keyframe the channel can contain. Here it's float3 for _channel2 and float4 for _channel1.

### Step 3
For a channel it's essentialy to know how he has to interpolate. This information is needed in the constructor. There exists a static Lerp Class that provides a basic set of linear interpolations. You can choose one of these or make a new one. If you want to make a new interpolation just look for the lerp Class in Components.Keyframeanimation. Be careful the method signature has to be the same as for the linear interpolation.

![](https://raw.github.com/wiki/FUSEEProjectTeam/Fusee/Images/Keyframe-Animation/Channel_initialisieren.jpg)

The second value in the constructor generates a new keyframe in the channel with the time 0 and the given value.

### Step 4
After this is done youjust need to create keyframes. A keyframe is generic, that means for every keyframe you decide what type he has. The first value the constructor takes is the time the second value is the value that the keyframe has at that time. After that you have to add the keyframes to one of the channels we created before. Be careful the types have to match or it wont work. Every channel has the AddKeyframe() method. This method requires a keyframe. You're allowed to give him a reference to a keyframe or create a new one at this point. Were nearly done. Now we have to add the channels to the Animation we made bevor (here myAnim).The method AddAnimation requires a channel, the Object that shall be animated and the path to the field or property in the object that shall be manipulated (as a String). 

![](https://raw.github.com/wiki/FUSEEProjectTeam/Fusee/Images/Keyframe-Animation/Keyframes_und_channels.jpg)

Now everything is set up and ready to go.

### Step 5
The last thing that has to be done is to start the animation. Therefore we have a method called Animate(). You have to choose wheater you give him the time that passed between the frames or not, like in the picture below. If you dont give the Animate a time Time.Instance.DeltaTime will be used. This method has to be called every frame.

![](https://raw.github.com/wiki/FUSEEProjectTeam/Fusee/Images/Keyframe-Animation/Animation_abspielen.jpg)

## Hints
* This animation system provides the basics!
* There is no bone animation.
* For complex animations the SceneManagment is a handy tool it provides a hierarchy system (e.g. bending of an arm).