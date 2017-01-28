using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Core;
using Fusee.Jometri.DCEL;
using Fusee.Math.Core;

namespace Fusee.Jometri.Triangulation
{
    /// <summary>
    /// Contains the triangulation of a geometry, stored in a half edge data structure list.
    /// </summary>
    public static class Triangulation
    {
        private static Geometry _geometry;
        private static VertexType _vertType;
        private static SweepLineStatus _sweepLineStatus;

        /// <summary>
        /// After triangulation all faces of a geometry consist of three vertices and three half edges.
        /// </summary>
        /// <param name="geometry"></param>
        public static void Triangulate(this Geometry geometry)
        {
            _geometry = geometry;

            var originalFaces = new Dictionary<int, Face>(_geometry.DictFaces);

            foreach (var face in originalFaces)
            {
                //If the face has no OuterHalfEdge it is unbounded and can be ignored.
                if (face.Value.OuterHalfEdge == default(int)) { continue; }

                if (!IsMonotone(face.Value))
                    MakeMonotone(face.Value);
            }

            var monotoneFaces = new Dictionary<int, Face>(_geometry.DictFaces);

            foreach (var face in monotoneFaces)
            {
                if (face.Value.OuterHalfEdge == default(int)) { continue; }

                TriangulateMonotone(face.Value);
            }
        }

        #region Triangulate monotone polygone

        private static void TriangulateMonotone(Face face)
        {
            var faceVertices = new List<Vertex>();
            foreach (var vHandle in _geometry.GetFaceVertices(face.Handle))
            {
                faceVertices.Add(vHandle);
            }

            if (faceVertices.Count.Equals(3)) return;

            var sortedVerts = GetSortedVertices(_geometry, face, faceVertices);
            var vertStack = new Stack<Vertex>();
            var leftChain = GetLeftChain(sortedVerts, face.Handle).ToList();

            vertStack.Push(sortedVerts[0]);
            vertStack.Push(sortedVerts[1]);

            for (var i = 2; i < sortedVerts.Count - 1; i++)
            {
                var current = sortedVerts[i];

                if (!IsLeftChain(leftChain, current) && IsLeftChain(leftChain, vertStack.Peek()) ||
                    IsLeftChain(leftChain, current) && !IsLeftChain(leftChain, vertStack.Peek()))
                {
                    while (vertStack.Count > 0)
                    {
                        var popped = vertStack.Pop();

                        if (vertStack.Count > 0)
                            _geometry.InsertDiagonal(current.Handle, popped.Handle);
                    }
                    vertStack.Push(sortedVerts[i - 1]);
                    vertStack.Push(current);
                }
                else
                {
                    var popped = vertStack.Pop();

                    Vertex next;
                    Vertex prev;

                    if (IsLeftChain(leftChain, popped))
                    {
                        next = sortedVerts[i];
                        prev = vertStack.Peek();
                    }
                    else
                    {
                        next = vertStack.Peek();
                        prev = sortedVerts[i];
                    }

                    while (vertStack.Count > 0 && !_geometry.IsAngleGreaterOrEqualPi(face, next, popped, prev))
                    {
                        popped = vertStack.Pop();

                        if (vertStack.Count > 0)
                        {
                            if (IsLeftChain(leftChain, popped))
                            {
                                next = sortedVerts[i];
                                prev = vertStack.Peek();
                            }
                            else
                            {
                                next = vertStack.Peek();
                                prev = sortedVerts[i];
                            }
                        }

                        _geometry.InsertDiagonal(current.Handle, popped.Handle);
                    }
                    vertStack.Push(popped);
                    vertStack.Push(current);
                }
            }

            var count = vertStack.Count;

            for (var j = 0; j < count; j++)
            {
                var popped = vertStack.Pop();

                if (j == 0) continue;
                if (j != count - 1)
                    _geometry.InsertDiagonal(sortedVerts.LastItem().Handle, popped.Handle);
            }
        }

