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
        RECT = 0,
        /// <summary>
        /// Every point appears as a circle.
        /// </summary>
        CIRCLE = 1,
        /// <summary>
        /// Every point appears as a rectangle with weighted Z-Buffer values to generate a paraboloid.
        /// </summary>
        PARABOLID = 2,
        /// <summary>
        /// Every point appears as a rectangle with weighted Z-Buffer values to generate a cone.
        /// </summary>
        CONE = 3,
        /// <summary>
        /// Every point appears as a rectangle with weighted Z-Buffer values to generate a hemisphere.
        /// </summary>
        HEMISPHERE = 4

    }

    /// <summary>
    /// Available color modes.
    /// </summary>
    public enum ColorMode
    {
        /// <summary>
        /// RGB value of the point.
        /// </summary>
        POINT,
        /// <summary>
        /// Every point has a single color.
        /// </summary>
        SINGLE,
        /// <summary>
        /// Color coded normals, if available, else every point is black.
        /// </summary>
        NORMAL,
        /// <summary>
        /// Weight in gray scale, if a point shape is used, that uses a weight function.
        /// </summary>
        WEIGHT,
        /// <summary>
        /// Non linear depth.
        /// </summary>
        DEPTH,
        /// <summary>
        /// Intensity, if available from the source point cloud, else every point is black.
        /// </summary>
        INTENSITY
    }

    /// <summary>
    /// Available point size modes.
    /// </summary>
    public enum PointSizeMode
    {
        /// <summary>
        /// Given point size is interpreted as diameter of the point in px
        /// </summary>
        FIXED_PIXELSIZE = 0,

        /// <summary>
        /// Given point size is interpreted as diameter of the point in px for the initial camera position.
        /// If the camera gets closer, the point size in px will increase.
        /// </summary>
        FIXED_WORLDSIZE = 1,

        /// <summary>
        /// Given point size is interpreted as diameter of the point in px for the initial camera position.
        /// If the camera gets closer, the point size in px will increase.
        /// Additionally the point size in px depends on the level and therefor the spacing of the octant a point lies in.
        /// </summary>
        NODELEVELDEPENDENT = 2,

        /// <summary>
        /// Given point size is interpreted as diameter of the point in px for the initial camera position.
        /// If the camera gets closer, the point size in px will increase.
        /// Additionally the size in px is adapted relative to a shader-calculated level of detail to allow the same point size across different octree levels.
        /// </summary>
        ADAPTIVE_SIZE = 3
    }

    /// <summary>
    /// Available Lighting methods.
    /// </summary>
    public enum Lighting
    {      
        /// <summary>
        /// Albedo only - no lighting is calculated.
        /// </summary>
        UNLIT = 0,
        /// <summary>
        /// Eye dome lighting
        /// </summary>
        EDL = 1,
        /// <summary>
        /// Diffuse lighting
        /// </summary>
        DIFFUSE = 2,
        /// <summary>
        /// Diffuse lighting with specular highlights.
        /// </summary>
        BLINN_PHONG = 3,   
        /// <summary>
        /// Only occlusion, calculated in screen space.
        /// </summary>
        SSAO_ONLY = 4
    }
}
