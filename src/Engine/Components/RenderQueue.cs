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
        public List<DrawCall> Drawcalls = new List<DrawCall>();
        public float4x4 Camera = float4x4.LookAt(0, 200, 2000, 0, 50, 0, 0, 1, 0);
        private Material _material = new Material();
        private FuseeObject _scene = new FuseeObject();
        
        //TestZone
        private GameEntity _testGE;
        private TestBehaviour _testBE;
        protected float4 _farbe = new float4(0.5f, 1, 1, 0.3f);
        protected IShaderParam _vColorParam;

        public override void Init()
        {
            _scene._RenderQueue = this;
            _testGE = new GameEntity(_scene);
            _testBE = new TestBehaviour(_testGE._traversalState);
            _testGE.AddComponent(_testBE);
            _testGE.AddComponent(new Renderer());
            _scene.Instantiate(_testGE);
            ShaderProgram sp = RC.CreateShader(_material._vs, _material._ps);
            RC.SetShader(sp);
            _vColorParam = sp.GetShaderParam("vColor");
            RC.ClearColor = new float4(1, 1, 1, 1);
            RC.SetShaderParam(_vColorParam, _farbe);
        }


        public override void  RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            _scene.Traverse();
            //Console.WriteLine("Draw Call Count: "+Drawcalls.Count);
            foreach (var drawCall in Drawcalls)
            {
                RC.ModelView = drawCall._Transform.WorldMatrix*Camera;
                RC.Render(drawCall._Mesh);
                //Console.WriteLine("The mesh is "+drawCall._Mesh.ToString());
            }
            Present();
            //Console.WriteLine("Rendering at "+DeltaTime+"ms and "+(1/DeltaTime)+" FPS"); // Use this to checkout Framerate
            Drawcalls.Clear();
        }

        public void AddDrawCall(DrawCall draw)
        {
            Drawcalls.Add(draw);
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
