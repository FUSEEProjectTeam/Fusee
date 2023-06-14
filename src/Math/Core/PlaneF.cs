﻿using Newtonsoft.Json;
using System;

namespace Fusee.Math.Core
{
    /// <summary>
    /// Represents a plane in the form of Ax + By + Cz = D. The following applies: Ax + Bx + Cz - D = 0.
    /// The plane's normal equals n = (A, B, C) and may NOT necessarily be of unit length.
    /// The plane divides a space into two half-spaces.The direction plane's normal vector defines the "outer" or negative half-space.
    /// Points that lie in the positive half space of the plane do have a negative signed distance to the plane.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public struct PlaneF
    {
        /// <summary>
        /// The A plane coefficient.
        /// </summary>
        [JsonProperty(PropertyName = "A")]
        public float A
        {
            get => _a;
            set
            {
                _a = value;
                _normal.x = _a;
            }
        }
        private float _a = 0;

        /// <summary>
        /// The B plane coefficient.
        /// </summary>
        [JsonProperty(PropertyName = "B")]
        public float B
        {
            get => _b;
            set
            {
                _b = value;
                _normal.y = _b;
            }
        }
        private float _b = 0;

        /// <summary>
        /// The C plane coefficient.
        /// </summary>
        [JsonProperty(PropertyName = "C")]
        public float C
        {
            get => _c;
            set
            {
                _c = value;
                _normal.z = _c;
            }
        }
        private float _c = 1;

        /// <summary>
        /// The D plane coefficient.
        /// </summary>
        [JsonProperty(PropertyName = "D")]
        public float D = 1;

        /// <summary>
        /// The plane's normal vector. May NOT be of unit length if the plane isn't normalized.
        /// </summary>
        public float3 Normal
        {
            get
            {
                if (_normal == float3.Zero)
                    _normal = new float3(_a, _b, _c);

                return _normal;
            }
        }
        private float3 _normal = float3.UnitZ;

        /// <summary>
        /// Creates a plane from a normal and a point.
        /// </summary>
        /// <param name="normal">The plane's normal vector.</param>
        /// <param name="point">A point on the plane.</param>
        public PlaneF(float3 normal, float3 point)
        {
            A = normal.x;
            B = normal.y;
            C = normal.z;

            D = A * point.x + B * point.y + C * point.z;
        }

        /// <summary>
        /// Normalizes this plane.
        /// </summary>
        public PlaneF Normalize()
        {
            var mag = MathF.Sqrt(A * A + B * B + C * C);
            var a = A / mag;
            var b = B / mag;
            var c = C / mag;
            var d = D / mag;

            return new PlaneF() { A = a, B = b, C = c, D = d };
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
        public float SignedDistanceFromPoint(float3 pt)
        {
            return A * pt.x + B * pt.y + C * pt.z - D;
        }

        /// <summary>
        /// Calculates the angle between this plane and another one.
        /// </summary>
        /// <param name="other">The other plane to calculate the angle with.</param>
        public float AngleBetween(PlaneF other)
        {
            var numerator = MathF.Abs((A * other.A) + (B * other.B) + (C * other.C));
            var denominator = Normal.Length * other.Normal.Length;

            var cosAlpha = numerator / denominator;

            return MathF.Acos(cosAlpha);
        }

        #region Plane-Box Intersection

        /// <summary>
        /// Test whether a <see cref="AABBf"/> intersects this plane.
        /// See: Ericson 2005, Real Time Collision Detection, p. 161 - 164
        /// </summary>
        /// <param name="aabb">The axis aligned bounding box.</param> 
        public bool Intersects(AABBf aabb)
        {
            var r = BoxExtendInNormalDirection(aabb);
            var s = SignedDistanceFromPoint(aabb.Center);

            return MathF.Abs(s) <= r;
        }

        /// <summary>
        /// Test whether a cuboid intersects this plane.
        /// See: Ericson 2005, Real Time Collision Detection, p. 161 - 164
        /// </summary>
        /// <param name="center">The center of the cuboid.</param>
        /// <param name="size">The width, height and length of the cuboid.</param>
        public bool Intersects(float3 center, float3 size)
        {
            var r = BoxExtendInNormalDirection(size);
            var s = SignedDistanceFromPoint(center);

            return MathF.Abs(s) <= r;
        }

        /// <summary>
        /// Test whether a <see cref="OBBf"/> intersects this plane.
        /// See: Ericson 2005, Real Time Collision Detection, p. 161 - 164
        /// </summary>
        /// <param name="obb">The axis aligned bounding box.</param> 
        public bool Intersects(OBBf obb)
        {
            var r = BoxExtendInNormalDirection(obb);
            var s = SignedDistanceFromPoint(obb.Center);

            return MathF.Abs(s) <= r;
        }

        /// <summary>
        /// Test whether a cuboid intersects this plane.
        /// See: Ericson 2005, Real Time Collision Detection, p. 161 - 164
        /// CAREFUL: the definition whats completely inside and outside is flipped in comparison to Ericson, 
        /// because FUSEE defines a point with a negative signed distance to be inside.
        /// </summary>
        /// <param name="center">The center of the cuboid.</param>
        /// <param name="size">The width, height and length of the cuboid.</param>
        public bool InsideOrIntersecting(float3 center, float3 size)
        {
            var r = BoxExtendInNormalDirection(size);

            //Distance from center to plane
            var s = SignedDistanceFromPoint(center);

            //Completely inside
            if (s <= -r)
                return true;
            //Completely outside
            else if (r <= s)
                return false;
            //else intersecting
            return true;
        }

        /// <summary>
        /// Test whether a cuboid intersects this plane.
        /// See: Ericson 2005, Real Time Collision Detection, p. 161 - 164
        /// CAREFUL: the definition whats completely inside and outside is flipped in comparison to Ericson, 
        /// because FUSEE defines a point with a negative signed distance to be inside.
        /// </summary>
        /// <param name="center">The center of the cuboid.</param>
        /// <param name="size">The width, height and length of the cuboid.</param>
        public bool InsideOrIntersecting(float3 center, float size)
        {
            var r = BoxExtendInNormalDirection(size);

            //Distance from aabb center to plane
            var s = SignedDistanceFromPoint(center);

            //Completely inside
            if (s <= -r)
                return true;
            //Completely outside
            else if (r <= s)
                return false;
            //else intersecting
            return true;
        }

        /// <summary>
        /// Test whether a <see cref="AABBf"/> intersects this plane.
        /// See: Ericson 2005, Real Time Collision Detection, p. 161 - 164
        /// CAREFUL: the definition whats completely inside and outside is flipped in comparison to Ericson, 
        /// because FUSEE defines a point with a negative signed distance to be inside.
        /// </summary>
        /// <param name="aabb">The axis aligned bounding box.</param> 
        public bool InsideOrIntersecting(AABBf aabb)
        {
            var r = BoxExtendInNormalDirection(aabb);

            //Distance from aabb center to plane
            var s = SignedDistanceFromPoint(aabb.Center);

            //Completely inside
            if (s <= -r)
                return true;
            //Completely outside
            else if (r <= s)
                return false;
            //else intersecting
            return true;
        }

        /// <summary>
        /// Test whether a <see cref="OBBf"/> intersects this plane.
        /// See: Ericson 2005, Real Time Collision Detection, p. 161 - 164
        /// CAREFUL: the definition whats completely inside and outside is flipped in comparison to Ericson, 
        /// because FUSEE defines a point with a negative signed distance to be inside.
        /// </summary>
        /// <param name="obb">The object oriented bounding box.</param> 
        public bool InsideOrIntersecting(OBBf obb)
        {
            var r = BoxExtendInNormalDirection(obb);
            //Distance from obb center to plane
            var s = SignedDistanceFromPoint(obb.Center);

            //Completely outside
            if (s <= -r)
                return true;
            //Completely inside
            else if (r <= s)
                return false;
            //else intersecting
            return true;
        }

        /// <summary>
        /// Calculates the projection interval radius of an cuboid onto line L(t) = cuboid.Center + t * plane.Normal (extend (radius) in direction of the plane normal).      
        /// <param name="size">The width, height and length of a cuboid.</param>
        /// </summary>
        private double BoxExtendInNormalDirection(float3 size)
        {
            var boxExtend = size * 0.5f;
            return boxExtend.x * MathF.Abs(Normal.x) + boxExtend.y * MathF.Abs(Normal.y) + boxExtend.z * MathF.Abs(Normal.z);
        }

        /// <summary>
        /// Calculates the projection interval radius of an cuboid onto line L(t) = cuboid.Center + t * plane.Normal (extend (radius) in direction of the plane normal).      
        /// <param name="size">The width, height and length of a cuboid.</param>
        /// </summary>
        private double BoxExtendInNormalDirection(float size)
        {
            var boxExtend = size * 0.5f;
            return boxExtend * MathF.Abs(Normal.x) + boxExtend * MathF.Abs(Normal.y) + boxExtend * MathF.Abs(Normal.z);
        }

        /// <summary>
        /// Calculates the projection interval radius of aabb onto line L(t) = aabb.Center + t * plane.Normal (extend (radius) in direction of the plane normal).      
        /// <param name="aabb">The axis aligned bounding box.</param>
        /// </summary>
        private float BoxExtendInNormalDirection(AABBf aabb)
        {
            var boxExtend = aabb.Size * 0.5f;
            return boxExtend.x * MathF.Abs(Normal.x) + boxExtend.y * MathF.Abs(Normal.y) + boxExtend.z * MathF.Abs(Normal.z);
        }

        /// <summary>
        /// Calculates the projection interval radius of obb onto line L(t) = aabb.Center + t * plane.Normal (extend (radius) in direction of the plane normal).      
        /// <param name="obb">The object oriented bounding box.</param>
        /// </summary>
        private float BoxExtendInNormalDirection(OBBf obb)
        {
            var transformationMat = obb.Rotation * float4x4.CreateTranslation(obb.Translation); //without scale!

            var xAxis = (float3.UnitX * transformationMat).Normalize();
            var yAxis = (float3.UnitY * transformationMat).Normalize();
            var zAxis = (float3.UnitZ * transformationMat).Normalize();

            var boxExtend = obb.Size * 0.5f;

            return boxExtend.x * MathF.Abs(float3.Dot(Normal, xAxis)) +
                    boxExtend.y * MathF.Abs(float3.Dot(Normal, yAxis)) +
                    boxExtend.z * MathF.Abs(float3.Dot(Normal, zAxis));
        }

        #endregion

        #region Operators

        /// <summary>
        /// Operator override for multiplying a Plane with a float.
        /// </summary>
        /// <param name="plane">The plane.</param>
        /// <param name="scalar">The scalar value.</param>
        /// <returns></returns>
        public static PlaneF operator *(PlaneF plane, float scalar)
        {
            return new PlaneF()
            {
                A = plane.A * scalar,
                B = plane.B * scalar,
                C = plane.C * scalar,
                D = plane.D * scalar
            };
        }

        /// <summary>
        /// Operator override for equality.
        /// </summary>
        /// <param name="left">The plane.</param>
        /// <param name="right">The scalar value.</param>        
        public static bool operator ==(PlaneF left, PlaneF right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Operator override for inequality.
        /// </summary>
        /// <param name="left">The plane.</param>
        /// <param name="right">The scalar value.</param>        
        public static bool operator !=(PlaneF left, PlaneF right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Indicates whether this plane is equal to another object.
        /// </summary>
        /// <param name="obj">The object. This method will throw an exception if the object isn't of type <see cref="PlaneF"/>.</param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (obj?.GetType() != typeof(PlaneF)) throw new ArgumentException($"{obj} is not of Type 'Plane'.");

            var other = (PlaneF)obj;
            return
                MathF.Abs(A - other.A) < M.EpsilonFloat &&
                MathF.Abs(B - other.B) < M.EpsilonFloat &&
                MathF.Abs(C - other.C) < M.EpsilonFloat &&
                MathF.Abs(D - other.D) < M.EpsilonFloat;
        }

        /// <summary>
        /// Generates a hash code for this plane.
        /// </summary>        
        public override int GetHashCode()
        {
            return HashCode.Combine(A, B, C, D);
        }

        #endregion
    }
}