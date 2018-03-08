using Fusee.Math.Core;
using Fusee.Serialization;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Creates a simple plane geomentry straight from the code.
    /// </summary>
    public class Plane : Mesh
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Plane" /> class.
        /// The default plane is 1 unit big.
        /// </summary>
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
                new float2(0, 0),
                new float2(0, 1),
                new float2(1, 1),
                new float2(1, 0),
                
            };
        }
        #endregion
    }
}

