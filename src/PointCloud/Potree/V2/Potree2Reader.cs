using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance;
using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using Fusee.PointCloud.Core.Scene;
using Fusee.PointCloud.Potree.V2.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;

namespace Fusee.PointCloud.Potree.V2
{
    /// <summary>
    /// Reads Potree V2 files and is able to create a point cloud scene component, that can be rendered.
    /// </summary>
    public class Potree2Reader : Potree2AccessBase, IPointReader
    {
        /// <summary>
        /// Pass method how to handle the extra bytes, resulting uint will be passed into <see cref="Mesh.Flags"/>.
        /// </summary>
        public HandleReadExtraBytes? HandleReadExtraBytes { get; set; }

        /// <summary>
        /// If any errors during load occur this event is being called
        /// </summary>
        public EventHandler<ErrorEventArgs>? OnPointCloudReadError;

        /// <summary>
        /// Generate a new instance of <see cref="Potree2Reader"/>.
        /// </summary>
        /// <param name="filepath"></param>
        public Potree2Reader(string filepath)
        {
            ReadNewFile(filepath);
        }

        /// <summary>
        /// Generate a new instance of <see cref="Potree2Reader"/>.
        /// </summary>
        /// <param name="potreeData"></param>
        public Potree2Reader(PotreeData potreeData) : base(potreeData)
        {
            ReadFile(potreeData);
        }

        /// <summary>
        /// Returns a renderable point cloud component.
        /// </summary>
        /// <param name="renderMode">Determines which <see cref="RenderMode"/> is used to display the returned point cloud."/></param>
        public IPointCloud GetPointCloudComponent(RenderMode renderMode = RenderMode.StaticMesh)
        {
            var metaData = new CreateMeshMetaData()
            {
                PointSize = PotreeData.Metadata.PointSize,
                Scale = PotreeData.Metadata.Scale,
                OffsetColor = offsetColor,
                OffsetIntensity = offsetIntensity,
                OffsetToPosValues = offsetPosition,
                OffsetToExtraBytes = PotreeData.Metadata.OffsetToExtraBytes,
                IntensityMax = PotreeData.Metadata.Attributes["intensity"].MaxList[0],
                IntensityMin = PotreeData.Metadata.Attributes["intensity"].MinList[0],
                PositionType = SupportedPositionTypes.int32

            };

            var shiftedAabbCenter = (float3)(PotreeData.Metadata.AABB.Center - PotreeData.Metadata.Offset);

            switch (renderMode)
            {
                default:
                case RenderMode.StaticMesh:
                    {
                        var dataHandler = new PointCloudDataHandler<GpuMesh>(MeshMaker.CreateStaticMesh, HandleReadExtraBytes, metaData, LoadVisualizationPointData, GetNumberOfPointsInNode, GetAllBytesForAttribute);
                        dataHandler.OnLoadingErrorEvent += OnPointCloudReadError;
                        var imp = new Potree2Cloud(dataHandler, GetOctree(), (float3)PotreeData.Metadata.AABB.Size, shiftedAabbCenter);
                        return new PointCloudComponent(imp, renderMode);
                    }
                case RenderMode.Instanced:
                    {
                        var dataHandlerInstanced = new PointCloudDataHandler<InstanceData>(MeshMaker.CreateInstanceData, HandleReadExtraBytes, metaData, LoadVisualizationPointData, GetNumberOfPointsInNode, GetAllBytesForAttribute, true);
                        dataHandlerInstanced.OnLoadingErrorEvent += OnPointCloudReadError;
                        var imp = new Potree2CloudInstanced(dataHandlerInstanced, GetOctree(), (float3)PotreeData.Metadata.AABB.Size, shiftedAabbCenter);
                        return new PointCloudComponent(imp, renderMode);
                    }
                case RenderMode.DynamicMesh:
                    {
                        var dataHandlerDynamic = new PointCloudDataHandler<Mesh>(MeshMaker.CreateDynamicMesh, HandleReadExtraBytes, metaData, LoadVisualizationPointData, GetNumberOfPointsInNode, GetAllBytesForAttribute);
                        dataHandlerDynamic.OnLoadingErrorEvent += OnPointCloudReadError;
                        var imp = new Potree2CloudDynamic(dataHandlerDynamic, GetOctree(), (float3)PotreeData.Metadata.AABB.Size, shiftedAabbCenter);
                        return new PointCloudComponent(imp, renderMode);
                    }
            }
        }

        private long GetNumberOfPointsInNode(OctantId octantId)
        {
            Guard.IsNotNull(PotreeData);
            var node = PotreeData.GetNode(octantId);
            return node == null ? throw new ArgumentException($"Couldn't get node for id {octantId}.") : node.NumPoints;
        }

