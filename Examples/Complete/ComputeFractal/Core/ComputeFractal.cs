using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Primitives;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Gui;
using Fusee.Math.Core;
using static Fusee.Engine.Core.Input;
namespace Fusee.Examples.ComputeFractal.Core
{
    //see: https://www.reddit.com/r/Unity3D/comments/7pa6bq/drawing_mandelbrot_fractal_using_gpu_compute/
    [FuseeApplication(Name = "FUSEE ComputeFractal Example", Description = "A very simple example.")]
    public class ComputeFractal : RenderCanvas
    {
        private readonly Plane _plane = new();
        private WritableTexture RWTexture;
        private ComputeEffect _computeShader;
        private ShaderEffect _renderEffect;
        private bool _move;
        private bool _inputChange;
        private StorageBuffer<double> _rect;
        private StorageBuffer<float4> _colors;
        private readonly float4[] _colorData = new float4[256];
        private readonly double[] _rectData = new double[4];
        // center of the view rect
        private double _cx, _cy;
        private float _depthFactor = 1f;
        private double _k;
        private SceneContainer _gui;
        private SceneRendererForward _guiRenderer;
        private SceneInteractionHandler _sih;
        private GuiText _depthFactorText;
        private readonly float _zNear = 0.1f;
        private readonly float _zFar = 100f;

        // Init is called on startup.
        public override void Init()
        {
            _gui = FuseeGuiHelper.CreateDefaultGui(this, CanvasRenderMode.Screen, "Fractal Magnification Factor: " + _depthFactor);
            _guiRenderer = new SceneRendererForward(_gui);
            _depthFactorText = _gui.Children[1].Children[1].Children[0].GetComponent<GuiText>();

            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);
            RWTexture = WritableTexture.CreateForComputeShader(1024, 1024);

            _k = 1f / RWTexture.Width;
            _rect = new StorageBuffer<double>(this, 4, sizeof(double), 0);
            _rectData[0] = -1.0f;
            _rectData[1] = -1.0f;
            _rectData[2] = 1.0f;
            _rectData[3] = 1.0f;
            _colors = new StorageBuffer<float4>(this, 256, sizeof(float) * 4, 1);

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
            _computeShader = new ComputeEffect(
            shaderCode: AssetStorage.Get<string>("MandelbrotFractal.comp"),
            effectParameters: new IFxParamDeclaration[]
            {
                new FxParamDeclaration<WritableTexture> { Name = "destTex", Value = RWTexture},
                new FxParamDeclaration<StorageBuffer<float4>>{ Name = "colorStorageBuffer", Value = _colors},
                new FxParamDeclaration<StorageBuffer<double>>{ Name = "rectStorageBuffer", Value = _rect},
                new FxParamDeclaration<double>{ Name = "k", Value = _k},
            });

            _renderEffect = new ShaderEffect(
            new IFxParamDeclaration[]
            {
                new FxParamDeclaration<WritableTexture> { Name = "srcTex", Value = RWTexture}
            },
            new RenderStateSet
            {
                AlphaBlendEnable = false,
                ZEnable = true,
            },
            AssetStorage.Get<string>("RenderTexToScreen.vert"),
            AssetStorage.Get<string>("RenderTexToScreen.frag"));

            RC.SetEffect(_computeShader);
            _rect.SetData(_rectData);
            _colors.SetData(_colorData);

            _sih = new SceneInteractionHandler(_gui, _guiRenderer.PrePassVisitor.CameraPrepassResults);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            MoveFractal();

            RWTexture.AsImage = true;
            RC.SetEffect(_computeShader);
            RC.DispatchCompute(-1, RWTexture.Width / 16, RWTexture.Width / 16, 1);
            RC.MemoryBarrier();
            RWTexture.AsImage = false;

            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            RC.SetEffect(_renderEffect);
            RC.Render(_plane);

            RC.View = float4x4.LookAt(0, 0, 1, 0, 0, 0, 0, 1, 0);
            RC.Projection = float4x4.CreateOrthographic(Width, Height, _zNear, _zFar);
            _guiRenderer.Render(RC);

            if (!Mouse.Desc.Contains("Android"))
            {
                _sih.CheckForInteractiveObjects(Mouse.Position, Width, Height);
            }
            if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _sih.CheckForInteractiveObjects(Touch.GetPosition(TouchPoints.Touchpoint_0), Width, Height);
            }
            Present();
        }

        private void MoveFractal()
        {
            // in this method we simply change size and position of the view rect in fractal dimension, but it is always projected to the texture
            double borderChange = 1.0;
            if (Mouse.WheelVel != 0)
            {
                _depthFactor += 0.001f * _depthFactor * Mouse.WheelVel;
                _depthFactorText.Text = "Fractal Magnification Factor: " + _depthFactor;
                _inputChange = true;
            }
            if (Mouse.GetButton(1))
            {
                _move = true;
            }
            if (!Mouse.GetButton(1))
            {
                _move = false;
            }
            if (_move)
            {
                _cx -= 0.1 * _k * _depthFactor * Mouse.Velocity.x;
                _cy += 0.1 * _k * _depthFactor * Mouse.Velocity.y;
            }
            if (_move && (Mouse.GetAxis(1) != 0 || Mouse.GetAxis(2) != 0))
                _inputChange = true;
            if (_inputChange)
            {
                _rectData[0] = _cx - _depthFactor * borderChange;
                _rectData[1] = _cy - _depthFactor * borderChange;
                _rectData[2] = _cx + _depthFactor * borderChange;
                _rectData[3] = _cy + _depthFactor * borderChange;
                _rect.SetData(_rectData);
            }
        }

        private float Sawtooth(float i, float m)
        {
            return m - System.Math.Abs(i % (2 * m) - m);
        }
    }
}