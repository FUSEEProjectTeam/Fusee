using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fusee.Base.Core;
using Fusee.Jometri.DCEL;
using Fusee.Math.Core;

namespace Fusee.Jometri.Triangulate
{
    /// <summary>
    /// Contains the triangulation of some geometry, stored in a doubly connected halfe edge list.
    /// </summary>
    public class Triangulation
    {

        private readonly Geometry _geometry;
        private VertexType _vertType;
        private BinarySearchTree<StatusNode> _sweepLineStatus;
        private Node<StatusNode> _root;


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

        private void Triangulate()
        {
            var originalFaces = new List<FaceHandle>();
            originalFaces.AddRange(_geometry.FaceHandles);

            foreach (var fHandle in originalFaces)
            {
                if (!IsMonotone(fHandle))
                    MakeMonotone(fHandle);
            }

            var monotoneFaces  = new List<FaceHandle>();
            monotoneFaces.AddRange(_geometry.FaceHandles);
            foreach (var fHandle in monotoneFaces)
            {
                TriangulateMonotone(fHandle);
            }
        }
        #region Triangulate monotone polygone
        private void TriangulateMonotone(FaceHandle fHandle)
        {
            var faceVertices = _geometry.GetFaceVertices(fHandle).ToList();
            if (faceVertices.Count.Equals(3)) return;

            var sortedVerts = GetSortedVertices(faceVertices);
            var vertStack = new Stack<Geometry.Vertex>();
            var leftCain = GetLeftChain(sortedVerts, fHandle).ToList();

            vertStack.Push(sortedVerts[0]);
            vertStack.Push(sortedVerts[1]);

            for (var i = 2; i < sortedVerts.Count-1; i++)
            {
                var current = sortedVerts[i];
                
                if (!IsLeftChain(leftCain, current) && IsLeftChain(leftCain, vertStack.Peek()) ||
                    IsLeftChain(leftCain, current) && !IsLeftChain(leftCain, vertStack.Peek()))
                {
                    while (vertStack.Count > 0)
                    {
                        var popped = vertStack.Pop();

                        if (vertStack.Count >= 1)
                            _geometry.InsertHalfEdge(current.Handle, popped.Handle);
                    }
                    vertStack.Push(sortedVerts[i - 1]);
                    vertStack.Push(current);
                }
                else
                {
                    var popped = vertStack.Pop();
                    var isValidDiagonal = true;

                    while (vertStack.Count > 0 && isValidDiagonal)
                    {
                        var v2 = vertStack.Peek().Coord - popped.Coord;
                        var v1 = sortedVerts[i].Coord - popped.Coord;

                        popped = vertStack.Pop();

                        if (IsAngleGreaterOrEqualPi(v1, v2))
                            isValidDiagonal = false;

                        if(isValidDiagonal)
                            _geometry.InsertHalfEdge(current.Handle, popped.Handle);
                    }
                    vertStack.Push(popped);
                    vertStack.Push(current);
                }
            }
            for (var j = 0; j < vertStack.Count; j++)
            {
                if (j == 0 || j == vertStack.Count - 1) { }
                else { _geometry.InsertHalfEdge(sortedVerts.LastItem().Handle, sortedVerts[j].Handle); }

            }
        }

