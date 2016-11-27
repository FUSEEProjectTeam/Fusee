using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Core;
using Fusee.Jometri.Triangulation;
using Fusee.Math.Core;

namespace Fusee.Jometri.DCEL
{
    /// <summary>
    /// Geometry, stored in a DCEL (doubly conneted edge list).
    /// </summary>
    public class Geometry
    {
        #region Members

        /// <summary>
        /// Contains handles to half edges. Use this to find adjecant half edges or the origin vertex.
        /// </summary>
        public IList<HalfEdgeHandle> HalfEdgeHandles;

        /// <summary>
        /// Contains handles to outlines. Use this to find the first half edge.
        /// </summary>
        public IList<FaceHandle> FaceHandles;

        /// <summary>
        /// Contains handles to outline. Use this to get the vertexes coordinates.
        /// </summary>
        public IList<VertHandle> VertHandles;

        private readonly List<Vertex> _vertices;
        private readonly List<HalfEdge> _halfEdges;
        private readonly List<Face> _faces;

        private int _highestVertHandle;
        private int _highestHalfEdgeHandle;
        private int _highestFaceHandle;

        #endregion

        /// <summary>
        /// Geometry, stored in a DCEL (doubly conneted edge list).
        /// </summary>
        /// <param name="outlines">A collection of the geometrys' outlines, each containing the geometric information as a list of float3 in ccw order</param>
        /// <param name="triangulate">If triangulate is set to true, the created geometry will be triangulated</param>
        public Geometry(IEnumerable<Outline> outlines, bool triangulate = false)
        {
            _vertices = new List<Vertex>();
            _halfEdges = new List<HalfEdge>();
            _faces = new List<Face>();

            HalfEdgeHandles = new List<HalfEdgeHandle>();
            FaceHandles = new List<FaceHandle>();
            VertHandles = new List<VertHandle>();

            CreateHalfEdgesForGeometry(outlines);

            if (triangulate)
                this.Triangulate();
        }

        //Copies a existing Geometry object. E.g to create a backface for an extrusion
        internal Geometry(Geometry geom)
        {
            VertHandles = new List<VertHandle>();
            foreach (var vHandle in geom.VertHandles)
            {
                VertHandles.Add(vHandle);
            }

            _vertices = new List<Vertex>();
            foreach (var vert in geom._vertices)
            {
                _vertices.Add(vert);
            }

            HalfEdgeHandles = new List<HalfEdgeHandle>();
            foreach (var heHandle in geom.HalfEdgeHandles)
            {
                HalfEdgeHandles.Add(heHandle);
            }
            _halfEdges = new List<HalfEdge>();
            foreach (var he in geom._halfEdges)
            {
                _halfEdges.Add(he);
            }

            FaceHandles = new List<FaceHandle>();
            foreach (var fHandle in geom.FaceHandles)
            {
                FaceHandles.Add(fHandle);
            }
            _faces = new List<Face>();
            foreach (var f in geom._faces)
            {
                _faces.Add(f);
            }
        }

        #region Structs

        /// <summary>
        /// Each face contains:
        /// A handle to assign a abstract reference to it.
        /// A handle to one of the half edges that belongs to this faces outer boundary.
        /// A List that contains handles to one half edge for each hole in a face
        /// Note that unbounded faces can't have a OuterHalfEdge but need to have at least one InnerHalfEdge - bounded faces must have a OuterComponent
        /// </summary>
        internal struct Face
        {
            internal FaceHandle Handle;
            internal HalfEdgeHandle OuterHalfEdge;
            internal List<HalfEdgeHandle> InnerHalfEdges;
        }

        /// <summary>
        /// Each vertex contains:
        /// A handle to assign a abstract reference to it.
        /// The vertex' coordinates.
        /// </summary>
        public struct Vertex
        {
            /// <summary>
            /// The vertex' reference.
            /// </summary>
            public VertHandle Handle;

            /// <summary>
            /// The geometric data of the vertex
            /// </summary>
            public float3 Coord;

            /// <summary>
            /// The handle to the half edge with this vertex as origin
            /// </summary>
            public HalfEdgeHandle IncidentHalfEdge;


            /// <summary>
            /// The vertex' constuctor.
            /// </summary>
            /// <param name="coord">The new vertex' coordinates</param>
            public Vertex(float3 coord)
            {
                Handle = new VertHandle();
                IncidentHalfEdge = new HalfEdgeHandle();
                Coord = coord;
            }

