using CommunityToolkit.Diagnostics;
using Fusee.Math.Core;
using Fusee.PointCloud.Potree.V2.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fusee.PointCloud.Potree.V2
{
    /// <summary>
    /// Contains information about the Potree file's meta data and hierarchy/octree.
    /// </summary>
    public class PotreeData
    {
        /// <summary>
        /// The hierarchy as linear list of <see cref="PotreeNode"/>s.
        /// </summary>
        public PotreeHierarchy Hierarchy;

        /// <summary>
        /// The meta data of the file.
        /// </summary>
        public PotreeMetadata Metadata;

        public PotreeData(string folderPath)
        {
            Guard.IsNotNullOrWhiteSpace(folderPath, nameof(folderPath));

            LoadHierarchy(folderPath);
        }

        private void LoadHierarchy(string folderPath)
        {
            var metadataFilePath = Path.Combine(folderPath, Potree2Consts.MetadataFileName);
            var hierarchyFilePath = Path.Combine(folderPath, Potree2Consts.HierarchyFileName);

            Guard.IsTrue(File.Exists(metadataFilePath), metadataFilePath);
            Guard.IsTrue(File.Exists(metadataFilePath), hierarchyFilePath);

            Metadata = LoadPotreeMetadata(metadataFilePath);
            Hierarchy = new()
            {
                Root = new()
                {
                    Name = "r",
                }
            };

            Metadata.Attributes = GetAttributesDict(Metadata.AttributesList);

            Metadata.FolderPath = folderPath;

            CalculateAttributeOffsets(ref Metadata);

            Hierarchy.Root.Aabb = new AABBd(Metadata.BoundingBox.Min, Metadata.BoundingBox.Max);

            var data = File.ReadAllBytes(hierarchyFilePath);

            Guard.IsNotNull(data, nameof(data));

            LoadHierarchyRecursive(ref Hierarchy.Root, ref data, 0, Metadata.Hierarchy.FirstChunkSize);

            Hierarchy.Nodes = new();
            Hierarchy.Root.Traverse(n => Hierarchy.Nodes.Add(n));

            FlipYZAxis();

            Metadata.BoundingBox.MinList = new List<double>(3) { Hierarchy.Root.Aabb.min.x, Hierarchy.Root.Aabb.min.y, Hierarchy.Root.Aabb.min.z };
            Metadata.BoundingBox.MaxList = new List<double>(3) { Hierarchy.Root.Aabb.max.x, Hierarchy.Root.Aabb.max.y, Hierarchy.Root.Aabb.max.z };
        }

        private static PotreeMetadata LoadPotreeMetadata(string metadataFilepath)
        {
            var potreeData = JsonConvert.DeserializeObject<PotreeMetadata>(File.ReadAllText(metadataFilepath));

            Guard.IsNotNull(potreeData, nameof(potreeData));

            return potreeData;
        }

        private static void LoadHierarchyRecursive(ref PotreeNode root, ref byte[] data, long offset, long size)
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
                        bool childExists = (1 << childIndex & childMask) != 0;

                        if (!childExists)
                        {
                            continue;
                        }

                        string childName = currentNode.Name + childIndex.ToString();

                        PotreeNode child = new()
                        {
                            Aabb = ChildAABB(currentNode.Aabb, childIndex),
                            Name = childName
                        };
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

        private void FlipYZAxis()
        {
            for (int i = 0; i < Hierarchy.Nodes.Count; i++)
            {
                var node = Hierarchy.Nodes[i];
                node.Aabb = new AABBd(Potree2Consts.YZflip * (node.Aabb.min - Metadata.Offset), Potree2Consts.YZflip * (node.Aabb.max - Metadata.Offset));
            }
            Metadata.OffsetList = new List<double>(3) { Metadata.Offset.x, Metadata.Offset.z, Metadata.Offset.y };
            Metadata.ScaleList = new List<double>(3) { Metadata.Scale.x, Metadata.Scale.z, Metadata.Scale.y };
        }

        private static void CalculateAttributeOffsets(ref PotreeMetadata potreeMetadata)
        {
            var attributeOffset = 0;

            for (int i = 0; i < potreeMetadata.AttributesList.Count; i++)
            {
                potreeMetadata.AttributesList[i].AttributeOffset = attributeOffset;

                attributeOffset += potreeMetadata.AttributesList[i].Size;
            }
        }

        private static Dictionary<string, PotreeSettingsAttribute> GetAttributesDict(List<PotreeSettingsAttribute> attributes)
        {
            return attributes.ToDictionary(x => x.Name, x => x);
        }
    }
}