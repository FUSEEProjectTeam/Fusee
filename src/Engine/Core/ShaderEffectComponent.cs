using Fusee.Engine.Core.Effects;
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
    }

    /// <summary>
    /// Use this component in Code. Will not serialize/deserialize
    /// </summary>
    public class SurfaceEffectComponent : SceneComponentContainer
    {
        /// <summary>
        /// The effect.
        /// </summary>
        public SurfaceEffect Effect { get; set; }
    }
}