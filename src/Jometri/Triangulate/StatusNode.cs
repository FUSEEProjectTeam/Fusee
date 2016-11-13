using System;
using Fusee.Jometri.DCEL;
using Fusee.Math.Core;

namespace Fusee.Jometri.Triangulate
{
    internal class StatusNode : IComparable<StatusNode>
    {
        //Key is needed to insert the Node into a binary search tree and to find the edge directly left of an vertex (FindLargestSmalerThan(vertex.x)). 
        internal float3 Key;
        //HalfEdge(Handle) identifies the HalfEdge
        internal HalfEdgeHandle HalfEdge;

        //The helper is the vertex to which a possible new diagonal is drawn. Additionally we need to know if the helper vertex is of type merge vertex.
        internal VertHandle Helper;
        internal bool IsMergeVertex;

        public int CompareTo(StatusNode other)
        {
            return Key.x.CompareTo(other.Key.x);
        }

        internal void SetKey(Geometry.Vertex origin, Geometry.Vertex target)
        {
            Key = origin.Coord.x < target.Coord.x ? origin.Coord : target.Coord;
        }
    }
}
