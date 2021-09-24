using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Fusee.Examples.PointCloudOutOfCore.Core
{
    public sealed class PtRenderingParams : IDisposable
    {
        public static PtRenderingParams Instance { get; private set; } = new();

        public ConcurrentDictionary<int, object> ShaderParamsToUpdate = new();
        public int MaxNoOfVisiblePoints = 500000;
        public string PathToOocFile = "D://HolbeinPferdOctree";

        public ShaderEffect DepthPassEf;
        public ShaderEffect ColorPassEf;

        
        private int _lightingParamHash = "Lighting".GetHashCode();
        private int _pointShapeParamHash = "PointShape".GetHashCode();
        private int _pointModeParamHash = "PointMode".GetHashCode();
        private int _colorModeParamHash = "ColorMode".GetHashCode();
        private int _shininessParamHash = "Shininess".GetHashCode();
        private int _pointSizeParamHash = "PointSize".GetHashCode();
        private int _colorParamHash = "Color".GetHashCode();
        private int _calcSSAOParamHash = "CalcSSAO".GetHashCode();
        private int _ssaoStrengthParamHash = "SSAOStrength".GetHashCode();
        private int _edlNeighbourPixelsParamHash = "EDLNeighbourPixels".GetHashCode();
        private int _edlStrengthParamHash = "EDLStrength".GetHashCode();
        private int _specularStrengthParamHash = "SpecularStrength".GetHashCode();

        private Lighting _lighting = Lighting.Edl;
        public Lighting Lighting
        {
            get { return _lighting; }
            set
            {
                _lighting = value;
                ShaderParamsToUpdate.AddOrUpdate(_lightingParamHash, (int)Lighting, (key, val) => val);
            }
        }

        private PointShape _shape = PointShape.Paraboloid;
        public PointShape Shape
        {
            get { return _shape; }
            set
            {
                _shape = value;
                ShaderParamsToUpdate.AddOrUpdate(_pointShapeParamHash, (int)Shape, (key, val) => val);
            }
        }

        private PointSizeMode _ptMode = PointSizeMode.FixedPixelSize;
        public PointSizeMode PtMode
        {
            get { return _ptMode; }
            set
            {
                _ptMode = value;
                ShaderParamsToUpdate.AddOrUpdate(_pointModeParamHash, (int)PtMode, (key, val) => val);
            }
        }

        private ColorMode _colorMode = ColorMode.Single;

        public ColorMode ColorMode
        {
            get { return _colorMode; }
            set
            {
                _colorMode = value;
                ShaderParamsToUpdate.AddOrUpdate(_colorModeParamHash, (int)ColorMode, (key, val) => val);
            }
        }

        private int _size = 10;
        public int Size
        {
            get { return _size; }
            set
            {
                _size = value;
                ShaderParamsToUpdate.AddOrUpdate(_pointSizeParamHash, Size, (key, val) => val);
            }
        }

        private float4 _singleColor = new(0.8f, 0.8f, 0.8f, 1);
        public float4 SingleColor
        {
            get => _singleColor;
            set
            {
                _singleColor = value;
                _ = ShaderParamsToUpdate.AddOrUpdate(_colorParamHash, SingleColor, (key, val) => val);
            }
        }

        private bool _calcSSAO = false;
        public bool CalcSSAO
        {
            get { return _calcSSAO; }
            set
            {
                _calcSSAO = value;
                ShaderParamsToUpdate.AddOrUpdate(_calcSSAOParamHash, CalcSSAO ? 1 : 0, (key, val) => val);
            }
        }

        private float _ssaoStrength = 0.2f;
        public float SSAOStrength
        {
            get { return _ssaoStrength; }
            set
            {
                _ssaoStrength = value;
                ShaderParamsToUpdate.AddOrUpdate(_ssaoStrengthParamHash, SSAOStrength, (key, val) => val);
            }
        }

        private int _edlNoOfNeighbourPx = 2;
        public int EdlNoOfNeighbourPx
        {
            get { return _edlNoOfNeighbourPx; }
            set
            {
                _edlNoOfNeighbourPx = value;
                ShaderParamsToUpdate.AddOrUpdate(_edlNeighbourPixelsParamHash, EdlNoOfNeighbourPx, (key, val) => val);
            }
        }

        private float _edlStrength = 0.1f;
        public float EdlStrength
        {
            get { return _edlStrength; }
            set
            {
                _edlStrength = value;
                ShaderParamsToUpdate.AddOrUpdate(_edlStrengthParamHash, EdlStrength, (key, val) => val);
            }
        }

        private float _specularStrength = 0.2f;
        public float SpecularStrength
        {
            get { return _specularStrength; }
            set
            {
                _specularStrength = value;
                ShaderParamsToUpdate.AddOrUpdate(_specularStrengthParamHash, SpecularStrength, (key, val) => val);
            }
        }

        private float _shininess = 2000;
        public float Shininess
        {
            get { return _shininess; }
            set
            {
                _shininess = value;
                ShaderParamsToUpdate.AddOrUpdate(_shininessParamHash, Shininess, (key, val) => val);
            }
        }

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static PtRenderingParams()
        {
        }

        internal ShaderEffect CreateDepthPassEffect(float2 screenParams, float initCamPosZ, Texture octreeTex, double3 octreeRootCenter, double octreeRootLength)
        {
            return new ShaderEffect(
            new FxPassDeclaration
            {
                VS = AssetStorage.Get<string>("PointCloud.vert"),
                PS = AssetStorage.Get<string>("PointDepth.frag"),
                StateSet = new RenderStateSet
                {
                    AlphaBlendEnable = true,
                    ZEnable = true,
                }
            },
            new List<IFxParamDeclaration>
            {
                new FxParamDeclaration<float4x4> {Name = "FUSEE_MVP", Value = float4x4.Identity},
                new FxParamDeclaration<float4x4> {Name = "FUSEE_MV", Value = float4x4.Identity},
                new FxParamDeclaration<float4x4> {Name = "FUSEE_M", Value = float4x4.Identity},
                new FxParamDeclaration<float4x4> {Name = "FUSEE_P", Value = float4x4.Identity},
                new FxParamDeclaration<float4x4> {Name = "FUSEE_IV", Value = float4x4.Identity},
                new FxParamDeclaration<float4x4> {Name = "FUSEE_V", Value = float4x4.Identity},

                new FxParamDeclaration<float2> {Name = "ScreenParams", Value = screenParams},
                new FxParamDeclaration<float> {Name = "InitCamPosZ", Value = System.Math.Abs(initCamPosZ)},

                new FxParamDeclaration<int> {Name = "PointSize", Value = Size},
                new FxParamDeclaration<int> {Name = "PointShape", Value = (int)Shape},
                new FxParamDeclaration<int> {Name = "PointMode", Value = (int)PtMode},

                new FxParamDeclaration<float> {Name = "OctantRes", Value = 0f},
                new FxParamDeclaration<int> {Name = "OctantLevel", Value = 0},

                new FxParamDeclaration<Texture> {Name = "OctreeTex", Value = octreeTex},
                new FxParamDeclaration<int> {Name = "OctreeTexWidth", Value = octreeTex.Width}, //Used to access a specific pixel in the tex
                new FxParamDeclaration<float3> {Name = "OctreeRootCenter", Value = (float3)octreeRootCenter},
                new FxParamDeclaration<float> {Name = "OctreeRootLength", Value = (float)octreeRootLength}
            });
        }

        internal ShaderEffect CreateColorPassEffect(float2 screenParams, float initCamPosZ, float2 clipPlaneDist, WritableTexture depthTex, Texture octreeTex, double3 octreeRootCenter, double octreeRootLength)
        {
            var kernelLength = 32;
            var ssaoKernel = FuseeSsaoHelper.CreateKernel(kernelLength);
            var ssaoNoiseTex = FuseeSsaoHelper.CreateNoiseTex(32);

            return new ShaderEffect(
                new FxPassDeclaration
                {
                    VS = AssetStorage.Get<string>("PointCloud.vert"),
                    PS = AssetStorage.Get<string>("PointCloud.frag"),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = true,
                        ZEnable = true,
                    }
                },
            new List<IFxParamDeclaration>
            {
                new FxParamDeclaration<float4x4> {Name = "FUSEE_ITMV", Value = float4x4.Identity},
                new FxParamDeclaration<float4x4> {Name = "FUSEE_MVP", Value = float4x4.Identity},
                new FxParamDeclaration<float4x4> {Name = "FUSEE_MV", Value = float4x4.Identity},
                new FxParamDeclaration<float4x4> {Name = "FUSEE_M", Value = float4x4.Identity},
                new FxParamDeclaration<float4x4> {Name = "FUSEE_P", Value = float4x4.Identity},
                new FxParamDeclaration<float4x4> {Name = "FUSEE_IV", Value = float4x4.Identity},
                new FxParamDeclaration<float4x4> {Name = "FUSEE_V", Value = float4x4.Identity},

                new FxParamDeclaration<float2> {Name = "ClipPlaneDist", Value = clipPlaneDist},
                new FxParamDeclaration<float2> {Name = "ScreenParams", Value = screenParams},
                new FxParamDeclaration<float> {Name = "InitCamPosZ", Value = System.Math.Abs(initCamPosZ)},
                new FxParamDeclaration<float4> {Name = "Color", Value = SingleColor},

                new FxParamDeclaration<int> {Name = "PointMode", Value = (int)PtMode},
                new FxParamDeclaration<int> {Name = "PointSize", Value = Size},
                new FxParamDeclaration<int> {Name = "PointShape", Value = (int)Shape},
                new FxParamDeclaration<int> {Name = "ColorMode", Value = (int)ColorMode},

                new FxParamDeclaration<int> {Name = "Lighting", Value = (int)Lighting},
                new FxParamDeclaration<WritableTexture>{Name = "DepthTex", Value = depthTex},
                new FxParamDeclaration<float>{Name = "EDLStrength", Value = EdlStrength},
                new FxParamDeclaration<int>{Name = "EDLNeighbourPixels", Value = EdlNoOfNeighbourPx},
                new FxParamDeclaration<float> {Name = "SpecularStrength", Value = SpecularStrength},
                new FxParamDeclaration<float> {Name = "Shininess", Value = Shininess},
                new FxParamDeclaration<float4> {Name = "SpecularColor", Value = new float4(1,1,1,1)},

                new FxParamDeclaration<float> {Name = "OctantRes", Value = 0f},
                new FxParamDeclaration<int> {Name = "OctantLevel", Value = 0},

                new FxParamDeclaration<Texture> {Name = "OctreeTex", Value = octreeTex},
                new FxParamDeclaration<int> {Name = "OctreeTexWidth", Value = octreeTex.Width}, //Used to access a specific pixel in the tex
                new FxParamDeclaration<float3> {Name = "OctreeRootCenter", Value = (float3)octreeRootCenter},
                new FxParamDeclaration<float> {Name = "OctreeRootLength", Value = (float)octreeRootLength},

                new FxParamDeclaration<float3[]> {Name = "SSAOKernel[0]", Value = ssaoKernel},
                new FxParamDeclaration<Texture> {Name = "NoiseTex", Value = ssaoNoiseTex},
                new FxParamDeclaration<int> {Name = "CalcSSAO", Value = CalcSSAO ? 1 : 0},
                new FxParamDeclaration<float> {Name = "SSAOStrength", Value = SSAOStrength}
            });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;

        void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                if (Instance != null)
                {
                    Instance = null;
                }

                // Note disposing has been done.
                _disposed = true;
            }

        }

        ~PtRenderingParams()
        {
            Dispose(false);
        }
    }
}