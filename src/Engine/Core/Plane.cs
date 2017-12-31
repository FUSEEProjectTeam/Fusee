using Fusee.Math.Core;
using Fusee.Serialization;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Orientation options for a plane (facing direction).
    /// </summary>
    public enum Orientation
    {
        UP,
        FRONT,
        LEFT,
        RIGHT
    }

    /// <summary>
    /// Creates a simple plane geomentry straight from the code.
    /// </summary>
    public class Plane : Mesh
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Plane" /> class.
        /// The default plane is 1 unit big.
        /// </summary>
        public Plane(Orientation orientation)
        {
            var rotMat = float4x4.Identity;

            switch (orientation)
            {
                case Orientation.UP:
                    rotMat = float4x4.CreateRotationX(M.PiOver2);
                    break;
                case Orientation.LEFT:
                    rotMat = float4x4.CreateRotationY(M.PiOver2);
                    break;
                case Orientation.RIGHT:
                    rotMat = float4x4.CreateRotationY(M.Pi);
                    break;
            }

            #region Fields

            // TODO: Remove redundant vertices
            Vertices = new[]
            {
                rotMat* new float3 {x = -0.5f, y = -0.5f, z = 0},
                rotMat* new float3 {x = -0.5f, y = +0.5f, z = 0},
                rotMat* new float3 {x = +0.5f, y = +0.5f, z = 0},
                rotMat* new float3 {x = +0.5f, y = -0.5f, z = 0}
            };


            Triangles = new ushort[]
            {
                0, 2, 1, 0, 3, 2
            };

            Normals = new[]
            {
                rotMat * new float3(0, 0, -1),
                rotMat * new float3(0, 0, -1),
                rotMat * new float3(0, 0, -1),
                rotMat * new float3(0, 0, -1)
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



        public static MeshComponent CreatePlane(Orientation orientation)
        {
            var plane = new Plane(orientation);
            return new MeshComponent
            {
                Vertices = plane.Vertices,
                Triangles = plane.Triangles,
                UVs = plane.UVs,
                Normals = plane.Normals,
            };
        }
    }
}

