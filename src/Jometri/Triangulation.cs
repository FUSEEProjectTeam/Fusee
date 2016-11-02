using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fusee.Base.Core;
using Fusee.Math.Core;

namespace Fusee.Jometri
{
    /// <summary>
    /// Contains the triangulation of some geometry, stored in a doubly connected halfe edge list.
    /// </summary>
    public class Triangulation
    {

        private readonly Geometry _geometry;
        private VertexType _vertType;
        private BinarySearchTree<float> _sweepLineStatus;

        /// <summary>
        /// Constructs a new triangulation.
        /// </summary>
        /// <param name="geometry">Geometry, stored in a doubly connected half edge list</param>
        public Triangulation(Geometry geometry)
        {
            _geometry = geometry;
            _vertType = new VertexType();

            Triangulate();
        }

        private struct StatusNode
        {
            private HalfEdgeHandle halfEdge;
            private VertHandle helper;
            private VertexType helperType;
        }

        private bool IsMonotone(FaceHandle faceHandle)
        {
            var face = _geometry.GetFaceByHandle(faceHandle);
            var noSplitOrMerge = NoSplitOrMerge(faceHandle);

            return noSplitOrMerge && face.InnerHalfEdges.Count == 0;
        }

        private bool NoSplitOrMerge(FaceHandle faceHandle)
        {
            var verts = _geometry.GetVeticesFromFace(faceHandle).ToList();

            foreach (var vert in verts)
            {
                TestVertexType(vert, faceHandle);
                if (_vertType.Equals(VertexType.SplitVertex) || _vertType.Equals(VertexType.MergeVertex))
                    return false;
            }
            return true;
        }

