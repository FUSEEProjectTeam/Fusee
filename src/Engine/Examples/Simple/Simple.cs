using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.Simple
{
 
    [FuseeApplication(Name = "Simple Example", Description = "A very simple example.")]
    public class Simple : RenderCanvas
    {
        private static void ReflectionTest()
        {
        }

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

        // is called on startup
        public override void Init()
        {
            ReflectionTest();

            RC.ClearColor = new float4(1, 1, 1, 1);

            // initialize the variables
            _meshTea = MeshReader.LoadMesh(@"Assets/Teapot.obj.model");
            _meshFace = new Cube();
            
            _spColor = Shaders.GetDiffuseColorShader(RC);
            _colorParam = _spColor.GetShaderParam("color");
        }

        // is called once a frame
        public override void RenderAFrame()
        {
            
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            // move per mouse
            if (Input.Instance.IsButton(MouseButtons.Left))
            {
                _angleVelHorz = -RotationSpeed*Input.Instance.GetAxis(InputAxis.MouseX);
                _angleVelVert = -RotationSpeed*Input.Instance.GetAxis(InputAxis.MouseY);
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
                _angleHorz += RotationSpeed*(float) Time.Instance.DeltaTime;

            if (Input.Instance.IsKey(KeyCodes.Right))
                _angleHorz -= RotationSpeed*(float) Time.Instance.DeltaTime;

            if (Input.Instance.IsKey(KeyCodes.Up))
                _angleVert += RotationSpeed*(float) Time.Instance.DeltaTime;

            if (Input.Instance.IsKey(KeyCodes.Down))
                _angleVert -= RotationSpeed*(float) Time.Instance.DeltaTime;

            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, 200, -500, 0, 0, 0, 0, 1, 0);

            RC.SetShader(_spColor);

            // render first mesh
            var modelViewMesh1 = mtxCam * mtxRot * float4x4.CreateTranslation(-150, 0, 0) * float4x4.CreateTranslation(0, -50, 0);
            RC.ModelView = modelViewMesh1;
            RC.SetShaderParam(_colorParam, new float4(0.5f, 0.8f, 0, 1));
            RC.Render(_meshTea);

            // render second mesh
            var modelViewMesh2 = mtxCam*mtxRot*float4x4.CreateTranslation(150, 0, 0) * new float4x4(100, 0, 0, 0, 0, 100, 0, 0, 0, 0, 100, 0, 0, 0, 0, 1);
            RC.ModelView = modelViewMesh2;
            RC.SetShaderParam(_colorParam, new float4(1, 0, 0, 1));
            RC.Render(_meshFace);

            // swap buffers
            Present();
        }

        // is called when the window was resized
        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width/(float) Height;
            var projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 5000);
            RC.Projection = projection;
        }

        public static void Main()
        {
            var app = new Simple();
            app.Run();
        }
    }
}