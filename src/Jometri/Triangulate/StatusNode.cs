using System;
using Fusee.Jometri.DCEL;
using Fusee.Math.Core;

namespace Fusee.Jometri.Triangulate
{
    internal class StatusNode : IComparable<StatusNode>
    {
        internal float3 Coord;
        internal HalfEdgeHandle HalfEdge;
        internal VertHandle Helper;
        internal bool IsMergeVertex;

        public int CompareTo(StatusNode other)
        {
            return Coord.x.CompareTo(other.Coord.x);
        }
    }
}
