using Fusee.Engine.Core;
using Fusee.Math.Core;

namespace Fusee.Tutorial.Core
{
    public class Cube
    {
        public Cube()
        {
            
        }

        public Mesh BuildComp()
        {

            Mesh arm = new Mesh
            {
                Vertices = new[]
                {
                    // left, down, front vertex
                    new float3(-1, -1, -1), // 0  - belongs to left
                    new float3(-1, -1, -1), // 1  - belongs to down
                    new float3(-1, -1, -1), // 2  - belongs to front

                    // left, down, back vertex
                    new float3(-1, -1, 1), // 3  - belongs to left
                    new float3(-1, -1, 1), // 4  - belongs to down
                    new float3(-1, -1, 1), // 5  - belongs to back

                    // left, up, front vertex
                    new float3(-1, 1, -1), // 6  - belongs to left
                    new float3(-1, 1, -1), // 7  - belongs to up
                    new float3(-1, 1, -1), // 8  - belongs to front

                    // left, up, back vertex
                    new float3(-1, 1, 1), // 9  - belongs to left
                    new float3(-1, 1, 1), // 10 - belongs to up
                    new float3(-1, 1, 1), // 11 - belongs to back

                    // right, down, front vertex
                    new float3(1, -1, -1), // 12 - belongs to right
                    new float3(1, -1, -1), // 13 - belongs to down
                    new float3(1, -1, -1), // 14 - belongs to front

                    // right, down, back vertex
                    new float3(1, -1, 1), // 15 - belongs to right
                    new float3(1, -1, 1), // 16 - belongs to down
                    new float3(1, -1, 1), // 17 - belongs to back

                    // right, up, front vertex
                    new float3(1, 1, -1), // 18 - belongs to right
                    new float3(1, 1, -1), // 19 - belongs to up
                    new float3(1, 1, -1), // 20 - belongs to front

                    // right, up, back vertex
                    new float3(1, 1, 1), // 21 - belongs to right
                    new float3(1, 1, 1), // 22 - belongs to up
                    new float3(1, 1, 1), // 23 - belongs to back

                },
                Normals = new[]
                {
                    // left, down, front vertex
                    new float3(-1, 0, 0), // 0  - belongs to left
                    new float3(0, -1, 0), // 1  - belongs to down
                    new float3(0, 0, -1), // 2  - belongs to front

                    // left, down, back vertex
                    new float3(-1, 0, 0), // 3  - belongs to left
                    new float3(0, -1, 0), // 4  - belongs to down
                    new float3(0, 0, 1), // 5  - belongs to back

                    // left, up, front vertex
                    new float3(-1, 0, 0), // 6  - belongs to left
                    new float3(0, 1, 0), // 7  - belongs to up
                    new float3(0, 0, -1), // 8  - belongs to front

                    // left, up, back vertex
                    new float3(-1, 0, 0), // 9  - belongs to left
                    new float3(0, 1, 0), // 10 - belongs to up
                    new float3(0, 0, 1), // 11 - belongs to back

                    // right, down, front vertex
                    new float3(1, 0, 0), // 12 - belongs to right
                    new float3(0, -1, 0), // 13 - belongs to down
                    new float3(0, 0, -1), // 14 - belongs to front

                    // right, down, back vertex
                    new float3(1, 0, 0), // 15 - belongs to right
                    new float3(0, -1, 0), // 16 - belongs to down
                    new float3(0, 0, 1), // 17 - belongs to back

                    // right, up, front vertex
                    new float3(1, 0, 0), // 18 - belongs to right
                    new float3(0, 1, 0), // 19 - belongs to up
                    new float3(0, 0, -1), // 20 - belongs to front

                    // right, up, back vertex
                    new float3(1, 0, 0), // 21 - belongs to right
                    new float3(0, 1, 0), // 22 - belongs to up
                    new float3(0, 0, 1), // 23 - belongs to back
                },
                Triangles = new ushort[]
                {
                    0, 6, 3, 3, 6, 9, // left
                    2, 14, 20, 2, 20, 8, // front
                    12, 15, 18, 15, 21, 18, // right
                    5, 11, 17, 17, 11, 23, // back
                    7, 22, 10, 7, 19, 22, // top
                    1, 4, 16, 1, 16, 13, // bottom 
                },
            };

            return arm;
        }
    }
}