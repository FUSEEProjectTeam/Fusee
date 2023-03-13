using CommunityToolkit.Diagnostics;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Potree.V2.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;

namespace Fusee.PointCloud.Potree.V2
{
    /// <summary>
    /// This is the base class for reading and writing <see cref="VisualizationPoint"/>s.
    /// </summary>
    public abstract class Potree2ReaderBase : IDisposable
    {
        /// <summary>
        /// The <see cref="Data.PotreeData"/>
        /// </summary>
        public PotreeData? PotreeData
        {
            get => _potreeData;
            private set
            {
                _potreeData = value;
            }
        }
        private PotreeData? _potreeData;

        /// <summary>
        /// Save if metadata has already been cached
        /// </summary>
        protected bool _isMetadataCached = false;

        /// <summary>
        /// Offset in bytes to the position value in bytes in raw Potree stream
        /// </summary>
        protected int offsetPosition = -1;
        /// <summary>
        /// Offset in bytes to the intensity value in bytes in raw Potree stream
        /// </summary>
        protected int offsetIntensity = -1;
        /// <summary>
        /// Offset in bytes to the return number value in bytes in raw Potree stream
        /// </summary>
        protected int offsetReturnNumber = -1;
        /// <summary>
        /// Offset in bytes to the number of returns value in bytes in raw Potree stream
        /// </summary>
        protected int offsetNumberOfReturns = -1;
        /// <summary>
        /// Offset in bytes to the classification value in bytes in raw Potree stream
        /// </summary>
        protected int offsetClassification = -1;
        /// <summary>
        /// Offset in bytes to the scan angle rank value in bytes in raw Potree stream
        /// </summary>
        protected int offsetScanAngleRank = -1;
        /// <summary>
        /// Offset in bytes to the user data value in bytes in raw Potree stream
        /// </summary>
        protected int offsetUserData = -1;
        /// <summary>
        /// Offset in bytes to the point source id value in bytes in raw Potree stream
        /// </summary>
        protected int offsetPointSourceId = -1;
        /// <summary>
        /// Offset in bytes to the color value in bytes in raw Potree stream
        /// </summary>
        protected int offsetColor = -1;

        private MemoryMappedViewAccessor? _octreeViewAccessor;
        private bool disposedValue;

        /// <summary>
        /// The <see cref="MemoryMappedViewAccessor"/> to the underlying octree.bin file. This is threadsafe since there is never any overlapping access per our design.
        /// </summary>
        protected MemoryMappedViewAccessor OctreeMappedViewAccessor
        {
            get
            {
                Guard.IsNotNull(_octreeViewAccessor, nameof(_octreeViewAccessor));

                return _octreeViewAccessor;
            }
            set
            {
                _octreeViewAccessor?.Dispose();

                _octreeViewAccessor = value;
            }
        }

        /// <summary>
        /// Reads a potree file.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <returns>Meta and octree data of the potree file.</returns>
        public PotreeData ReadNewFile(string path)
        {
            (var Metadata, var Hierarchy) = LoadHierarchy(path);

            PotreeData = new PotreeData(Hierarchy, Metadata);

            CacheMetadata(true);

            OctreeMappedViewAccessor = PotreeData.OctreeMappedFile.CreateViewAccessor();

            return PotreeData;
        }

        /// <summary>
        /// Changes the potree data package that is currently bound to the reader. So a reader can be used for multiple data packages, this avoids rereading the potree data like in <see cref="ReadNewFile(string)"/>.
        /// </summary>
        /// <param name="potreeData">Meta and octree data of the potree file.</param>
        public void ReadFile(PotreeData potreeData)
        {
            PotreeData = potreeData;

            CacheMetadata(true);

            OctreeMappedViewAccessor = PotreeData.OctreeMappedFile.CreateViewAccessor();
        }

