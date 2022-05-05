using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Fusee.Tests.Math.Core
{
    public class Double3x3Test
    {
        #region Fields

        [Fact]
        public void Zero_IsZero()
        {
            var expected = new double3x3(0, 0, 0, 0, 0, 0, 0, 0, 0);

            Assert.Equal(expected, double3x3.Zero);
        }

        [Fact]
        public void Identity_IsIdentity()
        {
            var expected = new double3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);

            Assert.Equal(expected, double3x3.Identity);
        }

        [Fact]
        public void Row0_IsRow()
        {
            var expected = new double3(1, 0, 0);
            var matrix = new double3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);

            var actual = matrix.Row1;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Row1_IsRow()
        {
            var expected = new double3(0, 1, 0);
            var matrix = new double3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);

            var actual = matrix.Row2;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Row2_IsRow()
        {
            var expected = new double3(0, 0, 1);
            var matrix = new double3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);

            var actual = matrix.Row3;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Contructors

        [Fact]
        public void Constructor_Vectors()
        {
            var expected = double3x3.Identity;

            var actual = new double3x3(new double3(1, 0, 0), new double3(0, 1, 0), new double3(0, 0, 1));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Constructor_Singles()
        {
            var expected = double3x3.Identity;

            var actual = new double3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Constructor_Double4x4()
        {
            var expected = double3x3.Identity;

            var actual = new double3x3(new double4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1));

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Properties

        [Fact]
        public void Determinant_IsZero()
        {
            var matrix = new double3x3(1, 1, 1, 1, 1, 1, 1, 1, 1);

            var actual = matrix.Determinant;

            Assert.Equal(0, actual);
        }

        [Fact]
        public void Column0_IsColumn()
        {
            var expected = new double3(3, 3, 3);
            var matrix = double3x3.Identity;

            matrix.Column1 = expected;

            var actual = matrix.Column1;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Column1_IsColumn()
        {
            var expected = new double3(3, 3, 3);
            var matrix = double3x3.Identity;

            matrix.Column2 = expected;

            var actual = matrix.Column2;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Column2_IsColumn()
        {
            var expected = new double3(3, 3, 3);
            var matrix = double3x3.Identity;

            matrix.Column3 = expected;

            var actual = matrix.Column3;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Instance

        [Fact]
        public void Transpose_Instance()
        {
            var mat = new double3x3(1, 1, 1, 2, 2, 2, 3, 3, 3);

            var actual = mat.Transpose();

            Assert.Equal(new double3x3(1, 2, 3, 1, 2, 3, 1, 2, 3), actual);
        }

        [Fact]
        public void AsArray_Instance()
        {
            var mat = new double3x3(1, 1, 1, 2, 2, 2, 3, 3, 3);

            var actual = mat.ToArray();

            Assert.Equal(new double[] { 1, 1, 1, 2, 2, 2, 3, 3, 3 }, actual);
        }

        #endregion

        #region Arithmetic Functions

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_Static(double3x3 left, double3x3 right, double3x3 expected)
        {
            var actual = double3x3.Add(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_Static(double3x3 left, double3x3 right, double3x3 expected)
        {
            var actual = double3x3.Subtract(left, right);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Multiply

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Mult_Static(double3x3 left, double3x3 right, double3x3 expected)
        {
            var actual = double3x3.Mult(left, right);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Transpose

        [Fact]
        public void Transpose_Static()
        {
            var mat = new double3x3(1, 1, 1, 2, 2, 2, 3, 3, 3);

            var actual = double3x3.Transpose(mat);

            Assert.Equal(new double3x3(1, 2, 3, 1, 2, 3, 1, 2, 3), actual);
        }

        #endregion

        #region Transform

        [Theory]
        [MemberData(nameof(GetTransformDouble3))]
        public void Transform_MatrixDouble3_Static(double3x3 mat, double3 vec, double3 expected)
        {
            var actual = double3x3.Transform(mat, vec);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTransformDouble3))]
        public void Transform_Double3Matrix_Static(double3x3 mat, double3 vec, double3 expected)
        {
            var actual = double3x3.Transform(vec, mat);

            Assert.Equal(expected, -actual);
        }

        [Theory]
        [MemberData(nameof(GetTransformDouble2))]
        public void Transform_MatrixDouble2_Static(double3x3 mat, double2 vec, double2 expected)
        {
            var actual = double3x3.Transform(mat, vec);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTransformDouble2))]
        public void Transform_Double2Matrix_Static(double3x3 mat, double2 vec, double2 expected)
        {
            var actual = double3x3.Transform(vec, mat);

            Assert.Equal(expected, -actual);
        }

        #endregion

        #region Operators

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_Operator(double3x3 left, double3x3 right, double3x3 expected)
        {
            var actual = left + right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_Operator(double3x3 left, double3x3 right, double3x3 expected)
        {
            var actual = left - right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Mult_Operator(double3x3 left, double3x3 right, double3x3 expected)
        {
            var actual = left * right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTransformDouble3))]
        public void Transform_MatrixDouble3_Operator(double3x3 mat, double3 vec, double3 expected)
        {
            var actual = mat * vec;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTransformDouble3))]
        public void Transform_Double3Matrix_Operator(double3x3 mat, double3 vec, double3 expected)
        {
            var actual = vec * mat;

            Assert.Equal(expected, -actual);
        }

        [Theory]
        [MemberData(nameof(GetTransformDouble2))]
        public void Transform_MatrixDouble2_Operator(double3x3 mat, double2 vec, double2 expected)
        {
            var actual = mat * vec;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTransformDouble2))]
        public void Transform_Double2Matrix_Operator(double3x3 mat, double2 vec, double2 expected)
        {
            var actual = vec * mat;

            Assert.Equal(expected, -actual);
        }

        [Fact]
        public void Equality_IsEqual()
        {
            var a = new double3x3(1, 1, 1, 1, 1, 1, 1, 1, 1);
            var b = new double3x3(1, 1, 1, 1, 1, 1, 1, 1, 1);

            Assert.True((a == b));
        }

        [Fact]
        public void Equality_IsInequal()
        {
            var a = new double3x3(1, 1, 1, 1, 1, 1, 1, 1, 1);
            var b = new double3x3(0, 0, 0, 0, 0, 0, 0, 0, 0);

            Assert.False((a == b));
        }

        [Fact]
        public void Inequality_IsEqual()
        {
            var a = new double3x3(1, 1, 1, 1, 1, 1, 1, 1, 1);
            var b = new double3x3(1, 1, 1, 1, 1, 1, 1, 1, 1);

            Assert.False((a != b));
        }

        [Fact]
        public void Inequality_IsInequal()
        {
            var a = new double3x3(1, 1, 1, 1, 1, 1, 1, 1, 1);
            var b = new double3x3(0, 0, 0, 0, 0, 0, 0, 0, 0);

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
            var zero = new double3x3(0, 0, 0, 0, 0, 0, 0, 0, 0);
            var one = new double3x3(1, 1, 1, 1, 1, 1, 1, 1, 1);

            yield return new object[] { zero, one, one };
            yield return new object[] { one, zero, one };
        }

        public static IEnumerable<object[]> GetSubtraction()
        {
            var one = new double3x3(1, 1, 1, 1, 1, 1, 1, 1, 1);
            var zero = new double3x3(0, 0, 0, 0, 0, 0, 0, 0, 0);

            yield return new object[] { one, one, zero };
            yield return new object[] { one, zero, one };
        }

        public static IEnumerable<object[]> GetMultiply()
        {
            var one = new double3x3(1, 1, 1, 1, 1, 1, 1, 1, 1);
            var ident = new double3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);
            var zero = new double3x3(0, 0, 0, 0, 0, 0, 0, 0, 0);

            yield return new object[] { one, ident, one };
            yield return new object[] { one, zero, zero };
            yield return new object[] { one, one, new double3x3(3, 3, 3, 3, 3, 3, 3, 3, 3) };
        }

        public static IEnumerable<object[]> GetTransformDouble3()
        {
            var x = new double3(1, 0, 0);
            var y = new double3(0, 1, 0);
            var z = new double3(0, 0, 1);

            var xRot = new double3x3(new double3(1, 0, 0), new double3(0, 0, -1), new double3(0, 1, 0));
            var yRot = new double3x3(new double3(0, 0, 1), new double3(0, 1, 0), new double3(-1, 0, 0));
            var zRot = new double3x3(new double3(0, -1, 0), new double3(1, 0, 0), new double3(0, 0, 1));

            yield return new object[] { xRot, y, z };
            yield return new object[] { yRot, z, x };
            yield return new object[] { zRot, x, y };
        }

        public static IEnumerable<object[]> GetTransformDouble2()
        {
            var x = new double2(1, 0);
            var y = new double2(0, 1);

            var rot90 = new double3x3(0, -1, 0, 1, 0, 0, 0, 0, 1);
            var rot270 = new double3x3(0, 1, 0, -1, 0, 0, 0, 0, 1);

            yield return new object[] { rot90, x, y };
            yield return new object[] { rot270, x, -y };
        }

        #endregion

        #region ToString/Parse

        [Fact]
        public void ToString_NoCulture()
        {
            Assert.NotNull(new double3x3().ToString());
        }

        [Fact]
        public void ToString_InvariantCulture()
        {
            string s = "(1.5, 0, 0)\n(0, 1.5, 0)\n(0, 0, 1)";
            double3x3 f = new(1.5f, 0, 0, 0, 1.5f, 0, 0, 0, 1);

            Assert.Equal(s, f.ToString(CultureInfo.InvariantCulture));
        }

        [Fact]
        public void ToString_CultureDE()
        {
            string s = "(1,5; 0; 0)\n(0; 1,5; 0)\n(0; 0; 1)";
            double3x3 f = new(1.5f, 0, 0, 0, 1.5f, 0, 0, 0, 1);

            Assert.Equal(s, f.ToString(new CultureInfo("de-DE")));
        }

        [Fact]
        public void Parse_InvariantCulture()
        {
            string s = "(1.5, 0, 0)\n(0, 1.5, 0)\n(0, 0, 1)";
            double3x3 f = new(1.5f, 0, 0, 0, 1.5f, 0, 0, 0, 1);

            Assert.Equal(f, double3x3.Parse(s, CultureInfo.InvariantCulture));
        }

        [Fact]
        public void Parse_CultureDE()
        {
            string s = "(1,5; 0; 0)\n(0; 1,5; 0)\n(0; 0; 1)";
            double3x3 f = new(1.5f, 0, 0, 0, 1.5f, 0, 0, 0, 1);

            Assert.Equal(f, double3x3.Parse(s, new CultureInfo("de-DE")));
        }

        [Fact]
        public void Parse_ToString_NoCulture()
        {
            double3x3 f = new(1.5f, 0, 0, 0, 1.5f, 0, 0, 0, 1);

            Assert.Equal(f, double3x3.Parse(f.ToString()));
        }

        [Fact]
        public void Parse_ToString_InvariantCulture()
        {
            double3x3 f = new(1.5f, 0, 0, 0, 1.5f, 0, 0, 0, 1);

            Assert.Equal(f, double3x3.Parse(f.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture));
        }

        [Fact]
        public void Parse_ToString_CultureDE()
        {
            double3x3 f = new(1.5f, 0, 0, 0, 1.5f, 0, 0, 0, 1);

            Assert.Equal(f, double3x3.Parse(f.ToString(new CultureInfo("de-DE")), new CultureInfo("de-DE")));
        }

        [Fact]
        public void Parse_Exception()
        {
            string s = "Fusee";

            Assert.Throws<FormatException>(() => double3x3.Parse(s));
        }

        #endregion ToString/Parse
    }
}