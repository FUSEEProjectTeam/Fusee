using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Jometri
{

    /// <summary>
    /// Represents an outer or inner boundary of a polygon.
    /// </summary>
    public struct PolyBoundary
    {
        /// <summary>
        /// The geometric information of the vertices which belong to a boundary.
        /// </summary>
        public IList<float3> Points;

        /// <summary>
        /// Determines whether a boundary is a outer boundary or a inner boundary (which forms a hole in the face).
        /// </summary>
        public bool IsOuter;
    }

    /// <summary>
    /// Helper class for initializing Geometry objects.
    /// </summary>
    public static class GeomInitializeHelper
    {
        private static Geometry _geometry;

        #region geometry initialization from PolyBoundary

        /// <summary>
        /// Used in the initialization process of a new Geometry.
        /// A BoundaryEdge contains one edge of the boundary and the information whether the source vertex of the half edge (not the twin half edge) is already part of the Geometry.
        /// </summary>
        internal struct BoundaryEdge
        {
            internal bool IsOriginOldVert;
            internal HalfEdge HalfEdge;
            internal HalfEdge TwinHalfEdge;
        }

        internal static void CreateHalfEdgesForGeometry(this Geometry geometry, IEnumerable<PolyBoundary> outlines)
        {
            _geometry = geometry;
            var unboundedFace = new Face(geometry.DictHalfEdges.Count + 1);

            geometry.DictFaces.Add(unboundedFace.Handle, unboundedFace);

            foreach (var o in outlines)
            {
                var boundaryHalfEdges = CreateHalfEdgesForBoundary(o);
                foreach (var be in boundaryHalfEdges)
                {
                    geometry.DictHalfEdges.Add(be.HalfEdge.Handle, be.HalfEdge);
                    geometry.DictHalfEdges.Add(be.TwinHalfEdge.Handle, be.TwinHalfEdge);
                }
            }

            geometry.SetHighestHandles();
        }

        private static IEnumerable<BoundaryEdge> CreateHalfEdgesForBoundary(PolyBoundary outline)
        {
            var outlineVerts = OutlineVertices(outline);
            var boundaryEdges = BoundaryEdges(outlineVerts, outline);

            SetPrevAndNextForBoundary(boundaryEdges);

            var halfEdgesToUpdate = new List<HalfEdge>();

            for (var i = boundaryEdges.Count - 1; i > -1; i--)
            {
                var bEdge = boundaryEdges[i];

                if (!bEdge.IsOriginOldVert) continue; //A half-edge can only exist if its source vertex is an old one.

                if (!IsEdgeExisting(bEdge.HalfEdge, boundaryEdges, out int existingHeHandle))
                    continue; //Check the target vertex to identify the existing half edge.

                //If the existing half edge is halfedge.IncidentFace.OuterHalfEdge, replace it.
                var face = _geometry.GetFaceByHandle(bEdge.HalfEdge.IncidentFace);
                if (face.OuterHalfEdge == bEdge.HalfEdge.Handle)
                {
                    face.OuterHalfEdge = existingHeHandle;
                    _geometry.ReplaceFace(face);
                }

                //If the existing half edge is one of the unbounded faces inner half edges, replace it.
                var unboundedFace = _geometry.DictFaces[1];
                for (var k = 0; k < unboundedFace.InnerHalfEdges.Count; k++)
                {
                    var heHandle = unboundedFace.InnerHalfEdges[k];
                    if (heHandle != existingHeHandle) continue;
                    var nextHe = _geometry.GetHalfEdgeByHandle(heHandle).NextHalfEdge;

                    unboundedFace.InnerHalfEdges[k] = nextHe;
                    _geometry.DictFaces[1] = unboundedFace;
                    break;
                }

                var existingHe = _geometry.GetHalfEdgeByHandle(existingHeHandle);

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
                _geometry.ReplaceHalfEdge(he);
            }
            return boundaryEdges;
        }

        private static int AddFace(int firstHalfEdge, out Face face)
        {
            face = new Face(_geometry.DictFaces.Count + 1, firstHalfEdge);
            return face.Handle;
        }

        private static Vertex CreateOrFindVertex(float3 pointCoord, out bool isOldVertex, int handle)
        {
            int vertHandle;
            Vertex vert;

            //Check if a Vertex already exists and assign it to the HalfEdge instead of creating a new.
            if (_geometry.DictVertices.Count != 0)
            {
                foreach (var v in _geometry.GetAllVertices())
                {
                    if (!pointCoord.Equals(v.VertData.Pos)) continue;
                    isOldVertex = true;
                    return v;
                }

                //Create Vertex and VertHandle.
                vertHandle = handle;
                vert = new Vertex(vertHandle, pointCoord);
            }
            else
            {
                //Create Vertex and VertHandle.
                vertHandle = handle;
                vert = new Vertex(vertHandle, pointCoord);
            }
            isOldVertex = false;
            return vert;
        }

        private static List<KeyValuePair<Vertex, bool>> OutlineVertices(PolyBoundary polyBoundary)
        {
            var outlineVerts = new List<KeyValuePair<Vertex, bool>>();

            var handle = 0;
            if (_geometry.DictVertices.Count != 0)
                handle = _geometry.DictVertices.Count;

            foreach (var coord in polyBoundary.Points)
            {
                handle++;
                var vert = CreateOrFindVertex(coord, out bool isOldVert, handle);
                outlineVerts.Add(new KeyValuePair<Vertex, bool>(vert, isOldVert));
            }
            return outlineVerts;
        }

        private static List<BoundaryEdge> BoundaryEdges(IList<KeyValuePair<Vertex, bool>> outlineVerts,
            PolyBoundary polyBoundary)
        {
            var faceHandle = new int();
            var boundaryEdges = new List<BoundaryEdge>();

            var halfEdgeHandle = 0;
            if (_geometry.DictHalfEdges.Count != 0)
                halfEdgeHandle = _geometry.DictHalfEdges.Count;

            for (var j = 0; j < outlineVerts.Count; j++)
            {
                var currentVert = outlineVerts[j];
                halfEdgeHandle++;

                if (!currentVert.Value)
                {
                    //Only necessary for new vertices.
                    var vert = currentVert.Key;
                    vert.IncidentHalfEdge = halfEdgeHandle;
                    _geometry.DictVertices.Add(vert.Handle, vert);
                }

                var halfEdge = new HalfEdge(halfEdgeHandle, currentVert.Key.Handle);


                halfEdgeHandle++;
                var twinHalfEdge = new HalfEdge(halfEdgeHandle, outlineVerts[(j + 1) % outlineVerts.Count].Key.Handle,
                    halfEdge.Handle, 0, 0, 1);
                //The unbounded face is always added at first and therefor has 1 as handle.


                halfEdge.TwinHalfEdge = twinHalfEdge.Handle;

                //Assumption: outlines are processed from outer to inner, therefore faceHandle will never has its default value if "else" is hit.
                if (polyBoundary.IsOuter)
                {
                    if (faceHandle == default)
                    {
                        faceHandle = AddFace(halfEdge.Handle, out Face face);
                        _geometry.DictFaces.Add(face.Handle, face);
                    }
                }
                else
                {
                    if (j == 0)
                    {
                        var lastFace = _geometry.DictFaces[_geometry.DictFaces.Keys.Max()];
                        lastFace.InnerHalfEdges.Add(halfEdge.Handle);
                    }
                    faceHandle = _geometry.DictFaces.Last().Value.Handle;
                }

                halfEdge.IncidentFace = faceHandle;

                if (!outlineVerts[j].Value)
                {
                    var unboundFace = _geometry.DictFaces[1];

                    if (j == 0)
                    {
                        unboundFace.InnerHalfEdges.Add(twinHalfEdge.Handle);
                        _geometry.DictFaces[1] = unboundFace;
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
                    halfEdge.PrevHalfEdge = boundaryEdges.Last().HalfEdge.Handle;
                    twinHalfEdge.NextHalfEdge = boundaryEdges.Last().TwinHalfEdge.Handle;
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

        private static void SetPrevAndNextToExistingHalfEdge(BoundaryEdge bEdge, int existingHeHandle,
            IList<BoundaryEdge> boundaryEdges, ICollection<HalfEdge> halfEdgesToUpdate)
        {
            var existingHe = _geometry.GetHalfEdgeByHandle(existingHeHandle);
            var existingHeNext = _geometry.GetHalfEdgeByHandle(existingHe.NextHalfEdge);
            var existingHePrev = _geometry.GetHalfEdgeByHandle(existingHe.PrevHalfEdge);

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

        private static bool IsEdgeExisting(HalfEdge halfEdge, IEnumerable<BoundaryEdge> boundaryEdges, out int existingHeHandle)
        {
            existingHeHandle = new int();

            var newHeTargetVert = new int();

            foreach (var be in boundaryEdges)
            {
                if (be.HalfEdge.Handle == halfEdge.NextHalfEdge)
                    newHeTargetVert = be.HalfEdge.OriginVertex;
            }

            if (newHeTargetVert == default)
                throw new ArgumentException("Target vertex not found!");

            var heStartingAtOldV = _geometry.GetVertexStartingHalfEdges(halfEdge.OriginVertex).ToList();

            foreach (var heHandle in heStartingAtOldV)
            {
                var he = heHandle;
                var oldHeTargetVert = _geometry.GetHalfEdgeByHandle(he.NextHalfEdge).OriginVertex;

                if (oldHeTargetVert != newHeTargetVert) continue;
                existingHeHandle = heHandle.Handle;
                return true;
            }
            return false;
        }

        #endregion
    }
}