using System;
using Fusee.Engine;
using Fusee.Math;
using ProtoBufTest;

namespace Examples.Simple
{
    [FuseeApplication(Name = "Simple Example", Description = "A very simple example.")]
    public class Simple : RenderCanvas
    {
        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 1f;
        private const float Damping = 0.92f;

        // model variables
        private Mesh _meshTea, _meshFace;

        // variables for shader
        private ShaderProgram _spColor;
        private ShaderProgram _spTexture;

        private IShaderParam _colorParam;
        private IShaderParam _textureParam;

        private ITexture _iTex;

        // is called on startup
        public override void Init()
        {
            RC.ClearColor = new float4(1, 1, 1, 1);
            PersonSerializer ser = new PersonSerializer();
            Person readInMyModel;
            var rawBytes = System.IO.File.ReadAllBytes(@"Assets/werner.bytes");
            using (System.IO.Stream s = new System.IO.MemoryStream(rawBytes))
            {
                readInMyModel = ser.Deserialize(s, null, typeof(Person)) as Person;
            }
            Console.WriteLine(readInMyModel.Name);
            // initialize the variables
            _meshTea = MeshReader.LoadMesh(@"Assets/Teapot.obj.model");
            _meshFace = MeshReader.LoadMesh(@"Assets/Face.obj.model");

            _spColor = MoreShaders.GetDiffuseColorShader(RC);
            _spTexture = MoreShaders.GetDiffuseTextureShader(RC);

            _colorParam = _spColor.GetShaderParam("color");
            _textureParam = _spTexture.GetShaderParam("texture1");

            // load texture
            var imgData = RC.LoadImage("Assets/world_map.jpg");
            _iTex = RC.CreateTexture(imgData);
        }

        // is called once a frame
        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // move per mouse
            if (Input.Instance.IsButton(MouseButtons.Left))
            {
                _angleVelHorz = RotationSpeed*Input.Instance.GetAxis(InputAxis.MouseX);
                _angleVelVert = RotationSpeed*Input.Instance.GetAxis(InputAxis.MouseY);
            }
            else
            {
                var curDamp = (float) Math.Exp(-Damping*Time.Instance.DeltaTime);

                _angleVelHorz *= curDamp;
                _angleVelVert *= curDamp;
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            // move per keyboard
            if (Input.Instance.IsKey(KeyCodes.Left))
                _angleHorz -= RotationSpeed*(float) Time.Instance.DeltaTime;

            if (Input.Instance.IsKey(KeyCodes.Right))
                _angleHorz += RotationSpeed*(float) Time.Instance.DeltaTime;

            if (Input.Instance.IsKey(KeyCodes.Up))
                _angleVert -= RotationSpeed*(float) Time.Instance.DeltaTime;

            if (Input.Instance.IsKey(KeyCodes.Down))
                _angleVert += RotationSpeed*(float) Time.Instance.DeltaTime;

            var mtxRot = float4x4.CreateRotationY(_angleHorz)*float4x4.CreateRotationX(_angleVert);
            var mtxCam = float4x4.LookAt(0, 200, 500, 0, 0, 0, 0, 1, 0);

            // first mesh
            RC.ModelView = float4x4.CreateTranslation(0, -50, 0)*mtxRot*float4x4.CreateTranslation(-150, 0, 0)*mtxCam;

            RC.SetShader(_spColor);
            RC.SetShaderParam(_colorParam, new float4(0.5f, 0.8f, 0, 1));

            RC.Render(_meshTea);

            // second mesh
            RC.ModelView = mtxRot*float4x4.CreateTranslation(150, 0, 0)*mtxCam;

            RC.SetShader(_spTexture);
            RC.SetShaderParamTexture(_textureParam, _iTex);

            RC.Render(_meshFace);

            // swap buffers
            Present();
        }

        // is called when the window was resized
        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width/(float) Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 5000);
        }

        public static void Main()
        {
            var app = new Simple();
            app.Run();
        }
    }
}