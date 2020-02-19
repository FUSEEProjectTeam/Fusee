namespace Fusee.Math.Core
{
    /// <summary>
    /// Represents a plane in the form of Ax + By + Cz + D = 0    
    /// </summary>
    public struct Plane
    {
        /// <summary>
        /// The A plane coefficient.
        /// </summary>
        public float A
        {
            get
            {
                return _a;
            }
            set
            {
                _a = value;
                _normal = new float3(A, B, C);
            }
        }
        private float _a;

        /// <summary>
        /// The B plane coefficient.
        /// </summary>
        public float B
        {
            get
            {
                return _b;
            }
            set
            {
                _b = value;
                _normal = new float3(A, B, C);
            }
        }
        private float _b;

        /// <summary>
        /// The C plane coefficient.
        /// </summary>
        public float C
        {
            get
            {
                return _c;
            }
            set
            {
                _c = value;
                _normal = new float3(A, B, C);
            }
        }
        private float _c;

        /// <summary>
        /// The D plane coefficient.
        /// </summary>
        public float D
        {
            get
            {
                return _d;
            }
            set
            {
                _d = value;
                _normal = new float3(A, B, C);
            }
        }
        private float _d;

        /// <summary>
        /// The plane's normal vector. May NOT be of unit length if the plane isn't normalized.
        /// </summary>
        public float3 Normal
        {
            get
            {
                if (_normal == float3.Zero)
                    _normal = new float3(A, B, C);

                return _normal;
            }
        }
        private float3 _normal;

        /// <summary>
        /// Normalizes this plane.
        /// </summary>
        public void Normalize()
        {
            var mag = (float)System.Math.Sqrt(A * A + B * B + C * C);
            A /= mag;
            B /= mag;
            C /= mag;
            D /= mag;

            _normal = new float3(A, B, C);
        }

        /// <summary>
        /// Returns the signed distance from a point to this plane.
        /// If the plane isn't normalized this may not be the euclidean distance!
        /// For normalized and unnormalized planes the following is true:
        /// 1.If the distance is negative, the point lies in the negative half-space.
        /// 2.If 0 = dist, the point lies in the plane.
        /// 3.If the distance is positive the point lies in the positive half-space.
        /// </summary>
        /// <param name="pt">An arbitrary point.</param>
        public float DistancePointToPlane(float3 pt)
        {
            //Is equal to float3.Dot(Normal, pt) + w if the plane is normalized.
            return A * pt.x + B * pt.y + C * pt.z + D;
        }

        /// <summary>
        /// Test whether a <see cref="AABBf"/> intersects this plane.
        /// See: Ericson 2005, Real Time Collision Detection, p. 161 - 164
        /// </summary>
        /// <param name="aabb">The axis aligned bounding box.</param> 
        public bool Intersects(AABBf aabb)
        {
            var r = BoxExtendInNormalDirection(aabb);
            var s = SignedDistanceBoxCenter(aabb);

            return System.Math.Abs(s) <= r;
        }

        /// <summary>
        /// Test whether a <see cref="OBBf"/> intersects this plane.
        /// See: Ericson 2005, Real Time Collision Detection, p. 161 - 164
        /// </summary>
        /// <param name="obb">The axis aligned bounding box.</param> 
        public bool Intersects(OBBf obb)
        {
            var r = BoxExtendInNormalDirection(obb);
            var s = SignedDistanceBoxCenter(obb);

            return System.Math.Abs(s) <= r;
        }

        /// <summary>
        /// Test whether a <see cref="AABBf"/> intersects this plane.
        /// See: Ericson 2005, Real Time Collision Detection, p. 161 - 164
        /// </summary>
        /// <param name="aabb">The axis aligned bounding box.</param> 
        public bool InsideOrIntersectingAABB(AABBf aabb)
        {
            var r = BoxExtendInNormalDirection(aabb);
            //Distance from aabb center to plane
            var s = DistancePointToPlane(aabb.Center);

            //Completely outside
            if (s <= -r)
                return false;
            //Completely inside
            else if (r <= s)
                return true;
            //else intersecting
            return true;
        }

        /// <summary>
        /// Calculates the projection interval radius of aabb onto line L(t) = aabb.Center + t * plane.Normal (extend (radius) in direction of the plane normal).      
        /// <param name="aabb">The axis aligned bounding box.</param> 
        private float BoxExtendInNormalDirection(AABBf aabb)
        {
            var boxExtend = aabb.Size * 0.5f;
            return boxExtend.x * System.Math.Abs(Normal.x) + boxExtend.y * System.Math.Abs(Normal.y) + boxExtend.z * System.Math.Abs(Normal.z);
        }

        /// <summary>
        /// Calculates the projection interval radius of obb onto line L(t) = aabb.Center + t * plane.Normal (extend (radius) in direction of the plane normal).      
        /// <param name="aabb">The axis aligned bounding box.</param> 
        private float BoxExtendInNormalDirection(OBBf obb)
        {
            var transformationMat = obb.Rotation * float4x4.CreateTranslation(obb.Translation); //without scale!

            var xAxis = float3.UnitX * transformationMat;
            var yAxis = float3.UnitY * transformationMat;
            var zAxis = float3.UnitZ * transformationMat;

            var boxExtend = obb.Size * 0.5f;

            return boxExtend.x * System.Math.Abs(float3.Dot(Normal, xAxis)) +
                    boxExtend.y * System.Math.Abs(float3.Dot(Normal, yAxis)) +
                    boxExtend.z * System.Math.Abs(float3.Dot(Normal, zAxis));
        }

        /// <summary>
        /// Calculates the projection interval radius of aabb onto line L(t) = aabb.Center + t * plane.Normal (extend (radius) in direction of the plane normal).      
        /// <param name="aabb">The axis aligned bounding box.</param> 
        private float SignedDistanceBoxCenter(AABBf aabb)
        {
            return float3.Dot(aabb.Center, Normal) + D;
        }

        /// <summary>
        /// Calculates the projection interval radius of obb onto line L(t) = aabb.Center + t * plane.Normal (extend (radius) in direction of the plane normal).      
        /// <param name="aabb">The axis aligned bounding box.</param> 
        private float SignedDistanceBoxCenter(OBBf obb)
        {
            return float3.Dot(obb.Center, Normal) + D;
        }
    }
}
