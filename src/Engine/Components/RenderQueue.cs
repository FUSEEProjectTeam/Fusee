using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.Math;


namespace SceneManagement
{
    public class RenderQueue : RenderCanvas
    {
        public List<RenderJob> RenderJobs = new List<RenderJob>(); 
        public float4x4 Camera = float4x4.LookAt(0, 200, 2000, 0, 50, 0, 0, 1, 0);
        private Material _material = new Material();
       
        //TestZone
        private SceneEntity _testGE;
        private SceneEntity _childGE;
        private TestBehaviour _testBE;

        protected float4 _farbe = new float4(0.5f, 1, 1, 0.3f);
        protected IShaderParam _vColorParam;

        public override void Init()
        {
            
            _testBE = new TestBehaviour(_testGE._traversalState);
            _testGE.AddComponent(_testBE);
            _testGE.AddComponent(new Renderer());
            _childGE.AddComponent(new Renderer());
            _testGE.AddChild(_childGE);

            ShaderProgram sp = RC.CreateShader(_material._vs, _material._ps);
            RC.SetShader(sp);
            _vColorParam = sp.GetShaderParam("vColor");
            RC.ClearColor = new float4(1, 1, 1, 1);
            RC.SetShaderParam(_vColorParam, _farbe);
        }


        public override void  RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            //Console.WriteLine("Draw Call Count: "+Drawcalls.Count);
            
            //Order: Matrix, Material, Mesh
            for (int i = 0; i < RenderJobs.Count; i+=3 )
            {
                RC.ModelView = RenderJobs[i].GetMatrix() * Camera;
                RC.Render(RenderJobs[i+2].GetMesh());
                //Console.WriteLine("The mesh is "+drawCall._Mesh.ToString());
            }
            Present();
            _childGE.Log("Kind Weltmatrix");
            //Console.WriteLine("Rendering at "+DeltaTime+"ms and "+(1/DeltaTime)+" FPS"); // Use this to checkout Framerate
            RenderJobs.Clear();
        }

        public void AddRenderJob(RenderJob job)
        {
            RenderJobs.Add(job);
        }

        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            float aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 5000);
        }

        public static void Main()
        {
            RenderQueue app = new RenderQueue();
            app.Run();
        }
    }
}
