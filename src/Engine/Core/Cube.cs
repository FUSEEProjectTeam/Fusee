using Fusee.Math;

namespace Fusee.Engine
{
    /// <summary>
    /// Creates a simple cube geomentry straight from the code.
    /// </summary>
    public class Cube : Mesh
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cube" /> class.
        /// Cube is a derivate of the <see cref="Mesh" /> class.
        /// The default cube is 1 unit big and contains various default vertex colors.
        /// The vertex colors are only visible during rendering when a vertexcolor shader is applied on the Mesh.
        /// </summary>
        public Cube()
        {
            #region Fields

            // TODO: Remove redundant vertices
            Vertices = new[]
            {
                new float3 {x = +0.5f, y = -0.5f, z = +0.5f},
                new float3 {x = +0.5f, y = +0.5f, z = +0.5f},
                new float3 {x = -0.5f, y = +0.5f, z = +0.5f},
                new float3 {x = -0.5f, y = -0.5f, z = +0.5f},
                new float3 {x = +0.5f, y = -0.5f, z = -0.5f},
                new float3 {x = +0.5f, y = +0.5f, z = -0.5f},
                new float3 {x = +0.5f, y = +0.5f, z = +0.5f},
                new float3 {x = +0.5f, y = -0.5f, z = +0.5f},
                new float3 {x = -0.5f, y = -0.5f, z = -0.5f},
                new float3 {x = -0.5f, y = +0.5f, z = -0.5f},
                new float3 {x = +0.5f, y = +0.5f, z = -0.5f},
                new float3 {x = +0.5f, y = -0.5f, z = -0.5f},
                new float3 {x = -0.5f, y = -0.5f, z = +0.5f},
                new float3 {x = -0.5f, y = +0.5f, z = +0.5f},
                new float3 {x = -0.5f, y = +0.5f, z = -0.5f},
                new float3 {x = -0.5f, y = -0.5f, z = -0.5f},
                new float3 {x = +0.5f, y = +0.5f, z = +0.5f},
                new float3 {x = +0.5f, y = +0.5f, z = -0.5f},
                new float3 {x = -0.5f, y = +0.5f, z = -0.5f},
                new float3 {x = -0.5f, y = +0.5f, z = +0.5f},
                new float3 {x = +0.5f, y = -0.5f, z = -0.5f},
                new float3 {x = +0.5f, y = -0.5f, z = +0.5f},
                new float3 {x = -0.5f, y = -0.5f, z = +0.5f},
                new float3 {x = -0.5f, y = -0.5f, z = -0.5f}
            
            };

            Triangles = new ushort[]
            {
                // front face
                0, 2, 1, 0, 3, 2,

                // right face
                4, 6, 5, 4, 7, 6,
                
                // back face
                8, 10, 9, 8, 11, 10,
               
                // left face
                12, 14, 13, 12, 15, 14,
                
                // top face
                16, 18, 17, 16, 19, 18,

                // bottom face
                20, 22, 21, 20, 23, 22
            
            };

            Normals = new[]
            {
                new float3(0, 0, 1),
                new float3(0, 0, 1),
                new float3(0, 0, 1),
                new float3(0, 0, 1),
                new float3(1, 0, 0),
                new float3(1, 0, 0),
                new float3(1, 0, 0),
                new float3(1, 0, 0),
                new float3(0, 0, -1),
                new float3(0, 0, -1),
                new float3(0, 0, -1),
                new float3(0, 0, -1),
                new float3(-1, 0, 0),
                new float3(-1, 0, 0),
                new float3(-1, 0, 0),
                new float3(-1, 0, 0),
                new float3(0, 1, 0),
                new float3(0, 1, 0),
                new float3(0, 1, 0),
                new float3(0, 1, 0),
                new float3(0, -1, 0),
                new float3(0, -1, 0),
                new float3(0, -1, 0),
                new float3(0, -1, 0)
            };

            UVs = new[]
            {
                new float2(1, 0),
                new float2(1, 1),
                new float2(0, 1),
                new float2(0, 0),
                new float2(1, 0),
                new float2(1, 1),
                new float2(0, 1),
                new float2(0, 0),
                new float2(1, 0),
                new float2(1, 1),
                new float2(0, 1),
                new float2(0, 0),
                new float2(1, 0),
                new float2(1, 1),
                new float2(0, 1),
                new float2(0, 0),
                new float2(1, 0),
                new float2(1, 1),
                new float2(0, 1),
                new float2(0, 0),
                new float2(1, 0),
                new float2(1, 1),
                new float2(0, 1),
                new float2(0, 0)
            };
        }
        #endregion
    }
}