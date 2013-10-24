using System.Runtime.InteropServices;
using Fusee.Engine;
using Fusee.SceneManagement;
using Fusee.Math;

namespace Examples.TextDemo
{
    public class TextDemo : RenderCanvas
    {
        private int _myFont;
        private int _myFont2;

        public override void Init()
        {
            // is called on startup
            RC.ClearColor = new float4(0.5f, 1, 1, 1);
            _myFont = RC.LoadFont("Assets/Cousine.ttf");
            _myFont2 = RC.LoadFont("Assets/Cabin.ttf");
        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            var sx = 2.0f/Width;
            var sy = 2.0f/Height;
            RC.TextOut("Victor jagt zwölf Boxkämpfer quer über den großen Sylter Deich.", _myFont, 12, -1 + 8 * sx, 1 - 50 * sy, sx, sy);
            RC.TextOut("Victor jagt zwölf Boxkämpfer quer über den großen Sylter Deich.", _myFont, 18, -1 + 8 * sx, 1 - 80 * sy, sx, sy);
            RC.TextOut("Victor jagt zwölf Boxkämpfer quer über den großen Sylter Deich.", _myFont, 24, -1 + 8 * sx, 1 - 110 * sy, sx, sy);
            RC.TextOut("Victor jagt zwölf Boxkämpfer quer über den großen Sylter Deich.", _myFont2, 12, -1 + 8 * sx, 1 - 180 * sy, sx, sy);
            RC.TextOut("Victor jagt zwölf Boxkämpfer quer über den großen Sylter Deich.", _myFont2, 18, -1 + 8 * sx, 1 - 210 * sy, sx, sy);
            RC.TextOut("Victor jagt zwölf Boxkämpfer quer über den großen Sylter Deich.", _myFont2, 24, -1 + 8 * sx, 1 - 240 * sy, sx, sy);

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
