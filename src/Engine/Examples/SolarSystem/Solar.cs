using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;


//namespace Examples.SolarSystem

namespace Examples.Solar
{
    public class Solar : RenderCanvas
    {
        

        //Speed of Planets
        private static float3 _speedearth = new float3(0, 0.69635f, 0);

        Camera scenecamera;

        public override void Init()
        {
            SceneManager.RC = RC;
            SceneEntity _planet;
            // Lights
            DirectionalLight direct = new DirectionalLight(new float3(-500, 1000, 0), new float4(1, 1, 1, 1), new float3(0, 0, 0), 0);

            // Load Geometry

            Geometry planetgeometry = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Sphere.obj.model"));
            Geometry spacebox = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/spacebox.obj.model"));

            // Setup Empty Objects
            // Null Objects
            SceneEntity _emptyMoon;
            SceneEntity _emptyMercury;
            SceneEntity _emptyVenus;
            SceneEntity _emptyEarth;
            SceneEntity _emptyMars;
            SceneEntity _emptyJupiter;
            SceneEntity _emptySaturn;
            SceneEntity _emptyUranus;
            SceneEntity _emptyNeptun;
            _emptyMoon = new SceneEntity("emptyPlanetHolder", new MoonAction(_speedearth * 5.0f));
            _emptyMercury = new SceneEntity("emptyPlanetHolder", new PlanetAction(_speedearth * 4.1477f));
            _emptyVenus = new SceneEntity("emptyPlanetHolder", new PlanetAction(_speedearth * 1.6150f));
            _emptyEarth = new SceneEntity("emptyPlanetHolder", new PlanetAction(_speedearth));
            _emptyMars = new SceneEntity("emptyPlanetHolder", new PlanetAction(_speedearth * 0.5320f));
            _emptyJupiter = new SceneEntity("emptyPlanetHolder", new PlanetAction(_speedearth * 0.0833f));
            _emptySaturn = new SceneEntity("emptyPlanetHolder", new PlanetAction(_speedearth * 0.03476f));
            _emptyUranus = new SceneEntity("emptyPlanetHolder", new PlanetAction(_speedearth * 0.0119f));
            _emptyNeptun = new SceneEntity("emptyPlanetHolder", new PlanetAction(_speedearth * 0.0062f));
            SceneManager.Manager.AddSceneEntity(_emptyMoon);
            SceneManager.Manager.AddSceneEntity(_emptyMercury);
            SceneManager.Manager.AddSceneEntity(_emptyVenus);
            SceneManager.Manager.AddSceneEntity(_emptyEarth);
            SceneManager.Manager.AddSceneEntity(_emptyMars);
            SceneManager.Manager.AddSceneEntity(_emptyJupiter);
            SceneManager.Manager.AddSceneEntity(_emptySaturn);
            SceneManager.Manager.AddSceneEntity(_emptyUranus);
            SceneManager.Manager.AddSceneEntity(_emptyNeptun);

            //Setup Camera
            // Scene Camera
            SceneEntity cameraholder;
            CameraScript camscript;
            SceneEntity WorldOrigin;
            WorldOrigin = new SceneEntity("WorldOrigin", new RotationScript());
            SceneManager.Manager.AddSceneEntity(WorldOrigin);
            cameraholder = new SceneEntity("CameraOwner", new CameraScript(), WorldOrigin);
            cameraholder.transform.GlobalPosition = new float3(0, 0, 10);
            scenecamera = new Camera(cameraholder);
            scenecamera.Resize(Width, Height);

           
            // Setup Space Box
            SceneEntity _spaceBox = new SceneEntity("Spacebox", new PlanetMaterial(MoreShaders.GetShader("simpel", RC), "Assets/spaceboxTexture.png"), new Renderer(spacebox));
            SceneManager.Manager.AddSceneEntity(_spaceBox);

            
            // Setup Earth
            _planet = new SceneEntity("Earth", new PlanetAction(new float3(0, 0.69635f * 365, 0)), _emptyEarth, new PlanetMaterial(MoreShaders.GetShader("diffuse2", RC), "Assets/earth.jpg"), new Renderer(planetgeometry));
            _planet.transform.GlobalPosition = new float3(2.9f, 0, 0);
            _planet.transform.GlobalScale = new float3(0.1f, 0.1f, 0.1f);
            _planet.AddComponent(direct);
            
            //Setup Moon
            //_emptyMoon.transform.LocalPosition = _earth.transform.LocalPosition;
            //_emptyMoonAction = new MoonAction(_speedearth * 5.0f);
            //_emptyMoon.AddComponent(_emptyMoonAction);
            _planet = new SceneEntity("Moon", new PlanetAction(new float3(0, 2.7f, 0)), _emptyMoon, new PlanetMaterial(MoreShaders.GetShader("diffuse2", RC), "Assets/moon.jpg"), new Renderer(planetgeometry));
            _planet.transform.GlobalPosition = new float3(0.5f, 0, 0);
            _planet.transform.GlobalScale = new float3(0.05f, 0.05f, 0.05f);


            //SceneManager.Manager.AddSceneEntity(_emptyMoon);
            //_emptyMoon.AddChild(_moon);
            //_emptyMoonAction.Init(_emptyMoon);
            /*
            _moon = new SceneEntity { name = "Moon" };
            _emptyMoonAction = new MoonAction(_earth, _speedearth * 5.0f);
            
            
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
            _emptyMoonAction.Init(_emptyMoon);
            */
            // Setup sun
            _planet = new SceneEntity("Sun", new PlanetMaterial(MoreShaders.GetShader("diffuse2", RC), "Assets/sun.jpg"), new Renderer(planetgeometry));
            _planet.transform.LocalScale = new float3(2, 2, 2);
            SceneManager.Manager.AddSceneEntity(_planet);

            /*
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
            */
            // Setup mercury
            _planet = new SceneEntity("Mercury", new PlanetAction(_speedearth * 6.2234f), _emptyMercury, new PlanetMaterial(MoreShaders.GetShader("diffuse2", RC), "Assets/merkur.jpg"), new Renderer(planetgeometry));
            _planet.transform.GlobalPosition = new float3(2.35f, 0, 0);
            _planet.transform.GlobalScale = new float3(0.05f, 0.05f, 0.05f);
            /*
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
            */
            // Setup venus
            _planet = new SceneEntity("Venus", new PlanetAction(_speedearth * 1.5021f), _emptyVenus, new PlanetMaterial(MoreShaders.GetShader("diffuse2", RC), "Assets/venus.jpg"), new Renderer(planetgeometry));
            _planet.transform.GlobalPosition = new float3(2.6f, 0, 0);
            _planet.transform.GlobalScale = new float3(0.08f, 0.08f, 0.08f);
            /*
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
            _emptyVenus.AddChild(_venus);*/

            // Setup mars
            _planet = new SceneEntity("Mars", new PlanetAction(_speedearth * 374.125f), _emptyMars, new PlanetMaterial(MoreShaders.GetShader("diffuse2", RC), "Assets/mars.jpg"), new Renderer(planetgeometry));
            _planet.transform.GlobalPosition = new float3(3.25f, 0, 0);
            _planet.transform.GlobalScale = new float3(0.07f, 0.07f, 0.07f);
            /*
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
            _emptyMars.AddChild(_mars);*/

            // Setup jupiter
            _planet = new SceneEntity("Jupiter", new PlanetAction(_speedearth * 882.62f), _emptyJupiter, new PlanetMaterial(MoreShaders.GetShader("diffuse2", RC), "Assets/jupiter.jpg"), new Renderer(planetgeometry));
            _planet.transform.GlobalPosition = new float3(4, 0, 0);
            _planet.transform.GlobalScale = new float3(0.4f, 0.4f, 0.4f);
            /*
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
            */
            // Setup saturn
            _planet = new SceneEntity("Saturn", new PlanetAction(_speedearth * 820.61f), _emptySaturn, new PlanetMaterial(MoreShaders.GetShader("diffuse2", RC), "Assets/saturn.jpg"), new Renderer(planetgeometry));
            _planet.transform.GlobalPosition = new float3(5, 0, 0);
            _planet.transform.GlobalScale = new float3(0.3f, 0.3f, 0.3f);
            /*_saturn = new SceneEntity { name = "Saturn" };
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
            _emptySaturn.AddChild(_saturn);*/

            // Setup uranus
            _planet = new SceneEntity("Uranus", new PlanetAction(_speedearth * 509.30f), _emptyUranus, new PlanetMaterial(MoreShaders.GetShader("diffuse2", RC), "Assets/uranus.jpg"), new Renderer(planetgeometry));
            _planet.transform.GlobalPosition = new float3(6, 0, 0);
            _planet.transform.GlobalScale = new float3(0.12f, 0.12f, 0.12f);
            /*_uranus = new SceneEntity { name = "Uranus" };
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
            _emptyUranus.AddChild(_uranus);*/

            // Setup neptun
            _planet = new SceneEntity("Neptun", new PlanetAction(_speedearth * 544.10f), _emptyNeptun, new PlanetMaterial(MoreShaders.GetShader("diffuse2", RC), "Assets/neptune.jpg"), new Renderer(planetgeometry));
            _planet.transform.GlobalPosition = new float3(7, 0, 0);
            _planet.transform.GlobalScale = new float3(0.14f, 0.14f, 0.14f);
            /*_neptun = new SceneEntity { name = "Neptun" };
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
            _emptyNeptun.AddChild(_neptun);*/
            SceneManager.Manager.StartActionCode();
            // Random Rotations
            _emptyEarth.transform.LocalEulerAngles = new float3(0, 45, 0);
            _emptyMercury.transform.LocalEulerAngles = new float3(0, 55, 0);
            _emptyVenus.transform.LocalEulerAngles = new float3(0, 335, 0);
            _emptyMars.transform.LocalEulerAngles = new float3(0, 125, 0);
            _emptyJupiter.transform.LocalEulerAngles = new float3(0, 65, 0);
            _emptySaturn.transform.LocalEulerAngles = new float3(0, 95, 0);
            _emptyUranus.transform.LocalEulerAngles = new float3(0, 145, 0);
            _emptyNeptun.transform.LocalEulerAngles = new float3(0, 245, 0);

            //camrotation.Init(WorldOrigin);
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
