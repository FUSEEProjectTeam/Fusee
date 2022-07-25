using Fusee.Math.Core;
using Fusee.PointCloud.Potree.V2.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fusee.PointCloud.Potree.V2
{
    internal static class Potree2HierarchyReader
    {
        internal static PotreeReaderMetadata LoadHierarchy(string folderPath)
        {
            var metadataFilePath = Path.Combine(folderPath, Constants.MetadataFileName);
            var hierarchyFilePath = Path.Combine(folderPath, Constants.HierarchyFileName);

            PotreeReaderMetadata potreeReaderMetadata = new()
            {
                Metadata = LoadPotreeMetadata(metadataFilePath),
                Hierarchy = new()
                {
                    Root = new()
                    {
                        Name = "r",
                    }
                }
            };

            potreeReaderMetadata.Metadata.FolderPath = folderPath;

            CalculateAttributeOffsets(ref potreeReaderMetadata.Metadata);
            BoolifyKnownAttributes(ref potreeReaderMetadata.Metadata);

            potreeReaderMetadata.Hierarchy.Root.Aabb = new AABBd(potreeReaderMetadata.Metadata.BoundingBox.Min, potreeReaderMetadata.Metadata.BoundingBox.Max);

            var data = File.ReadAllBytes(hierarchyFilePath);
            LoadHierarchyRecursive(ref potreeReaderMetadata.Hierarchy.Root, ref data, 0, potreeReaderMetadata.Metadata.Hierarchy.FirstChunkSize);

            potreeReaderMetadata.Hierarchy.Nodes = new();
            potreeReaderMetadata.Hierarchy.Root.Traverse(n => potreeReaderMetadata.Hierarchy.Nodes.Add(n));

            FlipYZAxis(ref potreeReaderMetadata);

            potreeReaderMetadata.Metadata.BoundingBox.MinList = new List<double>(3) { potreeReaderMetadata.Hierarchy.Root.Aabb.min.x, potreeReaderMetadata.Hierarchy.Root.Aabb.min.y, potreeReaderMetadata.Hierarchy.Root.Aabb.min.z };
            potreeReaderMetadata.Metadata.BoundingBox.MaxList = new List<double>(3) { potreeReaderMetadata.Hierarchy.Root.Aabb.max.x, potreeReaderMetadata.Hierarchy.Root.Aabb.max.y, potreeReaderMetadata.Hierarchy.Root.Aabb.max.z };

            return potreeReaderMetadata;
        }

        static PotreeMetadata LoadPotreeMetadata(string metadataFilepath)
        {
            return JsonConvert.DeserializeObject<PotreeMetadata>(File.ReadAllText(metadataFilepath));
        }

        static void LoadHierarchyRecursive(ref PotreeNode root, ref byte[] data, long offset, long size)
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

        static void FlipYZAxis(ref PotreeReaderMetadata potreeReaderMetadata)
        {
            for (int i = 0; i < potreeReaderMetadata.Hierarchy.Nodes.Count; i++)
            {
                var node = potreeReaderMetadata.Hierarchy.Nodes[i];
                node.Aabb = new AABBd(Constants.YZflip * (node.Aabb.min - potreeReaderMetadata.Metadata.Offset), Constants.YZflip * (node.Aabb.max - potreeReaderMetadata.Metadata.Offset));
            }
            potreeReaderMetadata.Metadata.OffsetList = new List<double>(3) { potreeReaderMetadata.Metadata.Offset.x, potreeReaderMetadata.Metadata.Offset.z, potreeReaderMetadata.Metadata.Offset.y };
            potreeReaderMetadata.Metadata.ScaleList = new List<double>(3) { potreeReaderMetadata.Metadata.Scale.x, potreeReaderMetadata.Metadata.Scale.z, potreeReaderMetadata.Metadata.Scale.y };
        }

        static void CalculateAttributeOffsets(ref PotreeMetadata potreeMetadata)
        {
            var attributeOffset = 0;


            for (int i = 0; i < potreeMetadata.Attributes.Count; i++)
            {
                potreeMetadata.Attributes[i].AttributeOffset = attributeOffset;

                attributeOffset += potreeMetadata.Attributes[i].Size;
            }
        }

        static void BoolifyKnownAttributes(ref PotreeMetadata potreeMetadata)
        {
            foreach (var attribute in potreeMetadata.Attributes)
            {
                if (attribute.Name.Equals("position"))
                {
                    potreeMetadata.HasPositionAttribute = true;
                }
                if (attribute.Name.Equals("classification"))
                {
                    potreeMetadata.HasClassificationAttribute = true;
                }
                if (attribute.Name.Equals("rgb"))
                {
                    potreeMetadata.HasColorAttribute = true;
                }
            }
        }
    }
}