        /// <summary>
        /// Reads the Potree file and returns an octree.
        /// </summary>
        /// <returns></returns>
        public IPointCloudOctree GetOctree()
        {
            Guard.IsNotNull(PotreeData);
            Guard.IsNotNull(PotreeData.Metadata);
            Guard.IsNotNull(PotreeData.Hierarchy);
            Guard.IsNotNull(PotreeData.Hierarchy.Root);
            Guard.IsNotNull(PotreeData.Hierarchy.Root.Aabb);
            Guard.IsNotNull(PotreeData.Metadata.Hierarchy);

            var center = PotreeData.Hierarchy.Root.Aabb.Center;
            var size = PotreeData.Hierarchy.Root.Aabb.Size;//System.Math.Max(System.Math.Max(PotreeData.Hierarchy.Root.Aabb.Size.x, PotreeData.Hierarchy.Root.Aabb.Size.y), PotreeData.Hierarchy.Root.Aabb.Size.z);
            var maxLvl = PotreeData.Metadata.Hierarchy.Depth;

            var octree = new PointCloudOctree(center, size, maxLvl);

            MapChildNodesRecursive(octree.Root, PotreeData.Hierarchy.Root);

            return octree;
        }

        /// <summary>
        /// Returns all bytes of one node for a specific attribute.
        /// </summary>
        /// <param name="attribName"></param>
        /// <param name="pointsMmf"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public byte[] GetAllBytesForAttribute(string attribName, MemoryMappedFile pointsMmf, OctantId guid)
        {
            Guard.IsNotNull(PotreeData);
            var node = PotreeData.GetNode(guid);
            Guard.IsNotNull(node);
            var potreePointSize = (int)node.NumPoints * PotreeData.Metadata.PointSize;

            using var accessor = pointsMmf.CreateViewAccessor();
            var pointArray = new byte[potreePointSize];
            accessor.ReadArray(0, pointArray, 0, potreePointSize);

            var memStream = new MemoryStream();
            var attrib = PotreeData.Metadata.Attributes[attribName];

            for (var i = 0; i < pointArray.Length; i += PotreeData.Metadata.PointSize)
            {
                var extraBytesSpan = pointArray.AsSpan().Slice(i + attrib.AttributeOffset, attrib.Size);
                memStream.Write(extraBytesSpan);
            }

            return memStream.ToArray();
        }

        /// <summary>
        /// Reads the points for a specific octant of type <see cref="VisualizationPoint"/>.
        /// </summary>
        /// <param name="id">Id of the octant.</param>
        /// <returns></returns>
        public MemoryMappedFile LoadVisualizationPointData(OctantId id)
        {
            Guard.IsNotNull(PotreeData);
            var node = PotreeData.GetNode(id);

            // if node is null the hierarchy is broken and we look for an octant that isn't there...
            Guard.IsNotNull(node);

            return LoadVisualizationPoint(node);
        }

        private MemoryMappedFile LoadVisualizationPoint(PotreeNode node)
        {
            Guard.IsLessThanOrEqualTo(node.NumPoints, int.MaxValue);
            Guard.IsNotNull(PotreeData);

            return ReadRawNodeData(node);
        }