            public void ChangeCoord(float3 newCoord)
            {
                Coord = newCoord;
            }
        }

        /// <summary>
        /// Represents a half edge.
        /// Each half edge contains:
        /// A handle to assign a abstract reference to it.
        /// A handle to the half edges' origin vertex.
        /// A handle to the next half edge (in ccw order).
        /// A handle to the previous half edge (in ccw order).
        /// A handle to the face it belongs to.
        /// </summary>
        internal struct HalfEdge
        {
            internal HalfEdgeHandle Handle;

            internal VertHandle Origin;
            internal HalfEdgeHandle Twin;
            internal HalfEdgeHandle Next;
            internal HalfEdgeHandle Prev;
            internal FaceHandle IncidentFace;
        }

        /// <summary>
        /// Represents a outer or inner boundary of a face
        /// </summary>
        public struct Outline
        {
            /// <summary>
            /// The geometric information of the vertices which belong to a boundary
            /// </summary>
            public IList<float3> Points;

            /// <summary>
            /// Determines wheather a boundary is a outer bondary or a inner boundary (which forms a hole in the face).
            /// </summary>
            public bool IsOuter;
        }

        #endregion

        /*Insert methods like:
            >InsertVertex
            >InsertFace
            >Get all edges adjecant to a vertex
            >Get all edges that belong to a face
            >etc.
        */

        #region public Methods

        /// <summary>
        /// Inserts a new face between vertices of the geometry
        /// </summary>
        /// <param name="vertices">The vertices of the new face, they have to be in ccw order (according to the orientation of the new face)</param>
        public void InsertFace(IList<VertHandle> vertices)
        {
            var faceHandle = new FaceHandle(FaceHandles.Count + 1);
            var face = new Face
            {
                Handle = faceHandle,
                InnerHalfEdges = new List<HalfEdgeHandle>()
            };


            var faceHalfEdges = new List<HalfEdge>();

            foreach (var vertHandle in vertices)
            {
                var halfEdgeHandle = new HalfEdgeHandle(HalfEdgeHandles.Count + 1);
                HalfEdgeHandles.Add(halfEdgeHandle);
                var halfEdge = new HalfEdge
                {
                    Origin = vertHandle,
                    Handle = halfEdgeHandle,
                    IncidentFace = face.Handle
                };

                faceHalfEdges.Add(halfEdge);
            }

            var origHalfEdges = new List<HalfEdge>();
            origHalfEdges.AddRange(_halfEdges);

            for (var i = 0; i < faceHalfEdges.Count; i++)
            {
                //Assign face.OuterHalfEdge
                if (i == 0)
                {
                    face.OuterHalfEdge = faceHalfEdges[i].Handle;
                    _faces.Add(face);
                    FaceHandles.Add(face.Handle);
                }

                var current = faceHalfEdges[i];
                current.Next = faceHalfEdges[(i + 1) % faceHalfEdges.Count].Handle;
                current.Prev = i - 1 >= 0 ? faceHalfEdges[i - 1].Handle : faceHalfEdges.LastItem().Handle;

                //Check if new HalfEdge is a twin to some other (already existing) HalfEdge
                var curOrigin = current.Origin;
                var curTarget = new VertHandle();
                foreach (var hEdge in faceHalfEdges)
                {
                    if (current.Next != hEdge.Handle) continue;
                    curTarget = hEdge.Origin;
                    break;
                }

                for (var j = 0; j < origHalfEdges.Count; j++)
                {
                    var he = origHalfEdges[j];
                    var heOrigin = he.Origin;
                    var heTarget = GetHalfEdgeByHandle(he.Next).Origin;

                    if (heOrigin == curTarget && heTarget == curOrigin && he.Twin == default(HalfEdgeHandle))
                    {
                        current.Twin = he.Handle;
                        he.Twin = current.Handle;

                        _halfEdges[j] = he;
                        break;
                    }
                    if (heOrigin == curOrigin && heTarget == curTarget)
                    {
                        throw new DublicatedHalfEdgeException("HalfEdge with origin vertex " + heOrigin.Id +
                                                              " and target vertex " +
                                                              heTarget.Id + " already exists");
                    }
                }
                _halfEdges.Add(current);
            }
        }


