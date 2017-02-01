using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Math.Core;

namespace Fusee.Jometri.DCEL
{
     /// <summary>
     /// Abstract class for creating geometry, stored in a DCEL (doubly connected (half) edge list).
     /// </summary>
     public class Geometry
    {
        #region Members
        private readonly Dictionary<Face, Dictionary<int, float3>> _vertPos2DCache = new Dictionary<Face, Dictionary<int, float3>>();

        /// <summary>
        /// Contains all vertices of the Geometry.
        /// </summary>
        internal Dictionary<int, Vertex> DictVertices { get; set; }

        /// <summary>
        /// Contains all half edges of the Geometry.
        /// </summary>
        internal Dictionary<int, HalfEdge> DictHalfEdges { get; set; }

        /// <summary>
        /// Contains all Faces of the Geometry.
        /// </summary>
        internal Dictionary<int, Face> DictFaces { get; set; }

        /// <summary>
        /// The highest handle of all half edge handles - used to create a new handle.
        /// </summary>
        internal int HighestHalfEdgeHandle { get; set; }

        /// <summary>
        /// The highest handle of all vertex handles - used to create a new handle.
        /// </summary>
        internal int HighestVertHandle { get; set; }

        /// <summary>
        /// The highest handle of all face handles - used to create a new handle.
        /// </summary>
        protected internal int HighestFaceHandle { get; set; }

        #endregion

        #region Constructors
        /// <summary>
        /// Creates an empty geometry, that can be filled by the user using InsertFace, InsertHalfEdge and InsertVertex methodes
        /// </summary>
        internal Geometry()
        {
            DictVertices = new Dictionary<int, Vertex>();
            DictHalfEdges = new Dictionary<int, HalfEdge>();
            DictFaces = new Dictionary<int, Face>();
        }

        /// <summary>
        /// 2D Geometry, stored in a DCEL (half edge data structure).
        /// </summary>
        /// <param name="outlines">A collection of the geometry's outlines, each containing the geometric information as a list of float3 in ccw order.</param>
        public Geometry(IEnumerable<PolyBoundary> outlines)
        {
            DictVertices = new Dictionary<int, Vertex>();
            DictHalfEdges = new Dictionary<int, HalfEdge>();
            DictFaces = new Dictionary<int, Face>();

            this.CreateHalfEdgesForGeometry(outlines);

            var keys = new List<int>(DictFaces.Keys);
            foreach (var key in keys)
            {
                if (key == 1) continue;
                this.SetFaceNormal(GetFaceOuterVertices(key).ToList(), DictFaces[key]);
            }
        }
        #endregion

        internal Geometry CloneGeometry()
        {
            var clone = new Geometry
            {
                DictVertices = new Dictionary<int, Vertex>(DictVertices),
                DictHalfEdges = new Dictionary<int, HalfEdge>(DictHalfEdges),
                DictFaces = new Dictionary<int, Face>()
            };

            foreach (var f in DictFaces)
            {
                var oldFace = f.Value;
                var face = new Face(oldFace.Handle, oldFace.OuterHalfEdge);
                face.InnerHalfEdges.AddRange(oldFace.InnerHalfEdges);
                clone.DictFaces.Add(face.Handle, face);
            }

            return clone;
        }

        internal float3 Get2DVertPos(Face face, int vertHandle)
        {
            Dictionary<int, float3> verts;
            float3 pos;
            if (_vertPos2DCache.TryGetValue(face, out verts))
            {
                if (verts.TryGetValue(vertHandle, out pos))
                    return pos;

                // it is not in the cache...
                var vertex = GetVertexByHandle(vertHandle);
                pos = vertex.VertData.Pos.Reduce2D(face.FaceData.FaceNormal);

                verts[vertHandle] = pos;
                return pos;
            }

            // it is not in the cache...
            var vert = GetVertexByHandle(vertHandle);

            pos = vert.VertData.Pos.Reduce2D(face.FaceData.FaceNormal);

            _vertPos2DCache[face] = new Dictionary<int, float3> { { vert.Handle, pos } };
            return pos;
        }

