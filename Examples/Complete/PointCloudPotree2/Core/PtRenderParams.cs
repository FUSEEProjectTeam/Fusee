using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using System;
using System.Collections.Generic;

namespace Fusee.Examples.PointCloudPotree2.Core
{
    public delegate void PointThresholdHandler(int val);
    public delegate void ProjectedSizeModifierHandler(float val);

    public sealed class PtRenderingParams : IDisposable
    {
        public static PtRenderingParams Instance { get; private set; } = new();

        public PointThresholdHandler PointThresholdHandler;
        public ProjectedSizeModifierHandler ProjectedSizeModifierHandler;

        public string PathToOocFile = @"D:\Dokumente\Git\PotreeSharp\AxisCloud\pointclouds";

        public ShaderEffect DepthPassEf;
        public PointCloudSurfaceEffect ColorPassEf;

        public PointType PointType;

        private PointShape _shape = PointShape.Paraboloid;
        public PointShape Shape
        {
            get { return _shape; }
            set
            {
                _shape = value;
                ColorPassEf.PointShape = (int)_shape;
                DepthPassEf.SetFxParam(UniformNameDeclarations.PointShapeHash, (int)Shape);
            }
        }

        private PointSizeMode _ptMode = PointSizeMode.FixedPixelSize;
        public PointSizeMode PtMode
        {
            get { return _ptMode; }
            set
            {
                _ptMode = value;
                ColorPassEf.PointSizeMode = (int)_ptMode;
                DepthPassEf.SetFxParam(UniformNameDeclarations.PointSizeModeHash, (int)_ptMode);
            }
        }

        private PointColorMode _colorMode = PointColorMode.VertexColor0;

        public PointColorMode ColorMode
        {
            get { return _colorMode; }
            set
            {
                _colorMode = value;
                ColorPassEf.ColorMode = (int)_colorMode;
            }
        }

        private int _size = 3;
        public int Size
        {
            get { return _size; }
            set
            {
                _size = value;
                DepthPassEf.SetFxParam(UniformNameDeclarations.PointSizeHash, Size);
                ColorPassEf.PointSize = _size;
            }
        }

        private int _edlNoOfNeighbourPx = 1;
        public int EdlNoOfNeighbourPx
        {
            get { return _edlNoOfNeighbourPx; }
            set
            {
                _edlNoOfNeighbourPx = value;
                ColorPassEf.EDLNeighbourPixels = _edlNoOfNeighbourPx;
            }
        }

        private float _edlStrength = 0.3f;
        public float EdlStrength
        {
            get { return _edlStrength; }
            set
            {
                _edlStrength = value;
                ColorPassEf.EDLStrength = _edlStrength;
            }
        }

        private float _projSizeMod = 0.1f;
        public float ProjectedSizeModifier
        {
            get { return _projSizeMod; }
            set
            {
                _projSizeMod = value;
                ProjectedSizeModifierHandler(_projSizeMod);
            }
        }

        private int _ptThreshold = 5000000;

        public int PointThreshold
        {
            get { return _ptThreshold; }
            set
            {
                _ptThreshold = value;
                PointThresholdHandler(_ptThreshold);
            }
        }

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static PtRenderingParams()
        {
        }

        internal ShaderEffect CreateDepthPassEffect()
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
                new FxParamDeclaration<float4x4> {Name = UniformNameDeclarations.ModelViewProjection, Value = float4x4.Identity},
                new FxParamDeclaration<float4x4> {Name = UniformNameDeclarations.ModelView, Value = float4x4.Identity},
                new FxParamDeclaration<float4x4> {Name = UniformNameDeclarations.Projection, Value = float4x4.Identity},

                new FxParamDeclaration<float2> {Name = UniformNameDeclarations.ViewportPx, Value = float2.One},

                new FxParamDeclaration<int> {Name = UniformNameDeclarations.PointSize, Value = _size},
                new FxParamDeclaration<int> {Name = UniformNameDeclarations.PointShape, Value = (int)_shape},
                new FxParamDeclaration<int> {Name = UniformNameDeclarations.PointSizeMode, Value = (int)_ptMode},
            })
            {
                Active = false
            };
        }

        internal PointCloudSurfaceEffect CreateColorPassEffect()
        {
            var fx = new PointCloudSurfaceEffect
            {
                PointSize = _size,
                ColorMode = (int)_colorMode,
                PointShape = (int)_shape,
                DepthTex = null,
                EDLStrength = _edlStrength,
                EDLNeighbourPixels = _edlNoOfNeighbourPx
            };
            fx.SurfaceInput.Albedo = new float4(0.5f, 0.5f, 0.5f, 1.0f);
            return fx;
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