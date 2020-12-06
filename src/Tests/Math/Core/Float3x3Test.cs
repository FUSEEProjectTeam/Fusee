using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Fusee.Test.Math.Core
{
    public class Float3x3Test
    {
        #region Fields

        [Fact]
        public void Zero_IsZero()
        {
            var expected = new float3x3(0, 0, 0, 0, 0, 0, 0, 0, 0);

            Assert.Equal(expected, float3x3.Zero);
        }

        [Fact]
        public void Identity_IsIdentity()
        {
            var expected = new float3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);

            Assert.Equal(expected, float3x3.Identity);
        }

        [Fact]
        public void Row0_IsRow()
        {
            var expected = new float3(1, 0, 0);
            var matrix = new float3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);

            var actual = matrix.Row0;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Row1_IsRow()
        {
            var expected = new float3(0, 1, 0);
            var matrix = new float3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);

            var actual = matrix.Row1;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Row2_IsRow()
        {
            var expected = new float3(0, 0, 1);
            var matrix = new float3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);

            var actual = matrix.Row2;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Contructors

        [Fact]
        public void Constructor_Vectors()
        {
            var expected = float3x3.Identity;

            var actual = new float3x3(new float3(1, 0, 0), new float3(0, 1, 0), new float3(0, 0, 1));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Constructor_Singles()
        {
            var expected = float3x3.Identity;

            var actual = new float3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Constructor_Float4x4()
        {
            var expected = float3x3.Identity;

            var actual = new float3x3(new float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1));

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Properties

        [Fact]
        public void Determinant_IsZero()
        {
            var matrix = new float3x3(1, 1, 1, 1, 1, 1, 1, 1, 1);

            var actual = matrix.Determinant;

            Assert.Equal(0, actual);
        }

        [Fact]
        public void Column0_IsColumn()
        {
            var expected = new float3(3, 3, 3);
            var matrix = float3x3.Identity;

            matrix.Column0 = expected;

            var actual = matrix.Column0;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Column1_IsColumn()
        {
            var expected = new float3(3, 3, 3);
            var matrix = float3x3.Identity;

            matrix.Column1 = expected;

            var actual = matrix.Column1;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Column2_IsColumn()
        {
            var expected = new float3(3, 3, 3);
            var matrix = float3x3.Identity;

            matrix.Column2 = expected;

            var actual = matrix.Column2;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Instance

        [Fact]
        public void Transpose_Instance()
        {
            var mat = new float3x3(1, 1, 1, 2, 2, 2, 3, 3, 3);

            var actual = mat.Transpose();

            Assert.Equal(new float3x3(1, 2, 3, 1, 2, 3, 1, 2, 3), actual);
        }

        [Fact]
        public void AsArray_Instance()
        {
            var mat = new float3x3(1, 1, 1, 2, 2, 2, 3, 3, 3);

            var actual = mat.ToArray();

            Assert.Equal(new float[] { 1, 1, 1, 2, 2, 2, 3, 3, 3 }, actual);
        }

        #endregion

        #region Arithmetic Functions

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_Static(float3x3 left, float3x3 right, float3x3 expected)
        {
            var actual = float3x3.Add(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_Static(float3x3 left, float3x3 right, float3x3 expected)
        {
            var actual = float3x3.Subtract(left, right);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Multiply

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Mult_Static(float3x3 left, float3x3 right, float3x3 expected)
        {
            var actual = float3x3.Mult(left, right);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Transpose

        [Fact]
        public void Transpose_Static()
        {
            var mat = new float3x3(1, 1, 1, 2, 2, 2, 3, 3, 3);

            var actual = float3x3.Transpose(mat);

            Assert.Equal(new float3x3(1, 2, 3, 1, 2, 3, 1, 2, 3), actual);
        }

        #endregion

        #region Transform

        [Theory]
        [MemberData(nameof(GetTransformFloat3))]
        public void Transform_MatrixFloat3_Static(float3x3 mat, float3 vec, float3 expected)
        {
            var actual = float3x3.Transform(mat, vec);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTransformFloat3))]
        public void Transform_Float3Matrix_Static(float3x3 mat, float3 vec, float3 expected)
        {
            var actual = float3x3.Transform(vec, mat);

            Assert.Equal(expected, -actual);
        }

        [Theory]
        [MemberData(nameof(GetTransformFloat2))]
        public void Transform_MatrixFloat2_Static(float3x3 mat, float2 vec, float2 expected)
        {
            var actual = float3x3.Transform(mat, vec);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTransformFloat2))]
        public void Transform_Float2Matrix_Static(float3x3 mat, float2 vec, float2 expected)
        {
            var actual = float3x3.Transform(vec, mat);

            Assert.Equal(expected, -actual);
        }

        #endregion

        #region Operators

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_Operator(float3x3 left, float3x3 right, float3x3 expected)
        {
            var actual = left + right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_Operator(float3x3 left, float3x3 right, float3x3 expected)
        {
            var actual = left - right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Mult_Operator(float3x3 left, float3x3 right, float3x3 expected)
        {
            var actual = left * right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTransformFloat3))]
        public void Transform_MatrixFloat3_Operator(float3x3 mat, float3 vec, float3 expected)
        {
            var actual = mat * vec;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTransformFloat3))]
        public void Transform_Float3Matrix_Operator(float3x3 mat, float3 vec, float3 expected)
        {
            var actual = vec * mat;

            Assert.Equal(expected, -actual);
        }

        [Theory]
        [MemberData(nameof(GetTransformFloat2))]
        public void Transform_MatrixFloat2_Operator(float3x3 mat, float2 vec, float2 expected)
        {
            var actual = mat * vec;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTransformFloat2))]
        public void Transform_Float2Matrix_Operator(float3x3 mat, float2 vec, float2 expected)
        {
            var actual = vec * mat;

            Assert.Equal(expected, -actual);
        }

        [Fact]
        public void Equality_IsEqual()
        {
            var a = new float3x3(1, 1, 1, 1, 1, 1, 1, 1, 1);
            var b = new float3x3(1, 1, 1, 1, 1, 1, 1, 1, 1);

            Assert.True((a == b));
        }

        [Fact]
        public void Equality_IsInequal()
        {
            var a = new float3x3(1, 1, 1, 1, 1, 1, 1, 1, 1);
            var b = new float3x3(0, 0, 0, 0, 0, 0, 0, 0, 0);

            Assert.False((a == b));
        }

        [Fact]
        public void Inequality_IsEqual()
        {
            var a = new float3x3(1, 1, 1, 1, 1, 1, 1, 1, 1);
            var b = new float3x3(1, 1, 1, 1, 1, 1, 1, 1, 1);

            Assert.False((a != b));
        }

        [Fact]
        public void Inequality_IsInequal()
        {
            var a = new float3x3(1, 1, 1, 1, 1, 1, 1, 1, 1);
            var b = new float3x3(0, 0, 0, 0, 0, 0, 0, 0, 0);

            Assert.True((a != b));
        }

        #endregion

        #region Overrides

        //TODO: GetHashCode
        //TODO: Equals(obj)

        #endregion

        #region IEnumerables

        public static IEnumerable<object[]> GetAddition()
        {
            var zero = new float3x3(0, 0, 0, 0, 0, 0, 0, 0, 0);
            var one = new float3x3(1, 1, 1, 1, 1, 1, 1, 1, 1);

            yield return new object[] { zero, one, one };
            yield return new object[] { one, zero, one };
        }

        public static IEnumerable<object[]> GetSubtraction()
        {
            var one = new float3x3(1, 1, 1, 1, 1, 1, 1, 1, 1);
            var zero = new float3x3(0, 0, 0, 0, 0, 0, 0, 0, 0);

            yield return new object[] { one, one, zero };
            yield return new object[] { one, zero, one };
        }

        public static IEnumerable<object[]> GetMultiply()
        {
            var one = new float3x3(1, 1, 1, 1, 1, 1, 1, 1, 1);
            var ident = new float3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);
            var zero = new float3x3(0, 0, 0, 0, 0, 0, 0, 0, 0);

            yield return new object[] { one, ident, one };
            yield return new object[] { one, zero, zero };
            yield return new object[] { one, one, new float3x3(3, 3, 3, 3, 3, 3, 3, 3, 3) };
        }

        public static IEnumerable<object[]> GetTransformFloat3()
        {
            var x = new float3(1, 0, 0);
            var y = new float3(0, 1, 0);
            var z = new float3(0, 0, 1);

            var xRot = new float3x3(new float3(1, 0, 0), new float3(0, 0, -1), new float3(0, 1, 0));
            var yRot = new float3x3(new float3(0, 0, 1), new float3(0, 1, 0), new float3(-1, 0, 0));
            var zRot = new float3x3(new float3(0, -1, 0), new float3(1, 0, 0), new float3(0, 0, 1));

            yield return new object[] { xRot, y, z };
            yield return new object[] { yRot, z, x };
            yield return new object[] { zRot, x, y };
        }

        public static IEnumerable<object[]> GetTransformFloat2()
        {
            var x = new float2(1, 0);
            var y = new float2(0, 1);

            var rot90 = new float3x3(0, -1, 0, 1, 0, 0, 0, 0, 1);
            var rot270 = new float3x3(0, 1, 0, -1, 0, 0, 0, 0, 1);

            yield return new object[] { rot90, x, y };
            yield return new object[] { rot270, x, -y };
        }

        #endregion

        #region ToString/Parse

        [Fact]
        public void ToString_NoCulture()
        {
            Assert.NotNull(new float3x3().ToString());
        }

        [Fact]
        public void ToString_InvariantCulture()
        {
            string s = "(1.5, 0, 0)\n(0, 1.5, 0)\n(0, 0, 1)";
            float3x3 f = new float3x3(1.5f, 0, 0, 0, 1.5f, 0, 0, 0, 1);

            Assert.Equal(s, f.ToString(CultureInfo.InvariantCulture));
        }

        [Fact]
        public void ToString_CultureDE()
        {
            string s = "(1,5; 0; 0)\n(0; 1,5; 0)\n(0; 0; 1)";
            float3x3 f = new float3x3(1.5f, 0, 0, 0, 1.5f, 0, 0, 0, 1);

            Assert.Equal(s, f.ToString(new CultureInfo("de-DE")));
        }

        [Fact]
        public void Parse_InvariantCulture()
        {
            string s = "(1.5, 0, 0)\n(0, 1.5, 0)\n(0, 0, 1)";
            float3x3 f = new float3x3(1.5f, 0, 0, 0, 1.5f, 0, 0, 0, 1);

            Assert.Equal(f, float3x3.Parse(s, CultureInfo.InvariantCulture));
        }

        [Fact]
        public void Parse_CultureDE()
        {
            string s = "(1,5; 0; 0)\n(0; 1,5; 0)\n(0; 0; 1)";
            float3x3 f = new float3x3(1.5f, 0, 0, 0, 1.5f, 0, 0, 0, 1);

            Assert.Equal(f, float3x3.Parse(s, new CultureInfo("de-DE")));
        }

        [Fact]
        public void Parse_ToString_NoCulture()
        {
            float3x3 f = new float3x3(1.5f, 0, 0, 0, 1.5f, 0, 0, 0, 1);

            Assert.Equal(f, float3x3.Parse(f.ToString()));
        }

        [Fact]
        public void Parse_ToString_InvariantCulture()
        {
            float3x3 f = new float3x3(1.5f, 0, 0, 0, 1.5f, 0, 0, 0, 1);

            Assert.Equal(f, float3x3.Parse(f.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture));
        }

        [Fact]
        public void Parse_ToString_CultureDE()
        {
            float3x3 f = new float3x3(1.5f, 0, 0, 0, 1.5f, 0, 0, 0, 1);

            Assert.Equal(f, float3x3.Parse(f.ToString(new CultureInfo("de-DE")), new CultureInfo("de-DE")));
        }

        [Fact]
        public void Parse_Exception()
        {
            string s = "Fusee";

            Assert.Throws<FormatException>(() => float3x3.Parse(s));
        }

        #endregion ToString/Parse
    }
}