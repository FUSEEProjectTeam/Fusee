using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    /// <inheritdoc />
    /// <summary>
    /// Creates a fusee-compatible mesh from a triangulated Jometri geometry
    /// </summary>
    public class JometriMesh : Mesh
    {
        /// <summary>
        /// Creates a fusee-compatible mesh from a Jometri geometry
        /// </summary>
        /// <param name="geometry">The triangulated Jometri geometry, saved in a doubly connected edge list</param>
        public JometriMesh(Jometri.Geometry geometry)
        {
            ConvertToMesh(geometry, out var vertices, out var triangles, out var normals);

            Vertices = vertices;
            Triangles = triangles;
            Normals = normals.ToArray();
        }

        //Geometry has to be triangulated! Translates a Jometri.Geometry into a Fusee.Mesh.
        private static void ConvertToMesh(Jometri.Geometry geometry, out float3[] vertices, out ushort[] triangles, out List<float3> normals)
        {
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

                    if (face.FaceData.FaceNormal == float3.Zero)
                        geometry.SetFaceNormal(geometry.GetFaceOuterVertices(face.Handle).ToList(), face);

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