        #region Get component by handle

        /// <summary>
        /// Gets a vertex by its handle.
        /// </summary>
        /// <param name="vHandle">The vertex's reference.</param>
        /// <returns></returns>
        public Vertex GetVertexByHandle(int vHandle)
        {

            if (DictVertices.ContainsKey(vHandle))
            {
                return DictVertices[vHandle];
            }
            throw new HandleNotFoundException("Vertex with id " + vHandle + " not found!");
        }

        internal HalfEdge GetHalfEdgeByHandle(int hehandle)
        {
            if (DictHalfEdges.ContainsKey(hehandle))
            {
                return DictHalfEdges[hehandle];
            }
            throw new HandleNotFoundException("HalfEdge with id " + hehandle + " not found!");
        }

        internal Face GetFaceByHandle(int fHandle)
        {
            if (DictFaces.ContainsKey(fHandle))
            {
                return DictFaces[fHandle];
            }
            throw new HandleNotFoundException("Face with id " + fHandle + " not found!");
        }

        #endregion

        #region Enumerators

        /// <summary>
        /// Returns all Vertices of the Geometry.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Vertex> GetAllVertices()
        {
            foreach (var vert in DictVertices)
            {
                yield return vert.Value;
            }
        }

        /// <summary>
        /// Returns all HalfEdges of the Geometry.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<HalfEdge> GetAllHalfEdges()
        {
            foreach (var he in DictHalfEdges)
            {
                yield return he.Value;
            }
        }

        /// <summary>
        /// Returns all Faces of the Geometry.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Face> GetAllFaces()
        {
            foreach (var face in DictFaces)
            {
                yield return face.Value;
            }
        }

        #endregion

        #region circulators 

        /// <summary>
        /// This collection contains all vertices neighbouring a given vertex.
        /// </summary>
        /// <param name="vHandle">The reference of the vertex.</param>
        /// <returns></returns>
        public IEnumerable<Vertex> GetVertexAdjacentVertices(int vHandle)
        {
            var vert = GetVertexByHandle(vHandle);
            var e = GetHalfEdgeByHandle(vert.IncidentHalfEdge);
            var startEdge = e;

            var twin = GetHalfEdgeByHandle(e.TwinHalfEdge);
            yield return GetVertexByHandle(twin.OriginVertex);

            while (TwinNext(e).Handle != startEdge.Handle)
            {
                e = TwinNext(e);
                twin = GetHalfEdgeByHandle(e.TwinHalfEdge);
                yield return GetVertexByHandle(twin.OriginVertex);
            }
        }

        /// <summary>
        /// This collection contains all handles to Faces adjacent to a given vertex.
        /// </summary>
        /// <param name="vHandle">The reference of the vertex.</param>
        /// <returns></returns>
        public IEnumerable<Face> GetVertexAdajacentFaces(int vHandle)
        {
            var vert = GetVertexByHandle(vHandle);
            var e = GetHalfEdgeByHandle(vert.IncidentHalfEdge);
            var startEdge = e;

            yield return GetFaceByHandle(startEdge.IncidentFace);

            while (TwinNext(e).Handle != startEdge.Handle)
            {
                e = TwinNext(e);
                yield return GetFaceByHandle(e.IncidentFace);
            }
        }

        /// <summary>
        /// This collection contains all handles to HalfEdges starting at or targeting a given vertex.
        /// </summary>
        /// <param name="vHandle">The reference of the vertex.</param>
        /// <returns></returns>
        public IEnumerable<HalfEdge> GetVertexIncidentHalfEdges(int vHandle)
        {
            var vert = GetVertexByHandle(vHandle);
            var e = GetHalfEdgeByHandle(vert.IncidentHalfEdge);
            var startEdge = e;

            yield return GetHalfEdgeByHandle(startEdge.Handle);

            while (TwinNext(e).Handle != startEdge.Handle)
            {
                e = TwinNext(e);
                yield return GetHalfEdgeByHandle(e.Handle);
                yield return GetHalfEdgeByHandle(e.TwinHalfEdge);
            }
        }