        /// <summary>
        /// Inserts a pair of half edges between two vertices of a face.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <exception cref="Exception"></exception>
        public void InsertEdge(VertHandle p, VertHandle q)
        {
            var vertP = GetVertexByHandle(p);
            var vertQ = GetVertexByHandle(q);

            var heStartingAtP = GetHalfEdgesStartingAtV(vertP).ToList();
            var heStaringAtQ = GetHalfEdgesStartingAtV(vertQ).ToList();

            var face = new Face();
            var pStartHe = new HalfEdge();
            var qStartHe = new HalfEdge();

            foreach (var heP in heStartingAtP)
            {
                var faceHandleP = GetHalfEdgeByHandle(heP).IncidentFace;

                foreach (var heQ in heStaringAtQ)
                {
                    var faceHandleQ = GetHalfEdgeByHandle(heQ).IncidentFace;

                    if (faceHandleP != faceHandleQ) continue;

                    var commonFace = GetFaceByHandle(faceHandleP);

                    if (commonFace.OuterHalfEdge == default(HalfEdgeHandle)) break;

                    face = GetFaceByHandle(faceHandleP);
                    pStartHe = GetHalfEdgeByHandle(heP);
                    qStartHe = GetHalfEdgeByHandle(heQ);

                    break;
                }
            }
            if (pStartHe.Handle == default(HalfEdgeHandle))
                throw new ArgumentException("Vertex " + p + " vertex " + q + " have no common Face!");

            var holes = GetHoles(face);

            var newFromP = new HalfEdge();
            var newFromQ = new HalfEdge();

            newFromP.Origin = p;
            newFromP.Next = qStartHe.Handle;
            newFromP.Prev = pStartHe.Prev;
            newFromP.IncidentFace = face.Handle;
            newFromP.Handle = new HalfEdgeHandle(CreateHalfEdgeHandleId());
            HalfEdgeHandles.Add(newFromP.Handle);

            newFromQ.Origin = q;
            newFromQ.Next = pStartHe.Handle;
            newFromQ.Prev = qStartHe.Prev;
            newFromQ.IncidentFace = face.Handle;
            newFromQ.Handle = new HalfEdgeHandle(CreateHalfEdgeHandleId());
            HalfEdgeHandles.Add(newFromQ.Handle);

            newFromP.Twin = newFromQ.Handle;
            newFromQ.Twin = newFromP.Handle;

            _halfEdges.Add(newFromP);
            _halfEdges.Add(newFromQ);

            //Assign new Next to previous HalfEdges from p and q  //Assign new prev for qStartHe and halfEdge
            var prevHeP = GetHalfEdgeByHandle(pStartHe.Prev);
            var prevHeQ = GetHalfEdgeByHandle(qStartHe.Prev);
            var count = 0;
            for (var i = 0; i < _halfEdges.Count; i++)
            {
                var he = _halfEdges[i];
                if (he.Handle == prevHeP.Handle)
                {
                    he.Next = newFromP.Handle;
                    _halfEdges[i] = he;
                    count++;
                }
                else if (he.Handle == prevHeQ.Handle)
                {
                    he.Next = newFromQ.Handle;
                    _halfEdges[i] = he;
                    count++;
                }
                else if (_halfEdges[i].Handle == pStartHe.Handle)
                {
                    he.Prev = newFromQ.Handle;
                    _halfEdges[i] = he;
                    count++;
                }
                else if (_halfEdges[i].Handle == qStartHe.Handle)
                {
                    he.Prev = newFromP.Handle;
                    _halfEdges[i] = he;
                    count++;
                }
                if (count == 4) break;
            }

            if (holes.Count != 0 && IsHalfEdgeToHole(holes, p, q, face)) return;

            var newFace = new Face
            {
                Handle = new FaceHandle(CreateFaceHandleId()),
                InnerHalfEdges = new List<HalfEdgeHandle>()
            };
            FaceHandles.Add(newFace.Handle);

            newFace.OuterHalfEdge = newFromQ.Handle;
            _faces.Add(newFace);

            //Assign the handle of the new face to its half edges
            AssignFaceHandle(newFace.OuterHalfEdge, ref newFace);

            //Set face.OuterHalfEdge to newFromP - old OuterHalfEdge can be part of new face now!
            for (var i = 0; i < _faces.Count; i++)
            {
                if (_faces[i].Handle != face.Handle) continue;

                var firstHe = _faces[i];
                firstHe.OuterHalfEdge = newFromP.Handle;
                _faces[i] = firstHe;
            }

        }

