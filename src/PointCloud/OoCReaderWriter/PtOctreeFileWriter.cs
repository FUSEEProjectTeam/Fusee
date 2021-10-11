using Fusee.Base.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using Fusee.Structures;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fusee.PointCloud.OoCReaderWriter
{
    /// <summary>
    /// Contains methods to write a converted Point Cloud to the hard drive.
    /// Files that will be generated:
    /// .hierarchy: The binary encoded octree hierarchy. Contains information about the octree and the point type of the point cloud.
    /// meta.json: Contains the meta data of the point cloud
    /// .node: contains information about an octant and its payload.
    /// </summary>
    /// <typeparam name="TPoint">The type of the point cloud points."/></typeparam>
    public class PtOctreeFileWriter<TPoint>
    {
        private readonly string _fileFolderPath;
        private readonly Dictionary<Guid, FileStream> _fileStreams = new();

        /// <summary>
        /// Creates a new instance of type PtOctantFileWriter.
        /// </summary>
        /// <param name="pathToNodeFileFolder">The path the files are written to.</param>
        public PtOctreeFileWriter(string pathToNodeFileFolder)
        {
            _fileFolderPath = pathToNodeFileFolder;

            if (!Directory.Exists(_fileFolderPath)) Directory.CreateDirectory(_fileFolderPath);
        }

        /// <summary>
        /// Creates all files (meta.json, .hierarchy and .node).
        /// </summary>
        /// <param name="octree">The source octree.</param>
        /// <param name="ptAccessor">point accessor to get the actual point information.</param>
        public void WriteCompleteData(PtOctree<TPoint> octree, PointAccessor<TPoint> ptAccessor)
        {
            var watch = new Stopwatch();
            watch.Restart();

            WriteMeta(octree, ptAccessor);
            Diagnostics.Debug("-------------- Write meta file: " + watch.ElapsedMilliseconds + "ms.");

            watch.Restart();
            WriteHierarchy(octree);
            Diagnostics.Debug("-------------- Write hierarchy file: " + watch.ElapsedMilliseconds + "ms.");

            watch.Restart();

            var nodesToWrite = new List<PtOctantWrite<TPoint>>();

            octree.Traverse((OctantD<TPoint> node) =>
            {
                var ptNode = (PtOctantWrite<TPoint>)node;
                nodesToWrite.Add(ptNode);
                _fileStreams.Add(ptNode.Guid, File.Create(GetPathToFile(ptNode)));
            });
            Diagnostics.Debug("-------------- Traverse tree: " + watch.ElapsedMilliseconds + "ms.");

            watch.Restart();
            Parallel.ForEach(nodesToWrite, new ParallelOptions { MaxDegreeOfParallelism = nodesToWrite.Count / 3 }, (node) =>
            {
                WriteNode(octree.PtAccessor, node);
            });

            Diagnostics.Debug("-------------- Write node files: " + watch.ElapsedMilliseconds + "ms.");
        }

        /// <summary>
        /// Writes the binary encoded octree hierarchy into a file.
        /// </summary>
        /// <param name="octree">The source octree.</param>
        public void WriteHierarchy(PtOctree<TPoint> octree)
        {
            using BinaryWriter bw = new(File.Open(_fileFolderPath + "\\octree.hierarchy", FileMode.OpenOrCreate));
            octree.Traverse((OctantD<TPoint> node) =>
            {
                // write loadable properties (in which file the node's content - i.e. points - are stored)
                bw.Write(((PtOctantWrite<TPoint>)node).Guid.ToByteArray()); // 16 bytes
                bw.Write(node.Level);
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

        /// <summary>
        /// Writes the meta data into a json file. Contains information about the octree and the point type of the point cloud.
        /// </summary>
        /// <param name="octree">The source octree.</param>
        /// <param name="ptAccessor">Point accessor to get the actual point information.</param>
        public void WriteMeta(PtOctree<TPoint> octree, PointAccessor<TPoint> ptAccessor)
        {
            var rootCenter = octree.Root.Center;
            var rootSize = octree.Root.Size;

            var ptOctant = (PtOctantWrite<TPoint>)octree.Root;

            var spacing = ptOctant.Resolution;

            var jsonObj = new JObject(
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

            //Add JProperty for "pointAccessorBools" that contains all bools from the point accessor that are set to true.
            var ptAccessorObj = new JObject();

            foreach (var propertyName in ptAccessor.GetPointType())
            {
                ptAccessorObj.Add(propertyName, true);
            }
            var ptAccessorBools = new JProperty("ptAccessorBools", ptAccessorObj);
            jsonObj.Add(ptAccessorBools);

            //Add JPorperty for "pointType"
            var pointType = typeof(TPoint);
            var ptType = new JProperty("pointType", pointType.Name);
            jsonObj.Add(ptType);

            //Write file
            using StreamWriter file = File.CreateText(_fileFolderPath + "/meta.json");
            file.Write(jsonObj.ToString());
        }

        /// <summary>
        /// Creates a binary encoded .node file. This file contains information about an octant and its payload.
        /// </summary>
        /// <param name="ptAccessor">Point accessor to get the actual point information.</param>
        /// <param name="node">The source octant.</param>
        public void WriteNode(PointAccessor<TPoint> ptAccessor, PtOctantWrite<TPoint> node)
        {
            var points = GetPointsFromGrid(node).ToList();

            if (node.IsLeaf)
                points.AddRange(node.Payload);

            if (points.Count == 0)
                return;

            var stream = _fileStreams[node.Guid];

            using (stream)
            {
                var writer = new BinaryWriter(stream);

                // write point count
                writer.Write(points.Count);

                // write length of one point
                var firstPt = points[0];
                var length = ptAccessor.GetRawPoint(ref firstPt).Length;
                writer.Write(length);

                //List<byte> ptBytes = new List<byte>(length * points.Count);
                byte[] ptBytes = new byte[length * points.Count];
                for (int i = 0; i < points.Count; i++)
                {
                    TPoint point = points[i];
                    var pt = point;
                    var rawPt = ptAccessor.GetRawPoint(ref pt);
                    for (var j = 0; j < rawPt.Length; j++)
                        ptBytes[i * length + j] = rawPt[j];
                }

                writer.Write(ptBytes);
            }
        }

        #region Write .node files

        private IEnumerable<TPoint> GetPointsFromGrid(PtOctantWrite<TPoint> node)
        {
            foreach (var cell in node.Grid.GridCellsDict)
            {
                if (cell.Value != null)
                    yield return cell.Value.Payload;
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
    }
}