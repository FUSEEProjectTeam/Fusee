using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Math.Core
{
    /// <summary>
    /// Represents a ray with a given origin and direction.
    /// </summary>
    public struct Rayf
    {
        /// <summary>
        /// The point in world coordinates from which the ray originates.
        /// </summary>
        public float3 Origin;

        /// <summary>
        /// The direction of the ray.
        /// </summary>
        public float3 Direction { get; private set; }

        /// <summary>
        /// The inverse of the direction vector of the ray (1 / direction).
        /// </summary>
        public float3 Inverse { get; private set; }

        /// <summary>
        /// Create a new ray.
        /// </summary>
        /// <param name="origin_">The origin of the ray in world coordinates.</param>
        /// <param name="direction_">The direction of the ray.</param>
        public Rayf(float3 origin_, float3 direction_)
        {
            Origin = origin_;
            Direction = float3.Normalize(direction_);
            Inverse = new float3(1 / Direction.x, 1 / Direction.y, 1 / Direction.z);
        }
    }
}