        /// <summary>
        /// Gets a vertex by its handle
        /// </summary>
        /// <param name="vertexHandle">The vertex' reference</param>
        /// <returns></returns>
        public Vertex GetVertexByHandle(VertHandle vertexHandle)
        {
            foreach (var e in _vertices)
            {
                if (e.Handle == vertexHandle)
                    return e;
            }
            throw new HandleNotFoundException("HalfEdge with id " + vertexHandle.Id + " not found!");
        }

        /// <summary>
        /// This collection contains all Vertices of a certain face.
        /// </summary>
        /// <param name="face">The faces reference</param>
        /// <returns></returns>
        public IEnumerable<Vertex> GetFaceVertices(FaceHandle face)
        {
            //Outer Outline
            var fistHalfEdgeHandle = GetFaceByHandle(face).OuterHalfEdge;
            var halfEdgeOuter = GetHalfEdgeByHandle(fistHalfEdgeHandle);

            do
            {
                var originVert = halfEdgeOuter.Origin;
                yield return GetVertexByHandle(originVert);
                halfEdgeOuter = GetHalfEdgeByHandle(halfEdgeOuter.Next);

            } while (halfEdgeOuter.Handle != fistHalfEdgeHandle);

            //Inner Outlines
            var innerComponents = GetFaceByHandle(face).InnerHalfEdges;

            if (innerComponents.Count == 0) yield break;

            foreach (var comp in innerComponents)
            {
                var halfEdgeInner = GetHalfEdgeByHandle(comp);

                do
                {
                    var originVert = halfEdgeInner.Origin;
                    yield return GetVertexByHandle(originVert);
                    halfEdgeInner = GetHalfEdgeByHandle(halfEdgeInner.Next);

                } while (halfEdgeInner.Handle != comp);

            }
        }
        #endregion

        #region internal methods for creating, removing and updating HalfEdge, Vertex and Face and their Handles

        internal void UpdateAllVertexCoordZ(int zOffset)
        {
            for (var i = 0; i < _vertices.Count; i++)
            {
                var v = _vertices[i];
                v.Coord = new float3(v.Coord.x, v.Coord.y, v.Coord.z + zOffset);
                _vertices[i] = v;
            }
        }

        internal void UpdateVertex(Vertex vert)
        {
            var handle = vert.Handle;
            for (var i = 0; i < _vertices.Count; i++)
            {
                var v = _vertices[i];
                if (v.Handle != handle) continue;
                _vertices[i] = vert;
                break;
            }
        }

        internal void UpdateFace(Face face)
        {
            var handle = face.Handle;
            for (var i = 0; i < _faces.Count; i++)
            {
                var f = _faces[i];
                if (f.Handle != handle) continue;
                _faces[i] = face;
                break;
            }
        }

        internal void UpdateHalfEdge(HalfEdge halfEdge)
        {
            var handle = halfEdge.Handle;
            for (var i = 0; i < _halfEdges.Count; i++)
            {
                var f = _halfEdges[i];
                if (f.Handle != handle) continue;
                _halfEdges[i] = halfEdge;
                break;
            }
        }

        internal void AddHalfEdge(HalfEdge halfEdge)
        {
            _halfEdges.Add(halfEdge);
        }

        internal void AddFace(Face face)
        {
            _faces.Add(face);
        }

        internal void AddVertex(Vertex vert)
        {
            _vertices.Add(vert);
        }

        internal void RemoveFace(Face face)
        {
            _faces.Remove(face);
        }

        internal int CreateVertHandleId()
        {
            var newId = _highestVertHandle + 1;
            _highestVertHandle = newId;
            return newId;
        }

        internal int CreateHalfEdgeHandleId()
        {
            var newId = _highestHalfEdgeHandle + 1;
            _highestHalfEdgeHandle = newId;
            return newId;
        }

        internal int CreateFaceHandleId()
        {
            var newId = _highestFaceHandle + 1;
            _highestFaceHandle = newId;
            return newId;
        }

        #endregion

        #region internal methods 

