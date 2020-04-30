using Fusee.Engine.Core.Scene;
using Fusee.Xene;
using System;

namespace Fusee.Engine.Core
{
    internal class ProtoToFrag : Visitor<SceneNode, SceneComponent>
    {
        /// <summary>
        /// The SceneContainer, containing the scene that gets rendered.
        /// </summary>
        private readonly SceneContainer _sc;

        private readonly bool _renderForward;

        public ProtoToFrag(SceneContainer sc, bool renderForward)
        {
            _sc = sc;
            _renderForward = renderForward;
        }

        internal void BuildFragmentShaders()
        {
            Traverse(_sc.Children);
        }

        /// <summary>
        /// If a ShaderEffectComponent is visited the ShaderEffect of the <see cref="RendererState"/> is updated and the effect is set in the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="shaderComponent">The ShaderEffectComponent</param>
        [VisitMethod]
        public void BuildFragmentShaderFor(ShaderEffect shaderComponent)
        {
            if (shaderComponent.GetType() != typeof(ShaderEffectProtoPixel)) return;

            var effect = (ShaderEffectProtoPixel)shaderComponent;
            effect.CreateFragmentShader(_renderForward);

        }
    }
}
