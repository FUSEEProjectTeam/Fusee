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

        private readonly Geometry _geometry;
        private readonly Face _face;


        public StatusEdge(Geometry geometry, Face face, Vertex origin, Vertex target, Vertex eventPoint)
        {
            _geometry = geometry;
            _face = face;
            _origin = origin;
            _target = target;

            SetKey(eventPoint);
        }

        //If the half edge is parallel (m = -Infinity) to x or y axis: Key = x value of the intersection point from sweep line with HalfEdge. Else Key = origin.x
        internal void SetKey(Vertex eventPoint)
        {
            var targetPos = _geometry.Get2DVertPos(_face,_target.Handle);
            var originPos = _geometry.Get2DVertPos(_face,_origin.Handle);

            var y = _geometry.Get2DVertPos(_face,eventPoint.Handle).y;
            var m = (targetPos.y - originPos.y) / (targetPos.x - originPos.x);

            if (targetPos.y.Equals(originPos.y) || targetPos.x.Equals(originPos.x))
                IntersectionPointX = originPos.x;
            else
            {
                var b = originPos.y - (m* originPos.x);
                IntersectionPointX = (y - b)/m;
            }
        }
    }
}
