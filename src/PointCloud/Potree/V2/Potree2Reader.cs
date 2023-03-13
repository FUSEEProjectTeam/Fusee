using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using Fusee.PointCloud.Core.Scene;
using Fusee.PointCloud.Potree.V2.Data;
using System;
using System.Runtime.InteropServices;

namespace Fusee.PointCloud.Potree.V2
{
    /// <summary>
    /// Reads Potree V2 files and is able to create a point cloud scene component, that can be rendered.
    /// </summary>
    public class Potree2Reader : Potree2ReaderBase, IPointReader
    {
        /// <summary>
        /// Specify the byte offset for one point until the extra byte data is reached
        /// </summary>
        public readonly int OffsetToExtraBytes;

        /// <summary>
        /// Pass method how to handle the extra bytes, resulting uint will be passed into <see cref="Mesh.Flags"/>.
        /// </summary>
        public Func<byte[], uint>? HandleExtraBytes { get; set; }

        /// <summary>
        /// Generate a new instance of <see cref="Potree2Reader"/>.
        /// </summary>
        /// <param name="offsetToExtraBytes"></param>
        public Potree2Reader(int offsetToExtraBytes = 0) : base() => OffsetToExtraBytes = offsetToExtraBytes;


        /// <summary>
        /// Returns a renderable point cloud component.
        /// </summary>
        /// <param name="renderMode">Determines which <see cref="RenderMode"/> is used to display the returned point cloud."/></param>
        public IPointCloud GetPointCloudComponent(RenderMode renderMode = RenderMode.StaticMesh)
        {
            switch (renderMode)
            {
                default:
                case RenderMode.StaticMesh:
                    {
                        var dataHandler = new PointCloudDataHandler<GpuMesh>(MeshMaker.CreateStaticMesh,
                            LoadVisualizationPointData);
                        var imp = new Potree2Cloud(dataHandler, GetOctree());
                        return new PointCloudComponent(imp, renderMode);
                    }
                case RenderMode.Instanced:
                    {
                        var dataHandlerInstanced = new PointCloudDataHandler<InstanceData>(MeshMaker.CreateInstanceData,
                            LoadVisualizationPointData, true);
                        var imp = new Potree2CloudInstanced(dataHandlerInstanced, GetOctree());
                        return new PointCloudComponent(imp, renderMode);
                    }
                case RenderMode.DynamicMesh:
                    {
                        var dataHandlerDynamic = new PointCloudDataHandler<Mesh>(MeshMaker.CreateDynamicMesh,
                            LoadVisualizationPointData);
                        var imp = new Potree2CloudDynamic(dataHandlerDynamic, GetOctree());
                        return new PointCloudComponent(imp, renderMode);
                    }
            }
        }

        /// <summary>
        /// Reads the Potree file and returns an octree.
        /// </summary>
        /// <returns></returns>
        public IPointCloudOctree GetOctree()
        {
            Guard.IsNotNull(PotreeData);
            Guard.IsNotNull(PotreeData.Metadata);

            int pointSize = 0;

            foreach (var metaAttributeItem in PotreeData.Metadata.AttributesList)
            {
                pointSize += metaAttributeItem.Size;
            }

            PotreeData.Metadata.PointSize = pointSize;

            var center = PotreeData.Hierarchy.Root.Aabb.Center;
            var size = PotreeData.Hierarchy.Root.Aabb.Size.y;
            var maxLvl = PotreeData.Metadata.Hierarchy.Depth;

            var octree = new PointCloudOctree(center, size, maxLvl);

            MapChildNodesRecursive(octree.Root, PotreeData.Hierarchy.Root);

            return octree;
        }

        /// <summary>
        /// Reads the points for a specific octant of type <see cref="VisualizationPoint"/>.
        /// </summary>
        /// <param name="id">Id of the octant.</param>
        /// <returns></returns>
        public MemoryOwner<VisualizationPoint> LoadVisualizationPointData(OctantId id)
        {
            Guard.IsNotNull(PotreeData);
            var node = FindNode(ref PotreeData.Hierarchy, id);

            // if node is null the hierarchy is broken and we look for an octant that isn't there...
            Guard.IsNotNull(node);

            return LoadVisualizationPoint(node);
        }

