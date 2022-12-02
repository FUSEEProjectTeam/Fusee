﻿using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.ShaderShards;
using Fusee.PointCloud.Common;
using System.IO;

namespace Fusee.Examples.PointCloudPotree2.Core
{
    public delegate void PointThresholdHandler(int val);
    public delegate void ProjectedSizeModifierHandler(float val);

    public sealed class PointRenderingParams
    {
        public static PointRenderingParams Instance { get; private set; } = new();

        public PointThresholdHandler PointThresholdHandler;
        public ProjectedSizeModifierHandler ProjectedSizeModifierHandler;

        public string PathToOocFile = Path.Combine("Assets", "Cube1030301", "Potree");

        public ShaderEffect DepthPassEf;
        public SurfaceEffectPointCloud ColorPassEf;

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
        static PointRenderingParams()
        {
        }
    }
}