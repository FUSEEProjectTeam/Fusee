using Fusee.Math.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Serialization.V1
{
    /// <summary>
    /// Not yet implemented - Octant serialization currently not used.
    /// </summary>
    /// <seealso cref="Fusee.Serialization.V1.FusComponent" />
    /// TODO (MR): Check if we need some more or if this suffice or is even too much
    public class FusOctant : FusComponent
    {
        /// <summary>
        /// Defines the position in the parent octant.
        /// </summary>
        [ProtoMember(0)]
        public int PosInParent;

        /// <summary>
        /// The level of the octree the node lies in.
        /// </summary>
        [ProtoMember(1)]
        public int Level;

        /// <summary>
        /// Unique identifier of the node.
        /// </summary>
        [ProtoMember(2)]
        public Guid Guid;

        /// <summary>
        /// Center in world space.
        /// </summary>
        [ProtoMember(3)]
        public double3 Center;

        /// <summary>
        /// Length of on side of the cubical node.
        /// </summary>
        [ProtoMember(4)]
        public double Size;

        /// <summary>
        /// Defines if the node is a leaf node.
        /// </summary>
        [ProtoMember(5)]
        public bool IsLeaf;

        /// <summary>
        /// Defines if the node was loaded into memory.
        /// </summary>
        [ProtoMember(6)]
        public bool WasLoaded;

        /// <summary>
        /// Number of point cloud points, this node holds.
        /// </summary>
        [ProtoMember(7)]
        public int NumberOfPointsInNode;

        /// <summary>
        /// TBD
        /// </summary>
        [ProtoMember(8)]
        public int PosInHierarchyTex;

        /// <summary>
        /// TBD
        /// </summary>
        [ProtoMember(9)]
        public byte VisibleChildIndices;

    }
}
