using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Core;
using Fusee.Jometri.DCEL;

namespace Fusee.Jometri.Extrusion
{
    /// <summary>
    /// Extrudes a 2D geometry
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
            //Copy frontface
            var backface = new Geometry(geometry);

            //Add z value to each vertex coord
            backface.UpdateAllVertexCoordZ(zOffset);

            geometry.Join2DGeometries(backface);
        }

        private static void CreateSidefaces(Geometry geometry)
        {
            var unboundedFace = geometry.GetFaceByHandle(geometry.FaceHandles[0]);
            var frontHalfEdgesCount = geometry.HalfEdgeHandles.Count / 2;

            for (var i = 0; i < unboundedFace.InnerHalfEdges.Count/2; i++)
            {
                var edgeLoop = geometry.GetEdgeLoop(unboundedFace.InnerHalfEdges[i]).ToList();
                var newHalfEdges = new List<Geometry.HalfEdge>();

                for (var j = 0; j < edgeLoop.Count; j++)
                {
                    var halfEdgeFront = edgeLoop[j];
               
                    var id = halfEdgeFront.Handle.Id + frontHalfEdgesCount;
                    var halfEdgeBack = geometry.GetHalfEdgeByHandle(HalfEdgeBackHandle(geometry, id));

                    var backTargetVert = geometry.GetHalfEdgeByHandle(halfEdgeBack.Next).Origin;
                    var frontTargetVert = geometry.GetHalfEdgeByHandle(halfEdgeFront.Next).Origin;

                    var newFromBack = new Geometry.HalfEdge();
                    newFromBack.Origin = backTargetVert;
                    newFromBack.Handle = new HalfEdgeHandle(geometry.HalfEdgeHandles.Count + 1);
                    newFromBack.Next = halfEdgeFront.Handle;
                    newFromBack.Prev = halfEdgeBack.Handle;

                    geometry.HalfEdgeHandles.Add(newFromBack.Handle);
                    
                    var newFace = new Geometry.Face();
                    newFace.Handle = new FaceHandle(geometry.FaceHandles.LastItem().Id + 1); //TODO: other way to create id?!
                    newFace.OuterHalfEdge = newFromBack.Handle;
                    newFace.InnerHalfEdges = new List<HalfEdgeHandle>();

                    geometry.AddFace(newFace);
                    geometry.FaceHandles.Add(newFace.Handle);

                    newFromBack.IncidentFace = newFace.Handle;

                    var newFromFront = new Geometry.HalfEdge();
                    newFromFront.Origin = frontTargetVert;
                    newFromFront.Handle = new HalfEdgeHandle(geometry.HalfEdgeHandles.Count + 1);
                    newFromFront.Next = halfEdgeBack.Handle;
                    newFromFront.Prev = halfEdgeFront.Handle;
                    newFromFront.IncidentFace = newFace.Handle;

                    geometry.HalfEdgeHandles.Add(newFromBack.Handle);

                    halfEdgeFront.IncidentFace = newFace.Handle;
                    halfEdgeFront.Next = newFromFront.Handle;
                    halfEdgeFront.Prev = newFromBack.Handle;

                    halfEdgeBack.IncidentFace = newFace.Handle;
                    halfEdgeBack.Next = newFromBack.Handle;
                    halfEdgeBack.Prev = newFromFront.Handle;

                    geometry.UpdateHalfEdge(halfEdgeFront);
                    geometry.UpdateHalfEdge(halfEdgeBack);

                    newHalfEdges.Add(newFromBack);
                    newHalfEdges.Add(newFromFront);
                }

                for (var j = 0; j < newHalfEdges.Count-1; j++ )
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
                    geometry.AddHalfEdge(halfEdge);
                }

                for (int j = 0; j < newHalfEdges.Count; j+=2)
                {
                    geometry.InsertEdge(newHalfEdges[j].Origin, newHalfEdges[j+1].Origin);
                }
            }
        }

        private static HalfEdgeHandle HalfEdgeBackHandle(Geometry geometry, int id)
        {
            foreach (var heHandle in geometry.HalfEdgeHandles)
            {
                if (heHandle.Id == id)
                    return heHandle;
            }
            return default(HalfEdgeHandle);
        }
    }
}
