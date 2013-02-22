using System;
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

        //SampleToWaveProvider _mainOutputStream;
        //PanningSampleProvider _panningSampleProvider;
        private float _panningVal;
        protected Mesh Mesh;
        private static float _angleHorz;
        private IAudioStream _audio2;
        private IAudioStream _audio1;
        private bool _once = false;
        private float _vol = 1.0f;

        public override void Init()
        {
            RC.ClearColor = new float4(0, 0, 0, 1);

            Geometry geo = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Cube.obj.model"));
            Mesh = geo.ToMesh();

            ShaderProgram sp = RC.CreateShader(Vs, Ps);
            RC.SetShader(sp);

            Audio.Instance.OpenDevice();
            _audio1 = Audio.Instance.LoadFile("Assets/tetris.mp3");
            _audio2 = Audio.Instance.LoadFile("Assets/pacman.mp3");
            _audio1.Play();
            
            _panningVal = 0;
        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

         /*  if ((!_once) && (Input.Instance.IsKeyDown(KeyCodes.S)))
            {
                _audio2.Play();
                _once = true;
            }

            if ((_once) && (Input.Instance.IsKeyDown(KeyCodes.D)))
            {
                _audio2.Stop();
                _audio1.Stop();
                _once = false;
            }
            */

            //if (Input.Instance.IsKeyDown(KeyCodes.Up))
            //    _vol = Math.Min(_vol + 0.001f, 1.0f);
            //if (Input.Instance.IsKeyDown(KeyCodes.Down))
            //    _vol = Math.Max(_vol - 0.001f, 0f);
         
            //_audio1.SetVolume(_vol);

            if (Input.Instance.IsKeyDown(KeyCodes.Left))
            {
                _panningVal = Math.Max(-1, _panningVal - 0.001f);
            }
            if (Input.Instance.IsKeyDown(KeyCodes.Right))
            {
                _panningVal = Math.Min(+1, _panningVal + 0.001f);
            }

            _angleHorz += 0.002f;
            float4x4 mtxRot = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(0);
            float4x4 mtxCam = float4x4.LookAt(0, 200, 400, 0, 50, 0, 0, 1, 0);

            RC.ModelView = mtxRot * float4x4.CreateTranslation(-100, 0, 0) * mtxCam;
            RC.Render(Mesh);

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
