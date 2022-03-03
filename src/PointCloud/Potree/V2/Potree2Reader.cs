using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Fusee.PointCloud.Potree.V2.Data;

namespace Fusee.PointCloud.Potree.V2
{
    public class Potree2Reader : IIntermediatePointFileReader
    {
        public PotreeData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PotreeData();
                    Instance.Metadata = JsonConvert.DeserializeObject<PotreeMetadata>(File.ReadAllText(_metadataFilePath));
                    Instance.Hierarchy = LoadHierarchy(_fileFolderPath);
                    Instance.Metadata.FolderPath = _fileFolderPath;

                    // Changing AABBs to have local coordinates
                    // Fliping all YZ coordinates
                    for (int i = 0; i < Instance.Hierarchy.Nodes.Count; i++ )
                    {
                        var node = Instance.Hierarchy.Nodes[i];
                        node.Aabb = new AABBd(Constants.YZflip * (node.Aabb.min - Instance.Metadata.Offset), Constants.YZflip * (node.Aabb.max - Instance.Metadata.Offset));
                    }
                    Instance.Metadata.OffsetList = new List<double>(3) { Instance.Metadata.Offset.x, Instance.Metadata.Offset.z, Instance.Metadata.Offset.y };
                    Instance.Metadata.ScaleList = new List<double>(3) { Instance.Metadata.Scale.x, Instance.Metadata.Scale.z, Instance.Metadata.Scale.y };

                    // Setting the metadata BoundingBox to the values of the root node. No fliping required since that was done in the for loop
                    Instance.Metadata.BoundingBox.MinList = new List<double>(3) { Instance.Hierarchy.Root.Aabb.min.x, Instance.Hierarchy.Root.Aabb.min.y, Instance.Hierarchy.Root.Aabb.min.z };
                    Instance.Metadata.BoundingBox.MaxList = new List<double>(3) { Instance.Hierarchy.Root.Aabb.max.x, Instance.Hierarchy.Root.Aabb.max.y, Instance.Hierarchy.Root.Aabb.max.z };
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

        public PointType GetPointType(string pathToNodeFileFolder = "")
        {
            return PointType.PosD3ColF3LblB;
        }

        /// <summary>
        /// Reads the meta.json and .hierarchy files and returns an octree.
        /// </summary>
        /// <param name="fileFolderPath">Path to the folder the point cloud is saved</param>
        /// <returns></returns>
        public IPointCloudOctree GetOctree(string fileFolderPath)
        {
            _fileFolderPath = fileFolderPath;
            _metadataFilePath = Path.Combine(fileFolderPath, Constants.MetadataFileName);

            int pointSize = 0;

            if (Instance.Metadata != null)
            {
                foreach (var metaAttributeItem in Instance.Metadata.Attributes)
                {
                    pointSize += metaAttributeItem.Size;
                }

                Instance.Metadata.PointSize = pointSize;
            }

            var center = Instance.Hierarchy.Root.Aabb.Center;
            var size = Instance.Hierarchy.Root.Aabb.Size.y;
            var maxLvl = Instance.Metadata.Hierarchy.Depth;

            var octree = new PointCloudOctree(center, size, maxLvl);

            MapChildNodesRecursive(octree.Root, Instance.Hierarchy.Root);

            return octree;
        }

        public async Task<TPoint[]> LoadPointsForNodeAsync<TPoint>(string guid, IPointAccessor pointAccessor) where TPoint : new()
        {
            return await Task.Run(() => { return LoadNodeData<TPoint>(guid, pointAccessor); });
        }

        public TPoint[] LoadNodeData<TPoint>(string id, IPointAccessor pointAccessor) where TPoint : new()
        {
            var node = FindNode(id);
            TPoint[] points = null;

            if (node != null)
            {
                var octreeFilePath = Path.Combine(Instance.Metadata.FolderPath, Constants.OctreeFileName);
                var binaryReader = new BinaryReader(File.OpenRead(octreeFilePath));
                points = LoadNodeData<TPoint>((PointAccessor<TPoint>)pointAccessor, node, binaryReader);

                node.IsLoaded = true;

                binaryReader.Close();
                binaryReader.Dispose();
            }

            return points;
        }

