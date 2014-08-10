#pragma warning disable 0649

using System;
using CrossSL.Meta;
using Fusee.Math;

namespace Example
{
    [xSLTarget(xSLTarget.GLSLMix.V110)]
    [xSLDebug(xSLDebug.PreCompile | xSLDebug.SaveToFile)]
    public class DiffuseShader : xSLShader
    {
        // Vertex Shader
        [xSLAttribute] internal float3 FuColor;
        [xSLAttribute] internal float3 FuVertex;
        [xSLAttribute] internal float3 FuNormal;
        [xSLAttribute] internal float2 FuUV;

        [xSLVarying] private float3 _vNormal;
        [xSLVarying] private float2 _vUV;
        [xSLVarying] private float3 _vViewPos;

        [xSLUniform] internal float4x4 FuseeMVP;
        [xSLUniform] internal float4x4 FuseeMV;

        // Fragment Shader
        [xSLUniform] private sampler2D _texture1;

        [xSLUniform] internal int FuseeL0Active;
        [xSLUniform] internal float3 FuseeL0Pos;
        [xSLUniform] internal float3 FuseeL0Dir;
        [xSLUniform] internal float4 FuseeL0Diffuse;
        [xSLUniform] internal float4 FuseeL0Ambient;
        [xSLUniform] internal float FuseeL0Spotangle;

        // Vertex Shader
        public override void VertexShader()
        {
            var vViewPosTemp = FuseeMV*new float4(FuVertex, 1)*10;
            _vViewPos = new float3(vViewPosTemp)/vViewPosTemp.w;

            _vUV = FuUV;
            _vNormal = float3.Normalize(new float3x3(FuseeMV)*FuNormal);

            xslPosition = FuseeMVP*new float4(FuVertex, 1.0f);
        }

        // Fragment Shader
        private void CalcDirectLight(float4 difColor, float4 ambColor, float3 direction, ref float4 intensity)
        {
            intensity += ambColor;

            var dot = float3.Dot(-float3.Normalize(direction), float3.Normalize(_vNormal));
            intensity += Math.Max(dot, 0.0f)*difColor;
        }

        private void CalcPointLight(float4 difColor, float4 ambColor, float3 position, ref float4 intensity)
        {
            intensity += ambColor;
            var pos = position - _vViewPos;
            intensity += Math.Max(float3.Dot(float3.Normalize(pos), float3.Normalize(_vNormal)), 0.0f)*difColor;
        }

        private void CalcSpotLight(float4 difColor, float4 ambColor, float3 pos, float3 dir, float angle,
            ref float4 intensity)
        {
            intensity += ambColor;
            var tmppos = pos - _vViewPos;
            float alpha = float3.Dot(float3.Normalize(tmppos), float3.Normalize(dir));

            if (alpha < angle)
            {
                var dot = float3.Dot(float3.Normalize(tmppos), float3.Normalize(_vNormal));
                intensity += Math.Max(dot, 0.0f)*difColor;
            }
        }

        [xSLPrecision(floatPrecision = xSLPrecision.High)]
        public override void FragmentShader()
        {
            var endIntensity = new float4(0, 0, 0, 0);

            if (FuseeL0Active != 0)
            {
                if (FuseeL0Active == 1)
                {
                    var x = new float4(1, 9, 4, 8);
                    CalcDirectLight(x + FuseeL0Diffuse, FuseeL0Ambient, FuseeL0Pos, ref endIntensity);
                }

                if (FuseeL0Active == 2)
                    CalcPointLight(FuseeL0Diffuse, FuseeL0Ambient, FuseeL0Pos, ref endIntensity);

                if (FuseeL0Active == 3)
                    CalcSpotLight(FuseeL0Diffuse, FuseeL0Ambient, FuseeL0Pos, FuseeL0Dir, FuseeL0Spotangle,
                        ref endIntensity);
            }

            xslFragColor = Texture2D(_texture1, _vUV)*endIntensity;
        }
    }
}

#pragma warning restore 0649