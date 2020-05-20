using System.Collections.Generic;
using System.Linq;
using Fusee.Math.Core;

namespace Fusee.Jometri
{
    /// <summary>
    /// Provides extrusion functionality for geometries.
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

            var extrusion = new Geometry
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

        /// <summary>
        /// Extrudes a given Face by a given offset along its normal vector.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="faceHandle">The handle of the face to extrude.</param>
        /// <param name="offset">How far the face should get extruded.</param>
        /// <returns></returns>
        public static Geometry ExtrudeFace(this Geometry geometry, int faceHandle, float offset)
        {
            var face = geometry.GetFaceByHandle(faceHandle);
            return ExtrudeFaceByHandle(geometry, faceHandle, offset, face.FaceData.FaceNormal);
        }

        /// <summary>
        /// Extrudes a given Face along a specified Vector.
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="faceHandle"></param>
        /// <param name="offset"></param>
        /// <param name="extrusionVector"></param>
        /// <returns></returns>
        public static Geometry ExtrudeFace(this Geometry geometry, int faceHandle, float offset, float3 extrusionVector)
        {
            extrusionVector = extrusionVector.Normalize();
            return ExtrudeFaceByHandle(geometry, faceHandle, offset, extrusionVector);
        }

        private static Geometry ExtrudeFaceByHandle(Geometry geometry, int faceHandle, float offset, float3 extrusionVector)
        {
            var face = geometry.GetFaceByHandle(faceHandle);

            //get HE of Face
            var start = geometry.GetHalfEdgeByHandle(face.OuterHalfEdge);
            var next = start;

            var vertexIncHe = new Dictionary<int, List<HalfEdge>>();
            var allFaceVertices = geometry.GetFaceVertices(faceHandle);
            foreach (var vertex in allFaceVertices)
                vertexIncHe.Add(vertex.Handle, geometry.GetVertexStartingHalfEdges(vertex.Handle).ToList());
            var allH2NEdges = new List<HalfEdge>();

            do
            {
                var nextOriginV = geometry.GetVertexByHandle(next.OriginVertex);
                var newVertex = new Vertex(geometry.CreateVertHandleId(), nextOriginV.VertData.Pos);

                var twinEdge = geometry.GetHalfEdgeByHandle(next.TwinHalfEdge);
                var prevEdge = geometry.GetHalfEdgeByHandle(next.PrevHalfEdge);
                var prevTwinEdge = geometry.GetHalfEdgeByHandle(prevEdge.TwinHalfEdge);

                nextOriginV.VertData.Pos = nextOriginV.VertData.Pos + extrusionVector * offset;

                var h4 = new HalfEdge(geometry.CreateHalfEdgeHandleId());
                var h2n = new HalfEdge(geometry.CreateHalfEdgeHandleId());

                var h1 = new HalfEdge(geometry.CreateHalfEdgeHandleId());

                var currentList = vertexIncHe[nextOriginV.Handle];
                foreach (var halfEdge in currentList)
                {
                    if (halfEdge == next) continue;
                    var edge = GeomEditing.UpdateHalfEdgeOrigin(halfEdge, newVertex.Handle);
                    geometry.ReplaceHalfEdge(edge);
                }

                nextOriginV.IncidentHalfEdge = next.Handle;

                h4.OriginVertex = nextOriginV.Handle;
                h2n.OriginVertex = newVertex.Handle;
                h1.OriginVertex = newVertex.Handle;

                h4.TwinHalfEdge = h2n.Handle;
                h2n.TwinHalfEdge = h4.Handle;

                h4.NextHalfEdge = h1.Handle;
                h1.PrevHalfEdge = h4.Handle;

                h1.TwinHalfEdge = next.TwinHalfEdge;
                twinEdge.TwinHalfEdge = h1.Handle;

                prevTwinEdge.OriginVertex = newVertex.Handle;

                newVertex.IncidentHalfEdge = h2n.Handle;

                geometry.ReplaceHalfEdge(twinEdge);
                geometry.ReplaceHalfEdge(prevTwinEdge);
                geometry.ReplaceVertex(nextOriginV);
                geometry.DictVertices.Add(newVertex.Handle, newVertex);
                geometry.DictHalfEdges.Add(h4.Handle, h4);
                geometry.DictHalfEdges.Add(h1.Handle, h1);
                geometry.DictHalfEdges.Add(h2n.Handle, h2n);

                allH2NEdges.Add(h2n);

                next = geometry.GetHalfEdgeByHandle(next.NextHalfEdge);
            } while (start != next);

            start = geometry.GetHalfEdgeByHandle(face.OuterHalfEdge);
            next = start;
            do
            {
                var newFace = new Face(geometry.CreateFaceHandleId());

                var twinEdge = geometry.GetHalfEdgeByHandle(next.TwinHalfEdge);

                var h1 = geometry.GetHalfEdgeByHandle(twinEdge.TwinHalfEdge);
                var h2 = allH2NEdges.First(n => n.OriginVertex == twinEdge.OriginVertex);
                var h3 = new HalfEdge(geometry.CreateHalfEdgeHandleId());
                var h4 = geometry.GetHalfEdgeByHandle(h1.PrevHalfEdge);

                //set Face
                h1.IncidentFace = newFace.Handle;
                h2.IncidentFace = newFace.Handle;
                h3.IncidentFace = newFace.Handle;
                h4.IncidentFace = newFace.Handle;

                h1.NextHalfEdge = h2.Handle;
                h2.NextHalfEdge = h3.Handle;
                h3.NextHalfEdge = h4.Handle;
                h4.NextHalfEdge = h1.Handle;

                h1.PrevHalfEdge = h4.Handle;
                h2.PrevHalfEdge = h1.Handle;
                h3.PrevHalfEdge = h2.Handle;
                h4.PrevHalfEdge = h3.Handle;

                h3.TwinHalfEdge = next.Handle;
                h3.OriginVertex = geometry.GetHalfEdgeByHandle(next.NextHalfEdge).OriginVertex;
                next.TwinHalfEdge = h3.Handle;
                newFace.OuterHalfEdge = h1.Handle;

                //write all changes
                geometry.ReplaceHalfEdge(h1);
                geometry.ReplaceHalfEdge(h2);
                geometry.ReplaceHalfEdge(h4);
                geometry.ReplaceHalfEdge(next);

                geometry.DictHalfEdges.Add(h3.Handle, h3);
                geometry.DictFaces.Add(newFace.Handle, newFace);

                newFace.FaceData.FaceNormal = GeometricOperations.CalculateFaceNormal(geometry.GetFaceVertices(newFace.Handle).ToList());

                geometry.ReplaceFace(newFace);

                next = geometry.GetHalfEdgeByHandle(next.NextHalfEdge);
            } while (start != next);

            return geometry;
        }

        private static void CreateTopSurface(Geometry geometry, float zOffset, bool exturdeAlongNormal)
        {
            //Clone front face.
            var backface = geometry.CloneGeometry();

            if (!exturdeAlongNormal)
                //Add zOffset to each vertex coordinate.
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
                var newPos = v.VertData.Pos + normal * zOffset;
                var vert = new Vertex(v, newPos);
                backface.DictVertices[vert.Handle] = vert;
            }
        }

        private static void CreateSidefaces(Geometry geometry)
        {
            var unboundedFace = geometry.GetFaceByHandle(1); //The unbounded face is always added first - therefore it will always have 1 as handle.

            var frontLoopsStartHalfEdges = unboundedFace.InnerHalfEdges.Take(unboundedFace.InnerHalfEdges.Count / 2).ToList();
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

            //Write content of second unbounded face into the first - delete second unbounded face
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
