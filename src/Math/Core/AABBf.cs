using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Runtime.InteropServices;

namespace Fusee.Math.Core
{
    /// <summary>
    ///     Represents an axis aligned bounding box.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    [ProtoContract]
    [StructLayout(LayoutKind.Sequential)]
    public struct AABBf
    {
        /// <summary>
        /// The minimum values of the axis aligned bounding box in x, y and z direction
        /// </summary>
        [JsonProperty(PropertyName = "Min")]
        [ProtoMember(1)] public float3 min;

        /// <summary>
        /// The maximum values of the axis aligned bounding box in x, y and z direction
        /// </summary>
        [JsonProperty(PropertyName = "Max")]
        [ProtoMember(2)] public float3 max;

        /// <summary>
        /// Create a new axis aligned bounding box.
        /// </summary>
        /// <param name="min_">the minimum x y and z values.</param>
        /// <param name="max_">the maximum x y and z values.</param>
        public AABBf(float3 min_, float3 max_)
        {
            min = min_;
            max = max_;
        }

        /// <summary>
        /// Create a new axis aligned bounding box.
        /// </summary>
        /// <param name="vertices">The list of vertices the bounding box is created for.</param>
        public AABBf(ReadOnlySpan<float3> vertices)
        {
            min = vertices[0];
            max = vertices[0];
            foreach (float3 p in vertices)
                this |= p;
        }

        /// <summary>
        /// Applies a transformation on the bounding box. After the transformation another
        /// axis aligned bounding box results. This is done by transforming all eight
        /// vertices of the box and re-aligning to the axes afterwards.
        /// </summary>
        /// <param name="m">The transformation matrix.</param>
        /// <param name="box">the box to transform.</param>
        /// <returns>A new axis aligned bounding box.</returns>
        public static AABBf operator *(float4x4 m, AABBf box)
        {
            float3[] cube =
            {
                new float3(box.min.x, box.min.y, box.min.z),
                new float3(box.min.x, box.min.y, box.max.z),
                new float3(box.min.x, box.max.y, box.min.z),
                new float3(box.min.x, box.max.y, box.max.z),
                new float3(box.max.x, box.min.y, box.min.z),
                new float3(box.max.x, box.min.y, box.max.z),
                new float3(box.max.x, box.max.y, box.min.z),
                new float3(box.max.x, box.max.y, box.max.z)
            };

            for (int i = 0; i < 8; i++)
            {
                cube[i] = m * cube[i];
            }

            AABBf ret;
            ret.min = cube[0];
            ret.max = cube[0];
            for (int i = 1; i < 8; i++)
            {
                if (cube[i].x < ret.min.x) ret.min.x = cube[i].x;
                if (cube[i].y < ret.min.y) ret.min.y = cube[i].y;
                if (cube[i].z < ret.min.z) ret.min.z = cube[i].z;
                if (cube[i].x > ret.max.x) ret.max.x = cube[i].x;
                if (cube[i].y > ret.max.y) ret.max.y = cube[i].y;
                if (cube[i].z > ret.max.z) ret.max.z = cube[i].z;
            }
            return ret;
        }

        /// <summary>
        /// Calculates the bounding box around two existing bounding boxes.
        /// </summary>
        /// <param name="a">One of the bounding boxes to build the union from</param>
        /// <param name="b">The other bounding box to build the union from</param>
        /// <returns>The smallest axis aligned bounding box containing both input boxes</returns>
        public static AABBf Union(AABBf a, AABBf b)
        {
            AABBf ret;
            ret.min.x = (a.min.x < b.min.x) ? a.min.x : b.min.x;
            ret.min.y = (a.min.y < b.min.y) ? a.min.y : b.min.y;
            ret.min.z = (a.min.z < b.min.z) ? a.min.z : b.min.z;
            ret.max.x = (a.max.x > b.max.x) ? a.max.x : b.max.x;
            ret.max.y = (a.max.y > b.max.y) ? a.max.y : b.max.y;
            ret.max.z = (a.max.z > b.max.z) ? a.max.z : b.max.z;
            return ret;
        }

        /// <summary>
        /// Calculates the bounding box around an existing bounding box and a single point.
        /// </summary>
        /// <param name="a">The bounding boxes to build the union from.</param>
        /// <param name="p">The point to be enclosed by the resulting bounding box</param>
        /// <returns>The smallest axis aligned bounding box containing the input box and the point.</returns>
        public static AABBf Union(AABBf a, float3 p)
        {
            AABBf ret;
            ret.min.x = (a.min.x < p.x) ? a.min.x : p.x;
            ret.min.y = (a.min.y < p.y) ? a.min.y : p.y;
            ret.min.z = (a.min.z < p.z) ? a.min.z : p.z;
            ret.max.x = (a.max.x > p.x) ? a.max.x : p.x;
            ret.max.y = (a.max.y > p.y) ? a.max.y : p.y;
            ret.max.z = (a.max.z > p.z) ? a.max.z : p.z;
            return ret;
        }

        /// <summary>
        /// Calculates the bounding box around two existing bounding boxes.
        /// </summary>
        /// <param name="a">One of the bounding boxes to build the union from</param>
        /// <param name="b">The other bounding box, to build the union from</param>
        /// <returns>The smallest axis aligned bounding box containing both input boxes</returns>
        public static AABBf operator |(AABBf a, AABBf b)
        {
            return Union(a, b);
        }