        /// <summary>
        /// Read and cache metadata from the metadata.json file
        /// </summary>
        protected void CacheMetadata(bool force = false)
        {
            Guard.IsNotNull(_potreeData);

            if (!_isMetadataCached || force)
            {
                offsetPosition = -1;
                offsetIntensity = -1;
                offsetReturnNumber = -1;
                offsetNumberOfReturns = -1;
                offsetClassification = -1;
                offsetScanAngleRank = -1;
                offsetUserData = -1;
                offsetPointSourceId = -1;
                offsetColor = -1;

                if (_potreeData.Metadata.Attributes.ContainsKey("position"))
                {
                    offsetPosition = _potreeData.Metadata.Attributes["position"].AttributeOffset;
                }
                if (_potreeData.Metadata.Attributes.ContainsKey("intensity"))
                {
                    offsetIntensity = _potreeData.Metadata.Attributes["intensity"].AttributeOffset;
                }
                if (_potreeData.Metadata.Attributes.ContainsKey("return number"))
                {
                    offsetReturnNumber = _potreeData.Metadata.Attributes["return number"].AttributeOffset;
                }
                if (_potreeData.Metadata.Attributes.ContainsKey("number of returns"))
                {
                    offsetNumberOfReturns = _potreeData.Metadata.Attributes["number of returns"].AttributeOffset;
                }
                if (_potreeData.Metadata.Attributes.ContainsKey("classification"))
                {
                    offsetClassification = _potreeData.Metadata.Attributes["classification"].AttributeOffset;
                }
                if (_potreeData.Metadata.Attributes.ContainsKey("scan angle rank"))
                {
                    offsetScanAngleRank = _potreeData.Metadata.Attributes["scan angle rank"].AttributeOffset;
                }
                if (_potreeData.Metadata.Attributes.ContainsKey("user data"))
                {
                    offsetUserData = _potreeData.Metadata.Attributes["user data"].AttributeOffset;
                }
                if (_potreeData.Metadata.Attributes.ContainsKey("point source id"))
                {
                    offsetPointSourceId = _potreeData.Metadata.Attributes["point source id"].AttributeOffset;
                }
                if (_potreeData.Metadata.Attributes.ContainsKey("rgb"))
                {
                    offsetColor = _potreeData.Metadata.Attributes["rgb"].AttributeOffset;
                }

                int pointSize = 0;

                if (_potreeData.Metadata != null)
                {
                    foreach (var metaAttributeItem in _potreeData.Metadata.AttributesList)
                    {
                        pointSize += metaAttributeItem.Size;
                    }

                    _potreeData.Metadata.PointSize = pointSize;
                }

                _isMetadataCached = true;
            }
        }

        /// <summary>
        /// Iterate the hierarchy, find the node from the given <see cref="OctantId"/>.
        /// </summary>
        /// <param name="potreeHierarchy"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static PotreeNode? FindNode(ref PotreeHierarchy potreeHierarchy, OctantId id)
        {
            return potreeHierarchy.Nodes.Find(n => n.Name == OctantId.OctantIdToPotreeName(id));
        }

        #region LoadHierarchy

        private (PotreeMetadata, PotreeHierarchy) LoadHierarchy(string folderPath)
        {
            var metadataFilePath = Path.Combine(folderPath, Potree2Consts.MetadataFileName);
            var hierarchyFilePath = Path.Combine(folderPath, Potree2Consts.HierarchyFileName);

            Guard.IsTrue(File.Exists(metadataFilePath), metadataFilePath);
            Guard.IsTrue(File.Exists(metadataFilePath), hierarchyFilePath);

            var Metadata = LoadPotreeMetadata(metadataFilePath);
            var Hierarchy = new PotreeHierarchy()
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

            FlipYZAxis(Metadata, Hierarchy);

            Metadata.BoundingBox.MinList = new List<double>(3) { Hierarchy.Root.Aabb.min.x, Hierarchy.Root.Aabb.min.y, Hierarchy.Root.Aabb.min.z };
            Metadata.BoundingBox.MaxList = new List<double>(3) { Hierarchy.Root.Aabb.max.x, Hierarchy.Root.Aabb.max.y, Hierarchy.Root.Aabb.max.z };

            return (Metadata, Hierarchy);
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

        private void FlipYZAxis(PotreeMetadata potreeMetadata, PotreeHierarchy potreeHierarchy)
        {
            for (int i = 0; i < potreeHierarchy.Nodes.Count; i++)
            {
                var node = potreeHierarchy.Nodes[i];
                node.Aabb = new AABBd(Potree2Consts.YZflip * (node.Aabb.min - potreeMetadata.Offset), Potree2Consts.YZflip * (node.Aabb.max - potreeMetadata.Offset));
            }
            potreeMetadata.OffsetList = new List<double>(3) { potreeMetadata.Offset.x, potreeMetadata.Offset.z, potreeMetadata.Offset.y };
            potreeMetadata.ScaleList = new List<double>(3) { potreeMetadata.Scale.x, potreeMetadata.Scale.z, potreeMetadata.Scale.y };
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

        #endregion LoadHierarchy


        /// <summary>
        /// Disposes of the MemoryMapped objects to free the file.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _octreeViewAccessor?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        /// <summary>
        /// Manually calling this disposes of the MemoryMapped objects to free the file.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}