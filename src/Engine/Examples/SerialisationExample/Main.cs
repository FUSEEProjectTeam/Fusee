using System.IO;
using Fusee.Engine;
using Fusee.Math;
using System.Diagnostics;


namespace Examples.SerialisationExample
{
    [FuseeApplication(Name = "Serialisation Example", Description = "Example of Mesh loading via MeshReader and Protobuf.")]
    public class SerialisationExample : RenderCanvas
    {
        private FuseeSerializer _ser;
        private Stopwatch _watch;
        private Mesh _currentMesh;

        private ShaderProgram _spColor;
        private IShaderParam _colorParam;

        private readonly float4x4 _mtxCam = float4x4.LookAt(0, 200, 500, 0, 0, 0, 0, 1, 0);
        private const float RotationSpeed = -1.0f;
        private float _smoothRotation = 0;
        private bool _isCurrentlyLoading = false;

        public override void Init()
        {
            RC.ClearColor = new float4(1, 1, 1, 1);
            _spColor = MoreShaders.GetDiffuseColorShader(RC);
            _colorParam = _spColor.GetShaderParam("color");
            RC.SetShader(_spColor);
            RC.SetShaderParam(_colorParam, new float4(1, 0, 0, 1));
            _watch = new Stopwatch();
            _ser = new FuseeSerializer();
            Debug.WriteLine("Serialisation Example started. \nPress F1 to load Mesh using MeshReader. \nPress F2 to load Mesh using Protobuf.");

            // Example of how to save a Mesh with protobuf to file.
            /*Mesh temp = MeshReader.LoadMesh(@"Assets/Teapot.obj.model");
            using (var file = File.Create(@"Assets/Teapot2.protobuf.model"))
            {
                _ser.Serialize(file,temp);
            }*/
        }

        public override void RenderAFrame()
        {
            // is called once a frame
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            if (Input.Instance.IsKeyDown(KeyCodes.F1) && !_isCurrentlyLoading)
            {
                LoadMeshWithObjParser();
            }
            if (Input.Instance.IsKeyDown(KeyCodes.F2) && !_isCurrentlyLoading)
            {
                LoadMeshWithProtobuf();
            }

            if (_currentMesh != null)
            {
                _smoothRotation += RotationSpeed * (float)Time.Instance.DeltaTime;
                RC.ModelView = _mtxCam * float4x4.CreateRotationY(_smoothRotation);
                RC.Render(_currentMesh);
            }
            Present();
        }

        private void LoadMeshWithObjParser()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            _isCurrentlyLoading = true;
            _currentMesh = null;
            Debug.WriteLine("Started loading Mesh using MeshReader.");
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            _watch.Start();
            _currentMesh = MeshReader.LoadMesh(@"Assets/Teapot.obj.model");
            _watch.Stop();
            Debug.WriteLine("Mesh loaded using MeshReader in " + _watch.ElapsedMilliseconds + "ms");
            _watch.Reset();
            _isCurrentlyLoading = false;
        }

        private void LoadMeshWithProtobuf()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            _isCurrentlyLoading = true;
            _currentMesh = null;
            Debug.WriteLine("Started loading Mesh using Protobuf.");
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            _watch.Start();
            using (var file = File.OpenRead(@"Assets/Teapot.protobuf.model"))
            {
                _currentMesh = _ser.Deserialize(file, null, typeof(Mesh)) as Mesh;
            }
            _watch.Stop();
            Debug.WriteLine("Mesh loaded using protobuf in " + _watch.ElapsedMilliseconds + "ms");
            _watch.Reset();
            _isCurrentlyLoading = false;
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
            var app = new SerialisationExample();
            app.Run();
        }

    }
}
