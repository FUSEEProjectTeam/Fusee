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
        private IShaderParam _alphaParam;

        private ITexture _iTex;

        private GUIButton[] _guiBDiffs;
        private IFont _guiFontCabin18;
        private IFont _guiFontCabin24;
        private GUIHandler _guiHandler;

        private GUIImage _guiImage;

        private GUIPanel _guiPanel;

        private GUIButton _guiExampleOneButton;
        private GUIButton _guiExampleTwoButton;
        private GUIButton _guiExampleThreeButton;
        private GUIText _guiText;

        private GUIButton[] _guiUDiffs;

        // is called on startup
        public override void Init()
        {
            RC.ClearColor = new float4(0.7f, 0.7f, 1, 1);
            //particelcount,minLife, maxLife,minSize, maxSize,randPosX,randPosY,randPosY,randVelX,randVelY,randVelZ,gravityX, gravityY, gravityZ,
            _particleEmitter = new ParticleEmitter(100, 200, 580, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 5.0f, 5.0f, 0.0f, 0.0f, 0.01f, 0.0f);
            // initialize the variables
            //_meshTea = new ParticleEmitter();//MeshReader.LoadMesh(@"Assets/Teapot.obj.model");
            _meshFace = MeshReader.LoadMesh(@"Assets/Face.obj.model");

            _spColor = MoreShaders.GetDiffuseColorShader(RC);
            _spTexture = RC.CreateShader(_particleEmitter.VsSimpleTexture, _particleEmitter.PsSimpleTexture);

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
            // GUIHandler
            _guiHandler = new GUIHandler();
            _guiHandler.AttachToContext(RC);

            // font + text
            _guiFontCabin18 = RC.LoadFont("Assets/Cabin.ttf", 18);
            _guiFontCabin24 = RC.LoadFont("Assets/Cabin.ttf", 24);

            _guiText = new GUIText("Fusee3D Particle System ", _guiFontCabin24, 310, 35);
            _guiText.TextColor = new float4(1, 1, 1, 1);

            _guiHandler.Add(_guiText);
            // buttons / rectangles
            _guiUDiffs = new GUIButton[7];
            _guiBDiffs = new GUIButton[7];


            // panel
            _guiPanel = new GUIPanel("Menu", _guiFontCabin18, 10, 10, 150, 150);
            _guiHandler.Add(_guiPanel);

            // Example 1 button
            _guiExampleOneButton = new GUIButton("Example 1", _guiFontCabin18, 25, 40, 100, 25);

            _guiExampleOneButton.OnGUIButtonDown += OnMenuButtonDown;
            _guiExampleOneButton.OnGUIButtonUp += OnMenuButtonUp;
            _guiExampleOneButton.OnGUIButtonEnter += OnMenuButtonEnter;
            _guiExampleOneButton.OnGUIButtonLeave += OnMenuButtonLeave;

            _guiPanel.ChildElements.Add(_guiExampleOneButton);

            // Example 2 button
            _guiExampleTwoButton = new GUIButton("Example 2", _guiFontCabin18, 25, 70, 100, 25);

            _guiExampleTwoButton.OnGUIButtonDown += OnMenuButtonDown;
            _guiExampleTwoButton.OnGUIButtonUp += OnMenuButtonUp;
            _guiExampleTwoButton.OnGUIButtonEnter += OnMenuButtonEnter;
            _guiExampleTwoButton.OnGUIButtonLeave += OnMenuButtonLeave;

            _guiPanel.ChildElements.Add(_guiExampleTwoButton);

            // Example 2 button
            _guiExampleThreeButton = new GUIButton("Example 3", _guiFontCabin18, 25, 100, 100, 25);

            _guiExampleThreeButton.OnGUIButtonDown += OnMenuButtonDown;
            _guiExampleThreeButton.OnGUIButtonUp += OnMenuButtonUp;
            _guiExampleThreeButton.OnGUIButtonEnter += OnMenuButtonEnter;
            _guiExampleThreeButton.OnGUIButtonLeave += OnMenuButtonLeave;

            _guiPanel.ChildElements.Add(_guiExampleThreeButton);

        }

        private void OnDiffButtonDown(GUIButton sender, MouseEventArgs mea)
        {
            /*sender.BorderWidth = 2;*/

            var guiButton = sender.Tag as GUIButton;
            if (guiButton != null) guiButton.BorderWidth = 2;
        }

        private void OnMenuButtonDown(GUIButton sender, MouseEventArgs mea)
        {
            if (sender == _guiExampleOneButton)
            {
                _particleEmitter = new ParticleEmitter(100, 150, 600, 1.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 5.0f, 0.0f, 0.0f, 0.02f, 0.0f);
                Console.WriteLine("Yeah Boy");
            }
            if (sender == _guiExampleTwoButton)
            {
                _particleEmitter = new ParticleEmitter(100, 150, 600, 0.2f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 5.0f, 0.0f, 0.0f, 0.02f, 0.0f);
                Console.WriteLine("Yeah Girl");
            }
            if (sender == _guiExampleThreeButton)
            {
                _particleEmitter = new ParticleEmitter(100, 150, 600, 0.6f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 5.0f, 0.0f, 0.0f, 0.02f, 0.0f);
                Console.WriteLine("Yolo");
            }
            sender.BorderWidth = 2;
        }

        private void OnMenuButtonUp(GUIButton sender, MouseEventArgs mea)
        {
            sender.BorderWidth = 1;

          /*  var bWidth = (sender == _guiSolveButton) ? 2 : 0;

            foreach (var guiButton in _guiUDiffs)
                guiButton.BorderWidth = bWidth;
            foreach (var guiButton in _guiBDiffs)
                guiButton.BorderWidth = bWidth;*/
        }

        private static void OnMenuButtonEnter(GUIButton sender, MouseEventArgs mea)
        {
           /* if (Input.Instance.IsButton(MouseButtons.Left))
                sender.BorderWidth = 2;*/

            sender.TextColor = new float4(0.8f, 0.1f, 0.1f, 1);
        }

        private static void OnMenuButtonLeave(GUIButton sender, MouseEventArgs mea)
        {
           /* sender.BorderWidth = 1;*/
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

            // first mesh
            RC.ModelView = float4x4.CreateTranslation(0, -50, 0) * mtxRot * float4x4.CreateTranslation(-150, 0, 0) * mtxCam;
            RC.ModelView = new float4x4(15, 0, 0, 0, 0, 15, 0, 0, 0, 0, 15, 0, 0, 0, 0, 1) * mtxRot * float4x4.CreateTranslation(-150, 0, 0) * mtxCam;

            RC.SetShader(_spTexture);
            RC.SetShaderParamTexture(_textureParam, _iTex);

            _particleEmitter.Tick(Time.Instance.DeltaTime);
            // _particleEmitter2.Tick(Time.Instance.DeltaTime);
            RC.Render(_particleEmitter.ParticleMesh);
            // RC.Render(_particleEmitter2.ParticleMesh);

            _guiHandler.RenderGUI();
            // second mesh
            RC.ModelView = mtxRot * float4x4.CreateTranslation(150, 0, 0) * mtxCam;
            RC.ModelView = new float4x4(15, 0, 0, 0, 0, 15, 0, 0, 0, 0, 15, 0, 0, 0, 0, 1) * mtxRot * float4x4.CreateTranslation(150, 0, 0) * mtxCam;
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
