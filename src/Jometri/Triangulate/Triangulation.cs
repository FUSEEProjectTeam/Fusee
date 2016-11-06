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
        
        #region Test face for y monotony

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

        #endregion


        #region MakeMonotone
        private void MakeMonotone(FaceHandle face)
        {
            var vertices = _geometry.GetVeticesFromFace(face);
            var sortedVertices = SortedVertices(vertices.ToList());
            var faceHalfEdges = _geometry.HalfEdgesOfFace(face).ToList();

            var newFaces = new List<FaceHandle>();

            _sweepLineStatus = new BinarySearchTree<StatusNode>();

            while (sortedVertices.Count != 0)
            {
                var current = sortedVertices[0];
                

                TestVertexType(current, face, newFaces);

                Debug.WriteLine(_vertType);

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
                    Coord = _geometry.GetVertexByHandle(he.Origin).Coord, HalfEdge = he.Handle, Helper = v.Handle, IsMergeVertex = false
                };
                _root = tree.InsertNode(_root, ei);
                break;
            }
        }

        private void HandleEndVertex(ref BinarySearchTree<StatusNode> tree, Geometry.Vertex v, IEnumerable<HalfEdgeHandle> faceHalfEdges, ICollection<FaceHandle> newFaces )
        {
            foreach (var heh in faceHalfEdges)
            {
                var he = _geometry.GetHalfEdgeByHandle(heh);

                if (he.Origin.Id != v.Handle.Id) continue;

                var prevHalfEdge = _geometry.GetHalfEdgeByHandle(he.Prev);
                var x = _geometry.GetVertexByHandle(prevHalfEdge.Origin).Coord.x;

                var eMinOne = FindStatusNode(_root, x).Value;

                if (eMinOne.IsMergeVertex)
                {
                    _geometry.InsertHalfEdge(v.Handle, eMinOne.Helper);
                   newFaces.Add(_geometry.FaceHandles.LastItem());
                }

                tree.DeleteNode(ref _root, eMinOne);

                break;
            }
        }

        private void HandleSplitVertex(ref BinarySearchTree<StatusNode> tree, Geometry.Vertex v, ICollection<FaceHandle> newFaces )
        {
            foreach (var n in tree.PreorderTraverseTree(_root))
            {
                Debug.WriteLine(n.Coord + " " + n.Helper.Id + " Is Merge " + n.IsMergeVertex);
            }

            var ej = FindLargestSmallerThan(_root, v.Coord.x).Value;

            _geometry.InsertHalfEdge(v.Handle, ej.Helper);
            newFaces.Add(_geometry.FaceHandles.LastItem());

            tree.FindNode(_root, ej).Value.Helper = v.Handle;
            tree.FindNode(_root, ej).Value.IsMergeVertex = false;

            var ei = new StatusNode
            {
                HalfEdge = v.IncidentHalfEdge, Helper = v.Handle, IsMergeVertex = false, Coord = v.Coord
            };

            tree.InsertNode(_root, ei);
        }

        private void HandleMergeVertex(ref BinarySearchTree<StatusNode> tree, Geometry.Vertex v, IEnumerable<HalfEdgeHandle> faceHalfEdges, ICollection<FaceHandle> newFaces)
        {
            foreach (var heh in faceHalfEdges)
            {
                var he = _geometry.GetHalfEdgeByHandle(heh);

                if (he.Origin.Id != v.Handle.Id) continue;

                var prevHalfEdge = _geometry.GetHalfEdgeByHandle(he.Prev);
                var x = _geometry.GetVertexByHandle(prevHalfEdge.Origin).Coord.x;

                var eMinOne = FindStatusNode(_root, x).Value;

                if (eMinOne.IsMergeVertex)
                {
                    _geometry.InsertHalfEdge(v.Handle, eMinOne.Helper);
                    newFaces.Add(_geometry.FaceHandles.LastItem());
                }

                tree.DeleteNode(ref _root, eMinOne);

                var ej = FindLargestSmallerThan(_root, v.Coord.x).Value;

                if (ej.IsMergeVertex)
                {
                    _geometry.InsertHalfEdge(v.Handle, ej.Helper);
                    newFaces.Add(_geometry.FaceHandles.LastItem());
                }

                tree.FindNode(_root, ej).Value.Helper = v.Handle;
                tree.FindNode(_root, ej).Value.IsMergeVertex = true;

                break;
            }
        }

        private void HandleRegularVertex(BinarySearchTree<StatusNode> tree, Geometry.Vertex v, IEnumerable<HalfEdgeHandle> faceHalfEdges, ICollection<FaceHandle> newFaces)
        {
            if (IsPolygonRightOfRegular(v))
            {
                foreach (var heh in faceHalfEdges)
                {
                    var he = _geometry.GetHalfEdgeByHandle(heh);

                    if (he.Origin.Id != v.Handle.Id) continue;

                    var prevHalfEdge = _geometry.GetHalfEdgeByHandle(he.Prev);
                    var x = _geometry.GetVertexByHandle(prevHalfEdge.Origin).Coord.x;
                    var eMinOne = FindStatusNode(_root, x).Value;

                    if (eMinOne.IsMergeVertex)
                    {
                        _geometry.InsertHalfEdge(v.Handle, eMinOne.Helper);
                        newFaces.Add(_geometry.FaceHandles.LastItem());
                    }

                    tree.DeleteNode(ref _root, eMinOne);

                    var ei = new StatusNode
                    {
                        HalfEdge = v.IncidentHalfEdge, Helper = v.Handle, IsMergeVertex = false, Coord = v.Coord
                    };
                    tree.InsertNode(_root, ei);

                    break;
                }
            }
            else
            {
                foreach (var n in tree.PreorderTraverseTree(_root))
                {
                    Debug.WriteLine(n.Coord + " " + n.Helper.Id + " Is Merge " + n.IsMergeVertex);
                }

                var ej = FindLargestSmallerThan(_root, v.Coord.x).Value;

                if (ej.IsMergeVertex)
                {
                    _geometry.InsertHalfEdge(v.Handle, ej.Helper);
                    newFaces.Add(_geometry.FaceHandles.LastItem());
                }

                tree.FindNode(_root, ej).Value.Helper = v.Handle;
                tree.FindNode(_root, ej).Value.IsMergeVertex = false;
            }
        }

        private static IList<Geometry.Vertex> SortedVertices(IEnumerable<Geometry.Vertex> unsorted)
        {
            var sorted = new List<Geometry.Vertex>();
            sorted.AddRange(unsorted);
            sorted.Sort(delegate(Geometry.Vertex a, Geometry.Vertex b)
            {
                var ydiff = -1*a.Coord.y.CompareTo(b.Coord.y);
                if (ydiff != 0) return ydiff;
                return a.Coord.x.CompareTo(b.Coord.x);
            });

            return sorted;
        }

        private bool IsPolygonRightOfRegular(Geometry.Vertex vert)
        {
            var prevV = GetPrevVertex(vert);
            var nextV = GetNextVertex(vert);

            return prevV.Coord.y > nextV.Coord.y;
        }

        //Custom implementation of BinaryTrees FindNode method - works with StatusNodes
        private static Node<StatusNode> FindStatusNode(Node<StatusNode> root, float value)
        {
            Node<StatusNode> res = null;
            if (root.LeftNode != null)
                res = FindStatusNode(root.LeftNode, value);

            if (value.CompareTo(root.Value.Coord.x) == 0)
                return root;

            if (res == null && root.RightNode != null)
                res = FindStatusNode(root.RightNode, value);

            return res;
        }

        private static Node<StatusNode> FindLargestSmallerThan(Node<StatusNode> root, float value)
        {
            var res = root;

            while (root != null)
            {
                if (root.Value.Coord.x >= value)
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

        private void TestVertexType(Geometry.Vertex vert, FaceHandle faceHandle, ICollection<FaceHandle> newFaces)
        {
            var heStartingAtFace = _geometry.HalfEdgesStartingAtV(vert).ToList();

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
            if (incidentHalfEdge.Handle.Id == 0)
            {
                Debug.WriteLine("error!");
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
            var heStartingAtFace = _geometry.HalfEdgesStartingAtV(vert).ToList();

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

            var tNumerator = b.y*b.x - b.x*c.y;
            var iNumerator = a.x*c.y - a.y*c.x;

            var denominator = a.y*b.x - a.x*b.y;

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