        private TPoint[] LoadNodeData<TPoint>(PointAccessor<TPoint> pointAccessor, PotreeNode node, BinaryReader binaryReader) where TPoint : new()
        {
            var points = new TPoint[node.NumPoints];
            for (int i = 0; i < node.NumPoints; i++)
            {
                points[i] = new TPoint();
            }

            var attributeOffset = 0;

            foreach (var metaitem in Instance.Metadata.Attributes)
            {
                if (metaitem.Name == "position")
                {
                    for (int i = 0; i < node.NumPoints; i++)
                    {
                        binaryReader.BaseStream.Position = node.ByteOffset + attributeOffset + i * Instance.Metadata.PointSize;

                        double x = (binaryReader.ReadInt32() * Instance.Metadata.Scale.x); // + Instance.Metadata.Offset.x;
                        double y = (binaryReader.ReadInt32() * Instance.Metadata.Scale.y); // + Instance.Metadata.Offset.y;
                        double z = (binaryReader.ReadInt32() * Instance.Metadata.Scale.z); // + Instance.Metadata.Offset.z;

                        double3 position = new(x, y, z);
                        position = Constants.YZflip * position;

                        pointAccessor.SetPositionFloat3_64(ref points[i], position);
                    }
                }
                //else if (metaitem.Name.Equals("intensity"))
                //{
                //    for (int i = 0; i < node.NumPoints; i++)
                //    {
                //        binaryReader.BaseStream.Position = node.ByteOffset + attributeOffset + i * Instance.Metadata.PointSize;

                //        Int16 intensity = binaryReader.ReadInt16();
                //    }
                //}
                //else if (metaitem.Name.Equals("return number"))
                //{
                //    for (int i = 0; i < node.NumPoints; i++)
                //    {
                //        binaryReader.BaseStream.Position = node.ByteOffset + attributeOffset + i * Instance.Metadata.PointSize;

                //        byte returnNumber = binaryReader.ReadByte();
                //    }
                //}
                //else if (metaitem.Name.Equals("number of returns"))
                //{
                //    for (int i = 0; i < node.NumPoints; i++)
                //    {
                //        binaryReader.BaseStream.Position = node.ByteOffset + attributeOffset + i * Instance.Metadata.PointSize;

                //        byte numberOfReturns = binaryReader.ReadByte();
                //    }
                //}
                else if (metaitem.Name.Equals("classification"))
                {
                    for (int i = 0; i < node.NumPoints; i++)
                    {
                        binaryReader.BaseStream.Position = node.ByteOffset + attributeOffset + i * Instance.Metadata.PointSize;

                        byte label = binaryReader.ReadByte();

                        pointAccessor.SetLabelUInt_8(ref points[i], label);
                    }
                }
                //else if (metaitem.Name.Equals("scan angle rank"))
                //{
                //    for (int i = 0; i < node.NumPoints; i++)
                //    {
                //        binaryReader.BaseStream.Position = node.ByteOffset + attributeOffset + i * Instance.Metadata.PointSize;

                //        byte scanAnleRank = binaryReader.ReadByte();
                //    }
                //}
                //else if (metaitem.Name.Equals("user data"))
                //{
                //    for (int i = 0; i < node.NumPoints; i++)
                //    {
                //        binaryReader.BaseStream.Position = node.ByteOffset + attributeOffset + i * Instance.Metadata.PointSize;

                //        byte userData = binaryReader.ReadByte();
                //    }
                //}
                //else if (metaitem.Name.Equals("point source id"))
                //{
                //    for (int i = 0; i < node.NumPoints; i++)
                //    {
                //        binaryReader.BaseStream.Position = node.ByteOffset + attributeOffset + i * Instance.Metadata.PointSize;

                //        byte pointSourceId = binaryReader.ReadByte();
                //    }
                //}
                else if (metaitem.Name.Equals("rgb"))
                {
                    for (int i = 0; i < node.NumPoints; i++)
                    {
                        binaryReader.BaseStream.Position = node.ByteOffset + attributeOffset + i * Instance.Metadata.PointSize;

                        ushort r = binaryReader.ReadUInt16();
                        ushort g = binaryReader.ReadUInt16();
                        ushort b = binaryReader.ReadUInt16();

                        float3 color = float3.Zero;

                        color.r = ((byte)(r > 255 ? r / 256 : r));
                        color.g = ((byte)(g > 255 ? g / 256 : g));
                        color.b = ((byte)(b > 255 ? b / 256 : b));
                        pointAccessor.SetColorFloat3_32(ref points[i], color);
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

            var firstChunkSize = Instance.Metadata.Hierarchy.FirstChunkSize;
            var stepSize = Instance.Metadata.Hierarchy.StepSize;
            var depth = Instance.Metadata.Hierarchy.Depth;

            var data = File.ReadAllBytes(hierarchyFilePath);

            PotreeNode root = new PotreeNode()
            {
                Name = "r",
                Aabb = new AABBd(Instance.Metadata.BoundingBox.Min, Instance.Metadata.BoundingBox.Max)
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

                        PotreeNode child = new PotreeNode();

                        child.Aabb = childAABB(currentNode.Aabb, childIndex);
                        child.Name = childName;
                        currentNode.Children[childIndex] = child;
                        child.Parent = currentNode;

                        nodes.Add(child);
                    }
                }
            }

            AABBd childAABB(AABBd aabb, int index)
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
            return Instance.Hierarchy.Nodes.Find(n => n.Name == id);
        }
    }
}
