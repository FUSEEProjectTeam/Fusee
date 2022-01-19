using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using Fusee.PointCloud.PotreeReader.V2.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.PointCloud.PotreeReader.V2
{
    public class ReadPotree2Data
    {
        public static PotreeData Instance;
        private static BinaryReader binaryReader;

        /// <summary>
        /// Reads the meta.json and .hierarchy files and returns an octree.
        /// </summary>
        /// <param name="ptAccessor">Point accessor to get the actual point information.</param>
        /// <param name="fileFolderPath">Path to the folder the point cloud is saved</param>
        /// <returns></returns>
        public static PtOctreeRead<TPoint> GetOctree<TPoint>(IPointAccessor ptAccessor, string fileFolderPath) where TPoint : new()
        {
            var metadataFilePath = Path.Combine(fileFolderPath, Constants.MetadataFileName);
            var octreeFilePath = Path.Combine(fileFolderPath, Constants.OctreeFileName);

            binaryReader = new BinaryReader(File.OpenRead(octreeFilePath));

            Instance = new PotreeData();
            Instance.Metadata = JsonConvert.DeserializeObject<PotreeMetadata>(File.ReadAllText(metadataFilePath));
            Instance.Hierarchy = LoadHierarchy(fileFolderPath);

            int pointSize = 0;

            if (Instance.Metadata != null)
            {
                foreach (var metaAttributeItem in Instance.Metadata.Attributes)
                {
                    pointSize += metaAttributeItem.Size;
                }

                Instance.Metadata.PointSize = pointSize;
            }

            //TODO: ???
            var center = new double3(0, 0, 0);
            var size = 15;
            var maxLvl = Instance.Metadata.Hierarchy.Depth;

            var root = new PtOctantRead<TPoint>(center, size, "r");
            var octree = new PtOctreeRead<TPoint>(root, ptAccessor)
            {
                MaxLevel = maxLvl
            };

            return octree;
        }

        private static PotreeHierarchy LoadHierarchy(string fileFolderPath)
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

        public static PotreeNode FindNode(string id)
        {
            return Instance.Hierarchy.Nodes.Find(n => n.Name == id);
        }

        public static async Task<TPoint[]> LoadPointsForNodeAsync<TPoint>(string guid, IPointAccessor pointAccessor, PtOctantRead<TPoint> octant) where TPoint : new()
        {
            return await Task.Run(() => { return LoadNodeData<TPoint>(guid, pointAccessor, octant); });
        }

        public static TPoint[] LoadNodeData<TPoint>(string id, IPointAccessor pointAccessor, PtOctantRead<TPoint> octant) where TPoint : new()
        { 

            var node = FindNode(id);

            //var points = new Position_double__Color_float__Label_byte[node.NumPoints];
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

                        double3 position = new double3((binaryReader.ReadInt32() * Instance.Metadata.Scale.x) + Instance.Metadata.Offset.x,
                                                  (binaryReader.ReadInt32() * Instance.Metadata.Scale.y) + Instance.Metadata.Offset.y,
                                                  (binaryReader.ReadInt32() * Instance.Metadata.Scale.z) + Instance.Metadata.Offset.z);

                        ((PointAccessor<TPoint>)pointAccessor).SetPositionFloat3_64(ref points[i], position);

                        //points[i].Position.x = (binaryReader.ReadInt32() * (float)Instance.Metadata.Scale.x) + (float)Instance.Metadata.Offset.x;
                        //points[i].Position.y = (binaryReader.ReadInt32() * (float)Instance.Metadata.Scale.y) + (float)Instance.Metadata.Offset.y;
                        //points[i].Position.z = (binaryReader.ReadInt32() * (float)Instance.Metadata.Scale.z) + (float)Instance.Metadata.Offset.z;

                        // In js they subtract the min offset for every point,I guess that is just moving the pointcloud to the coordinate origin.
                        // We should do this in usercode
                    }
                }
                else if (metaitem.Name.Contains("rgb"))
                {
                    for (int i = 0; i < node.NumPoints; i++)
                    {
                        binaryReader.BaseStream.Position = node.ByteOffset + attributeOffset + i * Instance.Metadata.PointSize;

                        UInt16 r = binaryReader.ReadUInt16();
                        UInt16 g = binaryReader.ReadUInt16();
                        UInt16 b = binaryReader.ReadUInt16();

                        float3 color = float3.Zero;

                        color.r = ((byte)(r > 255 ? r / 256 : r))/* / 255f*/;
                        color.g = ((byte)(g > 255 ? g / 256 : g))/* / 255f*/;
                        color.b = ((byte)(b > 255 ? b / 256 : b))/* / 255f*/;

                        ((PointAccessor<TPoint>)pointAccessor).SetColorFloat3_32(ref points[i], color);
                    }
                }
                else if (metaitem.Name.Equals("classification"))
                {
                    for (int i = 0; i < node.NumPoints; i++)
                    {
                        binaryReader.BaseStream.Position = node.ByteOffset + attributeOffset + i + Instance.Metadata.PointSize;

                        byte label = (byte)binaryReader.ReadSByte();

                        ((PointAccessor<TPoint>)pointAccessor).SetLabelUInt_8(ref points[i], label);

                        //points[i].Label = (byte)binaryReader.ReadSByte();
                    }
                }

                attributeOffset += metaitem.Size;
            }

            octant.NumberOfPointsInNode = (int)node.NumPoints;

            node.IsLoaded = true;

            return (TPoint[]) points;
        }
    }
}
