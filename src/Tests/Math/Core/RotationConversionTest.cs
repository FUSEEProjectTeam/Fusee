using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Fusee.Test.Math.Core
{
    /// <summary>
    /// This test is comparing all rotation conversions with the results in Unity3D.
    /// </summary>
    public class RotationConversionTest
    {
        float precision = 1E-05f;

        private readonly ITestOutputHelper output;

        public RotationConversionTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [MemberData(nameof(GetRotation))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters")]
        public void EulerToQuaternion(string text, float3 euler, Quaternion quaternion, float4x4 matrix, float3 euler2)
        {
            var valid = false;
            var expected = quaternion;
            var actual = Quaternion.EulerToQuaternion(euler);

            if (MathF.Abs(expected.x - actual.x) < precision &&
                MathF.Abs(expected.y - actual.y) < precision &&
                MathF.Abs(expected.z - actual.z) < precision &&
                MathF.Abs(expected.w - actual.w) < precision ||
                //Q-1 is also valid
                MathF.Abs(expected.x + actual.x) < precision &&
                MathF.Abs(expected.y + actual.y) < precision &&
                MathF.Abs(expected.z + actual.z) < precision &&
                MathF.Abs(expected.w + actual.w) < precision)
                valid = true;

            output.WriteLine($"{text}\nExpected:\n{expected}\n Actual:\n{actual}");
            Assert.True(valid);
        }

        [Theory]
        [MemberData(nameof(GetRotation))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters")]
        public void EulerToMatrix(string text, float3 euler, Quaternion quaternion, float4x4 matrix, float3 euler2)
        {
            var valid = false;
            var expected = matrix;
            var actual = float4x4.CreateRotationZXY(euler);

            if (MathF.Abs(expected.M11 - actual.M11) < precision &&
                MathF.Abs(expected.M12 - actual.M12) < precision &&
                MathF.Abs(expected.M13 - actual.M13) < precision &&
                MathF.Abs(expected.M14 - actual.M14) < precision &&
                MathF.Abs(expected.M21 - actual.M21) < precision &&
                MathF.Abs(expected.M22 - actual.M22) < precision &&
                MathF.Abs(expected.M23 - actual.M23) < precision &&
                MathF.Abs(expected.M24 - actual.M24) < precision &&
                MathF.Abs(expected.M31 - actual.M31) < precision &&
                MathF.Abs(expected.M32 - actual.M32) < precision &&
                MathF.Abs(expected.M33 - actual.M33) < precision &&
                MathF.Abs(expected.M34 - actual.M34) < precision &&
                MathF.Abs(expected.M41 - actual.M41) < precision &&
                MathF.Abs(expected.M42 - actual.M42) < precision &&
                MathF.Abs(expected.M43 - actual.M43) < precision &&
                MathF.Abs(expected.M44 - actual.M44) < precision)
                valid = true;

            output.WriteLine($"{text}\nExpected:\n{expected}\n Actual:\n{actual}");
            Assert.True(valid);
        }

        [Theory]
        [MemberData(nameof(GetRotation))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters")]
        public void QuaternionToEuler(string text, float3 euler, Quaternion quaternion, float4x4 matrix, float3 euler2)
        {
            var valid = false;
            var expected = euler;
            var actual = Quaternion.QuaternionToEuler(quaternion);

            //Since we test quaternions seperately, we can use them to compare here
            var expectedQ = Quaternion.EulerToQuaternion(expected);
            var actualQ = Quaternion.EulerToQuaternion(actual);

            if (MathF.Abs(expectedQ.x - actualQ.x) < precision &&
                MathF.Abs(expectedQ.y - actualQ.y) < precision &&
                MathF.Abs(expectedQ.z - actualQ.z) < precision &&
                MathF.Abs(expectedQ.w - actualQ.w) < precision ||
                //Q-1 is also valid
                MathF.Abs(expectedQ.x + actualQ.x) < precision &&
                MathF.Abs(expectedQ.y + actualQ.y) < precision &&
                MathF.Abs(expectedQ.z + actualQ.z) < precision &&
                MathF.Abs(expectedQ.w + actualQ.w) < precision)
                valid = true;

            output.WriteLine($"{text}\nExpected:\n{expected}\nExpectedQ:\n{expectedQ}\n Actual:\n{actual}\nActualQ:\n{actualQ}");
            Assert.True(valid);
        }

        [Theory]
        [MemberData(nameof(GetRotation))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters")]
        public void QuaternionToMatrix(string text, float3 euler, Quaternion quaternion, float4x4 matrix, float3 euler2)
        {
            var valid = false;
            var expected = matrix;
            var actual = Quaternion.ToRotMat(quaternion);

            if (MathF.Abs(expected.M11 - actual.M11) < precision &&
                MathF.Abs(expected.M12 - actual.M12) < precision &&
                MathF.Abs(expected.M13 - actual.M13) < precision &&
                MathF.Abs(expected.M14 - actual.M14) < precision &&
                MathF.Abs(expected.M21 - actual.M21) < precision &&
                MathF.Abs(expected.M22 - actual.M22) < precision &&
                MathF.Abs(expected.M23 - actual.M23) < precision &&
                MathF.Abs(expected.M24 - actual.M24) < precision &&
                MathF.Abs(expected.M31 - actual.M31) < precision &&
                MathF.Abs(expected.M32 - actual.M32) < precision &&
                MathF.Abs(expected.M33 - actual.M33) < precision &&
                MathF.Abs(expected.M34 - actual.M34) < precision &&
                MathF.Abs(expected.M41 - actual.M41) < precision &&
                MathF.Abs(expected.M42 - actual.M42) < precision &&
                MathF.Abs(expected.M43 - actual.M43) < precision &&
                MathF.Abs(expected.M44 - actual.M44) < precision)
                valid = true;

            output.WriteLine($"{text}\nExpected:\n{expected}\n Actual:\n{actual}");
            Assert.True(valid);
        }

        [Theory]
        [MemberData(nameof(GetRotation))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters")]
        public void MatrixToEuler(string text, float3 euler, Quaternion quaternion, float4x4 matrix, float3 euler2)
        {
            var valid = false;

            var expected = euler;
            var actual = float4x4.RotMatToEuler(matrix);

            //Since we test quaternions seperately, we can use them to compare here
            var expectedQ = Quaternion.EulerToQuaternion(expected);
            var actualQ = Quaternion.EulerToQuaternion(actual);

            if (MathF.Abs(expectedQ.x - actualQ.x) < precision &&
                MathF.Abs(expectedQ.y - actualQ.y) < precision &&
                MathF.Abs(expectedQ.z - actualQ.z) < precision &&
                MathF.Abs(expectedQ.w - actualQ.w) < precision ||
                //Q-1 is also valid
                MathF.Abs(expectedQ.x + actualQ.x) < precision &&
                MathF.Abs(expectedQ.y + actualQ.y) < precision &&
                MathF.Abs(expectedQ.z + actualQ.z) < precision &&
                MathF.Abs(expectedQ.w + actualQ.w) < precision)
                valid = true;

            output.WriteLine($"{text}\nExpected:\n{expected}\nExpectedQ:\n{expectedQ}\n Actual:\n{actual}\nActualQ:\n{actualQ}");
            Assert.True(valid);
        }

        [Theory]
        [MemberData(nameof(GetRotation))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters")]
        public void MatrixToQuaternion(string text, float3 euler, Quaternion quaternion, float4x4 matrix, float3 euler2)
        {
            var valid = false;
            var expected = quaternion;
            var actual = Quaternion.FromRotationMatrix(matrix);

            if (MathF.Abs(expected.x - actual.x) < precision &&
                MathF.Abs(expected.y - actual.y) < precision &&
                MathF.Abs(expected.z - actual.z) < precision &&
                MathF.Abs(expected.w - actual.w) < precision ||
                //Q-1 is also valid
                MathF.Abs(expected.x + actual.x) < precision &&
                MathF.Abs(expected.y + actual.y) < precision &&
                MathF.Abs(expected.z + actual.z) < precision &&
                MathF.Abs(expected.w + actual.w) < precision)
                valid = true;

            output.WriteLine($"{text}\nExpected:\n{expected}\n Actual:\n{actual}");
            Assert.True(valid);
        }

        /// <summary>
        /// These results where precalculated in Unity3D
        /// [0] Euler string in degrees
        /// [1] Euler input in radiant.
        /// [2] Resulting quaternion
        /// [3] Resulting matrix
        /// [4] Backconversion quaternion to euler in radiant
        /// 
        /// Use the following code to reproduce this input:
        /// 
        /// :GenFusTests.cs
        /// using System.Collections.Generic;
        /// using System.Globalization;
        /// using System.IO;
        /// using UnityEngine;
        /// 
        /// public class QTest : MonoBehaviour
        ///     {
        ///         float[] baseAngles = new float[] { 0, 0.1f, 1, 45, 90, 180, 270, 360, 585, -90, -540 };
        ///         List<Vector3> eulerList = new List<Vector3>();
        ///         string text;
        ///         void Start()
        ///         {
        ///             System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        /// 
        ///             for (int i = 0; i < baseAngles.Length; i++)
        ///             {
        ///                 for (int j = 0; j < baseAngles.Length; j++)
        ///                 {
        ///                     for (int k = 0; k < baseAngles.Length; k++)
        ///                     {
        ///                         eulerList.Add(new Vector3(baseAngles[k], baseAngles[j], baseAngles[i]));
        ///                     }
        ///                 }
        ///             }
        /// 
        ///             for (int i = 1; i <= eulerList.Count; i++)
        ///             {
        ///                 var e = eulerList[i - 1];
        ///                 var q = Quaternion.Euler(e);
        ///                 var m = Matrix4x4.Rotate(q);
        ///                 var e2 = q.eulerAngles;
        /// 
        ///                 text += $"// {i}. - X: {e.x}, Y: {e.y}, Z: {e.z}\n";
        ///                 text += "yield return new object[]\n{\n";
        /// 
        ///                 text += $"\t\t\t\t\"X: {e.x}, Y: {e.y}, Z: {e.z}\",\n";
        ///                 text += $"\t\t\t\t{e.ToFus()},\n";
        ///                 text += $"\t\t\t\t{q.ToFus()},\n";
        ///                 text += $"\t\t\t\t{m.ToFus()},\n";
        ///                 text += $"\t\t\t\t{e2.ToFus()}\n";
        /// 
        ///                 text += "};\n";
        ///             }
        ///             File.WriteAllText("blub.txt", text);
        ///         }
        ///     }
        /// 
        ///     public static class MyExtensions
        ///     {
        ///         public static string ToFus(this Vector3 v)
        ///         { return $"new float3({v.x * Mathf.Deg2Rad}f, {v.y * Mathf.Deg2Rad}f, {v.z * Mathf.Deg2Rad}f)"; }
        ///         public static string ToFus(this Quaternion q)
        ///         { return $"new Quaternion({q.x}f, {q.y}f, {q.z}f, {q.w}f)"; }
        ///         public static string ToFus(this Matrix4x4 m)
        ///         { return $"new float4x4({m.m00}f, {m.m01}f, {m.m02}f, {m.m03}f, {m.m10}f, {m.m11}f, {m.m12}f, {m.m13}f, {m.m20}f, {m.m21}f, {m.m22}f, {m.m23}f, {m.m30}f, {m.m31}f, {m.m32}f, {m.m33}f)"; }
        ///     }
        /// :EOF
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> GetRotation()
        {
            // 1. - X: 0, Y: 0, Z: 0
            yield return new object[]
            {
                "X: 0, Y: 0, Z: 0",
                new float3(0f, 0f, 0f),
                new Quaternion(0f, 0f, 0f, 1f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0f, 0f)
            };
            // 2. - X: 0.1, Y: 0, Z: 0
            yield return new object[]
            {
                "X: 0.1, Y: 0, Z: 0",
                new float3(0.001745329f, 0f, 0f),
                new Quaternion(0.0008726645f, 0f, 0f, 0.9999996f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 0.9999985f, -0.001745328f, 0f, 0f, 0.001745328f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0f, 0f)
            };
            // 3. - X: 1, Y: 0, Z: 0
            yield return new object[]
            {
                "X: 1, Y: 0, Z: 0",
                new float3(0.01745329f, 0f, 0f),
                new Quaternion(0.008726535f, 0f, 0f, 0.9999619f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 0.9998477f, -0.01745241f, 0f, 0f, 0.01745241f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0f, 0f)
            };
            // 4. - X: 45, Y: 0, Z: 0
            yield return new object[]
            {
                "X: 45, Y: 0, Z: 0",
                new float3(0.7853982f, 0f, 0f),
                new Quaternion(0.3826835f, 0f, 0f, 0.9238795f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 0.7071067f, -0.7071068f, 0f, 0f, 0.7071068f, 0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 0f, 0f)
            };
            // 5. - X: 90, Y: 0, Z: 0
            yield return new object[]
            {
                "X: 90, Y: 0, Z: 0",
                new float3(1.570796f, 0f, 0f),
                new Quaternion(0.7071068f, 0f, 0f, 0.7071068f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 5.960464E-08f, -0.9999999f, 0f, 0f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 0f, 0f)
            };
            // 6. - X: 180, Y: 0, Z: 0
            yield return new object[]
            {
                "X: 180, Y: 0, Z: 0",
                new float3(3.141593f, 0f, 0f),
                new Quaternion(1f, 0f, 0f, -4.371139E-08f),
                new float4x4(1f, 0f, 0f, 0f, 0f, -1f, 8.742278E-08f, 0f, 0f, -8.742278E-08f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 3.141593f, 3.141593f)
            };
            // 7. - X: 270, Y: 0, Z: 0
            yield return new object[]
            {
                "X: 270, Y: 0, Z: 0",
                new float3(4.712389f, 0f, 0f),
                new Quaternion(0.7071068f, 0f, 0f, -0.7071068f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, 0f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0f, 0f)
            };
            // 8. - X: 360, Y: 0, Z: 0
            yield return new object[]
            {
                "X: 360, Y: 0, Z: 0",
                new float3(6.283185f, 0f, 0f),
                new Quaternion(-8.742278E-08f, 0f, 0f, -1f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 1f, -1.748456E-07f, 0f, 0f, 1.748456E-07f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 0f, 0f)
            };
            // 9. - X: 585, Y: 0, Z: 0
            yield return new object[]
            {
                "X: 585, Y: 0, Z: 0",
                new float3(10.21018f, 0f, 0f),
                new Quaternion(-0.9238794f, 0f, 0f, 0.3826836f),
                new float4x4(1f, 0f, 0f, 0f, 0f, -0.7071065f, 0.707107f, 0f, 0f, -0.707107f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.141593f, 3.141593f)
            };
            // 10. - X: -90, Y: 0, Z: 0
            yield return new object[]
            {
                "X: -90, Y: 0, Z: 0",
                new float3(-1.570796f, 0f, 0f),
                new Quaternion(-0.7071068f, 0f, 0f, 0.7071068f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, 0f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0f, 0f)
            };
            // 11. - X: -540, Y: 0, Z: 0
            yield return new object[]
            {
                "X: -540, Y: 0, Z: 0",
                new float3(-9.424778f, 0f, 0f),
                new Quaternion(1f, 0f, 0f, 1.192488E-08f),
                new float4x4(1f, 0f, 0f, 0f, 0f, -1f, -2.384976E-08f, 0f, 0f, 2.384976E-08f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.141593f, 3.141593f)
            };
            // 12. - X: 0, Y: 0.1, Z: 0
            yield return new object[]
            {
                "X: 0, Y: 0.1, Z: 0",
                new float3(0f, 0.001745329f, 0f),
                new Quaternion(0f, 0.0008726645f, 0f, 0.9999996f),
                new float4x4(0.9999985f, 0f, 0.001745328f, 0f, 0f, 1f, 0f, 0f, -0.001745328f, 0f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0.001745329f, 0f)
            };
            // 13. - X: 0.1, Y: 0.1, Z: 0
            yield return new object[]
            {
                "X: 0.1, Y: 0.1, Z: 0",
                new float3(0.001745329f, 0.001745329f, 0f),
                new Quaternion(0.0008726642f, 0.0008726642f, -7.615433E-07f, 0.9999993f),
                new float4x4(0.9999985f, 3.046171E-06f, 0.001745326f, 0f, 1.136868E-13f, 0.9999985f, -0.001745328f, 0f, -0.001745328f, 0.001745326f, 0.999997f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0.001745329f, 1.13687E-13f)
            };
            // 14. - X: 1, Y: 0.1, Z: 0
            yield return new object[]
            {
                "X: 1, Y: 0.1, Z: 0",
                new float3(0.01745329f, 0.001745329f, 0f),
                new Quaternion(0.008726533f, 0.0008726313f, -7.615338E-06f, 0.9999616f),
                new float4x4(0.9999985f, 3.046018E-05f, 0.001745063f, 0f, 0f, 0.9998477f, -0.01745241f, 0f, -0.001745328f, 0.01745238f, 0.9998462f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0.001745329f, 0f)
            };
            // 15. - X: 45, Y: 0.1, Z: 0
            yield return new object[]
            {
                "X: 45, Y: 0.1, Z: 0",
                new float3(0.7853982f, 0.001745329f, 0f),
                new Quaternion(0.3826833f, 0.0008062369f, -0.0003339543f, 0.9238791f),
                new float4x4(0.9999985f, 0.001234133f, 0.001234133f, 0f, 5.820766E-11f, 0.7071068f, -0.7071068f, 0f, -0.001745328f, 0.7071057f, 0.7071057f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 0.001745329f, 8.231807E-11f)
            };
            // 16. - X: 90, Y: 0.1, Z: 0
            yield return new object[]
            {
                "X: 90, Y: 0.1, Z: 0",
                new float3(1.570796f, 0.001745329f, 0f),
                new Quaternion(0.7071065f, 0.000617067f, -0.000617067f, 0.7071065f),
                new float4x4(0.9999985f, 0.001745328f, 0f, 0f, 0f, -4.62876E-08f, -1f, 0f, -0.001745328f, 0.9999985f, -4.62876E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 0.001745329f, 0f)
            };
            // 17. - X: 180, Y: 0.1, Z: 0
            yield return new object[]
            {
                "X: 180, Y: 0.1, Z: 0",
                new float3(3.141593f, 0.001745329f, 0f),
                new Quaternion(0.9999996f, -3.814538E-11f, -0.0008726645f, -4.371137E-08f),
                new float4x4(0.9999985f, -1.525814E-10f, -0.001745328f, 0f, 6.938894E-18f, -1f, 8.742278E-08f, 0f, -0.001745328f, -8.742266E-08f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 3.143338f, 3.141593f)
            };
            // 18. - X: 270, Y: 0.1, Z: 0
            yield return new object[]
            {
                "X: 270, Y: 0.1, Z: 0",
                new float3(4.712389f, 0.001745329f, 0f),
                new Quaternion(0.7071065f, -0.000617067f, -0.000617067f, -0.7071065f),
                new float4x4(0.9999985f, -0.001745328f, 0f, 0f, 0f, -4.62876E-08f, 1f, 0f, -0.001745328f, -0.9999985f, -4.62876E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.001745329f, 0f)
            };
            // 19. - X: 360, Y: 0.1, Z: 0
            yield return new object[]
            {
                "X: 360, Y: 0.1, Z: 0",
                new float3(6.283185f, 0.001745329f, 0f),
                new Quaternion(-8.742275E-08f, -0.0008726645f, 7.629075E-11f, -0.9999996f),
                new float4x4(0.9999985f, 3.051629E-10f, 0.001745328f, 0f, 1.387779E-17f, 1f, -1.748456E-07f, 0f, -0.001745328f, 1.748453E-07f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 0.001745329f, 1.387779E-17f)
            };
            // 20. - X: 585, Y: 0.1, Z: 0
            yield return new object[]
            {
                "X: 585, Y: 0.1, Z: 0",
                new float3(10.21018f, 0.001745329f, 0f),
                new Quaternion(-0.9238791f, 0.0003339544f, 0.0008062368f, 0.3826835f),
                new float4x4(0.9999985f, -0.001234134f, -0.001234133f, 0f, 0f, -0.7071065f, 0.707107f, 0f, -0.001745328f, -0.7071059f, -0.7071054f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.143338f, 3.141593f)
            };
            // 21. - X: -90, Y: 0.1, Z: 0
            yield return new object[]
            {
                "X: -90, Y: 0.1, Z: 0",
                new float3(-1.570796f, 0.001745329f, 0f),
                new Quaternion(-0.7071065f, 0.000617067f, 0.000617067f, 0.7071065f),
                new float4x4(0.9999985f, -0.001745328f, 0f, 0f, 0f, -4.62876E-08f, 1f, 0f, -0.001745328f, -0.9999985f, -4.62876E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.001745329f, 0f)
            };
            // 22. - X: -540, Y: 0.1, Z: 0
            yield return new object[]
            {
                "X: -540, Y: 0.1, Z: 0",
                new float3(-9.424778f, 0.001745329f, 0f),
                new Quaternion(0.9999996f, 1.040642E-11f, -0.0008726645f, 1.192488E-08f),
                new float4x4(0.9999985f, 4.162566E-11f, -0.001745328f, 0f, 0f, -1f, -2.384976E-08f, 0f, -0.001745328f, 2.384973E-08f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.143338f, 3.141593f)
            };
            // 23. - X: 0, Y: 1, Z: 0
            yield return new object[]
            {
                "X: 0, Y: 1, Z: 0",
                new float3(0f, 0.01745329f, 0f),
                new Quaternion(0f, 0.008726535f, 0f, 0.9999619f),
                new float4x4(0.9998477f, 0f, 0.01745241f, 0f, 0f, 1f, 0f, 0f, -0.01745241f, 0f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0.01745329f, 0f)
            };
            // 24. - X: 0.1, Y: 1, Z: 0
            yield return new object[]
            {
                "X: 0.1, Y: 1, Z: 0",
                new float3(0.001745329f, 0.01745329f, 0f),
                new Quaternion(0.0008726313f, 0.008726533f, -7.615338E-06f, 0.9999616f),
                new float4x4(0.9998477f, 3.046018E-05f, 0.01745238f, 0f, 0f, 0.9999985f, -0.001745328f, 0f, -0.01745241f, 0.001745063f, 0.9998462f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0.01745329f, 0f)
            };
            // 25. - X: 1, Y: 1, Z: 0
            yield return new object[]
            {
                "X: 1, Y: 1, Z: 0",
                new float3(0.01745329f, 0.01745329f, 0f),
                new Quaternion(0.008726203f, 0.008726203f, -7.615242E-05f, 0.9999238f),
                new float4x4(0.9998477f, 0.0003045865f, 0.01744975f, 0f, -1.455192E-11f, 0.9998477f, -0.01745241f, 0f, -0.01745241f, 0.01744975f, 0.9996954f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.0174533f, 0.01745329f, 1.455413E-11f)
            };
            // 26. - X: 45, Y: 1, Z: 0
            yield return new object[]
            {
                "X: 45, Y: 1, Z: 0",
                new float3(0.7853982f, 0.01745329f, 0f),
                new Quaternion(0.3826689f, 0.008062267f, -0.003339501f, 0.9238443f),
                new float4x4(0.9998477f, 0.01234072f, 0.01234071f, 0f, -4.656613E-10f, 0.7071068f, -0.7071068f, 0f, -0.0174524f, 0.7069991f, 0.7069991f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 0.01745329f, -6.585445E-10f)
            };
            // 27. - X: 90, Y: 1, Z: 0
            yield return new object[]
            {
                "X: 90, Y: 1, Z: 0",
                new float3(1.570796f, 0.01745329f, 0f),
                new Quaternion(0.7070798f, 0.006170592f, -0.006170592f, 0.7070798f),
                new float4x4(0.9998477f, 0.01745241f, 0f, 0f, 0f, 8.192001E-08f, -0.9999999f, 0f, -0.01745241f, 0.9998476f, 8.192001E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 0.01745329f, 0f)
            };
            // 28. - X: 180, Y: 1, Z: 0
            yield return new object[]
            {
                "X: 180, Y: 1, Z: 0",
                new float3(3.141593f, 0.01745329f, 0f),
                new Quaternion(0.9999619f, -3.81449E-10f, -0.008726535f, -4.370972E-08f),
                new float4x4(0.9998477f, -1.525738E-09f, -0.01745241f, 0f, -5.551115E-17f, -0.9999999f, 8.742277E-08f, 0f, -0.01745241f, -8.740945E-08f, -0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742277E-08f, 3.159046f, 3.141593f)
            };
            // 29. - X: 270, Y: 1, Z: 0
            yield return new object[]
            {
                "X: 270, Y: 1, Z: 0",
                new float3(4.712389f, 0.01745329f, 0f),
                new Quaternion(0.7070798f, -0.006170592f, -0.006170592f, -0.7070798f),
                new float4x4(0.9998477f, -0.01745241f, 0f, 0f, 0f, 8.192001E-08f, 0.9999999f, 0f, -0.01745241f, -0.9998476f, 8.192001E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.01745329f, 0f)
            };
            // 30. - X: 360, Y: 1, Z: 0
            yield return new object[]
            {
                "X: 360, Y: 1, Z: 0",
                new float3(6.283185f, 0.01745329f, 0f),
                new Quaternion(-8.741944E-08f, -0.008726535f, 7.62898E-10f, -0.9999619f),
                new float4x4(0.9998477f, 3.051476E-09f, 0.01745241f, 0f, -1.110223E-16f, 1f, -1.748455E-07f, 0f, -0.01745241f, 1.748189E-07f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748455E-07f, 0.01745329f, -1.110223E-16f)
            };
            // 31. - X: 585, Y: 1, Z: 0
            yield return new object[]
            {
                "X: 585, Y: 1, Z: 0",
                new float3(10.21018f, 0.01745329f, 0f),
                new Quaternion(-0.9238443f, 0.003339502f, 0.008062267f, 0.382669f),
                new float4x4(0.9998477f, -0.01234072f, -0.01234071f, 0f, -4.656613E-10f, -0.7071065f, 0.707107f, 0f, -0.01745241f, -0.7069994f, -0.7069988f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.159046f, 3.141593f)
            };
            // 32. - X: -90, Y: 1, Z: 0
            yield return new object[]
            {
                "X: -90, Y: 1, Z: 0",
                new float3(-1.570796f, 0.01745329f, 0f),
                new Quaternion(-0.7070798f, 0.006170592f, 0.006170592f, 0.7070798f),
                new float4x4(0.9998477f, -0.01745241f, 0f, 0f, 0f, 8.192001E-08f, 0.9999999f, 0f, -0.01745241f, -0.9998476f, 8.192001E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.01745329f, 0f)
            };
            // 33. - X: -540, Y: 1, Z: 0
            yield return new object[]
            {
                "X: -540, Y: 1, Z: 0",
                new float3(-9.424778f, 0.01745329f, 0f),
                new Quaternion(0.9999619f, 1.040629E-10f, -0.008726535f, 1.192443E-08f),
                new float4x4(0.9998477f, 4.162357E-10f, -0.01745241f, 0f, -1.387779E-17f, -0.9999999f, -2.384976E-08f, 0f, -0.01745241f, 2.384613E-08f, -0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.159046f, 3.141593f)
            };
            // 34. - X: 0, Y: 45, Z: 0
            yield return new object[]
            {
                "X: 0, Y: 45, Z: 0",
                new float3(0f, 0.7853982f, 0f),
                new Quaternion(0f, 0.3826835f, 0f, 0.9238795f),
                new float4x4(0.7071067f, 0f, 0.7071068f, 0f, 0f, 1f, 0f, 0f, -0.7071068f, 0f, 0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0.7853982f, 0f)
            };
            // 35. - X: 0.1, Y: 45, Z: 0
            yield return new object[]
            {
                "X: 0.1, Y: 45, Z: 0",
                new float3(0.001745329f, 0.7853982f, 0f),
                new Quaternion(0.0008062369f, 0.3826833f, -0.0003339543f, 0.9238791f),
                new float4x4(0.7071068f, 0.001234133f, 0.7071057f, 0f, 5.820766E-11f, 0.9999985f, -0.001745328f, 0f, -0.7071068f, 0.001234133f, 0.7071057f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0.7853982f, 5.820775E-11f)
            };
            // 36. - X: 1, Y: 45, Z: 0
            yield return new object[]
            {
                "X: 1, Y: 45, Z: 0",
                new float3(0.01745329f, 0.7853982f, 0f),
                new Quaternion(0.008062267f, 0.3826689f, -0.003339501f, 0.9238443f),
                new float4x4(0.7071068f, 0.01234072f, 0.7069991f, 0f, -4.656613E-10f, 0.9998477f, -0.0174524f, 0f, -0.7071068f, 0.01234071f, 0.7069991f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0.7853982f, -4.657322E-10f)
            };
            // 37. - X: 45, Y: 45, Z: 0
            yield return new object[]
            {
                "X: 45, Y: 45, Z: 0",
                new float3(0.7853982f, 0.7853982f, 0f),
                new Quaternion(0.3535534f, 0.3535534f, -0.1464466f, 0.8535534f),
                new float4x4(0.7071067f, 0.5000001f, 0.5f, 0f, 0f, 0.7071067f, -0.7071068f, 0f, -0.7071068f, 0.5f, 0.4999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 0.7853982f, 0f)
            };
            // 38. - X: 90, Y: 45, Z: 0
            yield return new object[]
            {
                "X: 90, Y: 45, Z: 0",
                new float3(1.570796f, 0.7853982f, 0f),
                new Quaternion(0.6532815f, 0.2705981f, -0.2705981f, 0.6532815f),
                new float4x4(0.7071068f, 0.7071068f, 0f, 0f, 0f, 8.940697E-08f, -0.9999999f, 0f, -0.7071068f, 0.7071067f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 0.7853982f, 0f)
            };
            // 39. - X: 180, Y: 45, Z: 0
            yield return new object[]
            {
                "X: 180, Y: 45, Z: 0",
                new float3(3.141593f, 0.7853982f, 0f),
                new Quaternion(0.9238795f, -1.672763E-08f, -0.3826835f, -4.038406E-08f),
                new float4x4(0.7071067f, -6.181724E-08f, -0.7071068f, 0f, 0f, -1f, 8.742278E-08f, 0f, -0.7071068f, -6.181723E-08f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 3.926991f, 3.141593f)
            };
            // 40. - X: 270, Y: 45, Z: 0
            yield return new object[]
            {
                "X: 270, Y: 45, Z: 0",
                new float3(4.712389f, 0.7853982f, 0f),
                new Quaternion(0.6532815f, -0.2705981f, -0.2705981f, -0.6532815f),
                new float4x4(0.7071068f, -0.7071068f, 0f, 0f, 0f, 8.940697E-08f, 0.9999999f, 0f, -0.7071068f, -0.7071067f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.7853982f, 0f)
            };
            // 41. - X: 360, Y: 45, Z: 0
            yield return new object[]
            {
                "X: 360, Y: 45, Z: 0",
                new float3(6.283185f, 0.7853982f, 0f),
                new Quaternion(-8.076811E-08f, -0.3826835f, 3.345525E-08f, -0.9238795f),
                new float4x4(0.7071067f, 1.236345E-07f, 0.7071068f, 0f, 0f, 1f, -1.748456E-07f, 0f, -0.7071068f, 1.236345E-07f, 0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 0.7853982f, 0f)
            };
            // 42. - X: 585, Y: 45, Z: 0
            yield return new object[]
            {
                "X: 585, Y: 45, Z: 0",
                new float3(10.21018f, 0.7853982f, 0f),
                new Quaternion(-0.8535533f, 0.1464467f, 0.3535534f, 0.3535535f),
                new float4x4(0.7071068f, -0.5000002f, -0.4999998f, 0f, -2.980232E-08f, -0.7071065f, 0.7071071f, 0f, -0.7071068f, -0.5000001f, -0.4999997f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.926991f, 3.141593f)
            };
            // 43. - X: -90, Y: 45, Z: 0
            yield return new object[]
            {
                "X: -90, Y: 45, Z: 0",
                new float3(-1.570796f, 0.7853982f, 0f),
                new Quaternion(-0.6532815f, 0.2705981f, 0.2705981f, 0.6532815f),
                new float4x4(0.7071068f, -0.7071068f, 0f, 0f, 0f, 8.940697E-08f, 0.9999999f, 0f, -0.7071068f, -0.7071067f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.7853982f, 0f)
            };
            // 44. - X: -540, Y: 45, Z: 0
            yield return new object[]
            {
                "X: -540, Y: 45, Z: 0",
                new float3(-9.424778f, 0.7853982f, 0f),
                new Quaternion(0.9238795f, 4.563455E-09f, -0.3826835f, 1.101715E-08f),
                new float4x4(0.7071067f, 1.686433E-08f, -0.7071068f, 0f, 0f, -1f, -2.384976E-08f, 0f, -0.7071068f, 1.686433E-08f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.926991f, 3.141593f)
            };
            // 45. - X: 0, Y: 90, Z: 0
            yield return new object[]
            {
                "X: 0, Y: 90, Z: 0",
                new float3(0f, 1.570796f, 0f),
                new Quaternion(0f, 0.7071068f, 0f, 0.7071068f),
                new float4x4(5.960464E-08f, 0f, 0.9999999f, 0f, 0f, 1f, 0f, 0f, -0.9999999f, 0f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.570796f, 0f)
            };
            // 46. - X: 0.1, Y: 90, Z: 0
            yield return new object[]
            {
                "X: 0.1, Y: 90, Z: 0",
                new float3(0.001745329f, 1.570796f, 0f),
                new Quaternion(0.000617067f, 0.7071065f, -0.000617067f, 0.7071065f),
                new float4x4(-4.62876E-08f, 0.001745328f, 0.9999985f, 0f, 0f, 0.9999985f, -0.001745328f, 0f, -1f, 0f, -4.62876E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 1.570796f, 0f)
            };
            // 47. - X: 1, Y: 90, Z: 0
            yield return new object[]
            {
                "X: 1, Y: 90, Z: 0",
                new float3(0.01745329f, 1.570796f, 0f),
                new Quaternion(0.006170592f, 0.7070798f, -0.006170592f, 0.7070798f),
                new float4x4(8.192001E-08f, 0.01745241f, 0.9998476f, 0f, 0f, 0.9998477f, -0.01745241f, 0f, -0.9999999f, 0f, 8.192001E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 1.570796f, 0f)
            };
            // 48. - X: 45, Y: 90, Z: 0
            yield return new object[]
            {
                "X: 45, Y: 90, Z: 0",
                new float3(0.7853982f, 1.570796f, 0f),
                new Quaternion(0.2705981f, 0.6532815f, -0.2705981f, 0.6532815f),
                new float4x4(8.940697E-08f, 0.7071068f, 0.7071067f, 0f, 0f, 0.7071068f, -0.7071068f, 0f, -0.9999999f, 0f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 1.570796f, 0f)
            };
            // 49. - X: 90, Y: 90, Z: 0
            yield return new object[]
            {
                "X: 90, Y: 90, Z: 0",
                new float3(1.570796f, 1.570796f, 0f),
                new Quaternion(0.5f, 0.5f, -0.5f, 0.5f),
                new float4x4(1.192093E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, -0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 1.570796f, 0f)
            };
            // 50. - X: 180, Y: 90, Z: 0
            yield return new object[]
            {
                "X: 180, Y: 90, Z: 0",
                new float3(3.141593f, 1.570796f, 0f),
                new Quaternion(0.7071068f, -3.090862E-08f, -0.7071068f, -3.090862E-08f),
                new float4x4(5.960464E-08f, -8.742278E-08f, -0.9999999f, 0f, 0f, -0.9999999f, 8.742278E-08f, 0f, -0.9999999f, 0f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742279E-08f, 4.712389f, 3.141593f)
            };
            // 51. - X: 270, Y: 90, Z: 0
            yield return new object[]
            {
                "X: 270, Y: 90, Z: 0",
                new float3(4.712389f, 1.570796f, 0f),
                new Quaternion(0.5f, -0.5f, -0.5f, -0.5f),
                new float4x4(1.192093E-07f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 52. - X: 360, Y: 90, Z: 0
            yield return new object[]
            {
                "X: 360, Y: 90, Z: 0",
                new float3(6.283185f, 1.570796f, 0f),
                new Quaternion(-6.181724E-08f, -0.7071068f, 6.181724E-08f, -0.7071068f),
                new float4x4(5.960464E-08f, 1.748456E-07f, 0.9999999f, 0f, 0f, 1f, -1.748456E-07f, 0f, -0.9999999f, 0f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 1.570796f, 0f)
            };
            // 53. - X: 585, Y: 90, Z: 0
            yield return new object[]
            {
                "X: 585, Y: 90, Z: 0",
                new float3(10.21018f, 1.570796f, 0f),
                new Quaternion(-0.6532814f, 0.2705982f, 0.6532814f, 0.2705982f),
                new float4x4(7.450581E-08f, -0.707107f, -0.7071064f, 0f, 0f, -0.7071064f, 0.707107f, 0f, -0.9999999f, 0f, 7.450581E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 4.712389f, 3.141593f)
            };
            // 54. - X: -90, Y: 90, Z: 0
            yield return new object[]
            {
                "X: -90, Y: 90, Z: 0",
                new float3(-1.570796f, 1.570796f, 0f),
                new Quaternion(-0.5f, 0.5f, 0.5f, 0.5f),
                new float4x4(1.192093E-07f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 55. - X: -540, Y: 90, Z: 0
            yield return new object[]
            {
                "X: -540, Y: 90, Z: 0",
                new float3(-9.424778f, 1.570796f, 0f),
                new Quaternion(0.7071068f, 8.432163E-09f, -0.7071068f, 8.432163E-09f),
                new float4x4(5.960464E-08f, 2.384976E-08f, -0.9999999f, 0f, 0f, -0.9999999f, -2.384976E-08f, 0f, -0.9999999f, 0f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 4.712389f, 3.141593f)
            };
            // 56. - X: 0, Y: 180, Z: 0
            yield return new object[]
            {
                "X: 0, Y: 180, Z: 0",
                new float3(0f, 3.141593f, 0f),
                new Quaternion(0f, 1f, 0f, -4.371139E-08f),
                new float4x4(-1f, 0f, -8.742278E-08f, 0f, 0f, 1f, 0f, 0f, 8.742278E-08f, 0f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.141593f, 0f)
            };
            // 57. - X: 0.1, Y: 180, Z: 0
            yield return new object[]
            {
                "X: 0.1, Y: 180, Z: 0",
                new float3(0.001745329f, 3.141593f, 0f),
                new Quaternion(-3.814538E-11f, 0.9999996f, -0.0008726645f, -4.371137E-08f),
                new float4x4(-1f, -1.525814E-10f, -8.742266E-08f, 0f, 6.938894E-18f, 0.9999985f, -0.001745328f, 0f, 8.742278E-08f, -0.001745328f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.141593f, 6.938905E-18f)
            };
            // 58. - X: 1, Y: 180, Z: 0
            yield return new object[]
            {
                "X: 1, Y: 180, Z: 0",
                new float3(0.01745329f, 3.141593f, 0f),
                new Quaternion(-3.81449E-10f, 0.9999619f, -0.008726535f, -4.370972E-08f),
                new float4x4(-0.9999999f, -1.525738E-09f, -8.740945E-08f, 0f, -5.551115E-17f, 0.9998477f, -0.01745241f, 0f, 8.742277E-08f, -0.01745241f, -0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.141593f, -5.551961E-17f)
            };
            // 59. - X: 45, Y: 180, Z: 0
            yield return new object[]
            {
                "X: 45, Y: 180, Z: 0",
                new float3(0.7853982f, 3.141593f, 0f),
                new Quaternion(-1.672763E-08f, 0.9238795f, -0.3826835f, -4.038406E-08f),
                new float4x4(-1f, -6.181724E-08f, -6.181723E-08f, 0f, 0f, 0.7071067f, -0.7071068f, 0f, 8.742278E-08f, -0.7071068f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 3.141593f, 0f)
            };
            // 60. - X: 90, Y: 180, Z: 0
            yield return new object[]
            {
                "X: 90, Y: 180, Z: 0",
                new float3(1.570796f, 3.141593f, 0f),
                new Quaternion(-3.090862E-08f, 0.7071068f, -0.7071068f, -3.090862E-08f),
                new float4x4(-0.9999999f, -8.742278E-08f, 0f, 0f, 0f, 5.960464E-08f, -0.9999999f, 0f, 8.742278E-08f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 3.141593f, 0f)
            };
            // 61. - X: 180, Y: 180, Z: 0
            yield return new object[]
            {
                "X: 180, Y: 180, Z: 0",
                new float3(3.141593f, 3.141593f, 0f),
                new Quaternion(-4.371139E-08f, -4.371139E-08f, -1f, 1.910685E-15f),
                new float4x4(-1f, 7.642742E-15f, 8.742278E-08f, 0f, 0f, -1f, 8.742278E-08f, 0f, 8.742278E-08f, 8.742278E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 8.742278E-08f, 3.141593f)
            };
            // 62. - X: 270, Y: 180, Z: 0
            yield return new object[]
            {
                "X: 270, Y: 180, Z: 0",
                new float3(4.712389f, 3.141593f, 0f),
                new Quaternion(-3.090862E-08f, -0.7071068f, -0.7071068f, 3.090862E-08f),
                new float4x4(-0.9999999f, 8.742278E-08f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, 8.742278E-08f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.141593f, 0f)
            };
            // 63. - X: 360, Y: 180, Z: 0
            yield return new object[]
            {
                "X: 360, Y: 180, Z: 0",
                new float3(6.283185f, 3.141593f, 0f),
                new Quaternion(3.821371E-15f, -1f, 8.742278E-08f, 4.371139E-08f),
                new float4x4(-1f, -1.528548E-14f, -8.742278E-08f, 0f, 0f, 1f, -1.748456E-07f, 0f, 8.742278E-08f, -1.748456E-07f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 3.141593f, 0f)
            };
            // 64. - X: 585, Y: 180, Z: 0
            yield return new object[]
            {
                "X: 585, Y: 180, Z: 0",
                new float3(10.21018f, 3.141593f, 0f),
                new Quaternion(4.038405E-08f, 0.3826836f, 0.9238794f, -1.672763E-08f),
                new float4x4(-0.9999999f, 6.181726E-08f, 6.181721E-08f, 0f, 0f, -0.7071065f, 0.707107f, 0f, 8.742277E-08f, 0.707107f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 8.742278E-08f, 3.141593f)
            };
            // 65. - X: -90, Y: 180, Z: 0
            yield return new object[]
            {
                "X: -90, Y: 180, Z: 0",
                new float3(-1.570796f, 3.141593f, 0f),
                new Quaternion(3.090862E-08f, 0.7071068f, 0.7071068f, -3.090862E-08f),
                new float4x4(-0.9999999f, 8.742278E-08f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, 8.742278E-08f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.141593f, 0f)
            };
            // 66. - X: -540, Y: 180, Z: 0
            yield return new object[]
            {
                "X: -540, Y: 180, Z: 0",
                new float3(-9.424778f, 3.141593f, 0f),
                new Quaternion(-4.371139E-08f, 1.192488E-08f, -1f, -5.212531E-16f),
                new float4x4(-1f, -2.085012E-15f, 8.742278E-08f, 0f, 0f, -1f, -2.384976E-08f, 0f, 8.742278E-08f, -2.384976E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 8.742278E-08f, 3.141593f)
            };
            // 67. - X: 0, Y: 270, Z: 0
            yield return new object[]
            {
                "X: 0, Y: 270, Z: 0",
                new float3(0f, 4.712389f, 0f),
                new Quaternion(0f, 0.7071068f, 0f, -0.7071068f),
                new float4x4(5.960464E-08f, 0f, -0.9999999f, 0f, 0f, 1f, 0f, 0f, 0.9999999f, 0f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 0f)
            };
            // 68. - X: 0.1, Y: 270, Z: 0
            yield return new object[]
            {
                "X: 0.1, Y: 270, Z: 0",
                new float3(0.001745329f, 4.712389f, 0f),
                new Quaternion(-0.000617067f, 0.7071065f, -0.000617067f, -0.7071065f),
                new float4x4(-4.62876E-08f, -0.001745328f, -0.9999985f, 0f, 0f, 0.9999985f, -0.001745328f, 0f, 1f, 0f, -4.62876E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 4.712389f, 0f)
            };
            // 69. - X: 1, Y: 270, Z: 0
            yield return new object[]
            {
                "X: 1, Y: 270, Z: 0",
                new float3(0.01745329f, 4.712389f, 0f),
                new Quaternion(-0.006170592f, 0.7070798f, -0.006170592f, -0.7070798f),
                new float4x4(8.192001E-08f, -0.01745241f, -0.9998476f, 0f, 0f, 0.9998477f, -0.01745241f, 0f, 0.9999999f, 0f, 8.192001E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 4.712389f, 0f)
            };
            // 70. - X: 45, Y: 270, Z: 0
            yield return new object[]
            {
                "X: 45, Y: 270, Z: 0",
                new float3(0.7853982f, 4.712389f, 0f),
                new Quaternion(-0.2705981f, 0.6532815f, -0.2705981f, -0.6532815f),
                new float4x4(8.940697E-08f, -0.7071068f, -0.7071067f, 0f, 0f, 0.7071068f, -0.7071068f, 0f, 0.9999999f, 0f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 4.712389f, 0f)
            };
            // 71. - X: 90, Y: 270, Z: 0
            yield return new object[]
            {
                "X: 90, Y: 270, Z: 0",
                new float3(1.570796f, 4.712389f, 0f),
                new Quaternion(-0.5f, 0.5f, -0.5f, -0.5f),
                new float4x4(1.192093E-07f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, 0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 4.712389f, 0f)
            };
            // 72. - X: 180, Y: 270, Z: 0
            yield return new object[]
            {
                "X: 180, Y: 270, Z: 0",
                new float3(3.141593f, 4.712389f, 0f),
                new Quaternion(-0.7071068f, -3.090862E-08f, -0.7071068f, 3.090862E-08f),
                new float4x4(5.960464E-08f, 8.742278E-08f, 0.9999999f, 0f, 0f, -0.9999999f, 8.742278E-08f, 0f, 0.9999999f, 0f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742279E-08f, 1.570796f, 3.141593f)
            };
            // 73. - X: 270, Y: 270, Z: 0
            yield return new object[]
            {
                "X: 270, Y: 270, Z: 0",
                new float3(4.712389f, 4.712389f, 0f),
                new Quaternion(-0.5f, -0.5f, -0.5f, 0.5f),
                new float4x4(1.192093E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, 0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 74. - X: 360, Y: 270, Z: 0
            yield return new object[]
            {
                "X: 360, Y: 270, Z: 0",
                new float3(6.283185f, 4.712389f, 0f),
                new Quaternion(6.181724E-08f, -0.7071068f, 6.181724E-08f, 0.7071068f),
                new float4x4(5.960464E-08f, -1.748456E-07f, -0.9999999f, 0f, 0f, 1f, -1.748456E-07f, 0f, 0.9999999f, 0f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 4.712389f, 0f)
            };
            // 75. - X: 585, Y: 270, Z: 0
            yield return new object[]
            {
                "X: 585, Y: 270, Z: 0",
                new float3(10.21018f, 4.712389f, 0f),
                new Quaternion(0.6532814f, 0.2705982f, 0.6532814f, -0.2705982f),
                new float4x4(7.450581E-08f, 0.707107f, 0.7071064f, 0f, 0f, -0.7071064f, 0.707107f, 0f, 0.9999999f, 0f, 7.450581E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.570796f, 3.141593f)
            };
            // 76. - X: -90, Y: 270, Z: 0
            yield return new object[]
            {
                "X: -90, Y: 270, Z: 0",
                new float3(-1.570796f, 4.712389f, 0f),
                new Quaternion(0.5f, 0.5f, 0.5f, -0.5f),
                new float4x4(1.192093E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, 0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 77. - X: -540, Y: 270, Z: 0
            yield return new object[]
            {
                "X: -540, Y: 270, Z: 0",
                new float3(-9.424778f, 4.712389f, 0f),
                new Quaternion(-0.7071068f, 8.432163E-09f, -0.7071068f, -8.432163E-09f),
                new float4x4(5.960464E-08f, -2.384976E-08f, 0.9999999f, 0f, 0f, -0.9999999f, -2.384976E-08f, 0f, 0.9999999f, 0f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 1.570796f, 3.141593f)
            };
            // 78. - X: 0, Y: 360, Z: 0
            yield return new object[]
            {
                "X: 0, Y: 360, Z: 0",
                new float3(0f, 6.283185f, 0f),
                new Quaternion(0f, -8.742278E-08f, 0f, -1f),
                new float4x4(1f, 0f, 1.748456E-07f, 0f, 0f, 1f, 0f, 0f, -1.748456E-07f, 0f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.748456E-07f, 0f)
            };
            // 79. - X: 0.1, Y: 360, Z: 0
            yield return new object[]
            {
                "X: 0.1, Y: 360, Z: 0",
                new float3(0.001745329f, 6.283185f, 0f),
                new Quaternion(-0.0008726645f, -8.742275E-08f, 7.629075E-11f, -0.9999996f),
                new float4x4(1f, 3.051629E-10f, 1.748453E-07f, 0f, 1.387779E-17f, 0.9999985f, -0.001745328f, 0f, -1.748456E-07f, 0.001745328f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 1.748456E-07f, 1.387781E-17f)
            };
            // 80. - X: 1, Y: 360, Z: 0
            yield return new object[]
            {
                "X: 1, Y: 360, Z: 0",
                new float3(0.01745329f, 6.283185f, 0f),
                new Quaternion(-0.008726535f, -8.741944E-08f, 7.62898E-10f, -0.9999619f),
                new float4x4(1f, 3.051476E-09f, 1.748189E-07f, 0f, -1.110223E-16f, 0.9998477f, -0.01745241f, 0f, -1.748455E-07f, 0.01745241f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 1.748455E-07f, -1.110392E-16f)
            };
            // 81. - X: 45, Y: 360, Z: 0
            yield return new object[]
            {
                "X: 45, Y: 360, Z: 0",
                new float3(0.7853982f, 6.283185f, 0f),
                new Quaternion(-0.3826835f, -8.076811E-08f, 3.345525E-08f, -0.9238795f),
                new float4x4(1f, 1.236345E-07f, 1.236345E-07f, 0f, 0f, 0.7071067f, -0.7071068f, 0f, -1.748456E-07f, 0.7071068f, 0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 1.748456E-07f, 0f)
            };
            // 82. - X: 90, Y: 360, Z: 0
            yield return new object[]
            {
                "X: 90, Y: 360, Z: 0",
                new float3(1.570796f, 6.283185f, 0f),
                new Quaternion(-0.7071068f, -6.181724E-08f, 6.181724E-08f, -0.7071068f),
                new float4x4(1f, 1.748456E-07f, 0f, 0f, 0f, 5.960464E-08f, -0.9999999f, 0f, -1.748456E-07f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 1.748456E-07f, 0f)
            };
            // 83. - X: 180, Y: 360, Z: 0
            yield return new object[]
            {
                "X: 180, Y: 360, Z: 0",
                new float3(3.141593f, 6.283185f, 0f),
                new Quaternion(-1f, 3.821371E-15f, 8.742278E-08f, 4.371139E-08f),
                new float4x4(1f, -1.528548E-14f, -1.748456E-07f, 0f, 0f, -1f, 8.742278E-08f, 0f, -1.748456E-07f, -8.742278E-08f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 3.141593f, 3.141593f)
            };
            // 84. - X: 270, Y: 360, Z: 0
            yield return new object[]
            {
                "X: 270, Y: 360, Z: 0",
                new float3(4.712389f, 6.283185f, 0f),
                new Quaternion(-0.7071068f, 6.181724E-08f, 6.181724E-08f, 0.7071068f),
                new float4x4(1f, -1.748456E-07f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, -1.748456E-07f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.748456E-07f, 0f)
            };
            // 85. - X: 360, Y: 360, Z: 0
            yield return new object[]
            {
                "X: 360, Y: 360, Z: 0",
                new float3(6.283185f, 6.283185f, 0f),
                new Quaternion(8.742278E-08f, 8.742278E-08f, -7.642742E-15f, 1f),
                new float4x4(1f, 3.057097E-14f, 1.748456E-07f, 0f, 0f, 1f, -1.748456E-07f, 0f, -1.748456E-07f, 1.748456E-07f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 1.748456E-07f, 0f)
            };
            // 86. - X: 585, Y: 360, Z: 0
            yield return new object[]
            {
                "X: 585, Y: 360, Z: 0",
                new float3(10.21018f, 6.283185f, 0f),
                new Quaternion(0.9238794f, -3.345526E-08f, -8.07681E-08f, -0.3826836f),
                new float4x4(1f, -1.236345E-07f, -1.236344E-07f, 0f, 0f, -0.7071065f, 0.707107f, 0f, -1.748455E-07f, -0.707107f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.141593f, 3.141593f)
            };
            // 87. - X: -90, Y: 360, Z: 0
            yield return new object[]
            {
                "X: -90, Y: 360, Z: 0",
                new float3(-1.570796f, 6.283185f, 0f),
                new Quaternion(0.7071068f, -6.181724E-08f, -6.181724E-08f, -0.7071068f),
                new float4x4(1f, -1.748456E-07f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, -1.748456E-07f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.748456E-07f, 0f)
            };
            // 88. - X: -540, Y: 360, Z: 0
            yield return new object[]
            {
                "X: -540, Y: 360, Z: 0",
                new float3(-9.424778f, 6.283185f, 0f),
                new Quaternion(-1f, -1.042506E-15f, 8.742278E-08f, -1.192488E-08f),
                new float4x4(1f, 4.170025E-15f, -1.748456E-07f, 0f, 0f, -1f, -2.384976E-08f, 0f, -1.748456E-07f, 2.384976E-08f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.141593f, 3.141593f)
            };
            // 89. - X: 0, Y: 585, Z: 0
            yield return new object[]
            {
                "X: 0, Y: 585, Z: 0",
                new float3(0f, 10.21018f, 0f),
                new Quaternion(0f, -0.9238794f, 0f, 0.3826836f),
                new float4x4(-0.7071065f, 0f, -0.707107f, 0f, 0f, 1f, 0f, 0f, 0.707107f, 0f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.926991f, 0f)
            };
            // 90. - X: 0.1, Y: 585, Z: 0
            yield return new object[]
            {
                "X: 0.1, Y: 585, Z: 0",
                new float3(0.001745329f, 10.21018f, 0f),
                new Quaternion(0.0003339544f, -0.9238791f, 0.0008062368f, 0.3826835f),
                new float4x4(-0.7071065f, -0.001234134f, -0.7071059f, 0f, 0f, 0.9999985f, -0.001745328f, 0f, 0.707107f, -0.001234133f, -0.7071054f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.926991f, 0f)
            };
            // 91. - X: 1, Y: 585, Z: 0
            yield return new object[]
            {
                "X: 1, Y: 585, Z: 0",
                new float3(0.01745329f, 10.21018f, 0f),
                new Quaternion(0.003339502f, -0.9238443f, 0.008062267f, 0.382669f),
                new float4x4(-0.7071065f, -0.01234072f, -0.7069994f, 0f, -4.656613E-10f, 0.9998477f, -0.01745241f, 0f, 0.707107f, -0.01234071f, -0.7069988f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.926991f, -4.657322E-10f)
            };
            // 92. - X: 45, Y: 585, Z: 0
            yield return new object[]
            {
                "X: 45, Y: 585, Z: 0",
                new float3(0.7853982f, 10.21018f, 0f),
                new Quaternion(0.1464467f, -0.8535533f, 0.3535534f, 0.3535535f),
                new float4x4(-0.7071065f, -0.5000002f, -0.5000001f, 0f, -2.980232E-08f, 0.7071068f, -0.7071068f, 0f, 0.7071071f, -0.4999998f, -0.4999997f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 3.926991f, -4.214685E-08f)
            };
            // 93. - X: 90, Y: 585, Z: 0
            yield return new object[]
            {
                "X: 90, Y: 585, Z: 0",
                new float3(1.570796f, 10.21018f, 0f),
                new Quaternion(0.2705982f, -0.6532814f, 0.6532814f, 0.2705982f),
                new float4x4(-0.7071064f, -0.707107f, 0f, 0f, 0f, 7.450581E-08f, -0.9999999f, 0f, 0.707107f, -0.7071064f, 7.450581E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 3.926991f, 0f)
            };
            // 94. - X: 180, Y: 585, Z: 0
            yield return new object[]
            {
                "X: 180, Y: 585, Z: 0",
                new float3(3.141593f, 10.21018f, 0f),
                new Quaternion(0.3826836f, 4.038405E-08f, 0.9238794f, -1.672763E-08f),
                new float4x4(-0.7071065f, 6.181726E-08f, 0.707107f, 0f, 0f, -0.9999999f, 8.742277E-08f, 0f, 0.707107f, 6.181721E-08f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742277E-08f, 0.7853985f, 3.141593f)
            };
            // 95. - X: 270, Y: 585, Z: 0
            yield return new object[]
            {
                "X: 270, Y: 585, Z: 0",
                new float3(4.712389f, 10.21018f, 0f),
                new Quaternion(0.2705982f, 0.6532814f, 0.6532814f, -0.2705982f),
                new float4x4(-0.7071064f, 0.707107f, 0f, 0f, 0f, 7.450581E-08f, 0.9999999f, 0f, 0.707107f, 0.7071064f, 7.450581E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.926991f, 0f)
            };
            // 96. - X: 360, Y: 585, Z: 0
            yield return new object[]
            {
                "X: 360, Y: 585, Z: 0",
                new float3(6.283185f, 10.21018f, 0f),
                new Quaternion(-3.345526E-08f, 0.9238794f, -8.07681E-08f, -0.3826836f),
                new float4x4(-0.7071065f, -1.236345E-07f, -0.707107f, 0f, 0f, 1f, -1.748455E-07f, 0f, 0.707107f, -1.236344E-07f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748455E-07f, 3.926991f, 0f)
            };
            // 97. - X: 585, Y: 585, Z: 0
            yield return new object[]
            {
                "X: 585, Y: 585, Z: 0",
                new float3(10.21018f, 10.21018f, 0f),
                new Quaternion(-0.3535535f, -0.3535535f, -0.8535532f, 0.1464467f),
                new float4x4(-0.7071064f, 0.5000004f, 0.5f, 0f, -2.980232E-08f, -0.7071064f, 0.707107f, 0f, 0.707107f, 0.5f, 0.4999997f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 0.7853986f, 3.141593f)
            };
            // 98. - X: -90, Y: 585, Z: 0
            yield return new object[]
            {
                "X: -90, Y: 585, Z: 0",
                new float3(-1.570796f, 10.21018f, 0f),
                new Quaternion(-0.2705982f, -0.6532814f, -0.6532814f, 0.2705982f),
                new float4x4(-0.7071064f, 0.707107f, 0f, 0f, 0f, 7.450581E-08f, 0.9999999f, 0f, 0.707107f, 0.7071064f, 7.450581E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.926991f, 0f)
            };
            // 99. - X: -540, Y: 585, Z: 0
            yield return new object[]
            {
                "X: -540, Y: 585, Z: 0",
                new float3(-9.424778f, 10.21018f, 0f),
                new Quaternion(0.3826836f, -1.101715E-08f, 0.9238794f, 4.563456E-09f),
                new float4x4(-0.7071065f, -1.686433E-08f, 0.707107f, 0f, 0f, -0.9999999f, -2.384976E-08f, 0f, 0.707107f, -1.686432E-08f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 0.7853985f, 3.141593f)
            };
            // 100. - X: 0, Y: -90, Z: 0
            yield return new object[]
            {
                "X: 0, Y: -90, Z: 0",
                new float3(0f, -1.570796f, 0f),
                new Quaternion(0f, -0.7071068f, 0f, 0.7071068f),
                new float4x4(5.960464E-08f, 0f, -0.9999999f, 0f, 0f, 1f, 0f, 0f, 0.9999999f, 0f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 0f)
            };
            // 101. - X: 0.1, Y: -90, Z: 0
            yield return new object[]
            {
                "X: 0.1, Y: -90, Z: 0",
                new float3(0.001745329f, -1.570796f, 0f),
                new Quaternion(0.000617067f, -0.7071065f, 0.000617067f, 0.7071065f),
                new float4x4(-4.62876E-08f, -0.001745328f, -0.9999985f, 0f, 0f, 0.9999985f, -0.001745328f, 0f, 1f, 0f, -4.62876E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 4.712389f, 0f)
            };
            // 102. - X: 1, Y: -90, Z: 0
            yield return new object[]
            {
                "X: 1, Y: -90, Z: 0",
                new float3(0.01745329f, -1.570796f, 0f),
                new Quaternion(0.006170592f, -0.7070798f, 0.006170592f, 0.7070798f),
                new float4x4(8.192001E-08f, -0.01745241f, -0.9998476f, 0f, 0f, 0.9998477f, -0.01745241f, 0f, 0.9999999f, 0f, 8.192001E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 4.712389f, 0f)
            };
            // 103. - X: 45, Y: -90, Z: 0
            yield return new object[]
            {
                "X: 45, Y: -90, Z: 0",
                new float3(0.7853982f, -1.570796f, 0f),
                new Quaternion(0.2705981f, -0.6532815f, 0.2705981f, 0.6532815f),
                new float4x4(8.940697E-08f, -0.7071068f, -0.7071067f, 0f, 0f, 0.7071068f, -0.7071068f, 0f, 0.9999999f, 0f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 4.712389f, 0f)
            };
            // 104. - X: 90, Y: -90, Z: 0
            yield return new object[]
            {
                "X: 90, Y: -90, Z: 0",
                new float3(1.570796f, -1.570796f, 0f),
                new Quaternion(0.5f, -0.5f, 0.5f, 0.5f),
                new float4x4(1.192093E-07f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, 0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 4.712389f, 0f)
            };
            // 105. - X: 180, Y: -90, Z: 0
            yield return new object[]
            {
                "X: 180, Y: -90, Z: 0",
                new float3(3.141593f, -1.570796f, 0f),
                new Quaternion(0.7071068f, 3.090862E-08f, 0.7071068f, -3.090862E-08f),
                new float4x4(5.960464E-08f, 8.742278E-08f, 0.9999999f, 0f, 0f, -0.9999999f, 8.742278E-08f, 0f, 0.9999999f, 0f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742279E-08f, 1.570796f, 3.141593f)
            };
            // 106. - X: 270, Y: -90, Z: 0
            yield return new object[]
            {
                "X: 270, Y: -90, Z: 0",
                new float3(4.712389f, -1.570796f, 0f),
                new Quaternion(0.5f, 0.5f, 0.5f, -0.5f),
                new float4x4(1.192093E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, 0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 107. - X: 360, Y: -90, Z: 0
            yield return new object[]
            {
                "X: 360, Y: -90, Z: 0",
                new float3(6.283185f, -1.570796f, 0f),
                new Quaternion(-6.181724E-08f, 0.7071068f, -6.181724E-08f, -0.7071068f),
                new float4x4(5.960464E-08f, -1.748456E-07f, -0.9999999f, 0f, 0f, 1f, -1.748456E-07f, 0f, 0.9999999f, 0f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 4.712389f, 0f)
            };
            // 108. - X: 585, Y: -90, Z: 0
            yield return new object[]
            {
                "X: 585, Y: -90, Z: 0",
                new float3(10.21018f, -1.570796f, 0f),
                new Quaternion(-0.6532814f, -0.2705982f, -0.6532814f, 0.2705982f),
                new float4x4(7.450581E-08f, 0.707107f, 0.7071064f, 0f, 0f, -0.7071064f, 0.707107f, 0f, 0.9999999f, 0f, 7.450581E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.570796f, 3.141593f)
            };
            // 109. - X: -90, Y: -90, Z: 0
            yield return new object[]
            {
                "X: -90, Y: -90, Z: 0",
                new float3(-1.570796f, -1.570796f, 0f),
                new Quaternion(-0.5f, -0.5f, -0.5f, 0.5f),
                new float4x4(1.192093E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, 0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 110. - X: -540, Y: -90, Z: 0
            yield return new object[]
            {
                "X: -540, Y: -90, Z: 0",
                new float3(-9.424778f, -1.570796f, 0f),
                new Quaternion(0.7071068f, -8.432163E-09f, 0.7071068f, 8.432163E-09f),
                new float4x4(5.960464E-08f, -2.384976E-08f, 0.9999999f, 0f, 0f, -0.9999999f, -2.384976E-08f, 0f, 0.9999999f, 0f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 1.570796f, 3.141593f)
            };
            // 111. - X: 0, Y: -540, Z: 0
            yield return new object[]
            {
                "X: 0, Y: -540, Z: 0",
                new float3(0f, -9.424778f, 0f),
                new Quaternion(0f, 1f, 0f, 1.192488E-08f),
                new float4x4(-1f, 0f, 2.384976E-08f, 0f, 0f, 1f, 0f, 0f, -2.384976E-08f, 0f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.141593f, 0f)
            };
            // 112. - X: 0.1, Y: -540, Z: 0
            yield return new object[]
            {
                "X: 0.1, Y: -540, Z: 0",
                new float3(0.001745329f, -9.424778f, 0f),
                new Quaternion(1.040642E-11f, 0.9999996f, -0.0008726645f, 1.192488E-08f),
                new float4x4(-1f, 4.162566E-11f, 2.384973E-08f, 0f, 0f, 0.9999985f, -0.001745328f, 0f, -2.384976E-08f, -0.001745328f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.141593f, 0f)
            };
            // 113. - X: 1, Y: -540, Z: 0
            yield return new object[]
            {
                "X: 1, Y: -540, Z: 0",
                new float3(0.01745329f, -9.424778f, 0f),
                new Quaternion(1.040629E-10f, 0.9999619f, -0.008726535f, 1.192443E-08f),
                new float4x4(-0.9999999f, 4.162357E-10f, 2.384613E-08f, 0f, -1.387779E-17f, 0.9998477f, -0.01745241f, 0f, -2.384976E-08f, -0.01745241f, -0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.141593f, -1.38799E-17f)
            };
            // 114. - X: 45, Y: -540, Z: 0
            yield return new object[]
            {
                "X: 45, Y: -540, Z: 0",
                new float3(0.7853982f, -9.424778f, 0f),
                new Quaternion(4.563455E-09f, 0.9238795f, -0.3826835f, 1.101715E-08f),
                new float4x4(-1f, 1.686433E-08f, 1.686433E-08f, 0f, 0f, 0.7071067f, -0.7071068f, 0f, -2.384976E-08f, -0.7071068f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 3.141593f, 0f)
            };
            // 115. - X: 90, Y: -540, Z: 0
            yield return new object[]
            {
                "X: 90, Y: -540, Z: 0",
                new float3(1.570796f, -9.424778f, 0f),
                new Quaternion(8.432163E-09f, 0.7071068f, -0.7071068f, 8.432163E-09f),
                new float4x4(-0.9999999f, 2.384976E-08f, 0f, 0f, 0f, 5.960464E-08f, -0.9999999f, 0f, -2.384976E-08f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 3.141593f, 0f)
            };
            // 116. - X: 180, Y: -540, Z: 0
            yield return new object[]
            {
                "X: 180, Y: -540, Z: 0",
                new float3(3.141593f, -9.424778f, 0f),
                new Quaternion(1.192488E-08f, -4.371139E-08f, -1f, -5.212531E-16f),
                new float4x4(-1f, -2.085012E-15f, -2.384976E-08f, 0f, 0f, -1f, 8.742278E-08f, 0f, -2.384976E-08f, 8.742278E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, -2.384976E-08f, 3.141593f)
            };
            // 117. - X: 270, Y: -540, Z: 0
            yield return new object[]
            {
                "X: 270, Y: -540, Z: 0",
                new float3(4.712389f, -9.424778f, 0f),
                new Quaternion(8.432163E-09f, -0.7071068f, -0.7071068f, -8.432163E-09f),
                new float4x4(-0.9999999f, -2.384976E-08f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, -2.384976E-08f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.141593f, 0f)
            };
            // 118. - X: 360, Y: -540, Z: 0
            yield return new object[]
            {
                "X: 360, Y: -540, Z: 0",
                new float3(6.283185f, -9.424778f, 0f),
                new Quaternion(-1.042506E-15f, -1f, 8.742278E-08f, -1.192488E-08f),
                new float4x4(-1f, 4.170025E-15f, 2.384976E-08f, 0f, 0f, 1f, -1.748456E-07f, 0f, -2.384976E-08f, -1.748456E-07f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 3.141593f, 0f)
            };
            // 119. - X: 585, Y: -540, Z: 0
            yield return new object[]
            {
                "X: 585, Y: -540, Z: 0",
                new float3(10.21018f, -9.424778f, 0f),
                new Quaternion(-1.101715E-08f, 0.3826836f, 0.9238794f, 4.563456E-09f),
                new float4x4(-0.9999999f, -1.686433E-08f, -1.686432E-08f, 0f, 0f, -0.7071065f, 0.707107f, 0f, -2.384976E-08f, 0.707107f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, -2.384976E-08f, 3.141593f)
            };
            // 120. - X: -90, Y: -540, Z: 0
            yield return new object[]
            {
                "X: -90, Y: -540, Z: 0",
                new float3(-1.570796f, -9.424778f, 0f),
                new Quaternion(-8.432163E-09f, 0.7071068f, 0.7071068f, 8.432163E-09f),
                new float4x4(-0.9999999f, -2.384976E-08f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, -2.384976E-08f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.141593f, 0f)
            };
            // 121. - X: -540, Y: -540, Z: 0
            yield return new object[]
            {
                "X: -540, Y: -540, Z: 0",
                new float3(-9.424778f, -9.424778f, 0f),
                new Quaternion(1.192488E-08f, 1.192488E-08f, -1f, 1.422028E-16f),
                new float4x4(-1f, 5.688111E-16f, -2.384976E-08f, 0f, 0f, -1f, -2.384976E-08f, 0f, -2.384976E-08f, -2.384976E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, -2.384976E-08f, 3.141593f)
            };
            // 122. - X: 0, Y: 0, Z: 0.1
            yield return new object[]
            {
                "X: 0, Y: 0, Z: 0.1",
                new float3(0f, 0f, 0.001745329f),
                new Quaternion(0f, 0f, 0.0008726645f, 0.9999996f),
                new float4x4(0.9999985f, -0.001745328f, 0f, 0f, 0.001745328f, 0.9999985f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0f, 0.001745329f)
            };
            // 123. - X: 0.1, Y: 0, Z: 0.1
            yield return new object[]
            {
                "X: 0.1, Y: 0, Z: 0.1",
                new float3(0.001745329f, 0f, 0.001745329f),
                new Quaternion(0.0008726642f, -7.615433E-07f, 0.0008726642f, 0.9999993f),
                new float4x4(0.9999985f, -0.001745328f, 1.136868E-13f, 0f, 0.001745326f, 0.999997f, -0.001745328f, 0f, 3.046171E-06f, 0.001745326f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 1.13687E-13f, 0.001745329f)
            };
            // 124. - X: 1, Y: 0, Z: 0.1
            yield return new object[]
            {
                "X: 1, Y: 0, Z: 0.1",
                new float3(0.01745329f, 0f, 0.001745329f),
                new Quaternion(0.008726533f, -7.615338E-06f, 0.0008726313f, 0.9999616f),
                new float4x4(0.9999985f, -0.001745328f, 0f, 0f, 0.001745063f, 0.9998462f, -0.01745241f, 0f, 3.046018E-05f, 0.01745238f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0f, 0.001745329f)
            };
            // 125. - X: 45, Y: 0, Z: 0.1
            yield return new object[]
            {
                "X: 45, Y: 0, Z: 0.1",
                new float3(0.7853982f, 0f, 0.001745329f),
                new Quaternion(0.3826833f, -0.0003339543f, 0.0008062369f, 0.9238791f),
                new float4x4(0.9999985f, -0.001745328f, 5.820766E-11f, 0f, 0.001234133f, 0.7071057f, -0.7071068f, 0f, 0.001234133f, 0.7071057f, 0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 8.231807E-11f, 0.001745329f)
            };
            // 126. - X: 90, Y: 0, Z: 0.1
            yield return new object[]
            {
                "X: 90, Y: 0, Z: 0.1",
                new float3(1.570796f, 0f, 0.001745329f),
                new Quaternion(0.7071065f, -0.000617067f, 0.000617067f, 0.7071065f),
                new float4x4(0.9999985f, -0.001745328f, 0f, 0f, 0f, -4.62876E-08f, -1f, 0f, 0.001745328f, 0.9999985f, -4.62876E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 6.28144f, 0f)
            };
            // 127. - X: 180, Y: 0, Z: 0.1
            yield return new object[]
            {
                "X: 180, Y: 0, Z: 0.1",
                new float3(3.141593f, 0f, 0.001745329f),
                new Quaternion(0.9999996f, -0.0008726645f, -3.814538E-11f, -4.371137E-08f),
                new float4x4(0.9999985f, -0.001745328f, 6.938894E-18f, 0f, -0.001745328f, -0.9999986f, 8.742278E-08f, 0f, -1.525814E-10f, -8.742266E-08f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 3.141593f, 3.143338f)
            };
            // 128. - X: 270, Y: 0, Z: 0.1
            yield return new object[]
            {
                "X: 270, Y: 0, Z: 0.1",
                new float3(4.712389f, 0f, 0.001745329f),
                new Quaternion(0.7071065f, -0.000617067f, -0.000617067f, -0.7071065f),
                new float4x4(0.9999985f, -0.001745328f, 0f, 0f, 0f, -4.62876E-08f, 1f, 0f, -0.001745328f, -0.9999985f, -4.62876E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.001745329f, 0f)
            };
            // 129. - X: 360, Y: 0, Z: 0.1
            yield return new object[]
            {
                "X: 360, Y: 0, Z: 0.1",
                new float3(6.283185f, 0f, 0.001745329f),
                new Quaternion(-8.742275E-08f, 7.629075E-11f, -0.0008726645f, -0.9999996f),
                new float4x4(0.9999985f, -0.001745328f, 1.387779E-17f, 0f, 0.001745328f, 0.9999985f, -1.748456E-07f, 0f, 3.051629E-10f, 1.748453E-07f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 1.387779E-17f, 0.001745329f)
            };
            // 130. - X: 585, Y: 0, Z: 0.1
            yield return new object[]
            {
                "X: 585, Y: 0, Z: 0.1",
                new float3(10.21018f, 0f, 0.001745329f),
                new Quaternion(-0.9238791f, 0.0008062368f, 0.0003339544f, 0.3826835f),
                new float4x4(0.9999985f, -0.001745328f, 0f, 0f, -0.001234133f, -0.7071054f, 0.707107f, 0f, -0.001234134f, -0.7071059f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.141593f, 3.143338f)
            };
            // 131. - X: -90, Y: 0, Z: 0.1
            yield return new object[]
            {
                "X: -90, Y: 0, Z: 0.1",
                new float3(-1.570796f, 0f, 0.001745329f),
                new Quaternion(-0.7071065f, 0.000617067f, 0.000617067f, 0.7071065f),
                new float4x4(0.9999985f, -0.001745328f, 0f, 0f, 0f, -4.62876E-08f, 1f, 0f, -0.001745328f, -0.9999985f, -4.62876E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.001745329f, 0f)
            };
            // 132. - X: -540, Y: 0, Z: 0.1
            yield return new object[]
            {
                "X: -540, Y: 0, Z: 0.1",
                new float3(-9.424778f, 0f, 0.001745329f),
                new Quaternion(0.9999996f, -0.0008726645f, 1.040642E-11f, 1.192488E-08f),
                new float4x4(0.9999985f, -0.001745328f, 0f, 0f, -0.001745328f, -0.9999986f, -2.384976E-08f, 0f, 4.162566E-11f, 2.384973E-08f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.141593f, 3.143338f)
            };
            // 133. - X: 0, Y: 0.1, Z: 0.1
            yield return new object[]
            {
                "X: 0, Y: 0.1, Z: 0.1",
                new float3(0f, 0.001745329f, 0.001745329f),
                new Quaternion(7.615433E-07f, 0.0008726642f, 0.0008726642f, 0.9999993f),
                new float4x4(0.999997f, -0.001745326f, 0.001745328f, 0f, 0.001745328f, 0.9999985f, 1.136868E-13f, 0f, -0.001745326f, 3.046171E-06f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(-1.136868E-13f, 0.001745329f, 0.001745329f)
            };
            // 134. - X: 0.1, Y: 0.1, Z: 0.1
            yield return new object[]
            {
                "X: 0.1, Y: 0.1, Z: 0.1",
                new float3(0.001745329f, 0.001745329f, 0.001745329f),
                new Quaternion(0.0008734255f, 0.0008719024f, 0.0008719023f, 0.9999989f),
                new float4x4(0.999997f, -0.00174228f, 0.001745326f, 0f, 0.001745326f, 0.999997f, -0.001745329f, 0f, -0.00174228f, 0.001748369f, 0.999997f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.00174533f, 0.001745329f, 0.001745329f)
            };
            // 135. - X: 1, Y: 0.1, Z: 0.1
            yield return new object[]
            {
                "X: 1, Y: 0.1, Z: 0.1",
                new float3(0.01745329f, 0.001745329f, 0.001745329f),
                new Quaternion(0.008727292f, 0.0008650157f, 0.0008650157f, 0.9999612f),
                new float4x4(0.999997f, -0.001714866f, 0.001745063f, 0f, 0.001745063f, 0.9998462f, -0.01745241f, 0f, -0.001714866f, 0.0174554f, 0.9998462f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.0174533f, 0.001745329f, 0.001745329f)
            };
            // 136. - X: 45, Y: 0.1, Z: 0.1
            yield return new object[]
            {
                "X: 45, Y: 0.1, Z: 0.1",
                new float3(0.7853982f, 0.001745329f, 0.001745329f),
                new Quaternion(0.3826839f, 0.0004722824f, 0.0004722824f, 0.9238791f),
                new float4x4(0.9999991f, -0.0005111939f, 0.001234133f, 0f, 0.001234133f, 0.7071057f, -0.7071068f, 0f, -0.0005111941f, 0.7071077f, 0.7071057f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 0.001745329f, 0.001745329f)
            };
            // 137. - X: 90, Y: 0.1, Z: 0.1
            yield return new object[]
            {
                "X: 90, Y: 0.1, Z: 0.1",
                new float3(1.570796f, 0.001745329f, 0.001745329f),
                new Quaternion(0.7071068f, 0f, 0f, 0.7071068f),
                new float4x4(1f, 0f, 0f, 0f, 0f, -1.192093E-07f, -1f, 0f, 0f, 1f, -1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 0f, 0f)
            };
            // 138. - X: 180, Y: 0.1, Z: 0.1
            yield return new object[]
            {
                "X: 180, Y: 0.1, Z: 0.1",
                new float3(3.141593f, 0.001745329f, 0.001745329f),
                new Quaternion(0.9999993f, -0.0008726643f, -0.0008726643f, 7.17832E-07f),
                new float4x4(0.999997f, -0.001745326f, -0.001745329f, 0f, -0.001745329f, -0.9999987f, 8.74229E-08f, 0f, -0.001745326f, 2.958749E-06f, -0.9999987f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.74229E-08f, 3.143338f, 3.143338f)
            };
            // 139. - X: 270, Y: 0.1, Z: 0.1
            yield return new object[]
            {
                "X: 270, Y: 0.1, Z: 0.1",
                new float3(4.712389f, 0.001745329f, 0.001745329f),
                new Quaternion(0.7071058f, -0.001234133f, -0.001234133f, -0.7071058f),
                new float4x4(0.9999939f, -0.003490651f, 0f, 0f, 0f, -1.255435E-07f, 1f, 0f, -0.003490651f, -0.999994f, -1.255435E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.003490658f, 0f)
            };
            // 140. - X: 360, Y: 0.1, Z: 0.1
            yield return new object[]
            {
                "X: 360, Y: 0.1, Z: 0.1",
                new float3(6.283185f, 0.001745329f, 0.001745329f),
                new Quaternion(-8.489661E-07f, -0.0008726642f, -0.0008726642f, -0.9999993f),
                new float4x4(0.999997f, -0.001745326f, 0.001745329f, 0f, 0.001745329f, 0.9999985f, -1.748455E-07f, 0f, -0.001745326f, 3.221016E-06f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748455E-07f, 0.001745329f, 0.001745329f)
            };
            // 141. - X: 585, Y: 0.1, Z: 0.1
            yield return new object[]
            {
                "X: 585, Y: 0.1, Z: 0.1",
                new float3(10.21018f, 0.001745329f, 0.001745329f),
                new Quaternion(-0.9238784f, 0.001140191f, 0.001140191f, 0.3826826f),
                new float4x4(0.9999948f, -0.002979458f, -0.001234133f, 0f, -0.001234133f, -0.7071053f, 0.707107f, 0f, -0.002979458f, -0.7071018f, -0.7071053f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.143338f, 3.143338f)
            };
            // 142. - X: -90, Y: 0.1, Z: 0.1
            yield return new object[]
            {
                "X: -90, Y: 0.1, Z: 0.1",
                new float3(-1.570796f, 0.001745329f, 0.001745329f),
                new Quaternion(-0.7071058f, 0.001234133f, 0.001234133f, 0.7071058f),
                new float4x4(0.9999939f, -0.003490651f, 0f, 0f, 0f, -1.255435E-07f, 1f, 0f, -0.003490651f, -0.999994f, -1.255435E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.003490658f, 0f)
            };
            // 143. - X: -540, Y: 0.1, Z: 0.1
            yield return new object[]
            {
                "X: -540, Y: 0.1, Z: 0.1",
                new float3(-9.424778f, 0.001745329f, 0.001745329f),
                new Quaternion(0.9999993f, -0.0008726642f, -0.0008726642f, 7.734682E-07f),
                new float4x4(0.999997f, -0.001745326f, -0.001745329f, 0f, -0.001745329f, -0.9999987f, -2.384968E-08f, 0f, -0.001745326f, 3.070021E-06f, -0.9999987f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384968E-08f, 3.143338f, 3.143338f)
            };
            // 144. - X: 0, Y: 1, Z: 0.1
            yield return new object[]
            {
                "X: 0, Y: 1, Z: 0.1",
                new float3(0f, 0.01745329f, 0.001745329f),
                new Quaternion(7.615338E-06f, 0.008726533f, 0.0008726313f, 0.9999616f),
                new float4x4(0.9998462f, -0.001745063f, 0.01745241f, 0f, 0.001745328f, 0.9999985f, 0f, 0f, -0.01745238f, 3.046018E-05f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0.01745329f, 0.001745329f)
            };
            // 145. - X: 0.1, Y: 1, Z: 0.1
            yield return new object[]
            {
                "X: 0.1, Y: 1, Z: 0.1",
                new float3(0.001745329f, 0.01745329f, 0.001745329f),
                new Quaternion(0.0008802463f, 0.008725768f, 0.0008650157f, 0.9999612f),
                new float4x4(0.9998462f, -0.001714602f, 0.01745238f, 0f, 0.001745326f, 0.999997f, -0.001745328f, 0f, -0.01744933f, 0.00177552f, 0.9998462f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0.01745329f, 0.001745329f)
            };
            // 146. - X: 1, Y: 1, Z: 0.1
            yield return new object[]
            {
                "X: 1, Y: 1, Z: 0.1",
                new float3(0.01745329f, 0.01745329f, 0.001745329f),
                new Quaternion(0.008733816f, 0.008718585f, 0.0007964456f, 0.9999235f),
                new float4x4(0.9998467f, -0.001440477f, 0.01744975f, 0f, 0.001745063f, 0.9998462f, -0.01745241f, 0f, -0.01742192f, 0.01748018f, 0.9996954f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0.01745329f, 0.001745329f)
            };
            // 147. - X: 45, Y: 1, Z: 0.1
            yield return new object[]
            {
                "X: 45, Y: 1, Z: 0.1",
                new float3(0.7853982f, 0.01745329f, 0.001745329f),
                new Quaternion(0.3826758f, 0.007728322f, -0.002533293f, 0.9238469f),
                new float4x4(0.9998677f, 0.01059563f, 0.01234071f, 0f, 0.001234133f, 0.7071057f, -0.7071068f, 0f, -0.01621843f, 0.7070285f, 0.7069991f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 0.01745329f, 0.001745329f)
            };
            // 148. - X: 90, Y: 1, Z: 0.1
            yield return new object[]
            {
                "X: 90, Y: 1, Z: 0.1",
                new float3(1.570796f, 0.01745329f, 0.001745329f),
                new Quaternion(0.707085f, 0.005553546f, -0.005553546f, 0.707085f),
                new float4x4(0.9998766f, 0.01570732f, 0f, 0f, 0f, 6.665505E-08f, -0.9999999f, 0f, -0.01570732f, 0.9998766f, 6.665505E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 0.01570796f, 0f)
            };
            // 149. - X: 180, Y: 1, Z: 0.1
            yield return new object[]
            {
                "X: 180, Y: 1, Z: 0.1",
                new float3(3.141593f, 0.01745329f, 0.001745329f),
                new Quaternion(0.9999616f, -0.0008726317f, -0.008726533f, 7.571628E-06f),
                new float4x4(0.9998462f, -0.001745064f, -0.01745241f, 0f, -0.001745328f, -0.9999985f, 8.742336E-08f, 0f, -0.01745238f, 3.037277E-05f, -0.9998478f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742337E-08f, 3.159046f, 3.143338f)
            };
            // 150. - X: 270, Y: 1, Z: 0.1
            yield return new object[]
            {
                "X: 270, Y: 1, Z: 0.1",
                new float3(4.712389f, 0.01745329f, 0.001745329f),
                new Quaternion(0.7070742f, -0.006787634f, -0.006787634f, -0.7070742f),
                new float4x4(0.9998157f, -0.01919744f, 0f, 0f, 0f, -5.475886E-08f, 1f, 0f, -0.01919744f, -0.9998158f, -5.475886E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.01919862f, 0f)
            };
            // 151. - X: 360, Y: 1, Z: 0.1
            yield return new object[]
            {
                "X: 360, Y: 1, Z: 0.1",
                new float3(6.283185f, 0.01745329f, 0.001745329f),
                new Quaternion(-7.702757E-06f, -0.008726533f, -0.0008726305f, -0.9999616f),
                new float4x4(0.9998462f, -0.00174506f, 0.01745241f, 0f, 0.001745328f, 0.9999985f, -1.74844E-07f, 0f, -0.01745238f, 3.0635E-05f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.74844E-07f, 0.01745329f, 0.001745329f)
            };
            // 152. - X: 585, Y: 1, Z: 0.1
            yield return new object[]
            {
                "X: 585, Y: 1, Z: 0.1",
                new float3(10.21018f, 0.01745329f, 0.001745329f),
                new Quaternion(-0.923841f, 0.004145707f, 0.008396205f, 0.3826618f),
                new float4x4(0.9998246f, -0.01408576f, -0.01234071f, 0f, -0.001234133f, -0.7071053f, 0.707107f, 0f, -0.01868633f, -0.7069678f, -0.7069988f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.159046f, 3.143338f)
            };
            // 153. - X: -90, Y: 1, Z: 0.1
            yield return new object[]
            {
                "X: -90, Y: 1, Z: 0.1",
                new float3(-1.570796f, 0.01745329f, 0.001745329f),
                new Quaternion(-0.7070742f, 0.006787634f, 0.006787634f, 0.7070742f),
                new float4x4(0.9998157f, -0.01919744f, 0f, 0f, 0f, -5.475886E-08f, 1f, 0f, -0.01919744f, -0.9998158f, -5.475886E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.01919862f, 0f)
            };
            // 154. - X: -540, Y: 1, Z: 0.1
            yield return new object[]
            {
                "X: -540, Y: 1, Z: 0.1",
                new float3(-9.424778f, 0.01745329f, 0.001745329f),
                new Quaternion(0.9999616f, -0.0008726311f, -0.008726533f, 7.627262E-06f),
                new float4x4(0.9998462f, -0.001745062f, -0.01745241f, 0f, -0.001745328f, -0.9999985f, -2.384968E-08f, 0f, -0.01745238f, 3.048403E-05f, -0.9998478f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384968E-08f, 3.159046f, 3.143338f)
            };
            // 155. - X: 0, Y: 45, Z: 0.1
            yield return new object[]
            {
                "X: 0, Y: 45, Z: 0.1",
                new float3(0f, 0.7853982f, 0.001745329f),
                new Quaternion(0.0003339543f, 0.3826833f, 0.0008062369f, 0.9238791f),
                new float4x4(0.7071057f, -0.001234133f, 0.7071068f, 0f, 0.001745328f, 0.9999985f, 5.820766E-11f, 0f, -0.7071057f, 0.001234133f, 0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(-5.820766E-11f, 0.7853982f, 0.001745329f)
            };
            // 156. - X: 0.1, Y: 45, Z: 0.1
            yield return new object[]
            {
                "X: 0.1, Y: 45, Z: 0.1",
                new float3(0.001745329f, 0.7853982f, 0.001745329f),
                new Quaternion(0.001140191f, 0.3826824f, 0.0004722824f, 0.9238791f),
                new float4x4(0.7071078f, -1.688022E-09f, 0.7071057f, 0f, 0.001745326f, 0.999997f, -0.001745328f, 0f, -0.7071036f, 0.002468265f, 0.7071057f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.00174533f, 0.7853982f, 0.001745329f)
            };
            // 157. - X: 1, Y: 45, Z: 0.1
            yield return new object[]
            {
                "X: 1, Y: 45, Z: 0.1",
                new float3(0.01745329f, 0.7853982f, 0.001745329f),
                new Quaternion(0.008396205f, 0.3826617f, -0.002533293f, 0.9238469f),
                new float4x4(0.7071272f, 0.01110656f, 0.7069991f, 0f, 0.001745062f, 0.9998462f, -0.01745241f, 0f, -0.7070842f, 0.01357483f, 0.7069991f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0.7853982f, 0.001745329f)
            };
            // 158. - X: 45, Y: 45, Z: 0.1
            yield return new object[]
            {
                "X: 45, Y: 45, Z: 0.1",
                new float3(0.7853982f, 0.7853982f, 0.001745329f),
                new Quaternion(0.3538618f, 0.3532448f, -0.1457017f, 0.8536808f),
                new float4x4(0.7079783f, 0.4987652f, 0.5f, 0f, 0.001234159f, 0.7071056f, -0.7071069f, 0f, -0.7062331f, 0.5012334f, 0.4999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 0.7853982f, 0.001745365f)
            };
            // 159. - X: 90, Y: 45, Z: 0.1
            yield return new object[]
            {
                "X: 90, Y: 45, Z: 0.1",
                new float3(1.570796f, 0.7853982f, 0.001745329f),
                new Quaternion(0.6535174f, 0.2700279f, -0.2700279f, 0.6535174f),
                new float4x4(0.7083398f, 0.7058716f, 0f, 0f, 0f, -1.490116E-08f, -1f, 0f, -0.7058716f, 0.7083398f, -1.490116E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 0.7836529f, 0f)
            };
            // 160. - X: 180, Y: 45, Z: 0.1
            yield return new object[]
            {
                "X: 180, Y: 45, Z: 0.1",
                new float3(3.141593f, 0.7853982f, 0.001745329f),
                new Quaternion(0.9238791f, -0.0008062536f, -0.3826833f, 0.0003339139f),
                new float4x4(0.7071057f, -0.001234195f, -0.7071068f, 0f, -0.001745328f, -0.9999985f, 8.742791E-08f, 0f, -0.7071057f, 0.001234072f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742791E-08f, 3.926991f, 3.143338f)
            };
            // 161. - X: 270, Y: 45, Z: 0.1
            yield return new object[]
            {
                "X: 270, Y: 45, Z: 0.1",
                new float3(4.712389f, 0.7853982f, 0.001745329f),
                new Quaternion(0.6530451f, -0.2711681f, -0.2711681f, -0.6530451f),
                new float4x4(0.7058716f, -0.7083398f, 0f, 0f, 0f, 1.043081E-07f, 0.9999999f, 0f, -0.7083398f, -0.7058715f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.7871436f, 0f)
            };
            // 162. - X: 360, Y: 45, Z: 0.1
            yield return new object[]
            {
                "X: 360, Y: 45, Z: 0.1",
                new float3(6.283185f, 0.7853982f, 0.001745329f),
                new Quaternion(-0.000334035f, -0.3826833f, -0.0008062034f, -0.9238791f),
                new float4x4(0.7071057f, -0.00123401f, 0.7071068f, 0f, 0.001745328f, 0.9999985f, -1.747976E-07f, 0f, -0.7071057f, 0.001234257f, 0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.747976E-07f, 0.7853982f, 0.001745329f)
            };
            // 163. - X: 585, Y: 45, Z: 0.1
            yield return new object[]
            {
                "X: 585, Y: 45, Z: 0.1",
                new float3(10.21018f, 0.7853982f, 0.001745329f),
                new Quaternion(-0.8534252f, 0.1471915f, 0.3538618f, 0.3532449f),
                new float4x4(0.706233f, -0.5012336f, -0.4999999f, 0f, -0.001234144f, -0.7071056f, 0.7071071f, 0f, -0.7079785f, -0.4987653f, -0.4999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.926991f, 3.143338f)
            };
            // 164. - X: -90, Y: 45, Z: 0.1
            yield return new object[]
            {
                "X: -90, Y: 45, Z: 0.1",
                new float3(-1.570796f, 0.7853982f, 0.001745329f),
                new Quaternion(-0.6530451f, 0.2711681f, 0.2711681f, 0.6530451f),
                new float4x4(0.7058716f, -0.7083398f, 0f, 0f, 0f, 1.043081E-07f, 0.9999999f, 0f, -0.7083398f, -0.7058715f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.7871436f, 0f)
            };
            // 165. - X: -540, Y: 45, Z: 0.1
            yield return new object[]
            {
                "X: -540, Y: 45, Z: 0.1",
                new float3(-9.424778f, 0.7853982f, 0.001745329f),
                new Quaternion(0.9238791f, -0.0008062323f, -0.3826833f, 0.0003339653f),
                new float4x4(0.7071057f, -0.001234117f, -0.7071068f, 0f, -0.001745328f, -0.9999985f, -2.380693E-08f, 0f, -0.7071057f, 0.00123415f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.380693E-08f, 3.926991f, 3.143338f)
            };
            // 166. - X: 0, Y: 90, Z: 0.1
            yield return new object[]
            {
                "X: 0, Y: 90, Z: 0.1",
                new float3(0f, 1.570796f, 0.001745329f),
                new Quaternion(0.000617067f, 0.7071065f, 0.000617067f, 0.7071065f),
                new float4x4(-4.62876E-08f, 0f, 1f, 0f, 0.001745328f, 0.9999985f, 0f, 0f, -0.9999985f, 0.001745328f, -4.62876E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.570796f, 0.001745329f)
            };
            // 167. - X: 0.1, Y: 90, Z: 0.1
            yield return new object[]
            {
                "X: 0.1, Y: 90, Z: 0.1",
                new float3(0.001745329f, 1.570796f, 0.001745329f),
                new Quaternion(0.001234133f, 0.7071058f, 0f, 0.7071068f),
                new float4x4(2.920628E-06f, 0.001745326f, 0.9999986f, 0f, 0.001745326f, 0.999997f, -0.001745328f, 0f, -0.9999986f, 0.001745328f, -1.255435E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 1.570796f, 0.001745329f)
            };
            // 168. - X: 1, Y: 90, Z: 0.1
            yield return new object[]
            {
                "X: 1, Y: 90, Z: 0.1",
                new float3(0.01745329f, 1.570796f, 0.001745329f),
                new Quaternion(0.006787634f, 0.7070742f, -0.005553546f, 0.707085f),
                new float4x4(3.040542E-05f, 0.01745238f, 0.9998477f, 0f, 0.001745064f, 0.9998462f, -0.01745241f, 0f, -0.9999985f, 0.001745328f, -5.475886E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.0174533f, 1.570796f, 0.001745329f)
            };
            // 169. - X: 45, Y: 90, Z: 0.1
            yield return new object[]
            {
                "X: 45, Y: 90, Z: 0.1",
                new float3(0.7853982f, 1.570796f, 0.001745329f),
                new Quaternion(0.2711681f, 0.6530451f, -0.2700279f, 0.6535174f),
                new float4x4(0.001234218f, 0.7071058f, 0.7071067f, 0f, 0.001234084f, 0.7071056f, -0.7071068f, 0f, -0.9999985f, 0.001745313f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 1.570796f, 0.00174526f)
            };
            // 170. - X: 90, Y: 90, Z: 0.1
            yield return new object[]
            {
                "X: 90, Y: 90, Z: 0.1",
                new float3(1.570796f, 1.570796f, 0.001745329f),
                new Quaternion(0.5004361f, 0.4995635f, -0.4995635f, 0.5004361f),
                new float4x4(0.001745403f, 0.9999984f, 0f, 0f, 0f, 8.940697E-08f, -0.9999999f, 0f, -0.9999984f, 0.001745313f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 1.569051f, 0f)
            };
            // 171. - X: 180, Y: 90, Z: 0.1
            yield return new object[]
            {
                "X: 180, Y: 90, Z: 0.1",
                new float3(3.141593f, 1.570796f, 0.001745329f),
                new Quaternion(0.7071065f, -0.0006170979f, -0.7071065f, 0.0006170361f),
                new float4x4(-4.636388E-08f, -8.73697E-08f, -1f, 0f, -0.001745328f, -0.9999986f, 8.73697E-08f, 0f, -0.9999985f, 0.001745328f, -4.636388E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.73697E-08f, 4.712389f, 3.143338f)
            };
            // 172. - X: 270, Y: 90, Z: 0.1
            yield return new object[]
            {
                "X: 270, Y: 90, Z: 0.1",
                new float3(4.712389f, 1.570796f, 0.001745329f),
                new Quaternion(0.4995635f, -0.5004361f, -0.5004361f, -0.4995635f),
                new float4x4(-0.001745224f, -0.9999984f, 0f, 0f, 0f, 8.940697E-08f, 0.9999999f, 0f, -0.9999984f, 0.001745313f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.572542f, 0f)
            };
            // 173. - X: 360, Y: 90, Z: 0.1
            yield return new object[]
            {
                "X: 360, Y: 90, Z: 0.1",
                new float3(6.283185f, 1.570796f, 0.001745329f),
                new Quaternion(-0.0006171288f, -0.7071065f, -0.0006170052f, -0.7071065f),
                new float4x4(-4.613503E-08f, 1.747976E-07f, 1f, 0f, 0.001745328f, 0.9999985f, -1.747976E-07f, 0f, -0.9999985f, 0.001745328f, -4.644016E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.747976E-07f, 1.570796f, 0.001745329f)
            };
            // 174. - X: 585, Y: 90, Z: 0.1
            yield return new object[]
            {
                "X: 585, Y: 90, Z: 0.1",
                new float3(10.21018f, 1.570796f, 0.001745329f),
                new Quaternion(-0.653045f, 0.2711682f, 0.6535173f, 0.270028f),
                new float4x4(-0.001234084f, -0.707106f, -0.7071064f, 0f, -0.001234084f, -0.7071053f, 0.7071071f, 0f, -0.9999984f, 0.001745313f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 4.712389f, 3.143338f)
            };
            // 175. - X: -90, Y: 90, Z: 0.1
            yield return new object[]
            {
                "X: -90, Y: 90, Z: 0.1",
                new float3(-1.570796f, 1.570796f, 0.001745329f),
                new Quaternion(-0.4995635f, 0.5004361f, 0.5004361f, 0.4995635f),
                new float4x4(-0.001745224f, -0.9999984f, 0f, 0f, 0f, 8.940697E-08f, 0.9999999f, 0f, -0.9999984f, 0.001745313f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.572542f, 0f)
            };
            // 176. - X: -540, Y: 90, Z: 0.1
            yield return new object[]
            {
                "X: -540, Y: 90, Z: 0.1",
                new float3(-9.424778f, 1.570796f, 0.001745329f),
                new Quaternion(0.7071065f, -0.0006170585f, -0.7071065f, 0.0006170754f),
                new float4x4(-4.626673E-08f, 2.392335E-08f, -1f, 0f, -0.001745328f, -0.9999986f, -2.392335E-08f, 0f, -0.9999985f, 0.001745328f, -4.626673E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.392335E-08f, 4.712389f, 3.143338f)
            };
            // 177. - X: 0, Y: 180, Z: 0.1
            yield return new object[]
            {
                "X: 0, Y: 180, Z: 0.1",
                new float3(0f, 3.141593f, 0.001745329f),
                new Quaternion(0.0008726645f, 0.9999996f, -3.814538E-11f, -4.371137E-08f),
                new float4x4(-0.9999986f, 0.001745328f, -8.742278E-08f, 0f, 0.001745328f, 0.9999985f, 6.938894E-18f, 0f, 8.742266E-08f, -1.525814E-10f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-6.938894E-18f, 3.141593f, 0.001745329f)
            };
            // 178. - X: 0.1, Y: 180, Z: 0.1
            yield return new object[]
            {
                "X: 0.1, Y: 180, Z: 0.1",
                new float3(0.001745329f, 3.141593f, 0.001745329f),
                new Quaternion(0.0008726642f, 0.9999993f, -0.0008726643f, 7.17832E-07f),
                new float4x4(-0.9999987f, 0.001745328f, -8.742268E-08f, 0f, 0.001745326f, 0.999997f, -0.001745329f, 0f, -2.958749E-06f, -0.001745326f, -0.9999987f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.141593f, 0.001745329f)
            };
            // 179. - X: 1, Y: 180, Z: 0.1
            yield return new object[]
            {
                "X: 1, Y: 180, Z: 0.1",
                new float3(0.01745329f, 3.141593f, 0.001745329f),
                new Quaternion(0.0008726309f, 0.9999616f, -0.008726533f, 7.571628E-06f),
                new float4x4(-0.9999985f, 0.001745327f, -8.740972E-08f, 0f, 0.001745063f, 0.9998462f, -0.01745241f, 0f, -3.037276E-05f, -0.01745238f, -0.9998478f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.141593f, 0.001745329f)
            };
            // 180. - X: 45, Y: 180, Z: 0.1
            yield return new object[]
            {
                "X: 45, Y: 180, Z: 0.1",
                new float3(0.7853982f, 3.141593f, 0.001745329f),
                new Quaternion(0.0008062202f, 0.9238791f, -0.3826833f, 0.0003339139f),
                new float4x4(-0.9999985f, 0.001745267f, -6.187474E-08f, 0f, 0.001234133f, 0.7071057f, -0.7071068f, 0f, -0.001234046f, -0.7071057f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 3.141593f, 0.001745329f)
            };
            // 181. - X: 90, Y: 180, Z: 0.1
            yield return new object[]
            {
                "X: 90, Y: 180, Z: 0.1",
                new float3(1.570796f, 3.141593f, 0.001745329f),
                new Quaternion(0.0006170361f, 0.7071065f, -0.7071065f, 0.0006170361f),
                new float4x4(-0.9999986f, 0.001745241f, 0f, 0f, 0f, -4.621131E-08f, -1f, 0f, -0.001745241f, -0.9999985f, -4.621131E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 3.139848f, 0f)
            };
            // 182. - X: 180, Y: 180, Z: 0.1
            yield return new object[]
            {
                "X: 180, Y: 180, Z: 0.1",
                new float3(3.141593f, 3.141593f, 0.001745329f),
                new Quaternion(-4.374952E-08f, -4.367323E-08f, -0.9999996f, 0.0008726645f),
                new float4x4(-0.9999986f, 0.001745328f, 8.742278E-08f, 0f, -0.001745328f, -0.9999986f, 8.742278E-08f, 0f, 8.757524E-08f, 8.727007E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 8.742278E-08f, 3.143338f)
            };
            // 183. - X: 270, Y: 180, Z: 0.1
            yield return new object[]
            {
                "X: 270, Y: 180, Z: 0.1",
                new float3(4.712389f, 3.141593f, 0.001745329f),
                new Quaternion(-0.0006170979f, -0.7071065f, -0.7071065f, 0.0006170979f),
                new float4x4(-0.9999986f, 0.001745416f, 0f, 0f, 0f, -4.636388E-08f, 1f, 0f, 0.001745416f, 0.9999985f, -4.636388E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.143338f, 0f)
            };
            // 184. - X: 360, Y: 180, Z: 0.1
            yield return new object[]
            {
                "X: 360, Y: 180, Z: 0.1",
                new float3(6.283185f, 3.141593f, 0.001745329f),
                new Quaternion(-0.0008726645f, -0.9999996f, 8.746089E-08f, 4.363508E-08f),
                new float4x4(-0.9999986f, 0.001745328f, -8.742278E-08f, 0f, 0.001745328f, 0.9999985f, -1.748456E-07f, 0f, 8.711749E-08f, -1.749979E-07f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 3.141593f, 0.001745329f)
            };
            // 185. - X: 585, Y: 180, Z: 0.1
            yield return new object[]
            {
                "X: 585, Y: 180, Z: 0.1",
                new float3(10.21018f, 3.141593f, 0.001745329f),
                new Quaternion(0.0003339948f, 0.3826835f, 0.9238791f, -0.0008062535f),
                new float4x4(-0.9999985f, 0.00174539f, 6.181654E-08f, 0f, -0.001234133f, -0.7071054f, 0.707107f, 0f, 0.001234221f, 0.7071059f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 8.750413E-08f, 3.143338f)
            };
            // 186. - X: -90, Y: 180, Z: 0.1
            yield return new object[]
            {
                "X: -90, Y: 180, Z: 0.1",
                new float3(-1.570796f, 3.141593f, 0.001745329f),
                new Quaternion(0.0006170979f, 0.7071065f, 0.7071065f, -0.0006170979f),
                new float4x4(-0.9999986f, 0.001745416f, 0f, 0f, 0f, -4.636388E-08f, 1f, 0f, 0.001745416f, 0.9999985f, -4.636388E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.143338f, 0f)
            };
            // 187. - X: -540, Y: 180, Z: 0.1
            yield return new object[]
            {
                "X: -540, Y: 180, Z: 0.1",
                new float3(-9.424778f, 3.141593f, 0.001745329f),
                new Quaternion(-4.370097E-08f, 1.196302E-08f, -0.9999996f, 0.0008726645f),
                new float4x4(-0.9999986f, 0.001745328f, 8.742279E-08f, 0f, -0.001745328f, -0.9999986f, -2.384976E-08f, 0f, 8.738103E-08f, -2.400231E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 8.742279E-08f, 3.143338f)
            };
            // 188. - X: 0, Y: 270, Z: 0.1
            yield return new object[]
            {
                "X: 0, Y: 270, Z: 0.1",
                new float3(0f, 4.712389f, 0.001745329f),
                new Quaternion(0.000617067f, 0.7071065f, -0.000617067f, -0.7071065f),
                new float4x4(-4.62876E-08f, 0f, -1f, 0f, 0.001745328f, 0.9999985f, 0f, 0f, 0.9999985f, -0.001745328f, -4.62876E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 0.001745329f)
            };
            // 189. - X: 0.1, Y: 270, Z: 0.1
            yield return new object[]
            {
                "X: 0.1, Y: 270, Z: 0.1",
                new float3(0.001745329f, 4.712389f, 0.001745329f),
                new Quaternion(0f, 0.7071068f, -0.001234133f, -0.7071058f),
                new float4x4(-3.16538E-06f, -0.001745326f, -0.9999986f, 0f, 0.001745326f, 0.999997f, -0.001745328f, 0f, 0.9999986f, -0.001745328f, -1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 4.712389f, 0.001745329f)
            };
            // 190. - X: 1, Y: 270, Z: 0.1
            yield return new object[]
            {
                "X: 1, Y: 270, Z: 0.1",
                new float3(0.01745329f, 4.712389f, 0.001745329f),
                new Quaternion(-0.005553546f, 0.707085f, -0.006787634f, -0.7070742f),
                new float4x4(-3.039352E-05f, -0.01745238f, -0.9998477f, 0f, 0.001745064f, 0.9998462f, -0.01745241f, 0f, 0.9999985f, -0.001745328f, 6.665505E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 4.712389f, 0.001745331f)
            };
            // 191. - X: 45, Y: 270, Z: 0.1
            yield return new object[]
            {
                "X: 45, Y: 270, Z: 0.1",
                new float3(0.7853982f, 4.712389f, 0.001745329f),
                new Quaternion(-0.2700279f, 0.6535174f, -0.2711681f, -0.6530451f),
                new float4x4(-0.001234129f, -0.7071058f, -0.7071067f, 0f, 0.001234084f, 0.7071056f, -0.7071068f, 0f, 0.9999985f, -0.001745313f, -1.490116E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853984f, 4.712389f, 0.001745302f)
            };
            // 192. - X: 90, Y: 270, Z: 0.1
            yield return new object[]
            {
                "X: 90, Y: 270, Z: 0.1",
                new float3(1.570796f, 4.712389f, 0.001745329f),
                new Quaternion(-0.4995635f, 0.5004361f, -0.5004361f, -0.4995635f),
                new float4x4(-0.001745224f, -0.9999984f, 0f, 0f, 0f, 8.940697E-08f, -0.9999999f, 0f, 0.9999984f, -0.001745313f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 4.710643f, 0f)
            };
            // 193. - X: 180, Y: 270, Z: 0.1
            yield return new object[]
            {
                "X: 180, Y: 270, Z: 0.1",
                new float3(3.141593f, 4.712389f, 0.001745329f),
                new Quaternion(-0.7071065f, 0.0006170361f, -0.7071065f, 0.0006170979f),
                new float4x4(-4.621131E-08f, 8.73697E-08f, 1f, 0f, -0.001745328f, -0.9999986f, 8.73697E-08f, 0f, 0.9999985f, -0.001745328f, -4.621131E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.73697E-08f, 1.570796f, 3.143338f)
            };
            // 194. - X: 270, Y: 270, Z: 0.1
            yield return new object[]
            {
                "X: 270, Y: 270, Z: 0.1",
                new float3(4.712389f, 4.712389f, 0.001745329f),
                new Quaternion(-0.5004361f, -0.4995635f, -0.4995635f, 0.5004361f),
                new float4x4(0.001745403f, 0.9999984f, 0f, 0f, 0f, 8.940697E-08f, 0.9999999f, 0f, 0.9999984f, -0.001745313f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.714134f, 0f)
            };
            // 195. - X: 360, Y: 270, Z: 0.1
            yield return new object[]
            {
                "X: 360, Y: 270, Z: 0.1",
                new float3(6.283185f, 4.712389f, 0.001745329f),
                new Quaternion(-0.0006170052f, -0.7071065f, 0.0006171288f, 0.7071065f),
                new float4x4(-4.644016E-08f, -1.747976E-07f, -1f, 0f, 0.001745328f, 0.9999985f, -1.747976E-07f, 0f, 0.9999985f, -0.001745328f, -4.613503E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.747976E-07f, 4.712389f, 0.001745329f)
            };
            // 196. - X: 585, Y: 270, Z: 0.1
            yield return new object[]
            {
                "X: 585, Y: 270, Z: 0.1",
                new float3(10.21018f, 4.712389f, 0.001745329f),
                new Quaternion(0.6535173f, 0.270028f, 0.653045f, -0.2711682f),
                new float4x4(0.001234218f, 0.707106f, 0.7071064f, 0f, -0.001234084f, -0.7071053f, 0.7071071f, 0f, 0.9999984f, -0.001745313f, 4.470348E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.570796f, 3.143338f)
            };
            // 197. - X: -90, Y: 270, Z: 0.1
            yield return new object[]
            {
                "X: -90, Y: 270, Z: 0.1",
                new float3(-1.570796f, 4.712389f, 0.001745329f),
                new Quaternion(0.5004361f, 0.4995635f, 0.4995635f, -0.5004361f),
                new float4x4(0.001745403f, 0.9999984f, 0f, 0f, 0f, 8.940697E-08f, 0.9999999f, 0f, 0.9999984f, -0.001745313f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.714134f, 0f)
            };
            // 198. - X: -540, Y: 270, Z: 0.1
            yield return new object[]
            {
                "X: -540, Y: 270, Z: 0.1",
                new float3(-9.424778f, 4.712389f, 0.001745329f),
                new Quaternion(-0.7071065f, 0.0006170754f, -0.7071065f, 0.0006170585f),
                new float4x4(-4.63084E-08f, -2.392335E-08f, 1f, 0f, -0.001745328f, -0.9999986f, -2.392335E-08f, 0f, 0.9999985f, -0.001745328f, -4.63084E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.392335E-08f, 1.570796f, 3.143338f)
            };
            // 199. - X: 0, Y: 360, Z: 0.1
            yield return new object[]
            {
                "X: 0, Y: 360, Z: 0.1",
                new float3(0f, 6.283185f, 0.001745329f),
                new Quaternion(-7.629075E-11f, -8.742275E-08f, -0.0008726645f, -0.9999996f),
                new float4x4(0.9999985f, -0.001745328f, 1.748456E-07f, 0f, 0.001745328f, 0.9999985f, 1.387779E-17f, 0f, -1.748453E-07f, 3.051629E-10f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-1.387779E-17f, 1.748456E-07f, 0.001745329f)
            };
            // 200. - X: 0.1, Y: 360, Z: 0.1
            yield return new object[]
            {
                "X: 0.1, Y: 360, Z: 0.1",
                new float3(0.001745329f, 6.283185f, 0.001745329f),
                new Quaternion(-0.0008726643f, 6.741206E-07f, -0.0008726642f, -0.9999993f),
                new float4x4(0.9999985f, -0.001745328f, 1.748454E-07f, 0f, 0.001745326f, 0.999997f, -0.001745328f, 0f, 2.871326E-06f, 0.001745326f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 1.748456E-07f, 0.001745329f)
            };
            // 201. - X: 1, Y: 360, Z: 0.1
            yield return new object[]
            {
                "X: 1, Y: 360, Z: 0.1",
                new float3(0.01745329f, 6.283185f, 0.001745329f),
                new Quaternion(-0.008726533f, 7.527919E-06f, -0.0008726305f, -0.9999616f),
                new float4x4(0.9999985f, -0.001745325f, 1.748185E-07f, 0f, 0.001745063f, 0.9998462f, -0.01745241f, 0f, 3.028534E-05f, 0.01745238f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 1.748452E-07f, 0.001745329f)
            };
            // 202. - X: 45, Y: 360, Z: 0.1
            yield return new object[]
            {
                "X: 45, Y: 360, Z: 0.1",
                new float3(0.7853982f, 6.283185f, 0.001745329f),
                new Quaternion(-0.3826833f, 0.0003338735f, -0.0008062034f, -0.9238791f),
                new float4x4(0.9999985f, -0.001745205f, 1.236913E-07f, 0f, 0.001234133f, 0.7071057f, -0.7071068f, 0f, 0.001233959f, 0.7071057f, 0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 1.749259E-07f, 0.001745329f)
            };
            // 203. - X: 90, Y: 360, Z: 0.1
            yield return new object[]
            {
                "X: 90, Y: 360, Z: 0.1",
                new float3(1.570796f, 6.283185f, 0.001745329f),
                new Quaternion(-0.7071065f, 0.0006170052f, -0.0006170052f, -0.7071065f),
                new float4x4(0.9999985f, -0.001745154f, 0f, 0f, 0f, -4.613503E-08f, -1f, 0f, 0.001745154f, 0.9999985f, -4.613503E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 6.28144f, 0f)
            };
            // 204. - X: 180, Y: 360, Z: 0.1
            yield return new object[]
            {
                "X: 180, Y: 360, Z: 0.1",
                new float3(3.141593f, 6.283185f, 0.001745329f),
                new Quaternion(-0.9999996f, 0.0008726645f, 8.746089E-08f, 4.363508E-08f),
                new float4x4(0.9999985f, -0.001745328f, -1.748456E-07f, 0f, -0.001745328f, -0.9999986f, 8.742278E-08f, 0f, -1.749979E-07f, -8.711749E-08f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 3.141593f, 3.143338f)
            };
            // 205. - X: 270, Y: 360, Z: 0.1
            yield return new object[]
            {
                "X: 270, Y: 360, Z: 0.1",
                new float3(4.712389f, 6.283185f, 0.001745329f),
                new Quaternion(-0.7071065f, 0.0006171288f, 0.0006171288f, 0.7071065f),
                new float4x4(0.9999985f, -0.001745503f, 0f, 0f, 0f, -4.644016E-08f, 1f, 0f, -0.001745503f, -0.9999985f, -4.644016E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.001745504f, 0f)
            };
            // 206. - X: 360, Y: 360, Z: 0.1
            yield return new object[]
            {
                "X: 360, Y: 360, Z: 0.1",
                new float3(6.283185f, 6.283185f, 0.001745329f),
                new Quaternion(8.749904E-08f, 8.734646E-08f, 0.0008726645f, 0.9999996f),
                new float4x4(0.9999985f, -0.001745328f, 1.748456E-07f, 0f, 0.001745328f, 0.9999985f, -1.748456E-07f, 0f, -1.745401E-07f, 1.751505E-07f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 1.748456E-07f, 0.001745329f)
            };
            // 207. - X: 585, Y: 360, Z: 0.1
            yield return new object[]
            {
                "X: 585, Y: 360, Z: 0.1",
                new float3(10.21018f, 6.283185f, 0.001745329f),
                new Quaternion(0.9238791f, -0.0008062703f, -0.0003340352f, -0.3826835f),
                new float4x4(0.9999985f, -0.001745452f, -1.236331E-07f, 0f, -0.001234133f, -0.7071054f, 0.707107f, 0f, -0.001234309f, -0.7071059f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.141593f, 3.143338f)
            };
            // 208. - X: -90, Y: 360, Z: 0.1
            yield return new object[]
            {
                "X: -90, Y: 360, Z: 0.1",
                new float3(-1.570796f, 6.283185f, 0.001745329f),
                new Quaternion(0.7071065f, -0.0006171288f, -0.0006171288f, -0.7071065f),
                new float4x4(0.9999985f, -0.001745503f, 0f, 0f, 0f, -4.644016E-08f, 1f, 0f, -0.001745503f, -0.9999985f, -4.644016E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.001745504f, 0f)
            };
            // 209. - X: -540, Y: 360, Z: 0.1
            yield return new object[]
            {
                "X: -540, Y: 360, Z: 0.1",
                new float3(-9.424778f, 6.283185f, 0.001745329f),
                new Quaternion(-0.9999996f, 0.0008726645f, 8.741234E-08f, -1.200117E-08f),
                new float4x4(0.9999985f, -0.001745328f, -1.748456E-07f, 0f, -0.001745328f, -0.9999986f, -2.384976E-08f, 0f, -1.748037E-07f, 2.415489E-08f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.141593f, 3.143338f)
            };
            // 210. - X: 0, Y: 585, Z: 0.1
            yield return new object[]
            {
                "X: 0, Y: 585, Z: 0.1",
                new float3(0f, 10.21018f, 0.001745329f),
                new Quaternion(-0.0008062368f, -0.9238791f, 0.0003339544f, 0.3826835f),
                new float4x4(-0.7071054f, 0.001234133f, -0.707107f, 0f, 0.001745328f, 0.9999985f, 0f, 0f, 0.7071059f, -0.001234134f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.926991f, 0.001745329f)
            };
            // 211. - X: 0.1, Y: 585, Z: 0.1
            yield return new object[]
            {
                "X: 0.1, Y: 585, Z: 0.1",
                new float3(0.001745329f, 10.21018f, 0.001745329f),
                new Quaternion(-0.0004722822f, -0.923879f, 0.001140191f, 0.3826826f),
                new float4x4(-0.7071075f, 9.313226E-10f, -0.7071059f, 0f, 0.001745326f, 0.999997f, -0.001745328f, 0f, 0.7071038f, -0.002468265f, -0.7071053f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.926991f, 0.001745329f)
            };
            // 212. - X: 1, Y: 585, Z: 0.1
            yield return new object[]
            {
                "X: 1, Y: 585, Z: 0.1",
                new float3(0.01745329f, 10.21018f, 0.001745329f),
                new Quaternion(0.002533295f, -0.9238468f, 0.008396205f, 0.3826618f),
                new float4x4(-0.7071269f, -0.01110657f, -0.7069993f, 0f, 0.001745062f, 0.9998462f, -0.01745241f, 0f, 0.7070844f, -0.01357483f, -0.7069988f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.926991f, 0.001745329f)
            };
            // 213. - X: 45, Y: 585, Z: 0.1
            yield return new object[]
            {
                "X: 45, Y: 585, Z: 0.1",
                new float3(0.7853982f, 10.21018f, 0.001745329f),
                new Quaternion(0.1457018f, -0.8536808f, 0.3538618f, 0.3532449f),
                new float4x4(-0.7079782f, -0.4987653f, -0.5000001f, 0f, 0.001234129f, 0.7071056f, -0.7071069f, 0f, 0.7062333f, -0.5012333f, -0.4999998f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 3.926991f, 0.001745323f)
            };
            // 214. - X: 90, Y: 585, Z: 0.1
            yield return new object[]
            {
                "X: 90, Y: 585, Z: 0.1",
                new float3(1.570796f, 10.21018f, 0.001745329f),
                new Quaternion(0.270028f, -0.6535173f, 0.6535173f, 0.270028f),
                new float4x4(-0.7083395f, -0.7058719f, 0f, 0f, 0f, 4.470348E-08f, -0.9999999f, 0f, 0.7058719f, -0.7083395f, 4.470348E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 3.925246f, 0f)
            };
            // 215. - X: 180, Y: 585, Z: 0.1
            yield return new object[]
            {
                "X: 180, Y: 585, Z: 0.1",
                new float3(3.141593f, 10.21018f, 0.001745329f),
                new Quaternion(0.3826835f, -0.000333914f, 0.9238791f, -0.0008062535f),
                new float4x4(-0.7071054f, 0.001234195f, 0.707107f, 0f, -0.001745328f, -0.9999985f, 8.742791E-08f, 0f, 0.7071059f, -0.001234072f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742791E-08f, 0.7853985f, 3.143338f)
            };
            // 216. - X: 270, Y: 585, Z: 0.1
            yield return new object[]
            {
                "X: 270, Y: 585, Z: 0.1",
                new float3(4.712389f, 10.21018f, 0.001745329f),
                new Quaternion(0.2711682f, 0.653045f, 0.653045f, -0.2711682f),
                new float4x4(-0.7058711f, 0.70834f, 0f, 0f, 0f, 8.940697E-08f, 0.9999999f, 0f, 0.70834f, 0.7058712f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.928736f, 0f)
            };
            // 217. - X: 360, Y: 585, Z: 0.1
            yield return new object[]
            {
                "X: 360, Y: 585, Z: 0.1",
                new float3(6.283185f, 10.21018f, 0.001745329f),
                new Quaternion(0.0008062033f, 0.9238791f, -0.0003340352f, -0.3826835f),
                new float4x4(-0.7071054f, 0.001234009f, -0.707107f, 0f, 0.001745328f, 0.9999985f, -1.748558E-07f, 0f, 0.7071059f, -0.001234258f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748558E-07f, 3.926991f, 0.001745329f)
            };
            // 218. - X: 585, Y: 585, Z: 0.1
            yield return new object[]
            {
                "X: 585, Y: 585, Z: 0.1",
                new float3(10.21018f, 10.21018f, 0.001745329f),
                new Quaternion(-0.3538619f, -0.3532448f, -0.8534251f, 0.1471915f),
                new float4x4(-0.7062328f, 0.5012338f, 0.5000001f, 0f, -0.001234129f, -0.7071055f, 0.7071071f, 0f, 0.7079787f, 0.4987651f, 0.4999996f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 0.7853986f, 3.143338f)
            };
            // 219. - X: -90, Y: 585, Z: 0.1
            yield return new object[]
            {
                "X: -90, Y: 585, Z: 0.1",
                new float3(-1.570796f, 10.21018f, 0.001745329f),
                new Quaternion(-0.2711682f, -0.653045f, -0.653045f, 0.2711682f),
                new float4x4(-0.7058711f, 0.70834f, 0f, 0f, 0f, 8.940697E-08f, 0.9999999f, 0f, 0.70834f, 0.7058712f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.928736f, 0f)
            };
            // 220. - X: -540, Y: 585, Z: 0.1
            yield return new object[]
            {
                "X: -540, Y: 585, Z: 0.1",
                new float3(-9.424778f, 10.21018f, 0.001745329f),
                new Quaternion(0.3826835f, -0.0003339654f, 0.9238791f, -0.0008062323f),
                new float4x4(-0.7071054f, 0.001234116f, 0.707107f, 0f, -0.001745328f, -0.9999985f, -2.386514E-08f, 0f, 0.7071059f, -0.001234151f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.386514E-08f, 0.7853985f, 3.143338f)
            };
            // 221. - X: 0, Y: -90, Z: 0.1
            yield return new object[]
            {
                "X: 0, Y: -90, Z: 0.1",
                new float3(0f, -1.570796f, 0.001745329f),
                new Quaternion(-0.000617067f, -0.7071065f, 0.000617067f, 0.7071065f),
                new float4x4(-4.62876E-08f, 0f, -1f, 0f, 0.001745328f, 0.9999985f, 0f, 0f, 0.9999985f, -0.001745328f, -4.62876E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 0.001745329f)
            };
            // 222. - X: 0.1, Y: -90, Z: 0.1
            yield return new object[]
            {
                "X: 0.1, Y: -90, Z: 0.1",
                new float3(0.001745329f, -1.570796f, 0.001745329f),
                new Quaternion(0f, -0.7071068f, 0.001234133f, 0.7071058f),
                new float4x4(-3.16538E-06f, -0.001745326f, -0.9999986f, 0f, 0.001745326f, 0.999997f, -0.001745328f, 0f, 0.9999986f, -0.001745328f, -1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 4.712389f, 0.001745329f)
            };
            // 223. - X: 1, Y: -90, Z: 0.1
            yield return new object[]
            {
                "X: 1, Y: -90, Z: 0.1",
                new float3(0.01745329f, -1.570796f, 0.001745329f),
                new Quaternion(0.005553546f, -0.707085f, 0.006787634f, 0.7070742f),
                new float4x4(-3.039352E-05f, -0.01745238f, -0.9998477f, 0f, 0.001745064f, 0.9998462f, -0.01745241f, 0f, 0.9999985f, -0.001745328f, 6.665505E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 4.712389f, 0.001745331f)
            };
            // 224. - X: 45, Y: -90, Z: 0.1
            yield return new object[]
            {
                "X: 45, Y: -90, Z: 0.1",
                new float3(0.7853982f, -1.570796f, 0.001745329f),
                new Quaternion(0.2700279f, -0.6535174f, 0.2711681f, 0.6530451f),
                new float4x4(-0.001234129f, -0.7071058f, -0.7071067f, 0f, 0.001234084f, 0.7071056f, -0.7071068f, 0f, 0.9999985f, -0.001745313f, -1.490116E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853984f, 4.712389f, 0.001745302f)
            };
            // 225. - X: 90, Y: -90, Z: 0.1
            yield return new object[]
            {
                "X: 90, Y: -90, Z: 0.1",
                new float3(1.570796f, -1.570796f, 0.001745329f),
                new Quaternion(0.4995635f, -0.5004361f, 0.5004361f, 0.4995635f),
                new float4x4(-0.001745224f, -0.9999984f, 0f, 0f, 0f, 8.940697E-08f, -0.9999999f, 0f, 0.9999984f, -0.001745313f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 4.710643f, 0f)
            };
            // 226. - X: 180, Y: -90, Z: 0.1
            yield return new object[]
            {
                "X: 180, Y: -90, Z: 0.1",
                new float3(3.141593f, -1.570796f, 0.001745329f),
                new Quaternion(0.7071065f, -0.0006170361f, 0.7071065f, -0.0006170979f),
                new float4x4(-4.621131E-08f, 8.73697E-08f, 1f, 0f, -0.001745328f, -0.9999986f, 8.73697E-08f, 0f, 0.9999985f, -0.001745328f, -4.621131E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.73697E-08f, 1.570796f, 3.143338f)
            };
            // 227. - X: 270, Y: -90, Z: 0.1
            yield return new object[]
            {
                "X: 270, Y: -90, Z: 0.1",
                new float3(4.712389f, -1.570796f, 0.001745329f),
                new Quaternion(0.5004361f, 0.4995635f, 0.4995635f, -0.5004361f),
                new float4x4(0.001745403f, 0.9999984f, 0f, 0f, 0f, 8.940697E-08f, 0.9999999f, 0f, 0.9999984f, -0.001745313f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.714134f, 0f)
            };
            // 228. - X: 360, Y: -90, Z: 0.1
            yield return new object[]
            {
                "X: 360, Y: -90, Z: 0.1",
                new float3(6.283185f, -1.570796f, 0.001745329f),
                new Quaternion(0.0006170052f, 0.7071065f, -0.0006171288f, -0.7071065f),
                new float4x4(-4.644016E-08f, -1.747976E-07f, -1f, 0f, 0.001745328f, 0.9999985f, -1.747976E-07f, 0f, 0.9999985f, -0.001745328f, -4.613503E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.747976E-07f, 4.712389f, 0.001745329f)
            };
            // 229. - X: 585, Y: -90, Z: 0.1
            yield return new object[]
            {
                "X: 585, Y: -90, Z: 0.1",
                new float3(10.21018f, -1.570796f, 0.001745329f),
                new Quaternion(-0.6535173f, -0.270028f, -0.653045f, 0.2711682f),
                new float4x4(0.001234218f, 0.707106f, 0.7071064f, 0f, -0.001234084f, -0.7071053f, 0.7071071f, 0f, 0.9999984f, -0.001745313f, 4.470348E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.570796f, 3.143338f)
            };
            // 230. - X: -90, Y: -90, Z: 0.1
            yield return new object[]
            {
                "X: -90, Y: -90, Z: 0.1",
                new float3(-1.570796f, -1.570796f, 0.001745329f),
                new Quaternion(-0.5004361f, -0.4995635f, -0.4995635f, 0.5004361f),
                new float4x4(0.001745403f, 0.9999984f, 0f, 0f, 0f, 8.940697E-08f, 0.9999999f, 0f, 0.9999984f, -0.001745313f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.714134f, 0f)
            };
            // 231. - X: -540, Y: -90, Z: 0.1
            yield return new object[]
            {
                "X: -540, Y: -90, Z: 0.1",
                new float3(-9.424778f, -1.570796f, 0.001745329f),
                new Quaternion(0.7071065f, -0.0006170754f, 0.7071065f, -0.0006170585f),
                new float4x4(-4.63084E-08f, -2.392335E-08f, 1f, 0f, -0.001745328f, -0.9999986f, -2.392335E-08f, 0f, 0.9999985f, -0.001745328f, -4.63084E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.392335E-08f, 1.570796f, 3.143338f)
            };
            // 232. - X: 0, Y: -540, Z: 0.1
            yield return new object[]
            {
                "X: 0, Y: -540, Z: 0.1",
                new float3(0f, -9.424778f, 0.001745329f),
                new Quaternion(0.0008726645f, 0.9999996f, 1.040642E-11f, 1.192488E-08f),
                new float4x4(-0.9999986f, 0.001745328f, 2.384976E-08f, 0f, 0.001745328f, 0.9999985f, 0f, 0f, -2.384973E-08f, 4.162566E-11f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.141593f, 0.001745329f)
            };
            // 233. - X: 0.1, Y: -540, Z: 0.1
            yield return new object[]
            {
                "X: 0.1, Y: -540, Z: 0.1",
                new float3(0.001745329f, -9.424778f, 0.001745329f),
                new Quaternion(0.0008726642f, 0.9999993f, -0.0008726642f, 7.734682E-07f),
                new float4x4(-0.9999987f, 0.001745329f, 2.384968E-08f, 0f, 0.001745326f, 0.999997f, -0.001745329f, 0f, -3.070021E-06f, -0.001745326f, -0.9999987f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.141593f, 0.001745329f)
            };
            // 234. - X: 1, Y: -540, Z: 0.1
            yield return new object[]
            {
                "X: 1, Y: -540, Z: 0.1",
                new float3(0.01745329f, -9.424778f, 0.001745329f),
                new Quaternion(0.0008726314f, 0.9999616f, -0.008726533f, 7.627262E-06f),
                new float4x4(-0.9999985f, 0.001745329f, 2.384513E-08f, 0f, 0.001745063f, 0.9998462f, -0.01745241f, 0f, -3.048403E-05f, -0.01745238f, -0.9998478f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.141593f, 0.001745329f)
            };
            // 235. - X: 45, Y: -540, Z: 0.1
            yield return new object[]
            {
                "X: 45, Y: -540, Z: 0.1",
                new float3(0.7853982f, -9.424778f, 0.001745329f),
                new Quaternion(0.0008062414f, 0.9238791f, -0.3826833f, 0.0003339653f),
                new float4x4(-0.9999985f, 0.001745345f, 1.688022E-08f, 0f, 0.001234133f, 0.7071057f, -0.7071068f, 0f, -0.001234157f, -0.7071057f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 3.141593f, 0.001745329f)
            };
            // 236. - X: 90, Y: -540, Z: 0.1
            yield return new object[]
            {
                "X: 90, Y: -540, Z: 0.1",
                new float3(1.570796f, -9.424778f, 0.001745329f),
                new Quaternion(0.0006170754f, 0.7071065f, -0.7071065f, 0.0006170754f),
                new float4x4(-0.9999986f, 0.001745352f, 0f, 0f, 0f, -4.63084E-08f, -1f, 0f, -0.001745352f, -0.9999985f, -4.63084E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 3.139847f, 0f)
            };
            // 237. - X: 180, Y: -540, Z: 0.1
            yield return new object[]
            {
                "X: 180, Y: -540, Z: 0.1",
                new float3(3.141593f, -9.424778f, 0.001745329f),
                new Quaternion(1.188673E-08f, -4.372178E-08f, -0.9999996f, 0.0008726645f),
                new float4x4(-0.9999986f, 0.001745328f, -2.384976E-08f, 0f, -0.001745328f, -0.9999986f, 8.742278E-08f, 0f, -2.369714E-08f, 8.746428E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, -2.384976E-08f, 3.143338f)
            };
            // 238. - X: 270, Y: -540, Z: 0.1
            yield return new object[]
            {
                "X: 270, Y: -540, Z: 0.1",
                new float3(4.712389f, -9.424778f, 0.001745329f),
                new Quaternion(-0.0006170585f, -0.7071065f, -0.7071065f, 0.0006170585f),
                new float4x4(-0.9999986f, 0.001745304f, 0f, 0f, 0f, -4.626673E-08f, 1f, 0f, 0.001745304f, 0.9999985f, -4.626673E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.143338f, 0f)
            };
            // 239. - X: 360, Y: -540, Z: 0.1
            yield return new object[]
            {
                "X: 360, Y: -540, Z: 0.1",
                new float3(6.283185f, -9.424778f, 0.001745329f),
                new Quaternion(-0.0008726645f, -0.9999996f, 8.741234E-08f, -1.200117E-08f),
                new float4x4(-0.9999986f, 0.001745328f, 2.384976E-08f, 0f, 0.001745328f, 0.9999985f, -1.748456E-07f, 0f, -2.415489E-08f, -1.748037E-07f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 3.141593f, 0.001745329f)
            };
            // 240. - X: 585, Y: -540, Z: 0.1
            yield return new object[]
            {
                "X: 585, Y: -540, Z: 0.1",
                new float3(10.21018f, -9.424778f, 0.001745329f),
                new Quaternion(0.0003339434f, 0.3826835f, 0.9238791f, -0.0008062323f),
                new float4x4(-0.9999985f, 0.001745312f, -1.688022E-08f, 0f, -0.001234133f, -0.7071054f, 0.707107f, 0f, 0.00123411f, 0.7071059f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, -2.395456E-08f, 3.143338f)
            };
            // 241. - X: -90, Y: -540, Z: 0.1
            yield return new object[]
            {
                "X: -90, Y: -540, Z: 0.1",
                new float3(-1.570796f, -9.424778f, 0.001745329f),
                new Quaternion(0.0006170585f, 0.7071065f, 0.7071065f, -0.0006170585f),
                new float4x4(-0.9999986f, 0.001745304f, 0f, 0f, 0f, -4.626673E-08f, 1f, 0f, 0.001745304f, 0.9999985f, -4.626673E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.143338f, 0f)
            };
            // 242. - X: -540, Y: -540, Z: 0.1
            yield return new object[]
            {
                "X: -540, Y: -540, Z: 0.1",
                new float3(-9.424778f, -9.424778f, 0.001745329f),
                new Quaternion(1.193528E-08f, 1.191447E-08f, -0.9999996f, 0.0008726645f),
                new float4x4(-0.9999986f, 0.001745328f, -2.384976E-08f, 0f, -0.001745328f, -0.9999986f, -2.384976E-08f, 0f, -2.389135E-08f, -2.38081E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, -2.384976E-08f, 3.143338f)
            };
            // 243. - X: 0, Y: 0, Z: 1
            yield return new object[]
            {
                "X: 0, Y: 0, Z: 1",
                new float3(0f, 0f, 0.01745329f),
                new Quaternion(0f, 0f, 0.008726535f, 0.9999619f),
                new float4x4(0.9998477f, -0.01745241f, 0f, 0f, 0.01745241f, 0.9998477f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0f, 0.01745329f)
            };
            // 244. - X: 0.1, Y: 0, Z: 1
            yield return new object[]
            {
                "X: 0.1, Y: 0, Z: 1",
                new float3(0.001745329f, 0f, 0.01745329f),
                new Quaternion(0.0008726313f, -7.615338E-06f, 0.008726533f, 0.9999616f),
                new float4x4(0.9998477f, -0.01745241f, 0f, 0f, 0.01745238f, 0.9998462f, -0.001745328f, 0f, 3.046018E-05f, 0.001745063f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0f, 0.01745329f)
            };
            // 245. - X: 1, Y: 0, Z: 1
            yield return new object[]
            {
                "X: 1, Y: 0, Z: 1",
                new float3(0.01745329f, 0f, 0.01745329f),
                new Quaternion(0.008726203f, -7.615242E-05f, 0.008726203f, 0.9999238f),
                new float4x4(0.9998477f, -0.01745241f, -1.455192E-11f, 0f, 0.01744975f, 0.9996954f, -0.01745241f, 0f, 0.0003045865f, 0.01744975f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.0174533f, 1.455413E-11f, 0.01745329f)
            };
            // 246. - X: 45, Y: 0, Z: 1
            yield return new object[]
            {
                "X: 45, Y: 0, Z: 1",
                new float3(0.7853982f, 0f, 0.01745329f),
                new Quaternion(0.3826689f, -0.003339501f, 0.008062267f, 0.9238443f),
                new float4x4(0.9998477f, -0.0174524f, -4.656613E-10f, 0f, 0.01234071f, 0.7069991f, -0.7071068f, 0f, 0.01234072f, 0.7069991f, 0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, -6.585445E-10f, 0.01745329f)
            };
            // 247. - X: 90, Y: 0, Z: 1
            yield return new object[]
            {
                "X: 90, Y: 0, Z: 1",
                new float3(1.570796f, 0f, 0.01745329f),
                new Quaternion(0.7070798f, -0.006170592f, 0.006170592f, 0.7070798f),
                new float4x4(0.9998477f, -0.01745241f, 0f, 0f, 0f, 8.192001E-08f, -0.9999999f, 0f, 0.01745241f, 0.9998476f, 8.192001E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 6.265732f, 0f)
            };
            // 248. - X: 180, Y: 0, Z: 1
            yield return new object[]
            {
                "X: 180, Y: 0, Z: 1",
                new float3(3.141593f, 0f, 0.01745329f),
                new Quaternion(0.9999619f, -0.008726535f, -3.81449E-10f, -4.370972E-08f),
                new float4x4(0.9998477f, -0.01745241f, -5.551115E-17f, 0f, -0.01745241f, -0.9998477f, 8.742277E-08f, 0f, -1.525738E-09f, -8.740945E-08f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742277E-08f, 3.141593f, 3.159046f)
            };
            // 249. - X: 270, Y: 0, Z: 1
            yield return new object[]
            {
                "X: 270, Y: 0, Z: 1",
                new float3(4.712389f, 0f, 0.01745329f),
                new Quaternion(0.7070798f, -0.006170592f, -0.006170592f, -0.7070798f),
                new float4x4(0.9998477f, -0.01745241f, 0f, 0f, 0f, 8.192001E-08f, 0.9999999f, 0f, -0.01745241f, -0.9998476f, 8.192001E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.01745329f, 0f)
            };
            // 250. - X: 360, Y: 0, Z: 1
            yield return new object[]
            {
                "X: 360, Y: 0, Z: 1",
                new float3(6.283185f, 0f, 0.01745329f),
                new Quaternion(-8.741944E-08f, 7.62898E-10f, -0.008726535f, -0.9999619f),
                new float4x4(0.9998477f, -0.01745241f, -1.110223E-16f, 0f, 0.01745241f, 0.9998477f, -1.748455E-07f, 0f, 3.051476E-09f, 1.748189E-07f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748455E-07f, -1.110223E-16f, 0.01745329f)
            };
            // 251. - X: 585, Y: 0, Z: 1
            yield return new object[]
            {
                "X: 585, Y: 0, Z: 1",
                new float3(10.21018f, 0f, 0.01745329f),
                new Quaternion(-0.9238443f, 0.008062267f, 0.003339502f, 0.382669f),
                new float4x4(0.9998477f, -0.01745241f, -4.656613E-10f, 0f, -0.01234071f, -0.7069988f, 0.707107f, 0f, -0.01234072f, -0.7069994f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.141593f, 3.159046f)
            };
            // 252. - X: -90, Y: 0, Z: 1
            yield return new object[]
            {
                "X: -90, Y: 0, Z: 1",
                new float3(-1.570796f, 0f, 0.01745329f),
                new Quaternion(-0.7070798f, 0.006170592f, 0.006170592f, 0.7070798f),
                new float4x4(0.9998477f, -0.01745241f, 0f, 0f, 0f, 8.192001E-08f, 0.9999999f, 0f, -0.01745241f, -0.9998476f, 8.192001E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.01745329f, 0f)
            };
            // 253. - X: -540, Y: 0, Z: 1
            yield return new object[]
            {
                "X: -540, Y: 0, Z: 1",
                new float3(-9.424778f, 0f, 0.01745329f),
                new Quaternion(0.9999619f, -0.008726535f, 1.040629E-10f, 1.192443E-08f),
                new float4x4(0.9998477f, -0.01745241f, -1.387779E-17f, 0f, -0.01745241f, -0.9998477f, -2.384976E-08f, 0f, 4.162357E-10f, 2.384613E-08f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.141593f, 3.159046f)
            };
            // 254. - X: 0, Y: 0.1, Z: 1
            yield return new object[]
            {
                "X: 0, Y: 0.1, Z: 1",
                new float3(0f, 0.001745329f, 0.01745329f),
                new Quaternion(7.615338E-06f, 0.0008726313f, 0.008726533f, 0.9999616f),
                new float4x4(0.9998462f, -0.01745238f, 0.001745328f, 0f, 0.01745241f, 0.9998477f, 0f, 0f, -0.001745063f, 3.046018E-05f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0.001745329f, 0.01745329f)
            };
            // 255. - X: 0.1, Y: 0.1, Z: 1
            yield return new object[]
            {
                "X: 0.1, Y: 0.1, Z: 1",
                new float3(0.001745329f, 0.001745329f, 0.01745329f),
                new Quaternion(0.0008802463f, 0.0008650157f, 0.008725767f, 0.9999612f),
                new float4x4(0.9998462f, -0.01744933f, 0.001745326f, 0f, 0.01745238f, 0.9998462f, -0.001745328f, 0f, -0.001714602f, 0.00177552f, 0.999997f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0.001745329f, 0.01745329f)
            };
            // 256. - X: 1, Y: 0.1, Z: 1
            yield return new object[]
            {
                "X: 1, Y: 0.1, Z: 1",
                new float3(0.01745329f, 0.001745329f, 0.01745329f),
                new Quaternion(0.008733816f, 0.0007964456f, 0.008718585f, 0.9999235f),
                new float4x4(0.9998467f, -0.01742192f, 0.001745063f, 0f, 0.01744975f, 0.9996954f, -0.01745241f, 0f, -0.001440477f, 0.01748018f, 0.9998462f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0.001745329f, 0.01745329f)
            };
            // 257. - X: 45, Y: 0.1, Z: 1
            yield return new object[]
            {
                "X: 45, Y: 0.1, Z: 1",
                new float3(0.7853982f, 0.001745329f, 0.01745329f),
                new Quaternion(0.3826758f, -0.002533293f, 0.007728322f, 0.9238469f),
                new float4x4(0.9998677f, -0.01621843f, 0.001234133f, 0f, 0.01234071f, 0.7069991f, -0.7071068f, 0f, 0.01059563f, 0.7070285f, 0.7071057f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 0.001745329f, 0.01745329f)
            };
            // 258. - X: 90, Y: 0.1, Z: 1
            yield return new object[]
            {
                "X: 90, Y: 0.1, Z: 1",
                new float3(1.570796f, 0.001745329f, 0.01745329f),
                new Quaternion(0.707085f, -0.005553546f, 0.005553546f, 0.707085f),
                new float4x4(0.9998766f, -0.01570732f, 0f, 0f, 0f, 6.665505E-08f, -0.9999999f, 0f, 0.01570732f, 0.9998766f, 6.665505E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 6.267478f, 0f)
            };
            // 259. - X: 180, Y: 0.1, Z: 1
            yield return new object[]
            {
                "X: 180, Y: 0.1, Z: 1",
                new float3(3.141593f, 0.001745329f, 0.01745329f),
                new Quaternion(0.9999616f, -0.008726533f, -0.0008726317f, 7.571628E-06f),
                new float4x4(0.9998462f, -0.01745238f, -0.001745328f, 0f, -0.01745241f, -0.9998478f, 8.742336E-08f, 0f, -0.001745064f, 3.037277E-05f, -0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742337E-08f, 3.143338f, 3.159046f)
            };
            // 260. - X: 270, Y: 0.1, Z: 1
            yield return new object[]
            {
                "X: 270, Y: 0.1, Z: 1",
                new float3(4.712389f, 0.001745329f, 0.01745329f),
                new Quaternion(0.7070742f, -0.006787634f, -0.006787634f, -0.7070742f),
                new float4x4(0.9998157f, -0.01919744f, 0f, 0f, 0f, -5.475886E-08f, 1f, 0f, -0.01919744f, -0.9998158f, -5.475886E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.01919862f, 0f)
            };
            // 261. - X: 360, Y: 0.1, Z: 1
            yield return new object[]
            {
                "X: 360, Y: 0.1, Z: 1",
                new float3(6.283185f, 0.001745329f, 0.01745329f),
                new Quaternion(-7.702757E-06f, -0.0008726305f, -0.008726533f, -0.9999616f),
                new float4x4(0.9998462f, -0.01745238f, 0.001745328f, 0f, 0.01745241f, 0.9998477f, -1.74844E-07f, 0f, -0.00174506f, 3.0635E-05f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.74844E-07f, 0.001745329f, 0.01745329f)
            };
            // 262. - X: 585, Y: 0.1, Z: 1
            yield return new object[]
            {
                "X: 585, Y: 0.1, Z: 1",
                new float3(10.21018f, 0.001745329f, 0.01745329f),
                new Quaternion(-0.923841f, 0.008396205f, 0.004145707f, 0.3826618f),
                new float4x4(0.9998246f, -0.01868632f, -0.001234132f, 0f, -0.01234071f, -0.7069988f, 0.707107f, 0f, -0.01408576f, -0.7069678f, -0.7071053f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.143338f, 3.159046f)
            };
            // 263. - X: -90, Y: 0.1, Z: 1
            yield return new object[]
            {
                "X: -90, Y: 0.1, Z: 1",
                new float3(-1.570796f, 0.001745329f, 0.01745329f),
                new Quaternion(-0.7070742f, 0.006787634f, 0.006787634f, 0.7070742f),
                new float4x4(0.9998157f, -0.01919744f, 0f, 0f, 0f, -5.475886E-08f, 1f, 0f, -0.01919744f, -0.9998158f, -5.475886E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.01919862f, 0f)
            };
            // 264. - X: -540, Y: 0.1, Z: 1
            yield return new object[]
            {
                "X: -540, Y: 0.1, Z: 1",
                new float3(-9.424778f, 0.001745329f, 0.01745329f),
                new Quaternion(0.9999616f, -0.008726533f, -0.0008726311f, 7.627262E-06f),
                new float4x4(0.9998462f, -0.01745238f, -0.001745328f, 0f, -0.01745241f, -0.9998478f, -2.384968E-08f, 0f, -0.001745062f, 3.048403E-05f, -0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384968E-08f, 3.143338f, 3.159046f)
            };
            // 265. - X: 0, Y: 1, Z: 1
            yield return new object[]
            {
                "X: 0, Y: 1, Z: 1",
                new float3(0f, 0.01745329f, 0.01745329f),
                new Quaternion(7.615242E-05f, 0.008726203f, 0.008726203f, 0.9999238f),
                new float4x4(0.9996954f, -0.01744975f, 0.01745241f, 0f, 0.01745241f, 0.9998477f, -1.455192E-11f, 0f, -0.01744975f, 0.0003045865f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(-1.455192E-11f, 0.01745329f, 0.01745329f)
            };
            // 266. - X: 0.1, Y: 1, Z: 1
            yield return new object[]
            {
                "X: 0.1, Y: 1, Z: 1",
                new float3(0.001745329f, 0.01745329f, 0.01745329f),
                new Quaternion(0.0009487504f, 0.008718585f, 0.008718585f, 0.9999235f),
                new float4x4(0.999696f, -0.01741929f, 0.01745238f, 0f, 0.01745238f, 0.9998462f, -0.001745328f, 0f, -0.01741929f, 0.002049383f, 0.9998462f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0.01745329f, 0.01745329f)
            };
            // 267. - X: 1, Y: 1, Z: 1
            yield return new object[]
            {
                "X: 1, Y: 1, Z: 1",
                new float3(0.01745329f, 0.01745329f, 0.01745329f),
                new Quaternion(0.00880202f, 0.008649721f, 0.008649721f, 0.9998864f),
                new float4x4(0.9997007f, -0.01714521f, 0.01744975f, 0f, 0.01744975f, 0.9996954f, -0.0174524f, 0f, -0.01714521f, 0.01775168f, 0.9996954f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0.01745329f, 0.01745329f)
            };
            // 268. - X: 45, Y: 1, Z: 1
            yield return new object[]
            {
                "X: 45, Y: 1, Z: 1",
                new float3(0.7853982f, 0.01745329f, 0.01745329f),
                new Quaternion(0.3827247f, 0.004722586f, 0.004722587f, 0.9238383f),
                new float4x4(0.9999108f, -0.005110913f, 0.01234071f, 0f, 0.01234071f, 0.7069991f, -0.7071068f, 0f, -0.005110911f, 0.707196f, 0.7069991f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 0.01745329f, 0.01745329f)
            };
            // 269. - X: 90, Y: 1, Z: 1
            yield return new object[]
            {
                "X: 90, Y: 1, Z: 1",
                new float3(1.570796f, 0.01745329f, 0.01745329f),
                new Quaternion(0.7071067f, 0f, 0f, 0.7071067f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 1.788139E-07f, -0.9999998f, 0f, 0f, 0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570451f, 0f, 0f)
            };
            // 270. - X: 180, Y: 1, Z: 1
            yield return new object[]
            {
                "X: 180, Y: 1, Z: 1",
                new float3(3.141593f, 0.01745329f, 0.01745329f),
                new Quaternion(0.9999238f, -0.008726203f, -0.008726203f, 7.610871E-05f),
                new float4x4(0.9996954f, -0.01744975f, -0.0174524f, 0f, -0.0174524f, -0.9998476f, 8.73988E-08f, 0f, -0.01744975f, 0.0003044991f, -0.9998476f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.739881E-08f, 3.159046f, 3.159046f)
            };
            // 271. - X: 270, Y: 1, Z: 1
            yield return new object[]
            {
                "X: 270, Y: 1, Z: 1",
                new float3(4.712389f, 0.01745329f, 0.01745329f),
                new Quaternion(0.7069991f, -0.01234071f, -0.01234071f, -0.7069991f),
                new float4x4(0.9993908f, -0.03489949f, 0f, 0f, 0f, 5.288166E-08f, 0.9999999f, 0f, -0.03489949f, -0.9993908f, 5.288166E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 0.03490658f, 0f)
            };
            // 272. - X: 360, Y: 1, Z: 1
            yield return new object[]
            {
                "X: 360, Y: 1, Z: 1",
                new float3(6.283185f, 0.01745329f, 0.01745329f),
                new Quaternion(-7.623983E-05f, -0.008726202f, -0.008726202f, -0.9999238f),
                new float4x4(0.9996954f, -0.01744974f, 0.0174524f, 0f, 0.0174524f, 0.9998477f, -1.748558E-07f, 0f, -0.01744974f, 0.0003047613f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748413E-07f, 0.01745329f, 0.01745329f)
            };
            // 273. - X: 585, Y: 1, Z: 1
            yield return new object[]
            {
                "X: 585, Y: 1, Z: 1",
                new float3(10.21018f, 0.01745329f, 0.01745329f),
                new Quaternion(-0.92378f, 0.01140133f, 0.01140133f, 0.3825841f),
                new float4x4(0.99948f, -0.02978859f, -0.01234071f, 0f, -0.01234071f, -0.7069988f, 0.7071071f, 0f, -0.02978859f, -0.7065871f, -0.7069988f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.159046f, 3.159046f)
            };
            // 274. - X: -90, Y: 1, Z: 1
            yield return new object[]
            {
                "X: -90, Y: 1, Z: 1",
                new float3(-1.570796f, 0.01745329f, 0.01745329f),
                new Quaternion(-0.7069991f, 0.01234071f, 0.01234071f, 0.7069991f),
                new float4x4(0.9993908f, -0.03489949f, 0f, 0f, 0f, 5.288166E-08f, 0.9999999f, 0f, -0.03489949f, -0.9993908f, 5.288166E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 0.03490658f, 0f)
            };
            // 275. - X: -540, Y: 1, Z: 1
            yield return new object[]
            {
                "X: -540, Y: 1, Z: 1",
                new float3(-9.424778f, 0.01745329f, 0.01745329f),
                new Quaternion(0.9999238f, -0.008726203f, -0.008726203f, 7.616435E-05f),
                new float4x4(0.9996954f, -0.01744975f, -0.01745241f, 0f, -0.01745241f, -0.9998476f, -2.386514E-08f, 0f, -0.01744975f, 0.0003046103f, -0.9998476f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.386514E-08f, 3.159046f, 3.159046f)
            };
            // 276. - X: 0, Y: 45, Z: 1
            yield return new object[]
            {
                "X: 0, Y: 45, Z: 1",
                new float3(0f, 0.7853982f, 0.01745329f),
                new Quaternion(0.003339501f, 0.3826689f, 0.008062267f, 0.9238443f),
                new float4x4(0.7069991f, -0.01234071f, 0.7071068f, 0f, 0.0174524f, 0.9998477f, -4.656613E-10f, 0f, -0.7069991f, 0.01234072f, 0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.656613E-10f, 0.7853982f, 0.01745329f)
            };
            // 277. - X: 0.1, Y: 45, Z: 1
            yield return new object[]
            {
                "X: 0.1, Y: 45, Z: 1",
                new float3(0.001745329f, 0.7853982f, 0.01745329f),
                new Quaternion(0.004145706f, 0.3826617f, 0.007728322f, 0.9238469f),
                new float4x4(0.7070206f, -0.01110677f, 0.7071058f, 0f, 0.01745238f, 0.9998462f, -0.001745329f, 0f, -0.7069776f, 0.01357466f, 0.7071057f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.00174533f, 0.7853982f, 0.01745329f)
            };
            // 278. - X: 1, Y: 45, Z: 1
            yield return new object[]
            {
                "X: 1, Y: 45, Z: 1",
                new float3(0.01745329f, 0.7853982f, 0.01745329f),
                new Quaternion(0.01140133f, 0.3825839f, 0.004722587f, 0.9238383f),
                new float4x4(0.7072145f, -1.879409E-06f, 0.7069991f, 0f, 0.01744975f, 0.9996954f, -0.0174524f, 0f, -0.7067837f, 0.02467955f, 0.7069991f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0.7853982f, 0.01745329f)
            };
            // 279. - X: 45, Y: 45, Z: 1
            yield return new object[]
            {
                "X: 45, Y: 45, Z: 1",
                new float3(0.7853982f, 0.7853982f, 0.01745329f),
                new Quaternion(0.3566252f, 0.3504547f, -0.1389925f, 0.8547989f),
                new float4x4(0.7157252f, 0.4875832f, 0.5f, 0f, 0.01234071f, 0.7069991f, -0.7071068f, 0f, -0.6982729f, 0.5122645f, 0.4999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 0.7853982f, 0.01745328f)
            };
            // 280. - X: 90, Y: 45, Z: 1
            yield return new object[]
            {
                "X: 90, Y: 45, Z: 1",
                new float3(1.570796f, 0.7853982f, 0.01745329f),
                new Quaternion(0.655618f, 0.2648869f, -0.2648869f, 0.655618f),
                new float4x4(0.7193398f, 0.6946583f, 0f, 0f, 0f, 1.043081E-07f, -0.9999999f, 0f, -0.6946583f, 0.7193397f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 0.7679449f, 0f)
            };
            // 281. - X: 180, Y: 45, Z: 1
            yield return new object[]
            {
                "X: 180, Y: 45, Z: 1",
                new float3(3.141593f, 0.7853982f, 0.01745329f),
                new Quaternion(0.9238443f, -0.008062284f, -0.3826689f, 0.00333946f),
                new float4x4(0.7069991f, -0.01234077f, -0.7071068f, 0f, -0.01745241f, -0.9998477f, 8.707866E-08f, 0f, -0.7069991f, 0.01234065f, -0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.707866E-08f, 3.926991f, 3.159046f)
            };
            // 282. - X: 270, Y: 45, Z: 1
            yield return new object[]
            {
                "X: 270, Y: 45, Z: 1",
                new float3(4.712389f, 0.7853982f, 0.01745329f),
                new Quaternion(0.6508952f, -0.2762886f, -0.2762886f, -0.6508952f),
                new float4x4(0.6946584f, -0.7193398f, 0f, 0f, 0f, -1.490116E-08f, 1f, 0f, -0.7193398f, -0.6946584f, -1.490116E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.8028514f, 0f)
            };
            // 283. - X: 360, Y: 45, Z: 1
            yield return new object[]
            {
                "X: 360, Y: 45, Z: 1",
                new float3(6.283185f, 0.7853982f, 0.01745329f),
                new Quaternion(-0.003339581f, -0.3826689f, -0.008062233f, -0.9238443f),
                new float4x4(0.7069991f, -0.01234059f, 0.7071068f, 0f, 0.0174524f, 0.9998477f, -1.750886E-07f, 0f, -0.7069991f, 0.01234084f, 0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.750886E-07f, 0.7853982f, 0.01745329f)
            };
            // 284. - X: 585, Y: 45, Z: 1
            yield return new object[]
            {
                "X: 585, Y: 45, Z: 1",
                new float3(10.21018f, 0.7853982f, 0.01745329f),
                new Quaternion(-0.8522428f, 0.1538897f, 0.3566252f, 0.3504548f),
                new float4x4(0.6982729f, -0.5122648f, -0.4999998f, 0f, -0.01234074f, -0.7069987f, 0.707107f, 0f, -0.7157253f, -0.4875832f, -0.4999997f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.926991f, 3.159046f)
            };
            // 285. - X: -90, Y: 45, Z: 1
            yield return new object[]
            {
                "X: -90, Y: 45, Z: 1",
                new float3(-1.570796f, 0.7853982f, 0.01745329f),
                new Quaternion(-0.6508952f, 0.2762886f, 0.2762886f, 0.6508952f),
                new float4x4(0.6946584f, -0.7193398f, 0f, 0f, 0f, -1.490116E-08f, 1f, 0f, -0.7193398f, -0.6946584f, -1.490116E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.8028514f, 0f)
            };
            // 286. - X: -540, Y: 45, Z: 1
            yield return new object[]
            {
                "X: -540, Y: 45, Z: 1",
                new float3(-9.424778f, 0.7853982f, 0.01745329f),
                new Quaternion(0.9238443f, -0.008062262f, -0.3826689f, 0.003339512f),
                new float4x4(0.7069991f, -0.0123407f, -0.7071068f, 0f, -0.0174524f, -0.9998477f, -2.374873E-08f, 0f, -0.7069991f, 0.01234073f, -0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.374873E-08f, 3.926991f, 3.159046f)
            };
            // 287. - X: 0, Y: 90, Z: 1
            yield return new object[]
            {
                "X: 0, Y: 90, Z: 1",
                new float3(0f, 1.570796f, 0.01745329f),
                new Quaternion(0.006170592f, 0.7070798f, 0.006170592f, 0.7070798f),
                new float4x4(8.192001E-08f, 0f, 0.9999999f, 0f, 0.01745241f, 0.9998477f, 0f, 0f, -0.9998476f, 0.01745241f, 8.192001E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.570796f, 0.01745329f)
            };
            // 288. - X: 0.1, Y: 90, Z: 1
            yield return new object[]
            {
                "X: 0.1, Y: 90, Z: 1",
                new float3(0.001745329f, 1.570796f, 0.01745329f),
                new Quaternion(0.006787634f, 0.7070742f, 0.005553546f, 0.707085f),
                new float4x4(3.040542E-05f, 0.001745064f, 0.9999985f, 0f, 0.01745238f, 0.9998462f, -0.001745328f, 0f, -0.9998477f, 0.01745241f, -5.475886E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 1.570796f, 0.01745329f)
            };
            // 289. - X: 1, Y: 90, Z: 1
            yield return new object[]
            {
                "X: 1, Y: 90, Z: 1",
                new float3(0.01745329f, 1.570796f, 0.01745329f),
                new Quaternion(0.01234071f, 0.7069991f, 0f, 0.7071067f),
                new float4x4(0.0003046393f, 0.01744975f, 0.9998476f, 0f, 0.01744975f, 0.9996954f, -0.0174524f, 0f, -0.9998476f, 0.0174524f, 5.288166E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 1.570796f, 0.01745329f)
            };
            // 290. - X: 45, Y: 90, Z: 1
            yield return new object[]
            {
                "X: 45, Y: 90, Z: 1",
                new float3(0.7853982f, 1.570796f, 0.01745329f),
                new Quaternion(0.2762886f, 0.6508952f, -0.2648869f, 0.655618f),
                new float4x4(0.01234071f, 0.7069991f, 0.7071067f, 0f, 0.01234072f, 0.7069991f, -0.7071067f, 0f, -0.9998477f, 0.01745239f, -1.490116E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853984f, 1.570796f, 0.01745335f)
            };
            // 291. - X: 90, Y: 90, Z: 1
            yield return new object[]
            {
                "X: 90, Y: 90, Z: 1",
                new float3(1.570796f, 1.570796f, 0.01745329f),
                new Quaternion(0.5043442f, 0.4956177f, -0.4956177f, 0.5043442f),
                new float4x4(0.01745254f, 0.9998475f, 0f, 0f, 0f, 2.086163E-07f, -0.9999998f, 0f, -0.9998475f, 0.01745233f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 1.553343f, 0f)
            };
            // 292. - X: 180, Y: 90, Z: 1
            yield return new object[]
            {
                "X: 180, Y: 90, Z: 1",
                new float3(3.141593f, 1.570796f, 0.01745329f),
                new Quaternion(0.7070798f, -0.006170623f, -0.7070798f, 0.006170562f),
                new float4x4(8.116331E-08f, -8.6613E-08f, -0.9999999f, 0f, -0.0174524f, -0.9998475f, 8.6613E-08f, 0f, -0.9998476f, 0.0174524f, 8.116331E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.6613E-08f, 4.712389f, 3.159046f)
            };
            // 293. - X: 270, Y: 90, Z: 1
            yield return new object[]
            {
                "X: 270, Y: 90, Z: 1",
                new float3(4.712389f, 1.570796f, 0.01745329f),
                new Quaternion(0.4956177f, -0.5043442f, -0.5043442f, -0.4956177f),
                new float4x4(-0.01745212f, -0.9998475f, 0f, 0f, 0f, 2.086163E-07f, 0.9999998f, 0f, -0.9998475f, 0.01745233f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.58825f, 0f)
            };
            // 294. - X: 360, Y: 90, Z: 1
            yield return new object[]
            {
                "X: 360, Y: 90, Z: 1",
                new float3(6.283185f, 1.570796f, 0.01745329f),
                new Quaternion(-0.006170654f, -0.7070798f, -0.00617053f, -0.7070798f),
                new float4x4(8.344796E-08f, 1.750886E-07f, 0.9999999f, 0f, 0.01745241f, 0.9998477f, -1.750886E-07f, 0f, -0.9998476f, 0.01745241f, 8.039206E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.750886E-07f, 1.570796f, 0.01745329f)
            };
            // 295. - X: 585, Y: 90, Z: 1
            yield return new object[]
            {
                "X: 585, Y: 90, Z: 1",
                new float3(10.21018f, 1.570796f, 0.01745329f),
                new Quaternion(-0.6508952f, 0.2762887f, 0.6556179f, 0.264887f),
                new float4x4(-0.01234058f, -0.7069993f, -0.7071065f, 0f, -0.01234072f, -0.7069987f, 0.707107f, 0f, -0.9998477f, 0.01745236f, -2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 4.712389f, 3.159046f)
            };
            // 296. - X: -90, Y: 90, Z: 1
            yield return new object[]
            {
                "X: -90, Y: 90, Z: 1",
                new float3(-1.570796f, 1.570796f, 0.01745329f),
                new Quaternion(-0.4956177f, 0.5043442f, 0.5043442f, 0.4956177f),
                new float4x4(-0.01745212f, -0.9998475f, 0f, 0f, 0f, 2.086163E-07f, 0.9999998f, 0f, -0.9998475f, 0.01745233f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.58825f, 0f)
            };
            // 297. - X: -540, Y: 90, Z: 1
            yield return new object[]
            {
                "X: -540, Y: 90, Z: 1",
                new float3(-9.424778f, 1.570796f, 0.01745329f),
                new Quaternion(0.7070798f, -0.006170584f, -0.7070798f, 0.006170601f),
                new float4x4(8.213101E-08f, 2.328306E-08f, -0.9999999f, 0f, -0.0174524f, -0.9998475f, -2.328306E-08f, 0f, -0.9998476f, 0.0174524f, 8.213101E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.328306E-08f, 4.712389f, 3.159046f)
            };
            // 298. - X: 0, Y: 180, Z: 1
            yield return new object[]
            {
                "X: 0, Y: 180, Z: 1",
                new float3(0f, 3.141593f, 0.01745329f),
                new Quaternion(0.008726535f, 0.9999619f, -3.81449E-10f, -4.370972E-08f),
                new float4x4(-0.9998477f, 0.01745241f, -8.742277E-08f, 0f, 0.01745241f, 0.9998477f, -5.551115E-17f, 0f, 8.740945E-08f, -1.525738E-09f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.551115E-17f, 3.141593f, 0.01745329f)
            };
            // 299. - X: 0.1, Y: 180, Z: 1
            yield return new object[]
            {
                "X: 0.1, Y: 180, Z: 1",
                new float3(0.001745329f, 3.141593f, 0.01745329f),
                new Quaternion(0.008726533f, 0.9999616f, -0.0008726317f, 7.571628E-06f),
                new float4x4(-0.9998478f, 0.01745241f, -8.742336E-08f, 0f, 0.01745238f, 0.9998462f, -0.001745328f, 0f, -3.037277E-05f, -0.001745064f, -0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.141593f, 0.01745329f)
            };
            // 300. - X: 1, Y: 180, Z: 1
            yield return new object[]
            {
                "X: 1, Y: 180, Z: 1",
                new float3(0.01745329f, 3.141593f, 0.01745329f),
                new Quaternion(0.008726203f, 0.9999238f, -0.008726203f, 7.610871E-05f),
                new float4x4(-0.9998476f, 0.0174524f, -8.73988E-08f, 0f, 0.01744975f, 0.9996954f, -0.0174524f, 0f, -0.0003044991f, -0.01744975f, -0.9998476f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.141593f, 0.01745329f)
            };
            // 301. - X: 45, Y: 180, Z: 1
            yield return new object[]
            {
                "X: 45, Y: 180, Z: 1",
                new float3(0.7853982f, 3.141593f, 0.01745329f),
                new Quaternion(0.00806225f, 0.9238443f, -0.3826689f, 0.00333946f),
                new float4x4(-0.9998477f, 0.01745234f, -6.146729E-08f, 0f, 0.01234071f, 0.7069991f, -0.7071068f, 0f, -0.01234063f, -0.7069991f, -0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 3.141593f, 0.01745329f)
            };
            // 302. - X: 90, Y: 180, Z: 1
            yield return new object[]
            {
                "X: 90, Y: 180, Z: 1",
                new float3(1.570796f, 3.141593f, 0.01745329f),
                new Quaternion(0.006170562f, 0.7070798f, -0.7070798f, 0.006170562f),
                new float4x4(-0.9998475f, 0.01745232f, 0f, 0f, 0f, 8.268398E-08f, -0.9999999f, 0f, -0.01745232f, -0.9998476f, 8.268398E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 3.12414f, 0f)
            };
            // 303. - X: 180, Y: 180, Z: 1
            yield return new object[]
            {
                "X: 180, Y: 180, Z: 1",
                new float3(3.141593f, 3.141593f, 0.01745329f),
                new Quaternion(-4.409117E-08f, -4.332827E-08f, -0.9999619f, 0.008726535f),
                new float4x4(-0.9998477f, 0.01745241f, 8.742277E-08f, 0f, -0.01745241f, -0.9998477f, 8.742277E-08f, 0f, 8.893519E-08f, 8.588372E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742277E-08f, 8.742277E-08f, 3.159046f)
            };
            // 304. - X: 270, Y: 180, Z: 1
            yield return new object[]
            {
                "X: 270, Y: 180, Z: 1",
                new float3(4.712389f, 3.141593f, 0.01745329f),
                new Quaternion(-0.006170623f, -0.7070798f, -0.7070798f, 0.006170623f),
                new float4x4(-0.9998475f, 0.01745249f, 0f, 0f, 0f, 8.116331E-08f, 0.9999999f, 0f, 0.01745249f, 0.9998476f, 8.116331E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.159046f, 0f)
            };
            // 305. - X: 360, Y: 180, Z: 1
            yield return new object[]
            {
                "X: 360, Y: 180, Z: 1",
                new float3(6.283185f, 3.141593f, 0.01745329f),
                new Quaternion(-0.008726535f, -0.9999619f, 8.780089E-08f, 4.294682E-08f),
                new float4x4(-0.9998477f, 0.01745241f, -8.742277E-08f, 0f, 0.01745241f, 0.9998477f, -1.748455E-07f, 0f, 8.435799E-08f, -1.763446E-07f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748455E-07f, 3.141593f, 0.01745329f)
            };
            // 306. - X: 585, Y: 180, Z: 1
            yield return new object[]
            {
                "X: 585, Y: 180, Z: 1",
                new float3(10.21018f, 3.141593f, 0.01745329f),
                new Quaternion(0.003339542f, 0.382669f, 0.9238443f, -0.008062284f),
                new float4x4(-0.9998477f, 0.01745247f, 6.146729E-08f, 0f, -0.01234071f, -0.7069988f, 0.707107f, 0f, 0.01234081f, 0.7069994f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 8.69279E-08f, 3.159046f)
            };
            // 307. - X: -90, Y: 180, Z: 1
            yield return new object[]
            {
                "X: -90, Y: 180, Z: 1",
                new float3(-1.570796f, 3.141593f, 0.01745329f),
                new Quaternion(0.006170623f, 0.7070798f, 0.7070798f, -0.006170623f),
                new float4x4(-0.9998475f, 0.01745249f, 0f, 0f, 0f, 8.116331E-08f, 0.9999999f, 0f, 0.01745249f, 0.9998476f, 8.116331E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.159046f, 0f)
            };
            // 308. - X: -540, Y: 180, Z: 1
            yield return new object[]
            {
                "X: -540, Y: 180, Z: 1",
                new float3(-9.424778f, 3.141593f, 0.01745329f),
                new Quaternion(-4.360566E-08f, 1.230588E-08f, -0.9999619f, 0.008726535f),
                new float4x4(-0.9998477f, 0.01745241f, 8.742278E-08f, 0f, -0.01745241f, -0.9998477f, -2.384976E-08f, 0f, 8.699323E-08f, -2.537187E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 8.742278E-08f, 3.159046f)
            };
            // 309. - X: 0, Y: 270, Z: 1
            yield return new object[]
            {
                "X: 0, Y: 270, Z: 1",
                new float3(0f, 4.712389f, 0.01745329f),
                new Quaternion(0.006170592f, 0.7070798f, -0.006170592f, -0.7070798f),
                new float4x4(8.192001E-08f, 0f, -0.9999999f, 0f, 0.01745241f, 0.9998477f, 0f, 0f, 0.9998476f, -0.01745241f, 8.192001E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 0.01745329f)
            };
            // 310. - X: 0.1, Y: 270, Z: 1
            yield return new object[]
            {
                "X: 0.1, Y: 270, Z: 1",
                new float3(0.001745329f, 4.712389f, 0.01745329f),
                new Quaternion(0.005553546f, 0.707085f, -0.006787634f, -0.7070742f),
                new float4x4(-3.039352E-05f, -0.001745064f, -0.9999985f, 0f, 0.01745238f, 0.9998462f, -0.001745328f, 0f, 0.9998477f, -0.01745241f, 6.665505E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 4.712389f, 0.01745329f)
            };
            // 311. - X: 1, Y: 270, Z: 1
            yield return new object[]
            {
                "X: 1, Y: 270, Z: 1",
                new float3(0.01745329f, 4.712389f, 0.01745329f),
                new Quaternion(0f, 0.7071067f, -0.01234071f, -0.7069991f),
                new float4x4(-0.0003044076f, -0.01744975f, -0.9998476f, 0f, 0.01744975f, 0.9996954f, -0.0174524f, 0f, 0.9998476f, -0.0174524f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 4.712389f, 0.01745329f)
            };
            // 312. - X: 45, Y: 270, Z: 1
            yield return new object[]
            {
                "X: 45, Y: 270, Z: 1",
                new float3(0.7853982f, 4.712389f, 0.01745329f),
                new Quaternion(-0.2648869f, 0.655618f, -0.2762886f, -0.6508952f),
                new float4x4(-0.01234062f, -0.7069991f, -0.7071067f, 0f, 0.01234072f, 0.7069991f, -0.7071067f, 0f, 0.9998477f, -0.01745239f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853981f, 4.712389f, 0.01745331f)
            };
            // 313. - X: 90, Y: 270, Z: 1
            yield return new object[]
            {
                "X: 90, Y: 270, Z: 1",
                new float3(1.570796f, 4.712389f, 0.01745329f),
                new Quaternion(-0.4956177f, 0.5043442f, -0.5043442f, -0.4956177f),
                new float4x4(-0.01745212f, -0.9998475f, 0f, 0f, 0f, 2.086163E-07f, -0.9999998f, 0f, 0.9998475f, -0.01745233f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 4.694936f, 0f)
            };
            // 314. - X: 180, Y: 270, Z: 1
            yield return new object[]
            {
                "X: 180, Y: 270, Z: 1",
                new float3(3.141593f, 4.712389f, 0.01745329f),
                new Quaternion(-0.7070798f, 0.006170562f, -0.7070798f, 0.006170623f),
                new float4x4(8.268398E-08f, 8.6613E-08f, 0.9999999f, 0f, -0.0174524f, -0.9998475f, 8.6613E-08f, 0f, 0.9998476f, -0.0174524f, 8.268398E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.6613E-08f, 1.570796f, 3.159046f)
            };
            // 315. - X: 270, Y: 270, Z: 1
            yield return new object[]
            {
                "X: 270, Y: 270, Z: 1",
                new float3(4.712389f, 4.712389f, 0.01745329f),
                new Quaternion(-0.5043442f, -0.4956177f, -0.4956177f, 0.5043442f),
                new float4x4(0.01745254f, 0.9998475f, 0f, 0f, 0f, 2.086163E-07f, 0.9999998f, 0f, 0.9998475f, -0.01745233f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.729842f, 0f)
            };
            // 316. - X: 360, Y: 270, Z: 1
            yield return new object[]
            {
                "X: 360, Y: 270, Z: 1",
                new float3(6.283185f, 4.712389f, 0.01745329f),
                new Quaternion(-0.00617053f, -0.7070798f, 0.006170654f, 0.7070798f),
                new float4x4(8.039206E-08f, -1.750886E-07f, -0.9999999f, 0f, 0.01745241f, 0.9998477f, -1.750886E-07f, 0f, 0.9998476f, -0.01745241f, 8.344796E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.750886E-07f, 4.712389f, 0.01745329f)
            };
            // 317. - X: 585, Y: 270, Z: 1
            yield return new object[]
            {
                "X: 585, Y: 270, Z: 1",
                new float3(10.21018f, 4.712389f, 0.01745329f),
                new Quaternion(0.6556179f, 0.264887f, 0.6508952f, -0.2762887f),
                new float4x4(0.01234069f, 0.7069993f, 0.7071065f, 0f, -0.01234072f, -0.7069987f, 0.707107f, 0f, 0.9998477f, -0.01745236f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.570796f, 3.159046f)
            };
            // 318. - X: -90, Y: 270, Z: 1
            yield return new object[]
            {
                "X: -90, Y: 270, Z: 1",
                new float3(-1.570796f, 4.712389f, 0.01745329f),
                new Quaternion(0.5043442f, 0.4956177f, 0.4956177f, -0.5043442f),
                new float4x4(0.01745254f, 0.9998475f, 0f, 0f, 0f, 2.086163E-07f, 0.9999998f, 0f, 0.9998475f, -0.01745233f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.729842f, 0f)
            };
            // 319. - X: -540, Y: 270, Z: 1
            yield return new object[]
            {
                "X: -540, Y: 270, Z: 1",
                new float3(-9.424778f, 4.712389f, 0.01745329f),
                new Quaternion(-0.7070798f, 0.006170601f, -0.7070798f, 0.006170584f),
                new float4x4(8.171628E-08f, -2.328306E-08f, 0.9999999f, 0f, -0.0174524f, -0.9998475f, -2.328306E-08f, 0f, 0.9998476f, -0.0174524f, 8.171628E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.328306E-08f, 1.570796f, 3.159046f)
            };
            // 320. - X: 0, Y: 360, Z: 1
            yield return new object[]
            {
                "X: 0, Y: 360, Z: 1",
                new float3(0f, 6.283185f, 0.01745329f),
                new Quaternion(-7.62898E-10f, -8.741944E-08f, -0.008726535f, -0.9999619f),
                new float4x4(0.9998477f, -0.01745241f, 1.748455E-07f, 0f, 0.01745241f, 0.9998477f, -1.110223E-16f, 0f, -1.748189E-07f, 3.051476E-09f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.110223E-16f, 1.748455E-07f, 0.01745329f)
            };
            // 321. - X: 0.1, Y: 360, Z: 1
            yield return new object[]
            {
                "X: 0.1, Y: 360, Z: 1",
                new float3(0.001745329f, 6.283185f, 0.01745329f),
                new Quaternion(-0.000872632f, 7.527919E-06f, -0.008726533f, -0.9999616f),
                new float4x4(0.9998477f, -0.01745241f, 1.748449E-07f, 0f, 0.01745238f, 0.9998462f, -0.001745328f, 0f, 3.028536E-05f, 0.001745066f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 1.748452E-07f, 0.01745329f)
            };
            // 322. - X: 1, Y: 360, Z: 1
            yield return new object[]
            {
                "X: 1, Y: 360, Z: 1",
                new float3(0.01745329f, 6.283185f, 0.01745329f),
                new Quaternion(-0.008726204f, 7.606501E-05f, -0.008726202f, -0.9999238f),
                new float4x4(0.9998477f, -0.0174524f, 1.747976E-07f, 0f, 0.01744975f, 0.9996954f, -0.01745241f, 0f, 0.0003044117f, 0.01744975f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.0174533f, 1.748533E-07f, 0.01745329f)
            };
            // 323. - X: 45, Y: 360, Z: 1
            yield return new object[]
            {
                "X: 45, Y: 360, Z: 1",
                new float3(0.7853982f, 6.283185f, 0.01745329f),
                new Quaternion(-0.3826689f, 0.00333942f, -0.008062233f, -0.9238443f),
                new float4x4(0.9998477f, -0.01745228f, 1.234002E-07f, 0f, 0.01234071f, 0.7069991f, -0.7071068f, 0f, 0.01234054f, 0.7069991f, 0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 1.745143E-07f, 0.01745329f)
            };
            // 324. - X: 90, Y: 360, Z: 1
            yield return new object[]
            {
                "X: 90, Y: 360, Z: 1",
                new float3(1.570796f, 6.283185f, 0.01745329f),
                new Quaternion(-0.7070798f, 0.00617053f, -0.00617053f, -0.7070798f),
                new float4x4(0.9998477f, -0.01745223f, 0f, 0f, 0f, 8.344796E-08f, -0.9999999f, 0f, 0.01745223f, 0.9998476f, 8.344796E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 6.265732f, 0f)
            };
            // 325. - X: 180, Y: 360, Z: 1
            yield return new object[]
            {
                "X: 180, Y: 360, Z: 1",
                new float3(3.141593f, 6.283185f, 0.01745329f),
                new Quaternion(-0.9999619f, 0.008726535f, 8.780089E-08f, 4.294682E-08f),
                new float4x4(0.9998477f, -0.01745241f, -1.748455E-07f, 0f, -0.01745241f, -0.9998477f, 8.742277E-08f, 0f, -1.763446E-07f, -8.435799E-08f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742277E-08f, 3.141593f, 3.159046f)
            };
            // 326. - X: 270, Y: 360, Z: 1
            yield return new object[]
            {
                "X: 270, Y: 360, Z: 1",
                new float3(4.712389f, 6.283185f, 0.01745329f),
                new Quaternion(-0.7070798f, 0.006170654f, 0.006170654f, 0.7070798f),
                new float4x4(0.9998477f, -0.01745258f, 0f, 0f, 0f, 8.039206E-08f, 0.9999999f, 0f, -0.01745258f, -0.9998476f, 8.039206E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.01745347f, 0f)
            };
            // 327. - X: 360, Y: 360, Z: 1
            yield return new object[]
            {
                "X: 360, Y: 360, Z: 1",
                new float3(6.283185f, 6.283185f, 0.01745329f),
                new Quaternion(8.818234E-08f, 8.665655E-08f, 0.008726535f, 0.9999619f),
                new float4x4(0.9998477f, -0.01745241f, 1.748455E-07f, 0f, 0.01745241f, 0.9998477f, -1.748455E-07f, 0f, -1.717674E-07f, 1.778704E-07f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748455E-07f, 1.748455E-07f, 0.01745329f)
            };
            // 328. - X: 585, Y: 360, Z: 1
            yield return new object[]
            {
                "X: 585, Y: 360, Z: 1",
                new float3(10.21018f, 6.283185f, 0.01745329f),
                new Quaternion(0.9238443f, -0.0080623f, -0.003339583f, -0.382669f),
                new float4x4(0.9998477f, -0.01745253f, -1.238659E-07f, 0f, -0.01234071f, -0.7069988f, 0.707107f, 0f, -0.01234089f, -0.7069994f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.141593f, 3.159046f)
            };
            // 329. - X: -90, Y: 360, Z: 1
            yield return new object[]
            {
                "X: -90, Y: 360, Z: 1",
                new float3(-1.570796f, 6.283185f, 0.01745329f),
                new Quaternion(0.7070798f, -0.006170654f, -0.006170654f, -0.7070798f),
                new float4x4(0.9998477f, -0.01745258f, 0f, 0f, 0f, 8.039206E-08f, 0.9999999f, 0f, -0.01745258f, -0.9998476f, 8.039206E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.01745347f, 0f)
            };
            // 330. - X: -540, Y: 360, Z: 1
            yield return new object[]
            {
                "X: -540, Y: 360, Z: 1",
                new float3(-9.424778f, 6.283185f, 0.01745329f),
                new Quaternion(-0.9999619f, 0.008726535f, 8.731538E-08f, -1.268732E-08f),
                new float4x4(0.9998477f, -0.01745241f, -1.748455E-07f, 0f, -0.01745241f, -0.9998477f, -2.384976E-08f, 0f, -1.744027E-07f, 2.689761E-08f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.141593f, 3.159046f)
            };
            // 331. - X: 0, Y: 585, Z: 1
            yield return new object[]
            {
                "X: 0, Y: 585, Z: 1",
                new float3(0f, 10.21018f, 0.01745329f),
                new Quaternion(-0.008062267f, -0.9238443f, 0.003339502f, 0.382669f),
                new float4x4(-0.7069988f, 0.01234071f, -0.707107f, 0f, 0.01745241f, 0.9998477f, -4.656613E-10f, 0f, 0.7069994f, -0.01234072f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.656613E-10f, 3.926991f, 0.01745329f)
            };
            // 332. - X: 0.1, Y: 585, Z: 1
            yield return new object[]
            {
                "X: 0.1, Y: 585, Z: 1",
                new float3(0.001745329f, 10.21018f, 0.01745329f),
                new Quaternion(-0.007728322f, -0.9238468f, 0.004145707f, 0.3826618f),
                new float4x4(-0.7070203f, 0.01110676f, -0.7071059f, 0f, 0.01745238f, 0.9998462f, -0.001745328f, 0f, 0.7069778f, -0.01357466f, -0.7071054f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.926991f, 0.01745329f)
            };
            // 333. - X: 1, Y: 585, Z: 1
            yield return new object[]
            {
                "X: 1, Y: 585, Z: 1",
                new float3(0.01745329f, 10.21018f, 0.01745329f),
                new Quaternion(-0.004722585f, -0.9238383f, 0.01140133f, 0.3825841f),
                new float4x4(-0.7072142f, 1.870096E-06f, -0.7069994f, 0f, 0.01744975f, 0.9996954f, -0.01745241f, 0f, 0.706784f, -0.02467955f, -0.7069988f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.926991f, 0.01745329f)
            };
            // 334. - X: 45, Y: 585, Z: 1
            yield return new object[]
            {
                "X: 45, Y: 585, Z: 1",
                new float3(0.7853982f, 10.21018f, 0.01745329f),
                new Quaternion(0.1389925f, -0.8547988f, 0.3566252f, 0.3504548f),
                new float4x4(-0.7157251f, -0.4875833f, -0.5000002f, 0f, 0.01234069f, 0.7069991f, -0.7071068f, 0f, 0.6982731f, -0.5122644f, -0.4999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 3.926991f, 0.01745326f)
            };
            // 335. - X: 90, Y: 585, Z: 1
            yield return new object[]
            {
                "X: 90, Y: 585, Z: 1",
                new float3(1.570796f, 10.21018f, 0.01745329f),
                new Quaternion(0.264887f, -0.6556179f, 0.6556179f, 0.264887f),
                new float4x4(-0.7193393f, -0.6946586f, 0f, 0f, 0f, 1.490116E-07f, -0.9999999f, 0f, 0.6946586f, -0.7193394f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 3.909538f, 0f)
            };
            // 336. - X: 180, Y: 585, Z: 1
            yield return new object[]
            {
                "X: 180, Y: 585, Z: 1",
                new float3(3.141593f, 10.21018f, 0.01745329f),
                new Quaternion(0.382669f, -0.003339462f, 0.9238443f, -0.008062284f),
                new float4x4(-0.7069988f, 0.01234077f, 0.707107f, 0f, -0.01745241f, -0.9998477f, 8.707866E-08f, 0f, 0.7069994f, -0.01234066f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.707866E-08f, 0.7853985f, 3.159046f)
            };
            // 337. - X: 270, Y: 585, Z: 1
            yield return new object[]
            {
                "X: 270, Y: 585, Z: 1",
                new float3(4.712389f, 10.21018f, 0.01745329f),
                new Quaternion(0.2762887f, 0.6508952f, 0.6508952f, -0.2762887f),
                new float4x4(-0.6946582f, 0.71934f, 0f, 0f, 0f, -2.980232E-08f, 1f, 0f, 0.71934f, 0.6946582f, -2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.944444f, 0f)
            };
            // 338. - X: 360, Y: 585, Z: 1
            yield return new object[]
            {
                "X: 360, Y: 585, Z: 1",
                new float3(6.283185f, 10.21018f, 0.01745329f),
                new Quaternion(0.008062233f, 0.9238443f, -0.003339583f, -0.382669f),
                new float4x4(-0.7069988f, 0.01234059f, -0.707107f, 0f, 0.01745241f, 0.9998477f, -1.750886E-07f, 0f, 0.7069994f, -0.01234084f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.750886E-07f, 3.926991f, 0.01745329f)
            };
            // 339. - X: 585, Y: 585, Z: 1
            yield return new object[]
            {
                "X: 585, Y: 585, Z: 1",
                new float3(10.21018f, 10.21018f, 0.01745329f),
                new Quaternion(-0.3566253f, -0.3504547f, -0.8522428f, 0.1538897f),
                new float4x4(-0.6982726f, 0.5122649f, 0.4999999f, 0f, -0.01234074f, -0.7069988f, 0.707107f, 0f, 0.7157255f, 0.4875831f, 0.4999997f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 0.7853985f, 3.159046f)
            };
            // 340. - X: -90, Y: 585, Z: 1
            yield return new object[]
            {
                "X: -90, Y: 585, Z: 1",
                new float3(-1.570796f, 10.21018f, 0.01745329f),
                new Quaternion(-0.2762887f, -0.6508952f, -0.6508952f, 0.2762887f),
                new float4x4(-0.6946582f, 0.71934f, 0f, 0f, 0f, -2.980232E-08f, 1f, 0f, 0.71934f, 0.6946582f, -2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.944444f, 0f)
            };
            // 341. - X: -540, Y: 585, Z: 1
            yield return new object[]
            {
                "X: -540, Y: 585, Z: 1",
                new float3(-9.424778f, 10.21018f, 0.01745329f),
                new Quaternion(0.382669f, -0.003339513f, 0.9238443f, -0.008062262f),
                new float4x4(-0.7069988f, 0.01234069f, 0.707107f, 0f, -0.01745241f, -0.9998477f, -2.374873E-08f, 0f, 0.7069994f, -0.01234074f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.374873E-08f, 0.7853985f, 3.159046f)
            };
            // 342. - X: 0, Y: -90, Z: 1
            yield return new object[]
            {
                "X: 0, Y: -90, Z: 1",
                new float3(0f, -1.570796f, 0.01745329f),
                new Quaternion(-0.006170592f, -0.7070798f, 0.006170592f, 0.7070798f),
                new float4x4(8.192001E-08f, 0f, -0.9999999f, 0f, 0.01745241f, 0.9998477f, 0f, 0f, 0.9998476f, -0.01745241f, 8.192001E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 0.01745329f)
            };
            // 343. - X: 0.1, Y: -90, Z: 1
            yield return new object[]
            {
                "X: 0.1, Y: -90, Z: 1",
                new float3(0.001745329f, -1.570796f, 0.01745329f),
                new Quaternion(-0.005553546f, -0.707085f, 0.006787634f, 0.7070742f),
                new float4x4(-3.039352E-05f, -0.001745064f, -0.9999985f, 0f, 0.01745238f, 0.9998462f, -0.001745328f, 0f, 0.9998477f, -0.01745241f, 6.665505E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 4.712389f, 0.01745329f)
            };
            // 344. - X: 1, Y: -90, Z: 1
            yield return new object[]
            {
                "X: 1, Y: -90, Z: 1",
                new float3(0.01745329f, -1.570796f, 0.01745329f),
                new Quaternion(0f, -0.7071067f, 0.01234071f, 0.7069991f),
                new float4x4(-0.0003044076f, -0.01744975f, -0.9998476f, 0f, 0.01744975f, 0.9996954f, -0.0174524f, 0f, 0.9998476f, -0.0174524f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 4.712389f, 0.01745329f)
            };
            // 345. - X: 45, Y: -90, Z: 1
            yield return new object[]
            {
                "X: 45, Y: -90, Z: 1",
                new float3(0.7853982f, -1.570796f, 0.01745329f),
                new Quaternion(0.2648869f, -0.655618f, 0.2762886f, 0.6508952f),
                new float4x4(-0.01234062f, -0.7069991f, -0.7071067f, 0f, 0.01234072f, 0.7069991f, -0.7071067f, 0f, 0.9998477f, -0.01745239f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853981f, 4.712389f, 0.01745331f)
            };
            // 346. - X: 90, Y: -90, Z: 1
            yield return new object[]
            {
                "X: 90, Y: -90, Z: 1",
                new float3(1.570796f, -1.570796f, 0.01745329f),
                new Quaternion(0.4956177f, -0.5043442f, 0.5043442f, 0.4956177f),
                new float4x4(-0.01745212f, -0.9998475f, 0f, 0f, 0f, 2.086163E-07f, -0.9999998f, 0f, 0.9998475f, -0.01745233f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 4.694936f, 0f)
            };
            // 347. - X: 180, Y: -90, Z: 1
            yield return new object[]
            {
                "X: 180, Y: -90, Z: 1",
                new float3(3.141593f, -1.570796f, 0.01745329f),
                new Quaternion(0.7070798f, -0.006170562f, 0.7070798f, -0.006170623f),
                new float4x4(8.268398E-08f, 8.6613E-08f, 0.9999999f, 0f, -0.0174524f, -0.9998475f, 8.6613E-08f, 0f, 0.9998476f, -0.0174524f, 8.268398E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.6613E-08f, 1.570796f, 3.159046f)
            };
            // 348. - X: 270, Y: -90, Z: 1
            yield return new object[]
            {
                "X: 270, Y: -90, Z: 1",
                new float3(4.712389f, -1.570796f, 0.01745329f),
                new Quaternion(0.5043442f, 0.4956177f, 0.4956177f, -0.5043442f),
                new float4x4(0.01745254f, 0.9998475f, 0f, 0f, 0f, 2.086163E-07f, 0.9999998f, 0f, 0.9998475f, -0.01745233f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.729842f, 0f)
            };
            // 349. - X: 360, Y: -90, Z: 1
            yield return new object[]
            {
                "X: 360, Y: -90, Z: 1",
                new float3(6.283185f, -1.570796f, 0.01745329f),
                new Quaternion(0.00617053f, 0.7070798f, -0.006170654f, -0.7070798f),
                new float4x4(8.039206E-08f, -1.750886E-07f, -0.9999999f, 0f, 0.01745241f, 0.9998477f, -1.750886E-07f, 0f, 0.9998476f, -0.01745241f, 8.344796E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.750886E-07f, 4.712389f, 0.01745329f)
            };
            // 350. - X: 585, Y: -90, Z: 1
            yield return new object[]
            {
                "X: 585, Y: -90, Z: 1",
                new float3(10.21018f, -1.570796f, 0.01745329f),
                new Quaternion(-0.6556179f, -0.264887f, -0.6508952f, 0.2762887f),
                new float4x4(0.01234069f, 0.7069993f, 0.7071065f, 0f, -0.01234072f, -0.7069987f, 0.707107f, 0f, 0.9998477f, -0.01745236f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.570796f, 3.159046f)
            };
            // 351. - X: -90, Y: -90, Z: 1
            yield return new object[]
            {
                "X: -90, Y: -90, Z: 1",
                new float3(-1.570796f, -1.570796f, 0.01745329f),
                new Quaternion(-0.5043442f, -0.4956177f, -0.4956177f, 0.5043442f),
                new float4x4(0.01745254f, 0.9998475f, 0f, 0f, 0f, 2.086163E-07f, 0.9999998f, 0f, 0.9998475f, -0.01745233f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.729842f, 0f)
            };
            // 352. - X: -540, Y: -90, Z: 1
            yield return new object[]
            {
                "X: -540, Y: -90, Z: 1",
                new float3(-9.424778f, -1.570796f, 0.01745329f),
                new Quaternion(0.7070798f, -0.006170601f, 0.7070798f, -0.006170584f),
                new float4x4(8.171628E-08f, -2.328306E-08f, 0.9999999f, 0f, -0.0174524f, -0.9998475f, -2.328306E-08f, 0f, 0.9998476f, -0.0174524f, 8.171628E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.328306E-08f, 1.570796f, 3.159046f)
            };
            // 353. - X: 0, Y: -540, Z: 1
            yield return new object[]
            {
                "X: 0, Y: -540, Z: 1",
                new float3(0f, -9.424778f, 0.01745329f),
                new Quaternion(0.008726535f, 0.9999619f, 1.040629E-10f, 1.192443E-08f),
                new float4x4(-0.9998477f, 0.01745241f, 2.384976E-08f, 0f, 0.01745241f, 0.9998477f, -1.387779E-17f, 0f, -2.384613E-08f, 4.162357E-10f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.387779E-17f, 3.141593f, 0.01745329f)
            };
            // 354. - X: 0.1, Y: -540, Z: 1
            yield return new object[]
            {
                "X: 0.1, Y: -540, Z: 1",
                new float3(0.001745329f, -9.424778f, 0.01745329f),
                new Quaternion(0.008726533f, 0.9999616f, -0.0008726311f, 7.627262E-06f),
                new float4x4(-0.9998478f, 0.01745241f, 2.384968E-08f, 0f, 0.01745238f, 0.9998462f, -0.001745328f, 0f, -3.048403E-05f, -0.001745062f, -0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.141593f, 0.01745329f)
            };
            // 355. - X: 1, Y: -540, Z: 1
            yield return new object[]
            {
                "X: 1, Y: -540, Z: 1",
                new float3(0.01745329f, -9.424778f, 0.01745329f),
                new Quaternion(0.008726203f, 0.9999238f, -0.008726203f, 7.616435E-05f),
                new float4x4(-0.9998476f, 0.01745241f, 2.386514E-08f, 0f, 0.01744975f, 0.9996954f, -0.01745241f, 0f, -0.0003046103f, -0.01744975f, -0.9998476f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.141593f, 0.01745329f)
            };
            // 356. - X: 45, Y: -540, Z: 1
            yield return new object[]
            {
                "X: 45, Y: -540, Z: 1",
                new float3(0.7853982f, -9.424778f, 0.01745329f),
                new Quaternion(0.008062271f, 0.9238443f, -0.3826689f, 0.003339512f),
                new float4x4(-0.9998477f, 0.01745242f, 1.676381E-08f, 0f, 0.01234071f, 0.7069991f, -0.7071068f, 0f, -0.01234074f, -0.7069991f, -0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 3.141593f, 0.01745329f)
            };
            // 357. - X: 90, Y: -540, Z: 1
            yield return new object[]
            {
                "X: 90, Y: -540, Z: 1",
                new float3(1.570796f, -9.424778f, 0.01745329f),
                new Quaternion(0.006170601f, 0.7070798f, -0.7070798f, 0.006170601f),
                new float4x4(-0.9998475f, 0.01745243f, 0f, 0f, 0f, 8.171628E-08f, -0.9999999f, 0f, -0.01745243f, -0.9998476f, 8.171628E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 3.124139f, 0f)
            };
            // 358. - X: 180, Y: -540, Z: 1
            yield return new object[]
            {
                "X: 180, Y: -540, Z: 1",
                new float3(3.141593f, -9.424778f, 0.01745329f),
                new Quaternion(1.154298E-08f, -4.381378E-08f, -0.9999619f, 0.008726535f),
                new float4x4(-0.9998477f, 0.01745241f, -2.384976E-08f, 0f, -0.01745241f, -0.9998477f, 8.742277E-08f, 0f, -2.232039E-08f, 8.782569E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742277E-08f, -2.384976E-08f, 3.159046f)
            };
            // 359. - X: 270, Y: -540, Z: 1
            yield return new object[]
            {
                "X: 270, Y: -540, Z: 1",
                new float3(4.712389f, -9.424778f, 0.01745329f),
                new Quaternion(-0.006170584f, -0.7070798f, -0.7070798f, 0.006170584f),
                new float4x4(-0.9998475f, 0.01745238f, 0f, 0f, 0f, 8.213101E-08f, 0.9999999f, 0f, 0.01745238f, 0.9998476f, 8.213101E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.159046f, 0f)
            };
            // 360. - X: 360, Y: -540, Z: 1
            yield return new object[]
            {
                "X: 360, Y: -540, Z: 1",
                new float3(6.283185f, -9.424778f, 0.01745329f),
                new Quaternion(-0.008726535f, -0.9999619f, 8.731538E-08f, -1.268732E-08f),
                new float4x4(-0.9998477f, 0.01745241f, 2.384976E-08f, 0f, 0.01745241f, 0.9998477f, -1.748455E-07f, 0f, -2.689761E-08f, -1.744027E-07f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748455E-07f, 3.141593f, 0.01745329f)
            };
            // 361. - X: 585, Y: -540, Z: 1
            yield return new object[]
            {
                "X: 585, Y: -540, Z: 1",
                new float3(10.21018f, -9.424778f, 0.01745329f),
                new Quaternion(0.003339491f, 0.382669f, 0.9238443f, -0.008062262f),
                new float4x4(-0.9998477f, 0.01745239f, -1.676381E-08f, 0f, -0.01234071f, -0.7069988f, 0.707107f, 0f, 0.0123407f, 0.7069994f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, -2.370761E-08f, 3.159046f)
            };
            // 362. - X: -90, Y: -540, Z: 1
            yield return new object[]
            {
                "X: -90, Y: -540, Z: 1",
                new float3(-1.570796f, -9.424778f, 0.01745329f),
                new Quaternion(0.006170584f, 0.7070798f, 0.7070798f, -0.006170584f),
                new float4x4(-0.9998475f, 0.01745238f, 0f, 0f, 0f, 8.213101E-08f, 0.9999999f, 0f, 0.01745238f, 0.9998476f, 8.213101E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.159046f, 0f)
            };
            // 363. - X: -540, Y: -540, Z: 1
            yield return new object[]
            {
                "X: -540, Y: -540, Z: 1",
                new float3(-9.424778f, -9.424778f, 0.01745329f),
                new Quaternion(1.202849E-08f, 1.182036E-08f, -0.9999619f, 0.008726535f),
                new float4x4(-0.9998477f, 0.01745241f, -2.384976E-08f, 0f, -0.01745241f, -0.9998477f, -2.384976E-08f, 0f, -2.426236E-08f, -2.342989E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, -2.384976E-08f, 3.159046f)
            };
            // 364. - X: 0, Y: 0, Z: 45
            yield return new object[]
            {
                "X: 0, Y: 0, Z: 45",
                new float3(0f, 0f, 0.7853982f),
                new Quaternion(0f, 0f, 0.3826835f, 0.9238795f),
                new float4x4(0.7071067f, -0.7071068f, 0f, 0f, 0.7071068f, 0.7071067f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0f, 0.7853982f)
            };
            // 365. - X: 0.1, Y: 0, Z: 45
            yield return new object[]
            {
                "X: 0.1, Y: 0, Z: 45",
                new float3(0.001745329f, 0f, 0.7853982f),
                new Quaternion(0.0008062369f, -0.0003339543f, 0.3826833f, 0.9238791f),
                new float4x4(0.7071068f, -0.7071068f, 5.820766E-11f, 0f, 0.7071057f, 0.7071057f, -0.001745328f, 0f, 0.001234133f, 0.001234133f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 5.820775E-11f, 0.7853982f)
            };
            // 366. - X: 1, Y: 0, Z: 45
            yield return new object[]
            {
                "X: 1, Y: 0, Z: 45",
                new float3(0.01745329f, 0f, 0.7853982f),
                new Quaternion(0.008062267f, -0.003339501f, 0.3826689f, 0.9238443f),
                new float4x4(0.7071068f, -0.7071068f, -4.656613E-10f, 0f, 0.7069991f, 0.7069991f, -0.0174524f, 0f, 0.01234072f, 0.01234071f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, -4.657322E-10f, 0.7853982f)
            };
            // 367. - X: 45, Y: 0, Z: 45
            yield return new object[]
            {
                "X: 45, Y: 0, Z: 45",
                new float3(0.7853982f, 0f, 0.7853982f),
                new Quaternion(0.3535534f, -0.1464466f, 0.3535534f, 0.8535534f),
                new float4x4(0.7071067f, -0.7071068f, 0f, 0f, 0.5f, 0.4999999f, -0.7071068f, 0f, 0.5000001f, 0.5f, 0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 0f, 0.7853982f)
            };
            // 368. - X: 90, Y: 0, Z: 45
            yield return new object[]
            {
                "X: 90, Y: 0, Z: 45",
                new float3(1.570796f, 0f, 0.7853982f),
                new Quaternion(0.6532815f, -0.2705981f, 0.2705981f, 0.6532815f),
                new float4x4(0.7071068f, -0.7071068f, 0f, 0f, 0f, 8.940697E-08f, -0.9999999f, 0f, 0.7071068f, 0.7071067f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 5.497787f, 0f)
            };
            // 369. - X: 180, Y: 0, Z: 45
            yield return new object[]
            {
                "X: 180, Y: 0, Z: 45",
                new float3(3.141593f, 0f, 0.7853982f),
                new Quaternion(0.9238795f, -0.3826835f, -1.672763E-08f, -4.038406E-08f),
                new float4x4(0.7071067f, -0.7071068f, 0f, 0f, -0.7071068f, -0.7071067f, 8.742278E-08f, 0f, -6.181724E-08f, -6.181723E-08f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 3.141593f, 3.926991f)
            };
            // 370. - X: 270, Y: 0, Z: 45
            yield return new object[]
            {
                "X: 270, Y: 0, Z: 45",
                new float3(4.712389f, 0f, 0.7853982f),
                new Quaternion(0.6532815f, -0.2705981f, -0.2705981f, -0.6532815f),
                new float4x4(0.7071068f, -0.7071068f, 0f, 0f, 0f, 8.940697E-08f, 0.9999999f, 0f, -0.7071068f, -0.7071067f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.7853982f, 0f)
            };
            // 371. - X: 360, Y: 0, Z: 45
            yield return new object[]
            {
                "X: 360, Y: 0, Z: 45",
                new float3(6.283185f, 0f, 0.7853982f),
                new Quaternion(-8.076811E-08f, 3.345525E-08f, -0.3826835f, -0.9238795f),
                new float4x4(0.7071067f, -0.7071068f, 0f, 0f, 0.7071068f, 0.7071067f, -1.748456E-07f, 0f, 1.236345E-07f, 1.236345E-07f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 0f, 0.7853982f)
            };
            // 372. - X: 585, Y: 0, Z: 45
            yield return new object[]
            {
                "X: 585, Y: 0, Z: 45",
                new float3(10.21018f, 0f, 0.7853982f),
                new Quaternion(-0.8535533f, 0.3535534f, 0.1464467f, 0.3535535f),
                new float4x4(0.7071068f, -0.7071068f, -2.980232E-08f, 0f, -0.4999998f, -0.4999997f, 0.7071071f, 0f, -0.5000002f, -0.5000001f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.141593f, 3.926991f)
            };
            // 373. - X: -90, Y: 0, Z: 45
            yield return new object[]
            {
                "X: -90, Y: 0, Z: 45",
                new float3(-1.570796f, 0f, 0.7853982f),
                new Quaternion(-0.6532815f, 0.2705981f, 0.2705981f, 0.6532815f),
                new float4x4(0.7071068f, -0.7071068f, 0f, 0f, 0f, 8.940697E-08f, 0.9999999f, 0f, -0.7071068f, -0.7071067f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.7853982f, 0f)
            };
            // 374. - X: -540, Y: 0, Z: 45
            yield return new object[]
            {
                "X: -540, Y: 0, Z: 45",
                new float3(-9.424778f, 0f, 0.7853982f),
                new Quaternion(0.9238795f, -0.3826835f, 4.563455E-09f, 1.101715E-08f),
                new float4x4(0.7071067f, -0.7071068f, 0f, 0f, -0.7071068f, -0.7071067f, -2.384976E-08f, 0f, 1.686433E-08f, 1.686433E-08f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.141593f, 3.926991f)
            };
            // 375. - X: 0, Y: 0.1, Z: 45
            yield return new object[]
            {
                "X: 0, Y: 0.1, Z: 45",
                new float3(0f, 0.001745329f, 0.7853982f),
                new Quaternion(0.0003339543f, 0.0008062369f, 0.3826833f, 0.9238791f),
                new float4x4(0.7071057f, -0.7071057f, 0.001745328f, 0f, 0.7071068f, 0.7071068f, 5.820766E-11f, 0f, -0.001234133f, 0.001234133f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(-5.820766E-11f, 0.001745329f, 0.7853982f)
            };
            // 376. - X: 0.1, Y: 0.1, Z: 45
            yield return new object[]
            {
                "X: 0.1, Y: 0.1, Z: 45",
                new float3(0.001745329f, 0.001745329f, 0.7853982f),
                new Quaternion(0.001140191f, 0.0004722824f, 0.3826825f, 0.9238791f),
                new float4x4(0.7071078f, -0.7071036f, 0.001745326f, 0f, 0.7071058f, 0.7071056f, -0.001745329f, 0f, -1.74623E-09f, 0.002468265f, 0.999997f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0.001745329f, 0.7853982f)
            };
            // 377. - X: 1, Y: 0.1, Z: 45
            yield return new object[]
            {
                "X: 1, Y: 0.1, Z: 45",
                new float3(0.01745329f, 0.001745329f, 0.7853982f),
                new Quaternion(0.008396206f, -0.002533294f, 0.3826617f, 0.9238469f),
                new float4x4(0.7071272f, -0.7070842f, 0.001745062f, 0f, 0.7069991f, 0.7069991f, -0.01745241f, 0f, 0.01110656f, 0.01357483f, 0.9998462f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0.001745329f, 0.7853982f)
            };
            // 378. - X: 45, Y: 0.1, Z: 45
            yield return new object[]
            {
                "X: 45, Y: 0.1, Z: 45",
                new float3(0.7853982f, 0.001745329f, 0.7853982f),
                new Quaternion(0.3538618f, -0.1457017f, 0.3532447f, 0.8536808f),
                new float4x4(0.7079784f, -0.706233f, 0.001234129f, 0f, 0.4999999f, 0.5f, -0.7071068f, 0f, 0.4987651f, 0.5012333f, 0.7071056f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 0.001745323f, 0.7853982f)
            };
            // 379. - X: 90, Y: 0.1, Z: 45
            yield return new object[]
            {
                "X: 90, Y: 0.1, Z: 45",
                new float3(1.570796f, 0.001745329f, 0.7853982f),
                new Quaternion(0.6535174f, -0.2700279f, 0.2700279f, 0.6535174f),
                new float4x4(0.7083398f, -0.7058716f, 0f, 0f, 0f, -1.490116E-08f, -1f, 0f, 0.7058716f, 0.7083398f, -1.490116E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 5.499533f, 0f)
            };
            // 380. - X: 180, Y: 0.1, Z: 45
            yield return new object[]
            {
                "X: 180, Y: 0.1, Z: 45",
                new float3(3.141593f, 0.001745329f, 0.7853982f),
                new Quaternion(0.9238791f, -0.3826833f, -0.0008062536f, 0.0003339139f),
                new float4x4(0.7071057f, -0.7071057f, -0.001745328f, 0f, -0.7071068f, -0.7071067f, 8.742791E-08f, 0f, -0.001234195f, 0.001234072f, -0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742791E-08f, 3.143338f, 3.926991f)
            };
            // 381. - X: 270, Y: 0.1, Z: 45
            yield return new object[]
            {
                "X: 270, Y: 0.1, Z: 45",
                new float3(4.712389f, 0.001745329f, 0.7853982f),
                new Quaternion(0.6530451f, -0.2711681f, -0.2711681f, -0.6530451f),
                new float4x4(0.7058716f, -0.7083398f, 0f, 0f, 0f, 1.043081E-07f, 0.9999999f, 0f, -0.7083398f, -0.7058715f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.7871436f, 0f)
            };
            // 382. - X: 360, Y: 0.1, Z: 45
            yield return new object[]
            {
                "X: 360, Y: 0.1, Z: 45",
                new float3(6.283185f, 0.001745329f, 0.7853982f),
                new Quaternion(-0.000334035f, -0.0008062034f, -0.3826833f, -0.9238791f),
                new float4x4(0.7071057f, -0.7071057f, 0.001745328f, 0f, 0.7071068f, 0.7071068f, -1.747976E-07f, 0f, -0.00123401f, 0.001234257f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.747976E-07f, 0.001745329f, 0.7853982f)
            };
            // 383. - X: 585, Y: 0.1, Z: 45
            yield return new object[]
            {
                "X: 585, Y: 0.1, Z: 45",
                new float3(10.21018f, 0.001745329f, 0.7853982f),
                new Quaternion(-0.8534251f, 0.3538618f, 0.1471915f, 0.3532449f),
                new float4x4(0.706233f, -0.7079784f, -0.001234129f, 0f, -0.4999998f, -0.4999996f, 0.7071069f, 0f, -0.5012336f, -0.4987652f, -0.7071053f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.143338f, 3.926991f)
            };
            // 384. - X: -90, Y: 0.1, Z: 45
            yield return new object[]
            {
                "X: -90, Y: 0.1, Z: 45",
                new float3(-1.570796f, 0.001745329f, 0.7853982f),
                new Quaternion(-0.6530451f, 0.2711681f, 0.2711681f, 0.6530451f),
                new float4x4(0.7058716f, -0.7083398f, 0f, 0f, 0f, 1.043081E-07f, 0.9999999f, 0f, -0.7083398f, -0.7058715f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.7871436f, 0f)
            };
            // 385. - X: -540, Y: 0.1, Z: 45
            yield return new object[]
            {
                "X: -540, Y: 0.1, Z: 45",
                new float3(-9.424778f, 0.001745329f, 0.7853982f),
                new Quaternion(0.9238791f, -0.3826833f, -0.0008062323f, 0.0003339653f),
                new float4x4(0.7071057f, -0.7071057f, -0.001745328f, 0f, -0.7071068f, -0.7071067f, -2.380693E-08f, 0f, -0.001234117f, 0.00123415f, -0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.380693E-08f, 3.143338f, 3.926991f)
            };
            // 386. - X: 0, Y: 1, Z: 45
            yield return new object[]
            {
                "X: 0, Y: 1, Z: 45",
                new float3(0f, 0.01745329f, 0.7853982f),
                new Quaternion(0.003339501f, 0.008062267f, 0.3826689f, 0.9238443f),
                new float4x4(0.7069991f, -0.7069991f, 0.0174524f, 0f, 0.7071068f, 0.7071068f, -4.656613E-10f, 0f, -0.01234071f, 0.01234072f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.656613E-10f, 0.01745329f, 0.7853982f)
            };
            // 387. - X: 0.1, Y: 1, Z: 45
            yield return new object[]
            {
                "X: 0.1, Y: 1, Z: 45",
                new float3(0.001745329f, 0.01745329f, 0.7853982f),
                new Quaternion(0.004145706f, 0.007728323f, 0.3826617f, 0.9238469f),
                new float4x4(0.7070206f, -0.7069776f, 0.01745238f, 0f, 0.7071058f, 0.7071057f, -0.001745329f, 0f, -0.01110677f, 0.01357466f, 0.9998462f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.00174533f, 0.01745329f, 0.7853982f)
            };
            // 388. - X: 1, Y: 1, Z: 45
            yield return new object[]
            {
                "X: 1, Y: 1, Z: 45",
                new float3(0.01745329f, 0.01745329f, 0.7853982f),
                new Quaternion(0.01140133f, 0.004722587f, 0.3825839f, 0.9238383f),
                new float4x4(0.7072145f, -0.7067837f, 0.01744975f, 0f, 0.7069991f, 0.7069991f, -0.0174524f, 0f, -1.878478E-06f, 0.02467955f, 0.9996954f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0.0174533f, 0.7853982f)
            };
            // 389. - X: 45, Y: 1, Z: 45
            yield return new object[]
            {
                "X: 45, Y: 1, Z: 45",
                new float3(0.7853982f, 0.01745329f, 0.7853982f),
                new Quaternion(0.3566252f, -0.1389925f, 0.3504547f, 0.8547989f),
                new float4x4(0.7157252f, -0.6982729f, 0.01234071f, 0f, 0.5f, 0.4999999f, -0.7071068f, 0f, 0.4875832f, 0.5122645f, 0.7069991f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 0.01745328f, 0.7853982f)
            };
            // 390. - X: 90, Y: 1, Z: 45
            yield return new object[]
            {
                "X: 90, Y: 1, Z: 45",
                new float3(1.570796f, 0.01745329f, 0.7853982f),
                new Quaternion(0.6556179f, -0.2648869f, 0.2648869f, 0.6556179f),
                new float4x4(0.7193398f, -0.6946582f, 0f, 0f, 0f, 2.831221E-07f, -0.9999997f, 0f, 0.6946582f, 0.7193395f, 2.831221E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570451f, 5.51524f, 0f)
            };
            // 391. - X: 180, Y: 1, Z: 45
            yield return new object[]
            {
                "X: 180, Y: 1, Z: 45",
                new float3(3.141593f, 0.01745329f, 0.7853982f),
                new Quaternion(0.9238443f, -0.3826689f, -0.008062284f, 0.00333946f),
                new float4x4(0.7069991f, -0.7069991f, -0.01745241f, 0f, -0.7071068f, -0.7071068f, 8.707866E-08f, 0f, -0.01234077f, 0.01234065f, -0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.707866E-08f, 3.159046f, 3.926991f)
            };
            // 392. - X: 270, Y: 1, Z: 45
            yield return new object[]
            {
                "X: 270, Y: 1, Z: 45",
                new float3(4.712389f, 0.01745329f, 0.7853982f),
                new Quaternion(0.6508952f, -0.2762886f, -0.2762886f, -0.6508952f),
                new float4x4(0.6946584f, -0.7193397f, 0f, 0f, 0f, 1.043081E-07f, 0.9999999f, 0f, -0.7193397f, -0.6946583f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.8028516f, 0f)
            };
            // 393. - X: 360, Y: 1, Z: 45
            yield return new object[]
            {
                "X: 360, Y: 1, Z: 45",
                new float3(6.283185f, 0.01745329f, 0.7853982f),
                new Quaternion(-0.003339581f, -0.008062233f, -0.3826689f, -0.9238443f),
                new float4x4(0.7069991f, -0.7069991f, 0.0174524f, 0f, 0.7071068f, 0.7071068f, -1.750886E-07f, 0f, -0.01234059f, 0.01234084f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.750886E-07f, 0.01745329f, 0.7853982f)
            };
            // 394. - X: 585, Y: 1, Z: 45
            yield return new object[]
            {
                "X: 585, Y: 1, Z: 45",
                new float3(10.21018f, 0.01745329f, 0.7853982f),
                new Quaternion(-0.8522428f, 0.3566252f, 0.1538897f, 0.3504548f),
                new float4x4(0.6982729f, -0.7157253f, -0.01234071f, 0f, -0.4999998f, -0.4999997f, 0.707107f, 0f, -0.5122647f, -0.4875832f, -0.7069987f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.159046f, 3.926991f)
            };
            // 395. - X: -90, Y: 1, Z: 45
            yield return new object[]
            {
                "X: -90, Y: 1, Z: 45",
                new float3(-1.570796f, 0.01745329f, 0.7853982f),
                new Quaternion(-0.6508952f, 0.2762886f, 0.2762886f, 0.6508952f),
                new float4x4(0.6946584f, -0.7193397f, 0f, 0f, 0f, 1.043081E-07f, 0.9999999f, 0f, -0.7193397f, -0.6946583f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.8028516f, 0f)
            };
            // 396. - X: -540, Y: 1, Z: 45
            yield return new object[]
            {
                "X: -540, Y: 1, Z: 45",
                new float3(-9.424778f, 0.01745329f, 0.7853982f),
                new Quaternion(0.9238443f, -0.3826689f, -0.008062262f, 0.003339512f),
                new float4x4(0.7069991f, -0.7069991f, -0.0174524f, 0f, -0.7071068f, -0.7071068f, -2.374873E-08f, 0f, -0.0123407f, 0.01234073f, -0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.374873E-08f, 3.159046f, 3.926991f)
            };
            // 397. - X: 0, Y: 45, Z: 45
            yield return new object[]
            {
                "X: 0, Y: 45, Z: 45",
                new float3(0f, 0.7853982f, 0.7853982f),
                new Quaternion(0.1464466f, 0.3535534f, 0.3535534f, 0.8535534f),
                new float4x4(0.4999999f, -0.5f, 0.7071068f, 0f, 0.7071068f, 0.7071067f, 0f, 0f, -0.5f, 0.5000001f, 0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0.7853982f, 0.7853982f)
            };
            // 398. - X: 0.1, Y: 45, Z: 45
            yield return new object[]
            {
                "X: 0.1, Y: 45, Z: 45",
                new float3(0.001745329f, 0.7853982f, 0.7853982f),
                new Quaternion(0.1471914f, 0.3532447f, 0.3532447f, 0.8536808f),
                new float4x4(0.5008727f, -0.4991273f, 0.7071056f, 0f, 0.7071056f, 0.7071057f, -0.001745343f, 0f, -0.4991273f, 0.5008727f, 0.7071057f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0.7853982f, 0.7853982f)
            };
            // 399. - X: 1, Y: 45, Z: 45
            yield return new object[]
            {
                "X: 1, Y: 45, Z: 45",
                new float3(0.01745329f, 0.7853982f, 0.7853982f),
                new Quaternion(0.1538896f, 0.3504547f, 0.3504547f, 0.8547989f),
                new float4x4(0.5087261f, -0.4912738f, 0.7069991f, 0f, 0.7069991f, 0.706999f, -0.01745242f, 0f, -0.4912738f, 0.5087263f, 0.706999f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745331f, 0.7853982f, 0.7853982f)
            };
            // 400. - X: 45, Y: 45, Z: 45
            yield return new object[]
            {
                "X: 45, Y: 45, Z: 45",
                new float3(0.7853982f, 0.7853982f, 0.7853982f),
                new Quaternion(0.4619398f, 0.1913417f, 0.1913417f, 0.8446232f),
                new float4x4(0.8535534f, -0.1464466f, 0.5f, 0f, 0.5f, 0.4999999f, -0.7071069f, 0f, -0.1464466f, 0.8535535f, 0.4999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 0.7853982f, 0.7853982f)
            };
            // 401. - X: 90, Y: 45, Z: 45
            yield return new object[]
            {
                "X: 90, Y: 45, Z: 45",
                new float3(1.570796f, 0.7853982f, 0.7853982f),
                new Quaternion(0.7071068f, 0f, 0f, 0.7071068f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 5.960464E-08f, -0.9999999f, 0f, 0f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 0f, 0f)
            };
            // 402. - X: 180, Y: 45, Z: 45
            yield return new object[]
            {
                "X: 180, Y: 45, Z: 45",
                new float3(3.141593f, 0.7853982f, 0.7853982f),
                new Quaternion(0.8535534f, -0.3535534f, -0.3535534f, 0.1464466f),
                new float4x4(0.4999998f, -0.5000001f, -0.7071069f, 0f, -0.7071069f, -0.7071068f, 1.341105E-07f, 0f, -0.5000001f, 0.5000001f, -0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(-1.341105E-07f, 3.926991f, 3.926991f)
            };
            // 403. - X: 270, Y: 45, Z: 45
            yield return new object[]
            {
                "X: 270, Y: 45, Z: 45",
                new float3(4.712389f, 0.7853982f, 0.7853982f),
                new Quaternion(0.4999999f, -0.5f, -0.5f, -0.4999999f),
                new float4x4(0f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 404. - X: 360, Y: 45, Z: 45
            yield return new object[]
            {
                "X: 360, Y: 45, Z: 45",
                new float3(6.283185f, 0.7853982f, 0.7853982f),
                new Quaternion(-0.1464467f, -0.3535534f, -0.3535534f, -0.8535534f),
                new float4x4(0.5f, -0.4999999f, 0.7071068f, 0f, 0.7071068f, 0.7071067f, -1.639128E-07f, 0f, -0.4999999f, 0.5000001f, 0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.639128E-07f, 0.7853982f, 0.7853982f)
            };
            // 405. - X: 585, Y: 45, Z: 45
            yield return new object[]
            {
                "X: 585, Y: 45, Z: 45",
                new float3(10.21018f, 0.7853982f, 0.7853982f),
                new Quaternion(-0.7325377f, 0.4619398f, 0.4619398f, 0.1913418f),
                new float4x4(0.1464465f, -0.8535535f, -0.4999998f, 0f, -0.4999998f, -0.4999996f, 0.707107f, 0f, -0.8535535f, 0.1464465f, -0.4999996f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.926991f, 3.926991f)
            };
            // 406. - X: -90, Y: 45, Z: 45
            yield return new object[]
            {
                "X: -90, Y: 45, Z: 45",
                new float3(-1.570796f, 0.7853982f, 0.7853982f),
                new Quaternion(-0.4999999f, 0.5f, 0.5f, 0.4999999f),
                new float4x4(0f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 407. - X: -540, Y: 45, Z: 45
            yield return new object[]
            {
                "X: -540, Y: 45, Z: 45",
                new float3(-9.424778f, 0.7853982f, 0.7853982f),
                new Quaternion(0.8535534f, -0.3535534f, -0.3535534f, 0.1464466f),
                new float4x4(0.4999999f, -0.5f, -0.7071068f, 0f, -0.7071068f, -0.7071067f, -2.980232E-08f, 0f, -0.5f, 0.5000001f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.980232E-08f, 3.926991f, 3.926991f)
            };
            // 408. - X: 0, Y: 90, Z: 45
            yield return new object[]
            {
                "X: 0, Y: 90, Z: 45",
                new float3(0f, 1.570796f, 0.7853982f),
                new Quaternion(0.2705981f, 0.6532815f, 0.2705981f, 0.6532815f),
                new float4x4(8.940697E-08f, 0f, 0.9999999f, 0f, 0.7071068f, 0.7071068f, 0f, 0f, -0.7071067f, 0.7071068f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.570796f, 0.7853982f)
            };
            // 409. - X: 0.1, Y: 90, Z: 45
            yield return new object[]
            {
                "X: 0.1, Y: 90, Z: 45",
                new float3(0.001745329f, 1.570796f, 0.7853982f),
                new Quaternion(0.2711681f, 0.6530451f, 0.2700279f, 0.6535174f),
                new float4x4(0.001234218f, 0.001234084f, 0.9999985f, 0f, 0.7071058f, 0.7071056f, -0.001745313f, 0f, -0.7071067f, 0.7071068f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745314f, 1.570796f, 0.7853982f)
            };
            // 410. - X: 1, Y: 90, Z: 45
            yield return new object[]
            {
                "X: 1, Y: 90, Z: 45",
                new float3(0.01745329f, 1.570796f, 0.7853982f),
                new Quaternion(0.2762886f, 0.6508952f, 0.2648869f, 0.6556179f),
                new float4x4(0.01234083f, 0.01234075f, 0.9998475f, 0f, 0.7069989f, 0.7069991f, -0.01745239f, 0f, -0.7071066f, 0.7071067f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745328f, 1.570796f, 0.7853983f)
            };
            // 411. - X: 45, Y: 90, Z: 45
            yield return new object[]
            {
                "X: 45, Y: 90, Z: 45",
                new float3(0.7853982f, 1.570796f, 0.7853982f),
                new Quaternion(0.5f, 0.4999999f, 0f, 0.7071068f),
                new float4x4(0.5000001f, 0.4999999f, 0.7071067f, 0f, 0.4999999f, 0.5f, -0.7071068f, 0f, -0.7071067f, 0.7071068f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 1.570796f, 0.7853982f)
            };
            // 412. - X: 90, Y: 90, Z: 45
            yield return new object[]
            {
                "X: 90, Y: 90, Z: 45",
                new float3(1.570796f, 1.570796f, 0.7853982f),
                new Quaternion(0.6532815f, 0.270598f, -0.270598f, 0.6532815f),
                new float4x4(0.7071069f, 0.7071066f, 0f, 0f, 0f, 1.490116E-07f, -0.9999999f, 0f, -0.7071066f, 0.7071067f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 0.7853981f, 0f)
            };
            // 413. - X: 180, Y: 90, Z: 45
            yield return new object[]
            {
                "X: 180, Y: 90, Z: 45",
                new float3(3.141593f, 1.570796f, 0.7853982f),
                new Quaternion(0.6532815f, -0.2705981f, -0.6532815f, 0.270598f),
                new float4x4(5.960464E-08f, -8.940697E-08f, -0.9999999f, 0f, -0.7071067f, -0.7071066f, 8.940697E-08f, 0f, -0.7071067f, 0.7071067f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(-5.960464E-08f, 4.712389f, 3.926991f)
            };
            // 414. - X: 270, Y: 90, Z: 45
            yield return new object[]
            {
                "X: 270, Y: 90, Z: 45",
                new float3(4.712389f, 1.570796f, 0.7853982f),
                new Quaternion(0.270598f, -0.6532815f, -0.6532815f, -0.270598f),
                new float4x4(-0.7071066f, -0.7071066f, 0f, 0f, 0f, 1.490116E-07f, 0.9999999f, 0f, -0.7071066f, 0.7071067f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 2.356194f, 0f)
            };
            // 415. - X: 360, Y: 90, Z: 45
            yield return new object[]
            {
                "X: 360, Y: 90, Z: 45",
                new float3(6.283185f, 1.570796f, 0.7853982f),
                new Quaternion(-0.2705981f, -0.6532815f, -0.270598f, -0.6532815f),
                new float4x4(1.490116E-07f, 1.490116E-07f, 0.9999999f, 0f, 0.7071067f, 0.7071068f, -1.490116E-07f, 0f, -0.7071067f, 0.7071067f, 2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.490116E-07f, 1.570796f, 0.7853982f)
            };
            // 416. - X: 585, Y: 90, Z: 45
            yield return new object[]
            {
                "X: 585, Y: 90, Z: 45",
                new float3(10.21018f, 1.570796f, 0.7853982f),
                new Quaternion(-0.4999999f, 0.5000001f, 0.7071067f, 1.341105E-07f),
                new float4x4(-0.5000001f, -0.5000002f, -0.7071064f, 0f, -0.4999998f, -0.4999995f, 0.707107f, 0f, -0.7071066f, 0.7071068f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 4.712389f, 3.926991f)
            };
            // 417. - X: -90, Y: 90, Z: 45
            yield return new object[]
            {
                "X: -90, Y: 90, Z: 45",
                new float3(-1.570796f, 1.570796f, 0.7853982f),
                new Quaternion(-0.270598f, 0.6532815f, 0.6532815f, 0.270598f),
                new float4x4(-0.7071066f, -0.7071066f, 0f, 0f, 0f, 1.490116E-07f, 0.9999999f, 0f, -0.7071066f, 0.7071067f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 2.356194f, 0f)
            };
            // 418. - X: -540, Y: 90, Z: 45
            yield return new object[]
            {
                "X: -540, Y: 90, Z: 45",
                new float3(-9.424778f, 1.570796f, 0.7853982f),
                new Quaternion(0.6532815f, -0.2705981f, -0.6532815f, 0.2705981f),
                new float4x4(8.940697E-08f, 0f, -0.9999999f, 0f, -0.7071068f, -0.7071066f, 0f, 0f, -0.7071067f, 0.7071068f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 3.926991f)
            };
            // 419. - X: 0, Y: 180, Z: 45
            yield return new object[]
            {
                "X: 0, Y: 180, Z: 45",
                new float3(0f, 3.141593f, 0.7853982f),
                new Quaternion(0.3826835f, 0.9238795f, -1.672763E-08f, -4.038406E-08f),
                new float4x4(-0.7071067f, 0.7071068f, -8.742278E-08f, 0f, 0.7071068f, 0.7071067f, 0f, 0f, 6.181723E-08f, -6.181724E-08f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.141593f, 0.7853982f)
            };
            // 420. - X: 0.1, Y: 180, Z: 45
            yield return new object[]
            {
                "X: 0.1, Y: 180, Z: 45",
                new float3(0.001745329f, 3.141593f, 0.7853982f),
                new Quaternion(0.3826833f, 0.9238791f, -0.0008062536f, 0.0003339139f),
                new float4x4(-0.7071067f, 0.7071068f, -8.742791E-08f, 0f, 0.7071057f, 0.7071057f, -0.001745328f, 0f, -0.001234072f, -0.001234195f, -0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.141593f, 0.7853982f)
            };
            // 421. - X: 1, Y: 180, Z: 45
            yield return new object[]
            {
                "X: 1, Y: 180, Z: 45",
                new float3(0.01745329f, 3.141593f, 0.7853982f),
                new Quaternion(0.3826689f, 0.9238443f, -0.008062284f, 0.00333946f),
                new float4x4(-0.7071068f, 0.7071068f, -8.707866E-08f, 0f, 0.7069991f, 0.7069991f, -0.01745241f, 0f, -0.01234065f, -0.01234077f, -0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.141593f, 0.7853982f)
            };
            // 422. - X: 45, Y: 180, Z: 45
            yield return new object[]
            {
                "X: 45, Y: 180, Z: 45",
                new float3(0.7853982f, 3.141593f, 0.7853982f),
                new Quaternion(0.3535534f, 0.8535534f, -0.3535534f, 0.1464466f),
                new float4x4(-0.7071068f, 0.7071068f, -7.450581E-08f, 0f, 0.5f, 0.4999999f, -0.7071068f, 0f, -0.5f, -0.5000001f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 3.141593f, 0.7853982f)
            };
            // 423. - X: 90, Y: 180, Z: 45
            yield return new object[]
            {
                "X: 90, Y: 180, Z: 45",
                new float3(1.570796f, 3.141593f, 0.7853982f),
                new Quaternion(0.270598f, 0.6532815f, -0.6532815f, 0.270598f),
                new float4x4(-0.7071066f, 0.7071066f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, -0.7071066f, -0.7071067f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 2.356194f, 0f)
            };
            // 424. - X: 180, Y: 180, Z: 45
            yield return new object[]
            {
                "X: 180, Y: 180, Z: 45",
                new float3(3.141593f, 3.141593f, 0.7853982f),
                new Quaternion(-5.711168E-08f, -2.365643E-08f, -0.9238795f, 0.3826835f),
                new float4x4(-0.7071067f, 0.7071068f, 8.742277E-08f, 0f, -0.7071068f, -0.7071067f, 8.742278E-08f, 0f, 1.236345E-07f, -1.065814E-14f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 8.742277E-08f, 3.926991f)
            };
            // 425. - X: 270, Y: 180, Z: 45
            yield return new object[]
            {
                "X: 270, Y: 180, Z: 45",
                new float3(4.712389f, 3.141593f, 0.7853982f),
                new Quaternion(-0.2705981f, -0.6532815f, -0.6532815f, 0.2705981f),
                new float4x4(-0.7071066f, 0.7071068f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, 0.7071068f, 0.7071066f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 3.926991f, 0f)
            };
            // 426. - X: 360, Y: 180, Z: 45
            yield return new object[]
            {
                "X: 360, Y: 180, Z: 45",
                new float3(6.283185f, 3.141593f, 0.7853982f),
                new Quaternion(-0.3826835f, -0.9238795f, 9.749574E-08f, 6.928804E-09f),
                new float4x4(-0.7071067f, 0.7071068f, -8.742277E-08f, 0f, 0.7071068f, 0.7071067f, -1.748455E-07f, 0f, -6.181725E-08f, -1.854517E-07f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748455E-07f, 3.141593f, 0.7853982f)
            };
            // 427. - X: 585, Y: 180, Z: 45
            yield return new object[]
            {
                "X: 585, Y: 180, Z: 45",
                new float3(10.21018f, 3.141593f, 0.7853982f),
                new Quaternion(0.1464467f, 0.3535535f, 0.8535533f, -0.3535534f),
                new float4x4(-0.7071066f, 0.7071069f, 8.940697E-08f, 0f, -0.4999999f, -0.4999998f, 0.707107f, 0f, 0.5000002f, 0.5000001f, 0.7071066f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.264406E-07f, 3.926991f)
            };
            // 428. - X: -90, Y: 180, Z: 45
            yield return new object[]
            {
                "X: -90, Y: 180, Z: 45",
                new float3(-1.570796f, 3.141593f, 0.7853982f),
                new Quaternion(0.2705981f, 0.6532815f, 0.6532815f, -0.2705981f),
                new float4x4(-0.7071066f, 0.7071068f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, 0.7071068f, 0.7071066f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 3.926991f, 0f)
            };
            // 429. - X: -540, Y: 180, Z: 45
            yield return new object[]
            {
                "X: -540, Y: 180, Z: 45",
                new float3(-9.424778f, 3.141593f, 0.7853982f),
                new Quaternion(-3.58206E-08f, 2.774478E-08f, -0.9238795f, 0.3826835f),
                new float4x4(-0.7071067f, 0.7071068f, 8.742278E-08f, 0f, -0.7071068f, -0.7071067f, -2.384976E-08f, 0f, 4.49529E-08f, -7.868157E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 8.742278E-08f, 3.926991f)
            };
            // 430. - X: 0, Y: 270, Z: 45
            yield return new object[]
            {
                "X: 0, Y: 270, Z: 45",
                new float3(0f, 4.712389f, 0.7853982f),
                new Quaternion(0.2705981f, 0.6532815f, -0.2705981f, -0.6532815f),
                new float4x4(8.940697E-08f, 0f, -0.9999999f, 0f, 0.7071068f, 0.7071068f, 0f, 0f, 0.7071067f, -0.7071068f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 0.7853982f)
            };
            // 431. - X: 0.1, Y: 270, Z: 45
            yield return new object[]
            {
                "X: 0.1, Y: 270, Z: 45",
                new float3(0.001745329f, 4.712389f, 0.7853982f),
                new Quaternion(0.2700279f, 0.6535174f, -0.2711681f, -0.6530451f),
                new float4x4(-0.001234129f, -0.001234084f, -0.9999985f, 0f, 0.7071058f, 0.7071056f, -0.001745313f, 0f, 0.7071067f, -0.7071068f, -1.490116E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745344f, 4.712389f, 0.7853982f)
            };
            // 432. - X: 1, Y: 270, Z: 45
            yield return new object[]
            {
                "X: 1, Y: 270, Z: 45",
                new float3(0.01745329f, 4.712389f, 0.7853982f),
                new Quaternion(0.2648869f, 0.6556179f, -0.2762886f, -0.6508952f),
                new float4x4(-0.01234044f, -0.01234075f, -0.9998475f, 0f, 0.7069989f, 0.7069991f, -0.01745239f, 0f, 0.7071066f, -0.7071067f, 2.831221E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745328f, 4.712389f, 0.7853983f)
            };
            // 433. - X: 45, Y: 270, Z: 45
            yield return new object[]
            {
                "X: 45, Y: 270, Z: 45",
                new float3(0.7853982f, 4.712389f, 0.7853982f),
                new Quaternion(0f, 0.7071068f, -0.5f, -0.4999999f),
                new float4x4(-0.4999999f, -0.4999999f, -0.7071067f, 0f, 0.4999999f, 0.5f, -0.7071068f, 0f, 0.7071067f, -0.7071068f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 4.712389f, 0.7853982f)
            };
            // 434. - X: 90, Y: 270, Z: 45
            yield return new object[]
            {
                "X: 90, Y: 270, Z: 45",
                new float3(1.570796f, 4.712389f, 0.7853982f),
                new Quaternion(-0.270598f, 0.6532815f, -0.6532815f, -0.270598f),
                new float4x4(-0.7071066f, -0.7071066f, 0f, 0f, 0f, 1.490116E-07f, -0.9999999f, 0f, 0.7071066f, -0.7071067f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 3.926991f, 0f)
            };
            // 435. - X: 180, Y: 270, Z: 45
            yield return new object[]
            {
                "X: 180, Y: 270, Z: 45",
                new float3(3.141593f, 4.712389f, 0.7853982f),
                new Quaternion(-0.6532815f, 0.270598f, -0.6532815f, 0.2705981f),
                new float4x4(1.192093E-07f, 8.940697E-08f, 0.9999999f, 0f, -0.7071067f, -0.7071066f, 8.940697E-08f, 0f, 0.7071067f, -0.7071067f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(-5.960464E-08f, 1.570796f, 3.926991f)
            };
            // 436. - X: 270, Y: 270, Z: 45
            yield return new object[]
            {
                "X: 270, Y: 270, Z: 45",
                new float3(4.712389f, 4.712389f, 0.7853982f),
                new Quaternion(-0.6532815f, -0.270598f, -0.270598f, 0.6532815f),
                new float4x4(0.7071069f, 0.7071066f, 0f, 0f, 0f, 1.490116E-07f, 0.9999999f, 0f, 0.7071066f, -0.7071067f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 5.497787f, 0f)
            };
            // 437. - X: 360, Y: 270, Z: 45
            yield return new object[]
            {
                "X: 360, Y: 270, Z: 45",
                new float3(6.283185f, 4.712389f, 0.7853982f),
                new Quaternion(-0.270598f, -0.6532815f, 0.2705981f, 0.6532815f),
                new float4x4(2.980232E-08f, -1.490116E-07f, -0.9999999f, 0f, 0.7071067f, 0.7071068f, -1.490116E-07f, 0f, 0.7071067f, -0.7071067f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.490116E-07f, 4.712389f, 0.7853982f)
            };
            // 438. - X: 585, Y: 270, Z: 45
            yield return new object[]
            {
                "X: 585, Y: 270, Z: 45",
                new float3(10.21018f, 4.712389f, 0.7853982f),
                new Quaternion(0.7071067f, 1.341105E-07f, 0.4999999f, -0.5000001f),
                new float4x4(0.5000003f, 0.5000002f, 0.7071064f, 0f, -0.4999998f, -0.4999995f, 0.707107f, 0f, 0.7071066f, -0.7071068f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.570796f, 3.926991f)
            };
            // 439. - X: -90, Y: 270, Z: 45
            yield return new object[]
            {
                "X: -90, Y: 270, Z: 45",
                new float3(-1.570796f, 4.712389f, 0.7853982f),
                new Quaternion(0.6532815f, 0.270598f, 0.270598f, -0.6532815f),
                new float4x4(0.7071069f, 0.7071066f, 0f, 0f, 0f, 1.490116E-07f, 0.9999999f, 0f, 0.7071066f, -0.7071067f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 5.497787f, 0f)
            };
            // 440. - X: -540, Y: 270, Z: 45
            yield return new object[]
            {
                "X: -540, Y: 270, Z: 45",
                new float3(-9.424778f, 4.712389f, 0.7853982f),
                new Quaternion(-0.6532815f, 0.2705981f, -0.6532815f, 0.2705981f),
                new float4x4(8.940697E-08f, 0f, 0.9999999f, 0f, -0.7071068f, -0.7071066f, 0f, 0f, 0.7071067f, -0.7071068f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.570796f, 3.926991f)
            };
            // 441. - X: 0, Y: 360, Z: 45
            yield return new object[]
            {
                "X: 0, Y: 360, Z: 45",
                new float3(0f, 6.283185f, 0.7853982f),
                new Quaternion(-3.345525E-08f, -8.076811E-08f, -0.3826835f, -0.9238795f),
                new float4x4(0.7071067f, -0.7071068f, 1.748456E-07f, 0f, 0.7071068f, 0.7071067f, 0f, 0f, -1.236345E-07f, 1.236345E-07f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.748456E-07f, 0.7853982f)
            };
            // 442. - X: 0.1, Y: 360, Z: 45
            yield return new object[]
            {
                "X: 0.1, Y: 360, Z: 45",
                new float3(0.001745329f, 6.283185f, 0.7853982f),
                new Quaternion(-0.0008062703f, 0.0003338735f, -0.3826833f, -0.9238791f),
                new float4x4(0.7071068f, -0.7071068f, 1.74914E-07f, 0f, 0.7071057f, 0.7071057f, -0.001745328f, 0f, 0.00123401f, 0.001234257f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 1.749143E-07f, 0.7853982f)
            };
            // 443. - X: 1, Y: 360, Z: 45
            yield return new object[]
            {
                "X: 1, Y: 360, Z: 45",
                new float3(0.01745329f, 6.283185f, 0.7853982f),
                new Quaternion(-0.0080623f, 0.00333942f, -0.3826689f, -0.9238443f),
                new float4x4(0.7071068f, -0.7071068f, 1.74623E-07f, 0f, 0.7069991f, 0.7069991f, -0.01745241f, 0f, 0.01234059f, 0.01234084f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 1.746496E-07f, 0.7853982f)
            };
            // 444. - X: 45, Y: 360, Z: 45
            yield return new object[]
            {
                "X: 45, Y: 360, Z: 45",
                new float3(0.7853982f, 6.283185f, 0.7853982f),
                new Quaternion(-0.3535534f, 0.1464466f, -0.3535534f, -0.8535534f),
                new float4x4(0.7071068f, -0.7071067f, 1.341105E-07f, 0f, 0.5f, 0.4999999f, -0.7071068f, 0f, 0.4999999f, 0.5000001f, 0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 1.896608E-07f, 0.7853982f)
            };
            // 445. - X: 90, Y: 360, Z: 45
            yield return new object[]
            {
                "X: 90, Y: 360, Z: 45",
                new float3(1.570796f, 6.283185f, 0.7853982f),
                new Quaternion(-0.6532815f, 0.270598f, -0.270598f, -0.6532815f),
                new float4x4(0.7071069f, -0.7071066f, 0f, 0f, 0f, 1.490116E-07f, -0.9999999f, 0f, 0.7071066f, 0.7071067f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 5.497787f, 0f)
            };
            // 446. - X: 180, Y: 360, Z: 45
            yield return new object[]
            {
                "X: 180, Y: 360, Z: 45",
                new float3(3.141593f, 6.283185f, 0.7853982f),
                new Quaternion(-0.9238795f, 0.3826835f, 9.749574E-08f, 6.928804E-09f),
                new float4x4(0.7071067f, -0.7071068f, -1.748455E-07f, 0f, -0.7071068f, -0.7071067f, 8.742277E-08f, 0f, -1.854517E-07f, 6.181725E-08f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742277E-08f, 3.141593f, 3.926991f)
            };
            // 447. - X: 270, Y: 360, Z: 45
            yield return new object[]
            {
                "X: 270, Y: 360, Z: 45",
                new float3(4.712389f, 6.283185f, 0.7853982f),
                new Quaternion(-0.6532815f, 0.2705981f, 0.2705981f, 0.6532815f),
                new float4x4(0.7071066f, -0.7071069f, 0f, 0f, 0f, 2.980232E-08f, 1f, 0f, -0.7071069f, -0.7071066f, 2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.7853984f, 0f)
            };
            // 448. - X: 360, Y: 360, Z: 45
            yield return new object[]
            {
                "X: 360, Y: 360, Z: 45",
                new float3(6.283185f, 6.283185f, 0.7853982f),
                new Quaternion(1.142234E-07f, 4.731286E-08f, 0.3826835f, 0.9238795f),
                new float4x4(0.7071067f, -0.7071068f, 1.748456E-07f, 0f, 0.7071068f, 0.7071067f, -1.748455E-07f, 0f, 2.131628E-14f, 2.472689E-07f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748455E-07f, 1.748456E-07f, 0.7853982f)
            };
            // 449. - X: 585, Y: 360, Z: 45
            yield return new object[]
            {
                "X: 585, Y: 360, Z: 45",
                new float3(10.21018f, 6.283185f, 0.7853982f),
                new Quaternion(0.8535533f, -0.3535534f, -0.1464468f, -0.3535535f),
                new float4x4(0.7071066f, -0.7071069f, -1.490116E-07f, 0f, -0.4999998f, -0.4999998f, 0.7071071f, 0f, -0.5000004f, -0.5f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.141593f, 3.926991f)
            };
            // 450. - X: -90, Y: 360, Z: 45
            yield return new object[]
            {
                "X: -90, Y: 360, Z: 45",
                new float3(-1.570796f, 6.283185f, 0.7853982f),
                new Quaternion(0.6532815f, -0.2705981f, -0.2705981f, -0.6532815f),
                new float4x4(0.7071066f, -0.7071069f, 0f, 0f, 0f, 2.980232E-08f, 1f, 0f, -0.7071069f, -0.7071066f, 2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.7853984f, 0f)
            };
            // 451. - X: -540, Y: 360, Z: 45
            yield return new object[]
            {
                "X: -540, Y: 360, Z: 45",
                new float3(-9.424778f, 6.283185f, 0.7853982f),
                new Quaternion(-0.9238795f, 0.3826835f, 7.620466E-08f, -4.44724E-08f),
                new float4x4(0.7071067f, -0.7071068f, -1.748456E-07f, 0f, -0.7071068f, -0.7071067f, -2.384976E-08f, 0f, -1.067701E-07f, 1.404988E-07f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.141593f, 3.926991f)
            };
            // 452. - X: 0, Y: 585, Z: 45
            yield return new object[]
            {
                "X: 0, Y: 585, Z: 45",
                new float3(0f, 10.21018f, 0.7853982f),
                new Quaternion(-0.3535534f, -0.8535533f, 0.1464467f, 0.3535535f),
                new float4x4(-0.4999997f, 0.4999998f, -0.7071071f, 0f, 0.7071068f, 0.7071068f, -2.980232E-08f, 0f, 0.5000001f, -0.5000002f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.980232E-08f, 3.926991f, 0.7853982f)
            };
            // 453. - X: 0.1, Y: 585, Z: 45
            yield return new object[]
            {
                "X: 0.1, Y: 585, Z: 45",
                new float3(0.001745329f, 10.21018f, 0.7853982f),
                new Quaternion(-0.3532447f, -0.8536807f, 0.1471915f, 0.3532449f),
                new float4x4(-0.5008723f, 0.4991271f, -0.7071059f, 0f, 0.7071057f, 0.7071057f, -0.001745343f, 0f, 0.4991274f, -0.5008729f, -0.7071052f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.926991f, 0.7853982f)
            };
            // 454. - X: 1, Y: 585, Z: 45
            yield return new object[]
            {
                "X: 1, Y: 585, Z: 45",
                new float3(0.01745329f, 10.21018f, 0.7853982f),
                new Quaternion(-0.3504546f, -0.8547988f, 0.1538897f, 0.3504548f),
                new float4x4(-0.5087261f, 0.4912737f, -0.7069994f, 0f, 0.7069992f, 0.7069991f, -0.01745242f, 0f, 0.4912739f, -0.5087264f, -0.7069989f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745331f, 3.926991f, 0.7853983f)
            };
            // 455. - X: 45, Y: 585, Z: 45
            yield return new object[]
            {
                "X: 45, Y: 585, Z: 45",
                new float3(0.7853982f, 10.21018f, 0.7853982f),
                new Quaternion(-0.1913416f, -0.8446231f, 0.4619398f, 0.1913418f),
                new float4x4(-0.8535533f, 0.1464463f, -0.5000001f, 0f, 0.5f, 0.5f, -0.7071068f, 0f, 0.1464469f, -0.8535534f, -0.4999998f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 3.926991f, 0.7853982f)
            };
            // 456. - X: 90, Y: 585, Z: 45
            yield return new object[]
            {
                "X: 90, Y: 585, Z: 45",
                new float3(1.570796f, 10.21018f, 0.7853982f),
                new Quaternion(1.341105E-07f, -0.7071067f, 0.7071067f, 1.341105E-07f),
                new float4x4(-0.9999996f, -3.793216E-07f, 0f, 0f, 0f, 1.788139E-07f, -0.9999998f, 0f, 3.793216E-07f, -0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570451f, 3.141593f, 0f)
            };
            // 457. - X: 180, Y: 585, Z: 45
            yield return new object[]
            {
                "X: 180, Y: 585, Z: 45",
                new float3(3.141593f, 10.21018f, 0.7853982f),
                new Quaternion(0.3535536f, -0.1464466f, 0.8535533f, -0.3535534f),
                new float4x4(-0.4999997f, 0.4999999f, 0.707107f, 0f, -0.7071068f, -0.7071067f, 1.192093E-07f, 0f, 0.5000002f, -0.5000002f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(-1.192093E-07f, 0.7853985f, 3.926991f)
            };
            // 458. - X: 270, Y: 585, Z: 45
            yield return new object[]
            {
                "X: 270, Y: 585, Z: 45",
                new float3(4.712389f, 10.21018f, 0.7853982f),
                new Quaternion(0.5000001f, 0.4999999f, 0.4999999f, -0.5000001f),
                new float4x4(5.960464E-07f, 0.9999999f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, 0.9999999f, -5.364418E-07f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 459. - X: 360, Y: 585, Z: 45
            yield return new object[]
            {
                "X: 360, Y: 585, Z: 45",
                new float3(6.283185f, 10.21018f, 0.7853982f),
                new Quaternion(0.3535534f, 0.8535533f, -0.1464468f, -0.3535535f),
                new float4x4(-0.4999998f, 0.4999997f, -0.707107f, 0f, 0.7071068f, 0.7071067f, -1.788139E-07f, 0f, 0.5000001f, -0.5000003f, -0.7071064f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.788139E-07f, 3.926991f, 0.7853982f)
            };
            // 460. - X: 585, Y: 585, Z: 45
            yield return new object[]
            {
                "X: 585, Y: 585, Z: 45",
                new float3(10.21018f, 10.21018f, 0.7853982f),
                new Quaternion(-0.4619399f, -0.1913418f, -0.7325376f, 0.4619398f),
                new float4x4(-0.1464461f, 0.8535534f, 0.5f, 0f, -0.4999998f, -0.4999998f, 0.7071069f, 0f, 0.8535535f, -0.1464468f, 0.4999996f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 0.7853987f, 3.926991f)
            };
            // 461. - X: -90, Y: 585, Z: 45
            yield return new object[]
            {
                "X: -90, Y: 585, Z: 45",
                new float3(-1.570796f, 10.21018f, 0.7853982f),
                new Quaternion(-0.5000001f, -0.4999999f, -0.4999999f, 0.5000001f),
                new float4x4(5.960464E-07f, 0.9999999f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, 0.9999999f, -5.364418E-07f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 462. - X: -540, Y: 585, Z: 45
            yield return new object[]
            {
                "X: -540, Y: 585, Z: 45",
                new float3(-9.424778f, 10.21018f, 0.7853982f),
                new Quaternion(0.3535535f, -0.1464467f, 0.8535533f, -0.3535534f),
                new float4x4(-0.4999998f, 0.4999998f, 0.7071071f, 0f, -0.7071068f, -0.7071067f, -5.960464E-08f, 0f, 0.5000001f, -0.5000002f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.960464E-08f, 0.7853986f, 3.926991f)
            };
            // 463. - X: 0, Y: -90, Z: 45
            yield return new object[]
            {
                "X: 0, Y: -90, Z: 45",
                new float3(0f, -1.570796f, 0.7853982f),
                new Quaternion(-0.2705981f, -0.6532815f, 0.2705981f, 0.6532815f),
                new float4x4(8.940697E-08f, 0f, -0.9999999f, 0f, 0.7071068f, 0.7071068f, 0f, 0f, 0.7071067f, -0.7071068f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 0.7853982f)
            };
            // 464. - X: 0.1, Y: -90, Z: 45
            yield return new object[]
            {
                "X: 0.1, Y: -90, Z: 45",
                new float3(0.001745329f, -1.570796f, 0.7853982f),
                new Quaternion(-0.2700279f, -0.6535174f, 0.2711681f, 0.6530451f),
                new float4x4(-0.001234129f, -0.001234084f, -0.9999985f, 0f, 0.7071058f, 0.7071056f, -0.001745313f, 0f, 0.7071067f, -0.7071068f, -1.490116E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745344f, 4.712389f, 0.7853982f)
            };
            // 465. - X: 1, Y: -90, Z: 45
            yield return new object[]
            {
                "X: 1, Y: -90, Z: 45",
                new float3(0.01745329f, -1.570796f, 0.7853982f),
                new Quaternion(-0.2648869f, -0.6556179f, 0.2762886f, 0.6508952f),
                new float4x4(-0.01234044f, -0.01234075f, -0.9998475f, 0f, 0.7069989f, 0.7069991f, -0.01745239f, 0f, 0.7071066f, -0.7071067f, 2.831221E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745328f, 4.712389f, 0.7853983f)
            };
            // 466. - X: 45, Y: -90, Z: 45
            yield return new object[]
            {
                "X: 45, Y: -90, Z: 45",
                new float3(0.7853982f, -1.570796f, 0.7853982f),
                new Quaternion(0f, -0.7071068f, 0.5f, 0.4999999f),
                new float4x4(-0.4999999f, -0.4999999f, -0.7071067f, 0f, 0.4999999f, 0.5f, -0.7071068f, 0f, 0.7071067f, -0.7071068f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 4.712389f, 0.7853982f)
            };
            // 467. - X: 90, Y: -90, Z: 45
            yield return new object[]
            {
                "X: 90, Y: -90, Z: 45",
                new float3(1.570796f, -1.570796f, 0.7853982f),
                new Quaternion(0.270598f, -0.6532815f, 0.6532815f, 0.270598f),
                new float4x4(-0.7071066f, -0.7071066f, 0f, 0f, 0f, 1.490116E-07f, -0.9999999f, 0f, 0.7071066f, -0.7071067f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 3.926991f, 0f)
            };
            // 468. - X: 180, Y: -90, Z: 45
            yield return new object[]
            {
                "X: 180, Y: -90, Z: 45",
                new float3(3.141593f, -1.570796f, 0.7853982f),
                new Quaternion(0.6532815f, -0.270598f, 0.6532815f, -0.2705981f),
                new float4x4(1.192093E-07f, 8.940697E-08f, 0.9999999f, 0f, -0.7071067f, -0.7071066f, 8.940697E-08f, 0f, 0.7071067f, -0.7071067f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(-5.960464E-08f, 1.570796f, 3.926991f)
            };
            // 469. - X: 270, Y: -90, Z: 45
            yield return new object[]
            {
                "X: 270, Y: -90, Z: 45",
                new float3(4.712389f, -1.570796f, 0.7853982f),
                new Quaternion(0.6532815f, 0.270598f, 0.270598f, -0.6532815f),
                new float4x4(0.7071069f, 0.7071066f, 0f, 0f, 0f, 1.490116E-07f, 0.9999999f, 0f, 0.7071066f, -0.7071067f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 5.497787f, 0f)
            };
            // 470. - X: 360, Y: -90, Z: 45
            yield return new object[]
            {
                "X: 360, Y: -90, Z: 45",
                new float3(6.283185f, -1.570796f, 0.7853982f),
                new Quaternion(0.270598f, 0.6532815f, -0.2705981f, -0.6532815f),
                new float4x4(2.980232E-08f, -1.490116E-07f, -0.9999999f, 0f, 0.7071067f, 0.7071068f, -1.490116E-07f, 0f, 0.7071067f, -0.7071067f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.490116E-07f, 4.712389f, 0.7853982f)
            };
            // 471. - X: 585, Y: -90, Z: 45
            yield return new object[]
            {
                "X: 585, Y: -90, Z: 45",
                new float3(10.21018f, -1.570796f, 0.7853982f),
                new Quaternion(-0.7071067f, -1.341105E-07f, -0.4999999f, 0.5000001f),
                new float4x4(0.5000003f, 0.5000002f, 0.7071064f, 0f, -0.4999998f, -0.4999995f, 0.707107f, 0f, 0.7071066f, -0.7071068f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.570796f, 3.926991f)
            };
            // 472. - X: -90, Y: -90, Z: 45
            yield return new object[]
            {
                "X: -90, Y: -90, Z: 45",
                new float3(-1.570796f, -1.570796f, 0.7853982f),
                new Quaternion(-0.6532815f, -0.270598f, -0.270598f, 0.6532815f),
                new float4x4(0.7071069f, 0.7071066f, 0f, 0f, 0f, 1.490116E-07f, 0.9999999f, 0f, 0.7071066f, -0.7071067f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 5.497787f, 0f)
            };
            // 473. - X: -540, Y: -90, Z: 45
            yield return new object[]
            {
                "X: -540, Y: -90, Z: 45",
                new float3(-9.424778f, -1.570796f, 0.7853982f),
                new Quaternion(0.6532815f, -0.2705981f, 0.6532815f, -0.2705981f),
                new float4x4(8.940697E-08f, 0f, 0.9999999f, 0f, -0.7071068f, -0.7071066f, 0f, 0f, 0.7071067f, -0.7071068f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.570796f, 3.926991f)
            };
            // 474. - X: 0, Y: -540, Z: 45
            yield return new object[]
            {
                "X: 0, Y: -540, Z: 45",
                new float3(0f, -9.424778f, 0.7853982f),
                new Quaternion(0.3826835f, 0.9238795f, 4.563455E-09f, 1.101715E-08f),
                new float4x4(-0.7071067f, 0.7071068f, 2.384976E-08f, 0f, 0.7071068f, 0.7071067f, 0f, 0f, -1.686433E-08f, 1.686433E-08f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.141593f, 0.7853982f)
            };
            // 475. - X: 0.1, Y: -540, Z: 45
            yield return new object[]
            {
                "X: 0.1, Y: -540, Z: 45",
                new float3(0.001745329f, -9.424778f, 0.7853982f),
                new Quaternion(0.3826833f, 0.9238791f, -0.0008062323f, 0.0003339653f),
                new float4x4(-0.7071067f, 0.7071068f, 2.380693E-08f, 0f, 0.7071057f, 0.7071057f, -0.001745328f, 0f, -0.00123415f, -0.001234117f, -0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.141593f, 0.7853982f)
            };
            // 476. - X: 1, Y: -540, Z: 45
            yield return new object[]
            {
                "X: 1, Y: -540, Z: 45",
                new float3(0.01745329f, -9.424778f, 0.7853982f),
                new Quaternion(0.3826689f, 0.9238443f, -0.008062262f, 0.003339512f),
                new float4x4(-0.7071068f, 0.7071068f, 2.374873E-08f, 0f, 0.7069991f, 0.7069991f, -0.0174524f, 0f, -0.01234073f, -0.0123407f, -0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.141593f, 0.7853982f)
            };
            // 477. - X: 45, Y: -540, Z: 45
            yield return new object[]
            {
                "X: 45, Y: -540, Z: 45",
                new float3(0.7853982f, -9.424778f, 0.7853982f),
                new Quaternion(0.3535534f, 0.8535534f, -0.3535534f, 0.1464466f),
                new float4x4(-0.7071067f, 0.7071068f, 2.980232E-08f, 0f, 0.5f, 0.4999999f, -0.7071068f, 0f, -0.5000001f, -0.5f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 3.141593f, 0.7853982f)
            };
            // 478. - X: 90, Y: -540, Z: 45
            yield return new object[]
            {
                "X: 90, Y: -540, Z: 45",
                new float3(1.570796f, -9.424778f, 0.7853982f),
                new Quaternion(0.2705981f, 0.6532815f, -0.6532815f, 0.2705981f),
                new float4x4(-0.7071066f, 0.7071068f, 0f, 0f, 0f, 8.940697E-08f, -0.9999999f, 0f, -0.7071068f, -0.7071067f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 2.356194f, 0f)
            };
            // 479. - X: 180, Y: -540, Z: 45
            yield return new object[]
            {
                "X: 180, Y: -540, Z: 45",
                new float3(3.141593f, -9.424778f, 0.7853982f),
                new Quaternion(-5.710473E-09f, -4.494751E-08f, -0.9238795f, 0.3826835f),
                new float4x4(-0.7071067f, 0.7071068f, -2.384976E-08f, 0f, -0.7071068f, -0.7071067f, 8.742277E-08f, 0f, 4.495291E-08f, 7.868156E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742277E-08f, -2.384976E-08f, 3.926991f)
            };
            // 480. - X: 270, Y: -540, Z: 45
            yield return new object[]
            {
                "X: 270, Y: -540, Z: 45",
                new float3(4.712389f, -9.424778f, 0.7853982f),
                new Quaternion(-0.2705981f, -0.6532815f, -0.6532815f, 0.2705981f),
                new float4x4(-0.7071066f, 0.7071068f, 0f, 0f, 0f, 8.940697E-08f, 0.9999999f, 0f, 0.7071068f, 0.7071067f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.926991f, 0f)
            };
            // 481. - X: 360, Y: -540, Z: 45
            yield return new object[]
            {
                "X: 360, Y: -540, Z: 45",
                new float3(6.283185f, -9.424778f, 0.7853982f),
                new Quaternion(-0.3826835f, -0.9238795f, 7.620466E-08f, -4.44724E-08f),
                new float4x4(-0.7071067f, 0.7071068f, 2.384976E-08f, 0f, 0.7071068f, 0.7071067f, -1.748456E-07f, 0f, -1.404988E-07f, -1.067701E-07f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 3.141593f, 0.7853982f)
            };
            // 482. - X: 585, Y: -540, Z: 45
            yield return new object[]
            {
                "X: 585, Y: -540, Z: 45",
                new float3(10.21018f, -9.424778f, 0.7853982f),
                new Quaternion(0.1464467f, 0.3535535f, 0.8535533f, -0.3535534f),
                new float4x4(-0.7071067f, 0.7071068f, 0f, 0f, -0.4999998f, -0.4999997f, 0.707107f, 0f, 0.5000002f, 0.5000002f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 0f, 3.926991f)
            };
            // 483. - X: -90, Y: -540, Z: 45
            yield return new object[]
            {
                "X: -90, Y: -540, Z: 45",
                new float3(-1.570796f, -9.424778f, 0.7853982f),
                new Quaternion(0.2705981f, 0.6532815f, 0.6532815f, -0.2705981f),
                new float4x4(-0.7071066f, 0.7071068f, 0f, 0f, 0f, 8.940697E-08f, 0.9999999f, 0f, 0.7071068f, 0.7071067f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.926991f, 0f)
            };
            // 484. - X: -540, Y: -540, Z: 45
            yield return new object[]
            {
                "X: -540, Y: -540, Z: 45",
                new float3(-9.424778f, -9.424778f, 0.7853982f),
                new Quaternion(1.558061E-08f, 6.453698E-09f, -0.9238795f, 0.3826835f),
                new float4x4(-0.7071067f, 0.7071068f, -2.384976E-08f, 0f, -0.7071068f, -0.7071067f, -2.384976E-08f, 0f, -3.372865E-08f, 2.664535E-15f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, -2.384976E-08f, 3.926991f)
            };
            // 485. - X: 0, Y: 0, Z: 90
            yield return new object[]
            {
                "X: 0, Y: 0, Z: 90",
                new float3(0f, 0f, 1.570796f),
                new Quaternion(0f, 0f, 0.7071068f, 0.7071068f),
                new float4x4(5.960464E-08f, -0.9999999f, 0f, 0f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0f, 1.570796f)
            };
            // 486. - X: 0.1, Y: 0, Z: 90
            yield return new object[]
            {
                "X: 0.1, Y: 0, Z: 90",
                new float3(0.001745329f, 0f, 1.570796f),
                new Quaternion(0.000617067f, -0.000617067f, 0.7071065f, 0.7071065f),
                new float4x4(-4.62876E-08f, -1f, 0f, 0f, 0.9999985f, -4.62876E-08f, -0.001745328f, 0f, 0.001745328f, 0f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0f, 1.570796f)
            };
            // 487. - X: 1, Y: 0, Z: 90
            yield return new object[]
            {
                "X: 1, Y: 0, Z: 90",
                new float3(0.01745329f, 0f, 1.570796f),
                new Quaternion(0.006170592f, -0.006170592f, 0.7070798f, 0.7070798f),
                new float4x4(8.192001E-08f, -0.9999999f, 0f, 0f, 0.9998476f, 8.192001E-08f, -0.01745241f, 0f, 0.01745241f, 0f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0f, 1.570796f)
            };
            // 488. - X: 45, Y: 0, Z: 90
            yield return new object[]
            {
                "X: 45, Y: 0, Z: 90",
                new float3(0.7853982f, 0f, 1.570796f),
                new Quaternion(0.2705981f, -0.2705981f, 0.6532815f, 0.6532815f),
                new float4x4(8.940697E-08f, -0.9999999f, 0f, 0f, 0.7071067f, 8.940697E-08f, -0.7071068f, 0f, 0.7071068f, 0f, 0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 0f, 1.570796f)
            };
            // 489. - X: 90, Y: 0, Z: 90
            yield return new object[]
            {
                "X: 90, Y: 0, Z: 90",
                new float3(1.570796f, 0f, 1.570796f),
                new Quaternion(0.5f, -0.5f, 0.5f, 0.5f),
                new float4x4(1.192093E-07f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, 0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 4.712389f, 0f)
            };
            // 490. - X: 180, Y: 0, Z: 90
            yield return new object[]
            {
                "X: 180, Y: 0, Z: 90",
                new float3(3.141593f, 0f, 1.570796f),
                new Quaternion(0.7071068f, -0.7071068f, -3.090862E-08f, -3.090862E-08f),
                new float4x4(5.960464E-08f, -0.9999999f, 0f, 0f, -0.9999999f, 5.960464E-08f, 8.742278E-08f, 0f, -8.742278E-08f, 0f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742279E-08f, 3.141593f, 4.712389f)
            };
            // 491. - X: 270, Y: 0, Z: 90
            yield return new object[]
            {
                "X: 270, Y: 0, Z: 90",
                new float3(4.712389f, 0f, 1.570796f),
                new Quaternion(0.5f, -0.5f, -0.5f, -0.5f),
                new float4x4(1.192093E-07f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 492. - X: 360, Y: 0, Z: 90
            yield return new object[]
            {
                "X: 360, Y: 0, Z: 90",
                new float3(6.283185f, 0f, 1.570796f),
                new Quaternion(-6.181724E-08f, 6.181724E-08f, -0.7071068f, -0.7071068f),
                new float4x4(5.960464E-08f, -0.9999999f, 0f, 0f, 0.9999999f, 5.960464E-08f, -1.748456E-07f, 0f, 1.748456E-07f, 0f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 0f, 1.570796f)
            };
            // 493. - X: 585, Y: 0, Z: 90
            yield return new object[]
            {
                "X: 585, Y: 0, Z: 90",
                new float3(10.21018f, 0f, 1.570796f),
                new Quaternion(-0.6532814f, 0.6532814f, 0.2705982f, 0.2705982f),
                new float4x4(7.450581E-08f, -0.9999999f, 0f, 0f, -0.7071064f, 7.450581E-08f, 0.707107f, 0f, -0.707107f, 0f, -0.7071064f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.141593f, 4.712389f)
            };
            // 494. - X: -90, Y: 0, Z: 90
            yield return new object[]
            {
                "X: -90, Y: 0, Z: 90",
                new float3(-1.570796f, 0f, 1.570796f),
                new Quaternion(-0.5f, 0.5f, 0.5f, 0.5f),
                new float4x4(1.192093E-07f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 495. - X: -540, Y: 0, Z: 90
            yield return new object[]
            {
                "X: -540, Y: 0, Z: 90",
                new float3(-9.424778f, 0f, 1.570796f),
                new Quaternion(0.7071068f, -0.7071068f, 8.432163E-09f, 8.432163E-09f),
                new float4x4(5.960464E-08f, -0.9999999f, 0f, 0f, -0.9999999f, 5.960464E-08f, -2.384976E-08f, 0f, 2.384976E-08f, 0f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.141593f, 4.712389f)
            };
            // 496. - X: 0, Y: 0.1, Z: 90
            yield return new object[]
            {
                "X: 0, Y: 0.1, Z: 90",
                new float3(0f, 0.001745329f, 1.570796f),
                new Quaternion(0.000617067f, 0.000617067f, 0.7071065f, 0.7071065f),
                new float4x4(-4.62876E-08f, -0.9999985f, 0.001745328f, 0f, 1f, -4.62876E-08f, 0f, 0f, 0f, 0.001745328f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0.001745329f, 1.570796f)
            };
            // 497. - X: 0.1, Y: 0.1, Z: 90
            yield return new object[]
            {
                "X: 0.1, Y: 0.1, Z: 90",
                new float3(0.001745329f, 0.001745329f, 1.570796f),
                new Quaternion(0.001234133f, 0f, 0.7071058f, 0.7071068f),
                new float4x4(2.920628E-06f, -0.9999986f, 0.001745326f, 0f, 0.9999986f, -1.255435E-07f, -0.001745328f, 0f, 0.001745326f, 0.001745328f, 0.999997f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0.001745329f, 1.570796f)
            };
            // 498. - X: 1, Y: 0.1, Z: 90
            yield return new object[]
            {
                "X: 1, Y: 0.1, Z: 90",
                new float3(0.01745329f, 0.001745329f, 1.570796f),
                new Quaternion(0.006787634f, -0.005553547f, 0.7070742f, 0.707085f),
                new float4x4(3.040541E-05f, -0.9999985f, 0.001745063f, 0f, 0.9998477f, -5.477341E-08f, -0.01745241f, 0f, 0.01745238f, 0.001745328f, 0.9998462f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0.00174533f, 1.570796f)
            };
            // 499. - X: 45, Y: 0.1, Z: 90
            yield return new object[]
            {
                "X: 45, Y: 0.1, Z: 90",
                new float3(0.7853982f, 0.001745329f, 1.570796f),
                new Quaternion(0.2711681f, -0.2700279f, 0.6530451f, 0.6535174f),
                new float4x4(0.001234218f, -0.9999985f, 0.001234084f, 0f, 0.7071067f, 1.043081E-07f, -0.7071068f, 0f, 0.7071058f, 0.001745313f, 0.7071056f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 0.00174526f, 1.570796f)
            };
            // 500. - X: 90, Y: 0.1, Z: 90
            yield return new object[]
            {
                "X: 90, Y: 0.1, Z: 90",
                new float3(1.570796f, 0.001745329f, 1.570796f),
                new Quaternion(0.5004361f, -0.4995635f, 0.4995635f, 0.5004361f),
                new float4x4(0.001745284f, -0.9999985f, 0f, 0f, 0f, 2.980232E-08f, -1f, 0f, 0.9999985f, 0.001745254f, 2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 4.714134f, 0f)
            };
            // 501. - X: 180, Y: 0.1, Z: 90
            yield return new object[]
            {
                "X: 180, Y: 0.1, Z: 90",
                new float3(3.141593f, 0.001745329f, 1.570796f),
                new Quaternion(0.7071065f, -0.7071065f, -0.0006170979f, 0.0006170361f),
                new float4x4(-4.636388E-08f, -0.9999985f, -0.001745328f, 0f, -1f, -4.636388E-08f, 8.73697E-08f, 0f, -8.73697E-08f, 0.001745328f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.73697E-08f, 3.143338f, 4.712389f)
            };
            // 502. - X: 270, Y: 0.1, Z: 90
            yield return new object[]
            {
                "X: 270, Y: 0.1, Z: 90",
                new float3(4.712389f, 0.001745329f, 1.570796f),
                new Quaternion(0.4995635f, -0.5004361f, -0.5004361f, -0.4995635f),
                new float4x4(-0.001745224f, -0.9999985f, 0f, 0f, 0f, 2.980232E-08f, 1f, 0f, -0.9999985f, 0.001745254f, 2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.572542f, 0f)
            };
            // 503. - X: 360, Y: 0.1, Z: 90
            yield return new object[]
            {
                "X: 360, Y: 0.1, Z: 90",
                new float3(6.283185f, 0.001745329f, 1.570796f),
                new Quaternion(-0.0006171288f, -0.0006170052f, -0.7071065f, -0.7071065f),
                new float4x4(-4.613503E-08f, -0.9999985f, 0.001745328f, 0f, 1f, -4.644016E-08f, -1.747976E-07f, 0f, 1.747976E-07f, 0.001745328f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.747976E-07f, 0.001745329f, 1.570796f)
            };
            // 504. - X: 585, Y: 0.1, Z: 90
            yield return new object[]
            {
                "X: 585, Y: 0.1, Z: 90",
                new float3(10.21018f, 0.001745329f, 1.570796f),
                new Quaternion(-0.653045f, 0.6535173f, 0.2711681f, 0.270028f),
                new float4x4(-0.001234055f, -0.9999983f, -0.001234084f, 0f, -0.7071065f, 1.192093E-07f, 0.7071069f, 0f, -0.7071059f, 0.001745313f, -0.7071053f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.143338f, 4.712389f)
            };
            // 505. - X: -90, Y: 0.1, Z: 90
            yield return new object[]
            {
                "X: -90, Y: 0.1, Z: 90",
                new float3(-1.570796f, 0.001745329f, 1.570796f),
                new Quaternion(-0.4995635f, 0.5004361f, 0.5004361f, 0.4995635f),
                new float4x4(-0.001745224f, -0.9999985f, 0f, 0f, 0f, 2.980232E-08f, 1f, 0f, -0.9999985f, 0.001745254f, 2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.572542f, 0f)
            };
            // 506. - X: -540, Y: 0.1, Z: 90
            yield return new object[]
            {
                "X: -540, Y: 0.1, Z: 90",
                new float3(-9.424778f, 0.001745329f, 1.570796f),
                new Quaternion(0.7071065f, -0.7071065f, -0.0006170585f, 0.0006170754f),
                new float4x4(-4.626673E-08f, -0.9999985f, -0.001745328f, 0f, -1f, -4.626673E-08f, -2.392335E-08f, 0f, 2.392335E-08f, 0.001745328f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.392335E-08f, 3.143338f, 4.712389f)
            };
            // 507. - X: 0, Y: 1, Z: 90
            yield return new object[]
            {
                "X: 0, Y: 1, Z: 90",
                new float3(0f, 0.01745329f, 1.570796f),
                new Quaternion(0.006170592f, 0.006170592f, 0.7070798f, 0.7070798f),
                new float4x4(8.192001E-08f, -0.9998476f, 0.01745241f, 0f, 0.9999999f, 8.192001E-08f, 0f, 0f, 0f, 0.01745241f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0.01745329f, 1.570796f)
            };
            // 508. - X: 0.1, Y: 1, Z: 90
            yield return new object[]
            {
                "X: 0.1, Y: 1, Z: 90",
                new float3(0.001745329f, 0.01745329f, 1.570796f),
                new Quaternion(0.006787634f, 0.005553547f, 0.7070742f, 0.707085f),
                new float4x4(3.040541E-05f, -0.9998477f, 0.01745238f, 0f, 0.9999985f, -5.477341E-08f, -0.001745328f, 0f, 0.001745063f, 0.01745241f, 0.9998462f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0.01745329f, 1.570796f)
            };
            // 509. - X: 1, Y: 1, Z: 90
            yield return new object[]
            {
                "X: 1, Y: 1, Z: 90",
                new float3(0.01745329f, 0.01745329f, 1.570796f),
                new Quaternion(0.01234071f, 0f, 0.7069991f, 0.7071067f),
                new float4x4(0.0003046393f, -0.9998476f, 0.01744975f, 0f, 0.9998476f, 5.288166E-08f, -0.0174524f, 0f, 0.01744975f, 0.0174524f, 0.9996954f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0.01745329f, 1.570796f)
            };
            // 510. - X: 45, Y: 1, Z: 90
            yield return new object[]
            {
                "X: 45, Y: 1, Z: 90",
                new float3(0.7853982f, 0.01745329f, 1.570796f),
                new Quaternion(0.2762886f, -0.2648869f, 0.6508952f, 0.655618f),
                new float4x4(0.01234071f, -0.9998477f, 0.01234072f, 0f, 0.7071067f, -1.490116E-08f, -0.7071067f, 0f, 0.7069991f, 0.01745239f, 0.7069991f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853984f, 0.01745335f, 1.570796f)
            };
            // 511. - X: 90, Y: 1, Z: 90
            yield return new object[]
            {
                "X: 90, Y: 1, Z: 90",
                new float3(1.570796f, 0.01745329f, 1.570796f),
                new Quaternion(0.5043442f, -0.4956177f, 0.4956177f, 0.5043442f),
                new float4x4(0.01745254f, -0.9998475f, 0f, 0f, 0f, 2.086163E-07f, -0.9999998f, 0f, 0.9998475f, 0.01745233f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 4.729842f, 0f)
            };
            // 512. - X: 180, Y: 1, Z: 90
            yield return new object[]
            {
                "X: 180, Y: 1, Z: 90",
                new float3(3.141593f, 0.01745329f, 1.570796f),
                new Quaternion(0.7070798f, -0.7070798f, -0.006170623f, 0.006170562f),
                new float4x4(8.116331E-08f, -0.9998476f, -0.0174524f, 0f, -0.9999999f, 8.116331E-08f, 8.6613E-08f, 0f, -8.6613E-08f, 0.0174524f, -0.9998475f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.6613E-08f, 3.159046f, 4.712389f)
            };
            // 513. - X: 270, Y: 1, Z: 90
            yield return new object[]
            {
                "X: 270, Y: 1, Z: 90",
                new float3(4.712389f, 0.01745329f, 1.570796f),
                new Quaternion(0.4956177f, -0.5043442f, -0.5043442f, -0.4956177f),
                new float4x4(-0.01745212f, -0.9998475f, 0f, 0f, 0f, 2.086163E-07f, 0.9999998f, 0f, -0.9998475f, 0.01745233f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.58825f, 0f)
            };
            // 514. - X: 360, Y: 1, Z: 90
            yield return new object[]
            {
                "X: 360, Y: 1, Z: 90",
                new float3(6.283185f, 0.01745329f, 1.570796f),
                new Quaternion(-0.006170654f, -0.00617053f, -0.7070798f, -0.7070798f),
                new float4x4(8.344796E-08f, -0.9998476f, 0.01745241f, 0f, 0.9999999f, 8.039206E-08f, -1.750886E-07f, 0f, 1.750886E-07f, 0.01745241f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.750886E-07f, 0.01745329f, 1.570796f)
            };
            // 515. - X: 585, Y: 1, Z: 90
            yield return new object[]
            {
                "X: 585, Y: 1, Z: 90",
                new float3(10.21018f, 0.01745329f, 1.570796f),
                new Quaternion(-0.6508952f, 0.6556179f, 0.2762887f, 0.264887f),
                new float4x4(-0.01234058f, -0.9998477f, -0.01234072f, 0f, -0.7071065f, -2.980232E-08f, 0.707107f, 0f, -0.7069993f, 0.01745236f, -0.7069987f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.159046f, 4.712389f)
            };
            // 516. - X: -90, Y: 1, Z: 90
            yield return new object[]
            {
                "X: -90, Y: 1, Z: 90",
                new float3(-1.570796f, 0.01745329f, 1.570796f),
                new Quaternion(-0.4956177f, 0.5043442f, 0.5043442f, 0.4956177f),
                new float4x4(-0.01745212f, -0.9998475f, 0f, 0f, 0f, 2.086163E-07f, 0.9999998f, 0f, -0.9998475f, 0.01745233f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.58825f, 0f)
            };
            // 517. - X: -540, Y: 1, Z: 90
            yield return new object[]
            {
                "X: -540, Y: 1, Z: 90",
                new float3(-9.424778f, 0.01745329f, 1.570796f),
                new Quaternion(0.7070798f, -0.7070798f, -0.006170584f, 0.006170601f),
                new float4x4(8.213101E-08f, -0.9998476f, -0.0174524f, 0f, -0.9999999f, 8.213101E-08f, -2.328306E-08f, 0f, 2.328306E-08f, 0.0174524f, -0.9998475f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.328306E-08f, 3.159046f, 4.712389f)
            };
            // 518. - X: 0, Y: 45, Z: 90
            yield return new object[]
            {
                "X: 0, Y: 45, Z: 90",
                new float3(0f, 0.7853982f, 1.570796f),
                new Quaternion(0.2705981f, 0.2705981f, 0.6532815f, 0.6532815f),
                new float4x4(8.940697E-08f, -0.7071067f, 0.7071068f, 0f, 0.9999999f, 8.940697E-08f, 0f, 0f, 0f, 0.7071068f, 0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0.7853982f, 1.570796f)
            };
            // 519. - X: 0.1, Y: 45, Z: 90
            yield return new object[]
            {
                "X: 0.1, Y: 45, Z: 90",
                new float3(0.001745329f, 0.7853982f, 1.570796f),
                new Quaternion(0.2711681f, 0.2700279f, 0.6530451f, 0.6535174f),
                new float4x4(0.001234218f, -0.7071067f, 0.7071058f, 0f, 0.9999985f, 1.043081E-07f, -0.001745313f, 0f, 0.001234084f, 0.7071068f, 0.7071056f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745314f, 0.7853982f, 1.570796f)
            };
            // 520. - X: 1, Y: 45, Z: 90
            yield return new object[]
            {
                "X: 1, Y: 45, Z: 90",
                new float3(0.01745329f, 0.7853982f, 1.570796f),
                new Quaternion(0.2762886f, 0.2648869f, 0.6508952f, 0.655618f),
                new float4x4(0.01234071f, -0.7071067f, 0.7069991f, 0f, 0.9998477f, -1.490116E-08f, -0.01745239f, 0f, 0.01234072f, 0.7071067f, 0.7069991f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745328f, 0.7853982f, 1.570796f)
            };
            // 521. - X: 45, Y: 45, Z: 90
            yield return new object[]
            {
                "X: 45, Y: 45, Z: 90",
                new float3(0.7853982f, 0.7853982f, 1.570796f),
                new Quaternion(0.5f, 0f, 0.4999999f, 0.7071068f),
                new float4x4(0.5000001f, -0.7071067f, 0.4999999f, 0f, 0.7071067f, 1.192093E-07f, -0.7071068f, 0f, 0.4999999f, 0.7071068f, 0.5f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 0.7853982f, 1.570796f)
            };
            // 522. - X: 90, Y: 45, Z: 90
            yield return new object[]
            {
                "X: 90, Y: 45, Z: 90",
                new float3(1.570796f, 0.7853982f, 1.570796f),
                new Quaternion(0.6532815f, -0.270598f, 0.270598f, 0.6532815f),
                new float4x4(0.7071069f, -0.7071066f, 0f, 0f, 0f, 1.490116E-07f, -0.9999999f, 0f, 0.7071066f, 0.7071067f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 5.497787f, 0f)
            };
            // 523. - X: 180, Y: 45, Z: 90
            yield return new object[]
            {
                "X: 180, Y: 45, Z: 90",
                new float3(3.141593f, 0.7853982f, 1.570796f),
                new Quaternion(0.6532815f, -0.6532815f, -0.2705981f, 0.270598f),
                new float4x4(5.960464E-08f, -0.7071067f, -0.7071067f, 0f, -0.9999999f, 5.960464E-08f, 8.940697E-08f, 0f, -8.940697E-08f, 0.7071067f, -0.7071066f, 0f, 0f, 0f, 0f, 1f),
                new float3(-5.960464E-08f, 3.926991f, 4.712389f)
            };
            // 524. - X: 270, Y: 45, Z: 90
            yield return new object[]
            {
                "X: 270, Y: 45, Z: 90",
                new float3(4.712389f, 0.7853982f, 1.570796f),
                new Quaternion(0.270598f, -0.6532815f, -0.6532815f, -0.270598f),
                new float4x4(-0.7071066f, -0.7071066f, 0f, 0f, 0f, 1.490116E-07f, 0.9999999f, 0f, -0.7071066f, 0.7071067f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 2.356194f, 0f)
            };
            // 525. - X: 360, Y: 45, Z: 90
            yield return new object[]
            {
                "X: 360, Y: 45, Z: 90",
                new float3(6.283185f, 0.7853982f, 1.570796f),
                new Quaternion(-0.2705981f, -0.270598f, -0.6532815f, -0.6532815f),
                new float4x4(1.490116E-07f, -0.7071067f, 0.7071067f, 0f, 0.9999999f, 2.980232E-08f, -1.490116E-07f, 0f, 1.490116E-07f, 0.7071067f, 0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.490116E-07f, 0.7853982f, 1.570796f)
            };
            // 526. - X: 585, Y: 45, Z: 90
            yield return new object[]
            {
                "X: 585, Y: 45, Z: 90",
                new float3(10.21018f, 0.7853982f, 1.570796f),
                new Quaternion(-0.4999999f, 0.7071067f, 0.5000001f, 1.043081E-07f),
                new float4x4(-0.4999999f, -0.7071066f, -0.4999998f, 0f, -0.7071064f, 1.788139E-07f, 0.7071069f, 0f, -0.5000001f, 0.7071066f, -0.4999995f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.926991f, 4.712389f)
            };
            // 527. - X: -90, Y: 45, Z: 90
            yield return new object[]
            {
                "X: -90, Y: 45, Z: 90",
                new float3(-1.570796f, 0.7853982f, 1.570796f),
                new Quaternion(-0.270598f, 0.6532815f, 0.6532815f, 0.270598f),
                new float4x4(-0.7071066f, -0.7071066f, 0f, 0f, 0f, 1.490116E-07f, 0.9999999f, 0f, -0.7071066f, 0.7071067f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 2.356194f, 0f)
            };
            // 528. - X: -540, Y: 45, Z: 90
            yield return new object[]
            {
                "X: -540, Y: 45, Z: 90",
                new float3(-9.424778f, 0.7853982f, 1.570796f),
                new Quaternion(0.6532815f, -0.6532815f, -0.2705981f, 0.2705981f),
                new float4x4(8.940697E-08f, -0.7071067f, -0.7071068f, 0f, -0.9999999f, 8.940697E-08f, 0f, 0f, 0f, 0.7071068f, -0.7071066f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.926991f, 4.712389f)
            };
            // 529. - X: 0, Y: 90, Z: 90
            yield return new object[]
            {
                "X: 0, Y: 90, Z: 90",
                new float3(0f, 1.570796f, 1.570796f),
                new Quaternion(0.5f, 0.5f, 0.5f, 0.5f),
                new float4x4(1.192093E-07f, 0f, 0.9999999f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.570796f, 1.570796f)
            };
            // 530. - X: 0.1, Y: 90, Z: 90
            yield return new object[]
            {
                "X: 0.1, Y: 90, Z: 90",
                new float3(0.001745329f, 1.570796f, 1.570796f),
                new Quaternion(0.5004361f, 0.4995635f, 0.4995635f, 0.5004361f),
                new float4x4(0.001745284f, 0f, 0.9999985f, 0f, 0.9999985f, 2.980232E-08f, -0.001745254f, 0f, 0f, 1f, 2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745255f, 1.570796f, 1.570796f)
            };
            // 531. - X: 1, Y: 90, Z: 90
            yield return new object[]
            {
                "X: 1, Y: 90, Z: 90",
                new float3(0.01745329f, 1.570796f, 1.570796f),
                new Quaternion(0.5043442f, 0.4956177f, 0.4956177f, 0.5043442f),
                new float4x4(0.01745254f, 0f, 0.9998475f, 0f, 0.9998475f, 2.086163E-07f, -0.01745233f, 0f, 0f, 0.9999998f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745322f, 1.570796f, 1.570796f)
            };
            // 532. - X: 45, Y: 90, Z: 90
            yield return new object[]
            {
                "X: 45, Y: 90, Z: 90",
                new float3(0.7853982f, 1.570796f, 1.570796f),
                new Quaternion(0.6532815f, 0.270598f, 0.270598f, 0.6532815f),
                new float4x4(0.7071069f, 0f, 0.7071066f, 0f, 0.7071066f, 1.490116E-07f, -0.7071067f, 0f, 0f, 0.9999999f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 1.570796f, 1.570796f)
            };
            // 533. - X: 90, Y: 90, Z: 90
            yield return new object[]
            {
                "X: 90, Y: 90, Z: 90",
                new float3(1.570796f, 1.570796f, 1.570796f),
                new Quaternion(0.7071067f, 0f, 0f, 0.7071067f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 1.788139E-07f, -0.9999998f, 0f, 0f, 0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570451f, 0f, 0f)
            };
            // 534. - X: 180, Y: 90, Z: 90
            yield return new object[]
            {
                "X: 180, Y: 90, Z: 90",
                new float3(3.141593f, 1.570796f, 1.570796f),
                new Quaternion(0.4999999f, -0.5f, -0.5f, 0.4999999f),
                new float4x4(0f, 0f, -0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(-1.788139E-07f, 4.712389f, 4.712389f)
            };
            // 535. - X: 270, Y: 90, Z: 90
            yield return new object[]
            {
                "X: 270, Y: 90, Z: 90",
                new float3(4.712389f, 1.570796f, 1.570796f),
                new Quaternion(0f, -0.7071067f, -0.7071067f, 0f),
                new float4x4(-0.9999996f, 0f, 0f, 0f, 0f, 1.788139E-07f, 0.9999998f, 0f, 0f, 0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 3.141593f, 0f)
            };
            // 536. - X: 360, Y: 90, Z: 90
            yield return new object[]
            {
                "X: 360, Y: 90, Z: 90",
                new float3(6.283185f, 1.570796f, 1.570796f),
                new Quaternion(-0.5f, -0.4999999f, -0.4999999f, -0.5f),
                new float4x4(2.384186E-07f, 0f, 0.9999999f, 0f, 0.9999999f, 1.192093E-07f, -1.192093E-07f, 0f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.788139E-07f, 1.570796f, 1.570796f)
            };
            // 537. - X: 585, Y: 90, Z: 90
            yield return new object[]
            {
                "X: 585, Y: 90, Z: 90",
                new float3(10.21018f, 1.570796f, 1.570796f),
                new Quaternion(-0.2705979f, 0.6532815f, 0.6532815f, -0.2705979f),
                new float4x4(-0.7071069f, 0f, -0.7071064f, 0f, -0.7071064f, 1.043081E-07f, 0.7071071f, 0f, 0f, 0.9999999f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 4.712389f, 4.712389f)
            };
            // 538. - X: -90, Y: 90, Z: 90
            yield return new object[]
            {
                "X: -90, Y: 90, Z: 90",
                new float3(-1.570796f, 1.570796f, 1.570796f),
                new Quaternion(0f, 0.7071067f, 0.7071067f, 0f),
                new float4x4(-0.9999996f, 0f, 0f, 0f, 0f, 1.788139E-07f, 0.9999998f, 0f, 0f, 0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 3.141593f, 0f)
            };
            // 539. - X: -540, Y: 90, Z: 90
            yield return new object[]
            {
                "X: -540, Y: 90, Z: 90",
                new float3(-9.424778f, 1.570796f, 1.570796f),
                new Quaternion(0.5f, -0.5f, -0.5f, 0.5f),
                new float4x4(1.192093E-07f, 0f, -0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 4.712389f)
            };
            // 540. - X: 0, Y: 180, Z: 90
            yield return new object[]
            {
                "X: 0, Y: 180, Z: 90",
                new float3(0f, 3.141593f, 1.570796f),
                new Quaternion(0.7071068f, 0.7071068f, -3.090862E-08f, -3.090862E-08f),
                new float4x4(5.960464E-08f, 0.9999999f, -8.742278E-08f, 0f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, -8.742278E-08f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.141593f, 1.570796f)
            };
            // 541. - X: 0.1, Y: 180, Z: 90
            yield return new object[]
            {
                "X: 0.1, Y: 180, Z: 90",
                new float3(0.001745329f, 3.141593f, 1.570796f),
                new Quaternion(0.7071065f, 0.7071065f, -0.0006170979f, 0.0006170361f),
                new float4x4(-4.636388E-08f, 1f, -8.73697E-08f, 0f, 0.9999985f, -4.636388E-08f, -0.001745328f, 0f, -0.001745328f, -8.73697E-08f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.141593f, 1.570796f)
            };
            // 542. - X: 1, Y: 180, Z: 90
            yield return new object[]
            {
                "X: 1, Y: 180, Z: 90",
                new float3(0.01745329f, 3.141593f, 1.570796f),
                new Quaternion(0.7070798f, 0.7070798f, -0.006170623f, 0.006170562f),
                new float4x4(8.116331E-08f, 0.9999999f, -8.6613E-08f, 0f, 0.9998476f, 8.116331E-08f, -0.0174524f, 0f, -0.0174524f, -8.6613E-08f, -0.9998475f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.141593f, 1.570796f)
            };
            // 543. - X: 45, Y: 180, Z: 90
            yield return new object[]
            {
                "X: 45, Y: 180, Z: 90",
                new float3(0.7853982f, 3.141593f, 1.570796f),
                new Quaternion(0.6532815f, 0.6532815f, -0.2705981f, 0.270598f),
                new float4x4(5.960464E-08f, 0.9999999f, -8.940697E-08f, 0f, 0.7071067f, 5.960464E-08f, -0.7071067f, 0f, -0.7071067f, -8.940697E-08f, -0.7071066f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 3.141593f, 1.570796f)
            };
            // 544. - X: 90, Y: 180, Z: 90
            yield return new object[]
            {
                "X: 90, Y: 180, Z: 90",
                new float3(1.570796f, 3.141593f, 1.570796f),
                new Quaternion(0.4999999f, 0.5f, -0.5f, 0.4999999f),
                new float4x4(0f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, -0.9999999f, -1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 1.570796f, 0f)
            };
            // 545. - X: 180, Y: 180, Z: 90
            yield return new object[]
            {
                "X: 180, Y: 180, Z: 90",
                new float3(3.141593f, 3.141593f, 1.570796f),
                new Quaternion(-6.181724E-08f, 0f, -0.7071068f, 0.7071068f),
                new float4x4(5.960464E-08f, 0.9999999f, 8.742278E-08f, 0f, -0.9999999f, 5.960464E-08f, 8.742278E-08f, 0f, 8.742278E-08f, -8.742278E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742279E-08f, 8.742278E-08f, 4.712389f)
            };
            // 546. - X: 270, Y: 180, Z: 90
            yield return new object[]
            {
                "X: 270, Y: 180, Z: 90",
                new float3(4.712389f, 3.141593f, 1.570796f),
                new Quaternion(-0.5f, -0.4999999f, -0.4999999f, 0.5f),
                new float4x4(2.384186E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, 0.9999999f, -1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 547. - X: 360, Y: 180, Z: 90
            yield return new object[]
            {
                "X: 360, Y: 180, Z: 90",
                new float3(6.283185f, 3.141593f, 1.570796f),
                new Quaternion(-0.7071068f, -0.7071068f, 9.272586E-08f, -3.090862E-08f),
                new float4x4(5.960463E-08f, 0.9999999f, -8.742277E-08f, 0f, 0.9999999f, 5.960463E-08f, -1.748456E-07f, 0f, -1.748456E-07f, -8.742277E-08f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 3.141593f, 1.570796f)
            };
            // 548. - X: 585, Y: 180, Z: 90
            yield return new object[]
            {
                "X: 585, Y: 180, Z: 90",
                new float3(10.21018f, 3.141593f, 1.570796f),
                new Quaternion(0.2705982f, 0.2705981f, 0.6532814f, -0.6532814f),
                new float4x4(1.192093E-07f, 0.9999999f, 5.960464E-08f, 0f, -0.7071064f, 4.470348E-08f, 0.707107f, 0f, 0.707107f, -5.960464E-08f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.264406E-07f, 4.712389f)
            };
            // 549. - X: -90, Y: 180, Z: 90
            yield return new object[]
            {
                "X: -90, Y: 180, Z: 90",
                new float3(-1.570796f, 3.141593f, 1.570796f),
                new Quaternion(0.5f, 0.4999999f, 0.4999999f, -0.5f),
                new float4x4(2.384186E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, 0.9999999f, -1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 550. - X: -540, Y: 180, Z: 90
            yield return new object[]
            {
                "X: -540, Y: 180, Z: 90",
                new float3(-9.424778f, 3.141593f, 1.570796f),
                new Quaternion(-2.247646E-08f, 3.934078E-08f, -0.7071068f, 0.7071068f),
                new float4x4(5.960464E-08f, 0.9999999f, 8.742278E-08f, 0f, -0.9999999f, 5.960464E-08f, -2.384976E-08f, 0f, -2.384976E-08f, -8.742278E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 8.742278E-08f, 4.712389f)
            };
            // 551. - X: 0, Y: 270, Z: 90
            yield return new object[]
            {
                "X: 0, Y: 270, Z: 90",
                new float3(0f, 4.712389f, 1.570796f),
                new Quaternion(0.5f, 0.5f, -0.5f, -0.5f),
                new float4x4(1.192093E-07f, 0f, -0.9999999f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, -0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 1.570796f)
            };
            // 552. - X: 0.1, Y: 270, Z: 90
            yield return new object[]
            {
                "X: 0.1, Y: 270, Z: 90",
                new float3(0.001745329f, 4.712389f, 1.570796f),
                new Quaternion(0.4995635f, 0.5004361f, -0.5004361f, -0.4995635f),
                new float4x4(-0.001745224f, 0f, -0.9999985f, 0f, 0.9999985f, 2.980232E-08f, -0.001745254f, 0f, 0f, -1f, 2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745255f, 4.712389f, 1.570796f)
            };
            // 553. - X: 1, Y: 270, Z: 90
            yield return new object[]
            {
                "X: 1, Y: 270, Z: 90",
                new float3(0.01745329f, 4.712389f, 1.570796f),
                new Quaternion(0.4956177f, 0.5043442f, -0.5043442f, -0.4956177f),
                new float4x4(-0.01745212f, 0f, -0.9998475f, 0f, 0.9998475f, 2.086163E-07f, -0.01745233f, 0f, 0f, -0.9999998f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745322f, 4.712389f, 1.570796f)
            };
            // 554. - X: 45, Y: 270, Z: 90
            yield return new object[]
            {
                "X: 45, Y: 270, Z: 90",
                new float3(0.7853982f, 4.712389f, 1.570796f),
                new Quaternion(0.270598f, 0.6532815f, -0.6532815f, -0.270598f),
                new float4x4(-0.7071066f, 0f, -0.7071066f, 0f, 0.7071066f, 1.490116E-07f, -0.7071067f, 0f, 0f, -0.9999999f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 4.712389f, 1.570796f)
            };
            // 555. - X: 90, Y: 270, Z: 90
            yield return new object[]
            {
                "X: 90, Y: 270, Z: 90",
                new float3(1.570796f, 4.712389f, 1.570796f),
                new Quaternion(0f, 0.7071067f, -0.7071067f, 0f),
                new float4x4(-0.9999996f, 0f, 0f, 0f, 0f, 1.788139E-07f, -0.9999998f, 0f, 0f, -0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570451f, 3.141593f, 0f)
            };
            // 556. - X: 180, Y: 270, Z: 90
            yield return new object[]
            {
                "X: 180, Y: 270, Z: 90",
                new float3(3.141593f, 4.712389f, 1.570796f),
                new Quaternion(-0.5f, 0.4999999f, -0.4999999f, 0.5f),
                new float4x4(2.384186E-07f, 0f, 0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, -0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(-1.788139E-07f, 1.570796f, 4.712389f)
            };
            // 557. - X: 270, Y: 270, Z: 90
            yield return new object[]
            {
                "X: 270, Y: 270, Z: 90",
                new float3(4.712389f, 4.712389f, 1.570796f),
                new Quaternion(-0.7071067f, 0f, 0f, 0.7071067f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 1.788139E-07f, 0.9999998f, 0f, 0f, -0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 0f, 0f)
            };
            // 558. - X: 360, Y: 270, Z: 90
            yield return new object[]
            {
                "X: 360, Y: 270, Z: 90",
                new float3(6.283185f, 4.712389f, 1.570796f),
                new Quaternion(-0.4999999f, -0.5f, 0.5f, 0.4999999f),
                new float4x4(0f, 0f, -0.9999999f, 0f, 0.9999999f, 1.192093E-07f, -1.192093E-07f, 0f, 0f, -0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.788139E-07f, 4.712389f, 1.570796f)
            };
            // 559. - X: 585, Y: 270, Z: 90
            yield return new object[]
            {
                "X: 585, Y: 270, Z: 90",
                new float3(10.21018f, 4.712389f, 1.570796f),
                new Quaternion(0.6532815f, -0.2705979f, 0.2705979f, -0.6532815f),
                new float4x4(0.7071072f, 0f, 0.7071064f, 0f, -0.7071064f, 1.043081E-07f, 0.7071071f, 0f, 0f, -0.9999999f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.570796f, 4.712389f)
            };
            // 560. - X: -90, Y: 270, Z: 90
            yield return new object[]
            {
                "X: -90, Y: 270, Z: 90",
                new float3(-1.570796f, 4.712389f, 1.570796f),
                new Quaternion(0.7071067f, 0f, 0f, -0.7071067f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 1.788139E-07f, 0.9999998f, 0f, 0f, -0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 0f, 0f)
            };
            // 561. - X: -540, Y: 270, Z: 90
            yield return new object[]
            {
                "X: -540, Y: 270, Z: 90",
                new float3(-9.424778f, 4.712389f, 1.570796f),
                new Quaternion(-0.5f, 0.5f, -0.5f, 0.5f),
                new float4x4(1.192093E-07f, 0f, 0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 0f, 0f, 0f, -0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.570796f, 4.712389f)
            };
            // 562. - X: 0, Y: 360, Z: 90
            yield return new object[]
            {
                "X: 0, Y: 360, Z: 90",
                new float3(0f, 6.283185f, 1.570796f),
                new Quaternion(-6.181724E-08f, -6.181724E-08f, -0.7071068f, -0.7071068f),
                new float4x4(5.960464E-08f, -0.9999999f, 1.748456E-07f, 0f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 1.748456E-07f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.748456E-07f, 1.570796f)
            };
            // 563. - X: 0.1, Y: 360, Z: 90
            yield return new object[]
            {
                "X: 0.1, Y: 360, Z: 90",
                new float3(0.001745329f, 6.283185f, 1.570796f),
                new Quaternion(-0.0006171288f, 0.0006170052f, -0.7071065f, -0.7071065f),
                new float4x4(-4.613503E-08f, -1f, 1.747976E-07f, 0f, 0.9999985f, -4.644016E-08f, -0.001745328f, 0f, 0.001745328f, 1.747976E-07f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 1.747979E-07f, 1.570796f)
            };
            // 564. - X: 1, Y: 360, Z: 90
            yield return new object[]
            {
                "X: 1, Y: 360, Z: 90",
                new float3(0.01745329f, 6.283185f, 1.570796f),
                new Quaternion(-0.006170654f, 0.00617053f, -0.7070798f, -0.7070798f),
                new float4x4(8.344796E-08f, -0.9999999f, 1.750886E-07f, 0f, 0.9998476f, 8.039206E-08f, -0.01745241f, 0f, 0.01745241f, 1.750886E-07f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 1.751153E-07f, 1.570796f)
            };
            // 565. - X: 45, Y: 360, Z: 90
            yield return new object[]
            {
                "X: 45, Y: 360, Z: 90",
                new float3(0.7853982f, 6.283185f, 1.570796f),
                new Quaternion(-0.2705981f, 0.270598f, -0.6532815f, -0.6532815f),
                new float4x4(1.490116E-07f, -0.9999999f, 1.490116E-07f, 0f, 0.7071067f, 2.980232E-08f, -0.7071067f, 0f, 0.7071067f, 1.490116E-07f, 0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853984f, 2.107342E-07f, 1.570796f)
            };
            // 566. - X: 90, Y: 360, Z: 90
            yield return new object[]
            {
                "X: 90, Y: 360, Z: 90",
                new float3(1.570796f, 6.283185f, 1.570796f),
                new Quaternion(-0.5f, 0.4999999f, -0.4999999f, -0.5f),
                new float4x4(2.384186E-07f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, 0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 4.712389f, 0f)
            };
            // 567. - X: 180, Y: 360, Z: 90
            yield return new object[]
            {
                "X: 180, Y: 360, Z: 90",
                new float3(3.141593f, 6.283185f, 1.570796f),
                new Quaternion(-0.7071068f, 0.7071068f, 9.272586E-08f, -3.090862E-08f),
                new float4x4(5.960463E-08f, -0.9999999f, -1.748456E-07f, 0f, -0.9999999f, 5.960463E-08f, 8.742277E-08f, 0f, -8.742277E-08f, 1.748456E-07f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742279E-08f, 3.141593f, 4.712389f)
            };
            // 568. - X: 270, Y: 360, Z: 90
            yield return new object[]
            {
                "X: 270, Y: 360, Z: 90",
                new float3(4.712389f, 6.283185f, 1.570796f),
                new Quaternion(-0.4999999f, 0.5f, 0.5f, 0.4999999f),
                new float4x4(0f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 569. - X: 360, Y: 360, Z: 90
            yield return new object[]
            {
                "X: 360, Y: 360, Z: 90",
                new float3(6.283185f, 6.283185f, 1.570796f),
                new Quaternion(1.236345E-07f, 0f, 0.7071068f, 0.7071068f),
                new float4x4(5.960464E-08f, -0.9999999f, 1.748456E-07f, 0f, 0.9999999f, 5.960461E-08f, -1.748456E-07f, 0f, 1.748456E-07f, 1.748456E-07f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 1.748456E-07f, 1.570796f)
            };
            // 570. - X: 585, Y: 360, Z: 90
            yield return new object[]
            {
                "X: 585, Y: 360, Z: 90",
                new float3(10.21018f, 6.283185f, 1.570796f),
                new Quaternion(0.6532814f, -0.6532814f, -0.2705982f, -0.2705981f),
                new float4x4(1.490116E-08f, -0.9999999f, -1.788139E-07f, 0f, -0.7071064f, 1.490116E-08f, 0.707107f, 0f, -0.707107f, 1.788139E-07f, -0.7071064f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.141593f, 4.712389f)
            };
            // 571. - X: -90, Y: 360, Z: 90
            yield return new object[]
            {
                "X: -90, Y: 360, Z: 90",
                new float3(-1.570796f, 6.283185f, 1.570796f),
                new Quaternion(0.4999999f, -0.5f, -0.5f, -0.4999999f),
                new float4x4(0f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 572. - X: -540, Y: 360, Z: 90
            yield return new object[]
            {
                "X: -540, Y: 360, Z: 90",
                new float3(-9.424778f, 6.283185f, 1.570796f),
                new Quaternion(-0.7071068f, 0.7071068f, 5.338508E-08f, -7.02494E-08f),
                new float4x4(5.960464E-08f, -0.9999999f, -1.748456E-07f, 0f, -0.9999999f, 5.960464E-08f, -2.384976E-08f, 0f, 2.384976E-08f, 1.748456E-07f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.141593f, 4.712389f)
            };
            // 573. - X: 0, Y: 585, Z: 90
            yield return new object[]
            {
                "X: 0, Y: 585, Z: 90",
                new float3(0f, 10.21018f, 1.570796f),
                new Quaternion(-0.6532814f, -0.6532814f, 0.2705982f, 0.2705982f),
                new float4x4(7.450581E-08f, 0.7071064f, -0.707107f, 0f, 0.9999999f, 7.450581E-08f, 0f, 0f, 0f, -0.707107f, -0.7071064f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.926991f, 1.570796f)
            };
            // 574. - X: 0.1, Y: 585, Z: 90
            yield return new object[]
            {
                "X: 0.1, Y: 585, Z: 90",
                new float3(0.001745329f, 10.21018f, 1.570796f),
                new Quaternion(-0.653045f, -0.6535173f, 0.2711681f, 0.270028f),
                new float4x4(-0.001234055f, 0.7071065f, -0.7071059f, 0f, 0.9999983f, 1.192093E-07f, -0.001745313f, 0f, -0.001234084f, -0.7071069f, -0.7071053f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745314f, 3.926991f, 1.570796f)
            };
            // 575. - X: 1, Y: 585, Z: 90
            yield return new object[]
            {
                "X: 1, Y: 585, Z: 90",
                new float3(0.01745329f, 10.21018f, 1.570796f),
                new Quaternion(-0.6508952f, -0.6556179f, 0.2762887f, 0.264887f),
                new float4x4(-0.01234058f, 0.7071065f, -0.7069993f, 0f, 0.9998477f, -2.980232E-08f, -0.01745236f, 0f, -0.01234072f, -0.707107f, -0.7069987f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745328f, 3.926991f, 1.570796f)
            };
            // 576. - X: 45, Y: 585, Z: 90
            yield return new object[]
            {
                "X: 45, Y: 585, Z: 90",
                new float3(0.7853982f, 10.21018f, 1.570796f),
                new Quaternion(-0.4999999f, -0.7071067f, 0.5000001f, 1.043081E-07f),
                new float4x4(-0.4999999f, 0.7071064f, -0.5000001f, 0f, 0.7071066f, 1.788139E-07f, -0.7071066f, 0f, -0.4999998f, -0.7071069f, -0.4999995f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 3.926991f, 1.570796f)
            };
            // 577. - X: 90, Y: 585, Z: 90
            yield return new object[]
            {
                "X: 90, Y: 585, Z: 90",
                new float3(1.570796f, 10.21018f, 1.570796f),
                new Quaternion(-0.2705979f, -0.6532815f, 0.6532815f, -0.2705979f),
                new float4x4(-0.7071069f, 0.7071064f, 0f, 0f, 0f, 1.043081E-07f, -0.9999999f, 0f, -0.7071064f, -0.7071071f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 2.356195f, 0f)
            };
            // 578. - X: 180, Y: 585, Z: 90
            yield return new object[]
            {
                "X: 180, Y: 585, Z: 90",
                new float3(3.141593f, 10.21018f, 1.570796f),
                new Quaternion(0.2705982f, -0.2705981f, 0.6532814f, -0.6532814f),
                new float4x4(1.192093E-07f, 0.7071064f, 0.707107f, 0f, -0.9999999f, 4.470348E-08f, 5.960464E-08f, 0f, 5.960464E-08f, -0.707107f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.940697E-08f, 0.7853987f, 4.712389f)
            };
            // 579. - X: 270, Y: 585, Z: 90
            yield return new object[]
            {
                "X: 270, Y: 585, Z: 90",
                new float3(4.712389f, 10.21018f, 1.570796f),
                new Quaternion(0.6532815f, 0.2705979f, 0.2705979f, -0.6532815f),
                new float4x4(0.7071072f, 0.7071064f, 0f, 0f, 0f, 1.043081E-07f, 0.9999999f, 0f, 0.7071064f, -0.7071071f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 5.497787f, 0f)
            };
            // 580. - X: 360, Y: 585, Z: 90
            yield return new object[]
            {
                "X: 360, Y: 585, Z: 90",
                new float3(6.283185f, 10.21018f, 1.570796f),
                new Quaternion(0.6532814f, 0.6532814f, -0.2705982f, -0.2705981f),
                new float4x4(1.490116E-08f, 0.7071064f, -0.707107f, 0f, 0.9999999f, 1.490116E-08f, -1.788139E-07f, 0f, -1.788139E-07f, -0.707107f, -0.7071064f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.490116E-07f, 3.926991f, 1.570796f)
            };
            // 581. - X: 585, Y: 585, Z: 90
            yield return new object[]
            {
                "X: 585, Y: 585, Z: 90",
                new float3(10.21018f, 10.21018f, 1.570796f),
                new Quaternion(-0.5000002f, 0f, -0.4999998f, 0.7071068f),
                new float4x4(0.5000004f, 0.7071065f, 0.5f, 0f, -0.7071065f, 0f, 0.707107f, 0f, 0.5f, -0.707107f, 0.4999996f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 0.7853985f, 4.712389f)
            };
            // 582. - X: -90, Y: 585, Z: 90
            yield return new object[]
            {
                "X: -90, Y: 585, Z: 90",
                new float3(-1.570796f, 10.21018f, 1.570796f),
                new Quaternion(-0.6532815f, -0.2705979f, -0.2705979f, 0.6532815f),
                new float4x4(0.7071072f, 0.7071064f, 0f, 0f, 0f, 1.043081E-07f, 0.9999999f, 0f, 0.7071064f, -0.7071071f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 5.497787f, 0f)
            };
            // 583. - X: -540, Y: 585, Z: 90
            yield return new object[]
            {
                "X: -540, Y: 585, Z: 90",
                new float3(-9.424778f, 10.21018f, 1.570796f),
                new Quaternion(0.2705982f, -0.2705982f, 0.6532814f, -0.6532814f),
                new float4x4(7.450581E-08f, 0.7071064f, 0.707107f, 0f, -0.9999999f, 7.450581E-08f, 0f, 0f, 0f, -0.707107f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0.7853986f, 4.712389f)
            };
            // 584. - X: 0, Y: -90, Z: 90
            yield return new object[]
            {
                "X: 0, Y: -90, Z: 90",
                new float3(0f, -1.570796f, 1.570796f),
                new Quaternion(-0.5f, -0.5f, 0.5f, 0.5f),
                new float4x4(1.192093E-07f, 0f, -0.9999999f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, -0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 1.570796f)
            };
            // 585. - X: 0.1, Y: -90, Z: 90
            yield return new object[]
            {
                "X: 0.1, Y: -90, Z: 90",
                new float3(0.001745329f, -1.570796f, 1.570796f),
                new Quaternion(-0.4995635f, -0.5004361f, 0.5004361f, 0.4995635f),
                new float4x4(-0.001745224f, 0f, -0.9999985f, 0f, 0.9999985f, 2.980232E-08f, -0.001745254f, 0f, 0f, -1f, 2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745255f, 4.712389f, 1.570796f)
            };
            // 586. - X: 1, Y: -90, Z: 90
            yield return new object[]
            {
                "X: 1, Y: -90, Z: 90",
                new float3(0.01745329f, -1.570796f, 1.570796f),
                new Quaternion(-0.4956177f, -0.5043442f, 0.5043442f, 0.4956177f),
                new float4x4(-0.01745212f, 0f, -0.9998475f, 0f, 0.9998475f, 2.086163E-07f, -0.01745233f, 0f, 0f, -0.9999998f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745322f, 4.712389f, 1.570796f)
            };
            // 587. - X: 45, Y: -90, Z: 90
            yield return new object[]
            {
                "X: 45, Y: -90, Z: 90",
                new float3(0.7853982f, -1.570796f, 1.570796f),
                new Quaternion(-0.270598f, -0.6532815f, 0.6532815f, 0.270598f),
                new float4x4(-0.7071066f, 0f, -0.7071066f, 0f, 0.7071066f, 1.490116E-07f, -0.7071067f, 0f, 0f, -0.9999999f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 4.712389f, 1.570796f)
            };
            // 588. - X: 90, Y: -90, Z: 90
            yield return new object[]
            {
                "X: 90, Y: -90, Z: 90",
                new float3(1.570796f, -1.570796f, 1.570796f),
                new Quaternion(0f, -0.7071067f, 0.7071067f, 0f),
                new float4x4(-0.9999996f, 0f, 0f, 0f, 0f, 1.788139E-07f, -0.9999998f, 0f, 0f, -0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570451f, 3.141593f, 0f)
            };
            // 589. - X: 180, Y: -90, Z: 90
            yield return new object[]
            {
                "X: 180, Y: -90, Z: 90",
                new float3(3.141593f, -1.570796f, 1.570796f),
                new Quaternion(0.5f, -0.4999999f, 0.4999999f, -0.5f),
                new float4x4(2.384186E-07f, 0f, 0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, -0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(-1.788139E-07f, 1.570796f, 4.712389f)
            };
            // 590. - X: 270, Y: -90, Z: 90
            yield return new object[]
            {
                "X: 270, Y: -90, Z: 90",
                new float3(4.712389f, -1.570796f, 1.570796f),
                new Quaternion(0.7071067f, 0f, 0f, -0.7071067f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 1.788139E-07f, 0.9999998f, 0f, 0f, -0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 0f, 0f)
            };
            // 591. - X: 360, Y: -90, Z: 90
            yield return new object[]
            {
                "X: 360, Y: -90, Z: 90",
                new float3(6.283185f, -1.570796f, 1.570796f),
                new Quaternion(0.4999999f, 0.5f, -0.5f, -0.4999999f),
                new float4x4(0f, 0f, -0.9999999f, 0f, 0.9999999f, 1.192093E-07f, -1.192093E-07f, 0f, 0f, -0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.788139E-07f, 4.712389f, 1.570796f)
            };
            // 592. - X: 585, Y: -90, Z: 90
            yield return new object[]
            {
                "X: 585, Y: -90, Z: 90",
                new float3(10.21018f, -1.570796f, 1.570796f),
                new Quaternion(-0.6532815f, 0.2705979f, -0.2705979f, 0.6532815f),
                new float4x4(0.7071072f, 0f, 0.7071064f, 0f, -0.7071064f, 1.043081E-07f, 0.7071071f, 0f, 0f, -0.9999999f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.570796f, 4.712389f)
            };
            // 593. - X: -90, Y: -90, Z: 90
            yield return new object[]
            {
                "X: -90, Y: -90, Z: 90",
                new float3(-1.570796f, -1.570796f, 1.570796f),
                new Quaternion(-0.7071067f, 0f, 0f, 0.7071067f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 1.788139E-07f, 0.9999998f, 0f, 0f, -0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 0f, 0f)
            };
            // 594. - X: -540, Y: -90, Z: 90
            yield return new object[]
            {
                "X: -540, Y: -90, Z: 90",
                new float3(-9.424778f, -1.570796f, 1.570796f),
                new Quaternion(0.5f, -0.5f, 0.5f, -0.5f),
                new float4x4(1.192093E-07f, 0f, 0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 0f, 0f, 0f, -0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.570796f, 4.712389f)
            };
            // 595. - X: 0, Y: -540, Z: 90
            yield return new object[]
            {
                "X: 0, Y: -540, Z: 90",
                new float3(0f, -9.424778f, 1.570796f),
                new Quaternion(0.7071068f, 0.7071068f, 8.432163E-09f, 8.432163E-09f),
                new float4x4(5.960464E-08f, 0.9999999f, 2.384976E-08f, 0f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 2.384976E-08f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.141593f, 1.570796f)
            };
            // 596. - X: 0.1, Y: -540, Z: 90
            yield return new object[]
            {
                "X: 0.1, Y: -540, Z: 90",
                new float3(0.001745329f, -9.424778f, 1.570796f),
                new Quaternion(0.7071065f, 0.7071065f, -0.0006170585f, 0.0006170754f),
                new float4x4(-4.626673E-08f, 1f, 2.392335E-08f, 0f, 0.9999985f, -4.626673E-08f, -0.001745328f, 0f, -0.001745328f, 2.392335E-08f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.141593f, 1.570796f)
            };
            // 597. - X: 1, Y: -540, Z: 90
            yield return new object[]
            {
                "X: 1, Y: -540, Z: 90",
                new float3(0.01745329f, -9.424778f, 1.570796f),
                new Quaternion(0.7070798f, 0.7070798f, -0.006170584f, 0.006170601f),
                new float4x4(8.213101E-08f, 0.9999999f, 2.328306E-08f, 0f, 0.9998476f, 8.213101E-08f, -0.0174524f, 0f, -0.0174524f, 2.328306E-08f, -0.9998475f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.141593f, 1.570796f)
            };
            // 598. - X: 45, Y: -540, Z: 90
            yield return new object[]
            {
                "X: 45, Y: -540, Z: 90",
                new float3(0.7853982f, -9.424778f, 1.570796f),
                new Quaternion(0.6532815f, 0.6532815f, -0.2705981f, 0.2705981f),
                new float4x4(8.940697E-08f, 0.9999999f, 0f, 0f, 0.7071067f, 8.940697E-08f, -0.7071068f, 0f, -0.7071068f, 0f, -0.7071066f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 3.141593f, 1.570796f)
            };
            // 599. - X: 90, Y: -540, Z: 90
            yield return new object[]
            {
                "X: 90, Y: -540, Z: 90",
                new float3(1.570796f, -9.424778f, 1.570796f),
                new Quaternion(0.5f, 0.5f, -0.5f, 0.5f),
                new float4x4(1.192093E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, -0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 1.570796f, 0f)
            };
            // 600. - X: 180, Y: -540, Z: 90
            yield return new object[]
            {
                "X: 180, Y: -540, Z: 90",
                new float3(3.141593f, -9.424778f, 1.570796f),
                new Quaternion(-2.247646E-08f, -3.934078E-08f, -0.7071068f, 0.7071068f),
                new float4x4(5.960464E-08f, 0.9999999f, -2.384976E-08f, 0f, -0.9999999f, 5.960464E-08f, 8.742278E-08f, 0f, 8.742278E-08f, 2.384976E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742279E-08f, -2.384976E-08f, 4.712389f)
            };
            // 601. - X: 270, Y: -540, Z: 90
            yield return new object[]
            {
                "X: 270, Y: -540, Z: 90",
                new float3(4.712389f, -9.424778f, 1.570796f),
                new Quaternion(-0.5f, -0.5f, -0.5f, 0.5f),
                new float4x4(1.192093E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, 0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 602. - X: 360, Y: -540, Z: 90
            yield return new object[]
            {
                "X: 360, Y: -540, Z: 90",
                new float3(6.283185f, -9.424778f, 1.570796f),
                new Quaternion(-0.7071068f, -0.7071068f, 5.338508E-08f, -7.02494E-08f),
                new float4x4(5.960464E-08f, 0.9999999f, 2.384976E-08f, 0f, 0.9999999f, 5.960464E-08f, -1.748456E-07f, 0f, -1.748456E-07f, 2.384976E-08f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 3.141593f, 1.570796f)
            };
            // 603. - X: 585, Y: -540, Z: 90
            yield return new object[]
            {
                "X: 585, Y: -540, Z: 90",
                new float3(10.21018f, -9.424778f, 1.570796f),
                new Quaternion(0.2705982f, 0.2705982f, 0.6532814f, -0.6532814f),
                new float4x4(7.450581E-08f, 0.9999999f, 0f, 0f, -0.7071064f, 7.450581E-08f, 0.707107f, 0f, 0.707107f, 0f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 0f, 4.712389f)
            };
            // 604. - X: -90, Y: -540, Z: 90
            yield return new object[]
            {
                "X: -90, Y: -540, Z: 90",
                new float3(-1.570796f, -9.424778f, 1.570796f),
                new Quaternion(0.5f, 0.5f, 0.5f, -0.5f),
                new float4x4(1.192093E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, 0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 605. - X: -540, Y: -540, Z: 90
            yield return new object[]
            {
                "X: -540, Y: -540, Z: 90",
                new float3(-9.424778f, -9.424778f, 1.570796f),
                new Quaternion(1.686433E-08f, 0f, -0.7071068f, 0.7071068f),
                new float4x4(5.960464E-08f, 0.9999999f, -2.384976E-08f, 0f, -0.9999999f, 5.960464E-08f, -2.384976E-08f, 0f, -2.384976E-08f, 2.384976E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, -2.384976E-08f, 4.712389f)
            };
            // 606. - X: 0, Y: 0, Z: 180
            yield return new object[]
            {
                "X: 0, Y: 0, Z: 180",
                new float3(0f, 0f, 3.141593f),
                new Quaternion(0f, 0f, 1f, -4.371139E-08f),
                new float4x4(-1f, 8.742278E-08f, 0f, 0f, -8.742278E-08f, -1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0f, 3.141593f)
            };
            // 607. - X: 0.1, Y: 0, Z: 180
            yield return new object[]
            {
                "X: 0.1, Y: 0, Z: 180",
                new float3(0.001745329f, 0f, 3.141593f),
                new Quaternion(-3.814538E-11f, -0.0008726645f, 0.9999996f, -4.371137E-08f),
                new float4x4(-1f, 8.742278E-08f, 6.938894E-18f, 0f, -8.742266E-08f, -0.9999986f, -0.001745328f, 0f, -1.525814E-10f, -0.001745328f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 6.938905E-18f, 3.141593f)
            };
            // 608. - X: 1, Y: 0, Z: 180
            yield return new object[]
            {
                "X: 1, Y: 0, Z: 180",
                new float3(0.01745329f, 0f, 3.141593f),
                new Quaternion(-3.81449E-10f, -0.008726535f, 0.9999619f, -4.370972E-08f),
                new float4x4(-0.9999999f, 8.742277E-08f, -5.551115E-17f, 0f, -8.740945E-08f, -0.9998477f, -0.01745241f, 0f, -1.525738E-09f, -0.01745241f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, -5.551961E-17f, 3.141593f)
            };
            // 609. - X: 45, Y: 0, Z: 180
            yield return new object[]
            {
                "X: 45, Y: 0, Z: 180",
                new float3(0.7853982f, 0f, 3.141593f),
                new Quaternion(-1.672763E-08f, -0.3826835f, 0.9238795f, -4.038406E-08f),
                new float4x4(-1f, 8.742278E-08f, 0f, 0f, -6.181723E-08f, -0.7071067f, -0.7071068f, 0f, -6.181724E-08f, -0.7071068f, 0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 0f, 3.141593f)
            };
            // 610. - X: 90, Y: 0, Z: 180
            yield return new object[]
            {
                "X: 90, Y: 0, Z: 180",
                new float3(1.570796f, 0f, 3.141593f),
                new Quaternion(-3.090862E-08f, -0.7071068f, 0.7071068f, -3.090862E-08f),
                new float4x4(-0.9999999f, 8.742278E-08f, 0f, 0f, 0f, 5.960464E-08f, -0.9999999f, 0f, -8.742278E-08f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 3.141593f, 0f)
            };
            // 611. - X: 180, Y: 0, Z: 180
            yield return new object[]
            {
                "X: 180, Y: 0, Z: 180",
                new float3(3.141593f, 0f, 3.141593f),
                new Quaternion(-4.371139E-08f, -1f, -4.371139E-08f, 1.910685E-15f),
                new float4x4(-1f, 8.742278E-08f, 0f, 0f, 8.742278E-08f, 1f, 8.742278E-08f, 0f, 7.642742E-15f, 8.742278E-08f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 3.141593f, 8.742278E-08f)
            };
            // 612. - X: 270, Y: 0, Z: 180
            yield return new object[]
            {
                "X: 270, Y: 0, Z: 180",
                new float3(4.712389f, 0f, 3.141593f),
                new Quaternion(-3.090862E-08f, -0.7071068f, -0.7071068f, 3.090862E-08f),
                new float4x4(-0.9999999f, 8.742278E-08f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, 8.742278E-08f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.141593f, 0f)
            };
            // 613. - X: 360, Y: 0, Z: 180
            yield return new object[]
            {
                "X: 360, Y: 0, Z: 180",
                new float3(6.283185f, 0f, 3.141593f),
                new Quaternion(3.821371E-15f, 8.742278E-08f, -1f, 4.371139E-08f),
                new float4x4(-1f, 8.742278E-08f, 0f, 0f, -8.742278E-08f, -1f, -1.748456E-07f, 0f, -1.528548E-14f, -1.748456E-07f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 0f, 3.141593f)
            };
            // 614. - X: 585, Y: 0, Z: 180
            yield return new object[]
            {
                "X: 585, Y: 0, Z: 180",
                new float3(10.21018f, 0f, 3.141593f),
                new Quaternion(4.038405E-08f, 0.9238794f, 0.3826836f, -1.672763E-08f),
                new float4x4(-0.9999999f, 8.742277E-08f, 0f, 0f, 6.181721E-08f, 0.7071065f, 0.707107f, 0f, 6.181726E-08f, 0.707107f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.141593f, 8.742278E-08f)
            };
            // 615. - X: -90, Y: 0, Z: 180
            yield return new object[]
            {
                "X: -90, Y: 0, Z: 180",
                new float3(-1.570796f, 0f, 3.141593f),
                new Quaternion(3.090862E-08f, 0.7071068f, 0.7071068f, -3.090862E-08f),
                new float4x4(-0.9999999f, 8.742278E-08f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, 8.742278E-08f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.141593f, 0f)
            };
            // 616. - X: -540, Y: 0, Z: 180
            yield return new object[]
            {
                "X: -540, Y: 0, Z: 180",
                new float3(-9.424778f, 0f, 3.141593f),
                new Quaternion(-4.371139E-08f, -1f, 1.192488E-08f, -5.212531E-16f),
                new float4x4(-1f, 8.742278E-08f, 0f, 0f, 8.742278E-08f, 1f, -2.384976E-08f, 0f, -2.085012E-15f, -2.384976E-08f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.141593f, 8.742278E-08f)
            };
            // 617. - X: 0, Y: 0.1, Z: 180
            yield return new object[]
            {
                "X: 0, Y: 0.1, Z: 180",
                new float3(0f, 0.001745329f, 3.141593f),
                new Quaternion(0.0008726645f, -3.814538E-11f, 0.9999996f, -4.371137E-08f),
                new float4x4(-0.9999986f, 8.742266E-08f, 0.001745328f, 0f, -8.742278E-08f, -1f, 6.938894E-18f, 0f, 0.001745328f, -1.525814E-10f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(-6.938894E-18f, 0.001745329f, 3.141593f)
            };
            // 618. - X: 0.1, Y: 0.1, Z: 180
            yield return new object[]
            {
                "X: 0.1, Y: 0.1, Z: 180",
                new float3(0.001745329f, 0.001745329f, 3.141593f),
                new Quaternion(0.0008726642f, -0.0008726643f, 0.9999993f, 7.17832E-07f),
                new float4x4(-0.9999987f, -2.958749E-06f, 0.001745326f, 0f, -8.742268E-08f, -0.9999987f, -0.001745329f, 0f, 0.001745328f, -0.001745326f, 0.999997f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0.001745329f, 3.141593f)
            };
            // 619. - X: 1, Y: 0.1, Z: 180
            yield return new object[]
            {
                "X: 1, Y: 0.1, Z: 180",
                new float3(0.01745329f, 0.001745329f, 3.141593f),
                new Quaternion(0.0008726309f, -0.008726533f, 0.9999616f, 7.571628E-06f),
                new float4x4(-0.9999985f, -3.037276E-05f, 0.001745063f, 0f, -8.740972E-08f, -0.9998478f, -0.01745241f, 0f, 0.001745327f, -0.01745238f, 0.9998462f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0.001745329f, 3.141593f)
            };
            // 620. - X: 45, Y: 0.1, Z: 180
            yield return new object[]
            {
                "X: 45, Y: 0.1, Z: 180",
                new float3(0.7853982f, 0.001745329f, 3.141593f),
                new Quaternion(0.0008062202f, -0.3826833f, 0.9238791f, 0.0003339139f),
                new float4x4(-0.9999985f, -0.001234046f, 0.001234133f, 0f, -6.187474E-08f, -0.7071067f, -0.7071068f, 0f, 0.001745267f, -0.7071057f, 0.7071057f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 0.001745329f, 3.141593f)
            };
            // 621. - X: 90, Y: 0.1, Z: 180
            yield return new object[]
            {
                "X: 90, Y: 0.1, Z: 180",
                new float3(1.570796f, 0.001745329f, 3.141593f),
                new Quaternion(0.0006170361f, -0.7071065f, 0.7071065f, 0.0006170361f),
                new float4x4(-0.9999986f, -0.001745241f, 0f, 0f, 0f, -4.621131E-08f, -1f, 0f, 0.001745241f, -0.9999985f, -4.621131E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 3.143338f, 0f)
            };
            // 622. - X: 180, Y: 0.1, Z: 180
            yield return new object[]
            {
                "X: 180, Y: 0.1, Z: 180",
                new float3(3.141593f, 0.001745329f, 3.141593f),
                new Quaternion(-4.374952E-08f, -0.9999996f, -4.367323E-08f, 0.0008726645f),
                new float4x4(-0.9999986f, 8.757524E-08f, -0.001745328f, 0f, 8.742278E-08f, 1f, 8.742278E-08f, 0f, 0.001745328f, 8.727007E-08f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 3.143338f, 8.742278E-08f)
            };
            // 623. - X: 270, Y: 0.1, Z: 180
            yield return new object[]
            {
                "X: 270, Y: 0.1, Z: 180",
                new float3(4.712389f, 0.001745329f, 3.141593f),
                new Quaternion(-0.0006170979f, -0.7071065f, -0.7071065f, 0.0006170979f),
                new float4x4(-0.9999986f, 0.001745416f, 0f, 0f, 0f, -4.636388E-08f, 1f, 0f, 0.001745416f, 0.9999985f, -4.636388E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.143338f, 0f)
            };
            // 624. - X: 360, Y: 0.1, Z: 180
            yield return new object[]
            {
                "X: 360, Y: 0.1, Z: 180",
                new float3(6.283185f, 0.001745329f, 3.141593f),
                new Quaternion(-0.0008726645f, 8.746089E-08f, -0.9999996f, 4.363508E-08f),
                new float4x4(-0.9999986f, 8.711749E-08f, 0.001745328f, 0f, -8.742278E-08f, -1f, -1.748456E-07f, 0f, 0.001745328f, -1.749979E-07f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 0.001745329f, 3.141593f)
            };
            // 625. - X: 585, Y: 0.1, Z: 180
            yield return new object[]
            {
                "X: 585, Y: 0.1, Z: 180",
                new float3(10.21018f, 0.001745329f, 3.141593f),
                new Quaternion(0.0003339948f, 0.9238791f, 0.3826835f, -0.0008062535f),
                new float4x4(-0.9999985f, 0.001234221f, -0.001234133f, 0f, 6.181654E-08f, 0.7071065f, 0.707107f, 0f, 0.00174539f, 0.7071059f, -0.7071054f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.143338f, 8.742182E-08f)
            };
            // 626. - X: -90, Y: 0.1, Z: 180
            yield return new object[]
            {
                "X: -90, Y: 0.1, Z: 180",
                new float3(-1.570796f, 0.001745329f, 3.141593f),
                new Quaternion(0.0006170979f, 0.7071065f, 0.7071065f, -0.0006170979f),
                new float4x4(-0.9999986f, 0.001745416f, 0f, 0f, 0f, -4.636388E-08f, 1f, 0f, 0.001745416f, 0.9999985f, -4.636388E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.143338f, 0f)
            };
            // 627. - X: -540, Y: 0.1, Z: 180
            yield return new object[]
            {
                "X: -540, Y: 0.1, Z: 180",
                new float3(-9.424778f, 0.001745329f, 3.141593f),
                new Quaternion(-4.370097E-08f, -0.9999996f, 1.196302E-08f, 0.0008726645f),
                new float4x4(-0.9999986f, 8.738103E-08f, -0.001745328f, 0f, 8.742279E-08f, 1f, -2.384976E-08f, 0f, 0.001745328f, -2.400231E-08f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.143338f, 8.742279E-08f)
            };
            // 628. - X: 0, Y: 1, Z: 180
            yield return new object[]
            {
                "X: 0, Y: 1, Z: 180",
                new float3(0f, 0.01745329f, 3.141593f),
                new Quaternion(0.008726535f, -3.81449E-10f, 0.9999619f, -4.370972E-08f),
                new float4x4(-0.9998477f, 8.740945E-08f, 0.01745241f, 0f, -8.742277E-08f, -0.9999999f, -5.551115E-17f, 0f, 0.01745241f, -1.525738E-09f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.551115E-17f, 0.01745329f, 3.141593f)
            };
            // 629. - X: 0.1, Y: 1, Z: 180
            yield return new object[]
            {
                "X: 0.1, Y: 1, Z: 180",
                new float3(0.001745329f, 0.01745329f, 3.141593f),
                new Quaternion(0.008726533f, -0.0008726317f, 0.9999616f, 7.571628E-06f),
                new float4x4(-0.9998478f, -3.037277E-05f, 0.01745238f, 0f, -8.742336E-08f, -0.9999985f, -0.001745328f, 0f, 0.01745241f, -0.001745064f, 0.9998462f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0.01745329f, 3.141593f)
            };
            // 630. - X: 1, Y: 1, Z: 180
            yield return new object[]
            {
                "X: 1, Y: 1, Z: 180",
                new float3(0.01745329f, 0.01745329f, 3.141593f),
                new Quaternion(0.008726203f, -0.008726203f, 0.9999238f, 7.610871E-05f),
                new float4x4(-0.9998476f, -0.0003044991f, 0.01744975f, 0f, -8.73988E-08f, -0.9998476f, -0.0174524f, 0f, 0.0174524f, -0.01744975f, 0.9996954f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0.0174533f, 3.141593f)
            };
            // 631. - X: 45, Y: 1, Z: 180
            yield return new object[]
            {
                "X: 45, Y: 1, Z: 180",
                new float3(0.7853982f, 0.01745329f, 3.141593f),
                new Quaternion(0.00806225f, -0.3826689f, 0.9238443f, 0.00333946f),
                new float4x4(-0.9998477f, -0.01234063f, 0.01234071f, 0f, -6.146729E-08f, -0.7071068f, -0.7071068f, 0f, 0.01745234f, -0.7069991f, 0.7069991f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 0.01745329f, 3.141593f)
            };
            // 632. - X: 90, Y: 1, Z: 180
            yield return new object[]
            {
                "X: 90, Y: 1, Z: 180",
                new float3(1.570796f, 0.01745329f, 3.141593f),
                new Quaternion(0.006170562f, -0.7070798f, 0.7070798f, 0.006170562f),
                new float4x4(-0.9998475f, -0.01745232f, 0f, 0f, 0f, 8.268398E-08f, -0.9999999f, 0f, 0.01745232f, -0.9998476f, 8.268398E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 3.159046f, 0f)
            };
            // 633. - X: 180, Y: 1, Z: 180
            yield return new object[]
            {
                "X: 180, Y: 1, Z: 180",
                new float3(3.141593f, 0.01745329f, 3.141593f),
                new Quaternion(-4.409117E-08f, -0.9999619f, -4.332827E-08f, 0.008726535f),
                new float4x4(-0.9998477f, 8.893519E-08f, -0.01745241f, 0f, 8.742277E-08f, 1f, 8.742277E-08f, 0f, 0.01745241f, 8.588372E-08f, -0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742277E-08f, 3.159046f, 8.742277E-08f)
            };
            // 634. - X: 270, Y: 1, Z: 180
            yield return new object[]
            {
                "X: 270, Y: 1, Z: 180",
                new float3(4.712389f, 0.01745329f, 3.141593f),
                new Quaternion(-0.006170623f, -0.7070798f, -0.7070798f, 0.006170623f),
                new float4x4(-0.9998475f, 0.01745249f, 0f, 0f, 0f, 8.116331E-08f, 0.9999999f, 0f, 0.01745249f, 0.9998476f, 8.116331E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.159046f, 0f)
            };
            // 635. - X: 360, Y: 1, Z: 180
            yield return new object[]
            {
                "X: 360, Y: 1, Z: 180",
                new float3(6.283185f, 0.01745329f, 3.141593f),
                new Quaternion(-0.008726535f, 8.780089E-08f, -0.9999619f, 4.294682E-08f),
                new float4x4(-0.9998477f, 8.435799E-08f, 0.01745241f, 0f, -8.742277E-08f, -0.9999999f, -1.748455E-07f, 0f, 0.01745241f, -1.763446E-07f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748455E-07f, 0.01745329f, 3.141593f)
            };
            // 636. - X: 585, Y: 1, Z: 180
            yield return new object[]
            {
                "X: 585, Y: 1, Z: 180",
                new float3(10.21018f, 0.01745329f, 3.141593f),
                new Quaternion(0.003339542f, 0.9238443f, 0.382669f, -0.008062284f),
                new float4x4(-0.9998477f, 0.01234081f, -0.01234071f, 0f, 6.146729E-08f, 0.7071065f, 0.707107f, 0f, 0.01745247f, 0.7069994f, -0.7069988f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.159046f, 8.69279E-08f)
            };
            // 637. - X: -90, Y: 1, Z: 180
            yield return new object[]
            {
                "X: -90, Y: 1, Z: 180",
                new float3(-1.570796f, 0.01745329f, 3.141593f),
                new Quaternion(0.006170623f, 0.7070798f, 0.7070798f, -0.006170623f),
                new float4x4(-0.9998475f, 0.01745249f, 0f, 0f, 0f, 8.116331E-08f, 0.9999999f, 0f, 0.01745249f, 0.9998476f, 8.116331E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.159046f, 0f)
            };
            // 638. - X: -540, Y: 1, Z: 180
            yield return new object[]
            {
                "X: -540, Y: 1, Z: 180",
                new float3(-9.424778f, 0.01745329f, 3.141593f),
                new Quaternion(-4.360566E-08f, -0.9999619f, 1.230588E-08f, 0.008726535f),
                new float4x4(-0.9998477f, 8.699323E-08f, -0.01745241f, 0f, 8.742278E-08f, 1f, -2.384976E-08f, 0f, 0.01745241f, -2.537187E-08f, -0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.159046f, 8.742278E-08f)
            };
            // 639. - X: 0, Y: 45, Z: 180
            yield return new object[]
            {
                "X: 0, Y: 45, Z: 180",
                new float3(0f, 0.7853982f, 3.141593f),
                new Quaternion(0.3826835f, -1.672763E-08f, 0.9238795f, -4.038406E-08f),
                new float4x4(-0.7071067f, 6.181723E-08f, 0.7071068f, 0f, -8.742278E-08f, -1f, 0f, 0f, 0.7071068f, -6.181724E-08f, 0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0.7853982f, 3.141593f)
            };
            // 640. - X: 0.1, Y: 45, Z: 180
            yield return new object[]
            {
                "X: 0.1, Y: 45, Z: 180",
                new float3(0.001745329f, 0.7853982f, 3.141593f),
                new Quaternion(0.3826833f, -0.0008062536f, 0.9238791f, 0.0003339139f),
                new float4x4(-0.7071067f, -0.001234072f, 0.7071057f, 0f, -8.742791E-08f, -0.9999985f, -0.001745328f, 0f, 0.7071068f, -0.001234195f, 0.7071057f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0.7853982f, 3.141593f)
            };
            // 641. - X: 1, Y: 45, Z: 180
            yield return new object[]
            {
                "X: 1, Y: 45, Z: 180",
                new float3(0.01745329f, 0.7853982f, 3.141593f),
                new Quaternion(0.3826689f, -0.008062284f, 0.9238443f, 0.00333946f),
                new float4x4(-0.7071068f, -0.01234065f, 0.7069991f, 0f, -8.707866E-08f, -0.9998477f, -0.01745241f, 0f, 0.7071068f, -0.01234077f, 0.7069991f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0.7853982f, 3.141593f)
            };
            // 642. - X: 45, Y: 45, Z: 180
            yield return new object[]
            {
                "X: 45, Y: 45, Z: 180",
                new float3(0.7853982f, 0.7853982f, 3.141593f),
                new Quaternion(0.3535534f, -0.3535534f, 0.8535534f, 0.1464466f),
                new float4x4(-0.7071068f, -0.5f, 0.5f, 0f, -7.450581E-08f, -0.7071067f, -0.7071068f, 0f, 0.7071068f, -0.5000001f, 0.4999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 0.7853982f, 3.141593f)
            };
            // 643. - X: 90, Y: 45, Z: 180
            yield return new object[]
            {
                "X: 90, Y: 45, Z: 180",
                new float3(1.570796f, 0.7853982f, 3.141593f),
                new Quaternion(0.270598f, -0.6532815f, 0.6532815f, 0.270598f),
                new float4x4(-0.7071066f, -0.7071066f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, 0.7071066f, -0.7071067f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 3.926991f, 0f)
            };
            // 644. - X: 180, Y: 45, Z: 180
            yield return new object[]
            {
                "X: 180, Y: 45, Z: 180",
                new float3(3.141593f, 0.7853982f, 3.141593f),
                new Quaternion(-5.711168E-08f, -0.9238795f, -2.365643E-08f, 0.3826835f),
                new float4x4(-0.7071067f, 1.236345E-07f, -0.7071068f, 0f, 8.742277E-08f, 1f, 8.742278E-08f, 0f, 0.7071068f, -1.065814E-14f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 3.926991f, 8.742277E-08f)
            };
            // 645. - X: 270, Y: 45, Z: 180
            yield return new object[]
            {
                "X: 270, Y: 45, Z: 180",
                new float3(4.712389f, 0.7853982f, 3.141593f),
                new Quaternion(-0.2705981f, -0.6532815f, -0.6532815f, 0.2705981f),
                new float4x4(-0.7071066f, 0.7071068f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, 0.7071068f, 0.7071066f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 3.926991f, 0f)
            };
            // 646. - X: 360, Y: 45, Z: 180
            yield return new object[]
            {
                "X: 360, Y: 45, Z: 180",
                new float3(6.283185f, 0.7853982f, 3.141593f),
                new Quaternion(-0.3826835f, 9.749574E-08f, -0.9238795f, 6.928804E-09f),
                new float4x4(-0.7071067f, -6.181725E-08f, 0.7071068f, 0f, -8.742277E-08f, -1f, -1.748455E-07f, 0f, 0.7071068f, -1.854517E-07f, 0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748455E-07f, 0.7853982f, 3.141593f)
            };
            // 647. - X: 585, Y: 45, Z: 180
            yield return new object[]
            {
                "X: 585, Y: 45, Z: 180",
                new float3(10.21018f, 0.7853982f, 3.141593f),
                new Quaternion(0.1464467f, 0.8535533f, 0.3535535f, -0.3535534f),
                new float4x4(-0.7071066f, 0.5000002f, -0.4999999f, 0f, 8.940697E-08f, 0.7071066f, 0.707107f, 0f, 0.7071069f, 0.5000001f, -0.4999998f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.926991f, 1.264406E-07f)
            };
            // 648. - X: -90, Y: 45, Z: 180
            yield return new object[]
            {
                "X: -90, Y: 45, Z: 180",
                new float3(-1.570796f, 0.7853982f, 3.141593f),
                new Quaternion(0.2705981f, 0.6532815f, 0.6532815f, -0.2705981f),
                new float4x4(-0.7071066f, 0.7071068f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, 0.7071068f, 0.7071066f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 3.926991f, 0f)
            };
            // 649. - X: -540, Y: 45, Z: 180
            yield return new object[]
            {
                "X: -540, Y: 45, Z: 180",
                new float3(-9.424778f, 0.7853982f, 3.141593f),
                new Quaternion(-3.58206E-08f, -0.9238795f, 2.774478E-08f, 0.3826835f),
                new float4x4(-0.7071067f, 4.49529E-08f, -0.7071068f, 0f, 8.742278E-08f, 1f, -2.384976E-08f, 0f, 0.7071068f, -7.868157E-08f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.926991f, 8.742278E-08f)
            };
            // 650. - X: 0, Y: 90, Z: 180
            yield return new object[]
            {
                "X: 0, Y: 90, Z: 180",
                new float3(0f, 1.570796f, 3.141593f),
                new Quaternion(0.7071068f, -3.090862E-08f, 0.7071068f, -3.090862E-08f),
                new float4x4(5.960464E-08f, 0f, 0.9999999f, 0f, -8.742278E-08f, -0.9999999f, 0f, 0f, 0.9999999f, -8.742278E-08f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.570796f, 3.141593f)
            };
            // 651. - X: 0.1, Y: 90, Z: 180
            yield return new object[]
            {
                "X: 0.1, Y: 90, Z: 180",
                new float3(0.001745329f, 1.570796f, 3.141593f),
                new Quaternion(0.7071065f, -0.0006170979f, 0.7071065f, 0.0006170361f),
                new float4x4(-4.636388E-08f, -0.001745328f, 0.9999985f, 0f, -8.73697E-08f, -0.9999986f, -0.001745328f, 0f, 1f, -8.73697E-08f, -4.636388E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 1.570796f, 3.141593f)
            };
            // 652. - X: 1, Y: 90, Z: 180
            yield return new object[]
            {
                "X: 1, Y: 90, Z: 180",
                new float3(0.01745329f, 1.570796f, 3.141593f),
                new Quaternion(0.7070798f, -0.006170623f, 0.7070798f, 0.006170562f),
                new float4x4(8.116331E-08f, -0.0174524f, 0.9998476f, 0f, -8.6613E-08f, -0.9998475f, -0.0174524f, 0f, 0.9999999f, -8.6613E-08f, 8.116331E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 1.570796f, 3.141593f)
            };
            // 653. - X: 45, Y: 90, Z: 180
            yield return new object[]
            {
                "X: 45, Y: 90, Z: 180",
                new float3(0.7853982f, 1.570796f, 3.141593f),
                new Quaternion(0.6532815f, -0.2705981f, 0.6532815f, 0.270598f),
                new float4x4(5.960464E-08f, -0.7071067f, 0.7071067f, 0f, -8.940697E-08f, -0.7071066f, -0.7071067f, 0f, 0.9999999f, -8.940697E-08f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 1.570796f, 3.141593f)
            };
            // 654. - X: 90, Y: 90, Z: 180
            yield return new object[]
            {
                "X: 90, Y: 90, Z: 180",
                new float3(1.570796f, 1.570796f, 3.141593f),
                new Quaternion(0.4999999f, -0.5f, 0.5f, 0.4999999f),
                new float4x4(0f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, 0.9999999f, -1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 4.712389f, 0f)
            };
            // 655. - X: 180, Y: 90, Z: 180
            yield return new object[]
            {
                "X: 180, Y: 90, Z: 180",
                new float3(3.141593f, 1.570796f, 3.141593f),
                new Quaternion(-6.181724E-08f, -0.7071068f, 0f, 0.7071068f),
                new float4x4(5.960464E-08f, 8.742278E-08f, -0.9999999f, 0f, 8.742278E-08f, 1f, 8.742278E-08f, 0f, 0.9999999f, -8.742278E-08f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742279E-08f, 4.712389f, 8.742278E-08f)
            };
            // 656. - X: 270, Y: 90, Z: 180
            yield return new object[]
            {
                "X: 270, Y: 90, Z: 180",
                new float3(4.712389f, 1.570796f, 3.141593f),
                new Quaternion(-0.5f, -0.4999999f, -0.4999999f, 0.5f),
                new float4x4(2.384186E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, 0.9999999f, -1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 657. - X: 360, Y: 90, Z: 180
            yield return new object[]
            {
                "X: 360, Y: 90, Z: 180",
                new float3(6.283185f, 1.570796f, 3.141593f),
                new Quaternion(-0.7071068f, 9.272586E-08f, -0.7071068f, -3.090862E-08f),
                new float4x4(5.960463E-08f, -1.748456E-07f, 0.9999999f, 0f, -8.742277E-08f, -0.9999999f, -1.748456E-07f, 0f, 0.9999999f, -8.742277E-08f, 5.960463E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 1.570796f, 3.141593f)
            };
            // 658. - X: 585, Y: 90, Z: 180
            yield return new object[]
            {
                "X: 585, Y: 90, Z: 180",
                new float3(10.21018f, 1.570796f, 3.141593f),
                new Quaternion(0.2705982f, 0.6532814f, 0.2705981f, -0.6532814f),
                new float4x4(1.192093E-07f, 0.707107f, -0.7071064f, 0f, 5.960464E-08f, 0.7071065f, 0.707107f, 0f, 0.9999999f, -5.960464E-08f, 4.470348E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 4.712389f, 1.264406E-07f)
            };
            // 659. - X: -90, Y: 90, Z: 180
            yield return new object[]
            {
                "X: -90, Y: 90, Z: 180",
                new float3(-1.570796f, 1.570796f, 3.141593f),
                new Quaternion(0.5f, 0.4999999f, 0.4999999f, -0.5f),
                new float4x4(2.384186E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, 0.9999999f, -1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 660. - X: -540, Y: 90, Z: 180
            yield return new object[]
            {
                "X: -540, Y: 90, Z: 180",
                new float3(-9.424778f, 1.570796f, 3.141593f),
                new Quaternion(-2.247646E-08f, -0.7071068f, 3.934078E-08f, 0.7071068f),
                new float4x4(5.960464E-08f, -2.384976E-08f, -0.9999999f, 0f, 8.742278E-08f, 1f, -2.384976E-08f, 0f, 0.9999999f, -8.742278E-08f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 4.712389f, 8.742278E-08f)
            };
            // 661. - X: 0, Y: 180, Z: 180
            yield return new object[]
            {
                "X: 0, Y: 180, Z: 180",
                new float3(0f, 3.141593f, 3.141593f),
                new Quaternion(1f, -4.371139E-08f, -4.371139E-08f, 1.910685E-15f),
                new float4x4(1f, -8.742278E-08f, -8.742278E-08f, 0f, -8.742278E-08f, -1f, 0f, 0f, -8.742278E-08f, 7.642742E-15f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.141593f, 3.141593f)
            };
            // 662. - X: 0.1, Y: 180, Z: 180
            yield return new object[]
            {
                "X: 0.1, Y: 180, Z: 180",
                new float3(0.001745329f, 3.141593f, 3.141593f),
                new Quaternion(0.9999996f, -4.367323E-08f, -4.367323E-08f, 0.0008726645f),
                new float4x4(1f, -8.72702E-08f, -8.742266E-08f, 0f, -8.742266E-08f, -0.9999986f, -0.001745328f, 0f, -8.72702E-08f, 0.001745328f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.141593f, 3.141593f)
            };
            // 663. - X: 1, Y: 180, Z: 180
            yield return new object[]
            {
                "X: 1, Y: 180, Z: 180",
                new float3(0.01745329f, 3.141593f, 3.141593f),
                new Quaternion(0.9999619f, -4.332827E-08f, -4.332827E-08f, 0.008726535f),
                new float4x4(1f, -8.589704E-08f, -8.740945E-08f, 0f, -8.740945E-08f, -0.9998477f, -0.01745241f, 0f, -8.589704E-08f, 0.01745241f, -0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.141593f, 3.141593f)
            };
            // 664. - X: 45, Y: 180, Z: 180
            yield return new object[]
            {
                "X: 45, Y: 180, Z: 180",
                new float3(0.7853982f, 3.141593f, 3.141593f),
                new Quaternion(0.9238795f, -2.365643E-08f, -2.365643E-08f, 0.3826835f),
                new float4x4(1f, -2.560553E-08f, -6.181723E-08f, 0f, -6.181723E-08f, -0.7071067f, -0.7071068f, 0f, -2.560553E-08f, 0.7071068f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 3.141593f, 3.141593f)
            };
            // 665. - X: 90, Y: 180, Z: 180
            yield return new object[]
            {
                "X: 90, Y: 180, Z: 180",
                new float3(1.570796f, 3.141593f, 3.141593f),
                new Quaternion(0.7071068f, 0f, 0f, 0.7071068f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 5.960464E-08f, -0.9999999f, 0f, 0f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 0f, 0f)
            };
            // 666. - X: 180, Y: 180, Z: 180
            yield return new object[]
            {
                "X: 180, Y: 180, Z: 180",
                new float3(3.141593f, 3.141593f, 3.141593f),
                new Quaternion(-4.371138E-08f, 4.371139E-08f, 4.371139E-08f, 1f),
                new float4x4(1f, -8.742279E-08f, 8.742278E-08f, 0f, 8.742278E-08f, 1f, 8.742278E-08f, 0f, -8.742279E-08f, -8.742276E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 8.742278E-08f, 8.742278E-08f)
            };
            // 667. - X: 270, Y: 180, Z: 180
            yield return new object[]
            {
                "X: 270, Y: 180, Z: 180",
                new float3(4.712389f, 3.141593f, 3.141593f),
                new Quaternion(-0.7071068f, 6.181724E-08f, 6.181724E-08f, 0.7071068f),
                new float4x4(1f, -1.748456E-07f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, -1.748456E-07f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.748456E-07f, 0f)
            };
            // 668. - X: 360, Y: 180, Z: 180
            yield return new object[]
            {
                "X: 360, Y: 180, Z: 180",
                new float3(6.283185f, 3.141593f, 3.141593f),
                new Quaternion(-1f, 4.371138E-08f, 4.371138E-08f, -8.742278E-08f),
                new float4x4(1f, -8.742276E-08f, -8.742278E-08f, 0f, -8.742278E-08f, -1f, -1.748456E-07f, 0f, -8.742276E-08f, 1.748456E-07f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 3.141593f, 3.141593f)
            };
            // 669. - X: 585, Y: 180, Z: 180
            yield return new object[]
            {
                "X: 585, Y: 180, Z: 180",
                new float3(10.21018f, 3.141593f, 3.141593f),
                new Quaternion(0.3826836f, -5.711168E-08f, -5.711168E-08f, -0.9238794f),
                new float4x4(1f, -1.4924E-07f, 6.181721E-08f, 0f, 6.181721E-08f, 0.7071065f, 0.707107f, 0f, -1.4924E-07f, -0.707107f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 8.742278E-08f, 8.742278E-08f)
            };
            // 670. - X: -90, Y: 180, Z: 180
            yield return new object[]
            {
                "X: -90, Y: 180, Z: 180",
                new float3(-1.570796f, 3.141593f, 3.141593f),
                new Quaternion(0.7071068f, -6.181724E-08f, -6.181724E-08f, -0.7071068f),
                new float4x4(1f, -1.748456E-07f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, -1.748456E-07f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.748456E-07f, 0f)
            };
            // 671. - X: -540, Y: 180, Z: 180
            yield return new object[]
            {
                "X: -540, Y: 180, Z: 180",
                new float3(-9.424778f, 3.141593f, 3.141593f),
                new Quaternion(1.192488E-08f, 4.371139E-08f, 4.371139E-08f, 1f),
                new float4x4(1f, -8.742278E-08f, 8.742278E-08f, 0f, 8.742278E-08f, 1f, -2.384976E-08f, 0f, -8.742278E-08f, 2.384977E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 8.742278E-08f, 8.742278E-08f)
            };
            // 672. - X: 0, Y: 270, Z: 180
            yield return new object[]
            {
                "X: 0, Y: 270, Z: 180",
                new float3(0f, 4.712389f, 3.141593f),
                new Quaternion(0.7071068f, -3.090862E-08f, -0.7071068f, 3.090862E-08f),
                new float4x4(5.960464E-08f, 0f, -0.9999999f, 0f, -8.742278E-08f, -0.9999999f, 0f, 0f, -0.9999999f, 8.742278E-08f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 3.141593f)
            };
            // 673. - X: 0.1, Y: 270, Z: 180
            yield return new object[]
            {
                "X: 0.1, Y: 270, Z: 180",
                new float3(0.001745329f, 4.712389f, 3.141593f),
                new Quaternion(0.7071065f, 0.0006170361f, -0.7071065f, 0.0006170979f),
                new float4x4(-4.621131E-08f, 0.001745328f, -0.9999985f, 0f, -8.73697E-08f, -0.9999986f, -0.001745328f, 0f, -1f, 8.73697E-08f, -4.621131E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 4.712389f, 3.141593f)
            };
            // 674. - X: 1, Y: 270, Z: 180
            yield return new object[]
            {
                "X: 1, Y: 270, Z: 180",
                new float3(0.01745329f, 4.712389f, 3.141593f),
                new Quaternion(0.7070798f, 0.006170562f, -0.7070798f, 0.006170623f),
                new float4x4(8.268398E-08f, 0.0174524f, -0.9998476f, 0f, -8.6613E-08f, -0.9998475f, -0.0174524f, 0f, -0.9999999f, 8.6613E-08f, 8.268398E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 4.712389f, 3.141593f)
            };
            // 675. - X: 45, Y: 270, Z: 180
            yield return new object[]
            {
                "X: 45, Y: 270, Z: 180",
                new float3(0.7853982f, 4.712389f, 3.141593f),
                new Quaternion(0.6532815f, 0.270598f, -0.6532815f, 0.2705981f),
                new float4x4(1.192093E-07f, 0.7071067f, -0.7071067f, 0f, -8.940697E-08f, -0.7071066f, -0.7071067f, 0f, -0.9999999f, 8.940697E-08f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 4.712389f, 3.141593f)
            };
            // 676. - X: 90, Y: 270, Z: 180
            yield return new object[]
            {
                "X: 90, Y: 270, Z: 180",
                new float3(1.570796f, 4.712389f, 3.141593f),
                new Quaternion(0.5f, 0.4999999f, -0.4999999f, 0.5f),
                new float4x4(2.384186E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 1.570796f, 0f)
            };
            // 677. - X: 180, Y: 270, Z: 180
            yield return new object[]
            {
                "X: 180, Y: 270, Z: 180",
                new float3(3.141593f, 4.712389f, 3.141593f),
                new Quaternion(0f, 0.7071068f, 6.181724E-08f, 0.7071068f),
                new float4x4(5.960464E-08f, -8.742278E-08f, 0.9999999f, 0f, 8.742278E-08f, 1f, 8.742278E-08f, 0f, -0.9999999f, 8.742278E-08f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742279E-08f, 1.570796f, 8.742278E-08f)
            };
            // 678. - X: 270, Y: 270, Z: 180
            yield return new object[]
            {
                "X: 270, Y: 270, Z: 180",
                new float3(4.712389f, 4.712389f, 3.141593f),
                new Quaternion(-0.4999999f, 0.5f, 0.5f, 0.4999999f),
                new float4x4(0f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 679. - X: 360, Y: 270, Z: 180
            yield return new object[]
            {
                "X: 360, Y: 270, Z: 180",
                new float3(6.283185f, 4.712389f, 3.141593f),
                new Quaternion(-0.7071068f, -3.090862E-08f, 0.7071068f, -9.272586E-08f),
                new float4x4(5.960464E-08f, 1.748456E-07f, -0.9999999f, 0f, -8.742277E-08f, -0.9999999f, -1.748456E-07f, 0f, -0.9999999f, 8.742277E-08f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 4.712389f, 3.141593f)
            };
            // 680. - X: 585, Y: 270, Z: 180
            yield return new object[]
            {
                "X: 585, Y: 270, Z: 180",
                new float3(10.21018f, 4.712389f, 3.141593f),
                new Quaternion(0.2705981f, -0.6532814f, -0.2705982f, -0.6532814f),
                new float4x4(4.470348E-08f, -0.707107f, 0.7071064f, 0f, 5.960464E-08f, 0.7071065f, 0.707107f, 0f, -0.9999999f, 5.960464E-08f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.570796f, 1.264406E-07f)
            };
            // 681. - X: -90, Y: 270, Z: 180
            yield return new object[]
            {
                "X: -90, Y: 270, Z: 180",
                new float3(-1.570796f, 4.712389f, 3.141593f),
                new Quaternion(0.4999999f, -0.5f, -0.5f, -0.4999999f),
                new float4x4(0f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 682. - X: -540, Y: 270, Z: 180
            yield return new object[]
            {
                "X: -540, Y: 270, Z: 180",
                new float3(-9.424778f, 4.712389f, 3.141593f),
                new Quaternion(3.934078E-08f, 0.7071068f, 2.247646E-08f, 0.7071068f),
                new float4x4(5.960464E-08f, 2.384976E-08f, 0.9999999f, 0f, 8.742278E-08f, 1f, -2.384976E-08f, 0f, -0.9999999f, 8.742278E-08f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 1.570796f, 8.742278E-08f)
            };
            // 683. - X: 0, Y: 360, Z: 180
            yield return new object[]
            {
                "X: 0, Y: 360, Z: 180",
                new float3(0f, 6.283185f, 3.141593f),
                new Quaternion(-8.742278E-08f, 3.821371E-15f, -1f, 4.371139E-08f),
                new float4x4(-1f, 8.742278E-08f, 1.748456E-07f, 0f, -8.742278E-08f, -1f, 0f, 0f, 1.748456E-07f, -1.528548E-14f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.748456E-07f, 3.141593f)
            };
            // 684. - X: 0.1, Y: 360, Z: 180
            yield return new object[]
            {
                "X: 0.1, Y: 360, Z: 180",
                new float3(0.001745329f, 6.283185f, 3.141593f),
                new Quaternion(-8.738461E-08f, 0.0008726645f, -0.9999996f, 4.363508E-08f),
                new float4x4(-1f, 8.711762E-08f, 1.748453E-07f, 0f, -8.742266E-08f, -0.9999986f, -0.001745328f, 0f, 1.74693E-07f, -0.001745328f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 1.748456E-07f, 3.141593f)
            };
            // 685. - X: 1, Y: 360, Z: 180
            yield return new object[]
            {
                "X: 1, Y: 360, Z: 180",
                new float3(0.01745329f, 6.283185f, 3.141593f),
                new Quaternion(-8.7038E-08f, 0.008726535f, -0.9999619f, 4.294682E-08f),
                new float4x4(-0.9999999f, 8.437129E-08f, 1.748189E-07f, 0f, -8.740946E-08f, -0.9998477f, -0.01745241f, 0f, 1.733198E-07f, -0.01745241f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 1.748455E-07f, 3.141593f)
            };
            // 686. - X: 45, Y: 360, Z: 180
            yield return new object[]
            {
                "X: 45, Y: 360, Z: 180",
                new float3(0.7853982f, 6.283185f, 3.141593f),
                new Quaternion(-6.404048E-08f, 0.3826835f, -0.9238795f, 6.928804E-09f),
                new float4x4(-1f, -3.621171E-08f, 1.236345E-07f, 0f, -6.181723E-08f, -0.7071067f, -0.7071068f, 0f, 1.130283E-07f, -0.7071068f, 0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 1.748456E-07f, 3.141593f)
            };
            // 687. - X: 90, Y: 360, Z: 180
            yield return new object[]
            {
                "X: 90, Y: 360, Z: 180",
                new float3(1.570796f, 6.283185f, 3.141593f),
                new Quaternion(-3.090862E-08f, 0.7071068f, -0.7071068f, -3.090862E-08f),
                new float4x4(-0.9999999f, -8.742278E-08f, 0f, 0f, 0f, 5.960464E-08f, -0.9999999f, 0f, 8.742278E-08f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 3.141593f, 0f)
            };
            // 688. - X: 180, Y: 360, Z: 180
            yield return new object[]
            {
                "X: 180, Y: 360, Z: 180",
                new float3(3.141593f, 6.283185f, 3.141593f),
                new Quaternion(4.371139E-08f, 1f, 4.371138E-08f, -8.742278E-08f),
                new float4x4(-1f, 8.742279E-08f, -1.748456E-07f, 0f, 8.742278E-08f, 1f, 8.742278E-08f, 0f, 1.748456E-07f, 8.742276E-08f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 3.141593f, 8.742278E-08f)
            };
            // 689. - X: 270, Y: 360, Z: 180
            yield return new object[]
            {
                "X: 270, Y: 360, Z: 180",
                new float3(4.712389f, 6.283185f, 3.141593f),
                new Quaternion(9.272586E-08f, 0.7071068f, 0.7071068f, -9.272586E-08f),
                new float4x4(-0.9999999f, 2.622683E-07f, 0f, 0f, 0f, 5.960463E-08f, 0.9999999f, 0f, 2.622683E-07f, 0.9999999f, 5.960463E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.141593f, 0f)
            };
            // 690. - X: 360, Y: 360, Z: 180
            yield return new object[]
            {
                "X: 360, Y: 360, Z: 180",
                new float3(6.283185f, 6.283185f, 3.141593f),
                new Quaternion(8.742277E-08f, -8.742278E-08f, 1f, -4.371138E-08f),
                new float4x4(-1f, 8.742275E-08f, 1.748456E-07f, 0f, -8.742278E-08f, -1f, -1.748456E-07f, 0f, 1.748455E-07f, -1.748456E-07f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 1.748456E-07f, 3.141593f)
            };
            // 691. - X: 585, Y: 360, Z: 180
            yield return new object[]
            {
                "X: 585, Y: 360, Z: 180",
                new float3(10.21018f, 6.283185f, 3.141593f),
                new Quaternion(-7.383932E-08f, -0.9238794f, -0.3826836f, 9.749574E-08f),
                new float4x4(-0.9999999f, 2.110573E-07f, -1.236344E-07f, 0f, 6.181721E-08f, 0.7071065f, 0.707107f, 0f, 2.366628E-07f, 0.707107f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.141593f, 8.742278E-08f)
            };
            // 692. - X: -90, Y: 360, Z: 180
            yield return new object[]
            {
                "X: -90, Y: 360, Z: 180",
                new float3(-1.570796f, 6.283185f, 3.141593f),
                new Quaternion(-9.272586E-08f, -0.7071068f, -0.7071068f, 9.272586E-08f),
                new float4x4(-0.9999999f, 2.622683E-07f, 0f, 0f, 0f, 5.960463E-08f, 0.9999999f, 0f, 2.622683E-07f, 0.9999999f, 5.960463E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.141593f, 0f)
            };
            // 693. - X: -540, Y: 360, Z: 180
            yield return new object[]
            {
                "X: -540, Y: 360, Z: 180",
                new float3(-9.424778f, 6.283185f, 3.141593f),
                new Quaternion(4.371139E-08f, 1f, -1.192488E-08f, -8.742278E-08f),
                new float4x4(-1f, 8.742278E-08f, -1.748456E-07f, 0f, 8.742278E-08f, 1f, -2.384976E-08f, 0f, 1.748456E-07f, -2.384978E-08f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.141593f, 8.742278E-08f)
            };
            // 694. - X: 0, Y: 585, Z: 180
            yield return new object[]
            {
                "X: 0, Y: 585, Z: 180",
                new float3(0f, 10.21018f, 3.141593f),
                new Quaternion(-0.9238794f, 4.038405E-08f, 0.3826836f, -1.672763E-08f),
                new float4x4(0.7071065f, -6.181721E-08f, -0.707107f, 0f, -8.742277E-08f, -0.9999999f, 0f, 0f, -0.707107f, 6.181726E-08f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.926991f, 3.141593f)
            };
            // 695. - X: 0.1, Y: 585, Z: 180
            yield return new object[]
            {
                "X: 0.1, Y: 585, Z: 180",
                new float3(0.001745329f, 10.21018f, 3.141593f),
                new Quaternion(-0.9238791f, -0.000333914f, 0.3826835f, -0.0008062535f),
                new float4x4(0.7071065f, 0.001234072f, -0.7071059f, 0f, -8.742791E-08f, -0.9999985f, -0.001745328f, 0f, -0.707107f, 0.001234195f, -0.7071054f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.926991f, 3.141593f)
            };
            // 696. - X: 1, Y: 585, Z: 180
            yield return new object[]
            {
                "X: 1, Y: 585, Z: 180",
                new float3(0.01745329f, 10.21018f, 3.141593f),
                new Quaternion(-0.9238443f, -0.003339462f, 0.382669f, -0.008062284f),
                new float4x4(0.7071065f, 0.01234066f, -0.7069994f, 0f, -8.707866E-08f, -0.9998477f, -0.01745241f, 0f, -0.707107f, 0.01234077f, -0.7069988f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.926991f, 3.141593f)
            };
            // 697. - X: 45, Y: 585, Z: 180
            yield return new object[]
            {
                "X: 45, Y: 585, Z: 180",
                new float3(0.7853982f, 10.21018f, 3.141593f),
                new Quaternion(-0.8535533f, -0.1464466f, 0.3535535f, -0.3535534f),
                new float4x4(0.7071066f, 0.5000001f, -0.5000001f, 0f, -5.960464E-08f, -0.7071066f, -0.7071068f, 0f, -0.7071069f, 0.4999999f, -0.4999997f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853984f, 3.926991f, 3.141593f)
            };
            // 698. - X: 90, Y: 585, Z: 180
            yield return new object[]
            {
                "X: 90, Y: 585, Z: 180",
                new float3(1.570796f, 10.21018f, 3.141593f),
                new Quaternion(-0.6532814f, -0.2705981f, 0.2705981f, -0.6532814f),
                new float4x4(0.7071066f, 0.7071069f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, -0.7071069f, 0.7071065f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 0.7853985f, 0f)
            };
            // 699. - X: 180, Y: 585, Z: 180
            yield return new object[]
            {
                "X: 180, Y: 585, Z: 180",
                new float3(3.141593f, 10.21018f, 3.141593f),
                new Quaternion(2.365642E-08f, -0.3826836f, -5.711168E-08f, -0.9238794f),
                new float4x4(0.7071065f, -1.236345E-07f, 0.707107f, 0f, 8.742277E-08f, 1f, 8.742278E-08f, 0f, -0.707107f, 4.618528E-14f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 0.7853985f, 8.742277E-08f)
            };
            // 700. - X: 270, Y: 585, Z: 180
            yield return new object[]
            {
                "X: 270, Y: 585, Z: 180",
                new float3(4.712389f, 10.21018f, 3.141593f),
                new Quaternion(0.6532814f, -0.2705982f, -0.2705982f, -0.6532814f),
                new float4x4(0.7071065f, -0.7071071f, 0f, 0f, 0f, 4.470348E-08f, 0.9999999f, 0f, -0.7071071f, -0.7071064f, 4.470348E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.7853988f, 0f)
            };
            // 701. - X: 360, Y: 585, Z: 180
            yield return new object[]
            {
                "X: 360, Y: 585, Z: 180",
                new float3(6.283185f, 10.21018f, 3.141593f),
                new Quaternion(0.9238794f, -6.92879E-09f, -0.3826836f, 9.749574E-08f),
                new float4x4(0.7071065f, 6.18173E-08f, -0.707107f, 0f, -8.742278E-08f, -0.9999999f, -1.748456E-07f, 0f, -0.707107f, 1.854517E-07f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 3.926991f, 3.141593f)
            };
            // 702. - X: 585, Y: 585, Z: 180
            yield return new object[]
            {
                "X: 585, Y: 585, Z: 180",
                new float3(10.21018f, 10.21018f, 3.141593f),
                new Quaternion(-0.3535535f, 0.3535535f, 0.1464468f, 0.8535532f),
                new float4x4(0.7071065f, -0.5000004f, 0.5f, 0f, 8.940697E-08f, 0.7071066f, 0.7071069f, 0f, -0.7071071f, -0.4999999f, 0.4999997f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 0.7853985f, 1.264406E-07f)
            };
            // 703. - X: -90, Y: 585, Z: 180
            yield return new object[]
            {
                "X: -90, Y: 585, Z: 180",
                new float3(-1.570796f, 10.21018f, 3.141593f),
                new Quaternion(-0.6532814f, 0.2705982f, 0.2705982f, 0.6532814f),
                new float4x4(0.7071065f, -0.7071071f, 0f, 0f, 0f, 4.470348E-08f, 0.9999999f, 0f, -0.7071071f, -0.7071064f, 4.470348E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.7853988f, 0f)
            };
            // 704. - X: -540, Y: 585, Z: 180
            yield return new object[]
            {
                "X: -540, Y: 585, Z: 180",
                new float3(-9.424778f, 10.21018f, 3.141593f),
                new Quaternion(-2.774478E-08f, -0.3826836f, -3.582059E-08f, -0.9238794f),
                new float4x4(0.7071065f, -4.495288E-08f, 0.707107f, 0f, 8.742277E-08f, 1f, -2.384976E-08f, 0f, -0.707107f, 7.868158E-08f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 0.7853985f, 8.742277E-08f)
            };
            // 705. - X: 0, Y: -90, Z: 180
            yield return new object[]
            {
                "X: 0, Y: -90, Z: 180",
                new float3(0f, -1.570796f, 3.141593f),
                new Quaternion(-0.7071068f, 3.090862E-08f, 0.7071068f, -3.090862E-08f),
                new float4x4(5.960464E-08f, 0f, -0.9999999f, 0f, -8.742278E-08f, -0.9999999f, 0f, 0f, -0.9999999f, 8.742278E-08f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 3.141593f)
            };
            // 706. - X: 0.1, Y: -90, Z: 180
            yield return new object[]
            {
                "X: 0.1, Y: -90, Z: 180",
                new float3(0.001745329f, -1.570796f, 3.141593f),
                new Quaternion(-0.7071065f, -0.0006170361f, 0.7071065f, -0.0006170979f),
                new float4x4(-4.621131E-08f, 0.001745328f, -0.9999985f, 0f, -8.73697E-08f, -0.9999986f, -0.001745328f, 0f, -1f, 8.73697E-08f, -4.621131E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 4.712389f, 3.141593f)
            };
            // 707. - X: 1, Y: -90, Z: 180
            yield return new object[]
            {
                "X: 1, Y: -90, Z: 180",
                new float3(0.01745329f, -1.570796f, 3.141593f),
                new Quaternion(-0.7070798f, -0.006170562f, 0.7070798f, -0.006170623f),
                new float4x4(8.268398E-08f, 0.0174524f, -0.9998476f, 0f, -8.6613E-08f, -0.9998475f, -0.0174524f, 0f, -0.9999999f, 8.6613E-08f, 8.268398E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 4.712389f, 3.141593f)
            };
            // 708. - X: 45, Y: -90, Z: 180
            yield return new object[]
            {
                "X: 45, Y: -90, Z: 180",
                new float3(0.7853982f, -1.570796f, 3.141593f),
                new Quaternion(-0.6532815f, -0.270598f, 0.6532815f, -0.2705981f),
                new float4x4(1.192093E-07f, 0.7071067f, -0.7071067f, 0f, -8.940697E-08f, -0.7071066f, -0.7071067f, 0f, -0.9999999f, 8.940697E-08f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 4.712389f, 3.141593f)
            };
            // 709. - X: 90, Y: -90, Z: 180
            yield return new object[]
            {
                "X: 90, Y: -90, Z: 180",
                new float3(1.570796f, -1.570796f, 3.141593f),
                new Quaternion(-0.5f, -0.4999999f, 0.4999999f, -0.5f),
                new float4x4(2.384186E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 1.570796f, 0f)
            };
            // 710. - X: 180, Y: -90, Z: 180
            yield return new object[]
            {
                "X: 180, Y: -90, Z: 180",
                new float3(3.141593f, -1.570796f, 3.141593f),
                new Quaternion(0f, -0.7071068f, -6.181724E-08f, -0.7071068f),
                new float4x4(5.960464E-08f, -8.742278E-08f, 0.9999999f, 0f, 8.742278E-08f, 1f, 8.742278E-08f, 0f, -0.9999999f, 8.742278E-08f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742279E-08f, 1.570796f, 8.742278E-08f)
            };
            // 711. - X: 270, Y: -90, Z: 180
            yield return new object[]
            {
                "X: 270, Y: -90, Z: 180",
                new float3(4.712389f, -1.570796f, 3.141593f),
                new Quaternion(0.4999999f, -0.5f, -0.5f, -0.4999999f),
                new float4x4(0f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 712. - X: 360, Y: -90, Z: 180
            yield return new object[]
            {
                "X: 360, Y: -90, Z: 180",
                new float3(6.283185f, -1.570796f, 3.141593f),
                new Quaternion(0.7071068f, 3.090862E-08f, -0.7071068f, 9.272586E-08f),
                new float4x4(5.960464E-08f, 1.748456E-07f, -0.9999999f, 0f, -8.742277E-08f, -0.9999999f, -1.748456E-07f, 0f, -0.9999999f, 8.742277E-08f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 4.712389f, 3.141593f)
            };
            // 713. - X: 585, Y: -90, Z: 180
            yield return new object[]
            {
                "X: 585, Y: -90, Z: 180",
                new float3(10.21018f, -1.570796f, 3.141593f),
                new Quaternion(-0.2705981f, 0.6532814f, 0.2705982f, 0.6532814f),
                new float4x4(4.470348E-08f, -0.707107f, 0.7071064f, 0f, 5.960464E-08f, 0.7071065f, 0.707107f, 0f, -0.9999999f, 5.960464E-08f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.570796f, 1.264406E-07f)
            };
            // 714. - X: -90, Y: -90, Z: 180
            yield return new object[]
            {
                "X: -90, Y: -90, Z: 180",
                new float3(-1.570796f, -1.570796f, 3.141593f),
                new Quaternion(-0.4999999f, 0.5f, 0.5f, 0.4999999f),
                new float4x4(0f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 715. - X: -540, Y: -90, Z: 180
            yield return new object[]
            {
                "X: -540, Y: -90, Z: 180",
                new float3(-9.424778f, -1.570796f, 3.141593f),
                new Quaternion(-3.934078E-08f, -0.7071068f, -2.247646E-08f, -0.7071068f),
                new float4x4(5.960464E-08f, 2.384976E-08f, 0.9999999f, 0f, 8.742278E-08f, 1f, -2.384976E-08f, 0f, -0.9999999f, 8.742278E-08f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 1.570796f, 8.742278E-08f)
            };
            // 716. - X: 0, Y: -540, Z: 180
            yield return new object[]
            {
                "X: 0, Y: -540, Z: 180",
                new float3(0f, -9.424778f, 3.141593f),
                new Quaternion(1f, -4.371139E-08f, 1.192488E-08f, -5.212531E-16f),
                new float4x4(1f, -8.742278E-08f, 2.384976E-08f, 0f, -8.742278E-08f, -1f, 0f, 0f, 2.384976E-08f, -2.085012E-15f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.141593f, 3.141593f)
            };
            // 717. - X: 0.1, Y: -540, Z: 180
            yield return new object[]
            {
                "X: 0.1, Y: -540, Z: 180",
                new float3(0.001745329f, -9.424778f, 3.141593f),
                new Quaternion(0.9999996f, -4.372178E-08f, 1.196302E-08f, 0.0008726645f),
                new float4x4(1f, -8.746441E-08f, 2.384973E-08f, 0f, -8.742265E-08f, -0.9999986f, -0.001745328f, 0f, 2.400234E-08f, 0.001745328f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.141593f, 3.141593f)
            };
            // 718. - X: 1, Y: -540, Z: 180
            yield return new object[]
            {
                "X: 1, Y: -540, Z: 180",
                new float3(0.01745329f, -9.424778f, 3.141593f),
                new Quaternion(0.9999619f, -4.381378E-08f, 1.230588E-08f, 0.008726535f),
                new float4x4(1f, -8.783901E-08f, 2.384613E-08f, 0f, -8.740945E-08f, -0.9998477f, -0.01745241f, 0f, 2.53755E-08f, 0.01745241f, -0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.141593f, 3.141593f)
            };
            // 719. - X: 45, Y: -540, Z: 180
            yield return new object[]
            {
                "X: 45, Y: -540, Z: 180",
                new float3(0.7853982f, -9.424778f, 3.141593f),
                new Quaternion(0.9238795f, -4.494751E-08f, 2.774478E-08f, 0.3826835f),
                new float4x4(1f, -1.042871E-07f, 1.686433E-08f, 0f, -6.181723E-08f, -0.7071067f, -0.7071068f, 0f, 8.5667E-08f, 0.7071068f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 3.141593f, 3.141593f)
            };
            // 720. - X: 90, Y: -540, Z: 180
            yield return new object[]
            {
                "X: 90, Y: -540, Z: 180",
                new float3(1.570796f, -9.424778f, 3.141593f),
                new Quaternion(0.7071068f, -3.934078E-08f, 3.934078E-08f, 0.7071068f),
                new float4x4(1f, -1.112725E-07f, 0f, 0f, 0f, 5.960464E-08f, -0.9999999f, 0f, 1.112725E-07f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, -1.112725E-07f, 0f)
            };
            // 721. - X: 180, Y: -540, Z: 180
            yield return new object[]
            {
                "X: 180, Y: -540, Z: 180",
                new float3(3.141593f, -9.424778f, 3.141593f),
                new Quaternion(-4.371139E-08f, -1.192488E-08f, 4.371139E-08f, 1f),
                new float4x4(1f, -8.742278E-08f, -2.384976E-08f, 0f, 8.742278E-08f, 1f, 8.742278E-08f, 0f, 2.384975E-08f, -8.742278E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, -2.384976E-08f, 8.742278E-08f)
            };
            // 722. - X: 270, Y: -540, Z: 180
            yield return new object[]
            {
                "X: 270, Y: -540, Z: 180",
                new float3(4.712389f, -9.424778f, 3.141593f),
                new Quaternion(-0.7071068f, 2.247646E-08f, 2.247646E-08f, 0.7071068f),
                new float4x4(1f, -6.357302E-08f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, -6.357302E-08f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 6.357302E-08f, 0f)
            };
            // 723. - X: 360, Y: -540, Z: 180
            yield return new object[]
            {
                "X: 360, Y: -540, Z: 180",
                new float3(6.283185f, -9.424778f, 3.141593f),
                new Quaternion(-1f, 4.371139E-08f, -1.192488E-08f, -8.742278E-08f),
                new float4x4(1f, -8.742278E-08f, 2.384976E-08f, 0f, -8.742278E-08f, -1f, -1.748456E-07f, 0f, 2.384978E-08f, 1.748456E-07f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 3.141593f, 3.141593f)
            };
            // 724. - X: 585, Y: -540, Z: 180
            yield return new object[]
            {
                "X: 585, Y: -540, Z: 180",
                new float3(10.21018f, -9.424778f, 3.141593f),
                new Quaternion(0.3826836f, -5.710478E-09f, -3.582059E-08f, -0.9238794f),
                new float4x4(1f, -7.055844E-08f, -1.686432E-08f, 0f, 6.181721E-08f, 0.7071065f, 0.707107f, 0f, -3.79675E-08f, -0.707107f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, -2.384976E-08f, 8.742278E-08f)
            };
            // 725. - X: -90, Y: -540, Z: 180
            yield return new object[]
            {
                "X: -90, Y: -540, Z: 180",
                new float3(-1.570796f, -9.424778f, 3.141593f),
                new Quaternion(0.7071068f, -2.247646E-08f, -2.247646E-08f, -0.7071068f),
                new float4x4(1f, -6.357302E-08f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, -6.357302E-08f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 6.357302E-08f, 0f)
            };
            // 726. - X: -540, Y: -540, Z: 180
            yield return new object[]
            {
                "X: -540, Y: -540, Z: 180",
                new float3(-9.424778f, -9.424778f, 3.141593f),
                new Quaternion(1.192488E-08f, -1.192488E-08f, 4.371139E-08f, 1f),
                new float4x4(1f, -8.742278E-08f, -2.384976E-08f, 0f, 8.742278E-08f, 1f, -2.384976E-08f, 0f, 2.384976E-08f, 2.384976E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, -2.384976E-08f, 8.742278E-08f)
            };
            // 727. - X: 0, Y: 0, Z: 270
            yield return new object[]
            {
                "X: 0, Y: 0, Z: 270",
                new float3(0f, 0f, 4.712389f),
                new Quaternion(0f, 0f, 0.7071068f, -0.7071068f),
                new float4x4(5.960464E-08f, 0.9999999f, 0f, 0f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0f, 4.712389f)
            };
            // 728. - X: 0.1, Y: 0, Z: 270
            yield return new object[]
            {
                "X: 0.1, Y: 0, Z: 270",
                new float3(0.001745329f, 0f, 4.712389f),
                new Quaternion(-0.000617067f, -0.000617067f, 0.7071065f, -0.7071065f),
                new float4x4(-4.62876E-08f, 1f, 0f, 0f, -0.9999985f, -4.62876E-08f, -0.001745328f, 0f, -0.001745328f, 0f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0f, 4.712389f)
            };
            // 729. - X: 1, Y: 0, Z: 270
            yield return new object[]
            {
                "X: 1, Y: 0, Z: 270",
                new float3(0.01745329f, 0f, 4.712389f),
                new Quaternion(-0.006170592f, -0.006170592f, 0.7070798f, -0.7070798f),
                new float4x4(8.192001E-08f, 0.9999999f, 0f, 0f, -0.9998476f, 8.192001E-08f, -0.01745241f, 0f, -0.01745241f, 0f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0f, 4.712389f)
            };
            // 730. - X: 45, Y: 0, Z: 270
            yield return new object[]
            {
                "X: 45, Y: 0, Z: 270",
                new float3(0.7853982f, 0f, 4.712389f),
                new Quaternion(-0.2705981f, -0.2705981f, 0.6532815f, -0.6532815f),
                new float4x4(8.940697E-08f, 0.9999999f, 0f, 0f, -0.7071067f, 8.940697E-08f, -0.7071068f, 0f, -0.7071068f, 0f, 0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 0f, 4.712389f)
            };
            // 731. - X: 90, Y: 0, Z: 270
            yield return new object[]
            {
                "X: 90, Y: 0, Z: 270",
                new float3(1.570796f, 0f, 4.712389f),
                new Quaternion(-0.5f, -0.5f, 0.5f, -0.5f),
                new float4x4(1.192093E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, -0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 1.570796f, 0f)
            };
            // 732. - X: 180, Y: 0, Z: 270
            yield return new object[]
            {
                "X: 180, Y: 0, Z: 270",
                new float3(3.141593f, 0f, 4.712389f),
                new Quaternion(-0.7071068f, -0.7071068f, -3.090862E-08f, 3.090862E-08f),
                new float4x4(5.960464E-08f, 0.9999999f, 0f, 0f, 0.9999999f, 5.960464E-08f, 8.742278E-08f, 0f, 8.742278E-08f, 0f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742279E-08f, 3.141593f, 1.570796f)
            };
            // 733. - X: 270, Y: 0, Z: 270
            yield return new object[]
            {
                "X: 270, Y: 0, Z: 270",
                new float3(4.712389f, 0f, 4.712389f),
                new Quaternion(-0.5f, -0.5f, -0.5f, 0.5f),
                new float4x4(1.192093E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, 0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 734. - X: 360, Y: 0, Z: 270
            yield return new object[]
            {
                "X: 360, Y: 0, Z: 270",
                new float3(6.283185f, 0f, 4.712389f),
                new Quaternion(6.181724E-08f, 6.181724E-08f, -0.7071068f, 0.7071068f),
                new float4x4(5.960464E-08f, 0.9999999f, 0f, 0f, -0.9999999f, 5.960464E-08f, -1.748456E-07f, 0f, -1.748456E-07f, 0f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 0f, 4.712389f)
            };
            // 735. - X: 585, Y: 0, Z: 270
            yield return new object[]
            {
                "X: 585, Y: 0, Z: 270",
                new float3(10.21018f, 0f, 4.712389f),
                new Quaternion(0.6532814f, 0.6532814f, 0.2705982f, -0.2705982f),
                new float4x4(7.450581E-08f, 0.9999999f, 0f, 0f, 0.7071064f, 7.450581E-08f, 0.707107f, 0f, 0.707107f, 0f, -0.7071064f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.141593f, 1.570796f)
            };
            // 736. - X: -90, Y: 0, Z: 270
            yield return new object[]
            {
                "X: -90, Y: 0, Z: 270",
                new float3(-1.570796f, 0f, 4.712389f),
                new Quaternion(0.5f, 0.5f, 0.5f, -0.5f),
                new float4x4(1.192093E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, 0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 737. - X: -540, Y: 0, Z: 270
            yield return new object[]
            {
                "X: -540, Y: 0, Z: 270",
                new float3(-9.424778f, 0f, 4.712389f),
                new Quaternion(-0.7071068f, -0.7071068f, 8.432163E-09f, -8.432163E-09f),
                new float4x4(5.960464E-08f, 0.9999999f, 0f, 0f, 0.9999999f, 5.960464E-08f, -2.384976E-08f, 0f, -2.384976E-08f, 0f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.141593f, 1.570796f)
            };
            // 738. - X: 0, Y: 0.1, Z: 270
            yield return new object[]
            {
                "X: 0, Y: 0.1, Z: 270",
                new float3(0f, 0.001745329f, 4.712389f),
                new Quaternion(0.000617067f, -0.000617067f, 0.7071065f, -0.7071065f),
                new float4x4(-4.62876E-08f, 0.9999985f, 0.001745328f, 0f, -1f, -4.62876E-08f, 0f, 0f, 0f, -0.001745328f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0.001745329f, 4.712389f)
            };
            // 739. - X: 0.1, Y: 0.1, Z: 270
            yield return new object[]
            {
                "X: 0.1, Y: 0.1, Z: 270",
                new float3(0.001745329f, 0.001745329f, 4.712389f),
                new Quaternion(0f, -0.001234133f, 0.7071068f, -0.7071058f),
                new float4x4(-3.16538E-06f, 0.9999986f, 0.001745326f, 0f, -0.9999986f, -1.192093E-07f, -0.001745328f, 0f, -0.001745326f, -0.001745328f, 0.999997f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0.001745329f, 4.712389f)
            };
            // 740. - X: 1, Y: 0.1, Z: 270
            yield return new object[]
            {
                "X: 1, Y: 0.1, Z: 270",
                new float3(0.01745329f, 0.001745329f, 4.712389f),
                new Quaternion(-0.005553547f, -0.006787634f, 0.707085f, -0.7070742f),
                new float4x4(-3.039354E-05f, 0.9999985f, 0.001745063f, 0f, -0.9998477f, 6.664777E-08f, -0.01745241f, 0f, -0.01745238f, -0.001745328f, 0.9998462f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0.00174533f, 4.712389f)
            };
            // 741. - X: 45, Y: 0.1, Z: 270
            yield return new object[]
            {
                "X: 45, Y: 0.1, Z: 270",
                new float3(0.7853982f, 0.001745329f, 4.712389f),
                new Quaternion(-0.2700279f, -0.2711681f, 0.6535174f, -0.6530451f),
                new float4x4(-0.001234129f, 0.9999985f, 0.001234084f, 0f, -0.7071067f, -1.490116E-08f, -0.7071068f, 0f, -0.7071058f, -0.001745313f, 0.7071056f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853984f, 0.001745302f, 4.712389f)
            };
            // 742. - X: 90, Y: 0.1, Z: 270
            yield return new object[]
            {
                "X: 90, Y: 0.1, Z: 270",
                new float3(1.570796f, 0.001745329f, 4.712389f),
                new Quaternion(-0.4995635f, -0.5004361f, 0.5004361f, -0.4995635f),
                new float4x4(-0.001745224f, 0.9999985f, 0f, 0f, 0f, 2.980232E-08f, -1f, 0f, -0.9999985f, -0.001745254f, 2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 1.572542f, 0f)
            };
            // 743. - X: 180, Y: 0.1, Z: 270
            yield return new object[]
            {
                "X: 180, Y: 0.1, Z: 270",
                new float3(3.141593f, 0.001745329f, 4.712389f),
                new Quaternion(-0.7071065f, -0.7071065f, 0.0006170361f, 0.0006170979f),
                new float4x4(-4.621131E-08f, 0.9999985f, -0.001745328f, 0f, 1f, -4.621131E-08f, 8.73697E-08f, 0f, 8.73697E-08f, -0.001745328f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.73697E-08f, 3.143338f, 1.570796f)
            };
            // 744. - X: 270, Y: 0.1, Z: 270
            yield return new object[]
            {
                "X: 270, Y: 0.1, Z: 270",
                new float3(4.712389f, 0.001745329f, 4.712389f),
                new Quaternion(-0.5004361f, -0.4995635f, -0.4995635f, 0.5004361f),
                new float4x4(0.001745284f, 0.9999985f, 0f, 0f, 0f, 2.980232E-08f, 1f, 0f, 0.9999985f, -0.001745254f, 2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.714134f, 0f)
            };
            // 745. - X: 360, Y: 0.1, Z: 270
            yield return new object[]
            {
                "X: 360, Y: 0.1, Z: 270",
                new float3(6.283185f, 0.001745329f, 4.712389f),
                new Quaternion(-0.0006170052f, 0.0006171288f, -0.7071065f, 0.7071065f),
                new float4x4(-4.644016E-08f, 0.9999985f, 0.001745328f, 0f, -1f, -4.613503E-08f, -1.747976E-07f, 0f, -1.747976E-07f, -0.001745328f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.747976E-07f, 0.001745329f, 4.712389f)
            };
            // 746. - X: 585, Y: 0.1, Z: 270
            yield return new object[]
            {
                "X: 585, Y: 0.1, Z: 270",
                new float3(10.21018f, 0.001745329f, 4.712389f),
                new Quaternion(0.6535173f, 0.653045f, 0.270028f, -0.2711681f),
                new float4x4(0.001234248f, 0.9999983f, -0.001234084f, 0f, 0.7071065f, 7.450581E-08f, 0.7071069f, 0f, 0.7071059f, -0.001745313f, -0.7071053f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.143338f, 1.570796f)
            };
            // 747. - X: -90, Y: 0.1, Z: 270
            yield return new object[]
            {
                "X: -90, Y: 0.1, Z: 270",
                new float3(-1.570796f, 0.001745329f, 4.712389f),
                new Quaternion(0.5004361f, 0.4995635f, 0.4995635f, -0.5004361f),
                new float4x4(0.001745284f, 0.9999985f, 0f, 0f, 0f, 2.980232E-08f, 1f, 0f, 0.9999985f, -0.001745254f, 2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.714134f, 0f)
            };
            // 748. - X: -540, Y: 0.1, Z: 270
            yield return new object[]
            {
                "X: -540, Y: 0.1, Z: 270",
                new float3(-9.424778f, 0.001745329f, 4.712389f),
                new Quaternion(-0.7071065f, -0.7071065f, 0.0006170754f, 0.0006170585f),
                new float4x4(-4.63084E-08f, 0.9999985f, -0.001745328f, 0f, 1f, -4.63084E-08f, -2.392335E-08f, 0f, -2.392335E-08f, -0.001745328f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.392335E-08f, 3.143338f, 1.570796f)
            };
            // 749. - X: 0, Y: 1, Z: 270
            yield return new object[]
            {
                "X: 0, Y: 1, Z: 270",
                new float3(0f, 0.01745329f, 4.712389f),
                new Quaternion(0.006170592f, -0.006170592f, 0.7070798f, -0.7070798f),
                new float4x4(8.192001E-08f, 0.9998476f, 0.01745241f, 0f, -0.9999999f, 8.192001E-08f, 0f, 0f, 0f, -0.01745241f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0.01745329f, 4.712389f)
            };
            // 750. - X: 0.1, Y: 1, Z: 270
            yield return new object[]
            {
                "X: 0.1, Y: 1, Z: 270",
                new float3(0.001745329f, 0.01745329f, 4.712389f),
                new Quaternion(0.005553547f, -0.006787634f, 0.707085f, -0.7070742f),
                new float4x4(-3.039354E-05f, 0.9998477f, 0.01745238f, 0f, -0.9999985f, 6.664777E-08f, -0.001745328f, 0f, -0.001745063f, -0.01745241f, 0.9998462f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0.01745329f, 4.712389f)
            };
            // 751. - X: 1, Y: 1, Z: 270
            yield return new object[]
            {
                "X: 1, Y: 1, Z: 270",
                new float3(0.01745329f, 0.01745329f, 4.712389f),
                new Quaternion(0f, -0.01234071f, 0.7071067f, -0.7069991f),
                new float4x4(-0.0003044076f, 0.9998476f, 0.01744975f, 0f, -0.9998476f, 1.788139E-07f, -0.0174524f, 0f, -0.01744975f, -0.0174524f, 0.9996954f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0.01745329f, 4.712389f)
            };
            // 752. - X: 45, Y: 1, Z: 270
            yield return new object[]
            {
                "X: 45, Y: 1, Z: 270",
                new float3(0.7853982f, 0.01745329f, 4.712389f),
                new Quaternion(-0.2648869f, -0.2762886f, 0.655618f, -0.6508952f),
                new float4x4(-0.01234062f, 0.9998477f, 0.01234072f, 0f, -0.7071067f, 1.043081E-07f, -0.7071067f, 0f, -0.7069991f, -0.01745239f, 0.7069991f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853981f, 0.01745331f, 4.712389f)
            };
            // 753. - X: 90, Y: 1, Z: 270
            yield return new object[]
            {
                "X: 90, Y: 1, Z: 270",
                new float3(1.570796f, 0.01745329f, 4.712389f),
                new Quaternion(-0.4956177f, -0.5043442f, 0.5043442f, -0.4956177f),
                new float4x4(-0.01745212f, 0.9998475f, 0f, 0f, 0f, 2.086163E-07f, -0.9999998f, 0f, -0.9998475f, -0.01745233f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 1.58825f, 0f)
            };
            // 754. - X: 180, Y: 1, Z: 270
            yield return new object[]
            {
                "X: 180, Y: 1, Z: 270",
                new float3(3.141593f, 0.01745329f, 4.712389f),
                new Quaternion(-0.7070798f, -0.7070798f, 0.006170562f, 0.006170623f),
                new float4x4(8.268398E-08f, 0.9998476f, -0.0174524f, 0f, 0.9999999f, 8.268398E-08f, 8.6613E-08f, 0f, 8.6613E-08f, -0.0174524f, -0.9998475f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.6613E-08f, 3.159046f, 1.570796f)
            };
            // 755. - X: 270, Y: 1, Z: 270
            yield return new object[]
            {
                "X: 270, Y: 1, Z: 270",
                new float3(4.712389f, 0.01745329f, 4.712389f),
                new Quaternion(-0.5043442f, -0.4956177f, -0.4956177f, 0.5043442f),
                new float4x4(0.01745254f, 0.9998475f, 0f, 0f, 0f, 2.086163E-07f, 0.9999998f, 0f, 0.9998475f, -0.01745233f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.729842f, 0f)
            };
            // 756. - X: 360, Y: 1, Z: 270
            yield return new object[]
            {
                "X: 360, Y: 1, Z: 270",
                new float3(6.283185f, 0.01745329f, 4.712389f),
                new Quaternion(-0.00617053f, 0.006170654f, -0.7070798f, 0.7070798f),
                new float4x4(8.039206E-08f, 0.9998476f, 0.01745241f, 0f, -0.9999999f, 8.344796E-08f, -1.750886E-07f, 0f, -1.750886E-07f, -0.01745241f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.750886E-07f, 0.01745329f, 4.712389f)
            };
            // 757. - X: 585, Y: 1, Z: 270
            yield return new object[]
            {
                "X: 585, Y: 1, Z: 270",
                new float3(10.21018f, 0.01745329f, 4.712389f),
                new Quaternion(0.6556179f, 0.6508952f, 0.264887f, -0.2762887f),
                new float4x4(0.01234069f, 0.9998477f, -0.01234072f, 0f, 0.7071065f, 1.490116E-07f, 0.707107f, 0f, 0.7069993f, -0.01745236f, -0.7069987f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.159046f, 1.570796f)
            };
            // 758. - X: -90, Y: 1, Z: 270
            yield return new object[]
            {
                "X: -90, Y: 1, Z: 270",
                new float3(-1.570796f, 0.01745329f, 4.712389f),
                new Quaternion(0.5043442f, 0.4956177f, 0.4956177f, -0.5043442f),
                new float4x4(0.01745254f, 0.9998475f, 0f, 0f, 0f, 2.086163E-07f, 0.9999998f, 0f, 0.9998475f, -0.01745233f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.729842f, 0f)
            };
            // 759. - X: -540, Y: 1, Z: 270
            yield return new object[]
            {
                "X: -540, Y: 1, Z: 270",
                new float3(-9.424778f, 0.01745329f, 4.712389f),
                new Quaternion(-0.7070798f, -0.7070798f, 0.006170601f, 0.006170584f),
                new float4x4(8.171628E-08f, 0.9998476f, -0.0174524f, 0f, 0.9999999f, 8.171628E-08f, -2.328306E-08f, 0f, -2.328306E-08f, -0.0174524f, -0.9998475f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.328306E-08f, 3.159046f, 1.570796f)
            };
            // 760. - X: 0, Y: 45, Z: 270
            yield return new object[]
            {
                "X: 0, Y: 45, Z: 270",
                new float3(0f, 0.7853982f, 4.712389f),
                new Quaternion(0.2705981f, -0.2705981f, 0.6532815f, -0.6532815f),
                new float4x4(8.940697E-08f, 0.7071067f, 0.7071068f, 0f, -0.9999999f, 8.940697E-08f, 0f, 0f, 0f, -0.7071068f, 0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0.7853982f, 4.712389f)
            };
            // 761. - X: 0.1, Y: 45, Z: 270
            yield return new object[]
            {
                "X: 0.1, Y: 45, Z: 270",
                new float3(0.001745329f, 0.7853982f, 4.712389f),
                new Quaternion(0.2700279f, -0.2711681f, 0.6535174f, -0.6530451f),
                new float4x4(-0.001234129f, 0.7071067f, 0.7071058f, 0f, -0.9999985f, -1.490116E-08f, -0.001745313f, 0f, -0.001234084f, -0.7071068f, 0.7071056f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745344f, 0.7853982f, 4.712389f)
            };
            // 762. - X: 1, Y: 45, Z: 270
            yield return new object[]
            {
                "X: 1, Y: 45, Z: 270",
                new float3(0.01745329f, 0.7853982f, 4.712389f),
                new Quaternion(0.2648869f, -0.2762886f, 0.655618f, -0.6508952f),
                new float4x4(-0.01234062f, 0.7071067f, 0.7069991f, 0f, -0.9998477f, 1.043081E-07f, -0.01745239f, 0f, -0.01234072f, -0.7071067f, 0.7069991f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745328f, 0.7853982f, 4.712389f)
            };
            // 763. - X: 45, Y: 45, Z: 270
            yield return new object[]
            {
                "X: 45, Y: 45, Z: 270",
                new float3(0.7853982f, 0.7853982f, 4.712389f),
                new Quaternion(0f, -0.5f, 0.7071068f, -0.4999999f),
                new float4x4(-0.4999999f, 0.7071067f, 0.4999999f, 0f, -0.7071067f, 5.960464E-08f, -0.7071068f, 0f, -0.4999999f, -0.7071068f, 0.5f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 0.7853982f, 4.712389f)
            };
            // 764. - X: 90, Y: 45, Z: 270
            yield return new object[]
            {
                "X: 90, Y: 45, Z: 270",
                new float3(1.570796f, 0.7853982f, 4.712389f),
                new Quaternion(-0.270598f, -0.6532815f, 0.6532815f, -0.270598f),
                new float4x4(-0.7071066f, 0.7071066f, 0f, 0f, 0f, 1.490116E-07f, -0.9999999f, 0f, -0.7071066f, -0.7071067f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 2.356194f, 0f)
            };
            // 765. - X: 180, Y: 45, Z: 270
            yield return new object[]
            {
                "X: 180, Y: 45, Z: 270",
                new float3(3.141593f, 0.7853982f, 4.712389f),
                new Quaternion(-0.6532815f, -0.6532815f, 0.270598f, 0.2705981f),
                new float4x4(1.192093E-07f, 0.7071067f, -0.7071067f, 0f, 0.9999999f, 1.192093E-07f, 8.940697E-08f, 0f, 8.940697E-08f, -0.7071067f, -0.7071066f, 0f, 0f, 0f, 0f, 1f),
                new float3(-5.960464E-08f, 3.926991f, 1.570796f)
            };
            // 766. - X: 270, Y: 45, Z: 270
            yield return new object[]
            {
                "X: 270, Y: 45, Z: 270",
                new float3(4.712389f, 0.7853982f, 4.712389f),
                new Quaternion(-0.6532815f, -0.270598f, -0.270598f, 0.6532815f),
                new float4x4(0.7071069f, 0.7071066f, 0f, 0f, 0f, 1.490116E-07f, 0.9999999f, 0f, 0.7071066f, -0.7071067f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 5.497787f, 0f)
            };
            // 767. - X: 360, Y: 45, Z: 270
            yield return new object[]
            {
                "X: 360, Y: 45, Z: 270",
                new float3(6.283185f, 0.7853982f, 4.712389f),
                new Quaternion(-0.270598f, 0.2705981f, -0.6532815f, 0.6532815f),
                new float4x4(2.980232E-08f, 0.7071067f, 0.7071067f, 0f, -0.9999999f, 1.490116E-07f, -1.490116E-07f, 0f, -1.490116E-07f, -0.7071067f, 0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.490116E-07f, 0.7853982f, 4.712389f)
            };
            // 768. - X: 585, Y: 45, Z: 270
            yield return new object[]
            {
                "X: 585, Y: 45, Z: 270",
                new float3(10.21018f, 0.7853982f, 4.712389f),
                new Quaternion(0.7071067f, 0.4999999f, 1.043081E-07f, -0.5000001f),
                new float4x4(0.5000003f, 0.7071066f, -0.4999998f, 0f, 0.7071064f, 1.788139E-07f, 0.7071069f, 0f, 0.5000001f, -0.7071066f, -0.4999995f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.926991f, 1.570796f)
            };
            // 769. - X: -90, Y: 45, Z: 270
            yield return new object[]
            {
                "X: -90, Y: 45, Z: 270",
                new float3(-1.570796f, 0.7853982f, 4.712389f),
                new Quaternion(0.6532815f, 0.270598f, 0.270598f, -0.6532815f),
                new float4x4(0.7071069f, 0.7071066f, 0f, 0f, 0f, 1.490116E-07f, 0.9999999f, 0f, 0.7071066f, -0.7071067f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 5.497787f, 0f)
            };
            // 770. - X: -540, Y: 45, Z: 270
            yield return new object[]
            {
                "X: -540, Y: 45, Z: 270",
                new float3(-9.424778f, 0.7853982f, 4.712389f),
                new Quaternion(-0.6532815f, -0.6532815f, 0.2705981f, 0.2705981f),
                new float4x4(8.940697E-08f, 0.7071067f, -0.7071068f, 0f, 0.9999999f, 8.940697E-08f, 0f, 0f, 0f, -0.7071068f, -0.7071066f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.926991f, 1.570796f)
            };
            // 771. - X: 0, Y: 90, Z: 270
            yield return new object[]
            {
                "X: 0, Y: 90, Z: 270",
                new float3(0f, 1.570796f, 4.712389f),
                new Quaternion(0.5f, -0.5f, 0.5f, -0.5f),
                new float4x4(1.192093E-07f, 0f, 0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 0f, 0f, 0f, -0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.570796f, 4.712389f)
            };
            // 772. - X: 0.1, Y: 90, Z: 270
            yield return new object[]
            {
                "X: 0.1, Y: 90, Z: 270",
                new float3(0.001745329f, 1.570796f, 4.712389f),
                new Quaternion(0.4995635f, -0.5004361f, 0.5004361f, -0.4995635f),
                new float4x4(-0.001745224f, 0f, 0.9999985f, 0f, -0.9999985f, 2.980232E-08f, -0.001745254f, 0f, 0f, -1f, 2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745255f, 1.570796f, 4.712389f)
            };
            // 773. - X: 1, Y: 90, Z: 270
            yield return new object[]
            {
                "X: 1, Y: 90, Z: 270",
                new float3(0.01745329f, 1.570796f, 4.712389f),
                new Quaternion(0.4956177f, -0.5043442f, 0.5043442f, -0.4956177f),
                new float4x4(-0.01745212f, 0f, 0.9998475f, 0f, -0.9998475f, 2.086163E-07f, -0.01745233f, 0f, 0f, -0.9999998f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745322f, 1.570796f, 4.712389f)
            };
            // 774. - X: 45, Y: 90, Z: 270
            yield return new object[]
            {
                "X: 45, Y: 90, Z: 270",
                new float3(0.7853982f, 1.570796f, 4.712389f),
                new Quaternion(0.270598f, -0.6532815f, 0.6532815f, -0.270598f),
                new float4x4(-0.7071066f, 0f, 0.7071066f, 0f, -0.7071066f, 1.490116E-07f, -0.7071067f, 0f, 0f, -0.9999999f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 1.570796f, 4.712389f)
            };
            // 775. - X: 90, Y: 90, Z: 270
            yield return new object[]
            {
                "X: 90, Y: 90, Z: 270",
                new float3(1.570796f, 1.570796f, 4.712389f),
                new Quaternion(0f, -0.7071067f, 0.7071067f, 0f),
                new float4x4(-0.9999996f, 0f, 0f, 0f, 0f, 1.788139E-07f, -0.9999998f, 0f, 0f, -0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570451f, 3.141593f, 0f)
            };
            // 776. - X: 180, Y: 90, Z: 270
            yield return new object[]
            {
                "X: 180, Y: 90, Z: 270",
                new float3(3.141593f, 1.570796f, 4.712389f),
                new Quaternion(-0.5f, -0.4999999f, 0.4999999f, 0.5f),
                new float4x4(2.384186E-07f, 0f, -0.9999999f, 0f, 0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, -0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(-1.788139E-07f, 4.712389f, 1.570796f)
            };
            // 777. - X: 270, Y: 90, Z: 270
            yield return new object[]
            {
                "X: 270, Y: 90, Z: 270",
                new float3(4.712389f, 1.570796f, 4.712389f),
                new Quaternion(-0.7071067f, 0f, 0f, 0.7071067f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 1.788139E-07f, 0.9999998f, 0f, 0f, -0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 0f, 0f)
            };
            // 778. - X: 360, Y: 90, Z: 270
            yield return new object[]
            {
                "X: 360, Y: 90, Z: 270",
                new float3(6.283185f, 1.570796f, 4.712389f),
                new Quaternion(-0.4999999f, 0.5f, -0.5f, 0.4999999f),
                new float4x4(0f, 0f, 0.9999999f, 0f, -0.9999999f, 1.192093E-07f, -1.192093E-07f, 0f, 0f, -0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.788139E-07f, 1.570796f, 4.712389f)
            };
            // 779. - X: 585, Y: 90, Z: 270
            yield return new object[]
            {
                "X: 585, Y: 90, Z: 270",
                new float3(10.21018f, 1.570796f, 4.712389f),
                new Quaternion(0.6532815f, 0.2705979f, -0.2705979f, -0.6532815f),
                new float4x4(0.7071072f, 0f, -0.7071064f, 0f, 0.7071064f, 1.043081E-07f, 0.7071071f, 0f, 0f, -0.9999999f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 4.712389f, 1.570796f)
            };
            // 780. - X: -90, Y: 90, Z: 270
            yield return new object[]
            {
                "X: -90, Y: 90, Z: 270",
                new float3(-1.570796f, 1.570796f, 4.712389f),
                new Quaternion(0.7071067f, 0f, 0f, -0.7071067f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 1.788139E-07f, 0.9999998f, 0f, 0f, -0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 0f, 0f)
            };
            // 781. - X: -540, Y: 90, Z: 270
            yield return new object[]
            {
                "X: -540, Y: 90, Z: 270",
                new float3(-9.424778f, 1.570796f, 4.712389f),
                new Quaternion(-0.5f, -0.5f, 0.5f, 0.5f),
                new float4x4(1.192093E-07f, 0f, -0.9999999f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, -0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 1.570796f)
            };
            // 782. - X: 0, Y: 180, Z: 270
            yield return new object[]
            {
                "X: 0, Y: 180, Z: 270",
                new float3(0f, 3.141593f, 4.712389f),
                new Quaternion(0.7071068f, -0.7071068f, -3.090862E-08f, 3.090862E-08f),
                new float4x4(5.960464E-08f, -0.9999999f, -8.742278E-08f, 0f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 8.742278E-08f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.141593f, 4.712389f)
            };
            // 783. - X: 0.1, Y: 180, Z: 270
            yield return new object[]
            {
                "X: 0.1, Y: 180, Z: 270",
                new float3(0.001745329f, 3.141593f, 4.712389f),
                new Quaternion(0.7071065f, -0.7071065f, 0.0006170361f, 0.0006170979f),
                new float4x4(-4.621131E-08f, -1f, -8.73697E-08f, 0f, -0.9999985f, -4.621131E-08f, -0.001745328f, 0f, 0.001745328f, 8.73697E-08f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.141593f, 4.712389f)
            };
            // 784. - X: 1, Y: 180, Z: 270
            yield return new object[]
            {
                "X: 1, Y: 180, Z: 270",
                new float3(0.01745329f, 3.141593f, 4.712389f),
                new Quaternion(0.7070798f, -0.7070798f, 0.006170562f, 0.006170623f),
                new float4x4(8.268398E-08f, -0.9999999f, -8.6613E-08f, 0f, -0.9998476f, 8.268398E-08f, -0.0174524f, 0f, 0.0174524f, 8.6613E-08f, -0.9998475f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.141593f, 4.712389f)
            };
            // 785. - X: 45, Y: 180, Z: 270
            yield return new object[]
            {
                "X: 45, Y: 180, Z: 270",
                new float3(0.7853982f, 3.141593f, 4.712389f),
                new Quaternion(0.6532815f, -0.6532815f, 0.270598f, 0.2705981f),
                new float4x4(1.192093E-07f, -0.9999999f, -8.940697E-08f, 0f, -0.7071067f, 1.192093E-07f, -0.7071067f, 0f, 0.7071067f, 8.940697E-08f, -0.7071066f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 3.141593f, 4.712389f)
            };
            // 786. - X: 90, Y: 180, Z: 270
            yield return new object[]
            {
                "X: 90, Y: 180, Z: 270",
                new float3(1.570796f, 3.141593f, 4.712389f),
                new Quaternion(0.5f, -0.4999999f, 0.4999999f, 0.5f),
                new float4x4(2.384186E-07f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, 0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 4.712389f, 0f)
            };
            // 787. - X: 180, Y: 180, Z: 270
            yield return new object[]
            {
                "X: 180, Y: 180, Z: 270",
                new float3(3.141593f, 3.141593f, 4.712389f),
                new Quaternion(0f, 6.181724E-08f, 0.7071068f, 0.7071068f),
                new float4x4(5.960464E-08f, -0.9999999f, 8.742278E-08f, 0f, 0.9999999f, 5.960464E-08f, 8.742278E-08f, 0f, -8.742278E-08f, 8.742278E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742279E-08f, 8.742278E-08f, 1.570796f)
            };
            // 788. - X: 270, Y: 180, Z: 270
            yield return new object[]
            {
                "X: 270, Y: 180, Z: 270",
                new float3(4.712389f, 3.141593f, 4.712389f),
                new Quaternion(-0.4999999f, 0.5f, 0.5f, 0.4999999f),
                new float4x4(0f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 789. - X: 360, Y: 180, Z: 270
            yield return new object[]
            {
                "X: 360, Y: 180, Z: 270",
                new float3(6.283185f, 3.141593f, 4.712389f),
                new Quaternion(-0.7071068f, 0.7071068f, -3.090862E-08f, -9.272586E-08f),
                new float4x4(5.960464E-08f, -0.9999999f, -8.742277E-08f, 0f, -0.9999999f, 5.960464E-08f, -1.748456E-07f, 0f, 1.748456E-07f, 8.742277E-08f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 3.141593f, 4.712389f)
            };
            // 790. - X: 585, Y: 180, Z: 270
            yield return new object[]
            {
                "X: 585, Y: 180, Z: 270",
                new float3(10.21018f, 3.141593f, 4.712389f),
                new Quaternion(0.2705981f, -0.2705982f, -0.6532814f, -0.6532814f),
                new float4x4(4.470348E-08f, -0.9999999f, 5.960464E-08f, 0f, 0.7071064f, 1.192093E-07f, 0.707107f, 0f, -0.707107f, 5.960464E-08f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.264406E-07f, 1.570796f)
            };
            // 791. - X: -90, Y: 180, Z: 270
            yield return new object[]
            {
                "X: -90, Y: 180, Z: 270",
                new float3(-1.570796f, 3.141593f, 4.712389f),
                new Quaternion(0.4999999f, -0.5f, -0.5f, -0.4999999f),
                new float4x4(0f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 792. - X: -540, Y: 180, Z: 270
            yield return new object[]
            {
                "X: -540, Y: 180, Z: 270",
                new float3(-9.424778f, 3.141593f, 4.712389f),
                new Quaternion(3.934078E-08f, 2.247646E-08f, 0.7071068f, 0.7071068f),
                new float4x4(5.960464E-08f, -0.9999999f, 8.742278E-08f, 0f, 0.9999999f, 5.960464E-08f, -2.384976E-08f, 0f, 2.384976E-08f, 8.742278E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 8.742278E-08f, 1.570796f)
            };
            // 793. - X: 0, Y: 270, Z: 270
            yield return new object[]
            {
                "X: 0, Y: 270, Z: 270",
                new float3(0f, 4.712389f, 4.712389f),
                new Quaternion(0.5f, -0.5f, -0.5f, 0.5f),
                new float4x4(1.192093E-07f, 0f, -0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 4.712389f)
            };
            // 794. - X: 0.1, Y: 270, Z: 270
            yield return new object[]
            {
                "X: 0.1, Y: 270, Z: 270",
                new float3(0.001745329f, 4.712389f, 4.712389f),
                new Quaternion(0.5004361f, -0.4995635f, -0.4995635f, 0.5004361f),
                new float4x4(0.001745284f, 0f, -0.9999985f, 0f, -0.9999985f, 2.980232E-08f, -0.001745254f, 0f, 0f, 1f, 2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745255f, 4.712389f, 4.712389f)
            };
            // 795. - X: 1, Y: 270, Z: 270
            yield return new object[]
            {
                "X: 1, Y: 270, Z: 270",
                new float3(0.01745329f, 4.712389f, 4.712389f),
                new Quaternion(0.5043442f, -0.4956177f, -0.4956177f, 0.5043442f),
                new float4x4(0.01745254f, 0f, -0.9998475f, 0f, -0.9998475f, 2.086163E-07f, -0.01745233f, 0f, 0f, 0.9999998f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745322f, 4.712389f, 4.712389f)
            };
            // 796. - X: 45, Y: 270, Z: 270
            yield return new object[]
            {
                "X: 45, Y: 270, Z: 270",
                new float3(0.7853982f, 4.712389f, 4.712389f),
                new Quaternion(0.6532815f, -0.270598f, -0.270598f, 0.6532815f),
                new float4x4(0.7071069f, 0f, -0.7071066f, 0f, -0.7071066f, 1.490116E-07f, -0.7071067f, 0f, 0f, 0.9999999f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 4.712389f, 4.712389f)
            };
            // 797. - X: 90, Y: 270, Z: 270
            yield return new object[]
            {
                "X: 90, Y: 270, Z: 270",
                new float3(1.570796f, 4.712389f, 4.712389f),
                new Quaternion(0.7071067f, 0f, 0f, 0.7071067f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 1.788139E-07f, -0.9999998f, 0f, 0f, 0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570451f, 0f, 0f)
            };
            // 798. - X: 180, Y: 270, Z: 270
            yield return new object[]
            {
                "X: 180, Y: 270, Z: 270",
                new float3(3.141593f, 4.712389f, 4.712389f),
                new Quaternion(0.4999999f, 0.5f, 0.5f, 0.4999999f),
                new float4x4(0f, 0f, 0.9999999f, 0f, 0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(-1.788139E-07f, 1.570796f, 1.570796f)
            };
            // 799. - X: 270, Y: 270, Z: 270
            yield return new object[]
            {
                "X: 270, Y: 270, Z: 270",
                new float3(4.712389f, 4.712389f, 4.712389f),
                new Quaternion(0f, 0.7071067f, 0.7071067f, 0f),
                new float4x4(-0.9999996f, 0f, 0f, 0f, 0f, 1.788139E-07f, 0.9999998f, 0f, 0f, 0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 3.141593f, 0f)
            };
            // 800. - X: 360, Y: 270, Z: 270
            yield return new object[]
            {
                "X: 360, Y: 270, Z: 270",
                new float3(6.283185f, 4.712389f, 4.712389f),
                new Quaternion(-0.5f, 0.4999999f, 0.4999999f, -0.5f),
                new float4x4(2.384186E-07f, 0f, -0.9999999f, 0f, -0.9999999f, 1.192093E-07f, -1.192093E-07f, 0f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.788139E-07f, 4.712389f, 4.712389f)
            };
            // 801. - X: 585, Y: 270, Z: 270
            yield return new object[]
            {
                "X: 585, Y: 270, Z: 270",
                new float3(10.21018f, 4.712389f, 4.712389f),
                new Quaternion(-0.2705979f, -0.6532815f, -0.6532815f, -0.2705979f),
                new float4x4(-0.7071069f, 0f, 0.7071064f, 0f, 0.7071064f, 1.043081E-07f, 0.7071071f, 0f, 0f, 0.9999999f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.570796f, 1.570796f)
            };
            // 802. - X: -90, Y: 270, Z: 270
            yield return new object[]
            {
                "X: -90, Y: 270, Z: 270",
                new float3(-1.570796f, 4.712389f, 4.712389f),
                new Quaternion(0f, -0.7071067f, -0.7071067f, 0f),
                new float4x4(-0.9999996f, 0f, 0f, 0f, 0f, 1.788139E-07f, 0.9999998f, 0f, 0f, 0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 3.141593f, 0f)
            };
            // 803. - X: -540, Y: 270, Z: 270
            yield return new object[]
            {
                "X: -540, Y: 270, Z: 270",
                new float3(-9.424778f, 4.712389f, 4.712389f),
                new Quaternion(0.5f, 0.5f, 0.5f, 0.5f),
                new float4x4(1.192093E-07f, 0f, 0.9999999f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.570796f, 1.570796f)
            };
            // 804. - X: 0, Y: 360, Z: 270
            yield return new object[]
            {
                "X: 0, Y: 360, Z: 270",
                new float3(0f, 6.283185f, 4.712389f),
                new Quaternion(-6.181724E-08f, 6.181724E-08f, -0.7071068f, 0.7071068f),
                new float4x4(5.960464E-08f, 0.9999999f, 1.748456E-07f, 0f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, -1.748456E-07f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.748456E-07f, 4.712389f)
            };
            // 805. - X: 0.1, Y: 360, Z: 270
            yield return new object[]
            {
                "X: 0.1, Y: 360, Z: 270",
                new float3(0.001745329f, 6.283185f, 4.712389f),
                new Quaternion(0.0006170052f, 0.0006171288f, -0.7071065f, 0.7071065f),
                new float4x4(-4.644016E-08f, 1f, 1.747976E-07f, 0f, -0.9999985f, -4.613503E-08f, -0.001745328f, 0f, -0.001745328f, -1.747976E-07f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 1.747979E-07f, 4.712389f)
            };
            // 806. - X: 1, Y: 360, Z: 270
            yield return new object[]
            {
                "X: 1, Y: 360, Z: 270",
                new float3(0.01745329f, 6.283185f, 4.712389f),
                new Quaternion(0.00617053f, 0.006170654f, -0.7070798f, 0.7070798f),
                new float4x4(8.039206E-08f, 0.9999999f, 1.750886E-07f, 0f, -0.9998476f, 8.344796E-08f, -0.01745241f, 0f, -0.01745241f, -1.750886E-07f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 1.751153E-07f, 4.712389f)
            };
            // 807. - X: 45, Y: 360, Z: 270
            yield return new object[]
            {
                "X: 45, Y: 360, Z: 270",
                new float3(0.7853982f, 6.283185f, 4.712389f),
                new Quaternion(0.270598f, 0.2705981f, -0.6532815f, 0.6532815f),
                new float4x4(2.980232E-08f, 0.9999999f, 1.490116E-07f, 0f, -0.7071067f, 1.490116E-07f, -0.7071067f, 0f, -0.7071067f, -1.490116E-07f, 0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853984f, 2.107342E-07f, 4.712389f)
            };
            // 808. - X: 90, Y: 360, Z: 270
            yield return new object[]
            {
                "X: 90, Y: 360, Z: 270",
                new float3(1.570796f, 6.283185f, 4.712389f),
                new Quaternion(0.4999999f, 0.5f, -0.5f, 0.4999999f),
                new float4x4(0f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, -0.9999999f, -1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 1.570796f, 0f)
            };
            // 809. - X: 180, Y: 360, Z: 270
            yield return new object[]
            {
                "X: 180, Y: 360, Z: 270",
                new float3(3.141593f, 6.283185f, 4.712389f),
                new Quaternion(0.7071068f, 0.7071068f, -3.090862E-08f, -9.272586E-08f),
                new float4x4(5.960464E-08f, 0.9999999f, -1.748456E-07f, 0f, 0.9999999f, 5.960464E-08f, 8.742277E-08f, 0f, 8.742277E-08f, -1.748456E-07f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742279E-08f, 3.141593f, 1.570796f)
            };
            // 810. - X: 270, Y: 360, Z: 270
            yield return new object[]
            {
                "X: 270, Y: 360, Z: 270",
                new float3(4.712389f, 6.283185f, 4.712389f),
                new Quaternion(0.5f, 0.4999999f, 0.4999999f, -0.5f),
                new float4x4(2.384186E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, 0.9999999f, -1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 811. - X: 360, Y: 360, Z: 270
            yield return new object[]
            {
                "X: 360, Y: 360, Z: 270",
                new float3(6.283185f, 6.283185f, 4.712389f),
                new Quaternion(0f, -1.236345E-07f, 0.7071068f, -0.7071068f),
                new float4x4(5.960461E-08f, 0.9999999f, 1.748456E-07f, 0f, -0.9999999f, 5.960464E-08f, -1.748456E-07f, 0f, -1.748456E-07f, -1.748456E-07f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 1.748456E-07f, 4.712389f)
            };
            // 812. - X: 585, Y: 360, Z: 270
            yield return new object[]
            {
                "X: 585, Y: 360, Z: 270",
                new float3(10.21018f, 6.283185f, 4.712389f),
                new Quaternion(-0.6532814f, -0.6532814f, -0.2705981f, 0.2705982f),
                new float4x4(1.490116E-07f, 0.9999999f, -1.788139E-07f, 0f, 0.7071064f, 1.490116E-07f, 0.707107f, 0f, 0.707107f, -1.788139E-07f, -0.7071064f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.141593f, 1.570796f)
            };
            // 813. - X: -90, Y: 360, Z: 270
            yield return new object[]
            {
                "X: -90, Y: 360, Z: 270",
                new float3(-1.570796f, 6.283185f, 4.712389f),
                new Quaternion(-0.5f, -0.4999999f, -0.4999999f, 0.5f),
                new float4x4(2.384186E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, 0.9999999f, -1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 814. - X: -540, Y: 360, Z: 270
            yield return new object[]
            {
                "X: -540, Y: 360, Z: 270",
                new float3(-9.424778f, 6.283185f, 4.712389f),
                new Quaternion(0.7071068f, 0.7071068f, -7.02494E-08f, -5.338508E-08f),
                new float4x4(5.960463E-08f, 0.9999999f, -1.748456E-07f, 0f, 0.9999999f, 5.960463E-08f, -2.384976E-08f, 0f, -2.384976E-08f, -1.748456E-07f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.141593f, 1.570796f)
            };
            // 815. - X: 0, Y: 585, Z: 270
            yield return new object[]
            {
                "X: 0, Y: 585, Z: 270",
                new float3(0f, 10.21018f, 4.712389f),
                new Quaternion(-0.6532814f, 0.6532814f, 0.2705982f, -0.2705982f),
                new float4x4(7.450581E-08f, -0.7071064f, -0.707107f, 0f, -0.9999999f, 7.450581E-08f, 0f, 0f, 0f, 0.707107f, -0.7071064f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.926991f, 4.712389f)
            };
            // 816. - X: 0.1, Y: 585, Z: 270
            yield return new object[]
            {
                "X: 0.1, Y: 585, Z: 270",
                new float3(0.001745329f, 10.21018f, 4.712389f),
                new Quaternion(-0.6535173f, 0.653045f, 0.270028f, -0.2711681f),
                new float4x4(0.001234248f, -0.7071065f, -0.7071059f, 0f, -0.9999983f, 7.450581E-08f, -0.001745313f, 0f, 0.001234084f, 0.7071069f, -0.7071053f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745314f, 3.926991f, 4.712389f)
            };
            // 817. - X: 1, Y: 585, Z: 270
            yield return new object[]
            {
                "X: 1, Y: 585, Z: 270",
                new float3(0.01745329f, 10.21018f, 4.712389f),
                new Quaternion(-0.6556179f, 0.6508952f, 0.264887f, -0.2762887f),
                new float4x4(0.01234069f, -0.7071065f, -0.7069993f, 0f, -0.9998477f, 1.490116E-07f, -0.01745236f, 0f, 0.01234072f, 0.707107f, -0.7069987f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745328f, 3.926991f, 4.712389f)
            };
            // 818. - X: 45, Y: 585, Z: 270
            yield return new object[]
            {
                "X: 45, Y: 585, Z: 270",
                new float3(0.7853982f, 10.21018f, 4.712389f),
                new Quaternion(-0.7071067f, 0.4999999f, 1.043081E-07f, -0.5000001f),
                new float4x4(0.5000003f, -0.7071064f, -0.5000001f, 0f, -0.7071066f, 1.788139E-07f, -0.7071066f, 0f, 0.4999998f, 0.7071069f, -0.4999995f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 3.926991f, 4.712389f)
            };
            // 819. - X: 90, Y: 585, Z: 270
            yield return new object[]
            {
                "X: 90, Y: 585, Z: 270",
                new float3(1.570796f, 10.21018f, 4.712389f),
                new Quaternion(-0.6532815f, 0.2705979f, -0.2705979f, -0.6532815f),
                new float4x4(0.7071072f, -0.7071064f, 0f, 0f, 0f, 1.043081E-07f, -0.9999999f, 0f, 0.7071064f, 0.7071071f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 5.497787f, 0f)
            };
            // 820. - X: 180, Y: 585, Z: 270
            yield return new object[]
            {
                "X: 180, Y: 585, Z: 270",
                new float3(3.141593f, 10.21018f, 4.712389f),
                new Quaternion(-0.2705981f, -0.2705982f, -0.6532814f, -0.6532814f),
                new float4x4(4.470348E-08f, -0.7071064f, 0.707107f, 0f, 0.9999999f, 1.192093E-07f, 5.960464E-08f, 0f, -5.960464E-08f, 0.707107f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.940697E-08f, 0.7853987f, 1.570796f)
            };
            // 821. - X: 270, Y: 585, Z: 270
            yield return new object[]
            {
                "X: 270, Y: 585, Z: 270",
                new float3(4.712389f, 10.21018f, 4.712389f),
                new Quaternion(0.2705979f, -0.6532815f, -0.6532815f, -0.2705979f),
                new float4x4(-0.7071069f, -0.7071064f, 0f, 0f, 0f, 1.043081E-07f, 0.9999999f, 0f, -0.7071064f, 0.7071071f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 2.356195f, 0f)
            };
            // 822. - X: 360, Y: 585, Z: 270
            yield return new object[]
            {
                "X: 360, Y: 585, Z: 270",
                new float3(6.283185f, 10.21018f, 4.712389f),
                new Quaternion(0.6532814f, -0.6532814f, -0.2705981f, 0.2705982f),
                new float4x4(1.490116E-07f, -0.7071064f, -0.707107f, 0f, -0.9999999f, 1.490116E-07f, -1.788139E-07f, 0f, 1.788139E-07f, 0.707107f, -0.7071064f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.490116E-07f, 3.926991f, 4.712389f)
            };
            // 823. - X: 585, Y: 585, Z: 270
            yield return new object[]
            {
                "X: 585, Y: 585, Z: 270",
                new float3(10.21018f, 10.21018f, 4.712389f),
                new Quaternion(0f, 0.5000002f, 0.7071068f, 0.4999998f),
                new float4x4(-0.5000003f, -0.7071065f, 0.5f, 0f, 0.7071065f, 5.960464E-08f, 0.707107f, 0f, -0.5f, 0.707107f, 0.4999996f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 0.7853985f, 1.570796f)
            };
            // 824. - X: -90, Y: 585, Z: 270
            yield return new object[]
            {
                "X: -90, Y: 585, Z: 270",
                new float3(-1.570796f, 10.21018f, 4.712389f),
                new Quaternion(-0.2705979f, 0.6532815f, 0.6532815f, 0.2705979f),
                new float4x4(-0.7071069f, -0.7071064f, 0f, 0f, 0f, 1.043081E-07f, 0.9999999f, 0f, -0.7071064f, 0.7071071f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 2.356195f, 0f)
            };
            // 825. - X: -540, Y: 585, Z: 270
            yield return new object[]
            {
                "X: -540, Y: 585, Z: 270",
                new float3(-9.424778f, 10.21018f, 4.712389f),
                new Quaternion(-0.2705982f, -0.2705982f, -0.6532814f, -0.6532814f),
                new float4x4(7.450581E-08f, -0.7071064f, 0.707107f, 0f, 0.9999999f, 7.450581E-08f, 0f, 0f, 0f, 0.707107f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0.7853986f, 1.570796f)
            };
            // 826. - X: 0, Y: -90, Z: 270
            yield return new object[]
            {
                "X: 0, Y: -90, Z: 270",
                new float3(0f, -1.570796f, 4.712389f),
                new Quaternion(-0.5f, 0.5f, 0.5f, -0.5f),
                new float4x4(1.192093E-07f, 0f, -0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 4.712389f)
            };
            // 827. - X: 0.1, Y: -90, Z: 270
            yield return new object[]
            {
                "X: 0.1, Y: -90, Z: 270",
                new float3(0.001745329f, -1.570796f, 4.712389f),
                new Quaternion(-0.5004361f, 0.4995635f, 0.4995635f, -0.5004361f),
                new float4x4(0.001745284f, 0f, -0.9999985f, 0f, -0.9999985f, 2.980232E-08f, -0.001745254f, 0f, 0f, 1f, 2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745255f, 4.712389f, 4.712389f)
            };
            // 828. - X: 1, Y: -90, Z: 270
            yield return new object[]
            {
                "X: 1, Y: -90, Z: 270",
                new float3(0.01745329f, -1.570796f, 4.712389f),
                new Quaternion(-0.5043442f, 0.4956177f, 0.4956177f, -0.5043442f),
                new float4x4(0.01745254f, 0f, -0.9998475f, 0f, -0.9998475f, 2.086163E-07f, -0.01745233f, 0f, 0f, 0.9999998f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745322f, 4.712389f, 4.712389f)
            };
            // 829. - X: 45, Y: -90, Z: 270
            yield return new object[]
            {
                "X: 45, Y: -90, Z: 270",
                new float3(0.7853982f, -1.570796f, 4.712389f),
                new Quaternion(-0.6532815f, 0.270598f, 0.270598f, -0.6532815f),
                new float4x4(0.7071069f, 0f, -0.7071066f, 0f, -0.7071066f, 1.490116E-07f, -0.7071067f, 0f, 0f, 0.9999999f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 4.712389f, 4.712389f)
            };
            // 830. - X: 90, Y: -90, Z: 270
            yield return new object[]
            {
                "X: 90, Y: -90, Z: 270",
                new float3(1.570796f, -1.570796f, 4.712389f),
                new Quaternion(-0.7071067f, 0f, 0f, -0.7071067f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 1.788139E-07f, -0.9999998f, 0f, 0f, 0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570451f, 0f, 0f)
            };
            // 831. - X: 180, Y: -90, Z: 270
            yield return new object[]
            {
                "X: 180, Y: -90, Z: 270",
                new float3(3.141593f, -1.570796f, 4.712389f),
                new Quaternion(-0.4999999f, -0.5f, -0.5f, -0.4999999f),
                new float4x4(0f, 0f, 0.9999999f, 0f, 0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(-1.788139E-07f, 1.570796f, 1.570796f)
            };
            // 832. - X: 270, Y: -90, Z: 270
            yield return new object[]
            {
                "X: 270, Y: -90, Z: 270",
                new float3(4.712389f, -1.570796f, 4.712389f),
                new Quaternion(0f, -0.7071067f, -0.7071067f, 0f),
                new float4x4(-0.9999996f, 0f, 0f, 0f, 0f, 1.788139E-07f, 0.9999998f, 0f, 0f, 0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 3.141593f, 0f)
            };
            // 833. - X: 360, Y: -90, Z: 270
            yield return new object[]
            {
                "X: 360, Y: -90, Z: 270",
                new float3(6.283185f, -1.570796f, 4.712389f),
                new Quaternion(0.5f, -0.4999999f, -0.4999999f, 0.5f),
                new float4x4(2.384186E-07f, 0f, -0.9999999f, 0f, -0.9999999f, 1.192093E-07f, -1.192093E-07f, 0f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.788139E-07f, 4.712389f, 4.712389f)
            };
            // 834. - X: 585, Y: -90, Z: 270
            yield return new object[]
            {
                "X: 585, Y: -90, Z: 270",
                new float3(10.21018f, -1.570796f, 4.712389f),
                new Quaternion(0.2705979f, 0.6532815f, 0.6532815f, 0.2705979f),
                new float4x4(-0.7071069f, 0f, 0.7071064f, 0f, 0.7071064f, 1.043081E-07f, 0.7071071f, 0f, 0f, 0.9999999f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.570796f, 1.570796f)
            };
            // 835. - X: -90, Y: -90, Z: 270
            yield return new object[]
            {
                "X: -90, Y: -90, Z: 270",
                new float3(-1.570796f, -1.570796f, 4.712389f),
                new Quaternion(0f, 0.7071067f, 0.7071067f, 0f),
                new float4x4(-0.9999996f, 0f, 0f, 0f, 0f, 1.788139E-07f, 0.9999998f, 0f, 0f, 0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 3.141593f, 0f)
            };
            // 836. - X: -540, Y: -90, Z: 270
            yield return new object[]
            {
                "X: -540, Y: -90, Z: 270",
                new float3(-9.424778f, -1.570796f, 4.712389f),
                new Quaternion(-0.5f, -0.5f, -0.5f, -0.5f),
                new float4x4(1.192093E-07f, 0f, 0.9999999f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.570796f, 1.570796f)
            };
            // 837. - X: 0, Y: -540, Z: 270
            yield return new object[]
            {
                "X: 0, Y: -540, Z: 270",
                new float3(0f, -9.424778f, 4.712389f),
                new Quaternion(0.7071068f, -0.7071068f, 8.432163E-09f, -8.432163E-09f),
                new float4x4(5.960464E-08f, -0.9999999f, 2.384976E-08f, 0f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, -2.384976E-08f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.141593f, 4.712389f)
            };
            // 838. - X: 0.1, Y: -540, Z: 270
            yield return new object[]
            {
                "X: 0.1, Y: -540, Z: 270",
                new float3(0.001745329f, -9.424778f, 4.712389f),
                new Quaternion(0.7071065f, -0.7071065f, 0.0006170754f, 0.0006170585f),
                new float4x4(-4.63084E-08f, -1f, 2.392335E-08f, 0f, -0.9999985f, -4.63084E-08f, -0.001745328f, 0f, 0.001745328f, -2.392335E-08f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.141593f, 4.712389f)
            };
            // 839. - X: 1, Y: -540, Z: 270
            yield return new object[]
            {
                "X: 1, Y: -540, Z: 270",
                new float3(0.01745329f, -9.424778f, 4.712389f),
                new Quaternion(0.7070798f, -0.7070798f, 0.006170601f, 0.006170584f),
                new float4x4(8.171628E-08f, -0.9999999f, 2.328306E-08f, 0f, -0.9998476f, 8.171628E-08f, -0.0174524f, 0f, 0.0174524f, -2.328306E-08f, -0.9998475f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.141593f, 4.712389f)
            };
            // 840. - X: 45, Y: -540, Z: 270
            yield return new object[]
            {
                "X: 45, Y: -540, Z: 270",
                new float3(0.7853982f, -9.424778f, 4.712389f),
                new Quaternion(0.6532815f, -0.6532815f, 0.2705981f, 0.2705981f),
                new float4x4(8.940697E-08f, -0.9999999f, 0f, 0f, -0.7071067f, 8.940697E-08f, -0.7071068f, 0f, 0.7071068f, 0f, -0.7071066f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 3.141593f, 4.712389f)
            };
            // 841. - X: 90, Y: -540, Z: 270
            yield return new object[]
            {
                "X: 90, Y: -540, Z: 270",
                new float3(1.570796f, -9.424778f, 4.712389f),
                new Quaternion(0.5f, -0.5f, 0.5f, 0.5f),
                new float4x4(1.192093E-07f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, 0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 4.712389f, 0f)
            };
            // 842. - X: 180, Y: -540, Z: 270
            yield return new object[]
            {
                "X: 180, Y: -540, Z: 270",
                new float3(3.141593f, -9.424778f, 4.712389f),
                new Quaternion(-3.934078E-08f, 2.247646E-08f, 0.7071068f, 0.7071068f),
                new float4x4(5.960464E-08f, -0.9999999f, -2.384976E-08f, 0f, 0.9999999f, 5.960464E-08f, 8.742278E-08f, 0f, -8.742278E-08f, -2.384976E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742279E-08f, -2.384976E-08f, 1.570796f)
            };
            // 843. - X: 270, Y: -540, Z: 270
            yield return new object[]
            {
                "X: 270, Y: -540, Z: 270",
                new float3(4.712389f, -9.424778f, 4.712389f),
                new Quaternion(-0.5f, 0.5f, 0.5f, 0.5f),
                new float4x4(1.192093E-07f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 844. - X: 360, Y: -540, Z: 270
            yield return new object[]
            {
                "X: 360, Y: -540, Z: 270",
                new float3(6.283185f, -9.424778f, 4.712389f),
                new Quaternion(-0.7071068f, 0.7071068f, -7.02494E-08f, -5.338508E-08f),
                new float4x4(5.960463E-08f, -0.9999999f, 2.384976E-08f, 0f, -0.9999999f, 5.960463E-08f, -1.748456E-07f, 0f, 1.748456E-07f, -2.384976E-08f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 3.141593f, 4.712389f)
            };
            // 845. - X: 585, Y: -540, Z: 270
            yield return new object[]
            {
                "X: 585, Y: -540, Z: 270",
                new float3(10.21018f, -9.424778f, 4.712389f),
                new Quaternion(0.2705982f, -0.2705982f, -0.6532814f, -0.6532814f),
                new float4x4(7.450581E-08f, -0.9999999f, 0f, 0f, 0.7071064f, 7.450581E-08f, 0.707107f, 0f, -0.707107f, 0f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 0f, 1.570796f)
            };
            // 846. - X: -90, Y: -540, Z: 270
            yield return new object[]
            {
                "X: -90, Y: -540, Z: 270",
                new float3(-1.570796f, -9.424778f, 4.712389f),
                new Quaternion(0.5f, -0.5f, -0.5f, -0.5f),
                new float4x4(1.192093E-07f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 847. - X: -540, Y: -540, Z: 270
            yield return new object[]
            {
                "X: -540, Y: -540, Z: 270",
                new float3(-9.424778f, -9.424778f, 4.712389f),
                new Quaternion(0f, -1.686433E-08f, 0.7071068f, 0.7071068f),
                new float4x4(5.960464E-08f, -0.9999999f, -2.384976E-08f, 0f, 0.9999999f, 5.960464E-08f, -2.384976E-08f, 0f, 2.384976E-08f, -2.384976E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, -2.384976E-08f, 1.570796f)
            };
            // 848. - X: 0, Y: 0, Z: 360
            yield return new object[]
            {
                "X: 0, Y: 0, Z: 360",
                new float3(0f, 0f, 6.283185f),
                new Quaternion(0f, 0f, -8.742278E-08f, -1f),
                new float4x4(1f, -1.748456E-07f, 0f, 0f, 1.748456E-07f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0f, 1.748456E-07f)
            };
            // 849. - X: 0.1, Y: 0, Z: 360
            yield return new object[]
            {
                "X: 0.1, Y: 0, Z: 360",
                new float3(0.001745329f, 0f, 6.283185f),
                new Quaternion(-0.0008726645f, 7.629075E-11f, -8.742275E-08f, -0.9999996f),
                new float4x4(1f, -1.748456E-07f, 1.387779E-17f, 0f, 1.748453E-07f, 0.9999985f, -0.001745328f, 0f, 3.051629E-10f, 0.001745328f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 1.387781E-17f, 1.748456E-07f)
            };
            // 850. - X: 1, Y: 0, Z: 360
            yield return new object[]
            {
                "X: 1, Y: 0, Z: 360",
                new float3(0.01745329f, 0f, 6.283185f),
                new Quaternion(-0.008726535f, 7.62898E-10f, -8.741944E-08f, -0.9999619f),
                new float4x4(1f, -1.748455E-07f, -1.110223E-16f, 0f, 1.748189E-07f, 0.9998477f, -0.01745241f, 0f, 3.051476E-09f, 0.01745241f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, -1.110392E-16f, 1.748455E-07f)
            };
            // 851. - X: 45, Y: 0, Z: 360
            yield return new object[]
            {
                "X: 45, Y: 0, Z: 360",
                new float3(0.7853982f, 0f, 6.283185f),
                new Quaternion(-0.3826835f, 3.345525E-08f, -8.076811E-08f, -0.9238795f),
                new float4x4(1f, -1.748456E-07f, 0f, 0f, 1.236345E-07f, 0.7071067f, -0.7071068f, 0f, 1.236345E-07f, 0.7071068f, 0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 0f, 1.748456E-07f)
            };
            // 852. - X: 90, Y: 0, Z: 360
            yield return new object[]
            {
                "X: 90, Y: 0, Z: 360",
                new float3(1.570796f, 0f, 6.283185f),
                new Quaternion(-0.7071068f, 6.181724E-08f, -6.181724E-08f, -0.7071068f),
                new float4x4(1f, -1.748456E-07f, 0f, 0f, 0f, 5.960464E-08f, -0.9999999f, 0f, 1.748456E-07f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, -1.748456E-07f, 0f)
            };
            // 853. - X: 180, Y: 0, Z: 360
            yield return new object[]
            {
                "X: 180, Y: 0, Z: 360",
                new float3(3.141593f, 0f, 6.283185f),
                new Quaternion(-1f, 8.742278E-08f, 3.821371E-15f, 4.371139E-08f),
                new float4x4(1f, -1.748456E-07f, 0f, 0f, -1.748456E-07f, -1f, 8.742278E-08f, 0f, -1.528548E-14f, -8.742278E-08f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 3.141593f, 3.141593f)
            };
            // 854. - X: 270, Y: 0, Z: 360
            yield return new object[]
            {
                "X: 270, Y: 0, Z: 360",
                new float3(4.712389f, 0f, 6.283185f),
                new Quaternion(-0.7071068f, 6.181724E-08f, 6.181724E-08f, 0.7071068f),
                new float4x4(1f, -1.748456E-07f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, -1.748456E-07f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.748456E-07f, 0f)
            };
            // 855. - X: 360, Y: 0, Z: 360
            yield return new object[]
            {
                "X: 360, Y: 0, Z: 360",
                new float3(6.283185f, 0f, 6.283185f),
                new Quaternion(8.742278E-08f, -7.642742E-15f, 8.742278E-08f, 1f),
                new float4x4(1f, -1.748456E-07f, 0f, 0f, 1.748456E-07f, 1f, -1.748456E-07f, 0f, 3.057097E-14f, 1.748456E-07f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 0f, 1.748456E-07f)
            };
            // 856. - X: 585, Y: 0, Z: 360
            yield return new object[]
            {
                "X: 585, Y: 0, Z: 360",
                new float3(10.21018f, 0f, 6.283185f),
                new Quaternion(0.9238794f, -8.07681E-08f, -3.345526E-08f, -0.3826836f),
                new float4x4(1f, -1.748455E-07f, 0f, 0f, -1.236344E-07f, -0.7071065f, 0.707107f, 0f, -1.236345E-07f, -0.707107f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.141593f, 3.141593f)
            };
            // 857. - X: -90, Y: 0, Z: 360
            yield return new object[]
            {
                "X: -90, Y: 0, Z: 360",
                new float3(-1.570796f, 0f, 6.283185f),
                new Quaternion(0.7071068f, -6.181724E-08f, -6.181724E-08f, -0.7071068f),
                new float4x4(1f, -1.748456E-07f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, -1.748456E-07f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.748456E-07f, 0f)
            };
            // 858. - X: -540, Y: 0, Z: 360
            yield return new object[]
            {
                "X: -540, Y: 0, Z: 360",
                new float3(-9.424778f, 0f, 6.283185f),
                new Quaternion(-1f, 8.742278E-08f, -1.042506E-15f, -1.192488E-08f),
                new float4x4(1f, -1.748456E-07f, 0f, 0f, -1.748456E-07f, -1f, -2.384976E-08f, 0f, 4.170025E-15f, 2.384976E-08f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.141593f, 3.141593f)
            };
            // 859. - X: 0, Y: 0.1, Z: 360
            yield return new object[]
            {
                "X: 0, Y: 0.1, Z: 360",
                new float3(0f, 0.001745329f, 6.283185f),
                new Quaternion(-7.629075E-11f, -0.0008726645f, -8.742275E-08f, -0.9999996f),
                new float4x4(0.9999985f, -1.748453E-07f, 0.001745328f, 0f, 1.748456E-07f, 1f, 1.387779E-17f, 0f, -0.001745328f, 3.051629E-10f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(-1.387779E-17f, 0.001745329f, 1.748456E-07f)
            };
            // 860. - X: 0.1, Y: 0.1, Z: 360
            yield return new object[]
            {
                "X: 0.1, Y: 0.1, Z: 360",
                new float3(0.001745329f, 0.001745329f, 6.283185f),
                new Quaternion(-0.0008726643f, -0.0008726642f, 6.741206E-07f, -0.9999993f),
                new float4x4(0.9999985f, 2.871326E-06f, 0.001745326f, 0f, 1.748454E-07f, 0.9999985f, -0.001745328f, 0f, -0.001745328f, 0.001745326f, 0.999997f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0.001745329f, 1.748456E-07f)
            };
            // 861. - X: 1, Y: 0.1, Z: 360
            yield return new object[]
            {
                "X: 1, Y: 0.1, Z: 360",
                new float3(0.01745329f, 0.001745329f, 6.283185f),
                new Quaternion(-0.008726533f, -0.0008726305f, 7.527919E-06f, -0.9999616f),
                new float4x4(0.9999985f, 3.028534E-05f, 0.001745063f, 0f, 1.748185E-07f, 0.9998477f, -0.01745241f, 0f, -0.001745325f, 0.01745238f, 0.9998462f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0.001745329f, 1.748452E-07f)
            };
            // 862. - X: 45, Y: 0.1, Z: 360
            yield return new object[]
            {
                "X: 45, Y: 0.1, Z: 360",
                new float3(0.7853982f, 0.001745329f, 6.283185f),
                new Quaternion(-0.3826833f, -0.0008062034f, 0.0003338735f, -0.9238791f),
                new float4x4(0.9999985f, 0.001233959f, 0.001234133f, 0f, 1.236913E-07f, 0.7071068f, -0.7071068f, 0f, -0.001745205f, 0.7071057f, 0.7071057f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 0.001745329f, 1.749259E-07f)
            };
            // 863. - X: 90, Y: 0.1, Z: 360
            yield return new object[]
            {
                "X: 90, Y: 0.1, Z: 360",
                new float3(1.570796f, 0.001745329f, 6.283185f),
                new Quaternion(-0.7071065f, -0.0006170052f, 0.0006170052f, -0.7071065f),
                new float4x4(0.9999985f, 0.001745154f, 0f, 0f, 0f, -4.613503E-08f, -1f, 0f, -0.001745154f, 0.9999985f, -4.613503E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 0.001745154f, 0f)
            };
            // 864. - X: 180, Y: 0.1, Z: 360
            yield return new object[]
            {
                "X: 180, Y: 0.1, Z: 360",
                new float3(3.141593f, 0.001745329f, 6.283185f),
                new Quaternion(-0.9999996f, 8.746089E-08f, 0.0008726645f, 4.363508E-08f),
                new float4x4(0.9999985f, -1.749979E-07f, -0.001745328f, 0f, -1.748456E-07f, -1f, 8.742278E-08f, 0f, -0.001745328f, -8.711749E-08f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 3.143338f, 3.141593f)
            };
            // 865. - X: 270, Y: 0.1, Z: 360
            yield return new object[]
            {
                "X: 270, Y: 0.1, Z: 360",
                new float3(4.712389f, 0.001745329f, 6.283185f),
                new Quaternion(-0.7071065f, 0.0006171288f, 0.0006171288f, 0.7071065f),
                new float4x4(0.9999985f, -0.001745503f, 0f, 0f, 0f, -4.644016E-08f, 1f, 0f, -0.001745503f, -0.9999985f, -4.644016E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.001745504f, 0f)
            };
            // 866. - X: 360, Y: 0.1, Z: 360
            yield return new object[]
            {
                "X: 360, Y: 0.1, Z: 360",
                new float3(6.283185f, 0.001745329f, 6.283185f),
                new Quaternion(8.749904E-08f, 0.0008726645f, 8.734646E-08f, 0.9999996f),
                new float4x4(0.9999985f, -1.745401E-07f, 0.001745328f, 0f, 1.748456E-07f, 1f, -1.748456E-07f, 0f, -0.001745328f, 1.751505E-07f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 0.001745329f, 1.748456E-07f)
            };
            // 867. - X: 585, Y: 0.1, Z: 360
            yield return new object[]
            {
                "X: 585, Y: 0.1, Z: 360",
                new float3(10.21018f, 0.001745329f, 6.283185f),
                new Quaternion(0.9238791f, -0.0003340352f, -0.0008062703f, -0.3826835f),
                new float4x4(0.9999985f, -0.001234309f, -0.001234133f, 0f, -1.236331E-07f, -0.7071065f, 0.707107f, 0f, -0.001745452f, -0.7071059f, -0.7071054f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.143338f, 3.141593f)
            };
            // 868. - X: -90, Y: 0.1, Z: 360
            yield return new object[]
            {
                "X: -90, Y: 0.1, Z: 360",
                new float3(-1.570796f, 0.001745329f, 6.283185f),
                new Quaternion(0.7071065f, -0.0006171288f, -0.0006171288f, -0.7071065f),
                new float4x4(0.9999985f, -0.001745503f, 0f, 0f, 0f, -4.644016E-08f, 1f, 0f, -0.001745503f, -0.9999985f, -4.644016E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.001745504f, 0f)
            };
            // 869. - X: -540, Y: 0.1, Z: 360
            yield return new object[]
            {
                "X: -540, Y: 0.1, Z: 360",
                new float3(-9.424778f, 0.001745329f, 6.283185f),
                new Quaternion(-0.9999996f, 8.741234E-08f, 0.0008726645f, -1.200117E-08f),
                new float4x4(0.9999985f, -1.748037E-07f, -0.001745328f, 0f, -1.748456E-07f, -1f, -2.384976E-08f, 0f, -0.001745328f, 2.415489E-08f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.143338f, 3.141593f)
            };
            // 870. - X: 0, Y: 1, Z: 360
            yield return new object[]
            {
                "X: 0, Y: 1, Z: 360",
                new float3(0f, 0.01745329f, 6.283185f),
                new Quaternion(-7.62898E-10f, -0.008726535f, -8.741944E-08f, -0.9999619f),
                new float4x4(0.9998477f, -1.748189E-07f, 0.01745241f, 0f, 1.748455E-07f, 1f, -1.110223E-16f, 0f, -0.01745241f, 3.051476E-09f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.110223E-16f, 0.01745329f, 1.748455E-07f)
            };
            // 871. - X: 0.1, Y: 1, Z: 360
            yield return new object[]
            {
                "X: 0.1, Y: 1, Z: 360",
                new float3(0.001745329f, 0.01745329f, 6.283185f),
                new Quaternion(-0.000872632f, -0.008726533f, 7.527919E-06f, -0.9999616f),
                new float4x4(0.9998477f, 3.028536E-05f, 0.01745238f, 0f, 1.748449E-07f, 0.9999985f, -0.001745328f, 0f, -0.01745241f, 0.001745066f, 0.9998462f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0.01745329f, 1.748452E-07f)
            };
            // 872. - X: 1, Y: 1, Z: 360
            yield return new object[]
            {
                "X: 1, Y: 1, Z: 360",
                new float3(0.01745329f, 0.01745329f, 6.283185f),
                new Quaternion(-0.008726204f, -0.008726202f, 7.606501E-05f, -0.9999238f),
                new float4x4(0.9998477f, 0.0003044117f, 0.01744975f, 0f, 1.747976E-07f, 0.9998477f, -0.01745241f, 0f, -0.0174524f, 0.01744975f, 0.9996954f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.0174533f, 0.01745329f, 1.748533E-07f)
            };
            // 873. - X: 45, Y: 1, Z: 360
            yield return new object[]
            {
                "X: 45, Y: 1, Z: 360",
                new float3(0.7853982f, 0.01745329f, 6.283185f),
                new Quaternion(-0.3826689f, -0.008062233f, 0.00333942f, -0.9238443f),
                new float4x4(0.9998477f, 0.01234054f, 0.01234071f, 0f, 1.234002E-07f, 0.7071068f, -0.7071068f, 0f, -0.01745228f, 0.7069991f, 0.7069991f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 0.01745329f, 1.745143E-07f)
            };
            // 874. - X: 90, Y: 1, Z: 360
            yield return new object[]
            {
                "X: 90, Y: 1, Z: 360",
                new float3(1.570796f, 0.01745329f, 6.283185f),
                new Quaternion(-0.7070798f, -0.00617053f, 0.00617053f, -0.7070798f),
                new float4x4(0.9998477f, 0.01745223f, 0f, 0f, 0f, 8.344796E-08f, -0.9999999f, 0f, -0.01745223f, 0.9998476f, 8.344796E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 0.01745312f, 0f)
            };
            // 875. - X: 180, Y: 1, Z: 360
            yield return new object[]
            {
                "X: 180, Y: 1, Z: 360",
                new float3(3.141593f, 0.01745329f, 6.283185f),
                new Quaternion(-0.9999619f, 8.780089E-08f, 0.008726535f, 4.294682E-08f),
                new float4x4(0.9998477f, -1.763446E-07f, -0.01745241f, 0f, -1.748455E-07f, -0.9999999f, 8.742277E-08f, 0f, -0.01745241f, -8.435799E-08f, -0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742277E-08f, 3.159046f, 3.141593f)
            };
            // 876. - X: 270, Y: 1, Z: 360
            yield return new object[]
            {
                "X: 270, Y: 1, Z: 360",
                new float3(4.712389f, 0.01745329f, 6.283185f),
                new Quaternion(-0.7070798f, 0.006170654f, 0.006170654f, 0.7070798f),
                new float4x4(0.9998477f, -0.01745258f, 0f, 0f, 0f, 8.039206E-08f, 0.9999999f, 0f, -0.01745258f, -0.9998476f, 8.039206E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.01745347f, 0f)
            };
            // 877. - X: 360, Y: 1, Z: 360
            yield return new object[]
            {
                "X: 360, Y: 1, Z: 360",
                new float3(6.283185f, 0.01745329f, 6.283185f),
                new Quaternion(8.818234E-08f, 0.008726535f, 8.665655E-08f, 0.9999619f),
                new float4x4(0.9998477f, -1.717674E-07f, 0.01745241f, 0f, 1.748455E-07f, 1f, -1.748455E-07f, 0f, -0.01745241f, 1.778704E-07f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748455E-07f, 0.01745329f, 1.748455E-07f)
            };
            // 878. - X: 585, Y: 1, Z: 360
            yield return new object[]
            {
                "X: 585, Y: 1, Z: 360",
                new float3(10.21018f, 0.01745329f, 6.283185f),
                new Quaternion(0.9238443f, -0.003339583f, -0.0080623f, -0.382669f),
                new float4x4(0.9998477f, -0.01234089f, -0.01234071f, 0f, -1.238659E-07f, -0.7071065f, 0.707107f, 0f, -0.01745253f, -0.7069994f, -0.7069988f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.159046f, 3.141593f)
            };
            // 879. - X: -90, Y: 1, Z: 360
            yield return new object[]
            {
                "X: -90, Y: 1, Z: 360",
                new float3(-1.570796f, 0.01745329f, 6.283185f),
                new Quaternion(0.7070798f, -0.006170654f, -0.006170654f, -0.7070798f),
                new float4x4(0.9998477f, -0.01745258f, 0f, 0f, 0f, 8.039206E-08f, 0.9999999f, 0f, -0.01745258f, -0.9998476f, 8.039206E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.01745347f, 0f)
            };
            // 880. - X: -540, Y: 1, Z: 360
            yield return new object[]
            {
                "X: -540, Y: 1, Z: 360",
                new float3(-9.424778f, 0.01745329f, 6.283185f),
                new Quaternion(-0.9999619f, 8.731538E-08f, 0.008726535f, -1.268732E-08f),
                new float4x4(0.9998477f, -1.744027E-07f, -0.01745241f, 0f, -1.748455E-07f, -0.9999999f, -2.384976E-08f, 0f, -0.01745241f, 2.689761E-08f, -0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.159046f, 3.141593f)
            };
            // 881. - X: 0, Y: 45, Z: 360
            yield return new object[]
            {
                "X: 0, Y: 45, Z: 360",
                new float3(0f, 0.7853982f, 6.283185f),
                new Quaternion(-3.345525E-08f, -0.3826835f, -8.076811E-08f, -0.9238795f),
                new float4x4(0.7071067f, -1.236345E-07f, 0.7071068f, 0f, 1.748456E-07f, 1f, 0f, 0f, -0.7071068f, 1.236345E-07f, 0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0.7853982f, 1.748456E-07f)
            };
            // 882. - X: 0.1, Y: 45, Z: 360
            yield return new object[]
            {
                "X: 0.1, Y: 45, Z: 360",
                new float3(0.001745329f, 0.7853982f, 6.283185f),
                new Quaternion(-0.0008062703f, -0.3826833f, 0.0003338735f, -0.9238791f),
                new float4x4(0.7071068f, 0.00123401f, 0.7071057f, 0f, 1.74914E-07f, 0.9999985f, -0.001745328f, 0f, -0.7071068f, 0.001234257f, 0.7071057f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0.7853982f, 1.749143E-07f)
            };
            // 883. - X: 1, Y: 45, Z: 360
            yield return new object[]
            {
                "X: 1, Y: 45, Z: 360",
                new float3(0.01745329f, 0.7853982f, 6.283185f),
                new Quaternion(-0.0080623f, -0.3826689f, 0.00333942f, -0.9238443f),
                new float4x4(0.7071068f, 0.01234059f, 0.7069991f, 0f, 1.74623E-07f, 0.9998477f, -0.01745241f, 0f, -0.7071068f, 0.01234084f, 0.7069991f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0.7853982f, 1.746496E-07f)
            };
            // 884. - X: 45, Y: 45, Z: 360
            yield return new object[]
            {
                "X: 45, Y: 45, Z: 360",
                new float3(0.7853982f, 0.7853982f, 6.283185f),
                new Quaternion(-0.3535534f, -0.3535534f, 0.1464466f, -0.8535534f),
                new float4x4(0.7071068f, 0.4999999f, 0.5f, 0f, 1.341105E-07f, 0.7071067f, -0.7071068f, 0f, -0.7071067f, 0.5000001f, 0.4999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 0.7853982f, 1.896608E-07f)
            };
            // 885. - X: 90, Y: 45, Z: 360
            yield return new object[]
            {
                "X: 90, Y: 45, Z: 360",
                new float3(1.570796f, 0.7853982f, 6.283185f),
                new Quaternion(-0.6532815f, -0.270598f, 0.270598f, -0.6532815f),
                new float4x4(0.7071069f, 0.7071066f, 0f, 0f, 0f, 1.490116E-07f, -0.9999999f, 0f, -0.7071066f, 0.7071067f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 0.7853981f, 0f)
            };
            // 886. - X: 180, Y: 45, Z: 360
            yield return new object[]
            {
                "X: 180, Y: 45, Z: 360",
                new float3(3.141593f, 0.7853982f, 6.283185f),
                new Quaternion(-0.9238795f, 9.749574E-08f, 0.3826835f, 6.928804E-09f),
                new float4x4(0.7071067f, -1.854517E-07f, -0.7071068f, 0f, -1.748455E-07f, -1f, 8.742277E-08f, 0f, -0.7071068f, 6.181725E-08f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742277E-08f, 3.926991f, 3.141593f)
            };
            // 887. - X: 270, Y: 45, Z: 360
            yield return new object[]
            {
                "X: 270, Y: 45, Z: 360",
                new float3(4.712389f, 0.7853982f, 6.283185f),
                new Quaternion(-0.6532815f, 0.2705981f, 0.2705981f, 0.6532815f),
                new float4x4(0.7071066f, -0.7071069f, 0f, 0f, 0f, 2.980232E-08f, 1f, 0f, -0.7071069f, -0.7071066f, 2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.7853984f, 0f)
            };
            // 888. - X: 360, Y: 45, Z: 360
            yield return new object[]
            {
                "X: 360, Y: 45, Z: 360",
                new float3(6.283185f, 0.7853982f, 6.283185f),
                new Quaternion(1.142234E-07f, 0.3826835f, 4.731286E-08f, 0.9238795f),
                new float4x4(0.7071067f, 2.131628E-14f, 0.7071068f, 0f, 1.748456E-07f, 1f, -1.748455E-07f, 0f, -0.7071068f, 2.472689E-07f, 0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748455E-07f, 0.7853982f, 1.748456E-07f)
            };
            // 889. - X: 585, Y: 45, Z: 360
            yield return new object[]
            {
                "X: 585, Y: 45, Z: 360",
                new float3(10.21018f, 0.7853982f, 6.283185f),
                new Quaternion(0.8535533f, -0.1464468f, -0.3535534f, -0.3535535f),
                new float4x4(0.7071066f, -0.5000004f, -0.4999998f, 0f, -1.490116E-07f, -0.7071065f, 0.7071071f, 0f, -0.7071069f, -0.5f, -0.4999998f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.926991f, 3.141593f)
            };
            // 890. - X: -90, Y: 45, Z: 360
            yield return new object[]
            {
                "X: -90, Y: 45, Z: 360",
                new float3(-1.570796f, 0.7853982f, 6.283185f),
                new Quaternion(0.6532815f, -0.2705981f, -0.2705981f, -0.6532815f),
                new float4x4(0.7071066f, -0.7071069f, 0f, 0f, 0f, 2.980232E-08f, 1f, 0f, -0.7071069f, -0.7071066f, 2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.7853984f, 0f)
            };
            // 891. - X: -540, Y: 45, Z: 360
            yield return new object[]
            {
                "X: -540, Y: 45, Z: 360",
                new float3(-9.424778f, 0.7853982f, 6.283185f),
                new Quaternion(-0.9238795f, 7.620466E-08f, 0.3826835f, -4.44724E-08f),
                new float4x4(0.7071067f, -1.067701E-07f, -0.7071068f, 0f, -1.748456E-07f, -1f, -2.384976E-08f, 0f, -0.7071068f, 1.404988E-07f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.926991f, 3.141593f)
            };
            // 892. - X: 0, Y: 90, Z: 360
            yield return new object[]
            {
                "X: 0, Y: 90, Z: 360",
                new float3(0f, 1.570796f, 6.283185f),
                new Quaternion(-6.181724E-08f, -0.7071068f, -6.181724E-08f, -0.7071068f),
                new float4x4(5.960464E-08f, 0f, 0.9999999f, 0f, 1.748456E-07f, 1f, 0f, 0f, -0.9999999f, 1.748456E-07f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.570796f, 1.748456E-07f)
            };
            // 893. - X: 0.1, Y: 90, Z: 360
            yield return new object[]
            {
                "X: 0.1, Y: 90, Z: 360",
                new float3(0.001745329f, 1.570796f, 6.283185f),
                new Quaternion(-0.0006171288f, -0.7071065f, 0.0006170052f, -0.7071065f),
                new float4x4(-4.613503E-08f, 0.001745328f, 0.9999985f, 0f, 1.747976E-07f, 0.9999985f, -0.001745328f, 0f, -1f, 1.747976E-07f, -4.644016E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 1.570796f, 1.747979E-07f)
            };
            // 894. - X: 1, Y: 90, Z: 360
            yield return new object[]
            {
                "X: 1, Y: 90, Z: 360",
                new float3(0.01745329f, 1.570796f, 6.283185f),
                new Quaternion(-0.006170654f, -0.7070798f, 0.00617053f, -0.7070798f),
                new float4x4(8.344796E-08f, 0.01745241f, 0.9998476f, 0f, 1.750886E-07f, 0.9998477f, -0.01745241f, 0f, -0.9999999f, 1.750886E-07f, 8.039206E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 1.570796f, 1.751153E-07f)
            };
            // 895. - X: 45, Y: 90, Z: 360
            yield return new object[]
            {
                "X: 45, Y: 90, Z: 360",
                new float3(0.7853982f, 1.570796f, 6.283185f),
                new Quaternion(-0.2705981f, -0.6532815f, 0.270598f, -0.6532815f),
                new float4x4(1.490116E-07f, 0.7071067f, 0.7071067f, 0f, 1.490116E-07f, 0.7071068f, -0.7071067f, 0f, -0.9999999f, 1.490116E-07f, 2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853984f, 1.570796f, 2.107342E-07f)
            };
            // 896. - X: 90, Y: 90, Z: 360
            yield return new object[]
            {
                "X: 90, Y: 90, Z: 360",
                new float3(1.570796f, 1.570796f, 6.283185f),
                new Quaternion(-0.5f, -0.4999999f, 0.4999999f, -0.5f),
                new float4x4(2.384186E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 1.570796f, 0f)
            };
            // 897. - X: 180, Y: 90, Z: 360
            yield return new object[]
            {
                "X: 180, Y: 90, Z: 360",
                new float3(3.141593f, 1.570796f, 6.283185f),
                new Quaternion(-0.7071068f, 9.272586E-08f, 0.7071068f, -3.090862E-08f),
                new float4x4(5.960463E-08f, -8.742277E-08f, -0.9999999f, 0f, -1.748456E-07f, -0.9999999f, 8.742277E-08f, 0f, -0.9999999f, 1.748456E-07f, 5.960463E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742279E-08f, 4.712389f, 3.141593f)
            };
            // 898. - X: 270, Y: 90, Z: 360
            yield return new object[]
            {
                "X: 270, Y: 90, Z: 360",
                new float3(4.712389f, 1.570796f, 6.283185f),
                new Quaternion(-0.4999999f, 0.5f, 0.5f, 0.4999999f),
                new float4x4(0f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 899. - X: 360, Y: 90, Z: 360
            yield return new object[]
            {
                "X: 360, Y: 90, Z: 360",
                new float3(6.283185f, 1.570796f, 6.283185f),
                new Quaternion(1.236345E-07f, 0.7071068f, 0f, 0.7071068f),
                new float4x4(5.960464E-08f, 1.748456E-07f, 0.9999999f, 0f, 1.748456E-07f, 1f, -1.748456E-07f, 0f, -0.9999999f, 1.748456E-07f, 5.960461E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 1.570796f, 1.748456E-07f)
            };
            // 900. - X: 585, Y: 90, Z: 360
            yield return new object[]
            {
                "X: 585, Y: 90, Z: 360",
                new float3(10.21018f, 1.570796f, 6.283185f),
                new Quaternion(0.6532814f, -0.2705982f, -0.6532814f, -0.2705981f),
                new float4x4(1.490116E-08f, -0.707107f, -0.7071064f, 0f, -1.788139E-07f, -0.7071064f, 0.707107f, 0f, -0.9999999f, 1.788139E-07f, 1.490116E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 4.712389f, 3.141593f)
            };
            // 901. - X: -90, Y: 90, Z: 360
            yield return new object[]
            {
                "X: -90, Y: 90, Z: 360",
                new float3(-1.570796f, 1.570796f, 6.283185f),
                new Quaternion(0.4999999f, -0.5f, -0.5f, -0.4999999f),
                new float4x4(0f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 902. - X: -540, Y: 90, Z: 360
            yield return new object[]
            {
                "X: -540, Y: 90, Z: 360",
                new float3(-9.424778f, 1.570796f, 6.283185f),
                new Quaternion(-0.7071068f, 5.338508E-08f, 0.7071068f, -7.02494E-08f),
                new float4x4(5.960464E-08f, 2.384976E-08f, -0.9999999f, 0f, -1.748456E-07f, -0.9999999f, -2.384976E-08f, 0f, -0.9999999f, 1.748456E-07f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 4.712389f, 3.141593f)
            };
            // 903. - X: 0, Y: 180, Z: 360
            yield return new object[]
            {
                "X: 0, Y: 180, Z: 360",
                new float3(0f, 3.141593f, 6.283185f),
                new Quaternion(-8.742278E-08f, -1f, 3.821371E-15f, 4.371139E-08f),
                new float4x4(-1f, 1.748456E-07f, -8.742278E-08f, 0f, 1.748456E-07f, 1f, 0f, 0f, 8.742278E-08f, -1.528548E-14f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.141593f, 1.748456E-07f)
            };
            // 904. - X: 0.1, Y: 180, Z: 360
            yield return new object[]
            {
                "X: 0.1, Y: 180, Z: 360",
                new float3(0.001745329f, 3.141593f, 6.283185f),
                new Quaternion(-8.738461E-08f, -0.9999996f, 0.0008726645f, 4.363508E-08f),
                new float4x4(-1f, 1.74693E-07f, -8.742266E-08f, 0f, 1.748453E-07f, 0.9999985f, -0.001745328f, 0f, 8.711762E-08f, -0.001745328f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.141593f, 1.748456E-07f)
            };
            // 905. - X: 1, Y: 180, Z: 360
            yield return new object[]
            {
                "X: 1, Y: 180, Z: 360",
                new float3(0.01745329f, 3.141593f, 6.283185f),
                new Quaternion(-8.7038E-08f, -0.9999619f, 0.008726535f, 4.294682E-08f),
                new float4x4(-0.9999999f, 1.733198E-07f, -8.740946E-08f, 0f, 1.748189E-07f, 0.9998477f, -0.01745241f, 0f, 8.437129E-08f, -0.01745241f, -0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.141593f, 1.748455E-07f)
            };
            // 906. - X: 45, Y: 180, Z: 360
            yield return new object[]
            {
                "X: 45, Y: 180, Z: 360",
                new float3(0.7853982f, 3.141593f, 6.283185f),
                new Quaternion(-6.404048E-08f, -0.9238795f, 0.3826835f, 6.928804E-09f),
                new float4x4(-1f, 1.130283E-07f, -6.181723E-08f, 0f, 1.236345E-07f, 0.7071067f, -0.7071068f, 0f, -3.621171E-08f, -0.7071068f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 3.141593f, 1.748456E-07f)
            };
            // 907. - X: 90, Y: 180, Z: 360
            yield return new object[]
            {
                "X: 90, Y: 180, Z: 360",
                new float3(1.570796f, 3.141593f, 6.283185f),
                new Quaternion(-3.090862E-08f, -0.7071068f, 0.7071068f, -3.090862E-08f),
                new float4x4(-0.9999999f, 8.742278E-08f, 0f, 0f, 0f, 5.960464E-08f, -0.9999999f, 0f, -8.742278E-08f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 3.141593f, 0f)
            };
            // 908. - X: 180, Y: 180, Z: 360
            yield return new object[]
            {
                "X: 180, Y: 180, Z: 360",
                new float3(3.141593f, 3.141593f, 6.283185f),
                new Quaternion(4.371139E-08f, 4.371138E-08f, 1f, -8.742278E-08f),
                new float4x4(-1f, 1.748456E-07f, 8.742278E-08f, 0f, -1.748456E-07f, -1f, 8.742278E-08f, 0f, 8.742279E-08f, 8.742276E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 8.742278E-08f, 3.141593f)
            };
            // 909. - X: 270, Y: 180, Z: 360
            yield return new object[]
            {
                "X: 270, Y: 180, Z: 360",
                new float3(4.712389f, 3.141593f, 6.283185f),
                new Quaternion(9.272586E-08f, 0.7071068f, 0.7071068f, -9.272586E-08f),
                new float4x4(-0.9999999f, 2.622683E-07f, 0f, 0f, 0f, 5.960463E-08f, 0.9999999f, 0f, 2.622683E-07f, 0.9999999f, 5.960463E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.141593f, 0f)
            };
            // 910. - X: 360, Y: 180, Z: 360
            yield return new object[]
            {
                "X: 360, Y: 180, Z: 360",
                new float3(6.283185f, 3.141593f, 6.283185f),
                new Quaternion(8.742277E-08f, 1f, -8.742278E-08f, -4.371138E-08f),
                new float4x4(-1f, 1.748455E-07f, -8.742278E-08f, 0f, 1.748456E-07f, 1f, -1.748456E-07f, 0f, 8.742275E-08f, -1.748456E-07f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 3.141593f, 1.748456E-07f)
            };
            // 911. - X: 585, Y: 180, Z: 360
            yield return new object[]
            {
                "X: 585, Y: 180, Z: 360",
                new float3(10.21018f, 3.141593f, 6.283185f),
                new Quaternion(-7.383932E-08f, -0.3826836f, -0.9238794f, 9.749574E-08f),
                new float4x4(-0.9999999f, 2.366628E-07f, 6.181721E-08f, 0f, -1.236344E-07f, -0.7071065f, 0.707107f, 0f, 2.110573E-07f, 0.707107f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 8.742278E-08f, 3.141593f)
            };
            // 912. - X: -90, Y: 180, Z: 360
            yield return new object[]
            {
                "X: -90, Y: 180, Z: 360",
                new float3(-1.570796f, 3.141593f, 6.283185f),
                new Quaternion(-9.272586E-08f, -0.7071068f, -0.7071068f, 9.272586E-08f),
                new float4x4(-0.9999999f, 2.622683E-07f, 0f, 0f, 0f, 5.960463E-08f, 0.9999999f, 0f, 2.622683E-07f, 0.9999999f, 5.960463E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.141593f, 0f)
            };
            // 913. - X: -540, Y: 180, Z: 360
            yield return new object[]
            {
                "X: -540, Y: 180, Z: 360",
                new float3(-9.424778f, 3.141593f, 6.283185f),
                new Quaternion(4.371139E-08f, -1.192488E-08f, 1f, -8.742278E-08f),
                new float4x4(-1f, 1.748456E-07f, 8.742278E-08f, 0f, -1.748456E-07f, -1f, -2.384976E-08f, 0f, 8.742278E-08f, -2.384978E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 8.742278E-08f, 3.141593f)
            };
            // 914. - X: 0, Y: 270, Z: 360
            yield return new object[]
            {
                "X: 0, Y: 270, Z: 360",
                new float3(0f, 4.712389f, 6.283185f),
                new Quaternion(-6.181724E-08f, -0.7071068f, 6.181724E-08f, 0.7071068f),
                new float4x4(5.960464E-08f, 0f, -0.9999999f, 0f, 1.748456E-07f, 1f, 0f, 0f, 0.9999999f, -1.748456E-07f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 1.748456E-07f)
            };
            // 915. - X: 0.1, Y: 270, Z: 360
            yield return new object[]
            {
                "X: 0.1, Y: 270, Z: 360",
                new float3(0.001745329f, 4.712389f, 6.283185f),
                new Quaternion(0.0006170052f, -0.7071065f, 0.0006171288f, 0.7071065f),
                new float4x4(-4.644016E-08f, -0.001745328f, -0.9999985f, 0f, 1.747976E-07f, 0.9999985f, -0.001745328f, 0f, 1f, -1.747976E-07f, -4.613503E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 4.712389f, 1.747979E-07f)
            };
            // 916. - X: 1, Y: 270, Z: 360
            yield return new object[]
            {
                "X: 1, Y: 270, Z: 360",
                new float3(0.01745329f, 4.712389f, 6.283185f),
                new Quaternion(0.00617053f, -0.7070798f, 0.006170654f, 0.7070798f),
                new float4x4(8.039206E-08f, -0.01745241f, -0.9998476f, 0f, 1.750886E-07f, 0.9998477f, -0.01745241f, 0f, 0.9999999f, -1.750886E-07f, 8.344796E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 4.712389f, 1.751153E-07f)
            };
            // 917. - X: 45, Y: 270, Z: 360
            yield return new object[]
            {
                "X: 45, Y: 270, Z: 360",
                new float3(0.7853982f, 4.712389f, 6.283185f),
                new Quaternion(0.270598f, -0.6532815f, 0.2705981f, 0.6532815f),
                new float4x4(2.980232E-08f, -0.7071067f, -0.7071067f, 0f, 1.490116E-07f, 0.7071068f, -0.7071067f, 0f, 0.9999999f, -1.490116E-07f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853984f, 4.712389f, 2.107342E-07f)
            };
            // 918. - X: 90, Y: 270, Z: 360
            yield return new object[]
            {
                "X: 90, Y: 270, Z: 360",
                new float3(1.570796f, 4.712389f, 6.283185f),
                new Quaternion(0.4999999f, -0.5f, 0.5f, 0.4999999f),
                new float4x4(0f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, 0.9999999f, -1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 4.712389f, 0f)
            };
            // 919. - X: 180, Y: 270, Z: 360
            yield return new object[]
            {
                "X: 180, Y: 270, Z: 360",
                new float3(3.141593f, 4.712389f, 6.283185f),
                new Quaternion(0.7071068f, -3.090862E-08f, 0.7071068f, -9.272586E-08f),
                new float4x4(5.960464E-08f, 8.742277E-08f, 0.9999999f, 0f, -1.748456E-07f, -0.9999999f, 8.742277E-08f, 0f, 0.9999999f, -1.748456E-07f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742279E-08f, 1.570796f, 3.141593f)
            };
            // 920. - X: 270, Y: 270, Z: 360
            yield return new object[]
            {
                "X: 270, Y: 270, Z: 360",
                new float3(4.712389f, 4.712389f, 6.283185f),
                new Quaternion(0.5f, 0.4999999f, 0.4999999f, -0.5f),
                new float4x4(2.384186E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, 0.9999999f, -1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 921. - X: 360, Y: 270, Z: 360
            yield return new object[]
            {
                "X: 360, Y: 270, Z: 360",
                new float3(6.283185f, 4.712389f, 6.283185f),
                new Quaternion(0f, 0.7071068f, -1.236345E-07f, -0.7071068f),
                new float4x4(5.960461E-08f, -1.748456E-07f, -0.9999999f, 0f, 1.748456E-07f, 1f, -1.748456E-07f, 0f, 0.9999999f, -1.748456E-07f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 4.712389f, 1.748456E-07f)
            };
            // 922. - X: 585, Y: 270, Z: 360
            yield return new object[]
            {
                "X: 585, Y: 270, Z: 360",
                new float3(10.21018f, 4.712389f, 6.283185f),
                new Quaternion(-0.6532814f, -0.2705981f, -0.6532814f, 0.2705982f),
                new float4x4(1.490116E-07f, 0.707107f, 0.7071064f, 0f, -1.788139E-07f, -0.7071064f, 0.707107f, 0f, 0.9999999f, -1.788139E-07f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.570796f, 3.141593f)
            };
            // 923. - X: -90, Y: 270, Z: 360
            yield return new object[]
            {
                "X: -90, Y: 270, Z: 360",
                new float3(-1.570796f, 4.712389f, 6.283185f),
                new Quaternion(-0.5f, -0.4999999f, -0.4999999f, 0.5f),
                new float4x4(2.384186E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, 0.9999999f, -1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 924. - X: -540, Y: 270, Z: 360
            yield return new object[]
            {
                "X: -540, Y: 270, Z: 360",
                new float3(-9.424778f, 4.712389f, 6.283185f),
                new Quaternion(0.7071068f, -7.02494E-08f, 0.7071068f, -5.338508E-08f),
                new float4x4(5.960463E-08f, -2.384976E-08f, 0.9999999f, 0f, -1.748456E-07f, -0.9999999f, -2.384976E-08f, 0f, 0.9999999f, -1.748456E-07f, 5.960463E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 1.570796f, 3.141593f)
            };
            // 925. - X: 0, Y: 360, Z: 360
            yield return new object[]
            {
                "X: 0, Y: 360, Z: 360",
                new float3(0f, 6.283185f, 6.283185f),
                new Quaternion(7.642742E-15f, 8.742278E-08f, 8.742278E-08f, 1f),
                new float4x4(1f, -1.748456E-07f, 1.748456E-07f, 0f, 1.748456E-07f, 1f, 0f, 0f, -1.748456E-07f, 3.057097E-14f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.748456E-07f, 1.748456E-07f)
            };
            // 926. - X: 0.1, Y: 360, Z: 360
            yield return new object[]
            {
                "X: 0.1, Y: 360, Z: 360",
                new float3(0.001745329f, 6.283185f, 6.283185f),
                new Quaternion(0.0008726645f, 8.734646E-08f, 8.734646E-08f, 0.9999996f),
                new float4x4(1f, -1.745404E-07f, 1.748453E-07f, 0f, 1.748453E-07f, 0.9999985f, -0.001745328f, 0f, -1.745404E-07f, 0.001745328f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 1.748456E-07f, 1.748456E-07f)
            };
            // 927. - X: 1, Y: 360, Z: 360
            yield return new object[]
            {
                "X: 1, Y: 360, Z: 360",
                new float3(0.01745329f, 6.283185f, 6.283185f),
                new Quaternion(0.008726535f, 8.665655E-08f, 8.665655E-08f, 0.9999619f),
                new float4x4(1f, -1.717941E-07f, 1.748189E-07f, 0f, 1.748189E-07f, 0.9998477f, -0.01745241f, 0f, -1.717941E-07f, 0.01745241f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 1.748455E-07f, 1.748455E-07f)
            };
            // 928. - X: 45, Y: 360, Z: 360
            yield return new object[]
            {
                "X: 45, Y: 360, Z: 360",
                new float3(0.7853982f, 6.283185f, 6.283185f),
                new Quaternion(0.3826835f, 4.731286E-08f, 4.731286E-08f, 0.9238795f),
                new float4x4(1f, -5.121106E-08f, 1.236345E-07f, 0f, 1.236345E-07f, 0.7071067f, -0.7071068f, 0f, -5.121106E-08f, 0.7071068f, 0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 1.748456E-07f, 1.748456E-07f)
            };
            // 929. - X: 90, Y: 360, Z: 360
            yield return new object[]
            {
                "X: 90, Y: 360, Z: 360",
                new float3(1.570796f, 6.283185f, 6.283185f),
                new Quaternion(0.7071068f, 0f, 0f, 0.7071068f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 5.960464E-08f, -0.9999999f, 0f, 0f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 0f, 0f)
            };
            // 930. - X: 180, Y: 360, Z: 360
            yield return new object[]
            {
                "X: 180, Y: 360, Z: 360",
                new float3(3.141593f, 6.283185f, 6.283185f),
                new Quaternion(1f, -8.742278E-08f, -8.742278E-08f, -4.371138E-08f),
                new float4x4(1f, -1.748456E-07f, -1.748456E-07f, 0f, -1.748456E-07f, -1f, 8.742278E-08f, 0f, -1.748456E-07f, -8.742275E-08f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 3.141593f, 3.141593f)
            };
            // 931. - X: 270, Y: 360, Z: 360
            yield return new object[]
            {
                "X: 270, Y: 360, Z: 360",
                new float3(4.712389f, 6.283185f, 6.283185f),
                new Quaternion(0.7071068f, -1.236345E-07f, -1.236345E-07f, -0.7071068f),
                new float4x4(1f, -3.496911E-07f, 0f, 0f, 0f, 5.960461E-08f, 0.9999999f, 0f, -3.496911E-07f, -0.9999999f, 5.960461E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.496911E-07f, 0f)
            };
            // 932. - X: 360, Y: 360, Z: 360
            yield return new object[]
            {
                "X: 360, Y: 360, Z: 360",
                new float3(6.283185f, 6.283185f, 6.283185f),
                new Quaternion(-8.742278E-08f, -8.742277E-08f, -8.742277E-08f, -1f),
                new float4x4(1f, -1.748455E-07f, 1.748456E-07f, 0f, 1.748456E-07f, 1f, -1.748456E-07f, 0f, -1.748455E-07f, 1.748456E-07f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 1.748456E-07f, 1.748456E-07f)
            };
            // 933. - X: 585, Y: 360, Z: 360
            yield return new object[]
            {
                "X: 585, Y: 360, Z: 360",
                new float3(10.21018f, 6.283185f, 6.283185f),
                new Quaternion(-0.9238794f, 1.142234E-07f, 1.142234E-07f, 0.3826836f),
                new float4x4(1f, -2.984801E-07f, -1.236344E-07f, 0f, -1.236344E-07f, -0.7071065f, 0.707107f, 0f, -2.984801E-07f, -0.707107f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.141593f, 3.141593f)
            };
            // 934. - X: -90, Y: 360, Z: 360
            yield return new object[]
            {
                "X: -90, Y: 360, Z: 360",
                new float3(-1.570796f, 6.283185f, 6.283185f),
                new Quaternion(-0.7071068f, 1.236345E-07f, 1.236345E-07f, 0.7071068f),
                new float4x4(1f, -3.496911E-07f, 0f, 0f, 0f, 5.960461E-08f, 0.9999999f, 0f, -3.496911E-07f, -0.9999999f, 5.960461E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.496911E-07f, 0f)
            };
            // 935. - X: -540, Y: 360, Z: 360
            yield return new object[]
            {
                "X: -540, Y: 360, Z: 360",
                new float3(-9.424778f, 6.283185f, 6.283185f),
                new Quaternion(1f, -8.742278E-08f, -8.742278E-08f, 1.192489E-08f),
                new float4x4(1f, -1.748456E-07f, -1.748456E-07f, 0f, -1.748456E-07f, -1f, -2.384976E-08f, 0f, -1.748456E-07f, 2.384979E-08f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.141593f, 3.141593f)
            };
            // 936. - X: 0, Y: 585, Z: 360
            yield return new object[]
            {
                "X: 0, Y: 585, Z: 360",
                new float3(0f, 10.21018f, 6.283185f),
                new Quaternion(8.07681E-08f, 0.9238794f, -3.345526E-08f, -0.3826836f),
                new float4x4(-0.7071065f, 1.236344E-07f, -0.707107f, 0f, 1.748455E-07f, 1f, 0f, 0f, 0.707107f, -1.236345E-07f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.926991f, 1.748455E-07f)
            };
            // 937. - X: 0.1, Y: 585, Z: 360
            yield return new object[]
            {
                "X: 0.1, Y: 585, Z: 360",
                new float3(0.001745329f, 10.21018f, 6.283185f),
                new Quaternion(-0.0003338736f, 0.9238791f, -0.0008062703f, -0.3826835f),
                new float4x4(-0.7071065f, -0.00123401f, -0.7071059f, 0f, 1.748558E-07f, 0.9999985f, -0.001745328f, 0f, 0.707107f, -0.001234257f, -0.7071054f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.926991f, 1.748561E-07f)
            };
            // 938. - X: 1, Y: 585, Z: 360
            yield return new object[]
            {
                "X: 1, Y: 585, Z: 360",
                new float3(0.01745329f, 10.21018f, 6.283185f),
                new Quaternion(-0.003339421f, 0.9238443f, -0.0080623f, -0.382669f),
                new float4x4(-0.7071065f, -0.0123406f, -0.7069994f, 0f, 1.74623E-07f, 0.9998477f, -0.01745241f, 0f, 0.707107f, -0.01234083f, -0.7069988f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.926991f, 1.746496E-07f)
            };
            // 939. - X: 45, Y: 585, Z: 360
            yield return new object[]
            {
                "X: 45, Y: 585, Z: 360",
                new float3(0.7853982f, 10.21018f, 6.283185f),
                new Quaternion(-0.1464466f, 0.8535533f, -0.3535534f, -0.3535535f),
                new float4x4(-0.7071065f, -0.5000001f, -0.5000001f, 0f, 1.043081E-07f, 0.7071068f, -0.7071068f, 0f, 0.7071069f, -0.4999999f, -0.4999997f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853984f, 3.926991f, 1.685874E-07f)
            };
            // 940. - X: 90, Y: 585, Z: 360
            yield return new object[]
            {
                "X: 90, Y: 585, Z: 360",
                new float3(1.570796f, 10.21018f, 6.283185f),
                new Quaternion(-0.2705981f, 0.6532814f, -0.6532814f, -0.2705981f),
                new float4x4(-0.7071064f, -0.7071068f, 0f, 0f, 0f, 1.490116E-07f, -0.9999999f, 0f, 0.7071068f, -0.7071065f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 3.926991f, 0f)
            };
            // 941. - X: 180, Y: 585, Z: 360
            yield return new object[]
            {
                "X: 180, Y: 585, Z: 360",
                new float3(3.141593f, 10.21018f, 6.283185f),
                new Quaternion(-0.3826836f, -6.92879E-09f, -0.9238794f, 9.749574E-08f),
                new float4x4(-0.7071065f, 1.854517E-07f, 0.707107f, 0f, -1.748456E-07f, -0.9999999f, 8.742278E-08f, 0f, 0.707107f, -6.18173E-08f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 0.7853985f, 3.141593f)
            };
            // 942. - X: 270, Y: 585, Z: 360
            yield return new object[]
            {
                "X: 270, Y: 585, Z: 360",
                new float3(4.712389f, 10.21018f, 6.283185f),
                new Quaternion(-0.2705982f, -0.6532814f, -0.6532814f, 0.2705982f),
                new float4x4(-0.7071064f, 0.7071072f, 0f, 0f, 0f, 1.490116E-08f, 1f, 0f, 0.7071072f, 0.7071064f, 1.490116E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.926991f, 0f)
            };
            // 943. - X: 360, Y: 585, Z: 360
            yield return new object[]
            {
                "X: 360, Y: 585, Z: 360",
                new float3(6.283185f, 10.21018f, 6.283185f),
                new Quaternion(-4.731284E-08f, -0.9238794f, 1.142234E-07f, 0.3826836f),
                new float4x4(-0.7071065f, -9.237056E-14f, -0.707107f, 0f, 1.748456E-07f, 1f, -1.748455E-07f, 0f, 0.707107f, -2.472689E-07f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748455E-07f, 3.926991f, 1.748456E-07f)
            };
            // 944. - X: 585, Y: 585, Z: 360
            yield return new object[]
            {
                "X: 585, Y: 585, Z: 360",
                new float3(10.21018f, 10.21018f, 6.283185f),
                new Quaternion(0.3535535f, 0.3535535f, 0.8535532f, -0.1464468f),
                new float4x4(-0.7071064f, 0.5000005f, 0.5f, 0f, -1.490116E-07f, -0.7071065f, 0.707107f, 0f, 0.7071071f, 0.4999999f, 0.4999997f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 0.7853984f, 3.141593f)
            };
            // 945. - X: -90, Y: 585, Z: 360
            yield return new object[]
            {
                "X: -90, Y: 585, Z: 360",
                new float3(-1.570796f, 10.21018f, 6.283185f),
                new Quaternion(0.2705982f, 0.6532814f, 0.6532814f, -0.2705982f),
                new float4x4(-0.7071064f, 0.7071072f, 0f, 0f, 0f, 1.490116E-08f, 1f, 0f, 0.7071072f, 0.7071064f, 1.490116E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.926991f, 0f)
            };
            // 946. - X: -540, Y: 585, Z: 360
            yield return new object[]
            {
                "X: -540, Y: 585, Z: 360",
                new float3(-9.424778f, 10.21018f, 6.283185f),
                new Quaternion(-0.3826836f, 4.447242E-08f, -0.9238794f, 7.620465E-08f),
                new float4x4(-0.7071065f, 1.067701E-07f, 0.707107f, 0f, -1.748455E-07f, -0.9999999f, -2.384976E-08f, 0f, 0.707107f, -1.404988E-07f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 0.7853985f, 3.141593f)
            };
            // 947. - X: 0, Y: -90, Z: 360
            yield return new object[]
            {
                "X: 0, Y: -90, Z: 360",
                new float3(0f, -1.570796f, 6.283185f),
                new Quaternion(6.181724E-08f, 0.7071068f, -6.181724E-08f, -0.7071068f),
                new float4x4(5.960464E-08f, 0f, -0.9999999f, 0f, 1.748456E-07f, 1f, 0f, 0f, 0.9999999f, -1.748456E-07f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 1.748456E-07f)
            };
            // 948. - X: 0.1, Y: -90, Z: 360
            yield return new object[]
            {
                "X: 0.1, Y: -90, Z: 360",
                new float3(0.001745329f, -1.570796f, 6.283185f),
                new Quaternion(-0.0006170052f, 0.7071065f, -0.0006171288f, -0.7071065f),
                new float4x4(-4.644016E-08f, -0.001745328f, -0.9999985f, 0f, 1.747976E-07f, 0.9999985f, -0.001745328f, 0f, 1f, -1.747976E-07f, -4.613503E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 4.712389f, 1.747979E-07f)
            };
            // 949. - X: 1, Y: -90, Z: 360
            yield return new object[]
            {
                "X: 1, Y: -90, Z: 360",
                new float3(0.01745329f, -1.570796f, 6.283185f),
                new Quaternion(-0.00617053f, 0.7070798f, -0.006170654f, -0.7070798f),
                new float4x4(8.039206E-08f, -0.01745241f, -0.9998476f, 0f, 1.750886E-07f, 0.9998477f, -0.01745241f, 0f, 0.9999999f, -1.750886E-07f, 8.344796E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 4.712389f, 1.751153E-07f)
            };
            // 950. - X: 45, Y: -90, Z: 360
            yield return new object[]
            {
                "X: 45, Y: -90, Z: 360",
                new float3(0.7853982f, -1.570796f, 6.283185f),
                new Quaternion(-0.270598f, 0.6532815f, -0.2705981f, -0.6532815f),
                new float4x4(2.980232E-08f, -0.7071067f, -0.7071067f, 0f, 1.490116E-07f, 0.7071068f, -0.7071067f, 0f, 0.9999999f, -1.490116E-07f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853984f, 4.712389f, 2.107342E-07f)
            };
            // 951. - X: 90, Y: -90, Z: 360
            yield return new object[]
            {
                "X: 90, Y: -90, Z: 360",
                new float3(1.570796f, -1.570796f, 6.283185f),
                new Quaternion(-0.4999999f, 0.5f, -0.5f, -0.4999999f),
                new float4x4(0f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, 0.9999999f, -1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 4.712389f, 0f)
            };
            // 952. - X: 180, Y: -90, Z: 360
            yield return new object[]
            {
                "X: 180, Y: -90, Z: 360",
                new float3(3.141593f, -1.570796f, 6.283185f),
                new Quaternion(-0.7071068f, 3.090862E-08f, -0.7071068f, 9.272586E-08f),
                new float4x4(5.960464E-08f, 8.742277E-08f, 0.9999999f, 0f, -1.748456E-07f, -0.9999999f, 8.742277E-08f, 0f, 0.9999999f, -1.748456E-07f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742279E-08f, 1.570796f, 3.141593f)
            };
            // 953. - X: 270, Y: -90, Z: 360
            yield return new object[]
            {
                "X: 270, Y: -90, Z: 360",
                new float3(4.712389f, -1.570796f, 6.283185f),
                new Quaternion(-0.5f, -0.4999999f, -0.4999999f, 0.5f),
                new float4x4(2.384186E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, 0.9999999f, -1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 954. - X: 360, Y: -90, Z: 360
            yield return new object[]
            {
                "X: 360, Y: -90, Z: 360",
                new float3(6.283185f, -1.570796f, 6.283185f),
                new Quaternion(0f, -0.7071068f, 1.236345E-07f, 0.7071068f),
                new float4x4(5.960461E-08f, -1.748456E-07f, -0.9999999f, 0f, 1.748456E-07f, 1f, -1.748456E-07f, 0f, 0.9999999f, -1.748456E-07f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 4.712389f, 1.748456E-07f)
            };
            // 955. - X: 585, Y: -90, Z: 360
            yield return new object[]
            {
                "X: 585, Y: -90, Z: 360",
                new float3(10.21018f, -1.570796f, 6.283185f),
                new Quaternion(0.6532814f, 0.2705981f, 0.6532814f, -0.2705982f),
                new float4x4(1.490116E-07f, 0.707107f, 0.7071064f, 0f, -1.788139E-07f, -0.7071064f, 0.707107f, 0f, 0.9999999f, -1.788139E-07f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.570796f, 3.141593f)
            };
            // 956. - X: -90, Y: -90, Z: 360
            yield return new object[]
            {
                "X: -90, Y: -90, Z: 360",
                new float3(-1.570796f, -1.570796f, 6.283185f),
                new Quaternion(0.5f, 0.4999999f, 0.4999999f, -0.5f),
                new float4x4(2.384186E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, 0.9999999f, -1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 957. - X: -540, Y: -90, Z: 360
            yield return new object[]
            {
                "X: -540, Y: -90, Z: 360",
                new float3(-9.424778f, -1.570796f, 6.283185f),
                new Quaternion(-0.7071068f, 7.02494E-08f, -0.7071068f, 5.338508E-08f),
                new float4x4(5.960463E-08f, -2.384976E-08f, 0.9999999f, 0f, -1.748456E-07f, -0.9999999f, -2.384976E-08f, 0f, 0.9999999f, -1.748456E-07f, 5.960463E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 1.570796f, 3.141593f)
            };
            // 958. - X: 0, Y: -540, Z: 360
            yield return new object[]
            {
                "X: 0, Y: -540, Z: 360",
                new float3(0f, -9.424778f, 6.283185f),
                new Quaternion(-8.742278E-08f, -1f, -1.042506E-15f, -1.192488E-08f),
                new float4x4(-1f, 1.748456E-07f, 2.384976E-08f, 0f, 1.748456E-07f, 1f, 0f, 0f, -2.384976E-08f, 4.170025E-15f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.141593f, 1.748456E-07f)
            };
            // 959. - X: 0.1, Y: -540, Z: 360
            yield return new object[]
            {
                "X: 0.1, Y: -540, Z: 360",
                new float3(0.001745329f, -9.424778f, 6.283185f),
                new Quaternion(-8.743316E-08f, -0.9999996f, 0.0008726645f, -1.200117E-08f),
                new float4x4(-1f, 1.748872E-07f, 2.384973E-08f, 0f, 1.748453E-07f, 0.9999985f, -0.001745328f, 0f, -2.415493E-08f, -0.001745328f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.141593f, 1.748456E-07f)
            };
            // 960. - X: 1, Y: -540, Z: 360
            yield return new object[]
            {
                "X: 1, Y: -540, Z: 360",
                new float3(0.01745329f, -9.424778f, 6.283185f),
                new Quaternion(-8.752351E-08f, -0.9999619f, 0.008726535f, -1.268732E-08f),
                new float4x4(-0.9999999f, 1.752618E-07f, 2.384613E-08f, 0f, 1.748189E-07f, 0.9998477f, -0.01745241f, 0f, -2.690124E-08f, -0.01745241f, -0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.141593f, 1.748456E-07f)
            };
            // 961. - X: 45, Y: -540, Z: 360
            yield return new object[]
            {
                "X: 45, Y: -540, Z: 360",
                new float3(0.7853982f, -9.424778f, 6.283185f),
                new Quaternion(-8.533156E-08f, -0.9238795f, 0.3826835f, -4.44724E-08f),
                new float4x4(-1f, 1.917099E-07f, 1.686433E-08f, 0f, 1.236345E-07f, 0.7071067f, -0.7071068f, 0f, -1.474842E-07f, -0.7071068f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 3.141593f, 1.748455E-07f)
            };
            // 962. - X: 90, Y: -540, Z: 360
            yield return new object[]
            {
                "X: 90, Y: -540, Z: 360",
                new float3(1.570796f, -9.424778f, 6.283185f),
                new Quaternion(-7.02494E-08f, -0.7071068f, 0.7071068f, -7.02494E-08f),
                new float4x4(-0.9999999f, 1.986953E-07f, 0f, 0f, 0f, 5.960463E-08f, -0.9999999f, 0f, -1.986953E-07f, -0.9999999f, 5.960463E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 3.141593f, 0f)
            };
            // 963. - X: 180, Y: -540, Z: 360
            yield return new object[]
            {
                "X: 180, Y: -540, Z: 360",
                new float3(3.141593f, -9.424778f, 6.283185f),
                new Quaternion(-1.192488E-08f, 4.371139E-08f, 1f, -8.742278E-08f),
                new float4x4(-1f, 1.748456E-07f, -2.384976E-08f, 0f, -1.748456E-07f, -1f, 8.742278E-08f, 0f, -2.384975E-08f, 8.742278E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, -2.384976E-08f, 3.141593f)
            };
            // 964. - X: 270, Y: -540, Z: 360
            yield return new object[]
            {
                "X: 270, Y: -540, Z: 360",
                new float3(4.712389f, -9.424778f, 6.283185f),
                new Quaternion(5.338508E-08f, 0.7071068f, 0.7071068f, -5.338508E-08f),
                new float4x4(-0.9999999f, 1.509958E-07f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, 1.509958E-07f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.141593f, 0f)
            };
            // 965. - X: 360, Y: -540, Z: 360
            yield return new object[]
            {
                "X: 360, Y: -540, Z: 360",
                new float3(6.283185f, -9.424778f, 6.283185f),
                new Quaternion(8.742278E-08f, 1f, -8.742278E-08f, 1.192489E-08f),
                new float4x4(-1f, 1.748456E-07f, 2.384976E-08f, 0f, 1.748456E-07f, 1f, -1.748456E-07f, 0f, -2.384979E-08f, -1.748456E-07f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 3.141593f, 1.748456E-07f)
            };
            // 966. - X: 585, Y: -540, Z: 360
            yield return new object[]
            {
                "X: 585, Y: -540, Z: 360",
                new float3(10.21018f, -9.424778f, 6.283185f),
                new Quaternion(-2.243811E-08f, -0.3826836f, -0.9238794f, 7.620465E-08f),
                new float4x4(-0.9999999f, 1.579812E-07f, -1.686433E-08f, 0f, -1.236344E-07f, -0.7071065f, 0.707107f, 0f, 9.978476E-08f, 0.707107f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, -2.384977E-08f, 3.141593f)
            };
            // 967. - X: -90, Y: -540, Z: 360
            yield return new object[]
            {
                "X: -90, Y: -540, Z: 360",
                new float3(-1.570796f, -9.424778f, 6.283185f),
                new Quaternion(-5.338508E-08f, -0.7071068f, -0.7071068f, 5.338508E-08f),
                new float4x4(-0.9999999f, 1.509958E-07f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, 1.509958E-07f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.141593f, 0f)
            };
            // 968. - X: -540, Y: -540, Z: 360
            yield return new object[]
            {
                "X: -540, Y: -540, Z: 360",
                new float3(-9.424778f, -9.424778f, 6.283185f),
                new Quaternion(-1.192488E-08f, -1.192488E-08f, 1f, -8.742278E-08f),
                new float4x4(-1f, 1.748456E-07f, -2.384976E-08f, 0f, -1.748456E-07f, -1f, -2.384976E-08f, 0f, -2.384976E-08f, -2.384976E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, -2.384976E-08f, 3.141593f)
            };
            // 969. - X: 0, Y: 0, Z: 585
            yield return new object[]
            {
                "X: 0, Y: 0, Z: 585",
                new float3(0f, 0f, 10.21018f),
                new Quaternion(0f, 0f, -0.9238794f, 0.3826836f),
                new float4x4(-0.7071065f, 0.707107f, 0f, 0f, -0.707107f, -0.7071065f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0f, 3.926991f)
            };
            // 970. - X: 0.1, Y: 0, Z: 585
            yield return new object[]
            {
                "X: 0.1, Y: 0, Z: 585",
                new float3(0.001745329f, 0f, 10.21018f),
                new Quaternion(0.0003339544f, 0.0008062368f, -0.9238791f, 0.3826835f),
                new float4x4(-0.7071065f, 0.707107f, 0f, 0f, -0.7071059f, -0.7071054f, -0.001745328f, 0f, -0.001234134f, -0.001234133f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0f, 3.926991f)
            };
            // 971. - X: 1, Y: 0, Z: 585
            yield return new object[]
            {
                "X: 1, Y: 0, Z: 585",
                new float3(0.01745329f, 0f, 10.21018f),
                new Quaternion(0.003339502f, 0.008062267f, -0.9238443f, 0.382669f),
                new float4x4(-0.7071065f, 0.707107f, -4.656613E-10f, 0f, -0.7069994f, -0.7069988f, -0.01745241f, 0f, -0.01234072f, -0.01234071f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, -4.657322E-10f, 3.926991f)
            };
            // 972. - X: 45, Y: 0, Z: 585
            yield return new object[]
            {
                "X: 45, Y: 0, Z: 585",
                new float3(0.7853982f, 0f, 10.21018f),
                new Quaternion(0.1464467f, 0.3535534f, -0.8535533f, 0.3535535f),
                new float4x4(-0.7071065f, 0.7071071f, -2.980232E-08f, 0f, -0.5000001f, -0.4999997f, -0.7071068f, 0f, -0.5000002f, -0.4999998f, 0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, -4.214685E-08f, 3.926991f)
            };
            // 973. - X: 90, Y: 0, Z: 585
            yield return new object[]
            {
                "X: 90, Y: 0, Z: 585",
                new float3(1.570796f, 0f, 10.21018f),
                new Quaternion(0.2705982f, 0.6532814f, -0.6532814f, 0.2705982f),
                new float4x4(-0.7071064f, 0.707107f, 0f, 0f, 0f, 7.450581E-08f, -0.9999999f, 0f, -0.707107f, -0.7071064f, 7.450581E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 2.356194f, 0f)
            };
            // 974. - X: 180, Y: 0, Z: 585
            yield return new object[]
            {
                "X: 180, Y: 0, Z: 585",
                new float3(3.141593f, 0f, 10.21018f),
                new Quaternion(0.3826836f, 0.9238794f, 4.038405E-08f, -1.672763E-08f),
                new float4x4(-0.7071065f, 0.707107f, 0f, 0f, 0.707107f, 0.7071065f, 8.742277E-08f, 0f, 6.181726E-08f, 6.181721E-08f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742277E-08f, 3.141593f, 0.7853985f)
            };
            // 975. - X: 270, Y: 0, Z: 585
            yield return new object[]
            {
                "X: 270, Y: 0, Z: 585",
                new float3(4.712389f, 0f, 10.21018f),
                new Quaternion(0.2705982f, 0.6532814f, 0.6532814f, -0.2705982f),
                new float4x4(-0.7071064f, 0.707107f, 0f, 0f, 0f, 7.450581E-08f, 0.9999999f, 0f, 0.707107f, 0.7071064f, 7.450581E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.926991f, 0f)
            };
            // 976. - X: 360, Y: 0, Z: 585
            yield return new object[]
            {
                "X: 360, Y: 0, Z: 585",
                new float3(6.283185f, 0f, 10.21018f),
                new Quaternion(-3.345526E-08f, -8.07681E-08f, 0.9238794f, -0.3826836f),
                new float4x4(-0.7071065f, 0.707107f, 0f, 0f, -0.707107f, -0.7071065f, -1.748455E-07f, 0f, -1.236345E-07f, -1.236344E-07f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748455E-07f, 0f, 3.926991f)
            };
            // 977. - X: 585, Y: 0, Z: 585
            yield return new object[]
            {
                "X: 585, Y: 0, Z: 585",
                new float3(10.21018f, 0f, 10.21018f),
                new Quaternion(-0.3535535f, -0.8535532f, -0.3535535f, 0.1464467f),
                new float4x4(-0.7071064f, 0.707107f, -2.980232E-08f, 0f, 0.5f, 0.4999997f, 0.707107f, 0f, 0.5000004f, 0.5f, -0.7071064f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.141593f, 0.7853986f)
            };
            // 978. - X: -90, Y: 0, Z: 585
            yield return new object[]
            {
                "X: -90, Y: 0, Z: 585",
                new float3(-1.570796f, 0f, 10.21018f),
                new Quaternion(-0.2705982f, -0.6532814f, -0.6532814f, 0.2705982f),
                new float4x4(-0.7071064f, 0.707107f, 0f, 0f, 0f, 7.450581E-08f, 0.9999999f, 0f, 0.707107f, 0.7071064f, 7.450581E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.926991f, 0f)
            };
            // 979. - X: -540, Y: 0, Z: 585
            yield return new object[]
            {
                "X: -540, Y: 0, Z: 585",
                new float3(-9.424778f, 0f, 10.21018f),
                new Quaternion(0.3826836f, 0.9238794f, -1.101715E-08f, 4.563456E-09f),
                new float4x4(-0.7071065f, 0.707107f, 0f, 0f, 0.707107f, 0.7071065f, -2.384976E-08f, 0f, -1.686433E-08f, -1.686432E-08f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.141593f, 0.7853985f)
            };
            // 980. - X: 0, Y: 0.1, Z: 585
            yield return new object[]
            {
                "X: 0, Y: 0.1, Z: 585",
                new float3(0f, 0.001745329f, 10.21018f),
                new Quaternion(-0.0008062368f, 0.0003339544f, -0.9238791f, 0.3826835f),
                new float4x4(-0.7071054f, 0.7071059f, 0.001745328f, 0f, -0.707107f, -0.7071065f, 0f, 0f, 0.001234133f, -0.001234134f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0.001745329f, 3.926991f)
            };
            // 981. - X: 0.1, Y: 0.1, Z: 585
            yield return new object[]
            {
                "X: 0.1, Y: 0.1, Z: 585",
                new float3(0.001745329f, 0.001745329f, 10.21018f),
                new Quaternion(-0.0004722822f, 0.001140191f, -0.9238791f, 0.3826826f),
                new float4x4(-0.7071078f, 0.7071038f, 0.001745326f, 0f, -0.707106f, -0.7071056f, -0.001745328f, 0f, 9.895302E-10f, -0.002468265f, 0.999997f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0.001745329f, 3.926991f)
            };
            // 982. - X: 1, Y: 0.1, Z: 585
            yield return new object[]
            {
                "X: 1, Y: 0.1, Z: 585",
                new float3(0.01745329f, 0.001745329f, 10.21018f),
                new Quaternion(0.002533295f, 0.008396205f, -0.9238468f, 0.3826618f),
                new float4x4(-0.7071269f, 0.7070844f, 0.001745062f, 0f, -0.7069993f, -0.7069988f, -0.01745241f, 0f, -0.01110657f, -0.01357483f, 0.9998462f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.0174533f, 0.001745329f, 3.926991f)
            };
            // 983. - X: 45, Y: 0.1, Z: 585
            yield return new object[]
            {
                "X: 45, Y: 0.1, Z: 585",
                new float3(0.7853982f, 0.001745329f, 10.21018f),
                new Quaternion(0.1457018f, 0.3538618f, -0.8536807f, 0.3532449f),
                new float4x4(-0.7079779f, 0.7062333f, 0.001234144f, 0f, -0.5000001f, -0.4999996f, -0.7071068f, 0f, -0.4987653f, -0.5012332f, 0.7071057f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 0.001745344f, 3.926991f)
            };
            // 984. - X: 90, Y: 0.1, Z: 585
            yield return new object[]
            {
                "X: 90, Y: 0.1, Z: 585",
                new float3(1.570796f, 0.001745329f, 10.21018f),
                new Quaternion(0.270028f, 0.6535174f, -0.6535174f, 0.270028f),
                new float4x4(-0.7083398f, 0.7058719f, 0f, 0f, 0f, -1.341105E-07f, -1f, 0f, -0.7058719f, -0.7083397f, -1.341105E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 2.357939f, 0f)
            };
            // 985. - X: 180, Y: 0.1, Z: 585
            yield return new object[]
            {
                "X: 180, Y: 0.1, Z: 585",
                new float3(3.141593f, 0.001745329f, 10.21018f),
                new Quaternion(0.3826835f, 0.9238791f, -0.000333914f, -0.0008062535f),
                new float4x4(-0.7071054f, 0.7071059f, -0.001745328f, 0f, 0.707107f, 0.7071065f, 8.742791E-08f, 0f, 0.001234195f, -0.001234072f, -0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742791E-08f, 3.143338f, 0.7853985f)
            };
            // 986. - X: 270, Y: 0.1, Z: 585
            yield return new object[]
            {
                "X: 270, Y: 0.1, Z: 585",
                new float3(4.712389f, 0.001745329f, 10.21018f),
                new Quaternion(0.2711682f, 0.6530451f, 0.6530451f, -0.2711682f),
                new float4x4(-0.7058713f, 0.7083402f, 0f, 0f, 0f, -2.980232E-08f, 1f, 0f, 0.7083402f, 0.7058713f, -2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.928736f, 0f)
            };
            // 987. - X: 360, Y: 0.1, Z: 585
            yield return new object[]
            {
                "X: 360, Y: 0.1, Z: 585",
                new float3(6.283185f, 0.001745329f, 10.21018f),
                new Quaternion(0.0008062033f, -0.0003340352f, 0.9238791f, -0.3826835f),
                new float4x4(-0.7071054f, 0.7071059f, 0.001745328f, 0f, -0.707107f, -0.7071065f, -1.748558E-07f, 0f, 0.001234009f, -0.001234258f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748558E-07f, 0.001745329f, 3.926991f)
            };
            // 988. - X: 585, Y: 0.1, Z: 585
            yield return new object[]
            {
                "X: 585, Y: 0.1, Z: 585",
                new float3(10.21018f, 0.001745329f, 10.21018f),
                new Quaternion(-0.3538619f, -0.8534251f, -0.3532448f, 0.1471916f),
                new float4x4(-0.7062325f, 0.7079786f, -0.001234129f, 0f, 0.5f, 0.4999996f, 0.707107f, 0f, 0.5012338f, 0.4987651f, -0.7071053f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.143338f, 0.7853985f)
            };
            // 989. - X: -90, Y: 0.1, Z: 585
            yield return new object[]
            {
                "X: -90, Y: 0.1, Z: 585",
                new float3(-1.570796f, 0.001745329f, 10.21018f),
                new Quaternion(-0.2711682f, -0.6530451f, -0.6530451f, 0.2711682f),
                new float4x4(-0.7058713f, 0.7083402f, 0f, 0f, 0f, -2.980232E-08f, 1f, 0f, 0.7083402f, 0.7058713f, -2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.928736f, 0f)
            };
            // 990. - X: -540, Y: 0.1, Z: 585
            yield return new object[]
            {
                "X: -540, Y: 0.1, Z: 585",
                new float3(-9.424778f, 0.001745329f, 10.21018f),
                new Quaternion(0.3826835f, 0.9238791f, -0.0003339654f, -0.0008062323f),
                new float4x4(-0.7071054f, 0.7071059f, -0.001745328f, 0f, 0.707107f, 0.7071065f, -2.386514E-08f, 0f, 0.001234116f, -0.001234151f, -0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.386514E-08f, 3.143338f, 0.7853985f)
            };
            // 991. - X: 0, Y: 1, Z: 585
            yield return new object[]
            {
                "X: 0, Y: 1, Z: 585",
                new float3(0f, 0.01745329f, 10.21018f),
                new Quaternion(-0.008062267f, 0.003339502f, -0.9238443f, 0.382669f),
                new float4x4(-0.7069988f, 0.7069994f, 0.01745241f, 0f, -0.707107f, -0.7071065f, -4.656613E-10f, 0f, 0.01234071f, -0.01234072f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.656613E-10f, 0.01745329f, 3.926991f)
            };
            // 992. - X: 0.1, Y: 1, Z: 585
            yield return new object[]
            {
                "X: 0.1, Y: 1, Z: 585",
                new float3(0.001745329f, 0.01745329f, 10.21018f),
                new Quaternion(-0.007728322f, 0.004145707f, -0.9238468f, 0.3826618f),
                new float4x4(-0.7070203f, 0.7069778f, 0.01745238f, 0f, -0.7071059f, -0.7071054f, -0.001745329f, 0f, 0.01110676f, -0.01357467f, 0.9998462f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.00174533f, 0.01745329f, 3.926991f)
            };
            // 993. - X: 1, Y: 1, Z: 585
            yield return new object[]
            {
                "X: 1, Y: 1, Z: 585",
                new float3(0.01745329f, 0.01745329f, 10.21018f),
                new Quaternion(-0.004722585f, 0.01140133f, -0.9238382f, 0.3825841f),
                new float4x4(-0.707214f, 0.7067839f, 0.01744975f, 0f, -0.7069993f, -0.7069986f, -0.0174524f, 0f, 1.871027E-06f, -0.02467955f, 0.9996954f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0.01745329f, 3.926991f)
            };
            // 994. - X: 45, Y: 1, Z: 585
            yield return new object[]
            {
                "X: 45, Y: 1, Z: 585",
                new float3(0.7853982f, 0.01745329f, 10.21018f),
                new Quaternion(0.1389925f, 0.3566252f, -0.8547988f, 0.3504548f),
                new float4x4(-0.7157251f, 0.6982732f, 0.01234074f, 0f, -0.5000002f, -0.4999999f, -0.7071068f, 0f, -0.4875833f, -0.5122644f, 0.7069991f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 0.01745333f, 3.926991f)
            };
            // 995. - X: 90, Y: 1, Z: 585
            yield return new object[]
            {
                "X: 90, Y: 1, Z: 585",
                new float3(1.570796f, 0.01745329f, 10.21018f),
                new Quaternion(0.264887f, 0.6556179f, -0.6556179f, 0.264887f),
                new float4x4(-0.7193393f, 0.6946586f, 0f, 0f, 0f, 1.490116E-07f, -0.9999999f, 0f, -0.6946586f, -0.7193394f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 2.373647f, 0f)
            };
            // 996. - X: 180, Y: 1, Z: 585
            yield return new object[]
            {
                "X: 180, Y: 1, Z: 585",
                new float3(3.141593f, 0.01745329f, 10.21018f),
                new Quaternion(0.382669f, 0.9238443f, -0.003339462f, -0.008062284f),
                new float4x4(-0.7069988f, 0.7069994f, -0.01745241f, 0f, 0.707107f, 0.7071065f, 8.707866E-08f, 0f, 0.01234077f, -0.01234066f, -0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.707866E-08f, 3.159046f, 0.7853985f)
            };
            // 997. - X: 270, Y: 1, Z: 585
            yield return new object[]
            {
                "X: 270, Y: 1, Z: 585",
                new float3(4.712389f, 0.01745329f, 10.21018f),
                new Quaternion(0.2762887f, 0.6508952f, 0.6508952f, -0.2762887f),
                new float4x4(-0.6946582f, 0.71934f, 0f, 0f, 0f, -2.980232E-08f, 1f, 0f, 0.71934f, 0.6946582f, -2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.944444f, 0f)
            };
            // 998. - X: 360, Y: 1, Z: 585
            yield return new object[]
            {
                "X: 360, Y: 1, Z: 585",
                new float3(6.283185f, 0.01745329f, 10.21018f),
                new Quaternion(0.008062233f, -0.003339583f, 0.9238443f, -0.382669f),
                new float4x4(-0.7069988f, 0.7069994f, 0.01745241f, 0f, -0.707107f, -0.7071065f, -1.750886E-07f, 0f, 0.01234059f, -0.01234084f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.750886E-07f, 0.01745329f, 3.926991f)
            };
            // 999. - X: 585, Y: 1, Z: 585
            yield return new object[]
            {
                "X: 585, Y: 1, Z: 585",
                new float3(10.21018f, 0.01745329f, 10.21018f),
                new Quaternion(-0.3566253f, -0.8522428f, -0.3504548f, 0.1538897f),
                new float4x4(-0.6982726f, 0.7157255f, -0.01234069f, 0f, 0.5f, 0.4999996f, 0.7071071f, 0f, 0.512265f, 0.4875832f, -0.7069988f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.159046f, 0.7853985f)
            };
            // 1000. - X: -90, Y: 1, Z: 585
            yield return new object[]
            {
                "X: -90, Y: 1, Z: 585",
                new float3(-1.570796f, 0.01745329f, 10.21018f),
                new Quaternion(-0.2762887f, -0.6508952f, -0.6508952f, 0.2762887f),
                new float4x4(-0.6946582f, 0.71934f, 0f, 0f, 0f, -2.980232E-08f, 1f, 0f, 0.71934f, 0.6946582f, -2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.944444f, 0f)
            };
            // 1001. - X: -540, Y: 1, Z: 585
            yield return new object[]
            {
                "X: -540, Y: 1, Z: 585",
                new float3(-9.424778f, 0.01745329f, 10.21018f),
                new Quaternion(0.382669f, 0.9238443f, -0.003339513f, -0.008062262f),
                new float4x4(-0.7069988f, 0.7069994f, -0.01745241f, 0f, 0.707107f, 0.7071065f, -2.374873E-08f, 0f, 0.01234069f, -0.01234074f, -0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.374873E-08f, 3.159046f, 0.7853985f)
            };
            // 1002. - X: 0, Y: 45, Z: 585
            yield return new object[]
            {
                "X: 0, Y: 45, Z: 585",
                new float3(0f, 0.7853982f, 10.21018f),
                new Quaternion(-0.3535534f, 0.1464467f, -0.8535533f, 0.3535535f),
                new float4x4(-0.4999997f, 0.5000001f, 0.7071068f, 0f, -0.7071071f, -0.7071065f, -2.980232E-08f, 0f, 0.4999998f, -0.5000002f, 0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.980232E-08f, 0.7853982f, 3.926991f)
            };
            // 1003. - X: 0.1, Y: 45, Z: 585
            yield return new object[]
            {
                "X: 0.1, Y: 45, Z: 585",
                new float3(0.001745329f, 0.7853982f, 10.21018f),
                new Quaternion(-0.3532447f, 0.1471915f, -0.8536807f, 0.3532449f),
                new float4x4(-0.5008723f, 0.4991274f, 0.7071057f, 0f, -0.7071059f, -0.7071052f, -0.001745343f, 0f, 0.4991271f, -0.5008729f, 0.7071057f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0.7853982f, 3.926991f)
            };
            // 1004. - X: 1, Y: 45, Z: 585
            yield return new object[]
            {
                "X: 1, Y: 45, Z: 585",
                new float3(0.01745329f, 0.7853982f, 10.21018f),
                new Quaternion(-0.3504546f, 0.1538897f, -0.8547988f, 0.3504548f),
                new float4x4(-0.5087261f, 0.491274f, 0.7069992f, 0f, -0.7069994f, -0.7069989f, -0.0174524f, 0f, 0.4912737f, -0.5087264f, 0.7069991f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0.7853982f, 3.926991f)
            };
            // 1005. - X: 45, Y: 45, Z: 585
            yield return new object[]
            {
                "X: 45, Y: 45, Z: 585",
                new float3(0.7853982f, 0.7853982f, 10.21018f),
                new Quaternion(-0.1913416f, 0.4619398f, -0.8446231f, 0.1913418f),
                new float4x4(-0.8535533f, 0.1464469f, 0.5f, 0f, -0.5000001f, -0.4999998f, -0.7071068f, 0f, 0.1464463f, -0.8535534f, 0.5f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 0.7853982f, 3.926991f)
            };
            // 1006. - X: 90, Y: 45, Z: 585
            yield return new object[]
            {
                "X: 90, Y: 45, Z: 585",
                new float3(1.570796f, 0.7853982f, 10.21018f),
                new Quaternion(1.043081E-07f, 0.7071067f, -0.7071067f, 1.043081E-07f),
                new float4x4(-0.9999996f, 2.950279E-07f, 0f, 0f, 0f, 1.788139E-07f, -0.9999998f, 0f, -2.950279E-07f, -0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570451f, 3.141592f, 0f)
            };
            // 1007. - X: 180, Y: 45, Z: 585
            yield return new object[]
            {
                "X: 180, Y: 45, Z: 585",
                new float3(3.141593f, 0.7853982f, 10.21018f),
                new Quaternion(0.3535536f, 0.8535533f, -0.1464466f, -0.3535534f),
                new float4x4(-0.4999997f, 0.5000002f, -0.7071068f, 0f, 0.707107f, 0.7071065f, 1.192093E-07f, 0f, 0.4999999f, -0.5000002f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(-1.192093E-07f, 3.926991f, 0.7853985f)
            };
            // 1008. - X: 270, Y: 45, Z: 585
            yield return new object[]
            {
                "X: 270, Y: 45, Z: 585",
                new float3(4.712389f, 0.7853982f, 10.21018f),
                new Quaternion(0.5000001f, 0.4999999f, 0.4999999f, -0.5000001f),
                new float4x4(5.960464E-07f, 0.9999998f, 0f, 0f, 0f, 1.788139E-07f, 0.9999998f, 0f, 0.9999998f, -4.172325E-07f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 1009. - X: 360, Y: 45, Z: 585
            yield return new object[]
            {
                "X: 360, Y: 45, Z: 585",
                new float3(6.283185f, 0.7853982f, 10.21018f),
                new Quaternion(0.3535534f, -0.1464468f, 0.8535533f, -0.3535535f),
                new float4x4(-0.4999998f, 0.5000001f, 0.7071068f, 0f, -0.707107f, -0.7071064f, -1.788139E-07f, 0f, 0.4999997f, -0.5000003f, 0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.788139E-07f, 0.7853982f, 3.926991f)
            };
            // 1010. - X: 585, Y: 45, Z: 585
            yield return new object[]
            {
                "X: 585, Y: 45, Z: 585",
                new float3(10.21018f, 0.7853982f, 10.21018f),
                new Quaternion(-0.4619399f, -0.7325376f, -0.1913418f, 0.4619398f),
                new float4x4(-0.1464461f, 0.8535535f, -0.4999999f, 0f, 0.5f, 0.4999996f, 0.7071069f, 0f, 0.8535534f, -0.1464469f, -0.4999998f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.926991f, 0.7853985f)
            };
            // 1011. - X: -90, Y: 45, Z: 585
            yield return new object[]
            {
                "X: -90, Y: 45, Z: 585",
                new float3(-1.570796f, 0.7853982f, 10.21018f),
                new Quaternion(-0.5000001f, -0.4999999f, -0.4999999f, 0.5000001f),
                new float4x4(5.960464E-07f, 0.9999998f, 0f, 0f, 0f, 1.788139E-07f, 0.9999998f, 0f, 0.9999998f, -4.172325E-07f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 1012. - X: -540, Y: 45, Z: 585
            yield return new object[]
            {
                "X: -540, Y: 45, Z: 585",
                new float3(-9.424778f, 0.7853982f, 10.21018f),
                new Quaternion(0.3535535f, 0.8535533f, -0.1464467f, -0.3535534f),
                new float4x4(-0.4999998f, 0.5000001f, -0.7071068f, 0f, 0.7071071f, 0.7071065f, -5.960464E-08f, 0f, 0.4999998f, -0.5000002f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.960464E-08f, 3.926991f, 0.7853986f)
            };
            // 1013. - X: 0, Y: 90, Z: 585
            yield return new object[]
            {
                "X: 0, Y: 90, Z: 585",
                new float3(0f, 1.570796f, 10.21018f),
                new Quaternion(-0.6532814f, 0.2705982f, -0.6532814f, 0.2705982f),
                new float4x4(7.450581E-08f, 0f, 0.9999999f, 0f, -0.707107f, -0.7071064f, 0f, 0f, 0.7071064f, -0.707107f, 7.450581E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.570796f, 3.926991f)
            };
            // 1014. - X: 0.1, Y: 90, Z: 585
            yield return new object[]
            {
                "X: 0.1, Y: 90, Z: 585",
                new float3(0.001745329f, 1.570796f, 10.21018f),
                new Quaternion(-0.6530451f, 0.2711682f, -0.6535174f, 0.270028f),
                new float4x4(-0.001234263f, -0.001234114f, 0.9999986f, 0f, -0.7071061f, -0.7071056f, -0.001745313f, 0f, 0.7071066f, -0.7071071f, -2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745314f, 1.570796f, 3.926991f)
            };
            // 1015. - X: 1, Y: 90, Z: 585
            yield return new object[]
            {
                "X: 1, Y: 90, Z: 585",
                new float3(0.01745329f, 1.570796f, 10.21018f),
                new Quaternion(-0.6508952f, 0.2762887f, -0.6556179f, 0.264887f),
                new float4x4(-0.01234058f, -0.01234072f, 0.9998477f, 0f, -0.7069993f, -0.7069987f, -0.01745236f, 0f, 0.7071065f, -0.707107f, -2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745328f, 1.570796f, 3.926991f)
            };
            // 1016. - X: 45, Y: 90, Z: 585
            yield return new object[]
            {
                "X: 45, Y: 90, Z: 585",
                new float3(0.7853982f, 1.570796f, 10.21018f),
                new Quaternion(-0.4999999f, 0.5000001f, -0.7071067f, 1.043081E-07f),
                new float4x4(-0.4999999f, -0.4999998f, 0.7071066f, 0f, -0.5000001f, -0.4999995f, -0.7071066f, 0f, 0.7071064f, -0.7071069f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 1.570796f, 3.926991f)
            };
            // 1017. - X: 90, Y: 90, Z: 585
            yield return new object[]
            {
                "X: 90, Y: 90, Z: 585",
                new float3(1.570796f, 1.570796f, 10.21018f),
                new Quaternion(-0.2705979f, 0.6532815f, -0.6532815f, -0.2705979f),
                new float4x4(-0.7071066f, -0.7071064f, 0f, 0f, 0f, 2.533197E-07f, -0.9999998f, 0f, 0.7071064f, -0.7071068f, 2.533197E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570451f, 3.926991f, 0f)
            };
            // 1018. - X: 180, Y: 90, Z: 585
            yield return new object[]
            {
                "X: 180, Y: 90, Z: 585",
                new float3(3.141593f, 1.570796f, 10.21018f),
                new Quaternion(0.2705982f, 0.6532814f, -0.2705981f, -0.6532814f),
                new float4x4(1.192093E-07f, 5.960464E-08f, -0.9999999f, 0f, 0.707107f, 0.7071065f, 5.960464E-08f, 0f, 0.7071064f, -0.707107f, 4.470348E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.940697E-08f, 4.712389f, 0.7853987f)
            };
            // 1019. - X: 270, Y: 90, Z: 585
            yield return new object[]
            {
                "X: 270, Y: 90, Z: 585",
                new float3(4.712389f, 1.570796f, 10.21018f),
                new Quaternion(0.6532815f, 0.2705979f, 0.2705979f, -0.6532815f),
                new float4x4(0.7071071f, 0.7071064f, 0f, 0f, 0f, 2.533197E-07f, 0.9999998f, 0f, 0.7071064f, -0.7071068f, 2.533197E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 5.497787f, 0f)
            };
            // 1020. - X: 360, Y: 90, Z: 585
            yield return new object[]
            {
                "X: 360, Y: 90, Z: 585",
                new float3(6.283185f, 1.570796f, 10.21018f),
                new Quaternion(0.6532814f, -0.2705982f, 0.6532814f, -0.2705981f),
                new float4x4(1.490116E-08f, -1.788139E-07f, 0.9999999f, 0f, -0.707107f, -0.7071064f, -1.788139E-07f, 0f, 0.7071064f, -0.707107f, 1.490116E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.490116E-07f, 1.570796f, 3.926991f)
            };
            // 1021. - X: 585, Y: 90, Z: 585
            yield return new object[]
            {
                "X: 585, Y: 90, Z: 585",
                new float3(10.21018f, 1.570796f, 10.21018f),
                new Quaternion(-0.5000002f, -0.4999998f, 0f, 0.7071067f),
                new float4x4(0.5000005f, 0.4999999f, -0.7071064f, 0f, 0.4999999f, 0.4999996f, 0.7071069f, 0f, 0.7071064f, -0.7071069f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 4.712389f, 0.7853987f)
            };
            // 1022. - X: -90, Y: 90, Z: 585
            yield return new object[]
            {
                "X: -90, Y: 90, Z: 585",
                new float3(-1.570796f, 1.570796f, 10.21018f),
                new Quaternion(-0.6532815f, -0.2705979f, -0.2705979f, 0.6532815f),
                new float4x4(0.7071071f, 0.7071064f, 0f, 0f, 0f, 2.533197E-07f, 0.9999998f, 0f, 0.7071064f, -0.7071068f, 2.533197E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 5.497787f, 0f)
            };
            // 1023. - X: -540, Y: 90, Z: 585
            yield return new object[]
            {
                "X: -540, Y: 90, Z: 585",
                new float3(-9.424778f, 1.570796f, 10.21018f),
                new Quaternion(0.2705982f, 0.6532814f, -0.2705982f, -0.6532814f),
                new float4x4(7.450581E-08f, 0f, -0.9999999f, 0f, 0.707107f, 0.7071065f, 0f, 0f, 0.7071064f, -0.707107f, 7.450581E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 0.7853986f)
            };
            // 1024. - X: 0, Y: 180, Z: 585
            yield return new object[]
            {
                "X: 0, Y: 180, Z: 585",
                new float3(0f, 3.141593f, 10.21018f),
                new Quaternion(-0.9238794f, 0.3826836f, 4.038405E-08f, -1.672763E-08f),
                new float4x4(0.7071065f, -0.707107f, -8.742277E-08f, 0f, -0.707107f, -0.7071065f, 0f, 0f, -6.181721E-08f, 6.181726E-08f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.141593f, 3.926991f)
            };
            // 1025. - X: 0.1, Y: 180, Z: 585
            yield return new object[]
            {
                "X: 0.1, Y: 180, Z: 585",
                new float3(0.001745329f, 3.141593f, 10.21018f),
                new Quaternion(-0.9238791f, 0.3826835f, -0.000333914f, -0.0008062535f),
                new float4x4(0.7071065f, -0.707107f, -8.742791E-08f, 0f, -0.7071059f, -0.7071054f, -0.001745328f, 0f, 0.001234072f, 0.001234195f, -0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.141593f, 3.926991f)
            };
            // 1026. - X: 1, Y: 180, Z: 585
            yield return new object[]
            {
                "X: 1, Y: 180, Z: 585",
                new float3(0.01745329f, 3.141593f, 10.21018f),
                new Quaternion(-0.9238443f, 0.382669f, -0.003339462f, -0.008062284f),
                new float4x4(0.7071065f, -0.707107f, -8.707866E-08f, 0f, -0.7069994f, -0.7069988f, -0.01745241f, 0f, 0.01234066f, 0.01234077f, -0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.141593f, 3.926991f)
            };
            // 1027. - X: 45, Y: 180, Z: 585
            yield return new object[]
            {
                "X: 45, Y: 180, Z: 585",
                new float3(0.7853982f, 3.141593f, 10.21018f),
                new Quaternion(-0.8535533f, 0.3535535f, -0.1464466f, -0.3535534f),
                new float4x4(0.7071066f, -0.7071069f, -5.960464E-08f, 0f, -0.5000001f, -0.4999997f, -0.7071068f, 0f, 0.5000001f, 0.4999999f, -0.7071066f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853984f, 3.141593f, 3.926991f)
            };
            // 1028. - X: 90, Y: 180, Z: 585
            yield return new object[]
            {
                "X: 90, Y: 180, Z: 585",
                new float3(1.570796f, 3.141593f, 10.21018f),
                new Quaternion(-0.6532814f, 0.2705981f, -0.2705981f, -0.6532814f),
                new float4x4(0.7071066f, -0.7071069f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, 0.7071069f, 0.7071065f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 5.497787f, 0f)
            };
            // 1029. - X: 180, Y: 180, Z: 585
            yield return new object[]
            {
                "X: 180, Y: 180, Z: 585",
                new float3(3.141593f, 3.141593f, 10.21018f),
                new Quaternion(2.365642E-08f, -5.711168E-08f, -0.3826836f, -0.9238794f),
                new float4x4(0.7071065f, -0.707107f, 8.742277E-08f, 0f, 0.707107f, 0.7071065f, 8.742278E-08f, 0f, -1.236345E-07f, 4.618528E-14f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 8.742277E-08f, 0.7853985f)
            };
            // 1030. - X: 270, Y: 180, Z: 585
            yield return new object[]
            {
                "X: 270, Y: 180, Z: 585",
                new float3(4.712389f, 3.141593f, 10.21018f),
                new Quaternion(0.6532814f, -0.2705982f, -0.2705982f, -0.6532814f),
                new float4x4(0.7071065f, -0.7071071f, 0f, 0f, 0f, 4.470348E-08f, 0.9999999f, 0f, -0.7071071f, -0.7071064f, 4.470348E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.7853988f, 0f)
            };
            // 1031. - X: 360, Y: 180, Z: 585
            yield return new object[]
            {
                "X: 360, Y: 180, Z: 585",
                new float3(6.283185f, 3.141593f, 10.21018f),
                new Quaternion(0.9238794f, -0.3826836f, -6.92879E-09f, 9.749574E-08f),
                new float4x4(0.7071065f, -0.707107f, -8.742278E-08f, 0f, -0.707107f, -0.7071065f, -1.748456E-07f, 0f, 6.18173E-08f, 1.854517E-07f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 3.141593f, 3.926991f)
            };
            // 1032. - X: 585, Y: 180, Z: 585
            yield return new object[]
            {
                "X: 585, Y: 180, Z: 585",
                new float3(10.21018f, 3.141593f, 10.21018f),
                new Quaternion(-0.3535535f, 0.1464468f, 0.3535535f, 0.8535532f),
                new float4x4(0.7071065f, -0.7071071f, 8.940697E-08f, 0f, 0.5f, 0.4999997f, 0.7071069f, 0f, -0.5000004f, -0.4999999f, 0.7071066f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.264406E-07f, 0.7853985f)
            };
            // 1033. - X: -90, Y: 180, Z: 585
            yield return new object[]
            {
                "X: -90, Y: 180, Z: 585",
                new float3(-1.570796f, 3.141593f, 10.21018f),
                new Quaternion(-0.6532814f, 0.2705982f, 0.2705982f, 0.6532814f),
                new float4x4(0.7071065f, -0.7071071f, 0f, 0f, 0f, 4.470348E-08f, 0.9999999f, 0f, -0.7071071f, -0.7071064f, 4.470348E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.7853988f, 0f)
            };
            // 1034. - X: -540, Y: 180, Z: 585
            yield return new object[]
            {
                "X: -540, Y: 180, Z: 585",
                new float3(-9.424778f, 3.141593f, 10.21018f),
                new Quaternion(-2.774478E-08f, -3.582059E-08f, -0.3826836f, -0.9238794f),
                new float4x4(0.7071065f, -0.707107f, 8.742277E-08f, 0f, 0.707107f, 0.7071065f, -2.384976E-08f, 0f, -4.495288E-08f, 7.868158E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 8.742277E-08f, 0.7853985f)
            };
            // 1035. - X: 0, Y: 270, Z: 585
            yield return new object[]
            {
                "X: 0, Y: 270, Z: 585",
                new float3(0f, 4.712389f, 10.21018f),
                new Quaternion(-0.6532814f, 0.2705982f, 0.6532814f, -0.2705982f),
                new float4x4(7.450581E-08f, 0f, -0.9999999f, 0f, -0.707107f, -0.7071064f, 0f, 0f, -0.7071064f, 0.707107f, 7.450581E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 3.926991f)
            };
            // 1036. - X: 0.1, Y: 270, Z: 585
            yield return new object[]
            {
                "X: 0.1, Y: 270, Z: 585",
                new float3(0.001745329f, 4.712389f, 10.21018f),
                new Quaternion(-0.6535174f, 0.270028f, 0.6530451f, -0.2711682f),
                new float4x4(0.001234099f, 0.001234114f, -0.9999986f, 0f, -0.7071061f, -0.7071056f, -0.001745313f, 0f, -0.7071066f, 0.7071071f, -1.341105E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745314f, 4.712389f, 3.926991f)
            };
            // 1037. - X: 1, Y: 270, Z: 585
            yield return new object[]
            {
                "X: 1, Y: 270, Z: 585",
                new float3(0.01745329f, 4.712389f, 10.21018f),
                new Quaternion(-0.6556179f, 0.264887f, 0.6508952f, -0.2762887f),
                new float4x4(0.01234069f, 0.01234072f, -0.9998477f, 0f, -0.7069993f, -0.7069987f, -0.01745236f, 0f, -0.7071065f, 0.707107f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745328f, 4.712389f, 3.926991f)
            };
            // 1038. - X: 45, Y: 270, Z: 585
            yield return new object[]
            {
                "X: 45, Y: 270, Z: 585",
                new float3(0.7853982f, 4.712389f, 10.21018f),
                new Quaternion(-0.7071067f, 1.043081E-07f, 0.4999999f, -0.5000001f),
                new float4x4(0.5000003f, 0.4999998f, -0.7071066f, 0f, -0.5000001f, -0.4999995f, -0.7071066f, 0f, -0.7071064f, 0.7071069f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 4.712389f, 3.926991f)
            };
            // 1039. - X: 90, Y: 270, Z: 585
            yield return new object[]
            {
                "X: 90, Y: 270, Z: 585",
                new float3(1.570796f, 4.712389f, 10.21018f),
                new Quaternion(-0.6532815f, -0.2705979f, 0.2705979f, -0.6532815f),
                new float4x4(0.7071071f, 0.7071064f, 0f, 0f, 0f, 2.533197E-07f, -0.9999998f, 0f, -0.7071064f, 0.7071068f, 2.533197E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570451f, 0.7853978f, 0f)
            };
            // 1040. - X: 180, Y: 270, Z: 585
            yield return new object[]
            {
                "X: 180, Y: 270, Z: 585",
                new float3(3.141593f, 4.712389f, 10.21018f),
                new Quaternion(-0.2705981f, -0.6532814f, -0.2705982f, -0.6532814f),
                new float4x4(4.470348E-08f, -5.960464E-08f, 0.9999999f, 0f, 0.707107f, 0.7071065f, 5.960464E-08f, 0f, -0.7071064f, 0.707107f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.940697E-08f, 1.570796f, 0.7853987f)
            };
            // 1041. - X: 270, Y: 270, Z: 585
            yield return new object[]
            {
                "X: 270, Y: 270, Z: 585",
                new float3(4.712389f, 4.712389f, 10.21018f),
                new Quaternion(0.2705979f, -0.6532815f, -0.6532815f, -0.2705979f),
                new float4x4(-0.7071066f, -0.7071064f, 0f, 0f, 0f, 2.533197E-07f, 0.9999998f, 0f, -0.7071064f, 0.7071068f, 2.533197E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 2.356195f, 0f)
            };
            // 1042. - X: 360, Y: 270, Z: 585
            yield return new object[]
            {
                "X: 360, Y: 270, Z: 585",
                new float3(6.283185f, 4.712389f, 10.21018f),
                new Quaternion(0.6532814f, -0.2705981f, -0.6532814f, 0.2705982f),
                new float4x4(1.490116E-07f, 1.788139E-07f, -0.9999999f, 0f, -0.707107f, -0.7071064f, -1.788139E-07f, 0f, -0.7071064f, 0.707107f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.490116E-07f, 4.712389f, 3.926991f)
            };
            // 1043. - X: 585, Y: 270, Z: 585
            yield return new object[]
            {
                "X: 585, Y: 270, Z: 585",
                new float3(10.21018f, 4.712389f, 10.21018f),
                new Quaternion(0f, 0.7071067f, 0.5000002f, 0.4999998f),
                new float4x4(-0.5000002f, -0.4999999f, 0.7071064f, 0f, 0.4999999f, 0.4999996f, 0.7071069f, 0f, -0.7071064f, 0.7071069f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.570796f, 0.7853987f)
            };
            // 1044. - X: -90, Y: 270, Z: 585
            yield return new object[]
            {
                "X: -90, Y: 270, Z: 585",
                new float3(-1.570796f, 4.712389f, 10.21018f),
                new Quaternion(-0.2705979f, 0.6532815f, 0.6532815f, 0.2705979f),
                new float4x4(-0.7071066f, -0.7071064f, 0f, 0f, 0f, 2.533197E-07f, 0.9999998f, 0f, -0.7071064f, 0.7071068f, 2.533197E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 2.356195f, 0f)
            };
            // 1045. - X: -540, Y: 270, Z: 585
            yield return new object[]
            {
                "X: -540, Y: 270, Z: 585",
                new float3(-9.424778f, 4.712389f, 10.21018f),
                new Quaternion(-0.2705982f, -0.6532814f, -0.2705982f, -0.6532814f),
                new float4x4(7.450581E-08f, 0f, 0.9999999f, 0f, 0.707107f, 0.7071065f, 0f, 0f, -0.7071064f, 0.707107f, 7.450581E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.570796f, 0.7853986f)
            };
            // 1046. - X: 0, Y: 360, Z: 585
            yield return new object[]
            {
                "X: 0, Y: 360, Z: 585",
                new float3(0f, 6.283185f, 10.21018f),
                new Quaternion(8.07681E-08f, -3.345526E-08f, 0.9238794f, -0.3826836f),
                new float4x4(-0.7071065f, 0.707107f, 1.748455E-07f, 0f, -0.707107f, -0.7071065f, 0f, 0f, 1.236344E-07f, -1.236345E-07f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.748455E-07f, 3.926991f)
            };
            // 1047. - X: 0.1, Y: 360, Z: 585
            yield return new object[]
            {
                "X: 0.1, Y: 360, Z: 585",
                new float3(0.001745329f, 6.283185f, 10.21018f),
                new Quaternion(-0.0003338736f, -0.0008062703f, 0.9238791f, -0.3826835f),
                new float4x4(-0.7071065f, 0.707107f, 1.748558E-07f, 0f, -0.7071059f, -0.7071054f, -0.001745328f, 0f, -0.00123401f, -0.001234257f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 1.748561E-07f, 3.926991f)
            };
            // 1048. - X: 1, Y: 360, Z: 585
            yield return new object[]
            {
                "X: 1, Y: 360, Z: 585",
                new float3(0.01745329f, 6.283185f, 10.21018f),
                new Quaternion(-0.003339421f, -0.0080623f, 0.9238443f, -0.382669f),
                new float4x4(-0.7071065f, 0.707107f, 1.74623E-07f, 0f, -0.7069994f, -0.7069988f, -0.01745241f, 0f, -0.0123406f, -0.01234083f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 1.746496E-07f, 3.926991f)
            };
            // 1049. - X: 45, Y: 360, Z: 585
            yield return new object[]
            {
                "X: 45, Y: 360, Z: 585",
                new float3(0.7853982f, 6.283185f, 10.21018f),
                new Quaternion(-0.1464466f, -0.3535534f, 0.8535533f, -0.3535535f),
                new float4x4(-0.7071065f, 0.7071069f, 1.043081E-07f, 0f, -0.5000001f, -0.4999997f, -0.7071068f, 0f, -0.5000001f, -0.4999999f, 0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853984f, 1.685874E-07f, 3.926991f)
            };
            // 1050. - X: 90, Y: 360, Z: 585
            yield return new object[]
            {
                "X: 90, Y: 360, Z: 585",
                new float3(1.570796f, 6.283185f, 10.21018f),
                new Quaternion(-0.2705981f, -0.6532814f, 0.6532814f, -0.2705981f),
                new float4x4(-0.7071064f, 0.7071068f, 0f, 0f, 0f, 1.490116E-07f, -0.9999999f, 0f, -0.7071068f, -0.7071065f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 2.356194f, 0f)
            };
            // 1051. - X: 180, Y: 360, Z: 585
            yield return new object[]
            {
                "X: 180, Y: 360, Z: 585",
                new float3(3.141593f, 6.283185f, 10.21018f),
                new Quaternion(-0.3826836f, -0.9238794f, -6.92879E-09f, 9.749574E-08f),
                new float4x4(-0.7071065f, 0.707107f, -1.748456E-07f, 0f, 0.707107f, 0.7071065f, 8.742278E-08f, 0f, 1.854517E-07f, -6.18173E-08f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 3.141593f, 0.7853985f)
            };
            // 1052. - X: 270, Y: 360, Z: 585
            yield return new object[]
            {
                "X: 270, Y: 360, Z: 585",
                new float3(4.712389f, 6.283185f, 10.21018f),
                new Quaternion(-0.2705982f, -0.6532814f, -0.6532814f, 0.2705982f),
                new float4x4(-0.7071064f, 0.7071072f, 0f, 0f, 0f, 1.490116E-08f, 1f, 0f, 0.7071072f, 0.7071064f, 1.490116E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.926991f, 0f)
            };
            // 1053. - X: 360, Y: 360, Z: 585
            yield return new object[]
            {
                "X: 360, Y: 360, Z: 585",
                new float3(6.283185f, 6.283185f, 10.21018f),
                new Quaternion(-4.731284E-08f, 1.142234E-07f, -0.9238794f, 0.3826836f),
                new float4x4(-0.7071065f, 0.707107f, 1.748456E-07f, 0f, -0.707107f, -0.7071065f, -1.748455E-07f, 0f, -9.237056E-14f, -2.472689E-07f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748455E-07f, 1.748456E-07f, 3.926991f)
            };
            // 1054. - X: 585, Y: 360, Z: 585
            yield return new object[]
            {
                "X: 585, Y: 360, Z: 585",
                new float3(10.21018f, 6.283185f, 10.21018f),
                new Quaternion(0.3535535f, 0.8535532f, 0.3535535f, -0.1464468f),
                new float4x4(-0.7071064f, 0.7071071f, -1.490116E-07f, 0f, 0.5f, 0.4999997f, 0.707107f, 0f, 0.5000005f, 0.4999999f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.141593f, 0.7853984f)
            };
            // 1055. - X: -90, Y: 360, Z: 585
            yield return new object[]
            {
                "X: -90, Y: 360, Z: 585",
                new float3(-1.570796f, 6.283185f, 10.21018f),
                new Quaternion(0.2705982f, 0.6532814f, 0.6532814f, -0.2705982f),
                new float4x4(-0.7071064f, 0.7071072f, 0f, 0f, 0f, 1.490116E-08f, 1f, 0f, 0.7071072f, 0.7071064f, 1.490116E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.926991f, 0f)
            };
            // 1056. - X: -540, Y: 360, Z: 585
            yield return new object[]
            {
                "X: -540, Y: 360, Z: 585",
                new float3(-9.424778f, 6.283185f, 10.21018f),
                new Quaternion(-0.3826836f, -0.9238794f, 4.447242E-08f, 7.620465E-08f),
                new float4x4(-0.7071065f, 0.707107f, -1.748455E-07f, 0f, 0.707107f, 0.7071065f, -2.384976E-08f, 0f, 1.067701E-07f, -1.404988E-07f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.141593f, 0.7853985f)
            };
            // 1057. - X: 0, Y: 585, Z: 585
            yield return new object[]
            {
                "X: 0, Y: 585, Z: 585",
                new float3(0f, 10.21018f, 10.21018f),
                new Quaternion(0.8535532f, -0.3535535f, -0.3535535f, 0.1464467f),
                new float4x4(0.4999997f, -0.5f, -0.707107f, 0f, -0.707107f, -0.7071064f, -2.980232E-08f, 0f, -0.5f, 0.5000004f, -0.7071064f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.926991f, 3.926991f)
            };
            // 1058. - X: 0.1, Y: 585, Z: 585
            yield return new object[]
            {
                "X: 0.1, Y: 585, Z: 585",
                new float3(0.001745329f, 10.21018f, 10.21018f),
                new Quaternion(0.8536807f, -0.3532448f, -0.3532448f, 0.1471916f),
                new float4x4(0.5008724f, -0.4991273f, -0.7071059f, 0f, -0.7071059f, -0.7071052f, -0.001745328f, 0f, -0.4991273f, 0.500873f, -0.7071052f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745344f, 3.926991f, 3.926991f)
            };
            // 1059. - X: 1, Y: 585, Z: 585
            yield return new object[]
            {
                "X: 1, Y: 585, Z: 585",
                new float3(0.01745329f, 10.21018f, 10.21018f),
                new Quaternion(0.8547987f, -0.3504548f, -0.3504548f, 0.1538897f),
                new float4x4(0.5087258f, -0.4912738f, -0.7069994f, 0f, -0.7069994f, -0.7069989f, -0.0174524f, 0f, -0.4912738f, 0.5087266f, -0.7069989f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.926991f, 3.926991f)
            };
            // 1060. - X: 45, Y: 585, Z: 585
            yield return new object[]
            {
                "X: 45, Y: 585, Z: 585",
                new float3(0.7853982f, 10.21018f, 10.21018f),
                new Quaternion(0.8446231f, -0.1913418f, -0.1913418f, 0.4619398f),
                new float4x4(0.8535533f, -0.1464466f, -0.5000001f, 0f, -0.5000001f, -0.4999996f, -0.7071068f, 0f, -0.1464466f, 0.8535535f, -0.4999996f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 3.926991f, 3.926991f)
            };
            // 1061. - X: 90, Y: 585, Z: 585
            yield return new object[]
            {
                "X: 90, Y: 585, Z: 585",
                new float3(1.570796f, 10.21018f, 10.21018f),
                new Quaternion(0.7071067f, 0f, 0f, 0.7071067f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 1.788139E-07f, -0.9999998f, 0f, 0f, 0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570451f, 0f, 0f)
            };
            // 1062. - X: 180, Y: 585, Z: 585
            yield return new object[]
            {
                "X: 180, Y: 585, Z: 585",
                new float3(3.141593f, 10.21018f, 10.21018f),
                new Quaternion(0.1464467f, 0.3535535f, 0.3535535f, 0.8535532f),
                new float4x4(0.4999996f, -0.5000001f, 0.707107f, 0f, 0.707107f, 0.7071065f, 1.192093E-07f, 0f, -0.5000001f, 0.5000003f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.940697E-08f, 0.7853985f, 0.7853985f)
            };
            // 1063. - X: 270, Y: 585, Z: 585
            yield return new object[]
            {
                "X: 270, Y: 585, Z: 585",
                new float3(4.712389f, 10.21018f, 10.21018f),
                new Quaternion(-0.4999998f, 0.5000002f, 0.5000002f, 0.4999998f),
                new float4x4(-7.152557E-07f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 8.34465E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570797f, 0f)
            };
            // 1064. - X: 360, Y: 585, Z: 585
            yield return new object[]
            {
                "X: 360, Y: 585, Z: 585",
                new float3(6.283185f, 10.21018f, 10.21018f),
                new Quaternion(-0.8535532f, 0.3535535f, 0.3535535f, -0.1464468f),
                new float4x4(0.4999998f, -0.4999999f, -0.7071069f, 0f, -0.7071069f, -0.7071064f, -1.788139E-07f, 0f, -0.4999999f, 0.5000004f, -0.7071064f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.788139E-07f, 3.926991f, 3.926991f)
            };
            // 1065. - X: 585, Y: 585, Z: 585
            yield return new object[]
            {
                "X: 585, Y: 585, Z: 585",
                new float3(10.21018f, 10.21018f, 10.21018f),
                new Quaternion(0.1913417f, -0.4619399f, -0.4619399f, -0.7325375f),
                new float4x4(0.146446f, -0.8535534f, 0.4999999f, 0f, 0.4999999f, 0.4999997f, 0.7071069f, 0f, -0.8535534f, 0.1464471f, 0.4999997f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 0.7853985f, 0.7853985f)
            };
            // 1066. - X: -90, Y: 585, Z: 585
            yield return new object[]
            {
                "X: -90, Y: 585, Z: 585",
                new float3(-1.570796f, 10.21018f, 10.21018f),
                new Quaternion(0.4999998f, -0.5000002f, -0.5000002f, -0.4999998f),
                new float4x4(-7.152557E-07f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 8.34465E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570797f, 0f)
            };
            // 1067. - X: -540, Y: 585, Z: 585
            yield return new object[]
            {
                "X: -540, Y: 585, Z: 585",
                new float3(-9.424778f, 10.21018f, 10.21018f),
                new Quaternion(0.1464467f, 0.3535535f, 0.3535535f, 0.8535532f),
                new float4x4(0.4999997f, -0.4999999f, 0.707107f, 0f, 0.707107f, 0.7071065f, -5.960464E-08f, 0f, -0.4999999f, 0.5000004f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.980232E-08f, 0.7853985f, 0.7853985f)
            };
            // 1068. - X: 0, Y: -90, Z: 585
            yield return new object[]
            {
                "X: 0, Y: -90, Z: 585",
                new float3(0f, -1.570796f, 10.21018f),
                new Quaternion(0.6532814f, -0.2705982f, -0.6532814f, 0.2705982f),
                new float4x4(7.450581E-08f, 0f, -0.9999999f, 0f, -0.707107f, -0.7071064f, 0f, 0f, -0.7071064f, 0.707107f, 7.450581E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 3.926991f)
            };
            // 1069. - X: 0.1, Y: -90, Z: 585
            yield return new object[]
            {
                "X: 0.1, Y: -90, Z: 585",
                new float3(0.001745329f, -1.570796f, 10.21018f),
                new Quaternion(0.6535174f, -0.270028f, -0.6530451f, 0.2711682f),
                new float4x4(0.001234099f, 0.001234114f, -0.9999986f, 0f, -0.7071061f, -0.7071056f, -0.001745313f, 0f, -0.7071066f, 0.7071071f, -1.341105E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745314f, 4.712389f, 3.926991f)
            };
            // 1070. - X: 1, Y: -90, Z: 585
            yield return new object[]
            {
                "X: 1, Y: -90, Z: 585",
                new float3(0.01745329f, -1.570796f, 10.21018f),
                new Quaternion(0.6556179f, -0.264887f, -0.6508952f, 0.2762887f),
                new float4x4(0.01234069f, 0.01234072f, -0.9998477f, 0f, -0.7069993f, -0.7069987f, -0.01745236f, 0f, -0.7071065f, 0.707107f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745328f, 4.712389f, 3.926991f)
            };
            // 1071. - X: 45, Y: -90, Z: 585
            yield return new object[]
            {
                "X: 45, Y: -90, Z: 585",
                new float3(0.7853982f, -1.570796f, 10.21018f),
                new Quaternion(0.7071067f, -1.043081E-07f, -0.4999999f, 0.5000001f),
                new float4x4(0.5000003f, 0.4999998f, -0.7071066f, 0f, -0.5000001f, -0.4999995f, -0.7071066f, 0f, -0.7071064f, 0.7071069f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 4.712389f, 3.926991f)
            };
            // 1072. - X: 90, Y: -90, Z: 585
            yield return new object[]
            {
                "X: 90, Y: -90, Z: 585",
                new float3(1.570796f, -1.570796f, 10.21018f),
                new Quaternion(0.6532815f, 0.2705979f, -0.2705979f, 0.6532815f),
                new float4x4(0.7071071f, 0.7071064f, 0f, 0f, 0f, 2.533197E-07f, -0.9999998f, 0f, -0.7071064f, 0.7071068f, 2.533197E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570451f, 0.7853978f, 0f)
            };
            // 1073. - X: 180, Y: -90, Z: 585
            yield return new object[]
            {
                "X: 180, Y: -90, Z: 585",
                new float3(3.141593f, -1.570796f, 10.21018f),
                new Quaternion(0.2705981f, 0.6532814f, 0.2705982f, 0.6532814f),
                new float4x4(4.470348E-08f, -5.960464E-08f, 0.9999999f, 0f, 0.707107f, 0.7071065f, 5.960464E-08f, 0f, -0.7071064f, 0.707107f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.940697E-08f, 1.570796f, 0.7853987f)
            };
            // 1074. - X: 270, Y: -90, Z: 585
            yield return new object[]
            {
                "X: 270, Y: -90, Z: 585",
                new float3(4.712389f, -1.570796f, 10.21018f),
                new Quaternion(-0.2705979f, 0.6532815f, 0.6532815f, 0.2705979f),
                new float4x4(-0.7071066f, -0.7071064f, 0f, 0f, 0f, 2.533197E-07f, 0.9999998f, 0f, -0.7071064f, 0.7071068f, 2.533197E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 2.356195f, 0f)
            };
            // 1075. - X: 360, Y: -90, Z: 585
            yield return new object[]
            {
                "X: 360, Y: -90, Z: 585",
                new float3(6.283185f, -1.570796f, 10.21018f),
                new Quaternion(-0.6532814f, 0.2705981f, 0.6532814f, -0.2705982f),
                new float4x4(1.490116E-07f, 1.788139E-07f, -0.9999999f, 0f, -0.707107f, -0.7071064f, -1.788139E-07f, 0f, -0.7071064f, 0.707107f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.490116E-07f, 4.712389f, 3.926991f)
            };
            // 1076. - X: 585, Y: -90, Z: 585
            yield return new object[]
            {
                "X: 585, Y: -90, Z: 585",
                new float3(10.21018f, -1.570796f, 10.21018f),
                new Quaternion(0f, -0.7071067f, -0.5000002f, -0.4999998f),
                new float4x4(-0.5000002f, -0.4999999f, 0.7071064f, 0f, 0.4999999f, 0.4999996f, 0.7071069f, 0f, -0.7071064f, 0.7071069f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.570796f, 0.7853987f)
            };
            // 1077. - X: -90, Y: -90, Z: 585
            yield return new object[]
            {
                "X: -90, Y: -90, Z: 585",
                new float3(-1.570796f, -1.570796f, 10.21018f),
                new Quaternion(0.2705979f, -0.6532815f, -0.6532815f, -0.2705979f),
                new float4x4(-0.7071066f, -0.7071064f, 0f, 0f, 0f, 2.533197E-07f, 0.9999998f, 0f, -0.7071064f, 0.7071068f, 2.533197E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 2.356195f, 0f)
            };
            // 1078. - X: -540, Y: -90, Z: 585
            yield return new object[]
            {
                "X: -540, Y: -90, Z: 585",
                new float3(-9.424778f, -1.570796f, 10.21018f),
                new Quaternion(0.2705982f, 0.6532814f, 0.2705982f, 0.6532814f),
                new float4x4(7.450581E-08f, 0f, 0.9999999f, 0f, 0.707107f, 0.7071065f, 0f, 0f, -0.7071064f, 0.707107f, 7.450581E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.570796f, 0.7853986f)
            };
            // 1079. - X: 0, Y: -540, Z: 585
            yield return new object[]
            {
                "X: 0, Y: -540, Z: 585",
                new float3(0f, -9.424778f, 10.21018f),
                new Quaternion(-0.9238794f, 0.3826836f, -1.101715E-08f, 4.563456E-09f),
                new float4x4(0.7071065f, -0.707107f, 2.384976E-08f, 0f, -0.707107f, -0.7071065f, 0f, 0f, 1.686432E-08f, -1.686433E-08f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.141593f, 3.926991f)
            };
            // 1080. - X: 0.1, Y: -540, Z: 585
            yield return new object[]
            {
                "X: 0.1, Y: -540, Z: 585",
                new float3(0.001745329f, -9.424778f, 10.21018f),
                new Quaternion(-0.9238791f, 0.3826835f, -0.0003339654f, -0.0008062323f),
                new float4x4(0.7071065f, -0.707107f, 2.386514E-08f, 0f, -0.7071059f, -0.7071054f, -0.001745328f, 0f, 0.001234151f, 0.001234116f, -0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.141593f, 3.926991f)
            };
            // 1081. - X: 1, Y: -540, Z: 585
            yield return new object[]
            {
                "X: 1, Y: -540, Z: 585",
                new float3(0.01745329f, -9.424778f, 10.21018f),
                new Quaternion(-0.9238443f, 0.382669f, -0.003339513f, -0.008062262f),
                new float4x4(0.7071065f, -0.707107f, 2.374873E-08f, 0f, -0.7069994f, -0.7069988f, -0.01745241f, 0f, 0.01234074f, 0.01234069f, -0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.141593f, 3.926991f)
            };
            // 1082. - X: 45, Y: -540, Z: 585
            yield return new object[]
            {
                "X: 45, Y: -540, Z: 585",
                new float3(0.7853982f, -9.424778f, 10.21018f),
                new Quaternion(-0.8535533f, 0.3535535f, -0.1464467f, -0.3535534f),
                new float4x4(0.7071065f, -0.7071071f, 5.960464E-08f, 0f, -0.5000001f, -0.4999998f, -0.7071068f, 0f, 0.5000002f, 0.4999998f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 3.141593f, 3.926991f)
            };
            // 1083. - X: 90, Y: -540, Z: 585
            yield return new object[]
            {
                "X: 90, Y: -540, Z: 585",
                new float3(1.570796f, -9.424778f, 10.21018f),
                new Quaternion(-0.6532814f, 0.2705982f, -0.2705982f, -0.6532814f),
                new float4x4(0.7071065f, -0.707107f, 0f, 0f, 0f, 7.450581E-08f, -0.9999999f, 0f, 0.707107f, 0.7071064f, 7.450581E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 5.497787f, 0f)
            };
            // 1084. - X: 180, Y: -540, Z: 585
            yield return new object[]
            {
                "X: 180, Y: -540, Z: 585",
                new float3(3.141593f, -9.424778f, 10.21018f),
                new Quaternion(4.494751E-08f, -5.710478E-09f, -0.3826836f, -0.9238794f),
                new float4x4(0.7071065f, -0.707107f, -2.384976E-08f, 0f, 0.707107f, 0.7071065f, 8.742277E-08f, 0f, -4.495294E-08f, -7.868154E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742277E-08f, -2.384976E-08f, 0.7853985f)
            };
            // 1085. - X: 270, Y: -540, Z: 585
            yield return new object[]
            {
                "X: 270, Y: -540, Z: 585",
                new float3(4.712389f, -9.424778f, 10.21018f),
                new Quaternion(0.6532814f, -0.2705982f, -0.2705982f, -0.6532814f),
                new float4x4(0.7071065f, -0.707107f, 0f, 0f, 0f, 7.450581E-08f, 0.9999999f, 0f, -0.707107f, -0.7071064f, 7.450581E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.7853986f, 0f)
            };
            // 1086. - X: 360, Y: -540, Z: 585
            yield return new object[]
            {
                "X: 360, Y: -540, Z: 585",
                new float3(6.283185f, -9.424778f, 10.21018f),
                new Quaternion(0.9238794f, -0.3826836f, 4.447242E-08f, 7.620465E-08f),
                new float4x4(0.7071065f, -0.707107f, 2.384976E-08f, 0f, -0.707107f, -0.7071065f, -1.748455E-07f, 0f, 1.404988E-07f, 1.067701E-07f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748455E-07f, 3.141593f, 3.926991f)
            };
            // 1087. - X: 585, Y: -540, Z: 585
            yield return new object[]
            {
                "X: 585, Y: -540, Z: 585",
                new float3(10.21018f, -9.424778f, 10.21018f),
                new Quaternion(-0.3535535f, 0.1464467f, 0.3535535f, 0.8535532f),
                new float4x4(0.7071066f, -0.7071069f, 0f, 0f, 0.5f, 0.4999997f, 0.7071069f, 0f, -0.5000003f, -0.5f, 0.7071066f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, -4.214686E-08f, 0.7853985f)
            };
            // 1088. - X: -90, Y: -540, Z: 585
            yield return new object[]
            {
                "X: -90, Y: -540, Z: 585",
                new float3(-1.570796f, -9.424778f, 10.21018f),
                new Quaternion(-0.6532814f, 0.2705982f, 0.2705982f, 0.6532814f),
                new float4x4(0.7071065f, -0.707107f, 0f, 0f, 0f, 7.450581E-08f, 0.9999999f, 0f, -0.707107f, -0.7071064f, 7.450581E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.7853986f, 0f)
            };
            // 1089. - X: -540, Y: -540, Z: 585
            yield return new object[]
            {
                "X: -540, Y: -540, Z: 585",
                new float3(-9.424778f, -9.424778f, 10.21018f),
                new Quaternion(-6.453696E-09f, 1.558061E-08f, -0.3826836f, -0.9238794f),
                new float4x4(0.7071065f, -0.707107f, -2.384976E-08f, 0f, 0.707107f, 0.7071065f, -2.384976E-08f, 0f, 3.372866E-08f, -1.24345E-14f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, -2.384976E-08f, 0.7853985f)
            };
            // 1090. - X: 0, Y: 0, Z: -90
            yield return new object[]
            {
                "X: 0, Y: 0, Z: -90",
                new float3(0f, 0f, -1.570796f),
                new Quaternion(0f, 0f, -0.7071068f, 0.7071068f),
                new float4x4(5.960464E-08f, 0.9999999f, 0f, 0f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0f, 4.712389f)
            };
            // 1091. - X: 0.1, Y: 0, Z: -90
            yield return new object[]
            {
                "X: 0.1, Y: 0, Z: -90",
                new float3(0.001745329f, 0f, -1.570796f),
                new Quaternion(0.000617067f, 0.000617067f, -0.7071065f, 0.7071065f),
                new float4x4(-4.62876E-08f, 1f, 0f, 0f, -0.9999985f, -4.62876E-08f, -0.001745328f, 0f, -0.001745328f, 0f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0f, 4.712389f)
            };
            // 1092. - X: 1, Y: 0, Z: -90
            yield return new object[]
            {
                "X: 1, Y: 0, Z: -90",
                new float3(0.01745329f, 0f, -1.570796f),
                new Quaternion(0.006170592f, 0.006170592f, -0.7070798f, 0.7070798f),
                new float4x4(8.192001E-08f, 0.9999999f, 0f, 0f, -0.9998476f, 8.192001E-08f, -0.01745241f, 0f, -0.01745241f, 0f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0f, 4.712389f)
            };
            // 1093. - X: 45, Y: 0, Z: -90
            yield return new object[]
            {
                "X: 45, Y: 0, Z: -90",
                new float3(0.7853982f, 0f, -1.570796f),
                new Quaternion(0.2705981f, 0.2705981f, -0.6532815f, 0.6532815f),
                new float4x4(8.940697E-08f, 0.9999999f, 0f, 0f, -0.7071067f, 8.940697E-08f, -0.7071068f, 0f, -0.7071068f, 0f, 0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 0f, 4.712389f)
            };
            // 1094. - X: 90, Y: 0, Z: -90
            yield return new object[]
            {
                "X: 90, Y: 0, Z: -90",
                new float3(1.570796f, 0f, -1.570796f),
                new Quaternion(0.5f, 0.5f, -0.5f, 0.5f),
                new float4x4(1.192093E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, -0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 1.570796f, 0f)
            };
            // 1095. - X: 180, Y: 0, Z: -90
            yield return new object[]
            {
                "X: 180, Y: 0, Z: -90",
                new float3(3.141593f, 0f, -1.570796f),
                new Quaternion(0.7071068f, 0.7071068f, 3.090862E-08f, -3.090862E-08f),
                new float4x4(5.960464E-08f, 0.9999999f, 0f, 0f, 0.9999999f, 5.960464E-08f, 8.742278E-08f, 0f, 8.742278E-08f, 0f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742279E-08f, 3.141593f, 1.570796f)
            };
            // 1096. - X: 270, Y: 0, Z: -90
            yield return new object[]
            {
                "X: 270, Y: 0, Z: -90",
                new float3(4.712389f, 0f, -1.570796f),
                new Quaternion(0.5f, 0.5f, 0.5f, -0.5f),
                new float4x4(1.192093E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, 0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 1097. - X: 360, Y: 0, Z: -90
            yield return new object[]
            {
                "X: 360, Y: 0, Z: -90",
                new float3(6.283185f, 0f, -1.570796f),
                new Quaternion(-6.181724E-08f, -6.181724E-08f, 0.7071068f, -0.7071068f),
                new float4x4(5.960464E-08f, 0.9999999f, 0f, 0f, -0.9999999f, 5.960464E-08f, -1.748456E-07f, 0f, -1.748456E-07f, 0f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 0f, 4.712389f)
            };
            // 1098. - X: 585, Y: 0, Z: -90
            yield return new object[]
            {
                "X: 585, Y: 0, Z: -90",
                new float3(10.21018f, 0f, -1.570796f),
                new Quaternion(-0.6532814f, -0.6532814f, -0.2705982f, 0.2705982f),
                new float4x4(7.450581E-08f, 0.9999999f, 0f, 0f, 0.7071064f, 7.450581E-08f, 0.707107f, 0f, 0.707107f, 0f, -0.7071064f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.141593f, 1.570796f)
            };
            // 1099. - X: -90, Y: 0, Z: -90
            yield return new object[]
            {
                "X: -90, Y: 0, Z: -90",
                new float3(-1.570796f, 0f, -1.570796f),
                new Quaternion(-0.5f, -0.5f, -0.5f, 0.5f),
                new float4x4(1.192093E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, 0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 1100. - X: -540, Y: 0, Z: -90
            yield return new object[]
            {
                "X: -540, Y: 0, Z: -90",
                new float3(-9.424778f, 0f, -1.570796f),
                new Quaternion(0.7071068f, 0.7071068f, -8.432163E-09f, 8.432163E-09f),
                new float4x4(5.960464E-08f, 0.9999999f, 0f, 0f, 0.9999999f, 5.960464E-08f, -2.384976E-08f, 0f, -2.384976E-08f, 0f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.141593f, 1.570796f)
            };
            // 1101. - X: 0, Y: 0.1, Z: -90
            yield return new object[]
            {
                "X: 0, Y: 0.1, Z: -90",
                new float3(0f, 0.001745329f, -1.570796f),
                new Quaternion(-0.000617067f, 0.000617067f, -0.7071065f, 0.7071065f),
                new float4x4(-4.62876E-08f, 0.9999985f, 0.001745328f, 0f, -1f, -4.62876E-08f, 0f, 0f, 0f, -0.001745328f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0.001745329f, 4.712389f)
            };
            // 1102. - X: 0.1, Y: 0.1, Z: -90
            yield return new object[]
            {
                "X: 0.1, Y: 0.1, Z: -90",
                new float3(0.001745329f, 0.001745329f, -1.570796f),
                new Quaternion(0f, 0.001234133f, -0.7071068f, 0.7071058f),
                new float4x4(-3.16538E-06f, 0.9999986f, 0.001745326f, 0f, -0.9999986f, -1.192093E-07f, -0.001745328f, 0f, -0.001745326f, -0.001745328f, 0.999997f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0.001745329f, 4.712389f)
            };
            // 1103. - X: 1, Y: 0.1, Z: -90
            yield return new object[]
            {
                "X: 1, Y: 0.1, Z: -90",
                new float3(0.01745329f, 0.001745329f, -1.570796f),
                new Quaternion(0.005553547f, 0.006787634f, -0.707085f, 0.7070742f),
                new float4x4(-3.039354E-05f, 0.9999985f, 0.001745063f, 0f, -0.9998477f, 6.664777E-08f, -0.01745241f, 0f, -0.01745238f, -0.001745328f, 0.9998462f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0.00174533f, 4.712389f)
            };
            // 1104. - X: 45, Y: 0.1, Z: -90
            yield return new object[]
            {
                "X: 45, Y: 0.1, Z: -90",
                new float3(0.7853982f, 0.001745329f, -1.570796f),
                new Quaternion(0.2700279f, 0.2711681f, -0.6535174f, 0.6530451f),
                new float4x4(-0.001234129f, 0.9999985f, 0.001234084f, 0f, -0.7071067f, -1.490116E-08f, -0.7071068f, 0f, -0.7071058f, -0.001745313f, 0.7071056f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853984f, 0.001745302f, 4.712389f)
            };
            // 1105. - X: 90, Y: 0.1, Z: -90
            yield return new object[]
            {
                "X: 90, Y: 0.1, Z: -90",
                new float3(1.570796f, 0.001745329f, -1.570796f),
                new Quaternion(0.4995635f, 0.5004361f, -0.5004361f, 0.4995635f),
                new float4x4(-0.001745224f, 0.9999985f, 0f, 0f, 0f, 2.980232E-08f, -1f, 0f, -0.9999985f, -0.001745254f, 2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 1.572542f, 0f)
            };
            // 1106. - X: 180, Y: 0.1, Z: -90
            yield return new object[]
            {
                "X: 180, Y: 0.1, Z: -90",
                new float3(3.141593f, 0.001745329f, -1.570796f),
                new Quaternion(0.7071065f, 0.7071065f, -0.0006170361f, -0.0006170979f),
                new float4x4(-4.621131E-08f, 0.9999985f, -0.001745328f, 0f, 1f, -4.621131E-08f, 8.73697E-08f, 0f, 8.73697E-08f, -0.001745328f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.73697E-08f, 3.143338f, 1.570796f)
            };
            // 1107. - X: 270, Y: 0.1, Z: -90
            yield return new object[]
            {
                "X: 270, Y: 0.1, Z: -90",
                new float3(4.712389f, 0.001745329f, -1.570796f),
                new Quaternion(0.5004361f, 0.4995635f, 0.4995635f, -0.5004361f),
                new float4x4(0.001745284f, 0.9999985f, 0f, 0f, 0f, 2.980232E-08f, 1f, 0f, 0.9999985f, -0.001745254f, 2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.714134f, 0f)
            };
            // 1108. - X: 360, Y: 0.1, Z: -90
            yield return new object[]
            {
                "X: 360, Y: 0.1, Z: -90",
                new float3(6.283185f, 0.001745329f, -1.570796f),
                new Quaternion(0.0006170052f, -0.0006171288f, 0.7071065f, -0.7071065f),
                new float4x4(-4.644016E-08f, 0.9999985f, 0.001745328f, 0f, -1f, -4.613503E-08f, -1.747976E-07f, 0f, -1.747976E-07f, -0.001745328f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.747976E-07f, 0.001745329f, 4.712389f)
            };
            // 1109. - X: 585, Y: 0.1, Z: -90
            yield return new object[]
            {
                "X: 585, Y: 0.1, Z: -90",
                new float3(10.21018f, 0.001745329f, -1.570796f),
                new Quaternion(-0.6535173f, -0.653045f, -0.270028f, 0.2711681f),
                new float4x4(0.001234248f, 0.9999983f, -0.001234084f, 0f, 0.7071065f, 7.450581E-08f, 0.7071069f, 0f, 0.7071059f, -0.001745313f, -0.7071053f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.143338f, 1.570796f)
            };
            // 1110. - X: -90, Y: 0.1, Z: -90
            yield return new object[]
            {
                "X: -90, Y: 0.1, Z: -90",
                new float3(-1.570796f, 0.001745329f, -1.570796f),
                new Quaternion(-0.5004361f, -0.4995635f, -0.4995635f, 0.5004361f),
                new float4x4(0.001745284f, 0.9999985f, 0f, 0f, 0f, 2.980232E-08f, 1f, 0f, 0.9999985f, -0.001745254f, 2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.714134f, 0f)
            };
            // 1111. - X: -540, Y: 0.1, Z: -90
            yield return new object[]
            {
                "X: -540, Y: 0.1, Z: -90",
                new float3(-9.424778f, 0.001745329f, -1.570796f),
                new Quaternion(0.7071065f, 0.7071065f, -0.0006170754f, -0.0006170585f),
                new float4x4(-4.63084E-08f, 0.9999985f, -0.001745328f, 0f, 1f, -4.63084E-08f, -2.392335E-08f, 0f, -2.392335E-08f, -0.001745328f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.392335E-08f, 3.143338f, 1.570796f)
            };
            // 1112. - X: 0, Y: 1, Z: -90
            yield return new object[]
            {
                "X: 0, Y: 1, Z: -90",
                new float3(0f, 0.01745329f, -1.570796f),
                new Quaternion(-0.006170592f, 0.006170592f, -0.7070798f, 0.7070798f),
                new float4x4(8.192001E-08f, 0.9998476f, 0.01745241f, 0f, -0.9999999f, 8.192001E-08f, 0f, 0f, 0f, -0.01745241f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0.01745329f, 4.712389f)
            };
            // 1113. - X: 0.1, Y: 1, Z: -90
            yield return new object[]
            {
                "X: 0.1, Y: 1, Z: -90",
                new float3(0.001745329f, 0.01745329f, -1.570796f),
                new Quaternion(-0.005553547f, 0.006787634f, -0.707085f, 0.7070742f),
                new float4x4(-3.039354E-05f, 0.9998477f, 0.01745238f, 0f, -0.9999985f, 6.664777E-08f, -0.001745328f, 0f, -0.001745063f, -0.01745241f, 0.9998462f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0.01745329f, 4.712389f)
            };
            // 1114. - X: 1, Y: 1, Z: -90
            yield return new object[]
            {
                "X: 1, Y: 1, Z: -90",
                new float3(0.01745329f, 0.01745329f, -1.570796f),
                new Quaternion(0f, 0.01234071f, -0.7071067f, 0.7069991f),
                new float4x4(-0.0003044076f, 0.9998476f, 0.01744975f, 0f, -0.9998476f, 1.788139E-07f, -0.0174524f, 0f, -0.01744975f, -0.0174524f, 0.9996954f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0.01745329f, 4.712389f)
            };
            // 1115. - X: 45, Y: 1, Z: -90
            yield return new object[]
            {
                "X: 45, Y: 1, Z: -90",
                new float3(0.7853982f, 0.01745329f, -1.570796f),
                new Quaternion(0.2648869f, 0.2762886f, -0.655618f, 0.6508952f),
                new float4x4(-0.01234062f, 0.9998477f, 0.01234072f, 0f, -0.7071067f, 1.043081E-07f, -0.7071067f, 0f, -0.7069991f, -0.01745239f, 0.7069991f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853981f, 0.01745331f, 4.712389f)
            };
            // 1116. - X: 90, Y: 1, Z: -90
            yield return new object[]
            {
                "X: 90, Y: 1, Z: -90",
                new float3(1.570796f, 0.01745329f, -1.570796f),
                new Quaternion(0.4956177f, 0.5043442f, -0.5043442f, 0.4956177f),
                new float4x4(-0.01745212f, 0.9998475f, 0f, 0f, 0f, 2.086163E-07f, -0.9999998f, 0f, -0.9998475f, -0.01745233f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 1.58825f, 0f)
            };
            // 1117. - X: 180, Y: 1, Z: -90
            yield return new object[]
            {
                "X: 180, Y: 1, Z: -90",
                new float3(3.141593f, 0.01745329f, -1.570796f),
                new Quaternion(0.7070798f, 0.7070798f, -0.006170562f, -0.006170623f),
                new float4x4(8.268398E-08f, 0.9998476f, -0.0174524f, 0f, 0.9999999f, 8.268398E-08f, 8.6613E-08f, 0f, 8.6613E-08f, -0.0174524f, -0.9998475f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.6613E-08f, 3.159046f, 1.570796f)
            };
            // 1118. - X: 270, Y: 1, Z: -90
            yield return new object[]
            {
                "X: 270, Y: 1, Z: -90",
                new float3(4.712389f, 0.01745329f, -1.570796f),
                new Quaternion(0.5043442f, 0.4956177f, 0.4956177f, -0.5043442f),
                new float4x4(0.01745254f, 0.9998475f, 0f, 0f, 0f, 2.086163E-07f, 0.9999998f, 0f, 0.9998475f, -0.01745233f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.729842f, 0f)
            };
            // 1119. - X: 360, Y: 1, Z: -90
            yield return new object[]
            {
                "X: 360, Y: 1, Z: -90",
                new float3(6.283185f, 0.01745329f, -1.570796f),
                new Quaternion(0.00617053f, -0.006170654f, 0.7070798f, -0.7070798f),
                new float4x4(8.039206E-08f, 0.9998476f, 0.01745241f, 0f, -0.9999999f, 8.344796E-08f, -1.750886E-07f, 0f, -1.750886E-07f, -0.01745241f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.750886E-07f, 0.01745329f, 4.712389f)
            };
            // 1120. - X: 585, Y: 1, Z: -90
            yield return new object[]
            {
                "X: 585, Y: 1, Z: -90",
                new float3(10.21018f, 0.01745329f, -1.570796f),
                new Quaternion(-0.6556179f, -0.6508952f, -0.264887f, 0.2762887f),
                new float4x4(0.01234069f, 0.9998477f, -0.01234072f, 0f, 0.7071065f, 1.490116E-07f, 0.707107f, 0f, 0.7069993f, -0.01745236f, -0.7069987f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.159046f, 1.570796f)
            };
            // 1121. - X: -90, Y: 1, Z: -90
            yield return new object[]
            {
                "X: -90, Y: 1, Z: -90",
                new float3(-1.570796f, 0.01745329f, -1.570796f),
                new Quaternion(-0.5043442f, -0.4956177f, -0.4956177f, 0.5043442f),
                new float4x4(0.01745254f, 0.9998475f, 0f, 0f, 0f, 2.086163E-07f, 0.9999998f, 0f, 0.9998475f, -0.01745233f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.729842f, 0f)
            };
            // 1122. - X: -540, Y: 1, Z: -90
            yield return new object[]
            {
                "X: -540, Y: 1, Z: -90",
                new float3(-9.424778f, 0.01745329f, -1.570796f),
                new Quaternion(0.7070798f, 0.7070798f, -0.006170601f, -0.006170584f),
                new float4x4(8.171628E-08f, 0.9998476f, -0.0174524f, 0f, 0.9999999f, 8.171628E-08f, -2.328306E-08f, 0f, -2.328306E-08f, -0.0174524f, -0.9998475f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.328306E-08f, 3.159046f, 1.570796f)
            };
            // 1123. - X: 0, Y: 45, Z: -90
            yield return new object[]
            {
                "X: 0, Y: 45, Z: -90",
                new float3(0f, 0.7853982f, -1.570796f),
                new Quaternion(-0.2705981f, 0.2705981f, -0.6532815f, 0.6532815f),
                new float4x4(8.940697E-08f, 0.7071067f, 0.7071068f, 0f, -0.9999999f, 8.940697E-08f, 0f, 0f, 0f, -0.7071068f, 0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0.7853982f, 4.712389f)
            };
            // 1124. - X: 0.1, Y: 45, Z: -90
            yield return new object[]
            {
                "X: 0.1, Y: 45, Z: -90",
                new float3(0.001745329f, 0.7853982f, -1.570796f),
                new Quaternion(-0.2700279f, 0.2711681f, -0.6535174f, 0.6530451f),
                new float4x4(-0.001234129f, 0.7071067f, 0.7071058f, 0f, -0.9999985f, -1.490116E-08f, -0.001745313f, 0f, -0.001234084f, -0.7071068f, 0.7071056f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745344f, 0.7853982f, 4.712389f)
            };
            // 1125. - X: 1, Y: 45, Z: -90
            yield return new object[]
            {
                "X: 1, Y: 45, Z: -90",
                new float3(0.01745329f, 0.7853982f, -1.570796f),
                new Quaternion(-0.2648869f, 0.2762886f, -0.655618f, 0.6508952f),
                new float4x4(-0.01234062f, 0.7071067f, 0.7069991f, 0f, -0.9998477f, 1.043081E-07f, -0.01745239f, 0f, -0.01234072f, -0.7071067f, 0.7069991f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745328f, 0.7853982f, 4.712389f)
            };
            // 1126. - X: 45, Y: 45, Z: -90
            yield return new object[]
            {
                "X: 45, Y: 45, Z: -90",
                new float3(0.7853982f, 0.7853982f, -1.570796f),
                new Quaternion(0f, 0.5f, -0.7071068f, 0.4999999f),
                new float4x4(-0.4999999f, 0.7071067f, 0.4999999f, 0f, -0.7071067f, 5.960464E-08f, -0.7071068f, 0f, -0.4999999f, -0.7071068f, 0.5f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 0.7853982f, 4.712389f)
            };
            // 1127. - X: 90, Y: 45, Z: -90
            yield return new object[]
            {
                "X: 90, Y: 45, Z: -90",
                new float3(1.570796f, 0.7853982f, -1.570796f),
                new Quaternion(0.270598f, 0.6532815f, -0.6532815f, 0.270598f),
                new float4x4(-0.7071066f, 0.7071066f, 0f, 0f, 0f, 1.490116E-07f, -0.9999999f, 0f, -0.7071066f, -0.7071067f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 2.356194f, 0f)
            };
            // 1128. - X: 180, Y: 45, Z: -90
            yield return new object[]
            {
                "X: 180, Y: 45, Z: -90",
                new float3(3.141593f, 0.7853982f, -1.570796f),
                new Quaternion(0.6532815f, 0.6532815f, -0.270598f, -0.2705981f),
                new float4x4(1.192093E-07f, 0.7071067f, -0.7071067f, 0f, 0.9999999f, 1.192093E-07f, 8.940697E-08f, 0f, 8.940697E-08f, -0.7071067f, -0.7071066f, 0f, 0f, 0f, 0f, 1f),
                new float3(-5.960464E-08f, 3.926991f, 1.570796f)
            };
            // 1129. - X: 270, Y: 45, Z: -90
            yield return new object[]
            {
                "X: 270, Y: 45, Z: -90",
                new float3(4.712389f, 0.7853982f, -1.570796f),
                new Quaternion(0.6532815f, 0.270598f, 0.270598f, -0.6532815f),
                new float4x4(0.7071069f, 0.7071066f, 0f, 0f, 0f, 1.490116E-07f, 0.9999999f, 0f, 0.7071066f, -0.7071067f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 5.497787f, 0f)
            };
            // 1130. - X: 360, Y: 45, Z: -90
            yield return new object[]
            {
                "X: 360, Y: 45, Z: -90",
                new float3(6.283185f, 0.7853982f, -1.570796f),
                new Quaternion(0.270598f, -0.2705981f, 0.6532815f, -0.6532815f),
                new float4x4(2.980232E-08f, 0.7071067f, 0.7071067f, 0f, -0.9999999f, 1.490116E-07f, -1.490116E-07f, 0f, -1.490116E-07f, -0.7071067f, 0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.490116E-07f, 0.7853982f, 4.712389f)
            };
            // 1131. - X: 585, Y: 45, Z: -90
            yield return new object[]
            {
                "X: 585, Y: 45, Z: -90",
                new float3(10.21018f, 0.7853982f, -1.570796f),
                new Quaternion(-0.7071067f, -0.4999999f, -1.043081E-07f, 0.5000001f),
                new float4x4(0.5000003f, 0.7071066f, -0.4999998f, 0f, 0.7071064f, 1.788139E-07f, 0.7071069f, 0f, 0.5000001f, -0.7071066f, -0.4999995f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.926991f, 1.570796f)
            };
            // 1132. - X: -90, Y: 45, Z: -90
            yield return new object[]
            {
                "X: -90, Y: 45, Z: -90",
                new float3(-1.570796f, 0.7853982f, -1.570796f),
                new Quaternion(-0.6532815f, -0.270598f, -0.270598f, 0.6532815f),
                new float4x4(0.7071069f, 0.7071066f, 0f, 0f, 0f, 1.490116E-07f, 0.9999999f, 0f, 0.7071066f, -0.7071067f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 5.497787f, 0f)
            };
            // 1133. - X: -540, Y: 45, Z: -90
            yield return new object[]
            {
                "X: -540, Y: 45, Z: -90",
                new float3(-9.424778f, 0.7853982f, -1.570796f),
                new Quaternion(0.6532815f, 0.6532815f, -0.2705981f, -0.2705981f),
                new float4x4(8.940697E-08f, 0.7071067f, -0.7071068f, 0f, 0.9999999f, 8.940697E-08f, 0f, 0f, 0f, -0.7071068f, -0.7071066f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.926991f, 1.570796f)
            };
            // 1134. - X: 0, Y: 90, Z: -90
            yield return new object[]
            {
                "X: 0, Y: 90, Z: -90",
                new float3(0f, 1.570796f, -1.570796f),
                new Quaternion(-0.5f, 0.5f, -0.5f, 0.5f),
                new float4x4(1.192093E-07f, 0f, 0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 0f, 0f, 0f, -0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.570796f, 4.712389f)
            };
            // 1135. - X: 0.1, Y: 90, Z: -90
            yield return new object[]
            {
                "X: 0.1, Y: 90, Z: -90",
                new float3(0.001745329f, 1.570796f, -1.570796f),
                new Quaternion(-0.4995635f, 0.5004361f, -0.5004361f, 0.4995635f),
                new float4x4(-0.001745224f, 0f, 0.9999985f, 0f, -0.9999985f, 2.980232E-08f, -0.001745254f, 0f, 0f, -1f, 2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745255f, 1.570796f, 4.712389f)
            };
            // 1136. - X: 1, Y: 90, Z: -90
            yield return new object[]
            {
                "X: 1, Y: 90, Z: -90",
                new float3(0.01745329f, 1.570796f, -1.570796f),
                new Quaternion(-0.4956177f, 0.5043442f, -0.5043442f, 0.4956177f),
                new float4x4(-0.01745212f, 0f, 0.9998475f, 0f, -0.9998475f, 2.086163E-07f, -0.01745233f, 0f, 0f, -0.9999998f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745322f, 1.570796f, 4.712389f)
            };
            // 1137. - X: 45, Y: 90, Z: -90
            yield return new object[]
            {
                "X: 45, Y: 90, Z: -90",
                new float3(0.7853982f, 1.570796f, -1.570796f),
                new Quaternion(-0.270598f, 0.6532815f, -0.6532815f, 0.270598f),
                new float4x4(-0.7071066f, 0f, 0.7071066f, 0f, -0.7071066f, 1.490116E-07f, -0.7071067f, 0f, 0f, -0.9999999f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 1.570796f, 4.712389f)
            };
            // 1138. - X: 90, Y: 90, Z: -90
            yield return new object[]
            {
                "X: 90, Y: 90, Z: -90",
                new float3(1.570796f, 1.570796f, -1.570796f),
                new Quaternion(0f, 0.7071067f, -0.7071067f, 0f),
                new float4x4(-0.9999996f, 0f, 0f, 0f, 0f, 1.788139E-07f, -0.9999998f, 0f, 0f, -0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570451f, 3.141593f, 0f)
            };
            // 1139. - X: 180, Y: 90, Z: -90
            yield return new object[]
            {
                "X: 180, Y: 90, Z: -90",
                new float3(3.141593f, 1.570796f, -1.570796f),
                new Quaternion(0.5f, 0.4999999f, -0.4999999f, -0.5f),
                new float4x4(2.384186E-07f, 0f, -0.9999999f, 0f, 0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, -0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(-1.788139E-07f, 4.712389f, 1.570796f)
            };
            // 1140. - X: 270, Y: 90, Z: -90
            yield return new object[]
            {
                "X: 270, Y: 90, Z: -90",
                new float3(4.712389f, 1.570796f, -1.570796f),
                new Quaternion(0.7071067f, 0f, 0f, -0.7071067f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 1.788139E-07f, 0.9999998f, 0f, 0f, -0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 0f, 0f)
            };
            // 1141. - X: 360, Y: 90, Z: -90
            yield return new object[]
            {
                "X: 360, Y: 90, Z: -90",
                new float3(6.283185f, 1.570796f, -1.570796f),
                new Quaternion(0.4999999f, -0.5f, 0.5f, -0.4999999f),
                new float4x4(0f, 0f, 0.9999999f, 0f, -0.9999999f, 1.192093E-07f, -1.192093E-07f, 0f, 0f, -0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.788139E-07f, 1.570796f, 4.712389f)
            };
            // 1142. - X: 585, Y: 90, Z: -90
            yield return new object[]
            {
                "X: 585, Y: 90, Z: -90",
                new float3(10.21018f, 1.570796f, -1.570796f),
                new Quaternion(-0.6532815f, -0.2705979f, 0.2705979f, 0.6532815f),
                new float4x4(0.7071072f, 0f, -0.7071064f, 0f, 0.7071064f, 1.043081E-07f, 0.7071071f, 0f, 0f, -0.9999999f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 4.712389f, 1.570796f)
            };
            // 1143. - X: -90, Y: 90, Z: -90
            yield return new object[]
            {
                "X: -90, Y: 90, Z: -90",
                new float3(-1.570796f, 1.570796f, -1.570796f),
                new Quaternion(-0.7071067f, 0f, 0f, 0.7071067f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 1.788139E-07f, 0.9999998f, 0f, 0f, -0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 0f, 0f)
            };
            // 1144. - X: -540, Y: 90, Z: -90
            yield return new object[]
            {
                "X: -540, Y: 90, Z: -90",
                new float3(-9.424778f, 1.570796f, -1.570796f),
                new Quaternion(0.5f, 0.5f, -0.5f, -0.5f),
                new float4x4(1.192093E-07f, 0f, -0.9999999f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, -0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 1.570796f)
            };
            // 1145. - X: 0, Y: 180, Z: -90
            yield return new object[]
            {
                "X: 0, Y: 180, Z: -90",
                new float3(0f, 3.141593f, -1.570796f),
                new Quaternion(-0.7071068f, 0.7071068f, 3.090862E-08f, -3.090862E-08f),
                new float4x4(5.960464E-08f, -0.9999999f, -8.742278E-08f, 0f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 8.742278E-08f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.141593f, 4.712389f)
            };
            // 1146. - X: 0.1, Y: 180, Z: -90
            yield return new object[]
            {
                "X: 0.1, Y: 180, Z: -90",
                new float3(0.001745329f, 3.141593f, -1.570796f),
                new Quaternion(-0.7071065f, 0.7071065f, -0.0006170361f, -0.0006170979f),
                new float4x4(-4.621131E-08f, -1f, -8.73697E-08f, 0f, -0.9999985f, -4.621131E-08f, -0.001745328f, 0f, 0.001745328f, 8.73697E-08f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.141593f, 4.712389f)
            };
            // 1147. - X: 1, Y: 180, Z: -90
            yield return new object[]
            {
                "X: 1, Y: 180, Z: -90",
                new float3(0.01745329f, 3.141593f, -1.570796f),
                new Quaternion(-0.7070798f, 0.7070798f, -0.006170562f, -0.006170623f),
                new float4x4(8.268398E-08f, -0.9999999f, -8.6613E-08f, 0f, -0.9998476f, 8.268398E-08f, -0.0174524f, 0f, 0.0174524f, 8.6613E-08f, -0.9998475f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.141593f, 4.712389f)
            };
            // 1148. - X: 45, Y: 180, Z: -90
            yield return new object[]
            {
                "X: 45, Y: 180, Z: -90",
                new float3(0.7853982f, 3.141593f, -1.570796f),
                new Quaternion(-0.6532815f, 0.6532815f, -0.270598f, -0.2705981f),
                new float4x4(1.192093E-07f, -0.9999999f, -8.940697E-08f, 0f, -0.7071067f, 1.192093E-07f, -0.7071067f, 0f, 0.7071067f, 8.940697E-08f, -0.7071066f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 3.141593f, 4.712389f)
            };
            // 1149. - X: 90, Y: 180, Z: -90
            yield return new object[]
            {
                "X: 90, Y: 180, Z: -90",
                new float3(1.570796f, 3.141593f, -1.570796f),
                new Quaternion(-0.5f, 0.4999999f, -0.4999999f, -0.5f),
                new float4x4(2.384186E-07f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, 0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 4.712389f, 0f)
            };
            // 1150. - X: 180, Y: 180, Z: -90
            yield return new object[]
            {
                "X: 180, Y: 180, Z: -90",
                new float3(3.141593f, 3.141593f, -1.570796f),
                new Quaternion(0f, -6.181724E-08f, -0.7071068f, -0.7071068f),
                new float4x4(5.960464E-08f, -0.9999999f, 8.742278E-08f, 0f, 0.9999999f, 5.960464E-08f, 8.742278E-08f, 0f, -8.742278E-08f, 8.742278E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742279E-08f, 8.742278E-08f, 1.570796f)
            };
            // 1151. - X: 270, Y: 180, Z: -90
            yield return new object[]
            {
                "X: 270, Y: 180, Z: -90",
                new float3(4.712389f, 3.141593f, -1.570796f),
                new Quaternion(0.4999999f, -0.5f, -0.5f, -0.4999999f),
                new float4x4(0f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 1152. - X: 360, Y: 180, Z: -90
            yield return new object[]
            {
                "X: 360, Y: 180, Z: -90",
                new float3(6.283185f, 3.141593f, -1.570796f),
                new Quaternion(0.7071068f, -0.7071068f, 3.090862E-08f, 9.272586E-08f),
                new float4x4(5.960464E-08f, -0.9999999f, -8.742277E-08f, 0f, -0.9999999f, 5.960464E-08f, -1.748456E-07f, 0f, 1.748456E-07f, 8.742277E-08f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 3.141593f, 4.712389f)
            };
            // 1153. - X: 585, Y: 180, Z: -90
            yield return new object[]
            {
                "X: 585, Y: 180, Z: -90",
                new float3(10.21018f, 3.141593f, -1.570796f),
                new Quaternion(-0.2705981f, 0.2705982f, 0.6532814f, 0.6532814f),
                new float4x4(4.470348E-08f, -0.9999999f, 5.960464E-08f, 0f, 0.7071064f, 1.192093E-07f, 0.707107f, 0f, -0.707107f, 5.960464E-08f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.264406E-07f, 1.570796f)
            };
            // 1154. - X: -90, Y: 180, Z: -90
            yield return new object[]
            {
                "X: -90, Y: 180, Z: -90",
                new float3(-1.570796f, 3.141593f, -1.570796f),
                new Quaternion(-0.4999999f, 0.5f, 0.5f, 0.4999999f),
                new float4x4(0f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 1155. - X: -540, Y: 180, Z: -90
            yield return new object[]
            {
                "X: -540, Y: 180, Z: -90",
                new float3(-9.424778f, 3.141593f, -1.570796f),
                new Quaternion(-3.934078E-08f, -2.247646E-08f, -0.7071068f, -0.7071068f),
                new float4x4(5.960464E-08f, -0.9999999f, 8.742278E-08f, 0f, 0.9999999f, 5.960464E-08f, -2.384976E-08f, 0f, 2.384976E-08f, 8.742278E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 8.742278E-08f, 1.570796f)
            };
            // 1156. - X: 0, Y: 270, Z: -90
            yield return new object[]
            {
                "X: 0, Y: 270, Z: -90",
                new float3(0f, 4.712389f, -1.570796f),
                new Quaternion(-0.5f, 0.5f, 0.5f, -0.5f),
                new float4x4(1.192093E-07f, 0f, -0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 4.712389f)
            };
            // 1157. - X: 0.1, Y: 270, Z: -90
            yield return new object[]
            {
                "X: 0.1, Y: 270, Z: -90",
                new float3(0.001745329f, 4.712389f, -1.570796f),
                new Quaternion(-0.5004361f, 0.4995635f, 0.4995635f, -0.5004361f),
                new float4x4(0.001745284f, 0f, -0.9999985f, 0f, -0.9999985f, 2.980232E-08f, -0.001745254f, 0f, 0f, 1f, 2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745255f, 4.712389f, 4.712389f)
            };
            // 1158. - X: 1, Y: 270, Z: -90
            yield return new object[]
            {
                "X: 1, Y: 270, Z: -90",
                new float3(0.01745329f, 4.712389f, -1.570796f),
                new Quaternion(-0.5043442f, 0.4956177f, 0.4956177f, -0.5043442f),
                new float4x4(0.01745254f, 0f, -0.9998475f, 0f, -0.9998475f, 2.086163E-07f, -0.01745233f, 0f, 0f, 0.9999998f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745322f, 4.712389f, 4.712389f)
            };
            // 1159. - X: 45, Y: 270, Z: -90
            yield return new object[]
            {
                "X: 45, Y: 270, Z: -90",
                new float3(0.7853982f, 4.712389f, -1.570796f),
                new Quaternion(-0.6532815f, 0.270598f, 0.270598f, -0.6532815f),
                new float4x4(0.7071069f, 0f, -0.7071066f, 0f, -0.7071066f, 1.490116E-07f, -0.7071067f, 0f, 0f, 0.9999999f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 4.712389f, 4.712389f)
            };
            // 1160. - X: 90, Y: 270, Z: -90
            yield return new object[]
            {
                "X: 90, Y: 270, Z: -90",
                new float3(1.570796f, 4.712389f, -1.570796f),
                new Quaternion(-0.7071067f, 0f, 0f, -0.7071067f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 1.788139E-07f, -0.9999998f, 0f, 0f, 0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570451f, 0f, 0f)
            };
            // 1161. - X: 180, Y: 270, Z: -90
            yield return new object[]
            {
                "X: 180, Y: 270, Z: -90",
                new float3(3.141593f, 4.712389f, -1.570796f),
                new Quaternion(-0.4999999f, -0.5f, -0.5f, -0.4999999f),
                new float4x4(0f, 0f, 0.9999999f, 0f, 0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(-1.788139E-07f, 1.570796f, 1.570796f)
            };
            // 1162. - X: 270, Y: 270, Z: -90
            yield return new object[]
            {
                "X: 270, Y: 270, Z: -90",
                new float3(4.712389f, 4.712389f, -1.570796f),
                new Quaternion(0f, -0.7071067f, -0.7071067f, 0f),
                new float4x4(-0.9999996f, 0f, 0f, 0f, 0f, 1.788139E-07f, 0.9999998f, 0f, 0f, 0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 3.141593f, 0f)
            };
            // 1163. - X: 360, Y: 270, Z: -90
            yield return new object[]
            {
                "X: 360, Y: 270, Z: -90",
                new float3(6.283185f, 4.712389f, -1.570796f),
                new Quaternion(0.5f, -0.4999999f, -0.4999999f, 0.5f),
                new float4x4(2.384186E-07f, 0f, -0.9999999f, 0f, -0.9999999f, 1.192093E-07f, -1.192093E-07f, 0f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.788139E-07f, 4.712389f, 4.712389f)
            };
            // 1164. - X: 585, Y: 270, Z: -90
            yield return new object[]
            {
                "X: 585, Y: 270, Z: -90",
                new float3(10.21018f, 4.712389f, -1.570796f),
                new Quaternion(0.2705979f, 0.6532815f, 0.6532815f, 0.2705979f),
                new float4x4(-0.7071069f, 0f, 0.7071064f, 0f, 0.7071064f, 1.043081E-07f, 0.7071071f, 0f, 0f, 0.9999999f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.570796f, 1.570796f)
            };
            // 1165. - X: -90, Y: 270, Z: -90
            yield return new object[]
            {
                "X: -90, Y: 270, Z: -90",
                new float3(-1.570796f, 4.712389f, -1.570796f),
                new Quaternion(0f, 0.7071067f, 0.7071067f, 0f),
                new float4x4(-0.9999996f, 0f, 0f, 0f, 0f, 1.788139E-07f, 0.9999998f, 0f, 0f, 0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 3.141593f, 0f)
            };
            // 1166. - X: -540, Y: 270, Z: -90
            yield return new object[]
            {
                "X: -540, Y: 270, Z: -90",
                new float3(-9.424778f, 4.712389f, -1.570796f),
                new Quaternion(-0.5f, -0.5f, -0.5f, -0.5f),
                new float4x4(1.192093E-07f, 0f, 0.9999999f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.570796f, 1.570796f)
            };
            // 1167. - X: 0, Y: 360, Z: -90
            yield return new object[]
            {
                "X: 0, Y: 360, Z: -90",
                new float3(0f, 6.283185f, -1.570796f),
                new Quaternion(6.181724E-08f, -6.181724E-08f, 0.7071068f, -0.7071068f),
                new float4x4(5.960464E-08f, 0.9999999f, 1.748456E-07f, 0f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, -1.748456E-07f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.748456E-07f, 4.712389f)
            };
            // 1168. - X: 0.1, Y: 360, Z: -90
            yield return new object[]
            {
                "X: 0.1, Y: 360, Z: -90",
                new float3(0.001745329f, 6.283185f, -1.570796f),
                new Quaternion(-0.0006170052f, -0.0006171288f, 0.7071065f, -0.7071065f),
                new float4x4(-4.644016E-08f, 1f, 1.747976E-07f, 0f, -0.9999985f, -4.613503E-08f, -0.001745328f, 0f, -0.001745328f, -1.747976E-07f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 1.747979E-07f, 4.712389f)
            };
            // 1169. - X: 1, Y: 360, Z: -90
            yield return new object[]
            {
                "X: 1, Y: 360, Z: -90",
                new float3(0.01745329f, 6.283185f, -1.570796f),
                new Quaternion(-0.00617053f, -0.006170654f, 0.7070798f, -0.7070798f),
                new float4x4(8.039206E-08f, 0.9999999f, 1.750886E-07f, 0f, -0.9998476f, 8.344796E-08f, -0.01745241f, 0f, -0.01745241f, -1.750886E-07f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 1.751153E-07f, 4.712389f)
            };
            // 1170. - X: 45, Y: 360, Z: -90
            yield return new object[]
            {
                "X: 45, Y: 360, Z: -90",
                new float3(0.7853982f, 6.283185f, -1.570796f),
                new Quaternion(-0.270598f, -0.2705981f, 0.6532815f, -0.6532815f),
                new float4x4(2.980232E-08f, 0.9999999f, 1.490116E-07f, 0f, -0.7071067f, 1.490116E-07f, -0.7071067f, 0f, -0.7071067f, -1.490116E-07f, 0.7071068f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853984f, 2.107342E-07f, 4.712389f)
            };
            // 1171. - X: 90, Y: 360, Z: -90
            yield return new object[]
            {
                "X: 90, Y: 360, Z: -90",
                new float3(1.570796f, 6.283185f, -1.570796f),
                new Quaternion(-0.4999999f, -0.5f, 0.5f, -0.4999999f),
                new float4x4(0f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, -0.9999999f, -1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 1.570796f, 0f)
            };
            // 1172. - X: 180, Y: 360, Z: -90
            yield return new object[]
            {
                "X: 180, Y: 360, Z: -90",
                new float3(3.141593f, 6.283185f, -1.570796f),
                new Quaternion(-0.7071068f, -0.7071068f, 3.090862E-08f, 9.272586E-08f),
                new float4x4(5.960464E-08f, 0.9999999f, -1.748456E-07f, 0f, 0.9999999f, 5.960464E-08f, 8.742277E-08f, 0f, 8.742277E-08f, -1.748456E-07f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742279E-08f, 3.141593f, 1.570796f)
            };
            // 1173. - X: 270, Y: 360, Z: -90
            yield return new object[]
            {
                "X: 270, Y: 360, Z: -90",
                new float3(4.712389f, 6.283185f, -1.570796f),
                new Quaternion(-0.5f, -0.4999999f, -0.4999999f, 0.5f),
                new float4x4(2.384186E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, 0.9999999f, -1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 1174. - X: 360, Y: 360, Z: -90
            yield return new object[]
            {
                "X: 360, Y: 360, Z: -90",
                new float3(6.283185f, 6.283185f, -1.570796f),
                new Quaternion(0f, 1.236345E-07f, -0.7071068f, 0.7071068f),
                new float4x4(5.960461E-08f, 0.9999999f, 1.748456E-07f, 0f, -0.9999999f, 5.960464E-08f, -1.748456E-07f, 0f, -1.748456E-07f, -1.748456E-07f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 1.748456E-07f, 4.712389f)
            };
            // 1175. - X: 585, Y: 360, Z: -90
            yield return new object[]
            {
                "X: 585, Y: 360, Z: -90",
                new float3(10.21018f, 6.283185f, -1.570796f),
                new Quaternion(0.6532814f, 0.6532814f, 0.2705981f, -0.2705982f),
                new float4x4(1.490116E-07f, 0.9999999f, -1.788139E-07f, 0f, 0.7071064f, 1.490116E-07f, 0.707107f, 0f, 0.707107f, -1.788139E-07f, -0.7071064f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.141593f, 1.570796f)
            };
            // 1176. - X: -90, Y: 360, Z: -90
            yield return new object[]
            {
                "X: -90, Y: 360, Z: -90",
                new float3(-1.570796f, 6.283185f, -1.570796f),
                new Quaternion(0.5f, 0.4999999f, 0.4999999f, -0.5f),
                new float4x4(2.384186E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, 0.9999999f, -1.192093E-07f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 1177. - X: -540, Y: 360, Z: -90
            yield return new object[]
            {
                "X: -540, Y: 360, Z: -90",
                new float3(-9.424778f, 6.283185f, -1.570796f),
                new Quaternion(-0.7071068f, -0.7071068f, 7.02494E-08f, 5.338508E-08f),
                new float4x4(5.960463E-08f, 0.9999999f, -1.748456E-07f, 0f, 0.9999999f, 5.960463E-08f, -2.384976E-08f, 0f, -2.384976E-08f, -1.748456E-07f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.141593f, 1.570796f)
            };
            // 1178. - X: 0, Y: 585, Z: -90
            yield return new object[]
            {
                "X: 0, Y: 585, Z: -90",
                new float3(0f, 10.21018f, -1.570796f),
                new Quaternion(0.6532814f, -0.6532814f, -0.2705982f, 0.2705982f),
                new float4x4(7.450581E-08f, -0.7071064f, -0.707107f, 0f, -0.9999999f, 7.450581E-08f, 0f, 0f, 0f, 0.707107f, -0.7071064f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.926991f, 4.712389f)
            };
            // 1179. - X: 0.1, Y: 585, Z: -90
            yield return new object[]
            {
                "X: 0.1, Y: 585, Z: -90",
                new float3(0.001745329f, 10.21018f, -1.570796f),
                new Quaternion(0.6535173f, -0.653045f, -0.270028f, 0.2711681f),
                new float4x4(0.001234248f, -0.7071065f, -0.7071059f, 0f, -0.9999983f, 7.450581E-08f, -0.001745313f, 0f, 0.001234084f, 0.7071069f, -0.7071053f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745314f, 3.926991f, 4.712389f)
            };
            // 1180. - X: 1, Y: 585, Z: -90
            yield return new object[]
            {
                "X: 1, Y: 585, Z: -90",
                new float3(0.01745329f, 10.21018f, -1.570796f),
                new Quaternion(0.6556179f, -0.6508952f, -0.264887f, 0.2762887f),
                new float4x4(0.01234069f, -0.7071065f, -0.7069993f, 0f, -0.9998477f, 1.490116E-07f, -0.01745236f, 0f, 0.01234072f, 0.707107f, -0.7069987f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745328f, 3.926991f, 4.712389f)
            };
            // 1181. - X: 45, Y: 585, Z: -90
            yield return new object[]
            {
                "X: 45, Y: 585, Z: -90",
                new float3(0.7853982f, 10.21018f, -1.570796f),
                new Quaternion(0.7071067f, -0.4999999f, -1.043081E-07f, 0.5000001f),
                new float4x4(0.5000003f, -0.7071064f, -0.5000001f, 0f, -0.7071066f, 1.788139E-07f, -0.7071066f, 0f, 0.4999998f, 0.7071069f, -0.4999995f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 3.926991f, 4.712389f)
            };
            // 1182. - X: 90, Y: 585, Z: -90
            yield return new object[]
            {
                "X: 90, Y: 585, Z: -90",
                new float3(1.570796f, 10.21018f, -1.570796f),
                new Quaternion(0.6532815f, -0.2705979f, 0.2705979f, 0.6532815f),
                new float4x4(0.7071072f, -0.7071064f, 0f, 0f, 0f, 1.043081E-07f, -0.9999999f, 0f, 0.7071064f, 0.7071071f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 5.497787f, 0f)
            };
            // 1183. - X: 180, Y: 585, Z: -90
            yield return new object[]
            {
                "X: 180, Y: 585, Z: -90",
                new float3(3.141593f, 10.21018f, -1.570796f),
                new Quaternion(0.2705981f, 0.2705982f, 0.6532814f, 0.6532814f),
                new float4x4(4.470348E-08f, -0.7071064f, 0.707107f, 0f, 0.9999999f, 1.192093E-07f, 5.960464E-08f, 0f, -5.960464E-08f, 0.707107f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.940697E-08f, 0.7853987f, 1.570796f)
            };
            // 1184. - X: 270, Y: 585, Z: -90
            yield return new object[]
            {
                "X: 270, Y: 585, Z: -90",
                new float3(4.712389f, 10.21018f, -1.570796f),
                new Quaternion(-0.2705979f, 0.6532815f, 0.6532815f, 0.2705979f),
                new float4x4(-0.7071069f, -0.7071064f, 0f, 0f, 0f, 1.043081E-07f, 0.9999999f, 0f, -0.7071064f, 0.7071071f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 2.356195f, 0f)
            };
            // 1185. - X: 360, Y: 585, Z: -90
            yield return new object[]
            {
                "X: 360, Y: 585, Z: -90",
                new float3(6.283185f, 10.21018f, -1.570796f),
                new Quaternion(-0.6532814f, 0.6532814f, 0.2705981f, -0.2705982f),
                new float4x4(1.490116E-07f, -0.7071064f, -0.707107f, 0f, -0.9999999f, 1.490116E-07f, -1.788139E-07f, 0f, 1.788139E-07f, 0.707107f, -0.7071064f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.490116E-07f, 3.926991f, 4.712389f)
            };
            // 1186. - X: 585, Y: 585, Z: -90
            yield return new object[]
            {
                "X: 585, Y: 585, Z: -90",
                new float3(10.21018f, 10.21018f, -1.570796f),
                new Quaternion(0f, -0.5000002f, -0.7071068f, -0.4999998f),
                new float4x4(-0.5000003f, -0.7071065f, 0.5f, 0f, 0.7071065f, 5.960464E-08f, 0.707107f, 0f, -0.5f, 0.707107f, 0.4999996f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 0.7853985f, 1.570796f)
            };
            // 1187. - X: -90, Y: 585, Z: -90
            yield return new object[]
            {
                "X: -90, Y: 585, Z: -90",
                new float3(-1.570796f, 10.21018f, -1.570796f),
                new Quaternion(0.2705979f, -0.6532815f, -0.6532815f, -0.2705979f),
                new float4x4(-0.7071069f, -0.7071064f, 0f, 0f, 0f, 1.043081E-07f, 0.9999999f, 0f, -0.7071064f, 0.7071071f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 2.356195f, 0f)
            };
            // 1188. - X: -540, Y: 585, Z: -90
            yield return new object[]
            {
                "X: -540, Y: 585, Z: -90",
                new float3(-9.424778f, 10.21018f, -1.570796f),
                new Quaternion(0.2705982f, 0.2705982f, 0.6532814f, 0.6532814f),
                new float4x4(7.450581E-08f, -0.7071064f, 0.707107f, 0f, 0.9999999f, 7.450581E-08f, 0f, 0f, 0f, 0.707107f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0.7853986f, 1.570796f)
            };
            // 1189. - X: 0, Y: -90, Z: -90
            yield return new object[]
            {
                "X: 0, Y: -90, Z: -90",
                new float3(0f, -1.570796f, -1.570796f),
                new Quaternion(0.5f, -0.5f, -0.5f, 0.5f),
                new float4x4(1.192093E-07f, 0f, -0.9999999f, 0f, -0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 4.712389f)
            };
            // 1190. - X: 0.1, Y: -90, Z: -90
            yield return new object[]
            {
                "X: 0.1, Y: -90, Z: -90",
                new float3(0.001745329f, -1.570796f, -1.570796f),
                new Quaternion(0.5004361f, -0.4995635f, -0.4995635f, 0.5004361f),
                new float4x4(0.001745284f, 0f, -0.9999985f, 0f, -0.9999985f, 2.980232E-08f, -0.001745254f, 0f, 0f, 1f, 2.980232E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745255f, 4.712389f, 4.712389f)
            };
            // 1191. - X: 1, Y: -90, Z: -90
            yield return new object[]
            {
                "X: 1, Y: -90, Z: -90",
                new float3(0.01745329f, -1.570796f, -1.570796f),
                new Quaternion(0.5043442f, -0.4956177f, -0.4956177f, 0.5043442f),
                new float4x4(0.01745254f, 0f, -0.9998475f, 0f, -0.9998475f, 2.086163E-07f, -0.01745233f, 0f, 0f, 0.9999998f, 2.086163E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745322f, 4.712389f, 4.712389f)
            };
            // 1192. - X: 45, Y: -90, Z: -90
            yield return new object[]
            {
                "X: 45, Y: -90, Z: -90",
                new float3(0.7853982f, -1.570796f, -1.570796f),
                new Quaternion(0.6532815f, -0.270598f, -0.270598f, 0.6532815f),
                new float4x4(0.7071069f, 0f, -0.7071066f, 0f, -0.7071066f, 1.490116E-07f, -0.7071067f, 0f, 0f, 0.9999999f, 1.490116E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 4.712389f, 4.712389f)
            };
            // 1193. - X: 90, Y: -90, Z: -90
            yield return new object[]
            {
                "X: 90, Y: -90, Z: -90",
                new float3(1.570796f, -1.570796f, -1.570796f),
                new Quaternion(0.7071067f, 0f, 0f, 0.7071067f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 1.788139E-07f, -0.9999998f, 0f, 0f, 0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570451f, 0f, 0f)
            };
            // 1194. - X: 180, Y: -90, Z: -90
            yield return new object[]
            {
                "X: 180, Y: -90, Z: -90",
                new float3(3.141593f, -1.570796f, -1.570796f),
                new Quaternion(0.4999999f, 0.5f, 0.5f, 0.4999999f),
                new float4x4(0f, 0f, 0.9999999f, 0f, 0.9999999f, 1.192093E-07f, 1.192093E-07f, 0f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(-1.788139E-07f, 1.570796f, 1.570796f)
            };
            // 1195. - X: 270, Y: -90, Z: -90
            yield return new object[]
            {
                "X: 270, Y: -90, Z: -90",
                new float3(4.712389f, -1.570796f, -1.570796f),
                new Quaternion(0f, 0.7071067f, 0.7071067f, 0f),
                new float4x4(-0.9999996f, 0f, 0f, 0f, 0f, 1.788139E-07f, 0.9999998f, 0f, 0f, 0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 3.141593f, 0f)
            };
            // 1196. - X: 360, Y: -90, Z: -90
            yield return new object[]
            {
                "X: 360, Y: -90, Z: -90",
                new float3(6.283185f, -1.570796f, -1.570796f),
                new Quaternion(-0.5f, 0.4999999f, 0.4999999f, -0.5f),
                new float4x4(2.384186E-07f, 0f, -0.9999999f, 0f, -0.9999999f, 1.192093E-07f, -1.192093E-07f, 0f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.788139E-07f, 4.712389f, 4.712389f)
            };
            // 1197. - X: 585, Y: -90, Z: -90
            yield return new object[]
            {
                "X: 585, Y: -90, Z: -90",
                new float3(10.21018f, -1.570796f, -1.570796f),
                new Quaternion(-0.2705979f, -0.6532815f, -0.6532815f, -0.2705979f),
                new float4x4(-0.7071069f, 0f, 0.7071064f, 0f, 0.7071064f, 1.043081E-07f, 0.7071071f, 0f, 0f, 0.9999999f, 1.043081E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.570796f, 1.570796f)
            };
            // 1198. - X: -90, Y: -90, Z: -90
            yield return new object[]
            {
                "X: -90, Y: -90, Z: -90",
                new float3(-1.570796f, -1.570796f, -1.570796f),
                new Quaternion(0f, -0.7071067f, -0.7071067f, 0f),
                new float4x4(-0.9999996f, 0f, 0f, 0f, 0f, 1.788139E-07f, 0.9999998f, 0f, 0f, 0.9999998f, 1.788139E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712734f, 3.141593f, 0f)
            };
            // 1199. - X: -540, Y: -90, Z: -90
            yield return new object[]
            {
                "X: -540, Y: -90, Z: -90",
                new float3(-9.424778f, -1.570796f, -1.570796f),
                new Quaternion(0.5f, 0.5f, 0.5f, 0.5f),
                new float4x4(1.192093E-07f, 0f, 0.9999999f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0.9999999f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.570796f, 1.570796f)
            };
            // 1200. - X: 0, Y: -540, Z: -90
            yield return new object[]
            {
                "X: 0, Y: -540, Z: -90",
                new float3(0f, -9.424778f, -1.570796f),
                new Quaternion(-0.7071068f, 0.7071068f, -8.432163E-09f, 8.432163E-09f),
                new float4x4(5.960464E-08f, -0.9999999f, 2.384976E-08f, 0f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, -2.384976E-08f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.141593f, 4.712389f)
            };
            // 1201. - X: 0.1, Y: -540, Z: -90
            yield return new object[]
            {
                "X: 0.1, Y: -540, Z: -90",
                new float3(0.001745329f, -9.424778f, -1.570796f),
                new Quaternion(-0.7071065f, 0.7071065f, -0.0006170754f, -0.0006170585f),
                new float4x4(-4.63084E-08f, -1f, 2.392335E-08f, 0f, -0.9999985f, -4.63084E-08f, -0.001745328f, 0f, 0.001745328f, -2.392335E-08f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.141593f, 4.712389f)
            };
            // 1202. - X: 1, Y: -540, Z: -90
            yield return new object[]
            {
                "X: 1, Y: -540, Z: -90",
                new float3(0.01745329f, -9.424778f, -1.570796f),
                new Quaternion(-0.7070798f, 0.7070798f, -0.006170601f, -0.006170584f),
                new float4x4(8.171628E-08f, -0.9999999f, 2.328306E-08f, 0f, -0.9998476f, 8.171628E-08f, -0.0174524f, 0f, 0.0174524f, -2.328306E-08f, -0.9998475f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.141593f, 4.712389f)
            };
            // 1203. - X: 45, Y: -540, Z: -90
            yield return new object[]
            {
                "X: 45, Y: -540, Z: -90",
                new float3(0.7853982f, -9.424778f, -1.570796f),
                new Quaternion(-0.6532815f, 0.6532815f, -0.2705981f, -0.2705981f),
                new float4x4(8.940697E-08f, -0.9999999f, 0f, 0f, -0.7071067f, 8.940697E-08f, -0.7071068f, 0f, 0.7071068f, 0f, -0.7071066f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 3.141593f, 4.712389f)
            };
            // 1204. - X: 90, Y: -540, Z: -90
            yield return new object[]
            {
                "X: 90, Y: -540, Z: -90",
                new float3(1.570796f, -9.424778f, -1.570796f),
                new Quaternion(-0.5f, 0.5f, -0.5f, -0.5f),
                new float4x4(1.192093E-07f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, 0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 4.712389f, 0f)
            };
            // 1205. - X: 180, Y: -540, Z: -90
            yield return new object[]
            {
                "X: 180, Y: -540, Z: -90",
                new float3(3.141593f, -9.424778f, -1.570796f),
                new Quaternion(3.934078E-08f, -2.247646E-08f, -0.7071068f, -0.7071068f),
                new float4x4(5.960464E-08f, -0.9999999f, -2.384976E-08f, 0f, 0.9999999f, 5.960464E-08f, 8.742278E-08f, 0f, -8.742278E-08f, -2.384976E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742279E-08f, -2.384976E-08f, 1.570796f)
            };
            // 1206. - X: 270, Y: -540, Z: -90
            yield return new object[]
            {
                "X: 270, Y: -540, Z: -90",
                new float3(4.712389f, -9.424778f, -1.570796f),
                new Quaternion(0.5f, -0.5f, -0.5f, -0.5f),
                new float4x4(1.192093E-07f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 1207. - X: 360, Y: -540, Z: -90
            yield return new object[]
            {
                "X: 360, Y: -540, Z: -90",
                new float3(6.283185f, -9.424778f, -1.570796f),
                new Quaternion(0.7071068f, -0.7071068f, 7.02494E-08f, 5.338508E-08f),
                new float4x4(5.960463E-08f, -0.9999999f, 2.384976E-08f, 0f, -0.9999999f, 5.960463E-08f, -1.748456E-07f, 0f, 1.748456E-07f, -2.384976E-08f, -0.9999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 3.141593f, 4.712389f)
            };
            // 1208. - X: 585, Y: -540, Z: -90
            yield return new object[]
            {
                "X: 585, Y: -540, Z: -90",
                new float3(10.21018f, -9.424778f, -1.570796f),
                new Quaternion(-0.2705982f, 0.2705982f, 0.6532814f, 0.6532814f),
                new float4x4(7.450581E-08f, -0.9999999f, 0f, 0f, 0.7071064f, 7.450581E-08f, 0.707107f, 0f, -0.707107f, 0f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 0f, 1.570796f)
            };
            // 1209. - X: -90, Y: -540, Z: -90
            yield return new object[]
            {
                "X: -90, Y: -540, Z: -90",
                new float3(-1.570796f, -9.424778f, -1.570796f),
                new Quaternion(-0.5f, 0.5f, 0.5f, 0.5f),
                new float4x4(1.192093E-07f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 1210. - X: -540, Y: -540, Z: -90
            yield return new object[]
            {
                "X: -540, Y: -540, Z: -90",
                new float3(-9.424778f, -9.424778f, -1.570796f),
                new Quaternion(0f, 1.686433E-08f, -0.7071068f, -0.7071068f),
                new float4x4(5.960464E-08f, -0.9999999f, -2.384976E-08f, 0f, 0.9999999f, 5.960464E-08f, -2.384976E-08f, 0f, 2.384976E-08f, -2.384976E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, -2.384976E-08f, 1.570796f)
            };
            // 1211. - X: 0, Y: 0, Z: -540
            yield return new object[]
            {
                "X: 0, Y: 0, Z: -540",
                new float3(0f, 0f, -9.424778f),
                new Quaternion(0f, 0f, 1f, 1.192488E-08f),
                new float4x4(-1f, -2.384976E-08f, 0f, 0f, 2.384976E-08f, -1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0f, 3.141593f)
            };
            // 1212. - X: 0.1, Y: 0, Z: -540
            yield return new object[]
            {
                "X: 0.1, Y: 0, Z: -540",
                new float3(0.001745329f, 0f, -9.424778f),
                new Quaternion(1.040642E-11f, -0.0008726645f, 0.9999996f, 1.192488E-08f),
                new float4x4(-1f, -2.384976E-08f, 0f, 0f, 2.384973E-08f, -0.9999986f, -0.001745328f, 0f, 4.162566E-11f, -0.001745328f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0f, 3.141593f)
            };
            // 1213. - X: 1, Y: 0, Z: -540
            yield return new object[]
            {
                "X: 1, Y: 0, Z: -540",
                new float3(0.01745329f, 0f, -9.424778f),
                new Quaternion(1.040629E-10f, -0.008726535f, 0.9999619f, 1.192443E-08f),
                new float4x4(-0.9999999f, -2.384976E-08f, -1.387779E-17f, 0f, 2.384613E-08f, -0.9998477f, -0.01745241f, 0f, 4.162357E-10f, -0.01745241f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, -1.38799E-17f, 3.141593f)
            };
            // 1214. - X: 45, Y: 0, Z: -540
            yield return new object[]
            {
                "X: 45, Y: 0, Z: -540",
                new float3(0.7853982f, 0f, -9.424778f),
                new Quaternion(4.563455E-09f, -0.3826835f, 0.9238795f, 1.101715E-08f),
                new float4x4(-1f, -2.384976E-08f, 0f, 0f, 1.686433E-08f, -0.7071067f, -0.7071068f, 0f, 1.686433E-08f, -0.7071068f, 0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 0f, 3.141593f)
            };
            // 1215. - X: 90, Y: 0, Z: -540
            yield return new object[]
            {
                "X: 90, Y: 0, Z: -540",
                new float3(1.570796f, 0f, -9.424778f),
                new Quaternion(8.432163E-09f, -0.7071068f, 0.7071068f, 8.432163E-09f),
                new float4x4(-0.9999999f, -2.384976E-08f, 0f, 0f, 0f, 5.960464E-08f, -0.9999999f, 0f, 2.384976E-08f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 3.141593f, 0f)
            };
            // 1216. - X: 180, Y: 0, Z: -540
            yield return new object[]
            {
                "X: 180, Y: 0, Z: -540",
                new float3(3.141593f, 0f, -9.424778f),
                new Quaternion(1.192488E-08f, -1f, -4.371139E-08f, -5.212531E-16f),
                new float4x4(-1f, -2.384976E-08f, 0f, 0f, -2.384976E-08f, 1f, 8.742278E-08f, 0f, -2.085012E-15f, 8.742278E-08f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 3.141593f, -2.384976E-08f)
            };
            // 1217. - X: 270, Y: 0, Z: -540
            yield return new object[]
            {
                "X: 270, Y: 0, Z: -540",
                new float3(4.712389f, 0f, -9.424778f),
                new Quaternion(8.432163E-09f, -0.7071068f, -0.7071068f, -8.432163E-09f),
                new float4x4(-0.9999999f, -2.384976E-08f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, -2.384976E-08f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.141593f, 0f)
            };
            // 1218. - X: 360, Y: 0, Z: -540
            yield return new object[]
            {
                "X: 360, Y: 0, Z: -540",
                new float3(6.283185f, 0f, -9.424778f),
                new Quaternion(-1.042506E-15f, 8.742278E-08f, -1f, -1.192488E-08f),
                new float4x4(-1f, -2.384976E-08f, 0f, 0f, 2.384976E-08f, -1f, -1.748456E-07f, 0f, 4.170025E-15f, -1.748456E-07f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 0f, 3.141593f)
            };
            // 1219. - X: 585, Y: 0, Z: -540
            yield return new object[]
            {
                "X: 585, Y: 0, Z: -540",
                new float3(10.21018f, 0f, -9.424778f),
                new Quaternion(-1.101715E-08f, 0.9238794f, 0.3826836f, 4.563456E-09f),
                new float4x4(-0.9999999f, -2.384976E-08f, 0f, 0f, -1.686432E-08f, 0.7071065f, 0.707107f, 0f, -1.686433E-08f, 0.707107f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.141593f, -2.384976E-08f)
            };
            // 1220. - X: -90, Y: 0, Z: -540
            yield return new object[]
            {
                "X: -90, Y: 0, Z: -540",
                new float3(-1.570796f, 0f, -9.424778f),
                new Quaternion(-8.432163E-09f, 0.7071068f, 0.7071068f, 8.432163E-09f),
                new float4x4(-0.9999999f, -2.384976E-08f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, -2.384976E-08f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.141593f, 0f)
            };
            // 1221. - X: -540, Y: 0, Z: -540
            yield return new object[]
            {
                "X: -540, Y: 0, Z: -540",
                new float3(-9.424778f, 0f, -9.424778f),
                new Quaternion(1.192488E-08f, -1f, 1.192488E-08f, 1.422028E-16f),
                new float4x4(-1f, -2.384976E-08f, 0f, 0f, -2.384976E-08f, 1f, -2.384976E-08f, 0f, 5.688111E-16f, -2.384976E-08f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.141593f, -2.384976E-08f)
            };
            // 1222. - X: 0, Y: 0.1, Z: -540
            yield return new object[]
            {
                "X: 0, Y: 0.1, Z: -540",
                new float3(0f, 0.001745329f, -9.424778f),
                new Quaternion(0.0008726645f, 1.040642E-11f, 0.9999996f, 1.192488E-08f),
                new float4x4(-0.9999986f, -2.384973E-08f, 0.001745328f, 0f, 2.384976E-08f, -1f, 0f, 0f, 0.001745328f, 4.162566E-11f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0.001745329f, 3.141593f)
            };
            // 1223. - X: 0.1, Y: 0.1, Z: -540
            yield return new object[]
            {
                "X: 0.1, Y: 0.1, Z: -540",
                new float3(0.001745329f, 0.001745329f, -9.424778f),
                new Quaternion(0.0008726642f, -0.0008726642f, 0.9999993f, 7.734682E-07f),
                new float4x4(-0.9999987f, -3.070021E-06f, 0.001745326f, 0f, 2.384968E-08f, -0.9999987f, -0.001745329f, 0f, 0.001745329f, -0.001745326f, 0.999997f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0.001745329f, 3.141593f)
            };
            // 1224. - X: 1, Y: 0.1, Z: -540
            yield return new object[]
            {
                "X: 1, Y: 0.1, Z: -540",
                new float3(0.01745329f, 0.001745329f, -9.424778f),
                new Quaternion(0.0008726314f, -0.008726533f, 0.9999616f, 7.627262E-06f),
                new float4x4(-0.9999985f, -3.048403E-05f, 0.001745063f, 0f, 2.384513E-08f, -0.9998478f, -0.01745241f, 0f, 0.001745329f, -0.01745238f, 0.9998462f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0.001745329f, 3.141593f)
            };
            // 1225. - X: 45, Y: 0.1, Z: -540
            yield return new object[]
            {
                "X: 45, Y: 0.1, Z: -540",
                new float3(0.7853982f, 0.001745329f, -9.424778f),
                new Quaternion(0.0008062414f, -0.3826833f, 0.9238791f, 0.0003339653f),
                new float4x4(-0.9999985f, -0.001234157f, 0.001234133f, 0f, 1.688022E-08f, -0.7071067f, -0.7071068f, 0f, 0.001745345f, -0.7071057f, 0.7071057f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 0.001745329f, 3.141593f)
            };
            // 1226. - X: 90, Y: 0.1, Z: -540
            yield return new object[]
            {
                "X: 90, Y: 0.1, Z: -540",
                new float3(1.570796f, 0.001745329f, -9.424778f),
                new Quaternion(0.0006170754f, -0.7071065f, 0.7071065f, 0.0006170754f),
                new float4x4(-0.9999986f, -0.001745352f, 0f, 0f, 0f, -4.63084E-08f, -1f, 0f, 0.001745352f, -0.9999985f, -4.63084E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 3.143338f, 0f)
            };
            // 1227. - X: 180, Y: 0.1, Z: -540
            yield return new object[]
            {
                "X: 180, Y: 0.1, Z: -540",
                new float3(3.141593f, 0.001745329f, -9.424778f),
                new Quaternion(1.188673E-08f, -0.9999996f, -4.372178E-08f, 0.0008726645f),
                new float4x4(-0.9999986f, -2.369714E-08f, -0.001745328f, 0f, -2.384976E-08f, 1f, 8.742278E-08f, 0f, 0.001745328f, 8.746428E-08f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 3.143338f, -2.384976E-08f)
            };
            // 1228. - X: 270, Y: 0.1, Z: -540
            yield return new object[]
            {
                "X: 270, Y: 0.1, Z: -540",
                new float3(4.712389f, 0.001745329f, -9.424778f),
                new Quaternion(-0.0006170585f, -0.7071065f, -0.7071065f, 0.0006170585f),
                new float4x4(-0.9999986f, 0.001745304f, 0f, 0f, 0f, -4.626673E-08f, 1f, 0f, 0.001745304f, 0.9999985f, -4.626673E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.143338f, 0f)
            };
            // 1229. - X: 360, Y: 0.1, Z: -540
            yield return new object[]
            {
                "X: 360, Y: 0.1, Z: -540",
                new float3(6.283185f, 0.001745329f, -9.424778f),
                new Quaternion(-0.0008726645f, 8.741234E-08f, -0.9999996f, -1.200117E-08f),
                new float4x4(-0.9999986f, -2.415489E-08f, 0.001745328f, 0f, 2.384976E-08f, -1f, -1.748456E-07f, 0f, 0.001745328f, -1.748037E-07f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 0.001745329f, 3.141593f)
            };
            // 1230. - X: 585, Y: 0.1, Z: -540
            yield return new object[]
            {
                "X: 585, Y: 0.1, Z: -540",
                new float3(10.21018f, 0.001745329f, -9.424778f),
                new Quaternion(0.0003339434f, 0.9238791f, 0.3826835f, -0.0008062323f),
                new float4x4(-0.9999985f, 0.00123411f, -0.001234133f, 0f, -1.688022E-08f, 0.7071065f, 0.707107f, 0f, 0.001745312f, 0.7071059f, -0.7071054f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.143338f, -2.387225E-08f)
            };
            // 1231. - X: -90, Y: 0.1, Z: -540
            yield return new object[]
            {
                "X: -90, Y: 0.1, Z: -540",
                new float3(-1.570796f, 0.001745329f, -9.424778f),
                new Quaternion(0.0006170585f, 0.7071065f, 0.7071065f, -0.0006170585f),
                new float4x4(-0.9999986f, 0.001745304f, 0f, 0f, 0f, -4.626673E-08f, 1f, 0f, 0.001745304f, 0.9999985f, -4.626673E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.143338f, 0f)
            };
            // 1232. - X: -540, Y: 0.1, Z: -540
            yield return new object[]
            {
                "X: -540, Y: 0.1, Z: -540",
                new float3(-9.424778f, 0.001745329f, -9.424778f),
                new Quaternion(1.193528E-08f, -0.9999996f, 1.191447E-08f, 0.0008726645f),
                new float4x4(-0.9999986f, -2.389135E-08f, -0.001745328f, 0f, -2.384976E-08f, 1f, -2.384976E-08f, 0f, 0.001745328f, -2.38081E-08f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.143338f, -2.384976E-08f)
            };
            // 1233. - X: 0, Y: 1, Z: -540
            yield return new object[]
            {
                "X: 0, Y: 1, Z: -540",
                new float3(0f, 0.01745329f, -9.424778f),
                new Quaternion(0.008726535f, 1.040629E-10f, 0.9999619f, 1.192443E-08f),
                new float4x4(-0.9998477f, -2.384613E-08f, 0.01745241f, 0f, 2.384976E-08f, -0.9999999f, -1.387779E-17f, 0f, 0.01745241f, 4.162357E-10f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.387779E-17f, 0.01745329f, 3.141593f)
            };
            // 1234. - X: 0.1, Y: 1, Z: -540
            yield return new object[]
            {
                "X: 0.1, Y: 1, Z: -540",
                new float3(0.001745329f, 0.01745329f, -9.424778f),
                new Quaternion(0.008726533f, -0.0008726311f, 0.9999616f, 7.627262E-06f),
                new float4x4(-0.9998478f, -3.048403E-05f, 0.01745238f, 0f, 2.384968E-08f, -0.9999985f, -0.001745328f, 0f, 0.01745241f, -0.001745062f, 0.9998462f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0.01745329f, 3.141593f)
            };
            // 1235. - X: 1, Y: 1, Z: -540
            yield return new object[]
            {
                "X: 1, Y: 1, Z: -540",
                new float3(0.01745329f, 0.01745329f, -9.424778f),
                new Quaternion(0.008726203f, -0.008726203f, 0.9999238f, 7.616435E-05f),
                new float4x4(-0.9998476f, -0.0003046103f, 0.01744975f, 0f, 2.386514E-08f, -0.9998476f, -0.01745241f, 0f, 0.01745241f, -0.01744975f, 0.9996954f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.0174533f, 0.01745329f, 3.141593f)
            };
            // 1236. - X: 45, Y: 1, Z: -540
            yield return new object[]
            {
                "X: 45, Y: 1, Z: -540",
                new float3(0.7853982f, 0.01745329f, -9.424778f),
                new Quaternion(0.008062271f, -0.3826689f, 0.9238443f, 0.003339512f),
                new float4x4(-0.9998477f, -0.01234074f, 0.01234071f, 0f, 1.676381E-08f, -0.7071068f, -0.7071068f, 0f, 0.01745242f, -0.7069991f, 0.7069991f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 0.01745329f, 3.141593f)
            };
            // 1237. - X: 90, Y: 1, Z: -540
            yield return new object[]
            {
                "X: 90, Y: 1, Z: -540",
                new float3(1.570796f, 0.01745329f, -9.424778f),
                new Quaternion(0.006170601f, -0.7070798f, 0.7070798f, 0.006170601f),
                new float4x4(-0.9998475f, -0.01745243f, 0f, 0f, 0f, 8.171628E-08f, -0.9999999f, 0f, 0.01745243f, -0.9998476f, 8.171628E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 3.159046f, 0f)
            };
            // 1238. - X: 180, Y: 1, Z: -540
            yield return new object[]
            {
                "X: 180, Y: 1, Z: -540",
                new float3(3.141593f, 0.01745329f, -9.424778f),
                new Quaternion(1.154298E-08f, -0.9999619f, -4.381378E-08f, 0.008726535f),
                new float4x4(-0.9998477f, -2.232039E-08f, -0.01745241f, 0f, -2.384976E-08f, 1f, 8.742277E-08f, 0f, 0.01745241f, 8.782569E-08f, -0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742277E-08f, 3.159046f, -2.384976E-08f)
            };
            // 1239. - X: 270, Y: 1, Z: -540
            yield return new object[]
            {
                "X: 270, Y: 1, Z: -540",
                new float3(4.712389f, 0.01745329f, -9.424778f),
                new Quaternion(-0.006170584f, -0.7070798f, -0.7070798f, 0.006170584f),
                new float4x4(-0.9998475f, 0.01745238f, 0f, 0f, 0f, 8.213101E-08f, 0.9999999f, 0f, 0.01745238f, 0.9998476f, 8.213101E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.159046f, 0f)
            };
            // 1240. - X: 360, Y: 1, Z: -540
            yield return new object[]
            {
                "X: 360, Y: 1, Z: -540",
                new float3(6.283185f, 0.01745329f, -9.424778f),
                new Quaternion(-0.008726535f, 8.731538E-08f, -0.9999619f, -1.268732E-08f),
                new float4x4(-0.9998477f, -2.689761E-08f, 0.01745241f, 0f, 2.384976E-08f, -0.9999999f, -1.748455E-07f, 0f, 0.01745241f, -1.744027E-07f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748455E-07f, 0.01745329f, 3.141593f)
            };
            // 1241. - X: 585, Y: 1, Z: -540
            yield return new object[]
            {
                "X: 585, Y: 1, Z: -540",
                new float3(10.21018f, 0.01745329f, -9.424778f),
                new Quaternion(0.003339491f, 0.9238443f, 0.382669f, -0.008062262f),
                new float4x4(-0.9998477f, 0.0123407f, -0.01234071f, 0f, -1.676381E-08f, 0.7071065f, 0.707107f, 0f, 0.01745239f, 0.7069994f, -0.7069988f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.159046f, -2.370761E-08f)
            };
            // 1242. - X: -90, Y: 1, Z: -540
            yield return new object[]
            {
                "X: -90, Y: 1, Z: -540",
                new float3(-1.570796f, 0.01745329f, -9.424778f),
                new Quaternion(0.006170584f, 0.7070798f, 0.7070798f, -0.006170584f),
                new float4x4(-0.9998475f, 0.01745238f, 0f, 0f, 0f, 8.213101E-08f, 0.9999999f, 0f, 0.01745238f, 0.9998476f, 8.213101E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.159046f, 0f)
            };
            // 1243. - X: -540, Y: 1, Z: -540
            yield return new object[]
            {
                "X: -540, Y: 1, Z: -540",
                new float3(-9.424778f, 0.01745329f, -9.424778f),
                new Quaternion(1.202849E-08f, -0.9999619f, 1.182036E-08f, 0.008726535f),
                new float4x4(-0.9998477f, -2.426236E-08f, -0.01745241f, 0f, -2.384976E-08f, 1f, -2.384976E-08f, 0f, 0.01745241f, -2.342989E-08f, -0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.159046f, -2.384976E-08f)
            };
            // 1244. - X: 0, Y: 45, Z: -540
            yield return new object[]
            {
                "X: 0, Y: 45, Z: -540",
                new float3(0f, 0.7853982f, -9.424778f),
                new Quaternion(0.3826835f, 4.563455E-09f, 0.9238795f, 1.101715E-08f),
                new float4x4(-0.7071067f, -1.686433E-08f, 0.7071068f, 0f, 2.384976E-08f, -1f, 0f, 0f, 0.7071068f, 1.686433E-08f, 0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 0.7853982f, 3.141593f)
            };
            // 1245. - X: 0.1, Y: 45, Z: -540
            yield return new object[]
            {
                "X: 0.1, Y: 45, Z: -540",
                new float3(0.001745329f, 0.7853982f, -9.424778f),
                new Quaternion(0.3826833f, -0.0008062323f, 0.9238791f, 0.0003339653f),
                new float4x4(-0.7071067f, -0.00123415f, 0.7071057f, 0f, 2.380693E-08f, -0.9999985f, -0.001745328f, 0f, 0.7071068f, -0.001234117f, 0.7071057f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 0.7853982f, 3.141593f)
            };
            // 1246. - X: 1, Y: 45, Z: -540
            yield return new object[]
            {
                "X: 1, Y: 45, Z: -540",
                new float3(0.01745329f, 0.7853982f, -9.424778f),
                new Quaternion(0.3826689f, -0.008062262f, 0.9238443f, 0.003339512f),
                new float4x4(-0.7071068f, -0.01234073f, 0.7069991f, 0f, 2.374873E-08f, -0.9998477f, -0.0174524f, 0f, 0.7071068f, -0.0123407f, 0.7069991f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 0.7853982f, 3.141593f)
            };
            // 1247. - X: 45, Y: 45, Z: -540
            yield return new object[]
            {
                "X: 45, Y: 45, Z: -540",
                new float3(0.7853982f, 0.7853982f, -9.424778f),
                new Quaternion(0.3535534f, -0.3535534f, 0.8535534f, 0.1464466f),
                new float4x4(-0.7071067f, -0.5000001f, 0.5f, 0f, 2.980232E-08f, -0.7071067f, -0.7071068f, 0f, 0.7071068f, -0.5f, 0.4999999f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 0.7853982f, 3.141593f)
            };
            // 1248. - X: 90, Y: 45, Z: -540
            yield return new object[]
            {
                "X: 90, Y: 45, Z: -540",
                new float3(1.570796f, 0.7853982f, -9.424778f),
                new Quaternion(0.2705981f, -0.6532815f, 0.6532815f, 0.2705981f),
                new float4x4(-0.7071066f, -0.7071068f, 0f, 0f, 0f, 8.940697E-08f, -0.9999999f, 0f, 0.7071068f, -0.7071067f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 3.926991f, 0f)
            };
            // 1249. - X: 180, Y: 45, Z: -540
            yield return new object[]
            {
                "X: 180, Y: 45, Z: -540",
                new float3(3.141593f, 0.7853982f, -9.424778f),
                new Quaternion(-5.710473E-09f, -0.9238795f, -4.494751E-08f, 0.3826835f),
                new float4x4(-0.7071067f, 4.495291E-08f, -0.7071068f, 0f, -2.384976E-08f, 1f, 8.742277E-08f, 0f, 0.7071068f, 7.868156E-08f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742277E-08f, 3.926991f, -2.384976E-08f)
            };
            // 1250. - X: 270, Y: 45, Z: -540
            yield return new object[]
            {
                "X: 270, Y: 45, Z: -540",
                new float3(4.712389f, 0.7853982f, -9.424778f),
                new Quaternion(-0.2705981f, -0.6532815f, -0.6532815f, 0.2705981f),
                new float4x4(-0.7071066f, 0.7071068f, 0f, 0f, 0f, 8.940697E-08f, 0.9999999f, 0f, 0.7071068f, 0.7071067f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.926991f, 0f)
            };
            // 1251. - X: 360, Y: 45, Z: -540
            yield return new object[]
            {
                "X: 360, Y: 45, Z: -540",
                new float3(6.283185f, 0.7853982f, -9.424778f),
                new Quaternion(-0.3826835f, 7.620466E-08f, -0.9238795f, -4.44724E-08f),
                new float4x4(-0.7071067f, -1.404988E-07f, 0.7071068f, 0f, 2.384976E-08f, -1f, -1.748456E-07f, 0f, 0.7071068f, -1.067701E-07f, 0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 0.7853982f, 3.141593f)
            };
            // 1252. - X: 585, Y: 45, Z: -540
            yield return new object[]
            {
                "X: 585, Y: 45, Z: -540",
                new float3(10.21018f, 0.7853982f, -9.424778f),
                new Quaternion(0.1464467f, 0.8535533f, 0.3535535f, -0.3535534f),
                new float4x4(-0.7071067f, 0.5000002f, -0.4999998f, 0f, 0f, 0.7071065f, 0.707107f, 0f, 0.7071068f, 0.5000002f, -0.4999997f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.926991f, 0f)
            };
            // 1253. - X: -90, Y: 45, Z: -540
            yield return new object[]
            {
                "X: -90, Y: 45, Z: -540",
                new float3(-1.570796f, 0.7853982f, -9.424778f),
                new Quaternion(0.2705981f, 0.6532815f, 0.6532815f, -0.2705981f),
                new float4x4(-0.7071066f, 0.7071068f, 0f, 0f, 0f, 8.940697E-08f, 0.9999999f, 0f, 0.7071068f, 0.7071067f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.926991f, 0f)
            };
            // 1254. - X: -540, Y: 45, Z: -540
            yield return new object[]
            {
                "X: -540, Y: 45, Z: -540",
                new float3(-9.424778f, 0.7853982f, -9.424778f),
                new Quaternion(1.558061E-08f, -0.9238795f, 6.453698E-09f, 0.3826835f),
                new float4x4(-0.7071067f, -3.372865E-08f, -0.7071068f, 0f, -2.384976E-08f, 1f, -2.384976E-08f, 0f, 0.7071068f, 2.664535E-15f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.926991f, -2.384976E-08f)
            };
            // 1255. - X: 0, Y: 90, Z: -540
            yield return new object[]
            {
                "X: 0, Y: 90, Z: -540",
                new float3(0f, 1.570796f, -9.424778f),
                new Quaternion(0.7071068f, 8.432163E-09f, 0.7071068f, 8.432163E-09f),
                new float4x4(5.960464E-08f, 0f, 0.9999999f, 0f, 2.384976E-08f, -0.9999999f, 0f, 0f, 0.9999999f, 2.384976E-08f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.570796f, 3.141593f)
            };
            // 1256. - X: 0.1, Y: 90, Z: -540
            yield return new object[]
            {
                "X: 0.1, Y: 90, Z: -540",
                new float3(0.001745329f, 1.570796f, -9.424778f),
                new Quaternion(0.7071065f, -0.0006170585f, 0.7071065f, 0.0006170754f),
                new float4x4(-4.626673E-08f, -0.001745328f, 0.9999985f, 0f, 2.392335E-08f, -0.9999986f, -0.001745328f, 0f, 1f, 2.392335E-08f, -4.626673E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 1.570796f, 3.141593f)
            };
            // 1257. - X: 1, Y: 90, Z: -540
            yield return new object[]
            {
                "X: 1, Y: 90, Z: -540",
                new float3(0.01745329f, 1.570796f, -9.424778f),
                new Quaternion(0.7070798f, -0.006170584f, 0.7070798f, 0.006170601f),
                new float4x4(8.213101E-08f, -0.0174524f, 0.9998476f, 0f, 2.328306E-08f, -0.9998475f, -0.0174524f, 0f, 0.9999999f, 2.328306E-08f, 8.213101E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 1.570796f, 3.141593f)
            };
            // 1258. - X: 45, Y: 90, Z: -540
            yield return new object[]
            {
                "X: 45, Y: 90, Z: -540",
                new float3(0.7853982f, 1.570796f, -9.424778f),
                new Quaternion(0.6532815f, -0.2705981f, 0.6532815f, 0.2705981f),
                new float4x4(8.940697E-08f, -0.7071068f, 0.7071067f, 0f, 0f, -0.7071066f, -0.7071068f, 0f, 0.9999999f, 0f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 1.570796f, 3.141593f)
            };
            // 1259. - X: 90, Y: 90, Z: -540
            yield return new object[]
            {
                "X: 90, Y: 90, Z: -540",
                new float3(1.570796f, 1.570796f, -9.424778f),
                new Quaternion(0.5f, -0.5f, 0.5f, 0.5f),
                new float4x4(1.192093E-07f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, 0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 4.712389f, 0f)
            };
            // 1260. - X: 180, Y: 90, Z: -540
            yield return new object[]
            {
                "X: 180, Y: 90, Z: -540",
                new float3(3.141593f, 1.570796f, -9.424778f),
                new Quaternion(-2.247646E-08f, -0.7071068f, -3.934078E-08f, 0.7071068f),
                new float4x4(5.960464E-08f, 8.742278E-08f, -0.9999999f, 0f, -2.384976E-08f, 1f, 8.742278E-08f, 0f, 0.9999999f, 2.384976E-08f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742279E-08f, 4.712389f, -2.384976E-08f)
            };
            // 1261. - X: 270, Y: 90, Z: -540
            yield return new object[]
            {
                "X: 270, Y: 90, Z: -540",
                new float3(4.712389f, 1.570796f, -9.424778f),
                new Quaternion(-0.5f, -0.5f, -0.5f, 0.5f),
                new float4x4(1.192093E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, 0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 1262. - X: 360, Y: 90, Z: -540
            yield return new object[]
            {
                "X: 360, Y: 90, Z: -540",
                new float3(6.283185f, 1.570796f, -9.424778f),
                new Quaternion(-0.7071068f, 5.338508E-08f, -0.7071068f, -7.02494E-08f),
                new float4x4(5.960464E-08f, -1.748456E-07f, 0.9999999f, 0f, 2.384976E-08f, -0.9999999f, -1.748456E-07f, 0f, 0.9999999f, 2.384976E-08f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 1.570796f, 3.141593f)
            };
            // 1263. - X: 585, Y: 90, Z: -540
            yield return new object[]
            {
                "X: 585, Y: 90, Z: -540",
                new float3(10.21018f, 1.570796f, -9.424778f),
                new Quaternion(0.2705982f, 0.6532814f, 0.2705982f, -0.6532814f),
                new float4x4(7.450581E-08f, 0.707107f, -0.7071064f, 0f, 0f, 0.7071065f, 0.707107f, 0f, 0.9999999f, 0f, 7.450581E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 4.712389f, 0f)
            };
            // 1264. - X: -90, Y: 90, Z: -540
            yield return new object[]
            {
                "X: -90, Y: 90, Z: -540",
                new float3(-1.570796f, 1.570796f, -9.424778f),
                new Quaternion(0.5f, 0.5f, 0.5f, -0.5f),
                new float4x4(1.192093E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, 0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 4.712389f, 0f)
            };
            // 1265. - X: -540, Y: 90, Z: -540
            yield return new object[]
            {
                "X: -540, Y: 90, Z: -540",
                new float3(-9.424778f, 1.570796f, -9.424778f),
                new Quaternion(1.686433E-08f, -0.7071068f, 0f, 0.7071068f),
                new float4x4(5.960464E-08f, -2.384976E-08f, -0.9999999f, 0f, -2.384976E-08f, 1f, -2.384976E-08f, 0f, 0.9999999f, 2.384976E-08f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 4.712389f, -2.384976E-08f)
            };
            // 1266. - X: 0, Y: 180, Z: -540
            yield return new object[]
            {
                "X: 0, Y: 180, Z: -540",
                new float3(0f, 3.141593f, -9.424778f),
                new Quaternion(1f, 1.192488E-08f, -4.371139E-08f, -5.212531E-16f),
                new float4x4(1f, 2.384976E-08f, -8.742278E-08f, 0f, 2.384976E-08f, -1f, 0f, 0f, -8.742278E-08f, -2.085012E-15f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.141593f, 3.141593f)
            };
            // 1267. - X: 0.1, Y: 180, Z: -540
            yield return new object[]
            {
                "X: 0.1, Y: 180, Z: -540",
                new float3(0.001745329f, 3.141593f, -9.424778f),
                new Quaternion(0.9999996f, 1.196302E-08f, -4.372178E-08f, 0.0008726645f),
                new float4x4(1f, 2.400234E-08f, -8.742265E-08f, 0f, 2.384973E-08f, -0.9999986f, -0.001745328f, 0f, -8.746441E-08f, 0.001745328f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.141593f, 3.141593f)
            };
            // 1268. - X: 1, Y: 180, Z: -540
            yield return new object[]
            {
                "X: 1, Y: 180, Z: -540",
                new float3(0.01745329f, 3.141593f, -9.424778f),
                new Quaternion(0.9999619f, 1.230588E-08f, -4.381378E-08f, 0.008726535f),
                new float4x4(1f, 2.53755E-08f, -8.740945E-08f, 0f, 2.384613E-08f, -0.9998477f, -0.01745241f, 0f, -8.783901E-08f, 0.01745241f, -0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.141593f, 3.141593f)
            };
            // 1269. - X: 45, Y: 180, Z: -540
            yield return new object[]
            {
                "X: 45, Y: 180, Z: -540",
                new float3(0.7853982f, 3.141593f, -9.424778f),
                new Quaternion(0.9238795f, 2.774478E-08f, -4.494751E-08f, 0.3826835f),
                new float4x4(1f, 8.5667E-08f, -6.181723E-08f, 0f, 1.686433E-08f, -0.7071067f, -0.7071068f, 0f, -1.042871E-07f, 0.7071068f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 3.141593f, 3.141593f)
            };
            // 1270. - X: 90, Y: 180, Z: -540
            yield return new object[]
            {
                "X: 90, Y: 180, Z: -540",
                new float3(1.570796f, 3.141593f, -9.424778f),
                new Quaternion(0.7071068f, 3.934078E-08f, -3.934078E-08f, 0.7071068f),
                new float4x4(1f, 1.112725E-07f, 0f, 0f, 0f, 5.960464E-08f, -0.9999999f, 0f, -1.112725E-07f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 1.112725E-07f, 0f)
            };
            // 1271. - X: 180, Y: 180, Z: -540
            yield return new object[]
            {
                "X: 180, Y: 180, Z: -540",
                new float3(3.141593f, 3.141593f, -9.424778f),
                new Quaternion(-4.371139E-08f, 4.371139E-08f, -1.192488E-08f, 1f),
                new float4x4(1f, 2.384975E-08f, 8.742278E-08f, 0f, -2.384976E-08f, 1f, 8.742278E-08f, 0f, -8.742278E-08f, -8.742278E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 8.742278E-08f, -2.384976E-08f)
            };
            // 1272. - X: 270, Y: 180, Z: -540
            yield return new object[]
            {
                "X: 270, Y: 180, Z: -540",
                new float3(4.712389f, 3.141593f, -9.424778f),
                new Quaternion(-0.7071068f, 2.247646E-08f, 2.247646E-08f, 0.7071068f),
                new float4x4(1f, -6.357302E-08f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, -6.357302E-08f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 6.357302E-08f, 0f)
            };
            // 1273. - X: 360, Y: 180, Z: -540
            yield return new object[]
            {
                "X: 360, Y: 180, Z: -540",
                new float3(6.283185f, 3.141593f, -9.424778f),
                new Quaternion(-1f, -1.192488E-08f, 4.371139E-08f, -8.742278E-08f),
                new float4x4(1f, 2.384978E-08f, -8.742278E-08f, 0f, 2.384976E-08f, -1f, -1.748456E-07f, 0f, -8.742278E-08f, 1.748456E-07f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 3.141593f, 3.141593f)
            };
            // 1274. - X: 585, Y: 180, Z: -540
            yield return new object[]
            {
                "X: 585, Y: 180, Z: -540",
                new float3(10.21018f, 3.141593f, -9.424778f),
                new Quaternion(0.3826836f, -3.582059E-08f, -5.710478E-09f, -0.9238794f),
                new float4x4(1f, -3.79675E-08f, 6.181721E-08f, 0f, -1.686432E-08f, 0.7071065f, 0.707107f, 0f, -7.055844E-08f, -0.707107f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 8.742278E-08f, -2.384976E-08f)
            };
            // 1275. - X: -90, Y: 180, Z: -540
            yield return new object[]
            {
                "X: -90, Y: 180, Z: -540",
                new float3(-1.570796f, 3.141593f, -9.424778f),
                new Quaternion(0.7071068f, -2.247646E-08f, -2.247646E-08f, -0.7071068f),
                new float4x4(1f, -6.357302E-08f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, -6.357302E-08f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 6.357302E-08f, 0f)
            };
            // 1276. - X: -540, Y: 180, Z: -540
            yield return new object[]
            {
                "X: -540, Y: 180, Z: -540",
                new float3(-9.424778f, 3.141593f, -9.424778f),
                new Quaternion(1.192488E-08f, 4.371139E-08f, -1.192488E-08f, 1f),
                new float4x4(1f, 2.384976E-08f, 8.742278E-08f, 0f, -2.384976E-08f, 1f, -2.384976E-08f, 0f, -8.742278E-08f, 2.384976E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 8.742278E-08f, -2.384976E-08f)
            };
            // 1277. - X: 0, Y: 270, Z: -540
            yield return new object[]
            {
                "X: 0, Y: 270, Z: -540",
                new float3(0f, 4.712389f, -9.424778f),
                new Quaternion(0.7071068f, 8.432163E-09f, -0.7071068f, -8.432163E-09f),
                new float4x4(5.960464E-08f, 0f, -0.9999999f, 0f, 2.384976E-08f, -0.9999999f, 0f, 0f, -0.9999999f, -2.384976E-08f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 3.141593f)
            };
            // 1278. - X: 0.1, Y: 270, Z: -540
            yield return new object[]
            {
                "X: 0.1, Y: 270, Z: -540",
                new float3(0.001745329f, 4.712389f, -9.424778f),
                new Quaternion(0.7071065f, 0.0006170754f, -0.7071065f, 0.0006170585f),
                new float4x4(-4.63084E-08f, 0.001745328f, -0.9999985f, 0f, 2.392335E-08f, -0.9999986f, -0.001745328f, 0f, -1f, -2.392335E-08f, -4.63084E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 4.712389f, 3.141593f)
            };
            // 1279. - X: 1, Y: 270, Z: -540
            yield return new object[]
            {
                "X: 1, Y: 270, Z: -540",
                new float3(0.01745329f, 4.712389f, -9.424778f),
                new Quaternion(0.7070798f, 0.006170601f, -0.7070798f, 0.006170584f),
                new float4x4(8.171628E-08f, 0.0174524f, -0.9998476f, 0f, 2.328306E-08f, -0.9998475f, -0.0174524f, 0f, -0.9999999f, -2.328306E-08f, 8.171628E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 4.712389f, 3.141593f)
            };
            // 1280. - X: 45, Y: 270, Z: -540
            yield return new object[]
            {
                "X: 45, Y: 270, Z: -540",
                new float3(0.7853982f, 4.712389f, -9.424778f),
                new Quaternion(0.6532815f, 0.2705981f, -0.6532815f, 0.2705981f),
                new float4x4(8.940697E-08f, 0.7071068f, -0.7071067f, 0f, 0f, -0.7071066f, -0.7071068f, 0f, -0.9999999f, 0f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 4.712389f, 3.141593f)
            };
            // 1281. - X: 90, Y: 270, Z: -540
            yield return new object[]
            {
                "X: 90, Y: 270, Z: -540",
                new float3(1.570796f, 4.712389f, -9.424778f),
                new Quaternion(0.5f, 0.5f, -0.5f, 0.5f),
                new float4x4(1.192093E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, -0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 1.570796f, 0f)
            };
            // 1282. - X: 180, Y: 270, Z: -540
            yield return new object[]
            {
                "X: 180, Y: 270, Z: -540",
                new float3(3.141593f, 4.712389f, -9.424778f),
                new Quaternion(-3.934078E-08f, 0.7071068f, 2.247646E-08f, 0.7071068f),
                new float4x4(5.960464E-08f, -8.742278E-08f, 0.9999999f, 0f, -2.384976E-08f, 1f, 8.742278E-08f, 0f, -0.9999999f, -2.384976E-08f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742279E-08f, 1.570796f, -2.384976E-08f)
            };
            // 1283. - X: 270, Y: 270, Z: -540
            yield return new object[]
            {
                "X: 270, Y: 270, Z: -540",
                new float3(4.712389f, 4.712389f, -9.424778f),
                new Quaternion(-0.5f, 0.5f, 0.5f, 0.5f),
                new float4x4(1.192093E-07f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 1284. - X: 360, Y: 270, Z: -540
            yield return new object[]
            {
                "X: 360, Y: 270, Z: -540",
                new float3(6.283185f, 4.712389f, -9.424778f),
                new Quaternion(-0.7071068f, -7.02494E-08f, 0.7071068f, -5.338508E-08f),
                new float4x4(5.960463E-08f, 1.748456E-07f, -0.9999999f, 0f, 2.384976E-08f, -0.9999999f, -1.748456E-07f, 0f, -0.9999999f, -2.384976E-08f, 5.960463E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 4.712389f, 3.141593f)
            };
            // 1285. - X: 585, Y: 270, Z: -540
            yield return new object[]
            {
                "X: 585, Y: 270, Z: -540",
                new float3(10.21018f, 4.712389f, -9.424778f),
                new Quaternion(0.2705982f, -0.6532814f, -0.2705982f, -0.6532814f),
                new float4x4(7.450581E-08f, -0.707107f, 0.7071064f, 0f, 0f, 0.7071065f, 0.707107f, 0f, -0.9999999f, 0f, 7.450581E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.570796f, 0f)
            };
            // 1286. - X: -90, Y: 270, Z: -540
            yield return new object[]
            {
                "X: -90, Y: 270, Z: -540",
                new float3(-1.570796f, 4.712389f, -9.424778f),
                new Quaternion(0.5f, -0.5f, -0.5f, -0.5f),
                new float4x4(1.192093E-07f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 1287. - X: -540, Y: 270, Z: -540
            yield return new object[]
            {
                "X: -540, Y: 270, Z: -540",
                new float3(-9.424778f, 4.712389f, -9.424778f),
                new Quaternion(0f, 0.7071068f, -1.686433E-08f, 0.7071068f),
                new float4x4(5.960464E-08f, 2.384976E-08f, 0.9999999f, 0f, -2.384976E-08f, 1f, -2.384976E-08f, 0f, -0.9999999f, -2.384976E-08f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 1.570796f, -2.384976E-08f)
            };
            // 1288. - X: 0, Y: 360, Z: -540
            yield return new object[]
            {
                "X: 0, Y: 360, Z: -540",
                new float3(0f, 6.283185f, -9.424778f),
                new Quaternion(-8.742278E-08f, -1.042506E-15f, -1f, -1.192488E-08f),
                new float4x4(-1f, -2.384976E-08f, 1.748456E-07f, 0f, 2.384976E-08f, -1f, 0f, 0f, 1.748456E-07f, 4.170025E-15f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 1.748456E-07f, 3.141593f)
            };
            // 1289. - X: 0.1, Y: 360, Z: -540
            yield return new object[]
            {
                "X: 0.1, Y: 360, Z: -540",
                new float3(0.001745329f, 6.283185f, -9.424778f),
                new Quaternion(-8.743316E-08f, 0.0008726645f, -0.9999996f, -1.200117E-08f),
                new float4x4(-1f, -2.415493E-08f, 1.748453E-07f, 0f, 2.384973E-08f, -0.9999986f, -0.001745328f, 0f, 1.748872E-07f, -0.001745328f, 0.9999985f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 1.748456E-07f, 3.141593f)
            };
            // 1290. - X: 1, Y: 360, Z: -540
            yield return new object[]
            {
                "X: 1, Y: 360, Z: -540",
                new float3(0.01745329f, 6.283185f, -9.424778f),
                new Quaternion(-8.752351E-08f, 0.008726535f, -0.9999619f, -1.268732E-08f),
                new float4x4(-0.9999999f, -2.690124E-08f, 1.748189E-07f, 0f, 2.384613E-08f, -0.9998477f, -0.01745241f, 0f, 1.752618E-07f, -0.01745241f, 0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 1.748456E-07f, 3.141593f)
            };
            // 1291. - X: 45, Y: 360, Z: -540
            yield return new object[]
            {
                "X: 45, Y: 360, Z: -540",
                new float3(0.7853982f, 6.283185f, -9.424778f),
                new Quaternion(-8.533156E-08f, 0.3826835f, -0.9238795f, -4.44724E-08f),
                new float4x4(-1f, -1.474842E-07f, 1.236345E-07f, 0f, 1.686433E-08f, -0.7071067f, -0.7071068f, 0f, 1.917099E-07f, -0.7071068f, 0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 1.748455E-07f, 3.141593f)
            };
            // 1292. - X: 90, Y: 360, Z: -540
            yield return new object[]
            {
                "X: 90, Y: 360, Z: -540",
                new float3(1.570796f, 6.283185f, -9.424778f),
                new Quaternion(-7.02494E-08f, 0.7071068f, -0.7071068f, -7.02494E-08f),
                new float4x4(-0.9999999f, -1.986953E-07f, 0f, 0f, 0f, 5.960463E-08f, -0.9999999f, 0f, 1.986953E-07f, -0.9999999f, 5.960463E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 3.141593f, 0f)
            };
            // 1293. - X: 180, Y: 360, Z: -540
            yield return new object[]
            {
                "X: 180, Y: 360, Z: -540",
                new float3(3.141593f, 6.283185f, -9.424778f),
                new Quaternion(-1.192488E-08f, 1f, 4.371139E-08f, -8.742278E-08f),
                new float4x4(-1f, -2.384975E-08f, -1.748456E-07f, 0f, -2.384976E-08f, 1f, 8.742278E-08f, 0f, 1.748456E-07f, 8.742278E-08f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, 3.141593f, -2.384976E-08f)
            };
            // 1294. - X: 270, Y: 360, Z: -540
            yield return new object[]
            {
                "X: 270, Y: 360, Z: -540",
                new float3(4.712389f, 6.283185f, -9.424778f),
                new Quaternion(5.338508E-08f, 0.7071068f, 0.7071068f, -5.338508E-08f),
                new float4x4(-0.9999999f, 1.509958E-07f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, 1.509958E-07f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.141593f, 0f)
            };
            // 1295. - X: 360, Y: 360, Z: -540
            yield return new object[]
            {
                "X: 360, Y: 360, Z: -540",
                new float3(6.283185f, 6.283185f, -9.424778f),
                new Quaternion(8.742278E-08f, -8.742278E-08f, 1f, 1.192489E-08f),
                new float4x4(-1f, -2.384979E-08f, 1.748456E-07f, 0f, 2.384976E-08f, -1f, -1.748456E-07f, 0f, 1.748456E-07f, -1.748456E-07f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 1.748456E-07f, 3.141593f)
            };
            // 1296. - X: 585, Y: 360, Z: -540
            yield return new object[]
            {
                "X: 585, Y: 360, Z: -540",
                new float3(10.21018f, 6.283185f, -9.424778f),
                new Quaternion(-2.243811E-08f, -0.9238794f, -0.3826836f, 7.620465E-08f),
                new float4x4(-0.9999999f, 9.978476E-08f, -1.236344E-07f, 0f, -1.686433E-08f, 0.7071065f, 0.707107f, 0f, 1.579812E-07f, 0.707107f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 3.141593f, -2.384977E-08f)
            };
            // 1297. - X: -90, Y: 360, Z: -540
            yield return new object[]
            {
                "X: -90, Y: 360, Z: -540",
                new float3(-1.570796f, 6.283185f, -9.424778f),
                new Quaternion(-5.338508E-08f, -0.7071068f, -0.7071068f, 5.338508E-08f),
                new float4x4(-0.9999999f, 1.509958E-07f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, 1.509958E-07f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 3.141593f, 0f)
            };
            // 1298. - X: -540, Y: 360, Z: -540
            yield return new object[]
            {
                "X: -540, Y: 360, Z: -540",
                new float3(-9.424778f, 6.283185f, -9.424778f),
                new Quaternion(-1.192488E-08f, 1f, -1.192488E-08f, -8.742278E-08f),
                new float4x4(-1f, -2.384976E-08f, -1.748456E-07f, 0f, -2.384976E-08f, 1f, -2.384976E-08f, 0f, 1.748456E-07f, -2.384976E-08f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 3.141593f, -2.384976E-08f)
            };
            // 1299. - X: 0, Y: 585, Z: -540
            yield return new object[]
            {
                "X: 0, Y: 585, Z: -540",
                new float3(0f, 10.21018f, -9.424778f),
                new Quaternion(-0.9238794f, -1.101715E-08f, 0.3826836f, 4.563456E-09f),
                new float4x4(0.7071065f, 1.686432E-08f, -0.707107f, 0f, 2.384976E-08f, -0.9999999f, 0f, 0f, -0.707107f, -1.686433E-08f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.926991f, 3.141593f)
            };
            // 1300. - X: 0.1, Y: 585, Z: -540
            yield return new object[]
            {
                "X: 0.1, Y: 585, Z: -540",
                new float3(0.001745329f, 10.21018f, -9.424778f),
                new Quaternion(-0.9238791f, -0.0003339654f, 0.3826835f, -0.0008062323f),
                new float4x4(0.7071065f, 0.001234151f, -0.7071059f, 0f, 2.386514E-08f, -0.9999985f, -0.001745328f, 0f, -0.707107f, 0.001234116f, -0.7071054f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.926991f, 3.141593f)
            };
            // 1301. - X: 1, Y: 585, Z: -540
            yield return new object[]
            {
                "X: 1, Y: 585, Z: -540",
                new float3(0.01745329f, 10.21018f, -9.424778f),
                new Quaternion(-0.9238443f, -0.003339513f, 0.382669f, -0.008062262f),
                new float4x4(0.7071065f, 0.01234074f, -0.7069994f, 0f, 2.374873E-08f, -0.9998477f, -0.01745241f, 0f, -0.707107f, 0.01234069f, -0.7069988f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.926991f, 3.141593f)
            };
            // 1302. - X: 45, Y: 585, Z: -540
            yield return new object[]
            {
                "X: 45, Y: 585, Z: -540",
                new float3(0.7853982f, 10.21018f, -9.424778f),
                new Quaternion(-0.8535533f, -0.1464467f, 0.3535535f, -0.3535534f),
                new float4x4(0.7071065f, 0.5000002f, -0.5000001f, 0f, 5.960464E-08f, -0.7071067f, -0.7071068f, 0f, -0.7071071f, 0.4999998f, -0.4999998f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 3.926991f, 3.141593f)
            };
            // 1303. - X: 90, Y: 585, Z: -540
            yield return new object[]
            {
                "X: 90, Y: 585, Z: -540",
                new float3(1.570796f, 10.21018f, -9.424778f),
                new Quaternion(-0.6532814f, -0.2705982f, 0.2705982f, -0.6532814f),
                new float4x4(0.7071065f, 0.707107f, 0f, 0f, 0f, 7.450581E-08f, -0.9999999f, 0f, -0.707107f, 0.7071064f, 7.450581E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 0.7853986f, 0f)
            };
            // 1304. - X: 180, Y: 585, Z: -540
            yield return new object[]
            {
                "X: 180, Y: 585, Z: -540",
                new float3(3.141593f, 10.21018f, -9.424778f),
                new Quaternion(4.494751E-08f, -0.3826836f, -5.710478E-09f, -0.9238794f),
                new float4x4(0.7071065f, -4.495294E-08f, 0.707107f, 0f, -2.384976E-08f, 1f, 8.742277E-08f, 0f, -0.707107f, -7.868154E-08f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742277E-08f, 0.7853985f, -2.384976E-08f)
            };
            // 1305. - X: 270, Y: 585, Z: -540
            yield return new object[]
            {
                "X: 270, Y: 585, Z: -540",
                new float3(4.712389f, 10.21018f, -9.424778f),
                new Quaternion(0.6532814f, -0.2705982f, -0.2705982f, -0.6532814f),
                new float4x4(0.7071065f, -0.707107f, 0f, 0f, 0f, 7.450581E-08f, 0.9999999f, 0f, -0.707107f, -0.7071064f, 7.450581E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.7853986f, 0f)
            };
            // 1306. - X: 360, Y: 585, Z: -540
            yield return new object[]
            {
                "X: 360, Y: 585, Z: -540",
                new float3(6.283185f, 10.21018f, -9.424778f),
                new Quaternion(0.9238794f, 4.447242E-08f, -0.3826836f, 7.620465E-08f),
                new float4x4(0.7071065f, 1.404988E-07f, -0.707107f, 0f, 2.384976E-08f, -0.9999999f, -1.748455E-07f, 0f, -0.707107f, 1.067701E-07f, -0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748455E-07f, 3.926991f, 3.141593f)
            };
            // 1307. - X: 585, Y: 585, Z: -540
            yield return new object[]
            {
                "X: 585, Y: 585, Z: -540",
                new float3(10.21018f, 10.21018f, -9.424778f),
                new Quaternion(-0.3535535f, 0.3535535f, 0.1464467f, 0.8535532f),
                new float4x4(0.7071066f, -0.5000003f, 0.5f, 0f, 0f, 0.7071066f, 0.7071069f, 0f, -0.7071069f, -0.5f, 0.4999997f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 0.7853985f, -4.214686E-08f)
            };
            // 1308. - X: -90, Y: 585, Z: -540
            yield return new object[]
            {
                "X: -90, Y: 585, Z: -540",
                new float3(-1.570796f, 10.21018f, -9.424778f),
                new Quaternion(-0.6532814f, 0.2705982f, 0.2705982f, 0.6532814f),
                new float4x4(0.7071065f, -0.707107f, 0f, 0f, 0f, 7.450581E-08f, 0.9999999f, 0f, -0.707107f, -0.7071064f, 7.450581E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 0.7853986f, 0f)
            };
            // 1309. - X: -540, Y: 585, Z: -540
            yield return new object[]
            {
                "X: -540, Y: 585, Z: -540",
                new float3(-9.424778f, 10.21018f, -9.424778f),
                new Quaternion(-6.453696E-09f, -0.3826836f, 1.558061E-08f, -0.9238794f),
                new float4x4(0.7071065f, 3.372866E-08f, 0.707107f, 0f, -2.384976E-08f, 1f, -2.384976E-08f, 0f, -0.707107f, -1.24345E-14f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 0.7853985f, -2.384976E-08f)
            };
            // 1310. - X: 0, Y: -90, Z: -540
            yield return new object[]
            {
                "X: 0, Y: -90, Z: -540",
                new float3(0f, -1.570796f, -9.424778f),
                new Quaternion(-0.7071068f, -8.432163E-09f, 0.7071068f, 8.432163E-09f),
                new float4x4(5.960464E-08f, 0f, -0.9999999f, 0f, 2.384976E-08f, -0.9999999f, 0f, 0f, -0.9999999f, -2.384976E-08f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 4.712389f, 3.141593f)
            };
            // 1311. - X: 0.1, Y: -90, Z: -540
            yield return new object[]
            {
                "X: 0.1, Y: -90, Z: -540",
                new float3(0.001745329f, -1.570796f, -9.424778f),
                new Quaternion(-0.7071065f, -0.0006170754f, 0.7071065f, -0.0006170585f),
                new float4x4(-4.63084E-08f, 0.001745328f, -0.9999985f, 0f, 2.392335E-08f, -0.9999986f, -0.001745328f, 0f, -1f, -2.392335E-08f, -4.63084E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 4.712389f, 3.141593f)
            };
            // 1312. - X: 1, Y: -90, Z: -540
            yield return new object[]
            {
                "X: 1, Y: -90, Z: -540",
                new float3(0.01745329f, -1.570796f, -9.424778f),
                new Quaternion(-0.7070798f, -0.006170601f, 0.7070798f, -0.006170584f),
                new float4x4(8.171628E-08f, 0.0174524f, -0.9998476f, 0f, 2.328306E-08f, -0.9998475f, -0.0174524f, 0f, -0.9999999f, -2.328306E-08f, 8.171628E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 4.712389f, 3.141593f)
            };
            // 1313. - X: 45, Y: -90, Z: -540
            yield return new object[]
            {
                "X: 45, Y: -90, Z: -540",
                new float3(0.7853982f, -1.570796f, -9.424778f),
                new Quaternion(-0.6532815f, -0.2705981f, 0.6532815f, -0.2705981f),
                new float4x4(8.940697E-08f, 0.7071068f, -0.7071067f, 0f, 0f, -0.7071066f, -0.7071068f, 0f, -0.9999999f, 0f, 8.940697E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853983f, 4.712389f, 3.141593f)
            };
            // 1314. - X: 90, Y: -90, Z: -540
            yield return new object[]
            {
                "X: 90, Y: -90, Z: -540",
                new float3(1.570796f, -1.570796f, -9.424778f),
                new Quaternion(-0.5f, -0.5f, 0.5f, -0.5f),
                new float4x4(1.192093E-07f, 0.9999999f, 0f, 0f, 0f, 1.192093E-07f, -0.9999999f, 0f, -0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 1.570796f, 0f)
            };
            // 1315. - X: 180, Y: -90, Z: -540
            yield return new object[]
            {
                "X: 180, Y: -90, Z: -540",
                new float3(3.141593f, -1.570796f, -9.424778f),
                new Quaternion(3.934078E-08f, -0.7071068f, -2.247646E-08f, -0.7071068f),
                new float4x4(5.960464E-08f, -8.742278E-08f, 0.9999999f, 0f, -2.384976E-08f, 1f, 8.742278E-08f, 0f, -0.9999999f, -2.384976E-08f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742279E-08f, 1.570796f, -2.384976E-08f)
            };
            // 1316. - X: 270, Y: -90, Z: -540
            yield return new object[]
            {
                "X: 270, Y: -90, Z: -540",
                new float3(4.712389f, -1.570796f, -9.424778f),
                new Quaternion(0.5f, -0.5f, -0.5f, -0.5f),
                new float4x4(1.192093E-07f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 1317. - X: 360, Y: -90, Z: -540
            yield return new object[]
            {
                "X: 360, Y: -90, Z: -540",
                new float3(6.283185f, -1.570796f, -9.424778f),
                new Quaternion(0.7071068f, 7.02494E-08f, -0.7071068f, 5.338508E-08f),
                new float4x4(5.960463E-08f, 1.748456E-07f, -0.9999999f, 0f, 2.384976E-08f, -0.9999999f, -1.748456E-07f, 0f, -0.9999999f, -2.384976E-08f, 5.960463E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 4.712389f, 3.141593f)
            };
            // 1318. - X: 585, Y: -90, Z: -540
            yield return new object[]
            {
                "X: 585, Y: -90, Z: -540",
                new float3(10.21018f, -1.570796f, -9.424778f),
                new Quaternion(-0.2705982f, 0.6532814f, 0.2705982f, 0.6532814f),
                new float4x4(7.450581E-08f, -0.707107f, 0.7071064f, 0f, 0f, 0.7071065f, 0.707107f, 0f, -0.9999999f, 0f, 7.450581E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, 1.570796f, 0f)
            };
            // 1319. - X: -90, Y: -90, Z: -540
            yield return new object[]
            {
                "X: -90, Y: -90, Z: -540",
                new float3(-1.570796f, -1.570796f, -9.424778f),
                new Quaternion(-0.5f, 0.5f, 0.5f, 0.5f),
                new float4x4(1.192093E-07f, -0.9999999f, 0f, 0f, 0f, 1.192093E-07f, 0.9999999f, 0f, -0.9999999f, 0f, 1.192093E-07f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, 1.570796f, 0f)
            };
            // 1320. - X: -540, Y: -90, Z: -540
            yield return new object[]
            {
                "X: -540, Y: -90, Z: -540",
                new float3(-9.424778f, -1.570796f, -9.424778f),
                new Quaternion(0f, -0.7071068f, 1.686433E-08f, -0.7071068f),
                new float4x4(5.960464E-08f, 2.384976E-08f, 0.9999999f, 0f, -2.384976E-08f, 1f, -2.384976E-08f, 0f, -0.9999999f, -2.384976E-08f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, 1.570796f, -2.384976E-08f)
            };
            // 1321. - X: 0, Y: -540, Z: -540
            yield return new object[]
            {
                "X: 0, Y: -540, Z: -540",
                new float3(0f, -9.424778f, -9.424778f),
                new Quaternion(1f, 1.192488E-08f, 1.192488E-08f, 1.422028E-16f),
                new float4x4(1f, 2.384976E-08f, 2.384976E-08f, 0f, 2.384976E-08f, -1f, 0f, 0f, 2.384976E-08f, 5.688111E-16f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(0f, 3.141593f, 3.141593f)
            };
            // 1322. - X: 0.1, Y: -540, Z: -540
            yield return new object[]
            {
                "X: 0.1, Y: -540, Z: -540",
                new float3(0.001745329f, -9.424778f, -9.424778f),
                new Quaternion(0.9999996f, 1.191447E-08f, 1.191447E-08f, 0.0008726645f),
                new float4x4(1f, 2.380814E-08f, 2.384972E-08f, 0f, 2.384972E-08f, -0.9999986f, -0.001745328f, 0f, 2.380814E-08f, 0.001745328f, -0.9999986f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.001745329f, 3.141593f, 3.141593f)
            };
            // 1323. - X: 1, Y: -540, Z: -540
            yield return new object[]
            {
                "X: 1, Y: -540, Z: -540",
                new float3(0.01745329f, -9.424778f, -9.424778f),
                new Quaternion(0.9999619f, 1.182036E-08f, 1.182036E-08f, 0.008726535f),
                new float4x4(1f, 2.343353E-08f, 2.384613E-08f, 0f, 2.384613E-08f, -0.9998477f, -0.01745241f, 0f, 2.343353E-08f, 0.01745241f, -0.9998477f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.01745329f, 3.141593f, 3.141593f)
            };
            // 1324. - X: 45, Y: -540, Z: -540
            yield return new object[]
            {
                "X: 45, Y: -540, Z: -540",
                new float3(0.7853982f, -9.424778f, -9.424778f),
                new Quaternion(0.9238795f, 6.453698E-09f, 6.453698E-09f, 0.3826835f),
                new float4x4(1f, 6.985432E-09f, 1.686433E-08f, 0f, 1.686433E-08f, -0.7071067f, -0.7071068f, 0f, 6.985432E-09f, 0.7071068f, -0.7071067f, 0f, 0f, 0f, 0f, 1f),
                new float3(0.7853982f, 3.141593f, 3.141593f)
            };
            // 1325. - X: 90, Y: -540, Z: -540
            yield return new object[]
            {
                "X: 90, Y: -540, Z: -540",
                new float3(1.570796f, -9.424778f, -9.424778f),
                new Quaternion(0.7071068f, 0f, 0f, 0.7071068f),
                new float4x4(1f, 0f, 0f, 0f, 0f, 5.960464E-08f, -0.9999999f, 0f, 0f, 0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.570796f, 0f, 0f)
            };
            // 1326. - X: 180, Y: -540, Z: -540
            yield return new object[]
            {
                "X: 180, Y: -540, Z: -540",
                new float3(3.141593f, -9.424778f, -9.424778f),
                new Quaternion(-4.371139E-08f, -1.192488E-08f, -1.192488E-08f, 1f),
                new float4x4(1f, 2.384976E-08f, -2.384976E-08f, 0f, -2.384976E-08f, 1f, 8.742278E-08f, 0f, 2.384976E-08f, -8.742278E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(-8.742278E-08f, -2.384976E-08f, -2.384976E-08f)
            };
            // 1327. - X: 270, Y: -540, Z: -540
            yield return new object[]
            {
                "X: 270, Y: -540, Z: -540",
                new float3(4.712389f, -9.424778f, -9.424778f),
                new Quaternion(-0.7071068f, -1.686433E-08f, -1.686433E-08f, 0.7071068f),
                new float4x4(1f, 4.769952E-08f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, 4.769952E-08f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, -4.769952E-08f, 0f)
            };
            // 1328. - X: 360, Y: -540, Z: -540
            yield return new object[]
            {
                "X: 360, Y: -540, Z: -540",
                new float3(6.283185f, -9.424778f, -9.424778f),
                new Quaternion(-1f, -1.192488E-08f, -1.192488E-08f, -8.742278E-08f),
                new float4x4(1f, 2.384976E-08f, 2.384976E-08f, 0f, 2.384976E-08f, -1f, -1.748456E-07f, 0f, 2.384976E-08f, 1.748456E-07f, -1f, 0f, 0f, 0f, 0f, 1f),
                new float3(1.748456E-07f, 3.141593f, 3.141593f)
            };
            // 1329. - X: 585, Y: -540, Z: -540
            yield return new object[]
            {
                "X: 585, Y: -540, Z: -540",
                new float3(10.21018f, -9.424778f, -9.424778f),
                new Quaternion(0.3826836f, 1.558061E-08f, 1.558061E-08f, -0.9238794f),
                new float4x4(1f, 4.07141E-08f, -1.686432E-08f, 0f, -1.686432E-08f, 0.7071065f, 0.707107f, 0f, 4.07141E-08f, -0.707107f, 0.7071065f, 0f, 0f, 0f, 0f, 1f),
                new float3(5.497787f, -2.384976E-08f, -2.384976E-08f)
            };
            // 1330. - X: -90, Y: -540, Z: -540
            yield return new object[]
            {
                "X: -90, Y: -540, Z: -540",
                new float3(-1.570796f, -9.424778f, -9.424778f),
                new Quaternion(0.7071068f, 1.686433E-08f, 1.686433E-08f, -0.7071068f),
                new float4x4(1f, 4.769952E-08f, 0f, 0f, 0f, 5.960464E-08f, 0.9999999f, 0f, 4.769952E-08f, -0.9999999f, 5.960464E-08f, 0f, 0f, 0f, 0f, 1f),
                new float3(4.712389f, -4.769952E-08f, 0f)
            };
            // 1331. - X: -540, Y: -540, Z: -540
            yield return new object[]
            {
                "X: -540, Y: -540, Z: -540",
                new float3(-9.424778f, -9.424778f, -9.424778f),
                new Quaternion(1.192488E-08f, -1.192488E-08f, -1.192488E-08f, 1f),
                new float4x4(1f, 2.384976E-08f, -2.384976E-08f, 0f, -2.384976E-08f, 1f, -2.384976E-08f, 0f, 2.384976E-08f, 2.384976E-08f, 1f, 0f, 0f, 0f, 0f, 1f),
                new float3(2.384976E-08f, -2.384976E-08f, -2.384976E-08f)
            };
        }
    }
}