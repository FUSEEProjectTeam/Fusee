using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
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
        public static List<GpuMesh> GetMeshsFromLasFile<TPoint>(PointAccessor<TPoint> ptAccessor, PointType ptType, string pathToFile, out AABBf box, CreateGpuMesh createGpuMesh, bool doExchangeYZ = false) where TPoint : new()
        {
            var reader = new LasPointReader(pathToFile);
            var pointCnt = ((LasMetaInfo)reader.MetaInfo).PointCnt;
            int maxVertCount = ushort.MaxValue - 1;
            var noOfMeshes = (int)System.Math.Ceiling((float)pointCnt / maxVertCount);
            TPoint[] points = new TPoint[maxVertCount];
            var meshCnt = 0;
            var meshes = new List<GpuMesh>();
            box = new();

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
                GpuMesh mesh = ptType switch
                {
                    PointType.Pos64 => MeshFromPointCloudPoints.GetMeshPos64(ptAccessor, points, doExchangeYZ, float3.Zero, createGpuMesh),
                    PointType.Pos64Col32IShort => MeshFromPointCloudPoints.GetMeshPos64Col32IShort(ptAccessor, points, doExchangeYZ, float3.Zero, createGpuMesh),
                    PointType.Pos64IShort => MeshFromPointCloudPoints.GetMeshPos64IShort(ptAccessor, points, doExchangeYZ, float3.Zero, createGpuMesh),
                    PointType.Pos64Col32 => MeshFromPointCloudPoints.GetMeshPos64Col32(ptAccessor, points, doExchangeYZ, float3.Zero, createGpuMesh),
                    PointType.Pos64Label8 => MeshFromPointCloudPoints.GetMeshPos64Label8(ptAccessor, points, doExchangeYZ, float3.Zero, createGpuMesh),
                    PointType.Pos64Nor32Col32IShort => MeshFromPointCloudPoints.GetMeshPos64Nor32Col32IShort(ptAccessor, points, doExchangeYZ, float3.Zero, createGpuMesh),
                    PointType.Pos64Nor32IShort => MeshFromPointCloudPoints.GetMeshPos64Nor32IShort(ptAccessor, points, doExchangeYZ, float3.Zero, createGpuMesh),
                    PointType.Pos64Nor32Col32 => MeshFromPointCloudPoints.GetMeshPos64Nor32Col32(ptAccessor, points, doExchangeYZ, float3.Zero, createGpuMesh),
                    _ => throw new ArgumentOutOfRangeException($"Invalid PointType {ptType}"),
                };
                if (i == 0)
                    box = mesh.BoundingBox;
                else
                    box |= mesh.BoundingBox;

                meshes.Add(mesh);
                meshCnt++;
            }
            reader.Dispose();
            return meshes;
        }
    }
}