using System;
using System.Diagnostics;
using Fusee.Engine;
using Fusee.Math;

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
        private Mesh _meshFace;
        private Mesh _meshTea;
        
        // variables for shader
        private ShaderProgram _spColor;
        private ShaderProgram _spTexture;

        private IShaderParam _colorParam;
        private IShaderParam _textureParam;

        private ITexture _iTex;
        private float _zz;

        // is called on startup
        public override void Init()
        {
            RC.ClearColor = new float4(1, 1, 1, 1);

            // initialize the variables
            _meshTea = MeshReader.LoadMesh(@"Assets/Teapot.obj.model");
            // _meshFace = MeshReader.LoadMesh(@"Assets/coords.obj.model"); ;
            _meshFace = new Cube();
            
            _spColor = MoreShaders.GetDiffuseColorShader(RC);
            _spTexture = MoreShaders.GetTextureShader(RC);

            _colorParam = _spColor.GetShaderParam("color");
            _textureParam = _spTexture.GetShaderParam("texture1");

            // load texture
            var imgData = RC.LoadImage("Assets/coords.jpg");
            _iTex = RC.CreateTexture(imgData);
            _zz = 0.0f;
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

            // Row order notation
            var mtxRot_ROW = float4x4.CreateRotationY_ROW(_angleHorz) * float4x4.CreateRotationX_ROW(_angleVert);
            // Column order notation
            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            //Debug.Assert(mtxRot_ROW == float4x4.Transpose(mtxRot));

            // Row order notation
            var mtxCam_ROW = float4x4.LookAt_ROW(0, 200, -500, 0, 0, 0, 0, 1, 0);
            mtxCam_ROW = float4x4.Transpose(mtxCam_ROW);
            var mtxCam = float4x4.LookAt(0, 200, -500, 0, 0, 0, 0, 1, 0);

            // Column order notation
            //RC.ModelView = float4x4.CreateTranslation(0, -50, 0)*mtxRot*float4x4.CreateTranslation(-150, 0, 0)*mtxCam;
            // Debug.Assert(mtxCam_ROW == float4x4.Transpose(mtxCam));

            RC.SetShader(_spColor);

            // first mesh
            // Row order notation
            var modelViewMesh1_ROW = float4x4.CreateTranslation_ROW(0, -50, 0) *  float4x4.CreateTranslation_ROW(-150, 0, 0) * mtxRot_ROW * mtxCam_ROW;
            // Column order notation
            var modelViewMesh1 = mtxCam * mtxRot * float4x4.CreateTranslation(-150, 0, 0) * float4x4.CreateTranslation(0, -50, 0);
            //Debug.Assert(modelViewMesh1_ROW == float4x4.Transpose(modelViewMesh1));
            // RC.ModelView = float4x4.Transpose(modelViewMesh1);
            RC.ModelView = modelViewMesh1;


            _zz += (float)(5.0 * Time.Instance.DeltaTime);

            RC.SetShaderParam(_colorParam, new float4(0.5f, 0.8f, 0, 1));
            
            RC.Render(_meshTea);

            // second mesh
            // Row order notation
            var modelViewMesh2_ROW = new float4x4(100, 0, 0, 0, 0, 100, 0, 0, 0, 0, 100, 0, 0, 0, 0, 1) * float4x4.CreateTranslation_ROW(150, 0, 0) * mtxRot_ROW * mtxCam_ROW;
            // Column order notation
            var modelViewMesh2 = mtxCam*mtxRot*float4x4.CreateTranslation(150, 0, 0) * new float4x4(100, 0, 0, 0, 0, 100, 0, 0, 0, 0, 100, 0, 0, 0, 0, 1);
            // Debug.Assert(modelViewMesh2_ROW == float4x4.Transpose(modelViewMesh2));
            // RC.ModelView = float4x4.Transpose(modelViewMesh2);
            RC.ModelView = modelViewMesh2;


            // RC.SetShader(_spTexture);
            // RC.SetShaderParamTexture(_textureParam, _iTex);
            RC.SetShaderParam(_colorParam, new float4(0.8f, 0.5f, 0, 1));

            RC.Render(_meshFace);





            // swap buffers
            Present();
        }

        // is called when the window was resized
        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width/(float) Height;
            var projection_ROW = float4x4.CreatePerspectiveFieldOfView_ROW(MathHelper.PiOver4, aspectRatio, 1, 5000);
            var projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 5000);
            // Debug.Assert(projection_ROW == float4x4.Transpose(projection));
            // RC.Projection = float4x4.Transpose(projection);
            RC.Projection = projection;
            // RC.Projection = float4x4.CreateOrthographic(Width, Height, 1, 5000);
        }

        public static void Main()
        {
            var app = new Simple();
            app.Run();
        }
    }
}