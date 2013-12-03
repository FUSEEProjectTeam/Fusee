using System;
using Fusee.Engine;
using Fusee.Math;


namespace Examples.ParticleSystem
{
    [FuseeApplication(Name = "Particle System", Description = "A very simple example.")]
    public class ParticleSystem : RenderCanvas
    {
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

            _spColor = MoreShaders.GetShader("simple", RC);
            _spTexture = MoreShaders.GetShader("texture", RC);

            _colorParam = _spColor.GetShaderParam("vColor");
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

            RC.SetShader(_spTexture);
            RC.SetShaderParamTexture(_textureParam, _iTex);

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
