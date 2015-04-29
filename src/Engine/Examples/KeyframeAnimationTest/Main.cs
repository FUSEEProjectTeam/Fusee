using Fusee.Engine;
using Fusee.KeyFrameAnimation;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.KeyframeAnimationTest
{
    [FuseeApplication(Name = "KeyframeAnimation",
        Description = "A sample application to show FUSEE's KeyFrameAnimation system.")]
    public class KeyframeAnimationTest : RenderCanvas
    {
        private SceneManager _sceneManager;
        private SceneEntity _sphere;

        private Camera _camera;

        private Channel<float3> _channel2;
        private Channel<float4> _channel1;

        private readonly Animation _myAnim = new Animation();

        public override void Init()
        {
            _sceneManager = new SceneManager();
            _sceneManager.AttachToContext(RC);

            var dir = new DirectionalLight(new float3(0, 10, -1), new float4(1, 1, 1, 1), new float4(1, 1, 1, 1),
                new float4(1, 1, 1, 1), new float3(0, 0, 0), 0);

            var stativ = new SceneEntity("stativ", new ActionCode())
            {
                Transform =
                {
                    GlobalPosition = new float3(0, 0, 100)
                }
            };

            stativ.AddComponent(dir);

            _sceneManager.AddSceneEntity(stativ);

            _camera = new Camera(stativ);
            _camera.Resize(Width, Height);

            Geometry sohGeo = MeshReader.LoadGeometry("Assets/Sphere.obj.model");
            _sphere = new SceneEntity("sphere", new SphereMaterial(MoreShaders.GetDiffuseColorShader(RC)),
                new Renderer(sohGeo));

            _sceneManager.AddSceneEntity(_sphere);

            _channel2 = new Channel<float3>(Lerp.Float3Lerp);
            _channel1 = new Channel<float4>(Lerp.Float4Lerp, new float4(0.5f, 0.5f, 0.5f, 0.5f));

            var key0 = new Keyframe<float4>(0, new float4(1, 0, 1, 1));
            var key1 = new Keyframe<float4>(2, new float4(0.125f, 1, 0.125f, 1));
            var key2 = new Keyframe<float4>(4, new float4(0.250f, 0.75f, 0.250f, 1));
            var key3 = new Keyframe<float4>(6, new float4(0.5f, 0.5f, 0.5f, 1));
            var key4 = new Keyframe<float4>(8, new float4(0.75f, 0.25f, 0.75f, 1));
            var key5 = new Keyframe<float4>(10, new float4(1, 25, 0.125f, 1));
            var key6 = new Keyframe<float4>(0, new float4(0, 1, 0, 1));

            _channel1.AddKeyframe(key0);
            _channel1.AddKeyframe(key1);
            _channel1.AddKeyframe(key2);
            _channel1.AddKeyframe(key3);
            _channel1.AddKeyframe(key4);
            _channel1.AddKeyframe(key5);
            _channel1.AddKeyframe(key6);

            var key40 = new Keyframe<float3>(8, new float3(8, 0, 80));
            var key00 = new Keyframe<float3>(0, new float3(0, 0, 0));
            var key10 = new Keyframe<float3>(2, new float3(1, 2, 20));
            var key20 = new Keyframe<float3>(4, new float3(2, 4, 40));
            var key30 = new Keyframe<float3>(6, new float3(4, 4, 60));
            var key50 = new Keyframe<float3>(12, new float3(0, 4, 60));
            var key60 = new Keyframe<float3>(0, new float3(8, 8, 8));

            _channel2.AddKeyframe(key00);
            _channel2.AddKeyframe(key10);
            _channel2.AddKeyframe(key20);
            _channel2.AddKeyframe(key30);
            _channel2.AddKeyframe(key40);
            _channel2.AddKeyframe(key50);
            _channel2.AddKeyframe(key60);

            _myAnim.AddAnimation(_channel1, RC, "ClearColor");
            _myAnim.AddAnimation(_channel2, _sphere, "Transform.GlobalPosition");
        }

        public override void RenderAFrame()
        {
            _sceneManager.Traverse(this);

            _myAnim.Animate();
        }

        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);
            _camera.Resize(Width, Height);
        }

        public static void Main()
        {
            var app = new KeyframeAnimationTest();
            app.Run();
        }
    }
}