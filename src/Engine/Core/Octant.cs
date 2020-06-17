using System.Collections.Generic;
using System;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{

    /// <summary>
    /// Node for use in an octree.
    /// </summary>
    /// <typeparam name="T">Defines the type of the payload.</typeparam>
    public class Octant<T> : Bucket<T>
    {
        /// <summary>
        /// Children of this Octant. Must contain eight or null (leaf node) children.
        /// </summary>
        public Octant<T>[] Children;

        /// <summary>
        /// The payload of this octant.
        /// </summary>
        public List<T> Payload;

        /// <summary>
        /// The level of the octant in the octree.
        /// </summary>
        public int Level;

        /// <summary>
        /// Gets a value indicating whether this instance is a leaf node.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is leaf; otherwise, <c>false</c>.
        /// </value>
        public bool IsLeaf;

        /// <summary>
        /// Creates a new instance of type Octant.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <param name="children"></param>
        /// <param name="payload"></param>
        public Octant(double3 center, double size, List<T> payload, Octant<T>[] children = null)
        {
            Center = center;
            Size = size;

            if (children == null)
                Children = new Octant<T>[8];
            else
                Children = children;

            Payload = payload;
        }

        /// <summary>
        /// Create a new instance of type Octant.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <param name="children"></param>        
        public Octant(double3 center, double size, Octant<T>[] children = null)
        {
            Center = center;
            Size = size;

            if (children == null)
                Children = new Octant<T>[8];
            else
                Children = children;
        }

        /// <summary>
        /// Create a new instance of type Octant.
        /// </summary>
        protected Octant() { }


        ////TODO: duplicate code, clean up! The original is found in AABB.
        ///// <summary>
        /////     Checks if a viewing frustum lies within this AABB.
        /////     If feeded with a projection matrix, the result of the clipping planes is in view space
        /////     If feeded with a projection view matrix, the clipping planes are given in model space
        ///// </summary>
        ///// <param name="vf">Viewing frustum / projection matrix</param>
        ///// <returns>false if fully outside, true if inside or intersects</returns>
        //public bool Intersects(float4x4 vf)
        //{
        //    // split the viewing frustum in 6 planes
        //    // plane equation = ax + by + cz + d = 0;
        //    // For the GL-style frustum we find, that the six frustum planes in view space are exactly the six planes p_4^T±p_i^T for i=1, 2, 3 
        //    var planes = new double4[6];
        //    // left
        //    planes[0] = new double4(vf.M41 + vf.M11,
        //                            vf.M42 + vf.M12,
        //                            vf.M43 + vf.M13,
        //                            vf.M44 + vf.M14);
        //    // right
        //    planes[1] = new double4(vf.M41 - vf.M11,
        //                            vf.M42 - vf.M12,
        //                            vf.M43 - vf.M13,
        //                            vf.M44 - vf.M14);

        //    // bottom
        //    planes[2] = new double4(vf.M41 + vf.M21,
        //                            vf.M42 + vf.M22,
        //                            vf.M43 + vf.M23,
        //                            vf.M44 + vf.M24);

        //    // top
        //    planes[3] = new double4(vf.M41 - vf.M21,
        //                            vf.M42 - vf.M22,
        //                            vf.M43 - vf.M23,
        //                            vf.M44 - vf.M24);

        //    // near
        //    planes[4] = new double4(vf.M41 + vf.M31,
        //                             vf.M42 + vf.M32,
        //                             vf.M43 + vf.M33,
        //                             vf.M44 + vf.M34);

        //    // far
        //    planes[5] = new double4(vf.M41 - vf.M31,
        //                            vf.M42 - vf.M32,
        //                            vf.M43 - vf.M33,
        //                            vf.M44 - vf.M34);

        //    foreach (var plane in planes)
        //    {
        //        var side = Classify(plane);
        //        if (side < 0) return false;
        //    }
        //    return true;

        //}

        //private double Classify(double4 plane)
        //{
        //    // maximum extent in direction of plane normal (plane.xyz)
        //    var r = System.Math.Abs(Size * plane.x)
        //        + System.Math.Abs(Size * plane.y)
        //        + System.Math.Abs(Size * plane.z);

        //    // signed distance between box center and plane
        //    //float d = plane.Test(mCenter);
        //    var d = double3.Dot(plane.xyz, Center) + plane.w;

        //    // return signed distance
        //    if (System.Math.Abs(d) < r)
        //        return 0.0f;
        //    else if (d < 0.0)
        //        return d + r;
        //    return d - r;
        //}

    }
}