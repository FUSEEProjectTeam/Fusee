using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Primitives;
using Fusee.Math.Core;
using static Fusee.Engine.Core.Input;

namespace Fusee.Examples.ComputeFractal.Core
{
    //see: https://www.reddit.com/r/Unity3D/comments/7pa6bq/drawing_mandelbrot_fractal_using_gpu_compute/

    [FuseeApplication(Name = "FUSEE Simple Example", Description = "A very simple example.")]
    public class ComputeFractal : RenderCanvas
    {
        private readonly Plane _plane = new Plane();

        private WritableTexture RWTexture;
        private ComputeShader _computeShader;
        private ShaderEffect _renderEffect;
        private bool _move;
        private bool _inputChange;
        private StorageBuffer<double> _rect;
        private StorageBuffer<float4> _colors;
        private float4[] _colorData = new float4[256];
        private double[] _rectData = new double[4];

        private double cx, cy;    // center of the view rect
        private float depthFactor = 1f;

        private float Sawtooth(float i, float m)
        {
            return m - System.Math.Abs(i % (2 * m) - m);
        }

        private void Input()
        {                   // in this method we simply change size and position of the view rect in fractal dimension, but it is always projected to the texture
            double k = 0.0009765625f;
            double borderChange = 2.0;

            if (Mouse.WheelVel != 0)
            {
                depthFactor -= 0.2f * depthFactor * Mouse.WheelVel;
                _inputChange = true;
            }
            if (Mouse.GetButton(2))
            {
                _move = true;
            }
            if (!Mouse.GetButton(2))
            {
                _move = false;
            }
            if (_move)
            {
                cx -= 100 * k * depthFactor * Mouse.GetAxis(1);
                cy -= 100 * k * depthFactor * Mouse.GetAxis(2);
            }
            if (_move && (Mouse.GetAxis(1) != 0 || Mouse.GetAxis(2) != 0))
                _inputChange = true;
            if (_inputChange)
            {
                _rectData[0] = cx - depthFactor * borderChange;
                _rectData[1] = cy - depthFactor * borderChange;
                _rectData[2] = cx + depthFactor * borderChange;
                _rectData[3] = cy + depthFactor * borderChange;

                _rect.SetData(_rectData);
            }
        }

        // Init is called on startup.
        public override void Init()
        {
            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);
            RWTexture = WritableTexture.CreateForComputeShader(1024, 1024);

            _rect = new StorageBuffer<double>(this, 4, sizeof(double), 1);

            _rectData[0] = -2.0f;
            _rectData[1] = -2.0f;
            _rectData[2] = 2.0f;
            _rectData[3] = 2.0f;            

            _colors = new StorageBuffer<float4>(this, 256, sizeof(float) * 4, 2);
            var i = 0;
            while (i < 256)
            {
                _colorData[i] = new float4(0, 0, 0, 1);
                if (i >= 0 && i < 128)
                    _colorData[i] += new float4(0, 0, Sawtooth(i * 4, 256) / 256, 1);
                if (i >= 64 && i < 192)
                    _colorData[i] += new float4(0, Sawtooth((i - 64) * 4, 256) / 256, 0, 1);
                if (i >= 128 && i < 256)
                    _colorData[i] += new float4(Sawtooth(i * 4, 256) / 256, 0, 0, 1);
                i++;
            }            

            _computeShader = new ComputeShader(

                shaderCode: AssetStorage.Get<string>("MandelbrotFractal.comp"),
                effectParameters: new IFxParamDeclaration[]
                {
                    new FxParamDeclaration<WritableTexture> { Name = "destTex", Value = RWTexture},
                    new FxParamDeclaration<StorageBuffer<float4>>{ Name = "colorStorageBuffer", Value = _colors},
                    new FxParamDeclaration<StorageBuffer<double>>{ Name = "rectStorageBuffer", Value = _rect},
                }
            );

            _renderEffect = new ShaderEffect(

             new FxPassDeclaration
             {
                 VS = AssetStorage.Get<string>("RenderTexToScreen.vert"),
                 PS = AssetStorage.Get<string>("RenderTexToScreen.frag"),
                 StateSet = new RenderStateSet
                 {
                     AlphaBlendEnable = false,
                     ZEnable = true,
                 }
             },
             new IFxParamDeclaration[]
             {
                new FxParamDeclaration<WritableTexture> { Name = "srcTex", Value = RWTexture}
             });

            RC.SetEffect(_computeShader);
            _rect.SetData(_rectData);
            _colors.SetData(_colorData);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            RWTexture.AsImage = true;
            RC.SetEffect(_computeShader);
            RC.DispatchCompute(-1, RWTexture.Width / 16, RWTexture.Width / 16, 1);
            RC.MemoryBarrier();

            RWTexture.AsImage = false;
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            RC.Viewport(0, 0, Width, Height);
            RC.SetEffect(_renderEffect);
            RC.Render(_plane);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }
    }
}