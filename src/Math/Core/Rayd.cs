namespace Fusee.Math.Core
{
    /// <summary>
    /// Represents a ray with a given origin and direction.
    /// </summary>
    public struct RayD
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
        public RayD(double3 origin_, double3 direction_)
        {
            Origin = origin_;
            Direction = double3.Normalize(direction_);
            Inverse = new double3(1 / Direction.x, 1 / Direction.y, 1 / Direction.z);
        }

        /// <summary>
        /// Creates a new ray.
        /// </summary>
        /// <param name="pickPosClip">A mouse position in Clip Space.</param>
        /// <param name="view">The View Matrix of the rendered scene.</param>
        /// <param name="projection">The Projection Matrix of the rendered scene.</param>
        public RayD(double2 pickPosClip, double4x4 view, double4x4 projection)
        {
            Origin = double4x4.Invert(view).Translation();

            double4x4 invViewProjection = double4x4.Invert(projection * view);

            var pickPosWorld4 = double4x4.Transform(invViewProjection, new double4(pickPosClip.x, pickPosClip.y, 1, 1));
            var pickPosWorld = (pickPosWorld4 / pickPosWorld4.w).xyz;

            Direction = (pickPosWorld - Origin).Normalize();

            Inverse = new double3(1 / Direction.x, 1 / Direction.y, 1 / Direction.z);
        }
    }
}