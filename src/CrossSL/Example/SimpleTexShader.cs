#pragma warning disable 0649

using System;
using CrossSL.Meta;
using Fusee.Math;

namespace Example
{
    [xSLTarget(xSLTarget.GLSLMix.V110)]
    [xSLDebug(xSLDebug.PreCompile | xSLDebug.SaveToFile)]
    public class SimpleTexShader : xSLShader
    {
        [xSLAttribute] internal float3 FuVertex;
        [xSLAttribute] internal float3 FuNormal;
        [xSLAttribute] internal float2 FuUV;

        [xSLVarying] private float3 _vNormal;
        [xSLVarying] private float2 _vUV;

        [xSLUniform] internal float4x4 FuseeMVP;
        [xSLUniform] internal float4x4 FuseeITMV;

        [xSLUniform] private sampler2D _texture1;

        public override void VertexShader()
        {
            _vUV = FuUV;
            _vNormal = new float3x3(FuseeITMV)*FuNormal;

            xslPosition = FuseeMVP*new float4(FuVertex, 1.0f);
        }

        [xSLPrecision(floatPrecision = xSLPrecision.Medium)]
        public override void FragmentShader()
        {
            var value = Math.Max(float3.Dot(new float3(0, 0, 1), float3.Normalize(_vNormal)), 0.2f);
            xslFragColor = value*Texture2D(_texture1, _vUV);
        }
    }
}

#pragma warning restore 0649