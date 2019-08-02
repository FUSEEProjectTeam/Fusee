using System.Linq;
using Fusee.Math.Core;
using Fusee.Serialization;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Creates a plane geometry straight from the code that is divided in 9 tiles.
    /// This geometry is intended for displaying GUI Textures.
    /// </summary>
    public class NineSlicePlane : Mesh
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NineSlicePlane" /> class.
        /// The default NineSlicePlane is 1 unit big.
        /// </summary>
        public NineSlicePlane()
        {
            #region Fields

            Vertices = new[]
            {
                new float3 {x = -0.5f, y = -0.5f, z = 0},
                new float3 {x = -0.5f, y = -1/6f, z = 0},
                new float3 {x = -0.5f, y = 1/6f, z = 0},
                new float3 {x = -0.5f, y = +0.5f, z = 0},
                new float3 {x = -1/6f, y = 0.5f, z = 0},
                new float3 {x = 1/6f, y = 0.5f, z = 0},
                new float3 {x = +0.5f, y = +0.5f, z = 0},
                new float3 {x = 0.5f, y = 1/6f, z = 0},
                new float3 {x = 0.5f, y = -1/6f, z = 0},
                new float3 {x = +0.5f, y = -0.5f, z = 0},
                new float3 {x = 1/6f, y = -0.5f, z = 0},
                new float3 {x = -1/6f, y = -0.5f, z = 0},

                new float3 {x = -1/6f, y = -1/6f, z = 0},
                new float3 {x = -1/6f, y = 1/6f, z = 0},
                new float3 {x = 1/6f, y = 1/6f, z = 0},
                new float3 {x = 1/6f, y = -1/6f, z = 0},
            };


            Triangles = new ushort[]
            {
                0,12,1,
                0,11,12,

                1,13,2,
                1,12,13,

                2,4,3,
                2,13,4,

                11,15,12,
                11,10,15,

                12,14,13,
                12,15,14,

                13,5,4,
                13,14,5,

                10,8,15,
                10,9,8,

                15,7,14,
                15,8,7,

                14,6,5,
                14,7,6

                
            };

            Normals = new[]
            {
                new float3(0, 0, -1),
                new float3(0, 0, -1),
                new float3(0, 0, -1),
                new float3(0, 0, -1),
                new float3(0, 0, -1),
                new float3(0, 0, -1),
                new float3(0, 0, -1),
                new float3(0, 0, -1),
                new float3(0, 0, -1),
                new float3(0, 0, -1),
                new float3(0, 0, -1),
                new float3(0, 0, -1),
                new float3(0, 0, -1),
                new float3(0, 0, -1),
                new float3(0, 0, -1),
                new float3(0, 0, -1)
            };

            UVs = Vertices.Select(vert => new float2(vert.x, vert.y) + new float2(0.5f, 0.5f)).ToArray();
        }
        #endregion
    }
}

