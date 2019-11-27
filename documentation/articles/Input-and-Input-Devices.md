FUSEE's Input is prepared to handle arbitrary input devices and provide their values in a consistent
 and centralized way to user code. Users can easily integrate existing handlers for supported input 
devices and also can provide input device handlers ("drivers") for new hardware.

### On this page
  - [Concepts](#Concepts)


Concepts
========

The Input singleton
-------------------

From the user code point of view all input access starts at the `Input` singleton object `Input.Instance`.
Use this object for the following use cases:

 - Directly access the most usual input devices: Mouse, Keyboard and Touch
 - Enumerate all currently present input devices
 - Filter for well-known input device types.
 - Listen to events about connecting and disconnecting devices.
 - Add device handlers ("drivers")
 - Add well-known input device types.

The Input class implements the classic singleton pattern (using the `Instance` static property) while at the 
same time granting direct access to all functionality through static methods (internally accessing the singleton).
In the FUSEE reference docs, this is sometimes called the "staticton" pattern.
This way, you can pass around an instance of input (whenever needed in the future) while at the same time
use shortcut acces to all functionality. Use the C# 6 `using static Fusee.Engine.Core.Input;` feature to directly
access the the `Input` instance's static methods and properties.  


Input Devices
---------------

An instance of the class `InputDevice` represents a single, typically physical, input device. Input 
devices expose axes and buttons providing an their current state. An axis consists of one single precision 
floating point value while a button provides a boolean value representing the button state 
(pressed: `true`, released: `false`). Input devices are involved in the following use cases:

 - Get current axis values and current button state (poll values).
 - Alternatively: Listen to axis change or button change events.

Both access methods (listen to events and polling) work with all axes and buttons provided by an input device,
no matter how the respective information is created and delivered by the underlying device 
handler ("driver"). Additional use cases around Input Devices include 

 - Register user-defined calculated axes. Values provided by these axes are calculated from 
   existing axes or buttons. Typical calculated axes are
   - Button Axes: Pressing and releasing one or two buttons drives an axis value to range 
     between [0, 1] (single button axis) or [-1, 1] (two button axis).
   - Derived Axes: A physical device provides absolute positional values but users are 
     interested in the speed of the changes of these values.
   - Arbitrary user-defined axes based on calculated combinations of input state from other axes and/or
     buttons.


Well-Known Input Device Types
-----------------------------

While user code can access any device listed by the Input singleton (`Input.Devices`) using the basic functionality
provided by the `InputDevice` class, some common input devicves deserve special handlig. A mouse, for example, will
expose its position in a two-dimensional manner rather than using two individual scalar axes. In addition, some 
devices will probably expose calcuated/derived/button-axes by default, without having a user define these axes from 
scratch. Certain device types might expose additional functionality. 

Users can provide classes derived from `InputDevice` to implement such special features. FUSEE already provides three classes
derived from `InputDevice` for the most common input categories:
 
 - Mouse
 - Keyboard, and
 - Touch

The input class already registers and connects the first found instance for each of those input device types so that 
you may directly access them using `Input.Mouse`, `Input.Keyboard` and `Input.Touch`. Any other special device can be implemented
in user code by deriving your own class from `InputDevice` and registere it with the `Input.RegisterInputDeviceType` method to be 
used whenever a device is connected meeting a certain condition.     

 

