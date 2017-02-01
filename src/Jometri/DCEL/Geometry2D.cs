using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Core;
using Fusee.Math.Core;

namespace Fusee.Jometri.DCEL
{

    /// <summary>
    /// Represents an outer or inner boundary of a polygon.
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
    /// 2D geometry, stored in a half edge data structure (doubly connected edge list) and made from polygon boundaries.
    /// </summary>
    public class Geometry2D : Geometry
    {
        /// <summary>
        /// Used in the initialisation process of a new Geometry.
        /// A BoundaryEdge contains one edge of the boundary and the information whether the source vertex of the half edge (not the twin half edge) is already part of the Geometry
        /// </summary>
        internal struct BoundaryEdge
        {
            internal bool IsOriginOldVert;
            internal HalfEdge HalfEdge;
            internal HalfEdge TwinHalfEdge;
        }

        /// <summary>
        /// Creates an empty geometry, that can be filled by the user using InsertFace, InsertHalfEdge and InsertVertex methodes
        /// </summary>
        internal Geometry2D()
        {
            DictVertices = new Dictionary<int, Vertex>();
            DictHalfEdges = new Dictionary<int, HalfEdge>();
            DictFaces = new Dictionary<int, Face>();
        }

        /// <summary>
        /// 2D Geometry, stored in a DCEL (half edge data structure).
        /// </summary>
        /// <param name="outlines">A collection of the geometry's outlines, each containing the geometric information as a list of float3 in ccw order.</param>
        public Geometry2D(IEnumerable<PolyBoundary> outlines)
        {
            DictVertices = new Dictionary<int, Vertex>();
            DictHalfEdges = new Dictionary<int, HalfEdge>();
            DictFaces = new Dictionary<int, Face>();

            CreateHalfEdgesForGeometry(outlines);

            var keys = new List<int>(DictFaces.Keys);
            foreach (var key in keys)
            {
                if (key == 1) continue;
                this.SetFaceNormal(GetFaceOuterVertices(key).ToList(), DictFaces[key]);
            }
        }

