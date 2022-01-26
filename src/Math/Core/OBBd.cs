using ProtoBuf;
using System.Linq;
using System.Runtime.InteropServices;

namespace Fusee.Math.Core
{
    /// <summary>
    ///     Represents an oriented bounding box.
    /// </summary>
    [ProtoContract]
    [StructLayout(LayoutKind.Sequential)]
    public struct OBBd
    {
        /// <summary>
        ///     The minimum values of the oriented bounding box in x, y and z direction
        /// </summary>
        [ProtoMember(1)] public double3 Min;

        /// <summary>
        ///     The maximum values of the oriented bounding box in x, y and z direction
        /// </summary>
        [ProtoMember(2)] public double3 Max;

        /// <summary>
        ///     The rotation of the oriented bounding box
        /// </summary>
        [ProtoMember(3)] public double4x4 Rotation;

        /// <summary>
        ///     The translation of the oriented bounding box
        /// </summary>
        [ProtoMember(4)] public double3 Translation;

        /// <summary>
        ///     Returns the with, height and depth of the box in x, y and z
        /// </summary>
        [ProtoMember(5)] public double3 Size;

        /// <summary>
        ///     Returns the center of the bounding box
        /// </summary>
        public double3 Center => (Max + Min) * 0.5;

        /// <summary>
        ///     Returns the rotation as euler angles
        /// </summary>
        public double3 EulerRotation => double4x4.RotMatToEuler(Rotation);

        /// <summary>
        ///     Generates a new  oriented bounding box from a given set of vertices or points
        /// </summary>
        /// <param name="vertices"></param>
        public OBBd(double3[] vertices)
        {
            var verticesList = vertices.ToList();
            if (verticesList.Any(pt => pt.IsInfinity) || verticesList.Any(pt => pt.IsNaN))
            {
                Max = double3.PositiveInfinity;
                Min = double3.NegativeInfinity;
                Rotation = double4x4.Identity;
                Translation = double3.Zero;
                Size = double3.Zero;
                return;
            }

            Translation = M.CalculateCentroid(vertices);
            var eigen = new Eigen(vertices);

            Rotation = eigen.RotationMatrix;

            var changeBasis = Rotation.Invert();

            Min = double3.One * double.MaxValue;
            Max = double3.One * double.MinValue;
            Size = double3.One;

            for (var i = 0; i < vertices.Length; i++)
            {
                // translate every point in first quadrant of [0, 0, 0] coordinate system
                var currentPointTranslated = vertices[i] - Translation;
                var currentPointTranslatedAndRotated = changeBasis * currentPointTranslated;

                var pt = currentPointTranslatedAndRotated;

                // check min and max points
                Min = new double3(
                    Min.x > pt.x ? pt.x : Min.x,
                    Min.y > pt.y ? pt.y : Min.y,
                    Min.z > pt.z ? pt.z : Min.z);

                Max = new double3(
                   Max.x < pt.x ? pt.x : Max.x,
                   Max.y < pt.y ? pt.y : Max.y,
                   Max.z < pt.z ? pt.z : Max.z);
            }

            Max = (Rotation * Max) + Translation;
            Min = (Rotation * Min) + Translation;

            Size = Max - Min;
        }
    }
}