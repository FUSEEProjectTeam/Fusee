using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Fusee.Test.Scene.Components
{
    public class TransformTest
    {
        [Fact]
        public void MatrixToComponentMatrices()
        {
            var t = new Transform();

            // Set matrix, check components to be the correct value
            t.Matrix = float4x4.CreateTranslation(1, 2, 3) * float4x4.CreateRotationZXY(3, 2, 1) * float4x4.Scale(2);
            Assert.Equal(float4x4.CreateTranslation(1, 2, 3), t.TranslationMatrix);
            Assert.Equal(float4x4.CreateRotationZXY(3, 2, 1), t.RotationMatrix);
            Assert.Equal(float4x4.Scale(2), t.ScaleMatrix);

            // Set matrix to identity, check components to be identity
            t.Matrix = float4x4.Identity;
            Assert.Equal(float4x4.Identity, t.TranslationMatrix);
            Assert.Equal(float4x4.Identity, t.RotationMatrix);
            Assert.Equal(float4x4.Identity, t.ScaleMatrix);
        }

        [Fact]
        public void MatrixToTranslationMatrix()
        {
            var t = new Transform();

            t.Matrix = float4x4.CreateTranslation(1, 2, 3);
            Assert.Equal(float4x4.CreateTranslation(1, 2, 3), t.TranslationMatrix);
            Assert.Equal(float4x4.Identity, t.RotationMatrix);
            Assert.Equal(float4x4.Identity, t.ScaleMatrix);
        }

        [Fact]
        public void MatrixToRotationMatrix()
        {
            var t = new Transform();

            t.Matrix = float4x4.CreateRotationZXY(3, 2, 1);
            Assert.Equal(float4x4.Identity, t.TranslationMatrix);
            Assert.Equal(float4x4.CreateRotationZXY(3, 2, 1), t.RotationMatrix);
            Assert.Equal(float4x4.Identity, t.ScaleMatrix);
        }

        [Fact]
        public void MatrixToScaleMatrix()
        {
            var t = new Transform();

            t.Matrix = float4x4.Scale(2);
            Assert.Equal(float4x4.Identity, t.TranslationMatrix);
            Assert.Equal(float4x4.Identity, t.RotationMatrix);
            Assert.Equal(float4x4.Scale(2), t.ScaleMatrix);
        }

        [Fact]
        public void TranslationMatrixToMatrix()
        {
            var t = new Transform();

            t.TranslationMatrix = float4x4.CreateTranslation(1, 2, 3);
            Assert.Equal(float4x4.CreateTranslation(1, 2, 3), t.Matrix);
        }

        [Fact]
        public void RotationMatrixToMatrix()
        {
            var t = new Transform();

            t.RotationMatrix = float4x4.CreateRotationZXY(3, 2, 1);
            Assert.Equal(float4x4.CreateRotationZXY(3, 2, 1), t.Matrix);
        }

        [Fact]
        public void ScaleMatrixToMatrix()
        {
            var t = new Transform();

            t.ScaleMatrix = float4x4.CreateScale(2);
            Assert.Equal(float4x4.CreateScale(2), t.Matrix);
        }

        [Fact]
        public void TRSMatrixToMatrix()
        {
            var t = new Transform();

            t.TranslationMatrix = float4x4.CreateTranslation(1, 2, 3);
            t.RotationMatrix = float4x4.CreateRotationZXY(3, 2, 1);
            t.ScaleMatrix = float4x4.CreateScale(2);
            Assert.Equal(float4x4.CreateTranslation(1, 2, 3) * float4x4.CreateRotationZXY(3, 2, 1) * float4x4.Scale(2), t.Matrix);
        }

        [Fact]
        public void TranslationVectorToTranslationMatrix()
        {
            var t = new Transform();

            t.TranslationVector = new float3(1, 2, 3);
            Assert.Equal(float4x4.CreateTranslation(1, 2, 3), t.TranslationMatrix);
        }

        [Fact]
        public void RotationEulerToRoationMatrix()
        {
            var t = new Transform();

            t.RotationEuler = new float3(3, 2, 1);
            Assert.Equal(float4x4.CreateRotationZXY(3, 2, 1), t.RotationMatrix);
        }

        [Fact]
        public void ScaleVectorToScaleMatrix()
        {
            var t = new Transform();

            t.ScaleVector = new float3(2, 2, 2);
            Assert.Equal(float4x4.CreateScale(2), t.ScaleMatrix);
        }

        [Fact]
        public void TranslationMatrixToTranslationVector()
        {
            var t = new Transform();

            t.TranslationMatrix = float4x4.CreateTranslation(1, 2, 3);
            Assert.Equal(new float3(1, 2, 3), t.TranslationVector);
        }

        [Fact]
        public void RotationMatrixToRotationEuler()
        {
            var t = new Transform();

            t.RotationMatrix = float4x4.CreateRotationZXY(3, 2, 1);
            Assert.Equal(new float3(3, 2, 1), t.RotationEuler);
        }

        [Fact]
        public void ScaleMatrixtoScaleVector()
        {
            var t = new Transform();

            t.ScaleMatrix = float4x4.CreateScale(2);
            Assert.Equal(new float3(2, 2, 2), t.ScaleVector);
        }

        [Fact]
        public void RotationQuaternionToRotationMatrix()
        {
            var t = new Transform();

            t.RotationQuaternion = Quaternion.EulerToQuaternion(new float3(3, 2, 1));
            Assert.Equal(float4x4.CreateRotationZXY(3, 2, 1), t.RotationMatrix);
        }

        [Fact]
        public void RotationQuaternionToRotaionEuler()
        {
            var t = new Transform();

            t.RotationQuaternion = Quaternion.EulerToQuaternion(new float3(3, 2, 1));
            Assert.Equal(new float3(3, 2, 1), t.RotationEuler);
        }

        [Fact]
        public void RotationMatrixToRotationQuaternion()
        {
            var t = new Transform();

            t.RotationMatrix = float4x4.CreateRotationZXY(3, 2, 1);
            Assert.Equal(Quaternion.EulerToQuaternion(new float3(3, 2, 1)), t.RotationQuaternion);
        }

        [Fact]
        public void RotationEulerToRotationQuaternion()
        {
            var t = new Transform();

            t.RotationEuler = new float3(3, 2, 1);
            Assert.Equal(Quaternion.EulerToQuaternion(new float3(3, 2, 1)), t.RotationQuaternion);
        }


        public static IEnumerable<object[]> GetEQM()
        {
            // 1. - X: 45, Y: 0, Z: 0
            yield return new object[]
            {
new float3(0.7853982f, 0f, 0f),
new Quaternion(0.3826835f, 0f, 0f, 0.9238795f),
new float4x4(1f, 0f, 0f, 0f, 0f, 0.7071067f, -0.7071068f, 0f, 0f, 0.7071068f, 0.7071067f, 0f, 0f, 0f, 0f, 1f)
            };
            // 2. - X: 90, Y: 0, Z: 0
            yield return new object[]
            {
new float3(1.570796f, 0f, 0f),
new Quaternion(0.7071068f, 0f, 0f, 0.7071068f),
new float4x4(1f, 0f, 0f, 0f, 0f, 5.960464E-08f, -0.9999999f, 0f, 0f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f)
            };
            // 3. - X: 135, Y: 0, Z: 0
            yield return new object[]
            {
new float3(2.356194f, 0f, 0f),
new Quaternion(0.9238795f, 0f, 0f, 0.3826834f),
new float4x4(1f, 0f, 0f, 0f, 0f, -0.7071067f, -0.7071068f, 0f, 0f, 0.7071068f, -0.7071067f, 0f, 0f, 0f, 0f, 1f)
            };
            // 4. - X: 180, Y: 0, Z: 0
            yield return new object[]
            {
new float3(3.141593f, 0f, 0f),
new Quaternion(1f, 0f, 0f, -4.371139E-08f),
new float4x4(1f, 0f, 0f, 0f, 0f, -1f, 8.742278E-08f, 0f, 0f, -8.742278E-08f, -1f, 0f, 0f, 0f, 0f, 1f)
            };
            // 5. - X: 360, Y: 0, Z: 0
            yield return new object[]
            {
new float3(6.283185f, 0f, 0f),
new Quaternion(-8.742278E-08f, 0f, 0f, -1f),
new float4x4(1f, 0f, 0f, 0f, 0f, 1f, -1.748456E-07f, 0f, 0f, 1.748456E-07f, 1f, 0f, 0f, 0f, 0f, 1f)
            };
            // 6. - X: 540, Y: 0, Z: 0
            yield return new object[]
            {
new float3(9.424778f, 0f, 0f),
new Quaternion(-1f, 0f, 0f, 1.192488E-08f),
new float4x4(1f, 0f, 0f, 0f, 0f, -1f, 2.384976E-08f, 0f, 0f, -2.384976E-08f, -1f, 0f, 0f, 0f, 0f, 1f)
            };
            // 7. - X: 0, Y: 45, Z: 0
            yield return new object[]
            {
new float3(0f, 0.7853982f, 0f),
new Quaternion(0f, 0.3826835f, 0f, 0.9238795f),
new float4x4(0.7071067f, 0f, 0.7071068f, 0f, 0f, 1f, 0f, 0f, -0.7071068f, 0f, 0.7071067f, 0f, 0f, 0f, 0f, 1f)
            };
            // 8. - X: 0, Y: 90, Z: 0
            yield return new object[]
            {
new float3(0f, 1.570796f, 0f),
new Quaternion(0f, 0.7071068f, 0f, 0.7071068f),
new float4x4(5.960464E-08f, 0f, 0.9999999f, 0f, 0f, 1f, 0f, 0f, -0.9999999f, 0f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f)
            };
            // 9. - X: 0, Y: 135, Z: 0
            yield return new object[]
            {
new float3(0f, 2.356194f, 0f),
new Quaternion(0f, 0.9238795f, 0f, 0.3826834f),
new float4x4(-0.7071067f, 0f, 0.7071068f, 0f, 0f, 1f, 0f, 0f, -0.7071068f, 0f, -0.7071067f, 0f, 0f, 0f, 0f, 1f)
            };
            // 10. - X: 0, Y: 180, Z: 0
            yield return new object[]
            {
new float3(0f, 3.141593f, 0f),
new Quaternion(0f, 1f, 0f, -4.371139E-08f),
new float4x4(-1f, 0f, -8.742278E-08f, 0f, 0f, 1f, 0f, 0f, 8.742278E-08f, 0f, -1f, 0f, 0f, 0f, 0f, 1f)
            };
            // 11. - X: 0, Y: 360, Z: 0
            yield return new object[]
            {
new float3(0f, 6.283185f, 0f),
new Quaternion(0f, -8.742278E-08f, 0f, -1f),
new float4x4(1f, 0f, 1.748456E-07f, 0f, 0f, 1f, 0f, 0f, -1.748456E-07f, 0f, 1f, 0f, 0f, 0f, 0f, 1f)
            };
            // 12. - X: 0, Y: 540, Z: 0
            yield return new object[]
            {
new float3(0f, 9.424778f, 0f),
new Quaternion(0f, -1f, 0f, 1.192488E-08f),
new float4x4(-1f, 0f, -2.384976E-08f, 0f, 0f, 1f, 0f, 0f, 2.384976E-08f, 0f, -1f, 0f, 0f, 0f, 0f, 1f)
            };
            // 13. - X: 0, Y: 0, Z: 45
            yield return new object[]
            {
new float3(0f, 0f, 0.7853982f),
new Quaternion(0f, 0f, 0.3826835f, 0.9238795f),
new float4x4(0.7071067f, -0.7071068f, 0f, 0f, 0.7071068f, 0.7071067f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f)
            };
            // 14. - X: 0, Y: 0, Z: 90
            yield return new object[]
            {
new float3(0f, 0f, 1.570796f),
new Quaternion(0f, 0f, 0.7071068f, 0.7071068f),
new float4x4(5.960464E-08f, -0.9999999f, 0f, 0f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f)
            };
            // 15. - X: 0, Y: 0, Z: 135
            yield return new object[]
            {
new float3(0f, 0f, 2.356194f),
new Quaternion(0f, 0f, 0.9238795f, 0.3826834f),
new float4x4(-0.7071067f, -0.7071068f, 0f, 0f, 0.7071068f, -0.7071067f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f)
            };
            // 16. - X: 0, Y: 0, Z: 180
            yield return new object[]
            {
new float3(0f, 0f, 3.141593f),
new Quaternion(0f, 0f, 1f, -4.371139E-08f),
new float4x4(-1f, 8.742278E-08f, 0f, 0f, -8.742278E-08f, -1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f)
            };
            // 17. - X: 0, Y: 0, Z: 360
            yield return new object[]
            {
new float3(0f, 0f, 6.283185f),
new Quaternion(0f, 0f, -8.742278E-08f, -1f),
new float4x4(1f, -1.748456E-07f, 0f, 0f, 1.748456E-07f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f)
            };
            // 18. - X: 0, Y: 0, Z: 540
            yield return new object[]
            {
new float3(0f, 0f, 9.424778f),
new Quaternion(0f, 0f, -1f, 1.192488E-08f),
new float4x4(-1f, 2.384976E-08f, 0f, 0f, -2.384976E-08f, -1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f)
            };
            // 19. - X: 0, Y: 0, Z: 0
            yield return new object[]
            {
new float3(0f, 0f, 0f),
new Quaternion(0f, 0f, 0f, 1f),
new float4x4(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f)
            };
        }
    }
}