        internal IEnumerable<HalfEdgeHandle> GetHalfEdgesOfFace(FaceHandle faceHandle)
        {
            var face = GetFaceByHandle(faceHandle);
            var firstHandle = face.OuterHalfEdge;
            var current = GetHalfEdgeByHandle(face.OuterHalfEdge);
            do
            {
                yield return current.Handle;
                current = GetHalfEdgeByHandle(current.Next);
            } while (firstHandle != current.Handle);

            foreach (var he in face.InnerHalfEdges)
            {
                var cur = GetHalfEdgeByHandle(he);
                do
                {
                    yield return cur.Handle;
                    cur = GetHalfEdgeByHandle(cur.Next);

                } while (he != cur.Handle);
            }
        }

        internal IEnumerable<HalfEdgeHandle> GetHalfEdgesStartingAtV(Vertex v)
        {
            var e = GetHalfEdgeByHandle(v.IncidentHalfEdge);
            var startEdge = e;

            yield return startEdge.Handle;

            while (TwinNext(e).Handle != startEdge.Handle)
            {
                e = TwinNext(e);
                yield return e.Handle;
            }
        }

        //Used in GetHalfEdgesStartingAtV()
        private HalfEdge TwinNext(HalfEdge halfEdge)
        {
            if (halfEdge.Twin == default(HalfEdgeHandle))
                return default(HalfEdge);

            var twin = GetHalfEdgeByHandle(halfEdge.Twin);
            return GetHalfEdgeByHandle(twin.Next);
        }

        internal IEnumerable<HalfEdge> GetEdgeLoop(HalfEdgeHandle handle)
        {
            var currentHandle = handle;

            do
            {
                var currentHalfEdge = GetHalfEdgeByHandle(currentHandle);
                currentHandle = currentHalfEdge.Next;
                yield return currentHalfEdge;

            } while (currentHandle != handle);
        }

        internal HalfEdge GetHalfEdgeByHandle(HalfEdgeHandle halfEdgeHandle)
        {
            foreach (var e in _halfEdges)
            {
                if (e.Handle.Id == halfEdgeHandle.Id)
                    return e;
            }
            throw new HandleNotFoundException("HalfEdge with id " + halfEdgeHandle.Id + " not found!");
        }

        internal Face GetFaceByHandle(FaceHandle faceHandle)
        {
            foreach (var e in _faces)
            {
                if (e.Handle.Id == faceHandle.Id)
                    return e;
            }
            throw new HandleNotFoundException("HalfEdge with id " + faceHandle.Id + " not found!");
        }

