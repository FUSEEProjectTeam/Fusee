namespace Fusee.Engine.Core.Effects
{
    /// <summary>
    /// Enum for determining the action that needs to be taken for a uniform variable in the <see cref="EffectManager"/>.
    /// </summary>
    public enum UniformChangedEnum
    {
        /// <summary>
        /// The effect isn't needed anymore - the uniform will be disposed of.
        /// </summary>
        Dispose = 0,

        /// <summary>
        /// The uniform value has changed and will be updated.
        /// </summary>
        Update = 1,

        //Not needed at the moment, because a ShaderEffect must declare all it's parameter declarations at creation.
        //Add = 2,

        /// <summary>
        /// The uniform value is unchanged - no action needed.
        /// </summary>
        Unchanged = 3
    }
}
