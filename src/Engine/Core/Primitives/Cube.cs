using Fusee.Engine.Common;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;

namespace Fusee.Engine.Core.Primitives
{
    /// <summary>
    /// Creates a simple wireframe cube geometry straight from the code.
    /// For line width use RC.LineWidth();
    /// </summary>
    public class WireframeCube : Mesh
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WireframeCube" /> class.
        /// The default cube is 1 unit big.
        /// </summary>
        public WireframeCube()
        {
            MeshType = (int)OpenGLPrimitiveType.LINES;
            Vertices = new float3[]
            {
                new float3(+0.5f, +0.5f, +0.5f),
                new float3(+0.5f, -0.5f, +0.5f),
                new float3(+0.5f, +0.5f, -0.5f),
                new float3(+0.5f, -0.5f, -0.5f),
                new float3(-0.5f, +0.5f, +0.5f),
                new float3(-0.5f, -0.5f, +0.5f),
                new float3(-0.5f, +0.5f, -0.5f),
                new float3(-0.5f, -0.5f, -0.5f)
            };
            Triangles = new ushort[] // these are our lines
            {
                0,4, // back
                4,5,
                5,1,
                1,0,

                0,2, // right, the back one can be discarded
                2,3,
                3,1,

                2,6, // front, the right one can be discarded
                6,7,
                7,3,

                6,4, // left, the rest
                7,5
            };
        }
    }

    /// <summary>
    /// Creates a simple cube geometry straight from the code.
    /// </summary>
    public class Cube : Mesh
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cube" /> class.
        /// The default cube is 1 unit big.
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