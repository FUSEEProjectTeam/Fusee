using Fusee.Engine;
using Fusee.SceneManagement;
using Fusee.Math;

namespace Examples.BulletSharpSoft
{
    public class BulletSharpSoft : RenderCanvas
    {
        public override void Init()
        {
            // is called on startup
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
        }

        public static void Main()
        {
            var app = new BulletSharpSoft();
            app.Run();
        }

    }
}
