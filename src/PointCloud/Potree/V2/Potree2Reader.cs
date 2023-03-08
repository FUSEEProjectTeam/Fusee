using CommunityToolkit.Diagnostics;
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
using System.Threading.Tasks;

namespace Fusee.PointCloud.Potree.V2
{
    /// <summary>
    /// Reads Potree V2 files and is able to create a point cloud scene component, that can be rendered.
    /// </summary>
    public class Potree2Reader : Potree2ReaderBase, IPointReader
    {
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
                        var dataHandler = new PointCloudDataHandler<GpuMesh, PosD3ColF3LblB>(MeshMaker.CreateMeshPosD3ColF3LblB,
                            LoadNodeDataPosD3ColF3LblB);
                        var imp = new Potree2Cloud(dataHandler, GetOctree());
                        return new PointCloudComponent(imp, renderMode);
                    }
                case RenderMode.Instanced:
                    {
                        var dataHandlerInstanced = new PointCloudDataHandler<InstanceData, PosD3ColF3LblB>(MeshMaker.CreateInstanceDataPosD3ColF3LblB,
                            LoadNodeDataPosD3ColF3LblB, true);
                        var imp = new Potree2CloudInstanced(dataHandlerInstanced, GetOctree());
                        return new PointCloudComponent(imp, renderMode);
                    }
                case RenderMode.DynamicMesh:
                    {
                        var dataHandlerDynamic = new PointCloudDataHandler<Mesh, PosD3ColF3LblB>(MeshMaker.CreateDynamicMeshPosD3ColF3LblB,
                            LoadNodeDataPosD3ColF3LblB);
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

            int pointSize = 0;

            if (PotreeData.Metadata != null)
            {
                foreach (var metaAttributeItem in PotreeData.Metadata.AttributesList)
                {
                    pointSize += metaAttributeItem.Size;
                }

                PotreeData.Metadata.PointSize = pointSize;
            }

            Guard.IsNotNull(PotreeData.Metadata);

            var center = PotreeData.Hierarchy.Root.Aabb.Center;
            var size = PotreeData.Hierarchy.Root.Aabb.Size.y;
            var maxLvl = PotreeData.Metadata.Hierarchy.Depth;

            var octree = new PointCloudOctree(center, size, maxLvl);

            MapChildNodesRecursive(octree.Root, PotreeData.Hierarchy.Root);

            return octree;
        }

        /// <summary>
        /// Reads the points for a specific octant of type <see cref="PosD3ColF3LblB"/>.
        /// </summary>
        /// <param name="id">Id of the octant.</param>
        /// <returns></returns>
        public MemoryOwner<PosD3ColF3LblB> LoadNodeDataPosD3ColF3LblB(OctantId id)
        {
            Guard.IsNotNull(PotreeData);
            var node = FindNode(ref PotreeData.Hierarchy, id);

            // if node is null the hierarchy is broken and we look for an octant that isn't there...
            Guard.IsNotNull(node);

            return LoadNodeData<PosD3ColF3LblB>(node, PointType.PosD3ColF3LblB);
        }

        /// <summary>
        /// Reads the points for a specific octant of type <see cref="PosD3ColF3LblB"/>.
        /// </summary>
        /// <param name="id">Id of the octant.</param>
        /// <returns></returns>
        public MemoryOwner<PotreePoint> LoadRawNodeData(OctantId id)
        {
            Guard.IsNotNull(PotreeData);
            var node = FindNode(ref PotreeData.Hierarchy, id);
            // if node is null the hierarchy is broken and we look for an octant that isn't there...
            Guard.IsNotNull(node);
            return ReadNodeData(node);
        }

        private MemoryOwner<TPoint> LoadNodeData<TPoint>(PotreeNode potreeNode, PointType type) where TPoint : struct
        {
            // if the potree node is null #nullable doesn't work!
            Guard.IsNotNull(potreeNode);
            potreeNode.IsLoaded = true;
            return type switch
            {
                // TODO: add missing cases
                PointType.PosD3ColF3LblB => ReadNodeDataPosD3ColF3LblB<TPoint>(potreeNode),
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<MemoryOwner<TPoint>>(nameof(type)),
            };
        }

