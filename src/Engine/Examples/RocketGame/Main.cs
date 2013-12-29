using Fusee.Engine;
using Fusee.SceneManagement;
using Fusee.Math;

namespace Examples.RocketGame
{
    public class RocketGame : RenderCanvas
    {
        public override void Init()
        {
            RC.ClearColor = new float4(0, 0, 0, 1);
        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
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
            var app = new RocketGame();
            app.Run();
        }

    }
}
