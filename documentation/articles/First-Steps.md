  > ❌ **Outdated**

Before you can start with FUSEE, you need the following software:

* Microsoft Visual Studio
* 3D modeling software (Blender, Cinema 4D)
* Git / Github / Smart Git / Source Tree (optional)
* C# experience
* .NET 4.0 or newer ([Download](http://www.microsoft.com/net))

***

**1. Download**

***Download FUSEE with Git***

Use your Git Software and download FUSEE with the following link:

     https://github.com/FUSEEProjectTeam/Fusee.git

Example with GitHub for Windows

Download [GitHub for Windows](http://windows.github.com/) and install the software. Then click GitHub/Fusee and click [Clone in Windows](github-windows://openRepo/https://github.com/FUSEEProjectTeam/Fusee). Now the FUSEE Project appears in GitHub for Windows. 

If you don't have a GitHub account and don't want to register, you can use SoureTree.

***Download FUSEE with SourceTree***

Download SourceTree [here](http://sourcetreeapp.com/). 

Then clone the project by clicking on clone and copying the address.

![Click on Clone](http://webuser.hs-furtwangen.de/~meierinh/Fusee_Bilder/SourceTree_01.png)

![Insert the address](http://webuser.hs-furtwangen.de/~meierinh/Fusee_Bilder/SourceTree_02.png)

***

**2. Open FUSEE**

Open your FUSEE directory. By starting Engine.sln, you will start the engine. If no other directory is specified, you will find the engine in the C: \ Users \ NAME \ Fusee \ Fusee. If you want to work with the FUSEE version "binaries", which is an excerpt from development, you will find the engine in C: \ Users \ NAME \ Fusee \ Fusee Binary Example.

***

**3. Start a new Project**

If you have started FUSEE, you have to go into the FUSEE directory. Then you open the "Help" folder. There is an executable named "fuProjectGen.exe". Running it will ask you to name your project and you'll be provided with an empty FUSEE project. You find it in the example folder of the engine.

![fuProjectGen](http://webuser.hs-furtwangen.de/~meierinh/Fusee_Bilder/fuProjectGen.png)

Project in VisualStudio 2010

![fuProjectGen](http://webuser.hs-furtwangen.de/~meierinh/Fusee_Bilder/fuProjectGen_02.png)


This is the Init method. Here is the definition of what should not change during the render and remain constant.

        public override void Init()
       {
           // is called on startup
       }
The RenderAFrame method is called once per frame (each time FUSEEasks your application to generate an image. Typically this method will read states from input devices (see the Input class) and contain image drawing code using the RenderContext class.

        public override void RenderAFrame()
       {
           // is called once per frame
       }
The RenderAFrame function changes the current image and variable change during the term.

        public override void Resize()
       {
          // is called when the window resizes
           RC.Viewport(0, 0, Width, Height);

           var aspectRatio = Width / (float)Height;
           RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);
       }
The resize function adjusts the image, if you change the window size.

       public static void Main()
       {
           var app = new FuseeExampleApp1();
           app.Run();
       }
The main function will start the programm.

***

**4. Objects in FUSEE**

Create a model in a 3D modeling program of your choice. Export it as a Wavefront (.obj) and rename it. The extension should be modified like this: _NAME.obj.model_.

The following screenshot shows Blender. Most other 3D modeling software are similar.
![Export to Wavefront](http://webuser.hs-furtwangen.de/~meierinh/Fusee_Bilder/wavefront.png)

Insert the file into the Assets folder of your porject. Open your project in Visual Studio as well as the Assets folder and select your file. In the properties box you can see its properties. Change within “In output_directory” from “not copy” to “if new, copy”.

![copy](http://webuser.hs-furtwangen.de/~meierinh/Fusee_Bilder/kpierenwennneuer.png)

***

**5. Shader**

Write a global vertex shader and a global pixel shader, which will be used in the whole class.


       protected string Vs = @"
            #version 120

           /* Copies incoming vertex color without change.
            * Applies the transformation matrix to vertex position.
            */

           attribute vec2 fuUV;

           attribute vec4 fuColor;
           attribute vec3 fuVertex;
           attribute vec3 fuNormal;
           
           

           varying vec4 vColor;
           varying vec3 vNormal;
           varying vec2 vUV;
       
           uniform mat4 FUSEE_MVP;
           uniform mat4 FUSEE_ITMV;

           void main()
           {
               vUV = fuUV;
               gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
               // vColor = vec4(fuNormal * 0.5 + 0.5, 1.0);
               // vec4 norm4 = FUSEE_MVP * vec4(fuNormal, 0.0);
               // vNormal = norm4.xyz;
               vNormal = mat3(FUSEE_ITMV) * fuNormal;
           }";

       protected string Ps = @"
            #version 120

           /* Copies incoming fragment color without change. */
           #ifdef GL_ES
               precision highp float;
           #endif
       
           uniform vec4 vColor;
           varying vec3 vNormal;

           //Variables for textures in the pixel shader
           uniform sampler2D texture_planet;
           varying vec2 vUV;
                       


           void main()
           {
               

               gl_FragColor = texture2D(texture_planet, vUV) ; //vColor * dot(vNormal, vec3(0, 0, 1));
           }";

***

**6. Meshs in FUSEE (Code)**

Now you have to set the globale variables to be able to get to the methods from all the models and angle variables.

//Variables for the angles
       private static float _angleHorz = 0.0f, _angleVert = 0.0f, _angleVelHorz = 0, _angleVelVert = 0, _rotationSpeed = 10.0f, _damping = 0.95f;
       //Variables for the models
       protected Mesh Mesh, MeshFace;
       protected ImageData Mesh_ID, Face_ID;
       //Variable for a color.
       protected IShaderParam VColorParam;


The variables, models and shades are going to be initialized in the init method.

         _angleHorz = 0;
           _rotationSpeed = 10.0f;
           ShaderProgram sp = RC.CreateShader(Vs, Ps);
           RC.SetShader(sp);
           VColorParam = sp.GetShaderParam("vColor");
           RC.ClearColor = new float4(1, 1, 1, 1);

In this method you write how the model reacts to movements of the mouse and in which angle the model will be shown.


RC.Clear(ClearFlags.Color| ClearFlags.Depth);

           
           if (In.IsButtonDown(MouseButtons.Left))
           {
               _angleVelHorz = _rotationSpeed * In.GetAxis(InputAxis.MouseX) * (float) DeltaTime;
               _angleVelVert = _rotationSpeed * In.GetAxis(InputAxis.MouseY) * (float) DeltaTime;
           }
           else
           {
               _angleVelHorz *= _damping;
               _angleVelVert *= _damping;
           }
           _angleHorz += _angleVelHorz;
           _angleVert += _angleVelVert;

The same you do with the keyboard. To the key codes are used.
There you need the key codes .

           if (Input.Instance.IsKeyDown(KeyCodes.Left))
           {
               _angleHorz -= _rotationSpeed * (float)Time.Instance.DeltaTime;
           }
           if (Input.Instance.IsKeyDown(KeyCodes.Right))
           {
               _angleHorz += _rotationSpeed * (float)Time.Instance.DeltaTime;
           }
           if (Input.Instance.IsKeyDown(KeyCodes.Up))
           {
               _angleVert -= _rotationSpeed * (float)Time.Instance.DeltaTime;
           }
           if (Input.Instance.IsKeyDown(KeyCodes.Down))
           {
               _angleVert += _rotationSpeed * (float)Time.Instance.DeltaTime;
           }

Now this should affect the hole model.
The local coordinates of your model going to be changed by muliply the rotationmatrix and the time of tapping the key.  
The colors of the model going to be set as a Shader with RGB parameter.
Finally the model going to render.

           RC.ModelView = mtxRot * float4x4.CreateTranslation(-100, 0, 0) * mtxCam;
           //colordecloration
           RC.SetShaderParam(VColorParam, new float4(0.5f, 0.8f, 0, 1));
           RC.Render(Mesh);

           RC.ModelView = mtxRot * float4x4.CreateTranslation(100, 0, 0) * mtxCam;
           //colordecloration
           RC.SetShaderParam(VColorParam, new float4(0.8f, 0.5f, 0, 1));
           RC.Render(MeshFace);
           Present();

***

**7. Textures**

To create the textures you need the shader to manage the variables.

        //Pixel and Vertex Shader
       protected string Vs = @"
            #ifndef GL_ES
              #version 120
           #endif

           /* Copies incoming vertex color without change.
            * Applies the transformation matrix to vertex position.
            */

           attribute vec4 fuColor;
           attribute vec3 fuVertex;
           attribute vec3 fuNormal;
           attribute vec2 fuUV; //for texture
       
           varying vec4 vColor;
           varying vec3 vNormal;
           varying vec2 vUV; //for texture
       
           uniform mat4 FUSEE_MVP;
           uniform mat4 FUSEE_ITMV;

           void main()
           {
               gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
               // vColor = vec4(fuNormal * 0.5 + 0.5, 1.0);
               // vec4 norm4 = FUSEE_MVP * vec4(fuNormal, 0.0);
               // vNormal = norm4.xyz;
               vNormal = mat3(FUSEE_ITMV) * fuNormal;
               vUV = fuUV; //for texture
           }";

       protected string Ps = @"
            #ifndef GL_ES
              #version 120
           #endif

           /* Copies incoming fragment color without change. */
           #ifdef GL_ES
               precision highp float;
           #endif
       
           uniform sampler2D texture1; //for texture
           uniform vec4 vColor;
           varying vec3 vNormal; //for texture
           varying vec2 vUV;

           void main()
           {
               gl_FragColor = texture2D(texture1, vUV); //for texture
           }";
To assign the data and to map the textures on the meshs, variables of the following types will be needed: IShaderParam, ImageData and ITexture. In the following example two different variables will be used. It’s necessary to set a new variable for each texture.

       protected IShaderParam VColorParam;
       protected IShaderParam _vTextureParam;
       protected ImageData _imgData1;
       protected ImageData _imgData2;
       protected ITexture _iTex1;
       protected ITexture _iTex2;

The new variables will be given the texture, the shaders and the function for creating textures. Afterwards the textures will be mapped on the mesh in the RenderAFrame function.

           _vTextureParam = sp.GetShaderParam("texture1");

           _imgData1 = RC.LoadImage("Assets/world_map.jpg");
           _imgData2 = RC.LoadImage("Assets/cube_tex.jpg");

           _iTex1 = RC.CreateTexture(_imgData1);
           _iTex2 = RC.CreateTexture(_imgData2);

        //RenderAFrame
        //mapping
           RC.SetShaderParamTexture(_vTextureParam, _iTex1);
           RC.Render(Mesh);

           RC.ModelView = mtxRot * float4x4.CreateTranslation(100, 0, 0) * mtxCam;
          
           //mapping
           RC.SetShaderParamTexture(_vTextureParam, _iTex2);

**

**8. Code**

    //The whole Code.

    using System.IO;
    using Fusee.Engine;
    using Fusee.Math;

    namespace Examples.Simple
    {
    public class Simple : RenderCanvas
    {
       //Pixel and Vertex Shader
       protected string Vs = @"
            #ifndef GL_ES
              #version 120
           #endif

           /* Copies incoming vertex color without change.
            * Applies the transformation matrix to vertex position.
            */

           attribute vec4 fuColor;
           attribute vec3 fuVertex;
           attribute vec3 fuNormal;
           attribute vec2 fuUV; //for texture
       
           varying vec4 vColor;
           varying vec3 vNormal;
           varying vec2 vUV; //for texture
       
           uniform mat4 FUSEE_MVP;
           uniform mat4 FUSEE_ITMV;

           void main()
           {
               gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
              
               vNormal = mat3(FUSEE_ITMV) * fuNormal;
               vUV = fuUV; //for texture
           }";

       protected string Ps = @"
            #ifndef GL_ES
              #version 120
           #endif

           /* Copies incoming fragment color without change. */
           #ifdef GL_ES
               precision highp float;
           #endif
       
           uniform sampler2D texture1; //for texture
           uniform vec4 vColor;
           varying vec3 vNormal; //for texture
           varying vec2 vUV;

           void main()
           {
               gl_FragColor = texture2D(texture1, vUV); //for texture
           }";
       //angle variable
       private static float _angleHorz = 0.0f, _angleVert = 0.0f, _angleVelHorz = 0, _angleVelVert = 0, _rotationSpeed = 10.0f, _damping = 0.95f;
       //modell variable
       protected Mesh Mesh, MeshFace;
       //variable for color
       protected IShaderParam VColorParam;
       protected IShaderParam _vTextureParam;
       protected ImageData _imgData1;
       protected ImageData _imgData2;
       protected ITexture _iTex1;
       protected ITexture _iTex2;

       public override void Init()
       {
           //initialize the variable
           Geometry geo = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Teapot.obj.model"));
           Mesh = geo.ToMesh();

           Geometry geo2 = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Face.obj.model"));
           MeshFace = geo2.ToMesh();

           _angleHorz = 0;
           _rotationSpeed = 10.0f;
           ShaderProgram sp = RC.CreateShader(Vs, Ps);
           RC.SetShader(sp);
           VColorParam = sp.GetShaderParam("vColor");
           _vTextureParam = sp.GetShaderParam("texture1");

           _imgData1 = RC.LoadImage("Assets/world_map.jpg");
           _imgData2 = RC.LoadImage("Assets/cube_tex.jpg");

           _iTex1 = RC.CreateTexture(_imgData1);
           _iTex2 = RC.CreateTexture(_imgData2);


           RC.ClearColor = new float4(1, 1, 1, 1);
       }

       public override void RenderAFrame()
       {
           RC.Clear(ClearFlags.Color | ClearFlags.Depth);

           //move per mouse
           if (Input.Instance.IsButtonDown(MouseButtons.Left))
           {
               _angleVelHorz = _rotationSpeed * Input.Instance.GetAxis(InputAxis.MouseX) * (float)Time.Instance.DeltaTime;
               _angleVelVert = _rotationSpeed * Input.Instance.GetAxis(InputAxis.MouseY) * (float)Time.Instance.DeltaTime;
           }
           else
           {
               _angleVelHorz *= _damping;
               _angleVelVert *= _damping;
           }
           _angleHorz += _angleVelHorz;
           _angleVert += _angleVelVert;

           //move per keyboard
           if (Input.Instance.IsKeyDown(KeyCodes.Left))
           {
               _angleHorz -= _rotationSpeed * (float)Time.Instance.DeltaTime;
           }
           if (Input.Instance.IsKeyDown(KeyCodes.Right))
           {
               _angleHorz += _rotationSpeed * (float)Time.Instance.DeltaTime;
           }
           if (Input.Instance.IsKeyDown(KeyCodes.Up))
           {
               _angleVert -= _rotationSpeed * (float)Time.Instance.DeltaTime;
           }
           if (Input.Instance.IsKeyDown(KeyCodes.Down))
           {
               _angleVert += _rotationSpeed * (float)Time.Instance.DeltaTime;
           }

           float4x4 mtxRot = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(_angleVert);
           float4x4 mtxCam = float4x4.LookAt(0, 200, 400, 0, 50, 0, 0, 1, 0);

           RC.ModelView = mtxRot * float4x4.CreateTranslation(-100, 0, 0) * mtxCam;
           //colordecloration
           //RC.SetShaderParam(VColorParam, new float4(0.5f, 0.8f, 0, 1));
           
           //mapping
           RC.SetShaderParamTexture(_vTextureParam, _iTex1);
           RC.Render(Mesh);

           RC.ModelView = mtxRot * float4x4.CreateTranslation(100, 0, 0) * mtxCam;
           //colordecloration
           //RC.SetShaderParam(VColorParam, new float4(0.8f, 0.5f, 0, 1));

           //mapping
           RC.SetShaderParamTexture(_vTextureParam, _iTex2);
           RC.Render(MeshFace);
           Present();
       }

       public override void Resize()
       {
           RC.Viewport(0, 0, Width, Height);

           float aspectRatio = Width / (float)Height;
           RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 5000);
       }

       public static void Main()
       {
           
           Simple app = new Simple();
           app.Run();
       }

    }
    }
