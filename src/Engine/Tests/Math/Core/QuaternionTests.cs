using System;
using NUnit.Framework;
using Fusee.Math;

namespace Fusee.Tests.Math.Core
{
    [TestFixture]
    public class QuaternionTests
    {
        //[SetUp]
        //public void Init()
        //{
        //}

        [Test]
        public static void EulerAngleConversion()
        {
            float3[] eulers =
            {
                new float3(0.744355f, 1.246385f, 0.9342039f),  // good
                new float3(0, 1.653684f, 0),    // y is bigger than pi/2 -> bad?
                new float3(0.8579133f, 1.653684f, 1.0163f),    // bad as of oct 2015 because y is bigger than pi/2

                /*
                new float3(0.2f, 0.4f, 0.8f),
                new float3(0f, 0f, 0f),
                new float3(1f, 0f, 0f),
                new float3(0f, 1f, 0f),
                new float3(0f, 0f, 1f),
                new float3(1f, 0f, 0f),
                */
            };

            foreach (var euler in eulers)
            {
                Quaternion quaternion = Quaternion.EulerToQuaternion(euler);

                float3 euler2 = Quaternion.QuaternionToEuler(quaternion);

                float3 diff = euler - euler2;
                float normDiff = diff.Length;

                Assert.Less(normDiff, 0.01f);
            }
        }


        [Test]
        public static void SimpleSlerpTest()
        {
            float3 euler1 = new float3(MathHelper.DegreesToRadians(30), MathHelper.DegreesToRadians(60),
                MathHelper.DegreesToRadians(90));
            float3 euler2 = new float3(MathHelper.DegreesToRadians(-45), MathHelper.DegreesToRadians(120),
                MathHelper.DegreesToRadians(160));

            Quaternion quaternion1 = Quaternion.EulerToQuaternion(euler1);
            Quaternion quaternion2 = Quaternion.EulerToQuaternion(euler2);

            Quaternion quaternionResult = Quaternion.Slerp(quaternion1, quaternion2, 0.0f);
            Assert.AreEqual(quaternion1, quaternionResult);

            quaternionResult = Quaternion.Slerp(quaternion1, quaternion2, 1.0f);
            Assert.AreEqual(quaternion2, quaternionResult);
        }




        public static readonly float startAngle = 0;
        public static readonly float endAngle = 720;


        public static float CalcExpected(float angle1, float angle2)
        {
            float temp = (((angle2 - angle1)%360.0f)/2.0f);
            return (System.Math.Abs(temp) >= 90.0f) ? (-System.Math.Sign(temp)*(180.0f - temp)) : temp;
        }

        [Test]
        public static void SlerpTestPitch()
        {
            float angle1 = 0;
            float3 euler1 = new float3(MathHelper.DegreesToRadians(angle1), 0, 0);
            Quaternion quaternion1 = Quaternion.EulerToQuaternion(euler1);

            for (float angle2 = startAngle; angle2 <= endAngle; angle2 += 1.0f)
            {
                float3 euler2 = new float3(MathHelper.DegreesToRadians(angle2), 0, 0);

                Quaternion quaternion2 = Quaternion.EulerToQuaternion(euler2);

                Quaternion quaternionMiddle = Quaternion.Slerp(quaternion1, quaternion2, 0.5f);

                float3 eulerRes = Quaternion.QuaternionToEuler(quaternionMiddle);

                float angleExp = CalcExpected(angle1, angle2);
                float3 eulerExp = new float3(MathHelper.DegreesToRadians(angleExp), 0, 0);

                Assert.AreEqual(eulerExp, eulerRes);
            }
        }


        [Test]
        public static void NormalizeTest()
        {
            float angle1 = 45;
            float3 euler1 = new float3(MathHelper.DegreesToRadians(angle1), 0, 0);
            float3 euler2 = new float3(MathHelper.DegreesToRadians(360 + angle1), 0, 0);

            Quaternion quaternion1 = Quaternion.EulerToQuaternion(euler1);
            Quaternion quaternion2 = Quaternion.EulerToQuaternion(euler2);

            Assert.AreNotEqual(quaternion1, quaternion2);

            Quaternion quaternion3 = Quaternion.Normalize(quaternion2);

            Assert.AreEqual(quaternion1, quaternion3);
        }
    }
}
