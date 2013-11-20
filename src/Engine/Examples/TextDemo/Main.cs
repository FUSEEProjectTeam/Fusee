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
        private IFont _fontCousine20;

        private Mesh _textMeshCabin12;
        private Mesh _textMeshCabin20;
        private Mesh _textMeshCabin30;
        private Mesh _textMeshCabin30N;
        private Mesh _textMeshCabin30K;
        private Mesh _textMeshCousine20;

        private Mesh _mesh;
        private IShaderParam _vColor;

        private static float _angleHorz;

        public override void Init()
        {
            RC.ClearColor = new float4(0.2f, 0.2f, 0.5f, 1);

            // load fonts
            _fontCousine20 = RC.LoadFont("Assets/Cousine.ttf", 20);
            _fontCabin12 = RC.LoadFont("Assets/Cabin.ttf", 15);
            _fontCabin20 = RC.LoadFont("Assets/Cabin.ttf", 20);
            _fontCabin30 = RC.LoadFont("Assets/Cabin.ttf", 30);

            // get text as mesh
            var buttonColor = new float4(0.5f, 0.5f, 0.5f, 1);
            var textColor = new float4(0, 0, 0, 1);

            _textMeshCabin12 = RC.GetButton("Button", _fontCabin12, 10, 10, 100, 25, buttonColor, textColor);

           // _textMeshCousine20 = RC.GetTextMesh("Victor jagt zwölf Boxkämpfer quer über den großen Sylter Deich.", _fontCousine20, 8, 50);
           // _textMeshCabin12 = RC.GetTextMesh("The quick brown fox jumps over the lazy dog.", _fontCabin12, 8, 180);
          //  _textMeshCabin20 = RC.GetTextMesh("The quick brown fox jumps over the lazy dog.", _fontCabin20, 8, 210);
          //  _textMeshCabin30 = RC.GetTextMesh("The quick brown fox jumps over the lazy dog.", _fontCabin30, 8, 250);

            // with and without kerning
         //   _textMeshCabin30N = RC.GetTextMesh("AVAVAVAVAVAVAVAVA", _fontCabin30, 8, 290);

         //   _fontCabin30.UseKerning = true;
         //   _textMeshCabin30K = RC.GetTextMesh("AVAVAVAVAVAVAVAVA", _fontCabin30, 8, 330);

            // dummy cube
            _mesh = new Cube();
        
            var sp = MoreShaders.GetShader("color", RC);
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

            // text examples: static text
            var col1 = new float4(0, 0, 0, 1);
            //RC.TextOut(_textMeshCousine20, _fontCousine20, col1);

          //  var col2 = new float4(0.5f, 0.5f, 0.5f, 1);
            RC.TextOut(_textMeshCabin12, _fontCabin12);

           /* var col3 = new float4(1, 1, 0, 1);
            RC.TextOut(_textMeshCabin20, _fontCabin20, col3);

            var col4 = new float4(0, 0, 0, 0.3f);
            RC.TextOut(_textMeshCabin30, _fontCabin30, col4);

            var col5 = new float4(0, 0, 0, 1);
            RC.TextOut(_textMeshCabin30N, _fontCabin30, col5);
            RC.TextOut(_textMeshCabin30K, _fontCabin30, col5);*/

            // text examples: dynamic text
            var col6 = new float4(0, 1, 1, 1);
            RC.TextOut("Framerate: " + Time.Instance.FramePerSecondSmooth + "fps", _fontCabin20, col6, 950, 50);
            RC.TextOut("Time: " + Math.Round(Time.Instance.TimeSinceStart, 1) + " seconds", _fontCabin20, col6, 950, 80);

            Present();
        }

        public override void Resize()
        {
            // is called when the window is resized
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);
        }

        public static void Main()
        {
            var app = new TextDemo();
            app.Run();
        }

    }
}
