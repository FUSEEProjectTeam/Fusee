using System;
using System.Collections.Generic;
using System.IO;
using Fusee.Engine;
using Fusee.Math;
using Fusee.Serialization;

namespace Examples.SceneViewer
{
    public class SceneViewer : RenderCanvas
    {
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 1f;
        private const float Damping = 0.92f;

        // model variables
        private Mesh _meshFace;
        private Mesh _meshTea;
        private SceneRenderer _sr;
        private ShaderProgram _spColor;
        private IShaderParam _colorParam;

        // is called on startup
        public override void Init()
        {
            //TestSerialize();
            //TestDeserialize();
            SceneContainer scene;
            var ser = new Serializer();
            using (var file = File.OpenRead(@"Assets/Model.fus"))
            {
                scene = ser.Deserialize(file, null, typeof(SceneContainer)) as SceneContainer;
            }
            _sr = new SceneRenderer(scene);

            _spColor = MoreShaders.GetDiffuseColorShader(RC);
            _colorParam = _spColor.GetShaderParam("color");
            RC.SetShaderParam(_colorParam, new float4(1, 1, 1, 1));
            RC.ClearColor = new float4(1, 1, 1, 1);
        }

        // is called once a frame
        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            // move per mouse
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

            _angleHorz -= _angleVelHorz;
            _angleVert -= _angleVelVert;

            // move per keyboard
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

            // first mesh
            RC.ModelView = mtxCam * mtxRot /* float4x4.CreateScale(100) * */;

            RC.SetShader(_spColor);
            RC.SetShaderParam(_colorParam, new float4(0.5f, 0.8f, 0, 1));

            //RC.Render(_meshTea);
            _sr.Render(RC);

            // swap buffers
              Present();
        }

        // is called when the window was resized
        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);
            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);
        }

        private void TestSerialize()
        {
            /**/
            var aMesh = new MeshContainer
            {
                Vertices = new[]
                {
                    new float3(-1, -1, -1),
                    new float3(-1, -1, 1),
                    new float3(-1, 1, -1),
                    new float3(1, -1, -1),
                    new float3(1, 1, 1),
                    new float3(-1, 1, 1),
                    new float3(1, -1, 1),
                    new float3(1, 1, -1),
                },
                Normals = new[]
                {
                    new float3(-1, -1, -1),
                    new float3(-1, -1, 1),
                    new float3(-1, 1, -1),
                    new float3(1, -1, -1),
                    new float3(1, 1, 1),
                    new float3(-1, 1, 1),
                    new float3(1, -1, 1),
                    new float3(1, 1, -1),
                },
                Triangles = new ushort[]
                {
                    0, 1, 2,
                    0, 2, 3,
                    0, 3, 1,
                    4, 5, 6,
                    4, 6, 7,
                    4, 7, 5,
                }
            };

            var aChild = new SceneObjectContainer()
            {
                Mesh = aMesh,
                Transform = float4x4.CreateTranslation(0.11f, 0.11f, 0)
            };

            var parent = new SceneContainer()
            {
                Header = new SceneHeader()
                {
                    Version = 1,
                    Generator = "Fusee.SceneViewer",
                    CreatedBy = "FuseeProjetTeam"
                },
                Children = new List<SceneObjectContainer>(new SceneObjectContainer[]
                {
                    aChild,
                    aChild,
                    new SceneObjectContainer()
                    {
                        Mesh = aMesh,
                        Transform = float4x4.CreateTranslation(0.22f, 0.22f, 0)
                    },
                }),
            };
            var ser = new Serializer();
            using (var file = File.Create(@"Assets/Test.fus"))
            {
                ser.Serialize(file, parent);
            }
        }

        private void TestDeserialize()
        {
            var ser = new Serializer();
            SceneContainer mc2;
            using (var file = File.OpenRead(@"Assets/Test.fus"))
            {
                mc2 = ser.Deserialize(file, null, typeof(SceneContainer)) as SceneContainer;
            }
            /**/            
            Diagnostics.Log(mc2.ToString());
        }

        public static void Main()
        {
            var app = new SceneViewer();
            app.Run();
        }
    }
}
