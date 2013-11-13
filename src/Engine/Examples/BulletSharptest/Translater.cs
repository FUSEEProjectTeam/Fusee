using System;
using System.Diagnostics;
using BulletSharp;
using Fusee.Math;

namespace Examples.BulletSharp
{
    class Translater
    {
        public Translater()
        {
            Debug.WriteLine("new Translater");
        }

        public float4x4 BtMatrixToFsMatrix(RigidBody rb)
        {
            float4x4 resultMatrix = new float4x4(rb.WorldTransform.M11, rb.WorldTransform.M12, rb.WorldTransform.M13,rb.WorldTransform.M14,
                                                  rb.WorldTransform.M21, rb.WorldTransform.M22, rb.WorldTransform.M23,rb.WorldTransform.M24,
                                                  rb.WorldTransform.M31, rb.WorldTransform.M32, rb.WorldTransform.M33,rb.WorldTransform.M34,
                                                  rb.WorldTransform.M41, rb.WorldTransform.M42, rb.WorldTransform.M44,1);
            return resultMatrix;
        }

        //TODO: MEthoden ide automatisch zwischen Physik und OpenTk Matriten übersetzenund updaten...

    }
}
