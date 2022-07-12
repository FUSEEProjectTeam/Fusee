using Fusee.Math.Core;
using System.Numerics;

namespace Fusee.ImGuiImp.Desktop
{
    public static class FuseeMath2VectorExtensions
    {
        public static Vector2 ToNumericsVector(this float2 f)
        {
            return new Vector2(f.x, f.y);
        }

        public static Vector3 ToNumericsVector(this float3 f)
        {
            return new Vector3(f.x, f.y, f.z);
        }

        public static Vector4 ToNumericsVector(this float4 f)
        {
            return new Vector4(f.x, f.y, f.z, f.w);
        }

        public static float2 ToFuseeVector(this Vector2 f)
        {
            return new float2(f.X, f.Y);
        }

        public static float3 ToFuseeVector(this Vector3 f)
        {
            return new float3(f.X, f.Y, f.Z);
        }

        public static float4 ToFuseeVector(this Vector4 f)
        {
            return new float4(f.X, f.Y, f.Z, f.W);
        }

        public static uint ToUintColor(this Vector4 color)
        {
            uint col = 0;

            col += (uint)(color.X) | byte.MinValue;
            col += (uint)(color.Y) << 8 | byte.MinValue;
            col += (uint)(color.Z) << 16 | byte.MinValue;
            col += (uint)(color.W) << 24 | byte.MinValue;

            return col;
        }

        public static uint ToUintColor(this Vector3 color)
        {
            uint col = 0;

            col += (uint)(color.X) | byte.MinValue;
            col += (uint)(color.Y) << 8 | byte.MinValue;
            col += (uint)(color.Z) << 16 | byte.MinValue;

            return col;
        }

        public static uint ToUintColor(this float4 color)
        {
            uint col = 0;

            col += (uint)(color.x) | byte.MinValue;
            col += (uint)(color.y) << 8 | byte.MinValue;
            col += (uint)(color.z) << 16 | byte.MinValue;
            col += (uint)(color.w) << 24 | byte.MinValue;

            return col;
        }

        public static uint ToUintColor(this float3 color)
        {
            uint col = 0;

            col += (uint)(color.x) | byte.MinValue;
            col += (uint)(color.y) << 8 | byte.MinValue;
            col += (uint)(color.z) << 16 | byte.MinValue;

            return col;
        }
    }
}