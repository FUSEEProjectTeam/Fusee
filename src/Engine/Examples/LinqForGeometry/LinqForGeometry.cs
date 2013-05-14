using Fusee.Engine;
using Fusee.Math;

using hsfurtwangen.dsteffen.lfg;
using Geometry = hsfurtwangen.dsteffen.lfg.Geometry;

namespace Examples.LinqForGeometry
{
    /// <summary>
    /// This example is used to test the LinqForGeometry data structure.
    /// It provides different methods to manipulate geometry data !DIRECTLY! on the data structure.
    /// This should not be used in productive code.
    /// You want to use the transformation algorithms fusee provides to manipulate data.
    /// The here shown transformation algorithms are not ment to be used every frame in an engine. They are just examples for whats possible with the data structure.
    /// </summary>
    public class LinqForGeometry : RenderCanvas
    {
        #region ShaderDefinition
        // pixel and vertex shader
        protected string _vs = @"
            #ifndef GL_ES
               #version 120
            #endif

            /* Copies incoming vertex color without change.
             * Applies the transformation matrix to vertex position.
             */

            attribute vec4 fuColor;
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;
            
        
            varying vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;
        
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
                // vColor = vec4(fuNormal * 0.5 + 0.5, 1.0);
                // vec4 norm4 = FUSEE_MVP * vec4(fuNormal, 0.0);
                // vNormal = norm4.xyz;
                vNormal = mat3(FUSEE_ITMV) * fuNormal;
                vUV = fuUV;
            }";

        protected string _ps = @"
           #ifndef GL_ES
              #version 120
           #endif

            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif

         
            uniform sampler2D texture1;
            uniform sampler2D texture2;
            uniform vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;

            void main()
            {     
                vec4 tex1 = texture2D(texture1,vUV);
                vec4 tex2 = texture2D(texture2,vUV);
                //gl_FragColor = texture2D(texture1, vUV);        
                gl_FragColor = mix(tex1, tex2, 0.6);  /* *dot(vNormal, vec3(0, 0, 1))*/;
                //gl_FragColor = vColor;
            }";

        #endregion ShaderDefinition

        // angle variables
        private static float _angleVelHorz, _angleVelVert;

        // control mechanics
        private const float RotationSpeed = 1f;

        // model variable
        private Mesh _lfgmesh, _FuseeMesh;

        // variables for color and texture
        private IShaderParam _vColorParam;
        private IShaderParam _vTextureParam;

        private ImageData _imgData;
        private ITexture _tex;

        // LFG Geometry
        private Geometry _Geo;

        // 'Gamedesign'
        private float _MovementSpeed = 10.0f;
        private float _InvertMouseAxis = -1.0f;

