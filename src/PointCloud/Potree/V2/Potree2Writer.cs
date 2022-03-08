using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Fusee.PointCloud.Potree.V2.Data;

namespace Fusee.PointCloud.Potree.V2
{
    public class Potree2Writer
    {
        public Potree2Writer(string filepath)
        {
            GetOctree(filepath);
        }

        public (long octants, long points) Label(Predicate<PotreeNode> nodeSelector, Predicate<double3> pointSelector, byte Label, bool dryrun = false)
        {
            long octantCount = 0;
            long pointsCount = 0;

            var octreeFilePath = Path.Combine(Instance.Metadata.FolderPath, Constants.OctreeFileName);

            using (Stream readStream = File.Open(octreeFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (Stream writeStream = File.Open(octreeFilePath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
            {
                BinaryReader binaryReader = new BinaryReader(readStream);
                BinaryWriter binaryWriter = new BinaryWriter(writeStream);

                double3 point = double3.Zero;

                foreach (var node in Instance.Hierarchy.Nodes)
                {
                    if (nodeSelector(node))
                    {
                        octantCount++;

                        for (int i = 0; i < node.NumPoints; i++)
                        {
                            binaryReader.BaseStream.Position = node.ByteOffset + 0 + i * Instance.Metadata.PointSize;

                            point.x = (binaryReader.ReadInt32() * Instance.Metadata.Scale.x);
                            point.z = (binaryReader.ReadInt32() * Instance.Metadata.Scale.y);
                            point.y = (binaryReader.ReadInt32() * Instance.Metadata.Scale.z);

                            if (pointSelector(point))
                            {
                                if (!dryrun)
                                {
                                    binaryWriter.BaseStream.Position = node.ByteOffset + 16 + i * Instance.Metadata.PointSize;
                                    binaryWriter.Write(Label);
                                }

                                pointsCount++;
                            }
                        }
                    }
                }
            }

            return (octantCount, pointsCount);
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

                    // Changing AABBs to have local coordinates
                    // Fliping all YZ coordinates
                    for (int i = 0; i < Instance.Hierarchy.Nodes.Count; i++)
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
