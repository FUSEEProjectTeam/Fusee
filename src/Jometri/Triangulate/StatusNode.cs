using System;
using Fusee.Jometri.DCEL;
using Fusee.Math.Core;

namespace Fusee.Jometri.Triangulate
{
    internal class StatusNode : IComparable<StatusNode>
    {
        //HalfEdgeOrigin is needed to find the edge directly left of an vertex (FindLargestSmalerThan(vertex.x)). HalfEdge(Handle) identifies the HalfEdge
        internal float3 HalfEdgeOrigin;
        internal HalfEdgeHandle HalfEdge;

        //The helper is the vertex to which a possible new diagonal is drawn. Additionally we need to know if the helper vertex is of type merge vertex.
        internal VertHandle Helper;
        internal bool IsMergeVertex;

        public int CompareTo(StatusNode other)
        {
            return HalfEdgeOrigin.x.CompareTo(other.HalfEdgeOrigin.x);
        }
    }
}
