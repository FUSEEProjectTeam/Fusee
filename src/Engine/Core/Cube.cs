using Fusee.Math;

namespace Fusee.Engine
{
    /// <summary>
    /// Creates a simple cube geomentry straight from the code.
    /// </summary>
    public class Cube : Mesh
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cube"/> class.
        /// </summary>
        public Cube()
        {
            Vertices = new float3[]
            {
                new float3{x=-0.5f, y=-0.5f, z= 0.5f},
                new float3{x= 0.5f, y=-0.5f, z= 0.5f},
                new float3{x= 0.5f, y= 0.5f, z= 0.5f},
                new float3{x=-0.5f, y= 0.5f, z= 0.5f},
                new float3{x=-0.5f, y=-0.5f, z=-0.5f},
                new float3{x= 0.5f, y=-0.5f, z=-0.5f}, 
                new float3{x= 0.5f, y= 0.5f, z=-0.5f},
                new float3{x=-0.5f, y= 0.5f, z=-0.5f},
            };

            Triangles = new short[]
            {
                // front face
                0, 1, 2, 2, 3, 0,
                // top face
                3, 2, 6, 6, 7, 3,
                // back face
                7, 6, 5, 5, 4, 7,
                // left face
                4, 0, 3, 3, 7, 4,
                // bottom face
                0, 1, 5, 5, 4, 0,
                // right face
                1, 5, 6, 6, 2, 1,
            };

            Normals = new float3[]
            {
                new float3{x=-1.0f, y=-1.0f, z= 1.0f},
                new float3{x= 1.0f, y=-1.0f, z= 1.0f},
                new float3{x= 1.0f, y= 1.0f, z= 1.0f},
                new float3{x=-1.0f, y= 1.0f, z= 1.0f},
                new float3{x=-1.0f, y=-1.0f, z=-1.0f},
                new float3{x= 1.0f, y=-1.0f, z=-1.0f},
                new float3{x= 1.0f, y= 1.0f, z=-1.0f},
                new float3{x=-1.0f, y= 1.0f, z=-1.0f},
            };

            Colors = new uint[]
            {
                0x7F0000FF,
                0x7F0000FF,
                0x7F00FFFF,
                0x7F00FFFF,
                0x7F0000FF,
                0x7F0000FF,
                0x7F00FFFF,
                0x7F00FFFF,
            };
        }
    }
}
