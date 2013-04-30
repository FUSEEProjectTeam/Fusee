using System.IO;
using Examples.LightTypeTest;
using Examples.Solar;
using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.FuseeExampleApp1
{
    public class FuseeExampleApp1 : RenderCanvas
    {

        Camera scenecamera;

        public override void Init()
        {
            SceneManager.RC = RC;
            SceneEntity _object;
            DirectionalLight direct = new DirectionalLight(new float3(1, 1, 1), new float4(0.7f, 0.7f, 0.7f, 1), new float4(0.3f, 0.3f, 0.3f, 1), new float4(0.1f, 0.1f, 0.1f, 1),  new float3(0, 0, 0), 0);

            Geometry sphere = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Sphere.obj.model"));
            Geometry spacebox = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/spacebox.obj.model"));
            Geometry cube = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Cube.obj.model"));
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

            SceneEntity _spaceBox = new SceneEntity("Spacebox", new SimpleMaterial(MoreShaders.GetShader("simple", RC), "Assets/sky.jpg"), new Renderer(spacebox));
            SceneManager.Manager.AddSceneEntity(_spaceBox);

            //Sphere
            _object = new SceneEntity("Sphere1", new ActionCode(), _emptySphere, new DiffuseMaterial(MoreShaders.GetShader("diffuse", RC), "Assets/wall.jpg"), new Renderer(sphere));
            _object.transform.GlobalPosition = new float3(2.9f, 0, 0);
            _object.transform.GlobalScale = new float3(1, 1, 1);

            // LightObject
            _object = new SceneEntity("DirLight", new RotateAction(new float3(0,20,0)), _emptyLight, new DiffuseMaterial(MoreShaders.GetShader("diffuse", RC), "Assets/metall.jpg"), new Renderer(lamp));
            _object.transform.GlobalPosition = new float3(0, 0, 0);
            _object.transform.GlobalScale = new float3(0.3f, 0.3f, 0.3f);
            _object.AddComponent(direct);

            //Cube
            _object = new SceneEntity("Cube1", new ActionCode(), _emptyCube, new DiffuseMaterial(MoreShaders.GetShader("diffuse", RC), "Assets/wall.jpg"), new Renderer(cube));
            _object.transform.GlobalPosition = new float3(-2.9f, 0, 0);
            _object.transform.GlobalScale = new float3(0.01f, 0.01f, 0.01f);

            //Root
            _object = new SceneEntity("Root1", new ActionCode(), _emptyRoot);
            _object.transform.GlobalPosition = new float3(0, 0, 0);
            _object.transform.GlobalScale = new float3(0.1f, 0.1f, 0.1f);


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
            var app = new FuseeExampleApp1();
            app.Run();
        }

    }
}
