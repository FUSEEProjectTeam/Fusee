using Fusee.Jometri.DCEL;
using Fusee.Math.Core;

namespace Fusee.Jometri.Extrusion
{
    public static class ExtrudeTriangulated
    {
        //zOffset will be added to each vertex' z coordinate, if the front face is not parallel to the x-y plane we have to rotate it there first, extrude and rotate back
        public static Geometry ExtrudePolygon(this Geometry geometry, int zOffset)
        {

            return CreateBackfaces(geometry, zOffset);
            //CreateSidefaces - Connect front and back face with triangles
        }

        private static Geometry CreateBackfaces(Geometry frontface, int zOffset)
        {
            //Copy frontface
            var backface = new Geometry(frontface);

            //Add z value to each vertex coord
            foreach (var vHandle in backface.VertHandles)
            {
                var vert = backface.GetVertexByHandle(vHandle);
                var newCoord = new float3(vert.Coord.x, vert.Coord.y, vert.Coord.x + zOffset);
                backface.OverwriteVertexCoord(vert,newCoord);
            }

            //Change winding of triangle (face)
            foreach (var fHandle in backface.FaceHandles)
            {
                var halfEdges = backface.GetHalfEdgesOfFace(fHandle);
                foreach (var heh in halfEdges)
                {
                    var he = backface.GetHalfEdgeByHandle(heh);
                    var prev = he.Next;
                    var next = he.Prev;

                    he.Next = next;
                    he.Prev = prev;

                    backface.OverwriteHalfEdge(he);
                }
            }

            //TODO: join backface and frontface

            return null;

        }
    }
}
