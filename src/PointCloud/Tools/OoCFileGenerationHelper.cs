using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.FileReader.LasReader;
using Fusee.PointCloud.OoCReaderWriter;
using System;
using System.Diagnostics;

namespace Fusee.PointCloud.Core
{
    /// <summary>
    /// Provides methods to create the ooc files according to the user-given point type.
    /// </summary>
    public static class OoCFileGenerationHelper
    {
        /// <summary>
        /// Converts the point cloud and saves the files (meta.json, .hierarchy and .node files).
        /// </summary>
        /// <param name="ptType">The type of the point cloud points (<see cref="PointType"/>).</param>
        /// <param name="pathToFile">Path of the original point cloud file.</param>
        /// <param name="pathToFolder">Path where the new files will be saved.</param>
        /// <param name="maxNoOfPointsInBucket">Number of points that a bucket/octant can hold. If additional point would fall into it they fall into the next level instead.</param>
        /// <param name="doExchangeXZ">Bool that determines if the y and z coordinates of the points should be exchanged.</param>
        public static void CreateFilesForPtType(PointType ptType, string pathToFile, string pathToFolder, int maxNoOfPointsInBucket, bool doExchangeXZ)
        {
            switch (ptType)
            {
                case PointType.Pos64:
                    {
                        var ptAcc = new Pos64_Accessor();
                        CreateFiles(ptAcc, pathToFile, pathToFolder, maxNoOfPointsInBucket, doExchangeXZ);
                        break;
                    }
                case PointType.Pos64Col32IShort:
                    {
                        var ptAcc = new Pos64Col32IShort_Accessor();
                        CreateFiles(ptAcc, pathToFile, pathToFolder, maxNoOfPointsInBucket, doExchangeXZ);
                        break;
                    }
                case PointType.Pos64IShort:
                    {
                        var ptAcc = new Pos64IShort_Accessor();
                        CreateFiles(ptAcc, pathToFile, pathToFolder, maxNoOfPointsInBucket, doExchangeXZ);
                        break;
                    }
                case PointType.Pos64Col32:
                    {
                        var ptAcc = new Pos64Col32_Accessor();
                        CreateFiles(ptAcc, pathToFile, pathToFolder, maxNoOfPointsInBucket, doExchangeXZ);
                        break;
                    }
                case PointType.Pos64Label8:
                    {
                        var ptAcc = new Pos64Label8_Accessor();
                        CreateFiles(ptAcc, pathToFile, pathToFolder, maxNoOfPointsInBucket, doExchangeXZ);
                        break;
                    }
                case PointType.Pos64Nor32Col32IShort:
                    {
                        var ptAcc = new Pos64Nor32Col32IShort_Accessor();
                        CreateFiles(ptAcc, pathToFile, pathToFolder, maxNoOfPointsInBucket, doExchangeXZ);
                        break;
                    }
                case PointType.Pos64Nor32IShort:
                    {
                        var ptAcc = new Pos64Nor32IShort_Accessor();
                        CreateFiles(ptAcc, pathToFile, pathToFolder, maxNoOfPointsInBucket, doExchangeXZ);
                        break;
                    }
                case PointType.Pos64Nor32Col32:
                    {
                        var ptAcc = new Pos64Nor32Col32_Accessor();
                        CreateFiles(ptAcc, pathToFile, pathToFolder, maxNoOfPointsInBucket, doExchangeXZ);
                        break;
                    }
                default:
                    break;
            }
        }

        private static void CreateFiles<TPoint>(PointAccessor<TPoint> ptAcc, string pathToFile, string pathToFolder, int maxNoOfPointsInBucket, bool doExchangeYZ) where TPoint : new()
        {
            var watch = new Stopwatch();
            watch.Restart();
            var points = FromLasToArray(ptAcc, pathToFile, doExchangeYZ, out var aabb);
            watch.Stop();
            Console.WriteLine("Get positions from accessor took: " + watch.ElapsedMilliseconds + "ms.");

            watch.Restart();

            var octree = new PtOctree<TPoint>(aabb, ptAcc, points, maxNoOfPointsInBucket);
            Console.WriteLine("Octree creation took: " + watch.ElapsedMilliseconds + "ms.");

            watch.Restart();
            var occFileWriter = new PtOctreeFileWriter<TPoint>(pathToFolder);
            occFileWriter.WriteCompleteData(octree, ptAcc);
            Console.WriteLine("Writing files took: " + watch.ElapsedMilliseconds + "ms.");
        }

        public static TPoint[] FromLasToArray<TPoint>(PointAccessor<TPoint> ptAcc, string pathToPc, bool doExchangeYZ, out AABBd aabb) where TPoint : new()
        {
            var reader = new LasPointReader(pathToPc);
            var metaInfo = (LasMetaInfo)reader.MetaInfo;
            var points = new TPoint[((int)metaInfo.PointCnt)];
            aabb = new AABBd();
            if (ptAcc.HasPositionFloat3_64)
            {
                for (var i = 0; i < points.Length; i++)
                {
                    var pt = points[i];
                    if (!reader.ReadNextPoint(ref pt, ptAcc)) break;
                    var posD = ptAcc.GetPositionFloat3_64(ref pt);
                    if (doExchangeYZ)
                        FuseePointCloudHelper.ExchangeYZ(ref posD);
                    if (i == 0)
                        aabb.min = aabb.max = posD;
                    aabb |= posD;
                    ptAcc.SetPositionFloat3_64(ref pt, posD);
                    points[i] = pt;
                }
            }
            else if (ptAcc.HasPositionFloat3_32)
            {
                for (var i = 0; i < points.Length; i++)
                {
                    var pt = points[i];
                    if (!reader.ReadNextPoint(ref pt, ptAcc)) break;
                    var posF = ptAcc.GetPositionFloat3_32(ref pt);
                    if (doExchangeYZ)
                        FuseePointCloudHelper.ExchangeYZ(ref posF);
                    var posD = new double3(posF.x, posF.y, posF.z);
                    if (i == 0)
                        aabb.min = aabb.max = posD;
                    aabb |= posD;
                    ptAcc.SetPositionFloat3_32(ref pt, posF);
                    points[i] = pt;
                }
            }
            else
            {
                throw new ArgumentException("Invalid Position type");
            }
            reader.Dispose();
            return points;
        }
    }
}