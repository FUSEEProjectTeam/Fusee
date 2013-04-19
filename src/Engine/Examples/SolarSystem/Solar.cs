using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.Solar
{
    public class Solar : RenderCanvas
    {
        public List<RenderJob> RenderJobs = new List<RenderJob>();
        public List<SceneEntity> SceneMembers = new List<SceneEntity>();
        
        // Planet Geometry
        private Geometry planetgeometry;

        // Spacebox
        private Geometry spacebox;
        private SceneEntity _spacebox;
        private Renderer spaceboxRenderer;
        private PlanetMaterial spaceboxMaterial;
        private IShaderParam spaceboxIShaderParam;
        private ImageData spaceboxImage;
        private ITexture spaceboxITexture;

        //Spped of Planets
     
        private static float3 _speedearth = new float3(0, 0.69635f, 0);

        
        // Null Objects
        private SceneEntity _emptyMoon = new SceneEntity();
        private SceneEntity _emptyMercury = new SceneEntity();
        private SceneEntity _emptyVenus = new SceneEntity();
        private SceneEntity _emptyEarth = new SceneEntity();
        private SceneEntity _emptyMars = new SceneEntity();
        private SceneEntity _emptyJupiter = new SceneEntity();
        private SceneEntity _emptySaturn = new SceneEntity();
        private SceneEntity _emptyUranus = new SceneEntity();
        private SceneEntity _emptyNeptun = new SceneEntity();

        // Null Object Actions
        private ActionCode _emptyMoonAction;
        private ActionCode _emptyMercuryAction = new PlanetAction(_speedearth * 4.1477f);
        private ActionCode _emptyVenusAction = new PlanetAction(_speedearth * 1.6150f);
        private ActionCode _emptyEarthAction = new PlanetAction(_speedearth);
        private ActionCode _emptyMarsAction = new PlanetAction(_speedearth * 0.5320f);
        private ActionCode _emptyJupiterAction = new PlanetAction(_speedearth * 0.0833f);
        private ActionCode _emptySaturnAction = new PlanetAction(_speedearth * 0.03476f);
        private ActionCode _emptyUranusAction = new PlanetAction(_speedearth * 0.0119f);
        private ActionCode _emptyNeptunAction = new PlanetAction(_speedearth * 0.0062f);

        // Sun
        private SceneEntity _sun;
        private Renderer _sunRenderer;
        private PlanetMaterial _sunMaterial;
        //private PlanetAction _sunAction = new PlanetAction(new float3(0, 0.1f, 0));
        private IShaderParam _sunIShaderParam;
        private ImageData _sunImage;
        private ITexture _sunITexture;

        // mercury
        private SceneEntity _mercury;
        private Renderer _mercuryRenderer;
        private PlanetMaterial _mercuryMaterial;
        private PlanetAction _mercuryAction = new PlanetAction(_speedearth * 6.2234f);
        private IShaderParam _mercuryIShaderParam;
        private ImageData _mercuryImage;
        private ITexture _mercuryITexture;

        // venus
        private SceneEntity _venus;
        private Renderer _venusRenderer;
        private PlanetMaterial _venusMaterial;
        private PlanetAction _venusAction = new PlanetAction(_speedearth * 1.5021f);
        private IShaderParam _venusIShaderParam;
        private ImageData _venusImage;
        private ITexture _venusITexture;

        // Earth
        private SceneEntity _earth;
        private Renderer _earthRenderer;
        private PlanetMaterial _earthMaterial;
        private PlanetAction _earthAction = new PlanetAction(new float3(0, 0.69635f * 365, 0));
        private IShaderParam _earthIShaderParam;
        private ImageData _earthImage;
        private ITexture _earthITexture;

        // moon
        private SceneEntity _moon;
        private Renderer _moonRenderer;
        private PlanetMaterial _moonMaterial;
        private PlanetAction _moonAction = new PlanetAction(new float3(0, 0.1f, 0));
        private IShaderParam _moonIShaderParam;
        private ImageData _moonImage;
        private ITexture _moonITexture;
        
         
        // mars
        private SceneEntity _mars;
        private Renderer _marsRenderer;
        private PlanetMaterial _marsMaterial;
        private PlanetAction _marsAction = new PlanetAction(_speedearth * 374.125f);
        private IShaderParam _marsIShaderParam;
        private ImageData _marsImage;
        private ITexture _marsITexture;

        // jupiter
        private SceneEntity _jupiter;
        private Renderer _jupiterRenderer;
        private PlanetMaterial _jupiterMaterial;
        private PlanetAction _jupiterAction = new PlanetAction(_speedearth  * 882.62f);
        private IShaderParam _jupiterIShaderParam;
        private ImageData _jupiterImage;
        private ITexture _jupiterITexture;

        // saturn
        private SceneEntity _saturn;
        private Renderer _saturnRenderer;
        private PlanetMaterial _saturnMaterial;
        private PlanetAction _saturnAction = new PlanetAction(_speedearth * 820.61f);
        private IShaderParam _saturnIShaderParam;
        private ImageData _saturnImage;
        private ITexture _saturnITexture;

        // uranus
        private SceneEntity _uranus;
        private Renderer _uranusRenderer;
        private PlanetMaterial _uranusMaterial;
        private PlanetAction _uranusAction = new PlanetAction(_speedearth * 509.30f);
        private IShaderParam _uranusIShaderParam;
        private ImageData _uranusImage;
        private ITexture _uranusITexture;

        // neptun
        private SceneEntity _neptun;
        private Renderer _neptunRenderer;
        private PlanetMaterial _neptunMaterial;
        private PlanetAction _neptunAction = new PlanetAction(_speedearth * 544.10f);
        private IShaderParam _neptunIShaderParam;
        private ImageData _neptunImage;
        private ITexture _neptunITexture;


        //Light test
        //private SpotLight spot = new SpotLight(0);
        //private PointLight point = new PointLight(1);
        private DirectionalLight direct = new DirectionalLight(new float3(-500,1000,0),new float4(1,1,1,1),new float3(0,0,0),0);
        //private DirectionalLight direct2 = new DirectionalLight(new float3(500, 1000, 0), new float4(1, 1, 1, 1), new float3(0, 0, 0), 1);



        // Scene Camera
        private SceneEntity cameraholder = new SceneEntity();
        private Camera scenecamera;
        private CameraScript camscript = new CameraScript();
        private SceneEntity WorldOrigin = new SceneEntity();
        private ActionCode camrotation = new RotationScript();
        public override void Init()
        {
            //Time.Instance.TimeFlow = 0.01f;
            
            SceneManager.RC = RC;

            WorldOrigin.AddComponent(camrotation);
            

            planetgeometry = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Sphere.obj.model"));
            spacebox = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/spacebox.obj.model")); 

            //Setup Scene Camera
            cameraholder.name = "CameraOwner";
            cameraholder.transform.LocalPosition = new float3(0, 0, 10);
            scenecamera = new Camera(cameraholder.transform);
            scenecamera.Resize(Width, Height);
            cameraholder.AddComponent(camscript);
            cameraholder.AddComponent(scenecamera);
            cameraholder.AddComponent(direct);
            //cameraholder.AddComponent(direct2);
            camscript.Init(cameraholder);
            SceneManager.Manager.AddSceneEntity(WorldOrigin);
            WorldOrigin.AddChild(cameraholder);

            // Setup Space Box
            _spacebox = new SceneEntity { name = "Spacebox" };
            _spacebox.transform.LocalPosition = cameraholder.transform.LocalPosition;
            spaceboxMaterial = new PlanetMaterial(MoreShaders.GetShader("simpel", RC));
            spaceboxImage = RC.LoadImage("Assets/spaceboxTexture.png");
            spaceboxIShaderParam = spaceboxMaterial.sp.GetShaderParam("texture1");
            spaceboxITexture = RC.CreateTexture(spaceboxImage);
            spaceboxImage.PixelData = null;
            spaceboxMaterial.Tex = spaceboxITexture;
            spaceboxITexture = null;
            spaceboxMaterial.Textureparam = spaceboxIShaderParam;
            spaceboxRenderer = new Renderer(spacebox);
            spaceboxRenderer.material = spaceboxMaterial;
            _spacebox.AddComponent(spaceboxRenderer);
            SceneManager.Manager.AddSceneEntity(_spacebox);

            // Setup Empty Objects
            _emptyMercuryAction.Init(_emptyMercury);
            _emptyVenusAction.Init(_emptyVenus);
            _emptyEarthAction.Init(_emptyEarth);
            _emptyMarsAction.Init(_emptyMars);
            _emptyJupiterAction.Init(_emptyJupiter);
            _emptySaturnAction.Init(_emptySaturn);
            _emptyUranusAction.Init(_emptyUranus);
            _emptyNeptunAction.Init(_emptyNeptun);

            SceneManager.Manager.AddSceneEntity(_emptyMercury);
            SceneManager.Manager.AddSceneEntity(_emptyVenus);
            SceneManager.Manager.AddSceneEntity(_emptyEarth);
            SceneManager.Manager.AddSceneEntity(_emptyMars);
            SceneManager.Manager.AddSceneEntity(_emptyJupiter);
            SceneManager.Manager.AddSceneEntity(_emptySaturn);
            SceneManager.Manager.AddSceneEntity(_emptyUranus);
            SceneManager.Manager.AddSceneEntity(_emptyNeptun);

            _emptyMercury.AddComponent(_emptyMercuryAction);
            _emptyVenus.AddComponent(_emptyVenusAction);
            _emptyEarth.AddComponent(_emptyEarthAction);
            _emptyMars.AddComponent(_emptyMarsAction);
            _emptyJupiter.AddComponent(_emptyJupiterAction);
            _emptySaturn.AddComponent(_emptySaturnAction);
            _emptyUranus.AddComponent(_emptyUranusAction);
            _emptyNeptun.AddComponent(_emptyNeptunAction);

            // Setup Earth
            _earth = new SceneEntity {name = "Earth"};
            _earthAction.Init(_earth);
            _earthMaterial = new PlanetMaterial(MoreShaders.GetShader("diffuse2", RC));
            _earthImage = RC.LoadImage("Assets/earth.jpg");
            _earthIShaderParam = _earthMaterial.sp.GetShaderParam("texture1");
            _earthITexture = RC.CreateTexture(_earthImage);
            _earthMaterial.Tex = _earthITexture;
            _earthMaterial.Textureparam = _earthIShaderParam;
            _earthRenderer = new Renderer(planetgeometry);
            _earthRenderer.material = _earthMaterial;
            _earth.AddComponent(_earthRenderer);
            _earth.AddComponent(_earthAction);
            _earth.transform.LocalPosition = new float3(2.9f,0,0);
            _earth.transform.LocalScale = new float3(0.1f, 0.1f, 0.1f);
            //SceneManager.Manager.AddSceneEntity(_earth);
            _emptyEarth.AddChild(_earth);

             //Setup Moon
            _emptyMoonAction = new MoonAction(_earth, _speedearth*5.0f);
            _emptyMoon.transform.LocalPosition = _earth.transform.LocalPosition;
            _moon = new SceneEntity { name = "Moon" };
            _emptyMoonAction.Init(_emptyMoon);
            _moonMaterial = new PlanetMaterial(MoreShaders.GetShader("diffuse2", RC));
            _moonImage = RC.LoadImage("Assets/moon.jpg");
            _moonIShaderParam = _moonMaterial.sp.GetShaderParam("texture1");
            _moonITexture = RC.CreateTexture(_moonImage);
            _moonMaterial.Tex = _moonITexture;
            _moonMaterial.Textureparam = _moonIShaderParam;
            _moonRenderer = new Renderer(planetgeometry);
            _moonRenderer.material = _moonMaterial;
            _moon.AddComponent(_moonRenderer);
            _emptyMoon.AddComponent(_emptyMoonAction);
            _moon.transform.LocalPosition = new float3(0.5f, 0, 0);
            _moon.transform.LocalScale = new float3(0.05f, 0.05f, 0.05f);
            SceneManager.Manager.AddSceneEntity(_emptyMoon);
            _emptyMoon.AddChild(_moon);

            // Setup sun
            _sun = new SceneEntity { name = "Sun" };
            //_sunAction.Init(_sun);
            _sunMaterial = new PlanetMaterial(MoreShaders.GetShader("diffuse2", RC));
            _sunImage = RC.LoadImage("Assets/sun.jpg");
            _sunIShaderParam = _sunMaterial.sp.GetShaderParam("texture1");
            _sunITexture = RC.CreateTexture(_sunImage);
            _sunMaterial.Tex = _sunITexture;
            _sunMaterial.Textureparam = _sunIShaderParam;
            _sunRenderer = new Renderer(planetgeometry);
            _sunRenderer.material = _sunMaterial;
            _sun.AddComponent(_sunRenderer);
            _sun.transform.LocalPosition = new float3(0,0,0);
            _sun.transform.LocalScale = new float3(2,2,2);
            //_earth.AddChild(_sun);
            SceneManager.Manager.AddSceneEntity(_sun);

            // Setup mercury
            _mercury = new SceneEntity { name = "Mercury" };
            _mercuryAction.Init(_mercury);
            _mercuryMaterial = new PlanetMaterial(MoreShaders.GetShader("diffuse2", RC));
            _mercuryImage = RC.LoadImage("Assets/merkur.jpg");
            _mercuryIShaderParam = _mercuryMaterial.sp.GetShaderParam("texture1");
            _mercuryITexture = RC.CreateTexture(_mercuryImage);
            _mercuryMaterial.Tex = _mercuryITexture;
            _mercuryMaterial.Textureparam = _mercuryIShaderParam;
            _mercuryRenderer = new Renderer(planetgeometry);
            _mercuryRenderer.material = _mercuryMaterial;
            _mercury.AddComponent(_mercuryRenderer);
            _mercury.AddComponent(_mercuryAction);
            _mercury.transform.LocalPosition = new float3(2.35f, 0, 0);
            _mercury.transform.LocalScale = new float3(0.05f, 0.05f, 0.05f);
            //SceneManager.Manager.AddSceneEntity(_mercury);
            _emptyMercury.AddChild(_mercury);

            // Setup venus
            _venus = new SceneEntity { name = "Venus" };
            _venusAction.Init(_venus);
            _venusMaterial = new PlanetMaterial(MoreShaders.GetShader("diffuse2", RC));
            _venusImage = RC.LoadImage("Assets/venus.jpg");
            _venusIShaderParam = _venusMaterial.sp.GetShaderParam("texture1");
            _venusITexture = RC.CreateTexture(_venusImage);
            _venusMaterial.Tex = _venusITexture;
            _venusMaterial.Textureparam = _venusIShaderParam;
            _venusRenderer = new Renderer(planetgeometry);
            _venusRenderer.material = _venusMaterial;
            _venus.AddComponent(_venusRenderer);
            _venus.AddComponent(_venusAction);
            _venus.transform.LocalPosition = new float3(2.6f, 0, 0);
            _venus.transform.LocalScale = new float3(0.08f, 0.08f, 0.08f);
            //SceneManager.Manager.AddSceneEntity(_venus);
            _emptyVenus.AddChild(_venus);

            // Setup mars
            _mars = new SceneEntity { name = "Mars" };
            _marsAction.Init(_mars);
            _marsMaterial = new PlanetMaterial(MoreShaders.GetShader("diffuse2", RC));
            _marsImage = RC.LoadImage("Assets/mars.jpg");
            _marsIShaderParam = _marsMaterial.sp.GetShaderParam("texture1");
            _marsITexture = RC.CreateTexture(_marsImage);
            _marsMaterial.Tex = _marsITexture;
            _marsMaterial.Textureparam = _marsIShaderParam;
            _marsRenderer = new Renderer(planetgeometry);
            _marsRenderer.material = _marsMaterial;
            _mars.AddComponent(_marsRenderer);
            _mars.AddComponent(_marsAction);
            _mars.transform.LocalPosition = new float3(3.25f, 0, 0);
            _mars.transform.LocalScale = new float3(0.07f, 0.07f, 0.07f);
            //SceneManager.Manager.AddSceneEntity(_mars);
            _emptyMars.AddChild(_mars);

            // Setup jupiter
            _jupiter = new SceneEntity { name = "Jupiter" };
            _jupiterAction.Init(_jupiter);
            _jupiterMaterial = new PlanetMaterial(MoreShaders.GetShader("diffuse2", RC));
            _jupiterImage = RC.LoadImage("Assets/jupiter.jpg");
            _jupiterIShaderParam = _jupiterMaterial.sp.GetShaderParam("texture1");
            _jupiterITexture = RC.CreateTexture(_jupiterImage);
            _jupiterMaterial.Tex = _jupiterITexture;
            _jupiterMaterial.Textureparam = _jupiterIShaderParam;
            _jupiterRenderer = new Renderer(planetgeometry);
            _jupiterRenderer.material = _jupiterMaterial;
            _jupiter.AddComponent(_jupiterRenderer);
            _jupiter.AddComponent(_jupiterAction);
            _jupiter.transform.LocalPosition = new float3(4, 0, 0);
            _jupiter.transform.LocalScale = new float3(0.4f, 0.4f, 0.4f);
            //SceneManager.Manager.AddSceneEntity(_jupiter);
            _emptyJupiter.AddChild(_jupiter);

            // Setup saturn
            _saturn = new SceneEntity { name = "Saturn" };
            _saturnAction.Init(_saturn);
            _saturnMaterial = new PlanetMaterial(MoreShaders.GetShader("diffuse2", RC));
            _saturnImage = RC.LoadImage("Assets/saturn.jpg");
            _saturnIShaderParam = _saturnMaterial.sp.GetShaderParam("texture1");
            _saturnITexture = RC.CreateTexture(_saturnImage);
            _saturnMaterial.Tex = _saturnITexture;
            _saturnMaterial.Textureparam = _saturnIShaderParam;
            _saturnRenderer = new Renderer(planetgeometry);
            _saturnRenderer.material = _saturnMaterial;
            _saturn.AddComponent(_saturnRenderer);
            _saturn.AddComponent(_saturnAction);
            _saturn.transform.LocalPosition = new float3(5, 0, 0);
            _saturn.transform.LocalScale = new float3(0.3f, 0.3f, 0.3f);
            //SceneManager.Manager.AddSceneEntity(_saturn);
            _emptySaturn.AddChild(_saturn);

            // Setup uranus
            _uranus = new SceneEntity { name = "Uranus" };
            _uranusAction.Init(_uranus);
            _uranusMaterial = new PlanetMaterial(MoreShaders.GetShader("diffuse2", RC));
            _uranusImage = RC.LoadImage("Assets/uranus.jpg");
            _uranusIShaderParam = _uranusMaterial.sp.GetShaderParam("texture1");
            _uranusITexture = RC.CreateTexture(_uranusImage);
            _uranusMaterial.Tex = _uranusITexture;
            _uranusMaterial.Textureparam = _uranusIShaderParam;
            _uranusRenderer = new Renderer(planetgeometry);
            _uranusRenderer.material = _uranusMaterial;
            _uranus.AddComponent(_uranusRenderer);
            _uranus.AddComponent(_uranusAction);
            _uranus.transform.LocalPosition = new float3(6, 0, 0);
            _uranus.transform.LocalScale = new float3(0.12f, 0.12f, 0.12f);
            //SceneManager.Manager.AddSceneEntity(_uranus);
            _emptyUranus.AddChild(_uranus);

            // Setup neptun
            _neptun = new SceneEntity { name = "Neptun" };
            _neptunAction.Init(_neptun);
            _neptunMaterial = new PlanetMaterial(MoreShaders.GetShader("diffuse2", RC));
            _neptunImage = RC.LoadImage("Assets/neptune.jpg");
            _neptunIShaderParam = _neptunMaterial.sp.GetShaderParam("texture1");
            _neptunITexture = RC.CreateTexture(_neptunImage);
            _neptunMaterial.Tex = _neptunITexture;
            _neptunMaterial.Textureparam = _neptunIShaderParam;
            _neptunRenderer = new Renderer(planetgeometry);
            _neptunRenderer.material = _neptunMaterial;
            _neptun.AddComponent(_neptunRenderer);
            _neptun.AddComponent(_neptunAction);
            _neptun.transform.LocalPosition = new float3(7, 0, 0);
            _neptun.transform.LocalScale = new float3(0.14f, 0.14f, 0.14f);
            //SceneManager.Manager.AddSceneEntity(_neptun);
            _emptyNeptun.AddChild(_neptun);

            // Random Rotations
            _emptyEarth.transform.LocalEulerAngles = new float3(0,45,0);
            _emptyMercury.transform.LocalEulerAngles = new float3(0, 55, 0);
            _emptyVenus.transform.LocalEulerAngles = new float3(0, 335, 0);
            _emptyMars.transform.LocalEulerAngles = new float3(0, 125, 0);
            _emptyJupiter.transform.LocalEulerAngles = new float3(0, 65, 0);
            _emptySaturn.transform.LocalEulerAngles = new float3(0, 95, 0);
            _emptyUranus.transform.LocalEulerAngles = new float3(0, 145, 0);
            _emptyNeptun.transform.LocalEulerAngles = new float3(0, 245, 0);
            
            camrotation.Init(WorldOrigin);
            RC.ClearColor = new float4(1, 0, 0, 1);
        }


        public override void RenderAFrame()
        {
            SceneManager.Manager.Traverse(this);
            //Debug.WriteLine(Time.Instance.FramePerSecondSmooth);
        }



        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);
            scenecamera.Resize(Width, Height);
        }

        public static void Main()
        {

            var app = new Solar();
            app.Run();
        }
    }
}
