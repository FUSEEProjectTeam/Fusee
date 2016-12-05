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
        //zOffset will be added to each vertex' z coordinate, if the front face is not parallel to the x-y plane we have to rotate it there first, extrude and rotate back
        /// <summary>
        /// Extrudes a trinagulated 2D geometry
        /// </summary>
        /// <param name="geometry">The geometry to be extruded.</param>
        /// <param name="zOffset">zOffset will be added to each vertex' z coordinate in order to create the back of the geometry </param>
        /// <returns></returns>
        public static Geometry Extrude2DPolygon(this Geometry geometry, int zOffset)
        {
            CreateBackface(geometry, zOffset);
            CreateSidefaces(geometry);

            return geometry;
        }

        private static void CreateBackface(Geometry geometry, int zOffset)
        {
            //Clone frontface
            var backface = geometry.CloneGeometry();

            //Add z value to each vertex coord
            UpdateAllVertexZCoord(backface, zOffset);

            Join2DGeometries(geometry, backface);
        }

        private static void UpdateAllVertexZCoord(Geometry geometry,int zOffset)
        {
            for (var i = 0; i < geometry.Vertices.Count; i++)
            {
                var v = geometry.Vertices[i];
                v.Coord = new float3(v.Coord.x, v.Coord.y, v.Coord.z + zOffset);
                geometry.Vertices[i] = v;
            }
        }

        private static void CreateSidefaces(Geometry geometry)
        {
            var unboundedFace = geometry.GetFaceByHandle(geometry.FaceHandles[0]);
            var halfEdgeCountInFront = geometry.HalfEdgeHandles.Count / 2;

            for (var i = 0; i < unboundedFace.InnerHalfEdges.Count / 2; i++)
            {
                var edgeLoop = geometry.GetEdgeLoop(unboundedFace.InnerHalfEdges[i]).ToList();
                var newHalfEdges = new List<HalfEdge>();

                foreach (var heHandle in edgeLoop)
                {
                    var halfEdgeFront = geometry.GetHalfEdgeByHandle(heHandle);

                    var id = halfEdgeFront.Handle.Id + halfEdgeCountInFront;
                    var halfEdgeInBack = geometry.GetHalfEdgeByHandle(GetFelowHalfEdgeInBackface(geometry, id));

                    var backTargetVert = geometry.GetHalfEdgeByHandle(halfEdgeInBack.Next).Origin;
                    var frontTargetVert = geometry.GetHalfEdgeByHandle(halfEdgeFront.Next).Origin;

                    var newFromBack = new HalfEdge
                    {
                        Origin = backTargetVert,
                        Handle = new HalfEdgeHandle(geometry.CreateHalfEdgeHandleId()),
                        Next = halfEdgeFront.Handle,
                        Prev = halfEdgeInBack.Handle
                    };

                    geometry.HalfEdgeHandles.Add(newFromBack.Handle);

                    var newFace = new Face
                    {
                        Handle = new FaceHandle(geometry.CreateFaceHandleId()),
                        OuterHalfEdge = newFromBack.Handle,
                        InnerHalfEdges = new List<HalfEdgeHandle>()
                    };

                    geometry.Faces.Add(newFace);
                    geometry.FaceHandles.Add(newFace.Handle);

                    newFromBack.IncidentFace = newFace.Handle;

                    var newFromFront = new HalfEdge
                    {
                        Origin = frontTargetVert,
                        Handle = new HalfEdgeHandle(geometry.CreateHalfEdgeHandleId()),
                        Next = halfEdgeInBack.Handle,
                        Prev = halfEdgeFront.Handle,
                        IncidentFace = newFace.Handle
                    };

                    geometry.HalfEdgeHandles.Add(newFromBack.Handle);

                    halfEdgeFront.IncidentFace = newFace.Handle;
                    halfEdgeFront.Next = newFromFront.Handle;
                    halfEdgeFront.Prev = newFromBack.Handle;

                    halfEdgeInBack.IncidentFace = newFace.Handle;
                    halfEdgeInBack.Next = newFromBack.Handle;
                    halfEdgeInBack.Prev = newFromFront.Handle;

                    geometry.UpdateHalfEdge(halfEdgeFront);
                    geometry.UpdateHalfEdge(halfEdgeInBack);

                    newHalfEdges.Add(newFromBack);
                    newHalfEdges.Add(newFromFront);
                }

                for (var j = 0; j < newHalfEdges.Count - 1; j++)
                {
                    var current = newHalfEdges[j];
                    if (j == 0)
                    {
                        var last = newHalfEdges.LastItem();

                        current.Twin = newHalfEdges.LastItem().Handle;
                        last.Twin = current.Handle;

                        newHalfEdges[newHalfEdges.Count - 1] = last;
                        newHalfEdges[j] = current;
                    }
                    else
                    {
                        var next = newHalfEdges[j + 1];

                        current.Twin = newHalfEdges[j + 1].Handle;
                        next.Twin = current.Handle;

                        newHalfEdges[j + 1] = next;
                        newHalfEdges[j] = current;
                        j = j + 1;
                    }
                }

                foreach (var halfEdge in newHalfEdges)
                {
                    geometry.HalfEdges.Add(halfEdge);
                }

                for (var j = 0; j < newHalfEdges.Count; j += 2)
                {
                    geometry.InsertDiagonal(newHalfEdges[j].Origin, newHalfEdges[j + 1].Origin);
                }
            }

            //Delete unbounded face
            geometry.Faces.Remove(unboundedFace);
            geometry.FaceHandles.Remove(unboundedFace.Handle);
        }

        private static HalfEdgeHandle GetFelowHalfEdgeInBackface(Geometry geometry, int id)
        {
            foreach (var heHandle in geometry.HalfEdgeHandles)
            {
                if (heHandle.Id == id)
                    return heHandle;
            }
            return default(HalfEdgeHandle);
        }

        private static void Join2DGeometries(Geometry first, Geometry second)
        {
            var vertStartHandle = first.Vertices.Count;
            for (var i = 0; i < second.VertHandles.Count; i++)
            {
                var vHandle = second.VertHandles[i];
                vHandle.Id = vHandle.Id + vertStartHandle;

                for (var j = 0; j < second.Vertices.Count; j++)
                {
                    var vert = second.Vertices[j];
                    if (vert.Handle != second.VertHandles[i]) continue;

                    vert.Handle.Id = vHandle.Id;
                    vert.IncidentHalfEdge.Id = vert.IncidentHalfEdge.Id + (first.HalfEdges.Count);
                    second.Vertices[j] = vert;
                    break;
                }
                second.VertHandles[i] = vHandle;
            }

            var faceStartHandle = first.Faces.Count;
            for (var i = 0; i < second.FaceHandles.Count; i++)
            {
                var fHandle = second.FaceHandles[i];
                fHandle.Id = fHandle.Id + faceStartHandle;

                for (var j = 0; j < second.Faces.Count; j++)
                {
                    var face = second.Faces[j];
                    if (face.Handle != second.FaceHandles[i]) continue;

                    face.Handle.Id = fHandle.Id;
                    face.OuterHalfEdge.Id = face.OuterHalfEdge.Id + (first.HalfEdges.Count);
                    second.Faces[j] = face;
                    break;
                }
                second.FaceHandles[i] = fHandle;
            }

            var heStartHandle = first.HalfEdges.Count;

            for (var i = 0; i < second.HalfEdgeHandles.Count; i++)
            {
                var heHandle = second.HalfEdgeHandles[i];
                heHandle.Id = heHandle.Id + heStartHandle;

                for (var j = 0; j < second.HalfEdges.Count; j++)
                {
                    var he = second.HalfEdges[j];
                    if (he.Handle != second.HalfEdgeHandles[i]) continue;


                    he.IncidentFace.Id = he.IncidentFace.Id + (first.Faces.Count);
                    he.Origin.Id = he.Origin.Id + (first.Vertices.Count);

                    he.Handle.Id = heHandle.Id;

                    he.Next.Id = he.Next.Id + (heStartHandle);
                    he.Prev.Id = he.Prev.Id + (heStartHandle);

                    if (he.Twin != default(HalfEdgeHandle))
                        he.Twin.Id = he.Twin.Id + heStartHandle;

                    second.HalfEdges[j] = he;
                    break;
                }
                second.HalfEdgeHandles[i] = heHandle;
            }

            //Change winding
            var zwerg = new List<HalfEdge>();
            foreach (var hEdge in second.HalfEdges)
            {
                var he = hEdge;
                var next = he.Prev;
                var prev = he.Next;

                he.Next = next;
                he.Prev = prev;

                var newOrigin = second.GetHalfEdgeByHandle(he.Prev).Origin;
                he.Origin = newOrigin;

                zwerg.Add(he);

                for (var i = 0; i < second.Vertices.Count; i++)
                {
                    var vert = second.Vertices[i];

                    if (vert.Handle != newOrigin) continue;

                    vert.IncidentHalfEdge = he.Handle;
                    second.Vertices[i] = vert;
                    break;
                }
            }

            for (var i = 0; i < second.HalfEdges.Count; i++)
            {
                second.HalfEdges[i] = zwerg[i];
            }

            //Add data of second geometry to this one
            for (var i = 0; i < second.VertHandles.Count; i++)
            {
                first.VertHandles.Add(second.VertHandles[i]);
                first.Vertices.Add(second.Vertices[i]);
            }

            //Write content of second undbounded face into the first - delete second unbounded face
            first.Faces[0].InnerHalfEdges.AddRange(second.Faces[0].InnerHalfEdges);
            second.Faces.RemoveAt(0);
            second.FaceHandles.RemoveAt(0);
            for (var i = 0; i < second.FaceHandles.Count; i++)
            {
                first.FaceHandles.Add(second.FaceHandles[i]);
                first.Faces.Add(second.Faces[i]);
            }

            for (var i = 0; i < second.HalfEdgeHandles.Count; i++)
            {
                first.HalfEdgeHandles.Add(second.HalfEdgeHandles[i]);
                first.HalfEdges.Add(second.HalfEdges[i]);
            }

            first.SetHighestHandles();
        }
    }
}
