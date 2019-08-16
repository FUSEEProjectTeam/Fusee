using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Pointcloud.Common;
using System;
using System.Collections.Generic;

namespace Fusee.Examples.PcRendering.Core
{

    public static class PtRenderingParams
    {
        public static int MaxNoOfVisiblePoints = 500000;

        public static string PathToOocFile = "E://HolbeinPferdOctree";

        public static Lighting Lighting = Lighting.EDL;

        public static PointShape Shape = PointShape.PARABOLID;
        public static PointSizeMode PtMode = PointSizeMode.ADAPTIVE_SIZE;
        public static ColorMode ColorMode = ColorMode.SINGLE;
        public static int Size = 20;        
        public static float4 SingleColor = new float4(0.8f, 0.8f, 0.8f, 1);

        public static bool CalcSSAO = false;
        public static float SSAOStrength = 0.2f;

        public static int EdlNoOfNeighbourPx = 2;
        public static float EdlStrength = 0.1f;

        public static float SpecularStrength = 0.2f;
        public static float Shininess = 2000;

        internal static ShaderEffect DepthPassEffect(float2 screenParams, float initCamPosZ, Texture octreeTex, double3 octreeRootCenter, double octreeRootLength)
        {
            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = AssetStorage.Get<string>("PointCloud.vert"),
                    PS = AssetStorage.Get<string>("PointDepth.frag"),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = true,
                        ZEnable = true,
                    }
                }
            },
            new[]
            {
                new EffectParameterDeclaration {Name = "FUSEE_MVP", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_MV", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_M", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_P", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_IV", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_V", Value = float4x4.Identity},

                new EffectParameterDeclaration {Name = "ScreenParams", Value = screenParams},
                new EffectParameterDeclaration {Name = "InitCamPosZ", Value = System.Math.Abs(initCamPosZ)},

                new EffectParameterDeclaration {Name = "PointSize", Value = Size},
                new EffectParameterDeclaration {Name = "PointShape", Value = (int)Shape},
                new EffectParameterDeclaration {Name = "PointMode", Value = (int)PtMode},

                new EffectParameterDeclaration {Name = "OctantRes", Value = 0f},
                new EffectParameterDeclaration {Name = "OctantLevel", Value = 0},

                new EffectParameterDeclaration {Name = "OctreeTex", Value = octreeTex},
                new EffectParameterDeclaration {Name = "OctreeTexWidth", Value = octreeTex.Width}, //Used to access a specific pixel in the tex
                new EffectParameterDeclaration {Name = "OctreeRootCenter", Value = (float3)octreeRootCenter},
                new EffectParameterDeclaration {Name = "OctreeRootLength", Value = (float)octreeRootLength}
            });
        }

        internal static ShaderEffect ColorPassEffect(float2 screenParams, float initCamPosZ, float2 clipPlaneDist, ITextureHandle depthTexHandle, Texture octreeTex, double3 octreeRootCenter, double octreeRootLength)
        {
            var kernelLength = 32;
            var ssaoKernel = SSAOHelper.CreateKernel(kernelLength);            
            var ssaoNoiseTex = SSAOHelper.CreateNoiseTex(32);

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = AssetStorage.Get<string>("PointCloud.vert"),
                    PS = AssetStorage.Get<string>("PointCloud.frag"),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = true,
                        ZEnable = true,
                    }
                }
            },
            new[]
            {
                new EffectParameterDeclaration {Name = "FUSEE_ITMV", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_MVP", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_MV", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_M", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_P", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_IV", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_V", Value = float4x4.Identity},

                new EffectParameterDeclaration {Name = "ClipPlaneDist", Value = clipPlaneDist},
                new EffectParameterDeclaration {Name = "ScreenParams", Value = screenParams},
                new EffectParameterDeclaration {Name = "InitCamPosZ", Value = System.Math.Abs(initCamPosZ)},
                new EffectParameterDeclaration {Name = "Color", Value = SingleColor},

                new EffectParameterDeclaration {Name = "PointMode", Value = (int)PtMode},
                new EffectParameterDeclaration {Name = "PointSize", Value = Size},
                new EffectParameterDeclaration {Name = "PointShape", Value = (int)Shape},
                new EffectParameterDeclaration {Name = "ColorMode", Value = (int)ColorMode},

                new EffectParameterDeclaration {Name = "Lighting", Value = (int)Lighting},
                new EffectParameterDeclaration{Name = "DepthTex", Value = depthTexHandle},
                new EffectParameterDeclaration{Name = "EDLStrength", Value = EdlStrength},
                new EffectParameterDeclaration{Name = "EDLNeighbourPixels", Value = EdlNoOfNeighbourPx},
                new EffectParameterDeclaration {Name = "SpecularStrength", Value = SpecularStrength},
                new EffectParameterDeclaration {Name = "Shininess", Value = Shininess},
                new EffectParameterDeclaration {Name = "SpecularColor", Value = new float4(1,1,1,1)},

                new EffectParameterDeclaration {Name = "OctantRes", Value = 0f},
                new EffectParameterDeclaration {Name = "OctantLevel", Value = 0},

                new EffectParameterDeclaration {Name = "OctreeTex", Value = octreeTex},
                new EffectParameterDeclaration {Name = "OctreeTexWidth", Value = octreeTex.Width}, //Used to access a specific pixel in the tex
                new EffectParameterDeclaration {Name = "OctreeRootCenter", Value = (float3)octreeRootCenter},
                new EffectParameterDeclaration {Name = "OctreeRootLength", Value = (float)octreeRootLength},

                new EffectParameterDeclaration {Name = "SSAOKernel[0]", Value = ssaoKernel},
                new EffectParameterDeclaration {Name = "NoiseTex", Value = ssaoNoiseTex},
                new EffectParameterDeclaration {Name = "CalcSSAO", Value = CalcSSAO ? 1 : 0},
                new EffectParameterDeclaration {Name = "SSAOStrength", Value = SSAOStrength},              


            });
        }

       
    }


}
