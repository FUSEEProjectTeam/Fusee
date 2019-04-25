using Xunit;
using System.Collections.Generic;
using System.Security.Cryptography;
using Xunit.Sdk;

namespace Fusee.Math.Core
{
    public class Float3Test
    {
        #region Fields

        [Fact]
        public void One_IsOne()
        {
            var expected = new float3(1,1,1);

            Assert.Equal(expected, float3.One);
        }

        [Fact]
        public void Zero_IsZero()
        {
            var expected = new float3(0, 0, 0);

            Assert.Equal(expected, float3.Zero);
        }

        [Fact]
        public void UnitX_IsXVector()
        {
            var expected = new float3(1, 0, 0);

            Assert.Equal(expected, float3.UnitX);
        }

        [Fact]
        public void UnitY_IsYVector()
        {
            var expected = new float3(0, 1, 0);

            Assert.Equal(expected, float3.UnitY);
        }

        [Fact]
        public void UnitZ_IsZVector()
        {
            var expected = new float3(0, 0, 1);

            Assert.Equal(expected, float3.UnitZ);
        }

        [Fact]
        public void X_IsOne()
        {
            var actual = new float3(1, 0, 0);

            Assert.Equal(1, actual.x);
        }

        [Fact]
        public void Y_IsOne()
        {
            var actual = new float3(0, 1, 0);

            Assert.Equal(1, actual.y);
        }

        [Fact]
        public void Z_IsOne()
        {
            var actual = new float3(0, 0, 1);

            Assert.Equal(1, actual.z);
        }

        #endregion

        #region Constructors

        [Fact]
        public void Constructor_FromDouble3()
        {
            var d3 = new double3(1, 2, 3);

            var actual = new float3(d3);

            Assert.Equal(new float3(1,2,3), actual);
        }

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

            Assert.Equal(f3, actual);
        }

        [Fact]
        public void Constructor_FromFloat4()
        {
            var f4 = new float4(1, 2, 3, 4);

            var actual = new float3(f4);

            Assert.Equal(new float3(1, 2, 3), actual);
        }

        #endregion

