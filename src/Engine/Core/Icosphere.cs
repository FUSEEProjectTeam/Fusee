using System;
using System.Collections.Generic;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Unit Icosahedron primitive.
    /// </summary>
    public class Icosahedron : Mesh
    {
        /// <summary>
        /// Creates an unit Icosahedron. Used to create a Icosphere.
        /// </summary>
        public Icosahedron()
        {
            //Create the 12 vertices of a icosahedron.
            var t = (float)(1.0 + System.Math.Sqrt(5.0)) / 2.0f;

            var verts = new List<float3>
            {
                new float3(-1, t, 0),
                new float3(1, t, 0),
                new float3(-1, -t, 0),
                new float3(1, -t, 0),

                new float3(0, -1, t),
                new float3(0, 1, t),
                new float3(0, -1, -t),
                new float3(0, 1, -t),

                new float3(t, 0, -1),
                new float3(t, 0, 1),
                new float3(-t, 0, -1),
                new float3(-t, 0, 1)
            };

            //Normalize to get the unit icosahedron.
            for (var i = 0; i < verts.Count; i++)
            {
                var vert = verts[i];
                vert.Normalize();
                verts[i] = vert;
            }

            Vertices = verts.ToArray();

            Triangles = new ushort[]
            {
                5, 11, 0,
                1, 5, 0,
                7, 1, 0,
                10, 7, 0,
                11, 10, 0,
                9, 5, 1,
                4, 11, 5,
                2, 10, 11,
                6, 7, 10,
                8, 1, 7,
                4, 9, 3,
                2, 4, 3,
                6, 2, 3,
                8, 6, 3,
                9, 8, 3,
                5, 9, 4,
                11, 4, 2,
                10, 2, 6,
                7, 6, 8,
                1, 8, 9
            };

            Normals = new float3[] { };
            CalcNormalHelper.CreateVertexNormals(this);
        }
    }


    /// <summary>
    /// Unit Icosphere primitive.
    /// </summary>
    public class Icosphere : Mesh
    {
        private ushort _newVertIndex;
        private readonly Dictionary<long, int> _middlePointIndexCache;
        private readonly List<float3> _sphereVertices;

        /// <summary>
        /// Creates an unit Icosphere from an Icosahedron.
        /// </summary>
        /// <param name="subdivLevel">Defines the number subdivisions and therefor the number of triangles and the smoothness of the sphere.</param>
        public Icosphere(int subdivLevel)
        {
            if (subdivLevel > 6)
            {
                //Restrict subdivision level to 6 at max - normals aren't calculated correctly above this value (possibly because of too small values).
                subdivLevel = 6;
            }

            _sphereVertices = new List<float3>();
            _middlePointIndexCache = new Dictionary<Int64, int>();
            var sphere = Create(subdivLevel);

            Vertices = sphere.Vertices;
            Triangles = sphere.Triangles;
            Normals = sphere.Normals;
        }

        private Mesh Create(int recursionLevel)
        {
            Mesh mesh = new Icosahedron();

            //Add Verts to temp vert list
            foreach (var vert in mesh.Vertices)
            {
                AddNormalizedVertex(vert);
            }

            //Subdevide triangles
            for (var i = 0; i < recursionLevel; i++)
            {
                var faces2 = new List<ushort>();
                var tri = new List<ushort>();

                for (var k = 0; k < mesh.Triangles.Length; k++)
                {
                    tri.Add(mesh.Triangles[k]);
                    if (tri.Count != 3) continue;

                    //Replace triangle by four triangles.
                    var a = GetMiddlePoint(tri[0], tri[1]);
                    var b = GetMiddlePoint(tri[1], tri[2]);
                    var c = GetMiddlePoint(tri[2], tri[0]);

                    var temp = new List<ushort>
                    {
                        tri[0], a, c,
                        tri[1], b, a,
                        tri[2], c, b,
                        a, b, c
                    };

                    faces2.AddRange(temp);

                    tri = new List<ushort>();
                }
                mesh.Triangles = faces2.ToArray();
            }

            mesh.Vertices = _sphereVertices.ToArray();
            CalcNormalHelper.CreateVertexNormals(mesh);
            
            return mesh;
        }

        //Add vertex to mesh, normalize position to be on unit sphere.
        private ushort AddNormalizedVertex(float3 p)
        {
            p.Normalize();
            _sphereVertices.Add(p);

            return _newVertIndex++;
        }

        //Return the index of the point in the middle between p1 and p2.
        private ushort GetMiddlePoint(int p1, int p2)
        {

            //Check the cache...
            var firstIsSmaller = p1 < p2;
            long smallerIndex = firstIsSmaller ? p1 : p2;
            long greaterIndex = firstIsSmaller ? p2 : p1;
            var key = (smallerIndex << 32) + greaterIndex;

            if (_middlePointIndexCache.TryGetValue(key, out var ret))
            {
                return (ushort)ret;
            }

            var point1 = _sphereVertices[p1];
            var point2 = _sphereVertices[p2];
            var middle = new float3(
                (point1.x + point2.x) / 2.0f,
                (point1.y + point2.y) / 2.0f,
                (point1.z + point2.z) / 2.0f);

            //Add the Vertex and make sure the point is on the unit sphere.
            var i = AddNormalizedVertex(middle);

            _middlePointIndexCache.Add(key, i);
            return i;
        }

    }

    /// <summary>
    /// Contains static methods to calculate the normals for a mesh.
    /// </summary>
    public static class CalcNormalHelper
    {
        /// <summary>
        /// Calculates the vertex normals for a given mesh by calculating the avarage of all normals of faces, adjacent to a vertex.
        /// </summary>
        /// <param name="mesh">The mesh for which to calculate the normals.</param>
        public static void CreateVertexNormals(Mesh mesh)
        {
            mesh.Normals = new float3[mesh.Vertices.Length];
            
            var triVerts = new List<float3>();
            var triIndeices =  new List<ushort>();
            foreach (var tri in mesh.Triangles)
            {
                triVerts.Add(mesh.Vertices[tri]);
                triIndeices.Add(tri);

                if (triVerts.Count != 3) continue;

                var triNormal = CalculateTriNormal(triVerts);

                CalcAverageNormal(mesh.Normals, triNormal, triIndeices[0]);
                CalcAverageNormal(mesh.Normals, triNormal, triIndeices[1]);
                CalcAverageNormal(mesh.Normals, triNormal, triIndeices[2]);

                triVerts = new List<float3>();
                triIndeices = new List<ushort>();
            }
        }

        private static void CalcAverageNormal(IList<float3> normals, float3 triNormal, ushort triIndex)
        {
            if (normals[triIndex] == float3.Zero)
                normals[triIndex] = triNormal;
            else
            {
                var averageNormal = (triNormal + normals[triIndex]) / 2;
                normals[triIndex] = averageNormal;
            }
        }

        private static float3 CalculateTriNormal(IList<float3> triVerts)
        {
            var v = triVerts[1] - triVerts[0];
            var w = triVerts[2] - triVerts[1];

            var normal = float3.Cross(w, v);

            normal.Normalize();
            return normal;
        }
    }

}