        internal void Join2DGeometries(Geometry second)
        {
            var vertStartHandle = _vertices.Count;
            for (var i = 0; i < second.VertHandles.Count; i++)
            {
                var vHandle = second.VertHandles[i];
                vHandle.Id = vHandle.Id + vertStartHandle;

                for (var j = 0; j < second._vertices.Count; j++)
                {
                    var vert = second._vertices[j];
                    if (vert.Handle != second.VertHandles[i]) continue;

                    vert.Handle.Id = vHandle.Id;
                    vert.IncidentHalfEdge.Id = vert.IncidentHalfEdge.Id + (_halfEdges.Count);
                    second._vertices[j] = vert;
                    break;
                }
                second.VertHandles[i] = vHandle;
            }

            var faceStartHandle = _faces.Count;
            for (var i = 0; i < second.FaceHandles.Count; i++)
            {
                var fHandle = second.FaceHandles[i];
                fHandle.Id = fHandle.Id + faceStartHandle;

                for (var j = 0; j < second._faces.Count; j++)
                {
                    var face = second._faces[j];
                    if (face.Handle != second.FaceHandles[i]) continue;

                    face.Handle.Id = fHandle.Id;
                    face.OuterHalfEdge.Id = face.OuterHalfEdge.Id + (_halfEdges.Count);
                    second._faces[j] = face;
                    break;
                }
                second.FaceHandles[i] = fHandle;
            }

            var heStartHandle = _halfEdges.Count;

            for (var i = 0; i < second.HalfEdgeHandles.Count; i++)
            {
                var heHandle = second.HalfEdgeHandles[i];
                heHandle.Id = heHandle.Id + heStartHandle;

                for (var j = 0; j < second._halfEdges.Count; j++)
                {
                    var he = second._halfEdges[j];
                    if (he.Handle != second.HalfEdgeHandles[i]) continue;


                    he.IncidentFace.Id = he.IncidentFace.Id + (_faces.Count);
                    he.Origin.Id = he.Origin.Id + (_vertices.Count);

                    he.Handle.Id = heHandle.Id;

                    he.Next.Id = he.Next.Id + (heStartHandle);
                    he.Prev.Id = he.Prev.Id + (heStartHandle);

                    if (he.Twin != default(HalfEdgeHandle))
                        he.Twin.Id = he.Twin.Id + heStartHandle;

                    second._halfEdges[j] = he;
                    break;
                }
                second.HalfEdgeHandles[i] = heHandle;
            }

            //Change winding
            var zwerg = new List<HalfEdge>();
            foreach (var hEdge in second._halfEdges)
            {
                var he = hEdge;
                var next = he.Prev;
                var prev = he.Next;

                he.Next = next;
                he.Prev = prev;

                var newOrigin = second.GetHalfEdgeByHandle(he.Prev).Origin;
                he.Origin = newOrigin;

                zwerg.Add(he);

                for (var i = 0; i < second._vertices.Count; i++)
                {
                    var vert = second._vertices[i];

                    if (vert.Handle != newOrigin) continue;

                    vert.IncidentHalfEdge = he.Handle;
                    second._vertices[i] = vert;
                    break;
                }
            }

            for (var i = 0; i < second._halfEdges.Count; i++)
            {
                second._halfEdges[i] = zwerg[i];
            }

            //Add data of second geometry to this one
            for (var i = 0; i < second.VertHandles.Count; i++)
            {
                VertHandles.Add(second.VertHandles[i]);
                _vertices.Add(second._vertices[i]);
            }

            //Write content of second undbounded face into the first - delete second unbounded face
            _faces[0].InnerHalfEdges.AddRange(second._faces[0].InnerHalfEdges);
            second._faces.RemoveAt(0);
            second.FaceHandles.RemoveAt(0);
            for (var i = 0; i < second.FaceHandles.Count; i++)
            {
                FaceHandles.Add(second.FaceHandles[i]);
                _faces.Add(second._faces[i]);
            }

            for (var i = 0; i < second.HalfEdgeHandles.Count; i++)
            {
                HalfEdgeHandles.Add(second.HalfEdgeHandles[i]);
                _halfEdges.Add(second._halfEdges[i]);
            }

            SetHighestHandles();
        }

        #endregion

        //TODO: initialisation for 3D geometry
        #region private Methods for 2D geometry initialisation
        
        private void CreateHalfEdgesForGeometry(IEnumerable<Outline> outlines)
        {

            var faceHandle = new FaceHandle { Id = FaceHandles.Count + 1 };
            var unboundedFace = new Face
            {
                Handle = faceHandle,
                OuterHalfEdge = new HalfEdgeHandle(),
                InnerHalfEdges = new List<HalfEdgeHandle>()
            };
            FaceHandles.Add(unboundedFace.Handle);
            _faces.Add(unboundedFace);

            foreach (var o in outlines)
            {
                var outlineHalfEdges = CreateHalfEdgesForBoundary(o);
                
                _halfEdges.AddRange(outlineHalfEdges);
            }

            //Initialise highestHandles with Id of last item in handle lists
            SetHighestHandles();
        }

        private IEnumerable<HalfEdge> CreateHalfEdgesForBoundary(Outline outline)
        {
            var outlineHalfEdges = new List<HalfEdge>();
            var faceHandle = new FaceHandle();

            for (var i = 0; i < outline.Points.Count; i++)
            {
                var coord = outline.Points[i];

                Vertex vert;
                var vertHandle = CreateAndAssignVertex(coord, out vert);

                var halfEdgeHandle = new HalfEdgeHandle(HalfEdgeHandles.Count + 1);

                if (vert.Handle != default(VertHandle))
                {
                    vert.IncidentHalfEdge = halfEdgeHandle;
                    _vertices.Add(vert);
                }

                HalfEdgeHandles.Add(halfEdgeHandle);
                var halfEdge = new HalfEdge
                {
                    Origin = vertHandle,
                    Handle = halfEdgeHandle,
                    Twin = new HalfEdgeHandle()
                };

                //Assumption: outlines are processed from outer to inner for every face, therfore faceHandle will never has its default value if "else" is hit.

                if (outline.IsOuter)
                {
                    if (faceHandle == default(FaceHandle))
                    {
                        Face face;
                        faceHandle = AddFace(halfEdge.Handle, out face);
                        FaceHandles.Add(faceHandle);
                        _faces.Add(face);
                    }
                }
                else
                {
                    if (i == 0)
                        _faces.LastItem().InnerHalfEdges.Add(halfEdge.Handle);

                    faceHandle = _faces.LastItem().Handle;
                }

                halfEdge.IncidentFace = faceHandle;
                outlineHalfEdges.Add(halfEdge);
            }

            for (var i = 0; i < outlineHalfEdges.Count; i++)
            {
                var he = outlineHalfEdges[i];

                //Assumption: a boundary is always closed!
                if (i + 1 < outlineHalfEdges.Count)
                    he.Next.Id = outlineHalfEdges[i + 1].Handle.Id;
                else { he.Next.Id = outlineHalfEdges[0].Handle.Id; }

                if (i - 1 < 0)
                    he.Prev.Id = outlineHalfEdges.LastItem().Handle.Id;
                else { he.Prev.Id = outlineHalfEdges[i - 1].Handle.Id; }

                outlineHalfEdges[i] = he;
            }

            outlineHalfEdges.AddRange(CreateOutlineTwins(outlineHalfEdges));

            return outlineHalfEdges;
        }

