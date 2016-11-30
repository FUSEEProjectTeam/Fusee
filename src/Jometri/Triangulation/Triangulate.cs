using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Core;
using Fusee.Jometri.DCEL;
using Fusee.Math.Core;

namespace Fusee.Jometri.Triangulation
{
    /// <summary>
    /// Contains the triangulation of some geometry, stored in a doubly connected halfe edge list.
    /// </summary>
    internal static class Triangulation
    {
        private static Geometry _geometry;
        private static VertexType _vertType;
        private static BinarySearchTree<StatusEdge> _sweepLineStatus;
        
        internal static void Triangulate(this Geometry geometry)
        {
            //TODO: Both, MakeMonotone and TriangulateMonotone need 2D coordinates instead of 3D. It is possibly more effective to call Reduce2D for the vertices of the whole face in those methods than in the sub methods

            _geometry = geometry;

            var originalFaces = new List<FaceHandle>();
            originalFaces.AddRange(_geometry.FaceHandles);

            var firstFace = _geometry.GetFaceByHandle(_geometry.FaceHandles[0]);
            var isFirstFaceUnbounded = firstFace.OuterHalfEdge == default(HalfEdgeHandle);

            for (var i = 0; i < originalFaces.Count; i++)
            {
                var fHandle = originalFaces[i];

                //If face has no OuterHalfEdge it's unbounded and can be ignored - if there is a unbounded face its always the first in _faces
                if (i == 0 && isFirstFaceUnbounded) { continue; }

                if (!IsMonotone(fHandle))
                    MakeMonotone(fHandle);
            }

            for (var i = 0; i < geometry.FaceHandles.Count; i++)
            {
                var fHandle = geometry.FaceHandles[i];

                if (i == 0 && isFirstFaceUnbounded) { continue; }

                TriangulateMonotone(fHandle);
            }
        }

        #region Triangulate monotone polygone
        private static void TriangulateMonotone(FaceHandle fHandle)
        {
            var faceVertices = _geometry.GetFaceVertices(fHandle).ToList();

            if (faceVertices.Count.Equals(3)) return;

            var sortedVerts = GetSortedVertices(faceVertices);
            var vertStack = new Stack<Geometry.Vertex>();
            var leftChain = GetLeftChain(sortedVerts, fHandle).ToList();

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
                            _geometry.InsertEdge(current.Handle, popped.Handle);
                    }
                    vertStack.Push(sortedVerts[i - 1]);
                    vertStack.Push(current);
                }
                else
                {
                    var popped = vertStack.Pop();

                    float3 v1;
                    float3 v2;

                    Geometry.Vertex next;
                    Geometry.Vertex prev;

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

                    v1 = next.Coord - popped.Coord;
                    v2 = prev.Coord - popped.Coord;

                    while (vertStack.Count > 0 && !IsAngleGreaterOrEqualPi(v1, v2))
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

                            v1 = next.Coord - popped.Coord;
                            v2 = prev.Coord - popped.Coord;
                        }

