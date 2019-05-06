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

        #region Constructors
        [Fact]
        public void Constructor_FromDouble4x4()
        {
            var d = new double4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

            var actual = new float4x4(d);

            Assert.Equal(new float4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1), actual);
        }

        [Fact]
        public void Constructor_FromFloat4()
        {
            var actual = new float4x4(new float4(1, 1, 1, 1), new float4(1, 1, 1, 1), new float4(1, 1, 1, 1), new float4(1, 1, 1, 1));

            Assert.Equal(new float4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1), actual);
        }
        #endregion

        #region Properties
        [Fact]
        public void Column0_IsColumn()
        {
            var matrix = float4x4.Identity;

            var actual = matrix.Column0;

            Assert.Equal(new float4(1, 0, 0, 0), actual);
        }

        [Fact]
        public void Column1_IsColumn()
        {
            var matrix = float4x4.Identity;

            var actual = matrix.Column1;

            Assert.Equal(new float4(0, 1, 0, 0), actual);
        }

        [Fact]
        public void Column2_IsColumn()
        {
            var matrix = float4x4.Identity;

            var actual = matrix.Column2;

            Assert.Equal(new float4(0, 0, 1, 0), actual);
        }

        [Fact]
        public void Column3_IsColumn()
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

        #region Instance

        [Fact]
        public void Invert_Instance()
        {
            var mat = new float4x4(1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1);

            var actual = mat.Invert();

            Assert.Equal(new float4x4(1, 0, 0, -1, 0, 1, 0, -1, 0, 0, 1, -1, 0, 0, 0, 1), actual);
        }

        [Fact]
        public void Transpose_Instance()
        {
            var mat = new float4x4(1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1);

            var actual = mat.Transpose();

            Assert.Equal(new float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 1, 1, 1, 1), actual);
        }

        [Fact]
        public void Round_Instance()
        {
            var mat = new float4x4( 1.23456789f, 0, 0, 1.23456712f, 
                                    0, 1.23456789f, 0, 1.23456712f, 
                                    0, 0, 1.23456789f, 1.23456712f, 
                                    0, 0, 0, 1.23456712f);

            var expected = new float4x4(1.234568f, 0, 0, 1.234567f,
                                        0, 1.234568f, 0, 1.234567f,
                                        0, 0, 1.234568f, 1.234567f,
                                        0, 0, 0, 1.234567f);

            var actual = mat.Round();

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Methods

        #region CreateFromAxisAngle

        [Theory]
        [MemberData(nameof(GetAxisAngle))]
        public void CreateFromAxisAngle_MainAxes(float3 axis, float angle, float4x4 expected)
        {
            var actual = float4x4.CreateFromAxisAngle(axis, M.DegreesToRadians(90));

            Assert.Equal(expected, actual);
        }

        #endregion

        #region CreateRotation[XYZ]

        [Fact]
        public void CreateRotationX_90Degrees()
        {
            var expected = new float4x4(1, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1);

            var actual = float4x4.CreateRotationX(M.DegreesToRadians(90));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateRotationY_90Degrees()
        {
            var expected = new float4x4(0, 0, 1, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 0, 1);

            var actual = float4x4.CreateRotationY(M.DegreesToRadians(90));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateRotationZ_90Degrees()
        {
            var expected = new float4x4(0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

            var actual = float4x4.CreateRotationZ(M.DegreesToRadians(90));

            Assert.Equal(expected, actual);
        }

        #endregion

        #region CreateTranslation

        [Fact]
        public void CreateTranslation_Singles()
        {
            var expected = new float4x4(1, 0, 0, 4, 0, 1, 0, 3, 0, 0, 1, 2, 0, 0, 0, 1);

            var actual = float4x4.CreateTranslation(4, 3, 2);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateTranslation_Vector()
        {
            var expected = new float4x4(1, 0, 0, 4, 0, 1, 0, 3, 0, 0, 1, 2, 0, 0, 0, 1);

            var actual = float4x4.CreateTranslation(new float3(4, 3, 2));

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Rotation matrix to euler representation

        [Theory]
        [MemberData(nameof(GetMatrixEuler))]
        public void RotMatToEuler_MainAxes(float4x4 mat, float3 expected)
        {
            var actual = float4x4.RotMatToEuler(mat);

            actual.x = M.RadiansToDegrees(actual.x);
            actual.y = M.RadiansToDegrees(actual.y);
            actual.z = M.RadiansToDegrees(actual.z);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region CreateScale

        [Fact]
        public void CreateScale_Single()
        {
            var expected = new float4x4(2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 1);

            var actual = float4x4.CreateScale(2);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateScale_ThreeSingles()
        {
            var expected = new float4x4(1, 0, 0, 0, 0, 2, 0, 0, 0, 0, 3, 0, 0, 0, 0, 1);

            var actual = float4x4.CreateScale(1, 2, 3);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateScale_Vector()
        {
            var expected = new float4x4(1, 0, 0, 0, 0, 2, 0, 0, 0, 0, 3, 0, 0, 0, 0, 1);

            var actual = float4x4.CreateScale(new float3(1, 2, 3));

            Assert.Equal(expected, actual);
        }

        #endregion

        #region CreateOrthographic

        [Fact]
        public void CreateOrthographic_Test()
        {
            var expected = new float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 2, -3, 0, 0, 0, 1);

            var actual = float4x4.CreateOrthographic(2, 2, 1, 2);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateOrthographicOffCenter_Test()
        {
            var expected = new float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 2, -3, 0, 0, 0, 1);

            var actual = float4x4.CreateOrthographicOffCenter(-1, 1, -1, 1, 1, 2);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateOrthographicOffCenterRH_Test()
        {
            var expected = new float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, -2, -3, 0, 0, 0, 1);

            var actual = float4x4.CreateOrthographicOffCenterRH(-1, 1, -1, 1, 1, 2);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region CreatePerspective

        [Fact]
        public void CreatePerspectiveFieldOfView_Result()
        {
            var expected = new float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 3, -4, 0, 0, 1, 0);

            var actual = float4x4.CreatePerspectiveFieldOfView(M.DegreesToRadians(90), 1, 1, 2);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreatePerspectiveOffCenter_Test()
        {
            var expected = new float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 3, -4, 0, 0, 1, 0);

            var actual = float4x4.CreatePerspectiveOffCenter(-1, 1, -1, 1, 1, 2);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreatePerspectiveOffCenterRH_Test()
        {
            var expected = new float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, -3, -4, 0, 0, -1, 0);

            var actual = float4x4.CreatePerspectiveOffCenterRH(-1, 1, -1, 1, 1, 2);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetPerspectiveFOWException))]
        public void CreatePerspectiveFieldOfView_Exceptions(float fovy, float aspect, float zNear, float zFar, string expected)
        {
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() =>
                float4x4.CreatePerspectiveFieldOfView(fovy, aspect, zNear, zFar));

            Assert.Equal(expected, actual.ParamName);
        }

        [Theory]
        [MemberData(nameof(GetPerspectiveOffCenterException))]
        public void CreatePerspectiveOffCenter_Exceptions(float zNear, float zFar, string expected)
        {
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() =>
                float4x4.CreatePerspectiveOffCenter(-1, 1, -1, 1,  zNear, zFar));

            Assert.Equal(expected, actual.ParamName);
        }

        [Theory]
        [MemberData(nameof(GetPerspectiveOffCenterException))]
        public void CreatePerspectiveOffCenter_ExceptionsRH(float zNear, float zFar, string expected)
        {
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() =>
                float4x4.CreatePerspectiveOffCenterRH(-1, 1, -1, 1, zNear, zFar));

            Assert.Equal(expected, actual.ParamName);
        }

        #endregion

        #region Scale Functions

        [Fact]
        public void Scale_Single()
        {
            var expected = new float4x4(2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 1);

            var actual = float4x4.Scale(2);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Scale_Vector()
        {
            var expected = new float4x4(1, 0, 0, 0, 0, 2, 0, 0, 0, 0, 3, 0, 0, 0, 0, 1);

            var actual = float4x4.Scale(new float3(1, 2, 3));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Scale_ThreeSingles()
        {
            var expected = new float4x4(1, 0, 0, 0, 0, 2, 0, 0, 0, 0, 3, 0, 0, 0, 0, 1);

            var actual = float4x4.Scale(1,2,3);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region CameraHelper

        //TODO: LookAt(float3s)
        //TODO: LookAtRH
        //TODO: LookAt(Singles)

        #endregion

        #region Arithmetic Functions

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_Static(float4x4 left, float4x4 right, float4x4 expected)
        {
            var actual = float4x4.Add(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Substract_Static(float4x4 left, float4x4 right, float4x4 expected)
        {
            var actual = float4x4.Substract(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Mult_Static(float4x4 left, float4x4 right, float4x4 expected)
        {
            var actual = float4x4.Mult(left, right);
        }

        #endregion

        #region Invert

        [Fact]
        public void Invert_Static()
        {
            var mat = new float4x4(1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1);

            var actual = mat.Invert();

            Assert.Equal(new float4x4(1, 0, 0, -1, 0, 1, 0, -1, 0, 0, 1, -1, 0, 0, 0, 1), actual);
        }

        #endregion

        #region Transpose

        [Fact]
        public void Transpose_Static()
        {
            var mat = new float4x4(1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1);

            var actual = mat.Transpose();

            Assert.Equal(new float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 1, 1, 1, 1), actual);
        }

        #endregion

        #region Transform

        [Theory]
        [MemberData(nameof(GetTransform4D))]
        public void Transform_4D_Static(float4x4 mat, float4 vec, float4 expected)
        {
            var actual = float4x4.Transform(mat, vec);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTransform4D))]
        public void TransformPremult_4D_Static(float4x4 mat, float4 vec, float4 expected)
        {
            var actual = float4x4.TransformPremult(vec, mat);

            Assert.Equal(expected, -actual);
        }

        [Theory]
        [MemberData(nameof(GetTransform3D))]
        public void Transform_3D_Static(float4x4 mat, float3 vec, float3 expected)
        {
            var actual = float4x4.Transform(mat, vec);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTransform3D))]
        public void TransformPremult_3D_Static(float4x4 mat, float3 vec, float3 expected)
        {
            var actual = float4x4.TransformPremult(vec, mat);

            Assert.Equal(expected, -actual);
        }

        [Fact]
        public void TransformPerspective_3D()
        {
            var mat = new float4x4(1, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 2);
            var vec = new float3(0, 1, 0);

            var actual = float4x4.TransformPerspective(mat, vec);

            Assert.Equal(new float3(0, 0, 0.5f), actual);
        }

        [Fact]
        public void TransformPerspective_4D()
        {
            var mat = new float4x4(1, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 2);
            var vec = new float4(0, 1, 0, 1);

            var actual = float4x4.TransformPerspective(mat, vec);

            Assert.Equal(new float4(0, 0, 0.5f, 1), actual);
        }

        #endregion

        #region Round

        [Fact]
        public void Round_Static()
        {
            var mat = new float4x4(1.23456789f, 0, 0, 1.23456712f,
                0, 1.23456789f, 0, 1.23456712f,
                0, 0, 1.23456789f, 1.23456712f,
                0, 0, 0, 1.23456712f);

            var expected = new float4x4(1.234568f, 0, 0, 1.234567f,
                0, 1.234568f, 0, 1.234567f,
                0, 0, 1.234568f, 1.234567f,
                0, 0, 0, 1.234567f);

            var actual = mat.Round();

            Assert.Equal(expected, actual);
        }

        #endregion

        #endregion

        #region Operators

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_Operator(float4x4 left, float4x4 right, float4x4 expected)
        {
            var actual = left + right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_Operator(float4x4 left, float4x4 right, float4x4 expected)
        {
            var actual = left - right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Mult_Operator(float4x4 left, float4x4 right, float4x4 expected)
        {
            var actual = left * right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTransform4D))]
        public void Transform_4D_Operator(float4x4 mat, float4 vec, float4 expected)
        {
            var actual = mat * vec;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTransform4D))]
        public void TransformPremult_4D_Operator(float4x4 mat, float4 vec, float4 expected)
        {
            var actual = vec * mat;

            Assert.Equal(expected, -actual);
        }

        [Theory]
        [MemberData(nameof(GetTransform3D))]
        public void Transform_3D_Operator(float4x4 mat, float3 vec, float3 expected)
        {
            var actual = mat * vec;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTransform3D))]
        public void TransformPremult_3D_Operator(float4x4 mat, float3 vec, float3 expected)
        {
            var actual = vec * mat;

            Assert.Equal(expected, -actual);
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
        public void Cast_FromDouble4x4()
        {
            var mat = new double4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

            var actual = (float4x4) mat;

            Assert.Equal(new float4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1), actual);
        }

        #endregion

        #region Overrides

        [Fact]
        public void ToString_IsString()
        {
            var mat = new float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

            var actual = mat.ToString();

            Assert.Equal("(1, 0, 0, 0)\n(0, 1, 0, 0)\n(0, 0, 1, 0)\n(0, 0, 0, 1)", actual);
        }

        //TODO: GetHasCode
        //TODO: Equals(obj)

        #endregion

        #region IEnumerables

        public static IEnumerable<object[]> GetAxisAngle()
        {
            yield return new object[]
                {new float3(1, 0, 0), 90, new float4x4(1, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1)};
            yield return new object[]
                {new float3(0, 1, 0), 90, new float4x4(0, 0, 1, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 0, 1)};
            yield return new object[]
                {new float3(0, 0, 1), 90, new float4x4(0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1)};
        }

        public static IEnumerable<object[]> GetMatrixEuler()
        {
            yield return new object[]
                {new float4x4(1, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1), new float3(90, 0, 0)};
            yield return new object[]
                {new float4x4(0, 0, 1, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 0, 1), new float3(0, 90, 0)};
            yield return new object[]
                {new float4x4(0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1), new float3(0, 0, 90)};
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

        public static IEnumerable<object[]> GetPerspectiveOffCenterException()
        {
            yield return new object[] { 0, 2, "zNear" };
            yield return new object[] { 1, 0, "zFar" };
            yield return new object[] { 2, 1, "zNear" };
        }

        public static IEnumerable<object[]> GetAddition()
        {
            var zero = new float4x4(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            var one = new float4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

            yield return new object[] {zero, one, one};
            yield return new object[] {one, zero, one};
            yield return new object[] {zero, zero, zero};
            yield return new object[] {one, one, new float4x4(2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2)};
        }

        public static IEnumerable<object[]> GetSubtraction()
        {
            var zero = new float4x4(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            var one = new float4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

            yield return new object[] {one, zero, one};
            yield return new object[] {zero, zero, zero};
            yield return new object[] {one, one, zero};
        }

        public static IEnumerable<object[]> GetMultiply()
        {
            var zero = new float4x4(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            var one = new float4x4(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
            var id = new float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

            yield return new object[] {one, zero, zero};
            yield return new object[] {zero, one, zero};
            yield return new object[] {one, one, new float4x4(4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4)};
            yield return new object[] {one, id, one};
            yield return new object[] {id, one, one};
        }

        public static IEnumerable<object[]> GetTransform4D()
        {
            var x = new float4(1, 0, 0, 0);
            var y = new float4(0, 1, 0, 0);
            var z = new float4(0, 0, 1, 0);

            var xRot = new float4x4(1, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1);
            var yRot = new float4x4(0, 0, 1, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 0, 1);
            var zRot = new float4x4(0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

            yield return new object[] {xRot, y, z};
            yield return new object[] {yRot, z, x};
            yield return new object[] {zRot, x, y};
        }

        public static IEnumerable<object[]> GetTransform3D()
        {
            var x = new float3(1, 0, 0);
            var y = new float3(0, 1, 0);
            var z = new float3(0, 0, 1);

            var xRot = new float4x4(1, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1);
            var yRot = new float4x4(0, 0, 1, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 0, 1);
            var zRot = new float4x4(0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

            yield return new object[] { xRot, y, z };
            yield return new object[] { yRot, z, x };
            yield return new object[] { zRot, x, y };
        }

        #endregion
    }
}
