﻿using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Primitives;
using Fusee.Engine.Core.Scene;
using Fusee.Jometri;
using Fusee.Math.Core;
using System.Collections.Generic;
using static Fusee.Engine.Core.Input;

namespace Fusee.Examples.ThreeDFont.Core
{
    [FuseeApplication(Name = "FUSEE ThreeDFont Example", Description = "Create meshes from Font-Files.")]
    public class ThreeDFont : RenderCanvas
    {
        private SceneRendererForward _renderer;

        private float _alpha;
        private float _beta;

        private string _text;
        private Mesh _textMeshLato;
        private Mesh _textMeshVlad;
        private Mesh _textMeshGnu;

        private ThreeDFontHelper _threeDFontHelper;
        private SceneNode _camNode;
        private readonly Camera _mainCam = new(ProjectionMethod.Perspective, 0.1f, 1000, M.PiOver4)
        {
            BackgroundColor = new float4(27 / 255f, 153 / 255f, 242 / 255f, 1)
        };
        private Transform _camPivot;

        // Init is called on startup.
        public override void Init()
        {
            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            var vladimir = AssetStorage.Get<Font>("VLADIMIR.TTF");
            var gnuSerif = AssetStorage.Get<Font>("GNU-FreeSerif.ttf");

            _text = "FUSEE ThreeDFont Example";

            _threeDFontHelper = new ThreeDFontHelper(_text, fontLato);
            var outlinesLato = _threeDFontHelper.GetTextOutlinesWAngle(20);
            var geomLato = new Jometri.Geometry(outlinesLato);
            geomLato.Extrude2DPolygon(2000, false);
            geomLato.Triangulate();
            _textMeshLato = new JometriMesh(geomLato);

            _threeDFontHelper = new ThreeDFontHelper(_text, vladimir);
            var outlinesVlad = _threeDFontHelper.GetTextOutlinesWAngle(7);
            var geomVlad = new Jometri.Geometry(outlinesVlad);
            geomVlad.Extrude2DPolygon(200, false);
            geomVlad.Triangulate();
            _textMeshVlad = new JometriMesh(geomVlad);

            _threeDFontHelper = new ThreeDFontHelper(_text, gnuSerif);
            var outlinesGnu = _threeDFontHelper.GetTextOutlinesWAngle(40);
            var geomGnu = new Jometri.Geometry(outlinesGnu);
            //geomVlad.Extrude2DPolygon(200, false);
            geomGnu.Triangulate();
            _textMeshGnu = new JometriMesh(geomGnu);

            //---------------------- Fill SceneNode -------------------------//

            var parentNode = new SceneNode
            {
                Components = new List<SceneComponent>()
                {
                    new Transform
                    {
                        Rotation = new float3(0,-M.PiOver6,0),
                        Translation = new float3(-60,1,0),
                        Scale = new float3(0.01f, 0.01f, 0.01f),
                    }
                },
                Children = new ChildList()
            };

            var checkerboardTex = new Texture(AssetStorage.Get<ImageData>("checkerboard.jpg"), true, TextureFilterMode.LinearMipmapLinear);
            var floorNode = new SceneNode()
            {
                Name = $"Plane",
                Components = new List<SceneComponent>
                {
                    new Transform()
                    {
                        Rotation = new float3(M.DegreesToRadians(90), 0, 0),
                        Translation = new float3(100, -20, 0),
                        Scale = new float3(500, 500,0.1f)
                    },
                    MakeEffect.FromDiffuse(float4.One, 0, float3.Zero, checkerboardTex, 1f, new float2(2,2)),
                    new Plane()
                }
            };

            //Vladimir
            var vladMesh = new Mesh();
            vladMesh.Vertices.Assign(_textMeshVlad.Vertices);
            vladMesh.Triangles.Assign(_textMeshVlad.Triangles);
            vladMesh.Normals.Assign(_textMeshVlad.Normals);
            var sceneNodeCVlad = new SceneNode
            {
                Components = new List<SceneComponent>()
                {
                    new Transform
                    {
                        Rotation = float3.Zero,
                        Scale = float3.One,
                        Translation = new float3(0, 2000, 0)
                    },
                    MakeEffect.FromDiffuseSpecular(new float4(26/255f,232/255f,148/255f,1)),
                    vladMesh
                }
            };

            //Lato
            var latoMesh = new Mesh();
            latoMesh.Vertices.Assign(_textMeshLato.Vertices);
            latoMesh.Triangles.Assign(_textMeshLato.Triangles);
            latoMesh.Normals.Assign(_textMeshLato.Normals);

            var sceneNodeCLato = new SceneNode
            {
                Components = new List<SceneComponent>()
                {
                    new Transform
                    {
                        Rotation = float3.Zero,
                        Scale = float3.One,
                        Translation = new float3(0, 0, 0)
                    },
                    MakeEffect.FromDiffuseSpecular(new float4(27/255f,242/255f,216/255f,1)),
                    latoMesh
                }
            };

            //GNU
            var gnuMesh = new Mesh();
            gnuMesh.Vertices.Assign(_textMeshGnu.Vertices);
            gnuMesh.Triangles.Assign(_textMeshGnu.Triangles);
            gnuMesh.Normals.Assign(_textMeshGnu.Normals);

            var sceneNodeCGnu = new SceneNode
            {
                Components = new List<SceneComponent>()
                {
                    new Transform
                    {
                        Rotation = float3.Zero,
                        Scale = float3.One,
                        Translation = new float3(0, -2000, 0)
                    },
                    MakeEffect.FromDiffuseSpecular(new float4(34/255f,190/255f,219/255f,1)),
                    gnuMesh
                }
            };

            parentNode.Children.Add(sceneNodeCVlad);
            parentNode.Children.Add(sceneNodeCLato);
            parentNode.Children.Add(sceneNodeCGnu);

            _camPivot = new Transform()
            {
                Rotation = float3.Zero,
                Scale = float3.One,
                Translation = new float3(0, 0, 80)
            };
            _camNode = new SceneNode
            {
                Name = "CamPivot",
                Components = new List<SceneComponent>()
                {
                    _camPivot
                },
                Children = new ChildList()
                {
                    new SceneNode()
                    {
                        Name = "MainCam",
                        Components = new List<SceneComponent>()
                        {
                            new Transform()
                            {
                                Rotation = float3.Zero,
                                Scale = float3.One,
                                Translation = new float3(0, 0, -160)
                            },
                            _mainCam
                        }
                    }
                }
            };
            var sc = new SceneContainer { Children = new List<SceneNode> { _camNode, floorNode, parentNode } };

            _renderer = new SceneRendererForward(sc);
        }

        public override void Update()
        {
            float2 speed = float2.Zero;
            if (Mouse.LeftButton)
                speed = Mouse.Velocity;
            else if (Touch.GetTouchActive(TouchPoints.Touchpoint_0))
                speed = Touch.GetVelocity(TouchPoints.Touchpoint_0);

            _alpha += speed.x * 0.0001f;
            _beta += speed.y * 0.0001f;

            _camPivot.RotationQuaternion = QuaternionF.FromEuler(_beta, _alpha, 0);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            _renderer.Render(RC);

            Present();
        }
    }
}