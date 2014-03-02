using System.IO;
using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.LightTypeTest
{
    [FuseeApplication(Name = "LightTypeTest", Description = "Tests a spotlight with different shaders.")]
    public class LightTypeTest : RenderCanvas
    {

        Camera scenecamera;

        public override void Init()
        {
            SceneManager.RC = RC;
            SceneEntity _object;

            DirectionalLight direct = new DirectionalLight(new float3(1, 1, 1), new float4(0.7f, 0.7f, 0.7f, 1), new float4(0.3f, 0.3f, 0.3f, 1), new float4(0.1f, 0.1f, 0.1f, 1), new float3(0, 0, 0), 2);
            PointLight point = new PointLight(new float3(0, 0, 0), new float4(0.7f, 0.7f, 0.7f, 1), new float4(0.3f, 0.3f, 0.3f, 1), new float4(0.1f, 0.1f, 0.1f, 1), 1);
            SpotLight spot = new SpotLight(new float3(1, 1, 1), new float3(1, 1, 1), new float4(0.7f, 0.7f, 0.7f, 1), new float4(0.3f, 0.3f, 0.3f, 1), new float4(0.1f, 0.1f, 0.1f, 1), 1.0f , 0);

            Geometry sphere = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Sphere.obj.model"));
            Geometry sphere2 = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Sphere.obj.model"));
            Geometry spacebox = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/spacebox.obj.model"));
            Geometry cube = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Sphere.obj.model"));
            Geometry lamp = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/lamp2.obj.model"));


            SceneEntity _emptySphere;
            SceneEntity _emptyCube;
            SceneEntity _emptyLight;
            SceneEntity _emptyRoot;

            _emptyRoot = new SceneEntity("emptyRoot", new MouseAction());
            _emptySphere = new SceneEntity("emptySphere", new ActionCode());
            _emptyCube = new SceneEntity("emptyCube", new ActionCode());
            _emptyLight = new SceneEntity("emptyLight", new ActionCode());

            SceneManager.Manager.AddSceneEntity(_emptyRoot);
            SceneManager.Manager.AddSceneEntity(_emptySphere);
            SceneManager.Manager.AddSceneEntity(_emptyCube);
            SceneManager.Manager.AddSceneEntity(_emptyLight);

            SceneEntity cameraholder;
            SceneEntity WorldOrigin;
            WorldOrigin = new SceneEntity("WorldOrigin", new MouseAction());
            SceneManager.Manager.AddSceneEntity(WorldOrigin);
            cameraholder = new SceneEntity("CameraOwner", new CamScript(), WorldOrigin);
            cameraholder.transform.GlobalPosition = new float3(0, 0, 10);
            scenecamera = new Camera(cameraholder);
            scenecamera.Resize(Width, Height);

            SceneEntity _spaceBox = new SceneEntity("Spacebox", new DiffuseMaterial(MoreShaders.GetDiffuseTextureShader(RC), "Assets/sky.jpg"), new Renderer(spacebox));
            SceneManager.Manager.AddSceneEntity(_spaceBox);

            //Sphere
            _object = new SceneEntity("Sphere1", new ActionCode(), _emptySphere, new SpecularMaterial(MoreShaders.GetSpecularShader(RC), "Assets/metall2.jpg"), new Renderer(sphere));
            _object.transform.GlobalPosition = new float3(2, 0, 0);
            _object.transform.GlobalScale = new float3(0.5f, 0.5f, 0.5f);

            // LightObject
            _object = new SceneEntity("DirLight", new RotateAction(new float3(0, 20, 0)), _emptyLight, new DiffuseMaterial(MoreShaders.GetDiffuseTextureShader(RC), "Assets/metall2.jpg"), new Renderer(lamp));
            _object.transform.GlobalPosition = new float3(0, 0, 0);
            _object.transform.GlobalScale = new float3(0.7f, 0.7f, 0.7f);
            _object.AddComponent(spot);

            SceneEntity _object2 = new SceneEntity("FlyingLight", new RotatingLightAction(new float3(0, 0, 0)), _object, new DiffuseMaterial(MoreShaders.GetDiffuseTextureShader(RC), "Assets/metall2.jpg"), new Renderer(sphere2));
            _object2.transform.GlobalPosition = new float3(0, 3, 0);
            //_object2.transform.GlobalScale = new float3(1, 1, 1);
            //_object2.AddComponent(point);

            //Cube
            _object = new SceneEntity("Cube1", new ActionCode(), _emptyCube, new BumpMaterial(MoreShaders.GetBumpDiffuseShader(RC), "Assets/metall2.jpg", "Assets/normal2.jpg"), new Renderer(cube));
            _object.transform.GlobalPosition = new float3(5, 0, 0);
            //_object.transform.GlobalScale = new float3(0.01f, 0.01f, 0.01f);

            //Root
            _object = new SceneEntity("Root1", new ActionCode(), _emptyRoot);
            _object.transform.GlobalPosition = new float3(0, 0, 0);
            _object.transform.GlobalScale = new float3(1, 1, 1);


            SceneManager.Manager.StartActionCode();

            RC.ClearColor = new float4(1, 0, 0, 1);

        }

        public override void RenderAFrame()
        {
            SceneManager.Manager.Traverse(this);
        }

        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);
            scenecamera.Resize(Width, Height);
        }

        public static void Main()
        {
            var app = new LightTypeTest();
            app.Run();
        }

    }
}