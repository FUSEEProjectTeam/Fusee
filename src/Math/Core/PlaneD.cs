using System;

namespace Fusee.Math.Core
{
    /// <summary>
    /// Represents a plane in the form of Ax + By + Cz = D. The following applies: Ax + Bx + Cz - D = 0.
    /// The plane's normal equals n = (A, B, C) and may NOT necessarily be of unit length.
    /// The plane divides a space into two half-spaces.The direction plane's normal vector defines the "outer" or negative half-space.
    /// Points that lie in the positive half space of the plane do have a negative signed distance to the plane.
    /// </summary>
    public struct PlaneD
    {
        /// <summary>
        /// The A plane coefficient.
        /// </summary>
        public double A
        {
            get
            {
                return _a;
            }
            set
            {
                _a = value;
                _normal.x = _a;
            }
        }
        private double _a;

        /// <summary>
        /// The B plane coefficient.
        /// </summary>
        public double B
        {
            get
            {
                return _b;
            }
            set
            {
                _b = value;
                _normal.y = _b;
            }
        }
        private double _b;

        /// <summary>
        /// The C plane coefficient.
        /// </summary>
        public double C
        {
            get
            {
                return _c;
            }
            set
            {
                _c = value;
                _normal.z = _c;
            }
        }
        private double _c;

        /// <summary>
        /// The D plane coefficient.
        /// </summary>
        public double D;

        /// <summary>
        /// The plane's normal vector. May NOT be of unit length if the plane isn't normalized.
        /// </summary>
        public double3 Normal
        {
            get
            {
                if (_normal == double3.Zero)
                    _normal = new double3(_a, _b, _c);

                return _normal;
            }
        }
        private double3 _normal;

