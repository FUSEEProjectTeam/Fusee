using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.SolarSystem
{
    [FuseeApplication(Name = "SolarSystem",
        Description = "A sample application showing how to use FUSEE's built-in scene graph management system.")]
    public class SolarSystem : RenderCanvas
    {
        private SceneManager _sceneManager;

        private static float3 _earthSpeed;
        private Camera _sceneCamera;

        public override void Init()
        {
            _sceneManager = new SceneManager();
            _sceneManager.AttachToContext(RC);

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
            var emptyMercury = new SceneEntity("emptyPlanetHolder", new PlanetAction(RC, _earthSpeed*4.1477f));
            var emptyVenus = new SceneEntity("emptyPlanetHolder", new PlanetAction(RC, _earthSpeed*1.6150f));
            var emptyEarth = new SceneEntity("emptyPlanetHolder", new PlanetAction(RC, _earthSpeed));
            var emptyMars = new SceneEntity("emptyPlanetHolder", new PlanetAction(RC, _earthSpeed*0.5320f));
            var emptyJupiter = new SceneEntity("emptyPlanetHolder", new PlanetAction(RC, _earthSpeed*0.0833f));
            var emptySaturn = new SceneEntity("emptyPlanetHolder", new PlanetAction(RC, _earthSpeed*0.03476f));
            var emptyUranus = new SceneEntity("emptyPlanetHolder", new PlanetAction(RC, _earthSpeed*0.0119f));
            var emptyNeptun = new SceneEntity("emptyPlanetHolder", new PlanetAction(RC, _earthSpeed*0.0062f));

            _sceneManager.AddSceneEntity(emptyMoon);
            _sceneManager.AddSceneEntity(emptyMercury);
            _sceneManager.AddSceneEntity(emptyVenus);
            _sceneManager.AddSceneEntity(emptyEarth);
            _sceneManager.AddSceneEntity(emptyMars);
            _sceneManager.AddSceneEntity(emptyJupiter);
            _sceneManager.AddSceneEntity(emptySaturn);
            _sceneManager.AddSceneEntity(emptyUranus);
            _sceneManager.AddSceneEntity(emptyNeptun);

            // Scene Camera
            var worldOrigin = new SceneEntity("WorldOrigin", new RotationScript());
            _sceneManager.AddSceneEntity(worldOrigin);

            var cameraholder = new SceneEntity("CameraOwner", new CameraScript(RC))
            {
                Transform =
                {
                    GlobalPosition = new float3(0, 0, -10)
                }
            };

            worldOrigin.AddChild(cameraholder);

            _sceneCamera = new Camera(cameraholder);
            _sceneCamera.Resize(Width, Height);

            // Setup Space Box
            var spaceBox = new SceneEntity("Spacebox",
                new PlanetMaterial(RC, MoreShaders.GetTextureShader(RC), "Assets/spaceboxTexture.png"),
                new Renderer(spaceBoxMesh));
            _sceneManager.AddSceneEntity(spaceBox);

            // Setup Sun
            var planet = new SceneEntity("Sun",
                new PlanetMaterial(RC, MoreShaders.GetDiffuseTextureShader(RC), "Assets/sun.jpg"),
                new Renderer(planetMesh))
            {
                Transform = {LocalScale = new float3(2, 2, 2)}
            };

            _sceneManager.AddSceneEntity(planet);

            // Setup Earth
            planet = new SceneEntity("Earth", new PlanetAction(RC, new float3(0, 0.69635f*365, 0)),
                new PlanetMaterial(RC, MoreShaders.GetDiffuseTextureShader(RC), "Assets/earth.jpg"),
                new Renderer(planetMesh))
            {
                Transform =
                {
                    GlobalPosition = new float3(2.9f, 0, 0),
                    GlobalScale = new float3(0.1f, 0.1f, 0.1f)
                }
            };

            planet.AddComponent(direct);

            emptyEarth.AddChild(planet);

            // Setup Moon
            planet = new SceneEntity("Moon", new PlanetAction(RC, new float3(0, 2.7f, 0)),
                new PlanetMaterial(RC, MoreShaders.GetDiffuseTextureShader(RC), "Assets/moon.jpg"),
                new Renderer(planetMesh))
            {
                Transform =
                {
                    GlobalPosition = new float3(0.5f, 0, 0),
                    GlobalScale = new float3(0.05f, 0.05f, 0.05f)
                }
            };

            emptyMoon.AddChild(planet);

            // Setup Mercury
            planet = new SceneEntity("Mercury", new PlanetAction(RC, _earthSpeed*6.2234f),
                new PlanetMaterial(RC, MoreShaders.GetDiffuseTextureShader(RC), "Assets/merkur.jpg"),
                new Renderer(planetMesh))
            {
                Transform =
                {
                    GlobalPosition = new float3(2.35f, 0, 0),
                    GlobalScale = new float3(0.05f, 0.05f, 0.05f)
                }
            };

            emptyMercury.AddChild(planet);

            // Setup Venus
            planet = new SceneEntity("Venus", new PlanetAction(RC, _earthSpeed*1.5021f),
                new PlanetMaterial(RC, MoreShaders.GetDiffuseTextureShader(RC), "Assets/venus.jpg"),
                new Renderer(planetMesh))
            {
                Transform =
                {
                    GlobalPosition = new float3(2.6f, 0, 0),
                    GlobalScale = new float3(0.08f, 0.08f, 0.08f)
                }
            };

            emptyVenus.AddChild(planet);


            // Setup Mars
            planet = new SceneEntity("Mars", new PlanetAction(RC, _earthSpeed*374.125f),
                new PlanetMaterial(RC, MoreShaders.GetDiffuseTextureShader(RC), "Assets/mars.jpg"),
                new Renderer(planetMesh))
            {
                Transform =
                {
                    GlobalPosition = new float3(3.25f, 0, 0),
                    GlobalScale = new float3(0.07f, 0.07f, 0.07f)
                }
            };

            emptyMars.AddChild(planet);

            // Setup Jupiter
            planet = new SceneEntity("Jupiter", new PlanetAction(RC, _earthSpeed*882.62f),
                new PlanetMaterial(RC, MoreShaders.GetDiffuseTextureShader(RC), "Assets/jupiter.jpg"),
                new Renderer(planetMesh))
            {
                Transform =
                {
                    GlobalPosition = new float3(4, 0, 0),
                    GlobalScale = new float3(0.4f, 0.4f, 0.4f)
                }
            };

            emptyJupiter.AddChild(planet);

            // Setup Saturn
            planet = new SceneEntity("Saturn", new PlanetAction(RC, _earthSpeed*820.61f),
                new PlanetMaterial(RC, MoreShaders.GetDiffuseTextureShader(RC), "Assets/saturn.jpg"),
                new Renderer(planetMesh))
            {
                Transform =
                {
                    GlobalPosition = new float3(5, 0, 0),
                    GlobalScale = new float3(0.3f, 0.3f, 0.3f)
                }
            };

            emptySaturn.AddChild(planet);


            // Setup Uranus
            planet = new SceneEntity("Uranus", new PlanetAction(RC, _earthSpeed*509.30f),
                new PlanetMaterial(RC, MoreShaders.GetDiffuseTextureShader(RC), "Assets/uranus.jpg"),
                new Renderer(planetMesh))
            {
                Transform =
                {
                    GlobalPosition = new float3(6, 0, 0),
                    GlobalScale = new float3(0.12f, 0.12f, 0.12f)
                }
            };

            emptyUranus.AddChild(planet);


            // Setup Neptun
            planet = new SceneEntity("Neptun", new PlanetAction(RC, _earthSpeed*544.10f),
                new PlanetMaterial(RC, MoreShaders.GetDiffuseTextureShader(RC), "Assets/neptune.jpg"),
                new Renderer(planetMesh))
            {
                Transform =
                {
                    GlobalPosition = new float3(7, 0, 0),
                    GlobalScale = new float3(0.14f, 0.14f, 0.14f)
                }
            };

            emptyNeptun.AddChild(planet);

            _sceneManager.StartActionCode();

            // Random Rotations
            emptyEarth.Transform.LocalEulerAngles = new float3(0, 45, 0);
            emptyMercury.Transform.LocalEulerAngles = new float3(0, 55, 0);
            emptyVenus.Transform.LocalEulerAngles = new float3(0, 335, 0);
            emptyMars.Transform.LocalEulerAngles = new float3(0, 125, 0);
            emptyJupiter.Transform.LocalEulerAngles = new float3(0, 65, 0);
            emptySaturn.Transform.LocalEulerAngles = new float3(0, 95, 0);
            emptyUranus.Transform.LocalEulerAngles = new float3(0, 145, 0);
            emptyNeptun.Transform.LocalEulerAngles = new float3(0, 245, 0);

            RC.ClearColor = new float4(1, 0, 0, 1);
        }

        public override void RenderAFrame()
        {
            _sceneManager.Traverse(this);
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
}