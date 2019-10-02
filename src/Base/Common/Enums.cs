namespace Fusee.Base.Common
{
    /// <summary>
    /// Keep this binary compatible with System.IO.FileMode (unsupported on portable libraries).
    /// </summary>
    public enum FileMode
    {
#pragma warning disable 1591
        CreateNew = 1,
        Create = 2,
        Open = 3,
        OpenOrCreate = 4,
        Truncate = 5,
        Append = 6,
#pragma warning restore 1591
    }

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

    public enum CubeMapFaces
    {
        POSITIVE_X,
        NEGATIVE_X,
        POSITIVE_Y,
        NEGATIVE_Y,
        POSITIVE_Z,
        NEGATIVE_Z,

    }
}
