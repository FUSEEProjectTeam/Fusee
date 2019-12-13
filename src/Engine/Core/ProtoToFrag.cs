using Fusee.Base.Core;
using Fusee.Serialization;
using Fusee.Xene;
using System;

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
            Diagnostics.Error("BuildFragmentShadersCalled");
            Traverse(_sc.Children);
            Diagnostics.Error("Traversal done");
        }

        /// <summary>
        /// If a ShaderEffectComponent is visited the ShaderEffect of the <see cref="RendererState"/> is updated and the effect is set in the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="shaderComponent">The ShaderEffectComponent</param>
        [VisitMethod]
        public void BuildFragmentShaderFor(Fusee.Engine.Core.ShaderEffectComponent shaderComponent)
        {
            Diagnostics.Error("ShaderEffectComponent called!", null, new object[] { shaderComponent });

            if (shaderComponent.Effect.GetType() != typeof(ShaderEffectProtoPixel)) return;

            var effect = (ShaderEffectProtoPixel)shaderComponent.Effect;
            effect.CreateFragmentShader(_renderForward);
        }

        /// <summary>
        /// If a ShaderEffectComponent is visited the ShaderEffect of the <see cref="RendererState"/> is updated and the effect is set in the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="shaderComponent">The ShaderEffectComponent</param>
        [VisitMethod]
        public void M(Fusee.Serialization.Mesh mesh)
        {
            Diagnostics.Error("MESH called!", null, new object[] { mesh });
        }

        /// <summary>
        /// If a ShaderEffectComponent is visited the ShaderEffect of the <see cref="RendererState"/> is updated and the effect is set in the <see cref="RenderContext"/>.
        /// </summary>
        /// <param name="shaderComponent">The ShaderEffectComponent</param>
        [VisitMethod]
        public void T(Fusee.Serialization.TransformComponent t)
        {
            Diagnostics.Error("TransformComp called!", null, new object[] { t });
        }
    }
}
