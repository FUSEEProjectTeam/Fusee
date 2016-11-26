using System;
using Fusee.Jometri.DCEL;

namespace Fusee.Jometri.Triangulation
{
    internal class StatusNode : IComparable<StatusNode>
    {
        //Key is needed to insert the Node into a binary search tree and to find the edge directly left of an vertex (FindLargestSmalerThan(vertex.x)). 
        internal float Key;
        //HalfEdge(Handle) identifies the HalfEdge
        internal HalfEdgeHandle HalfEdge;

        //The helper is the vertex to which a possible new diagonal is drawn. Additionally we need to know if the helper vertex is of type merge vertex.
        internal VertHandle Helper;
        internal bool IsMergeVertex;

        //The vertices that define the HalfEdge
        private Geometry.Vertex _origin;
        private Geometry.Vertex _target;

        public StatusNode(Geometry.Vertex origin, Geometry.Vertex target, Geometry.Vertex eventPoint)
        {
            _origin = origin;
            _target = target;

            SetKey(eventPoint);
        }

        //If HalfEdge is not parallel (m = -Infinity) to x or y axis: Key = x value of the intersection point from sweep line with HalfEdge. Else Key = origin.x
        internal void SetKey(Geometry.Vertex eventPoint)
        {
            _target.Coord = _target.Coord.Reduce2D();
            _origin.Coord = _origin.Coord.Reduce2D();

            var y = eventPoint.Coord.Reduce2D().y;
            var m = (_target.Coord.y - _origin.Coord.y) / (_target.Coord.x - _origin.Coord.x);

            if (_target.Coord.y.Equals(_origin.Coord.y) || _target.Coord.x.Equals(_origin.Coord.x))
                Key = _origin.Coord.x;
            else
            {
                var b = _origin.Coord.y - (m*_origin.Coord.x);
                Key = (y - b)/m;
            }
        }

        public int CompareTo(StatusNode other)
        {
            return Key.CompareTo(other.Key);
        }
    }
}
