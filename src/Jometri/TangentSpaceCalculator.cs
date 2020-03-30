using System;
using System.Diagnostics;
using Fusee.Engine.Common;
using Fusee.Math.Core;
// ReSharper disable ObjectCreationAsStatement

namespace Fusee.Jometri
{
    /// <summary>
    /// Static class for tangent space calculation
    /// </summary>
    public static class TangentSpaceCalulator
    {
        /// <summary>
        /// Calculates the tangents of a mesh
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static float4[] CalculateTangents(this Mesh m)
        {
            if (m == null)
                new ArgumentException("Mesh cannot be empty!");

            if (m.Normals == null || m.Normals.Length < 0)
                new ArgumentException($"Can not calculate tangents, empty normals in this mesh: {m.Name}");

            if (m.UVs == null)
                new ArgumentException("Can not calculate tangents, no uv map");


            var tangentList = new float4[m.Vertices.Length];

            var tan1 = new float3[m.Vertices.Length * 2];
            var tan2 = new float3[tan1.Length + m.Vertices.Length];

            for (var i = 0; i < m.Triangles.Length - 3; i++)
            {
                var i1 = m.Triangles[0 + i];
                var i2 = m.Triangles[1 + i];
                var i3 = m.Triangles[2 + i];

                var v1 = m.Vertices[i1];
                var v2 = m.Vertices[i2];
                var v3 = m.Vertices[i3];


                var w1 = m.UVs[i1];
                var w2 = m.UVs[i2];
                var w3 = m.UVs[i3];

                var x1 = v2.x - v1.x;
                var x2 = v3.x - v1.x;
                var y1 = v2.y - v1.y;
                var y2 = v3.y - v1.y;
                var z1 = v2.z - v1.z;
                var z2 = v3.z - v1.z;

                var s1 = w2.x - w1.x;
                var s2 = w3.x - w1.x;
                var t1 = w2.y - w1.y;
                var t2 = w3.y - w1.y;


                var r = System.Math.Abs((s1 * t2 - s2 * t1)) < float.Epsilon ? 0 : 1.0f / (s1 * t2 - s2 * t1); // texture coordinates broken! isInfinity not possible with JSIL 

                var sdir = new float3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r,
                    (t2 * z1 - t1 * z2) * r);

                var tdir = new float3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r,
                    (s1 * z2 - s2 * z1) * r);

                tan1[i1] += sdir;
                tan1[i2] += sdir;
                tan1[i3] += sdir;

                tan2[i1] += tdir;
                tan2[i2] += tdir;
                tan2[i3] += tdir;
            }

            for (var i = 0; i < m.Vertices.Length; i++)
            {
                Debug.Assert(m.Normals != null, "m.Normals != null");

                var n = m.Normals[i];
                var t = tan1[i];

                // Gram-Schmidt orthogonalize
                var tangent = t - n * float3.Dot(n, t);
                tangent = tangent.Normalize();

                // Calculate handedness
                var wComponent = (float3.Dot(float3.Cross(n, t), tan2[i]) < 0.0F) ? -1.0f : 1.0f;

                tangentList[i] = new float4(tangent, wComponent);
            }

            return tangentList;
        }

        /// <summary>
        /// Calculates the bitangents of a mesh
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static float3[] CalculateBiTangents(this Mesh m)
        {
            if (m == null)
                new ArgumentException("Mesh cannot be empty!");

            if (m?.Normals == null || m.Normals.Length < 0)
                new ArgumentException($"Can not calculate bitangents, empty normals in this mesh: {m.Name}");

            if (m.Tangents == null || m.Tangents.Length < 1)
                new ArgumentException($"Can not calculate bitangents, empty tangent list in this mesh: {m.Name}");

            if (m.Tangents != null && (m.Normals != null && m.Normals.Length != m.Tangents.Length))
                new ArgumentException($"Can not calculate bitangents, quantitiy of normals: {m.Normals.Length} and quanitity of tangents: {m.Tangents.Length} differs.");


            var bitangents = new float3[m.Tangents.Length];

            for (var i = 0; i < m.Tangents.Length; i++)
                bitangents[i] = float3.Cross(m.Normals[i], m.Tangents[i].xyz) * m.Tangents[i].w;
            
            return bitangents;
        }
    }
}