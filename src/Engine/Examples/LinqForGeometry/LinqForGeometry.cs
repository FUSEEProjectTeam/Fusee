/*
	Author: Dominik Steffen
	E-Mail: dominik.steffen@hs-furtwangen.de, dominik.steffen@gmail.com
	Bachelor Thesis Summer Semester 2013
	'Computer Science in Media'
	Project: LinqForGeometry
	Professors:
	Mr. Prof. C. Müller
	Mr. Prof. W. Walter
*/

using System;
using System.Diagnostics;
using Fusee.Engine;
using Fusee.Math;
using Geometry = LinqForGeometry.Core.Geometry;
using LFG.ExternalModules.Transformations;

namespace Examples.LinqForGeometry
{
    /// <summary>
    /// This example is used to test the LinqForGeometry data structure.
    /// It provides different methods to manipulate geometry data !DIRECTLY! on the data structure.
    /// This should rather not be used in productive game code but in editing softare of for special purposes.
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

        private static float _angleVelHorz, _angleVelVert;
        private const float RotationSpeed = 1f;
        private Mesh _lfgmesh;

        private ShaderProgram _spColor;
        private ShaderProgram _msDiffuse;
        private IShaderParam _colorParam;
        private IShaderParam _vLightShaderParam;
        private ITexture _tex;

        // The LFG object.
        private Geometry _Geo;

        // Control stuff.
        private float _MovementSpeed = 10.0f;
        private float _InvertMouseAxis = -1.0f;
        private float _demotimeDone = 0f;
        
        private bool demoMode = false;
        private const double demoTimeOut = 45.0; // 0.0 for infinite loop
        private int demoActionInterval = 2; // number in seconds for how long to do one specific action
        private double demoRunTime = 0;
        private int demoRaxis = 0;
        private bool runDemoAnimation = false;

        /// <summary>
        /// Const variable for shader testing.
        /// 0 -> Color Shader
        /// 1 -> Texture Shader
        /// ...
        /// </summary>
        private int _ShaderType = 0;

        /// <summary>
        /// Used for when the shader is changed on runtime.
        /// </summary>
        private bool _ShaderChange = true;

