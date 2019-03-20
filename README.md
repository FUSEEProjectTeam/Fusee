# RobotArm_Inverse_Kinematics
This is an example of a robot arm that can be moved using inverse kinematics. Instead of directly controlling the angles of the joints, you can controll a pointer (which is initially hidden within the upmost joint). The arm will then automatically adjust to follow the position of the pointer. The pincer of the arm is programmed to always stay parallel to the ground.

## Controls:

* Camera: Press and hold the left mouse button and move the mouse to rotate the camera around the arm.
* Arm: To move the pointer, use the arrow keys to navigate it along the x-z-plane, and "w" and "a" to move it along up and down (along the y-axis).
* Pincer: Press "o" to open/close the pincer.

## Math
```csharp
double xzDist = Math.Sqrt(Math.Pow((double)_virtualPos.x, 2.0d) + Math.Pow((double)_virtualPos.z, 2.0d));
```

![xz-plane](/Assets/xz-plane.png "xz-plane")

![xy-plane](/Assets/xy-plane.png "xy-plane")
