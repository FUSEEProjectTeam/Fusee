using Fusee.Math.Core;
using Fusee.Structures;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Component that allows a SceneNode to save information from a <see cref="IOctant{T, K, P}"/>.
    /// </summary>
    public class OctantD : SceneComponent, IOctant<double3, double, Mesh>
    {
        /// <summary>
        /// Children of this Octant. Must contain eight or null (leaf node) children.
        /// </summary>
        public IOctant<double3, double, Mesh>[] Children { get; set; }

        /// <summary>
        /// The payload of this octant.
        /// </summary>
        public List<Mesh> Payload { get; set; }

        /// <summary>
        /// Center of this Bucket in world space coordinates.
        /// </summary>
        public double3 Center { get; set; }

        /// <summary>
        /// Length, width and height of this Octant.
        /// </summary>
        public double Size { get; set; }

        /// <summary>
        /// Is this octant a leaf node in the octree?
        /// </summary>
        public bool IsLeaf { get; set; }

        /// <summary>
        /// Integer that defines this octants position (1 to 8) in its parent.
        /// </summary>
        public int PosInParent { get; set; }

        /// <summary>
        /// The level of the octree this octant belongs to.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Unique identifier of the node.
        /// </summary>
        public Guid Guid;

        /// <summary>
        /// Defines if the node was loaded into memory.
        /// </summary>
        public bool WasLoaded;

        /// <summary>
        /// Number of point cloud points, this node holds.
        /// </summary>
        public int NumberOfPointsInNode;

        /// <summary>
        /// The octant's position in the texture which contains the octree's hierarchy.
        /// </summary>
        public int PosInHierarchyTex;

        /// <summary>
        /// Used to decode which children of an octant are visible, given a certain viewing frustum.
        /// </summary>
        public byte VisibleChildIndices;

        /// <summary>
        /// Creates a new instance of type Octant.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <param name="children"></param>
        /// <param name="payload"></param>
        public OctantD(double3 center, double size, List<Mesh> payload, IOctant<double3, double, Mesh>[] children = null)
        {
            Center = center;
            Size = size;

            if (children == null)
                Children = new IOctant<double3, double, Mesh>[8];
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
        public OctantD(double3 center, double size, IOctant<double3, double, Mesh>[] children = null)
        {
            Center = center;
            Size = size;

            if (children == null)
                Children = new IOctant<double3, double, Mesh>[8];
            else
                Children = children;
        }

        /// <summary>
        /// Create a new instance of type Octant.
        /// </summary>
        public OctantD() { }

        /// <summary>
        /// The size, projected into screen space. Set with <seealso cref="ComputeScreenProjectedSize(double3, int, float)"/>.
        /// </summary>
        public double ProjectedScreenSize { get; private set; }

        /// <summary>
        /// Calculates the size, projected into screen space.
        /// </summary>
        /// <param name="camPos">Position of the camera.</param>
        /// <param name="screenHeight">Hight of the canvas.</param>
        /// <param name="fov">Field of view.</param>
        public void ComputeScreenProjectedSize(double3 camPos, int screenHeight, float fov)
        {
            var distance = (Center - camPos).Length;
            var slope = (float)System.Math.Tan(fov / 2d);
            ProjectedScreenSize = screenHeight / 2d * Size / (slope * distance);
        }

        /// <summary>
        /// <see cref="IOctant{T, K, P}.CreateChild(int)"/>.
        /// </summary>
        /// <param name="atPosInParent">The <see cref="PosInParent"/> the new child has.</param>
        /// <returns></returns>
        public IOctant<double3, double, Mesh> CreateChild(int atPosInParent)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if a viewing frustum lies within or intersects this Octant.
        /// </summary>
        /// <param name="plane">The plane to test against.</param>
        /// <returns>false if fully outside, true if inside or intersecting.</returns>
        public bool InsideOrIntersectingPlane(PlaneD plane)
        {
            return plane.InsideOrIntersecting(Center, Size);
        }

        /// <summary>
        /// Checks if a viewing frustum lies within or intersects this Octant.
        /// </summary>
        /// <param name="frustum">The frustum to test against.</param>
        /// <returns>false if fully outside, true if inside or intersecting.</returns>
        public bool InsideOrIntersectingFrustum(FrustumD frustum)
        {
            if (!frustum.Near.InsideOrIntersecting(Center, Size))
                return false;
            if (!frustum.Far.InsideOrIntersecting(Center, Size))
                return false;
            if (!frustum.Left.InsideOrIntersecting(Center, Size))
                return false;
            if (!frustum.Right.InsideOrIntersecting(Center, Size))
                return false;
            if (!frustum.Top.InsideOrIntersecting(Center, Size))
                return false;
            if (!frustum.Bottom.InsideOrIntersecting(Center, Size))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if a viewing frustum lies within or intersects this Octant.
        /// </summary>
        /// <param name="plane">The plane to test against.</param>
        /// <returns>false if fully outside, true if inside or intersecting.</returns>
        public bool InsideOrIntersectingPlane(PlaneF plane)
        {
            return plane.InsideOrIntersecting((float3)Center, (float)Size);
        }

        /// <summary>
        /// Checks if a viewing frustum lies within or intersects this Octant.
        /// </summary>
        /// <param name="frustum">The frustum to test against.</param>
        /// <returns>false if fully outside, true if inside or intersecting.</returns>
        public bool InsideOrIntersectingFrustum(FrustumF frustum)
        {
            if (!frustum.Near.InsideOrIntersecting((float3)Center, (float)Size))
                return false;
            if (!frustum.Far.InsideOrIntersecting((float3)Center, (float)Size))
                return false;
            if (!frustum.Left.InsideOrIntersecting((float3)Center, (float)Size))
                return false;
            if (!frustum.Right.InsideOrIntersecting((float3)Center, (float)Size))
                return false;
            if (!frustum.Top.InsideOrIntersecting((float3)Center, (float)Size))
                return false;
            if (!frustum.Bottom.InsideOrIntersecting((float3)Center, (float)Size))
                return false;

            return true;
        }
    }
}