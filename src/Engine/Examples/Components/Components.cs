using System;
using System.Collections.Generic;
using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.Components
{
    public class Components : RenderCanvas
    {
        public List<RenderJob> RenderJobs = new List<RenderJob>(); 
        public List<SceneEntity> SceneMembers = new List<SceneEntity>(); 
        public float4x4 Camera = float4x4.LookAt(0, 0, -10, 0, 0, 0, 0, 1, 0);
        private Material _material = new Material();
       
        //TestZone
        private SceneEntity TestEntity = new SceneEntity();
        private Renderer testrenderer = new Renderer();
        private TestBehaviour testscript = new TestBehaviour();

        // Child
        private SceneEntity ChildEntity = new SceneEntity();
        private Renderer Childrenderer = new Renderer();
        private ChildAction Childscript = new ChildAction();
        //Light test
        private SpotLight spot = new SpotLight(0);
        private PointLight point = new PointLight(1);
        private DirectionalLight direct = new DirectionalLight(2);


        //ExceptionTest
        private Renderer testrendererException = new Renderer();

        //some values
        protected float4 _farbe = new float4(1, 0, 0, 1);
        //protected IShaderParam _vColorParam;

        // Scene Camera Setup
        private SceneEntity cameraholder = new SceneEntity();
        private Camera scenecamera;
        private CameraScript camscript = new CameraScript();
        public override void Init()
        {

            SceneManager.RC = RC;
            //Setup Camera
            cameraholder.name = "CameraOwner";
            cameraholder.transform.LocalPosition = new float3(0,0,10);
            scenecamera = new Camera(cameraholder.transform);
            cameraholder.AddComponent(camscript);
            cameraholder.AddComponent(scenecamera);

            camscript.Init(cameraholder);
            SceneManager.Manager.AddSceneEntity(cameraholder);
            //SceneManager.Manager.AddSceneEntity(cameraholder);
           
            // Parent
            TestEntity.name = "erster";
            TestEntity.AddComponent(testrenderer);
            TestEntity.AddComponent(testrendererException); //TODO: Test Exceptions
            TestEntity.AddComponent(testscript);
            TestEntity.AddComponent(spot);
            TestEntity.AddComponent(point);
            TestEntity.AddComponent(direct);
            SceneManager.Manager.AddSceneEntity(TestEntity);
            testscript.Init(TestEntity);
            //TestEntity.AddChild(ChildEntity); // Als Child hinzugefuegt
            SceneManager.Manager.AddSceneEntity(ChildEntity);
            
            // Child
            ChildEntity.AddComponent(Childrenderer);
            ChildEntity.AddComponent(Childscript);
            Childscript.Init(ChildEntity);
            Childscript.Start();
            testscript.Start();
            //TestEntity.AddChild(cameraholder);
            /*
            ShaderProgram sp = RC.CreateShader(_material._vs, _material._ps);
            RC.SetShader(sp);
            _vColorParam = sp.GetShaderParam("uColor");        
            RC.SetShaderParam(_vColorParam, _farbe);*/


            RC.ClearColor = new float4(0, 0, 0, 1);
            //SceneManager.Manager.SetInput(In);
        }


        public override void  RenderAFrame()
        {
            SceneManager.Manager.Traverse(this);
        }



        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            float aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 5000);
        }

        public static void Main()
        {
            
            Components app = new Components();
            app.Run();
        }
    }
}
