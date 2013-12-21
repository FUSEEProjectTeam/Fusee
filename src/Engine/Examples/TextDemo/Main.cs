using System;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.TextDemo
{
    public class TextDemo : RenderCanvas
    {
        // GUI Elements
        private IFont _fontCabin12;
        private IFont _fontCabin20;
        private IFont _fontCabin30;

        private GUIText _guiText1;
        private GUIText _guiText2;

        private GUIButton _guiButton1;
        private GUIButton _guiButton2;

        private GUIPanel _guiPanel;

        // Cube
        private Mesh _mesh;

        private ShaderProgram _shaderProgram;
        private IShaderParam _vColor;

        private static float _angleHorz;

        public override void Init()
        {
            RC.ClearColor = new float4(0.5f, 0.5f, 0.8f, 1);

            // load fonts
            _fontCabin12 = RC.LoadFont("Assets/Cabin.ttf", 15);
            _fontCabin20 = RC.LoadFont("Assets/Cabin.ttf", 20);
            _fontCabin30 = RC.LoadFont("Assets/Cabin.ttf", 30);

            // panel
            _guiPanel = new GUIPanel(RC, "Menü", _fontCabin12, 10, 10, 150, 110);

            GUIHandler.GUIElements.Add(_guiPanel);

            // button 1
            _guiButton1 = new GUIButton(RC, "Exit", _fontCabin12, 25, 40, 100, 25)
            {
                ButtonColor = new float4(0.7f, 0.7f, 0.7f, 1),
                TextColor = new float4(0, 0, 0, 1),
                BorderWidth = 1,
                BorderColor = new float4(0, 0, 0, 1)
            };

            _guiButton1.OnGUIButtonDown += OnGUIButtonDown;
            _guiButton1.OnGUIButtonUp += OnGUIButtonUp;
            _guiButton1.OnGUIButtonEnter += OnGUIButtonEnter;
            _guiButton1.OnGUIButtonLeave += OnGUIButtonLeave;

            _guiPanel.ChildElements.Add(_guiButton1);

            // button 2
            _guiButton2 = new GUIButton(RC, "Debug", _fontCabin12, 25, 70, 100, 25)
            {
                ButtonColor = new float4(0.7f, 0.7f, 0.7f, 1),
                TextColor = new float4(0, 0, 0, 1),
                BorderWidth = 1,
                BorderColor = new float4(0, 0, 0, 1)
            };

            _guiButton2.OnGUIButtonDown += OnGUIButtonDown;
            _guiButton2.OnGUIButtonUp += OnGUIButtonUp;
            _guiButton2.OnGUIButtonEnter += OnGUIButtonEnter;
            _guiButton2.OnGUIButtonLeave += OnGUIButtonLeave;

            _guiPanel.ChildElements.Add(_guiButton2);

            // text
            _guiText1 = new GUIText(RC, "The quick brown fox jumps over the lazy dog.", _fontCabin20, 8, 150)
            {
                TextColor = new float4(1, 1, 1, 1)
            };

            _guiText2 = new GUIText(RC, "The quick brown fox jumps over the lazy dog.", _fontCabin30, 8, 190)
            {
                TextColor = new float4(0, 0, 0, 0.5f)
            };

            // dummy cube
            _mesh = new Cube();

            _shaderProgram = MoreShaders.GetDiffuseColorShader(RC);
            RC.SetShader(_shaderProgram);

            _vColor = _shaderProgram.GetShaderParam("color");
            RC.SetShaderParam(_vColor, new float4(1, 1, 1, 1));

            _angleHorz = 0;
        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // dummy cube
            RC.SetShader(_shaderProgram);           
            RC.SetRenderState(new RenderStateSet
            {
                AlphaBlendEnable = false,
                ZEnable = true
            });

            _angleHorz += 0.002f;

            var mtxRot = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(0);
            var mtxCam = float4x4.LookAt(0, 100, 200, 0, 0, 0, 0, 1, 0);

            RC.ModelView = float4x4.Scale(100, 100, 100) * mtxRot * float4x4.CreateTranslation(-60, 0, 0) * mtxCam;
            RC.SetShaderParam(_vColor, new float4(0.8f, 0.1f, 0.1f, 1));
            RC.Render(_mesh);

            // GUI
            GUIHandler.RenderGUI();

            Present();
        }

        private void OnGUIButtonDown(GUIButton sender, MouseEventArgs mea)
        {
            sender.BorderWidth = 2;
        }

        private void OnGUIButtonUp(GUIButton sender, MouseEventArgs mea)
        {
            sender.BorderWidth = 1;

            if (mea.Button == MouseButtons.Left)
            {
                if (sender == _guiButton1)
                    Environment.Exit(0);

                if (sender == _guiButton2)
                    if (!GUIHandler.GUIElements.Contains(_guiText1))
                    {
                        GUIHandler.GUIElements.Add(_guiText2);
                        GUIHandler.GUIElements.Add(_guiText1);
                    }
                    else
                    {
                        GUIHandler.GUIElements.Remove(_guiText2);
                        GUIHandler.GUIElements.Remove(_guiText1);
                    }
            }
        }

        private static void OnGUIButtonEnter(GUIButton sender, MouseEventArgs mea)
        {
            if (Input.Instance.IsButton(MouseButtons.Left))
                sender.BorderWidth = 2;

            sender.TextColor = new float4(0.8f, 0.1f, 0.1f, 1);
        }

        private static void OnGUIButtonLeave(GUIButton sender, MouseEventArgs mea)
        {
            sender.BorderWidth = 1;
            sender.TextColor = new float4(0f, 0f, 0f, 1);
        }

        public override void Resize()
        {
            // is called when the window is resized
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);

            _guiPanel.Refresh();
            _guiText1.Refresh();
            _guiText2.Refresh();
        }

        public static void Main()
        {
            var app = new TextDemo();
            app.Run();
        }
    }
}
