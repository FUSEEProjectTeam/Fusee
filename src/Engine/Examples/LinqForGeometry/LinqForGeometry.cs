using Fusee.Engine;
using Fusee.Math;

using hsfurtwangen.dsteffen.lfg;
using Geometry = hsfurtwangen.dsteffen.lfg.Geometry;

namespace Examples.Simple
{
    public class Simple : RenderCanvas
    {
        // pixel and vertex shader
        private const string Vs = @"
            /* Copies incoming vertex color without change.
             * Applies the transformation matrix to vertex position.
             */

            attribute vec4 fuColor;
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;        // for texture

            varying vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;           // for texture

            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
                vNormal = mat3(FUSEE_ITMV) * fuNormal;
                vColor = vec4(0.0, 1.0, 0.0, 0.8);
                vUV = fuUV;             // for texture
            }";

        private const string Ps = @"
            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif

            uniform sampler2D texture1; // for texture
            uniform vec4 vColor;
            varying vec3 vNormal;       // for texture
            varying vec2 vUV;

            void main()
            {
                gl_FragColor = texture2D(texture1, vUV);  // for texture
            }";

        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 1f;
        private const float Damping = 0.92f;
        
        // model variable
        private Mesh _lfgmesh, _meshFace;
        
        // variables for color and texture
        private IShaderParam _vColorParam;
        private IShaderParam _vTextureParam;

        private ImageData _imgData;
        private ITexture _tex;
        

        public override void Init()
        {
            // initialize the variables
            //_lfgmesh = MeshReader.LoadMesh("C:/Users/dominik/DevelopmentTEMP/LinqForGeometry/LinqForGeometry/assets/cube_square_1.obj");
            //_lfgmesh = MeshReader.LoadMesh("C:/Users/dominik/DevelopmentTEMP/LinqForGeometry/LinqForGeometry/assets/sphere_quadrangle_1.obj");
            _lfgmesh = MeshReader.LoadMesh("E:/Development/Fusee/Bin/Debug/Examples/EgoPerspective/Assets/Teapot.obj.model");
            /*
            var geo = new Geometry();
            geo.LoadAsset("C:/Users/dominik/DevelopmentTEMP/LinqForGeometry/LinqForGeometry/assets/cube_square_1.obj");
            _lfgmesh = geo.ToMesh();
            */

            var sp = RC.CreateShader(Vs, Ps);
            RC.SetShader(sp);

            //_vColorParam = sp.GetShaderParam("vColor");

            RC.ClearColor = new float4(1, 1, 1, 0.8f);
        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // move per mouse
            if (Input.Instance.IsButtonDown(MouseButtons.Left))
            {
                _angleVelHorz = RotationSpeed * Input.Instance.GetAxis(InputAxis.MouseX);
                _angleVelVert = RotationSpeed * Input.Instance.GetAxis(InputAxis.MouseY);
            }
            else
            {
                var curDamp = (float)System.Math.Exp(-Damping * Time.Instance.DeltaTime);

                _angleVelHorz *= curDamp;
                _angleVelVert *= curDamp;
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            // move per keyboard
            if (Input.Instance.IsKeyDown(KeyCodes.Left))
                _angleHorz -= RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKeyDown(KeyCodes.Right))
                _angleHorz += RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKeyDown(KeyCodes.Up))
                _angleVert -= RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKeyDown(KeyCodes.Down))
                _angleVert += RotationSpeed * (float)Time.Instance.DeltaTime;

            var mtxRot = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(_angleVert);
            var mtxCam = float4x4.LookAt(0, 200, 400, 0, 50, 0, 0, 1, 0);

            // first mesh (using color)
            RC.ModelView = mtxRot * float4x4.CreateTranslation(-100, 0, 0) * mtxCam;

            // TODO: The crash happens here ... (_vColorParam is null)
            //RC.SetShaderParam(_vColorParam, new float4(0.5f, 0.8f, 0, 1));
            RC.Render(_lfgmesh);

            // swap buffers
            Present();
        }

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