using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Jometri
{
    /// <summary>
    /// Stores geometry in a DCEL (doubly connected (half) edge list).
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
        /// Contains all HalfEdges of the Geometry.
        /// </summary>
        internal Dictionary<int, HalfEdge> DictHalfEdges { get; set; }

        /// <summary>
        /// Contains all Faces of the Geometry.
        /// </summary>
        internal Dictionary<int, Face> DictFaces { get; set; }

        /// <summary>
        /// The highest handle of all HalfEdge handles - used to create a new handle.
        /// </summary>
        internal int HighestHalfEdgeHandle { get; set; }

        /// <summary>
        /// The highest handle of all Vertex handles - used to create a new handle.
        /// </summary>
        internal int HighestVertHandle { get; set; }

        /// <summary>
        /// The highest handle of all face handles - used to create a new handle.
        /// </summary>
        internal int HighestFaceHandle { get; set; }

        #endregion

        #region Constructors
        /// <summary>
        /// Creates an empty geometry.
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
                SetFaceNormal(GetFaceOuterVertices(key).ToList(), DictFaces[key]);
            }
        }
        #endregion

        /// <summary>
        /// Creates an exact copy of the given Geometry.
        /// </summary>
        /// <returns></returns>
        public Geometry CloneGeometry()
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
                face.FaceData.FaceNormal = oldFace.FaceData.FaceNormal;
                clone.DictFaces.Add(face.Handle, face);
            }

            clone.SetHighestHandles();

            return clone;
        }

        internal float3 Get2DVertPos(Face face, int vertHandle)
        {
            float3 pos;
            if (_vertPos2DCache.TryGetValue(face, out var verts))
            {
                if (verts.TryGetValue(vertHandle, out pos))
                    return pos;

                //it is not in the cache...
                var vertex = GetVertexByHandle(vertHandle);
                pos = vertex.VertData.Pos.Reduce2D(face.FaceData.FaceNormal);

                verts[vertHandle] = pos;
                return pos;
            }

            //it is not in the cache...
            var vert = GetVertexByHandle(vertHandle);

            pos = vert.VertData.Pos.Reduce2D(face.FaceData.FaceNormal);

            _vertPos2DCache[face] = new Dictionary<int, float3> { { vert.Handle, pos } };
            return pos;
        }

        /// <summary>
        /// Calculates and saves the normal of the Face into its FaceData.
        /// </summary>
        /// <param name="faceOuterVertices">All vertices of the outer boundary of the Face.</param>
        /// <param name="face">The Face the normal belongs to.</param>
        public void SetFaceNormal(IList<Vertex> faceOuterVertices, Face face)
        {
            var normal = GeometricOperations.CalculateFaceNormal(faceOuterVertices);

            var cur = DictFaces[face.Handle];
            var faceData = face.FaceData;
            faceData.FaceNormal = normal;
            cur.FaceData = faceData;
            DictFaces[face.Handle] = cur;

        }

        #region Get component by handle

        /// <summary>
        /// Gets a Vertex by its handle.
        /// </summary>
        /// <param name="vHandle">The reference of the Vertex.</param>
        /// <returns></returns>
        internal Vertex GetVertexByHandle(int vHandle)
        {

            if (DictVertices.ContainsKey(vHandle))
            {
                return DictVertices[vHandle];
            }
            throw new ArgumentException("Vertex with id " + vHandle + " not found!");
        }

        internal HalfEdge GetHalfEdgeByHandle(int hehandle)
        {
            if (DictHalfEdges.ContainsKey(hehandle))
            {
                return DictHalfEdges[hehandle];
            }
            throw new ArgumentException("HalfEdge with id " + hehandle + " not found!");
        }

        internal Face GetFaceByHandle(int fHandle)
        {
            if (DictFaces.ContainsKey(fHandle))
            {
                return DictFaces[fHandle];
            }
            throw new ArgumentException("Face with id " + fHandle + " not found!");
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
        /// This collection contains all vertices neighboring a given Vertex.
        /// </summary>
        /// <param name="vHandle">The reference of the Vertex.</param>
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
        /// This collection contains all handles to Faces adjacent to a given Vertex.
        /// </summary>
        /// <param name="vHandle">The reference of the Vertex.</param>
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
        /// This collection contains all handles to HalfEdges starting at or targeting a given Vertex.
        /// </summary>
        /// <param name="vHandle">The reference of the Vertex.</param>
        /// <returns></returns>
        public IEnumerable<HalfEdge> GetVertexIncidentHalfEdges(int vHandle)
        {
            var vert = GetVertexByHandle(vHandle);
            var e = GetHalfEdgeByHandle(vert.IncidentHalfEdge);
            var startEdge = e;

            yield return GetHalfEdgeByHandle(startEdge.Handle);
            yield return GetHalfEdgeByHandle(startEdge.TwinHalfEdge);

            while (TwinNext(e).Handle != startEdge.Handle)
            {
                e = TwinNext(e);
                yield return GetHalfEdgeByHandle(e.Handle);
                yield return GetHalfEdgeByHandle(e.TwinHalfEdge);
            }
        }

        /// <summary>
        /// This collection contains all handles to HalfEdges starting at a given Vertex.
        /// </summary>
        /// <param name="vHandle">The reference of the Vertex.</param>
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
        /// This collection contains all handles to HalfEdges ending at a given Vertex.
        /// </summary>
        /// <param name="vHandle">The reference of the Vertex.</param>
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
        /// Collection is made by tracking the initial HalfEdge's successors.
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
        /// Calculation is made by tracking the initial HalfEdge's predecessors.
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
        /// This collection contains all Faces neighboring a given Face.
        /// </summary>
        /// <param name="fHandle">The reference of the Face.</param>
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
        /// This collection contains all Vertices of a given Face.
        /// </summary>
        /// <param name="fHandle">The reference of the Face.</param>
        /// <returns></returns>
        public IEnumerable<Vertex> GetFaceVertices(int fHandle)
        {
            //Outer boundaries
            var face = GetFaceByHandle(fHandle);
            if (face.OuterHalfEdge != 0)
            {
                var fistHalfEdgeHandle = face.OuterHalfEdge;
                var halfEdgeOuter = GetHalfEdgeByHandle(fistHalfEdgeHandle);

                do
                {
                    var originVert = halfEdgeOuter.OriginVertex;
                    yield return GetVertexByHandle(originVert);
                    halfEdgeOuter = GetHalfEdgeByHandle(halfEdgeOuter.NextHalfEdge);

                } while (halfEdgeOuter.Handle != fistHalfEdgeHandle);
            }

            //Inner boundaries

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
        /// This collection contains all Vertices of the outer boundary of a given Face.
        /// </summary>
        /// <param name="fHandle">The reference of the Face.</param>
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
        /// This collection contains all handles to HalfEdges of a given Face.
        /// </summary>
        /// <param name="fHandle">The reference of the Face.</param>
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
            if (HighestFaceHandle == 0) HighestFaceHandle = 1; //Face 1 is always the unbounded face
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

        internal Face GetFaceToInsertDiag(int p, int q, ref HalfEdge pStartHe, ref HalfEdge qStartHe)
        {
            var vertP = GetVertexByHandle(p);
            var vertQ = GetVertexByHandle(q);
            var heStartingAtP = GetVertexStartingHalfEdges(p).ToList();
            var heStartingAtQ = GetVertexStartingHalfEdges(q).ToList();

            var heWithSameFaceQ = new List<HalfEdge>(heStartingAtQ.Where(x => heStartingAtP.Any(z => z.IncidentFace == x.IncidentFace)));
            var heWithSameFaceP = new List<HalfEdge>(heStartingAtP.Where(x => heStartingAtQ.Any(z => z.IncidentFace == x.IncidentFace)));

            if (heWithSameFaceP.Count == 1)
            {
                var face = GetFaceByHandle(heWithSameFaceP[0].IncidentFace);
                pStartHe = heWithSameFaceP[0];
                qStartHe = heWithSameFaceQ[0];
                return face;
            }

            foreach (var he in heWithSameFaceP)
            {
                var face = GetFaceByHandle(he.IncidentFace);

                if (face.Handle == 1) continue;

                var diagMiddPoint = (1 - 0.5f) * vertP.VertData.Pos + 0.5f * vertQ.VertData.Pos;
                var redMidd = diagMiddPoint.Reduce2D(face.FaceData.FaceNormal);

                if (!this.IsPointInPolygon(face, redMidd)) continue;

                foreach (var heP in heWithSameFaceP)
                {
                    if (heP.IncidentFace == face.Handle)
                        pStartHe = heP;
                }

                foreach (var heQ in heWithSameFaceQ)
                {
                    if (heQ.IncidentFace == face.Handle)
                        qStartHe = heQ;
                }
                return face;
            }
            throw new ArgumentException("Vertex " + p + " vertex " + q + " have no common Face!");
        }
        #endregion
    }
}