        private IEnumerable<HalfEdge> CreateOutlineTwins(IList<HalfEdge> outlineHalfEdges)
        {
            var twinHalfEdges = new List<HalfEdge>();

            for (var i = 0; i < outlineHalfEdges.Count; i++)
            {
                var oldhalfEdge = outlineHalfEdges[i];
                var halfEdgeHandle = new HalfEdgeHandle(HalfEdgeHandles.Count + 1);
                HalfEdgeHandles.Add(halfEdgeHandle);

                //origin vertex for the twin is the origin vertex of the next half edge
                var originVert = outlineHalfEdges[(i + 1) % outlineHalfEdges.Count].Origin;

                var newHalfEdge = new HalfEdge
                {
                    Origin = originVert,
                    Handle = halfEdgeHandle,
                    Twin = oldhalfEdge.Handle,
                };
                var unboundFace = _faces.First();
                newHalfEdge.IncidentFace = unboundFace.Handle;

                twinHalfEdges.Add(newHalfEdge);

                //assign twin to "old" half edge
                oldhalfEdge.Twin = newHalfEdge.Handle;
                outlineHalfEdges[i] = oldhalfEdge;

                //Add fist half edge in loop to unboundFace InnerComponents
                if (i == 0)
                    unboundFace.InnerHalfEdges.Add(newHalfEdge.Handle);
            }

            //assign next and prev half edges
            for (var i = 0; i < twinHalfEdges.Count; i++)
            {
                var he = twinHalfEdges[i];

                //Assumption: a boundary is always closed!
                if (i + 1 < twinHalfEdges.Count)
                    he.Prev.Id = twinHalfEdges[i + 1].Handle.Id;
                else { he.Prev.Id = twinHalfEdges[0].Handle.Id; }

                if (i - 1 < 0)
                    he.Next.Id = twinHalfEdges.LastItem().Handle.Id;
                else { he.Next.Id = twinHalfEdges[i - 1].Handle.Id; }

                twinHalfEdges[i] = he;
            }

            return twinHalfEdges;
        }

        private FaceHandle AddFace(HalfEdgeHandle firstHalfEdge, out Face face)
        {
            var faceHandle = new FaceHandle { Id = FaceHandles.Count + 1 };

            face = new Face
            {
                Handle = faceHandle,
                OuterHalfEdge = firstHalfEdge,
                InnerHalfEdges = new List<HalfEdgeHandle>()
            };
            return faceHandle;
        }

        private VertHandle CreateAndAssignVertex(float3 pointCoord, out Vertex vert)
        {
            var vertHandle = new VertHandle();
            vert = new Vertex();

            //Check if a Vertex already exists and assign it to the HalfEdge instead of createing a new
            if (_vertices.Count != 0)
            {
                foreach (var v in _vertices)
                {
                    if (pointCoord.Equals(v.Coord))
                        vertHandle.Id = v.Handle.Id;
                    else
                    {
                        //Create Vertice and VertHandle
                        vertHandle.Id = VertHandles.Count + 1;
                        VertHandles.Add(vertHandle);
                        vert = new Vertex(pointCoord);
                        vert.Handle = vertHandle;
                        break;
                    }
                }
            }
            else
            {
                //Create Vertices and VertHandle
                vertHandle.Id = VertHandles.Count + 1;
                VertHandles.Add(vertHandle);
                vert = new Vertex(pointCoord);
                vert.Handle = vertHandle;
            }
            return vertHandle;
        }

