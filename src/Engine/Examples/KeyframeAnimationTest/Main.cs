using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Fusee.Engine;
using Fusee.Math;
using Fusee.KeyFrameAnimation;
using Fusee.SceneManagement;
using Fusse.KeyFrameAnimation;

namespace Examples.KeyframeAnimationTest
{
    public class KeyframeDemo : RenderCanvas
    {

        //private Channel<float3> _channel1 ;
        private Channel<float4> _channel1;
        //private Channel<float> _channel1;
        private Animation myAnim = new Animation();
        public override void Init()
        {
            SceneManager.RC = RC;
            
            //_channel1 = new Channel<float3>(Lerp.Float3Lerp);
            _channel1 = new Channel<float4>(delegate(float4 val) { RC.ClearColor = val; },Lerp.Float4Lerp);
            //_channel1 = new Channel<float>(Lerp.FloatLerp);
            /*
            _channel1.AddKeyframe(5f, 32f);
            _channel1.AddKeyframe(10f, 2f);
            _channel1.AddKeyframe(2f, 4f);
            _channel1.AddKeyframe(3f, 8f);
            _channel1.AddKeyframe(4f, 16f);
            _channel1.AddKeyframe(0f, 8f);
            */
            
            Keyframe<float4> key0 = new Keyframe<float4>(0, new float4(1, 0, 1, 1));
            Keyframe<float4> key1 = new Keyframe<float4>(2, new float4(0.125f, 1, 0.125f, 1));
            Keyframe<float4> key2 = new Keyframe<float4>(4, new float4(0.250f, 0.75f, 0.250f, 1));
            Keyframe<float4> key3 = new Keyframe<float4>(6, new float4(0.5f, 0.5f, 0.5f, 1));
            Keyframe<float4> key4 = new Keyframe<float4>(8, new float4(0.75f, 0.25f, 0.75f, 1));
            Keyframe<float4> key5 = new Keyframe<float4>(10, new float4(1, 25, 0.125f, 1));
            Keyframe<float4> key6 = new Keyframe<float4>(0, new float4(0, 1, 0, 1));

            _channel1.AddKeyframe(key0);
            _channel1.AddKeyframe(key1);
            _channel1.AddKeyframe(key2);
            _channel1.AddKeyframe(key3);
            _channel1.AddKeyframe(key4);
            _channel1.AddKeyframe(key5);
            _channel1.AddKeyframe(key6);
            //*/
            /*
            Keyframe<float3> key0 = new Keyframe<float3>(0, new float3(0, 0, 0));
            Keyframe<float3> key1 = new Keyframe<float3>(2, new float3(1, 2, 3));
            Keyframe<float3> key2 = new Keyframe<float3>(4, new float3(2, 4, 6));
            Keyframe<float3> key3 = new Keyframe<float3>(6, new float3(4, 8, 12));
            Keyframe<float3> key4 = new Keyframe<float3>(8, new float3(8, 16, 16));
            Keyframe<float3> key5 = new Keyframe<float3>(10, new float3(16, 16, 16));
            Keyframe<float3> key6 = new Keyframe<float3>(0, new float3(8, 8, 8));

            _channel1.AddKeyframe(key0);
            _channel1.AddKeyframe(key1);
            _channel1.AddKeyframe(key2);
            _channel1.AddKeyframe(key3);
            _channel1.AddKeyframe(key4);
            _channel1.AddKeyframe(key5);
            _channel1.AddKeyframe(key6);
            //*/

            myAnim.AddChannel(_channel1);
            
        }

        public override void RenderAFrame()
        {
            // is called once a frame
            SceneManager.Manager.Traverse(this);

            myAnim.SetTick((float) Time.Instance.TimeSinceStart);

            //RC.ClearColor = _channel1.GetValueAt((float)Time.Instance.TimeSinceStart);
            //Console.WriteLine("Actual time "+Time.Instance.TimeSinceStart+" Value of 'Animation' "+_channel1.GetValueAt((float)Time.Instance.TimeSinceStart));
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
