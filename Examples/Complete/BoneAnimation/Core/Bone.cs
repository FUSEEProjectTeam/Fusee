using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Gui;
using Fusee.Math.Core;
using Fusee.Xene;
using ProtoBuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Examples.BoneAnimation.Core
{
    [FuseeApplication(Name = "FUSEE Bone Animation Example", Description = "Quick bone animation example")]
    public class Bone : RenderCanvas
    {
        // angle variables
        private static float _angleHorz = M.PiOver3, _angleVert = -M.PiOver6 * 0.5f, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;
        private SceneRendererForward _sceneRenderer;
        List<float4x4> bone = new List<float4x4>();
        Weight weight;

        private readonly Camera _mainCam = new(ProjectionMethod.Perspective, 0.1f, 100, M.PiOver4);
        private ShaderEffect _renderEffect;
        private Transform _mainCamTransform;
        private SceneRendererForward _guiRenderer;
        private SceneContainer _gui;
        private SceneInteractionHandler _sih;
        private SceneContainer _scene;
        private bool _keys;

        //Shader

        private async Task Load()
        {
            Console.WriteLine("Loading scene ...");
            _scene = AssetStorage.Get<SceneContainer>("Walk.fus");
            SceneNode _armature = _scene.Children.FindNodes(node => node.Name == "Armature")?.FirstOrDefault();
            SceneNode _mesh = _scene.Children.FindNodes(node => node.Name == "A51_Alien2")?.FirstOrDefault();

            weight = _mesh.GetComponent<Weight>();
            List<float2> bi = new List<float2>();

            foreach (SceneNode b in weight.Bones)
            {
                bone.Add(b.GetComponent<Transform>().Matrix);
            }
            SceneNode cam = new()
            {
                Name = "MainCam",
                Components = new List<SceneComponent>()
                {
                    _mainCamTransform,
                    _mainCam

                }
            };
            _scene.Children.Add(cam);

            string vs = await AssetStorage.GetAsync<string>("BoneVertex.vert");
            string ps = await AssetStorage.GetAsync<string>("BoneFragment.frag");
            _gui = await FuseeGuiHelper.CreateDefaultGuiAsync(this, CanvasRenderMode.Screen, "FUSEE Simple Example");

            // Create the interaction handler
            _sih = new SceneInteractionHandler(_gui);
            //Load the rocket model
            _renderEffect = new ShaderEffect(
                new IFxParamDeclaration[]
                {
                    new FxParamDeclaration<float4x4>
                    {
                        Name = UniformNameDeclarations.ModelViewProjection, Value = float4x4.Identity
                    },
                    new FxParamDeclaration<float4x4>
                    {
                        Name = UniformNameDeclarations.ModelView, Value = float4x4.Identity
                    },
                    new FxParamDeclaration<float4x4>
                    {
                        Name = UniformNameDeclarations.ITModelView, Value = float4x4.Identity
                    },
                    new FxParamDeclaration<float4x4[]>
                    {
                        Name = "finalBonesMatrices[0]", Value = weight.BindingMatrices.ToArray()
                    },
                    new FxParamDeclaration<float4x4[]>
                    {
                        Name = "boneMatrices[0]", Value = bone.ToArray()
                    },
                },
                new RenderStateSet
                {
                    AlphaBlendEnable = false,
                    ZEnable = true
                },
                vs,
                ps);
            _mesh.RemoveComponent(typeof(SurfaceEffect));
            _mesh.Components.Insert(1, _renderEffect);
            //// Wrap a SceneRenderer around the model.
            _sceneRenderer = new SceneRendererForward(_scene);
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
            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).

            _mainCam.Viewport = new float4(0, 0, 100, 100);
            _mainCam.BackgroundColor = new float4(1f, 1f, 1f, 1);
            _mainCam.Layer = -1;

            _mainCamTransform = new Transform()
            {
                Rotation = float3.Zero,
                Translation = new float3(0, 1, 10),
                Scale = new float3(1, 1, 1)
            };
            _angleHorz = -135;
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
                _angleVelHorz = -RotationSpeed * Mouse.XVel * DeltaTimeUpdate * 0.0005f;
                _angleVelVert = -RotationSpeed * Mouse.YVel * DeltaTimeUpdate * 0.0005f;
            }
            else
            {
                if (_keys)
                {
                    _angleVelHorz = -RotationSpeed * Keyboard.LeftRightAxis * DeltaTimeUpdate;
                    _angleVelVert = -RotationSpeed * Keyboard.UpDownAxis * DeltaTimeUpdate;
                }
                else
                {
                    var curDamp = (float)System.Math.Exp(-Damping * DeltaTimeUpdate);
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }
            if (!Mouse.Desc.Contains("Android"))
                _sih.CheckForInteractiveObjects(RC, Mouse.Position, Width, Height);
            if (Touch != null && Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(RC, Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;
        }

        // RenderAFrame is called once a frame

        public override void RenderAFrame()
        {
            Dictionary<string, float4x4> x = new Dictionary<string, float4x4>();
            for (int i = 0; i < weight.Bones.Count; i++)
            {
                if (weight.Bones[i].Parent.GetComponent<Engine.Core.Scene.Bone>() == null)
                {
                    bone[i] = weight.Bones[i].Parent.GetTransform().Matrix * weight.Bones[i].GetTransform().Matrix;
                    x[weight.Bones[i].Name] = weight.Bones[i].Parent.GetTransform().Matrix * weight.Bones[i].GetTransform().Matrix;
                }
                else
                {
                    float4x4 parent = x[weight.Bones[i].Parent.Name];
                    bone[i] = parent * weight.Bones[i].GetTransform().Matrix;
                    x[weight.Bones[i].Name] = parent * weight.Bones[i].GetTransform().Matrix;
                }
            }
            _renderEffect.SetFxParam("boneMatrices[0]", bone.ToArray());
            // Clear the backbuffer
            //bPos = new float3(1, 1, 0);
            //_renderEffect.SetFxParam<float3>("bPos", bPos);
            // Create the camera matrix and set it as the current ModelView transformation
            _mainCamTransform.FpsView(_angleHorz, _angleVert, Keyboard.WSAxis, Keyboard.ADAxis, DeltaTime * 10);

            // Render the scene loaded in Init()
            _sceneRenderer.Render(RC);

            //Constantly check for interactive objects.
            _guiRenderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }
    }
}