        public override void Init()
        {
            VSync = false;

            // Start the application in demo mode?
            demoMode = true;

            #region MeshImports
            _Geo = new Geometry();
            //_Geo.LoadAsset("Assets/Cube.obj.model");
            //_Geo.LoadAsset("Assets/Cube_quads.obj.model");
            //_Geo.LoadAsset("Assets/Sphere.obj.model");
            //_Geo.LoadAsset("Assets/Sphere_quads.obj.model");
            //_Geo.LoadAsset("Assets/SharedCorners.obj.model");
            //_Geo.LoadAsset("Assets/Cylinder.obj.model");
            //_Geo.LoadAsset("Assets/Cylinder_quads.obj.model");
            //_Geo.LoadAsset("Assets/SharedCorners_pro.obj.model");
            _Geo.LoadAsset("Assets/Teapot.obj.model");

            // Due to copyright reasons, this file will not be delivered with the project.
            //_Geo.LoadAsset("Assets/Hellknight.obj.model");
            #endregion MeshImports

            // Set the smoothing angle for the edge based vertex normal calculation
            // Feel free to test around.
            _Geo._SmoothingAngle = 89.0;

            // The shader colors here are not supposed to be changed. They don't have an effect. If you want to change the shaders
            // then please change the values in the ShaderChanger() method. These ones are just for declaration.
            #region Shaders
            #region TextureShader

            _msDiffuse = MoreShaders.GetDiffuseTextureShader(RC);
            _vLightShaderParam = _msDiffuse.GetShaderParam("texture1");

            //ImageData imgData = RC.LoadImage("Assets/Cube_Mat_uv.jpg");
            ImageData imgData = RC.LoadImage("Assets/world_map.jpg");
            //ImageData imgData = RC.LoadImage("Assets/Teapot_Texture.jpg");

            // Due to copyright reasons, this file will not be delivered with the project.
            //ImageData imgData = RC.LoadImage("Assets/Hellknight.jpg");

            _tex = RC.CreateTexture(imgData);
            #endregion TextureShader

            #region ColorShader

            _spColor = MoreShaders.GetDiffuseColorShader(RC);
            _colorParam = _spColor.GetShaderParam("color");

            RC.SetShader(_spColor);
            RC.SetShaderParam(_colorParam, new float4(0f, 0f, 0f, 1));
            #endregion ColorShader

            #region LightPos

            RC.SetLightActive(0, 0f);
            RC.SetLightAmbient(0, new float4(0.0f, 0.0f, 0.0f, 1.0f));
            RC.SetLightDiffuse(0, new float4(1.0f, 1.0f, 1.0f, 1.0f));
            RC.SetLightDirection(0, new float3(0.0f, -1.0f, 0.0f));

            RC.SetLightActive(1, 0f);
            RC.SetLightAmbient(1, new float4(0.0f, 0.0f, 0.0f, 1.0f));
            RC.SetLightDiffuse(1, new float4(0.5f, 0.5f, 0.5f, 1.0f));
            RC.SetLightDirection(1, new float3(1.0f, 0.0f, 0.0f));

            #endregion LightPos
            #endregion

            // Convert the loaded lfg model to a fusee mesh the first time.
            _lfgmesh = _Geo.ToMesh();

            RC.ClearColor = new float4(0.2f, 0.2f, 0.2f, 1f);

            // TODO: For Benchmarking only.
            _ShaderType = 1;
            runDemoAnimation = true;
            _Geo._DoCalcVertexNormals = true;
        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            PullUserInput();

            if (runDemoAnimation)
                DemoRotation(demoActionInterval, 1f);

            RC.Render(_lfgmesh);
            //RC.Render(_FuseeMesh);

            // swap buffers
            Present();

            if (_ShaderChange)
            {
                ShaderChanger(_ShaderType);
            }

            if(demoMode)
                demoRunTime += Time.Instance.DeltaTime;

            // If demo time is done quit the app
            if (demoTimeOut != 0.0 && demoRunTime > demoTimeOut)
                System.Environment.Exit(-1);

        }

