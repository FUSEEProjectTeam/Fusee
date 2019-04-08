using Xunit;
using System;
using System.Collections.Generic;

namespace Fusee.Math.Core
{
    public class Float4x4Test
    {
        #region Fields
        [Fact]
        public void Identity_IsIdentity()
        {
            var actual = float4x4.Identity;
            var expected = new float4x4(new float4(1, 0, 0, 0), new float4(0, 1, 0, 0), new float4(0, 0, 1, 0), new float4(0, 0, 0, 1));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Row0_IsRow()
        {
            var matrix = new float4x4(new float4(1, 0, 0, 0), new float4(0, 1, 0, 0), new float4(0, 0, 1, 0), new float4(0, 0, 0, 1));

            var actual = matrix.Row0;

            Assert.Equal(new float4(1, 0, 0, 0), actual);
        }

        [Fact]
        public void Row1_IsRow()
        {
            var matrix = new float4x4(new float4(1, 0, 0, 0), new float4(0, 1, 0, 0), new float4(0, 0, 1, 0), new float4(0, 0, 0, 1));

            var actual = matrix.Row1;

            Assert.Equal(new float4(0, 1, 0, 0), actual);
        }

        [Fact]
        public void Row2_IsRow()
        {
            var matrix = new float4x4(new float4(1, 0, 0, 0), new float4(0, 1, 0, 0), new float4(0, 0, 1, 0), new float4(0, 0, 0, 1));

            var actual = matrix.Row2;

            Assert.Equal(new float4(0, 0, 1, 0), actual);
        }

        [Fact]
        public void Row3_IsRow()
        {
            var matrix = new float4x4(new float4(1, 0, 0, 0), new float4(0, 1, 0, 0), new float4(0, 0, 1, 0), new float4(0, 0, 0, 1));

            var actual = matrix.Row3;

            Assert.Equal(new float4(0, 0, 0, 1), actual);
        }

        [Fact]
        public void Zero_IsZero()
        {
            var actual = float4x4.Zero;
            var expected = new float4x4(new float4(0, 0, 0, 0), new float4(0, 0, 0, 0), new float4(0, 0, 0, 0), new float4(0, 0, 0, 0));

            Assert.Equal(expected, actual);
        }
        #endregion

        #region Operators
        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Addition_TwoMatrices_ReturnMatrix(float4x4 a, float4x4 b, float4x4 expected)
        {
            var actual = a + b;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Equality_IsEqual()
        {
            var a = new float4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
            var b = new float4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

            Assert.True(a == b);
        }

        [Fact]
        public void Equality_IsInequal()
        {
            var a = new float4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
            var b = new float4x4(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

            Assert.False(a == b);
        }

        [Fact]
        public void Inequality_IsEqual()
        {
            var a = new float4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
            var b = new float4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

            Assert.False(a != b);
        }

        [Fact]
        public void Inequality_IsInequal()
        {
            var a = new float4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
            var b = new float4x4(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

            Assert.True(a != b);
        }

        [Fact]
        public void Explicit_DoubleToFloat()
        {
            var d = new double4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

            var actual = (float4x4)d;

            Assert.Equal(new float4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1), actual);
        }

        [Theory]
        [MemberData(nameof(GetTransformFloat3))]
        public void Multiply_Float3Matrix(float3 vec, float4x4 matrix, float3 expected)
        {
            var actual = vec * matrix;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTransformFloat3))]
        public void Multiply_MatrixFloat3(float3 vec, float4x4 matrix, float3 expected)
        {
            var actual = matrix * vec;

            Assert.Equal(expected, -actual);
        }

        [Theory]
        [MemberData(nameof(GetTransformFloat4))]
        public void Multiply_MatrixFloat4(float4 vec, float4x4 matrix, float4 expected)
        {
            var actual = matrix * vec;

            Assert.Equal(expected, -actual);
        }

        [Theory]
        [MemberData(nameof(GetTransformFloat4))]
        public void Multiply_Float4Matrix(float4 vec, float4x4 matrix, float4 expected)
        {
            var actual = vec * matrix;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Multiply_TwoMatrices()
        {
            var a = float4x4.Identity;
            var b = new float4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

            var actual = a * b;
            var actual2 = b * a;

            Assert.Equal(b, actual);
            Assert.Equal(b, actual2);
        }

        [Fact]
        public void Subtraction_TwoMatrices_ReturnMatrix()
        {
            var a = new float4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
            var expected = new float4x4(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

            var actual = a - a;

            Assert.Equal(expected, actual);
        }
        #endregion

        #region Constructors
        [Fact]
        public void Float4x4_FromDouble4x4()
        {
            var d = new double4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

            var actual = new float4x4(d);

            Assert.Equal(new float4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1), actual);
        }

        [Fact]
        public void Float4x4_FromFloat4()
        {
            var actual = new float4x4(new float4(1, 1, 1, 1), new float4(1, 1, 1, 1), new float4(1, 1, 1, 1), new float4(1, 1, 1, 1));

            Assert.Equal(new float4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1), actual);
        }
        #endregion

        #region Properties
        [Fact]
        public void Column0_IsCoumn()
        {
            var matrix = float4x4.Identity;

            var actual = matrix.Column0;

            Assert.Equal(new float4(1, 0, 0, 0), actual);
        }

        [Fact]
        public void Column1_IsCoumn()
        {
            var matrix = float4x4.Identity;

            var actual = matrix.Column1;

            Assert.Equal(new float4(0, 1, 0, 0), actual);
        }

        [Fact]
        public void Column2_IsCoumn()
        {
            var matrix = float4x4.Identity;

            var actual = matrix.Column2;

            Assert.Equal(new float4(0, 0, 1, 0), actual);
        }

        [Fact]
        public void Column3_IsCoumn()
        {
            var matrix = float4x4.Identity;

            var actual = matrix.Column3;

            Assert.Equal(new float4(0, 0, 0, 1), actual);
        }

        [Fact]
        public void Determinant_IsDeterminant()
        {
            var matrix = float4x4.Identity;

            var actual = matrix.Determinant;

            Assert.Equal(1, actual);
        }

        [Fact]
        public void IsAffine_IsAffine()
        {
            var matrix = float4x4.Identity;

            Assert.True(matrix.IsAffine);
        }

        [Fact]
        public void IsAffine_IsNotAffine()
        {
            var matrix = new float4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

            Assert.False(matrix.IsAffine);
        }

        [Fact]
        public void Offset_IsOffset()
        {
            var matrix = float4x4.Identity;

            Assert.Equal(new float3(0, 0, 0), matrix.Offset);
        }
        #endregion

        #region Methods

        #region Arithmetic Functions
        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_TwoMatrices_ReturnMatrix(float4x4 a, float4x4 b, float4x4 expected)
        {
            var actual = float4x4.Add(a, b);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Substract_TwoMatrices_Returnmatrix()
        {
            var a = new float4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

            var actual = float4x4.Substract(a, a);

            Assert.Equal(float4x4.Zero, actual);
        }
        #endregion

        #region CreateFromAxisAngle
        [Theory]
        [MemberData(nameof(GetAxisAngle))]
        public void CreateFromAxisAngle_ReturnMatrix(float3 axis, float angle, float4x4 expected)
        {
            var actual = float4x4.CreateFromAxisAngle(axis, angle);

            Assert.Equal(expected.M11, actual.M11, 5);
            Assert.Equal(expected.M12, actual.M12, 5);
            Assert.Equal(expected.M13, actual.M13, 5);
            Assert.Equal(expected.M14, actual.M14, 5);
            Assert.Equal(expected.M21, actual.M21, 5);
            Assert.Equal(expected.M22, actual.M22, 5);
            Assert.Equal(expected.M23, actual.M23, 5);
            Assert.Equal(expected.M24, actual.M24, 5);
            Assert.Equal(expected.M31, actual.M31, 5);
            Assert.Equal(expected.M32, actual.M32, 5);
            Assert.Equal(expected.M33, actual.M33, 5);
            Assert.Equal(expected.M34, actual.M34, 5);
            Assert.Equal(expected.M41, actual.M41, 5);
            Assert.Equal(expected.M42, actual.M42, 5);
            Assert.Equal(expected.M43, actual.M43, 5);
            Assert.Equal(expected.M44, actual.M44, 5);
        }

        [Theory]
        [MemberData(nameof(GetAxisAngle))]
        public void CreateFromAxisAngle_ToMatrix(float3 axis, float angle, float4x4 expected)
        {
            float4x4 actual;

            float4x4.CreateFromAxisAngle(axis, angle, out actual);

            Assert.Equal(expected.M11, actual.M11, 5);
            Assert.Equal(expected.M12, actual.M12, 5);
            Assert.Equal(expected.M13, actual.M13, 5);
            Assert.Equal(expected.M14, actual.M14, 5);
            Assert.Equal(expected.M21, actual.M21, 5);
            Assert.Equal(expected.M22, actual.M22, 5);
            Assert.Equal(expected.M23, actual.M23, 5);
            Assert.Equal(expected.M24, actual.M24, 5);
            Assert.Equal(expected.M31, actual.M31, 5);
            Assert.Equal(expected.M32, actual.M32, 5);
            Assert.Equal(expected.M33, actual.M33, 5);
            Assert.Equal(expected.M34, actual.M34, 5);
            Assert.Equal(expected.M41, actual.M41, 5);
            Assert.Equal(expected.M42, actual.M42, 5);
            Assert.Equal(expected.M43, actual.M43, 5);
            Assert.Equal(expected.M44, actual.M44, 5);
        }
        #endregion

        #region CreateOrthographic
        [Theory]
        [MemberData(nameof(GetOrthographic))]
        public void CreateOrthographic_ReturnMatrix(float width, float height, float zNear, float zFar, float4x4 expected)
        {
            float4x4 actual;

            actual = float4x4.CreateOrthographic(width, height, zNear, zFar);

            Assert.Equal(expected.M11, actual.M11, 5);
            Assert.Equal(expected.M12, actual.M12, 5);
            Assert.Equal(expected.M13, actual.M13, 5);
            Assert.Equal(expected.M14, actual.M14, 5);
            Assert.Equal(expected.M21, actual.M21, 5);
            Assert.Equal(expected.M22, actual.M22, 5);
            Assert.Equal(expected.M23, actual.M23, 5);
            Assert.Equal(expected.M24, actual.M24, 5);
            Assert.Equal(expected.M31, actual.M31, 5);
            Assert.Equal(expected.M32, actual.M32, 5);
            Assert.Equal(expected.M33, actual.M33, 5);
            Assert.Equal(expected.M34, actual.M34, 5);
            Assert.Equal(expected.M41, actual.M41, 5);
            Assert.Equal(expected.M42, actual.M42, 5);
            Assert.Equal(expected.M43, actual.M43, 5);
            Assert.Equal(expected.M44, actual.M44, 5);
        }

        [Theory]
        [MemberData(nameof(GetOrthographic))]
        public void CreateOrthographic_ToMatrix(float width, float height, float zNear, float zFar, float4x4 expected)
        {
            float4x4 actual;

            float4x4.CreateOrthographic(width, height, zNear, zFar, out actual);

            Assert.Equal(expected.M11, actual.M11, 5);
            Assert.Equal(expected.M12, actual.M12, 5);
            Assert.Equal(expected.M13, actual.M13, 5);
            Assert.Equal(expected.M14, actual.M14, 5);
            Assert.Equal(expected.M21, actual.M21, 5);
            Assert.Equal(expected.M22, actual.M22, 5);
            Assert.Equal(expected.M23, actual.M23, 5);
            Assert.Equal(expected.M24, actual.M24, 5);
            Assert.Equal(expected.M31, actual.M31, 5);
            Assert.Equal(expected.M32, actual.M32, 5);
            Assert.Equal(expected.M33, actual.M33, 5);
            Assert.Equal(expected.M34, actual.M34, 5);
            Assert.Equal(expected.M41, actual.M41, 5);
            Assert.Equal(expected.M42, actual.M42, 5);
            Assert.Equal(expected.M43, actual.M43, 5);
            Assert.Equal(expected.M44, actual.M44, 5);
        }

        [Theory]
        [MemberData(nameof(GetOrthographic))]
        public void CreateOrthographicOffCenter_ReturnMatrix(float width, float height, float zNear, float zFar, float4x4 expected)
        {
            float4x4 actual;

            actual = float4x4.CreateOrthographicOffCenter(-width / 2, width / 2, -height / 2, height / 2, zNear, zFar);

            Assert.Equal(expected.M11, actual.M11, 5);
            Assert.Equal(expected.M12, actual.M12, 5);
            Assert.Equal(expected.M13, actual.M13, 5);
            Assert.Equal(expected.M14, actual.M14, 5);
            Assert.Equal(expected.M21, actual.M21, 5);
            Assert.Equal(expected.M22, actual.M22, 5);
            Assert.Equal(expected.M23, actual.M23, 5);
            Assert.Equal(expected.M24, actual.M24, 5);
            Assert.Equal(expected.M31, actual.M31, 5);
            Assert.Equal(expected.M32, actual.M32, 5);
            Assert.Equal(expected.M33, actual.M33, 5);
            Assert.Equal(expected.M34, actual.M34, 5);
            Assert.Equal(expected.M41, actual.M41, 5);
            Assert.Equal(expected.M42, actual.M42, 5);
            Assert.Equal(expected.M43, actual.M43, 5);
            Assert.Equal(expected.M44, actual.M44, 5);
        }

        [Theory]
        [MemberData(nameof(GetOrthographic))]
        public void CreateOrthographicOffCenter_ToMatrix(float width, float height, float zNear, float zFar, float4x4 expected)
        {
            float4x4 actual;

            float4x4.CreateOrthographicOffCenter(-width / 2, width / 2, -height / 2, height / 2, zNear, zFar, out actual);

            Assert.Equal(expected.M11, actual.M11, 5);
            Assert.Equal(expected.M12, actual.M12, 5);
            Assert.Equal(expected.M13, actual.M13, 5);
            Assert.Equal(expected.M14, actual.M14, 5);
            Assert.Equal(expected.M21, actual.M21, 5);
            Assert.Equal(expected.M22, actual.M22, 5);
            Assert.Equal(expected.M23, actual.M23, 5);
            Assert.Equal(expected.M24, actual.M24, 5);
            Assert.Equal(expected.M31, actual.M31, 5);
            Assert.Equal(expected.M32, actual.M32, 5);
            Assert.Equal(expected.M33, actual.M33, 5);
            Assert.Equal(expected.M34, actual.M34, 5);
            Assert.Equal(expected.M41, actual.M41, 5);
            Assert.Equal(expected.M42, actual.M42, 5);
            Assert.Equal(expected.M43, actual.M43, 5);
            Assert.Equal(expected.M44, actual.M44, 5);
        }

        [Theory]
        [MemberData(nameof(GetOrthographic))]
        public void CreateOrthographicOffCenterRH_ReturnMatrix(float width, float height, float zNear, float zFar, float4x4 expected)
        {
            float4x4 actual;

            float4x4.CreateOrthographicOffCenterRH(-width / 2, width / 2, -height / 2, height / 2, zNear, zFar, out actual);

            Assert.Equal(expected.M11, actual.M11, 5);
            Assert.Equal(expected.M12, actual.M12, 5);
            Assert.Equal(expected.M13, actual.M13, 5);
            Assert.Equal(expected.M14, actual.M14, 5);
            Assert.Equal(expected.M21, actual.M21, 5);
            Assert.Equal(expected.M22, actual.M22, 5);
            Assert.Equal(expected.M23, actual.M23, 5);
            Assert.Equal(expected.M24, actual.M24, 5);
            Assert.Equal(expected.M31, actual.M31, 5);
            Assert.Equal(expected.M32, actual.M32, 5);
            Assert.Equal(-expected.M33, actual.M33, 5);
            Assert.Equal(expected.M34, actual.M34, 5);
            Assert.Equal(expected.M41, actual.M41, 5);
            Assert.Equal(expected.M42, actual.M42, 5);
            Assert.Equal(expected.M43, actual.M43, 5);
            Assert.Equal(expected.M44, actual.M44, 5);
        }
        #endregion

        #region CreatePerspective
        [Theory]
        [MemberData(nameof(GetPerspectiveFOW))]
        public void CreatePerspectiveFieldOfView_ReturnMatrix(float fovy, float aspect, float zNear, float zFar, float4x4 expected)
        {
            float4x4 actual;

            actual = float4x4.CreatePerspectiveFieldOfView(fovy, aspect, zNear, zFar);

            Assert.Equal(expected.M11, actual.M11, 5);
            Assert.Equal(expected.M12, actual.M12, 5);
            Assert.Equal(expected.M13, actual.M13, 5);
            Assert.Equal(expected.M14, actual.M14, 5);
            Assert.Equal(expected.M21, actual.M21, 5);
            Assert.Equal(expected.M22, actual.M22, 5);
            Assert.Equal(expected.M23, actual.M23, 5);
            Assert.Equal(expected.M24, actual.M24, 5);
            Assert.Equal(expected.M31, actual.M31, 5);
            Assert.Equal(expected.M32, actual.M32, 5);
            Assert.Equal(expected.M33, actual.M33, 5);
            Assert.Equal(expected.M34, actual.M34, 5);
            Assert.Equal(expected.M41, actual.M41, 5);
            Assert.Equal(expected.M42, actual.M42, 5);
            Assert.Equal(expected.M43, actual.M43, 5);
            Assert.Equal(expected.M44, actual.M44, 5);
        }

        [Theory]
        [MemberData(nameof(GetPerspectiveFOW))]
        public void CreatePerspectiveFieldOfView_ToMatrix(float fovy, float aspect, float zNear, float zFar, float4x4 expected)
        {
            float4x4 actual;

            float4x4.CreatePerspectiveFieldOfView(fovy, aspect, zNear, zFar, out actual);

            Assert.Equal(expected.M11, actual.M11, 5);
            Assert.Equal(expected.M12, actual.M12, 5);
            Assert.Equal(expected.M13, actual.M13, 5);
            Assert.Equal(expected.M14, actual.M14, 5);
            Assert.Equal(expected.M21, actual.M21, 5);
            Assert.Equal(expected.M22, actual.M22, 5);
            Assert.Equal(expected.M23, actual.M23, 5);
            Assert.Equal(expected.M24, actual.M24, 5);
            Assert.Equal(expected.M31, actual.M31, 5);
            Assert.Equal(expected.M32, actual.M32, 5);
            Assert.Equal(expected.M33, actual.M33, 5);
            Assert.Equal(expected.M34, actual.M34, 5);
            Assert.Equal(expected.M41, actual.M41, 5);
            Assert.Equal(expected.M42, actual.M42, 5);
            Assert.Equal(expected.M43, actual.M43, 5);
            Assert.Equal(expected.M44, actual.M44, 5);
        }

        [Theory]
        [MemberData(nameof(GetPerspectiveFOWException))]
        public void CreatePerspectiveFieldOfView_ThrowException(float fovy, float aspect, float zNear, float zFar, string expected)
        {
            float4x4 matrix;

            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => float4x4.CreatePerspectiveFieldOfView(fovy, aspect, zNear, zFar, out matrix));

            Assert.Equal(expected, actual.ParamName);
        }

        [Theory]
        [MemberData(nameof(GetPerspectiveOffCenter))]
        public void CreatePerspectiveOffCenter_ReturnMatrix(float left, float right, float bottom, float top, float zNear, float zFar, float4x4 expected)
        {
            float4x4 actual;

            actual = float4x4.CreatePerspectiveOffCenter(left, right, bottom, top, zNear, zFar);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetPerspectiveOffCenter))]
        public void CreatePerspectiveOffCenter_ToMatrix(float left, float right, float bottom, float top, float zNear, float zFar, float4x4 expected)
        {
            float4x4 actual;

            float4x4.CreatePerspectiveOffCenter(left, right, bottom, top, zNear, zFar, out actual);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetPerspectiveOffCenterRH))]
        public void CreatePerspectiveOffCenterRH_ReturnMatrix(float left, float right, float bottom, float top, float zNear, float zFar, float4x4 expected)
        {
            float4x4 actual;

            float4x4.CreatePerspectiveOffCenterRH(left, right, bottom, top, zNear, zFar, out actual);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetPerspectiveOffCenterException))]
        public void CreatePerspectiveOffCenter_ThrowException(float zNear, float zFar, string expected)
        {
            float4x4 matrix;

            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => float4x4.CreatePerspectiveOffCenter(-1, 1, -1, 1, zNear, zFar, out matrix));

            Assert.Equal(expected, actual.ParamName);
        }

        [Theory]
        [MemberData(nameof(GetPerspectiveOffCenterException))]
        public void CreatePerspectiveOffCenterRH_ThrowException(float zNear, float zFar, string expected)
        {
            float4x4 matrix;

            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => float4x4.CreatePerspectiveOffCenterRH(-1, 1, -1, 1, zNear, zFar, out matrix));

            Assert.Equal(expected, actual.ParamName);
        }
        #endregion

        #region CreateRotation
        [Fact]
        public void CreateRotationX_ReturnMatrix()
        {
            float4x4 actual;
            float4x4 expected = new float4x4(1, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1);

            actual = float4x4.CreateRotationX(1.5708f);

            Assert.Equal(expected.M11, actual.M11, 5);
            Assert.Equal(expected.M12, actual.M12, 5);
            Assert.Equal(expected.M13, actual.M13, 5);
            Assert.Equal(expected.M14, actual.M14, 5);
            Assert.Equal(expected.M21, actual.M21, 5);
            Assert.Equal(expected.M22, actual.M22, 5);
            Assert.Equal(expected.M23, actual.M23, 5);
            Assert.Equal(expected.M24, actual.M24, 5);
            Assert.Equal(expected.M31, actual.M31, 5);
            Assert.Equal(expected.M32, actual.M32, 5);
            Assert.Equal(expected.M33, actual.M33, 5);
            Assert.Equal(expected.M34, actual.M34, 5);
            Assert.Equal(expected.M41, actual.M41, 5);
            Assert.Equal(expected.M42, actual.M42, 5);
            Assert.Equal(expected.M43, actual.M43, 5);
            Assert.Equal(expected.M44, actual.M44, 5);
        }

        [Fact]
        public void CreateRotationX_ToMatrix()
        {
            float4x4 actual;
            float4x4 expected = new float4x4(1, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1);

            float4x4.CreateRotationX(1.5708f, out actual);

            Assert.Equal(expected.M11, actual.M11, 5);
            Assert.Equal(expected.M12, actual.M12, 5);
            Assert.Equal(expected.M13, actual.M13, 5);
            Assert.Equal(expected.M14, actual.M14, 5);
            Assert.Equal(expected.M21, actual.M21, 5);
            Assert.Equal(expected.M22, actual.M22, 5);
            Assert.Equal(expected.M23, actual.M23, 5);
            Assert.Equal(expected.M24, actual.M24, 5);
            Assert.Equal(expected.M31, actual.M31, 5);
            Assert.Equal(expected.M32, actual.M32, 5);
            Assert.Equal(expected.M33, actual.M33, 5);
            Assert.Equal(expected.M34, actual.M34, 5);
            Assert.Equal(expected.M41, actual.M41, 5);
            Assert.Equal(expected.M42, actual.M42, 5);
            Assert.Equal(expected.M43, actual.M43, 5);
            Assert.Equal(expected.M44, actual.M44, 5);
        }

        [Fact]
        public void CreateRotationY_ReturnMatrix()
        {
            float4x4 actual;
            float4x4 expected = new float4x4(0, 0, 1, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 0, 1);

            actual = float4x4.CreateRotationY(1.5708f);

            Assert.Equal(expected.M11, actual.M11, 5);
            Assert.Equal(expected.M12, actual.M12, 5);
            Assert.Equal(expected.M13, actual.M13, 5);
            Assert.Equal(expected.M14, actual.M14, 5);
            Assert.Equal(expected.M21, actual.M21, 5);
            Assert.Equal(expected.M22, actual.M22, 5);
            Assert.Equal(expected.M23, actual.M23, 5);
            Assert.Equal(expected.M24, actual.M24, 5);
            Assert.Equal(expected.M31, actual.M31, 5);
            Assert.Equal(expected.M32, actual.M32, 5);
            Assert.Equal(expected.M33, actual.M33, 5);
            Assert.Equal(expected.M34, actual.M34, 5);
            Assert.Equal(expected.M41, actual.M41, 5);
            Assert.Equal(expected.M42, actual.M42, 5);
            Assert.Equal(expected.M43, actual.M43, 5);
            Assert.Equal(expected.M44, actual.M44, 5);
        }

        [Fact]
        public void CreateRotationY_ToMatrix()
        {
            float4x4 actual;
            float4x4 expected = new float4x4(0, 0, 1, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 0, 1);

            float4x4.CreateRotationY(1.5708f, out actual);

            Assert.Equal(expected.M11, actual.M11, 5);
            Assert.Equal(expected.M12, actual.M12, 5);
            Assert.Equal(expected.M13, actual.M13, 5);
            Assert.Equal(expected.M14, actual.M14, 5);
            Assert.Equal(expected.M21, actual.M21, 5);
            Assert.Equal(expected.M22, actual.M22, 5);
            Assert.Equal(expected.M23, actual.M23, 5);
            Assert.Equal(expected.M24, actual.M24, 5);
            Assert.Equal(expected.M31, actual.M31, 5);
            Assert.Equal(expected.M32, actual.M32, 5);
            Assert.Equal(expected.M33, actual.M33, 5);
            Assert.Equal(expected.M34, actual.M34, 5);
            Assert.Equal(expected.M41, actual.M41, 5);
            Assert.Equal(expected.M42, actual.M42, 5);
            Assert.Equal(expected.M43, actual.M43, 5);
            Assert.Equal(expected.M44, actual.M44, 5);
        }

        [Fact]
        public void CreateRotationZ_ReturnMatrix()
        {
            float4x4 actual;
            float4x4 expected = new float4x4(0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

            actual = float4x4.CreateRotationZ(1.5708f);

            Assert.Equal(expected.M11, actual.M11, 5);
            Assert.Equal(expected.M12, actual.M12, 5);
            Assert.Equal(expected.M13, actual.M13, 5);
            Assert.Equal(expected.M14, actual.M14, 5);
            Assert.Equal(expected.M21, actual.M21, 5);
            Assert.Equal(expected.M22, actual.M22, 5);
            Assert.Equal(expected.M23, actual.M23, 5);
            Assert.Equal(expected.M24, actual.M24, 5);
            Assert.Equal(expected.M31, actual.M31, 5);
            Assert.Equal(expected.M32, actual.M32, 5);
            Assert.Equal(expected.M33, actual.M33, 5);
            Assert.Equal(expected.M34, actual.M34, 5);
            Assert.Equal(expected.M41, actual.M41, 5);
            Assert.Equal(expected.M42, actual.M42, 5);
            Assert.Equal(expected.M43, actual.M43, 5);
            Assert.Equal(expected.M44, actual.M44, 5);
        }

        [Fact]
        public void CreateRotationZ_ToMatrix()
        {
            float4x4 actual;
            float4x4 expected = new float4x4(0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

            float4x4.CreateRotationZ(1.5708f, out actual);

            Assert.Equal(expected.M11, actual.M11, 5);
            Assert.Equal(expected.M12, actual.M12, 5);
            Assert.Equal(expected.M13, actual.M13, 5);
            Assert.Equal(expected.M14, actual.M14, 5);
            Assert.Equal(expected.M21, actual.M21, 5);
            Assert.Equal(expected.M22, actual.M22, 5);
            Assert.Equal(expected.M23, actual.M23, 5);
            Assert.Equal(expected.M24, actual.M24, 5);
            Assert.Equal(expected.M31, actual.M31, 5);
            Assert.Equal(expected.M32, actual.M32, 5);
            Assert.Equal(expected.M33, actual.M33, 5);
            Assert.Equal(expected.M34, actual.M34, 5);
            Assert.Equal(expected.M41, actual.M41, 5);
            Assert.Equal(expected.M42, actual.M42, 5);
            Assert.Equal(expected.M43, actual.M43, 5);
            Assert.Equal(expected.M44, actual.M44, 5);
        }
        #endregion

        #region CreateScale / Scale
        [Theory]
        [MemberData(nameof(GetScale))]
        public void CreateScale_Single_ReturnMatrix(float x, float y, float z, float4x4 expected)
        {
            float4x4 actual;

            actual = float4x4.CreateScale(x);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetScale))]
        public void CreateScale_Single_ToMatrix(float x, float y, float z, float4x4 expected)
        {
            float4x4 actual;

            float4x4.CreateScale(x, out actual);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetScale))]
        public void CreateScale_Float3_ReturnMatrix(float x, float y, float z, float4x4 expected)
        {
            float4x4 actual;
            float3 vec = new float3(x, y, z);

            actual = float4x4.CreateScale(vec);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetScale))]
        public void CreateScale_Float3_ToMatrix(float x, float y, float z, float4x4 expected)
        {
            float4x4 actual;
            float3 vec = new float3(x, y, z);

            float4x4.CreateScale(ref vec, out actual);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetScale))]
        public void CreateScale_3Singles_ReturnMatrix(float x, float y, float z, float4x4 expected)
        {
            float4x4 actual;

            actual = float4x4.CreateScale(x, y, z);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetScale))]
        public void CreateScale_3Singles_ToMatrix(float x, float y, float z, float4x4 expected)
        {
            float4x4 actual;

            float4x4.CreateScale(x, y, z, out actual);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetScale))]
        public void Scale_Single_ReturnMatrix(float x, float y, float z, float4x4 expected)
        {
            float4x4 actual;

            actual = float4x4.Scale(x);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetScale))]
        public void Scale_Vector_ReturnMatrix(float x, float y, float z, float4x4 expected)
        {
            float4x4 actual;
            var vec = new float3(x, y, z);

            actual = float4x4.Scale(vec);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetScale))]
        public void Scale_3Singles_ReturnMatrix(float x, float y, float z, float4x4 expected)
        {
            float4x4 actual;

            actual = float4x4.Scale(x, y, z);

            Assert.Equal(expected, actual);
        }
        #endregion

        #region CreateTranslation
        [Theory]
        [MemberData(nameof(GetTranslation))]
        public void CreateTranslation_Float3_ReturnMatrix(float x, float y, float z, float4x4 expected)
        {
            float4x4 actual;
            var vec = new float3(x, y, z);

            actual = float4x4.CreateTranslation(vec);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTranslation))]
        public void CreateTranslation_Float3_ToMatrix(float x, float y, float z, float4x4 expected)
        {
            float4x4 actual;
            var vec = new float3(x, y, z);

            float4x4.CreateTranslation(ref vec, out actual);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTranslation))]
        public void CreateTranslation_3Singles_ReturnMatrix(float x, float y, float z, float4x4 expected)
        {
            float4x4 actual;

            actual = float4x4.CreateTranslation(x, y, z);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTranslation))]
        public void CreateTranslation_3Singles_ToMatrix(float x, float y, float z, float4x4 expected)
        {
            float4x4 actual;

            float4x4.CreateTranslation(x, y, z, out actual);

            Assert.Equal(expected, actual);
        }
        #endregion

        #region Invert
        [Theory]
        [MemberData(nameof(GetInvert))]
        public void Invert_InvertMatrix(float4x4 matrix, float4x4 expected)
        {
            var actual = matrix;

            actual.Invert();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetInvert))]
        public void Invert_ReturnMatrix(float4x4 matrix, float4x4 expected)
        {
            float4x4 actual;

            actual = float4x4.Invert(matrix);

            Assert.Equal(expected, actual);
        }
        #endregion

        #region LookAt
        [Theory]
        [MemberData(nameof(GetLookAt))]
        public void LookAt_ThreeVectors_ReturnMatrix(float3 eye, float3 target, float3 up, float4x4 expected)
        {
            float4x4 actual;

            actual = float4x4.LookAt(eye, target, up);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetLookAt))]
        public void LookAt_Singles_ReturnMatrix(float3 eye, float3 target, float3 up, float4x4 expected)
        {
            float4x4 actual;

            actual = float4x4.LookAt(eye.x, eye.y, eye.z, target.x, target.y, target.z, up.x, up.y, up.z);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetLookAtRH))]
        public void LookAtRH_ThreeVectors_ReturnMatrix(float3 eye, float3 target, float3 up, float4x4 expected)
        {
            float4x4 actual;

            actual = float4x4.LookAtRH(eye, target, up);

            Assert.Equal(expected, actual);
        }
        #endregion

        #region Mult
        [Theory]
        [MemberData(nameof(GetMult))]
        public void Mult_TwoMatrices_ReturnMatrix(float4x4 left, float4x4 right, float4x4 expected)
        {
            float4x4 actual;

            actual = float4x4.Mult(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMult))]
        public void Mult_TwoMatrices_ToMatrix(float4x4 left, float4x4 right, float4x4 expected)
        {
            float4x4 actual;

            float4x4.Mult(ref left, ref right, out actual);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultAffine))]
        public void MultAffine_TwoMatrices_ToMatrix(float4x4 left, float4x4 right, float4x4 expected)
        {
            float4x4 actual;

            float4x4.MultAffine(ref left, ref right, out actual);

            Assert.Equal(expected, actual);
        }
        #endregion

        #region Round
        [Fact]
        public void Round_ReturnMatrix()
        {
            var matrix = new float4x4(1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f);

            var actual = float4x4.Round(matrix);

            Assert.Equal(new float4x4(1.234568f, 1.234568f, 1.234568f, 1.234568f, 1.234568f, 1.234568f, 1.234568f, 1.234568f, 1.234568f, 1.234568f, 1.234568f, 1.234568f, 1.234568f, 1.234568f, 1.234568f, 1.234568f), actual);
        }

        [Fact]
        public void Round_Direct()
        {
            var actual = new float4x4(1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f, 1.23456789f);

            actual.Round();

            Assert.Equal(new float4x4(1.234568f, 1.234568f, 1.234568f, 1.234568f, 1.234568f, 1.234568f, 1.234568f, 1.234568f, 1.234568f, 1.234568f, 1.234568f, 1.234568f, 1.234568f, 1.234568f, 1.234568f, 1.234568f), actual);
        }
        #endregion

        #region Transform
        [Theory]
        [MemberData(nameof(GetTransformFloat4))]
        public void Transform_MatrixVector_ReturnVector(float4 vector, float4x4 matrix, float4 expected)
        {
            float4 actual;

            actual = float4x4.Transform(matrix, vector);

            Assert.Equal(-expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTransformFloat3))]
        public void TransformPD_MatrixVector_ReturnVector(float3 vector, float4x4 matrix, float3 expected)
        {
            float3 actual;

            actual = float4x4.TransformPD(matrix, vector);

            Assert.Equal(-expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTransformFloat4))]
        public void TransformPremult_VectorMatrix_ReturnVector(float4 vector, float4x4 matrix, float4 expected)
        {
            float4 actual;

            actual = float4x4.TransformPremult(vector, matrix);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTransformFloat3))]
        public void TransformPremultPD_VectorMatrix_ReturnVector(float3 vector, float4x4 matrix, float3 expected)
        {
            float3 actual;

            actual = float4x4.TransformPremultPD(vector, matrix);

            Assert.Equal(expected, actual);
        }
        #endregion

        #region Transpose
        [Theory]
        [MemberData(nameof(GetTranspose))]
        public void Transpose_ReturnVector(float4x4 matrix, float4x4 expected)
        {
            float4x4 actual;

            actual = float4x4.Transpose(matrix);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTranspose))]
        public void Transpose_ToVector(float4x4 matrix, float4x4 expected)
        {
            float4x4 actual;

            float4x4.Transpose(ref matrix, out actual);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTranspose))]
        public void Transpose_Direct(float4x4 matrix, float4x4 expected)
        {
            float4x4 actual = matrix;

            actual.Transpose();

            Assert.Equal(expected, actual);
        }
        #endregion

        #region Other
        [Fact]
        public void Equals_IsEqual()
        {
            var mat1 = float4x4.Identity;
            var mat2 = float4x4.Identity;

            Assert.True(mat1.Equals(mat2));
        }

        [Fact]
        public void Equals_IsInequal()
        {
            var mat1 = float4x4.Identity;
            var mat2 = float4x4.Zero;

            Assert.False(mat1.Equals(mat2));
        }

        //TODO: GetHashCode

        //TODO: GetType

        [Fact]
        public void ToStrin_IsString()
        {
            var matrix = new float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

            var actual = matrix.ToString();

            Assert.Equal("(1, 0, 0, 0)\n(0, 1, 0, 0)\n(0, 0, 1, 0)\n(0, 0, 0, 1)", actual);
        }
        #endregion

        #endregion

        #region IEnumerables
        public static IEnumerable<object[]> GetAddition()
        {
            var a = new float4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
            var b = new float4x4(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

            yield return new object[] { a, b, a };
            yield return new object[] { b, a, a };
        }

        public static IEnumerable<object[]> GetTransformFloat3()
        {
            var a = new float3(1, 0, 0);
            var b = new float3(0, 1, 0);
            var c = new float3(0, 0, 1);

            var xRot = new float4x4(new float4(1, 0, 0, 0), new float4(0, 0, -1, 0), new float4(0, 1, 0, 0), new float4(0, 0, 0, 1));
            var yRot = new float4x4(new float4(0, 0, 1, 0), new float4(0, 1, 0, 0), new float4(-1, 0, 0, 0), new float4(0, 0, 0, 1));
            var zRot = new float4x4(new float4(0, -1, 0, 0), new float4(1, 0, 0, 0), new float4(0, 0, 1, 0), new float4(0, 0, 0, 1));

            yield return new object[] { b, xRot, -c };
            yield return new object[] { a, yRot, c };
            yield return new object[] { b, zRot, a };
        }

        public static IEnumerable<object[]> GetTransformFloat4()
        {
            var a = new float4(1, 0, 0, 0);
            var b = new float4(0, 1, 0, 0);
            var c = new float4(0, 0, 1, 0);

            var xRot = new float4x4(new float4(1, 0, 0, 0), new float4(0, 0, -1, 0), new float4(0, 1, 0, 0), new float4(0, 0, 0, 1));
            var yRot = new float4x4(new float4(0, 0, 1, 0), new float4(0, 1, 0, 0), new float4(-1, 0, 0, 0), new float4(0, 0, 0, 1));
            var zRot = new float4x4(new float4(0, -1, 0, 0), new float4(1, 0, 0, 0), new float4(0, 0, 1, 0), new float4(0, 0, 0, 1));

            yield return new object[] { b, xRot, -c };
            yield return new object[] { a, yRot, c };
            yield return new object[] { b, zRot, a };
        }

        public static IEnumerable<object[]> GetAxisAngle()
        {
            var x = new float3(1, 0, 0);
            var y = new float3(0, 1, 0);
            var z = new float3(0, 0, 1);

            var angle = 1.5708f;

            var xRot = new float4x4(new float4(1, 0, 0, 0), new float4(0, 0, -1, 0), new float4(0, 1, 0, 0), new float4(0, 0, 0, 1));
            var yRot = new float4x4(new float4(0, 0, 1, 0), new float4(0, 1, 0, 0), new float4(-1, 0, 0, 0), new float4(0, 0, 0, 1));
            var zRot = new float4x4(new float4(0, -1, 0, 0), new float4(1, 0, 0, 0), new float4(0, 0, 1, 0), new float4(0, 0, 0, 1));

            yield return new object[] { x, angle, xRot };
            yield return new object[] { y, angle, yRot };
            yield return new object[] { z, angle, zRot };
        }

        public static IEnumerable<object[]> GetOrthographic()
        {
            yield return new object[] { 2, 2, -1, 1, float4x4.Identity };
        }

        public static IEnumerable<object[]> GetPerspectiveFOW()
        {
            yield return new object[] { 1.5708f, 1, 1, 2, new float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 3, -4, 0, 0, 1, 0) };
        }

        public static IEnumerable<object[]> GetPerspectiveFOWException()
        {
            yield return new object[] { 0, 1, 1, 2, "fovy" };
            yield return new object[] { 4, 1, 1, 2, "fovy" };
            yield return new object[] { 1, 0, 1, 2, "aspect" };
            yield return new object[] { 1, 1, 0, 2, "zNear" };
            yield return new object[] { 1, 1, 1, 0, "zFar" };
            yield return new object[] { 1, 1, 2, 1, "zNear" };
        }

        public static IEnumerable<object[]> GetPerspectiveOffCenter()
        {
            yield return new object[] { -1, 1, -1, 1, 1, 2, new float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 3, -4, 0, 0, 1, 0) };
        }

        public static IEnumerable<object[]> GetPerspectiveOffCenterRH()
        {
            yield return new object[] { -1, 1, -1, 1, 1, 2, new float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, -3, -4, 0, 0, -1, 0) };
        }

        public static IEnumerable<object[]> GetPerspectiveOffCenterException()
        {
            yield return new object[] { 0, 2, "zNear" };
            yield return new object[] { 1, 0, "zFar" };
            yield return new object[] { 2, 1, "zNear" };
        }

        public static IEnumerable<object[]> GetScale()
        {
            yield return new object[] { 1, 1, 1, float4x4.Identity };
            yield return new object[] { 2, 2, 2, new float4x4(2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 1) };
        }

        public static IEnumerable<object[]> GetTranslation()
        {
            yield return new object[] { 1, 1, 1, new float4x4(1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1) };
        }

        public static IEnumerable<object[]> GetInvert()
        {
            yield return new object[] { new float4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1), new float4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1) };
            yield return new object[] { float4x4.Identity, float4x4.Identity };
            yield return new object[] { new float4x4(0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0), new float4x4(0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0) };
            yield return new object[] { new float4x4(1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1), new float4x4(1, 0, 0, -1, 0, 1, 0, -1, 0, 0, 1, -1, 0, 0, 0, 1) };
        }

        public static IEnumerable<object[]> GetLookAt()
        {
            yield return new object[] { new float3(1, 0, 0), new float3(0, 0, 0), new float3(0, 1, 0), new float4x4(0, 0, 1, 0, 0, 1, 0, 0, -1, 0, 0, 1, 0, 0, 0, 1) };
            yield return new object[] { new float3(0, 0, 1), new float3(0, 0, 0), new float3(0, 1, 0), new float4x4(-1, 0, 0, 0, 0, 1, 0, 0, 0, 0, -1, 1, 0, 0, 0, 1) };
        }

        public static IEnumerable<object[]> GetLookAtRH()
        {
            yield return new object[] { new float3(1, 0, 0), new float3(0, 0, 0), new float3(0, 1, 0), new float4x4(0, 0, -1, 0, 0, 1, 0, 0, 1, 0, 0, -1, 0, 0, 0, 1) };
            yield return new object[] { new float3(0, 0, 1), new float3(0, 0, 0), new float3(0, 1, 0), new float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, -1, 0, 0, 0, 1) };
        }

        public static IEnumerable<object[]> GetMult()
        {
            var mat1 = new float4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

            yield return new object[] { mat1, float4x4.Identity, mat1 };
            yield return new object[] { float4x4.Identity, mat1, mat1 };
            yield return new object[] { mat1, float4x4.Zero, float4x4.Zero };
            yield return new object[] { float4x4.Zero, mat1, float4x4.Zero };
            yield return new object[] { mat1, mat1, new float4x4(4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4) };
        }

        public static IEnumerable<object[]> GetMultAffine()
        {
            var mat1 = new float4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1);

            yield return new object[] { mat1, float4x4.Identity, mat1 };
            yield return new object[] { float4x4.Identity, mat1, mat1 };
            yield return new object[] { mat1, float4x4.Zero, new float4x4(0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1) };
            yield return new object[] { float4x4.Zero, mat1, new float4x4(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1) };
            yield return new object[] { mat1, mat1, new float4x4(3, 3, 3, 4, 3, 3, 3, 4, 3, 3, 3, 4, 0, 0, 0, 1) };
        }

        public static IEnumerable<object[]> GetTranspose()
        {
            yield return new object[] { new float4x4(1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0), new float4x4(1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0) };
            yield return new object[] { new float4x4(0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0), new float4x4(0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0) };
            yield return new object[] { new float4x4(0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0), new float4x4(0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0) };
            yield return new object[] { new float4x4(0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1), new float4x4(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1) };
        }
        #endregion
    }
}
