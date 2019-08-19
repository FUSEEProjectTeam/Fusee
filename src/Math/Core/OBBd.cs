using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

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
        ///     The roation of the oriented bounding box
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
        public double3 Center => (Max + Min) * 0.5f;

        /// <summary>
        ///     Returns the rotation as euler angles
        /// </summary>
        public double3 EulerRotation => double4x4.RotMatToEuler(Rotation);

        /// <summary>
        ///     Create a new axis aligned bounding box
        /// </summary>
        /// <param name="min_">the minimum x y and z values</param>
        /// <param name="max_">the maximum x y and z values</param>
        /// <param name="rotation_">the rotation of this box</param>
        /// <param name="translation_">the translation of this box</param>
        /// <param name="translation_">the size of this box</param>
        public OBBd(double3 min_, double3 max_, double4x4 rotation_, double3 translation_, double3 size_)
        {
            Min = min_;
            Max = max_;
            Size = size_;
            Rotation = rotation_;
            Translation = translation_;
        }

        /// <summary>
        ///     Generates a new  oriented bounding box from a given set of vertices or points
        /// </summary>
        /// <param name="vertices"></param>
        public OBBd(double3[] vertices)
        {
            Translation = M.CalculateCentroid(vertices);
            var covarianceMatrix = M.CreateCovarianceMatrix(Translation, vertices);
            var eigen = M.EigenFromCovarianceMat(covarianceMatrix);

            Rotation = eigen.Vectors;

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

            // Get size of box before translating back
            Size = Max - Min;

            Max = (Rotation * Max) + Translation;
            Min = (Rotation * Min) + Translation;
        }
    }
}
