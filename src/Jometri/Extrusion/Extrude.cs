using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Core;
using Fusee.Jometri.DCEL;
using Fusee.Math.Core;

namespace Fusee.Jometri.Extrusion
{
    /// <summary>
    /// Provides extrusion functionality for geometry.
    /// </summary>
    public static class Extrude
    {
        //zOffset will be added to each vertex' z coordinate, if the front face is not parallel to the x-y plane we have to rotate it there first, extrude and rotate back.
        /// <summary>
        /// Extrudes a trinagulated 2D geometry.
        /// </summary>
        /// <param name="geometry">The geometry to be extruded.</param>
        /// <param name="zOffset">zOffset will be added to each vertex' z coordinate in order to create the backface of the geometry.</param>
        /// <returns></returns>
        public static Geometry3D Extrude2DPolygon(this Geometry2D geometry, int zOffset)
        {
            CreateBackface(geometry, zOffset);
            CreateSidefaces(geometry);

            var geom3D = new Geometry3D
            {
                DictFaces = geometry.DictFaces, //TODO: Convert to List<Face3D>
                DictVertices = geometry.DictVertices,
                DictHalfEdges = geometry.DictHalfEdges,
            };

            //TODO: geom3D.HighestHalfEdgeHandle = geometry.HighestHalfEdgeHandle / HighestVertexHandle / HighestFaceHandle

            return geom3D;
        }

        private static void CreateBackface(Geometry2D geometry, int zOffset)
        {
            //Clone frontface
            var backface = geometry.CloneGeometry();

            //Add z value to each vertex coord
            UpdateAllVertexZCoord(backface, zOffset);

            Join2DGeometries(geometry, backface);
        }

        private static void UpdateAllVertexZCoord(Geometry2D geometry, int zOffset)
        {
            foreach (var vertkey in geometry.DictVertices.Keys.ToList())
            {
                var v = geometry.DictVertices[vertkey];
                v.VertData.Pos = new float3(v.VertData.Pos.x, v.VertData.Pos.y, v.VertData.Pos.z + zOffset);
                geometry.DictVertices[vertkey] = v;
            }
            
        }