        /// <summary>
        /// This collection contains all handles to HalfEdges starting at a given vertex.
        /// </summary>
        /// <param name="vHandle">The reference of the vertex.</param>
        /// <returns></returns>
        public IEnumerable<HalfEdge> GetVertexStartingHalfEdges(int vHandle)
        {
            var vert = GetVertexByHandle(vHandle);
            var e = GetHalfEdgeByHandle(vert.IncidentHalfEdge);
            var startEdge = e;

            yield return GetHalfEdgeByHandle(startEdge.Handle);

            while (TwinNext(e).Handle != startEdge.Handle)
            {
                e = TwinNext(e);
                yield return GetHalfEdgeByHandle(e.Handle);
            }
        }

        /// <summary>
        /// This collection contains all handles to HalfEdges ending at a given vertex.
        /// </summary>
        /// <param name="vHandle">The reference of the vertex.</param>
        /// <returns></returns>
        public IEnumerable<HalfEdge> GetVertexTargetingHalfEdges(int vHandle)
        {
            var vert = GetVertexByHandle(vHandle);
            var e = GetHalfEdgeByHandle(vert.IncidentHalfEdge);
            var startEdge = e;

            yield return GetHalfEdgeByHandle(e.TwinHalfEdge);

            while (TwinNext(e).Handle != startEdge.Handle)
            {
                e = TwinNext(e);
                yield return GetHalfEdgeByHandle(e.TwinHalfEdge);
            }
        }

        private HalfEdge TwinNext(HalfEdge halfEdge)
        {
            if (halfEdge.TwinHalfEdge == default(int))
                return default(HalfEdge);

            var twin = GetHalfEdgeByHandle(halfEdge.TwinHalfEdge);
            return GetHalfEdgeByHandle(twin.NextHalfEdge);
        }

        /// <summary>
        /// This collection contains all handles to HalfEdges belonging to a closed loop.
        /// Collection is made by following the initial half edges next half edges.
        /// </summary>
        /// <param name="heHandle">The reference to the HalfEdge with which the loop starts.</param>
        /// <returns></returns>
        public IEnumerable<HalfEdge> GetHalfEdgeLoop(int heHandle)
        {
            var currentHandle = heHandle;

            do
            {
                var currentHalfEdge = GetHalfEdgeByHandle(currentHandle);
                currentHandle = currentHalfEdge.NextHalfEdge;
                yield return GetHalfEdgeByHandle(currentHalfEdge.Handle);

            } while (currentHandle != heHandle);
        }

        /// <summary>
        /// This collection contains all handles to HalfEdges belonging to a closed loop.
        /// Calculation is made by following the initial half edges previous half edges.
        /// </summary>
        /// <param name="heHandle">The reference to the HalfEdge with which the loop starts.</param>
        /// <returns></returns>
        public IEnumerable<HalfEdge> GetHalfEdgeLoopReverse(int heHandle)
        {
            var currentHandle = heHandle;

            do
            {
                var currentHalfEdge = GetHalfEdgeByHandle(currentHandle);
                currentHandle = currentHalfEdge.PrevHalfEdge;
                yield return GetHalfEdgeByHandle(currentHalfEdge.Handle);

            } while (currentHandle != heHandle);
        }

        /// <summary>
        /// This collection contains all faces neighbouring a given face.
        /// </summary>
        /// <param name="fHandle">The reference of the face.</param>
        /// <returns></returns>
        public IEnumerable<Face> GetFacesAdajacentToFace(int fHandle)
        {
            var face = GetFaceByHandle(fHandle);
            var firstHandle = face.OuterHalfEdge;
            var current = GetHalfEdgeByHandle(face.OuterHalfEdge);
            do
            {
                yield return GetFaceByHandle(current.IncidentFace);
                current = GetHalfEdgeByHandle(current.NextHalfEdge);
            } while (firstHandle != current.Handle);

            var innerComponents = face.InnerHalfEdges;
            if (innerComponents.Count == 0) yield break;

            foreach (var he in innerComponents)
            {
                var cur = GetHalfEdgeByHandle(he);
                do
                {
                    yield return GetFaceByHandle(cur.IncidentFace);
                    cur = GetHalfEdgeByHandle(cur.NextHalfEdge);

                } while (he != cur.Handle);
            }
        }

