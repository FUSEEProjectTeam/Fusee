using System.Collections.Generic;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using static Fusee.Engine.Core.Input;
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
        private List<float3> _controlPoints;
        private Mesh _point;

        private ThreeDFontHelper _threeDFontHelper;

        private int _frameCount;
        private int _pointCount;
        private List<Mesh> _pointList;
        private List<float4x4> _xForms;

        // Init is called on startup. 
        public override void Init()
        {
            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            var vladimir = AssetStorage.Get<Font>("VLADIMIR.TTF");
            var arial = AssetStorage.Get<Font>("arial.ttf");
            _frameCount = 0;
            _pointCount = 0;
            _pointList = new List<Mesh>();
            _xForms = new List<float4x4>();

            _text = "B";
            _threeDFontHelper = new ThreeDFontHelper(_text, fontLato);

            _controlPoints = new List<float3>();

            var outlines = _threeDFontHelper.GetTextOutlinesWAngle(20);
            var geom = new Geometry(outlines, true);

            var test = new List<Geometry.Vertex>();

            foreach (var f in geom.FaceHandles)
            {
                var zwerg = new List<Geometry.Vertex>();
                zwerg.AddRange(geom.GetFaceVertices(f));
                test.AddRange(zwerg);
            }

            foreach (var vertex in test)
            {
                _controlPoints.Add(vertex.Coord);
            }

            for (var i = 0; i < _controlPoints.Count; i++)
            {
                _controlPoints[i] = new float3((_controlPoints[i].x / Width) - 1.5f, _controlPoints[i].y / Height, _controlPoints[i].z);
            }

            _point = new Cube();

            var shader = RC.CreateShader(AssetStorage.Get<string>("VertShader.vert"), AssetStorage.Get<string>("FragShader.frag"));
            RC.SetShader(shader);
            _xformParam = RC.GetShaderParam(shader, "xform");
            _alpha = 0;

            // Set the clear color for the backbuffer
            RC.ClearColor = new float4(0.1f, 0.3f, 0.2f, 1);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            _frameCount++;
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
            var projection = float4x4.CreatePerspectiveFieldOfView(3.141592f * 0.25f, aspectRatio, 0.01f, 20);
            var view = float4x4.CreateTranslation(0, 0, 10) * float4x4.CreateRotationY(_alpha);

            if (_frameCount % 25 == 0 && _pointCount < _controlPoints.Count)
            {
                _pointCount++;
                var point = _controlPoints[_pointCount - 1];
                var modelPoint = ModelXForm(new float3(point.x - 3f, point.y - 1f, 0), new float3(0, 0, 0));
                _xform = projection * view * modelPoint * float4x4.CreateScale(0.02f, 0.02f, 0.02f);

                _pointList.Add(_point);
                _xForms.Add(_xform);

            }
            for (var i = 0; i < _pointList.Count; i++)
            {
                RC.SetShaderParam(_xformParam, _xForms[i]);
                RC.Render(_pointList[i]);
            }

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
            var projection = float4x4.CreatePerspectiveFieldOfView(3.141592f * 0.25f, aspectRatio, 1, 20000);
            RC.Projection = projection;
        }

        static float4x4 ModelXForm(float3 pos, float3 pivot)
        {
            return float4x4.CreateTranslation(pos + pivot)
                   * float4x4.CreateTranslation(-pivot);
        }
    }
}