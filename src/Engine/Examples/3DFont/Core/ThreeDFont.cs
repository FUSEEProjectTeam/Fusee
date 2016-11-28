using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using static Fusee.Engine.Core.Input;
using Fusee.Jometri.Extrusion;
using Geometry = Fusee.Jometri.DCEL.Geometry;

namespace Fusee.Engine.Examples.ThreeDFont.Core
{

    [FuseeApplication(Name = "ThreeDFont Example", Description = "The official FUSEE ThreeDFont.")]
    public class ThreeDFont : RenderCanvas
    {
        private IShaderParam _xformParam;
        private float4x4 _xform;
        private float _alpha;

        private float _pitchCube1;
        private float _pitchCube2;

        private string _text;
        private Mesh _textMesh;

        private ThreeDFontHelper _threeDFontHelper;

        // Init is called on startup. 
        public override void Init()
        {
            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            var vladimir = AssetStorage.Get<Font>("VLADIMIR.TTF");
            var gnuSerif = AssetStorage.Get<Font>("GNU-FreeSerif.ttf");

            _text = "Hello World!";
            _threeDFontHelper = new ThreeDFontHelper(_text, gnuSerif);

            var outlines = _threeDFontHelper.GetTextOutlinesWAngle(10);
            var geom = new Geometry(outlines, true);

            geom = geom.Extrude2DPolygon(2000);

            _textMesh = new HalfEdgeListToMesh(geom);

            var shader = RC.CreateShader(AssetStorage.Get<string>("VertShader.vert"), AssetStorage.Get<string>("FragShader.frag"));
            RC.SetShader(shader);
            _xformParam = RC.GetShaderParam(shader, "xform");
            _alpha = 0;

            // Set the clear color for the backbuffer
            RC.ClearColor = new float4(1f, 1f, 1f, 1);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            _pitchCube1 += Keyboard.WSAxis * 0.1f;
            _pitchCube2 += Keyboard.UpDownAxis * 0.1f;

            _pitchCube1 = M.Clamp(_pitchCube1, -M.Pi / 2, M.Pi / 2);
            _pitchCube2 = M.Clamp(_pitchCube2, -M.Pi / 2, M.Pi / 2);

            var speed = Mouse.Velocity + Touch.GetVelocity(TouchPoints.Touchpoint_0);
            if (Mouse.LeftButton || Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                _alpha -= speed.x * 0.0001f;
            }

            var aspectRatio = Width / (float)Height;
            var projection = float4x4.CreatePerspectiveFieldOfView(3.141592f * 0.25f, aspectRatio, 0.01f, 30000);
            var view = float4x4.CreateTranslation(0, 0, 10) * float4x4.CreateRotationY(_alpha);
            var modelPoint = ModelXForm(new float3(-6, -1, 0), float3.Zero);

            _xform = projection * view * modelPoint * float4x4.CreateScale(0.001f, 0.001f, 0.001f);

            RC.SetShaderParam(_xformParam, _xform);
            RC.Render(_textMesh);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered farame) on the front buffer.
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

        static float4x4 ModelXForm(float3 pos, float3 pivot)
        {
            return float4x4.CreateTranslation(pos + pivot)
                   * float4x4.CreateTranslation(-pivot);
        }
    }
}