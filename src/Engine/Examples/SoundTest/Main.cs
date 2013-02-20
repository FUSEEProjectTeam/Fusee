using System;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.SoundTest
{
    public class SoundTest : RenderCanvas
    {
        protected string Vs = @"
             #version 120

            /* Copies incoming vertex color without change.
             * Applies the transformation matrix to vertex position.
             */

            attribute vec4 fuColor;
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
        
            varying vec4 vColor;
            varying vec3 vNormal;
        
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
                vNormal = mat3(FUSEE_ITMV) * fuNormal;
            }";

        protected string Ps = @"
            #version 120

            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif
        
            uniform vec4 vColor;
            varying vec3 vNormal;

            void main()
            {
                gl_FragColor = vColor * dot(vNormal, vec3(0, 0, 1));
            }";

        //SampleToWaveProvider _mainOutputStream;
        //PanningSampleProvider _panningSampleProvider;
        private float _panningVal;

        public override void Init()
        {
            RC.ClearColor = new float4(0, 0, 0, 1);
            
            Aud.OpenDevice();
            Aud.LoadFile("Assets/tetris.mp3");
            Aud.Play();

            _panningVal = 0;
        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            if (In.IsKeyDown(KeyCodes.Left))
            {
                _panningVal = Math.Max(-1, _panningVal - 0.001f);
            }
            if (In.IsKeyDown(KeyCodes.Right))
            {
                _panningVal = Math.Min(+1, _panningVal + 0.001f);
            }

            //_panningSampleProvider.Pan = _panningVal;
            Present();
        }

        public override void Resize()
        {
            // is called when the window is resized
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView((float) (Math.PI/4f), aspectRatio, 1, 10000);
        }

        public static void Main()
        {
            var app = new SoundTest();
            app.Run();
        }
    }
}
