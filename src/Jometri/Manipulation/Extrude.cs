using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Common;
using Fusee.Jometri.DCEL;
using Fusee.Math.Core;

namespace Fusee.Jometri.Manipulation
{
    /// <summary>
    /// Provides extrusion functionality for geometry.
    /// </summary>
    public static class Extrude
    {
        /// <summary>
        /// Extrudes a complete planar geometry.
        /// </summary>
        /// <param name="geometry">The geometry to be extruded.</param>
        /// <param name="offset">zOffset will be used to create the new vertices.</param>
        /// <param name="extrudeAlongNormal">Pass 'true' if you want do extrude the polygon along its normal and 'false' if you want to extrude along the worlds z axis.</param>
        /// <returns></returns>
        public static Geometry Extrude2DPolygon(this Geometry geometry, float offset, bool extrudeAlongNormal)
        {
            CreateTopSurface(geometry, offset, extrudeAlongNormal);
            CreateSidefaces(geometry);

            var extrusion = new Geometry()
            {
                DictFaces = new Dictionary<int, Face>(geometry.DictFaces),
                DictVertices = new Dictionary<int, Vertex>(geometry.DictVertices),
                DictHalfEdges = new Dictionary<int, HalfEdge>(geometry.DictHalfEdges),
                HighestHalfEdgeHandle = geometry.HighestHalfEdgeHandle,
                HighestFaceHandle = geometry.HighestFaceHandle,
                HighestVertHandle = geometry.HighestVertHandle,
            };

            return extrusion;
        }

        private static void CreateTopSurface(Geometry geometry, float zOffset, bool exturdeAlongNormal)
        {
            //Clone frontface.
            var backface = geometry.CloneGeometry();

            if(!exturdeAlongNormal)
                //Add zOffset to each vertex coord.
                UpdateVertexZCoord(backface, zOffset);
            else
            {
                var unbounded = backface.GetFaceVertices(1).ToList();
                var normal = GeometricOperations.CalculateFaceNormal(unbounded);
                UpdateVertexZCoord(backface, unbounded, normal, zOffset);
            }

            Join2DGeometries(geometry, backface);
        }

        private static void UpdateVertexZCoord(Geometry geometry, float zOffset)
        {
            foreach (var vertkey in geometry.DictVertices.Keys.ToList())
            {
                var v = geometry.DictVertices[vertkey];
                v.VertData.Pos = new float3(v.VertData.Pos.x, v.VertData.Pos.y, v.VertData.Pos.z + zOffset);
                geometry.DictVertices[vertkey] = v;
            }
        }

        private static void UpdateVertexZCoord(Geometry backface, IEnumerable<Vertex> verts, float3 normal, float zOffset)
        {
            foreach (var v in verts)
            {
                var newPos = v.VertData.Pos + normal*zOffset;
                var vert = new Vertex(v, newPos);
                backface.DictVertices[vert.Handle] = vert;
            }
        }

        private static void CreateSidefaces(Geometry geometry)
        {
            var unboundedFace = geometry.GetFaceByHandle(1); //The unbounded face is always added first - therefore it will always have 1 as handle.

            var frontLoopsStartHalfEdges = unboundedFace.InnerHalfEdges.TakeItems(unboundedFace.InnerHalfEdges.Count / 2).ToList();
            var backLoopsStartHalfEdges = unboundedFace.InnerHalfEdges.Skip(unboundedFace.InnerHalfEdges.Count / 2).ToList();

            for (var i = 0; i < frontLoopsStartHalfEdges.Count; i++)
            {
                var frontEdgeLoop = geometry.GetHalfEdgeLoop(frontLoopsStartHalfEdges[i]).ToList();
                var backEdgeLoop = geometry.GetHalfEdgeLoopReverse(backLoopsStartHalfEdges[i]).ToList();

                var newHalfEdges = new List<HalfEdge>();

                var newFaces = new List<Face>();

                for (var j = 0; j < frontEdgeLoop.Count; j++)
                {
                    var halfEdgeFront = frontEdgeLoop[j];
                    var halfEdgeInBack = backEdgeLoop[j];

                    var backOriginVert = geometry.GetHalfEdgeByHandle(halfEdgeInBack.NextHalfEdge).OriginVertex;
                    var frontOriginVert = geometry.GetHalfEdgeByHandle(halfEdgeFront.NextHalfEdge).OriginVertex;

                    var newFromBack = new HalfEdge(geometry.CreateHalfEdgeHandleId())
                    {
                        OriginVertex = backOriginVert,
                        NextHalfEdge = halfEdgeFront.Handle,
                        PrevHalfEdge = halfEdgeInBack.Handle
                    };

                    var newFace = new Face(geometry.CreateFaceHandleId(), newFromBack.Handle);
                    newFaces.Add(newFace);

                    geometry.DictFaces.Add(newFace.Handle, newFace);

                    newFromBack.IncidentFace = newFace.Handle;

                    var newFromFront = new HalfEdge(geometry.CreateHalfEdgeHandleId())
                    {
                        OriginVertex = frontOriginVert,
                        NextHalfEdge = halfEdgeInBack.Handle,
                        PrevHalfEdge = halfEdgeFront.Handle,
                        IncidentFace = newFace.Handle
                    };

                    halfEdgeFront.IncidentFace = newFace.Handle;
                    halfEdgeFront.NextHalfEdge = newFromFront.Handle;
                    halfEdgeFront.PrevHalfEdge = newFromBack.Handle;

                    halfEdgeInBack.IncidentFace = newFace.Handle;
                    halfEdgeInBack.NextHalfEdge = newFromBack.Handle;
                    halfEdgeInBack.PrevHalfEdge = newFromFront.Handle;

                    geometry.ReplaceHalfEdge(halfEdgeFront);
                    geometry.ReplaceHalfEdge(halfEdgeInBack);

                    newHalfEdges.Add(newFromBack);
                    newHalfEdges.Add(newFromFront);
                }

                for (var j = 0; j < newHalfEdges.Count; j++)
                {
                    var current = newHalfEdges[j];
                    if (j == 0)
                        current.TwinHalfEdge = newHalfEdges.Last().Handle;
                    else if (j == newHalfEdges.Count - 1)
                        current.TwinHalfEdge = newHalfEdges[0].Handle;
                    else if (j % 2 != 0 && j != newHalfEdges.Count - 1) //odd
                        current.TwinHalfEdge = newHalfEdges[j + 1].Handle;
                    else if (j % 2 == 0 && j != 0) //even
                        current.TwinHalfEdge = newHalfEdges[j - 1].Handle;
                    newHalfEdges[j] = current;

                    geometry.DictHalfEdges.Add(current.Handle, current);
                }

                foreach (var face in newFaces)
                {
                    geometry.SetFaceNormal(geometry.GetFaceOuterVertices(face.Handle).ToList(), geometry.DictFaces[face.Handle]);
                }
            }

            //Delete unbounded face
            geometry.DictFaces.Remove(unboundedFace.Handle);
        }

