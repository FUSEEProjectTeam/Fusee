using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.ShaderShards;
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
        public int MaxNoOfVisiblePoints = 1000000;

        public string PathToOocFile = "D://PW_ooc//Demo_A_06-Cloud02";

        public ShaderEffect DepthPassEf;
        public PointCloudSurfaceEffect ColorPassEf;

        private Lighting _lighting = Lighting.Edl;
        public Lighting Lighting
        {
            get { return _lighting; }
            set
            {
                _lighting = value;
                ColorPassEf.EDLStrength = _edlStrength == 0.0f ? (int)Lighting.Unlit : (int)Lighting.Edl;
            }
        }

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

        private ColorMode _colorMode = ColorMode.VertexColor0;

        public ColorMode ColorMode
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

        private float _edlStrength = 0.1f;
        public float EdlStrength
        {
            get { return _edlStrength; }
            set
            {
                _edlStrength = value;
                ColorPassEf.EDLStrength = _edlStrength;
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

                new FxParamDeclaration<int> {Name = UniformNameDeclarations.PointSize, Value = Size},
                new FxParamDeclaration<int> {Name = UniformNameDeclarations.PointShape, Value = (int)Shape},
                new FxParamDeclaration<int> {Name = UniformNameDeclarations.PointSizeMode, Value = (int)PtMode},
            });
        }

        internal PointCloudSurfaceEffect CreateColorPassEffect()
        {
            var fx = new PointCloudSurfaceEffect
            {
                PointSize = 5,
                ColorMode = (int)ColorMode,
                PointShape = (int)Shape,
                DepthTex = null,
                EDLStrength = 1f,
                EDLNeighbourPixels = 2
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