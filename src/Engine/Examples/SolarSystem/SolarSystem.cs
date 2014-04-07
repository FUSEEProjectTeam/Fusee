using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.SolarSystem
{
    // ReSharper disable UseObjectOrCollectionInitializer
    [FuseeApplication(Name = "SolarSystem", Description = "A Sample showing how to use FUSEE's built-in scene graph management system.")]
    public class SolarSystem : RenderCanvas
    {
        private static float3 _earthSpeed;
        private Camera _sceneCamera;

        public override void Init()
        {
            SceneManager.RC = RC;

            // Light
            var direct = new DirectionalLight(new float3(1, 1, 1), new float4(0.7f, 0.7f, 0.7f, 1),
                                              new float4(1f, 1f, 1f, 1), new float4(0.9f, 0.9f, 0.9f, 1),
                                              new float3(0, 0, 0), 0);

            // Load Meshes
            var planetMesh = MeshReader.LoadMesh(@"Assets/Sphere.obj.model");
            var spaceBoxMesh = MeshReader.LoadMesh(@"Assets/spacebox.obj.model");

            // Setup Empty Objects
            _earthSpeed = new float3(0, 0.69635f, 0);

            var emptyMoon = new SceneEntity("emptyPlanetHolder", new MoonAction(_earthSpeed*5.0f));
            var emptyMercury = new SceneEntity("emptyPlanetHolder", new PlanetAction(_earthSpeed*4.1477f));
            var emptyVenus = new SceneEntity("emptyPlanetHolder", new PlanetAction(_earthSpeed*1.6150f));
            var emptyEarth = new SceneEntity("emptyPlanetHolder", new PlanetAction(_earthSpeed));
            var emptyMars = new SceneEntity("emptyPlanetHolder", new PlanetAction(_earthSpeed*0.5320f));
            var emptyJupiter = new SceneEntity("emptyPlanetHolder", new PlanetAction(_earthSpeed*0.0833f));
            var emptySaturn = new SceneEntity("emptyPlanetHolder", new PlanetAction(_earthSpeed*0.03476f));
            var emptyUranus = new SceneEntity("emptyPlanetHolder", new PlanetAction(_earthSpeed*0.0119f));
            var emptyNeptun = new SceneEntity("emptyPlanetHolder", new PlanetAction(_earthSpeed*0.0062f));

            SceneManager.Manager.AddSceneEntity(emptyMoon);
            SceneManager.Manager.AddSceneEntity(emptyMercury);
            SceneManager.Manager.AddSceneEntity(emptyVenus);
            SceneManager.Manager.AddSceneEntity(emptyEarth);
            SceneManager.Manager.AddSceneEntity(emptyMars);
            SceneManager.Manager.AddSceneEntity(emptyJupiter);
            SceneManager.Manager.AddSceneEntity(emptySaturn);
            SceneManager.Manager.AddSceneEntity(emptyUranus);
            SceneManager.Manager.AddSceneEntity(emptyNeptun);

            // Scene Camera
            var worldOrigin = new SceneEntity("WorldOrigin", new RotationScript());
            SceneManager.Manager.AddSceneEntity(worldOrigin);

            var cameraholder = new SceneEntity("CameraOwner", new CameraScript(), worldOrigin);
            cameraholder.transform.GlobalPosition = new float3(0, 0, -10);

            _sceneCamera = new Camera(cameraholder);
            _sceneCamera.Resize(Width, Height);

            // Setup Space Box
            var spaceBox = new SceneEntity("Spacebox",
                                           new PlanetMaterial(MoreShaders.GetTextureShader(RC),
                                                              "Assets/spaceboxTexture.png"), new Renderer(spaceBoxMesh));
            SceneManager.Manager.AddSceneEntity(spaceBox);

            // Setup Sun
            var planet = new SceneEntity("Sun",
                                         new PlanetMaterial(MoreShaders.GetDiffuseTextureShader(RC), "Assets/sun.jpg"),
                                         new Renderer(planetMesh));

            planet.transform.LocalScale = new float3(2, 2, 2);
            SceneManager.Manager.AddSceneEntity(planet);

            // Setup Earth
            planet = new SceneEntity("Earth", new PlanetAction(new float3(0, 0.69635f*365, 0)), emptyEarth,
                                     new PlanetMaterial(MoreShaders.GetDiffuseTextureShader(RC), "Assets/earth.jpg"),
                                     new Renderer(planetMesh));

            planet.transform.GlobalPosition = new float3(2.9f, 0, 0);
            planet.transform.GlobalScale = new float3(0.1f, 0.1f, 0.1f);
            planet.AddComponent(direct);

            // Setup Moon
            planet = new SceneEntity("Moon", new PlanetAction(new float3(0, 2.7f, 0)), emptyMoon,
                                     new PlanetMaterial(MoreShaders.GetDiffuseTextureShader(RC), "Assets/moon.jpg"),
                                     new Renderer(planetMesh));

            planet.transform.GlobalPosition = new float3(0.5f, 0, 0);
            planet.transform.GlobalScale = new float3(0.05f, 0.05f, 0.05f);

            // Setup Mercury
            planet = new SceneEntity("Mercury", new PlanetAction(_earthSpeed*6.2234f), emptyMercury,
                                     new PlanetMaterial(MoreShaders.GetDiffuseTextureShader(RC), "Assets/merkur.jpg"),
                                     new Renderer(planetMesh));

            planet.transform.GlobalPosition = new float3(2.35f, 0, 0);
            planet.transform.GlobalScale = new float3(0.05f, 0.05f, 0.05f);


            // Setup Venus
            planet = new SceneEntity("Venus", new PlanetAction(_earthSpeed*1.5021f), emptyVenus,
                                     new PlanetMaterial(MoreShaders.GetDiffuseTextureShader(RC), "Assets/venus.jpg"),
                                     new Renderer(planetMesh));

            planet.transform.GlobalPosition = new float3(2.6f, 0, 0);
            planet.transform.GlobalScale = new float3(0.08f, 0.08f, 0.08f);


            // Setup Mars
            planet = new SceneEntity("Mars", new PlanetAction(_earthSpeed*374.125f), emptyMars,
                                     new PlanetMaterial(MoreShaders.GetDiffuseTextureShader(RC), "Assets/mars.jpg"),
                                     new Renderer(planetMesh));

            planet.transform.GlobalPosition = new float3(3.25f, 0, 0);
            planet.transform.GlobalScale = new float3(0.07f, 0.07f, 0.07f);

            // Setup Jupiter
            planet = new SceneEntity("Jupiter", new PlanetAction(_earthSpeed*882.62f), emptyJupiter,
                                     new PlanetMaterial(MoreShaders.GetDiffuseTextureShader(RC), "Assets/jupiter.jpg"),
                                     new Renderer(planetMesh));

            planet.transform.GlobalPosition = new float3(4, 0, 0);
            planet.transform.GlobalScale = new float3(0.4f, 0.4f, 0.4f);

            // Setup Saturn
            planet = new SceneEntity("Saturn", new PlanetAction(_earthSpeed*820.61f), emptySaturn,
                                     new PlanetMaterial(MoreShaders.GetDiffuseTextureShader(RC), "Assets/saturn.jpg"),
                                     new Renderer(planetMesh));

            planet.transform.GlobalPosition = new float3(5, 0, 0);
            planet.transform.GlobalScale = new float3(0.3f, 0.3f, 0.3f);


            // Setup Uranus
            planet = new SceneEntity("Uranus", new PlanetAction(_earthSpeed*509.30f), emptyUranus,
                                     new PlanetMaterial(MoreShaders.GetDiffuseTextureShader(RC), "Assets/uranus.jpg"),
                                     new Renderer(planetMesh));

            planet.transform.GlobalPosition = new float3(6, 0, 0);
            planet.transform.GlobalScale = new float3(0.12f, 0.12f, 0.12f);


            // Setup Neptun
            planet = new SceneEntity("Neptun", new PlanetAction(_earthSpeed*544.10f), emptyNeptun,
                                     new PlanetMaterial(MoreShaders.GetDiffuseTextureShader(RC), "Assets/neptune.jpg"),
                                     new Renderer(planetMesh));

            planet.transform.GlobalPosition = new float3(7, 0, 0);
            planet.transform.GlobalScale = new float3(0.14f, 0.14f, 0.14f);

            SceneManager.Manager.StartActionCode();

            // Random Rotations
            emptyEarth.transform.LocalEulerAngles = new float3(0, 45, 0);
            emptyMercury.transform.LocalEulerAngles = new float3(0, 55, 0);
            emptyVenus.transform.LocalEulerAngles = new float3(0, 335, 0);
            emptyMars.transform.LocalEulerAngles = new float3(0, 125, 0);
            emptyJupiter.transform.LocalEulerAngles = new float3(0, 65, 0);
            emptySaturn.transform.LocalEulerAngles = new float3(0, 95, 0);
            emptyUranus.transform.LocalEulerAngles = new float3(0, 145, 0);
            emptyNeptun.transform.LocalEulerAngles = new float3(0, 245, 0);

            RC.ClearColor = new float4(1, 0, 0, 1);
        }

        public override void RenderAFrame()
        {
            SceneManager.Manager.Traverse(this);
        }

        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);
            _sceneCamera.Resize(Width, Height);
        }

        public static void Main()
        {
            var app = new SolarSystem();
            app.Run();
        }
    }

    // ReSharper restore UseObjectOrCollectionInitializer
}