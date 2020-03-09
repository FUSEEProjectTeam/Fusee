using System;
using System.Collections.Generic;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.Core.ShaderShards.Fragment;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// A ShaderEffect contains a list of render passes with each pass item being a combination of a set of render states, and Shader Programs (the code running on the GPU).
    /// In addition a ShaderEffect contains the actual values for all the shaders' (uniform) variables.
    /// </summary>
    public class ShaderEffectProtoPixel : ShaderEffect
    {
        /// <summary>
        /// The effect props are the basis on which we can decide what kind of shards this effect supports.
        /// </summary>
        public ShaderEffectProps EffectProps { get; set; }

        private readonly FxPassDeclarationProto[] _effectPasses;

        /// <summary>
        /// Creates a new instance of type ShaderEffectProtoPixel.
        /// </summary>
        /// <param name="effectPasses"></param>
        /// <param name="effectParameters"></param>
        public ShaderEffectProtoPixel(FxPassDeclarationProto[] effectPasses, IEnumerable<IFxParamDeclaration> effectParameters)
        {
            if (effectPasses == null || effectPasses.Length == 0)
                throw new ArgumentNullException(nameof(effectPasses), "must not be null and must contain at least one pass");

            var nPasses = effectPasses.Length;

            States = new RenderStateSet[nPasses];

            VertexShaderSrc = new string[nPasses];
            PixelShaderSrc = new string[nPasses];
            GeometryShaderSrc = new string[nPasses];

            ParamDecl = new Dictionary<string, IFxParamDeclaration>();

            if (effectParameters != null)
            {
                foreach (var param in effectParameters)
                {
                    ParamDecl.Add(param.Name, param);
                }
            }

            _effectPasses = effectPasses;

            for (int i = 0; i < nPasses; i++)
            {
                States[i] = effectPasses[i].StateSet;
                VertexShaderSrc[i] = effectPasses[i].VS;
                GeometryShaderSrc[i] = effectPasses[i].GS;
                //PixelShaderSrc is not set here because it gets built in a pre-pass, depending on whether we render deferred or forward.
            }

            EffectEventArgs = new ShaderEffectEventArgs(this, ShaderEffectChangedEnum.UNCHANGED);
        }

        /// <summary>
        ///Called by the SceneVisitor in the pre-pass to create the correct fragment shader, whether we render forward or deferred.
        /// </summary>
        public void CreateFragmentShader(bool doRenderForward)
        {
            if (doRenderForward)
            {
                for (int i = 0; i < _effectPasses.Length; i++)
                {
                    var pxBody = new List<string>()
                    {
                        LightingShard.LightStructDeclaration,
                        FragPropertiesShard.FixedNumberLightArray,
                        FragPropertiesShard.ColorOut(),
                        LightingShard.AssembleLightingMethods(EffectProps),
                        FragMainShard.ForwardLighting(EffectProps)
                    };

                    PixelShaderSrc[i] = _effectPasses[i].ProtoPS + string.Join("\n", pxBody);
                }
            }
            else
            {
                for (int i = 0; i < _effectPasses.Length; i++)
                {
                    var pxBody = new List<string>()
                    {
                        FragPropertiesShard.GBufferOut(),
                        FragMainShard.RenderToGBuffer(EffectProps)
                    };
                    PixelShaderSrc[i] = _effectPasses[i].ProtoPS + string.Join("\n", pxBody);

                    States[i].AlphaBlendEnable = false;
                    States[i].ZEnable = true;
                }
            }
        }
    }
}
