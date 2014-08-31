using Fusee.Engine;
using Fusee.Math;

namespace Examples.PickingSimple
{
    public class PickingSimple : RenderCanvas
    {
        private PickingContext _pc;

        private Mesh _meshTea;
        private Mesh _meshCube;

        private ShaderProgram _spColor;
        private IShaderParam _colorParam;

        public override void Init()
        {
            RC.ClearColor = new float4(1, 1, 1, 1);

            _pc = new PickingContext(RC);

            _meshTea = MeshReader.LoadMesh(@"Assets/Teapot.obj.model");
            _meshCube = MeshReader.LoadMesh(@"Assets/Cube.obj.model");

            _spColor = MoreShaders.GetDiffuseColorShader(RC);
            _colorParam = _spColor.GetShaderParam("color");
        }

        // is called once a frame
        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            if (Input.Instance.IsButton(MouseButtons.Left))
            {
                _pc.Pick(Input.Instance.GetMousePos());
            }

            RC.SetShader(_spColor);

            var mtxCam = float4x4.LookAt(0, 200, 500, 0, 0, 0, 0, 1, 0);

            //Teapot
            RC.Model = float4x4.CreateTranslation(150, -50, 0);
            RC.View = mtxCam;
            RC.SetShaderParam(_colorParam, new float4(0.5f, 0.8f, 0, 1));
            RC.Render(_meshTea);
            _pc.AddPickableObject(_meshTea, "Teapot", RC.Model, mtxCam);

            //Cube
            RC.Model = float4x4.CreateTranslation(-150, 0, 0) * float4x4.Scale(0.6f);
            RC.View = mtxCam;
            RC.SetShaderParam(_colorParam, new float4(0.8f, 0.5f, 0, 1));
            RC.Render(_meshCube);
            _pc.AddPickableObject(_meshCube, "Cube", RC.Model, mtxCam);

            if (_pc.PickResults.Count > 0)
            {
                System.Console.WriteLine(_pc.PickResults[0].id);
                _pc.ClearResults();
            }

            Present();
        }

        // is called when the window was resized
        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);
        }

        public static void Main()
        {
            var app = new PickingSimple();
            app.Run();
        }
    }
}
