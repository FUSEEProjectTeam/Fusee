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

        //particelcount,minLife, maxLife,minSize, maxSize, rotation, transparency, randPosX,randPosY,randPosY,randVelX,randVelY,randVelZ,gravityX, gravityY, gravityZ,
        private ParticleEmitter _smokeEmitter       = new ParticleEmitter(0, 150, 600, 1.0f, 1.0f, 0.012f, 0.4f, 0.0f, 0.0f, 0.0f, 0.0f, 5.0f, 0.0f, 0.0f, 0.02f, 0.0f);
        private ParticleEmitter _fireRedEmitter     = new ParticleEmitter(0, 150, 600, 1.0f, 1.0f, 0.012f, 0.4f, 0.0f, 0.0f, 0.0f, 0.0f, 5.0f, 0.0f, 0.0f, 0.02f, 0.0f);
        private ParticleEmitter _fireYellowEmitter  = new ParticleEmitter(0, 150, 600, 1.0f, 1.0f, 0.012f, 0.4f, 0.0f, 0.0f, 0.0f, 0.0f, 5.0f, 0.0f, 0.0f, 0.02f, 0.0f);
        private ParticleEmitter _starEmitter        = new ParticleEmitter(0, 150, 600, 1.0f, 1.0f, 0.012f, 0.4f, 0.0f, 0.0f, 0.0f, 0.0f, 5.0f, 0.0f, 0.0f, 0.02f, 0.0f);

        // variables for shader
        private ShaderProgram _smokeTexture;
        private ShaderProgram _fireRedTexture;
        private ShaderProgram _fireYellowTexture;
        private ShaderProgram _starTexture;

        private IShaderParam _smokeParam;
        private IShaderParam _fireRedParam;
        private IShaderParam _fireYellowParam;
        private IShaderParam _starParam;

        private ITexture _iSmoke;
        private ITexture _iFireRed;
        private ITexture _iFireYellow;
        private ITexture _iStar;

        private IFont _guiFontCabin18;
        private IFont _guiFontCabin24;
        private GUIHandler _guiHandler;

        private GUIPanel _guiPanel;

        private GUIButton _guiExampleOneButton;
        private GUIButton _guiExampleTwoButton;
        private GUIButton _guiExampleThreeButton;
        private GUIText _guiText;

        // is called on startup
        public override void Init()
        {
            RC.ClearColor = new float4(0.2f, 0.2f, 0.2f, 1);

            _smokeTexture           = RC.CreateShader(_smokeEmitter.VsSimpleTexture, _smokeEmitter.PsSimpleTexture);
            _fireRedTexture         = RC.CreateShader(_fireRedEmitter.VsSimpleTexture, _fireRedEmitter.PsSimpleTexture);
            _fireYellowTexture      = RC.CreateShader(_fireYellowEmitter.VsSimpleTexture, _fireYellowEmitter.PsSimpleTexture);
            _starTexture            = RC.CreateShader(_starEmitter.VsSimpleTexture, _starEmitter.PsSimpleTexture);

            _smokeParam             = _smokeTexture.GetShaderParam("texture1");
            _fireRedParam           = _fireRedTexture.GetShaderParam("texture1");
            _fireYellowParam        = _fireYellowTexture.GetShaderParam("texture1");
            _starParam              = _starTexture.GetShaderParam("texture1");

            // load texture
            var imgSmokeData        = RC.LoadImage("Assets/smoke_particle.png");
            var imgFireRedData      = RC.LoadImage("Assets/fireRed.png");
            var imgFireYellowData   = RC.LoadImage("Assets/fireYellowTexture.png");
            var imgStarData         = RC.LoadImage("Assets/star.png");

            _iSmoke                 = RC.CreateTexture(imgSmokeData);
            _iFireRed               = RC.CreateTexture(imgFireRedData);
            _iFireYellow            = RC.CreateTexture(imgFireYellowData);
            _iStar                  = RC.CreateTexture(imgStarData);

            RC.SetRenderState(new RenderStateSet
            {
                ZEnable = false,
                AlphaBlendEnable = true,
                BlendOperation = BlendOperation.Add,
                SourceBlend = Blend.SourceAlpha,
                DestinationBlend = Blend.InverseSourceAlpha
            });

            // GUIHandler
            _guiHandler = new GUIHandler();
            _guiHandler.AttachToContext(RC);

            // font + text
            _guiFontCabin18 = RC.LoadFont("Assets/Cabin.ttf", 18);
            _guiFontCabin24 = RC.LoadFont("Assets/Cabin.ttf", 24);

            _guiText = new GUIText("Fusee3D Particle System ", _guiFontCabin24, 510, 35);
            _guiText.TextColor = new float4(1, 1, 1, 1);

            _guiHandler.Add(_guiText);

            // panel
            _guiPanel = new GUIPanel("Menu", _guiFontCabin18, 10, 10, 150, 150);
            _guiHandler.Add(_guiPanel);

            // Example 1 button
            _guiExampleOneButton = new GUIButton("Fog", _guiFontCabin18, 25, 40, 100, 25);

            _guiExampleOneButton.OnGUIButtonDown += OnMenuButtonDown;
            _guiExampleOneButton.OnGUIButtonUp += OnMenuButtonUp;
            _guiExampleOneButton.OnGUIButtonEnter += OnMenuButtonEnter;
            _guiExampleOneButton.OnGUIButtonLeave += OnMenuButtonLeave;

            _guiPanel.ChildElements.Add(_guiExampleOneButton);

            // Example 2 button
            _guiExampleTwoButton = new GUIButton("Fire", _guiFontCabin18, 25, 70, 100, 25);

            _guiExampleTwoButton.OnGUIButtonDown += OnMenuButtonDown;
            _guiExampleTwoButton.OnGUIButtonUp += OnMenuButtonUp;
            _guiExampleTwoButton.OnGUIButtonEnter += OnMenuButtonEnter;
            _guiExampleTwoButton.OnGUIButtonLeave += OnMenuButtonLeave;

            _guiPanel.ChildElements.Add(_guiExampleTwoButton);

            // Example 2 button
            _guiExampleThreeButton = new GUIButton("Stars", _guiFontCabin18, 25, 100, 100, 25);

            _guiExampleThreeButton.OnGUIButtonDown += OnMenuButtonDown;
            _guiExampleThreeButton.OnGUIButtonUp += OnMenuButtonUp;
            _guiExampleThreeButton.OnGUIButtonEnter += OnMenuButtonEnter;
            _guiExampleThreeButton.OnGUIButtonLeave += OnMenuButtonLeave;

            _guiPanel.ChildElements.Add(_guiExampleThreeButton);
        }

        private void OnMenuButtonDown(GUIButton sender, MouseEventArgs mea)
        {
            if (sender == _guiExampleOneButton)
            {
                //particelcount,minLife, maxLife,minSize, maxSize,transparency, randPosX,randPosY,randPosY,randVelX,randVelY,randVelZ,gravityX, gravityY, gravityZ,
                _smokeEmitter               = new ParticleEmitter(10000, 999999, 999999, 0.9f, 1.0f, 0.012f, 0.01f, 50.6f, 1.6f, 50.6f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
                //deactivate other emitter
                _fireYellowEmitter          = null;
                _fireRedEmitter             = null;
                _starEmitter                = null;
                 

            }

            if (sender == _guiExampleTwoButton)
            {
                //particelcount,minLife, maxLife,minSize, maxSize, rotation, transparency, randPosX,randPosY,randPosY,randVelX,randVelY,randVelZ,gravityX, gravityY, gravityZ,
                _smokeEmitter               = new ParticleEmitter(600, 0, 200, 0.4f, 0.5f, 0.012f, 0.2f, 0.5f, 1.0f, 0.5f, 1.0f, 1.5f, 1.0f, 0.0f, -0.05f, 0.0f);
                _fireYellowEmitter          = new ParticleEmitter(150, 5, 200, 0.5f, 0.9f, 0.012f, 0.1f, 1.0f, 0.5f, 1.0f, 0.025f, 0.0f, 0.025f, 0.0f, -0.03f, 0.0f);
                _fireRedEmitter             = new ParticleEmitter(450, 0, 200, 0.3f, 0.6f, 0.012f, 0.4f, 0.5f, 0.1f, 0.5f, 0.4f, 1.5f, 0.4f, 0.0f, -0.03f, 0.0f);
                
                //deactivate other emitter
                _starEmitter                = null;
            }

            if (sender == _guiExampleThreeButton)
            {
                //particelcount,minLife, maxLife,minSize, maxSize,rotation,transparency, randPosX,randPosY,randPosY,randVelX,randVelY,randVelZ,gravityX, gravityY, gravityZ,                           
                _starEmitter                = new ParticleEmitter(200, 600, 600, 0.2f, 0.2f, 0.012f, 1.0f, 0.5f, 0.0f, 0.5f, 8.2f, 8.0f, 8.2f, 0.0f, 0.032f, 0.0f);
                //deactivate other emitter
                _smokeEmitter               = null;
                _fireYellowEmitter          = null;
                _fireRedEmitter             = null;
            }
        }

        private void OnMenuButtonUp(GUIButton sender, MouseEventArgs mea)
        {
            sender.BorderWidth = 1;
        }

        private static void OnMenuButtonEnter(GUIButton sender, MouseEventArgs mea)
        {
            sender.TextColor = new float4(0.8f, 0.1f, 0.1f, 1);
        }

        private static void OnMenuButtonLeave(GUIButton sender, MouseEventArgs mea)
        {
            sender.TextColor = new float4(0f, 0f, 0f, 1);
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




            // mesh
            RC.ModelView = float4x4.CreateTranslation(0, -50, 0) * mtxRot * float4x4.CreateTranslation(-150, 0, 0) * mtxCam;
            RC.ModelView = new float4x4(15, 0, 0, 0, 0, 15, 0, 0, 0, 0, 15, 0, 0, 0, 0, 1) * mtxRot * float4x4.CreateTranslation(0, 20, 0) * mtxCam;

            //smoke
            if (_smokeEmitter != null)
            {
                RC.SetShader(_smokeTexture);
                RC.SetShaderParamTexture(_smokeParam, _iSmoke);
                _smokeEmitter.Tick(Time.Instance.DeltaTime);
                RC.Render(_smokeEmitter.ParticleMesh);
            }

            //fireRed
            if (_fireRedEmitter != null)
            {
                RC.SetShader(_fireRedTexture);
                RC.SetShaderParamTexture(_fireRedParam, _iFireRed);
                _fireRedEmitter.Tick(Time.Instance.DeltaTime);
                RC.Render(_fireRedEmitter.ParticleMesh);
            }
            // mesh
            RC.ModelView = float4x4.CreateTranslation(0, -50, 0) * mtxRot * float4x4.CreateTranslation(-150, 0, 0) * mtxCam;
            RC.ModelView = new float4x4(15, 0, 0, 0, 0, 15, 0, 0, 0, 0, 15, 0, 0, 0, 0, 1) * mtxRot * float4x4.CreateTranslation(0, 0, 0) * mtxCam;

            //star
            if (_starEmitter != null)
            {
                RC.SetShader(_starTexture);
                RC.SetShaderParamTexture(_starParam, _iStar);
                _starEmitter.Tick(Time.Instance.DeltaTime);
                RC.Render(_starEmitter.ParticleMesh);
            }



            //fireYellow
            if (_fireYellowEmitter != null)
            {
                RC.SetShader(_fireYellowTexture);
                RC.SetShaderParamTexture(_fireYellowParam, _iFireYellow);
                _fireYellowEmitter.Tick(Time.Instance.DeltaTime);
                RC.Render(_fireYellowEmitter.ParticleMesh);
            }



            _guiHandler.RenderGUI();

            // swap buffers
            Present();
        }

        // is called when the window was resized
        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 5000);
            // refresh all elements
            _guiHandler.Refresh();
        }

        public static void Main()
        {
            var app = new ParticleSystem();
            app.Run();
        }
    }
}
