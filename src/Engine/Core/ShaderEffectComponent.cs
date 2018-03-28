using Fusee.Serialization;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Use this component in Code. Will not serialize/deserialize
    /// </summary>
    public class ShaderEffectComponent: SceneComponentContainer
    {
        /// <summary>
        /// The effect.
        /// </summary>
        public ShaderEffect Effect { get; set; }


        public ShaderEffectComponent(RenderContext rc, ShaderEffect effect)
        {
            Effect = effect;
            rc.SetShaderEffect(effect);
        }
    }
}