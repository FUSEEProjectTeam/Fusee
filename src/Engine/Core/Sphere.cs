using System;
using System.Collections.Generic;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    public class Icosahedron : Mesh
    {
        public Icosahedron()
        {
            // create 12 vertices of a icosahedron
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
                
            //Normalize to get unit icosahedron
            foreach (var vert in verts)
            {
                vert.Normalize();
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
            Normals = CalcNormalHelper.CreateNormals(this).ToArray();
        }
    }


    public class Sphere: Mesh
    {
        private ushort newVertIndex;
        private readonly Dictionary<long, int> _middlePointIndexCache;
        private readonly List<float3> _sphereVertices;
        
        public Sphere(int recursionLevel)
        {
            _sphereVertices = new List<float3>();
            _middlePointIndexCache = new Dictionary<Int64, int>();
            var sphere = Create(recursionLevel);

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

            // refine triangles
            for (var i = 0; i < recursionLevel; i++)
            {
                var faces2 = new List<ushort>();
                var tri =  new List<ushort>();

                for (var k = 0; k < mesh.Triangles.Length; k++)
                {
                    tri.Add(mesh.Triangles[k]);
                    if(tri.Count != 3) continue;

                    // replace triangle by 4 triangles
                    var a = GetMiddlePoint(tri[0], tri[1]);
                    var b = GetMiddlePoint(tri[1], tri[2]);
                    var c = GetMiddlePoint(tri[2], tri[0]);

                    var temp = new List<ushort>
                    {
                        c, a, tri[0],
                        a, b, tri[1],
                        b, c, tri[2],
                        c, b, a
                    };

                    faces2.AddRange(temp);

                    tri = new List<ushort>();
                }
                mesh.Triangles = faces2.ToArray();
            }

            mesh.Vertices = _sphereVertices.ToArray();

            mesh.Normals = CalcNormalHelper.CreateNormals(mesh).ToArray();

            return mesh;
        }

        // Add vertex to mesh, normalize position to be on unit sphere
        private ushort AddNormalizedVertex(float3 p)
        {
            p.Normalize();
            _sphereVertices.Add(p);

            return newVertIndex++;
        }

        // return index of point in the middle of p1 and p2
        private ushort GetMiddlePoint(int p1, int p2)
        {

            // first check if we have it already
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

           // add vertex makes sure point is on unit sphere
           var i = AddNormalizedVertex(middle);

            _middlePointIndexCache.Add(key, i);
            return i;
        }
        
    }

    public static class CalcNormalHelper
    {

        public static  List<float3> CreateNormals(Mesh mesh)
        {
            var normals = new List<float3>();

            var triVerts = new List<float3>();
            foreach (var tri in mesh.Triangles)
            {
                triVerts.Add(mesh.Vertices[tri]);

                if (triVerts.Count != 3) continue;
                normals.Add(CalculateTriNormal(triVerts));
                normals.Add(CalculateTriNormal(triVerts));
                normals.Add(CalculateTriNormal(triVerts));
                triVerts = new List<float3>();
            }

            return normals;
        }

        private static float3 CalculateTriNormal(IList<float3> triVerts)
        {
            var v = triVerts[1] - triVerts[0];
            var w = triVerts[2] - triVerts[1];

            var normal = float3.Cross(w,v);
            
            normal.Normalize();

            return normal;
        }
    }

}