        private static IEnumerable<Vertex> GetLeftChain(IList<Vertex> sortedVerts, int fHandle)
        {
            var heHandle = new int();
            var endOfChain = sortedVerts.LastItem();

            var startingAtFirstV = _geometry.GetVertexStartingHalfEdges(sortedVerts[0].Handle).ToList();
            if (startingAtFirstV.Count > 1)
            {
                foreach (var heh in startingAtFirstV)
                {
                    var he = heh;
                    if (he.IncidentFace == fHandle)
                        heHandle = heh.Handle;
                }
            }
            else
            { heHandle = sortedVerts[0].IncidentHalfEdge; }

            do
            {
                var halfEdge = _geometry.GetHalfEdgeByHandle(heHandle);
                yield return _geometry.GetVertexByHandle(halfEdge.OriginVertex);
                heHandle = halfEdge.NextHalfEdge;

            } while (_geometry.GetHalfEdgeByHandle(heHandle).OriginVertex != endOfChain.Handle);
        }

        private static bool IsLeftChain(IEnumerable<Vertex> leftChain, Vertex vert)
        {
            foreach (var v in leftChain)
            {
                if (v.Handle == vert.Handle)
                    return true;
            }
            return false;
        }

        #endregion

        #region Test face for y monotony

        private static bool IsMonotone( Face face)
        {
            var noSplitOrMerge = HasNoSplitOrMerge(face);

            return noSplitOrMerge && face.InnerHalfEdges.Count == 0;
        }

        private static bool HasNoSplitOrMerge(Face face)
        {
            var verts = _geometry.GetFaceVertices(face.Handle).ToList();

            foreach (var vert in verts)
            {
                TestVertexType(vert, face);
                if (_vertType.Equals(VertexType.SPLIT_VERTEX) || _vertType.Equals(VertexType.MERGE_VERTEX))
                    return false;
            }
            return true;
        }

        private static void TestVertexType(Vertex vert, Face face)
        {
            var heStartingAtFace = _geometry.GetVertexStartingHalfEdges(vert.Handle).ToList();

            var incidentHalfEdge = new HalfEdge();
            foreach (var he in heStartingAtFace)
            {
                var incidentFace = he.IncidentFace;
                if (!incidentFace.Equals(face.Handle)) continue;
                incidentHalfEdge = he;
                break;
            }

            var nextHalfEdge = _geometry.GetHalfEdgeByHandle(incidentHalfEdge.NextHalfEdge);
            var nextVert = _geometry.GetVertexByHandle(nextHalfEdge.OriginVertex);

            var prevHalfEdge = _geometry.GetHalfEdgeByHandle(incidentHalfEdge.PrevHalfEdge);
            var prevVert = _geometry.GetVertexByHandle(prevHalfEdge.OriginVertex);

            if (IsUnderVert(face, vert, nextVert) && IsUnderVert(face, vert, prevVert))
            {
                if (_geometry.IsAngleGreaterPi(face, nextVert, vert, prevVert))
                    _vertType = VertexType.SPLIT_VERTEX;
                else
                {
                    _vertType = VertexType.START_VERTEX;
                }
            }
            else if (IsOverVert(face, vert, nextVert) && IsOverVert(face, vert, prevVert))
            {
                if (_geometry.IsAngleGreaterPi(face, nextVert, vert, prevVert))
                    _vertType = VertexType.MERGE_VERTEX;
                else
                {
                    _vertType = VertexType.END_VERTEX;
                }
            }
            else
            {
                _vertType = VertexType.REGULAR_VERTEX;
            }
        }

        //Vertices need to be reduced to 2D.
        private static bool IsUnderVert(Face face, Vertex middle, Vertex neighbour)
        {
            var redMiddle = _geometry.Get2DVertPos(face, middle.Handle);
            var redNeighbour = _geometry.Get2DVertPos(face, neighbour.Handle);

            if (redMiddle.y > redNeighbour.y)
                return true;
            if (redMiddle.y.Equals(redNeighbour.y) && redMiddle.x < redNeighbour.x)
            {
                return true;
            }
            return false;
        }

