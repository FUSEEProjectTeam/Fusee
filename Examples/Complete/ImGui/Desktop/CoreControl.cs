using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.ImGuiImp.Desktop.Templates;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Examples.FuseeImGui.Desktop
{
    internal struct SceneElement
    {
        public string Name;
        public Type Type;
        public string Value;
    }

    internal class CoreControl : FuseeControlToTexture, IDisposable
    {
        private SceneContainer _rocketScene;
        private SceneRendererForward _renderer;
        private WritableMultisampleTexture _renderTexture;

        private Transform _camPivotTransform;

        public int Width;
        public int Height;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;

        private const float ZNear = 1f;
        private const float ZFar = 1000;
        private readonly float _fovy = M.PiOver4;

        private Camera _cam;
        private bool disposedValue;


        public CoreControl(RenderContext ctx) : base(ctx)
        {
            _rc = ctx;
        }

        public void ColorRocket(float4 color)
        {
            _rocketScene.Children[0].Children[2].GetComponent<SurfaceEffect>().SurfaceInput.Albedo = color;
        }

        public List<SceneElement> GetSceneGraphRePresentation()
        {
            var res = new List<SceneElement>();
            res.AddRange(GetSceneGraphRePresentationForNode(_rocketScene.Children));

            return res;
        }

        private List<SceneElement> GetSceneGraphRePresentationForNode(List<SceneNode> nodes)
        {
            var res = new List<SceneElement>();

            foreach (var c in nodes)
            {
                if (c.Children.Count > 0)
                    res.AddRange(GetSceneGraphRePresentationForNode(c.Children.ToList()));

                foreach (var item in c.Components)
                {
                    var node = new SceneElement
                    {
                        Type = item.GetType()
                    };

                    if (item is Transform t)
                    {
                        node.Name = nameof(Transform);
                        node.Value = $"{t.Matrix}";
                    }

                    else if (item is SurfaceEffect fx)
                    {
                        node.Name = nameof(SurfaceEffect);
                        node.Value = $"Albedo color: {fx.SurfaceInput.Albedo}";
                    }

                    else if (item is Camera cam)
                    {
                        node.Name = nameof(Camera);
                        node.Value = $"FOV: {cam.Fov}, Scale: {cam.Scale}";
                    }

                    else if (item is Mesh m)
                    {
                        node.Name = nameof(Mesh);
                        node.Value = $"Vert count: {m.Vertices.Length}, Triangle count: {m.Triangles.Length}";
                    }

                    else
                    {
                        node.Name = "Unknown";
                        node.Value = "Undefined";
                    }

                    res.Add(node);
                }
            }

            return res;
        }

        public override void Init()
        {
            _rocketScene = AssetStorage.Get<SceneContainer>("RocketFus.fus");
            _camPivotTransform = new Transform();
            _cam = new Camera(ProjectionMethod.Perspective, ZNear, ZFar, _fovy) { BackgroundColor = new float4(0, 0, 0, 0) };
            var camNode = new SceneNode()
            {
                Name = "CamPivoteNode",
                Children = new ChildList()
                {
                    new SceneNode()
                    {
                        Name = "MainCam",
                        Components = new List<SceneComponent>()
                        {
                            new Transform() { Translation = new float3(0, 2, -10) },
                            _cam
                        }
                    }
                },
                Components = new List<SceneComponent>()
                {
                    _camPivotTransform
                }
            };

            _rocketScene.Children.Add(camNode);

            _renderer = new SceneRendererForward(_rocketScene);
        }

        public override void Update(bool allowInput)
        {
            if (!allowInput)
            {
                _angleVelHorz = 0;
                _angleVelVert = 0;
                return;
            }

            if (Input.Mouse.LeftButton)
            {
                _angleVelHorz = RotationSpeed * Input.Mouse.XVel * Time.DeltaTimeUpdate * 0.0005f;
                _angleVelVert = RotationSpeed * Input.Mouse.YVel * Time.DeltaTimeUpdate * 0.0005f;
            }

            else
            {
                var curDamp = (float)System.Math.Exp(-Damping * Time.DeltaTimeUpdate);
                _angleVelHorz *= curDamp;
                _angleVelVert *= curDamp;
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;
        }

        protected override ITextureHandle RenderAFrame()
        {
            if (_renderTexture != null)
            {
                _camPivotTransform.RotationQuaternion = QuaternionF.FromEuler(_angleVert, _angleHorz, 0);
                _renderer.Render(_rc);
            }
            return _renderTexture?.TextureHandle;
        }

        protected override void Resize(int width, int height)
        {
            if (width <= 0 || height <= 0)
                return;

            Width = width;
            Height = height;

            _renderTexture?.Dispose();
            _renderTexture = WritableMultisampleTexture.CreateAlbedoTex(Width, Height, 8);
            _cam.RenderTexture = _renderTexture;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _renderTexture?.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}