﻿using Fusee.Math.Core;
using System;

namespace Fusee.PointCloud.Potree.V2.Data
{
    /// <summary>
    /// This point is used for visualization purposes.
    /// It is read from a file and converted to mesh data.
    /// </summary>
    public struct VisualizationPoint
    {
        /// <summary>
        /// How Flags should be converted by ToString.
        /// </summary>
        public static Func<uint, string> FlagsParser = (flags) => flags.ToString();

        /// <summary>
        /// The position of a point.
        /// </summary>
        public float3 Position;

        /// <summary>
        /// The color (r,g,b,a) of a point.
        /// </summary>
        public float4 Color;

        /// <summary>
        /// Flags have to be interpreted manually or they will be ignored.
        /// </summary>

        public uint Flags;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Position: {Position} - Color: {Color} - Flags: {FlagsParser(Flags)}";
        }
    }
}