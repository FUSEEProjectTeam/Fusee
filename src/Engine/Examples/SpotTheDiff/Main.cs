using Fusee.Engine;
using Fusee.SceneManagement;
using Fusee.Math;

namespace Examples.SpotTheDiff
{
    public class SpotTheDiff : RenderCanvas
    {
        private GUIHandler _guiHandler;
        private GUIImage _guiImage;

        public override void Init()
        {
            // is called on startup
            Width = 750;
            Height = 800;

            // GUI
            _guiHandler = new GUIHandler();
            _guiHandler.AttachToContext(RC);

            _guiImage = new GUIImage("Assets/spot_the_diff.png", 0, 0, Width, Height);
        }

        public override void RenderAFrame()
        {
            // is called once a frame
        }

        public override void Resize()
        {
            // is called when the window is resized
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);

            // refresh all elements
            _guiHandler.Refresh();
        }

        public static void Main()
        {
            var app = new SpotTheDiff();
            app.Run();
        }

    }
}