        private IEnumerable<Geometry.Vertex> GetLeftChain(IList<Geometry.Vertex> sortedVerts, FaceHandle fHandle)
        {
            var heHandle = new HalfEdgeHandle();
            var endOfChain = sortedVerts.LastItem();

            var startingAtFirstV = _geometry.GetHalfEdgesStartingAtV(sortedVerts[0]).ToList();
            if (startingAtFirstV.Count > 1)
            {
                foreach (var heh in startingAtFirstV)
                {
                    var he = _geometry.GetHalfEdgeByHandle(heh);
                    if (he.IncidentFace.Id == fHandle.Id)
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

            } while (_geometry.GetHalfEdgeByHandle(heHandle).Origin.Id != endOfChain.Handle.Id);
        }

        private static bool IsLeftChain(IEnumerable<Geometry.Vertex> leftChain, Geometry.Vertex cur)
        {
            foreach (var v in leftChain)
            {
                if (v.Handle.Id == cur.Handle.Id)
                    return true;
            }
            return false;
        }

        private static bool IsAngleGreaterOrEqualPi(float3 first, float3 second)
        {
            var cross = first.x * second.y - first.y * second.x; //z component of the cross product
            var dot = float3.Dot(first, second);

            var angle = (float)System.Math.Atan2(cross, dot);
            var deg = M.RadiansToDegrees(angle);
            return angle <= 0;
        }

        #endregion

        #region Test face for y monotony

        private bool IsMonotone(FaceHandle faceHandle)
        {
            var face = _geometry.GetFaceByHandle(faceHandle);
            var noSplitOrMerge = HasNoSplitOrMerge(faceHandle);

            return noSplitOrMerge && face.InnerHalfEdges.Count == 0;
        }

        private bool HasNoSplitOrMerge(FaceHandle faceHandle)
        {
            var verts = _geometry.GetFaceVertices(faceHandle).ToList();

            foreach (var vert in verts)
            {
                TestVertexType(vert, faceHandle);
                if (_vertType.Equals(VertexType.SplitVertex) || _vertType.Equals(VertexType.MergeVertex))
                    return false;
            }
            return true;
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
        #endregion
        
        #region MakeMonotone
        private void MakeMonotone(FaceHandle face)
        {
            var vertices = _geometry.GetFaceVertices(face);
            var sortedVertices = GetSortedVertices(vertices.ToList());
            var faceHalfEdges = _geometry.GetHalfEdgesOfFace(face).ToList();

            var newFaces = new List<FaceHandle>();

            _sweepLineStatus = new BinarySearchTree<StatusNode>();
            _root = null;

            while (sortedVertices.Count != 0)
            {
                var current = sortedVertices[0];

                TestVertexType(current, face, newFaces);

                //Debug.WriteLine(current.Handle.Id+" HalfEdgeOrigin " +current.HalfEdgeOrigin +  _vertType);
                
                foreach (var p in _sweepLineStatus.PreorderTraverseTree(_root))
                {
                    Debug.WriteLine("HE " + p.HalfEdge.Id + " Helper " + p.Helper.Id + " Helper HalfEdgeOrigin " + p.HalfEdgeOrigin);
                }
                Debug.WriteLine(_vertType + "current " + current.Handle.Id);

                switch (_vertType)
                {
                    case VertexType.StartVertex:
                        HandleStartVertex(ref _sweepLineStatus, current, faceHalfEdges);
                        break;
                    case VertexType.EndVertex:
                        HandleEndVertex(ref _sweepLineStatus, current, faceHalfEdges, newFaces);
                        break;
                    case VertexType.SplitVertex:
                        HandleSplitVertex(ref _sweepLineStatus, current, newFaces);
                        break;
                    case VertexType.MergeVertex:
                        HandleMergeVertex(ref _sweepLineStatus, current, faceHalfEdges, newFaces);
                        break;
                    case VertexType.RegularVertex:
                        HandleRegularVertex(_sweepLineStatus, current, faceHalfEdges, newFaces);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                sortedVertices.RemoveAt(0);
            }
        }

        private void HandleStartVertex(ref BinarySearchTree<StatusNode> tree, Geometry.Vertex v, IEnumerable<HalfEdgeHandle> faceHalfEdges)
        {
            foreach (var halfEdge in faceHalfEdges)
            {
                var he = _geometry.GetHalfEdgeByHandle(halfEdge);

                if (he.Origin.Id != v.Handle.Id) continue;

                var ei = new StatusNode
                {
                    HalfEdgeOrigin = _geometry.GetVertexByHandle(he.Origin).Coord,
                    HalfEdge = he.Handle,
                    Helper = v.Handle,
                    IsMergeVertex = false
                };
                _root = tree.InsertNode(ref _root, ei);
                break;
            }
        }

        private void HandleEndVertex(ref BinarySearchTree<StatusNode> tree, Geometry.Vertex v, IEnumerable<HalfEdgeHandle> faceHalfEdges, ICollection<FaceHandle> newFaces)
        {
            foreach (var heh in faceHalfEdges)
            {
                var he = _geometry.GetHalfEdgeByHandle(heh);

                if (he.Origin.Id != v.Handle.Id) continue;

                //var prevHalfEdge = _geometry.GetHalfEdgeByHandle(he.Prev);
                //var x = _geometry.GetVertexByHandle(prevHalfEdge.Origin).HalfEdgeOrigin.x;

                var eMinOne = FindStatusNode(_root, he.Prev).Value;

                if (eMinOne.IsMergeVertex)
                {
                    var vert1 = _geometry.GetVertexByHandle(v.Handle);
                    var vert2 = _geometry.GetVertexByHandle(eMinOne.Helper);
                    var p = new VertHandle();
                    var q = new VertHandle();

                    if (vert1.Coord.x > vert2.Coord.x)
                    {
                        p = vert1.Handle;
                        q = vert2.Handle;
                    }
                    else
                    {
                        q = vert1.Handle;
                        p = vert2.Handle;
                    }

                    _geometry.InsertHalfEdge(p, q);
                    newFaces.Add(_geometry.FaceHandles.LastItem());
                }

                tree.DeleteNode(ref _root, eMinOne);
                break;
            }
        }

        private void HandleSplitVertex(ref BinarySearchTree<StatusNode> tree, Geometry.Vertex v, ICollection<FaceHandle> newFaces)
        {
            var ej = FindLargestSmallerThan(_root, v.Coord.x).Value;

            var vert1 = _geometry.GetVertexByHandle(v.Handle);
            var vert2 = _geometry.GetVertexByHandle(ej.Helper);
            var p = new VertHandle();
            var q = new VertHandle();

            if (vert1.Coord.x < vert2.Coord.x)
            {
                p = vert1.Handle;
                q = vert2.Handle;
            }
            else
            {
                q = vert1.Handle;
                p = vert2.Handle;
            }

            _geometry.InsertHalfEdge(p, q);
            newFaces.Add(_geometry.FaceHandles.LastItem());

            tree.FindNode(_root, ej).Value.Helper = v.Handle;
            tree.FindNode(_root, ej).Value.IsMergeVertex = false;

            //Rebuild tree after v.handle changes
            /*var nodes = tree.PreorderTraverseTree(_root);
            _root = null;
            foreach (var n in nodes)
            {
                tree.InsertNode(ref _root, n);
            }*/

            var ei = new StatusNode
            {
                HalfEdge = v.IncidentHalfEdge,
                Helper = v.Handle,
                IsMergeVertex = false,
                HalfEdgeOrigin = v.Coord
            };
            
            tree.InsertNode(ref _root, ei);
        }

        private void HandleMergeVertex(ref BinarySearchTree<StatusNode> tree, Geometry.Vertex v, IEnumerable<HalfEdgeHandle> faceHalfEdges, ICollection<FaceHandle> newFaces)
        {
            foreach (var heh in faceHalfEdges)
            {
                var he = _geometry.GetHalfEdgeByHandle(heh);

                if (he.Origin.Id != v.Handle.Id) continue;

                //var prevHalfEdge = _geometry.GetHalfEdgeByHandle(he.Prev);
                //var x = _geometry.GetVertexByHandle(prevHalfEdge.Origin).HalfEdgeOrigin.x;
                var eMinOne = FindStatusNode(_root, he.Prev).Value;

                if (eMinOne.IsMergeVertex)
                {
                    var vert1 = _geometry.GetVertexByHandle(v.Handle);
                    var vert2 = _geometry.GetVertexByHandle(eMinOne.Helper);
                    var p = new VertHandle();
                    var q = new VertHandle();

                    if (vert1.Coord.x > vert2.Coord.x)
                    {
                        p = vert1.Handle;
                        q = vert2.Handle;
                    }
                    else
                    {
                        q = vert1.Handle;
                        p = vert2.Handle;
                    }

                    _geometry.InsertHalfEdge(p, q);
                    newFaces.Add(_geometry.FaceHandles.LastItem());
                }

                tree.DeleteNode(ref _root, eMinOne); 

                var ej = FindLargestSmallerThan(_root, v.Coord.x).Value;

                if (ej.IsMergeVertex)
                {
                    var vert1 = _geometry.GetVertexByHandle(v.Handle);//TODO: Make Methode
                    var vert2 = _geometry.GetVertexByHandle(ej.Helper);
                    var p = new VertHandle();
                    var q = new VertHandle();

                    if (vert1.Coord.x > vert2.Coord.x)
                    {
                        p = vert1.Handle;
                        q = vert2.Handle;
                    }
                    else
                    {
                        q = vert1.Handle;
                        p = vert2.Handle;
                    }

                    _geometry.InsertHalfEdge(p, q);
                    newFaces.Add(_geometry.FaceHandles.LastItem());
                }

                tree.FindNode(_root, ej).Value.Helper = v.Handle;
                tree.FindNode(_root, ej).Value.IsMergeVertex = true;

                //Rebuild tree after v.handle changes TODO: Make Methode!
                /*var nodes = tree.PreorderTraverseTree(_root);
                _root = null;
                foreach (var n in nodes)
                {
                    tree.InsertNode(ref _root, n);
                }*/

                break;
            }
        }

        private void HandleRegularVertex(BinarySearchTree<StatusNode> tree, Geometry.Vertex v, IEnumerable<HalfEdgeHandle> faceHalfEdges, ICollection<FaceHandle> newFaces)
        {
            if (IsPolygonRightOfVert(v))
            {
                foreach (var heh in faceHalfEdges)
                {
                    var he = _geometry.GetHalfEdgeByHandle(heh);

                    if (he.Origin.Id != v.Handle.Id) continue;

                    //var prevHalfEdge = _geometry.GetHalfEdgeByHandle(he.Prev);
                    //var x = _geometry.GetVertexByHandle(prevHalfEdge.Origin).HalfEdgeOrigin.x;

                    var eMinOne = FindStatusNode(_root, he.Prev).Value;

                    if (eMinOne.IsMergeVertex)
                    {
                        var vert1 = _geometry.GetVertexByHandle(v.Handle);
                        var vert2 = _geometry.GetVertexByHandle(eMinOne.Helper);
                        var p = new VertHandle();
                        var q = new VertHandle();

                        if (vert1.Coord.x > vert2.Coord.x)
                        {
                            p = vert1.Handle;
                            q = vert2.Handle;
                        }
                        else
                        {
                            q = vert1.Handle;
                            p = vert2.Handle;
                        }

                        _geometry.InsertHalfEdge(p, q);
                        newFaces.Add(_geometry.FaceHandles.LastItem());
                    }

                    tree.DeleteNode(ref _root, eMinOne);

                    var ei = new StatusNode
                    {
                        HalfEdge = v.IncidentHalfEdge,
                        Helper = v.Handle,
                        IsMergeVertex = false,
                        //HalfEdgeOrigin = v.Coord
                    };
                    var halfEdge = _geometry.GetHalfEdgeByHandle(ei.HalfEdge);
                    var halfEdgeOrigin = _geometry.GetVertexByHandle(halfEdge.Origin);
                    ei.HalfEdgeOrigin = halfEdgeOrigin.Coord;

                    tree.InsertNode(ref _root, ei);

                    break;
                }
            }
            else
            {
                var ej = FindLargestSmallerThan(_root, v.Coord.x).Value;

                if (ej.IsMergeVertex)
                {

                    var vert1 = _geometry.GetVertexByHandle(v.Handle);
                    var vert2 = _geometry.GetVertexByHandle(ej.Helper);
                    VertHandle p;
                    VertHandle q;

                    if (vert1.Coord.x > vert2.Coord.x)
                    {
                        p = vert1.Handle;
                        q = vert2.Handle;
                    }
                    else
                    {
                        q = vert1.Handle;
                        p = vert2.Handle;
                    }

                    _geometry.InsertHalfEdge(p, q);
                    newFaces.Add(_geometry.FaceHandles.LastItem());
                }

                tree.FindNode(_root, ej).Value.Helper = v.Handle;
                tree.FindNode(_root, ej).Value.IsMergeVertex = false;

                //Rebuild tree after v.handle changes
                /*var nodes = tree.PreorderTraverseTree(_root);
                _root = null;
                foreach (var n in nodes)
                {
                    tree.InsertNode(ref _root, n);
                }*/

            }
        }

        //Custom implementation of BinaryTrees FindNode method - works with StatusNodes
        private static Node<StatusNode> FindStatusNode(Node<StatusNode> root, HalfEdgeHandle handle)
        {
            if (root == null) return null;

            if (root.Value.HalfEdge.Equals(handle))
                return root;

            var res = FindStatusNode(root.LeftNode, handle);

            if (res == null)
                res = FindStatusNode(root.RightNode, handle);
            return res;
        }

        private static Node<StatusNode> FindLargestSmallerThan(Node<StatusNode> root, float value) //TODO: left to right order of the LEAVES correspont to left to right order of the edges - fix methode to find leave with greatest value
        {
            var res = root;

            while (root != null)
            {
                if (root.Value.HalfEdgeOrigin.x >= value)
                    root = root.LeftNode;
                else
                {
                    res = root;
                    root = root.RightNode;
                }
            }
            return res;
        }

        #endregion

        #region General methods

        private bool IsPolygonRightOfVert(Geometry.Vertex vert)
        {
            var prevV = GetPrevVertex(vert);
            var nextV = GetNextVertex(vert);

            return prevV.Coord.y > nextV.Coord.y;
        }

        //Vertices need to be reduced to 2D
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

        private void TestVertexType(Geometry.Vertex vert, FaceHandle faceHandle, ICollection<FaceHandle> newFaces)
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

        private void TestVertexType(Geometry.Vertex vert, FaceHandle faceHandle)
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

        private Geometry.Vertex GetNextVertex(Geometry.Vertex current)
        {
            var currentHe = _geometry.GetHalfEdgeByHandle(current.IncidentHalfEdge);
            var nextHe = _geometry.GetHalfEdgeByHandle(currentHe.Next);

            return _geometry.GetVertexByHandle(nextHe.Origin);
        }

        private Geometry.Vertex GetPrevVertex(Geometry.Vertex current)
        {
            var currentHe = _geometry.GetHalfEdgeByHandle(current.IncidentHalfEdge);
            var prevHe = _geometry.GetHalfEdgeByHandle(currentHe.Prev);

            return _geometry.GetVertexByHandle(prevHe.Origin);
        }

        //(Obsolete)
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

        #endregion
    }
}