        internal Geometry2D CloneGeometry()
        {
            var clone = new Geometry2D
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

        #region 2D geometry initialisation

        private void CreateHalfEdgesForGeometry(IEnumerable<PolyBoundary> outlines)
        {
            var unboundedFace = new Face(DictHalfEdges.Count + 1);

            DictFaces.Add(unboundedFace.Handle, unboundedFace);

            foreach (var o in outlines)
            {
                var boundaryHalfEdges = CreateHalfEdgesForBoundary(o);
                foreach (var be in boundaryHalfEdges)
                {
                    DictHalfEdges.Add(be.HalfEdge.Handle, be.HalfEdge);
                    DictHalfEdges.Add(be.TwinHalfEdge.Handle, be.TwinHalfEdge);
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

                int existingHeHandle;
                if (!IsEdgeExisting(bEdge.HalfEdge, boundaryEdges, out existingHeHandle))
                    continue; //Check the target vert to identify the existing half edge

                //If the existing half edge is halfedge.IncidentFace.OuterHalfEdge - replace
                var face = GetFaceByHandle(bEdge.HalfEdge.IncidentFace);
                if (face.OuterHalfEdge == bEdge.HalfEdge.Handle)
                {
                    face.OuterHalfEdge = existingHeHandle;
                    ReplaceFace(face);
                }

                //If the existing half edge is one of the unbounded faces inner half edges - replace
                var unboundedFace = DictFaces[1];
                for (var k = 0; k < unboundedFace.InnerHalfEdges.Count; k++)
                {
                    var heHandle = unboundedFace.InnerHalfEdges[k];
                    if (heHandle != existingHeHandle) continue;
                    var nextHe = GetHalfEdgeByHandle(heHandle).NextHalfEdge;

                    unboundedFace.InnerHalfEdges[k] = nextHe;
                    DictFaces[1] = unboundedFace;
                    break;
                }

                var existingHe = GetHalfEdgeByHandle(existingHeHandle);

                existingHe.NextHalfEdge = bEdge.HalfEdge.NextHalfEdge;
                existingHe.PrevHalfEdge = bEdge.HalfEdge.PrevHalfEdge;
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

        private int AddFace(int firstHalfEdge, out Face face)
        {
            face = new Face(DictFaces.Count + 1, firstHalfEdge);
            return face.Handle;
        }

        private Vertex CreateOrFindVertex(float3 pointCoord, out bool isOldVertex, int handle)
        {
            int vertHandle;
            Vertex vert;

            //Check if a Vertex already exists and assign it to the HalfEdge instead of createing a new
            if (DictVertices.Count != 0)
            {
                foreach (var v in GetAllVertices())
                {
                    if (!pointCoord.Equals(v.VertData.Pos)) continue;
                    isOldVertex = true;
                    return v;
                }

                //Create Vertice and VertHandle
                vertHandle = handle;
                vert = new Vertex(vertHandle, pointCoord);
            }
            else
            {
                //Create Vertices and VertHandle
                vertHandle = handle;
                vert = new Vertex(vertHandle, pointCoord);
            }
            isOldVertex = false;
            return vert;
        }

        private List<KeyValuePair<Vertex, bool>> OutlineVertices(PolyBoundary polyBoundary)
        {
            var outlineVerts = new List<KeyValuePair<Vertex, bool>>();

            var handle = 0;
            if (DictVertices.Count != 0)
                handle = DictVertices.Count;

            foreach (var coord in polyBoundary.Points)
            {
                bool isOldVert;
                handle++;
                var vert = CreateOrFindVertex(coord, out isOldVert, handle);
                outlineVerts.Add(new KeyValuePair<Vertex, bool>(vert, isOldVert));
            }
            return outlineVerts;
        }

        private List<BoundaryEdge> BoundaryEdges(IList<KeyValuePair<Vertex, bool>> outlineVerts,
            PolyBoundary polyBoundary)
        {
            var faceHandle = new int();
            var boundaryEdges = new List<BoundaryEdge>();

            var halfEdgeHandle = 0;
            if (DictHalfEdges.Count != 0)
                halfEdgeHandle = DictHalfEdges.Count;

            for (var j = 0; j < outlineVerts.Count; j++)
            {
                var currentVert = outlineVerts[j];
                halfEdgeHandle++;

                if (!currentVert.Value)
                {
                    //Only necessary for new Vertices
                    var vert = currentVert.Key;
                    vert.IncidentHalfEdge = halfEdgeHandle;
                    DictVertices.Add(vert.Handle, vert);
                }

                var halfEdge = new HalfEdge(halfEdgeHandle, currentVert.Key.Handle);


                halfEdgeHandle++;
                var twinHalfEdge = new HalfEdge(halfEdgeHandle, outlineVerts[(j + 1) % outlineVerts.Count].Key.Handle,
                    halfEdge.Handle, 0, 0, 1);
                //The unbounded face is always added at first and therefor has 1 as handle.


                halfEdge.TwinHalfEdge = twinHalfEdge.Handle;

                //Assumption: outlines are processed from outer to inner for every face, therfore faceHandle will never has its default value if "else" is hit.
                if (polyBoundary.IsOuter)
                {
                    if (faceHandle == default(int))
                    {
                        Face face;
                        faceHandle = AddFace(halfEdge.Handle, out face);
                        DictFaces.Add(face.Handle, face);
                    }
                }
                else
                {
                    if (j == 0)
                    {
                        var lastFace = DictFaces[DictFaces.Keys.Max()];
                        lastFace.InnerHalfEdges.Add(halfEdge.Handle);
                    }
                    faceHandle = DictFaces.LastItem().Value.Handle;
                }

                halfEdge.IncidentFace = faceHandle;

                if (!outlineVerts[j].Value)
                {
                    var unboundFace = DictFaces[1];

                    if (j == 0)
                    {
                        unboundFace.InnerHalfEdges.Add(twinHalfEdge.Handle);
                        DictFaces[1] = unboundFace;
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
                halfEdge.NextHalfEdge = boundaryEdges[(i + 1) % boundaryEdges.Count].HalfEdge.Handle;
                twinHalfEdge.PrevHalfEdge = boundaryEdges[(i + 1) % boundaryEdges.Count].TwinHalfEdge.Handle;

                if (i - 1 < 0)
                {
                    halfEdge.PrevHalfEdge = boundaryEdges.LastItem().HalfEdge.Handle;
                    twinHalfEdge.NextHalfEdge = boundaryEdges.LastItem().TwinHalfEdge.Handle;
                }
                else
                {
                    halfEdge.PrevHalfEdge = boundaryEdges[i - 1].HalfEdge.Handle;
                    twinHalfEdge.NextHalfEdge = boundaryEdges[i - 1].TwinHalfEdge.Handle;
                }

                bEdge.HalfEdge = halfEdge;
                bEdge.TwinHalfEdge = twinHalfEdge;

                boundaryEdges[i] = bEdge;
            }
        }

        private void SetPrevAndNextToExistingHalfEdge(BoundaryEdge bEdge, int existingHeHandle,
            IList<BoundaryEdge> boundaryEdges, ICollection<HalfEdge> halfEdgesToUpdate)
        {
            var existingHe = GetHalfEdgeByHandle(existingHeHandle);
            var existingHeNext = GetHalfEdgeByHandle(existingHe.NextHalfEdge);
            var existingHePrev = GetHalfEdgeByHandle(existingHe.PrevHalfEdge);

            existingHe.NextHalfEdge = bEdge.HalfEdge.NextHalfEdge;
            existingHe.PrevHalfEdge = bEdge.HalfEdge.PrevHalfEdge;
            existingHe.IncidentFace = bEdge.HalfEdge.IncidentFace;

            for (var j = 0; j < boundaryEdges.Count; j++)
            {
                var count = 0;
                var be = boundaryEdges[j];
                if (be.TwinHalfEdge.Handle == bEdge.TwinHalfEdge.PrevHalfEdge)
                {
                    var twinHalfEdge = be.TwinHalfEdge;
                    twinHalfEdge.NextHalfEdge = existingHeNext.Handle;

                    var halfEdge = be.HalfEdge;
                    halfEdge.PrevHalfEdge = existingHeHandle;

                    be.TwinHalfEdge = twinHalfEdge;
                    be.HalfEdge = halfEdge;

                    existingHeNext.PrevHalfEdge = twinHalfEdge.Handle;

                    halfEdgesToUpdate.Add(existingHeNext);

                    boundaryEdges[j] = be;
                    count++;
                }

                if (be.TwinHalfEdge.Handle == bEdge.TwinHalfEdge.NextHalfEdge)
                {
                    var twinHalfEdge = be.TwinHalfEdge;
                    twinHalfEdge.PrevHalfEdge = existingHePrev.Handle;

                    var halfEdge = be.HalfEdge;
                    halfEdge.NextHalfEdge = existingHeHandle;

                    be.TwinHalfEdge = twinHalfEdge;
                    be.HalfEdge = halfEdge;

                    existingHePrev.NextHalfEdge = twinHalfEdge.Handle;

                    halfEdgesToUpdate.Add(existingHePrev);

                    boundaryEdges[j] = be;
                    count++;
                }
                if (count == 2)
                    break;
            }
        }

        private bool IsEdgeExisting(HalfEdge halfEdge, IEnumerable<BoundaryEdge> boundaryEdges, out int existingHeHandle)
        {
            existingHeHandle = new int();

            var newHeTargetVert = new int();

            foreach (var be in boundaryEdges)
            {
                if (be.HalfEdge.Handle == halfEdge.NextHalfEdge)
                    newHeTargetVert = be.HalfEdge.OriginVertex;
            }

            if (newHeTargetVert == default(int))
                throw new ArgumentException("target vert not found");

            var heStartingAtOldV = GetVertexStartingHalfEdges(halfEdge.OriginVertex).ToList();

            foreach (var heHandle in heStartingAtOldV)
            {
                var he = heHandle;
                var oldHeTargetVert = GetHalfEdgeByHandle(he.NextHalfEdge).OriginVertex;

                if (oldHeTargetVert != newHeTargetVert) continue;
                existingHeHandle = heHandle.Handle;
                return true;
            }
            return false;
        }

        #endregion
    }
}
