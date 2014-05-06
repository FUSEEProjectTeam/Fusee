using System.Windows.Forms;
using Fusee.Engine;
using Fusee.SceneManagement;
using Fusee.Math;

namespace Examples.WindowSizesDemo
{
    public class WindowSizesDemo : RenderCanvas
    {
        // is called on startup
        public override void Init()
        {
            RC.ClearColor = new float4(0.1f, 0.1f, 0.5f, 1);

            var windowPosX = Screen.PrimaryScreen.Bounds.Width/2;
            var windowPosY = Screen.PrimaryScreen.Bounds.Height/2;
            
            // IMPORTANT: You are supposed to use either one SetWindowSize() or VideoWall(). You can't use both.
            // !!!

            // IMPORTANT: It's possible to resize a window to fit on a video wall or display.
            // Here are some different variants how to use the method. Uncomment to test.
            //SetWindowSize(800, 600, false);
            SetWindowSize(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height/9 * 2, false, 0, 0);
            //SetWindowSize(Screen.PrimaryScreen.Bounds.Width/9 * 2, Screen.PrimaryScreen.Bounds.Height, true, 0, 0);
            //SetWindowSize(640, 480, false, windowPosX - 640 / 2, windowPosY - 480 / 2);
            // IMPORTANT: But it's also possible to let the system do the magic by giving it the number of monitors per axis and specify some border information.
            //VideoWall(0, 0, true, false);
        }

        // is called once a frame
        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            Present();
        }

        // is called when the window was resized
        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);
        }

        public static void Main()
        {
            var app = new WindowSizesDemo();
            app.Run();
        }
    }
}
