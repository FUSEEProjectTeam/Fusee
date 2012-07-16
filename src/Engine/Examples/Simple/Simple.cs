using System;
using System.IO;
using Fusee.Engine;
using Fusee.Math;

namespace Examples
{
    public class Simple : RenderCanvas 
    {
        protected string _vs = @"
/* Copies incoming vertex color without change.
    * Applies the transformation matrix to vertex position.
    */
attribute vec4 fuColor;
attribute vec3 fuVertex;
attribute vec3 fuNormal;
        
varying vec4 vColor;
        
uniform mat4 FUSEE_MVP;
void main()
{
    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
    vColor = fuColor; // vec4(fuNormal, 1.0);
}";

        protected string _ps = @"
/* Copies incoming fragment color without change. */
#ifdef GL_ES
precision highp float;
#endif
        
varying vec4 vColor;
        
void main()
{
    gl_FragColor = vColor;
}";

        private static float _angleHorz = 0.0f, _angleVert = 0.0f, _angleVelHorz = 0, _angleVelVert = 0, _rotationSpeed = 10.0f, _damping = 0.95f;
        protected Mesh _mesh;

        public override void Init()
        {
            //Mesh cube = new Cube();
            Geometry geo = MeshReader.ReadWavefrontObj(new StreamReader(@"Fusee.obj"));
            _mesh = geo.ToMesh();
            _angleHorz = 0;
            _rotationSpeed = 10.0f;
            ShaderProgram sp = RC.CreateShader(_vs, _ps);
            RC.SetShader(sp);
            RC.ClearColor = new float4(0.2f, 0, 0.3f, 1);
        }

        public override void RenderAFrame()
        {
            // TODO: eliminate the need to call the base class implementation here!!!
            base.RenderAFrame();

            RC.Clear(ClearFlags.Color| ClearFlags.Depth);

            
            if (In.IsButtonDown(MouseButtons.Left))
            {
                _angleVelHorz = _rotationSpeed * In.GetAxis(InputAxis.MouseX) * (float) DeltaTime;
                _angleVelVert = _rotationSpeed * In.GetAxis(InputAxis.MouseY) * (float) DeltaTime;
            }
            else
            {
                _angleVelHorz *= _damping;
                _angleVelVert *= _damping;
            }
            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            if (In.IsKeyDown(KeyCodes.Left))
            {
                _angleHorz -= _rotationSpeed * (float)DeltaTime;
            }
            if (In.IsKeyDown(KeyCodes.Right))
            {
                _angleHorz += _rotationSpeed * (float)DeltaTime;
            }
            if (In.IsKeyDown(KeyCodes.Up))
            {
                _angleVert -= _rotationSpeed * (float)DeltaTime;
            }
            if (In.IsKeyDown(KeyCodes.Down))
            {
                _angleVert += _rotationSpeed * (float)DeltaTime;
            }


            RC.ModelView = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(_angleVert) * float4x4.LookAt(0, 200, 400, 0, 50, 0, 0, 1, 0);

            RC.Render(_mesh);
            Present();
        }

        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            float aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 5000);
        }

        public static void Main()
        {
            Simple app = new Simple();
            app.Run();
        }

    }
}
