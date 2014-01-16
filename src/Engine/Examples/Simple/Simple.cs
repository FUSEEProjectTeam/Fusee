using System;
using Fusee.Engine;
using Fusee.Math;



namespace Examples.Simple
{
    [FuseeApplication(Name = "Simple Example", Description = "A very simple example.")]
    public class Simple : RenderCanvas
    {

        private Mesh _meshTea, _meshFace;

        private ShaderProgram _spColor;
        private ShaderProgram _spTexture;

        private IShaderParam _colorParam;
        private IShaderParam _textureParam;

        private ITexture _iTex;
        
        private InputDevice controller1;
        private InputDevice controller2;


        public override void Init()
        {

            Input.Instance.InitializeDevices();

            _meshTea = MeshReader.LoadMesh(@"Assets/Teapot.obj.model");
            _meshFace = MeshReader.LoadMesh(@"Assets/Face.obj.model");

            _spColor = MoreShaders.GetShader("simple", RC);
            _spTexture = MoreShaders.GetShader("texture", RC);

            _colorParam = _spColor.GetShaderParam("vColor");
            _textureParam = _spTexture.GetShaderParam("texture1");

            // load texture
            var imgData = RC.LoadImage("Assets/world_map.jpg");
            _iTex = RC.CreateTexture(imgData);


            foreach (InputDevice device in Input.Instance.Devices)
            {
                System.Diagnostics.Debug.Write(device.Name());
            }
                
        }

        public override void RenderAFrame()
        {

            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            //if (Input.Instance.IsKeyDown(KeyCodes.Up))
            //    _angleVert -= RotationSpeed * (float)Time.Instance.DeltaTime;

            //if (Input.Instance.IsKeyDown(KeyCodes.Down))
            //    _angleVert += RotationSpeed * (float)Time.Instance.DeltaTime;

            var mtxCam = float4x4.LookAt(0, 200, 500, 0, 0, 0, 0, 1, 0);

            // first mesh
            RC.ModelView = float4x4.CreateTranslation(Input.Instance.Devices[0].GetAxis("horizontal") * 200, Input.Instance.Devices[0].GetAxis("vertical") * 200 - 50, Input.Instance.Devices[0].GetAxis("z") * 200 ) * mtxCam;

            RC.SetShader(_spColor);
            RC.SetShaderParam(_colorParam, new float4(0.5f, 0.8f, 0, 1));

            RC.Render(_meshTea);

            // second mesh
           // RC.ModelView =  float4x4.CreateTranslation(150 + c2px, 0 + c2py, 0) * mtxCam;

            RC.SetShader(_spTexture);
            RC.SetShaderParamTexture(_textureParam, _iTex);

            //RC.Render(_meshFace);
           
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