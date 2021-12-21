using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using Fusee.PointCloud.PointCloudFileReader.Las;
using System;
using System.Collections.Generic;

namespace Fusee.Examples.PointCloudLive.Core
{
    public static class LasToMesh
    {
        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64Col32"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="ptType">The <see cref="PointType"/>, used to get the mesh data from the raw points.</param>
        /// <param name="pathToFile">The path to the las file.</param>
        /// <param name="box">The <see cref="AABBf"/> of the point cloud.</param>
        /// <param name="doExchangeYZ"></param>
        public static List<GpuMesh> GetMeshsFromLasFile(PointType ptType, string pathToFile, out AABBf box, CreateGpuMesh createGpuMesh, bool doExchangeYZ = false)
        {
            var reader = new LasPointReader(pathToFile);
            var pointCnt = ((LasMetaInfo)reader.MetaInfo).PointCnt;
            int maxVertCount = ushort.MaxValue - 1;
            var noOfMeshes = (int)System.Math.Ceiling((float)pointCnt / maxVertCount);
            var meshCnt = 0;
            var meshes = new List<GpuMesh>();
            box = new();

            switch (ptType)
            {
                case PointType.Undefined:
                    throw new InvalidOperationException("Invalid or undefined Point Type!");
                case PointType.Pos64:
                    {
                        var ptAccessor = new Pos64Accessor();
                        Pos64[] points = new Pos64[maxVertCount];
                        for (int i = 0; i < pointCnt; i += maxVertCount)
                        {
                            int numberOfPointsInMesh;
                            if (noOfMeshes == 1)
                                numberOfPointsInMesh = points.Length;
                            else if (noOfMeshes == meshCnt + 1)
                                numberOfPointsInMesh = (int)(pointCnt - maxVertCount * meshCnt);
                            else
                                numberOfPointsInMesh = maxVertCount;
                            points = reader.ReadNPoints(numberOfPointsInMesh, ptAccessor);


                            GpuMesh mesh = MeshFromPointCloudPoints.GetMeshPos64(ptAccessor, points, doExchangeYZ, float3.Zero, createGpuMesh);
                            if (i == 0)
                                box = mesh.BoundingBox;
                            else
                                box |= mesh.BoundingBox;

                            meshes.Add(mesh);
                            meshCnt++;
                        }
                        break;
                    }
                case PointType.Pos64Col32IShort:
                    {
                        var ptAccessor = new Pos64Col32IShortAccessor();
                        Pos64Col32IShort[] points = new Pos64Col32IShort[maxVertCount];
                        for (int i = 0; i < pointCnt; i += maxVertCount)
                        {
                            int numberOfPointsInMesh;
                            if (noOfMeshes == 1)
                                numberOfPointsInMesh = points.Length;
                            else if (noOfMeshes == meshCnt + 1)
                                numberOfPointsInMesh = (int)(pointCnt - maxVertCount * meshCnt);
                            else
                                numberOfPointsInMesh = maxVertCount;
                            points = reader.ReadNPoints(numberOfPointsInMesh, ptAccessor);


                            GpuMesh mesh = MeshFromPointCloudPoints.GetMeshPos64Col32IShort(ptAccessor, points, doExchangeYZ, float3.Zero, createGpuMesh);
                            if (i == 0)
                                box = mesh.BoundingBox;
                            else
                                box |= mesh.BoundingBox;

                            meshes.Add(mesh);
                            meshCnt++;
                        }
                        break;
                    }
                case PointType.Pos64IShort:
                    {
                        var ptAccessor = new Pos64IShortAccessor();
                        Pos64IShort[] points = new Pos64IShort[maxVertCount];
                        for (int i = 0; i < pointCnt; i += maxVertCount)
                        {
                            int numberOfPointsInMesh;
                            if (noOfMeshes == 1)
                                numberOfPointsInMesh = points.Length;
                            else if (noOfMeshes == meshCnt + 1)
                                numberOfPointsInMesh = (int)(pointCnt - maxVertCount * meshCnt);
                            else
                                numberOfPointsInMesh = maxVertCount;
                            points = reader.ReadNPoints(numberOfPointsInMesh, ptAccessor);


                            GpuMesh mesh = MeshFromPointCloudPoints.GetMeshPos64IShort(ptAccessor, points, doExchangeYZ, float3.Zero, createGpuMesh);
                            if (i == 0)
                                box = mesh.BoundingBox;
                            else
                                box |= mesh.BoundingBox;

                            meshes.Add(mesh);
                            meshCnt++;
                        }
                        break;
                    }
                case PointType.Pos64Col32:
                    {
                        var ptAccessor = new Pos64Col32Accessor();
                        Pos64Col32[] points = new Pos64Col32[maxVertCount];
                        for (int i = 0; i < pointCnt; i += maxVertCount)
                        {
                            int numberOfPointsInMesh;
                            if (noOfMeshes == 1)
                                numberOfPointsInMesh = points.Length;
                            else if (noOfMeshes == meshCnt + 1)
                                numberOfPointsInMesh = (int)(pointCnt - maxVertCount * meshCnt);
                            else
                                numberOfPointsInMesh = maxVertCount;
                            points = reader.ReadNPoints(numberOfPointsInMesh, ptAccessor);


                            GpuMesh mesh = MeshFromPointCloudPoints.GetMeshPos64Col32(ptAccessor, points, doExchangeYZ, float3.Zero, createGpuMesh);
                            if (i == 0)
                                box = mesh.BoundingBox;
                            else
                                box |= mesh.BoundingBox;

                            meshes.Add(mesh);
                            meshCnt++;
                        }
                        break;
                    }
                case PointType.Pos64Label8:
                    {
                        var ptAccessor = new Pos64Label8Accessor();
                        Pos64Label8[] points = new Pos64Label8[maxVertCount];
                        for (int i = 0; i < pointCnt; i += maxVertCount)
                        {
                            int numberOfPointsInMesh;
                            if (noOfMeshes == 1)
                                numberOfPointsInMesh = points.Length;
                            else if (noOfMeshes == meshCnt + 1)
                                numberOfPointsInMesh = (int)(pointCnt - maxVertCount * meshCnt);
                            else
                                numberOfPointsInMesh = maxVertCount;
                            points = reader.ReadNPoints(numberOfPointsInMesh, ptAccessor);


                            GpuMesh mesh = MeshFromPointCloudPoints.GetMeshPos64Label8(ptAccessor, points, doExchangeYZ, float3.Zero, createGpuMesh);
                            if (i == 0)
                                box = mesh.BoundingBox;
                            else
                                box |= mesh.BoundingBox;

                            meshes.Add(mesh);
                            meshCnt++;
                        }
                        break;
                    }
                case PointType.Pos64Nor32Col32IShort:
                    {
                        var ptAccessor = new Pos64Nor32Col32IShortAccessor();
                        Pos64Nor32Col32IShort[] points = new Pos64Nor32Col32IShort[maxVertCount];
                        for (int i = 0; i < pointCnt; i += maxVertCount)
                        {
                            int numberOfPointsInMesh;
                            if (noOfMeshes == 1)
                                numberOfPointsInMesh = points.Length;
                            else if (noOfMeshes == meshCnt + 1)
                                numberOfPointsInMesh = (int)(pointCnt - maxVertCount * meshCnt);
                            else
                                numberOfPointsInMesh = maxVertCount;
                            points = reader.ReadNPoints(numberOfPointsInMesh, ptAccessor);


                            GpuMesh mesh = MeshFromPointCloudPoints.GetMeshPos64Nor32Col32IShort(ptAccessor, points, doExchangeYZ, float3.Zero, createGpuMesh);
                            if (i == 0)
                                box = mesh.BoundingBox;
                            else
                                box |= mesh.BoundingBox;

                            meshes.Add(mesh);
                            meshCnt++;
                        }
                        break;
                    }
                case PointType.Pos64Nor32IShort:
                    {
                        var ptAccessor = new Pos64Nor32IShortAccessor();
                        Pos64Nor32IShort[] points = new Pos64Nor32IShort[maxVertCount];
                        for (int i = 0; i < pointCnt; i += maxVertCount)
                        {
                            int numberOfPointsInMesh;
                            if (noOfMeshes == 1)
                                numberOfPointsInMesh = points.Length;
                            else if (noOfMeshes == meshCnt + 1)
                                numberOfPointsInMesh = (int)(pointCnt - maxVertCount * meshCnt);
                            else
                                numberOfPointsInMesh = maxVertCount;
                            points = reader.ReadNPoints(numberOfPointsInMesh, ptAccessor);


                            GpuMesh mesh = MeshFromPointCloudPoints.GetMeshPos64Nor32IShort(ptAccessor, points, doExchangeYZ, float3.Zero, createGpuMesh);
                            if (i == 0)
                                box = mesh.BoundingBox;
                            else
                                box |= mesh.BoundingBox;

                            meshes.Add(mesh);
                            meshCnt++;
                        }
                        break;
                    }
                case PointType.Pos64Nor32Col32:
                    {
                        var ptAccessor = new Pos64Nor32Col32Accessor();
                        Pos64Nor32Col32[] points = new Pos64Nor32Col32[maxVertCount];
                        for (int i = 0; i < pointCnt; i += maxVertCount)
                        {
                            int numberOfPointsInMesh;
                            if (noOfMeshes == 1)
                                numberOfPointsInMesh = points.Length;
                            else if (noOfMeshes == meshCnt + 1)
                                numberOfPointsInMesh = (int)(pointCnt - maxVertCount * meshCnt);
                            else
                                numberOfPointsInMesh = maxVertCount;
                            points = reader.ReadNPoints(numberOfPointsInMesh, ptAccessor);

                            GpuMesh mesh = MeshFromPointCloudPoints.GetMeshPos64Nor32Col32(ptAccessor, points, doExchangeYZ, float3.Zero, createGpuMesh);
                            if (i == 0)
                                box = mesh.BoundingBox;
                            else
                                box |= mesh.BoundingBox;

                            meshes.Add(mesh);
                            meshCnt++;
                        }
                        break;
                    }
            }
            reader.Dispose();
            return meshes;

        }
    }
}