        #region Operators

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Operator_Addition_IsExpected(float3 a, float3 b, float3 expected)
        {
            var actual = a + b;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Operator_Subtraction(float3 a, float3 b, float3 expected)
        {
            var actual = a - b;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Operator_UnaryNegation()
        {
            var vec = new float3(1, 1, 1);

            var actual = -vec;

            Assert.Equal(new float3(-1, -1, -1), actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Operator_Multiply_Scalar(float scale, float3 vec, float3 expected)
        {
            var actual1 = scale * vec;
            var actual2 = vec * scale;

            Assert.Equal(expected, actual1);
            Assert.Equal(expected, actual2);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Operator_Multiply_Vector(float x, float3 vec1, float3 expected)
        {
            var vec2 = new float3(x, x, x);

            var actual = vec1 * vec2;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivision))]
        public void Operator_Division_Scalar(float3 vec, float scale, float3 expected)
        {
            var actual = vec / scale;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Operator_Equality_IsEqual()
        {
            var a = new float3(1, 1, 1);
            var b = new float3(1, 1, 1);

            Assert.True(a == b);
        }

        [Fact]
        public void Operator_Equality_IsInequal()
        {
            var a = new float3(1, 1, 1);
            var b = new float3(0, 0, 0);

            Assert.False(a == b);
        }

        [Fact]
        public void Operator_Inequality_IsInequal()
        {
            var a = new float3(1, 1, 1);
            var b = new float3(0, 0, 0);

            Assert.True(a != b);
        }

        [Fact]
        public void Operator_Inequality_IsEqual()
        {
            var a = new float3(1, 1, 1);
            var b = new float3(1, 1, 1);

            Assert.False(a != b);
        }

        [Fact]
        public void Operator_Explicit_FromDouble3()
        {
            var actual = (float3) new double3(1, 1, 1);

            Assert.Equal(new float3(1, 1, 1), actual);
        }

        #endregion

        #region Properties

        [Fact]
        public void Length_Is16()
        {
            var vec = new float3(1, 1, 1);

            var actual = vec.Length;

            Assert.Equal(System.Math.Sqrt(3), actual, 6);
        }

        [Fact]
        public void LengthSquared_Is3()
        {
            var vec = new float3(1, 1, 1);

            var actual = vec.LengthSquared;

            Assert.Equal(3, actual);
        }

        [Fact]
        public void Swizzle_Float2_Get()
        {
            var vec = new float3(1, 2, 3);

            Assert.Equal(new float2(1, 2), vec.xy);
            Assert.Equal(new float2(1, 3), vec.xz);
            Assert.Equal(new float2(2, 1), vec.yx);
            Assert.Equal(new float2(2, 3), vec.yz);
            Assert.Equal(new float2(3, 1), vec.zx);
            Assert.Equal(new float2(3, 2), vec.zy);
        }

        [Fact]
        public void Swizzle_Float2_SetXY()
        {
            var actual = new float3(0,0,0);

            actual.xy = new float2(1, 2);

            Assert.Equal(new float3(1, 2, 0), actual);
        }

        [Fact]
        public void Swizzle_Float2_SetXZ()
        {
            var actual = new float3(0, 0, 0);

            actual.xz = new float2(1, 2);

            Assert.Equal(new float3(1,0,2), actual);
        }

        [Fact]
        public void Swizzle_Float2_SetYX()
        {
            var actual = new float3(0, 0, 0);

            actual.yx = new float2(1, 2);

            Assert.Equal(new float3(2, 1, 0), actual);
        }

        [Fact]
        public void Swizzle_Float2_SetYZ()
        {
            var actual = new float3(0, 0, 0);

            actual.yz = new float2(1, 2);

            Assert.Equal(new float3(0, 1, 2), actual);
        }

        [Fact]
        public void Swizzle_Float2_SetZX()
        {
            var actual = new float3(0, 0, 0);

            actual.zx = new float2(1, 2);

            Assert.Equal(new float3(2, 0, 1), actual);
        }

        [Fact]
        public void Swizzle_Float2_SetZY()
        {
            var actual = new float3(0, 0, 0);

            actual.zy = new float2(1, 2);

            Assert.Equal(new float3(0, 2, 1), actual);
        }

        [Fact]
        public void Swizzle_Float3_Get()
        {
            var vec = new float3(1, 2, 3);

            Assert.Equal(new float3(1, 2, 3), vec.xyz);
            Assert.Equal(new float3(1, 3, 2), vec.xzy);
            Assert.Equal(new float3(2, 3, 1), vec.yzx);
            Assert.Equal(new float3(2, 1, 3), vec.yxz);
            Assert.Equal(new float3(3, 1, 2), vec.zxy);
            Assert.Equal(new float3(3, 2, 1), vec.zyx);
        }

        [Fact]
        public void Swizzle_Float3_SetXYZ()
        {
            var actual = new float3(0, 0, 0);

            actual.xyz = new float3(1, 2, 3);

            Assert.Equal(new float3(1, 2, 3), actual);
        }

        [Fact]
        public void Swizzle_Float3_SetXZY()
        {
            var actual = new float3(0, 0, 0);

            actual.xzy = new float3(1, 2, 3);

            Assert.Equal(new float3(1, 3, 2), actual);
        }

        [Fact]
        public void Swizzle_Float3_SetYZX()
        {
            var actual = new float3(0, 0, 0);

            actual.yzx = new float3(1, 2, 3);

            Assert.Equal(new float3(3, 1, 2), actual);
        }

        [Fact]
        public void Swizzle_Float3_SetYXZ()
        {
            var actual = new float3(0, 0, 0);

            actual.yxz = new float3(1, 2, 3);

            Assert.Equal(new float3(2, 1, 3), actual);
        }

        [Fact]
        public void Swizzle_Float3_SetZXY()
        {
            var actual = new float3(0, 0, 0);

            actual.zxy = new float3(1, 2, 3);

            Assert.Equal(new float3(2, 3, 1), actual);
        }

        [Fact]
        public void Swizzle_Float3_SetZYX()
        {
            var actual = new float3(0, 0, 0);

            actual.zyx = new float3(1, 2, 3);

            Assert.Equal(new float3(3, 2, 1), actual);
        }

        #endregion

        #region Methods

        #region Arithmetic Functions

        [Theory]
        [MemberData(nameof(GetAddition))]
        public void Add_IsOne(float3 a, float3 b, float3 expected)
        {
            var actual = float3.Add(a, b);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetSubtraction))]
        public void Subtract_IsZero(float3 a, float3 b, float3 expected)
        {
            var actual = float3.Subtract(a, b);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_VectorScale(float scale, float3 vec, float3 expected)
        {
            var actual = float3.Multiply(vec, scale);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetMultiply))]
        public void Multiply_TwoVectors(float x, float3 vec1, float3 expected)
        {
            var vec2 = new float3(x, x, x);

            var actual = float3.Multiply(vec1, vec2);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivision))]
        public void Divide_VectorScale(float3 vec, float scale, float3 expected)
        {
            var actual = float3.Divide(vec, scale);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetDivision))]
        public void Divide_TwoVectors(float3 vec1, float x, float3 expected)
        {
            var vec2 = new float3(x, x, x);

            var actual = float3.Divide(vec1, vec2);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region MinMax

        [Theory]
        [MemberData(nameof(GetComponentMin))]
        public void ComponentMin_IsOne(float3 a, float3 b, float3 expected)
        {
            var actual = float3.ComponentMin(a, b);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetComponentMax))]
        public void ComponentMax_IsTwo(float3 a, float3 b, float3 expected)
        {
            var actual = float3.ComponentMax(a, b);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetComponentMin))]
        public void Min_IsOne(float3 a, float3 b, float3 expected)
        {
            var actual = float3.Min(a, b);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetComponentMax))]
        public void Max_IsTwo(float3 a, float3 b, float3 expected)
        {
            var actual = float3.Max(a, b);

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
        public void OrthoNormalize_NormTangent()
        {
            var normal = new float3(5, 0, 0);
            var tangent = new float3(4, 4, 4);

            var actual = float3.OrthoNormalize(normal, tangent);

            Assert.Equal(new float3(1, 0, 0), actual[0]);
            Assert.Equal(0, actual[1].x, 3);
            Assert.Equal(0.707f, actual[1].y, 3);
            Assert.Equal(0.707f, actual[1].z, 3);
        }

        #endregion

        #region Dot

        [Fact]
        public void Dot_IsTen()
        {
            var a = new float3(1, 2, 3);
            var b = new float3(3, 2, 1);

            var actual = float3.Dot(a, b);

            Assert.Equal(10, actual);
        }

        #endregion

        #region Cross

        [Theory]
        [MemberData(nameof(GetCross))]
        public void Cross_TestCross(float3 a, float3 b, float3 expected)
        {
            var actual = float3.Cross(a, b);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Lerp

        [Theory]
        [MemberData(nameof(GetLerp))]
        public void Lerp_ZeroAndOne(float3 a, float3 b, float blend, float3 expected)
        {
            var actual = float3.Lerp(a, b, blend);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Barycentric

        [Theory]
        [MemberData(nameof(GetBarycentric))]
        public void Barycentric_TestBarycentric(float3 a, float3 b, float3 c, float u, float v, float3 expected)
        {
            var actual = float3.Barycentric(a, b, c, u, v);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetBarycentric))]
        public void GetBarycentric_GetUV(float3 a, float3 b, float3 c, float expectedU, float expectedV, float3 point)
        {
            float u;
            float v;

            float3.GetBarycentric(a, b, c, point, out u, out v);

            Assert.Equal(expectedU, u);
            Assert.Equal(expectedV, v);
        }

        #endregion

        #region Angle

        //TODO: CalculateAngle

        #endregion

        #region Rotate

        [Theory]
        [MemberData(nameof(GetEuler))]
        public void Rotate_Euler(float3 euler, float3 vec, bool inDegrees, float3 expected)
        {
            var actual = float3.Rotate(euler, vec, inDegrees);

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

        #region IEnumerables

        public static IEnumerable<object[]> GetAddition()
        {
            var one = new float3(1, 1, 1);
            var zero = new float3(0, 0, 0);

            yield return new object[] { one, zero, one };
            yield return new object[] { zero, one, one };
        }

        public static IEnumerable<object[]> GetDivision()
        {
            yield return new object[] { new float3(2, 2, 2), 2, new float3(1, 1, 1) };
        }

        public static IEnumerable<object[]> GetMultiply()
        {
            var one = new float3(1, 1, 1);

            yield return new object[] { 1, one, one };
            yield return new object[] { 2, one, new float3(2, 2, 2) };
            yield return new object[] { 0, one, new float3(0, 0, 0) };
        }

        public static IEnumerable<object[]> GetSubtraction()
        {
            yield return new object[] { new float3(1, 1, 1), new float3(1, 1, 1), new float3(0, 0, 0) };
        }

        public static IEnumerable<object[]> GetTransform4D()
        {
            var x = new float3(1, 0, 0);
            var y = new float3(0, 1, 0);
            var z = new float3(0, 0, 1);

            var xRot = new float4x4(new float4(1, 0, 0, 0), new float4(0, 0, -1, 0), new float4(0, 1, 0, 0), new float4(0, 0, 0, 1));
            var yRot = new float4x4(new float4(0, 0, 1, 0), new float4(0, 1, 0, 0), new float4(-1, 0, 0, 0), new float4(0, 0, 0, 1));
            var zRot = new float4x4(new float4(0, -1, 0, 0), new float4(1, 0, 0, 0), new float4(0, 0, 1, 0), new float4(0, 0, 0, 1));

            yield return new object[] {y, xRot, z};
            yield return new object[] {z, yRot, x};
            yield return new object[] {x, zRot, y};
        }

        public static IEnumerable<object[]> GetTransform3D()
        {
            var x = new float3(1, 0, 0);
            var y = new float3(0, 1, 0);
            var z = new float3(0, 0, 1);

            var xRot = new float3x3(new float3(1, 0, 0), new float3(0, 0, -1), new float3(0, 1, 0));
            var yRot = new float3x3(new float3(0, 0, 1), new float3(0, 1, 0), new float3(-1, 0, 0));
            var zRot = new float3x3(new float3(0, -1, 0), new float3(1, 0, 0), new float3(0, 0, 1));

            yield return new object[] { y, xRot, z };
            yield return new object[] { z, yRot, x };
            yield return new object[] { x, zRot, y };
        }

        public static IEnumerable<object[]> GetComponentMin()
        {
            yield return new object[] {new float3(1, 1, 1), new float3(2, 2, 2), new float3(1, 1, 1)};
            yield return new object[] {new float3(2, 2, 2), new float3(1, 1, 1), new float3(1, 1, 1)};
        }

        public static IEnumerable<object[]> GetComponentMax()
        {
            yield return new object[] { new float3(1, 1, 1), new float3(2, 2, 2), new float3(2, 2, 2) };
            yield return new object[] { new float3(2, 2, 2), new float3(1, 1, 1), new float3(2, 2, 2) };
        }

        public static IEnumerable<object[]> GetClamp()
        {
            var zero = new float3(0, 0, 0);
            var one = new float3(1, 1, 1);

            yield return new object[] {new float3(-1, -1, -1), zero, one, zero};
            yield return new object[] {new float3(2, 2, 2), zero, one, one};
            yield return new object[] {new float3(0.5f, 0.5f, 0.5f), zero, one, new float3(0.5f, 0.5f, 0.5f)};
        }

        public static IEnumerable<object[]> GetNormalize()
        {
            yield return new object[] {new float3(2, 0, 0), new float3(1, 0, 0)};
            yield return new object[] {new float3(0, 2, 0), new float3(0, 1, 0)};
            yield return new object[] {new float3(0, 0, 2), new float3(0, 0, 1)};
            yield return new object[] {new float3(1, 1, 1), new float3(0.5773503f, 0.5773503f, 0.5773503f) };
        }

        public static IEnumerable<object[]> GetCross()
        {
            yield return new object[] {new float3(1, 1, 1), new float3(1, 1, 1), new float3(0, 0, 0)};
            yield return new object[] {new float3(1, 1, 1), new float3(1, 2, 3), new float3(1, -2, 1)};
        }

        public static IEnumerable<object[]> GetLerp()
        {
            yield return new object[] {new float3(0, 0, 0), new float3(1, 1, 1), 0.5f, new float3(0.5f, 0.5f, 0.5f)};
            yield return new object[] {new float3(0, 0, 0), new float3(1, 1, 1), 0, new float3(0, 0, 0)};
            yield return new object[] {new float3(0, 0, 0), new float3(1, 1, 1), 1, new float3(1, 1, 1)};
        }

        public static IEnumerable<object[]> GetBarycentric()
        {
            var x = new float3(1, 0, 0);
            var y = new float3(0, 1, 0);
            var z = new float3(0, 0, 1);

            yield return new object[] {x, y, z, 1, 0, x};
            yield return new object[] {x, y, z, 0, 1, y};
            yield return new object[] {x, y, z, 0, 0, z};
        }

        public static IEnumerable<object[]> GetEuler()
        {
            var x = new float3(1, 0, 0);
            var y = new float3(0, 1, 0);
            var z = new float3(0, 0, 1);

            yield return new object[] {new float3(90, 0, 0), y, true, z};
            yield return new object[] {new float3(0, 90, 0), z, true, x};
            yield return new object[] {new float3(0, 0, 90), x, true, y};
        }

        public static IEnumerable<object[]> GetQuaternion()
        {
            var x = new float3(1, 0, 0);
            var y = new float3(0, 1, 0);
            var z = new float3(0, 0, 1);

            yield return new object[] {new Quaternion(0.7071068f, 0, 0, 0.7071068f), y, z};
            yield return new object[] {new Quaternion(0, 0.7071068f, 0, 0.7071068f), z, x};
            yield return new object[] {new Quaternion(0, 0, 0.7071068f, 0.7071068f), x, y};
        }

        #endregion

    }
}
