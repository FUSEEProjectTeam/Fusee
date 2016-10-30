using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math.Core;

namespace Fusee.Jometri
{
    public class Triangulation
    {
        private readonly Geometry _geometry;
        private VertexType _vertType;

        public Triangulation(Geometry geometry)
        {
            _geometry = geometry;
            _vertType = new VertexType();
        }

        public bool IsMonotone(FaceHandle faceHandle)
        {
            var face = _geometry.GetFaceByHandle(faceHandle);

            if (face.InnerHalfEdges.Count != 0)
                return false;
            /*if(face.InnerHalfEdges.Count != 0 && noSplitOrMerge)
                retrun true;
            else
            {
                return false;
            }*/

            var faceVertices = _geometry.GetVeticesFromFace(face.Handle);
            //TODO: Test if split or merge vertex occures

            return true;
        }

        private void TestVertexType(Geometry.Vertex vert)
        {
            var incidentHalfEdge = _geometry.GetHalfEdgeByHandle(vert.IncidentHalfEdge);

            var nextHalfEdge = _geometry.GetHalfEdgeByHandle(incidentHalfEdge.Next);
            var nextVert = _geometry.GetVertexByHandle(nextHalfEdge.Origin);

            var prevHalfEdge = _geometry.GetHalfEdgeByHandle(incidentHalfEdge.Prev);
            var prevVert = _geometry.GetVertexByHandle(prevHalfEdge.Origin);

            var v1 = new float2(prevVert.Coord.x - vert.Coord.x, prevVert.Coord.y - vert.Coord.y);
            var v2 = new float2(nextVert.Coord.x - vert.Coord.x, nextVert.Coord.y - vert.Coord.y);

            if (IsUnderVert(vert, nextVert) && IsUnderVert(vert, prevVert))
            {
                if (IsAngleGreaterPi(v1,v2))
                    _vertType = VertexType.SplitVertex;
                else
                {
                    _vertType = VertexType.StartVertex;
                }
            }
            else if (IsOverVert(vert, nextVert) && IsOverVert(vert, prevVert))
            {
                if (IsAngleGreaterPi(v1,v2))
                    _vertType = VertexType.MergeVertex;
                else
                {
                    _vertType = VertexType.EndVertex;
                }
            }
            else
            {
                _vertType = VertexType.RegularVertex;
            }
        }

        private static bool IsUnderVert(Geometry.Vertex middle, Geometry.Vertex neighbour)
        {
            if (middle.Coord.y > neighbour.Coord.y)
                return true;
            if (middle.Coord.y.Equals(neighbour.Coord.y) && middle.Coord.x < neighbour.Coord.y)
            {
                return true;
            }
            return false;
        }

        private static bool IsOverVert(Geometry.Vertex middle, Geometry.Vertex neighbour)
        {
            if (middle.Coord.y < neighbour.Coord.y)
                return true;
            if (middle.Coord.y.Equals(neighbour.Coord.y) && middle.Coord.x > neighbour.Coord.y)
            {
                return true;
            }
            return false;
        }

        private static bool IsAngleGreaterPi(float2 first, float2 second)
        {
            var cross = first.x * second.y - first.y * second.x;
            var dot = first.x * second.x + first.y * second.y;

            var angle = (float)System.Math.Atan2(cross, dot);
            if ((angle * -1).Equals(M.Pi))
                return false;
            return angle < 0;
        }

        //if !IsMonotone

        //Sort Vertices of a Face according to their coord.y value, if two vertices have the same y value the one with the smaller x value has higher priority

        //MakeMonotone

        //Triangulate (e.g by ear clipping)
    }
}
