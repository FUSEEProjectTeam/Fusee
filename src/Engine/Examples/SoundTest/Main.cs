using System;
using System.Diagnostics;
using System.IO;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.SoundTest
{
    public class SoundTest : RenderCanvas
    {
        protected string Vs = @"
            #ifndef GL_ES
                #version 120
            #endif            

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
                // vColor = vec4(fuNormal * 0.5 + 0.5, 1.0);
                // vec4 norm4 = FUSEE_MVP * vec4(fuNormal, 0.0);
                // vNormal = norm4.xyz;
                vNormal = mat3(FUSEE_ITMV) * fuNormal;
            }";

        protected string Ps = @"
            #ifndef GL_ES
                #version 120
            #endif

            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif         
        
            uniform vec4 vColor;
            varying vec3 vNormal;

            void main()
            {
                gl_FragColor = vec4(0.8, 0.2, 0.2, 1) * dot(vNormal, vec3(0, 0, 1));
            }";

        protected Mesh Mesh;
        private Tests _tests;

        private static float _angleHorz;
        private IAudioStream _audio2;
        private IAudioStream _audio1;
        private bool _once;
        private float _vol = 100.0f;

        private int _state;
        private int _testID;

        private float _timeStep;
        private float _curTime;

        public override void Init()
        {
            RC.ClearColor = new float4(0, 0, 0, 1);

            Mesh = MeshReader.LoadMesh("Assets/Cube.obj.model");

            var sp = RC.CreateShader(Vs, Ps);
            RC.SetShader(sp);

            _audio1 = Audio.Instance.LoadFile("Assets/beep.ogg");
            
            _state = 0;
            _testID = 1;

            _timeStep = 1.0f;
            _curTime = 3.0f;

            _tests = new Tests(_audio1);

          /*  Audio.Instance.GetVolume();

            Audio.Instance.Play();
            Audio.Instance.Pause();
            Audio.Instance.Stop();
            // to implement:
         //   Audio.Instance.OpenDevice();

            _audio1 = Audio.Instance.LoadFile("Assets/pacman.mp3");
            _audio1.Loop = true;
            _audio1.Play();
            _audio1.Pause();
            _audio1.Stop();
            _audio1.Volume = 0.5f;
            */
         /*   Audio.Instance.GetVolume();
            Audio.Instance.SetVolume(0.5f);
            Audio.Instance.Play();
            Audio.Instance.Play(_audio1);
            Audio.Instance.Pause();
            Audio.Instance.Pause(_audio1);
            Audio.Instance.Stop();
            Audio.Instance.Stop(_audio1);*/

           // Audio.Instance.CloseDevice();
        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            if (Time.Instance.TimeSinceStart > _curTime)
            {
                _curTime += _timeStep;

                var done = false;

                switch (_testID)
                {
                    case 1:
                        done = _tests.Test1(_state);
                        break;

                    case 2:
                        done = _tests.Test2(_state);
                        break;
                }

                if (done)
                {
                    _testID++;
                    _state = 0;
                }
                else
                    _state++;
            }

            _angleHorz += 0.002f;

            var mtxRot = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(0);
            var mtxCam = float4x4.LookAt(0, 200, 400, 0, 50, 0, 0, 1, 0);

            RC.ModelView = mtxRot * float4x4.CreateTranslation(-100, 0, 0) * mtxCam;
            RC.Render(Mesh);

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
