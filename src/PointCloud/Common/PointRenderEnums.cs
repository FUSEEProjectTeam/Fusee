namespace Fusee.PointCloud.Common
{
    /// <summary>
    /// The gpu data can take different states in its life cycle.
    /// Gpu data may need to be handled differently according to its current state.
    /// </summary>
    public enum GpuDataState
    {
        /// <summary>
        /// Default, gpu data wasn't created yet and or points havent been loaded yet.
        /// </summary>
        None = -1,

        /// <summary>
        /// Gpu data was newly created.
        /// </summary>
        New = 0,

        /// <summary>
        /// Gpu data accessed but hasn't changed.
        /// </summary>
        Unchanged = 1,

        /// <summary>
        /// Gpu data accessed and has changed. For example if a property of the data was updated.
        /// </summary>
        Changed = 2,
    }

    /// <summary>
    /// Available render modes.
    /// </summary>
    public enum RenderMode
    {
        /// <summary>
        /// Points are rendered by using gl_PointSize using a static mesh.
        /// </summary>
        StaticMesh = 0,

        /// <summary>
        /// Points are rendered by instanced rendering. Hardware needs to support geometry shaders.
        /// </summary>
        Instanced = 1,

        /// <summary>
        /// Points are rendered using gl_PointSize using a dynamic (editable) mesh.
        /// </summary>
        DynamicMesh = 2
    }

    /// <summary>
    /// Available point shapes.
    /// </summary>
    public enum PointShape
    {
        /// <summary>
        /// Every point appears as a rectangle.
        /// </summary>
        Rect = 0,
        /// <summary>
        /// Every point appears as a circle.
        /// </summary>
        Circle = 1,
        /// <summary>
        /// Every point appears as a rectangle with weighted Z-Buffer values to generate a paraboloid.
        /// </summary>
        Paraboloid = 2
    }

    /// <summary>
    /// Available point size modes.
    /// </summary>
    public enum PointSizeMode
    {
        /// <summary>
        /// Given point size is interpreted as diameter of the point in px
        /// </summary>
        FixedPixelSize = 0,

        /// <summary>
        /// Given point size is interpreted as diameter of the point in px for the initial camera position.
        /// If the camera gets closer, the point size in px will increase.
        /// </summary>
        FixedWorldSize = 1
    }

    /// <summary>
    /// Available Lighting methods.
    /// </summary>
    public enum PointCloudLighting
    {
        /// <summary>
        /// Albedo only - no lighting is calculated.
        /// </summary>
        Unlit = 0,

        /// <summary>
        /// Eye dome lighting
        /// </summary>
        Edl = 1
    }
}