using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    public class Plane : Mesh
    {
        public Plane()
        {
            #region Fields

            // TODO: Remove redundant vertices
            Vertices = new[]
            {
                new float3 {x = -0.5f, y = -0.5f, z = 0},
                new float3 {x = -0.5f, y = +0.5f, z = 0},
                new float3 {x = +0.5f, y = +0.5f, z = 0},
                new float3 {x = +0.5f, y = -0.5f, z = 0}
            };

            Triangles = new ushort[]
            {
                0, 2, 1, 0, 3, 2
            };

            Normals = new[]
            {
                new float3(0, 0, -1),
                new float3(0, 0, -1),
                new float3(0, 0, -1),
                new float3(0, 0, -1)
            };

            UVs = new[]
            {
                new float2(1, 0),
                new float2(1, 1),
                new float2(0, 1),
                new float2(0, 0)
            };
        }
        #endregion
    }
}

