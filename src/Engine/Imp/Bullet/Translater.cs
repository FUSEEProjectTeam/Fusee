using BulletSharp;
using Fusee.Math;

namespace Fusee.Engine
{
    public class Translater
    {
        public float3 BtVector3ToFloat3(Vector3 btVector)
        {
            var retval = new float3(btVector.X, btVector.Y, btVector.Z);
            return retval;
        }

        public Vector3 Float3ToBtVector3(float3 float_3)
        {
            var retval = new Vector3(float_3.x, float_3.y, float_3.z);
            return retval;
        }

        public float4x4 BtMatrixToFloat4X4(Matrix btMatrix)
        {
            var retval = new float4x4(btMatrix.M11, btMatrix.M12, btMatrix.M13, btMatrix.M14,
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

        public Fusee.Math.Quaternion BtQuaternionToQuaternion(BulletSharp.Quaternion btQuaternion)
        {
            var retval = new Fusee.Math.Quaternion(btQuaternion.X, btQuaternion.Y, btQuaternion.Z, btQuaternion.W);
            return retval;
        }

        public BulletSharp.Quaternion QuaternionToBtQuaternion(Fusee.Math.Quaternion quaternion)
        {
            var retval = new BulletSharp.Quaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
            return retval;
        }
    }
}