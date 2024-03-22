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
                    _inverse = new float3(1 / Direction.x, 1 / Direction.y, 1 / Direction.z).Normalize();
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
        /// Creates a new ray in world space using clip coordinates.
        /// Origin is the camera's world space position.
        /// </summary>
        /// <param name="pickPosClip">A mouse position in Clip Space.</param>
        /// <param name="view">The View Matrix of the rendered scene.</param>
        /// <param name="projection">The Projection Matrix of the rendered scene.</param>
        /// <param name="isOrthographic">Is the projection matrix a orthographic one?</param>
        public RayF(float2 pickPosClip, float4x4 view, float4x4 projection, bool isOrthographic = false)
        {
            if (!isOrthographic)
            {
                //No need for perspective devision here. Ray has no intrinsic depth.
                //Doesn't work for ortho projection.
                //Numerically more stable because of the missing perspective division.
                float4 rayClip = new(pickPosClip.x, pickPosClip.y, 1.0f, 1.0f);
                float4 rayEye = float4x4.Invert(projection) * rayClip;
                rayEye.z = 1.0f;
                rayEye.w = 0.0f;

                var invView = float4x4.Invert(view);

                float3 rayWorld = (invView * rayEye).xyz.Normalize();
                Origin = invView.Column4.xyz;
                Direction = rayWorld;
            }
            else
            {
                float4x4 invViewProjection = float4x4.Invert(projection * view);
                var pickPosFarWorld = invViewProjection * new float3(pickPosClip.x, pickPosClip.y, 1);
                var pickPosNearWorld = invViewProjection * new float3(pickPosClip.x, pickPosClip.y, -1);

                Origin = pickPosNearWorld;
                Direction = (pickPosFarWorld - pickPosNearWorld).Normalize();
            }
        }
    }
}