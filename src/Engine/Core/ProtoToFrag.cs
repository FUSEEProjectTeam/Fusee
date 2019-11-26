using Fusee.Serialization;
using Fusee.Xene;

namespace Fusee.Engine.Core
{
    internal class ProtoToFrag : SceneVisitor
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

        public void BuildFragmentShaders()
        {
            Traverse(_sc.Children);
        }

        /// <summary>
        /// If a ShaderEffectComponent is visited the ShaderEffect of the <see cref="RendererState"/> is updated and the effect is set in the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="shaderComponent">The ShaderEffectComponent</param>
        [VisitMethod]
        public void BuildFragmentShaderFor(ShaderEffectComponent shaderComponent)
        {
            if (shaderComponent.Effect.GetType() != typeof(ShaderEffectProtoPixel)) return;

            var effect = (ShaderEffectProtoPixel)shaderComponent.Effect;           
            effect.CreateFragmentShader(_renderForward);           

        }
    }
}