        public override void Init()
        {
            // Use the Fusee MeshReader to test against mine...
            _FuseeMesh = MeshReader.LoadMesh("Assets/cube_quadrangle_1_textured.obj.model");
            //_FuseeMesh = MeshReader.LoadMesh("Assets/cube_quadrangle_1_textured_quad.obj.model");

            // TODO: For Debug testing, set the normals for the fusee imported model to null.
            // the model should look strange. But nothing happens.

            #region MeshImports
            // This would be a solution to step over the MeshReader Class
            // Important for now to use the transformation methods on the data structure.
            _Geo = new Geometry();
            _Geo.LoadAsset("Assets/cube_quadrangle_1_textured.obj.model");
            //_Geo.LoadAsset("Assets/cube_triangulate.obj.model");
            //_Geo.LoadAsset("Assets/cube_quadrangle_1_textured_quad.obj.model");
            //_Geo.LoadAsset("Assets/sphere_quadrangle_1.obj.model");
            //_Geo.LoadAsset("Assets/Teapot_textured.obj.model");
            //_Geo.LoadAsset("Assets/Sphere.obj.model");
            //_Geo.LoadAsset("Assets/Sphere_triangulate.obj.model");
            #endregion MeshImports

            #region TextureLoad
            //_imgData = RC.LoadImage("Assets/checkerboard_tex.jpg");
            //_imgData = RC.LoadImage("Assets/world_map.jpg");
            _imgData = RC.LoadImage("Assets/Mat_Color.jpg");
            #endregion TextureLoad

            #region Shader
            //ShaderProgram sp = MoreShaders.GetShader("oneColor", RC);
            ShaderProgram sp = MoreShaders.GetShader("diffuse", RC);
            
            //ShaderProgram sp = RC.CreateShader(_vs, _ps);
            _vColorParam = sp.GetShaderParam("Col");
            //_vTextureParam = sp.GetShaderParam("texture1");
            //_tex = RC.CreateTexture(_imgData);

            RC.SetShader(sp);
            #endregion Shader

            // Convert to Mesh
            _lfgmesh = _Geo.ToMesh();

            RC.ClearColor = new float4(0f, 0f, 0f, 1f);
            //RC.SetLight(new float3(0.0f, 0.0f, 0.0f), new float4(1.0f, 0f, 1.0f, 1.0f), 0, 0);
            RC.SetLightActive(0, 1);
            RC.SetLightPosition(0, new float3(5.0f, 0.0f, -2.0f));
            RC.SetLightAmbient(0, new float4(0.2f, 0.2f, 0.2f, 1.0f));
            RC.SetLightSpecular(0, new float4(0.1f, 0.1f, 0.1f, 1.0f));
            RC.SetLightDiffuse(0, new float4(0.8f, 0.8f, 0.8f, 1.0f));
            RC.SetLightDirection(0, new float3(-1.0f, 0.0f, 0.0f));
        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Pull the users input
            PullInput();

            #region SetShaderActive
            RC.SetShaderParam(_vColorParam, new float4(0.5f, 0.8f, 0, 1));
            //RC.SetShaderParamTexture(_vTextureParam, _tex);
            #endregion SetShaderActive

            RC.Render(_lfgmesh);
            _FuseeMesh.Normals = null;
            RC.Render(_FuseeMesh);

            // swap buffers
            Present();
        }

