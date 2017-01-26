using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Jometri;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    public class JometriMesh : Mesh
    {
        public JometriMesh(Jometri.DCEL.Geometry geometry)
        {
            float3[] vertices;
            ushort[] triangles;
            List<float3> normals;

            ConvertToMesh(geometry, out vertices, out triangles, out normals);

            Vertices = vertices;
            Triangles = triangles;
            Normals = normals.ToArray();
        }

        //Geometry has to be trinagulated!
        private static void ConvertToMesh(Jometri.DCEL.Geometry geometry, out float3[] vertices, out ushort[] triangles, out List<float3> normals)
        {
            // Delete unbounded face - if it exists
            var faces = geometry.GetAllFaces().ToList();
            if (faces[0].Handle == 1)
                faces.RemoveAt(0);

            var triangleCount = faces.Count;
            var vertCount = triangleCount * 3;

            var verts = new List<float3>();

            vertices = new float3[vertCount];
            triangles = new ushort[vertCount];
            normals = new List<float3>();

            foreach (var face in faces)
            {
                if (face.Handle == 1) { continue; }

                var faceVerts = geometry.GetFaceVertices(face.Handle).ToList();

                if (faceVerts.Count > 3)
                    throw new ArgumentException("Invalid triangle - face has more than 3 Vertices");

                foreach (var vertex in faceVerts)
                {
                    verts.Add(vertex.VertData.Pos);

                    if(face.FaceData.FaceNormal == float3.Zero)
                        geometry.SetFaceNormal(face);

                    normals.Add(face.FaceData.FaceNormal);
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
