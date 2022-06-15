using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Primitives;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Gui;
using Fusee.Math.Core;
using System;
using System.Threading.Tasks;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Examples.Simple.Core
{
    [FuseeApplication(Name = "FUSEE Simple Example", Description = "A very simple example.")]
    public class Simple : RenderCanvas
    {
        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _rocketScene;
        private SceneRendererForward _sceneRenderer;

        private const float ZNear = 1f;
        private const float ZFar = 1000;
        private readonly float _fovy = M.PiOver4;

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;

        private Transform _camPivotTransform;

        private bool _keys;

        private int instanceAmount = 10000;
        private Random _random = new();

        private InstanceData _instanceComp;
        private InstanceData _instanceComp1;
        private SceneNode _cubeNode;
        private SceneNode _cubeNode1;
        private Plane _plane;
        private Cube _cube = new();

        private float3[] GetRndInstanceTranslations(int amount)
        {
            var instanceTranslations = new float3[amount];
            for (int i = 0; i < amount; i++)
            {
                var x = _random.Next(-2, 2);
                var y = _random.Next(-2, 2);
                var z = _random.Next(-2, 2);

                instanceTranslations[i] = new float3(x, y, z);
            }
            return instanceTranslations;
        }

        public float NextFloat(float min, float max)
        {
            double val = (_random.NextDouble() * (max - min) + min);
            return (float)val;
        }

        private float3[] GetRndInstanceScales(int amount)
        {
            var instanceTranslations = new float3[amount];
            for (int i = 0; i < amount; i++)
            {
                var x = NextFloat(0.1f, 0.1f);
                var y = NextFloat(0.1f, 0.1f);
                var z = NextFloat(0.1f, 0.1f);

                instanceTranslations[i] = new float3(x, y, z);
            }
            return instanceTranslations;
        }

        public float4[] RandomColors(int amount)
        {
            var instanceColors = new float4[amount];
            for (int i = 0; i < amount; i++)
            {
                var res = float4.Zero;
                // Very bad way to generate a random color
                res.r = NextFloat(0, 1);
                res.g = NextFloat(0, 1);
                res.b = NextFloat(0, 1);
                res.a = NextFloat(0.2f, 1f);

                instanceColors[i] = res;
            }

            return instanceColors;
        }

        // Init is called on startup.
        private async Task Load()
        {
            Console.WriteLine("Loading scene ...");

            _gui = await FuseeGuiHelper.CreateDefaultGuiAsync(this, CanvasRenderMode.Screen, "FUSEE Simple Example");

            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);

            // Load the rocket model
            _rocketScene = await AssetStorage.GetAsync<SceneContainer>("RocketFus.fus");
            _camPivotTransform = new Transform();
            var camNode = new SceneNode()
            {
                Name = "CamPivoteNode",
                Children = new ChildList()
                {
                    new SceneNode()
                    {
                        Name = "MainCam",
                        Components = new System.Collections.Generic.List<SceneComponent>()
                        {
                            new Transform() { Translation = new float3(0, 2, -10) },
                            new Camera(ProjectionMethod.Perspective, ZNear, ZFar, _fovy) { BackgroundColor = float4.One }
                        }
                    }
                },
                Components = new System.Collections.Generic.List<SceneComponent>()
                {
                    _camPivotTransform
                }
            };

            _instanceComp = new InstanceData(5, GetRndInstanceTranslations(5), null, GetRndInstanceScales(5), RandomColors(5));
            _instanceComp1 = new InstanceData(5, GetRndInstanceTranslations(5), null, GetRndInstanceScales(5), RandomColors(5));

            _plane = new Plane();

            _cubeNode = new SceneNode()
            {
                Name = "CubeNode",
                Components = new System.Collections.Generic.List<SceneComponent>()
                {
                    new Transform() { Translation = new float3(-5, -0, 0), Scale = float3.One},
                    _instanceComp,
                    MakeEffect.FromDiffuseInstanced(new float4(1,0,0,1)),
                    _plane
                }
            };

            _cubeNode1 = new SceneNode()
            {
                Name = "CubeNode1",
                Components = new System.Collections.Generic.List<SceneComponent>()
                {
                    new Transform() { Translation = new float3(5, -0, 0), Scale = float3.One},
                    _instanceComp1,
                    MakeEffect.FromDiffuseInstanced(new float4(0,1,0,1)),
                    _plane
                }
            };

            _rocketScene.Children.RemoveAt(0);
            _rocketScene.Children.Add(_cubeNode);
            _rocketScene.Children.Add(_cubeNode1);
            _rocketScene.Children.Add(camNode);

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_rocketScene);
            _guiRenderer = new SceneRendererForward(_gui);
        }

        public override async Task InitAsync()
        {
            await Load();
            await base.InitAsync();
        }

        // Init is called on startup.
        public override void Init()
        {

        }

        public override void Update()
        {
            // Mouse and keyboard movement
            if (Keyboard.LeftRightAxis != 0 || Keyboard.UpDownAxis != 0)
            {
                _keys = true;
            }

            if (Mouse.LeftButton)
            {
                _keys = false;
                _angleVelHorz = RotationSpeed * Mouse.XVel * DeltaTimeUpdate * 0.0005f;
                _angleVelVert = RotationSpeed * Mouse.YVel * DeltaTimeUpdate * 0.0005f;
            }
            else if (Touch != null && Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                _keys = false;
                var touchVel = Touch.GetVelocity(TouchPoints.Touchpoint_0);
                _angleVelHorz = RotationSpeed * touchVel.x * DeltaTimeUpdate * 0.0005f;
                _angleVelVert = RotationSpeed * touchVel.y * DeltaTimeUpdate * 0.0005f;
            }
            else
            {
                if (_keys)
                {
                    _angleVelHorz = RotationSpeed * Keyboard.LeftRightAxis * DeltaTimeUpdate;
                    _angleVelVert = RotationSpeed * Keyboard.UpDownAxis * DeltaTimeUpdate;
                }
                else
                {
                    var curDamp = (float)System.Math.Exp(-Damping * DeltaTimeUpdate);
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;
        }

        private int frameCount = 0;

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            frameCount++;

            if(frameCount % 10 == 0)
            {
                //_instanceComp.Colors = RandomColors(5);
                _instanceComp.Positions = GetRndInstanceTranslations(5);
                _instanceComp.Colors = RandomColors(5);
            }

            _camPivotTransform.RotationQuaternion = QuaternionF.FromEuler(_angleVert, _angleHorz, 0);
            _sceneRenderer.Render(RC);

            //Constantly check for interactive objects.
            //_guiRenderer.Render(RC);
            //if (!Mouse.Desc.Contains("Android"))
            //    _sih.CheckForInteractiveObjects(RC, Mouse.Position, Width, Height);
            //if (Touch != null && Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            //{
            //    _sih.CheckForInteractiveObjects(RC, Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            //}

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }
    }
}