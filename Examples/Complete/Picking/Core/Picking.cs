﻿using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Gui;
using Fusee.Math.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fusee.Examples.Picking.Core
{
    [FuseeApplication(Name = "FUSEE Picking Example", Description = "How to use the Scene Picker.")]
    public class Picking : RenderCanvas
    {
        // angle variables
        private static float _angleHorz = M.PiOver4, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private SceneContainer _scene;
        private Transform _camPivotTransform;
        private SceneRendererForward _sceneRenderer;
        private ScenePicker _scenePicker;

        private bool _keys;

        private const float ZNear = 1f;
        private const float ZFar = 1000;
        private readonly float _fovy = M.PiOver4;

        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;

        private PickResult _currentPick;
        private float4 _oldColor;
        private bool _pick;
        private float2 _pickPos;

        private async Task Load()
        {
            // Create the robot model
            _scene = CreateScene();

            // Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_scene);
            _scenePicker = new ScenePicker(_scene, RC.CurrentRenderState.CullMode);

            _gui = await FuseeGuiHelper.CreateDefaultGuiAsync(this, CanvasRenderMode.Screen, "FUSEE Picking Example");
            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);
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
            if (Input.Keyboard.LeftRightAxis != 0 || Input.Keyboard.UpDownAxis != 0)
            {
                _keys = true;
            }

            if (Input.Mouse.LeftButton)
            {
                _pick = true;
                _pickPos = Input.Mouse.Position;
                _keys = false;
                _angleVelHorz = RotationSpeed * Input.Mouse.XVel * Time.DeltaTimeUpdate * 0.0005f;
                _angleVelVert = RotationSpeed * Input.Mouse.YVel * Time.DeltaTimeUpdate * 0.0005f;
            }
            else if (Input.Touch != null && Input.Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                _pick = true;
                _pickPos = Input.Touch.GetPosition(TouchPoints.Touchpoint_0);
                var touchVel = Input.Touch.GetVelocity(TouchPoints.Touchpoint_0);
                _angleVelHorz = RotationSpeed * touchVel.x * Time.DeltaTimeUpdate * 0.0005f;
                _angleVelVert = RotationSpeed * touchVel.y * Time.DeltaTimeUpdate * 0.0005f;
            }
            else
            {
                _pick = false;
                if (_keys)
                {
                    _angleVelHorz = RotationSpeed * Input.Keyboard.LeftRightAxis * Time.DeltaTimeUpdate;
                    _angleVelVert = RotationSpeed * Input.Keyboard.UpDownAxis * Time.DeltaTimeUpdate;
                }
                else
                {
                    var curDamp = (float)System.Math.Exp(-Damping * Time.DeltaTimeUpdate);
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            _camPivotTransform.RotationQuaternion = QuaternionF.FromEuler(new float3(_angleVert, _angleHorz, 0));
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            //_sceneRenderer.Render(RC);

            //Picking
            //if (_pick)
            //{
            //    float2 pickPosClip = (_pickPos * new float2(2.0f / Width, -2.0f / Height)) + new float2(-1, 1);

            //    var newPick = (MeshPickResult)_scenePicker.Pick(pickPosClip, Width, Height).ToList().OrderBy(pr => pr.ClipPos.z)
            //        .FirstOrDefault();
            //    if (newPick != null)
            //        Diagnostics.Debug(newPick.Node.Name);

            //    if (newPick?.Node != _currentPick?.Node)
            //    {
            //        if (_currentPick != null)
            //        {
            //            var ef = _currentPick.Node.GetComponent<SurfaceEffect>();
            //            ef.SurfaceInput.Albedo = _oldColor;
            //        }

            //        if (newPick != null)
            //        {
            //            var ef = newPick.Node.GetComponent<SurfaceEffect>();
            //            _oldColor = ef.SurfaceInput.Albedo;
            //            ef.SurfaceInput.Albedo = (float4)ColorUint.LawnGreen;
            //        }

            //        _currentPick = newPick;
            //    }

            //    _pick = false;
            //}

            _guiRenderer.Render(RC);

            // Constantly check for interactive objects.
            if (!Input.Mouse.Desc.Contains("Android"))
                _sih.CheckForInteractiveObjects(Input.Mouse.Position, Width, Height);

            if (Input.Touch != null && Input.Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Input.Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(Input.Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }

            // Swap buffers: Show the contents of the back buffer (containing the currently rendered frame) on the front buffer.
            Present();
        }

        private SceneContainer CreateScene()
        {
            _camPivotTransform = new Transform();
            return new SceneContainer
            {
                Header =
                    new SceneHeader
                    {
                        CreationDate = "April 2017",
                        CreatedBy = "mch@hs-furtwangen.de",
                        Generator = "Handcoded with pride",
                    },
                Children = new List<SceneNode>
                {
                    new SceneNode()
                    {
                        Components = { _camPivotTransform, },
                        Children =
                        {
                            new SceneNode()
                            {
                                Name = "MainCam",
                                Components = new List<SceneComponent>
                                {
                                    new Transform() { Translation = new float3(0, 150, -600) },
                                    new Camera(ProjectionMethod.Perspective, ZNear, ZFar, _fovy)
                                    {
                                        BackgroundColor = float4.One
                                    }
                                }
                            }
                        }
                    },
                    new SceneNode
                    {
                        Name = "Base",
                        Components =
                            new List<SceneComponent>
                            {
                                new Transform { Scale = float3.One },
                                MakeEffect.FromDiffuseSpecular((float4)ColorUint.Red),
                                CreateCuboid(new float3(100, 20, 100))
                            },
                        Children = new ChildList
                        {
                            new SceneNode
                            {
                                Name = "Arm01",
                                Components =
                                    new List<SceneComponent>
                                    {
                                        new Transform { Translation = new float3(0, 60, 0), Scale = float3.One },
                                        MakeEffect.FromDiffuseSpecular((float4)ColorUint.Green),
                                        CreateCuboid(new float3(20, 100, 20))
                                    },
                                Children = new ChildList
                                {
                                    new SceneNode
                                    {
                                        Name = "Arm02Rot",
                                        Components =
                                            new List<SceneComponent>
                                            {
                                                new Transform
                                                {
                                                    Translation =
                                                        new float3(-20, 40, 0),
                                                    Rotation = new float3(0.35f, 0,
                                                        0),
                                                    Scale = float3.One
                                                },
                                            },
                                        Children = new ChildList
                                        {
                                            new SceneNode
                                            {
                                                Name = "Arm02",
                                                Components =
                                                    new List<SceneComponent>
                                                    {
                                                        new Transform
                                                        {
                                                            Translation =
                                                                new float3(
                                                                    0, 40,
                                                                    0),
                                                            Scale = float3
                                                                .One
                                                        },
                                                        MakeEffect
                                                            .FromDiffuseSpecular(
                                                                (float4)
                                                                ColorUint
                                                                    .Yellow),
                                                        CreateCuboid(
                                                            new float3(20, 100,
                                                                20))
                                                    },
                                                Children = new ChildList
                                                {
                                                    new SceneNode
                                                    {
                                                        Name = "Arm03Rot",
                                                        Components =
                                                            new List<
                                                                SceneComponent>
                                                            {
                                                                new
                                                                    Transform
                                                                    {
                                                                        Translation =
                                                                            new
                                                                                float3(
                                                                                    20,
                                                                                    40,
                                                                                    0),
                                                                        Rotation =
                                                                            new
                                                                                float3(
                                                                                    0.25f,
                                                                                    0,
                                                                                    0),
                                                                        Scale =
                                                                            float3
                                                                                .One
                                                                    },
                                                            },
                                                        Children =
                                                            new ChildList
                                                            {
                                                                new
                                                                    SceneNode
                                                                    {
                                                                        Name =
                                                                            "Arm03",
                                                                        Components =
                                                                            new
                                                                                List
                                                                                <SceneComponent>
                                                                                {
                                                                                    new
                                                                                        Transform
                                                                                        {
                                                                                            Translation =
                                                                                                new
                                                                                                    float3(
                                                                                                        0,
                                                                                                        40,
                                                                                                        0),
                                                                                            Scale =
                                                                                                float3
                                                                                                    .One
                                                                                        },
                                                                                    MakeEffect
                                                                                        .FromDiffuseSpecular(
                                                                                            (float4)
                                                                                            ColorUint
                                                                                                .Blue),
                                                                                    CreateCuboid(
                                                                                        new
                                                                                            float3(
                                                                                                20,
                                                                                                100,
                                                                                                20))
                                                                                }
                                                                    },
                                                            }
                                                    }
                                                }
                                            },
                                        }
                                    }
                                }
                            },
                        }
                    },
                }
            };
        }

        public static Mesh CreateCuboid(float3 size)
        {
            return new Mesh
            (new uint[]
                {
                    // front face
                    0, 2, 1, 0, 3, 2,

                    // right face
                    4, 6, 5, 4, 7, 6,

                    // back face
                    8, 10, 9, 8, 11, 10,

                    // left face
                    12, 14, 13, 12, 15, 14,

                    // top face
                    16, 18, 17, 16, 19, 18,

                    // bottom face
                    20, 22, 21, 20, 23, 22
                },
                new float3[]
                {
                    new float3 { x = +0.5f * size.x, y = -0.5f * size.y, z = +0.5f * size.z },
                    new float3 { x = +0.5f * size.x, y = +0.5f * size.y, z = +0.5f * size.z },
                    new float3 { x = -0.5f * size.x, y = +0.5f * size.y, z = +0.5f * size.z },
                    new float3 { x = -0.5f * size.x, y = -0.5f * size.y, z = +0.5f * size.z },
                    new float3 { x = +0.5f * size.x, y = -0.5f * size.y, z = -0.5f * size.z },
                    new float3 { x = +0.5f * size.x, y = +0.5f * size.y, z = -0.5f * size.z },
                    new float3 { x = +0.5f * size.x, y = +0.5f * size.y, z = +0.5f * size.z },
                    new float3 { x = +0.5f * size.x, y = -0.5f * size.y, z = +0.5f * size.z },
                    new float3 { x = -0.5f * size.x, y = -0.5f * size.y, z = -0.5f * size.z },
                    new float3 { x = -0.5f * size.x, y = +0.5f * size.y, z = -0.5f * size.z },
                    new float3 { x = +0.5f * size.x, y = +0.5f * size.y, z = -0.5f * size.z },
                    new float3 { x = +0.5f * size.x, y = -0.5f * size.y, z = -0.5f * size.z },
                    new float3 { x = -0.5f * size.x, y = -0.5f * size.y, z = +0.5f * size.z },
                    new float3 { x = -0.5f * size.x, y = +0.5f * size.y, z = +0.5f * size.z },
                    new float3 { x = -0.5f * size.x, y = +0.5f * size.y, z = -0.5f * size.z },
                    new float3 { x = -0.5f * size.x, y = -0.5f * size.y, z = -0.5f * size.z },
                    new float3 { x = +0.5f * size.x, y = +0.5f * size.y, z = +0.5f * size.z },
                    new float3 { x = +0.5f * size.x, y = +0.5f * size.y, z = -0.5f * size.z },
                    new float3 { x = -0.5f * size.x, y = +0.5f * size.y, z = -0.5f * size.z },
                    new float3 { x = -0.5f * size.x, y = +0.5f * size.y, z = +0.5f * size.z },
                    new float3 { x = +0.5f * size.x, y = -0.5f * size.y, z = -0.5f * size.z },
                    new float3 { x = +0.5f * size.x, y = -0.5f * size.y, z = +0.5f * size.z },
                    new float3 { x = -0.5f * size.x, y = -0.5f * size.y, z = +0.5f * size.z },
                    new float3 { x = -0.5f * size.x, y = -0.5f * size.y, z = -0.5f * size.z }
                },
                new float3[]
                {
                    new float3(0, 0, 1), new float3(0, 0, 1), new float3(0, 0, 1), new float3(0, 0, 1),
                    new float3(1, 0, 0), new float3(1, 0, 0), new float3(1, 0, 0), new float3(1, 0, 0),
                    new float3(0, 0, -1), new float3(0, 0, -1), new float3(0, 0, -1), new float3(0, 0, -1),
                    new float3(-1, 0, 0), new float3(-1, 0, 0), new float3(-1, 0, 0), new float3(-1, 0, 0),
                    new float3(0, 1, 0), new float3(0, 1, 0), new float3(0, 1, 0), new float3(0, 1, 0),
                    new float3(0, -1, 0), new float3(0, -1, 0), new float3(0, -1, 0), new float3(0, -1, 0)
                },
                new float2[]
                {
                    new float2(1, 0), new float2(1, 1), new float2(0, 1), new float2(0, 0), new float2(1, 0),
                    new float2(1, 1), new float2(0, 1), new float2(0, 0), new float2(1, 0), new float2(1, 1),
                    new float2(0, 1), new float2(0, 0), new float2(1, 0), new float2(1, 1), new float2(0, 1),
                    new float2(0, 0), new float2(1, 0), new float2(1, 1), new float2(0, 1), new float2(0, 0),
                    new float2(1, 0), new float2(1, 1), new float2(0, 1), new float2(0, 0)
                });
        }
    }
}