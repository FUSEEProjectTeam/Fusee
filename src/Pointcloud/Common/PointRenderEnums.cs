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

    public enum PointMode
    {
        FIXED_SIZE,
        ADAPTABLE_SIZE
    }

    public enum Lighting
    {
        UNLIT = 0,
        EDL = 1,
        DIFFUSE = 2,
        BLINN_PHONG = 3
    }
}
