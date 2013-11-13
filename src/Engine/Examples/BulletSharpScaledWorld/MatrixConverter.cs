using System;
using Fusee.Engine;
using Fusee.Math;
using BulletSharp;


namespace Examples.BulletSharpScaledWorld
{
    class MatrixConverter
    {
        public MatrixConverter()
        { 
        }

        public float4x4 BtMAtrixToF3dMatrix(CollisionObject co)
        {
            var returnValue = new float4x4(co.WorldTransform.M11, co.WorldTransform.M12, co.WorldTransform.M13, co.WorldTransform.M14,
                                           co.WorldTransform.M21, co.WorldTransform.M22, co.WorldTransform.M23, co.WorldTransform.M24,
                                           co.WorldTransform.M31, co.WorldTransform.M32, co.WorldTransform.M33, co.WorldTransform.M34,
                                           co.WorldTransform.M41, co.WorldTransform.M42, co.WorldTransform.M43, co.WorldTransform.M44);
            
            return returnValue;
        }
    }
}
