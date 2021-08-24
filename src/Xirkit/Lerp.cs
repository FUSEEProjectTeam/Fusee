using Fusee.Math.Core;

namespace Fusee.Xirkit
{
    /// <summary>
    /// provides different LerpFunctions that can be used in a channel. A Lerp Function represents a linear interpolation between 2 points in a timeline.
    /// </summary>
    public static class Lerp
    {

        /// <summary>
        /// Lerp Function for Doubles.
        /// </summary>
        public static double DoubleLerp(double val1, double val2, float time1, float time2)
        {

            return (val1 + ((val2 - val1) / time1) * time2);
        }

        /// <summary>
        /// Lerp Function for Int.
        /// </summary>
        public static int IntLerp(int val1, int val2, float time1, float time2)
        {
            return (int)(val1 + ((val2 - val1) / time1) * time2);
        }

        /// <summary>
        /// Lerp Function for Float.
        /// </summary>
        public static float FloatLerp(float val1, float val2, float time1, float time2)
        {
            return (val1 + ((val2 - val1) / time1) * time2);
        }

        /// <summary>
        /// Lerp Function for Float2.
        /// </summary>
        public static float2 Float2Lerp(float2 val1, float2 val2, float time1, float time2)
        {
            float2 values = new()
            {
                x = val1.x + ((val2.x - val1.x) / time1) * time2,
                y = val1.y + ((val2.y - val1.y) / time1) * time2
            };
            return values;
        }

        /// <summary>
        /// Lerp Function for float3 values. Linearly interpolates the three components independently.
        /// </summary>
        public static float3 Float3Lerp(float3 val1, float3 val2, float time1, float time2)
        {
            float3 values = new()
            {
                x = val1.x + ((val2.x - val1.x) / time1) * time2,
                y = val1.y + ((val2.y - val1.y) / time1) * time2,
                z = val1.z + ((val2.z - val1.z) / time1) * time2
            };
            return values;
        }

        /// <summary>
        /// Slerp Function for float3 using quaternion interpolation. Useful if the given float3 values contain euler angles in Pitch(x)/Yaw(y)/Roll(z) order.
        /// The euler angle set returned by this method is on the shortest spherical path between the two parameter euler angle sets.
        /// Note that using linear interpolation <see cref="Float3Lerp"/> of angle values will NOT yield in a path lying on a great circle between the two parameters. Instead, using
        /// linear interpolation, the interpolated values will describe a curve called loxodrome which spirals around the poles (due to gimbal lock). 
        /// </summary>
        public static float3 Float3QuaternionSlerp(float3 val1, float3 val2, float time1, float time2)
        {
            Quaternion q1 = Quaternion.EulerToQuaternion(val1);
            Quaternion q2 = Quaternion.EulerToQuaternion(val2);
            Quaternion res = Quaternion.Slerp(q1, q2, time2 / time1);
            return Quaternion.QuaternionToEuler(res);
        }

        /// <summary>
        /// Lerp Function for Float4s.
        /// </summary>
        public static float4 Float4Lerp(float4 val1, float4 val2, float time1, float time2)
        {
            float4 values = new()
            {
                w = val1.w + ((val2.w - val1.w) / time1) * time2,
                x = val1.x + ((val2.x - val1.x) / time1) * time2,
                y = val1.y + ((val2.y - val1.y) / time1) * time2,
                z = val1.z + ((val2.z - val1.z) / time1) * time2
            };
            return values;
        }
    }
}