        private void TestVertexType(Geometry.Vertex vert, FaceHandle faceHandle)
        {
            var heStartingAtFace = _geometry.HalfEdgesStartingAtV(vert).ToList();

            var incidentHalfEdge = new Geometry.HalfEdge();
            foreach (var he in heStartingAtFace)
            {
                var incidentFace = _geometry.GetHalfEdgeByHandle(he).IncidentFace;
                if (incidentFace.Equals(faceHandle))
                    incidentHalfEdge = _geometry.GetHalfEdgeByHandle(he);
            }

            var nextHalfEdge = _geometry.GetHalfEdgeByHandle(incidentHalfEdge.Next);
            var nextVert = _geometry.GetVertexByHandle(nextHalfEdge.Origin);

            var prevHalfEdge = _geometry.GetHalfEdgeByHandle(incidentHalfEdge.Prev);
            var prevVert = _geometry.GetVertexByHandle(prevHalfEdge.Origin);

            var v2 = new float3(prevVert.Coord - vert.Coord);
            var v1 = new float3(nextVert.Coord - vert.Coord);

            if (IsUnderVert(vert, nextVert) && IsUnderVert(vert, prevVert))
            {
                if (IsAngleGreaterPi(v1, v2))
                    _vertType = VertexType.SplitVertex;
                else
                {
                    _vertType = VertexType.StartVertex;
                }
            }
            else if (IsOverVert(vert, nextVert) && IsOverVert(vert, prevVert))
            {
                if (IsAngleGreaterPi(v1, v2))
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
            if (middle.Coord.y.Equals(neighbour.Coord.y) && middle.Coord.x < neighbour.Coord.x)
            {
                return true;
            }
            return false;
        }

        private static bool IsOverVert(Geometry.Vertex middle, Geometry.Vertex neighbour)
        {
            if (middle.Coord.y < neighbour.Coord.y)
                return true;
            if (middle.Coord.y.Equals(neighbour.Coord.y) && middle.Coord.x > neighbour.Coord.x)
            {
                return true;
            }
            return false;
        }

        private static bool IsAngleGreaterPi(float3 first, float3 second)
        {
            var cross = first.x * second.y - first.y * second.x; //z component of the cross product
            var dot = float3.Dot(first, second);

            var angle = (float)System.Math.Atan2(cross, dot);
            var deg = M.RadiansToDegrees(angle);
            if ((angle * -1).Equals(M.Pi))
                return false;
            return angle < 0;
        }

        //Test for calculate helper for split / merge vertices
        //Assumption: z component of float3 is always 0
        private bool AreLinesIntersecting(float3 p1, float3 p2, float3 p3, float3 p4)
        {
            var a = p2 - p1;
            var b = p3 - p4;
            var c = p1 - p3;

            var tNumerator = b.y * b.x - b.x * c.y;
            var iNumerator = a.x * c.y - a.y * c.x;

            var denominator = a.y * b.x - a.x * b.y;

            if (denominator > 0)
            {
                if (tNumerator < 0 || tNumerator > denominator)
                    return false;
            }
            else
            {
                if (tNumerator > 0 || tNumerator < denominator)
                    return false;
            }

            if (denominator > 0)
            {
                if (iNumerator < 0 || iNumerator > denominator)
                    return false;
            }
            else
            {
                if (iNumerator > 0 || iNumerator < denominator)
                    return false;
            }

            return true;
        }

        private bool IsVertLeftOfPolygon()
        {
            return false;
        }

        private void HandleStartVertex(BinarySearchTree<float> tree, Geometry.Vertex v)
        {
            //Add v.IncindentHalfEdge with v as Helper to tree
        }

        private void HandleEndVertex(BinarySearchTree<float> tree, Geometry.Vertex v)
        {
            //if v.IncidentHalfEdge.Prev == VertexType.MergeVertex
                //AddHalfEdge(v,helper of v.IncidentHalfEdge.Prev)
            // tree.DeleteNode(v.IncidentHalfEdge.Prev)
        }

        private void HandleSplitVertex(BinarySearchTree<float> tree, Geometry.Vertex v)
        {
            //find edge directly left of v in tree (=ej)
            //AddHalfEdge(v, helper of ej)
            //set helper of ej to v
            //Add ev.IncidentHalfEdge with v as Helper to tree
        }

        private void HandleMergeVertex(BinarySearchTree<float> tree, Geometry.Vertex v)
        {
            //if v.IncidentHalfEdge.Prev == VertexType.MergeVertex
                //AddHalfEdge(v,helper of v.IncidentHalfEdge.Prev)
            // tree.DeleteNode(v.IncidentHalfEdge.Prev)

            //find edge directly left of v in tree (=ej)
            //if v.IncidentHalfEdge.Prev == VertexType.MergeVertex
                //AddHalfEdge(v, helper of ej)
            //set helper of ej to v
        }

        private void HandleRegularVertex(BinarySearchTree<float> tree, Geometry.Vertex v)
        {
            //if face interior is right of v
                //if v.IncidentHalfEdge.Prev == VertexType.MergeVertex
                    //AddHalfEdge(v,helper of v.IncidentHalfEdge.Prev)
                // tree.DeleteNode(v.IncidentHalfEdge.Prev)
                //tree.InsertNode( v.IncindentHalfEdge with v as Helper )
            //else
                //find edge directly left of v in tree (=ej)
                //if v.IncidentHalfEdge.Prev == VertexType.MergeVertex
                    //AddHalfEdge(v, helper of ej)
                //set helper of ej to v
        }

        public IList<Geometry.Vertex> SortedVertices(List<Geometry.Vertex> unsorted)
        {
            var sorted = new List<Geometry.Vertex>();
            sorted.AddRange(unsorted);
            sorted.Sort(delegate (Geometry.Vertex a, Geometry.Vertex b)
            {
                var ydiff = -1 * a.Coord.y.CompareTo(b.Coord.y);
                if (ydiff != 0) return ydiff;
                return a.Coord.x.CompareTo(b.Coord.x);
            });

            return sorted;
        }

        private void MakeMonotone(FaceHandle face)
        {
            var vertices = _geometry.GetVeticesFromFace(face);

            var sortedVertices = SortedVertices(vertices.ToList());

            _sweepLineStatus = new BinarySearchTree<float>();

            while (sortedVertices.Count != 0)
            {
                var current = sortedVertices[0];
                TestVertexType(current, face);
                Debug.WriteLine(_vertType);

                switch (_vertType)
                {
                    case VertexType.StartVertex:
                        HandleStartVertex(_sweepLineStatus, current);
                        break;
                    case VertexType.EndVertex:
                        HandleEndVertex(_sweepLineStatus, current);
                        break;
                    case VertexType.SplitVertex:
                        HandleSplitVertex(_sweepLineStatus, current);
                        break;
                    case VertexType.MergeVertex:
                        HandleMergeVertex(_sweepLineStatus, current);
                        break;
                    case VertexType.RegularVertex:
                        HandleRegularVertex(_sweepLineStatus, current);
                        break;
                }

                sortedVertices.RemoveAt(0);
            }
        }

        private void Triangulate()
        {
            foreach (var fHandle in _geometry.FaceHandles)
            {
                if (IsMonotone(fHandle))
                {
                    Debug.WriteLine("is monotone!");
                    //Triangulate, e.g. by ear clipping
                }
                else
                {
                    MakeMonotone(fHandle);
                    //Triangulate, e.g. by ear clipping
                }
            }
        }
    }
}
