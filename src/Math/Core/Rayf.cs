namespace Fusee.Math.Core
{
    /// <summary>
    /// Represents a ray with a given origin and direction.
    /// </summary>
    public struct RayF
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
        public RayF(float3 origin_, float3 direction_)
        {
            Origin = origin_;
            Direction = float3.Normalize(direction_);
            Inverse = new float3(1 / Direction.x, 1 / Direction.y, 1 / Direction.z);
        }

        /// <summary>
        /// Creates a new ray.
        /// </summary>
        /// <param name="pickPosClip">A mouse position in Clip Space.</param>
        /// <param name="view">The View Matrix of the rendered scene.</param>
        /// <param name="projection">The Projection Matrix of the rendered scene.</param>
        public RayF(float2 pickPosClip, float4x4 view, float4x4 projection)
        {
            Origin = float4x4.Invert(view).Translation();

            float4x4 invViewProjection = float4x4.Invert(projection * view);

            var pickPosWorld4 = float4x4.Transform(invViewProjection, new float4(pickPosClip.x, pickPosClip.y, 1, 1));
            var pickPosWorld = (pickPosWorld4 / pickPosWorld4.w).xyz;

            Direction = (pickPosWorld - Origin).Normalize();

            Inverse = new float3(1 / Direction.x, 1 / Direction.y, 1 / Direction.z);
        }
    }
}