        /// <summary>
        /// This collection contains all Vertices of a given face.
        /// </summary>
        /// <param name="fHandle">The reference of the face.</param>
        /// <returns></returns>
        public IEnumerable<Vertex> GetFaceVertices(int fHandle)
        {
            //Outer boundaries
            var fistHalfEdgeHandle = GetFaceByHandle(fHandle).OuterHalfEdge;
            var halfEdgeOuter = GetHalfEdgeByHandle(fistHalfEdgeHandle);

            do
            {
                var originVert = halfEdgeOuter.OriginVertex;
                yield return GetVertexByHandle(originVert);
                halfEdgeOuter = GetHalfEdgeByHandle(halfEdgeOuter.NextHalfEdge);

            } while (halfEdgeOuter.Handle != fistHalfEdgeHandle);

            //Inner boundaries
            var face = GetFaceByHandle(fHandle);

            var innerComponents = face.InnerHalfEdges;

            if (innerComponents.Count == 0) yield break;

            foreach (var comp in innerComponents)
            {
                var halfEdgeInner = GetHalfEdgeByHandle(comp);

                do
                {
                    var originVert = halfEdgeInner.OriginVertex;
                    yield return GetVertexByHandle(originVert);
                    halfEdgeInner = GetHalfEdgeByHandle(halfEdgeInner.NextHalfEdge);

                } while (halfEdgeInner.Handle != comp);

            }
        }

        /// <summary>
        /// This collection contains all Vertices of the outer boundary of a given face.
        /// </summary>
        /// <param name="fHandle">The reference of the face.</param>
        /// <returns></returns>
        public IEnumerable<Vertex> GetFaceOuterVertices(int fHandle)
        {
            //Outer boundaries
            var fistHalfEdgeHandle = GetFaceByHandle(fHandle).OuterHalfEdge;
            var halfEdgeOuter = GetHalfEdgeByHandle(fistHalfEdgeHandle);

            do
            {
                var originVert = halfEdgeOuter.OriginVertex;
                yield return GetVertexByHandle(originVert);
                halfEdgeOuter = GetHalfEdgeByHandle(halfEdgeOuter.NextHalfEdge);

            } while (halfEdgeOuter.Handle != fistHalfEdgeHandle);

        }

        /// <summary>
        /// This collection contains all handles to HalfEdges of a given face.
        /// </summary>
        /// <param name="fHandle">The reference of the face.</param>
        /// <returns></returns>
        public IEnumerable<HalfEdge> GetFaceHalfEdges(int fHandle)
        {
            var face = GetFaceByHandle(fHandle);
            var firstHandle = face.OuterHalfEdge;
            var current = GetHalfEdgeByHandle(face.OuterHalfEdge);
            do
            {
                yield return GetHalfEdgeByHandle(current.Handle);
                current = GetHalfEdgeByHandle(current.NextHalfEdge);
            } while (firstHandle != current.Handle);


            foreach (var he in face.InnerHalfEdges)
            {
                var cur = GetHalfEdgeByHandle(he);
                do
                {
                    yield return GetHalfEdgeByHandle(cur.Handle);
                    cur = GetHalfEdgeByHandle(cur.NextHalfEdge);

                } while (he != cur.Handle);
            }
        }
        #endregion

        #region Replace Components
        internal void ReplaceVertex(Vertex vert)
        {
            var key = vert.Handle;
            DictVertices[key] = vert;
        }

        internal void ReplaceHalfEdge(HalfEdge halfEdge)
        {
            var key = halfEdge.Handle;
            DictHalfEdges[key] = halfEdge;
        }

        internal void ReplaceFace(Face face)
        {
            var key = face.Handle;
            DictFaces[key] = face;
        }

        #endregion

