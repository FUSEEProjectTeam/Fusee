Fusee
=====

Fusee aims at becoming a multiplatform 3D realtime engine with a strong emphasis on content transformation and manipulation.

Fusee is written mainly in C# and can be programmed in C#.

Fusee builds bridges to make your code run in environments that don't support C#, like Content-Creation-Software C++ Plug-In-APIs and native HTML5/WebGL Javascript code.


feat_dsteffen_mac
=====

This branch is an experiment to port the Fusee "Cinema 4D" functionality to mac osx 10.7 "Lion".
The project ist based on a swig build C4dApiWrapper.cpp file. This file can only be generated from the Windows project at the moment.

We got a buildable state for now. Next step is trying to include the native project in an example plugin project.

