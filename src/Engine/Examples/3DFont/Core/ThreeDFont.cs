using System.Collections.Generic;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using static Fusee.Engine.Core.Input;

namespace Fusee.Engine.Examples.ThreeDFont.Core
{

    [FuseeApplication(Name = "ThreeDFont Example", Description = "The official FUSEE ThreeDFont.")]
    public class ThreeDFont : RenderCanvas
    {
        private readonly string _vertexShader = @"
        attribute vec3 fuVertex;
        attribute vec3 fuNormal;
        uniform mat4 xform;            
        varying vec3 normal;

        void main()
        {
	        normal = fuNormal;               
            gl_Position = xform* vec4(fuVertex, 1);
        }";
        private readonly string _pixelShader = @"
        #ifdef GL_ES
	        precision highp float;
        #endif

        varying vec3 normal;

        void main()
        {
            gl_FragColor = vec4(normal*0.5+0.5, 1);
        }";

        private IShaderParam _xformParam;
        private float4x4 _xform;
        private float _alpha;

        private float _pitchCube1;
        private float _pitchCube2;

        private string _text;
        private List<float3> _controlPoints;
        private Mesh _point;

        private ThreeDFontHelper _threeDFontHelper;

        // Init is called on startup. 
        public override void Init()
        {
            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            var vladimir = AssetStorage.Get<Font>("VLADIMIR.TTF");
            var arial = AssetStorage.Get<Font>("arial.ttf");

            _text = "Hello World!";
            _threeDFontHelper = new ThreeDFontHelper(_text, fontLato);

            _controlPoints = (List<float3>) _threeDFontHelper.GetTextVerticesAdaptively(5);

            for (var i = 0; i < _controlPoints.Count; i++)
            {
                _controlPoints[i] = new float3((_controlPoints[i].x / Width) - 1.5f, _controlPoints[i].y / Height, _controlPoints[i].z);
            }

            _point = new Cube();

            var shader = RC.CreateShader(_vertexShader, _pixelShader);
            RC.SetShader(shader);
            _xformParam = RC.GetShaderParam(shader, "xform");
            _alpha = 0;

            // Set the clear color for the backbuffer
            RC.ClearColor = new float4(0.1f, 0.3f, 0.2f, 1);
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
            var projection = float4x4.CreatePerspectiveFieldOfView(3.141592f * 0.25f, aspectRatio, 0.01f, 20);
            var view = float4x4.CreateTranslation(0, 0, 10) * float4x4.CreateRotationY(_alpha);

            foreach (var point in _controlPoints)
            {
                var modelPoint = ModelXForm(new float3(point.x - 3f, point.y - 1f, 0), new float3(0, 0, 0));
                _xform = projection * view * modelPoint * float4x4.CreateScale(0.03f, 0.03f, 0.03f);
                RC.SetShaderParam(_xformParam, _xform);
                RC.Render(_point);
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