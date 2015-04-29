using System;
using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.LightTypeTest
{
    [FuseeApplication(Name = "LightTypeTest", Description = "Tests a spotlight with different shaders.")]
    public class LightTypeTest : RenderCanvas
    {
        private SceneManager _sceneManager;
        private Camera _sceneCamera;

        public override void Init()
        {
            _sceneManager = new SceneManager();
            _sceneManager.AttachToContext(RC);

            var spot = new SpotLight(new float3(0, 0, 0), new float3(0, 0, 0), new float4(0.7f, 0.7f, 0.7f, 1),
                new float4(0.3f, 0.3f, 0.3f, 1), new float4(0.1f, 0.1f, 0.1f, 1), (float) Math.PI/8, 0);

            var sphereGeom = MeshReader.LoadGeometry("Assets/Sphere.obj.model");
            var spaceboxGeom = MeshReader.LoadGeometry("Assets/Spacebox.obj.model");
            var lampGeom = MeshReader.LoadGeometry("Assets/Lamp.obj.model");

            // main objects
            var worldOrigin = new SceneEntity("WorldOrigin", new MouseAction());

            var cameraholder = new SceneEntity("CameraOwner", new CamScript())
            {
                Transform =
                {
                    GlobalPosition = new float3(0, 0, 10)
                }
            };

            worldOrigin.AddChild(cameraholder);

            _sceneCamera = new Camera(cameraholder);
            _sceneCamera.Resize(Width, Height);

            var root = new SceneEntity("Root1", new MouseAction())
            {
                Transform =
                {
                    GlobalPosition = new float3(0, 0, 0),
                    GlobalScale = new float3(1, 1, 1)
                }
            };

            var sphere = new SceneEntity("Sphere", new ActionCode(),
                new SpecularMaterial(RC, MoreShaders.GetSpecularShader(RC), "Assets/metall.jpg"),
                new Renderer(sphereGeom))
            {
                Transform =
                {
                    GlobalPosition = new float3(2, 0, 0),
                    GlobalScale = new float3(0.5f, 0.5f, 0.5f)
                }
            };

            var cube = new SceneEntity("Sphere2", new ActionCode(),
                new BumpMaterial(RC, MoreShaders.GetBumpDiffuseShader(RC), "Assets/metall.jpg", "Assets/normal.jpg"),
                new Renderer(sphereGeom))
            {
                Transform =
                {
                    GlobalPosition = new float3(5, 0, 0)
                }
            };

            var light = new SceneEntity("Light", new ActionCode());

            var spaceBox = new SceneEntity("Spacebox",
                new DiffuseMaterial(RC, MoreShaders.GetDiffuseTextureShader(RC), "Assets/sky.jpg"),
                new Renderer(spaceboxGeom));

            spaceBox.Transform.GlobalScale = new float3(0.5f, 0.5f, 0.5f);
            spaceBox.Transform.LocalEulerAngles = new float3(-90, 90, 0);

            _sceneManager.AddSceneEntity(worldOrigin);
            _sceneManager.AddSceneEntity(root);
            _sceneManager.AddSceneEntity(sphere);
            _sceneManager.AddSceneEntity(cube);
            _sceneManager.AddSceneEntity(light);
            _sceneManager.AddSceneEntity(spaceBox);

            // LightObject
            var lamp = new SceneEntity("DirLight", new RotateAction(new float3(0, 20, 0)), 
                new DiffuseMaterial(RC, MoreShaders.GetDiffuseTextureShader(RC), "Assets/metall.jpg"),
                new Renderer(lampGeom))
            {
                Transform =
                {
                    GlobalPosition = new float3(0, 0, 0),
                    GlobalScale = new float3(0.7f, 0.7f, 0.7f)
                }
            };

            light.AddChild(lamp);
            lamp.AddComponent(spot);

            _sceneManager.StartActionCode();

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
            var app = new LightTypeTest();
            app.Run();
        }
    }
}