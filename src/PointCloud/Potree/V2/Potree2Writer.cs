using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance.Buffers;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using Fusee.PointCloud.Potree.V2.Data;
using System;
using System.Collections.Generic;

namespace Fusee.PointCloud.Potree.V2
{
    /// <summary>
    /// Delegate for a method that knows how to parse a info from a flag uint back to its byte representation.
    /// </summary>
    /// <param name="flag"></param>
    /// <param name="attrib">The attribute.</param>
    /// <returns></returns>
    public delegate Span<byte> HandleWriteExtraBytes(uint flag, PotreeSettingsAttribute attrib);

    /// <summary>
    /// Writes Potree data 
    /// </summary>
    public class Potree2Writer : Potree2AccessBase
    {
        /// <summary>
        /// Pass method how to handle the extra bytes, resulting uint will be passed into <see cref="Fusee.Engine.Core.Scene.Mesh.Flags"/>.
        /// </summary>
        public HandleWriteExtraBytes? HandleWriteExtraBytes { get; set; }

        /// <summary>
        /// Generate a <see cref="Potree2Writer"/> instance.
        /// </summary>
        public Potree2Writer(PotreeData potreeData) : base(potreeData) { }

        /// <summary>
        /// Writes <see cref="VisualizationPoint"/> for a node to disk.
        /// </summary>
        /// <param name="octantId"></param>
        /// <param name="visualizationPoints"></param>
        /// <param name="attribs"></param>
        public void WriteVisualizationPointForNode(OctantId octantId, MemoryOwner<VisualizationPoint> visualizationPoints, List<PotreeSettingsAttribute> attribs)
        {
            Guard.IsNotNull(PotreeData);
            var node = PotreeData.GetNode(octantId);

            // if node is null the hierarchy is broken and we look for an octant that isn't there...
            Guard.IsNotNull(node);

            WriteVisualizationPoints(node, visualizationPoints, attribs);
        }

        private void WriteVisualizationPoints(PotreeNode potreeNode, MemoryOwner<VisualizationPoint> visualizationPoints, List<PotreeSettingsAttribute> attribs)
        {
            Guard.IsLessThanOrEqualTo(potreeNode.NumPoints, int.MaxValue);
            Guard.IsNotNull(PotreeData);
            Guard.IsNotNull(HandleWriteExtraBytes);

            for (int i = 0; i < visualizationPoints.Length; i++)
            {
                foreach (var attrib in attribs)
                {
                    if (attrib != null)
                    {
                        var extraBytesRaw = HandleWriteExtraBytes(visualizationPoints.Span[i].Flags, attrib).ToArray();
                        PotreeData.WriteViewAccessor.WriteArray(potreeNode.ByteOffset + (PotreeData.Metadata.PointSize * i) + attrib.AttributeOffset, extraBytesRaw, 0, extraBytesRaw.Length);
                    }
                }
            }
        }
    }
}