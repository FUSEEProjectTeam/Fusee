using System.IO;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.Simple
{
    public class Simple : RenderCanvas
    {
        //Pixel and Vertex Shader
        protected string Vs = @"
             #version 120

            /* Copies incoming vertex color without change.
             * Applies the transformation matrix to vertex position.
             */

            attribute vec4 fuColor;
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
        
            varying vec4 vColor;
            varying vec3 vNormal;
        
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
                // vColor = vec4(fuNormal * 0.5 + 0.5, 1.0);
                // vec4 norm4 = FUSEE_MVP * vec4(fuNormal, 0.0);
                // vNormal = norm4.xyz;
                vNormal = mat3(FUSEE_ITMV) * fuNormal;
            }";

        protected string Ps = @"
             #version 120

            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif
        
            uniform vec4 vColor;
            varying vec3 vNormal;

            void main()
            {
                gl_FragColor = vColor * dot(vNormal, vec3(0, 0, 1));
            }";
        //angle variable
        private static float _angleHorz = 0.0f, _angleVert = 0.0f, _angleVelHorz = 0, _angleVelVert = 0, _rotationSpeed = 10.0f, _damping = 0.95f;
        //modell variable
        protected Mesh Mesh, MeshFace;
        //variable for color
        protected IShaderParam VColorParam;

        public override void Init()
        {
            //initialize the variable
            Geometry geo = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Teapot.obj.model"));
            Mesh = geo.ToMesh();

            Geometry geo2 = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Face.obj.model"));
            MeshFace = geo2.ToMesh();

            _angleHorz = 0;
            _rotationSpeed = 10.0f;
            ShaderProgram sp = RC.CreateShader(Vs, Ps);
            RC.SetShader(sp);
            VColorParam = sp.GetShaderParam("vColor");


            RC.ClearColor = new float4(1, 1, 1, 1);
        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            //move per mouse
            if (Input.Instance.IsButtonDown(MouseButtons.Left))
            {
                _angleVelHorz = _rotationSpeed * Input.Instance.GetAxis(InputAxis.MouseX) * (float)Time.Instance.DeltaTime;
                _angleVelVert = _rotationSpeed * Input.Instance.GetAxis(InputAxis.MouseY) * (float)Time.Instance.DeltaTime;
            }
            else
            {
                _angleVelHorz *= _damping;
                _angleVelVert *= _damping;
            }
            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            //move per keyboard
            if (Input.Instance.IsKeyDown(KeyCodes.Left))
            {
                _angleHorz -= _rotationSpeed * (float)Time.Instance.DeltaTime;
            }
            if (Input.Instance.IsKeyDown(KeyCodes.Right))
            {
                _angleHorz += _rotationSpeed * (float)Time.Instance.DeltaTime;
            }
            if (Input.Instance.IsKeyDown(KeyCodes.Up))
            {
                _angleVert -= _rotationSpeed * (float)Time.Instance.DeltaTime;
            }
            if (Input.Instance.IsKeyDown(KeyCodes.Down))
            {
                _angleVert += _rotationSpeed * (float)Time.Instance.DeltaTime;
            }

            float4x4 mtxRot = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(_angleVert);
            float4x4 mtxCam = float4x4.LookAt(0, 200, 400, 0, 50, 0, 0, 1, 0);

            RC.ModelView = mtxRot * float4x4.CreateTranslation(-100, 0, 0) * mtxCam;
            //colordecloration
            RC.SetShaderParam(VColorParam, new float4(0.5f, 0.8f, 0, 1));
            RC.Render(Mesh);

            RC.ModelView = mtxRot * float4x4.CreateTranslation(100, 0, 0) * mtxCam;
            //colordecloration
            RC.SetShaderParam(VColorParam, new float4(0.8f, 0.5f, 0, 1));
            RC.Render(MeshFace);
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
