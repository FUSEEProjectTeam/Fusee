using Fusee.Engine.Common;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.Core.ShaderShards.Fragment;
using Fusee.Engine.Core.ShaderShards.Vertex;
using System.Collections.Generic;

namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// The default <see cref="Effect"/>, that is used if no other Effect is found.
    /// Provides properties to change the Diffuse Color, Specular Color, Specular Intensity and Specular Shininess.
    /// </summary>
    public class SurfaceEffect : SurfaceEffectBase
    {
        /// <summary>
        /// Set this to true if GPU instancing is used to render the mesh this effect is used for.
        /// </summary>
        readonly RenderFlags RenderModifications;

        /// <summary>
        /// Creates a new instance of type DefaultSurfaceEffect.
        /// </summary>
        /// <param name="input">See <see cref="SurfaceEffectBase.SurfaceInput"/>.</param>
        /// <param name="surfOutFragBody">The method body for the <see cref="SurfaceEffectBase.SurfOutFragMethod"/></param>
        /// <param name="surfOutVertBody">The method body for the <see cref="SurfaceEffectBase.SurfOutVertMethod"/></param>
        /// <param name="rendererStates">The renderer state set for this effect.</param>
        public SurfaceEffect(SurfaceEffectInput input, RenderFlags renderMod = RenderFlags.None, List<string> surfOutVertBody = null, List<string> surfOutFragBody = null, RenderStateSet rendererStates = null)
            : base(input, rendererStates)
        {
            RenderModifications = renderMod;
            var inputType = input.GetType();
            if (surfOutFragBody != null)
                SurfOutFragMethod = SurfaceOut.GetChangeSurfFragMethod(surfOutFragBody, inputType);
            else
                SurfOutFragMethod = SurfaceOut.GetChangeSurfFragMethod(FragShards.SurfOutBody(input), inputType);

            if (surfOutVertBody != null)
                SurfOutVertMethod = SurfaceOut.GetChangeSurfVertMethod(surfOutVertBody, input.ShadingModel);
            else
                SurfOutVertMethod = SurfaceOut.GetChangeSurfVertMethod(VertShards.SurfOutBody(input), input.ShadingModel);
            
            VertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Main, VertMain.VertexMain(SurfaceInput.ShadingModel, SurfaceInput.TextureSetup, RenderModifications)));

            //TODO: try to suppress adding these parameters if the effect is used only for deferred rendering.
            //May be difficult because we'd need to remove or add them (and only them) depending on the render method            
            foreach (var dcl in CreateForwardLightingParamDecls(ModuleExtensionPoint.NumberOfLightsForward))
                UniformParameters.Add(dcl.Hash, dcl);

            HandleFieldsAndProps();
        }
    }
}