﻿using Xunit;
using System;
using System.Collections.Generic;
using Fusee.Math.Core;
using System.Globalization;

namespace Fusee.Test.Math.Core
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
            matrix.Column0 = new float4(3, 3, 3, 3);

            var actual = matrix.Column0;

            Assert.Equal(new float4(3, 3, 3, 3), actual);
        }

        [Fact]
        public void Column1_IsColumn()
        {
            var matrix = float4x4.Identity;
            matrix.Column1 = new float4(3, 3, 3, 3);

            var actual = matrix.Column1;

            Assert.Equal(new float4(3, 3, 3, 3), actual);
        }

        [Fact]
        public void Column2_IsColumn()
        {
            var matrix = float4x4.Identity;
            matrix.Column2 = new float4(3, 3, 3, 3);

            var actual = matrix.Column2;

            Assert.Equal(new float4(3, 3, 3, 3), actual);
        }

        [Fact]
        public void Column3_IsColumn()
        {
            var matrix = float4x4.Identity;
            matrix.Column3 = new float4(3, 3, 3, 3);

            var actual = matrix.Column3;

            Assert.Equal(new float4(3, 3, 3, 3), actual);
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
        public void ToArray_Instance()
        {
            var mat = new float4x4(1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1);

            var actual = mat.ToArray();

            Assert.Equal(new float[] { 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1 }, actual);
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

        #region this

        [Fact]
        public void This_GetWithIdx_IsValid()
        {
            var actual = new float4x4(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);

            Assert.Equal(0, actual[0, 0]);
            Assert.Equal(1, actual[0, 1]);
            Assert.Equal(2, actual[0, 2]);
            Assert.Equal(3, actual[0, 3]);

            Assert.Equal(4, actual[1, 0]);
            Assert.Equal(5, actual[1, 1]);
            Assert.Equal(6, actual[1, 2]);
            Assert.Equal(7, actual[1, 3]);

            Assert.Equal(8, actual[2, 0]);
            Assert.Equal(9, actual[2, 1]);
            Assert.Equal(10, actual[2, 2]);
            Assert.Equal(11, actual[2, 3]);

            Assert.Equal(12, actual[3, 0]);
            Assert.Equal(13, actual[3, 1]);
            Assert.Equal(14, actual[3, 2]);
            Assert.Equal(15, actual[3, 3]);
        }

        [Fact]
        public void This_SetWithIdx_IsValid()
        {
            var actual = float4x4.Zero;
            actual[0, 0] = 0;
            actual[0, 1] = 1;
            actual[0, 2] = 2;
            actual[0, 3] = 3;

            actual[1, 0] = 4;
            actual[1, 1] = 5;
            actual[1, 2] = 6;
            actual[1, 3] = 7;

            actual[2, 0] = 8;
            actual[2, 1] = 9;
            actual[2, 2] = 10;
            actual[2, 3] = 11;

            actual[3, 0] = 12;
            actual[3, 1] = 13;
            actual[3, 2] = 14;
            actual[3, 3] = 15;

            Assert.Equal(0, actual[0, 0]);
            Assert.Equal(1, actual[0, 1]);
            Assert.Equal(2, actual[0, 2]);
            Assert.Equal(3, actual[0, 3]);

            Assert.Equal(4, actual[1, 0]);
            Assert.Equal(5, actual[1, 1]);
            Assert.Equal(6, actual[1, 2]);
            Assert.Equal(7, actual[1, 3]);

            Assert.Equal(8, actual[2, 0]);
            Assert.Equal(9, actual[2, 1]);
            Assert.Equal(10, actual[2, 2]);
            Assert.Equal(11, actual[2, 3]);

            Assert.Equal(12, actual[3, 0]);
            Assert.Equal(13, actual[3, 1]);
            Assert.Equal(14, actual[3, 2]);
            Assert.Equal(15, actual[3, 3]);
        }

        [Theory]
        [MemberData(nameof(ThisException))]
        public void Invalid_GetWithIdx_Exception(int idx, string expected)
        {
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => float4x4.Zero[idx, idx]);

            Assert.Equal(expected, actual.ParamName);
        }

        [Theory]
        [MemberData(nameof(ThisException))]
        public void Invalid_SetWithIdx_Exception(int idx, string expected)
        {
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => { var f4 = float4x4.Zero; f4[idx, idx] = 10; });

            Assert.Equal(expected, actual.ParamName);
        }

        public static IEnumerable<object[]> ThisException()
        {
            yield return new object[] { 4, "Index 4,4 not eligible for a float4x4 type" };
            yield return new object[] { 5, "Index 5,5 not eligible for a float4x4 type" };
            yield return new object[] { -1, "Index -1,-1 not eligible for a float4x4 type" };
        }

        #endregion

        #region Methods

        #region CreateFromAxisAngle

        [Theory]
        [MemberData(nameof(GetAxisAngle))]
        public void CreateFromAxisAngle_MainAxes(float3 axis, float angle, float4x4 expected)
        {
            var actual = float4x4.CreateFromAxisAngle(axis, M.DegreesToRadians(angle));

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

        [Fact]
        public void LookAt_Vectors()
        {
            var eye = new float3(1, 0, 0);
            var target = new float3(0, 0, 0);
            var up = new float3(0, 1, 0);
            var expected = new float4x4(0, 0, 1, 0, 0, 1, 0, 0, -1, 0, 0, 1, 0, 0, 0, 1);

            var actual = float4x4.LookAt(eye, target, up);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LookAtRH_Vectors()
        {
            var eye = new float3(1, 0, 0);
            var target = new float3(0, 0, 0);
            var up = new float3(0, 1, 0);
            var expected = new float4x4(0, 0, -1, 0, 0, 1, 0, 0, 1, 0, 0, -1, 0, 0, 0, 1);

            var actual = float4x4.LookAtRH(eye, target, up);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LookAt_Singles()
        {
            var expected = new float4x4(0, 0, 1, 0, 0, 1, 0, 0, -1, 0, 0, 1, 0, 0, 0, 1);

            var actual = float4x4.LookAt(1, 0, 0, 0, 0, 0, 0, 1, 0);

            Assert.Equal(expected, actual);
        }

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
        public void Subtract_Static(float4x4 left, float4x4 right, float4x4 expected)
        {
            var actual = float4x4.Subtract(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Mult_Static(float4x4 left, float4x4 right, float4x4 expected)
        {
            var actual = float4x4.Mult(left, right);

            Assert.Equal(expected, actual);
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

        [Fact]
        public void InvertAffine_Static()
        {
            var mat = new float4x4(1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1);

            var actual = mat.InvertAffine();

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

        #region TRS Decomposition

        [Theory]
        [MemberData(nameof(GetTRSDecompositionTranslationMtxs))]
        public void TranslationDecompositionMtx(float4x4 mat, float4x4 expected)
        {
            var result = float4x4.TranslationDecomposition(mat);

            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(GetTRSDecompositionTranslationVecs))]
        public void TranslationDecompositionVec(float4x4 mat, float3 expected)
        {
            var result = float4x4.GetTranslation(mat);

            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(GetTRSDecompositionRotationMtxs))]
        public void RotationDecomposition(float4x4 mat, float4x4 expected)
        {
            var result = float4x4.RotationDecomposition(mat);

            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(GetTRSDecompositionScaleMtxs))]
        public void ScaleDecompositionMtx(float4x4 mat, float4x4 expected)
        {
            var result = float4x4.ScaleDecomposition(mat);

            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(GetTRSDecompositionScaleVecs))]
        public void ScaleDecompositionVec(float4x4 mat, float3 expected)
        {
            var result = float4x4.GetScale(mat);

            Assert.Equal(expected, result);
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

        //TODO: GetHasCode
        //TODO: Equals(obj)

        #endregion

        #region IEnumerables

        public static IEnumerable<object[]> GetTRSDecompositionTranslationMtxs()
        {
            yield return new object[] { float4x4.Identity, float4x4.Identity };
            yield return new object[] { new float4x4(2, 0, 0, 0, 0, 4, 0, 0, 0, 0, 5, 0, 0, 0, 0, 1), float4x4.Identity };
            yield return new object[] { new float4x4(2, 0, 0, 5, 0, 3, 0, 6, 0, 0, 4, 7, 0, 0, 0, 1), new float4x4(1, 0, 0, 5, 0, 1, 0, 6, 0, 0, 1, 7, 0, 0, 0, 1) };
            yield return new object[] { new float4x4(1, 2, 3, 5, 2, 3, 1, 6, 3, 2, 1, 7, 0, 0, 0, 1), new float4x4(1, 0, 0, 5, 0, 1, 0, 6, 0, 0, 1, 7, 0, 0, 0, 1) };
        }
        public static IEnumerable<object[]> GetTRSDecompositionTranslationVecs()
        {
            yield return new object[] { new float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1), float3.Zero };
            yield return new object[] { new float4x4(2, 0, 0, 0, 0, 4, 0, 0, 0, 0, 5, 0, 0, 0, 0, 1), float3.Zero };
            yield return new object[] { new float4x4(2, 0, 0, 5, 0, 3, 0, 6, 0, 0, 4, 7, 0, 0, 0, 1), new float3(5, 6, 7) };
            yield return new object[] { new float4x4(1, 2, 3, 5, 2, 3, 1, 6, 3, 2, 1, 7, 0, 0, 0, 1), new float3(5, 6, 7) };
        }
        public static IEnumerable<object[]> GetTRSDecompositionRotationMtxs()
        {
            yield return new object[] { float4x4.Identity, float4x4.Identity };
            yield return new object[] { new float4x4(2, 0, 0, 0, 0, 4, 0, 0, 0, 0, 5, 0, 0, 0, 0, 1), float4x4.Identity };
            yield return new object[] { new float4x4(2, 0, 0, 5, 0, 3, 0, 6, 0, 0, 4, 7, 0, 0, 0, 1), float4x4.Identity };
            yield return new object[] { new float4x4(1, 2, 3, 5, 2, 3, 1, 6, 3, 2, 1, 7, 0, 0, 0, 1), new float4x4(0.2672612f, 0.4850713f, 0.904534f, 0, 0.5345225f, 0.7276069f, 0.3015113f, 0, 0.8017837f, 0.4850713f, 0.3015113f, 0, 0, 0, 0, 1) };
        }
        public static IEnumerable<object[]> GetTRSDecompositionScaleMtxs()
        {
            yield return new object[] { float4x4.Identity, float4x4.Identity };
            yield return new object[] { new float4x4(2, 0, 0, 0, 0, 4, 0, 0, 0, 0, 5, 0, 0, 0, 0, 1), new float4x4(2, 0, 0, 0, 0, 4, 0, 0, 0, 0, 5, 0, 0, 0, 0, 1) };
            yield return new object[] { new float4x4(2, 0, 0, 5, 0, 3, 0, 6, 0, 0, 4, 7, 0, 0, 0, 1), new float4x4(2, 0, 0, 0, 0, 3, 0, 0, 0, 0, 4, 0, 0, 0, 0, 1) };
            yield return new object[] { new float4x4(3, 6, 2, 5, 2, 3, 6, 6, 6, 2, 3, 7, 0, 0, 0, 1), new float4x4(7, 0, 0, 0, 0, 7, 0, 0, 0, 0, 7, 0, 0, 0, 0, 1) };
        }
        public static IEnumerable<object[]> GetTRSDecompositionScaleVecs()
        {
            yield return new object[] { float4x4.Identity, float3.One };
            yield return new object[] { new float4x4(2, 0, 0, 0, 0, 4, 0, 0, 0, 0, 5, 0, 0, 0, 0, 1), new float3(2, 4, 5) };
            yield return new object[] { new float4x4(2, 0, 0, 5, 0, 3, 0, 6, 0, 0, 4, 7, 0, 0, 0, 1), new float3(2, 3, 4) };
            yield return new object[] { new float4x4(3, 6, 2, 5, 2, 3, 6, 6, 6, 2, 3, 7, 0, 0, 0, 1), new float3(7, 7, 7) };
        }

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

        #region ToString/Parse

        [Fact]
        public void ToString_NoCulture()
        {
            Assert.NotNull(new float4x4().ToString());
        }

        [Fact]
        public void ToString_InvariantCulture()
        {
            string s = "(1.5, 0, 0, 0)\n(0, 1.5, 0, 0)\n(0, 0, 1.5, 0)\n(0, 0, 0, 1)";
            float4x4 f = float4x4.Scale(1.5f);

            Assert.Equal(s, f.ToString(CultureInfo.InvariantCulture));
        }

        [Fact]
        public void ToString_CultureDE()
        {
            string s = "(1,5; 0; 0; 0)\n(0; 1,5; 0; 0)\n(0; 0; 1,5; 0)\n(0; 0; 0; 1)";
            float4x4 f = float4x4.Scale(1.5f);

            Assert.Equal(s, f.ToString(new CultureInfo("de-DE")));
        }

        [Fact]
        public void Parse_InvariantCulture()
        {
            string s = "(1.5, 0, 0, 0)\n(0, 1.5, 0, 0)\n(0, 0, 1.5, 0)\n(0, 0, 0, 1)";
            float4x4 f = float4x4.Scale(1.5f);

            Assert.Equal(f, float4x4.Parse(s, CultureInfo.InvariantCulture));
        }

        [Fact]
        public void Parse_CultureDE()
        {
            string s = "(1,5; 0; 0; 0)\n(0; 1,5; 0; 0)\n(0; 0; 1,5; 0)\n(0; 0; 0; 1)";
            float4x4 f = float4x4.Scale(1.5f);

            Assert.Equal(f, float4x4.Parse(s, new CultureInfo("de-DE")));
        }

        [Fact]
        public void Parse_ToString_NoCulture()
        {
            float4x4 f = float4x4.Scale(1.5f);

            Assert.Equal(f, float4x4.Parse(f.ToString()));
        }

        [Fact]
        public void Parse_ToString_InvariantCulture()
        {
            float4x4 f = float4x4.Scale(1.5f);

            Assert.Equal(f, float4x4.Parse(f.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture));
        }

        [Fact]
        public void Parse_ToString_CultureDE()
        {
            float4x4 f = float4x4.Scale(1.5f);

            Assert.Equal(f, float4x4.Parse(f.ToString(new CultureInfo("de-DE")), new CultureInfo("de-DE")));
        }

        [Fact]
        public void Parse_Exception()
        {
            string s = "Fusee";

            Assert.Throws<FormatException>(() => float4x4.Parse(s));
        }

        #endregion ToString/Parse
    }
}