        // Pull the users input
        public void PullInput()
        {
            #region Mouse
            if (Input.Instance.IsButtonDown(MouseButtons.Left))
            {
                _angleVelHorz = RotationSpeed * Input.Instance.GetAxis(InputAxis.MouseX);
                _angleVelVert = RotationSpeed * Input.Instance.GetAxis(InputAxis.MouseY);

                if (_Geo.RotateY(_angleVelHorz * _InvertMouseAxis))
                {
                    if (_Geo.RotateX(_angleVelVert * _InvertMouseAxis))
                    {
                        _Geo._Changes = true;
                    }
                }
            }

            // Scale the model up with the middle mouse button
            if (Input.Instance.IsButtonDown(MouseButtons.Middle))
            {
                if (_Geo.Scale(1.1f, 1.1f, 1.1f))
                {
                    _Geo._Changes = true;
                }
            }

            // Reset the model with the right mouse button
            if (Input.Instance.IsButtonDown(MouseButtons.Right))
            {
                if (_Geo.ResetGeometryToDefault())
                {
                    _Geo._Changes = true;
                }
            }
            #endregion Mouse

            #region Scaling
            if (Input.Instance.IsKeyDown(KeyCodes.Q))
            {
                if (_Geo.Scale(1.1f, 1.1f, 1.1f))
                {
                    _Geo._Changes = true;
                }
            }
            else if (Input.Instance.IsKeyDown(KeyCodes.A))
            {
                if (_Geo.Scale(0.9f, 0.9f, 0.9f))
                {
                    _Geo._Changes = true;
                }
            }
            // Scale only x direction
            if (Input.Instance.IsKeyDown(KeyCodes.W))
            {
                if (_Geo.Scale(1.1f, 1.0f, 1.0f))
                {
                    _Geo._Changes = true;
                }
            }
            else if (Input.Instance.IsKeyDown(KeyCodes.S))
            {
                if (_Geo.Scale(0.9f, 1.0f, 1.0f))
                {
                    _Geo._Changes = true;
                }
            }

            // Scale only y direction
            if (Input.Instance.IsKeyDown(KeyCodes.E))
            {
                if (_Geo.Scale(1.0f, 1.1f, 1.0f))
                {
                    _Geo._Changes = true;
                }
            }
            else if (Input.Instance.IsKeyDown(KeyCodes.D))
            {
                if (_Geo.Scale(1.0f, 0.9f, 1.0f))
                {
                    _Geo._Changes = true;
                }
            }

            // Scale only z direction
            if (Input.Instance.IsKeyDown(KeyCodes.R))
            {
                if (_Geo.Scale(1.0f, 1.0f, 1.1f))
                {
                    _Geo._Changes = true;
                }
            }
            else if (Input.Instance.IsKeyDown(KeyCodes.F))
            {
                if (_Geo.Scale(1.0f, 1.0f, 0.9f))
                {
                    _Geo._Changes = true;
                }
            }

            if (Input.Instance.IsKeyDown(KeyCodes.T))
            {
                if (_Geo.ResetGeometryToDefault())
                {
                    _Geo._Changes = true;
                }
            }
            #endregion Scaling

            #region Translation
            if (Input.Instance.IsKeyDown(KeyCodes.U))
            {
                if (_Geo.Translate(0f, (_MovementSpeed * (float)Time.Instance.DeltaTime) * 2, 0f))
                {
                    _Geo._Changes = true;
                }
            }
            else if (Input.Instance.IsKeyDown(KeyCodes.J))
            {
                if (_Geo.Translate(0f, (-_MovementSpeed * (float)Time.Instance.DeltaTime) * 2, 0f))
                {
                    _Geo._Changes = true;
                }
            }
            if (Input.Instance.IsKeyDown(KeyCodes.H))
            {
                if (_Geo.Translate(-(_MovementSpeed * (float)Time.Instance.DeltaTime) * 2, 0f, 0f))
                {
                    _Geo._Changes = true;
                }
            }
            else if (Input.Instance.IsKeyDown(KeyCodes.K))
            {
                if (_Geo.Translate((_MovementSpeed * (float)Time.Instance.DeltaTime) * 2, 0f, 0f))
                {
                    _Geo._Changes = true;
                }
            }
            if (Input.Instance.IsKeyDown(KeyCodes.Z))
            {
                if (_Geo.Translate(0f, 0f, -(_MovementSpeed * (float)Time.Instance.DeltaTime) * 2))
                {
                    _Geo._Changes = true;
                }
            }
            else if (Input.Instance.IsKeyDown(KeyCodes.I))
            {
                if (_Geo.Translate(0f, 0f, (_MovementSpeed * (float)Time.Instance.DeltaTime) * 2))
                {
                    _Geo._Changes = true;
                }
            }
            #endregion Translation

            #region Rotation
            if (Input.Instance.IsKeyDown(KeyCodes.Up))
            {
                if (_Geo.RotateX(1f * (float)Time.Instance.DeltaTime))
                {
                    _Geo._Changes = true;
                }
            }
            else if (Input.Instance.IsKeyDown(KeyCodes.Down))
            {
                if (_Geo.RotateX(-1f * (float)Time.Instance.DeltaTime))
                {
                    _Geo._Changes = true;
                }
            }

            if (Input.Instance.IsKeyDown(KeyCodes.Left))
            {
                if (_Geo.RotateY(1f * (float)Time.Instance.DeltaTime))
                {
                    _Geo._Changes = true;
                }
            }
            else if (Input.Instance.IsKeyDown(KeyCodes.Right))
            {
                if (_Geo.RotateY(-1f * (float)Time.Instance.DeltaTime))
                {
                    _Geo._Changes = true;
                }
            }

            if (Input.Instance.IsKeyDown(KeyCodes.O))
            {
                if (_Geo.RotateZ(1f * (float)Time.Instance.DeltaTime))
                {
                    _Geo._Changes = true;
                }
            }
            else if (Input.Instance.IsKeyDown(KeyCodes.P))
            {
                if (_Geo.RotateZ(-1f * (float)Time.Instance.DeltaTime))
                {
                    _Geo._Changes = true;
                }
            }
            #endregion Rotation


            if (_Geo._Changes)
            {
                _lfgmesh = _Geo.ToMesh();
            }

            var mtxCam = float4x4.LookAt(0, 500, 500, 0, 0, 0, 0, 1, 0);

            RC.ModelView = float4x4.CreateTranslation(0, 0, 0) * mtxCam;

            _Geo._Changes = false;
        }

        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 5000);
        }

        public static void Main()
        {
            var app = new LinqForGeometry();
            app.Run();
        }

    }
}