using Newtonsoft.Json;

namespace Fusee.Math.Core
{
    /// <summary>
    /// Represents a ray with a given origin and direction.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public struct RayF
    {
        /// <summary>
        /// The point in world coordinates from which the ray originates.
        /// </summary>
        [JsonProperty(PropertyName = "Origin")]
        public float3 Origin;

        private float3 _direction;

        /// <summary>
        /// The direction of the ray.
        /// </summary>
        [JsonProperty(PropertyName = "Direction")]
        public float3 Direction
        {
            get { return _direction; }
            set
            {
                _direction = float3.Normalize(value);

                _inverseDirty = true;
            }
        }

        private float3 _inverse;
        private bool _inverseDirty;

        /// <summary>
        /// The inverse of the direction vector of the ray (1 / direction).
        /// </summary>
        public float3 Inverse
        {
            get
            {
                if (_inverseDirty)
                {
                    _inverse = new float3(1 / Direction.x, 1 / Direction.y, 1 / Direction.z);

                    _inverseDirty = false;
                }

                return _inverse;
            }
        }

        /// <summary>
        /// Create a new ray.
        /// </summary>
        /// <param name="origin_">The origin of the ray in world coordinates.</param>
        /// <param name="direction_">The direction of the ray.</param>
        public RayF(float3 origin_, float3 direction_)
        {
            Origin = origin_;

            _direction = float3.Normalize(direction_);

            _inverse = default;
            _inverseDirty = true;
        }

        /// <summary>
        /// Creates a new ray.
        /// </summary>
        /// <param name="pickPosClip">A mouse position in Clip Space.</param>
        /// <param name="view">The View Matrix of the rendered scene.</param>
        /// <param name="projection">The Projection Matrix of the rendered scene.</param>
        public RayF(float2 pickPosClip, float4x4 view, float4x4 projection)
        {
            float4x4 invViewProjection = float4x4.Invert(projection * view);

            var pickPosFarWorld = float4x4.TransformPerspective(invViewProjection, new float3(pickPosClip.x, pickPosClip.y, 1));
            var pickPosNearWorld = float4x4.TransformPerspective(invViewProjection, new float3(pickPosClip.x, pickPosClip.y, -1));

            Origin = pickPosNearWorld;

            _direction = (pickPosFarWorld - pickPosNearWorld).Normalize();

            _inverse = new float3(1 / _direction.x, 1 / _direction.y, 1 / _direction.z);
            _inverseDirty = false;
        }
    }
}