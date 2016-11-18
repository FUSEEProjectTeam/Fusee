using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Geometry = Fusee.Jometri.DCEL.Geometry;

namespace Fusee.Engine.Examples.ThreeDFont.Core
{
    public class HalfEdgesToMesh : Mesh
    {
        public HalfEdgesToMesh(Geometry geometry)
        {

            float3[] vertices;
            ushort[] triangles;

            ConvertToMesh(geometry, out vertices, out triangles);

            Vertices = vertices;
            Triangles = triangles;


        }

        //geometry has to be trinagulated
        private void ConvertToMesh(Geometry geometry, out float3[] vertices, out ushort[] triangles)
        {
            var triangleCount = geometry.FaceHandles.Count;
            var vertCount = triangleCount*3;
            

            var verts = new List<float3>();

            vertices = new float3[vertCount];
            triangles = new ushort[vertCount];

            foreach (var face in geometry.FaceHandles)
            {
                var faceVerts = geometry.GetFaceVertices(face).ToList();

                if (faceVerts.Count > 3)
                    throw new ArgumentException("Invalid Triangle - face has more than 3 Vertices");

                foreach (var vert in faceVerts)
                {
                    verts.Add(vert.Coord);
                }
            }

            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i] = verts[i];
                triangles[i] = (ushort)i;
            }
        }
    }
}
