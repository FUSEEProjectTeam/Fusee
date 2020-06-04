using Fusee.Engine.Core.ShaderShards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Engine.Core.Effects
{

    internal enum CompiledEffectUsage
    {
        Unknown,
        Forward,
        Deferred,
    }

    internal class RenderIndependendEffect
    {
        public readonly CompiledEffectUsage EffectUsage;

        public readonly string VS;
        public readonly string PS;
        public readonly string GS;

        public RenderIndependendEffect(CompiledEffectUsage effectUsage, Effect fx)
        {
            EffectUsage = effectUsage;
            var efType = fx.GetType();
            if (efType == typeof(ShaderEffect))
            {
                var shaderEffect = (ShaderEffect)fx;
                VS = shaderEffect.VertexShaderSrc;
                GS = shaderEffect.GeometryShaderSrc;
                PS = shaderEffect.PixelShaderSrc;
            }
            else
            {
                var surfEffect = (SurfaceEffect)fx;

                surfEffect.VertexShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Main, ShaderShards.Vertex.VertMain.VertexMain(surfEffect.LightingSetup)));

                if (effectUsage == CompiledEffectUsage.Forward)
                {
                    foreach (var dcl in SurfaceEffect.CreateForwardLightingParamDecls(ShaderShards.Fragment.Lighting.NumberOfLightsForward))
                        surfEffect.ParamDecl.Add(dcl.Name, dcl);

                    surfEffect.FragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Method, ShaderShards.Fragment.Lighting.AssembleLightingMethods(surfEffect.LightingSetup)));
                    surfEffect.FragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Main, ShaderShards.Fragment.FragMain.ForwardLighting(surfEffect.LightingSetup, nameof(surfEffect.SurfaceInput), SurfaceOut.StructName)));
                    surfEffect.FragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, ShaderShards.Fragment.Lighting.LightStructDeclaration));
                    surfEffect.FragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, ShaderShards.Fragment.FragProperties.FixedNumberLightArray));
                    surfEffect.FragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, ShaderShards.Fragment.FragProperties.ColorOut()));
                    surfEffect.FragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                }
                else if (effectUsage == CompiledEffectUsage.Deferred)
                {
                    surfEffect.FragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Property, ShaderShards.Fragment.FragProperties.GBufferOut()));
                    surfEffect.FragmentShaderSrc.Add(new KeyValuePair<ShardCategory, string>(ShardCategory.Main, ShaderShards.Fragment.FragMain.RenderToGBuffer(surfEffect.LightingSetup, nameof(surfEffect.SurfaceInput), SurfaceOut.StructName)));
                    surfEffect.FragmentShaderSrc.Sort((x, y) => (x.Key.CompareTo(y.Key)));
                }
                else
                {
                    throw new ArgumentException($"{effectUsage} isn't valid when using SurfaceEffects!");
                }

                VS = SurfaceEffect.JoinShards(surfEffect.VertexShaderSrc);
                GS = SurfaceEffect.JoinShards(surfEffect.GeometryShaderSrc);
                PS = SurfaceEffect.JoinShards(surfEffect.FragmentShaderSrc);
            }

        }
    }
}