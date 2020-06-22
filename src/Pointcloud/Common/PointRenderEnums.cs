namespace Fusee.Pointcloud.Common
{
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
        Paraboloid = 2,
        /// <summary>
        /// Every point appears as a rectangle with weighted Z-Buffer values to generate a cone.
        /// </summary>
        Cone = 3,
        /// <summary>
        /// Every point appears as a rectangle with weighted Z-Buffer values to generate a hemisphere.
        /// </summary>
        Hemisphere = 4

    }

    /// <summary>
    /// Available color modes.
    /// </summary>
    public enum ColorMode
    {
        /// <summary>
        /// RGB value of the point.
        /// </summary>
        Point,
        /// <summary>
        /// Every point has a single color.
        /// </summary>
        Single,
        /// <summary>
        /// Color coded normals, if available, else every point is black.
        /// </summary>
        Normal,
        /// <summary>
        /// Weight in gray scale, if a point shape is used, that uses a weight function.
        /// </summary>
        Weight,
        /// <summary>
        /// Non linear depth.
        /// </summary>
        Depth,
        /// <summary>
        /// Intensity, if available from the source point cloud, else every point is black.
        /// </summary>
        Intensity
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
        FixedWorldSize = 1,

        /// <summary>
        /// Given point size is interpreted as diameter of the point in px for the initial camera position.
        /// If the camera gets closer, the point size in px will increase.
        /// Additionally the point size in px depends on the level and therefor the spacing of the octant a point lies in.
        /// </summary>
        NodeLevelDependent = 2,

        /// <summary>
        /// Given point size is interpreted as diameter of the point in px for the initial camera position.
        /// If the camera gets closer, the point size in px will increase.
        /// Additionally the size in px is adapted relative to a shader-calculated level of detail to allow the same point size across different octree levels.
        /// </summary>
        AdaptiveSize = 3
    }

    /// <summary>
    /// Available Lighting methods.
    /// </summary>
    public enum Lighting
    {      
        /// <summary>
        /// Albedo only - no lighting is calculated.
        /// </summary>
        Unlit = 0,
        /// <summary>
        /// Eye dome lighting
        /// </summary>
        Edl = 1,
        /// <summary>
        /// Diffuse lighting
        /// </summary>
        Diffuse = 2,
        /// <summary>
        /// Diffuse lighting with specular highlights.
        /// </summary>
        BlinnPhong = 3,   
        /// <summary>
        /// Only occlusion, calculated in screen space.
        /// </summary>
        SsaoOnly = 4
    }
}
