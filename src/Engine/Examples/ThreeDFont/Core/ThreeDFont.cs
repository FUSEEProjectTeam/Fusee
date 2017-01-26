using System.Collections.Generic;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Jometri.DCEL;
using Fusee.Math.Core;
using static Fusee.Engine.Core.Input;
using Fusee.Jometri.Extrusion;
using Fusee.Jometri.Triangulation;
using Fusee.Serialization;
using Geometry = Fusee.Jometri.DCEL.Geometry;

namespace Fusee.Engine.Examples.ThreeDFont.Core
{

    [FuseeApplication(Name = "ThreeDFont Example", Description = "The official FUSEE ThreeDFont.")]
    public class ThreeDFont : RenderCanvas
    {
        private SceneRenderer _renderer;

        private float _alpha;
        private float _beta;

        private string _text;
        private Mesh _textMesh;

        private ThreeDFontHelper _threeDFontHelper;

        // Init is called on startup. 
        public override void Init()
        {
            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            var vladimir = AssetStorage.Get<Font>("VLADIMIR.TTF");
            var gnuSerif = AssetStorage.Get<Font>("GNU-FreeSerif.ttf");

            _text = "Hello World";
            _threeDFontHelper = new ThreeDFontHelper(_text, fontLato);

            var outlines = _threeDFontHelper.GetTextOutlinesWAngle(10);
            var geom2D = new Geometry(outlines);
            geom2D.Triangulate();

            var geom3D = geom2D.Extrude2DPolygon(2000);

            _textMesh = new JometriMesh(geom3D);

            var parentNode = new SceneNodeContainer
            {
                Components = new List<SceneComponentContainer>(),
                Children = new List<SceneNodeContainer>()
            };

            var parentTrans = new TransformComponent
            {
                Rotation = float3.Zero,
                Scale = new float3(0.01f, 0.01f, 0.01f),
                Translation = new float3(-10, 0, 0)
            };

            parentNode.Components.Add(parentTrans);

            var sceneNodeC = new SceneNodeContainer { Components = new List<SceneComponentContainer>() };

            var meshC = new MeshComponent
            {
                Vertices = _textMesh.Vertices,
                Triangles = _textMesh.Triangles,
                Normals = _textMesh.Normals,
            };


            var transC = new TransformComponent
            {
                Rotation = float3.Zero,
                Scale = float3.One,
                Translation = new float3(0, 0, 0)
            };

            sceneNodeC.Components.Add(transC);
            sceneNodeC.Components.Add(meshC);

            parentNode.Children.Add(sceneNodeC);

            var sc = new SceneContainer { Children = new List<SceneNodeContainer> { parentNode } };

            _renderer = new SceneRenderer(sc);


            var shader = RC.CreateShader(AssetStorage.Get<string>("VertShader.vert"), AssetStorage.Get<string>("FragShader.frag"));
            RC.SetShader(shader);

            // Set the clear color for the backbuffer
            RC.ClearColor = new float4(0, 1, 1, 1);
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

            // Create the camera matrix and set it as the current ModelView transformation
            var mtxRot = float4x4.CreateRotationX(_beta) * float4x4.CreateRotationY(_alpha);
            var mtxCam = float4x4.LookAt(0, 0, -80, 0, 0, 0, 0, 1, 0);
            RC.ModelView = mtxCam * mtxRot * ModelXForm(new float3(-55, -8, 0), float3.Zero);

            _renderer.Render(RC);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rerndered farame) on the front buffer.
            Present();
        }

        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width / (float)Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(3.141592f * 0.25f, aspectRatio, 0.01f, 30000);
            RC.Projection = projection;
        }

        private static float4x4 ModelXForm(float3 pos, float3 pivot)
        {
            return float4x4.CreateTranslation(pos + pivot)
                   * float4x4.CreateTranslation(-pivot);
        }
    }
}