        /// <summary>
        /// Calculates the bounding box around an existing bounding box and a single point.
        /// </summary>
        /// <param name="a">The bounding boxes to build the union from.</param>
        /// <param name="p">The point to be enclosed by the resulting bounding box</param>
        /// <returns>The smallest axis aligned bounding box containing the input box and the point.</returns>
        /// <example>
        ///   Use this operator e.g. to calculate the bounding box for a given list of points.
        ///   <code>
        ///     AABB box = new AABB(pointList.First(), pointList.First());
        ///     foreach (float3 p in pointList)
        ///         box |= p;
        ///   </code>
        /// </example>
        public static AABBf operator |(AABBf a, float3 p)
        {
            return Union(a, p);
        }

        /// <summary>
        ///     Returns the center of the bounding box
        /// </summary>
        public float3 Center => (max + min) * 0.5f;

        /// <summary>
        ///     Returns the with, height and depth of the box in x, y and z
        /// </summary>
        public float3 Size => (max - min);

        /// <summary>
        ///     Check if this AABB intersects with another
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool Intersects(AABBf b)
        {
            return (min.x <= b.max.x && max.x >= b.min.x) &&
           (min.y <= b.max.y && max.y >= b.min.y) &&
           (min.z <= b.max.z && max.z >= b.min.z);
        }

        /// <summary>
        /// Check if point lies in this AABB
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Intersects(float3 point)
        {
            return (point.x >= min.x && point.x <= max.x) &&
            (point.y >= min.y && point.y <= max.y) &&
            (point.z >= min.z && point.z <= max.z);
        }

        /// <summary>
        /// Checks if a viewing frustum lies within or intersects this AABB.
        /// </summary>
        /// <param name="frustum">The frustum to test against.</param>
        /// <returns>false if fully outside, true if inside or intersecting.</returns>
        public bool InsideOrIntersectingFrustum(FrustumF frustum)
        {
            if (!frustum.Near.InsideOrIntersecting(this))
                return false;
            if (!frustum.Far.InsideOrIntersecting(this))
                return false;
            if (!frustum.Left.InsideOrIntersecting(this))
                return false;
            if (!frustum.Right.InsideOrIntersecting(this))
                return false;
            if (!frustum.Top.InsideOrIntersecting(this))
                return false;
            if (!frustum.Bottom.InsideOrIntersecting(this))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if a plane lies within or intersects this AABB.
        /// </summary>
        /// <param name="plane">The plane to test against.</param>
        /// <returns>false if fully outside, true if inside or intersecting.</returns>
        public bool InsideOrIntersectingPlane(PlaneF plane)
        {
            return plane.InsideOrIntersecting(this);
        }

        /// <summary>
        /// Checks if a given ray originates in, or intersects this AABB.
        /// </summary>
        /// <param name="ray">The ray to test against.</param>
        /// <returns></returns>
        public bool IntersectRay(RayF ray)
        {
            if (this.Intersects(ray.Origin))
                return true;

            float t1 = (min.x - ray.Origin.x) * ray.Inverse.x;
            float t2 = (max.x - ray.Origin.x) * ray.Inverse.x;

            float tmin = M.Min(t1, t2);
            float tmax = M.Max(t1, t2);

            for (int i = 1; i < 3; i++)
            {
                t1 = (min[i] - ray.Origin[i]) * ray.Inverse[i];
                t2 = (max[i] - ray.Origin[i]) * ray.Inverse[i];

                t1 = float.IsNaN(t1) ? 0.0f : t1;
                t2 = float.IsNaN(t2) ? 0.0f : t2;

                tmin = M.Max(tmin, M.Min(t1, t2));
                tmax = M.Min(tmax, M.Max(t1, t2));
            }

            return tmax >= M.Max(tmin, 0.0);
        }

        /// <summary>
        /// Returns the closest point to a point p, that lies on the surface of the <see cref="AABBf"/>.
        /// </summary>
        /// <param name="point">The reference point.</param>
        /// <returns></returns>
        public float3 ClosestPoint(float3 point)
        {
            float3 d = point - Center;
            float3 q = Center;

            for (int i = 0; i < 3; i++)
            {
                var axis = i == 0 ? float3.UnitX : i == 1 ? float3.UnitY : float3.UnitZ;
                var halfLength = i == 0 ? Size.x / 2 : i == 1 ? Size.y / 2 : Size.z / 2;
                float dist = float3.Dot(d, axis);

                if (dist > halfLength) dist = halfLength;
                if (dist < -halfLength) dist = -halfLength;

                q += dist * axis;
            }

            return q;
        }


        /// <summary>
        /// Check if two AABBs intersect each other
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool Intersects(AABBf left, AABBf right)
        {
            return left.Intersects(right);
        }

        /// <summary>
        /// Check if a point lies within a AABB
        /// </summary>
        /// <param name="aabb"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool Intersects(AABBf aabb, float3 point)
        {
            return aabb.Intersects(point);
        }

        /// <summary>
        /// Operator override for equality.
        /// </summary>
        /// <param name="left">The AABBf.</param>
        /// <param name="right">The scalar value.</param>
        public static bool operator ==(AABBf left, AABBf right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Operator override for inequality.
        /// </summary>
        /// <param name="left">The AABBf.</param>
        /// <param name="right">The scalar value.</param>
        public static bool operator !=(AABBf left, AABBf right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Indicates whether this AABBf is equal to another object.
        /// </summary>
        /// <param name="obj">The object. This method will throw an exception if the object isn't of type <see cref="AABBf"/>.</param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (obj?.GetType() != typeof(AABBf)) return false;

            var other = (AABBf)obj;
            return max.Equals(other.max) && min.Equals(other.min);
        }

        /// <summary>
        /// Generates a hash code for this AABBf.
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(max, min);
        }
    }
}