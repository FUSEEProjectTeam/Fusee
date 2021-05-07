using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using Xunit;

namespace Fusee.Test.Scene.Components
{
    public class TransformTest
    {
        [Fact]
        public void MatrixToComponentMatrices()
        {
            var t = new Transform();

            t.Matrix = float4x4.Identity;
            Assert.Equal(float4x4.Identity, t.TranslationMatrix);
            Assert.Equal(float4x4.Identity, t.RotationMatrix);
            Assert.Equal(float4x4.Identity, t.ScaleMatrix);

            t.Matrix = float4x4.CreateTranslation(1, 2, 3) * float4x4.CreateRotationZXY(3, 2, 1) * float4x4.Scale(2);
            Assert.Equal(float4x4.CreateTranslation(1, 2, 3), t.TranslationMatrix);
            Assert.Equal(float4x4.CreateRotationZXY(3, 2, 1), t.RotationMatrix);
            Assert.Equal(float4x4.Scale(2), t.ScaleMatrix);
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

        [Theory]
        [MemberData(nameof(GetEQM))]
        public void RotationConversionEulerToQuaternion(float3 euler, Quaternion quaternion, float4x4 matrix)
        {
            var comparisonfactor = 1E-05;

            var valid = false;

            var fq = Quaternion.EulerToQuaternion(euler);
            var uq = quaternion;

            if (MathF.Abs(fq.x - uq.x) < comparisonfactor &&
                MathF.Abs(fq.y - uq.y) < comparisonfactor &&
                MathF.Abs(fq.z - uq.z) < comparisonfactor &&
                MathF.Abs(fq.w - uq.w) < comparisonfactor)
                valid = true;

            Assert.True(valid);
        }

        [Theory]
        [MemberData(nameof(GetEQM))]
        public void RotationConversionQuaternionToEuler(float3 euler, Quaternion quaternion, float4x4 matrix)
        {
            var comparisonfactor = 1E-05;

            var valid = false;

            var fe = Quaternion.QuaternionToEuler(quaternion);
            var ue = euler;

            if (MathF.Abs(fe.x - ue.x) < comparisonfactor &&
                MathF.Abs(fe.y - ue.y) < comparisonfactor &&
                MathF.Abs(fe.z - ue.z) < comparisonfactor)
                valid = true;

            Assert.True(valid);
        }

        [Theory]
        [MemberData(nameof(GetEQM))]
        public void RotationConversionEulerToMatrix(float3 euler, Quaternion quaternion, float4x4 matrix)
        {
            var comparisonfactor = 1E-06;

            var valid = false;

            var fm = float4x4.CreateRotationZXY(euler);
            var um = matrix;

            if (MathF.Abs(fm.M11 - um.M11) < comparisonfactor &&
                MathF.Abs(fm.M12 - um.M12) < comparisonfactor &&
                MathF.Abs(fm.M13 - um.M13) < comparisonfactor &&
                MathF.Abs(fm.M14 - um.M14) < comparisonfactor &&
                MathF.Abs(fm.M21 - um.M21) < comparisonfactor &&
                MathF.Abs(fm.M22 - um.M22) < comparisonfactor &&
                MathF.Abs(fm.M23 - um.M23) < comparisonfactor &&
                MathF.Abs(fm.M24 - um.M24) < comparisonfactor &&
                MathF.Abs(fm.M31 - um.M31) < comparisonfactor &&
                MathF.Abs(fm.M32 - um.M32) < comparisonfactor &&
                MathF.Abs(fm.M33 - um.M33) < comparisonfactor &&
                MathF.Abs(fm.M34 - um.M34) < comparisonfactor &&
                MathF.Abs(fm.M41 - um.M41) < comparisonfactor &&
                MathF.Abs(fm.M42 - um.M42) < comparisonfactor &&
                MathF.Abs(fm.M43 - um.M43) < comparisonfactor &&
                MathF.Abs(fm.M44 - um.M44) < comparisonfactor)
                valid = true;

            Assert.True(valid);
        }

        [Theory]
        [MemberData(nameof(GetEQM))]
        public void RotationConversionMatrixToEuler(float3 euler, Quaternion quaternion, float4x4 matrix)
        {
            var comparisonfactor = 1E-06;

            var valid = false;

            var fe = float4x4.RotMatToEuler(matrix);
            var ue = euler;

            if (MathF.Abs(fe.x - ue.x) < comparisonfactor &&
                MathF.Abs(fe.y - ue.y) < comparisonfactor &&
                MathF.Abs(fe.z - ue.z) < comparisonfactor)
                valid = true;

            Assert.True(valid);
        }


        [Theory]
        [MemberData(nameof(GetEQM))]
        public void RotationConversionQuaternionToMatrix(float3 euler, Quaternion quaternion, float4x4 matrix)
        {
            var comparisonfactor = 1E-06;

            var valid = false;

            var fm = Quaternion.ToRotMat(quaternion);
            var um = matrix;

            if (MathF.Abs(fm.M11 - um.M11) < comparisonfactor &&
                MathF.Abs(fm.M12 - um.M12) < comparisonfactor &&
                MathF.Abs(fm.M13 - um.M13) < comparisonfactor &&
                MathF.Abs(fm.M14 - um.M14) < comparisonfactor &&
                MathF.Abs(fm.M21 - um.M21) < comparisonfactor &&
                MathF.Abs(fm.M22 - um.M22) < comparisonfactor &&
                MathF.Abs(fm.M23 - um.M23) < comparisonfactor &&
                MathF.Abs(fm.M24 - um.M24) < comparisonfactor &&
                MathF.Abs(fm.M31 - um.M31) < comparisonfactor &&
                MathF.Abs(fm.M32 - um.M32) < comparisonfactor &&
                MathF.Abs(fm.M33 - um.M33) < comparisonfactor &&
                MathF.Abs(fm.M34 - um.M34) < comparisonfactor &&
                MathF.Abs(fm.M41 - um.M41) < comparisonfactor &&
                MathF.Abs(fm.M42 - um.M42) < comparisonfactor &&
                MathF.Abs(fm.M43 - um.M43) < comparisonfactor &&
                MathF.Abs(fm.M44 - um.M44) < comparisonfactor)
                valid = true;

            Assert.True(valid);
        }

        [Theory]
        [MemberData(nameof(GetEQM))]
        public void RotationConversionMatrixToQuaternion(float3 euler, Quaternion quaternion, float4x4 matrix)
        {
            var comparisonfactor = 1E-06;

            var valid = false;

            var fq = Quaternion.FromRotationMatrix(matrix);
            var uq = quaternion;

            if (MathF.Abs(fq.x - uq.x) < comparisonfactor &&
                MathF.Abs(fq.y - uq.y) < comparisonfactor &&
                MathF.Abs(fq.z - uq.z) < comparisonfactor &&
                MathF.Abs(fq.w - uq.w) < comparisonfactor)
                valid = true;

            Assert.True(valid);
        }

        public static IEnumerable<object[]> GetEQM()
        {
            // X 90�
            yield return new object[]
            {
                new float3(M.PiOver2, 0, 0),
                new Quaternion(0.7071068f, 0, 0, 0.7071068f),
                new float4x4(1, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1)
            };
            // X 45� 
            yield return new object[]
            {
                new float3(M.PiOver4, 0, 0),
                new Quaternion(0.3826835f, 0, 0, 0.9238795f),
                new float4x4(1, 0, 0, 0, 0, 0.7071067f, -0.7071068f, 0, 0, 0.7071068f, 0.7071067f, 0, 0, 0, 0, 1)
            };
            // X 135�
            yield return new object[]
            {
                new float3(M.PiOver4 * 3, 0, 0),
                new Quaternion(0.9238795f, 0, 0, 0.3826834f),
                new float4x4(1, 0, 0, 0, 0, -0.7071067f, -0.7071068f, 0, 0, 0.7071068f, -0.7071067f, 0, 0, 0, 0, 1)
            };

            // Y 90�
            yield return new object[]
            {
                new float3(0, M.PiOver2, 0),
                new Quaternion(0, 0.7071068f, 0, 0.7071068f),
                new float4x4(0, 0, 1, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 0, 1)
            };
            // Y 45�
            yield return new object[]
            {
                new float3(0, M.PiOver4, 0),
                new Quaternion(0, 0.3826835f, 0, 0.9238795f),
                new float4x4(0.7071067f, 0, 0.7071068f, 0, 0, 1, 0, 0, -0.7071068f, 0, 0.7071067f, 0, 0, 0, 0, 1)
            };
            // Y 135�
            yield return new object[]
            {
                new float3(0, M.PiOver4 * 3, 0),
                new Quaternion(0, 0.9238795f, 0, 0.3826834f),
                new float4x4(-0.7071067f, 0, 0.7071068f, 0, 0, 1, 0, 0, -0.7071068f, 0, -0.7071067f, 0, 0, 0, 0, 1)
            };

            // Z 90�
            yield return new object[]
            {
                new float3(0, 0, M.PiOver2),
                new Quaternion(0, 0, 0.7071068f, 0.7071068f),
                new float4x4(0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1)
            };
            // Z 45�
            yield return new object[]
            {
                new float3(0, 0, M.PiOver4),
                new Quaternion(0, 0, 0.3826835f, 0.9238795f),
                new float4x4(0.7071067f, -0.7071068f, 0, 0, 0.7071068f, 0.7071067f, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1)
            };
            // Z 135�
            yield return new object[]
            {
                new float3(0, 0, M.PiOver4 * 3),
                new Quaternion(0, 0, 0.9238795f, 0.3826834f),
                new float4x4(-0.7071067f, -0.7071068f, 0, 0, 0.7071068f, -0.7071067f, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1)
            };

            // X&Y 45�
            yield return new object[]
            {
                new float3(M.PiOver4, M.PiOver4, 0),
                new Quaternion(0.3535534f, 0.3535534f, -0.1464466f, 0.8535534f),
                new float4x4(0.7071067f, 0.5f, 0.5f, 0, 0, 0.7071067f, -0.7071068f, 0, -0.7071068f, 0.5f, 0.5f, 0, 0, 0, 0, 1)
            };
            // X&Z 45�
            yield return new object[]
            {
                new float3(M.PiOver4, 0, M.PiOver4),
                new Quaternion(0.3535534f, -0.1464466f, 0.3535534f, 0.8535534f),
                new float4x4(0.7071067f, -0.7071068f, 0, 0, 0.5f, 0.5f, -0.7071068f, 0, 0.5f, 0.5f, 0.7071067f, 0, 0, 0, 0, 1)
            };
            // Y&Z 45�
            yield return new object[]
            {
                new float3(0, M.PiOver4, M.PiOver4),
                new Quaternion(0.1464466f, 0.3535534f, 0.3535534f, 0.8535534f),
                new float4x4(0.5f, -0.5f, 0.7071068f, 0, 0.7071068f, 0.7071067f, 0, 0, -0.5f, 0.5f, 0.7071067f, 0, 0, 0, 0, 1)
            };

            // X&Y&Z 45�
            yield return new object[]
            {
                new float3(M.PiOver4, M.PiOver4, M.PiOver4),
                new Quaternion(0.4619398f, 0.1913417f, 0.1913417f, 0.8446232f),
                new float4x4(0.8535534f, -0.1464466f, 0.5f, 0, 0.5f, 0.5f, -0.7071069f, 0, -0.1464466f, 0.8535535f, 0.5f, 0, 0, 0, 0, 1)
            };
        }
    }
}