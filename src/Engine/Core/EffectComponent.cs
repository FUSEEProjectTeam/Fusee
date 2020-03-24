using Fusee.Engine.Core.Effects;
using Fusee.Serialization;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Use this component in Code. Will not serialize/deserialize
    /// </summary>
    public class EffectComponent: SceneComponentContainer
    {
        /// <summary>
        /// The effect.
        /// </summary>
        public Effect Effect { get; set; }
    }    
}