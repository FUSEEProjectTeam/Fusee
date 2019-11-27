using Xunit;
using System.Collections.Generic;

namespace Fusee.Math.Core.Test
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
            var expected = new float3(1, 0, 0);
            var matrix = new float3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);

            var actual = matrix.Column0;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Column1_IsColumn()
        {
            var expected = new float3(0, 1, 0);
            var matrix = new float3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);

            var actual = matrix.Column1;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Column2_IsColumn()
        {
            var expected = new float3(0, 0, 1);
            var matrix = new float3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);

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

            var actual = mat.AsArray;

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
        public void Substract_Static(float3x3 left, float3x3 right, float3x3 expected)
        {
            var actual = float3x3.Substract(left, right);

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

        [Fact]
        public void ToString_IsString()
        {
            var mat = new float3x3(1, 2, 3, 3, 1, 2, 2, 3, 1);

            var actual = mat.ToString();

            Assert.Equal("(1, 2, 3)\n(3, 1, 2)\n(2, 3, 1)", actual);
        }

        //TODO: GetHashCode
        //TODO: Equals(obj)

        #endregion

        #region IEnumerables

        public static IEnumerable<object[]> GetAddition()
        {
            var zero = new float3x3(0, 0, 0, 0, 0, 0, 0, 0, 0);
            var one = new float3x3(1, 1, 1, 1, 1, 1, 1, 1, 1);

            yield return new object[] {zero, one, one};
            yield return new object[] {one, zero, one};
        }

        public static IEnumerable<object[]> GetSubtraction()
        {
            var one = new float3x3(1, 1, 1, 1, 1, 1, 1, 1, 1);
            var zero = new float3x3(0, 0, 0, 0, 0, 0, 0, 0, 0);

            yield return new object[] {one, one, zero};
            yield return new object[] {one, zero, one};
        }

        public static IEnumerable<object[]> GetMultiply()
        {
            var one = new float3x3(1, 1, 1, 1, 1, 1, 1, 1, 1);
            var ident = new float3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);
            var zero = new float3x3(0, 0, 0, 0, 0, 0, 0, 0, 0);

            yield return new object[] {one, ident, one};
            yield return new object[] {one, zero, zero};
            yield return new object[] {one, one, new float3x3(3, 3, 3, 3, 3, 3, 3, 3, 3)};
        }

        public static IEnumerable<object[]> GetTransformFloat3()
        {
            var x = new float3(1, 0, 0);
            var y = new float3(0, 1, 0);
            var z = new float3(0, 0, 1);

            var xRot = new float3x3(new float3(1, 0, 0), new float3(0, 0, -1), new float3(0, 1, 0));
            var yRot = new float3x3(new float3(0, 0, 1), new float3(0, 1, 0), new float3(-1, 0, 0));
            var zRot = new float3x3(new float3(0, -1, 0), new float3(1, 0, 0), new float3(0, 0, 1));

            yield return new object[] {xRot, y, z};
            yield return new object[] {yRot, z, x};
            yield return new object[] {zRot, x, y};
        }

        public static IEnumerable<object[]> GetTransformFloat2()
        {
            var x = new float2(1, 0);
            var y = new float2(0, 1);

            var rot90 = new float3x3(0, -1, 0, 1, 0, 0, 0, 0, 1);
            var rot270 = new float3x3(0, 1, 0, -1, 0, 0, 0, 0, 1);

            yield return new object[] {rot90, x, y};
            yield return new object[] {rot270, x, -y};
        }

        #endregion
    }
}