        private static void CreateSidefaces(Geometry2D geometry)
        {
            var unboundedFace = (Face2D)geometry.GetFaceByHandle(1); //The unbounded face is always added first - therefore it will always have 1 as handle.

            var frontLoopsStartHalfEdges = unboundedFace.InnerHalfEdges.TakeItems(unboundedFace.InnerHalfEdges.Count / 2).ToList();
            var backLoopsStartHalfEdges = unboundedFace.InnerHalfEdges.SkipItems(unboundedFace.InnerHalfEdges.Count / 2).ToList();

            for (var i = 0; i < frontLoopsStartHalfEdges.Count; i++)
            {
                var frontEdgeLoop = geometry.GetHalfEdgeLoop(frontLoopsStartHalfEdges[i]).ToList();
                var backEdgeLoop = geometry.GetHalfEdgeLoop(backLoopsStartHalfEdges[i]).ToList();
                backEdgeLoop.Reverse();

                var newHalfEdges = new List<HalfEdge>();

                for (var j = 0; j < frontEdgeLoop.Count; j++)
                {
                    var heHandleFront = frontEdgeLoop[j];
                    var halfEdgeFront = heHandleFront;

                    var heHandleBack = backEdgeLoop[j];
                    var halfEdgeInBack = heHandleBack;

                    var backTargetVert = geometry.GetHalfEdgeByHandle(halfEdgeInBack.NextHalfEdge).OriginVertex;
                    var frontTargetVert = geometry.GetHalfEdgeByHandle(halfEdgeFront.NextHalfEdge).OriginVertex;

                    var newFromBack = new HalfEdge
                    {
                        OriginVertex = backTargetVert,
                        Handle = geometry.CreateHalfEdgeHandleId(),
                        NextHalfEdge = halfEdgeFront.Handle,
                        PrevHalfEdge = halfEdgeInBack.Handle
                    };

                    var newFace = new Face2D(geometry.CreateFaceHandleId(), newFromBack.Handle);
                    
                    geometry.DictFaces.Add(newFace.Handle, newFace);

                    newFromBack.IncidentFace = newFace.Handle;

                    var newFromFront = new HalfEdge
                    {
                        OriginVertex = frontTargetVert,
                        Handle = geometry.CreateHalfEdgeHandleId(),
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
                        current.TwinHalfEdge = newHalfEdges.LastItem().Handle;
                    else if (j == newHalfEdges.Count - 1)
                        current.TwinHalfEdge = newHalfEdges[0].Handle;
                    else if (j % 2 != 0 && j != newHalfEdges.Count - 1) //odd
                        current.TwinHalfEdge = newHalfEdges[j + 1].Handle;
                    else if (j % 2 == 0 && j != 0) //even
                        current.TwinHalfEdge = newHalfEdges[j - 1].Handle;
                    newHalfEdges[j] = current;

                    geometry.DictHalfEdges.Add(current.Handle, current);
                }

                for (var j = 0; j < newHalfEdges.Count; j += 2)
                {
                    geometry.InsertDiagonal(newHalfEdges[j].OriginVertex, newHalfEdges[j + 1].OriginVertex);
                }
            }

            //Delete unbounded face
            geometry.DictFaces.Remove(unboundedFace.Handle);
        }

        private static void Join2DGeometries(Geometry2D first, Geometry2D second)
        {
            var highestVertHandle = first.HighestVertHandle;
            var highestHalfEdgeHandle = first.HighestHalfEdgeHandle;
            var highestFaceHandle = first.HighestFaceHandle;

            var vertDictHelper = new Dictionary<int, Vertex>();
            foreach (var v in second.DictVertices)
            {
                var vert = v.Value;
                vert.Handle = vert.Handle + highestVertHandle;
                vert.IncidentHalfEdge = vert.IncidentHalfEdge + highestHalfEdgeHandle;
                vertDictHelper.Add(vert.Handle, vert);
            }
            second.DictVertices.Clear();
            second.DictVertices = vertDictHelper;

            var faceDictHelper = new Dictionary<int, IFace>();
            foreach (var f in second.DictFaces)
            {
                var face = (Face2D)f.Value;

                face.Handle = face.Handle + highestFaceHandle;

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
                var halfEdge = he.Value;

                halfEdge.IncidentFace = halfEdge.IncidentFace + first.HighestFaceHandle;
                halfEdge.OriginVertex = halfEdge.OriginVertex + first.HighestVertHandle;

                halfEdge.Handle = halfEdge.Handle + highestHalfEdgeHandle;

                halfEdge.NextHalfEdge = halfEdge.NextHalfEdge + highestHalfEdgeHandle;
                halfEdge.PrevHalfEdge = halfEdge.PrevHalfEdge + highestHalfEdgeHandle;

                if (halfEdge.TwinHalfEdge != default(int))
                    halfEdge.TwinHalfEdge = halfEdge.TwinHalfEdge + highestHalfEdgeHandle;

                heDictHelper.Add(halfEdge.Handle, halfEdge);
            }
            second.DictHalfEdges.Clear();
            second.DictHalfEdges = heDictHelper;

            //Change winding
            var hEdgesWChangedWinding = HalfEdgesWChangedWinding(second.GetAllHalfEdges(), second).ToList();

            //Add data of second geometry to first one
            foreach (var vert in second.DictVertices)
            {
                first.DictVertices.Add(vert.Key, vert.Value);
            }

            foreach (var halfEdge in hEdgesWChangedWinding)
            {
                first.DictHalfEdges.Add(halfEdge.Handle, halfEdge);
            }

            //Write content of second undbounded face into the first - delete second unbounded face
            var firstUnbounded = (Face2D)first.DictFaces[first.DictFaces.Keys.Min()];
            var secUnbounded = (Face2D)second.DictFaces[second.DictFaces.Keys.Min()];
            firstUnbounded.InnerHalfEdges.AddRange(secUnbounded.InnerHalfEdges);
            second.DictFaces.Remove(secUnbounded.Handle);

            foreach (var face in second.DictFaces)
            {
                first.DictFaces.Add(face.Key,face.Value);
            }

            first.SetHighestHandles();
        }

        private static IEnumerable<HalfEdge> HalfEdgesWChangedWinding(IEnumerable<HalfEdge> originHEdges, DCEL.Geometry geom)
        {
            foreach (var hEdge in originHEdges)
            {
                var he = hEdge;
                var next = he.PrevHalfEdge;
                var prev = he.NextHalfEdge;

                he.NextHalfEdge = next;
                he.PrevHalfEdge = prev;

                var newOrigin = geom.GetHalfEdgeByHandle(he.PrevHalfEdge).OriginVertex;
                he.OriginVertex = newOrigin;

                yield return he;

                var vertToUpdate = geom.DictVertices[newOrigin];
                vertToUpdate.IncidentHalfEdge = he.Handle;
                geom.DictVertices[newOrigin] = vertToUpdate;
            }
        }
    }
}
