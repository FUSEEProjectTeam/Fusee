using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Pointcloud.Common;
using System.Collections.Concurrent;

namespace Fusee.Examples.PcRendering.Core
{
    public static class PtRenderingParams
    {
        public static ConcurrentDictionary<string, object> ShaderParamsToUpdate = new ConcurrentDictionary<string, object>();
        public static int MaxNoOfVisiblePoints = 500000;
        public static string PathToOocFile = "D://HolbeinPferdOctree";

        public static ShaderEffect DepthPassEf;
        public static ShaderEffect ColorPassEf;

        private static Lighting _lighting = Lighting.EDL;
        public static Lighting Lighting
        {
            get { return _lighting; }
            set
            {
                _lighting = value;
                ShaderParamsToUpdate.AddOrUpdate("Lighting", (int)Lighting, (key, val) => val);
            }
        }

        private static PointShape _shape = PointShape.PARABOLID;
        public static PointShape Shape
        {
            get { return _shape; }
            set
            {
                _shape = value;
                ShaderParamsToUpdate.AddOrUpdate("PointShape", (int)Shape, (key, val) => val);
            }
        }

        private static PointSizeMode _ptMode = PointSizeMode.ADAPTIVE_SIZE;
        public static PointSizeMode PtMode
        {
            get { return _ptMode; }
            set
            {
                _ptMode = value;
                ShaderParamsToUpdate.AddOrUpdate("PointMode", (int)PtMode, (key, val) => val);
            }
        }

        private static ColorMode _colorMode = ColorMode.SINGLE;
        public static ColorMode ColorMode
        {
            get { return _colorMode; }
            set
            {
                _colorMode = value;
                ShaderParamsToUpdate.AddOrUpdate("ColorMode", (int)ColorMode, (key, val) => val);
            }
        }

        private static int _size = 10;
        public static int Size
        {
            get { return _size; }
            set
            {
                _size = value;
                ShaderParamsToUpdate.AddOrUpdate("PointSize", Size, (key, val) => val);
            }
        }

        private static float4 _singleColor = new float4(0.8f, 0.8f, 0.8f, 1);
        public static float4 SingleColor
        {
            get { return _singleColor; }
            set
            {
                _singleColor = value;
                ShaderParamsToUpdate.AddOrUpdate("Color", SingleColor, (key, val) => val);
            }
        }

        private static bool _calcSSAO = false;
        public static bool CalcSSAO
        {
            get { return _calcSSAO; }
            set
            {
                _calcSSAO = value;
                ShaderParamsToUpdate.AddOrUpdate("CalcSSAO", CalcSSAO ? 1 : 0, (key, val) => val);
            }
        }

        private static float _ssaoStrength = 0.2f;
        public static float SSAOStrength
        {
            get { return _ssaoStrength; }
            set
            {
                _ssaoStrength = value;
                ShaderParamsToUpdate.AddOrUpdate("SSAOStrength", SSAOStrength, (key, val) => val);
            }
        }

        private static int _edlNoOfNeighbourPx = 2;
        public static int EdlNoOfNeighbourPx
        {
            get { return _edlNoOfNeighbourPx; }
            set
            {
                _edlNoOfNeighbourPx = value;
                ShaderParamsToUpdate.AddOrUpdate("EDLNeighbourPixels", EdlNoOfNeighbourPx, (key, val) => val);
            }
        }

        private static float _edlStrength = 0.1f;
        public static float EdlStrength
        {
            get { return _edlStrength; }
            set
            {
                _edlStrength = value;
                ShaderParamsToUpdate.AddOrUpdate("EDLStrength", EdlStrength, (key, val) => val);
            }
        }

        private static float _specularStrength = 0.2f;
        public static float SpecularStrength
        {
            get { return _specularStrength; }
            set
            {
                _specularStrength = value;
                ShaderParamsToUpdate.AddOrUpdate("SpecularStrength", SpecularStrength, (key, val) => val);
            }
        }

        private static float _shininess = 2000;
        public static float Shininess
        {
            get { return _shininess; }
            set
            {
                _shininess = value;
                ShaderParamsToUpdate.AddOrUpdate("Shininess", Shininess, (key, val) => val);
            }
        }

        internal static ShaderEffect CreateDepthPassEffect(float2 screenParams, float initCamPosZ, Texture octreeTex, double3 octreeRootCenter, double octreeRootLength)
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

        internal static ShaderEffect CreateColorPassEffect(float2 screenParams, float initCamPosZ, float2 clipPlaneDist, WritableTexture depthTex, Texture octreeTex, double3 octreeRootCenter, double octreeRootLength)
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
                new EffectParameterDeclaration{Name = "DepthTex", Value = depthTex},
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
                new EffectParameterDeclaration {Name = "SSAOStrength", Value = SSAOStrength}
            });
        }
    }
}
