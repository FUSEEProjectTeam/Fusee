  > ❌ **Outdated**

Before you read the SceneEntity HowTo you should understand the FirstSteps HowTo of FUSEE.

* You will learn how to use the SceneManagement with the example of a rotation. To do that you need a Meshobject in the assets folder and a texture.

First a global camaraobject is added to the scene. The SceneManagement function has to be called in the Init-Methode

    SceneManager.RC = RC;

Now you have to create two geometryobjects for the SceneManagement. In this example there will be three.

But first, a material-class has to be created. In these methods the textures and shaders will be applied.


       public IShaderParam Textureparam;

       public ITexture Tex;

       public Material(ShaderProgram shaderProgram)

       {

           sp = shaderProgram;

       }

       public Material(ShaderProgram shaderProgram, string texturepath)

       {

           sp = shaderProgram;

           Textureparam = sp.GetShaderParam("texture1");

           ImageData Image = SceneManager.RC.LoadImage(texturepath);

           Tex = SceneManager.RC.CreateTexture(Image);

       }

       public override void Update(RenderContext renderContext)

       {

           renderContext.SetShader(sp);

           renderContext.SetShaderParamTexture(Textureparam, Tex);

       }

Now mesh objects are created

    Geometry object1 = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Teapot.obj.model"));

    //The object that holds the whole thing together is called SceneEntity

           SceneEntity teapot = new SceneEntity("teapot", new Material(MoreShaders.GetShader("diffuse2",RC),"Assets/world_map.jpg"),new Renderer(object1));

           SceneEntity teapot2 = new SceneEntity("teapot", new Material(MoreShaders.GetShader("diffuse2", RC), "Assets/jupiter.jpg"), new Renderer(object1));

           SceneEntity teapot3 = new SceneEntity("teapot", new Material(MoreShaders.GetShader("diffuse2", RC), "Assets/world_map.jpg"), new Renderer(object1));

Objects from type SceneEntity contain all properties of scene management.

In this example, a class is created, which can rotate the mesh objects.



       public override void Update()

       {

           transform.LocalEulerAngles += new float3(0, 20 * (float)Time.Instance.DeltaTime, 0);
       }

Next, the objects are attached to the objects they should depend on. The position is also determined.

    teapot.AddChild(teapot2);

           teapot2.transform.GlobalPosition = new float3(-300,0,0); stehen.

    //the second Teapot got an offset of 500 pixels to the first Teapot. So the Teapot is on the left side.

           teapot2.AddChild(teapot3);

           teapot3.transform.GlobalPosition = new float3(-500, 0, 0);

    //the third Teapot got an offset of 500 pixels to the second Teapot. So the Teapot is on the left side.

Now we need to determine the camera position and so we have write a class with a variable in it, containing the necessary information.

    transform.LocalPosition = new float3(0,0,1000);

    //the camara is now on the backside


A variable for the camera properties is created. It is of type SceneEntyty and receives the created camera control as constructor.

    SceneEntity camera= new SceneEntity("Camera",new cameracontrol());

The properties are set to the global camera.
    camera.AddComponent(SceneCam);

Afterwards, the camera is placed in the scene and you will be given a light shader.

    DirectionalLight directionallight= new DirectionalLight(0);

           camera.AddComponent(directionallight);
           SceneManager.Manager.AddSceneEntity(camera );

Finally, the applied rotation of the scene in which it is assigned an object is created with the rotational properties and is appended and started by SceneManement.

           teapot.AddComponent(teeaktion);

           teapot2.AddComponent(teeaktion2);

           teeaktion.Init(teapot);

           teeaktion2.Init(teapot2);

           SceneManager.Manager.AddSceneEntity(teapot);//teapot is added

           SceneManager.Manager.StartActionCode();

           RC.ClearColor = new float4(0.1f, 0.1f, 0.5f, 1);

Now the scene management must of course still in the RenderAFrame method.

    SceneManager.Manager.Traverse(this);

And also be clarified in the Resize method must change the camera even when the window size is not distorted.

    SceneCam.Resize(Width , Height);


