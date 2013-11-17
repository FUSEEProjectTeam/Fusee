using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletSharp;
using Fusee.Math;

namespace Examples.BulletSharp
{
    class Convert
    {

        public Convert()
        {
        }

        public float4x4 ConvertMatrixTof4X4(Matrix matrix)
        {
            var retval = new float4x4(matrix.M11, matrix.M12, matrix.M13, matrix.M14,
                                  matrix.M21, matrix.M22, matrix.M23, matrix.M24,
                                  matrix.M31, matrix.M32, matrix.M33, matrix.M34,
                                  matrix.M41, matrix.M42, matrix.M43, matrix.M44
                                  );
            return retval;
        }
    }
}
