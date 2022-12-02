using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Common.Accessors;
using Fusee.PointCloud.Core;
using Fusee.PointCloud.Core.Accessors;
using Fusee.PointCloud.Core.Scene;
using Fusee.PointCloud.Potree.V2.Data;
using System;
using System.IO;

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
        public Potree2Reader(ref PotreeData potreeData) : base(ref potreeData) { }

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
                        var dataHandler = new PointCloudDataHandler<GpuMesh, PosD3ColF3LblB>(
                            (PointAccessor<PosD3ColF3LblB>)PointAccessor, MeshMaker.CreateMeshPosD3ColF3LblB,
                            LoadNodeData<PosD3ColF3LblB>);
                        var imp = new Potree2Cloud(dataHandler, GetOctree());
                        return new PointCloudComponent(imp, renderMode);
                    }
                case RenderMode.Instanced:
                    {
                        var dataHandlerInstanced = new PointCloudDataHandler<InstanceData, PosD3ColF3LblB>(
                            (PointAccessor<PosD3ColF3LblB>)PointAccessor, MeshMaker.CreateInstanceDataPosD3ColF3LblB,
                            LoadNodeData<PosD3ColF3LblB>, true);
                        var imp = new Potree2CloudInstanced(dataHandlerInstanced, GetOctree());
                        return new PointCloudComponent(imp, renderMode);
                    }
                case RenderMode.DynamicMesh:
                    {
                        var dataHandlerDynamic = new PointCloudDataHandler<Mesh, PosD3ColF3LblB>(
                            (PointAccessor<PosD3ColF3LblB>)PointAccessor, MeshMaker.CreateDynamicMeshPosD3ColF3LblB,
                            LoadNodeData<PosD3ColF3LblB>, true);
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

            var center = _potreeData.Hierarchy.Root.Aabb.Center;
            var size = _potreeData.Hierarchy.Root.Aabb.Size.y;
            var maxLvl = _potreeData.Metadata.Hierarchy.Depth;

            var octree = new PointCloudOctree(center, size, maxLvl);

            MapChildNodesRecursive(octree.Root, _potreeData.Hierarchy.Root);

            return octree;
        }

        /// <summary>
        /// Returns the points for one octant as generic array.
        /// </summary>
        /// <typeparam name="TPoint">The generic point type.</typeparam>
        /// <param name="id">The unique id of the octant.</param>
        /// <returns></returns>
        public TPoint[] LoadNodeData<TPoint>(OctantId id) where TPoint : new()
        {
            TPoint[] points = null;

            var node = FindNode(ref _potreeData.Hierarchy, id);

            if (node != null)
            {
                points = LoadNodeData<TPoint>(node);
            }

            return points;
        }

        public TPoint[] LoadNodeData<TPoint>(PotreeNode potreeNode) where TPoint : new()
        {
            TPoint[] points = null;

            if (potreeNode != null)
            {
                points = ReadNodeData<TPoint>(potreeNode);
                potreeNode.IsLoaded = true;
            }

            return points;
        }

        private TPoint[] ReadNodeData<TPoint>(PotreeNode node) where TPoint : new()
        {
            var points = new TPoint[node.NumPoints];
            for (int i = 0; i < node.NumPoints; i++)
            {
                points[i] = new TPoint();
            }

            var binaryReader = new BinaryReader(File.OpenRead(OctreeFilePath));

            // Commented code is to read the entire Potree2 file format. Since we don't use everything atm unused 
            // things are commented for performance.
            for (int i = 0; i < node.NumPoints; i++)
            {
                if (offsetPosition > -1)
                {
                    binaryReader.BaseStream.Position = node.ByteOffset + offsetPosition + i * _potreeData.Metadata.PointSize;

                    double x = binaryReader.ReadInt32() * _potreeData.Metadata.Scale.x;
                    double y = binaryReader.ReadInt32() * _potreeData.Metadata.Scale.y;
                    double z = binaryReader.ReadInt32() * _potreeData.Metadata.Scale.z;

                    double3 position = new(x, y, z);
                    position = Potree2Consts.YZflip * position;

                    ((PointAccessor<TPoint>)PointAccessor).SetPositionFloat3_64(ref points[i], position);
                }

                //if (offsetIntensity > -1)
                //{
                //    binaryReader.BaseStream.Position = node.ByteOffset + offsetIntensity + i * _potreeData.Metadata.PointSize;
                //    Int16 intensity = binaryReader.ReadInt16();
                //}
                //if (offsetReturnNumber > -1)
                //{
                //    binaryReader.BaseStream.Position = node.ByteOffset + offsetReturnNumber + i * _potreeData.Metadata.PointSize;
                //    byte returnNumber = binaryReader.ReadByte();
                //}
                //if (offsetNumberOfReturns > -1)
                //{
                //    binaryReader.BaseStream.Position = node.ByteOffset + offsetNumberOfReturns + i * _potreeData.Metadata.PointSize;
                //    byte numberOfReturns = binaryReader.ReadByte();
                //}

                if (offsetClassification > -1)
                {
                    binaryReader.BaseStream.Position = node.ByteOffset + offsetClassification + i * _potreeData.Metadata.PointSize;

                    byte label = binaryReader.ReadByte();

                    ((PointAccessor<TPoint>)PointAccessor).SetLabelUInt_8(ref points[i], label);
                }

                //else if (offsetScanAngleRank > -1)
                //{
                //    binaryReader.BaseStream.Position = node.ByteOffset + offsetScanAngleRank + i * _potreeData.Metadata.PointSize;
                //    byte scanAngleRank = binaryReader.ReadByte();
                //}
                //else if (offsetUserData > -1)
                //{
                //    binaryReader.BaseStream.Position = node.ByteOffset + offsetUserData + i * _potreeData.Metadata.PointSize;
                //    byte userData = binaryReader.ReadByte();
                //}
                //else if (offsetPointSourceId > -1)
                //{
                //    binaryReader.BaseStream.Position = node.ByteOffset + offsetPointSourceId + i * _potreeData.Metadata.PointSize;
                //    byte pointSourceId = binaryReader.ReadByte();
                //}

                if (offsetColor > -1)
                {
                    binaryReader.BaseStream.Position = node.ByteOffset + offsetColor + i * _potreeData.Metadata.PointSize;

                    ushort r = binaryReader.ReadUInt16();
                    ushort g = binaryReader.ReadUInt16();
                    ushort b = binaryReader.ReadUInt16();

                    float3 color = float3.Zero;

                    color.r = ((byte)(r > 255 ? r / 256 : r));
                    color.g = ((byte)(g > 255 ? g / 256 : g));
                    color.b = ((byte)(b > 255 ? b / 256 : b));

                    ((PointAccessor<TPoint>)PointAccessor).SetColorFloat3_32(ref points[i], color);
                }
            }

            binaryReader.Close();
            binaryReader.Dispose();

            return points;
        }

        public TPotreePoint[] ReadRawPoints<TPotreePoint>(OctantId oid) where TPotreePoint : PotreePoint, new()
        {
            var node = FindNode(ref _potreeData.Hierarchy, oid);

            var points = new TPotreePoint[node.NumPoints];

            Array.Fill(points, new TPotreePoint());

            var binaryReader = new BinaryReader(File.OpenRead(OctreeFilePath));

            for (int i = 0; i < node.NumPoints; i++)
            {
                if (offsetPosition > -1)
                {
                    binaryReader.BaseStream.Position = node.ByteOffset + offsetPosition + i * _potreeData.Metadata.PointSize;

                    double x = binaryReader.ReadInt32() * _potreeData.Metadata.Scale.x;
                    double y = binaryReader.ReadInt32() * _potreeData.Metadata.Scale.y;
                    double z = binaryReader.ReadInt32() * _potreeData.Metadata.Scale.z;

                    double3 position = new(x, y, z);
                    position = Potree2Consts.YZflip * position;

                    points[i].Position = position;
                }

                if (offsetIntensity > -1)
                {
                    binaryReader.BaseStream.Position = node.ByteOffset + offsetIntensity + i * _potreeData.Metadata.PointSize;
                    Int16 intensity = binaryReader.ReadInt16();

                    points[i].Intensity = intensity;
                }
                if (offsetReturnNumber > -1)
                {
                    binaryReader.BaseStream.Position = node.ByteOffset + offsetReturnNumber + i * _potreeData.Metadata.PointSize;
                    byte returnNumber = binaryReader.ReadByte();

                    points[i].ReturnNumber = returnNumber;
                }
                if (offsetNumberOfReturns > -1)
                {
                    binaryReader.BaseStream.Position = node.ByteOffset + offsetNumberOfReturns + i * _potreeData.Metadata.PointSize;
                    byte numberOfReturns = binaryReader.ReadByte();

                    points[i].NumberOfReturns = numberOfReturns;
                }

                if (offsetClassification > -1)
                {
                    binaryReader.BaseStream.Position = node.ByteOffset + offsetClassification + i * _potreeData.Metadata.PointSize;

                    byte label = binaryReader.ReadByte();

                    points[i].Classification = label;
                }

                else if (offsetScanAngleRank > -1)
                {
                    binaryReader.BaseStream.Position = node.ByteOffset + offsetScanAngleRank + i * _potreeData.Metadata.PointSize;
                    byte scanAngleRank = binaryReader.ReadByte();

                    points[i].ScanAngleRank = scanAngleRank;
                }
                else if (offsetUserData > -1)
                {
                    binaryReader.BaseStream.Position = node.ByteOffset + offsetUserData + i * _potreeData.Metadata.PointSize;
                    byte userData = binaryReader.ReadByte();

                    points[i].UserData = userData;
                }
                else if (offsetPointSourceId > -1)
                {
                    binaryReader.BaseStream.Position = node.ByteOffset + offsetPointSourceId + i * _potreeData.Metadata.PointSize;
                    byte pointSourceId = binaryReader.ReadByte();

                    points[i].PointSourceId = pointSourceId;
                }

                if (offsetColor > -1)
                {
                    binaryReader.BaseStream.Position = node.ByteOffset + offsetColor + i * _potreeData.Metadata.PointSize;

                    ushort r = binaryReader.ReadUInt16();
                    ushort g = binaryReader.ReadUInt16();
                    ushort b = binaryReader.ReadUInt16();

                    float3 color = float3.Zero;

                    color.r = ((byte)(r > 255 ? r / 256 : r));
                    color.g = ((byte)(g > 255 ? g / 256 : g));
                    color.b = ((byte)(b > 255 ? b / 256 : b));

                    points[i].Color = color;
                }
            }

            binaryReader.Close();
            binaryReader.Dispose();

            return points;
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