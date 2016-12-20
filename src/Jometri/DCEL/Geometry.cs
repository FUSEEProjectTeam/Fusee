using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Core;
using Fusee.Jometri.Triangulation;
using Fusee.Math.Core;

namespace Fusee.Jometri.DCEL
{
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
        internal VertHandle Handle;

        /// <summary>
        /// The geometric data of the vertex
        /// </summary>
        public float3 Coord { get; internal set; }

        /// <summary>
        /// The handle to the half edge with this vertex as origin
        /// </summary>
        internal HalfEdgeHandle IncidentHalfEdge;


        /// <summary>
        /// The vertex' constuctor.
        /// </summary>
        /// <param name="coord">The new vertex' coordinates</param>
        internal Vertex(float3 coord)
        {
            Handle = new VertHandle();
            IncidentHalfEdge = new HalfEdgeHandle();
            Coord = coord;
        }


    }

    /// <summary>
    /// Represents a half edge.
    /// Each half edge contains:
    /// A handle to assign a abstract reference to it.
    /// A handle to the half edges' origin vertex.
    /// A handle to the next half edge.
    /// A handle to the previous half edge.
    /// A handle to the face it belongs to.
    /// </summary>
    public struct HalfEdge
    {
        internal HalfEdgeHandle Handle;

        internal VertHandle Origin;
        internal HalfEdgeHandle Twin;
        internal HalfEdgeHandle Next;
        internal HalfEdgeHandle Prev;
        internal FaceHandle IncidentFace;
    }


    /// <summary>
    /// Needed so the basic functions of Geometry work with both, 2D and 3D faces
    /// </summary>
    public interface IFace
    {
        /// <summary>
        /// Handle to assign a abstract reference
        /// </summary>
        FaceHandle Handle { get; set; }

        /// <summary>
        /// handle to one of the half edges that belongs to the faces outer boundary.
        /// </summary>
        HalfEdgeHandle OuterHalfEdge { get; set; }
    }

    /// <summary>
    /// Each face belonging to a 2D geometry contains:
    /// A handle to assign a abstract reference to it.
    /// A handle to one of the half edges that belongs to the faces outer boundary.
    /// A List that contains handles to one half edge for each hole in a face
    /// Note that unbounded faces can't have a OuterHalfEdge but must have at least one InnerHalfEdge - bounded faces must have a OuterComponent
    /// </summary>
    internal struct Face2D : IFace
    {
        public FaceHandle Handle { get; set; }
        public HalfEdgeHandle OuterHalfEdge { get; set; }

        internal List<HalfEdgeHandle> InnerHalfEdges;
    }

    /// <summary>
    /// Each face belonging to a 3D geometry contains:
    /// A handle to assign a abstract reference to it.
    /// A handle to one of the half edges that belongs to this faces outer boundary.
    /// </summary>
    internal struct Face3D : IFace
    {
        public FaceHandle Handle { get; set; }
        public HalfEdgeHandle OuterHalfEdge { get; set; }
    }

    /// <summary>
    /// Represents an outer or inner boundary of a polygon
    /// </summary>
    public struct PolyBoundary
    {
        /// <summary>
        /// The geometric information of the vertices which belong to a boundary
        /// </summary>
        public IList<float3> Points;

        /// <summary>
        /// Determines whether a boundary is a outer bondary or a inner boundary (which forms a hole in the face).
        /// </summary>
        public bool IsOuter;
    }

    /// <summary>
    /// Base class - Geometry, stored in a DCEL (doubly conneted edge list).
    /// </summary>
    public abstract class Geometry
    {
        #region Members

        /// <summary>
        /// Contains handles to all half edges of the Geometry.
        /// </summary>
        public IList<HalfEdgeHandle> HalfEdgeHandles;

        /// <summary>
        /// Contains handles all faces of the Geometry.
        /// </summary>
        public IList<FaceHandle> FaceHandles;

        /// <summary>
        /// Contains handles to all Vertices of the Geometry.
        /// </summary>
        public IList<VertHandle> VertHandles;

        /// <summary>
        /// Contains all vertices of the Geometry.
        /// </summary>
        protected internal List<Vertex> Vertices { get; set; }

        /// <summary>
        /// Contains all half edges of the Geometry.
        /// </summary>
        protected internal List<HalfEdge> HalfEdges { get; set; }

        /// <summary>
        /// Contains all Faces of the Geometry.
        /// </summary>
        protected internal List<IFace> Faces { get; set; }

        /// <summary>
        /// The highes id of all half edge handles - used to create a new handle.
        /// </summary>
        protected internal int HighestHalfEdgeHandle { get; private set; }

        /// <summary>
        /// The highes id of all vertex handles - used to create a new handle.
        /// </summary>
        protected internal int HighestVertHandle { get; private set; }

        /// <summary>
        /// The highes id of all face handles - used to create a new handle.
        /// </summary>
        protected internal int HighestFaceHandle { get; private set; }

        #endregion

        /// <summary>
        /// Used in the initialisation process of a new Geometry.
        /// A BoundaryEdge contains one edge of the boundary to be inserted into the Geometry
        /// and the information whether the source vertex of the half edge (not the twin half edge) is already part of the Geometry
        /// </summary>
        protected struct BoundaryEdge
        {
            internal bool IsOriginOldVert;
            internal HalfEdge HalfEdge;
            internal HalfEdge TwinHalfEdge;
        }

        /// <summary>
        /// Gets a vertex by its handle
        /// </summary>
        /// <param name="vHandle">The vertex' reference</param>
        /// <returns></returns>
        public Vertex GetVertexByHandle(VertHandle vHandle)
        {
            foreach (var e in Vertices)
            {
                if (e.Handle == vHandle)
                    return e;
            }
            throw new HandleNotFoundException("HalfEdge with id " + vHandle.Id + " not found!");
        }

        internal HalfEdge GetHalfEdgeByHandle(HalfEdgeHandle hehandle)
        {
            foreach (var e in HalfEdges)
            {
                if (e.Handle.Id == hehandle.Id)
                    return e;
            }
            throw new HandleNotFoundException("HalfEdge with id " + hehandle.Id + " not found!");
        }

        internal IFace GetFaceByHandle(FaceHandle fHandle)
        {
            foreach (var e in Faces)
            {
                if (e.Handle.Id == fHandle.Id)
                    return e;
            }
            throw new HandleNotFoundException("HalfEdge with id " + fHandle.Id + " not found!");
        }

        #region circulators 

        /// <summary>
        /// This collection contains all vertices neighbouring a given vertex.
        /// </summary>
        /// <param name="vHandle">The vertex reference</param>
        /// <returns></returns>
        public IEnumerable<VertHandle> GetVertexAdjacentVertices(VertHandle vHandle)
        {
            var vert = GetVertexByHandle(vHandle);
            var e = GetHalfEdgeByHandle(vert.IncidentHalfEdge);
            var startEdge = e;

            var twin = GetHalfEdgeByHandle(e.Twin);
            yield return twin.Origin;

            while (TwinNext(e).Handle != startEdge.Handle)
            {
                e = TwinNext(e);
                twin = GetHalfEdgeByHandle(e.Twin);
                yield return twin.Origin;
            }
        }

        /// <summary>
        /// This collection contains all handles to Faces adjacent to a given vertex.
        /// </summary>
        /// <param name="vHandle">The vertex reference</param>
        /// <returns></returns>
        public IEnumerable<FaceHandle> GetVertexAdajacentFaces(VertHandle vHandle)
        {
            var vert = GetVertexByHandle(vHandle);
            var e = GetHalfEdgeByHandle(vert.IncidentHalfEdge);
            var startEdge = e;

            yield return startEdge.IncidentFace;

            while (TwinNext(e).Handle != startEdge.Handle)
            {
                e = TwinNext(e);
                yield return e.IncidentFace;
            }
        }

        /// <summary>
        /// This collection contains all handles to HalfEdges starting at or targeting a given vertex.
        /// </summary>
        /// <param name="vHandle">The vertex reference</param>
        /// <returns></returns>
        public IEnumerable<HalfEdgeHandle> GetVertexIncidentHalfEdges(VertHandle vHandle)
        {
            var vert = GetVertexByHandle(vHandle);
            var e = GetHalfEdgeByHandle(vert.IncidentHalfEdge);
            var startEdge = e;

            yield return startEdge.Handle;

            while (TwinNext(e).Handle != startEdge.Handle)
            {
                e = TwinNext(e);
                yield return e.Handle;
                yield return e.Twin;
            }
        }

        /// <summary>
        /// This collection contains all handles to HalfEdges starting at a given vertex.
        /// </summary>
        /// <param name="vHandle">The vertex reference</param>
        /// <returns></returns>
        public IEnumerable<HalfEdgeHandle> GetVertexStartingHalfEdges(VertHandle vHandle)
        {
            var vert = GetVertexByHandle(vHandle);
            var e = GetHalfEdgeByHandle(vert.IncidentHalfEdge);
            var startEdge = e;

            yield return startEdge.Handle;

            while (TwinNext(e).Handle != startEdge.Handle)
            {
                e = TwinNext(e);
                yield return e.Handle;
            }
        }

        /// <summary>
        /// This collection contains all handles to HalfEdges ending at a given vertex.
        /// </summary>
        /// <param name="vHandle">The vertex reference</param>
        /// <returns></returns>
        public IEnumerable<HalfEdgeHandle> GetVertexTargetingHalfEdges(VertHandle vHandle)
        {
            var vert = GetVertexByHandle(vHandle);
            var e = GetHalfEdgeByHandle(vert.IncidentHalfEdge);
            var startEdge = e;

            yield return e.Twin;

            while (TwinNext(e).Handle != startEdge.Handle)
            {
                e = TwinNext(e);
                yield return e.Twin;
            }
        }

        private HalfEdge TwinNext(HalfEdge halfEdge)
        {
            if (halfEdge.Twin == default(HalfEdgeHandle))
                return default(HalfEdge);

            var twin = GetHalfEdgeByHandle(halfEdge.Twin);
            return GetHalfEdgeByHandle(twin.Next);
        }

        /// <summary>
        /// This collection contains all handles to HalfEdges belonging to a closed loop.
        /// </summary>
        /// <param name="heHandle">The reference to the HalfEdge with which the loop starts.</param>
        /// <returns></returns>
        public IEnumerable<HalfEdgeHandle> GetHalfEdgeLoop(HalfEdgeHandle heHandle)
        {
            var currentHandle = heHandle;

            do
            {
                var currentHalfEdge = GetHalfEdgeByHandle(currentHandle);
                currentHandle = currentHalfEdge.Next;
                yield return currentHalfEdge.Handle;

            } while (currentHandle != heHandle);
        }

        /// <summary>
        /// This collection contains all faces neighbouring a given face.
        /// </summary>
        /// <param name="fHandle">The Faces reference</param>
        /// <returns></returns>
        public IEnumerable<FaceHandle> GetFacesAdajacentToFace(FaceHandle fHandle)
        {
            var face = GetFaceByHandle(fHandle);
            var firstHandle = face.OuterHalfEdge;
            var current = GetHalfEdgeByHandle(face.OuterHalfEdge);
            do
            {
                yield return current.IncidentFace;
                current = GetHalfEdgeByHandle(current.Next);
            } while (firstHandle != current.Handle);


            if (!(face is Face2D)) yield break;

            var face2D = (Face2D)face;
            var innerComponents = face2D.InnerHalfEdges;
            if (innerComponents.Count == 0) yield break;

            foreach (var he in innerComponents)
            {
                var cur = GetHalfEdgeByHandle(he);
                do
                {
                    yield return cur.IncidentFace;
                    cur = GetHalfEdgeByHandle(cur.Next);

                } while (he != cur.Handle);
            }
        }

        /// <summary>
        /// This collection contains all Vertices of a given Face.
        /// </summary>
        /// <param name="fHandle">The faces reference</param>
        /// <returns></returns>
        public IEnumerable<VertHandle> GetFaceVertices(FaceHandle fHandle)
        {
            //Outer Outline
            var fistHalfEdgeHandle = GetFaceByHandle(fHandle).OuterHalfEdge;
            var halfEdgeOuter = GetHalfEdgeByHandle(fistHalfEdgeHandle);

            do
            {
                var originVert = halfEdgeOuter.Origin;
                yield return originVert;
                halfEdgeOuter = GetHalfEdgeByHandle(halfEdgeOuter.Next);

            } while (halfEdgeOuter.Handle != fistHalfEdgeHandle);

            //Inner Outlines
            var face = GetFaceByHandle(fHandle);

            if (!(face is Face2D)) yield break;
            var face2D = (Face2D)face;
            var innerComponents = face2D.InnerHalfEdges;

            if (innerComponents.Count == 0) yield break;

            foreach (var comp in innerComponents)
            {
                var halfEdgeInner = GetHalfEdgeByHandle(comp);

                do
                {
                    var originVert = halfEdgeInner.Origin;
                    yield return originVert;
                    halfEdgeInner = GetHalfEdgeByHandle(halfEdgeInner.Next);

                } while (halfEdgeInner.Handle != comp);

            }
        }

        /// <summary>
        /// This collection contains all handles to HalfEdges of a given face.
        /// </summary>
        /// <param name="fHandle">The faces reference</param>
        /// <returns></returns>
        public IEnumerable<HalfEdgeHandle> GetFaceHalfEdges(FaceHandle fHandle)
        {
            var face = GetFaceByHandle(fHandle);
            var firstHandle = face.OuterHalfEdge;
            var current = GetHalfEdgeByHandle(face.OuterHalfEdge);
            do
            {
                yield return current.Handle;
                current = GetHalfEdgeByHandle(current.Next);
            } while (firstHandle != current.Handle);


            if (!(face is Face2D)) yield break;

            var face2D = (Face2D)face;

            foreach (var he in face2D.InnerHalfEdges)
            {
                var cur = GetHalfEdgeByHandle(he);
                do
                {
                    yield return cur.Handle;
                    cur = GetHalfEdgeByHandle(cur.Next);

                } while (he != cur.Handle);
            }
        }
        #endregion

        /// <summary>
        /// Tests if a vertex is a direct neighbour of an other vertex. Use this if you do not know the vertices incident half edges. 
        /// </summary>
        /// <param name="p">First vertex</param>
        /// <param name="q">Secound vertex</param>
        /// <returns></returns>
        protected bool IsVertexAdjacentToVertex(VertHandle p, VertHandle q)
        {
            var vertP = GetVertexByHandle(p);
            var vertPStartHe = GetHalfEdgeByHandle(vertP.IncidentHalfEdge);

            var vertQ = GetVertexByHandle(q);
            var vertQStartHe = GetHalfEdgeByHandle(vertQ.IncidentHalfEdge);

            var nextHeP = GetHalfEdgeByHandle(vertPStartHe.Next);
            var nextHeQ = GetHalfEdgeByHandle(vertQStartHe.Next);

            return nextHeP.Origin == q || nextHeQ.Origin == p;
        }

        /// <summary>
        /// Tests if a vertex is a direct neighbour of an other vertex. Use this only if you know the vertices incident half edges. 
        /// </summary>
        /// <param name="p">First vertex</param>
        /// <param name="q">Secound vertex</param>
        /// <param name="vertPStartHe">p incident half edge </param>
        /// <param name="vertQStartHe">q incident half edge</param>
        /// <returns></returns>
        protected bool IsVertexAdjacentToVertex(VertHandle p, VertHandle q, HalfEdge vertPStartHe, HalfEdge vertQStartHe)
        {
            var nextHeP = GetHalfEdgeByHandle(vertPStartHe.Next);
            var nextHeQ = GetHalfEdgeByHandle(vertQStartHe.Next);

            return nextHeP.Origin == q || nextHeQ.Origin == p;
        }

        internal void ReplaceVertex(Vertex vert)
        {
            var handle = vert.Handle;
            for (var i = 0; i < Vertices.Count; i++)
            {
                var v = Vertices[i];
                if (v.Handle != handle) continue;
                Vertices[i] = vert;
                break;
            }
        }

        internal void ReplaceHalfEdge(HalfEdge halfEdge)
        {
            var handle = halfEdge.Handle;
            for (var i = 0; i < HalfEdges.Count; i++)
            {
                var f = HalfEdges[i];
                if (f.Handle != handle) continue;
                HalfEdges[i] = halfEdge;
                break;
            }
        }

        internal void ReplaceFace(IFace face)
        {
            var handle = face.Handle;
            for (var i = 0; i < Faces.Count; i++)
            {
                var f = Faces[i];
                if (f.Handle != handle) continue;
                Faces[i] = face;
                break;
            }
        }

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
            HighestVertHandle = VertHandles.Max().Id;
            HighestHalfEdgeHandle = HalfEdgeHandles.Max().Id;
            HighestFaceHandle = FaceHandles.Max().Id;
        }

        #endregion


    }

    /// <summary>
    /// 2D Geometry, stored in a DCEL (doubly conneted edge list).
    /// </summary>
    public class Geometry2D : Geometry
    {
        /// <summary>
        /// Creates an empty geometry, that can be filled by the user using InsertFace, InsertHalfEdge and InsertVertex methodes
        /// </summary>
        protected Geometry2D()
        {
            Vertices = new List<Vertex>();
            HalfEdges = new List<HalfEdge>();
            Faces = new List<IFace>();

            HalfEdgeHandles = new List<HalfEdgeHandle>();
            FaceHandles = new List<FaceHandle>();
            VertHandles = new List<VertHandle>();
        }

        /// <summary>
        /// 2D Geometry, stored in a DCEL (doubly conneted edge list). Geometry will be triangulated.
        /// </summary>
        /// <param name="outlines">A collection of the geometrys' outlines, each containing the geometric information as a list of float3 in ccw order</param>
        public Geometry2D(IEnumerable<PolyBoundary> outlines)
        {
            Vertices = new List<Vertex>();
            HalfEdges = new List<HalfEdge>();
            Faces = new List<IFace>();

            HalfEdgeHandles = new List<HalfEdgeHandle>();
            FaceHandles = new List<FaceHandle>();
            VertHandles = new List<VertHandle>();

            CreateHalfEdgesForGeometry(outlines);

            this.Triangulate();
        }

        //Clones a existing Geometry object. E.g to create a backface for an extrusion
        internal Geometry2D CloneGeometry()
        {
            var clone = new Geometry2D();

            foreach (var vHandle in VertHandles)
            {
                clone.VertHandles.Add(vHandle);
            }


            foreach (var vert in Vertices)
            {
                clone.Vertices.Add(vert);
            }


            foreach (var heHandle in HalfEdgeHandles)
            {
                clone.HalfEdgeHandles.Add(heHandle);
            }

            foreach (var he in HalfEdges)
            {
                clone.HalfEdges.Add(he);
            }


            foreach (var fHandle in FaceHandles)
            {
                clone.FaceHandles.Add(fHandle);
            }

            foreach (var f in Faces)
            {
                clone.Faces.Add(f);

                for (var i = 0; i < clone.Faces.Count; i++)
                {
                    var face = (Face2D)clone.Faces[i];
                    var innerHe = new List<HalfEdgeHandle>();
                    innerHe.AddRange(face.InnerHalfEdges);
                    face.InnerHalfEdges = innerHe;
                    clone.Faces[i] = face;
                }
            }
            return clone;
        }

        #region 2D geometry initialisation

        private void CreateHalfEdgesForGeometry(IEnumerable<PolyBoundary> outlines)
        {

            var faceHandle = new FaceHandle { Id = FaceHandles.Count + 1 };
            var unboundedFace = new Face2D
            {
                Handle = faceHandle,
                OuterHalfEdge = new HalfEdgeHandle(),
                InnerHalfEdges = new List<HalfEdgeHandle>()
            };

            FaceHandles.Add(unboundedFace.Handle);
            Faces.Add(unboundedFace);

            foreach (var o in outlines)
            {
                foreach (var be in CreateHalfEdgesForBoundary(o))
                {
                    HalfEdgeHandles.Add(be.HalfEdge.Handle);
                    HalfEdgeHandles.Add(be.TwinHalfEdge.Handle);
                    HalfEdges.Add(be.HalfEdge);
                    HalfEdges.Add(be.TwinHalfEdge);
                }
            }

            SetHighestHandles();
        }

        private IEnumerable<BoundaryEdge> CreateHalfEdgesForBoundary(PolyBoundary outline)
        {
            var outlineVerts = OutlineVertices(outline);
            var boundaryEdges = BoundaryEdges(outlineVerts, outline);

            SetPrevAndNextForBoundary(boundaryEdges);

            var halfEdgesToUpdate = new List<HalfEdge>();

            for (var i = boundaryEdges.Count - 1; i > -1; i--)
            {
                var bEdge = boundaryEdges[i];

                if (!bEdge.IsOriginOldVert) continue; //A half-edge can only exist if its source vertex is an old one.

                HalfEdgeHandle existingHeHandle;
                if (!IsEdgeExisting(bEdge.HalfEdge, boundaryEdges, out existingHeHandle)) continue; //Check the target vert to identify the existing half edge

                //If the existing half edge is halfedge.IncidentFace.OuterHalfEdge - replace
                var face = (Face2D)GetFaceByHandle(bEdge.HalfEdge.IncidentFace);
                if (face.OuterHalfEdge == bEdge.HalfEdge.Handle)
                {
                    face.OuterHalfEdge = existingHeHandle;
                    ReplaceFace(face);
                }

                //If the existing half edge is one of the unbounded faces inner half edges - replace
                var unboundedFace = (Face2D)Faces[0];
                for (var k = 0; k < unboundedFace.InnerHalfEdges.Count; k++)
                {
                    var heHandle = unboundedFace.InnerHalfEdges[k];
                    if (heHandle != existingHeHandle) continue;
                    var nextHe = GetHalfEdgeByHandle(heHandle).Next;

                    unboundedFace.InnerHalfEdges[k] = nextHe;
                    Faces[0] = unboundedFace;
                    break;
                }

                var existingHe = GetHalfEdgeByHandle(existingHeHandle);

                existingHe.Next = bEdge.HalfEdge.Next;
                existingHe.Prev = bEdge.HalfEdge.Prev;
                existingHe.IncidentFace = bEdge.HalfEdge.IncidentFace;

                halfEdgesToUpdate.Add(existingHe);

                SetPrevAndNextToExistingHalfEdge(bEdge, existingHeHandle, boundaryEdges, halfEdgesToUpdate);

                boundaryEdges.RemoveAt(i);
            }

            if (halfEdgesToUpdate.Count == 0) return boundaryEdges;

            foreach (var he in halfEdgesToUpdate)
            {
                ReplaceHalfEdge(he);
            }
            return boundaryEdges;
        }

        private FaceHandle AddFace(HalfEdgeHandle firstHalfEdge, out Face2D face)
        {
            var faceHandle = new FaceHandle { Id = FaceHandles.Count + 1 };

            face = new Face2D
            {
                Handle = faceHandle,
                OuterHalfEdge = firstHalfEdge,
                InnerHalfEdges = new List<HalfEdgeHandle>()
            };
            return faceHandle;
        }

        private Vertex CreateOrFindVertex(float3 pointCoord, out bool isOldVertex)
        {
            var vertHandle = new VertHandle();
            Vertex vert;

            //Check if a Vertex already exists and assign it to the HalfEdge instead of createing a new
            if (Vertices.Count != 0)
            {
                foreach (var v in Vertices)
                {
                    if (!pointCoord.Equals(v.Coord)) continue;
                    vertHandle.Id = v.Handle.Id;
                    isOldVertex = true;
                    return v;
                }

                //Create Vertice and VertHandle
                vertHandle.Id = VertHandles.Count + 1;
                VertHandles.Add(vertHandle);
                vert = new Vertex(pointCoord);
                vert.Handle = vertHandle;
            }
            else
            {
                //Create Vertices and VertHandle
                vertHandle.Id = VertHandles.Count + 1;
                VertHandles.Add(vertHandle);
                vert = new Vertex(pointCoord);
                vert.Handle = vertHandle;
            }
            isOldVertex = false;
            return vert;
        }

        private List<KeyValuePair<Vertex, bool>> OutlineVertices(PolyBoundary polyBoundary)
        {
            var outlineVerts = new List<KeyValuePair<Vertex, bool>>();
            foreach (var coord in polyBoundary.Points)
            {
                bool isOldVert;
                var vert = CreateOrFindVertex(coord, out isOldVert);
                outlineVerts.Add(new KeyValuePair<Vertex, bool>(vert, isOldVert));
            }
            return outlineVerts;
        }

        private List<BoundaryEdge> BoundaryEdges(IList<KeyValuePair<Vertex, bool>> outlineVerts, PolyBoundary polyBoundary)
        {
            var faceHandle = new FaceHandle();
            var boundaryEdges = new List<BoundaryEdge>();

            for (var j = 0; j < outlineVerts.Count; j++)
            {
                var currentVert = outlineVerts[j];

                var halfEdgeHandle = new HalfEdgeHandle(CreateHalfEdgeHandleId());

                if (!currentVert.Value)
                {
                    //Only necessary for new Vertices
                    var vert = currentVert.Key;
                    vert.IncidentHalfEdge = halfEdgeHandle;
                    Vertices.Add(vert);
                }

                var halfEdge = new HalfEdge
                {
                    Origin = currentVert.Key.Handle,
                    Handle = halfEdgeHandle,
                    Twin = new HalfEdgeHandle()
                };

                var twinHalfEdge = new HalfEdge
                {
                    Handle = new HalfEdgeHandle(CreateHalfEdgeHandleId()),
                    Twin = halfEdge.Handle,
                    Origin = outlineVerts[(j + 1) % outlineVerts.Count].Key.Handle,
                    IncidentFace = FaceHandles[0]
                };

                halfEdge.Twin = twinHalfEdge.Handle;

                //Assumption: outlines are processed from outer to inner for every face, therfore faceHandle will never has its default value if "else" is hit.
                if (polyBoundary.IsOuter)
                {
                    if (faceHandle == default(FaceHandle))
                    {
                        Face2D face;
                        faceHandle = AddFace(halfEdge.Handle, out face);
                        FaceHandles.Add(faceHandle);
                        Faces.Add(face);
                    }
                }
                else
                {
                    if (j == 0)
                    {
                        var lastFace = (Face2D)Faces.LastItem();
                        lastFace.InnerHalfEdges.Add(halfEdge.Handle);
                    }
                    faceHandle = Faces.LastItem().Handle;
                }

                halfEdge.IncidentFace = faceHandle;

                if (!outlineVerts[j].Value)
                {
                    var unboundFace = (Face2D)Faces[0];

                    if (j == 0)
                    {
                        unboundFace.InnerHalfEdges.Add(twinHalfEdge.Handle);
                        Faces[0] = unboundFace;
                    }
                }

                var boundaryEdge = new BoundaryEdge
                {
                    IsOriginOldVert = currentVert.Value,
                    HalfEdge = halfEdge,
                    TwinHalfEdge = twinHalfEdge
                };
                boundaryEdges.Add(boundaryEdge);
            }
            return boundaryEdges;
        }

        private static void SetPrevAndNextForBoundary(IList<BoundaryEdge> boundaryEdges)
        {
            for (var i = 0; i < boundaryEdges.Count; i++)
            {
                var bEdge = boundaryEdges[i];
                var halfEdge = bEdge.HalfEdge;
                var twinHalfEdge = bEdge.TwinHalfEdge;

                //Assumption: a boundary is always closed!
                halfEdge.Next.Id = boundaryEdges[(i + 1) % boundaryEdges.Count].HalfEdge.Handle.Id;
                twinHalfEdge.Prev.Id = boundaryEdges[(i + 1) % boundaryEdges.Count].TwinHalfEdge.Handle.Id;

                if (i - 1 < 0)
                {
                    halfEdge.Prev.Id = boundaryEdges.LastItem().HalfEdge.Handle.Id;
                    twinHalfEdge.Next.Id = boundaryEdges.LastItem().TwinHalfEdge.Handle.Id;
                }
                else
                {
                    halfEdge.Prev.Id = boundaryEdges[i - 1].HalfEdge.Handle.Id;
                    twinHalfEdge.Next.Id = boundaryEdges[i - 1].TwinHalfEdge.Handle.Id;
                }

                bEdge.HalfEdge = halfEdge;
                bEdge.TwinHalfEdge = twinHalfEdge;

                boundaryEdges[i] = bEdge;
            }
        }

        private void SetPrevAndNextToExistingHalfEdge(BoundaryEdge bEdge, HalfEdgeHandle existingHeHandle, List<BoundaryEdge> boundaryEdges, List<HalfEdge> halfEdgesToUpdate)
        {
            var existingHe = GetHalfEdgeByHandle(existingHeHandle);
            var existingHeNext = GetHalfEdgeByHandle(existingHe.Next);
            var existingHePrev = GetHalfEdgeByHandle(existingHe.Prev);

            existingHe.Next = bEdge.HalfEdge.Next;
            existingHe.Prev = bEdge.HalfEdge.Prev;
            existingHe.IncidentFace = bEdge.HalfEdge.IncidentFace;

            for (var j = 0; j < boundaryEdges.Count; j++)
            {
                var count = 0;
                var be = boundaryEdges[j];
                if (be.TwinHalfEdge.Handle == bEdge.TwinHalfEdge.Prev)
                {
                    var twinHalfEdge = be.TwinHalfEdge;
                    twinHalfEdge.Next = existingHeNext.Handle;

                    var halfEdge = be.HalfEdge;
                    halfEdge.Prev = existingHeHandle;

                    be.TwinHalfEdge = twinHalfEdge;
                    be.HalfEdge = halfEdge;

                    existingHeNext.Prev = twinHalfEdge.Handle;

                    halfEdgesToUpdate.Add(existingHeNext);

                    boundaryEdges[j] = be;
                    count++;
                }

                if (be.TwinHalfEdge.Handle == bEdge.TwinHalfEdge.Next)
                {
                    var twinHalfEdge = be.TwinHalfEdge;
                    twinHalfEdge.Prev = existingHePrev.Handle;

                    var halfEdge = be.HalfEdge;
                    halfEdge.Next = existingHeHandle;

                    be.TwinHalfEdge = twinHalfEdge;
                    be.HalfEdge = halfEdge;

                    existingHePrev.Next = twinHalfEdge.Handle;

                    halfEdgesToUpdate.Add(existingHePrev);

                    boundaryEdges[j] = be;
                    count++;
                }
                if (count == 2)
                    break;
            }
        }

        private bool IsEdgeExisting(HalfEdge halfEdge, IEnumerable<BoundaryEdge> boundaryEdges, out HalfEdgeHandle existingHeHandle)
        {
            existingHeHandle = new HalfEdgeHandle();

            var newHeTargetVert = new VertHandle();

            foreach (var be in boundaryEdges)
            {
                if (be.HalfEdge.Handle == halfEdge.Next)
                    newHeTargetVert = be.HalfEdge.Origin;
            }

            if (newHeTargetVert == default(VertHandle))
                throw new ArgumentException("target vert not found");

            var heStartingAtOldV = GetVertexStartingHalfEdges(halfEdge.Origin).ToList();

            foreach (var heHandle in heStartingAtOldV)
            {
                var he = GetHalfEdgeByHandle(heHandle);
                var oldHeTargetVert = GetHalfEdgeByHandle(he.Next).Origin;

                if (oldHeTargetVert != newHeTargetVert) continue;
                existingHeHandle = heHandle;
                return true;
            }
            return false;
        }

        #endregion

        #region Insert Diagonal
        /// <summary>
        /// Inserts a pair of half edges between two (non adjacant) vertices of a face.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <exception cref="Exception"></exception>
        public void InsertDiagonal(VertHandle p, VertHandle q)
        {
            var heStartingAtP = GetVertexStartingHalfEdges(p).ToList();
            var heStaringAtQ = GetVertexStartingHalfEdges(q).ToList();

            var face = new Face2D();
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

                    face = (Face2D)GetFaceByHandle(faceHandleP);
                    pStartHe = GetHalfEdgeByHandle(heP);
                    qStartHe = GetHalfEdgeByHandle(heQ);

                    break;
                }
            }
            if (pStartHe.Handle == default(HalfEdgeHandle))
                throw new ArgumentException("Vertex " + p + " vertex " + q + " have no common Face!");

            if (IsVertexAdjacentToVertex(p, q, pStartHe, qStartHe))
                throw new ArgumentException("A diagonal can not be insertet beween adjacent Vertices!");

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

            HalfEdges.Add(newFromP);
            HalfEdges.Add(newFromQ);

            //Assign new Next to previous HalfEdges from p and q & assign new prev for qStartHe and pStartHe
            var prevHeP = GetHalfEdgeByHandle(pStartHe.Prev);
            var prevHeQ = GetHalfEdgeByHandle(qStartHe.Prev);
            var count = 0;
            for (var i = 0; i < HalfEdges.Count; i++)
            {
                var he = HalfEdges[i];
                if (he.Handle == prevHeP.Handle)
                {
                    he.Next = newFromP.Handle;
                    HalfEdges[i] = he;
                    count++;
                }
                else if (he.Handle == prevHeQ.Handle)
                {
                    he.Next = newFromQ.Handle;
                    HalfEdges[i] = he;
                    count++;
                }
                else if (HalfEdges[i].Handle == pStartHe.Handle)
                {
                    he.Prev = newFromQ.Handle;
                    HalfEdges[i] = he;
                    count++;
                }
                else if (HalfEdges[i].Handle == qStartHe.Handle)
                {
                    he.Prev = newFromP.Handle;
                    HalfEdges[i] = he;
                    count++;
                }
                if (count == 4) break;
            }

            if (holes.Count != 0 && IsHalfEdgeToHole(holes, p, q, face)) return;

            var newFace = new Face2D
            {
                Handle = new FaceHandle(CreateFaceHandleId()),
                InnerHalfEdges = new List<HalfEdgeHandle>()
            };
            FaceHandles.Add(newFace.Handle);

            newFace.OuterHalfEdge = newFromQ.Handle;
            Faces.Add(newFace);

            //Assign the handle of the new face to its half edges
            AssignFaceHandle(newFace.OuterHalfEdge, newFace);

            //Set face.OuterHalfEdge to newFromP - old OuterHalfEdge can be part of new face now!
            for (var i = 0; i < Faces.Count; i++)
            {
                if (Faces[i].Handle != face.Handle) continue;

                var firstHe = Faces[i];
                firstHe.OuterHalfEdge = newFromP.Handle;
                Faces[i] = firstHe;
            }
        }

        private Dictionary<HalfEdgeHandle, List<HalfEdgeHandle>> GetHoles(Face2D face)
        {
            var holes = new Dictionary<HalfEdgeHandle, List<HalfEdgeHandle>>();

            foreach (var he in face.InnerHalfEdges)
            {
                holes.Add(he, GetHalfEdgeLoop(he).ToList());
            }

            return holes;
        }

        private void AssignFaceHandle(HalfEdgeHandle heHandle, Face2D newFace)
        {
            var oldFaceHandle = GetHalfEdgeByHandle(heHandle).IncidentFace;
            var currentHe = GetHalfEdgeByHandle(heHandle);
            do
            {
                currentHe.IncidentFace = newFace.Handle;

                for (var i = 0; i < HalfEdges.Count; i++)
                {
                    if (HalfEdges[i].Handle != currentHe.Handle) continue;
                    HalfEdges[i] = currentHe;
                    break;
                }
                currentHe = GetHalfEdgeByHandle(currentHe.Next);
            } while (currentHe.Handle != heHandle);

            //Assign newFace to possible holes in the "old" face

            var oldFace = (Face2D)GetFaceByHandle(oldFaceHandle);
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

                    for (var i = 0; i < HalfEdges.Count; i++)
                    {
                        if (HalfEdges[i].Handle != curHe.Handle) continue;
                        HalfEdges[i] = curHe;
                        break;
                    }
                    curHe = GetHalfEdgeByHandle(curHe.Next);

                } while (curHe.Handle != heh);
            }
        }

        private bool IsHalfEdgeToHole(Dictionary<HalfEdgeHandle, List<HalfEdgeHandle>> holes, VertHandle p, VertHandle q, Face2D face)
        {
            if (holes.Count == 0) return false;

            foreach (var hole in holes)
            {
                foreach (var heHandle in hole.Value)
                {
                    var he = GetHalfEdgeByHandle(heHandle);
                    if (p != he.Origin && q != he.Origin) continue;

                    face.InnerHalfEdges.Remove(hole.Key);
                    return true;
                }
            }
            return false;
        }
        #endregion

        //Vertices need to be reduced to 2D
        //see Akenine-Möller, Tomas; Haines, Eric; Hoffman, Naty (2016): Real-Time Rendering, p. 754
        /// <summary>
        /// Tests if a point/vertex lies inside or outside a face - only works in 2D!
        /// </summary>
        /// <param name="fHandle">The handle to the face</param>
        /// <param name="v">The vertex</param>
        /// <returns></returns>
        protected bool IsPointInPolygon(FaceHandle fHandle, Vertex v)
        {
            v.Coord = v.Coord.Reduce2D();

            var inside = false;
            var faceVerts = GetFaceVertices(fHandle).ToList();

            var e0 = GetVertexByHandle(faceVerts.LastItem());
            e0.Coord = e0.Coord.Reduce2D();

            var y0 = e0.Coord.y >= v.Coord.y;

            foreach (var vert in faceVerts)
            {
                var e1 = GetVertexByHandle(vert);
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
    }

    /// <summary>
    /// 3D Geometry, stored in a DCEL (doubly conneted edge list).
    /// </summary>
    public class Geometry3D : Geometry
    {


    }

}



