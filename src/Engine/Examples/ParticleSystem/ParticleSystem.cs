using System;
using Fusee.Engine;
using Fusee.Math;


namespace Examples.ParticleSystem
{
    [FuseeApplication(Name = "Particle System", Description = "A very simple example.")]
    public class ParticleSystem : RenderCanvas
    {
        #region Shader

        // At first we have to define the shader.
        protected string VsSimpleTexture = @"
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
                vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
                vUV = fuUV;
            }";

        protected string PsSimpleTexture = @"
            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif

            // The parameter required for the texturing process
            uniform sampler2D texture1;
            uniform vec4 vColor;
            varying vec3 vNormal;

            // The parameter holding the UV-Coordinates of the texture
            varying vec2 vUV;

            void main()
            {    
              // The most basic texturing function, expecting the above mentioned parameters  
                // max(dot(vec3(0,0,1),normalize(vNormal)), 0.1) 
              gl_FragColor = texture2D(texture1, vUV);        
            }";

        #endregion

 
        
        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 1f;
        private const float Damping = 0.92f;

        // model variables
        private Mesh _meshFace;
        //private Mesh _meshTea = new ParticleEmitter();

        private ParticleEmitter _particleEmitter;
        // variables for shader
        private ShaderProgram _spColor;
        private ShaderProgram _spTexture;

        private IShaderParam _colorParam;
        private IShaderParam _textureParam;

        private ITexture _iTex;

        // is called on startup
        public override void Init()
        {
            RC.ClearColor = new float4(0.7f, 0.7f, 1, 1);

            _particleEmitter = new ParticleEmitter(1);

            // initialize the variables
            //_meshTea = new ParticleEmitter();//MeshReader.LoadMesh(@"Assets/Teapot.obj.model");
            _meshFace = MeshReader.LoadMesh(@"Assets/Face.obj.model");

            _spColor = MoreShaders.GetDiffuseColorShader(RC);
            _spTexture = RC.CreateShader(VsSimpleTexture, PsSimpleTexture);

            _colorParam = _spColor.GetShaderParam("color");
            _textureParam = _spTexture.GetShaderParam("texture1");

            // load texture
            var imgData = RC.LoadImage("Assets/world_map.jpg");
            //var imgData = RC.LoadImage("Assets/smoke_particle.png");
            _iTex = RC.CreateTexture(imgData);

        }

        // is called once a frame
        public override void RenderAFrame()
        {

            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            // move per mouse
            if (Input.Instance.IsButton(MouseButtons.Left))
            {
                _angleVelHorz = RotationSpeed * Input.Instance.GetAxis(InputAxis.MouseX);
                _angleVelVert = RotationSpeed * Input.Instance.GetAxis(InputAxis.MouseY);
            }
            else
            {
                var curDamp = (float)Math.Exp(-Damping * Time.Instance.DeltaTime);

                _angleVelHorz *= curDamp;
                _angleVelVert *= curDamp;
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            // move per keyboard
            if (Input.Instance.IsKey(KeyCodes.Left))
                _angleHorz -= RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKey(KeyCodes.Right))
                _angleHorz += RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKey(KeyCodes.Up))
                _angleVert -= RotationSpeed * (float)Time.Instance.DeltaTime;

            if (Input.Instance.IsKey(KeyCodes.Down))
                _angleVert += RotationSpeed * (float)Time.Instance.DeltaTime;

            var mtxRot = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(_angleVert);
            var mtxCam = float4x4.LookAt(0, 200, 500, 0, 0, 0, 0, 1, 0);

            // first mesh
            RC.ModelView = float4x4.CreateTranslation(0, -50, 0)*mtxRot*float4x4.CreateTranslation(-150, 0, 0)*mtxCam;
            RC.ModelView = new float4x4(15, 0, 0, 0, 0, 15, 0, 0, 0, 0, 15, 0, 0, 0, 0, 1) * mtxRot * float4x4.CreateTranslation(-150, 0, 0) * mtxCam;

            RC.SetShader(_spTexture);
            RC.SetShaderParamTexture(_textureParam, _iTex);

            _particleEmitter.Tick(Time.Instance.DeltaTime);
            
            RC.Render(_particleEmitter.ParticleMesh);
            

            // second mesh
            RC.ModelView = mtxRot * float4x4.CreateTranslation(150, 0, 0) * mtxCam;

            RC.SetShader(_spColor);
            //RC.SetShaderParamTexture(_textureParam, _iTex);
            RC.SetShaderParam(_colorParam, new float4(1, 1, 1, 1));
            RC.Render(_meshFace);


            // swap buffers
            Present();
        }

        // is called when the window was resized
        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 5000);
        }

        public static void Main()
        {
            var app = new ParticleSystem();
            app.Run();
        }
    }
}
