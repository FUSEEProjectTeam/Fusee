using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Node for use in an Grid-like structure.
    /// </summary>
    /// <typeparam name="T">Defines the type of the payload.</typeparam>
    public class Bucket<T>
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
        protected Bucket() { }

        /// <summary>
        /// Creates a new instance of type Bucket.
        /// </summary>
        public Bucket(double3 center, double size)
        {
            Center = center;
            Size = size;
        }
    }
}