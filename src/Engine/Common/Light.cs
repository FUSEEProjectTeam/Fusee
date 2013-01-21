using Fusee.Math;

namespace Fusee.Engine
{
    public enum LightType
    {
        Directional,
        Point,
        Spot
    }

    public struct Light
    {
        public float Active;
        public float4 AmbientColor;
        public float4 DiffuseColor;
        public float4 SpecularColor;
        public float3 Position;
        public float3 Direction;
        public LightType Type;
    }
}
