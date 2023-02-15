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
using System.Buffers;
using System.Runtime.InteropServices;

namespace Fusee.PointCloud.Potree.V2
{
    /// <summary>
    /// Reads Potree V2 files and is able to create a point cloud scene component, that can be rendered.
    /// </summary>
    public class Potree2Reader : Potree2RwBase, IPointReader
    {
        /// <summary>
        /// Initializes a Potree 2 reader for the given Potree dataset
        /// </summary>
        /// <param name="potreeData"></param>
        public Potree2Reader(ref PotreeData potreeData) : base(ref potreeData)
        {
        }

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

            if (_potreeData.Metadata != null)
            {
                foreach (var metaAttributeItem in _potreeData.Metadata.AttributesList)
                {
                    pointSize += metaAttributeItem.Size;
                }

                _potreeData.Metadata.PointSize = pointSize;
            }

            Guard.IsNotNull(_potreeData.Metadata);

            var center = _potreeData.Hierarchy.Root.Aabb.Center;
            var size = _potreeData.Hierarchy.Root.Aabb.Size.y;
            var maxLvl = _potreeData.Metadata.Hierarchy.Depth;

            var octree = new PointCloudOctree(center, size, maxLvl);

            MapChildNodesRecursive(octree.Root, _potreeData.Hierarchy.Root);

            return octree;
        }

        public MemoryOwner<PosD3ColF3LblB> LoadNodeDataPosD3ColF3LblB(OctantId id)
        {
            var node = FindNode(ref _potreeData.Hierarchy, id);

            // if node is null the hierarchy is broken and we look for an octant that isn't there...
            Guard.IsNotNull(node);

            return LoadNodeData<PosD3ColF3LblB>(node, PointType.PosD3ColF3LblB);
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


        private MemoryOwner<TPoint> ReadNodeDataPosD3ColF3LblB<TPoint>(PotreeNode node) where TPoint : struct
        {
            Guard.IsLessThanOrEqualTo(node.NumPoints, int.MaxValue);

            var potreePointSize = (int)node.NumPoints * _potreeData.Metadata.PointSize;
            var pointArray = new byte[potreePointSize];

            var returnMemory = MemoryOwner<TPoint>.Allocate((int)node.NumPoints);

            OctreeMappedViewAccessor.ReadArray(node.ByteOffset, pointArray, 0, potreePointSize);

            var pointCount = 0;

            for (var i = 0; i < pointArray.Length; i += _potreeData.Metadata.PointSize)
            {
                var posSlice = new Span<byte>(pointArray).Slice(i + offsetPosition, Marshal.SizeOf<int>() * 3);
                var pos = MemoryMarshal.Cast<byte, int>(posSlice);

                double x = pos[0] * _potreeData.Metadata.Scale.x;
                double y = pos[1] * _potreeData.Metadata.Scale.y;
                double z = pos[2] * _potreeData.Metadata.Scale.z;

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

        //public TPotreePoint[] ReadRawPoints<TPotreePoint>(OctantId oid) where TPotreePoint : struct
        //{
        //    var node = FindNode(ref _potreeData.Hierarchy, oid);

        //    var points = new TPotreePoint[node.NumPoints];


        //    for (var i = 0; i < points.Length; i++)
        //        points[i] = new TPotreePoint();

        //    var binaryReader = new BinaryReader(File.OpenRead(OctreeFilePath));

        //    for (int i = 0; i < node.NumPoints; i++)
        //    {
        //        if (offsetPosition > -1)
        //        {
        //            binaryReader.BaseStream.Position = node.ByteOffset + offsetPosition + i * _potreeData.Metadata.PointSize;

        //            double x = binaryReader.ReadInt32() * _potreeData.Metadata.Scale.x;
        //            double y = binaryReader.ReadInt32() * _potreeData.Metadata.Scale.y;
        //            double z = binaryReader.ReadInt32() * _potreeData.Metadata.Scale.z;

        //            double3 position = new(x, y, z);
        //            position = Potree2Consts.YZflip * position;

        //            points[i].Position = position;
        //        }

        //        if (offsetIntensity > -1)
        //        {
        //            binaryReader.BaseStream.Position = node.ByteOffset + offsetIntensity + i * _potreeData.Metadata.PointSize;
        //            Int16 intensity = binaryReader.ReadInt16();

        //            points[i].Intensity = intensity;
        //        }
        //        if (offsetReturnNumber > -1)
        //        {
        //            binaryReader.BaseStream.Position = node.ByteOffset + offsetReturnNumber + i * _potreeData.Metadata.PointSize;
        //            byte returnNumber = binaryReader.ReadByte();

        //            points[i].ReturnNumber = returnNumber;
        //        }
        //        if (offsetNumberOfReturns > -1)
        //        {
        //            binaryReader.BaseStream.Position = node.ByteOffset + offsetNumberOfReturns + i * _potreeData.Metadata.PointSize;
        //            byte numberOfReturns = binaryReader.ReadByte();

        //            points[i].NumberOfReturns = numberOfReturns;
        //        }

        //        if (offsetClassification > -1)
        //        {
        //            binaryReader.BaseStream.Position = node.ByteOffset + offsetClassification + i * _potreeData.Metadata.PointSize;

        //            byte label = binaryReader.ReadByte();

        //            points[i].Classification = label;
        //        }

        //        else if (offsetScanAngleRank > -1)
        //        {
        //            binaryReader.BaseStream.Position = node.ByteOffset + offsetScanAngleRank + i * _potreeData.Metadata.PointSize;
        //            byte scanAngleRank = binaryReader.ReadByte();

        //            points[i].ScanAngleRank = scanAngleRank;
        //        }
        //        else if (offsetUserData > -1)
        //        {
        //            binaryReader.BaseStream.Position = node.ByteOffset + offsetUserData + i * _potreeData.Metadata.PointSize;
        //            byte userData = binaryReader.ReadByte();

        //            points[i].UserData = userData;
        //        }
        //        else if (offsetPointSourceId > -1)
        //        {
        //            binaryReader.BaseStream.Position = node.ByteOffset + offsetPointSourceId + i * _potreeData.Metadata.PointSize;
        //            byte pointSourceId = binaryReader.ReadByte();

        //            points[i].PointSourceId = pointSourceId;
        //        }

        //        if (offsetColor > -1)
        //        {
        //            binaryReader.BaseStream.Position = node.ByteOffset + offsetColor + i * _potreeData.Metadata.PointSize;

        //            ushort r = binaryReader.ReadUInt16();
        //            ushort g = binaryReader.ReadUInt16();
        //            ushort b = binaryReader.ReadUInt16();

        //            float3 color = float3.Zero;

        //            color.r = ((byte)(r > 255 ? r / 256 : r));
        //            color.g = ((byte)(g > 255 ? g / 256 : g));
        //            color.b = ((byte)(b > 255 ? b / 256 : b));

        //            points[i].Color = color;
        //        }
        //    }

        //    binaryReader.Close();
        //    binaryReader.Dispose();

        //    return points;
        //}

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