using BulletSharp;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Desktop
{
    /// <summary>
    /// Static class containing helper methods to convert FUSEE math types from and to bullet-internal math types .
    /// </summary>
    public static class Translator
    {
        /// <summary>
        /// Bullet Vector3 to FUSEE float3 conversion.
        /// </summary>
        /// <param name="btVector">The bullet vector.</param>
        /// <returns>A FUSEE vector.</returns>
        public static float3 BtVector3ToFloat3(Vector3 btVector)
        {
            var retval = new float3(btVector.X, btVector.Y, btVector.Z);
            return retval;
        }

        /// <summary>
        /// FUSEE float3 to Bullet Vector3 conversion.
        /// </summary>
        /// <param name="float_3">The FUSEE float3.</param>
        /// <returns>A Bullet vector</returns>
        public static Vector3 Float3ToBtVector3(float3 float_3)
        {
            var retval = new Vector3(float_3.x, float_3.y, float_3.z);
            return retval;
        }

        /// <summary>
        /// Bullet Matrix to FUSEE float4x4 conversion.
        /// </summary>
        /// <param name="btMatrix">The Bullet matrix.</param>
        /// <returns>A FUSEE matrix.</returns>
        public static float4x4 BtMatrixToFloat4X4(Matrix btMatrix)
        {
            var retval = new float4x4(btMatrix.M11, btMatrix.M12, btMatrix.M13, btMatrix.M14,
                btMatrix.M21, btMatrix.M22, btMatrix.M23, btMatrix.M24,
                btMatrix.M31, btMatrix.M32, btMatrix.M33, btMatrix.M34,
                btMatrix.M41, btMatrix.M42, btMatrix.M43, btMatrix.M44
                );
            return retval;
        }

        /// <summary>
        /// FUSEE float4x4 to Bullet Matrix conversion.
        /// </summary>
        /// <param name="fuMatrix">The FUSEE matrix.</param>
        /// <returns>A Bullet matrix</returns>
        public static Matrix Float4X4ToBtMatrix(float4x4 fuMatrix)
        {
            var retval = new Matrix();
            retval.set_Rows(0, new Vector4(fuMatrix.M11, fuMatrix.M12, fuMatrix.M13, fuMatrix.M14));
            retval.set_Rows(1, new Vector4(fuMatrix.M21, fuMatrix.M22, fuMatrix.M23, fuMatrix.M24));
            retval.set_Rows(2, new Vector4(fuMatrix.M31, fuMatrix.M32, fuMatrix.M33, fuMatrix.M34));
            retval.set_Rows(3, new Vector4(fuMatrix.M41, fuMatrix.M42, fuMatrix.M43, fuMatrix.M44));
            return retval;
        }

        /// <summary>
        /// Quaternion to FUSEE Bullet conversion.
        /// </summary>
        /// <param name="quaternion">The Bullet quaternion.</param>
        /// <returns>
        /// A FUSEE quaternion.
        /// </returns>
        public static Math.Core.Quaternion BtQuaternionToQuaternion(BulletSharp.Quaternion quaternion)
        {
            var retval = new Math.Core.Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
            return retval;
        }

        /// <summary>
        /// FUSEE to Bullet Quaternion conversion.
        /// </summary>
        /// <param name="quaternion">The FUSEE quaternion.</param>
        /// <returns>A Bullet quaternion.</returns>
        public static BulletSharp.Quaternion QuaternionToBtQuaternion(Math.Core.Quaternion quaternion)
        {
            var retval = new BulletSharp.Quaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
            return retval;
        }
    }
}