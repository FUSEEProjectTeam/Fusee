using Fusee.Jometri.DCEL;

namespace Fusee.Jometri.Triangulation
{
    internal class StatusEdge 
    {
        //A key is needed to insert the Node into a binary search tree and to find the edge directly left of an vertex. 
        internal float IntersectionPointX;
        //HalfEdge(Handle) identifies the HalfEdge.
        internal int HalfEdgeHandle;

        //The helper vertex is the vertex to which a possible new diagonal is drawn. Additionally we need to know if the helper vertex is of type merge vertex.
        internal int HelperVertexHandle;
        internal bool IsMergeVertex;

        //Origin and target vertex of the half edge.
        private Vertex _origin;
        private Vertex _target;

        public StatusEdge(Vertex origin, Vertex target, Vertex eventPoint)
        {
            _origin = origin;
            _target = target;

            SetKey(eventPoint);
        }

        //If the half edge is parallel (m = -Infinity) to x or y axis: Key = x value of the intersection point from sweep line with HalfEdge. Else Key = origin.x
        internal void SetKey(Vertex eventPoint)
        {
            _target.VertData.Pos = _target.VertData.Pos.Reduce2D();
            _origin.VertData.Pos = _origin.VertData.Pos.Reduce2D();

            var y = eventPoint.VertData.Pos.Reduce2D().y;
            var m = (_target.VertData.Pos.y - _origin.VertData.Pos.y) / (_target.VertData.Pos.x - _origin.VertData.Pos.x);

            if (_target.VertData.Pos.y.Equals(_origin.VertData.Pos.y) || _target.VertData.Pos.x.Equals(_origin.VertData.Pos.x))
                IntersectionPointX = _origin.VertData.Pos.x;
            else
            {
                var b = _origin.VertData.Pos.y - (m*_origin.VertData.Pos.x);
                IntersectionPointX = (y - b)/m;
            }
        }
    }
}
