using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Xunit;

namespace Fusee.Tests.Scene.Components
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
    }
}