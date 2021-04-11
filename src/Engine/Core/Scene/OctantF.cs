using Fusee.Math.Core;
using Fusee.Structures;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Component that allows a SceneNode to save information from a <see cref="IOctant{T, K, P}"/>.
    /// </summary>
    public class OctantF : SceneComponent, IOctant<float3, float, Mesh>
    {
        /// <summary>
        /// Children of this Octant. Must contain eight or null (leaf node) children.
        /// </summary>
        public IOctant<float3, float, Mesh>[] Children { get; set; }

        /// <summary>
        /// The payload of this octant.
        /// </summary>
        public List<Mesh> Payload { get; set; }

        /// <summary>
        /// Center of this Bucket in world space coordinates.
        /// </summary>
        public float3 Center { get; set; }

        /// <summary>
        /// Length, width and height of this Octant.
        /// </summary>
        public float Size { get; set; }

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
        public OctantF(float3 center, float size, List<Mesh> payload, IOctant<float3, float, Mesh>[] children = null)
        {
            Center = center;
            Size = size;

            if (children == null)
                Children = new IOctant<float3, float, Mesh>[8];
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
        public OctantF(float3 center, float size, IOctant<float3, float, Mesh>[] children = null)
        {
            Center = center;
            Size = size;

            if (children == null)
                Children = new IOctant<float3, float, Mesh>[8];
            else
                Children = children;
        }

        /// <summary>
        /// Create a new instance of type Octant.
        /// </summary>
        public OctantF() { }

        /// <summary>
        /// The size, projected into screen space. Set with <seealso cref="ComputeScreenProjectedSize(float3, int, float)"/>.
        /// </summary>
        public float ProjectedScreenSize { get; private set; }

        /// <summary>
        /// Calculates the size, projected into screen space.
        /// </summary>
        /// <param name="camPos">Position of the camera.</param>
        /// <param name="screenHeight">Hight of the canvas.</param>
        /// <param name="fov">Field of view.</param>
        public void ComputeScreenProjectedSize(float3 camPos, int screenHeight, float fov)
        {
            var distance = (Center - camPos).Length;
            var slope = (float)System.Math.Tan(fov / 2f);
            ProjectedScreenSize = screenHeight / 2f * Size / (slope * distance);
        }

        /// <summary>
        /// <see cref="IOctant{T, K, P}.CreateChild(int)"/>.
        /// </summary>
        /// <param name="atPosInParent">The <see cref="PosInParent"/> the new child has.</param>
        /// <returns></returns>
        public IOctant<float3, float, Mesh> CreateChild(int atPosInParent)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if a viewing frustum lies within or intersects this Octant.
        /// </summary>
        /// <param name="plane">The plane to test against.</param>
        /// <returns>false if fully outside, true if inside or intersecting.</returns>
        public bool InsideOrIntersectingPlane(PlaneF plane)
        {
            return plane.InsideOrIntersecting(Center, Size);
        }

        /// <summary>
        /// Checks if a viewing frustum lies within or intersects this Octant.
        /// </summary>
        /// <param name="frustum">The frustum to test against.</param>
        /// <returns>false if fully outside, true if inside or intersecting.</returns>
        public bool InsideOrIntersectingFrustum(FrustumF frustum)
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
    }
}