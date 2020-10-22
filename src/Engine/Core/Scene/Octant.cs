using System;
using Fusee.Structures.Octree;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Component that allows a SceneNode to save information usually associated with a <see cref="Structures.Octree.OctantD"/>.
    /// </summary>
    public class Octant : SceneComponent
    {
        /// <summary>
        /// The payload-independent information about an octant.
        /// </summary>
        public OctantD OctantD;

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
    }
}