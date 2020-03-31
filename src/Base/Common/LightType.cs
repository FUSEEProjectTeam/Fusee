namespace Fusee.Base.Common
{
    /// <summary>
    /// Specifies the type of the light.
    /// </summary>
    public enum LightType
    {
        /// <summary>
        /// Point light. Emits rays from a single point radially into all directions
        /// </summary>
        Point,

        /// <summary>
        /// Parallel light. Emits parallel rays into a specified direction. No attenuation.
        /// </summary>
        Parallel,

        /// <summary>
        /// Spot light. Like a point light but with rules describing the intensities of the
        /// rays depending on their direction.
        /// </summary>
        Spot,

        /// <summary>
        /// Simple infinite Softbox at CameraPosition
        /// </summary>
        Legacy
    }    
}
