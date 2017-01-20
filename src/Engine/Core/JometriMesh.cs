using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Jometri;
using Fusee.Jometri.DCEL;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    public class JometriMesh : Mesh
    {
        public JometriMesh(Geometry3D geometry)
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
        private static void ConvertToMesh(Geometry3D geometry, out float3[] vertices, out ushort[] triangles, out List<float3> normals)
        {
            var triangleCount = geometry.GetAllFaces().ToList().Count;
            var vertCount = triangleCount * 3;

            var verts = new List<float3>();

            vertices = new float3[vertCount];
            triangles = new ushort[vertCount];
            normals = new List<float3>();

            var faces = geometry.GetAllFaces().ToList();

            foreach (var face in faces)
            {
                var faceVerts = geometry.GetFaceVertices(face.Handle).ToList();

                if (faceVerts.Count > 3)
                    throw new ArgumentException("Invalid triangle - face has more than 3 Vertices");

                foreach (var vertex in faceVerts)
                {
                    verts.Add(vertex.VertData.Pos);
                    normals.Add(geometry.CalculateFaceNormal(face.Handle));
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
