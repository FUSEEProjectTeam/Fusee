using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance.Buffers;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using Fusee.PointCloud.Potree.V2.Data;
using System;

namespace Fusee.PointCloud.Potree.V2
{
    /// <summary>
    /// Delegate for a method that knows how to parse a enxtra byte uint back to its byte representation.
    /// </summary>
    /// <param name="flag"></param>
    /// <returns></returns>
    public delegate Span<byte> HandleWriteExtraBytes(uint flag);

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
        /// <param name="potreeSettingsAttribute"></param>
        public void WriteVisualizationPoint(OctantId octantId, MemoryOwner<VisualizationPoint> visualizationPoints, PotreeSettingsAttribute potreeSettingsAttribute)
        {
            Guard.IsNotNull(PotreeData);
            var node = PotreeData.GetNode(octantId);

            // if node is null the hierarchy is broken and we look for an octant that isn't there...
            Guard.IsNotNull(node);

            WriteVisualizationPoint(node, visualizationPoints, potreeSettingsAttribute);
        }

        private void WriteVisualizationPoint(PotreeNode potreeNode, MemoryOwner<VisualizationPoint> visualizationPoints, PotreeSettingsAttribute potreeSettingsAttribute)
        {
            Guard.IsLessThanOrEqualTo(potreeNode.NumPoints, int.MaxValue);
            Guard.IsNotNull(PotreeData);
            Guard.IsNotNull(HandleWriteExtraBytes);

            var pointArray = ReadRawNodeData(potreeNode);

            var visualizationArray = visualizationPoints.Span;
            var visualizationIdx = 0;

            for (int i = 0; i < pointArray.Length; i += PotreeData.Metadata.PointSize)
            {
                var attributeSlice = new Span<byte>(pointArray).Slice(i + potreeSettingsAttribute.AttributeOffset, potreeSettingsAttribute.Size);

                HandleWriteExtraBytes(visualizationArray[visualizationIdx].Flags).CopyTo(attributeSlice);

                visualizationIdx++;
            }

            WriteRawNodeData(potreeNode, pointArray);
        }

        private void WriteRawNodeData(PotreeNode potreeNode, byte[] rawNodeData)
        {
            Guard.IsNotNull(PotreeData);
            Guard.IsNotNull(rawNodeData);

            var potreePointSize = (int)potreeNode.NumPoints * PotreeData.Metadata.PointSize;
            PotreeData.WriteViewAccessor.WriteArray(potreeNode.ByteOffset, rawNodeData, 0, potreePointSize);
        }
    }
}