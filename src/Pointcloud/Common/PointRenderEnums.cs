namespace Fusee.Pointcloud.Common
{

    public enum PointShape
    {
        RECT = 0,
        CIRCLE = 1,
        PARABOLID = 2,
        CONE = 3,
        SPHERE = 4

    }

    public enum ColorMode
    {
        POINT,
        SINGLE,
        NORMAL,
        WEIGHT,
        DEPTH,
        INTENSITY
    }

    public enum PointSizeMode
    {
        /// <summary>
        /// Given point size is interpreted as diamter of the point in px
        /// </summary>
        FIXED_PIXELSIZE = 0,

        /// <summary>
        /// Given point size is interpreted as diamter of the point in px for the initial camera position.
        /// If the camera gets closer, the point size in px will increase.
        /// </summary>
        FIXED_WORLDSIZE = 1,

        /// <summary>
        /// Given point size is interpreted as diamter of the point in px for the initial camera position.
        /// If the camera gets closer, the point size in px will increase.
        /// Additionally the point size in px depends on the level and therefor the spacing of the octant a point lies in.
        /// </summary>
        NODELEVELDEPENDENT = 2,

        /// <summary>
        /// Given point size is interpreted as diamter of the point in px for the initial camera position.
        /// If the camera gets closer, the point size in px will increase.
        /// Additionally the size in px is adapted relative to the level and therefor the spacing of the octant a point lies in to allow the same point size in different octree levels.
        /// </summary>
        ADAPTIVE_SIZE = 3
    }

    public enum Lighting
    {
        UNLIT = 0,
        EDL = 1,
        DIFFUSE = 2,
        BLINN_PHONG = 3
    }
}
