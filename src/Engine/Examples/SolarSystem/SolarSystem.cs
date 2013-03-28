using System;
using System.Collections.Generic;
using System.IO;
using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.SolarSystem
{
    public class SolarSystem : RenderCanvas
    {
        public List<RenderJob> RenderJobs = new List<RenderJob>();
        public List<SceneEntity> SceneMembers = new List<SceneEntity>();
        
        // Planet Geometry
        private Geometry planetgeometry; 

        // Earth
        private SceneEntity _earth;
        private Renderer _earthRenderer;
        private PlanetMaterial _earthMaterial;
        private PlanetAction _earthAction = new PlanetAction(new float3(0,0.1f,0));
        private IShaderParam _earthIShaderParam;
        private ImageData _earthImage;
        private ITexture _earthITexture;

        // Glibber
        private SceneEntity _glibber;
        private Renderer _glibberRenderer;
        private PlanetMaterial _glibberMaterial;
        private PlanetAction _glibberAction = new PlanetAction(new float3(0, 0.1f, 0));
        private IShaderParam _glibberIShaderParam;
        private ImageData _glibberImage;
        private ITexture _glibberITexture;


        //Light test
        private SpotLight spot = new SpotLight(0);
        private PointLight point = new PointLight(1);
        private DirectionalLight direct = new DirectionalLight(2);




        // Scene Camera
        private SceneEntity cameraholder = new SceneEntity();
        private Camera scenecamera;
        private CameraScript camscript = new CameraScript();

        public override void Init()
        {

            SceneManager.RC = RC;



            planetgeometry = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Sphere.obj.model")); 
            //Setup Scene Camera
            cameraholder.name = "CameraOwner";
            cameraholder.transform.LocalPosition = new float3(0, 0, 10);
            scenecamera = new Camera(cameraholder.transform);
            cameraholder.AddComponent(camscript);
            cameraholder.AddComponent(scenecamera);
            cameraholder.AddComponent(direct);
            camscript.Init(cameraholder);
            SceneManager.Manager.AddSceneEntity(cameraholder);

            // Setup Earth
            _earth = new SceneEntity {name = "Earth"};
            _earthAction.Init(_earth);
            _earthMaterial = new PlanetMaterial(MoreShaders.GetShader("simpel", RC));
            _earthImage = RC.LoadImage("Assets/earth.png");
            _earthIShaderParam = _earthMaterial.sp.GetShaderParam("texture1");
            _earthITexture = RC.CreateTexture(_earthImage);
            _earthMaterial.Tex = _earthITexture;
            _earthMaterial.Textureparam = _earthIShaderParam;
            _earthRenderer = new Renderer(planetgeometry);
            _earthRenderer.material = _earthMaterial;
            _earth.AddComponent(_earthRenderer);
            _earth.AddComponent(_earthAction);
            SceneManager.Manager.AddSceneEntity(_earth);

            // Setup Earth
            _glibber = new SceneEntity { name = "Earth" };
            _glibberAction.Init(_glibber);
            _glibberMaterial = new PlanetMaterial(MoreShaders.GetShader("simpel", RC));
            _glibberImage = RC.LoadImage("Assets/gruen.jpg");
            _glibberIShaderParam = _glibberMaterial.sp.GetShaderParam("texture1");
            _glibberITexture = RC.CreateTexture(_glibberImage);
            _glibberMaterial.Tex = _glibberITexture;
            _glibberMaterial.Textureparam = _glibberIShaderParam;
            _glibberRenderer = new Renderer(planetgeometry);
            _glibberRenderer.material = _glibberMaterial;
            _glibber.AddComponent(_glibberRenderer);
            _glibber.AddComponent(_glibberAction);
            _glibber.transform.LocalPosition = new float3(-3,0,0);
            _earth.AddChild(_glibber);



            RC.ClearColor = new float4(1, 0, 0, 1);
        }


        public override void RenderAFrame()
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

            var app = new SolarSystem();
            app.Run();
        }
    }
}