        //Vertices need to be reduced to 2D.
        private static bool IsOverVert(Face face, Vertex middle, Vertex neighbour)
        {
            var redMiddle = _geometry.Get2DVertPos(face, middle.Handle);
            var redNeighbour = _geometry.Get2DVertPos(face, neighbour.Handle);

            if (redMiddle.y < redNeighbour.y)
                return true;
            if (redMiddle.y.Equals(redNeighbour.y) && redMiddle.x > redNeighbour.x)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region MakeMonotone
        private static void MakeMonotone(Face face)
        {
            var vertices = new List<Vertex>(_geometry.GetFaceVertices(face.Handle));
            var sortedVertices = GetSortedVertices(_geometry, face, vertices.ToList());
            var faceHalfEdges = _geometry.GetFaceHalfEdges(face.Handle).ToList();
            var newFaces = new List<Face>();

            var test = new List<float3>();
            foreach (var i in _geometry.GetFaceVertices(face.Handle))
            {
                test.Add(i.VertData.Pos);
            }

            _sweepLineStatus = new SweepLineStatus();

            while (sortedVertices.Count != 0)
            {
                var current = sortedVertices[0];

                TestVertexType(current, face, newFaces);

                switch (_vertType)
                {
                    case VertexType.START_VERTEX:
                        HandleStartVertex(face, current, faceHalfEdges);
                        break;
                    case VertexType.END_VERTEX:
                        HandleEndVertex(current, faceHalfEdges, newFaces);
                        break;
                    case VertexType.SPLIT_VERTEX:
                        HandleSplitVertex(face, current, faceHalfEdges, newFaces);
                        break;
                    case VertexType.MERGE_VERTEX:
                        HandleMergeVertex(face, current, faceHalfEdges, newFaces);
                        break;
                    case VertexType.REGULAR_VERTEX:
                        HandleRegularVertex(face, current, faceHalfEdges, newFaces);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                sortedVertices.RemoveAt(0);
            }
        }

        private static void TestVertexType(Vertex vert, Face face, ICollection<Face> newFaces)
        {
            var heStartingAtVert = _geometry.GetVertexStartingHalfEdges(vert.Handle).ToList();
            var incidentHalfEdge = new HalfEdge();

            foreach (var he in heStartingAtVert)
            {
                var incidentFace = he.IncidentFace;
                if (incidentFace.Equals(face.Handle))
                {
                    incidentHalfEdge = he;
                    break;
                }
                foreach (var fh in newFaces)
                {
                    if (incidentFace.Equals(fh.Handle))
                        incidentHalfEdge = he;
                }
            }

            var nextHalfEdge = _geometry.GetHalfEdgeByHandle(incidentHalfEdge.NextHalfEdge);
            var nextVert = _geometry.GetVertexByHandle(nextHalfEdge.OriginVertex);

            var prevHalfEdge = _geometry.GetHalfEdgeByHandle(incidentHalfEdge.PrevHalfEdge);
            var prevVert = _geometry.GetVertexByHandle(prevHalfEdge.OriginVertex);

            if (IsUnderVert(face, vert, nextVert) && IsUnderVert(face, vert, prevVert))
            {
                if (_geometry.IsAngleGreaterPi(face, nextVert, vert, prevVert))
                    _vertType = VertexType.SPLIT_VERTEX;
                else
                {
                    _vertType = VertexType.START_VERTEX;
                }
            }
            else if (IsOverVert(face, vert, nextVert) && IsOverVert(face, vert, prevVert))
            {
                if (_geometry.IsAngleGreaterPi(face, nextVert, vert, prevVert))
                    _vertType = VertexType.MERGE_VERTEX;
                else
                {
                    _vertType = VertexType.END_VERTEX;
                }
            }
            else
            {
                _vertType = VertexType.REGULAR_VERTEX;
            }
        }

        private static void HandleStartVertex(Face face, Vertex vert, IEnumerable<HalfEdge> faceHalfEdges)
        {
            foreach (var halfEdge in faceHalfEdges)
            {
                var he = halfEdge;

                if (he.OriginVertex != vert.Handle) continue;

                _sweepLineStatus.UpdateNodes(vert);
                var origin = _geometry.GetVertexByHandle(he.OriginVertex);
                var targetH = _geometry.GetHalfEdgeByHandle(he.NextHalfEdge).OriginVertex;
                var target = _geometry.GetVertexByHandle(targetH);

                var ei = new StatusEdge(_geometry, face, origin, target, vert);
                ei.HalfEdgeHandle = he.Handle;
                ei.HelperVertexHandle = vert.Handle;
                ei.IsMergeVertex = false;

                _sweepLineStatus.InsertNode(ei.IntersectionPointX, ei);
                break;
            }
        }

        private static void HandleEndVertex(Vertex vert, IEnumerable<HalfEdge> faceHalfEdges, ICollection<Face> newFaces)
        {
            foreach (var heh in faceHalfEdges)
            {
                var he = heh;

                if (he.OriginVertex != vert.Handle) continue;

                _sweepLineStatus.UpdateNodes(vert);

                var eMinOne = _sweepLineStatus.FindStatusEdgeWithHandle(he.PrevHalfEdge);

                if (eMinOne.IsMergeVertex)
                {
                    _geometry.InsertDiagonal(vert.Handle, eMinOne.HelperVertexHandle);
                    newFaces.Add(_geometry.DictFaces[_geometry.DictFaces.Keys.Max()]);
                }

                _sweepLineStatus.DeleteNode(eMinOne.IntersectionPointX);
                break;
            }
        }

        private static void HandleSplitVertex(Face face, Vertex vert, IEnumerable<HalfEdge> faceHalfEdges, ICollection<Face> newFaces)
        {
            _sweepLineStatus.UpdateNodes(vert);
            _sweepLineStatus.BalanceTree();

            var redXPos = _geometry.Get2DVertPos(face, vert.Handle).x;
            var ej = _sweepLineStatus.FindLargestSmallerThanInBalanced(redXPos);

            _geometry.InsertDiagonal(vert.Handle, ej.HelperVertexHandle);
            newFaces.Add(_geometry.DictFaces[_geometry.DictFaces.Keys.Max()]);

            _sweepLineStatus.FindNode(ej.IntersectionPointX).HelperVertexHandle = vert.Handle;
            _sweepLineStatus.FindNode(ej.IntersectionPointX).IsMergeVertex = false;

            var he = new HalfEdge();
            foreach (var halfEdge in faceHalfEdges)
            {
                if (halfEdge.OriginVertex == vert.Handle)
                    he = halfEdge;
            }

            var origin = _geometry.GetVertexByHandle(he.OriginVertex);
            var targetH = _geometry.GetHalfEdgeByHandle(he.NextHalfEdge).OriginVertex;
            var target = _geometry.GetVertexByHandle(targetH);

            var ei = new StatusEdge(_geometry, face, origin, target, vert);
            ei.HalfEdgeHandle = he.Handle;
            ei.HelperVertexHandle = vert.Handle;
            ei.IsMergeVertex = false;

            _sweepLineStatus.InsertNode(ei.IntersectionPointX, ei);
        }

        private static void HandleMergeVertex(Face face, Vertex vert, IEnumerable<HalfEdge> faceHalfEdges, ICollection<Face> newFaces)
        {
            var he = new HalfEdge();
            foreach (var heh in faceHalfEdges)
            {
                he = heh;

                if (he.OriginVertex == vert.Handle) break;
            }

            _sweepLineStatus.UpdateNodes(vert);

            var eMinOne = _sweepLineStatus.FindStatusEdgeWithHandle(he.PrevHalfEdge);

            if (eMinOne.IsMergeVertex)
            {
                _geometry.InsertDiagonal(vert.Handle, eMinOne.HelperVertexHandle);
                newFaces.Add(_geometry.DictFaces[_geometry.DictFaces.Keys.Max()]);
            }

            _sweepLineStatus.DeleteNode(eMinOne.IntersectionPointX);
            _sweepLineStatus.BalanceTree();

            var redXPos = _geometry.Get2DVertPos(face, vert.Handle).x;
            var ej = _sweepLineStatus.FindLargestSmallerThanInBalanced(redXPos);

            if (ej.IsMergeVertex)
            {
                _geometry.InsertDiagonal(vert.Handle, ej.HelperVertexHandle);
                newFaces.Add(_geometry.DictFaces[_geometry.DictFaces.Keys.Max()]);
            }

            _sweepLineStatus.FindNode(ej.IntersectionPointX).HelperVertexHandle = vert.Handle;
            _sweepLineStatus.FindNode(ej.IntersectionPointX).IsMergeVertex = true;
        }

        private static void HandleRegularVertex(Face face, Vertex vert, IList<HalfEdge> faceHalfEdges, ICollection<Face> newFaces)
        {
            if (IsPolygonRightOfVert(_geometry, face, faceHalfEdges,  vert))
            {
                foreach (var heh in faceHalfEdges)
                {
                    var he = heh;

                    if (he.OriginVertex != vert.Handle) continue;

                    _sweepLineStatus.UpdateNodes(vert);

                    var eMinOne = _sweepLineStatus.FindStatusEdgeWithHandle(he.PrevHalfEdge);

                    if (eMinOne.IsMergeVertex)
                    {
                        _geometry.InsertDiagonal(vert.Handle, eMinOne.HelperVertexHandle);
                        newFaces.Add(_geometry.DictFaces[_geometry.DictFaces.Keys.Max()]);
                    }

                    _sweepLineStatus.DeleteNode(eMinOne.IntersectionPointX);

                    var halfEdge = _geometry.GetHalfEdgeByHandle(vert.IncidentHalfEdge);
                    var origin = _geometry.GetVertexByHandle(halfEdge.OriginVertex);
                    var targetH = _geometry.GetHalfEdgeByHandle(halfEdge.NextHalfEdge).OriginVertex;
                    var target = _geometry.GetVertexByHandle(targetH);

                    var ei = new StatusEdge(_geometry, face, origin, target, vert);
                    ei.HalfEdgeHandle = he.Handle;
                    ei.HelperVertexHandle = vert.Handle;
                    ei.IsMergeVertex = false;

                    _sweepLineStatus.InsertNode(ei.IntersectionPointX, ei);

                    break;
                }
            }
            else
            {
                _sweepLineStatus.UpdateNodes(vert);
                _sweepLineStatus.BalanceTree();

                var redXPos = _geometry.Get2DVertPos(face, vert.Handle).x;
                var ej = _sweepLineStatus.FindLargestSmallerThanInBalanced(redXPos);

                if (ej.IsMergeVertex)
                {
                    _geometry.InsertDiagonal(vert.Handle, ej.HelperVertexHandle);
                    newFaces.Add(_geometry.DictFaces[_geometry.DictFaces.Keys.Max()]);
                }

                _sweepLineStatus.FindNode(ej.IntersectionPointX).HelperVertexHandle = vert.Handle;
                _sweepLineStatus.FindNode(ej.IntersectionPointX).IsMergeVertex = false;
            }
        }

        private static bool IsPolygonRightOfVert(Geometry geometry, Face face, IList<HalfEdge> faceHalfEdges,Vertex vert)
        {
            var prevV = GetPrevVertex(face,faceHalfEdges, vert);
            var nextV = GetNextVertex(face,faceHalfEdges, vert);

            var redPrevPos = geometry.Get2DVertPos(face, prevV.Handle);
            var redNextPos = geometry.Get2DVertPos(face, nextV.Handle);


            return redPrevPos.y > redNextPos.y;
        }

        private static Vertex GetNextVertex(Face face,IEnumerable<HalfEdge> faceHalfEdges , Vertex currentVert)
        {
            foreach (var he in faceHalfEdges)
            {
                if (he.OriginVertex != currentVert.Handle) continue;

                var nextHe = _geometry.GetHalfEdgeByHandle(he.NextHalfEdge);
                return _geometry.GetVertexByHandle(nextHe.OriginVertex);
            }
            throw new ArgumentException("Face " + face.Handle + " has no half edge with vertex " + currentVert.Handle + " as origin.");
        }

        private static Vertex GetPrevVertex(Face face, IEnumerable<HalfEdge> faceHalfEdges, Vertex currentVert)
        {
            foreach (var he in faceHalfEdges)
            {
                if (he.OriginVertex != currentVert.Handle) continue;

                var prevHe = _geometry.GetHalfEdgeByHandle(he.PrevHalfEdge);
                return _geometry.GetVertexByHandle(prevHe.OriginVertex);
            }
            throw new ArgumentException("Face "+face.Handle+" has no half edge with vertex "+currentVert.Handle+" as origin.");

        }

        #endregion

        //Vertices need to be reduced to 2D.
        //Can be optimized by implementing a priority queue data structure and use it instead of sorting a list.
        private static IList<Vertex> GetSortedVertices(Geometry geometry, Face face, IEnumerable<Vertex> unsortedVerts)
        {
            var sorted = new List<Vertex>();
            sorted.AddRange(unsortedVerts);
            sorted.Sort(delegate (Vertex a, Vertex b)
            {
                var redA = geometry.Get2DVertPos(face, a.Handle);
                var redB = geometry.Get2DVertPos(face, b.Handle);

                var ydiff = -1 * redA.y.CompareTo(redB.y);
                if (ydiff != 0) return ydiff;
                return redA.x.CompareTo(redB.x);
            });

            return sorted;
        }
    }
}
