using System;
using Xunit;
using System.Collections.Generic;
using Fusee.Math.Core;
using System.Globalization;

namespace Fusee.Test.Math.Core
{
    public class Float3Test
    {
        #region Fields

        [Fact]
        public void UnitX_IsUnit()
        {
            var expected = new float3(1, 0, 0);

            Assert.Equal(expected, float3.UnitX);
        }

        [Fact]
        public void UnitY_IsUnit()
        {
            var expected = new float3(0, 1, 0);

            Assert.Equal(expected, float3.UnitY);
        }

        [Fact]
        public void UnitZ_IsUnit()
        {
            var expected = new float3(0, 0, 1);

            Assert.Equal(expected, float3.UnitZ);
        }

        [Fact]
        public void Zero_IsZero()
        {
            var expected = new float3(0, 0, 0);

            Assert.Equal(expected, float3.Zero);
        }

        [Fact]
        public void One_IsOne()
        {
            var expected = new float3(1, 1, 1);

            Assert.Equal(expected, float3.One);
        }

        #endregion

        #region Constructors

        [Fact]
        public void Constructor_FromFloat2()
        {
            var f2 = new float2(1, 2);

            var actual = new float3(f2);

            Assert.Equal(new float3(1, 2, 0), actual);
        }

        [Fact]
        public void Constructor_FromFloat3()
        {
            var f3 = new float3(1, 2, 3);

            var actual = new float3(f3);

            Assert.Equal(new float3(1, 2, 3), actual);
        }

        [Fact]
        public void Constructor_FromFloat4()
        {
            var f4 = new float4(1, 2, 3, 4);

            var actual = new float3(f4);

            Assert.Equal(new float3(1, 2, 3), actual);
        }

        [Fact]
        public void Constructor_FromDouble3()
        {
            var d3 = new double3(1, 2, 3);

            var actual = new float3(d3);

            Assert.Equal(new float3(1, 2, 3), actual);
        }

        #endregion

        #region Instance

        [Fact]
        public void Length_Is3()
        {
            var vec = new float3(1, 2, 2);

            var actual = vec.Length;

            Assert.Equal(3, actual);
        }

