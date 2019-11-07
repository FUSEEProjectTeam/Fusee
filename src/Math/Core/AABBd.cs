using ProtoBuf;
using System;
using System.Runtime.InteropServices;

namespace Fusee.Math.Core
{
    /// <summary>
    ///     Represents an axis aligned bounding box.
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
        ///     Applies a transformation on the bounding box. After the transformation another
        ///     axis alignes bounding box results. This is done by transforming all eight
        ///     vertices of the box and re-aligning to the axes afterwards.
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
        ///     Applies a tranformation function on the bounding box. After the tranformation another
        ///     axis alignes bounding box results. This is done by transforming all eight
        ///     vertices of the box with the given transformation function and re-aligning to the axes afterwards.
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
        /// <param name="b">The other bounding boxe to build the union from</param>
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
        /// <param name="b">The other bounding boxe to build the union from</param>
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
        ///     Checks if a viewing frustrum lies within this AABB.
        ///     If feeded with a projection matrix, the result of the clipping planes is in view space
        ///     If feeded with a projection view matrix, the clipping planes are given in model space
        /// </summary>
        /// <param name="viewingFrustrum">Projection matrix</param>
        /// <returns>false if fully outside, true if inside or intersects</returns>
        public bool Intersects(double4x4 viewingFrustrum)
        {
            // shorter variable
            var vF = viewingFrustrum;

            // split the viewing frustrum in 6 planes
            // plane equation = ax + by + cz + d = 0;
            // For the GL-style frustum we find, that the six frustum planes in view space are exactly the six planes p_4^T±p_i^T for i=1, 2, 3 
            var planes = new double4[6];
            // left
            planes[0] = new double4(vF.M41 + vF.M11,
                                    vF.M42 + vF.M12,
                                    vF.M43 + vF.M13,
                                    vF.M44 + vF.M14);
            // right
            planes[1] = new double4(vF.M41 - vF.M11,
                                    vF.M42 - vF.M12,
                                    vF.M43 - vF.M13,
                                    vF.M44 - vF.M14);

            // bottom
            planes[2] = new double4(vF.M41 + vF.M21,
                                    vF.M42 + vF.M22,
                                    vF.M43 + vF.M23,
                                    vF.M44 + vF.M24);

            // top
            planes[3] = new double4(vF.M41 - vF.M21,
                                    vF.M42 - vF.M22,
                                    vF.M43 - vF.M23,
                                    vF.M44 - vF.M24);

            // near
            planes[4] = new double4(vF.M41 + vF.M31,
                                     vF.M42 + vF.M32,
                                     vF.M43 + vF.M33,
                                     vF.M44 + vF.M34);

            // far
            planes[5] = new double4(vF.M41 - vF.M31,
                                    vF.M42 - vF.M32,
                                    vF.M43 - vF.M33,
                                    vF.M44 - vF.M34);

            foreach (var plane in planes)
            {
                var side = Classify(this, plane);
                if (side < 0) return false;
            }
            return true;

        }

        private double Classify(AABBd aabb, double4 plane)
        {
            // maximum extent in direction of plane normal (plane.xyz)
            var r = System.Math.Abs(aabb.Size.x * plane.x)
                + System.Math.Abs(aabb.Size.y * plane.y)
                + System.Math.Abs(aabb.Size.z * plane.z);

            // signed distance between box center and plane
            //float d = plane.Test(mCenter);
            var d = double3.Dot(plane.xyz, aabb.Center) + plane.w;

            // return signed distance
            if (System.Math.Abs(d) < r)
                return 0.0f;
            else if (d < 0.0)
                return d + r;
            return d - r;
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