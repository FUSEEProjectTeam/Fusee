# RobotArm_Inverse_Kinematics
Welcome to the RobotArm Demo FUSEE App, which illustrates a method to control a robot arm using inverse kinematics.

`RobotArm_Inverse_Kinematics.cs` contains the source code for the working FUSEE application.  
The model of the arm and pincer was created using Blender and importet as `.fus` file.

## Basics
The arm is controlled using inverse kinematics. In this case it follows a pointer which is initially hidden within the upmost joint (behind the pincer), adjusting the angles of the joints automatically in the process.  
The pincer will always stay parallel to the ground, but can be opened and closed.

## Controls
* Camera: Press and hold the `left mouse button` and move the mouse to rotate the camera around the arm.
* Arm: To move the pointer, use the `arrow keys` to navigate it along the x-z-plane, and `W` and `A` to move it along up and down (along the y-axis).
* Pincer: Press `O` to open/close the pincer.

## Math
In order for the arm to properly follow the movements of the pointer a few calculations need to be made. Namely the rotation of the foot around the y-axis, as well as the rotation of the joints around the z-axis. In the following the relevant calculations are explained.  

*Note: Orientations might differ from those in the actual program.*

### First, Rotation of the Foot
The foot needs to rotate an amount of degrees equal to angle &epsilon; to follow the pointer (as can be seen in the graphic below). Since the height `y` can be disregarded for this, the angle can be calculated using the tangent (sin or cos could also be used, but arctan2 is more convenient since it calculates the angle for all quadrants). Furthermore, the distance between pointer and the centre of the foot `xzDist` is needed for calculations down the line. This leads to the following two equations:  

* &epsilon; = arctan2(z, x)
* xzDist = &radic;(x<sup>2</sup> + z<sup>2</sup>)

![xz-plane](/Assets/xz-plane.png "xz-plane")

### Second, Rotation of the Lower and Middle Joint
The two arms of the robot, together with the vector between the base and the pointer, form a triangle (as shown in the graphic below). Since the inner angles &alpha; and &beta;, as well as the angle &gamma; are required for the rotation of the joints, but are all unknown the first step is to calculate the distance `dist` as follows:

* dist = &radic;(xzDist<sup>2</sup> + y<sup>2</sup>) 

Now, knowing the length of all the sides of the triangles, the sides calculate as follows, suing the law of cosines:
* &alpha; = acos( (b<sup>2</sup> - a<sup>2</sup> - dist<sup>2</sup>) / (-2 a dist) )
* &beta; = acos( (dist<sup>2</sup> - a<sup>2</sup> - b<sup>2</sup>) / (-2 a b) )
* &gamma; = arctan2(y, xzDist)

Since in this example the two arms a and b are equal in length, the equations can be shortened as follows:
* &alpha; = acos(dist<sup>2</sup> / (4 dist) )
* &beta; = acos( (dist<sup>2</sup> - 2 a<sup>2</sup>) / (-4 a) )

![xy-plane](/Assets/xy-plane.png "xy-plane")

### Third, Rotation Angles
Since the starting position (and therefore "angle 0°) of the arm is not along the x-axis, but rather the y-axis a few adjustments have to be made, which are as follows:
* finalAlpha = 90° - &alpha; - &gamma;
* finalBeta = 180° - &beta;

### Fourth, Orientation of the Pincer
As seen in the graphic below, the pincer has to be rotated by an amount of degrees equal to angle &delta; to remain parallel to the ground. &delta; can be determined as follows:
* &delta; = -90° + finalAlpha + finalBeta

![pincer](/Assets/pincer.png "pincer")