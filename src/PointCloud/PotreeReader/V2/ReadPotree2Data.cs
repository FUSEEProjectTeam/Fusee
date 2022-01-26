using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using Fusee.PointCloud.PotreeReader.V2.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Fusee.PointCloud.PotreeReader.V2
{
    public class ReadPotree2Data : IOutOfCoreReader
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
            return PointType.Position_double__Color_float__Label_byte;
        }

        /// <summary>
        /// Reads the meta.json and .hierarchy files and returns an octree.
        /// </summary>
        /// <param name="fileFolderPath">Path to the folder the point cloud is saved</param>
        /// <returns></returns>
        public PtOctree GetOctree(string fileFolderPath)
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

            var center = double3.Zero; // Instance.Hierarchy.TreeRoot.Aabb.Center;
            var size = Instance.Hierarchy.TreeRoot.Aabb.Size.y;
            var maxLvl = Instance.Metadata.Hierarchy.Depth;

            var root = new PtOctant(center, size, "r");

            MapChildNodesRecursive(root, Instance.Hierarchy.TreeRoot);

            var octree = new PtOctree(root)
            {
                MaxLevel = maxLvl
            };

            return octree;
        }

        public async Task<IPointCloudPoint[]> LoadPointsForNodeAsync(string guid, IPointAccessor pointAccessor)
        {
            return await Task.Run(() => { return LoadNodeData(guid, pointAccessor); });
        }

        public IPointCloudPoint[] LoadNodeData(string id, IPointAccessor pointAccessor)
        {
            var node = FindNode(id);

            var octreeFilePath = Path.Combine(Instance.Metadata.FolderPath, Constants.OctreeFileName);
            var binaryReader = new BinaryReader(File.OpenRead(octreeFilePath));
            IPointCloudPoint[] points;

            switch (pointAccessor.PointType)
            {
                default:
                case PointType.Undefined:
                    throw new ArgumentException();
                case PointType.Pos64:
                case PointType.Pos64Col32IShort:
                case PointType.Pos64IShort:
                case PointType.Pos64Col32:
                case PointType.Pos64Label8:
                case PointType.Pos64Nor32Col32IShort:
                case PointType.Pos64Nor32IShort:
                case PointType.Pos64Nor32Col32:
                    throw new NotImplementedException();
                case PointType.Position_double__Color_float__Label_byte:
                    points = LoadNodeData__((Position_double__Color_float__Label_byte___Accessor)pointAccessor, node, binaryReader);
                    break;
            }

            node.IsLoaded = true;

            binaryReader.Close();
            binaryReader.Dispose();

            return points;
        }

        private IPointCloudPoint[] LoadNodeData__(Position_double__Color_float__Label_byte___Accessor pointAccessor, PotreeNode node, BinaryReader binaryReader)
        {
            var points = new IPointCloudPoint[node.NumPoints];
            for (int i = 0; i < node.NumPoints; i++)
            {
                points[i] = new Position_double__Color_float__Label_byte();
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
                        var typedPt = (Position_double__Color_float__Label_byte)points[i];
                        pointAccessor.SetPositionFloat3_64(ref typedPt, position);
                        points[i] = typedPt;
                    }
                }
                else if (metaitem.Name.Contains("rgb"))
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
                        var typedPt = (Position_double__Color_float__Label_byte)points[i];
                        pointAccessor.SetColorFloat3_32(ref typedPt, color);
                        points[i] = typedPt;
                    }
                }
                else if (metaitem.Name.Equals("classification"))
                {
                    for (int i = 0; i < node.NumPoints; i++)
                    {
                        binaryReader.BaseStream.Position = node.ByteOffset + attributeOffset + i + Instance.Metadata.PointSize;

                        byte label = (byte)binaryReader.ReadSByte();
                        var typedPt = (Position_double__Color_float__Label_byte)points[i];
                        pointAccessor.SetLabelUInt_8(ref typedPt, label);
                        points[i] = typedPt;
                    }
                }

                attributeOffset += metaitem.Size;
            }

            return points;
        }
        
        private void MapChildNodesRecursive(PtOctant octreeNode, PotreeNode potreeNode)
        {
            octreeNode.NumberOfPointsInNode = (int)potreeNode.NumPoints;

            for (int i = 0; i < potreeNode.children.Length; i++)
            {
                if (potreeNode.children[i] != null)
                {
                    var octant = new PtOctant(potreeNode.children[i].Aabb.Center - Instance.Metadata.Offset, potreeNode.children[i].Aabb.Size.y, potreeNode.children[i].Name);

                    if (potreeNode.children[i].NodeType == NodeType.LEAF)
                    {
                        octant.IsLeaf = true;
                    }

                    MapChildNodesRecursive(octant, potreeNode.children[i]);

                    octreeNode.Children[i] = octant;
                }
            }
        }

        private PotreeHierarchy LoadHierarchy(string fileFolderPath)
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

            hierarchy.TreeRoot = root;

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
                        currentNode.children[childIndex] = child;
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
