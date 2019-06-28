using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Pointcloud.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fusee.Pointcloud.OoCFileReaderWriter
{
    public class GridPtAccessor<TPoint> : PointAccessor<TPoint>
    {
        public bool HasGridIdx = true;

        public virtual ref int3 GetGridIdx(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetPositionFloat32");
        }

        public virtual void SetGridIdx(ref TPoint point, int3 val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetPositionFloat32");
        }

        public List<string> GetPointType()
        {
            return GetType().GetProperties().Where(p => p.PropertyType == typeof(bool) && (bool)p.GetValue(this, null)).Select(p => p.Name).ToList();
        }
    }

    public class PtOctreeFileWriter<TPoint>
    {
        private string _fileFolderPath;

        public PtOctreeFileWriter(string pathToNodeFileFolder)
        {
            _fileFolderPath = pathToNodeFileFolder;
        }

        public void WriteCompleteData(PtOctree<TPoint> octree, GridPtAccessor<TPoint> ptAccessor)
        {
            WriteMeta(octree, ptAccessor);
            WriteHierarchy(octree);
            octree.Traverse((PtOctantWrite<TPoint> node) =>
            {
                WriteNode(octree.PtAccessor, node);
            });
        }

        #region Write .node files

        private IEnumerable<TPoint> GetPointsFromGrid(PtOctantWrite<TPoint> node)
        {
            foreach (var cell in node.Grid.GridCells)
            {
                if (cell != null)
                    yield return cell.Occupant;
            }
        }

        private string GetPathToFile(PtOctantWrite<TPoint> node)
        {
            var directoryInfo = Directory.CreateDirectory(_fileFolderPath + "\\Octants");
            return directoryInfo.FullName + "\\" + GetFilename(node);
        }

        private string GetFilename(PtOctantWrite<TPoint> node)
        {
            var fileName = node.Guid.ToString("N");

            fileName += ".node";
            return fileName;
        }

        #endregion

        /// <summary>
        /// Writes the contents, i.e. points, into a single file of the nodes folder.
        /// </summary>
        public void WriteNode(GridPtAccessor<TPoint> ptAccessor, PtOctantWrite<TPoint> node)
        {
            var points = GetPointsFromGrid(node).ToList();

            if (node.IsLeaf)
                points.AddRange(node.Payload);

            if (points.Count == 0)
                return;

            var stream = File.Open(GetPathToFile(node), FileMode.OpenOrCreate);

            var writer = new BinaryWriter(stream);

            // first, write point count
            writer.Write(node.Payload.Count);

            foreach (var point in points)
            {
                var pt = point;
                var rawPt = ptAccessor.GetRawPoint(ref pt);
                writer.Write(rawPt);
            }
            stream.Dispose();
        }

        /// <summary>
        /// Writes the hierarchy of the octree into a separate file.
        /// </summary>
        public void WriteHierarchy(PtOctree<TPoint> octree)
        {
            using (BinaryWriter bw = new BinaryWriter(File.Open(_fileFolderPath + "\\octree.hierarchy", FileMode.OpenOrCreate)))
            {
                octree.Traverse((PtOctantWrite<TPoint> node) =>
                {
                    // write loadable properties (in which file the node's content - i.e. points - are stored)
                    bw.Write(node.Guid.ToByteArray()); // 16 bytes
                    bw.Write(node.Level);
                    bw.Write(node.Resolution);
                    bw.Write(node.IsLeaf);
                    //bw.Write(node.StreamPosition);

                    // write child indices (1 byte). For example: Octant has child 0 and 1: 2^0 + 2^1 = 3                   
                    byte childIndices = 0;

                    int exp = 0;
                    foreach (var childNode in node.Children)
                    {
                        if (childNode != null)
                            childIndices += (byte)System.Math.Pow(2, exp);

                        exp++;
                    }

                    bw.Write(childIndices);
                });
            }
        }

        /// <summary>
        /// Writes some header information into a .json file.
        /// </summary>
        public void WriteMeta(PtOctree<TPoint> octree, GridPtAccessor<TPoint> ptAccessor)
        {
            var rootCenter = octree.Root.Center;
            var rootSize = octree.Root.Size;

            var ptOctant = (PtOctantWrite<TPoint>)octree.Root;

            var spacing = ptOctant.Resolution;

            var jsonObj = new JObject(
                //new JProperty("numberOfPoints", octree.PointCount),
                //new JProperty("boundingBox",
                //    new JObject(
                //        new JProperty("center", new JArray(center.X, center.Y, center.Z)),
                //        new JProperty("size", new JArray(length.X, length.Y, length.Z))
                //    )
                //),

                new JProperty("octree",
                    new JObject(
                        new JProperty("maxLevel", octree.MaxLevel),
                        new JProperty("maxNoOfPointsInBucket", octree.MaxNoOfPointsInBucket),
                        new JProperty("spacingFactor", spacing),
                        new JProperty("rootNode",
                            new JObject(
                                new JProperty("center", new JArray(rootCenter.x, rootCenter.y, rootCenter.z)),
                                new JProperty("size", rootSize)
                            )
                        )
                    )
                )                
            );

            //Add JProperty for "pointType" that contains all bools from the point accessor that are set to true.
            var ptTypeObj = new JObject();
            foreach (var propertyName in ptAccessor.GetPointType())
            {
                ptTypeObj.Add(propertyName, true);
            }
            var ptType = new JProperty("pointType", ptTypeObj);
            jsonObj.Add(ptType);

            //Write file
            using (StreamWriter file = File.CreateText(_fileFolderPath + "/meta.json"))
            {
                file.Write(jsonObj.ToString());
            }
        }
    }

    public class PtOctreeFileReader<TPoint>
    {
        private string _fileFolderPath;

        public PtOctreeFileReader(string pathToNodeFileFolder)
        {
            _fileFolderPath = pathToNodeFileFolder;
        }

        public PtOctree<TPoint> GetOctree(GridPtAccessor<TPoint> ptAccessor)
        {
            var pathToMetaJson = _fileFolderPath + "\\meta.json";
            JObject jsonObj;

            using (StreamReader sr = new StreamReader(pathToMetaJson))
            {
                jsonObj = (JObject)JToken.ReadFrom(new JsonTextReader(sr));
            }

            var jsonCenter = (JArray)jsonObj["octree"]["rootNode"]["center"];
            var center = new double3((double)jsonCenter[0], (double)jsonCenter[1], (double)jsonCenter[2]);
            var jsonSize = (JValue)jsonObj["octree"]["rootNode"]["size"];
            var size = (double)jsonSize;
            var jsonNoOfPts = (JValue)jsonObj["octree"]["maxNoOfPointsInBucket"];
            var maxNoOfPointsInBucket = (int)jsonNoOfPts;
            var jsonMaxLvl = (JValue)jsonObj["octree"]["maxLevel"];
            var maxLvl = (int)jsonMaxLvl;

            //TODO: get infos from meta.json
            var root = new PtOctant<TPoint>(center, size);
            var octree = new PtOctree<TPoint>(root, ptAccessor, maxNoOfPointsInBucket)
            {
                MaxLevel = maxLvl
            };
            ReadHierarchy(octree);
            return octree;
        }

        /// <summary>
        /// Creates the octree hierarchy structure by reading in the octree.info file.
        /// </summary>
        private void ReadHierarchy(PtOctree<TPoint> octree)
        {
            var pathToHierarchy = _fileFolderPath + "\\octree.hierarchy";

            FileStream fileStream = File.Open(pathToHierarchy, FileMode.Open, FileAccess.Read);
            long sizeInBytes = fileStream.Length;

            using (BinaryReader br = new BinaryReader(fileStream))
            {
                ProcessNode((PtOctant<TPoint>)octree.Root, br);
            }

            fileStream.Dispose();
        }

        /// <summary>
        /// Processes the current node with the given set of childs as bit set.
        /// </summary>
        /// <param name="node">The current node to process.</param>
        /// <param name="binaryReader">The binary reader to read bytes from. A byte indicating which of the given node's childs exist.</param>
        private void ProcessNode(PtOctant<TPoint> node, BinaryReader binaryReader)
        {
            try
            {
                // loadable properties
                byte[] guidBytes = new byte[16];
                binaryReader.Read(guidBytes, 0, 16);
                node.Guid = new Guid(guidBytes);
                node.Level = binaryReader.ReadInt32();
                node.Resolution = binaryReader.ReadDouble();
                node.IsLeaf = binaryReader.ReadBoolean();
                //node.StreamPosition = binaryReader.ReadInt64();

                // create children
                byte children = binaryReader.ReadByte();

                for (byte index = 0; index < 8; index++)
                {
                    bool childExists = (children & (1 << index)) != 0;

                    if (childExists)
                    {
                        PtOctant<TPoint> child = node.CreateChild(index);
                        node.Children[index] = child;
                        ProcessNode(child, binaryReader);
                    }
                }
            }
            catch (EndOfStreamException e)
            {

            }
        }
    }
}
