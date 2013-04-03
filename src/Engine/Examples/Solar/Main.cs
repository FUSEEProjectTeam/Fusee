using System.Collections.Generic;
using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.Solar
{
    public class Solar : RenderCanvas
    {

        //Renderer
        private Renderer _sunrenderer = new Renderer();
        private Renderer _earthrenderer = new Renderer();
        private Renderer _moonrenderer = new Renderer();
        private Renderer _marsrenderer = new Renderer();

        public List<RenderJob> RenderJobs = new List<RenderJob>();
        public List<SceneEntity> SceneMembers = new List<SceneEntity>();

        // Camera
        private SceneEntity _cameraholder;
        private Camera _scenecamera;

        // Testplanet
        private Material _sunmaterial = new Material("sun.png");
        //PlanetAction _sunaction = new PlanetAction(new float3(0,0.2f,0.3f));
        private SceneEntity _sun = new SceneEntity();

        // Testplanet2
        //private Material _earthmaterial = new Material();
        private Material _earthmaterial = new Material("earth.jpg");
        PlanetAction _earthaction = new PlanetAction(new float3(0,0.2f,0));
        private SceneEntity _earth = new SceneEntity();
        private SceneEntity _earthholder = new SceneEntity();
        


        //private Material _moonmaterial = new Material();
        private Material _moonmaterial = new Material("moon.jpg");
        private SceneEntity _moon = new SceneEntity();

        //private Material _marsmaterial = new Material();
        private Material _marsmaterial = new Material("mars.jpg");
        private SceneEntity _mars = new SceneEntity();


        public override void Init()
        {

            _earthholder.transform.LocalPosition = new float3(-3, 0, 0);
            _earthholder.AddChild(_earth);
            _earthholder.AddComponent(_earthaction);

            SceneManager.RC = RC;

            // Camera
            _cameraholder = new SceneEntity();
            _cameraholder.transform.LocalPosition = new float3(0, 0, 10);
            _scenecamera = new Camera(_cameraholder.transform);
            _cameraholder.AddComponent(_scenecamera);
            SceneManager.Manager.AddSceneEntity(_cameraholder);

            // Planets _earthmaterial

            _sun.AddComponent(_sunrenderer);
            _sunmaterial.sp = RC.CreateShader(_sunmaterial._vs, _sunmaterial._ps);
            _sunmaterial.InitValues(RC);
            _sunrenderer.material = _sunmaterial;
            _sun.transform.LocalPosition = new float3(-3, 0, 0);
            SceneManager.Manager.AddSceneEntity(_sun);


            _earth.AddComponent(_earthrenderer);
            _earthmaterial.sp = RC.CreateShader(_earthmaterial._vs, _earthmaterial._ps);
            _earthmaterial.InitValues(RC);
            _earthrenderer.material = _earthmaterial;
            _earth.AddComponent(_earthaction);
            _earthaction.Init(_earth);
            _earth.transform.LocalPosition = new float3(0, 0, 0);
            SceneManager.Manager.AddSceneEntity(_earth);
            

            _moon.AddComponent(_moonrenderer);
            _moonmaterial.sp = RC.CreateShader(_moonmaterial._vs, _moonmaterial._ps);
            _moonmaterial.InitValues(RC);
            _moonrenderer.material = _moonmaterial;
            _moon.transform.LocalPosition = new float3(3,0,0);
            SceneManager.Manager.AddSceneEntity(_moon);

            _mars.AddComponent(_marsrenderer);
            _marsmaterial.sp = RC.CreateShader(_marsmaterial._vs, _marsmaterial._ps);
            _marsmaterial.InitValues(RC);
            _marsrenderer.material = _marsmaterial;
            _mars.transform.LocalPosition = new float3(6,0,0);
            SceneManager.Manager.AddSceneEntity(_mars);


            RC.ClearColor = new float4(0, 0, 0, 1);
            // is called on startup
        }

        public override void RenderAFrame()
        {
            // is called once a frame
            SceneManager.Manager.Traverse(this);
        }

        public override void Resize()
        {
            // is called when the window is resized
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 5000);
        }

        public static void Main()
        {
            var app = new Solar();
            app.Run();
        }

    }
}
