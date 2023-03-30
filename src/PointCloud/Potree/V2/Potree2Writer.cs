using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance.Buffers;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Potree.V2.Data;

namespace Fusee.PointCloud.Potree.V2
{
    /// <summary>
    /// Writes Potree data 
    /// </summary>
    public class Potree2Writer : Potree2AccessBase
    {
        /// <summary>
        /// Generate a <see cref="Potree2Writer"/> instance.
        /// </summary>
        public Potree2Writer(PotreeData potreeData) : base(potreeData) { }

        /// <summary>
        /// Writes <see cref="VisualizationPoint"/> for a node to disk.
        /// </summary>
        /// <param name="octantId"></param>
        /// <param name="visualizationPoints"></param>
        public void WriteVisualizationPoint(OctantId octantId, MemoryOwner<VisualizationPoint> visualizationPoints)
        {
            Guard.IsNotNull(PotreeData);
            var node = PotreeData.GetNode(octantId);

            // if node is null the hierarchy is broken and we look for an octant that isn't there...
            Guard.IsNotNull(node);

            WriteVisualizationPoint(node, visualizationPoints);
        }

        private void WriteVisualizationPoint(PotreeNode potreeNode, MemoryOwner<VisualizationPoint> visualizationPoints)
        {
            Guard.IsLessThanOrEqualTo(potreeNode.NumPoints, int.MaxValue);
            Guard.IsNotNull(PotreeData);

            var pointArray = ReadRawNodeData(potreeNode);

            var visualizationArray = visualizationPoints.Span.ToArray();

            //MemoryMarshal.Cast<byte, VisualizationPoint>()


            WriteRawNodeData(potreeNode, pointArray);
        }

        private void WriteRawNodeData(PotreeNode potreeNode, byte[] rawNodeData)
        {
            Guard.IsNotNull(PotreeData);
            Guard.IsNotNull(rawNodeData);

            var potreePointSize = (int)potreeNode.NumPoints * PotreeData.Metadata.PointSize;
            var pointArray = new byte[potreePointSize];
            PotreeData.WriteViewAccessor.WriteArray(potreeNode.ByteOffset, pointArray, 0, potreePointSize);
        }
    }
}