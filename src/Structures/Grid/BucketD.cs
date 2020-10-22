using Fusee.Math.Core;

namespace Fusee.Structures.Grid
{
    /// <summary>
    /// Node for use in an Grid-like structure.
    /// </summary>
    public class BucketD
    {
        /// <summary>
        /// Center of this Bucket in world space coordinates.
        /// </summary>
        public double3 Center { get; protected set; }

        /// <summary>
        /// Length, width and height of this Octant.
        /// </summary>
        public double Size { get; protected set; }


        /// <summary>
        /// Creates a new instance of type Bucket.
        /// </summary>
        protected BucketD() { }

        /// <summary>
        /// Creates a new instance of type Bucket.
        /// </summary>
        public BucketD(double3 center, double size)
        {
            Center = center;
            Size = size;
        }
    }
}