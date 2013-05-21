using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusse.KeyFrameAnimation
{
    public static class Lerp
    {

        public static double DoubleLerp(double val1, double val2, float time1, float time2)
        {
            
            return (val1 + ((val2 - val1)/time1)*time2);
        }

        public static int IntLerp(int val1, int val2, float time1, float time2)
        {
            return (int)(val1 + ((val2 - val1) / time1) * time2);
        }

        public static float FloatLerp(float val1, float val2, float time1, float time2)
        {
            return (val1 + ((val2 - val1) / time1) * time2);
        }

        public static float2 Float2Lerp(float2 val1, float2 val2, float time1, float time2)
        {
            float2 values = new float2();
            values.x = val1.x + ((val2.x - val1.x)/time1)*time2;
            values.y = val1.y + ((val2.y - val1.y) / time1) * time2;
            return values;
        }

        public static float3 Float3Lerp(float3 val1, float3 val2, float time1, float time2)
        {
            float3 values = new float3();
            values.x = val1.x + ((val2.x - val1.x) / time1) * time2;
            values.y = val1.y + ((val2.y - val1.y) / time1) * time2;
            values.z = val1.z + ((val2.z - val1.z) / time1) * time2;
            return values;
        }

        public static float4 Float4Lerp(float4 val1, float4 val2, float time1, float time2)
        {
            float4 values = new float4();
            values.w = val1.w + ((val2.w - val1.w) / time1) * time2;
            values.x = val1.x + ((val2.x - val1.x) / time1) * time2;
            values.y = val1.y + ((val2.y - val1.y) / time1) * time2;
            values.z = val1.z + ((val2.z - val1.z) / time1) * time2;
            return values;
        }
    }
}