        /// <summary>
        /// Normalizes this plane.
        /// </summary>
        public PlaneD Normalize()
        {
            var mag = System.Math.Sqrt(A * A + B * B + C * C);
            var a = A / mag;
            var b = B / mag;
            var c = C / mag;
            var d = D / mag;

            return new PlaneD() { A = a, B = b, C = c, D = d };
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
        public double SignedDistanceFromPoint(double3 pt)
        {
            return A * pt.x + B * pt.y + C * pt.z - D;
        }

        /// <summary>
        /// Calculates the angle between this plane and another one.
        /// </summary>
        /// <param name="other">The other plane to calculate the angle with.</param>        
        public double AngleBetween(PlaneD other)
        {
            var numerator = System.Math.Abs((A * other.A) + (B * other.B) + (C * other.C));
            var denominator = Normal.Length * other.Normal.Length;

            var cosAlpha = numerator / denominator;

            return System.Math.Acos(cosAlpha);
        }

        #region Plane-Box Intersection

        /// <summary>
        /// Test whether a <see cref="AABBd"/> intersects this plane.
        /// See: Ericson 2005, Real Time Collision Detection, p. 161 - 164
        /// </summary>
        /// <param name="aabb">The axis aligned bounding box.</param> 
        public bool Intersects(AABBd aabb)
        {
            var r = BoxExtendInNormalDirection(aabb);
            var s = SignedDistanceFromPoint(aabb.Center);

            return System.Math.Abs(s) <= r;
        }

        /// <summary>
        /// Test whether a cuboid intersects this plane.
        /// See: Ericson 2005, Real Time Collision Detection, p. 161 - 164
        /// </summary>
        /// <param name="center">The center of the cuboid.</param>
        /// <param name="size">The width, height and length of the cuboid.</param>
        public bool Intersects(double3 center, double3 size)
        {
            var r = BoxExtendInNormalDirection(size);
            var s = SignedDistanceFromPoint(center);

            return System.Math.Abs(s) <= r;
        }

        /// <summary>
        /// Test whether a <see cref="OBBd"/> intersects this plane.
        /// See: Ericson 2005, Real Time Collision Detection, p. 161 - 164
        /// </summary>
        /// <param name="obb">The axis aligned bounding box.</param> 
        public bool Intersects(OBBd obb)
        {
            var r = BoxExtendInNormalDirection(obb);
            var s = SignedDistanceFromPoint(obb.Center);

            return System.Math.Abs(s) <= r;
        }

        /// <summary>
        /// Test whether a cuboid intersects this plane.
        /// See: Ericson 2005, Real Time Collision Detection, p. 161 - 164
        /// CAREFUL: the definition whats completely inside and outside is flipped in comparison to Ericson, 
        /// because FUSEE defines a point with a negative signed distance to be inside.
        /// </summary>
        /// <param name="center">The center of the cuboid.</param>
        /// <param name="size">The width, height and length of the cuboid.</param>
        public bool InsideOrIntersecting(double3 center, double3 size)
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
        /// Test whether a cuboid intersects this plane.
        /// See: Ericson 2005, Real Time Collision Detection, p. 161 - 164
        /// CAREFUL: the definition whats completely inside and outside is flipped in comparison to Ericson, 
        /// because FUSEE defines a point with a negative signed distance to be inside.
        /// </summary>
        /// <param name="center">The center of the cuboid.</param>
        /// <param name="size">The width, height and length of the cuboid.</param>
        public bool InsideOrIntersecting(double3 center, double size)
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
        /// Test whether a <see cref="AABBd"/> intersects this plane.
        /// See: Ericson 2005, Real Time Collision Detection, p. 161 - 164
        /// CAREFUL: the definition whats completely inside and outside is flipped in comparison to Ericson, 
        /// because FUSEE defines a point with a negative signed distance to be inside.
        /// </summary>
        /// <param name="aabb">The axis aligned bounding box.</param> 
        public bool InsideOrIntersecting(AABBd aabb)
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
        /// Test whether a <see cref="OBBd"/> intersects this plane.
        /// See: Ericson 2005, Real Time Collision Detection, p. 161 - 164
        /// CAREFUL: the definition whats completely inside and outside is flipped in comparison to Ericson, 
        /// because FUSEE defines a point with a negative signed distance to be inside.
        /// </summary>
        /// <param name="obb">The object oriented bounding box.</param> 
        public bool InsideOrIntersecting(OBBd obb)
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
        /// Calculates the projection interval radius of aabb onto line L(t) = aabb.Center + t * plane.Normal (extend (radius) in direction of the plane normal).      
        /// <param name="aabb">The axis aligned bounding box.</param>
        /// </summary>
        private double BoxExtendInNormalDirection(AABBd aabb)
        {
            var boxExtend = aabb.Size * 0.5f;
            return boxExtend.x * System.Math.Abs(Normal.x) + boxExtend.y * System.Math.Abs(Normal.y) + boxExtend.z * System.Math.Abs(Normal.z);
        }

        /// <summary>
        /// Calculates the projection interval radius of an cuboid onto line L(t) = cuboid.Center + t * plane.Normal (extend (radius) in direction of the plane normal).      
        /// <param name="size">The width, height and length of a cuboid.</param>
        /// </summary>
        private double BoxExtendInNormalDirection(double3 size)
        {
            var boxExtend = size * 0.5f;
            return boxExtend.x * System.Math.Abs(Normal.x) + boxExtend.y * System.Math.Abs(Normal.y) + boxExtend.z * System.Math.Abs(Normal.z);
        }

        /// <summary>
        /// Calculates the projection interval radius of an cuboid onto line L(t) = cuboid.Center + t * plane.Normal (extend (radius) in direction of the plane normal).      
        /// <param name="size">The width, height and length of a cuboid.</param>
        /// </summary>
        private double BoxExtendInNormalDirection(double size)
        {
            var boxExtend = size * 0.5f;
            return boxExtend * System.Math.Abs(Normal.x) + boxExtend * System.Math.Abs(Normal.y) + boxExtend * System.Math.Abs(Normal.z);
        }

        /// <summary>
        /// Calculates the projection interval radius of obb onto line L(t) = aabb.Center + t * plane.Normal (extend (radius) in direction of the plane normal).      
        /// <param name="obb">The object oriented bounding box.</param>
        ///</summary>
        private double BoxExtendInNormalDirection(OBBd obb)
        {
            var transformationMat = obb.Rotation * double4x4.CreateTranslation(obb.Translation); //without scale!

            var xAxis = (double3.UnitX * transformationMat).Normalize();
            var yAxis = (double3.UnitY * transformationMat).Normalize();
            var zAxis = (double3.UnitZ * transformationMat).Normalize();

            var boxExtend = obb.Size * 0.5f;

            return boxExtend.x * System.Math.Abs(double3.Dot(Normal, xAxis)) +
                    boxExtend.y * System.Math.Abs(double3.Dot(Normal, yAxis)) +
                    boxExtend.z * System.Math.Abs(double3.Dot(Normal, zAxis));
        }

        #endregion

        #region Operators

        /// <summary>
        /// Operator override for multiplying a Plane with a double.
        /// </summary>
        /// <param name="plane">The plane.</param>
        /// <param name="scalar">The scalar value.</param>
        /// <returns></returns>
        public static PlaneD operator *(PlaneD plane, double scalar)
        {
            return new PlaneD()
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
        public static bool operator ==(PlaneD left, PlaneD right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Operator override for inequality.
        /// </summary>
        /// <param name="left">The plane.</param>
        /// <param name="right">The scalar value.</param>        
        public static bool operator !=(PlaneD left, PlaneD right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Indicates whether this plane is equal to another object.
        /// </summary>
        /// <param name="obj">The object. This method will throw an exception if the object isn't of type <see cref="PlaneD"/>.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(PlaneD)) throw new ArgumentException($"{obj} is not of Type 'Plane'.");

            var other = (PlaneD)obj;
            return
                System.Math.Abs(A - other.A) < M.EpsilonDouble &&
                System.Math.Abs(B - other.B) < M.EpsilonDouble &&
                System.Math.Abs(C - other.C) < M.EpsilonDouble &&
                System.Math.Abs(D - other.D) < M.EpsilonDouble;
        }

        /// <summary>
        /// Generates a hash code for this plane.
        /// </summary>        
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 29 + A.GetHashCode();
                hash = hash * 29 + B.GetHashCode();
                hash = hash * 29 + C.GetHashCode();
                hash = hash * 29 + D.GetHashCode();
                return hash;
            }
        }

        #endregion
    }
}