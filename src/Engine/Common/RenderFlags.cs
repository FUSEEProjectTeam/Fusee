using System;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Helps with automatically building shader code for special render cases like point cloud rendering, bone animation and instanced rendering.
    /// </summary>
    [Flags]
    public enum RenderFlags
    {
        /// <summary>
        /// No render modification.
        /// </summary>
        None = 1,

        /// <summary>
        /// Flag for rendering bones.
        /// </summary>
        Bones = 2,

        /// <summary>
        /// Flag for rendering using gpu instancing.
        /// </summary>
        Instanced = 4,

        /// <summary>
        /// Flag for rendering point clouds.
        /// </summary>
        PointCloud = 8,
    }
}