        #region Insert Diagonal

        /// <summary>
        /// Inserts a pair of half edges between two (non adjacant) vertices of a face.
        /// </summary>
        /// <param name="p">First vertex handle.</param>
        /// <param name="q">Second vertex handle.</param>
        /// <exception cref="Exception"></exception>
        public void InsertDiagonal(int p, int q)
        {
            var heStartingAtP = GetVertexStartingHalfEdges(p).ToList();
            var heStaringAtQ = GetVertexStartingHalfEdges(q).ToList();

            var face = new Face();
            var pStartHe = new HalfEdge();
            var qStartHe = new HalfEdge();

            foreach (var heP in heStartingAtP)
            {
                var faceHandleP = heP.IncidentFace;
                if (GetFaceByHandle(faceHandleP).OuterHalfEdge == default(int)) continue;//If heP is unbounded we can not insert a half edge

                foreach (var heQ in heStaringAtQ)
                {
                    var faceHandleQ = heQ.IncidentFace;

                    if (faceHandleP != faceHandleQ) continue;

                    var commonFace = GetFaceByHandle(faceHandleP);

                    if (commonFace.OuterHalfEdge == default(int)) continue; //If commonFace is unbounded we can not insert a half edge

                    face = GetFaceByHandle(faceHandleP);
                    pStartHe = heP;
                    qStartHe = heQ;

                    break;
                }
            }
            if (pStartHe.Handle == default(int))
                throw new ArgumentException("Vertex " + p + " vertex " + q + " have no common Face!");

            if (this.IsVertexAdjacentToVertex(p, q, pStartHe, qStartHe))
                throw new ArgumentException("A diagonal can not be insertet beween adjacent Vertices!");

            var newFromP = new HalfEdge(CreateHalfEdgeHandleId());
            var newFromQ = new HalfEdge(CreateHalfEdgeHandleId());

            newFromP.OriginVertex = p;
            newFromP.NextHalfEdge = qStartHe.Handle;
            newFromP.PrevHalfEdge = pStartHe.PrevHalfEdge;
            newFromP.IncidentFace = face.Handle;

            newFromQ.OriginVertex = q;
            newFromQ.NextHalfEdge = pStartHe.Handle;
            newFromQ.PrevHalfEdge = qStartHe.PrevHalfEdge;
            newFromQ.IncidentFace = face.Handle;

            newFromP.TwinHalfEdge = newFromQ.Handle;
            newFromQ.TwinHalfEdge = newFromP.Handle;

            DictHalfEdges.Add(newFromP.Handle, newFromP);
            DictHalfEdges.Add(newFromQ.Handle, newFromQ);

            //Assign new Next to previous HalfEdges from p and q & assign new prev for qStartHe and pStartHe
            var prevHeP = GetHalfEdgeByHandle(pStartHe.PrevHalfEdge);
            var prevHeQ = GetHalfEdgeByHandle(qStartHe.PrevHalfEdge);

            var prevHePUpdate = DictHalfEdges[prevHeP.Handle];
            prevHePUpdate.NextHalfEdge = newFromP.Handle;
            DictHalfEdges[prevHeP.Handle] = prevHePUpdate;

            var prevHeQUpdate = DictHalfEdges[prevHeQ.Handle];
            prevHeQUpdate.NextHalfEdge = newFromQ.Handle;
            DictHalfEdges[prevHeQ.Handle] = prevHeQUpdate;

            var nextHePUpdate = DictHalfEdges[pStartHe.Handle];
            nextHePUpdate.PrevHalfEdge = newFromQ.Handle;
            DictHalfEdges[pStartHe.Handle] = nextHePUpdate;

            var nextHeQUpdate = DictHalfEdges[qStartHe.Handle];
            nextHeQUpdate.PrevHalfEdge = newFromP.Handle;
            DictHalfEdges[qStartHe.Handle] = nextHeQUpdate;

            var holes = GetHoles(face);

            if (holes.Count != 0 && IsHalfEdgeToHole(holes, p, q, face)) return;

            var newFace = new Face(CreateFaceHandleId(), newFromQ.Handle);

            //The face normal of the newFace equals the normal of the original face because adding a diagonal does not change the face vertices position.
            var newFaceData = newFace.FaceData;
            newFaceData.FaceNormal = face.FaceData.FaceNormal;
            newFace.FaceData = newFaceData;

            DictFaces.Add(newFace.Handle, newFace);

            //Assign the handle of the new face to its half edges.
            AssignFaceHandle(newFace.OuterHalfEdge, newFace);

            //Set face.OuterHalfEdge to newFromP - old OuterHalfEdge can be part of new face now!
            var faces = GetAllFaces().ToList();
            for (var i = 0; i < faces.Count; i++)
            {
                if (faces[i].Handle != face.Handle) continue;

                var currentFace = faces[i];
                currentFace.OuterHalfEdge = newFromP.Handle;
                faces[i] = currentFace;
                DictFaces[faces[i].Handle] = faces[i];
            }
        }

