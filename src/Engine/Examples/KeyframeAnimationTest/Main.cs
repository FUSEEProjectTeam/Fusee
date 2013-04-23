using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Fusee.Engine;
using Fusee.Math;
using Fusee.KeyFrameAnimation;
using Fusee.SceneManagement;

namespace Examples.KeyframeAnimationTest
{
    public class KeyframeDemo : RenderCanvas
    {
        private static Channel _channel1 = new Channel();

        public override void Init()
        {
            _channel1.AddKeyframe(5f, 32f);
            _channel1.AddKeyframe(10f, 1f);
            _channel1.AddKeyframe(2f, 4f);
            _channel1.AddKeyframe(3f, 8f);
            _channel1.AddKeyframe(4f, 16f);
        }

        public override void RenderAFrame()
        {
            // is called once a frame
            
            Console.WriteLine("Actual time "+Time.Instance.TimeSinceStart+" Value of 'Animation' "+_channel1.GetValueAt((float)Time.Instance.TimeSinceStart));
            //Console.WriteLine(_channel1.GetValueAt((float)Time.Instance.TimeSinceStart));
            //_channel1.GetValueAt((float) Time.Instance.TimeSinceStart);
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
            var app = new KeyframeDemo();
            app.Run();
        }

    }
}
