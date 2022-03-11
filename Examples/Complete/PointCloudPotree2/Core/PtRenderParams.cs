using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.ShaderShards;
using Fusee.PointCloud.Common;
using System;

namespace Fusee.Examples.PointCloudPotree2.Core
{
    public delegate void PointThresholdHandler(int val);
    public delegate void ProjectedSizeModifierHandler(float val);

    public sealed class PtRenderingParams : IDisposable
    {
        public static PtRenderingParams Instance { get; private set; } = new();

        public PointThresholdHandler PointThresholdHandler;
        public ProjectedSizeModifierHandler ProjectedSizeModifierHandler;

        public string PathToOocFile = @"Assets\Cube1030301\Potree";

        public ShaderEffect DepthPassEf;
        public PointCloudSurfaceEffect ColorPassEf;

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

        private float _projSizeMod = 0.0001f;
        public float ProjectedSizeModifier
        {
            get { return _projSizeMod; }
            set
            {
                _projSizeMod = value;
                ProjectedSizeModifierHandler(_projSizeMod);
            }
        }

        private int _ptThreshold = 1000000;

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