        private Dictionary<int, List<HalfEdge>> GetHoles(Face face)
        {
            var holes = new Dictionary<int, List<HalfEdge>>();

            foreach (var he in face.InnerHalfEdges)
            {
                holes.Add(he, GetHalfEdgeLoop(he).ToList());
            }

            return holes;
        }

        private void AssignFaceHandle(int heHandle, Face newFace)
        {
            var oldFaceHandle = GetHalfEdgeByHandle(heHandle).IncidentFace;
            var currentHe = GetHalfEdgeByHandle(heHandle);
            do
            {
                currentHe.IncidentFace = newFace.Handle;

                DictHalfEdges[currentHe.Handle] = currentHe;

                currentHe = GetHalfEdgeByHandle(currentHe.NextHalfEdge);
            } while (currentHe.Handle != heHandle);

            //Assign newFace to possible holes in the "old" face
            var oldFace = GetFaceByHandle(oldFaceHandle);
            if (oldFace.InnerHalfEdges.Count == 0) return;

            var inner = new List<int>();
            inner.AddRange(oldFace.InnerHalfEdges);

            foreach (var heh in inner)
            {
                var origin = GetHalfEdgeByHandle(heh).OriginVertex;

                if (!this.IsPointInPolygon(newFace, GetVertexByHandle(origin))) continue;

                oldFace.InnerHalfEdges.Remove(heh);
                newFace.InnerHalfEdges.Add(heh);

                var curHe = GetHalfEdgeByHandle(heh);
                do
                {
                    curHe.IncidentFace = newFace.Handle;

                    DictHalfEdges[curHe.Handle] = curHe;

                    curHe = GetHalfEdgeByHandle(curHe.NextHalfEdge);

                } while (curHe.Handle != heh);
            }
        }

        private static bool IsHalfEdgeToHole(Dictionary<int, List<HalfEdge>> holes, int pHandle, int qHandle,
            Face face)
        {
            if (holes.Count == 0) return false;

            foreach (var hole in holes)
            {
                foreach (var heHandle in hole.Value)
                {
                    var he = heHandle;
                    if (pHandle != he.OriginVertex && qHandle != he.OriginVertex) continue;

                    face.InnerHalfEdges.Remove(hole.Key);
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region ID creation for Vert-, Face- and HalfEdgeHandles

        internal int CreateVertHandleId()
        {
            var newId = HighestVertHandle + 1;
            HighestVertHandle = newId;
            return newId;
        }

        internal int CreateHalfEdgeHandleId()
        {
            var newId = HighestHalfEdgeHandle + 1;
            HighestHalfEdgeHandle = newId;
            return newId;
        }

        internal int CreateFaceHandleId()
        {
            var newId = HighestFaceHandle + 1;
            HighestFaceHandle = newId;
            return newId;
        }

        internal void SetHighestHandles()
        {
            HighestVertHandle = DictVertices.Keys.Max();
            HighestHalfEdgeHandle = DictHalfEdges.Keys.Max();
            HighestFaceHandle = DictFaces.Keys.Max();
        }

        #endregion
    }
}



