using Fusee.Math.Core;
using Fusee.Serialization;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Creates a simple sphere geometry straight from the code.
    /// </summary>
    public class Sphere: Mesh
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sphere" /> class.
        /// The default sphere has a radius of 1 unit.
        /// </summary>
        public Sphere(int segments, int rings)
        {
            BuildSphere(segments, rings);
        }

        private void BuildSphere(int segments, int rings)// segments: Longitude ||| - rings: Latitude ---
        {
            const float radius = 1f;
            const double pi = System.Math.PI;
            const double twoPi = pi * 2f;

            #region Vertices
            var vertices = new float3[(segments + 1) * rings + 2];

            vertices[0] = float3.UnitY * radius;
            
            for (var lat = 0; lat < rings; lat++)
            {
                var a1 = pi * (lat + 1) / (rings + 1);
                var sin1 = (float)System.Math.Sin(a1);
                var cos1 = (float)System.Math.Cos(a1);

                for (var lon = 0; lon <= segments; lon++)
                {
                    var a2 = twoPi * (lon == segments ? 0 : lon) / segments;
                    var sin2 = (float)System.Math.Sin(a2);
                    var cos2 = (float)System.Math.Cos(a2);

                    vertices[lon + lat * (segments + 1) + 1] = new float3(sin1 * cos2, cos1, sin1 * sin2) * radius;
                }
            }
            vertices[vertices.Length - 1] = float3.UnitY * -radius;
            #endregion

            #region Normals		
            var normals = new float3[vertices.Length];
            for (var n = 0; n < vertices.Length; n++)
            {
                var norm = vertices[n];
                norm.Normalize();
                normals[n] = norm;
            }
            #endregion

            #region UVs
            var uvs = new float2[vertices.Length];
            uvs[0] = new float2(0.5f, 1f);
            uvs[uvs.Length - 1] = new float2(0.5f, 0f);
            for (var lat = 0; lat < rings; lat++)
                for (var lon = 0; lon <= segments; lon++)
                    uvs[lon + lat * (segments + 1) + 1] = new float2((float)lon / segments, 1f - (float)(lat + 1) / (rings + 1));
            #endregion

            #region Triangles
            var nbFaces = vertices.Length;
            var nbTriangles = nbFaces * 2;
            var nbIndexes = nbTriangles * 3;
            var triangles = new ushort[nbIndexes];

            //Top Cap
            var i = 0;
            for (var lon = 0; lon < segments; lon++)
            {
                triangles[i++] = (ushort) (lon + 2);
                triangles[i++] = 0;
                triangles[i++] = (ushort) (lon + 1);
                
            }

            //Middle
            for (var lat = 0; lat < rings - 1; lat++)
            {
                for (var lon = 0; lon < segments; lon++)
                {
                    var current = (ushort)(lon + lat * (segments + 1) + 1);
                    var next = (ushort) (current + segments + 1);

                    triangles[i++] =  current;
                    triangles[i++] = (ushort)(next + 1);
                    triangles[i++] = (ushort) ( current + 1);
                    

                    triangles[i++] =  current;
                    triangles[i++] = next;
                    triangles[i++] = (ushort) (next + 1);
                   
                }
            }

            //Bottom Cap
            for (var lon = 0; lon < segments; lon++)
            {
                triangles[i++] = (ushort) (vertices.Length - 1);
                triangles[i++] = (ushort)(vertices.Length - (lon + 1) - 1);
                triangles[i++] = (ushort) (vertices.Length - (lon + 2) - 1);
                
            }
            #endregion

            Vertices = vertices;
            Triangles = triangles;
            Normals = normals;
            UVs = uvs;
            
        }

    }
}
