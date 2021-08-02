using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Primitives;
using Fusee.Math.Core;

namespace Fusee.Examples.ComputeFractal.Core
{

    [FuseeApplication(Name = "FUSEE Simple Example", Description = "A very simple example.")]
    public class ComputeFractal : RenderCanvas
    {
        private readonly Plane _plane = new Plane();

        private WritableTexture RWTexture;
        private ComputeShader _computeShader;
        private ShaderEffect _renderEffect;

        // Init is called on startup.
        public override void Init()
        {
            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(1, 1, 1, 1);

            RWTexture = WritableTexture.CreateForComputeShader(1024, 1024);

            _computeShader = new ComputeShader(

                shaderCode: AssetStorage.Get<string>("MandelbrotFractal.comp"),
                effectParameters: new IFxParamDeclaration[]
                {
                    new FxParamDeclaration<WritableTexture> { Name = "RWTexture", Value = RWTexture}
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
                new FxParamDeclaration<WritableTexture> { Name = "RWTexture", Value = RWTexture}
             });
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {           
            RWTexture.AsImage = true;
            RC.SetEffect(_computeShader);
            RC.DispatchCompute(-1, 1, 1, 1);
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