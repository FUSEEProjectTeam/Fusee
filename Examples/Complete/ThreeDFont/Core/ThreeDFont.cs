using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Jometri;
using Fusee.Math.Core;
using Fusee.Serialization;
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

            ////////////////// Fill SceneNodeContainer ////////////////////////////////
            var parentNode = new SceneNodeContainer
            {
                Components = new List<SceneComponentContainer>(),
                Children = new ChildList()
            };

            var parentTrans = new TransformComponent
            {
                Rotation = float3.Zero,
                Scale = new float3(0.01f, 0.01f, 0.01f),
                Translation = new float3(0, 0, 10)
            };

            parentNode.Components.Add(parentTrans);

            //Vladimir
            var sceneNodeCVlad = new SceneNodeContainer { Components = new List<SceneComponentContainer>() };

            var meshCVlad = new Mesh
            {
                Vertices = _textMeshVlad.Vertices,
                Triangles = _textMeshVlad.Triangles,
                Normals = _textMeshVlad.Normals,
            };

            var tranCVlad = new TransformComponent
            {
                Rotation = float3.Zero,
                Scale = float3.One,
                Translation = new float3(0, 2000, 0)
            };

            sceneNodeCVlad.Components.Add(tranCVlad);
            sceneNodeCVlad.Components.Add(meshCVlad);

            //Lato
            var sceneNodeCLato = new SceneNodeContainer { Components = new List<SceneComponentContainer>() };

            var meshCLato = new Mesh
            {
                Vertices = _textMeshLato.Vertices,
                Triangles = _textMeshLato.Triangles,
                Normals = _textMeshLato.Normals,
            };
            var tranCLato = new TransformComponent
            {
                Rotation = float3.Zero,
                Scale = float3.One,
                Translation = new float3(0, 0, 0)
            };

            sceneNodeCLato.Components.Add(tranCLato);
            sceneNodeCLato.Components.Add(meshCLato);

            //GNU
            var sceneNodeCGnu = new SceneNodeContainer { Components = new List<SceneComponentContainer>() };

            var meshCGnu = new Mesh
            {
                Vertices = _textMeshGnu.Vertices,
                Triangles = _textMeshGnu.Triangles,
                Normals = _textMeshGnu.Normals,
            };
            var tranCGnu = new TransformComponent
            {
                Rotation = float3.Zero,
                Scale = float3.One,
                Translation = new float3(0, -2000, 0)
            };

            sceneNodeCGnu.Components.Add(tranCGnu);
            sceneNodeCGnu.Components.Add(meshCGnu);

            parentNode.Children.Add(sceneNodeCVlad);
            parentNode.Children.Add(sceneNodeCLato);
            parentNode.Children.Add(sceneNodeCGnu);

            var sc = new SceneContainer { Children = new List<SceneNodeContainer> { parentNode } };           

            _renderer = new SceneRendererForward(sc);

            var shaderFx = new ShaderEffect(new[] {
                new EffectPassDeclaration
                {
                    PS = AssetStorage.Get<string>("FragShader.frag"),
                    VS = AssetStorage.Get<string>("VertShader.vert"),
                    StateSet = new RenderStateSet
                    {
                        ZEnable = true
                    }
                }
            },
            new List<EffectParameterDeclaration>
            {
                new EffectParameterDeclaration { Name = "xform", Value = float4x4.Identity}
            });

            RC.SetShaderEffect(shaderFx);

            // Set the clear color for the backbuffer
            RC.ClearColor = new float4(0, 0.61f, 0.88f, 1);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            var speed = Mouse.Velocity + Touch.GetVelocity(TouchPoints.Touchpoint_0);
            if (Mouse.LeftButton || Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                _alpha -= speed.x * 0.0001f;
                _beta -= speed.y * 0.0001f;
            }

            // Create the camera matrix and set it as the current View transformation.
            var mtxRot = float4x4.CreateRotationX(_beta) * float4x4.CreateRotationY(_alpha);
            var mtxCam = float4x4.LookAt(0, 0, -80, 0, 0, 0, 0, 1, 0);
            RC.View = mtxCam * mtxRot * ModelXForm(new float3(-55, -8, 0), float3.Zero);

            _renderer.Render(RC);

            Present();
        }

        // Is called when the window was resized
        public override void Resize(ResizeEventArgs e)
        {
        }

        private static float4x4 ModelXForm(float3 pos, float3 pivot)
        {
            return float4x4.CreateTranslation(pos + pivot)
                   * float4x4.CreateTranslation(-pivot);
        }
    }
}