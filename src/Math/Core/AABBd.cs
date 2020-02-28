using ProtoBuf;
using System;
using System.Runtime.InteropServices;

namespace Fusee.Math.Core
{
    /// <summary>
    /// Represents an axis aligned bounding box.
    /// </summary>
    [ProtoContract]
    [StructLayout(LayoutKind.Sequential)]
    public struct AABBd
    {
        /// <summary>
        ///     The minimum values of the axis aligned bounding box in x, y and z direction
        /// </summary>
        [ProtoMember(1)] public double3 min;

        /// <summary>
        ///     The maximum values of the axis aligned bounding box in x, y and z direction
        /// </summary>
        [ProtoMember(2)] public double3 max;

        /// <summary>
        ///     Create a new axis aligned bounding box
        /// </summary>
        /// <param name="min_">the minimum x y and z values</param>
        /// <param name="max_">the maximum x y and z values</param>
        public AABBd(double3 min_, double3 max_)
        {
            min = min_;
            max = max_;
        }

        /// <summary>
        /// Applies a transformation on the bounding box. After the transformation another
        /// axis aligned bounding box results. This is done by transforming all eight
        /// vertices of the box and re-aligning to the axes afterwards.
        /// </summary>
        /// <param name="m">The transformation matrix</param>
        /// <param name="box">the box to transform</param>
        /// <returns>A new axis aligned bounding box.</returns>
        public static AABBd operator *(double4x4 m, AABBd box)
        {
            double3[] cube =
            {
                new double3(box.min.x, box.min.y, box.min.z),
                new double3(box.min.x, box.min.y, box.max.z),
                new double3(box.min.x, box.max.y, box.min.z),
                new double3(box.min.x, box.max.y, box.max.z),
                new double3(box.max.x, box.min.y, box.min.z),
                new double3(box.max.x, box.min.y, box.max.z),
                new double3(box.max.x, box.max.y, box.min.z),
                new double3(box.max.x, box.max.y, box.max.z)
            };

            for (int i = 0; i < 8; i++)
            {
                cube[i] = m * cube[i];
            }

            AABBd ret;
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
        /// Applies a transformation function on the bounding box. After the transformation another
        /// axis aligned bounding box results. This is done by transforming all eight
        /// vertices of the box with the given transformation function and re-aligning to the axes afterwards.
        /// </summary>
        /// <param name="func">The transformation function</param>
        /// <param name="box">the box to transform</param>
        /// <returns>A new axis aligned bounding box.</returns>
        public static AABBd operator ^(Func<double3, double3> func, AABBd box)
        {
            double3[] cube =
            {
                new double3(box.min.x, box.min.y, box.min.z),
                new double3(box.min.x, box.min.y, box.max.z),
                new double3(box.min.x, box.max.y, box.min.z),
                new double3(box.min.x, box.max.y, box.max.z),
                new double3(box.max.x, box.min.y, box.min.z),
                new double3(box.max.x, box.min.y, box.max.z),
                new double3(box.max.x, box.max.y, box.min.z),
                new double3(box.max.x, box.max.y, box.max.z)
            };

            for (int i = 0; i < 8; i++)
            {
                cube[i] = func(cube[i]);
            }

            AABBd ret;
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
        ///     Calculates the bounding box around two existing bounding boxes.
        /// </summary>
        /// <param name="a">One of the bounding boxes to build the union from</param>
        /// <param name="b">The other bounding box to build the union from</param>
        /// <returns>The smallest axis aligned bounding box containing both input boxes</returns>
        public static AABBd Union(AABBd a, AABBd b)
        {
            AABBd ret;
            ret.min.x = (a.min.x < b.min.x) ? a.min.x : b.min.x;
            ret.min.y = (a.min.y < b.min.y) ? a.min.y : b.min.y;
            ret.min.z = (a.min.z < b.min.z) ? a.min.z : b.min.z;
            ret.max.x = (a.max.x > b.max.x) ? a.max.x : b.max.x;
            ret.max.y = (a.max.y > b.max.y) ? a.max.y : b.max.y;
            ret.max.z = (a.max.z > b.max.z) ? a.max.z : b.max.z;
            return ret;
        }

        /// <summary>
        ///     Calculates the bounding box around an existing bounding box and a single point.
        /// </summary>
        /// <param name="a">The bounding boxes to build the union from.</param>
        /// <param name="p">The point to be enclosed by the resulting bounding box</param>
        /// <returns>The smallest axis aligned bounding box containing the input box and the point.</returns>
        public static AABBd Union(AABBd a, double3 p)
        {
            AABBd ret;
            ret.min.x = (a.min.x < p.x) ? a.min.x : p.x;
            ret.min.y = (a.min.y < p.y) ? a.min.y : p.y;
            ret.min.z = (a.min.z < p.z) ? a.min.z : p.z;
            ret.max.x = (a.max.x > p.x) ? a.max.x : p.x;
            ret.max.y = (a.max.y > p.y) ? a.max.y : p.y;
            ret.max.z = (a.max.z > p.z) ? a.max.z : p.z;
            return ret;
        }

        /// <summary>
        ///     Calculates the bounding box around two existing bounding boxes.
        /// </summary>
        /// <param name="a">One of the bounding boxes to build the union from</param>
        /// <param name="b">The other bounding box to build the union from</param>
        /// <returns>The smallest axis aligned bounding box containing both input boxes</returns>
        public static AABBd operator |(AABBd a, AABBd b) => Union(a, b);

        /// <summary>
        ///     Calculates the bounding box around an existing bounding box and a single point.
        /// </summary>
        /// <param name="a">The bounding boxes to build the union from.</param>
        /// <param name="p">The point to be enclosed by the resulting bounding box</param>
        /// <returns>The smallest axis aligned bounding box containing the input box and the point.</returns>
        /// <example>
        ///   Use this operator e.g. to calculate the bounding box for a given list of points.
        ///   <code>
        ///     AABB box = new AABB(pointList.First());
        ///     foreach (double3 p in pointList)
        ///         box |= p;
        ///   </code>
        /// </example>
        public static AABBd operator |(AABBd a, double3 p) => Union(a, p);

        /// <summary>
        ///     Returns the center of the bounding box
        /// </summary>
        public double3 Center
        {
            get { return (max + min) * 0.5f; }
        }

        /// <summary>
        ///     Returns the with, height and depth of the box in x, y and z
        /// </summary>
        public double3 Size
        {
            get { return (max - min); }
        }

        /// <summary>
        ///     Check if this AABB intersects with another
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool Intersects(AABBd b)
        {
            return (min.x <= b.max.x && max.x >= b.min.x) &&
           (min.y <= b.max.y && max.y >= b.min.y) &&
           (min.z <= b.max.z && max.z >= b.min.z);
        }

        /// <summary>
        ///     Check if point lies in this AABB
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Intersects(double3 point)
        {
            return (point.x >= min.x && point.x <= max.x) &&
            (point.y >= min.y && point.y <= max.y) &&
            (point.z >= min.z && point.z <= max.z);
        }
        /// <summary>
        /// Checks if a viewing frustum lies within or intersects this AABB.      
        /// </summary>
        /// <param name="frustumPlanes">The frustum planes to test against.</param>
        /// <returns>false if fully outside, true if inside or intersecting.</returns>
        public bool InsideOrIntersectingFrustum(PlaneD[] frustumPlanes)
        {
            foreach (var plane in frustumPlanes)
            {
                if (!plane.InsideOrIntersecting(this))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if a viewing frustum lies within or intersects this AABB.      
        /// </summary>
        /// <param name="plane">The plane to test against.</param>
        /// <returns>false if fully outside, true if inside or intersecting.</returns>
        public bool InsideOrIntersectingPlane(PlaneD plane)
        {
            return plane.InsideOrIntersecting(this);
        }

        /// <summary>
        ///     Check if two AABBs intersect each other
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool Intersects(AABBd left, AABBd right)
        {
            return left.Intersects(right);
        }

        /// <summary>
        ///     Check if a point lies within a AABB
        /// </summary>
        /// <param name="aabb"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool Intersects(AABBd aabb, double3 point)
        {
            return aabb.Intersects(point);
        }
    }
}