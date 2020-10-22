using Fusee.Math.Core;

namespace Fusee.Structures.Grid
{
    /// <summary>
    /// Node for use in an Grid-like structure.
    /// </summary>
    public class BucketF
    {
        /// <summary>
        /// Center of this Bucket in world space coordinates.
        /// </summary>
        public float3 Center { get; protected set; }

        /// <summary>
        /// Length, width and height of this Octant.
        /// </summary>
        public float Size { get; protected set; }

        /// <summary>
        /// Creates a new instance of type Bucket.
        /// </summary>
        protected BucketF() { }

        /// <summary>
        /// Creates a new instance of type Bucket.
        /// </summary>
        public BucketF(float3 center, float size)
        {
            Center = center;
            Size = size;
        }
    }
}