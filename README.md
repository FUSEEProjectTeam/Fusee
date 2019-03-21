# RobotArm_Inverse_Kinematics
Welcome to the RobotArm Demo FUSEE App, which illustrates a method to control a robot arm using inverse kinematics.

`RobotArm_Inverse_Kinematics.cs` contains the source code for the working FUSEE application.  
The model of the arm and pincer was created using Blender and importat as `.fus` file.

## Basics
The arm is controled using inverse kinematics. In this case it follows a pointer which is initially hidden within the upmost joint (behind the pincer), adjusting the angles of the joints automatically in the process.  
The pincer will always stay parallel to the ground, but can be opened and closed.

## Controls
* Camera: Press and hold the `left mouse button` and move the mouse to rotate the camera around the arm.
* Arm: To move the pointer, use the `arrow keys` to navigate it along the x-z-plane, and `W` and `A` to move it along up and down (along the y-axis).
* Pincer: Press `O` to open/close the pincer.

## Math
To make the arm follow the movements of the pointer, we need to calculate the rotation of the foot around the y-axis, as well as the two joints around the z-axis. *Note: Due to orientation in FUSEE, the signs might be switched in the actual program.*

### First, let's look at the rotation of the foot:
To get the rotation of the foot we need to calculate the angle &epsilon;, as shown in the graphic below. We also need to calculate the distance between the arm and pointer (xzDist), which we will need in the next step. It calculates as follows:

* &epsilon; = arctan2(z, x)
* xzDist = &radic;(x<sup>2</sup> + z<sup>2</sup>)

![xz-plane](/Assets/xz-plane.png "xz-plane")

### Secondly, we'll calculate rotation of the joints:
To do that we have to calculate the inner angles &alpha;, &beta;, as well as the angle &gamma; between the vector to the pointer and the x-axis. (Note that we are using the objects coordinate system here, rather than the global coordinate system.)  
Before we can calculate the angles, we need to figure out the vector "dist":
* dist = &radic;(xzDist<sup>2</sup> + y<sup>2</sup>) 

The angles then calculate as follows:
* &alpha; = acos( (b<sup>2</sup> - a<sup>2</sup> - dist<sup>2</sup>) / (-2 a dist) )
* &beta; = acos( (dist<sup>2</sup> - a<sup>2</sup> - b<sup>2</sup>) / (-2 a b) )
* &gamma; = arctan2(y, xzDist)

Knowing that a and b are equal in length the equations can be shortened to the following:
* &alpha; = acos(dist<sup>2</sup> / (4 dist) )
* &beta; = acos( (dist<sup>2</sup> - 2 a<sup>2</sup>) / (-4 a) )

![xy-plane](/Assets/xy-plane.png "xy-plane")

### Third, calculating the actual angles:
Since the starting position (and therefore "angle 0°) of the arm is not along the x-axis, but rather the y-axis a few adjustments have to be made, which are as follows:
* finalAlpha = 90° - &alpha; - &gamma;
* finalBeta = 180° - &beta;

### Last but not least, the pincer.
As seen in the graphic below, to properly adjust position of the pincer, we need to know the angle &delta;. It calculates as follows:
* &delta; = -90° + finalAlpha + finalBeta

![pincer](/Assets/pincer.png "pincer")