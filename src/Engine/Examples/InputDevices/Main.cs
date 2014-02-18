using Fusee.Engine;
using Fusee.SceneManagement;
using Fusee.Math;

namespace Examples.InputDevices
{
    public class InputDevices : RenderCanvas
    {

        private Mesh _meshTea;

        private ShaderProgram _spColor;
        private IShaderParam _colorParam;

        public override void Init()
        {
            Input.Instance.InitializeDevices();

            _meshTea = MeshReader.LoadMesh(@"Assets/Teapot.obj.model");

            _spColor = MoreShaders.GetDiffuseColorShader(RC);
            _colorParam = _spColor.GetShaderParam("color");
        }

        public override void RenderAFrame()
        {
            float y = 0;
            float z = 0;
            float x = 0;
            if (Input.Instance.CountDevices() != 0)
            {
                y = 50*Input.Instance.GetDevice(0).GetAxis(InputDevice.Axis.Vertical);
                z = 50*Input.Instance.GetDevice(0).GetAxis(InputDevice.Axis.Z);
                x = 50*Input.Instance.GetDevice(0).GetAxis(InputDevice.Axis.Horizontal);
            }

            else
            {
                if (Input.Instance.IsKeyDown(KeyCodes.Up))
                    y++;
                if (Input.Instance.IsKeyDown(KeyCodes.Down))
                    y--;
                if (Input.Instance.IsKeyDown(KeyCodes.Left))
                    x--;
                if (Input.Instance.IsKeyDown(KeyCodes.Right))
                    x++;

            }
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            //if (Input.Instance.IsKeyDown(KeyCodes.Up))
            //    _angleVert -= RotationSpeed * (float)Time.Instance.DeltaTime;

            //if (Input.Instance.IsKeyDown(KeyCodes.Down))
            //    _angleVert += RotationSpeed * (float)Time.Instance.DeltaTime;

            var mtxCam = float4x4.LookAt(0, 200, 500, 0, 0, 0, 0, 1, 0);

            // first mesh
            RC.ModelView = float4x4.CreateTranslation(x, 50 + y, 200 + z) * mtxCam;

            RC.SetShader(_spColor);
            RC.SetShaderParam(_colorParam, new float4(0.5f, 0.8f, 0, 1));

            RC.Render(_meshTea);

            Present();
        }

        public override void Resize()
        {
            // is called when the window is resized
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);
        }

        public static void Main()
        {
            var app = new InputDevices();
            app.Run();
        }

    }
}
