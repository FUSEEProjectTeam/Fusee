using Fusee.Engine;
using Fusee.Math;

namespace Examples.WebUnitTests
{
    public class WebUnitTests : RenderCanvas
    {
        // is called on startup
        public override void Init()
        {
            Fusee.Tests.Scene.Core.VisitorTests.BasicVisitorTest();
            Fusee.Tests.Scene.Core.VisitorTests.BasicEnumeratorTests();
            Fusee.Tests.Scene.Core.VisitorTests.BasicViseratorTest();

            RC.ClearColor = new float4(0.1f, 0.8f, 0.1f, 1);
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
            var app = new WebUnitTests();
            app.Run();
        }
    }
}
