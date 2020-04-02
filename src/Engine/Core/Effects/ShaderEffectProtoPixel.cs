using System;
using System.Collections.Generic;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.Core.ShaderShards.Fragment;

namespace Fusee.Engine.Core.Effects
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
        public EffectProps EffectProps { get; set; }

        private readonly FxPassDeclarationProto _effectPass;

        /// <summary>
        /// Creates a new instance of type ShaderEffectProtoPixel.
        /// </summary>
        /// <param name="effectPass"></param>
        /// <param name="effectParameters"></param>
        public ShaderEffectProtoPixel(FxPassDeclarationProto effectPass, IEnumerable<IFxParamDeclaration> effectParameters)
        {
            ParamDecl = new Dictionary<string, IFxParamDeclaration>();
            _effectPass = effectPass;

            if (effectParameters != null)
            {
                foreach (var param in effectParameters)
                {
                    ParamDecl.Add(param.Name, param);
                }
            }

            RendererStates = effectPass.StateSet;
            VertexShaderSrc = effectPass.VS;
            GeometryShaderSrc = effectPass.GS;
            //PixelShaderSrc is not set here because it gets built in a pre-pass, depending on whether we render deferred or forward.            

            EffectManagerEventArgs = new EffectManagerEventArgs(this, ChangedEnum.UNCHANGED);
        }

        /// <summary>
        ///Called by the SceneVisitor in the pre-pass to create the correct fragment shader, whether we render forward or deferred.
        /// </summary>
        public void CreateFragmentShader(bool doRenderForward)
        {
            if (doRenderForward)
            {
                var pxBody = new List<string>()
                    {
                        Lighting.LightStructDeclaration,
                        FragProperties.FixedNumberLightArray,
                        FragProperties.ColorOut(),
                        Lighting.AssembleLightingMethods(EffectProps),
                        FragMain.ForwardLighting(EffectProps)
                    };

                PixelShaderSrc = _effectPass.ProtoPS + string.Join("\n", pxBody);

            }
            else
            {
                var pxBody = new List<string>()
                    {
                        FragProperties.GBufferOut(),
                        FragMain.RenderToGBuffer(EffectProps)
                    };
                PixelShaderSrc = _effectPass.ProtoPS + string.Join("\n", pxBody);
            }
        }
    }
}