        [Fact]
        public void LengthSquared_Is3()
        {
            var vec = new float3(1, 1, 1);

            var actual = vec.LengthSquared;

            Assert.Equal(3, actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Instance(float3 vec, float3 expected)
        {
            var actual = vec.Normalize();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void NormalizeFast_Instance(float3 vec, float3 expected)
        {
            var actual = vec.NormalizeFast();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToArray_IsArray()
        {
            var vec = new float3(1, 2, 3);

            var actual = vec.ToArray();

            Assert.Equal(new float[] {1, 2, 3}, actual);
        }

        #endregion

        #region this

        [Fact]
        public void This_GetWithIdx_IsValid()
        {
            var actual = new float3(3, 7, 8);

            Assert.Equal(3, actual[0]);

            Assert.Equal(7, actual[1]);

            Assert.Equal(8, actual[2]);
        }

        [Fact]
        public void This_SetWithIdx_IsValid()
        {
            var actual = new float3(0, 0, 0);
            actual[0] = 3;
            actual[1] = 7;
            actual[2] = 8;

            Assert.Equal(3, actual[0]);

            Assert.Equal(7, actual[1]);

            Assert.Equal(8, actual[2]);
        }

        [Theory]
        [MemberData(nameof(ThisException))]
        public void Invalid_GetWithIdx_Exception(int idx, string expected)
        {
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => new float3(0, 0, 0)[idx]);

            Assert.Equal(expected, actual.ParamName);
        }

        [Theory]
        [MemberData(nameof(ThisException))]
        public void Invalid_SetWithIdx_Exception(int idx, string expected)
        {
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => { var f3 = new float3(0, 0, 0); f3[idx] = 10; });

            Assert.Equal(expected, actual.ParamName);
        }

        public static IEnumerable<object[]> ThisException()
        {
            yield return new object[] { 7, "Index 7 not eligible for a float3 type" };
            yield return new object[] { 6, "Index 6 not eligible for a float3 type" };
        }

        #endregion

        #region Methods

        #region Arithmetic Functions

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_TwoVectors_Static(float3 left, float3 right, float3 expected)
        {
            var actual = float3.Add(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_VectorScalar_Static(float3 left, float3 right, float3 expected)
        {
            var actual = float3.Add(left, right.x);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_TwoVectors_Static(float3 left, float3 right, float3 expected)
        {
            var actual = float3.Subtract(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_VectorScalar_Static(float3 left, float3 right, float3 expected)
        {
            var actual = float3.Subtract(left, right.x);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_VectorScalar_Static(float3 vec, float scale, float3 expected)
        {
            var actual = float3.Multiply(vec, scale);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multitply_TwoVectors_Static(float3 left, float scale, float3 expected)
        {
            var right = new float3(scale, scale, scale);

            var actual = float3.Multiply(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivide))]
        public void Divide_VectorScalar_Static(float3 vec, float scale, float3 expected)
        {
            var actual = float3.Divide(vec, scale);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivide))]
        public void Divide_TwoVectors_Static(float3 left, float scale, float3 expected)
        {
            var right = new float3(scale, scale, scale);

            var actual = float3.Divide(left, right);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region MinMax

        [Theory]
        [MemberData(nameof(GetComponentMin))]
        public void ComponentMin_IsZero(float3 left, float3 right, float3 expected)
        {
            var actual = float3.ComponentMin(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetComponentMax))]
        public void ComponentMax_IsOne(float3 left, float3 right, float3 expected)
        {
            var actual = float3.ComponentMax(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMin))]
        public void Min_IsZero(float3 left, float3 right, float3 expected)
        {
            var actual = float3.Min(left, right);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMax))]
        public void Max_IsOne(float3 left, float3 right, float3 expected)
        {
            var actual = float3.Max(left, right);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Clamp

        [Theory]
        [MemberData(nameof(GetClamp))]
        public void Clamp_TestClamp(float3 vec, float3 min, float3 max, float3 expected)
        {
            var actual = float3.Clamp(vec, min, max);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Normalize

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void Normalize_Static(float3 vec, float3 expected)
        {
            var actual = float3.Normalize(vec);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetNormalize))]
        public void NormalizeFast_Static(float3 vec, float3 expected)
        {
            var actual = float3.NormalizeFast(vec);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Dot

        [Fact]
        public void Dot_Is10()
        {
            var left = new float3(1, 2, 3);
            var right = new float3(3, 2, 1);

            var actual = float3.Dot(left, right);

            Assert.Equal(10, actual);
        }

        #endregion

        #region Cross

        [Fact]
        public void Cross_IsVector()
        {
            var left = new float3(1, 2, 3);
            var right = new float3(3, 2, 1);

            var actual = float3.Cross(left, right);

            Assert.Equal(new float3(-4, 8, -4), actual);
        }

        #endregion

        #region Lerp

        [Theory]
        [MemberData(nameof(GetLerp))]
        public void Lerp_TestLerp(float3 left, float3 right, float blend, float3 expected)
        {
            var actual = float3.Lerp(left, right, blend);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Barycentric

        [Theory]
        [MemberData(nameof(GetBarycentric))]
        public void Barycentric_Edges(float3 a, float3 b, float3 c, float u, float v, float3 expected)
        {
            var actual = float3.Barycentric(a, b, c, u, v);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetBarycentric))]
        public void GetBarycentric_Edges(float3 a, float3 b, float3 c, float uExpected, float vExpected, float3 point)
        {
            float uActual;
            float vActual;

            float3.GetBarycentric(a, b, c, point, out uActual, out vActual);

            Assert.Equal(uExpected, uActual);
            Assert.Equal(vExpected, vActual);
        }

        #endregion

        #region CalculateAngle

        [Fact]
        public void CalculateAngle_90Degrees()
        {
            var x = new float3(1, 0, 0);
            var y = new float3(0, 1, 0);

            var actual = float3.CalculateAngle(x, y);

            Assert.Equal(M.DegreesToRadians(90), actual);
        }

        #endregion

        #region Rotate

        [Theory]
        [MemberData(nameof(GetEuler))]
        public void Rotate_Euler(float3 euler, float3 vec, float3 expected)
        {
            var actual = float3.Rotate(euler, vec, true);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetQuaternion))]
        public void Rotate_Quaternion(Quaternion quat, float3 vec, float3 expected)
        {
            var actual = float3.Rotate(quat, vec);

            Assert.Equal(expected, actual);
        }

        #endregion

        #endregion

        #region Swizzle

        [Fact]
        public void Swizzle_Float2_Get()
        {
            var actual = new float3(1, 2, 3);

            Assert.Equal(new float2(1, 2), actual.xy);
            Assert.Equal(new float2(1, 3), actual.xz);
            Assert.Equal(new float2(2, 1), actual.yx);
            Assert.Equal(new float2(2, 3), actual.yz);
            Assert.Equal(new float2(3, 1), actual.zx);
            Assert.Equal(new float2(3, 2), actual.zy);
        }

        [Fact]
        public void Swizzle_Float2_Set()
        {
            var actual = new float3();

            actual.xy = new float2(1, 2);
            Assert.Equal(new float3(1, 2, 0), actual);

            actual.xz = new float2(3, 1);
            Assert.Equal(new float3(3, 2, 1), actual);

            actual.yx = new float2(3, 2);
            Assert.Equal(new float3(2, 3, 1), actual);

            actual.yz = new float2(1, 3);
            Assert.Equal(new float3(2, 1, 3), actual);

            actual.zx = new float2(2, 3);
            Assert.Equal(new float3(3, 1, 2), actual);

            actual.zy = new float2(1, 2);
            Assert.Equal(new float3(3, 2, 1), actual);
        }

        [Fact]
        public void Swizzle_Float3_Get()
        {
            var actual = new float3(1, 2, 3);

            Assert.Equal(new float3(1, 2, 3), actual.xyz);
            Assert.Equal(new float3(1, 3, 2), actual.xzy);
            Assert.Equal(new float3(2, 3, 1), actual.yzx);
            Assert.Equal(new float3(2, 1, 3), actual.yxz);
            Assert.Equal(new float3(3, 1, 2), actual.zxy);
            Assert.Equal(new float3(3, 2, 1), actual.zyx);
        }

        [Fact]
        public void Swizzle_Float3_Set()
        {
            var actual = new float3();

            actual.xyz = new float3(1, 2, 3);
            Assert.Equal(new float3(1, 2, 3), actual);

            actual.xzy = new float3(1, 2, 3);
            Assert.Equal(new float3(1, 3, 2), actual);

            actual.yzx = new float3(1, 2, 3);
            Assert.Equal(new float3(3, 1, 2), actual);

            actual.yxz = new float3(1, 2, 3);
            Assert.Equal(new float3(2, 1, 3), actual);

            actual.zxy = new float3(1, 2, 3);
            Assert.Equal(new float3(2, 3, 1), actual);

            actual.zyx = new float3(1, 2, 3);
            Assert.Equal(new float3(3, 2, 1), actual);
        }

        #endregion

        #region Operators

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_TwoVectors_Operator(float3 left, float3 right, float3 expected)
        {
            var actual = left + right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_VectorScalar_Operator(float3 left, float3 right, float3 expected)
        {
            var actual = left + right.x;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_TwoVectors_Operator(float3 left, float3 right, float3 expected)
        {
            var actual = left - right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_VectorScalar_Operator(float3 left, float3 right, float3 expected)
        {
            var actual = left - right.x;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UnaryNegation_IsNegative()
        {
            var vec = new float3(1, 1, 1);

            var actual = -vec;

            Assert.Equal(new float3(-1, -1, -1), actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_VectorScalar_Operator(float3 vec, float scale, float3 expected)
        {
            var actual = vec * scale;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_ScalarVector_Operator(float3 vec, float scale, float3 expected)
        {
            var actual = scale * vec;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_TwoVectors_Operator(float3 left, float scale, float3 expected)
        {
            var right = new float3(scale, scale, scale);

            var actual = left * right;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivide))]
        public void Divide_Operator(float3 vec, float scale, float3 expected)
        {
            var actual = vec / scale;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Equality_IsEqual()
        {
            var a = new float3(1, 1, 1);
            var b = new float3(1, 1, 1);

            Assert.True(a == b);
        }

        [Fact]
        public void Equality_IsInequal()
        {
            var a = new float3(1, 1, 1);
            var b = new float3(0, 0, 0);

            Assert.False(a == b);
        }

        [Fact]
        public void Inequality_IsEqual()
        {
            var a = new float3(1, 1, 1);
            var b = new float3(1, 1, 1);

            Assert.False(a != b);
        }

        [Fact]
        public void Inequality_IsInequal()
        {
            var a = new float3(1, 1, 1);
            var b = new float3(0, 0, 0);

            Assert.True(a != b);
        }

        [Fact]
        public void Cast_FromDouble3()
        {
            var vec = new double3(1, 2, 3);

            var actual = (float3)vec;

            Assert.Equal(new float3(1, 2, 3), actual);
        }

        #endregion

        #region Overrides

        //TODO: GetHashCode
        //TODO: Equals(obj)

        #endregion

        #region Color

        [Fact]
        public void Color_Get()
        {
            var vec = new float3(1, 2, 3);

            Assert.Equal(1, vec.r);
            Assert.Equal(2, vec.g);
            Assert.Equal(3, vec.b);
        }

        [Fact]
        public void Color_Set()
        {
            var actual = new float3();

            actual.r = 1;
            Assert.Equal(new float3(1, 0, 0), actual);

            actual.g = 2;
            Assert.Equal(new float3(1, 2, 0), actual);

            actual.b = 3;
            Assert.Equal(new float3(1, 2, 3), actual);
        }

        #endregion

        #region IEnumerables

        public static IEnumerable<object[]> GetNormalize()
        {
            yield return new object[] {new float3(4, 0, 0), new float3(1, 0, 0)};
            yield return new object[] {new float3(0, 4, 0), new float3(0, 1, 0)};
            yield return new object[] {new float3(0, 0, 4), new float3(0, 0, 1)};
            yield return new object[]
            {
                new float3(1, 1, 1),
                new float3((float)System.Math.Sqrt(1d / 3d), (float)System.Math.Sqrt(1d / 3d), (float)System.Math.Sqrt(1d / 3d))
            };
        }

        public static IEnumerable<object[]> GetAddition()
        {
            var zero = new float3(0, 0,0);
            var one = new float3(1, 1, 1);

            yield return new object[] {one, zero, one};
            yield return new object[] {zero, one, one};
            yield return new object[] {one, one, new float3(2, 2, 2)};
        }

        public static IEnumerable<object[]> GetSubtraction()
        {
            var zero = new float3(0, 0, 0);
            var one = new float3(1, 1, 1);

            yield return new object[] {one, one, zero};
            yield return new object[] {one, zero, one};
            yield return new object[] {zero, one, -one};
        }

        public static IEnumerable<object[]> GetMultiply()
        {
            var one = new float3(1, 1, 1);

            yield return new object[] {one, 1, one};
            yield return new object[] {one, 2, new float3(2, 2, 2)};
            yield return new object[] {one, 0, new float3(0, 0, 0)};
        }

        public static IEnumerable<object[]> GetDivide()
        {
            var one = new float3(1, 1, 1);

            yield return new object[] {new float3(2, 2, 2), 2, one};
            yield return new object[] {one, 1, one};
        }

        public static IEnumerable<object[]> GetComponentMin()
        {
            var one = new float3(1, 1, 1);
            var zero = new float3(0, 0, 0);

            yield return new object[] {one, zero, zero};
            yield return new object[] {zero, one, zero};
            yield return new object[] {new float3(1, 0, 1), new float3(0, 1, 0), zero};
        }

        public static IEnumerable<object[]> GetComponentMax()
        {
            var one = new float3(1, 1, 1);
            var zero = new float3(0, 0, 0);

            yield return new object[] {one, zero, one};
            yield return new object[] {zero, one, one};
            yield return new object[] {new float3(1, 0, 1), new float3(0, 1, 0), one};
        }

        public static IEnumerable<object[]> GetMin()
        {
            var one = new float3(1, 1, 1);
            var zero = new float3(0, 0, 0);

            yield return new object[] {one, zero, zero};
            yield return new object[] {zero, one, zero};
        }

        public static IEnumerable<object[]> GetMax()
        {
            var one = new float3(1, 1, 1);
            var zero = new float3(0, 0, 0);

            yield return new object[] {one, zero, one};
            yield return new object[] {zero, one, one};
        }

        public static IEnumerable<object[]> GetClamp()
        {
            var one = new float3(1, 1, 1);
            var zero = new float3(0, 0, 0);

            yield return new object[] {new float3(2, 2, 2), zero, one, one};
            yield return new object[] {new float3(-1, -1, -1), zero, one, zero};
            yield return new object[] {new float3(0.5f, 0.5f, 0.5f), zero, one, new float3(0.5f, 0.5f, 0.5f)};
        }

        public static IEnumerable<object[]> GetLerp()
        {
            var one = new float3(1, 1, 1);
            var zero = new float3(0, 0, 0);

            yield return new object[] {zero, one, 0.5f, new float3(0.5f, 0.5f, 0.5f)};
            yield return new object[] {zero, one, 0, zero};
            yield return new object[] {zero, one, 1, one};
        }

        public static IEnumerable<object[]> GetBarycentric()
        {
            var x = new float3(1, 0, 0);
            var y = new float3(0, 1, 0);
            var z = new float3(0, 0, 1);

            yield return new object[] {x, y, z, 0, 0, z};
            yield return new object[] {x, y, z, 1, 0, x};
            yield return new object[] {x, y, z, 0, 1, y};
        }

        public static IEnumerable<object[]> GetEuler()
        {
            var x = new float3(1, 0, 0);
            var y = new float3(0, 1, 0);
            var z = new float3(0, 0, 1);

            yield return new object[] {new float3(90, 0, 0), y, z};
            yield return new object[] {new float3(0, 90, 0), z, x};
            yield return new object[] {new float3(0, 0, 90), x, y};
        }

        public static IEnumerable<object[]> GetQuaternion()
        {
            var x = new float3(1, 0, 0);
            var y = new float3(0, 1, 0);
            var z = new float3(0, 0, 1);

            var xRot = new Quaternion((float) System.Math.Sqrt(0.5), 0, 0, (float) System.Math.Sqrt(0.5));
            var yRot = new Quaternion(0, (float) System.Math.Sqrt(0.5), 0, (float) System.Math.Sqrt(0.5));
            var zRot = new Quaternion(0, 0, (float) System.Math.Sqrt(0.5), (float) System.Math.Sqrt(0.5));

            yield return new object[] {xRot, y, z};
            yield return new object[] {yRot, z, x};
            yield return new object[] {zRot, x, y};
        }

        #endregion

        #region ToString/Parse

        [Fact]
        public void ToString_NoCulture()
        {
            Assert.NotNull(new float3().ToString());
        }

        [Fact]
        public void ToString_InvariantCulture()
        {
            string s = "(1.5, 1.5, 1.5)";
            float3 f = float3.One * 1.5f;

            Assert.Equal(s, f.ToString(CultureInfo.InvariantCulture));
        }

        [Fact]
        public void ToString_CultureDE()
        {
            string s = "(1,5; 1,5; 1,5)";
            float3 f = float3.One * 1.5f;

            Assert.Equal(s, f.ToString(new CultureInfo("de-DE")));
        }

        [Fact]
        public void Parse_InvariantCulture()
        {
            string s = "(1.5, 1.5, 1.5)";
            float3 f = float3.One * 1.5f;

            Assert.Equal(f, float3.Parse(s, CultureInfo.InvariantCulture));
        }

        [Fact]
        public void Parse_CultureDE()
        {
            string s = "(1,5; 1,5; 1,5)";
            float3 f = float3.One * 1.5f;

            Assert.Equal(f, float3.Parse(s, new CultureInfo("de-DE")));
        }

        [Fact]
        public void Parse_ToString_NoCulture()
        {
            float3 f = float3.One * 1.5f;

            Assert.Equal(f, float3.Parse(f.ToString()));
        }

        [Fact]
        public void Parse_ToString_InvariantCulture()
        {
            float3 f = float3.One * 1.5f;

            Assert.Equal(f, float3.Parse(f.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture));
        }

        [Fact]
        public void Parse_ToString_CultureDE()
        {
            float3 f = float3.One * 1.5f;

            Assert.Equal(f, float3.Parse(f.ToString(new CultureInfo("de-DE")), new CultureInfo("de-DE")));
        }

        [Fact]
        public void Parse_Exception()
        {
            string s = "Fusee";

            Assert.Throws<FormatException>(() => float3.Parse(s));
        }

        #endregion ToString/Parse
    }
}
