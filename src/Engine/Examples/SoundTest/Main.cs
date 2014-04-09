using System;
using System.Diagnostics;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.SoundTest
{
    [FuseeApplication(Name = "Sound Test", Description = "The name of this example says it all...")]
    public class SoundTest : RenderCanvas
    {
        protected Mesh Mesh;
        private Tests _tests;

        private static float _angleHorz;

        private IAudioStream _audio2;
        private IAudioStream _audio1;

        private int _state;
        private int _testID;

        private float _timeStep;
        private float _curTime;

        private IShaderParam _vColor;

        public override void Init()
        {
            RC.ClearColor = new float4(0, 0, 0, 1);

            Mesh = new Cube();

            var sp = MoreShaders.GetDiffuseColorShader(RC);
            RC.SetShader(sp);

            _vColor = RC.GetShaderParam(sp, "color");
            RC.SetShaderParam(_vColor, new float4(0.8f, 0.1f, 0.1f, 1));

            // sound by http://www.soundjay.com
            _audio1 = Audio.Instance.LoadFile("Assets/beep.ogg");

            // excerpt from "the final rewind" by tryad (http://www.tryad.org) - cc-by-sa
            _audio2 = Audio.Instance.LoadFile("Assets/music.ogg");

            _state = 0;
            _testID = 1;

            _timeStep = 1.0f;
            _curTime = 2.0f;

            _tests = new Tests();
        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            if (_testID < 8 && Time.Instance.TimeSinceStart > _curTime)
            {
                _curTime += _timeStep;

                var done = false;

                switch (_testID)
                {
                    case 1: // play, pause, stop, global stop
                        done = _tests.Test1(_audio2, _state);
                        break;

                    case 2: // global volume
                        done = _tests.Test2(_audio1, _state);
                        break;

                    case 3: // individual volume
                        done = _tests.Test3(_audio1, _state);
                        break;

                    case 4: // loop as attribute
                        done = _tests.Test4(_audio1, _state);
                        break;

                    case 5: // loop as parameter
                        done = _tests.Test5(_audio1, _state);
                        break;

                    case 6: // global panning
                        done = _tests.Test6(_audio2, _state);
                        break;

                    case 7: // individual panning
                        done = _tests.Test7(_audio2, _state);
                        break;
                }

                if (done)
                {
                    _testID++;
                    _state = 0;

                    _curTime += 2*_timeStep;

                    if (_testID == 8)
                        Debug.WriteLine("-- done --");
                }
                else
                    _state++;
            }

            _angleHorz += 0.002f;

            // colh
            //var mtxRot = float4x4.CreateRotationY(_angleHorz)*float4x4.CreateRotationX(0);
            //var mtxCam = float4x4.LookAt(0, 200, 400, 0, 50, 0, 0, 1, 0);
            var mtxRot = float4x4.CreateRotationX(-0) * float4x4.CreateRotationY(-_angleHorz);
            var mtxCam = float4x4.LookAt(0, 200, -400, 0, 50, 0, 0, 1, 0);

            // colh
            // RC.ModelView = mtxRot * float4x4.CreateTranslation(-100, 0, 0) * mtxCam;
            RC.ModelView = mtxCam * float4x4.CreateTranslation(-100, 0, 0) * mtxRot * float4x4.Scale(200,200,200);
            RC.Render(Mesh);

            Present();
        }

        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width/(float) Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView((float) (Math.PI/4f), aspectRatio, 1, 10000);
        }

        public static void Main()
        {
            var app = new SoundTest();
            app.Run();
        }
    }
}
