  > ❌ **Outdated**  
--> FUSEE uses column-major matrix layout (the rightmost column of a 4x4 matrix contains the translation part, NOT the bottom row). To transform a vector with a matrix, multiply the vector from the right: `vtransformed = M * v`

***

# Matrices

This article explains how matrix operations are handled by FUSEE. 

FUSEE uses the float4x4 class for Matrix operations. The class file is located inside the Math namespace.
The float4x4 class has the following characteristics:
*  Row-Major Matrix layout
*  Premultiplication of float3, float4 objects, e.g. `float3 result = float3*float4x4;`

![float4x4 Layout](https://raw.githubusercontent.com/wiki/FUSEEProjectTeam/Fusee/Images/Matrices/float4x4layout.png)

### Why is that important to know?
The OpenTK implementation, which is relying on OpenGL, does "convert" float4x4 row-major matrices to column-major matrices. This can be confusing when writing shaders where vectors need to be transformed, because OpenGL is using column major matrices and postmultiplication of vectors. While this seems to be very uncomfortable the only thing that needs to be considered is: 
FUSEE `float3 result = float3*float4x4` is equivalent to `vec3 result = mat4 * vec3` in GLSL context.

### General explanation of row-major vs. column-major
Matrices are associative: A * (B * C) = (A * B) * C. 
Matrices are NOT commutative: A * B != B * A. 
(in words: A multiplied by B is not equal to B multiplied by A.)

Row-major matrices are working as follows: D = A*B*C, in words: D is equal to first A, then B, then C.
Column-major matrices are working as follows: D = A*B*C, in words: D is equal to first C, then B, then A.

If you want to transform a vector from its local coordinates to another coordinate system you usually want to transform the vector through multiplication with a matrix(=another coordinate system). The following example shows how this can be achieved in FUSEE and inside of GLSL context.

FUSEE (Row-major) point transformation Example:

`float3 localPoint = new float3(0,10,-10);`
`float3 pointInViewCoordinates = localPoint * RC.View;`

In literature this approach is called Premultiplication of a vector to a matrix. 
This example is equivalent to the following. 

GLSL (Column-major) point transformation Example:

`vec3 localPoint = vec3(0,10,-10);`
`vec3 pointInViewCoordinates = FUSEE_V * localPoint;`

In literature this approach is called Postmultiplication of a vector to a matrix.

### Do I have to convert matrices when I want to set them into the RenderContext?
No, the conversion is handled by OpenTK, you do not need to worry about row and major order conversion when you set or get matrices inside of RenderContext. Just remember that you are always working with Row-major matrices inside of FUSEE.

### I need to convert a Row-Major matrix to Column-major order.
You can convert matrices from one order to another using float4x4.Transpose.
`float4x4 matrixinRowMajorOrder = float4x4.Transpose(matrixInColumnMajorOrder);`
`float4x4 matrixInColumnMajorOrder = float4x4.Transpose(matrixinRowMajorOrder);`

