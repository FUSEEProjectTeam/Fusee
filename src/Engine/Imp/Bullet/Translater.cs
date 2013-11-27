using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Fusee.Math;
using BulletSharp;

namespace Fusee.Engine
{
    public class Translater
    {

        public float4x4 BtMatrixToFloat4X4(Matrix btMatrix)
        {
            var retval = new float4x4 (btMatrix.M11, btMatrix.M12, btMatrix.M13, btMatrix.M14,
                                       btMatrix.M21, btMatrix.M22, btMatrix.M23, btMatrix.M24,
                                       btMatrix.M31, btMatrix.M32, btMatrix.M33, btMatrix.M34,
                                       btMatrix.M41, btMatrix.M42, btMatrix.M43, btMatrix.M44
                                       );
            return retval;
        }

        public Matrix Float4X4ToBtMatrix(float4x4 float4X4)
        {
            var retval = new Matrix();
            retval.set_Rows(0, new Vector4(float4X4.M11, float4X4.M12, float4X4.M13, float4X4.M14));
            retval.set_Rows(1, new Vector4(float4X4.M21, float4X4.M22, float4X4.M23, float4X4.M24));
            retval.set_Rows(2, new Vector4(float4X4.M31, float4X4.M32, float4X4.M33, float4X4.M34));
            retval.set_Rows(3, new Vector4(float4X4.M41, float4X4.M42, float4X4.M43, float4X4.M44));
            return retval;
        }
    }
}