        private void SetHighestHandles()
        {
            _highestVertHandle = VertHandles.LastItem().Id;
            _highestHalfEdgeHandle = HalfEdgeHandles.LastItem().Id;
            _highestFaceHandle = FaceHandles.LastItem().Id;
        }

        #endregion

        #region private methods concerning InsertEdge

        private Dictionary<HalfEdgeHandle, List<HalfEdge>> GetHoles(Face face)
        {
            var holes = new Dictionary<HalfEdgeHandle, List<HalfEdge>>();

            foreach (var he in face.InnerHalfEdges)
            {
                holes.Add(he, GetEdgeLoop(he).ToList());
            }

            return holes;
        }

        //Vertices need to be reduced to 2D
        //see Akenine-Möller, Tomas; Haines, Eric; Hoffman, Naty (2016): Real-Time Rendering, p. 754
        private bool IsPointInPolygon(FaceHandle face, Vertex v)
        {

            v.Coord = v.Coord.Reduce2D();

            var inside = false;
            var faceVerts = GetFaceVertices(face).ToList();

            var e0 = faceVerts.LastItem();
            e0.Coord = e0.Coord.Reduce2D();

            var y0 = e0.Coord.y >= v.Coord.y;

            foreach (var vert in faceVerts)
            {
                var e1 = vert;
                e1.Coord = e1.Coord.Reduce2D();

                var y1 = e1.Coord.y >= v.Coord.y;
                if (y0 != y1)
                {
                    if ((e1.Coord.y - v.Coord.y) * (e0.Coord.x - e1.Coord.x) >=
                        (e1.Coord.x - v.Coord.x) * (e0.Coord.y - e1.Coord.y) == y1)
                    {
                        inside = !inside;
                    }
                }
                y0 = y1;
                e0 = e1;
            }
            return inside;
        }

        private void AssignFaceHandle(HalfEdgeHandle halfEdge, ref Face newFace)
        {
            var oldFaceHandle = GetHalfEdgeByHandle(halfEdge).IncidentFace;
            var currentHe = GetHalfEdgeByHandle(halfEdge);
            do
            {
                currentHe.IncidentFace = newFace.Handle;

                for (var i = 0; i < _halfEdges.Count; i++)
                {
                    if (_halfEdges[i].Handle != currentHe.Handle) continue;
                    _halfEdges[i] = currentHe;
                    break;
                }
                currentHe = GetHalfEdgeByHandle(currentHe.Next);
            } while (currentHe.Handle != halfEdge);

            //Assign newFace to possible holes in the "old" face

            var oldFace = GetFaceByHandle(oldFaceHandle);
            if (oldFace.InnerHalfEdges.Count == 0) return;

            var inner = new List<HalfEdgeHandle>();
            inner.AddRange(oldFace.InnerHalfEdges);

            foreach (var heh in inner)
            {
                var origin = GetHalfEdgeByHandle(heh).Origin;

                if (!IsPointInPolygon(newFace.Handle, GetVertexByHandle(origin))) continue;

                oldFace.InnerHalfEdges.Remove(heh);
                newFace.InnerHalfEdges.Add(heh);

                var curHe = GetHalfEdgeByHandle(heh);
                do
                {
                    curHe.IncidentFace = newFace.Handle;

                    for (var i = 0; i < _halfEdges.Count; i++)
                    {
                        if (_halfEdges[i].Handle != curHe.Handle) continue;
                        _halfEdges[i] = curHe;
                        break;
                    }
                    curHe = GetHalfEdgeByHandle(curHe.Next);

                } while (curHe.Handle != heh);
            }
        }

        private static bool IsHalfEdgeToHole(Dictionary<HalfEdgeHandle, List<HalfEdge>> holes, VertHandle p, VertHandle q, Face face)
        {
            if (holes.Count == 0) return false;

            foreach (var hole in holes)
            {
                foreach (var he in hole.Value)
                {
                    if (p != he.Origin && q != he.Origin) continue;

                    face.InnerHalfEdges.Remove(hole.Key);
                    return true;
                }
            }
            return false;
        }
        #endregion

    }
}