        private MemoryOwner<PotreePoint> ReadNodeData(PotreeNode node)
        {
            Guard.IsLessThanOrEqualTo(node.NumPoints, int.MaxValue);

            var potreePointSize = (int)node.NumPoints * PotreeData.Metadata.PointSize;
            var pointArray = new byte[potreePointSize];

            var returnMemory = MemoryOwner<PotreePoint>.Allocate((int)node.NumPoints);

            OctreeMappedViewAccessor.ReadArray(node.ByteOffset, pointArray, 0, potreePointSize);

            var pointCount = 0;
           
            for (var i = 0; i < pointArray.Length; i += PotreeData.Metadata.PointSize)
            {
                var pt = pointArray.AsSpan(i, PotreeData.Metadata.PointSize);
                var posSlice = pt.Slice(offsetPosition, Marshal.SizeOf<int>() * 3);
                var pos = MemoryMarshal.Cast<byte, int>(posSlice);

                double x = pos[0] * PotreeData.Metadata.Scale.x + PotreeData.Metadata.Offset.x;
                double y = pos[1] * PotreeData.Metadata.Scale.y + PotreeData.Metadata.Offset.y;
                double z = pos[2] * PotreeData.Metadata.Scale.z + PotreeData.Metadata.Offset.z;

                double3 position = new(x, y, z);
                position = Potree2Consts.YZflip * position;

                /*  public short Intensity;
                    public byte ReturnNumber;
                    public byte NumberOfReturns;
                    public byte Classification;
                    public byte ScanAngleRank;
                    public byte UserData;
                    public byte PointSourceId;
                    public float3 Color;
                */

                var intensity = MemoryMarshal.Cast<byte, short>(pt.Slice(offsetIntensity, Marshal.SizeOf<short>()))[0];
                var returnNumber = pt.Slice(offsetReturnNumber, 1)[0];
                var numberOfReturns = pt.Slice(offsetNumberOfReturns, 1)[0];
                var classification = pt.Slice(offsetClassification, 1)[0];
                var scanAngle = pt.Slice(offsetScanAngleRank, 1)[0];
                var userData = pt.Slice(offsetUserData, 1)[0];
                var ptSourceId = pt.Slice(offsetPointSourceId, 1)[0];
                var r = MemoryMarshal.Cast<byte, ushort>(pt.Slice(offsetColor, Marshal.SizeOf<ushort>()))[0];
                var g = MemoryMarshal.Cast<byte, ushort>(pt.Slice(offsetColor + Marshal.SizeOf<ushort>(), Marshal.SizeOf<ushort>()))[0];
                var b = MemoryMarshal.Cast<byte, ushort>(pt.Slice(offsetColor + Marshal.SizeOf<ushort>() + Marshal.SizeOf<ushort>(), Marshal.SizeOf<ushort>()))[0];

                var color = new float3
                {
                    x = ((float)r / ushort.MaxValue),
                    y = ((float)g / ushort.MaxValue),
                    z = ((float)b / ushort.MaxValue)
                };

                returnMemory.Span[pointCount].Position = position;
                returnMemory.Span[pointCount].Intensity = intensity;
                returnMemory.Span[pointCount].ReturnNumber = returnNumber;
                returnMemory.Span[pointCount].NumberOfReturns = numberOfReturns;
                returnMemory.Span[pointCount].Classification = classification;
                returnMemory.Span[pointCount].ScanAngleRank = scanAngle;
                returnMemory.Span[pointCount].UserData = userData;
                returnMemory.Span[pointCount].PointSourceId = ptSourceId;
                returnMemory.Span[pointCount].Color = color;

                pointCount++;
            }

            return returnMemory;
        }

        private MemoryOwner<TPoint> ReadNodeDataPosD3ColF3LblB<TPoint>(PotreeNode node) where TPoint : struct
        {
            Guard.IsLessThanOrEqualTo(node.NumPoints, int.MaxValue);

            var potreePointSize = (int)node.NumPoints * PotreeData.Metadata.PointSize;
            var pointArray = new byte[potreePointSize];

            var returnMemory = MemoryOwner<TPoint>.Allocate((int)node.NumPoints);

            OctreeMappedViewAccessor.ReadArray(node.ByteOffset, pointArray, 0, potreePointSize);

            var pointCount = 0;

            for (var i = 0; i < pointArray.Length; i += PotreeData.Metadata.PointSize)
            {
                var posSlice = new Span<byte>(pointArray).Slice(i + offsetPosition, Marshal.SizeOf<int>() * 3);
                var pos = MemoryMarshal.Cast<byte, int>(posSlice);

                double x = pos[0] * PotreeData.Metadata.Scale.x;
                double y = pos[1] * PotreeData.Metadata.Scale.y;
                double z = pos[2] * PotreeData.Metadata.Scale.z;

                double3 position = new(x, y, z);
                position = Potree2Consts.YZflip * position;

                var posSpan = MemoryMarshal.Cast<double, byte>(position.ToArray());

                var colorSlice = new Span<byte>(pointArray).Slice(i + offsetColor, Marshal.SizeOf<ushort>() * 3);
                var rgb = MemoryMarshal.Cast<byte, ushort>(colorSlice);

                var color = float3.Zero;

                color.r = ((byte)(rgb[0] > 255 ? rgb[0] / 256 : rgb[0]));
                color.g = ((byte)(rgb[1] > 255 ? rgb[1] / 256 : rgb[1]));
                color.b = ((byte)(rgb[2] > 255 ? rgb[2] / 256 : rgb[2]));

                var colorSpan = MemoryMarshal.Cast<float, byte>(color.ToArray());

                byte label = new Span<byte>(pointArray).Slice(i + offsetClassification, Marshal.SizeOf<byte>())[0];

                var currentMemoryPt = MemoryMarshal.Cast<TPoint, byte>(returnMemory.Span.Slice(pointCount, 1));
                posSpan.CopyTo(currentMemoryPt[..]);
                colorSpan.CopyTo(currentMemoryPt[posSpan.Length..]);
                currentMemoryPt[^1] = label;

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