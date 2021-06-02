using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Math.Core
{
    class Rayd
    {
        /// <summary>
        /// The point in world coordinates from which the ray originates.
        /// </summary>
        public double3 Origin;

        /// <summary>
        /// The direction of the ray.
        /// </summary>
        public double3 Direction { get; private set; }

        /// <summary>
        /// The inverse of the direction vector of the ray (1 / direction).
        /// </summary>
        public double3 Inverse { get; private set; }

        /// <summary>
        /// Create a new ray.
        /// </summary>
        /// <param name="origin_">The origin of the ray in world coordinates.</param>
        /// <param name="direction_">The direction of the ray.</param>
        public Rayd(double3 origin_, double3 direction_)
        {
            Origin = origin_;
            Direction = double3.Normalize(direction_);
            Inverse = new double3(1 / Direction.x, 1 / Direction.y, 1 / Direction.z);
        }
    }
}
