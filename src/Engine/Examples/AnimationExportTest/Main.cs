using Fusee.Engine;
using Fusee.SceneManagement;
using Fusee.Math;
using Fusee.Serialization;
using System.IO;
using Fusee.Engine.SimpleScene;
using System;

namespace Examples.AnimationExportTest
{
    public class AnimationExportTest : RenderCanvas
    {

        private SceneRenderer _sr;
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;
        private const float RotationSpeed = 1f;
        private const float Damping = 0.92f;

        public override void Init()
        {
            RC.ClearColor = new float4(0.5f, 0.5f, 0.5f, 1);

            SceneContainer scene;
            var ser = new Serializer();
            using (var file = File.OpenRead(@"Assets/Robo.fus"))
            {
                scene = ser.Deserialize(file, null, typeof(SceneContainer)) as SceneContainer;
            }
            _sr = new SceneRenderer(scene, "Assets");

        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            if (Input.Instance.IsButton(MouseButtons.Left))
            {
                _angleVelHorz = RotationSpeed * Input.Instance.GetAxis(InputAxis.MouseX);
                _angleVelVert = RotationSpeed * Input.Instance.GetAxis(InputAxis.MouseY);
            }
            else
            {
                var curDamp = (float)Math.Exp(-Damping * Time.Instance.DeltaTime);

                _angleVelHorz *= curDamp;
                _angleVelVert *= curDamp;
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            if (Input.Instance.IsKey(KeyCodes.Left))
                _angleHorz -= RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKey(KeyCodes.Right))
                _angleHorz += RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKey(KeyCodes.Up))
                _angleVert -= RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKey(KeyCodes.Down))
                _angleVert += RotationSpeed * (float)Time.Instance.DeltaTime;

            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, 200, -500, 0, 0, 0, 0, 1, 0);
            RC.ModelView = mtxCam * mtxRot * float4x4.CreateScale(0.5f,0.5f,0.5f);

            _sr.Animate();
            _sr.Render(RC);

            Present();
        }

        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);
        }

        public static void Main()
        {
            var app = new AnimationExportTest();
            app.Run();
        }
    }
}
