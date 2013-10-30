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

        private IFont _fontCalibri20;

        private Mesh _mesh;
        private IShaderParam _vColor;

        private static float _angleHorz;

        public override void Init()
        {
            RC.ClearColor = new float4(0.2f, 0.2f, 0.5f, 1);

            _fontCousine20 = RC.LoadFont("Assets/Cousine.ttf", 20);
            _fontCabin12 = RC.LoadFont("Assets/Cabin.ttf", 12);
            _fontCabin20 = RC.LoadFont("Assets/Cabin.ttf", 20);
            _fontCabin30 = RC.LoadFont("Assets/Cabin.ttf", 30);
            _fontCalibri20 = RC.LoadSystemFont("calibri", 20);

            // dummy cube
            _mesh = new Cube();
        
            var sp = MoreShaders.GetShader("color", RC);
            RC.SetShader(sp);

            _vColor = sp.GetShaderParam("color");
            //_vColor = RC.GetShaderParam(sp, "color");

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

            // text examples
            var col1 = new float4(1, 1, 1, 1);
            RC.TextOut("Victor jagt zwölf Boxkämpfer quer über den großen Sylter Deich.", _fontCousine20, col1, 8, 50);

            var col2 = new float4(0.5f, 0.5f, 0.5f, 1);
            RC.TextOut("Victor jagt zwölf Boxkämpfer quer über den großen Sylter Deich.", _fontCabin12, col2, 8, 180);

            var col3 = new float4(1, 1, 0, 1);
            RC.TextOut("Victor jagt zwölf Boxkämpfer quer über den großen Sylter Deich.", _fontCabin20, col3, 8, 210);

            _fontCabin30.UseKerning = false;
            var col4 = new float4(0, 0, 0, 0.3f);

            RC.TextOut("AVictor jagt zwölf Boxkämpfer quer über den großen Sylter Deich.", _fontCabin30, col4, 8, 250);

            _fontCabin30.UseKerning = true;
            var col5 = new float4(0, 0, 0, 1);

            RC.TextOut("AVictor jagt zwölf Boxkämpfer quer über den großen Sylter Deich.", _fontCabin30, col5, 8, 290);
            RC.TextOut("AVictor jagt zwölf Boxkämpfer quer über den großen Sylter Deich.", _fontCalibri20, col5, 8, 400);

            var col6 = new float4(0, 1, 1, 1);
            RC.TextOut("Aktuelle Framerate: " + Time.Instance.FramePerSecondSmooth + "fps", _fontCabin20, col6, 950, 50);
            RC.TextOut("Zeit seit Start: " + Math.Round(Time.Instance.TimeSinceStart, 1) + " Sek", _fontCabin20, col6, 950, 80);

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
