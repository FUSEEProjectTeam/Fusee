using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Common.Accessors;
using Fusee.PointCloud.Core;
using Fusee.PointCloud.Core.Accessors;
using Fusee.PointCloud.Core.Scene;
using Fusee.PointCloud.Potree.V2.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fusee.PointCloud.Potree.V2
{
    /// <summary>
    /// Reads Potree V2 files and is able to create a point cloud scene component, that can be rendered.
    /// </summary>
    public class Potree2Reader : IPointReader
    {
        /// <summary>
        /// A PointAccessor allows access to the point information (position, color, ect.) without casting it to a specific <see cref="PointType"/>.
        /// </summary>
        public IPointAccessor PointAccessor { get; private set; }

        internal PotreeData FileDataInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PotreeData();
                    FileDataInstance.Metadata = JsonConvert.DeserializeObject<PotreeMetadata>(File.ReadAllText(_metadataFilePath));
                    FileDataInstance.Hierarchy = LoadHierarchy(_fileFolderPath);
                    FileDataInstance.Metadata.FolderPath = _fileFolderPath;

                    // Changing AABBs to have local coordinates
                    // Fliping all YZ coordinates
                    for (int i = 0; i < FileDataInstance.Hierarchy.Nodes.Count; i++)
                    {
                        var node = FileDataInstance.Hierarchy.Nodes[i];
                        node.Aabb = new AABBd(Constants.YZflip * (node.Aabb.min - FileDataInstance.Metadata.Offset), Constants.YZflip * (node.Aabb.max - FileDataInstance.Metadata.Offset));
                    }
                    FileDataInstance.Metadata.OffsetList = new List<double>(3) { FileDataInstance.Metadata.Offset.x, FileDataInstance.Metadata.Offset.z, FileDataInstance.Metadata.Offset.y };
                    FileDataInstance.Metadata.ScaleList = new List<double>(3) { FileDataInstance.Metadata.Scale.x, FileDataInstance.Metadata.Scale.z, FileDataInstance.Metadata.Scale.y };

                    // Setting the metadata BoundingBox to the values of the root node. No fliping required since that was done in the for loop
                    FileDataInstance.Metadata.BoundingBox.MinList = new List<double>(3) { FileDataInstance.Hierarchy.Root.Aabb.min.x, FileDataInstance.Hierarchy.Root.Aabb.min.y, FileDataInstance.Hierarchy.Root.Aabb.min.z };
                    FileDataInstance.Metadata.BoundingBox.MaxList = new List<double>(3) { FileDataInstance.Hierarchy.Root.Aabb.max.x, FileDataInstance.Hierarchy.Root.Aabb.max.y, FileDataInstance.Hierarchy.Root.Aabb.max.z };
                }
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        private static PotreeData _instance;
        private string _fileFolderPath;
        private string _metadataFilePath;

        /// <summary>
        /// Returns the point type.
        /// </summary>
        public PointType GetPointType()
        {
            return PointType.PosD3ColF3LblB;
        }

        /// <summary>
        /// Returns a renderable point cloud component.
        /// </summary>
        /// <param name="fileFolderPath">Path to the file.</param>
        public IPointCloud GetPointCloudComponent(string fileFolderPath)
        {
            _fileFolderPath = fileFolderPath;

            var ptType = GetPointType();

            switch (ptType)
            {
                default:
                case PointType.PosD3:
                case PointType.PosD3ColF3InUs:
                case PointType.PosD3InUs:
                case PointType.PosD3ColF3:
                case PointType.PosD3LblB:
                case PointType.PosD3NorF3ColF3InUs:
                case PointType.PosD3NorF3InUs:
                case PointType.PosD3NorF3ColF3:
                case PointType.PosD3ColF3InUsLblB:
                    throw new ArgumentOutOfRangeException($"Invalid point type {ptType}");
                case PointType.PosD3ColF3LblB:
                    PointAccessor = new PosD3ColF3LblBAccessor();
                    var dataHandler = new PointCloudDataHandler<PosD3ColF3LblB>((PointAccessor<PosD3ColF3LblB>)PointAccessor, MeshMaker.CreateMeshPosD3ColF3LblB, LoadNodeData<PosD3ColF3LblB>);
                    var imp = new Potree2Cloud(dataHandler, GetOctree());
                    return new PointCloudComponent(imp);
            }
        }

        /// <summary>
        /// Reads the Potree file and returns an octree.
        /// </summary>
        /// <returns></returns>
        public IPointCloudOctree GetOctree()
        {
            _metadataFilePath = Path.Combine(_fileFolderPath, Constants.MetadataFileName);

            int pointSize = 0;

            if (FileDataInstance.Metadata != null)
            {
                foreach (var metaAttributeItem in FileDataInstance.Metadata.Attributes)
                {
                    pointSize += metaAttributeItem.Size;
                }

                FileDataInstance.Metadata.PointSize = pointSize;
            }

            var center = FileDataInstance.Hierarchy.Root.Aabb.Center;
            var size = FileDataInstance.Hierarchy.Root.Aabb.Size.y;
            var maxLvl = FileDataInstance.Metadata.Hierarchy.Depth;

            var octree = new PointCloudOctree(center, size, maxLvl);

            MapChildNodesRecursive(octree.Root, FileDataInstance.Hierarchy.Root);

            return octree;
        }


        /// <summary>
        /// Returns the points for one octant as generic array.
        /// </summary>
        /// <typeparam name="TPoint">The generic point type.</typeparam>
        /// <param name="id">The unique id of the octant.</param>
        /// <returns></returns>
        public TPoint[] LoadNodeData<TPoint>(string id) where TPoint : new()
        {
            var node = FindNode(id);

            var octreeFilePath = Path.Combine(FileDataInstance.Metadata.FolderPath, Constants.OctreeFileName);
            var binaryReader = new BinaryReader(File.OpenRead(octreeFilePath));
            TPoint[] points = LoadNodeData<TPoint>(node, binaryReader);

            node.IsLoaded = true;

            binaryReader.Close();
            binaryReader.Dispose();

            return points;
        }

        private TPoint[] LoadNodeData<TPoint>(PotreeNode node, BinaryReader binaryReader) where TPoint : new()
        {
            var points = new TPoint[node.NumPoints];
            for (int i = 0; i < node.NumPoints; i++)
            {
                points[i] = new TPoint();
            }

            var attributeOffset = 0;

            foreach (var metaitem in FileDataInstance.Metadata.Attributes)
            {
                if (metaitem.Name == "position")
                {
                    for (int i = 0; i < node.NumPoints; i++)
                    {
                        binaryReader.BaseStream.Position = node.ByteOffset + attributeOffset + i * FileDataInstance.Metadata.PointSize;

                        double x = (binaryReader.ReadInt32() * FileDataInstance.Metadata.Scale.x);
                        double y = (binaryReader.ReadInt32() * FileDataInstance.Metadata.Scale.y);
                        double z = (binaryReader.ReadInt32() * FileDataInstance.Metadata.Scale.z);

                        double3 position = new(x, y, z);
                        position = Constants.YZflip * position;

                        ((PointAccessor<TPoint>)PointAccessor).SetPositionFloat3_64(ref points[i], position);
                    }
                }
                else if (metaitem.Name.Contains("rgb"))
                {
                    for (int i = 0; i < node.NumPoints; i++)
                    {
                        binaryReader.BaseStream.Position = node.ByteOffset + attributeOffset + i * FileDataInstance.Metadata.PointSize;

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
                else if (metaitem.Name.Equals("classification"))
                {
                    for (int i = 0; i < node.NumPoints; i++)
                    {
                        binaryReader.BaseStream.Position = node.ByteOffset + attributeOffset + i + FileDataInstance.Metadata.PointSize;

                        byte label = (byte)binaryReader.ReadSByte();

                        switch (label)
                        {
                            case 0:
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            default:
                                break;
                        }

                        ((PointAccessor<TPoint>)PointAccessor).SetLabelUInt_8(ref points[i], label);
                    }
                }

                attributeOffset += metaitem.Size;
            }

            return points;
        }

        private void MapChildNodesRecursive(IPointCloudOctant octreeNode, PotreeNode potreeNode)
        {
            octreeNode.NumberOfPointsInNode = (int)potreeNode.NumPoints;

            for (int i = 0; i < potreeNode.Children.Length; i++)
            {
                if (potreeNode.Children[i] != null)
                {
                    var octant = new PointCloudOctant(potreeNode.Children[i].Aabb.Center, potreeNode.Children[i].Aabb.Size.y, potreeNode.Children[i].Name);

                    if (potreeNode.Children[i].NodeType == NodeType.LEAF)
                    {
                        octant.IsLeaf = true;
                    }

                    MapChildNodesRecursive(octant, potreeNode.Children[i]);

                    octreeNode.Children[i] = octant;
                }
            }
        }

        public PotreeHierarchy LoadHierarchy(string fileFolderPath)
        {
            var hierarchyFilePath = Path.Combine(fileFolderPath, Constants.HierarchyFileName);

            var firstChunkSize = FileDataInstance.Metadata.Hierarchy.FirstChunkSize;
            var stepSize = FileDataInstance.Metadata.Hierarchy.StepSize;
            var depth = FileDataInstance.Metadata.Hierarchy.Depth;

            var data = File.ReadAllBytes(hierarchyFilePath);

            PotreeNode root = new()
            {
                Name = "r",
                Aabb = new AABBd(FileDataInstance.Metadata.BoundingBox.Min, FileDataInstance.Metadata.BoundingBox.Max)
            };

            var hierarchy = new PotreeHierarchy();

            long offset = 0;

            LoadHierarchyRecursive(ref root, ref data, offset, firstChunkSize);

            hierarchy.Nodes = new();
            root.Traverse(n => hierarchy.Nodes.Add(n));

            hierarchy.Root = root;

            return hierarchy;
        }

        private void LoadHierarchyRecursive(ref PotreeNode root, ref byte[] data, long offset, long size)
        {
            int bytesPerNode = 22;
            int numNodes = (int)(size / bytesPerNode);

            var nodes = new List<PotreeNode>(numNodes)
            {
                root
            };

            for (int i = 0; i < numNodes; i++)
            {
                var currentNode = nodes[i];
                if (currentNode == null)
                    currentNode = new PotreeNode();

                ulong offsetNode = (ulong)offset + (ulong)(i * bytesPerNode);

                var nodeType = data[offsetNode + 0];
                int childMask = BitConverter.ToInt32(data, (int)offsetNode + 1);
                var numPoints = BitConverter.ToUInt32(data, (int)offsetNode + 2);
                var byteOffset = BitConverter.ToInt64(data, (int)offsetNode + 6);
                var byteSize = BitConverter.ToInt64(data, (int)offsetNode + 14);

                currentNode.NodeType = (NodeType)nodeType;
                currentNode.NumPoints = numPoints;
                currentNode.ByteOffset = byteOffset;
                currentNode.ByteSize = byteSize;

                if (currentNode.NodeType == NodeType.PROXY)
                {
                    LoadHierarchyRecursive(ref currentNode, ref data, byteOffset, byteSize);
                }
                else
                {
                    for (int childIndex = 0; childIndex < 8; childIndex++)
                    {
                        bool childExists = ((1 << childIndex) & childMask) != 0;

                        if (!childExists)
                        {
                            continue;
                        }

                        string childName = currentNode.Name + childIndex.ToString();

                        PotreeNode child = new();

                        child.Aabb = ChildAABB(currentNode.Aabb, childIndex);
                        child.Name = childName;
                        currentNode.Children[childIndex] = child;
                        child.Parent = currentNode;

                        nodes.Add(child);
                    }
                }
            }

            static AABBd ChildAABB(AABBd aabb, int index)
            {

                double3 min = aabb.min;
                double3 max = aabb.max;

                double3 size = max - min;

                if ((index & 0b0001) > 0)
                {
                    min.z += size.z / 2;
                }
                else
                {
                    max.z -= size.z / 2;
                }

                if ((index & 0b0010) > 0)
                {
                    min.y += size.y / 2;
                }
                else
                {
                    max.y -= size.y / 2;
                }

                if ((index & 0b0100) > 0)
                {
                    min.x += size.x / 2;
                }
                else
                {
                    max.x -= size.x / 2;
                }

                return new AABBd(min, max);
            }
        }

        private PotreeNode FindNode(string id)
        {
            return FileDataInstance.Hierarchy.Nodes.Find(n => n.Name == id);
        }
    }
}