***

    //Main
    using System.IO;
    using Fusee.Engine;
    using Fusee.SceneManagement;
    using Fusee.Math;

    namespace Examples.Scene_Management_Tut
    {
    public class Scene_Management_Tut : RenderCanvas
    {

        Camera SceneCam = new Camera();
        // is called on startup
        public override void Init()
        {
            SceneManager.RC = RC;
            Geometry object1 = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Teapot.obj.model"));
           
            SceneEntity teapot = new SceneEntity("teapot", new SourceMaterial(MoreShaders.GetShader("diffuse2",RC),"Assets/world_map.jpg"),new Renderer(object1));
            SceneEntity teapot2 = new SceneEntity("teapot", new SourceMaterial(MoreShaders.GetShader("diffuse2", RC), "Assets/jupiter.jpg"), new Renderer(object1));
            SceneEntity teapot3 = new SceneEntity("teapot", new SourceMaterial(MoreShaders.GetShader("diffuse2", RC), "Assets/world_map.jpg"), new Renderer(object1));
            teapot.AddChild(teapot2);
            teapot2.transform.GlobalPosition = new float3(-300,0,0);
            teapot2.AddChild(teapot3);
            teapot3.transform.GlobalPosition = new float3(-500, 0, 0);
            SceneEntity camera = new SceneEntity("Camera",new cameracontrol()); 
            camera.AddComponent(SceneCam);
            DirectionalLight directionallight = new DirectionalLight(0);
            camera.AddComponent(directionallight );
            SceneManager.Manager.AddSceneEntity(camera);
            teapotaction teaaction = new teapotaction();
            teapotaction teaaction2 =  new teapotaction();

            teapot.AddComponent(teaaction);
            teapot2.AddComponent(teaaction2);
            teaaction.Init(teapot);
            teaaction2.Init(teapot2);
            SceneManager.Manager.AddSceneEntity(teapot);//teapot is added
            SceneManager.Manager.StartActionCode();
            RC.ClearColor = new float4(0.1f, 0.1f, 0.5f, 1);
        }

        // is called once a frame
        public override void RenderAFrame()
        {
            SceneManager.Manager.Traverse(this);
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            //Present();
        }

        // is called when the window was resized
        public override void Resize()
        {
            SceneCam.Resize(Width , Height); 
            RC.Viewport(0, 0, Width, Height);

         //   var aspectRatio = Width / (float)Height;
           // RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);
        }

        public static void Main()
        {
            var app = new Scene_Management_Tut();
            app.Run();
        }
    }
    }

***

    //teapotaction
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fusee.Engine;
    using Fusee.Math;
    using Fusee.SceneManagement;

    namespace Examples.Scene_Management_Tut
    {
    public class teapotaction:ActionCode

    {
        public override void Update()
        {
            transform.LocalEulerAngles += new float3(0, 20 * (float)Time.Instance.DeltaTime, 0);
        }
        
    }
    }

***


    //Material
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fusee.Engine;
    using Fusee.Math;
    using Fusee.SceneManagement;

    namespace Examples.Scene_Management_Tut
    {
    public class SourceMaterial : Material
    {
        public IShaderParam Textureparam;
        public ITexture Tex;

        public SourceMaterial(ShaderProgram shaderProgram)
        {
            sp = shaderProgram;
        }

        public SourceMaterial(ShaderProgram shaderProgram, string texturepath)
        {
            sp = shaderProgram;
            Textureparam = sp.GetShaderParam("texture1");
            ImageData Image = SceneManager.RC.LoadImage(texturepath);
            Tex = SceneManager.RC.CreateTexture(Image);
        }

        public override void Update(RenderContext renderContext)
        {
            renderContext.SetShader(sp);
            renderContext.SetShaderParamTexture(Textureparam, Tex);

        }
    }
    }


***


    //Cameracontrol
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fusee.Math;
    using Fusee.SceneManagement;

    namespace Examples.Scene_Management_Tut
    {
    public class cameracontrol:ActionCode
       
    {
        public override void Start()
        {
            
           transform.LocalPosition = new float3(0,0,1000);


        }
        public override void Update()
        {
            base.Update();
        }
    }
    }




