using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;
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
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Fusee.PointCloud.Potree.V2
{
    /// <summary>
    /// Delegate for a method that knows how to parse a slice of a point's extra bytes to a valid uint.
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public delegate uint HandleReadExtraBytes(Span<byte> bytes);

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
        /// Specify the byte offset for one point until the extra byte data is reached
        /// </summary>
        public int OffsetToExtraBytes = -1;

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
            switch (renderMode)
            {
                default:
                case RenderMode.StaticMesh:
                    {
                        var dataHandler = new PointCloudDataHandler<GpuMesh>(MeshMaker.CreateStaticMesh,
                            LoadVisualizationPointData);
                        dataHandler.OnLoadingErrorEvent += OnPointCloudReadError;
                        var imp = new Potree2Cloud(dataHandler, GetOctree());
                        return new PointCloudComponent(imp, renderMode);
                    }
                case RenderMode.Instanced:
                    {
                        var dataHandlerInstanced = new PointCloudDataHandler<InstanceData>(MeshMaker.CreateInstanceData,
                            LoadVisualizationPointData, true);
                        dataHandlerInstanced.OnLoadingErrorEvent += OnPointCloudReadError;
                        var imp = new Potree2CloudInstanced(dataHandlerInstanced, GetOctree());
                        return new PointCloudComponent(imp, renderMode);
                    }
                case RenderMode.DynamicMesh:
                    {
                        var dataHandlerDynamic = new PointCloudDataHandler<Mesh>(MeshMaker.CreateDynamicMesh,
                            LoadVisualizationPointData);
                        dataHandlerDynamic.OnLoadingErrorEvent += OnPointCloudReadError;
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
            Guard.IsNotNull(PotreeData);
            Guard.IsNotNull(PotreeData.Metadata);

            var center = PotreeData.Hierarchy.Root.Aabb.Center;
            var size = PotreeData.Hierarchy.Root.Aabb.Size.y;
            var maxLvl = PotreeData.Metadata.Hierarchy.Depth;

            var octree = new PointCloudOctree(center, size, maxLvl);

            MapChildNodesRecursive(octree.Root, PotreeData.Hierarchy.Root);

            return octree;
        }

        /// <summary>
        /// Reads the points for a specific octant of type <see cref="VisualizationPoint"/>.
        /// </summary>
        /// <param name="id">Id of the octant.</param>
        /// <returns></returns>
        public MemoryOwner<VisualizationPoint> LoadVisualizationPointData(OctantId id)
        {
            Guard.IsNotNull(PotreeData);
            var node = PotreeData.GetNode(id);

            // if node is null the hierarchy is broken and we look for an octant that isn't there...
            Guard.IsNotNull(node);

            return LoadVisualizationPoint(node);
        }

        private MemoryOwner<VisualizationPoint> LoadVisualizationPoint(PotreeNode node)
        {
            Guard.IsLessThanOrEqualTo(node.NumPoints, int.MaxValue);
            //if (HandleExtraBytes != null)
            //    Guard.IsGreaterThan(OffsetToExtraBytes, 0);
            Guard.IsNotNull(PotreeData);

            var pointArray = ReadRawNodeData(node);

            var returnMemory = MemoryOwner<VisualizationPoint>.Allocate((int)node.NumPoints);

            var pointCount = 0;

            for (var i = 0; i < pointArray.Length; i += PotreeData.Metadata.PointSize)
            {
                var posSlice = new Span<byte>(pointArray).Slice(i + offsetPosition, Marshal.SizeOf<int>() * 3);
                var pos = MemoryMarshal.Cast<byte, int>(posSlice);

                double x = pos[0] * PotreeData.Metadata.Scale.x;
                double y = pos[1] * PotreeData.Metadata.Scale.y;
                double z = pos[2] * PotreeData.Metadata.Scale.z;

                float3 position = new((float)x, (float)y, (float)z);
                position = (float4x4)Potree2Consts.YZflip * position;

                var posSpan = MemoryMarshal.Cast<float, byte>(position.ToArray());

                Span<byte> colorSlice;
                Span<ushort> rgb;
                float4 color;
                if (offsetColor != -1)
                {
                    colorSlice = new Span<byte>(pointArray).Slice(i + offsetColor, Marshal.SizeOf<ushort>() * 3);
                    rgb = MemoryMarshal.Cast<byte, ushort>(colorSlice);

                    color = float4.Zero;

                    color.r = ((byte)(rgb[0] > 255 ? rgb[0] / 256 : rgb[0]));
                    color.g = ((byte)(rgb[1] > 255 ? rgb[1] / 256 : rgb[1]));
                    color.b = ((byte)(rgb[2] > 255 ? rgb[2] / 256 : rgb[2]));
                    color.a = 1;
                }
                else if (offsetIntensity != -1)
                {
                    var attrib = PotreeData.Metadata.Attributes["intensity"];
                    colorSlice = new Span<byte>(pointArray).Slice(i + offsetIntensity, Marshal.SizeOf<ushort>());
                    rgb = MemoryMarshal.Cast<byte, ushort>(colorSlice);
                    color = float4.Zero;

                    color.r = (float)((rgb[0] - attrib.MinList[0]) / (attrib.MaxList[0] - attrib.MinList[0]) * 1f);
                    color.g = color.r;
                    color.b = color.r;
                    color.a = 1;
                }
                else
                {
                    color = float4.UnitW;
                }

                var colorSpan = MemoryMarshal.Cast<float, byte>(color.ToArray());

                uint flags = 0;
                Span<byte> extraBytesSpan = null;
                if (PotreeData.Metadata.OffsetToExtraBytes != -1 && PotreeData.Metadata.OffsetToExtraBytes != 0)
                {
                    var extraByteSize = PotreeData.Metadata.PointSize - PotreeData.Metadata.OffsetToExtraBytes;
                    extraBytesSpan = pointArray.AsSpan().Slice(i + PotreeData.Metadata.OffsetToExtraBytes, extraByteSize);
                }
                if (HandleReadExtraBytes != null)
                {
                    try
                    {
                        flags = HandleReadExtraBytes(extraBytesSpan);
                    }
                    catch (Exception e)
                    {
                        OnPointCloudReadError?.Invoke(this, new ErrorEventArgs(e));
                    }
                }

                var flagsSpan = MemoryMarshal.Cast<uint, byte>(new uint[] { flags });

                var currentMemoryPt = MemoryMarshal.Cast<VisualizationPoint, byte>(returnMemory.Span.Slice(pointCount, 1));
                posSpan.CopyTo(currentMemoryPt[..]);
                colorSpan.CopyTo(currentMemoryPt[posSpan.Length..]);
                flagsSpan.CopyTo(currentMemoryPt.Slice(posSpan.Length + colorSpan.Length, Marshal.SizeOf<uint>()));

                pointCount++;
            }

            return returnMemory;
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

            PotreeData.Metadata.PrincipalAxisRotation = CalculatePrincipalAxis(PotreeData);

            return PotreeData;
        }



        /// <summary>
        /// Calculate the principal axis rotation from given data
        ///    - Load root node
        ///    - Convert to covariance matrix
        ///    - Calculate principal axis
        /// </summary>
        /// <param name="data">The potree data</param>
        /// <returns></returns>
        private float4x4 CalculatePrincipalAxis(PotreeData data)
        {
            using var allPoints = LoadVisualizationPoint(data.Hierarchy.Root);
            using var positions = MemoryOwner<float3>.Allocate(allPoints.Length);

            var allPtsBytes = allPoints.Span.AsBytes();

            // convert all points to byte, slice the position value (stride/offset) and copy to position array
            for (var i = 0; i < allPoints.Length; i++)
            {
                positions.Span[i] = allPoints.Span[i].Position;
            }

            // dangerous, undefined behavior -> do not use the array values after this method
            // this is irrelevant, as we use only a local variable
            try
            {
                var eigen = new Eigen(positions.DangerousGetArray().Array);
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

            // adapt the global AABB after conversion, this works with the current LAS writer
            Metadata.BoundingBox.MinList = new List<double>(3) { Hierarchy.Root.Aabb.min.x + Metadata.Offset.x, Hierarchy.Root.Aabb.min.z + Metadata.Offset.z, Hierarchy.Root.Aabb.min.y + Metadata.Offset.y };
            Metadata.BoundingBox.MaxList = new List<double>(3) { Hierarchy.Root.Aabb.max.x + Metadata.Offset.x, Hierarchy.Root.Aabb.max.z + Metadata.Offset.z, Hierarchy.Root.Aabb.max.y + Metadata.Offset.y };

            return (Metadata, Hierarchy);
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

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return serializer.Deserialize(reader, typeof(PotreeSettingsHierarchy));
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
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
    }
}