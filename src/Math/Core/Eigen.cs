using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Math.Core
{
    /// <summary>
    ///     This class holds eigen structures
    /// </summary>
    public class Eigen
    {
        /// <summary>
        ///     Eigen data structure with values and vectors in single precision
        /// </summary>
        public struct EigenF
        {
            /// <summary>
            /// eigen values
            /// </summary>
            public float[] Values;
            /// <summary>
            /// eigen vectors
            /// </summary>
            public float4x4 Vectors;
        }

        /// <summary>
        ///     Eigen data structure with values and vectors in double precision
        /// </summary>
        public struct EigenD
        {
            /// <summary>
            /// eigen values
            /// </summary>
            public double[] Values;
            /// <summary>
            /// eigen vectors
            /// </summary>
            public double4x4 Vectors;
        }

    }
}
