using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Fusee.PointCloud.PotreeReader.V1
{
    /// <summary>
    /// Reads the point cloud files created by the OoCFileGeneration tool into a renderable Scene.
    /// </summary>
    public static class ReadPotreeData<TPoint> where TPoint : new()
    {
        /// <summary>
        /// Reads the meta.json and .hierarchy files and returns an octree.
        /// </summary>
        /// <param name="ptAccessor">Point accessor to get the actual point information.</param>
        /// <param name="fileFolderPath">Path to the folder the point cloud is saved</param>
        /// <returns></returns>
        public static PtOctreeRead<TPoint> GetOctree(IPointAccessor ptAccessor, string fileFolderPath)
        {
            var pathToMetaJson = fileFolderPath + "\\meta.json";
            JObject jsonObj;

            using (StreamReader sr = new(pathToMetaJson))
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

            var root = new PtOctantRead<TPoint>(center, size);
            var octree = new PtOctreeRead<TPoint>(root, ptAccessor, maxNoOfPointsInBucket)
            {
                MaxLevel = maxLvl
            };

            ReadHierarchy(octree, fileFolderPath);
            return octree;
        }

        /// <summary>
        /// Loads the point data from an octant file.
        /// </summary>
        /// <param name="fileFolderPath">Path to the file.</param>
        /// <param name="ptAccessor"></param>
        /// <param name="octant"></param>
        /// <returns>An array of TPoint points.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static TPoint[] LoadPointsForNode(string fileFolderPath, IPointAccessor ptAccessor, PtOctantRead<TPoint> octant)
        {
            var pathToFile = $"{fileFolderPath}/Octants/{octant.Guid:N}.node";

            if (!File.Exists(pathToFile))
                throw new ArgumentException($"File: { octant.Guid }.node does not exist!");

            using BinaryReader br = new(File.Open(pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read));
            // step to stream position
            //br.BaseStream.Position = node.StreamPosition;

            // read number of points
            var numberOfPoints = br.ReadInt32();
            var lengthOfPoint = br.ReadInt32();

            octant.NumberOfPointsInNode = numberOfPoints;
            TPoint[] points = new TPoint[numberOfPoints];

            for (var i = 0; i < numberOfPoints; i++)
            {
                var pt = new TPoint();
                var ptBytes = br.ReadBytes(lengthOfPoint);

                ((PointAccessor<TPoint>)ptAccessor).SetRawPoint(ref pt, ptBytes);

                points[i] = pt;
            }

            return points;
        }

        /// <summary>
        /// Loads the point data from an octant file.
        /// </summary>
        /// <param name="fileFolderPath">Path to the file.</param>
        /// <param name="ptAccessor"></param>
        /// <param name="octant"></param>
        /// <returns>An array of TPoint points.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static async Task<TPoint[]> LoadPointsForNodeAsync(string fileFolderPath, IPointAccessor ptAccessor, PtOctantRead<TPoint> octant)
        {
            return await Task.Run(() => { return LoadPointsForNode(fileFolderPath, ptAccessor, octant); });
        }

        /// <summary>
        /// Creates the octree hierarchy structure by reading in the .hierarchy file.
        /// </summary>
        private static void ReadHierarchy(PtOctreeRead<TPoint> octree, string fileFolderPath)
        {
            var pathToHierarchy = fileFolderPath + "\\octree.hierarchy";

            FileStream fileStream = File.Open(pathToHierarchy, FileMode.Open, FileAccess.Read);

            using (BinaryReader br = new(fileStream))
            {
                CreateNode((PtOctantRead<TPoint>)octree.Root, br);
            }

            fileStream.Dispose();
        }

        /// <summary>
        /// Processes the current node with the given set of children as bit set.
        /// </summary>
        /// <param name="node">The current node to process.</param>
        /// <param name="binaryReader">The binary reader to read bytes from. A byte indicating which of the given node's children exist.</param>
        private static void CreateNode(PtOctantRead<TPoint> node, BinaryReader binaryReader)
        {
            try
            {
                // loadable properties
                byte[] guidBytes = new byte[16];
                binaryReader.Read(guidBytes, 0, 16);
                node.Guid = new Guid(guidBytes);
                node.Level = binaryReader.ReadInt32();
                node.IsLeaf = binaryReader.ReadBoolean();

                // create children
                byte children = binaryReader.ReadByte();

                for (byte index = 0; index < 8; index++)
                {
                    bool childExists = (children & (1 << index)) != 0;

                    if (childExists)
                    {
                        PtOctantRead<TPoint> child = (PtOctantRead<TPoint>)node.CreateChild(index);
                        node.Children[index] = child;
                        CreateNode(child, binaryReader);
                    }
                }
            }
            catch (EndOfStreamException) { }
        }

    }
}