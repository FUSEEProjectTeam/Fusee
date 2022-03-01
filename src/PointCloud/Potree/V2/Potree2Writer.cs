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
    public class Potree2Writer
    {
        public Potree2Writer(string filepath)
        {
            GetOctree(filepath);
        }

        private double4x4 XYflipZup = new double4x4()
        {
            M11 = 0, M12 = 1, M13 = 0, M14 = 0,
            M21 = 1, M22 = 0, M23 = 0, M24 = 0,
            M31 = 0, M32 = 0, M33 = -1, M34 = 0,
            M41 = 0, M42 = 0, M43 = 0, M44 = 0
        };

        public long LableMinMax(double3 min, double3 max, byte Label)
        {
            min = XYflipZup * min;
            max = XYflipZup * max;

            var octreeFilePath = Path.Combine(Instance.Metadata.FolderPath, Constants.OctreeFileName);
            var aabb = new AABBd(min, max);

            long cnt = 0;

            using (Stream readStream = File.Open(octreeFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (Stream writeStream = File.Open(octreeFilePath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
            {
                BinaryReader binaryReader = new BinaryReader(readStream);
                BinaryWriter binaryWriter = new BinaryWriter(writeStream);

                foreach (var node in Instance.Hierarchy.Nodes)
                {
                    if (aabb.Intersects(node.Aabb))
                    {
                        for (int i = 0; i < node.NumPoints; i++)
                        {
                            binaryReader.BaseStream.Position = node.ByteOffset + 0 + i * Instance.Metadata.PointSize;

                            double x = (binaryReader.ReadInt32() * Instance.Metadata.Scale.x); //- Instance.Metadata.Offset.x;
                            double y = (binaryReader.ReadInt32() * Instance.Metadata.Scale.y); //- Instance.Metadata.Offset.y;
                            double z = (binaryReader.ReadInt32() * Instance.Metadata.Scale.z); //- Instance.Metadata.Offset.z;

                            if (x > aabb.min.x && y > aabb.min.y && z > aabb.min.z &&
                                x < aabb.max.x && y < aabb.max.y && z < aabb.max.z)
                            {
                                //binaryWriter.BaseStream.Position = node.ByteOffset + 16 + i + Instance.Metadata.PointSize;
                                //binaryWriter.Write((sbyte)Label);

                                cnt++;
                            }
                        }
                    }
                }
            }
            return cnt;
        }

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
            return PointType.PosD3ColF3LblB;
        }

        /// <summary>
        /// Reads the meta.json and .hierarchy files and returns an octree.
        /// </summary>
        /// <param name="fileFolderPath">Path to the folder the point cloud is saved</param>
        /// <returns></returns>
        public void GetOctree(string fileFolderPath)
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
                Aabb = new AABBd(Instance.Metadata.BoundingBox.Min - Instance.Metadata.Offset, Instance.Metadata.BoundingBox.Max - Instance.Metadata.Offset)
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
