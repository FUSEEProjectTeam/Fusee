using System.Collections.Generic;
using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.Components
{
    public class Components : RenderCanvas
    {
        private SceneManager _queue = new SceneManager();
        public List<RenderJob> RenderJobs = new List<RenderJob>(); 
        public List<SceneEntity> SceneMembers = new List<SceneEntity>(); 
        public float4x4 Camera = float4x4.LookAt(0, 200, 2000, 0, 50, 0, 0, 1, 0);
        private Material _material = new Material();
       
        //TestZone
        private SceneEntity TestEntity = new SceneEntity();
        private Renderer testrenderer = new Renderer();
        private TestBehaviour testscript = new TestBehaviour();

        // Child
        private SceneEntity ChildEntity = new SceneEntity();
        private Renderer Childrenderer = new Renderer();
        private ChildAction Childscript = new ChildAction();
        

        protected float4 _farbe = new float4(1, 0, 0, 1);
        protected IShaderParam _vColorParam;

        public override void Init()
        {
            RC.Camera = Camera;
           
            // Parent
            TestEntity.AddComponent(testrenderer);
            TestEntity.AddComponent(testscript);
            testscript.Init(TestEntity);
            TestEntity.AddChild(ChildEntity); // Als Child hinzugefuegt
            _queue.SceneMembers.Add(TestEntity);
            
            // Child
            ChildEntity.AddComponent(Childrenderer);
            ChildEntity.AddComponent(Childscript);
            Childscript.Init(ChildEntity);
            Childscript.Start();
            testscript.Start();

            ShaderProgram sp = RC.CreateShader(_material._vs, _material._ps);
            RC.SetShader(sp);
            _vColorParam = sp.GetShaderParam("vColor");
            RC.ClearColor = new float4(1, 1, 1, 1);
            RC.SetShaderParam(_vColorParam, _farbe);
        }


        public override void  RenderAFrame()
        {
            _queue.Traverse(this, RC, Camera);
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