                        _geometry.InsertEdge(current.Handle, popped.Handle);
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
                    _geometry.InsertEdge(sortedVerts.LastItem().Handle, popped.Handle);
            }
        }

        private static IEnumerable<Geometry.Vertex> GetLeftChain(IList<Geometry.Vertex> sortedVerts, FaceHandle fHandle)
        {
            var heHandle = new HalfEdgeHandle();
            var endOfChain = sortedVerts.LastItem();

            var startingAtFirstV = _geometry.GetHalfEdgesStartingAtV(sortedVerts[0]).ToList();
            if (startingAtFirstV.Count > 1)
            {
                foreach (var heh in startingAtFirstV)
                {
                    var he = _geometry.GetHalfEdgeByHandle(heh);
                    if (he.IncidentFace == fHandle)
                        heHandle = heh;
                }
            }
            else
            { heHandle = sortedVerts[0].IncidentHalfEdge; }

            do
            {
                var halfEdge = _geometry.GetHalfEdgeByHandle(heHandle);
                yield return _geometry.GetVertexByHandle(halfEdge.Origin);
                heHandle = halfEdge.Next;

            } while (_geometry.GetHalfEdgeByHandle(heHandle).Origin != endOfChain.Handle);
        }

        private static bool IsLeftChain(IEnumerable<Geometry.Vertex> leftChain, Geometry.Vertex vert)
        {
            foreach (var v in leftChain)
            {
                if (v.Handle == vert.Handle)
                    return true;
            }
            return false;
        }

        //Vertices need to be reduced to 2D
        private static bool IsAngleGreaterOrEqualPi(float3 first, float3 second)
        {
            var redFirst = first.Reduce2D();
            var redSecond = second.Reduce2D();

            var cross = redFirst.x * redSecond.y - redFirst.y * redSecond.x; //z component of the cross product
            var dot = float3.Dot(first, second);

            var angle = (float)System.Math.Atan2(cross, dot);
            var deg = M.RadiansToDegrees(angle);
            return angle <= 0;
        }

        #endregion

        #region Test face for y monotony

        private static bool IsMonotone(FaceHandle faceHandle)
        {
            var face = _geometry.GetFaceByHandle(faceHandle);
            var noSplitOrMerge = HasNoSplitOrMerge(faceHandle);

            return noSplitOrMerge && face.InnerHalfEdges.Count == 0;
        }

        private static bool HasNoSplitOrMerge(FaceHandle faceHandle)
        {
            var verts = _geometry.GetFaceVertices(faceHandle).ToList();

            foreach (var vert in verts)
            {
                TestVertexType(vert, faceHandle);
                if (_vertType.Equals(VertexType.SPLIT_VERTEX) || _vertType.Equals(VertexType.MERGE_VERTEX))
                    return false;
            }
            return true;
        }

        private static void TestVertexType(Geometry.Vertex vert, FaceHandle faceHandle)
        {
            var heStartingAtFace = _geometry.GetHalfEdgesStartingAtV(vert).ToList();

            var incidentHalfEdge = new Geometry.HalfEdge();
            foreach (var he in heStartingAtFace)
            {
                var incidentFace = _geometry.GetHalfEdgeByHandle(he).IncidentFace;
                if (!incidentFace.Equals(faceHandle)) continue;
                incidentHalfEdge = _geometry.GetHalfEdgeByHandle(he);
                break;
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
                    _vertType = VertexType.SPLIT_VERTEX;
                else
                {
                    _vertType = VertexType.START_VERTEX;
                }
            }
            else if (IsOverVert(vert, nextVert) && IsOverVert(vert, prevVert))
            {
                if (IsAngleGreaterPi(v1, v2))
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

        //Vertices need to be reduced to 2D
        private static bool IsUnderVert(Geometry.Vertex middle, Geometry.Vertex neighbour)
        {
            var redMiddle = middle.Coord.Reduce2D();
            var redNeighbour = neighbour.Coord.Reduce2D();

            if (redMiddle.y > redNeighbour.y)
                return true;
            if (redMiddle.y.Equals(redNeighbour.y) && redMiddle.x < redNeighbour.x)
            {
                return true;
            }
            return false;
        }

        //Vertices need to be reduced to 2D
        private static bool IsOverVert(Geometry.Vertex middle, Geometry.Vertex neighbour)
        {
            var redMiddle = middle.Coord.Reduce2D();
            var redNeighbour = neighbour.Coord.Reduce2D();

            if (redMiddle.y < redNeighbour.y)
                return true;
            if (redMiddle.y.Equals(redNeighbour.y) && redMiddle.x > redNeighbour.x)
            {
                return true;
            }
            return false;
        }

        //Vertices need to be reduced to 2D
        private static bool IsAngleGreaterPi(float3 first, float3 second)
        {
            var redFirst = first.Reduce2D();
            var redSecond = second.Reduce2D();

            var cross = redFirst.x * redSecond.y - redFirst.y * redSecond.x; //z component of the cross product
            var dot = float3.Dot(first, second);

            var angle = (float)System.Math.Atan2(cross, dot);
            var deg = M.RadiansToDegrees(angle);
            if ((angle * -1).Equals(M.Pi))
                return false;
            return angle < 0;
        }
        #endregion

        #region MakeMonotone
        private static void MakeMonotone(FaceHandle face)
        {
            var vertices = _geometry.GetFaceVertices(face).ToList();
            var sortedVertices = GetSortedVertices(vertices.ToList());
            var faceHalfEdges = _geometry.GetHalfEdgesOfFace(face).ToList();

            var newFaces = new List<FaceHandle>();

            _sweepLineStatus = new BinarySearchTree<StatusEdge>();
            
            while (sortedVertices.Count != 0)
            {
                var current = sortedVertices[0];

                TestVertexType(current, face, newFaces);

                switch (_vertType)
                {
                    case VertexType.START_VERTEX:
                        HandleStartVertex(ref _sweepLineStatus, current, faceHalfEdges);
                        break;
                    case VertexType.END_VERTEX:
                        HandleEndVertex(ref _sweepLineStatus, current, faceHalfEdges, newFaces);
                        break;
                    case VertexType.SPLIT_VERTEX:
                        HandleSplitVertex(ref _sweepLineStatus, current, newFaces);
                        break;
                    case VertexType.MERGE_VERTEX:
                        HandleMergeVertex(ref _sweepLineStatus, current, faceHalfEdges, newFaces);
                        break;
                    case VertexType.REGULAR_VERTEX:
                        HandleRegularVertex(ref _sweepLineStatus, current, faceHalfEdges, newFaces);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                sortedVertices.RemoveAt(0);
            }
        }

        private static void TestVertexType(Geometry.Vertex vert, FaceHandle faceHandle, ICollection<FaceHandle> newFaces)
        {
            var heStartingAtFace = _geometry.GetHalfEdgesStartingAtV(vert).ToList();
            var incidentHalfEdge = new Geometry.HalfEdge();

            foreach (var he in heStartingAtFace)
            {
                var incidentFace = _geometry.GetHalfEdgeByHandle(he).IncidentFace;
                if (incidentFace.Equals(faceHandle))
                {
                    incidentHalfEdge = _geometry.GetHalfEdgeByHandle(he);
                    break;
                }
                foreach (var fh in newFaces)
                {
                    if (incidentFace.Equals(fh))
                        incidentHalfEdge = _geometry.GetHalfEdgeByHandle(he);
                }
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
                    _vertType = VertexType.SPLIT_VERTEX;
                else
                {
                    _vertType = VertexType.START_VERTEX;
                }
            }
            else if (IsOverVert(vert, nextVert) && IsOverVert(vert, prevVert))
            {
                if (IsAngleGreaterPi(v1, v2))
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

        private static void HandleStartVertex(ref BinarySearchTree<StatusEdge> tree, Geometry.Vertex v, IEnumerable<HalfEdgeHandle> faceHalfEdges)
        {
            foreach (var halfEdge in faceHalfEdges)
            {
                var he = _geometry.GetHalfEdgeByHandle(halfEdge);

                if (he.Origin != v.Handle) continue;

                UpdateKey(tree, v);

                //Optimization: Resort the try by balancing it once (only if "FindLargestSamllerThan()" is called)
                ResortTree(ref tree);

                var origin = _geometry.GetVertexByHandle(he.Origin);
                var targetH = _geometry.GetHalfEdgeByHandle(he.Next).Origin;
                var target = _geometry.GetVertexByHandle(targetH);

                var ei = new StatusEdge(origin, target, v);
                ei.HalfEdge = he.Handle;
                ei.Helper = v.Handle;
                ei.IsMergeVertex = false;

                tree.InsertNode(ei);
                break;
            }
        }

        private static void HandleEndVertex(ref BinarySearchTree<StatusEdge> tree, Geometry.Vertex v, IEnumerable<HalfEdgeHandle> faceHalfEdges, ICollection<FaceHandle> newFaces)
        {
            foreach (var heh in faceHalfEdges)
            {
                var he = _geometry.GetHalfEdgeByHandle(heh);

                if (he.Origin != v.Handle) continue;

                UpdateKey(tree, v);

                //Optimization: Resort the try by balancing it once (only if "FindLargestSamllerThan()" is called)
                ResortTree(ref tree);

                var eMinOne = FindStatusEdgeWithHandle(tree, he.Prev);

                if (eMinOne.IsMergeVertex)
                {
                    _geometry.InsertEdge(v.Handle, eMinOne.Helper);
                    newFaces.Add(_geometry.FaceHandles.LastItem());
                }

                tree.DeleteNode(eMinOne);
                break;
            }
        }

        private static void HandleSplitVertex(ref BinarySearchTree<StatusEdge> tree, Geometry.Vertex v, ICollection<FaceHandle> newFaces)
        {
            UpdateKey(tree, v);
            //Optimization: Resort the tree by balancing it once (only if "FindLargestSamllerThan()" is called)
            ResortTree(ref tree);

            //Optimization: Resort the tree by balancing it once (only if "FindLargestSamllerThan()" is called)
            var ej = FindLargestSmallerThan(v.Coord.x, tree);

            _geometry.InsertEdge(v.Handle, ej.Helper);
            newFaces.Add(_geometry.FaceHandles.LastItem());

            tree.FindNode(ej).Helper = v.Handle;
            tree.FindNode(ej).IsMergeVertex = false;

            var he = _geometry.GetHalfEdgeByHandle(v.IncidentHalfEdge);
            var origin = _geometry.GetVertexByHandle(he.Origin);
            var targetH = _geometry.GetHalfEdgeByHandle(he.Next).Origin;
            var target = _geometry.GetVertexByHandle(targetH);

            var ei = new StatusEdge(origin, target, v);
            ei.HalfEdge = v.IncidentHalfEdge;
            ei.Helper = v.Handle;
            ei.IsMergeVertex = false;

            tree.InsertNode(ei);
        }

        private static void HandleMergeVertex(ref BinarySearchTree<StatusEdge> tree, Geometry.Vertex v, IEnumerable<HalfEdgeHandle> faceHalfEdges, ICollection<FaceHandle> newFaces)
        {
            var he = new Geometry.HalfEdge();
            foreach (var heh in faceHalfEdges)
            {
                he = _geometry.GetHalfEdgeByHandle(heh);

                if (he.Origin == v.Handle) break;
            }

            UpdateKey(tree, v);

            //Optimization: Resort the try by balancing it once (only if "FindLargestSamllerThan()" is called)
            ResortTree(ref tree);

            var eMinOne = FindStatusEdgeWithHandle(tree,he.Prev);

            if (eMinOne.IsMergeVertex)
            {
                _geometry.InsertEdge(v.Handle, eMinOne.Helper);
                newFaces.Add(_geometry.FaceHandles.LastItem());
            }

            tree.DeleteNode(eMinOne);

            //Optimization: Resort the try by balancing it once (only if "FindLargestSamllerThan()" is called)
            var ej = FindLargestSmallerThan(v.Coord.x, tree);

            if (ej.IsMergeVertex)
            {
                _geometry.InsertEdge(v.Handle, ej.Helper);
                newFaces.Add(_geometry.FaceHandles.LastItem());
            }

            tree.FindNode(ej).Helper = v.Handle;
            tree.FindNode(ej).IsMergeVertex = true;
        }

        private static void HandleRegularVertex(ref BinarySearchTree<StatusEdge> tree, Geometry.Vertex v, IEnumerable<HalfEdgeHandle> faceHalfEdges, ICollection<FaceHandle> newFaces)
        {
            if (IsPolygonRightOfVert(v))
            {
                foreach (var heh in faceHalfEdges)
                {
                    var he = _geometry.GetHalfEdgeByHandle(heh);

                    if (he.Origin != v.Handle) continue;

                    UpdateKey(tree, v);

                    //Optimization: Resort the tree by balancing it once (only if "FindLargestSamllerThan()" is called)
                    ResortTree(ref tree);

                    var eMinOne = FindStatusEdgeWithHandle(tree,he.Prev);

                    if (eMinOne.IsMergeVertex)
                    {
                        _geometry.InsertEdge(v.Handle, eMinOne.Helper);
                        newFaces.Add(_geometry.FaceHandles.LastItem());
                    }

                    tree.DeleteNode(eMinOne);

                    var halfEdge = _geometry.GetHalfEdgeByHandle(v.IncidentHalfEdge);
                    var origin = _geometry.GetVertexByHandle(halfEdge.Origin);
                    var targetH = _geometry.GetHalfEdgeByHandle(halfEdge.Next).Origin;
                    var target = _geometry.GetVertexByHandle(targetH);

                    var ei = new StatusEdge(origin, target, v);
                    ei.HalfEdge = v.IncidentHalfEdge;
                    ei.Helper = v.Handle;
                    ei.IsMergeVertex = false;


                    tree.InsertNode(ei);

                    break;
                }
            }
            else
            {

                UpdateKey(tree, v);

                //Optimization: Resort the tree by balancing it once (only if "FindLargestSamllerThan()" is called)
                ResortTree(ref tree);

                //Optimization: Resort the tree by balancing it once (only if "FindLargestSamllerThan()" is called)
                var ej = FindLargestSmallerThan(v.Coord.x, tree);

                if (ej.IsMergeVertex)
                {
                    _geometry.InsertEdge(v.Handle, ej.Helper);
                    newFaces.Add(_geometry.FaceHandles.LastItem());
                }

                tree.FindNode(ej).Helper = v.Handle;
                tree.FindNode(ej).IsMergeVertex = false;
            }
        }

        private static void UpdateKey(BinarySearchTree<StatusEdge> tree, Geometry.Vertex eventPoint)
        {
            foreach (var node in tree.PreorderTraverseTree())
            {
                node.SetKey(eventPoint);
            }
        }

        private static void ResortTree(ref BinarySearchTree<StatusEdge> tree)
        {
            var nodes = tree.PreorderTraverseTree();
            tree = new BinarySearchTree<StatusEdge>();
            foreach (var n in nodes)
            {
                tree.InsertNode(n);
            }
        }

        //Custom implementation of BinaryTrees FindNode method - works with StatusNodes
        private static StatusEdge FindStatusEdgeWithHandle(BinarySearchTree<StatusEdge> tree, HalfEdgeHandle handle)
        {
            foreach (var n in tree.PreorderTraverseTree())
            {
                if (n.HalfEdge == handle)
                    return n;
            }
            return null;
        }

        //Can be optimized by using a self balancing binary search tree. See: http://stackoverflow.com/questions/6334514/to-find-largest-element-smaller-than-k-in-a-bst
        //Idea: Instead of resorting the tree if key are changed (if a new event point is hit), one-time-balance it if this method is called) - possibly faster
        private static StatusEdge FindLargestSmallerThan(float value, BinarySearchTree<StatusEdge> tree)
        {
            var preorder = tree.PreorderTraverseTree().ToList();
            var temp = default(StatusEdge);

            foreach (var n in preorder)
            {
                if (!(n.Key < value)) continue;
                temp = n;
                break;
            }

            if (preorder.Count == 1) return temp;

            foreach (var n in preorder)
            {
                if (n.Key < value && n.Key > temp.Key)
                    temp = n;
            }
            return temp;
        }
        
        #endregion

        #region General methods

        private static bool IsPolygonRightOfVert(Geometry.Vertex vert)
        {
            var prevV = GetPrevVertex(vert);
            var nextV = GetNextVertex(vert);

            return prevV.Coord.y > nextV.Coord.y;
        }

        //Vertices need to be reduced to 2D.
        //Can be optimized by implementing a priority queue data structure and use it insted of sorting a list
        private static IList<Geometry.Vertex> GetSortedVertices(IEnumerable<Geometry.Vertex> unsorted)
        {
            var sorted = new List<Geometry.Vertex>();
            sorted.AddRange(unsorted);
            sorted.Sort(delegate (Geometry.Vertex a, Geometry.Vertex b)
            {
                var redA = a.Coord.Reduce2D();
                var redB = b.Coord.Reduce2D();

                var ydiff = -1 * redA.y.CompareTo(redB.y);
                if (ydiff != 0) return ydiff;
                return redA.x.CompareTo(redB.x);
            });

            return sorted;
        }
        
        private static Geometry.Vertex GetNextVertex(Geometry.Vertex current)
        {
            var currentHe = _geometry.GetHalfEdgeByHandle(current.IncidentHalfEdge);
            var nextHe = _geometry.GetHalfEdgeByHandle(currentHe.Next);

            return _geometry.GetVertexByHandle(nextHe.Origin);
        }

        private static Geometry.Vertex GetPrevVertex(Geometry.Vertex current)
        {
            var currentHe = _geometry.GetHalfEdgeByHandle(current.IncidentHalfEdge);
            var prevHe = _geometry.GetHalfEdgeByHandle(currentHe.Prev);

            return _geometry.GetVertexByHandle(prevHe.Origin);
        }

        //(Obsolete) See: Antionio, Franklin - Faster line intersection (1992)
        //Assumption: z component of float3 is always 0
        private static bool AreLinesIntersecting(float3 p1, float3 p2, float3 p3, float3 p4)
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

        #endregion
    }
}
