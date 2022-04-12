using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using System;

namespace Fusee.Engine.Core
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
        public static void CalculateTangents(this Mesh m)
        {
            if (m == null)
                throw new ArgumentException("Mesh cannot be empty!");

            if (m.Normals == null || m.Normals.Length < 0)
                throw new ArgumentException($"Can not calculate tangents, empty normals in this mesh: {m.Name}");

            if (m.UVs == null)
                throw new ArgumentException("Can not calculate tangents, no uv map");


            var tangents = new float4[m.Vertices.Length];

            for (var i = 0; i < m.Triangles.Length; i += 3)
            {
                var edge1 = m.Vertices[m.Triangles[i + 1]] - m.Vertices[m.Triangles[i]];
                var edge2 = m.Vertices[m.Triangles[i + 2]] - m.Vertices[m.Triangles[i]];

                var deltaUv1 = m.UVs[m.Triangles[i + 1]] - m.UVs[m.Triangles[i]];
                var deltaUv2 = m.UVs[m.Triangles[i + 2]] - m.UVs[m.Triangles[i]];

                var f = 1.0f / (deltaUv1.x * deltaUv2.y - deltaUv2.x * deltaUv1.y);

                var tangent = new float3
                {
                    x = f * (deltaUv2.y * edge1.x - deltaUv1.y * edge2.x),
                    y = f * (deltaUv2.y * edge1.y - deltaUv1.y * edge2.y),
                    z = f * (deltaUv2.y * edge1.z - deltaUv1.y * edge2.z)
                };

                tangent.Normalize();

                //Orthogonalization
                var normal = m.Normals[m.Triangles[i]];
                var dot = normal.x * tangent.x + normal.y * tangent.y + normal.z * tangent.z;
                tangent -= normal * dot;

                //Set w component to 0 for correct normalization of the per vertex tangent. Then set w to 1 for correct handedness.
                //Handedness: a advantage of tangent space normal maps is, that you can use just one "side" of the uv map IF you have a symmetric model.
                //For the other side our uv coordinates will be orientated the wrong way so our tangent has the wrong orientation and the handedness needs to be -1.
                //If we want to support "half" normal maps calculate handedness as shown in the link below (bitangent needed).
                // see: http://www.opengl-tutorial.org/intermediate-tutorials/tutorial-13-normal-mapping/#handedness and https://docs.unity3d.com/ScriptReference/m-tangents.html
                var tangent4 = new float4(tangent.x, tangent.y, tangent.z, 0);

                tangents[m.Triangles[i]] += tangent4;
                tangents[m.Triangles[i]].Normalize();
                tangents[m.Triangles[i]].w = 1;

                tangents[m.Triangles[i + 1]] += tangent4;
                tangents[m.Triangles[i + 1]].Normalize();
                tangents[m.Triangles[i + 1]].w = 1;

                tangents[m.Triangles[i + 2]] += tangent4;
                tangents[m.Triangles[i + 2]].Normalize();
                tangents[m.Triangles[i + 2]].w = 1;
            }
            m.Tangents = tangents;
        }

        /// <summary>
        /// Calculates the bitangents of a mesh
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static void CalculateBiTangents(this Mesh m)
        {
            if (m == null)
                throw new ArgumentException("Mesh cannot be empty!");

            if (m?.Normals == null || m.Normals.Length < 0)
                throw new ArgumentException($"Can not calculate bitangents, empty normals in this mesh: {m.Name}");

            if (m.Tangents == null || m.Tangents.Length < 1)
                throw new ArgumentException($"Can not calculate bitangents, empty tangent list in this mesh: {m.Name}");

            if (m.Tangents != null && (m.Normals != null && m.Normals.Length != m.Tangents.Length))
                throw new ArgumentException($"Can not calculate bitangents, quantity of normals: {m.Normals.Length} and quantity of tangents: {m.Tangents.Length} differs.");


            var bitangents = new float3[m.Tangents.Length];

            for (var i = 0; i < m.Tangents.Length; i++)
                bitangents[i] = float3.Cross(m.Normals[i], m.Tangents[i].xyz) * m.Tangents[i].w;

            m.BiTangents = bitangents;
        }
    }
}