        private static void MapChildNodesRecursive(IPointCloudOctant octreeNode, PotreeNode potreeNode)
        {
            octreeNode.NumberOfPointsInNode = (int)potreeNode.NumPoints;

            for (int i = 0; i < potreeNode.Children.Length; i++)
            {
                if (potreeNode.Children[i] != null)
                {
                    var potreeChild = potreeNode.Children[i];

                    var octant = new PointCloudOctant(potreeNode.Children[i].Aabb.Center, potreeNode.Children[i].Aabb.Size, new OctantId(potreeChild.Name));

                    if (potreeChild.NodeType == NodeType.LEAF)
                    {
                        octant.IsLeaf = true;
                    }

                    MapChildNodesRecursive(octant, potreeChild);

                    octreeNode.Children[i] = octant;
                }
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

            foreach (var item in PotreeData.Metadata.Attributes.Values)
            {
                PotreeData.Metadata.PointSize += item.Size;
                if (PotreeData.Metadata.OffsetToExtraBytes > -1 && PotreeData.Metadata.PointSize > PotreeData.Metadata.OffsetToExtraBytes)
                    item.IsExtraByte = true;
                else
                    item.IsExtraByte = false;
            }

            CacheMetadata(true);

            return PotreeData;
        }

        /// <summary>
        /// Calculate the principal axis rotation from given data
        ///    - Load root node
        ///    - Convert to covariance matrix
        ///    - Calculate principal axis
        /// </summary>
        /// <returns></returns>
        public float4x4 CalculatePrincipalAxis()
        {
            Guard.IsNotNull(PotreeData);
            var potreeRootSize = (int)PotreeData.Hierarchy.Root.NumPoints * PotreeData.Metadata.PointSize;
            using var mmf = LoadVisualizationPoint(PotreeData.Hierarchy.Root);
            using var accessor = mmf.CreateViewAccessor();

            var potreeRootRaw = new byte[potreeRootSize];
            accessor.ReadArray(0, potreeRootRaw, 0, potreeRootSize);

            var rootPoints = new float3[(int)PotreeData.Hierarchy.Root.NumPoints];
            var pointCounter = 0;
            var sizeOffloat3 = Marshal.SizeOf<int>() * 3;

            for (var i = 0; i < potreeRootRaw.Length; i += PotreeData.Metadata.PointSize)
            {
                var posSlice = new Span<byte>(potreeRootRaw).Slice(i + offsetPosition, sizeOffloat3);
                var pos = MemoryMarshal.Cast<byte, int>(posSlice);

                float x = (float)(pos[0] * PotreeData.Metadata.Scale.x);
                float y = (float)(pos[1] * PotreeData.Metadata.Scale.y);
                float z = (float)(pos[2] * PotreeData.Metadata.Scale.z);

                float3 position = (float4x4)Potree2Consts.YZflip * new float3(x, y, z);
                rootPoints[pointCounter] = position;
                pointCounter++;
            }

            try
            {
                var eigen = new Eigen(rootPoints);
                PotreeData.Metadata.PrincipalAxisRotation = (float4x4)eigen.RotationMatrix;
                return (float4x4)eigen.RotationMatrix;
            }
            catch (ArithmeticException ex)
            {
                Diagnostics.Warn(ex);
                return float4x4.Identity;
            }
        }



        /// <summary>
        /// Changes the potree data package that is currently bound to the reader. So a reader can be used for multiple data packages, this avoids rereading the potree data like in <see cref="ReadNewFile(string)"/>.
        /// </summary>
        /// <param name="potreeData">Meta and octree data of the potree file.</param>
        public void ReadFile(PotreeData potreeData)
        {
            PotreeData = potreeData;
            CacheMetadata(true);
        }

        #region LoadHierarchy

        private static (PotreeMetadata, PotreeHierarchy) LoadHierarchy(string folderPath)
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

            //TODO: this is needed because the .NET Potree Converter is producing Nodes with 0 points right now. Remove when this is fixed.
            CleanupHierarchy(Hierarchy.Root);

            Hierarchy.Nodes = new();
            Hierarchy.Root.Traverse(n => Hierarchy.Nodes.Add(n));

            Potree2Reader.FlipYZAxis(Metadata, Hierarchy);

            return (Metadata, Hierarchy);
        }

        private static void CleanupHierarchy(PotreeNode root)
        {
            Stack<PotreeNode> stack = new();
            stack.Push(root);

            while (stack.Count > 0)
            {
                PotreeNode node = stack.Pop();

                for (int i = 0; i < node.Children.Length; i++)
                {
                    var child = node.Children[i];

                    if (child != null)
                    {
                        if (child.NumPoints == 0)
                            node.Children[i] = null;
                        else
                            stack.Push(child);
                    }
                }
            }
        }

        private static PotreeMetadata LoadPotreeMetadata(string metadataFilepath)
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new ConvertIPointWriterHierarchy());
            var metaData = File.ReadAllText(metadataFilepath);
            var potreeData = JsonConvert.DeserializeObject<PotreeMetadata>(metaData, settings);
            Guard.IsNotNull(potreeData, nameof(potreeData));

            return potreeData;
        }


        internal class ConvertIPointWriterHierarchy : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(IPointWriterHierarchy);
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                return serializer.Deserialize(reader, typeof(PotreeSettingsHierarchy));
            }

            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
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
                currentNode ??= new PotreeNode();

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

        private static void FlipYZAxis(PotreeMetadata potreeMetadata, PotreeHierarchy potreeHierarchy)
        {
            for (int i = 0; i < potreeHierarchy?.Nodes?.Count; i++)
            {
                var node = potreeHierarchy.Nodes[i];
                node.Aabb = new AABBd(Potree2Consts.YZflip * (node.Aabb.min - potreeMetadata.Offset), Potree2Consts.YZflip * (node.Aabb.max - potreeMetadata.Offset));
            }
            potreeMetadata.OffsetList = new List<double>(3) { potreeMetadata.Offset.x, potreeMetadata.Offset.z, potreeMetadata.Offset.y };
            potreeMetadata.ScaleList = new List<double>(3) { potreeMetadata.Scale.x, potreeMetadata.Scale.z, potreeMetadata.Scale.y };
            potreeMetadata.BoundingBox.MaxList = new List<double>(3) { potreeMetadata.BoundingBox.Max.x, potreeMetadata.BoundingBox.Max.z, potreeMetadata.BoundingBox.Max.y };
            potreeMetadata.BoundingBox.MinList = new List<double>(3) { potreeMetadata.BoundingBox.Min.x, potreeMetadata.BoundingBox.Min.z, potreeMetadata.BoundingBox.Min.y };
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
    }
}