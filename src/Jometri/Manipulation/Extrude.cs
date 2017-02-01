using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Fusee.Base.Core;
using Fusee.Jometri.DCEL;
using Fusee.Math.Core;

namespace Fusee.Jometri.Manipulation
{
    /// <summary>
    /// Provides extrusion functionality for geometry.
    /// </summary>
    public static class Extrude
    {
        private static Dictionary<int, Face> _frontFaces;
        private static Dictionary<int, Face> _backfaces;

        //zOffset will be added to each vertex's z coordinate, if the front face is not parallel to the x-y plane we have to rotate it there first, extrude and rotate back.
        /// <summary>
        /// Extrudes a trinagulated 2D geometry.
        /// </summary>
        /// <param name="geometry">The geometry to be extruded.</param>
        /// <param name="zOffset">zOffset will be added to each vertex's z coordinate in order to create the backface of the geometry.</param>
        /// <returns></returns>
        [Pure] //Contract to ensure that the return value is used.
        public static Extrusion Extrude2DPolygon(this Geometry2D geometry, int zOffset)
        {
            _frontFaces = new Dictionary<int, Face>(geometry.DictFaces);
            _frontFaces.Remove(1);
            _backfaces = new Dictionary<int, Face>();

            CreateBackface(geometry, zOffset);
            CreateSidefaces(geometry);

            var extrusion = new Extrusion()
            {
                DictFaces = new Dictionary<int, Face>(geometry.DictFaces),
                DictVertices = new Dictionary<int, Vertex>(geometry.DictVertices),
                DictHalfEdges = new Dictionary<int, HalfEdge>(geometry.DictHalfEdges),
                FrontFaces = _frontFaces,
                Backfaces = _backfaces,
                HighestHalfEdgeHandle = geometry.HighestHalfEdgeHandle,
                HighestFaceHandle = geometry.HighestFaceHandle,
                HighestVertHandle = geometry.HighestVertHandle,
            };

            return extrusion;
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
            var unboundedFace = geometry.GetFaceByHandle(1); //The unbounded face is always added first - therefore it will always have 1 as handle.

            var frontLoopsStartHalfEdges = unboundedFace.InnerHalfEdges.TakeItems(unboundedFace.InnerHalfEdges.Count / 2).ToList();
            var backLoopsStartHalfEdges = unboundedFace.InnerHalfEdges.SkipItems(unboundedFace.InnerHalfEdges.Count / 2).ToList();

            for (var i = 0; i < frontLoopsStartHalfEdges.Count; i++)
            {
                var frontEdgeLoop = geometry.GetHalfEdgeLoop(frontLoopsStartHalfEdges[i]).ToList();
                var backEdgeLoop = geometry.GetHalfEdgeLoopReverse(backLoopsStartHalfEdges[i]).ToList();
                
                var newHalfEdges = new List<HalfEdge>();

                var newFaces = new List<Face>();

                for (var j = 0; j < frontEdgeLoop.Count; j++)
                {
                    var heHandleFront = frontEdgeLoop[j];
                    var halfEdgeFront = heHandleFront;

                    var heHandleBack = backEdgeLoop[j];
                    var halfEdgeInBack = heHandleBack;

                    var backTargetVert = geometry.GetHalfEdgeByHandle(halfEdgeInBack.NextHalfEdge).OriginVertex;
                    var frontTargetVert = geometry.GetHalfEdgeByHandle(halfEdgeFront.NextHalfEdge).OriginVertex;

                    var newFromBack = new HalfEdge(geometry.CreateHalfEdgeHandleId())
                    {
                        OriginVertex = backTargetVert,
                        NextHalfEdge = halfEdgeFront.Handle,
                        PrevHalfEdge = halfEdgeInBack.Handle
                    };

                    var newFace = new Face(geometry.CreateFaceHandleId(), newFromBack.Handle);
                    newFaces.Add(newFace);
                    
                    geometry.DictFaces.Add(newFace.Handle, newFace);

                    newFromBack.IncidentFace = newFace.Handle;

                    var newFromFront = new HalfEdge(geometry.CreateHalfEdgeHandleId())
                    {
                        OriginVertex = frontTargetVert,
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

                /*for (var j = 0; j < newHalfEdges.Count; j += 2)
                {
                    geometry.InsertDiagonal(newHalfEdges[j].OriginVertex, newHalfEdges[j + 1].OriginVertex);
                }*/

                foreach (var face in newFaces)
                {
                    geometry.SetFaceNormal(geometry.GetFaceOuterVertices(face.Handle).ToList(), geometry.DictFaces[face.Handle]);
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
                var vert = new Vertex(v.Value.Handle + highestVertHandle,v.Value.VertData.Pos);
                vert.IncidentHalfEdge = vert.IncidentHalfEdge + highestHalfEdgeHandle;
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
                var halfEdge = new HalfEdge(he.Value.Handle+ highestHalfEdgeHandle, he.Value);

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

            //Change winding
            var hEdgesWChangedWinding = second.GetHalfEdgesWChangedWinding(second.GetAllHalfEdges()).ToList();

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
            var firstUnbounded = first.DictFaces[first.DictFaces.Keys.Min()];
            var secUnbounded = second.DictFaces[second.DictFaces.Keys.Min()];
            firstUnbounded.InnerHalfEdges.AddRange(secUnbounded.InnerHalfEdges);
            second.DictFaces.Remove(secUnbounded.Handle);

            var secUnboundedHalfEdges = new List<HalfEdge>();
            foreach (var he in first.GetAllHalfEdges())
            {
                if(he.IncidentFace == secUnbounded.Handle)
                    secUnboundedHalfEdges.Add(he);
            }

            //Replace incident face of half edges with second unbounded face with first unbounded face
            foreach (var he in secUnboundedHalfEdges)
            {
                var halfEdge = he;
                halfEdge.IncidentFace = firstUnbounded.Handle;
                first.DictHalfEdges.Remove(halfEdge.Handle);
                first.DictHalfEdges.Add(halfEdge.Handle, halfEdge);
            }

            //Add faces to first and recalculate face normals (because of changed winding).
            foreach (var face in second.DictFaces)
            {
                _backfaces.Add(face.Key, face.Value);
                first.DictFaces.Add(face.Key,face.Value);
                first.SetFaceNormal(first.GetFaceOuterVertices(face.Key).ToList(), first.DictFaces[face.Key]);
            }

            first.SetHighestHandles();
        }

       
    }
}