        private MemoryOwner<VisualizationPoint> LoadVisualizationPoint(PotreeNode node)
        {
            Guard.IsLessThanOrEqualTo(node.NumPoints, int.MaxValue);
            if (HandleExtraBytes != null)
                Guard.IsGreaterThan(OffsetToExtraBytes, 0);
            Guard.IsNotNull(PotreeData);

            var potreePointSize = (int)node.NumPoints * PotreeData.Metadata.PointSize;
            var pointArray = new byte[potreePointSize];

            var returnMemory = MemoryOwner<VisualizationPoint>.Allocate((int)node.NumPoints);

            OctreeMappedViewAccessor.ReadArray(node.ByteOffset, pointArray, 0, potreePointSize);

            var pointCount = 0;

            for (var i = 0; i < pointArray.Length; i += PotreeData.Metadata.PointSize)
            {
                var posSlice = new Span<byte>(pointArray).Slice(i + offsetPosition, Marshal.SizeOf<int>() * 3);
                var pos = MemoryMarshal.Cast<byte, int>(posSlice);

                double x = pos[0] * PotreeData.Metadata.Scale.x;
                double y = pos[1] * PotreeData.Metadata.Scale.y;
                double z = pos[2] * PotreeData.Metadata.Scale.z;

                float3 position = new((float)x, (float)y, (float)z);
                position = (float4x4)Potree2Consts.YZflip * position;

                var posSpan = MemoryMarshal.Cast<float, byte>(position.ToArray());

                var colorSlice = new Span<byte>(pointArray).Slice(i + offsetColor, Marshal.SizeOf<ushort>() * 3);
                var rgb = MemoryMarshal.Cast<byte, ushort>(colorSlice);

                var color = float4.Zero;

                color.r = ((byte)(rgb[0] > 255 ? rgb[0] / 256 : rgb[0]));
                color.g = ((byte)(rgb[1] > 255 ? rgb[1] / 256 : rgb[1]));
                color.b = ((byte)(rgb[2] > 255 ? rgb[2] / 256 : rgb[2]));
                color.a = 1;

                var colorSpan = MemoryMarshal.Cast<float, byte>(color.ToArray());

                var extraByteSize = PotreeData.Metadata.PointSize - OffsetToExtraBytes;
                var extraBytesSpan = pointArray.AsSpan().Slice(i + OffsetToExtraBytes, extraByteSize);

                uint flags = 0;
                if (HandleExtraBytes != null)
                {
                    flags = HandleExtraBytes(extraBytesSpan.ToArray());
                }
                var flagsSpan = MemoryMarshal.Cast<uint, byte>(new uint[] { flags });

                var currentMemoryPt = MemoryMarshal.Cast<VisualizationPoint, byte>(returnMemory.Span.Slice(pointCount, 1));
                posSpan.CopyTo(currentMemoryPt[..]);
                colorSpan.CopyTo(currentMemoryPt[posSpan.Length..]);
                flagsSpan.CopyTo(currentMemoryPt.Slice(posSpan.Length + colorSpan.Length, Marshal.SizeOf<uint>()));

                pointCount++;
            }

            return returnMemory;
        }

        private static void MapChildNodesRecursive(IPointCloudOctant octreeNode, PotreeNode potreeNode)
        {
            octreeNode.NumberOfPointsInNode = (int)potreeNode.NumPoints;

            for (int i = 0; i < potreeNode.Children.Length; i++)
            {
                if (potreeNode.Children[i] != null)
                {
                    var potreeChild = potreeNode.Children[i];

                    var octant = new PointCloudOctant(potreeNode.Children[i].Aabb.Center, potreeNode.Children[i].Aabb.Size.y, new OctantId(potreeChild.Name));

                    if (potreeChild.NodeType == NodeType.LEAF)
                    {
                        octant.IsLeaf = true;
                    }

                    MapChildNodesRecursive(octant, potreeChild);

                    octreeNode.Children[i] = octant;
                }
            }
        }
    }
}