        // Pull the users input
        public void PullUserInput()
        {
            #region Mouse
            if (Input.Instance.IsButton(MouseButtons.Left))
            {
                _angleVelHorz = RotationSpeed * Input.Instance.GetAxis(InputAxis.MouseX);
                _angleVelVert = RotationSpeed * Input.Instance.GetAxis(InputAxis.MouseY);

                if (Transformations.RotateY(_angleVelHorz * _InvertMouseAxis, ref _Geo))
                {
                    if (Transformations.RotateX(_angleVelVert * _InvertMouseAxis, ref _Geo))
                    {
                        _Geo._Changes = true;
                    }
                }
            }

            // Scale the model up with the middle mouse button
            if (Input.Instance.IsButton(MouseButtons.Middle))
            {
                if (Transformations.Scale(1.1f, 1.1f, 1.1f, ref _Geo))
                {
                    _Geo._Changes = true;
                }
            }

            // Reset the model with the right mouse button
            if (Input.Instance.IsButton(MouseButtons.Right))
            {
                if (_Geo.ResetGeometryToDefault())
                {
                    _Geo._Changes = true;
                }
            }
            #endregion Mouse

            #region Scaling
            if (Input.Instance.IsKey(KeyCodes.Q))
            {
                if (Transformations.Scale(1.1f, 1.1f, 1.1f, ref _Geo))
                {
                    _Geo._Changes = true;
                }
            }
            else if (Input.Instance.IsKey(KeyCodes.A))
            {
                if (Transformations.Scale(0.9f, 0.9f, 0.9f, ref _Geo))
                {
                    _Geo._Changes = true;
                }
            }
            // Scale only x direction
            if (Input.Instance.IsKey(KeyCodes.W))
            {
                if (Transformations.Scale(1.1f, 1.0f, 1.0f, ref _Geo))
                {
                    _Geo._Changes = true;
                }
            }
            else if (Input.Instance.IsKey(KeyCodes.S))
            {
                if (Transformations.Scale(0.9f, 1.0f, 1.0f, ref _Geo))
                {
                    _Geo._Changes = true;
                }
            }

            // Scale only y direction
            if (Input.Instance.IsKey(KeyCodes.E))
            {
                if (Transformations.Scale(1.0f, 1.1f, 1.0f, ref _Geo))
                {
                    _Geo._Changes = true;
                }
            }
            else if (Input.Instance.IsKey(KeyCodes.D))
            {
                if (Transformations.Scale(1.0f, 0.9f, 1.0f, ref _Geo))
                {
                    _Geo._Changes = true;
                }
            }

            // Scale only z direction
            if (Input.Instance.IsKey(KeyCodes.R))
            {
                if (Transformations.Scale(1.0f, 1.0f, 1.1f, ref _Geo))
                {
                    _Geo._Changes = true;
                }
            }
            else if (Input.Instance.IsKey(KeyCodes.F))
            {
                if (Transformations.Scale(1.0f, 1.0f, 0.9f, ref _Geo))
                {
                    _Geo._Changes = true;
                }
            }
            #endregion Scaling

            #region Translation
            if (Input.Instance.IsKey(KeyCodes.U))
            {
                if (Transformations.Translate(0f, (_MovementSpeed * (float)Time.Instance.DeltaTime) * 20, 0f, ref _Geo))
                {
                    _Geo._Changes = true;
                }
            }
            else if (Input.Instance.IsKey(KeyCodes.J))
            {
                if (Transformations.Translate(0f, (-_MovementSpeed * (float)Time.Instance.DeltaTime) * 20, 0f, ref _Geo))
                {
                    _Geo._Changes = true;
                }
            }
            if (Input.Instance.IsKey(KeyCodes.H))
            {
                if (Transformations.Translate(-(_MovementSpeed * (float)Time.Instance.DeltaTime) * 20, 0f, 0f, ref _Geo))
                {
                    _Geo._Changes = true;
                }
            }
            else if (Input.Instance.IsKey(KeyCodes.K))
            {
                if (Transformations.Translate((_MovementSpeed * (float)Time.Instance.DeltaTime) * 20, 0f, 0f, ref _Geo))
                {
                    _Geo._Changes = true;
                }
            }
            if (Input.Instance.IsKey(KeyCodes.Z))
            {
                if (Transformations.Translate(0f, 0f, -(_MovementSpeed * (float)Time.Instance.DeltaTime) * 20, ref _Geo))
                {
                    _Geo._Changes = true;
                }
            }
            else if (Input.Instance.IsKey(KeyCodes.I))
            {
                if (Transformations.Translate(0f, 0f, (_MovementSpeed * (float)Time.Instance.DeltaTime) * 20, ref _Geo))
                {
                    _Geo._Changes = true;
                }
            }
            #endregion Translation

            #region Rotation
            if (Input.Instance.IsKey(KeyCodes.Up))
            {
                if (Transformations.RotateX(1f * (float)Time.Instance.DeltaTime, ref _Geo))
                {
                    _Geo._Changes = true;
                }
            }
            else if (Input.Instance.IsKey(KeyCodes.Down))
            {
                if (Transformations.RotateX(-1f * (float)Time.Instance.DeltaTime, ref _Geo))
                {
                    _Geo._Changes = true;
                }
            }

            if (Input.Instance.IsKey(KeyCodes.Left))
            {
                if (Transformations.RotateY(1f * (float)Time.Instance.DeltaTime, ref _Geo))
                {
                    _Geo._Changes = true;
                }
            }
            else if (Input.Instance.IsKey(KeyCodes.Right))
            {
                if (Transformations.RotateY(-1f * (float)Time.Instance.DeltaTime, ref _Geo))
                {
                    _Geo._Changes = true;
                }
            }

            if (Input.Instance.IsKey(KeyCodes.O))
            {
                if (Transformations.RotateZ(1f * (float)Time.Instance.DeltaTime, ref _Geo))
                {
                    _Geo._Changes = true;
                }
            }
            else if (Input.Instance.IsKey(KeyCodes.P))
            {
                if (Transformations.RotateZ(-1f * (float)Time.Instance.DeltaTime, ref _Geo))
                {
                    _Geo._Changes = true;
                }
            }
            #endregion Rotation

            #region ActivateVertexNormals
            if (Input.Instance.IsKeyDown(KeyCodes.N))
            {
                _Geo._DoCalcVertexNormals = true;
            }
            if (Input.Instance.IsKeyDown(KeyCodes.M))
            {
                _Geo._DoCalcVertexNormals = false;
            }
            #endregion ActivateVertexNormals

            if (Input.Instance.IsKeyDown(KeyCodes.T))
            {
                if (_Geo.ResetGeometryToDefault())
                {
                    _Geo._Changes = true;
                }
            }

            if (_Geo._Changes)
            {
                _lfgmesh = _Geo.ToMesh();
                _Geo._Changes = false;
            }

            #region Shader Render Settings and demo
            if (Input.Instance.IsKeyDown(KeyCodes.F1) && Input.Instance.IsKey(KeyCodes.LControl))
            {
                _ShaderChange = true;
                _ShaderType = 0;
            }
            else if (Input.Instance.IsKeyDown(KeyCodes.F2) && Input.Instance.IsKey(KeyCodes.LControl))
            {
                _ShaderChange = true;
                _ShaderType = 1;
            }
            else if (Input.Instance.IsKeyDown(KeyCodes.F3) && Input.Instance.IsKey(KeyCodes.LControl))
            {
                runDemoAnimation = !runDemoAnimation;
            }
            #endregion

            float4x4 mtxCam = float4x4.LookAt(0, 500, 500, 0, 0, 0, 0, 1, 0);

            RC.ModelView = float4x4.CreateTranslation(0, 0, 0) * mtxCam;
        }

        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);
        }

        public static void Main()
        {
            var app = new LinqForGeometry();
            app.Run();
        }

        /// <summary>
        /// Changes the shader during runtime depending on an integer value.
        /// </summary>
        private void ShaderChanger(int shaderId)
        {
            // Determines which shader will be used in testing.
            switch (shaderId)
            {
                case 0:
                    RC.SetShader(_spColor);
                    RC.SetShaderParam(_colorParam, new float4(0.8f, 0.0f, 0, 1));
                    RC.SetLightActive(0, 0f);
                    RC.SetLightActive(1, 0f);
                    break;
                case 1:
                    RC.SetShader(_msDiffuse);
                    RC.SetShaderParamTexture(_vLightShaderParam, _tex);
                    RC.SetLightActive(0, 1f);
                    RC.SetLightActive(1, 1f);
                    break;
                default:
                    break;
            }

            _ShaderChange = false;
        }

        /// <summary>
        /// This method is used for a demo rotation on the models. It uses an interval to rotate in one direction.
        /// </summary>
        /// <param name="intervall">A interval in seconds. e.g. 3.0f for 3 sec.</param>
        /// <param name="rSpeed">The rotation speed. Typically 1.0f</param>
        private void DemoRotation(float interval, float rSpeed)
        {
            if (_demotimeDone < interval)
            {
                float deltaTime = (float)Time.Instance.DeltaTime;

                if (demoRaxis == 0)
                    Transformations.RotateX(rSpeed * deltaTime, ref _Geo);
                if (demoRaxis == 1)
                    Transformations.RotateY(rSpeed * deltaTime, ref _Geo);
                if (demoRaxis == 2)
                    Transformations.RotateZ(rSpeed * deltaTime, ref _Geo);

                _Geo._Changes = true;
                _lfgmesh = _Geo.ToMesh();
                _Geo._Changes = false;

                _demotimeDone += deltaTime;
            }
            else
            {
                _demotimeDone = 0;
                if (demoRaxis < 2)
                {
                    demoRaxis++;
                }
                else
                {
                    demoRaxis = 0;
                }
            }
        }


    }
}