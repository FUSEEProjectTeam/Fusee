using System;
using System.Diagnostics;
using System.Net.Mime;
using System.Runtime.InteropServices;
using Fusee.Engine;
using Fusee.SceneManagement;
using Fusee.Math;

namespace Examples.TextDemo
{
    public class TextDemo : RenderCanvas
    {
        private IFont _fontCabin12;
        private IFont _fontCabin20;
        private IFont _fontCabin30;

        private GUIText _textMeshCabin20;
        private GUIText _textMeshCabin30;

        private Mesh _mesh;
        private IShaderParam _vColor;

        private static float _angleHorz;

        private GUIButton _testButton1;
        private GUIButton _testButton2;

        private bool _showDebug;

        public override void Init()
        {
            _showDebug = false;

            RC.ClearColor = new float4(0.5f, 0.5f, 0.8f, 1);

            // load fonts
            _fontCabin12 = RC.LoadFont("Assets/Cabin.ttf", 15);
            _fontCabin20 = RC.LoadFont("Assets/Cabin.ttf", 20);
            _fontCabin30 = RC.LoadFont("Assets/Cabin.ttf", 30);

            // button 1
            _testButton1 = new GUIButton(RC, "Exit", _fontCabin12, 10, 10, 100, 25)
            {
                ButtonColor = new float4(0.7f, 0.7f, 0.7f, 1),
                TextColor = new float4(0, 0, 0, 1),
                BorderWidth = 1,
                BorderColor = new float4(0, 0, 0, 1)
            };

            _testButton1.Refresh();

            _testButton1.OnGUIButtonDown += OnGUIButtonDown;
            _testButton1.OnGUIButtonUp += OnGUIButtonUp;
            _testButton1.OnGUIButtonEnter += OnGUIButtonEnter;
            _testButton1.OnGUIButtonLeave += OnGUIButtonLeave;

            // button 2
            _testButton2 = new GUIButton(RC, "Debug", _fontCabin12, 10, 40, 100, 25)
            {
                ButtonColor = new float4(0.7f, 0.7f, 0.7f, 1),
                TextColor = new float4(0, 0, 0, 1),
                BorderWidth = 1,
                BorderColor = new float4(0, 0, 0, 1)
            };
            
            _testButton2.Refresh();

            _testButton2.OnGUIButtonDown += OnGUIButtonDown;
            _testButton2.OnGUIButtonUp += OnGUIButtonUp;
            _testButton2.OnGUIButtonEnter += OnGUIButtonEnter;
            _testButton2.OnGUIButtonLeave += OnGUIButtonLeave;

            // text
            _textMeshCabin20 = new GUIText(RC, "The quick brown fox jumps over the lazy dog.", _fontCabin20, 8, 100)
            {
                TextColor = new float4(1, 1, 1, 1)
            };

            _textMeshCabin20.Refresh();

            _textMeshCabin30 = new GUIText(RC, "The quick brown fox jumps over the lazy dog.", _fontCabin30, 8, 140)
            {
                TextColor = new float4(0, 0, 0, 0.5f)
            };

            _textMeshCabin30.Refresh();

            // dummy cube
            _mesh = new Cube();

            var sp = MoreShaders.GetDiffuseColorShader(RC);
            RC.SetShader(sp);

            _vColor = sp.GetShaderParam("color");
            RC.SetShaderParam(_vColor, new float4(1, 1, 1, 1));
            
            _angleHorz = 0;
        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // dummy cube
            _angleHorz += 0.002f;

            var mtxRot = float4x4.CreateRotationY(_angleHorz) * float4x4.CreateRotationX(0);
            var mtxCam = float4x4.LookAt(0, 100, 200, 0, 0, 0, 0, 1, 0);

            RC.ModelView = float4x4.Scale(100, 100, 100) * mtxRot * float4x4.CreateTranslation(-60, 0, 0) * mtxCam;
            RC.SetShaderParam(_vColor, new float4(0.8f, 0.1f, 0.1f, 1));
            RC.Render(_mesh);

            // button
            RC.TextOut(_testButton1.GUIMesh, _fontCabin12);
            RC.TextOut(_testButton2.GUIMesh, _fontCabin12);

            if (_showDebug)
            {
                // text
                RC.TextOut(_textMeshCabin20.GUIMesh, _fontCabin20);
                RC.TextOut(_textMeshCabin30.GUIMesh, _fontCabin30);

                // text examples: dynamic text
                var col6 = new float4(0, 1, 1, 1);
                RC.TextOut("Framerate: " + Time.Instance.FramePerSecondSmooth + "fps", _fontCabin20, col6, 8, 210);
                RC.TextOut("Time: " + Math.Round(Time.Instance.TimeSinceStart, 1) + " sec", _fontCabin20, col6, 8, 250);
            }

            Present();
        }

        private void OnGUIButtonDown(GUIButton sender, MouseEventArgs mea)
        {
            sender.BorderWidth = 2;
            sender.Refresh();
        }

        private void OnGUIButtonUp(GUIButton sender, MouseEventArgs mea)
        {
            sender.BorderWidth = 1;
            sender.Refresh();

            if (mea.Button == MouseButtons.Left)
            {
                if (sender == _testButton1)
                    Environment.Exit(0);

                if (sender == _testButton2)
                    _showDebug = !_showDebug;
            }
        }

        private static void OnGUIButtonEnter(GUIButton sender, MouseEventArgs mea)
        {
            if (Input.Instance.IsButton(MouseButtons.Left))
                sender.BorderWidth = 2;

            sender.TextColor = new float4(0.8f, 0.1f, 0.1f, 1);
            sender.Refresh();
        }

        private static void OnGUIButtonLeave(GUIButton sender, MouseEventArgs mea)
        {
            sender.BorderWidth = 1;
            sender.TextColor = new float4(0f, 0f, 0f, 1);
            sender.Refresh();
        }

        public override void Resize()
        {
            // is called when the window is resized
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);

            _testButton1.Refresh();
            _testButton2.Refresh();
        }

        public static void Main()
        {
            var app = new TextDemo();
            app.Run();
        }
    }
}
