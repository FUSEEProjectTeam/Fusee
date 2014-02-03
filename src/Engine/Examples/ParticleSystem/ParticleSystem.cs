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
            uniform mat4 FUSEE_MV;
            uniform mat4 FUSEE_P;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                float PI = 3.14159265358979323846264;
                float angle = 125.0;
                float rad_angle = angle*PI/180.0;

                vec4 vPos = FUSEE_MV * vec4(fuVertex, 1.0);
                vPos = vPos + 100.0*vec4(fuUV, 0, 0);                
                gl_Position = FUSEE_P * vPos;
                vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
                vUV.x = (fuUV.x != 0.0) ? 1.0 : 0.0;
                vUV.y = (fuUV.y != 0.0) ? 1.0 : 0.0;

                //vPos.x  = vPos.x*cos(rad_angle) - vPos.y*sin(rad_angle);
               // vPos.y = vPos.y*cos(rad_angle) + vPos.x*sin(rad_angle);
            }";

        public string PsSimpleTexture = @"
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

           const vec4 AlphaColor = vec4(1.0, 1.0, 1.0, 0.4);


            void main()
            {    
              // The most basic texturing function, expecting the above mentioned parameters  
              // max(dot(vec3(0,0,1),normalize(vNormal)), 0.1) 
              gl_FragColor = texture2D(texture1, vUV)*AlphaColor;        
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
        private ParticleEmitter _particleEmitter2;
        // variables for shader
        private ShaderProgram _spColor;
        private ShaderProgram _spTexture;

        private IShaderParam _colorParam;
        private IShaderParam _textureParam;
        private IShaderParam _alphaParam;

        private ITexture _iTex;
        private float alp = 0.2f;

        // is called on startup
        public override void Init()
        {
            RC.ClearColor = new float4(0.7f, 0.7f, 1, 1);

            //_particleEmitter = new ParticleEmitter(500, 200, 700, 1.0f, 5.0f, 5.0f, 5.0f, 10.0, 10.0, 10.0, 0.0f, -8.5f, 0.0f);
            _particleEmitter = new ParticleEmitter(705, 800, 1200, 1.0f, 4.0f, 4.0f, 4.0f, 0.0f, 0.0f, 5.0f, 0.0f, 0.0f,
                0.0f);
            //_particleEmitter2 = new ParticleEmitter(1200, 100000, 100000, 1.0f, 30.0f, 1.0f, 30.0f, 0.1, 0.0, 0.1, 0.0f, 0.0f, 0.0f);
            // initialize the variables
            //_meshTea = new ParticleEmitter();//MeshReader.LoadMesh(@"Assets/Teapot.obj.model");
            _meshFace = MeshReader.LoadMesh(@"Assets/Face.obj.model");

            _spColor = MoreShaders.GetDiffuseColorShader(RC);
            _spTexture = RC.CreateShader(VsSimpleTexture, PsSimpleTexture);

            _colorParam = _spColor.GetShaderParam("color");
            _textureParam = _spTexture.GetShaderParam("texture1");
            _alphaParam = _spTexture.GetShaderParam("alpha1");

            // load texture
            //var imgData = RC.LoadImage("Assets/world_map.jpg");
            var imgData = RC.LoadImage("Assets/smoke_particle.png");
            _iTex = RC.CreateTexture(imgData);


            RC.SetRenderState(new RenderStateSet
            {
                ZEnable = false,
                AlphaBlendEnable = true,
                //BlendFactor = new float4(0.5f, 0.5f, 0.5f, 0.5f),
                BlendOperation = BlendOperation.Add,
                //SourceBlend = Blend.BlendFactor,
                //DestinationBlend = Blend.InverseBlendFactor
                SourceBlend = Blend.SourceAlpha,
                DestinationBlend = Blend.InverseSourceAlpha
            });
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

            var mtxRot = float4x4.CreateRotationY(_angleHorz)*float4x4.CreateRotationX(_angleVert);
            var mtxCam = float4x4.LookAt(0, 200, 500, 0, 0, 0, 0, 1, 0);

            // first mesh
            RC.ModelView = float4x4.CreateTranslation(0, -50, 0)*mtxRot*float4x4.CreateTranslation(-150, 0, 0)*mtxCam;
            RC.ModelView = new float4x4(15, 0, 0, 0, 0, 15, 0, 0, 0, 0, 15, 0, 0, 0, 0, 1)*mtxRot*
                           float4x4.CreateTranslation(-150, 0, 0)*mtxCam;

            RC.SetShader(_spTexture);
            RC.SetShaderParamTexture(_textureParam, _iTex);

            _particleEmitter.Tick(Time.Instance.DeltaTime);
            // _particleEmitter2.Tick(Time.Instance.DeltaTime);
            RC.Render(_particleEmitter.ParticleMesh);
            // RC.Render(_particleEmitter2.ParticleMesh);


            // second mesh
            RC.ModelView = mtxRot*float4x4.CreateTranslation(150, 0, 0)*mtxCam;
            RC.ModelView = new float4x4(15, 0, 0, 0, 0, 15, 0, 0, 0, 0, 15, 0, 0, 0, 0, 1)*mtxRot*
                           float4x4.CreateTranslation(150, 0, 0)*mtxCam;
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

            var aspectRatio = Width/(float) Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 5000);
        }

        public static void Main()
        {
            var app = new ParticleSystem();
            app.Run();
        }
    }
}