        private static void Join2DGeometries(Geometry first, Geometry second)
        {
            var highestVertHandle = first.HighestVertHandle;
            var highestHalfEdgeHandle = first.HighestHalfEdgeHandle;
            var highestFaceHandle = first.HighestFaceHandle;

            var vertDictHelper = new Dictionary<int, Vertex>();
            foreach (var v in second.DictVertices)
            {
                var vert = new Vertex(v.Value.Handle + highestVertHandle, v.Value.VertData.Pos);
                vert.IncidentHalfEdge = v.Value.IncidentHalfEdge + highestHalfEdgeHandle;

                vertDictHelper.Add(vert.Handle, vert);
            }
            second.DictVertices.Clear();
            second.DictVertices = vertDictHelper;

            var faceDictHelper = new Dictionary<int, Face>();
            foreach (var f in second.DictFaces)
            {
                var face = new Face(f.Value.Handle + highestFaceHandle, f.Value);

                if (face.OuterHalfEdge != default(int))
                {
                    var outerHeId = face.OuterHalfEdge + first.HighestHalfEdgeHandle;
                    face.OuterHalfEdge = outerHeId;
                }

                for (var k = 0; k < face.InnerHalfEdges.Count; k++)
                {
                    var innerHe = face.InnerHalfEdges[k];
                    innerHe = innerHe + first.HighestHalfEdgeHandle;
                    face.InnerHalfEdges[k] = innerHe;
                }

                faceDictHelper.Add(face.Handle, face);
            }
            second.DictFaces.Clear();
            second.DictFaces = faceDictHelper;

            var heDictHelper = new Dictionary<int, HalfEdge>();
            foreach (var he in second.DictHalfEdges)
            {
                var halfEdge = new HalfEdge(he.Value.Handle + highestHalfEdgeHandle, he.Value);

                halfEdge.IncidentFace = halfEdge.IncidentFace + first.HighestFaceHandle;
                halfEdge.OriginVertex = halfEdge.OriginVertex + first.HighestVertHandle;

                halfEdge.NextHalfEdge = halfEdge.NextHalfEdge + highestHalfEdgeHandle;
                halfEdge.PrevHalfEdge = halfEdge.PrevHalfEdge + highestHalfEdgeHandle;

                if (halfEdge.TwinHalfEdge != default(int))
                    halfEdge.TwinHalfEdge = halfEdge.TwinHalfEdge + highestHalfEdgeHandle;

                heDictHelper.Add(halfEdge.Handle, halfEdge);
            }
            second.DictHalfEdges.Clear();
            second.DictHalfEdges = heDictHelper;

            //Change winding.
            var hEdgesWChangedWinding = second.GetHalfEdgesWChangedWinding(second.GetAllHalfEdges()).ToList();

            //Add data of second geometry to the first.
            foreach (var vert in second.DictVertices)
            {
                first.DictVertices.Add(vert.Key, vert.Value);
            }

            foreach (var halfEdge in hEdgesWChangedWinding)
            {
                first.DictHalfEdges.Add(halfEdge.Handle, halfEdge);
            }

            //Write content of second undbounded face into the first - delete second unbounded face
            var firstUnbounded = first.DictFaces[first.DictFaces.Keys.Min()];
            var secUnbounded = second.DictFaces[second.DictFaces.Keys.Min()];
            firstUnbounded.InnerHalfEdges.AddRange(secUnbounded.InnerHalfEdges);
            second.DictFaces.Remove(secUnbounded.Handle);

            var secUnboundedHalfEdges = new List<HalfEdge>();
            foreach (var he in first.GetAllHalfEdges())
            {
                if (he.IncidentFace == secUnbounded.Handle)
                    secUnboundedHalfEdges.Add(he);
            }

            //Replace the incident face of the half edges.
            foreach (var he in secUnboundedHalfEdges)
            {
                var halfEdge = he;
                halfEdge.IncidentFace = firstUnbounded.Handle;
                first.DictHalfEdges.Remove(halfEdge.Handle);
                first.DictHalfEdges.Add(halfEdge.Handle, halfEdge);
            }

            //Add faces to the first geometry and recalculate the face normals.
            foreach (var face in second.DictFaces)
            {
                first.DictFaces.Add(face.Key, face.Value);
                first.SetFaceNormal(first.GetFaceOuterVertices(face.Key).ToList(), first.DictFaces[face.Key]);
            }

            first.SetHighestHandles();
        }
    }
}
