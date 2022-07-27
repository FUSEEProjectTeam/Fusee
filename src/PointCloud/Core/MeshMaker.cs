using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.PointCloud.Common.Accessors;
using Fusee.PointCloud.Core.Accessors;
using System;
using System.Collections.Generic;

namespace Fusee.PointCloud.Core
{
    /// <summary>
    /// Static class that provides generic methods that take point cloud points and return <see cref="GpuMesh"/>s.
    /// </summary>
    public static class MeshMaker
    {
        /// <summary>
        /// Generic method that creates meshes with 65k points maximum.
        /// </summary>
        /// <param name="pointAccessor">The point accessor allows access to the point data without casting to a explicit point type."/></param>
        /// <param name="points">The generic point cloud points.</param>
        /// <param name="createGpuDataHandler">The method that defines how to create a GpuMesh from the point cloud points.</param>
        /// <returns></returns>
        public static IEnumerable<TGpuData> CreateMeshes<TGpuData, TPoint>(PointAccessor<TPoint> pointAccessor, TPoint[] points, CreateGpuData<TGpuData, TPoint> createGpuDataHandler)
        {
            List<TGpuData> meshes;

            var ptCnt = points.Length;
            int maxVertCount = ushort.MaxValue - 1;
            var noOfMeshes = (int)System.Math.Ceiling((float)ptCnt / maxVertCount);
            meshes = new(noOfMeshes);
            int meshCnt = 0;
            for (int i = 0; i < ptCnt; i += maxVertCount)
            {
                int numberOfPointsInMesh;
                if (noOfMeshes == 1)
                    numberOfPointsInMesh = ptCnt;
                else if (noOfMeshes == meshCnt + 1)
                    numberOfPointsInMesh = (ptCnt - maxVertCount * meshCnt);
                else
                    numberOfPointsInMesh = maxVertCount;

                TPoint[] pointsPerMesh;
                if (ptCnt > maxVertCount)
                {
                    pointsPerMesh = new TPoint[numberOfPointsInMesh];
                    Array.Copy(points, i, pointsPerMesh, 0, numberOfPointsInMesh);
                }
                else
                {
                    pointsPerMesh = points;
                }

                meshes.Add(createGpuDataHandler(pointAccessor, pointsPerMesh));
                meshCnt++;
            }
            return meshes;
        }

        /// <summary>
        /// Returns the instance Data for a given point type by using the provided delegate.
        /// </summary>
        /// <typeparam name="TGpuData">Can be of type <see cref="GpuMesh"/> or <see cref="InstanceData"/>. The latter is used when rendering instanced.</typeparam>
        /// <typeparam name="TPoint">The generic point type.</typeparam>
        /// <param name="pointAccessor">The point accessor allows access to the point data without casting to a explicit point type."/></param>
        /// <param name="points">The generic point cloud points.</param>
        /// <param name="createGpuDataHandler">The method that defines how to create a InstanceData from the point cloud points.</param>
        /// <returns></returns>
        public static IEnumerable<TGpuData> CreateInstanceData<TGpuData, TPoint>(PointAccessor<TPoint> pointAccessor, TPoint[] points, CreateGpuData<TGpuData, TPoint> createGpuDataHandler)
        {
            return new List<TGpuData>
            {
                createGpuDataHandler(pointAccessor, points)
            };
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="PosD3"/>.
        /// </summary>
        /// /// <param name="pointAccessor">The point accessor allows access to the point data without casting to explicit a explicit point type."/></param>
        /// <param name="points">The lists of "raw" points.</param>
        public static GpuMesh CreateMeshPosD3<TPoint>(PointAccessor<TPoint> pointAccessor, TPoint[] points)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;
            var firstPos = (float3)pointAccessor.GetPositionFloat3_64(ref points[0]);
            var vertices = new float3[numberOfPointsInMesh];
            var triangles = new ushort[numberOfPointsInMesh];
            var boundingBox = new AABBf(firstPos, firstPos);

            for (int i = 0; i < points.Length; i++)
            {
                var pos = (float3)pointAccessor.GetPositionFloat3_64(ref points[i]);

                vertices[i] = pos;
                boundingBox |= pos;
                triangles[i] = (ushort)i;
            }
            var mesh = ModuleExtensionPoint.CreateGpuMesh(PrimitiveType.Points, vertices, triangles);
            mesh.BoundingBox = boundingBox;
            return mesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="PosD3LblB"/>.
        /// </summary>
        /// <param name="pointAccessor">The point accessor allows access to the point data without casting to explicit a explicit point type."/></param>
        /// <param name="points">The lists of "raw" points.</param>
        public static GpuMesh CreateMeshPosD3ColF3LblB<TPoint>(PointAccessor<TPoint> pointAccessor, TPoint[] points)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var firstPos = (float3)pointAccessor.GetPositionFloat3_64(ref points[0]);
            var vertices = new float3[numberOfPointsInMesh];
            var triangles = new ushort[numberOfPointsInMesh];
            var colors = new uint[numberOfPointsInMesh];
            var boundingBox = new AABBf(firstPos, firstPos);

            for (int i = 0; i < points.Length; i++)
            {
                var pos = (float3)pointAccessor.GetPositionFloat3_64(ref points[i]);

                vertices[i] = pos;
                boundingBox |= vertices[i];

                triangles[i] = (ushort)i;
                var col = pointAccessor.GetColorFloat3_32(ref points[i]);//points[i].Color;
                colors[i] = ColorToUInt((int)col.r, (int)col.g, (int)col.b, 255);

                //TODO: add labels correctly
                var label = pointAccessor.GetLabelUInt_8(ref points[i]);//points[i].Label;
            }
            var mesh = ModuleExtensionPoint.CreateGpuMesh(PrimitiveType.Points, vertices, triangles, null, colors);
            mesh.BoundingBox = boundingBox;
            return mesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="PosD3LblB"/>.
        /// </summary>
        /// <param name="pointAccessor">The point accessor allows access to the point data without casting to explicit a explicit point type."/></param>
        /// <param name="points">The lists of "raw" points.</param>
        public static InstanceData CreateInstanceDataPosD3ColF3LblB<TPoint>(PointAccessor<TPoint> pointAccessor, TPoint[] points)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var firstPos = (float3)pointAccessor.GetPositionFloat3_64(ref points[0]);
            var vertices = new float3[numberOfPointsInMesh];
            var triangles = new ushort[numberOfPointsInMesh];
            var colors = new float4[numberOfPointsInMesh];
            var boundingBox = new AABBf(firstPos, firstPos);

            for (int i = 0; i < points.Length; i++)
            {
                var pos = (float3)pointAccessor.GetPositionFloat3_64(ref points[i]);

                vertices[i] = pos;
                boundingBox |= vertices[i];

                triangles[i] = (ushort)i;
                colors[i] = new float4(pointAccessor.GetColorFloat3_32(ref points[i]) / 256, 1.0f);

                //TODO: add labels correctly
                var label = pointAccessor.GetLabelUInt_8(ref points[i]);//points[i].Label;
            }

            return new InstanceData(points.Length, vertices, null, null, colors);
        }

        #region Color Conversion
        /// <summary>
        /// Converts a color, saved as four int values (0 to 255), to uint.
        /// </summary>
        /// <param name="r">The red value.</param>
        /// <param name="g">The green value.</param>
        /// <param name="b">The blue value.</param>
        /// <param name="a">The alpha value.</param>
        /// <returns></returns>
        private static uint ColorToUInt(int r, int g, int b, int a)
        {
            return (uint)((b << 16) | (g << 8) | (r << 0) | (a << 24));
        }

        /// <summary>
        /// Converts a color, saved as an uint, to float4.
        /// </summary>
        /// <param name="col">The color.</param>
        private static float4 UintToColor(uint col)
        {
            float4 c = new();
            c.b = (byte)((col) & 0xFF);
            c.g = (byte)((col >> 8) & 0xFF);
            c.r = (byte)((col >> 16) & 0xFF);
            c.a = (byte)((col >> 24) & 0xFF);

            return c;
        }

        /// <summary>
        /// Converts a color, saved as an uint, to float3.
        /// </summary>
        /// <param name="col">The color.</param>
        private static uint ColorToUint(float3 col)
        {
            uint packedR = (uint)(col.r * 255);
            uint packedG = (uint)(col.g * 255) << 8;
            uint packedB = (uint)(col.b * 255) << 16;

            return packedR + packedG + packedB;
        }

        /// <summary>
        /// Converts a color, saved as float4, to uint.
        /// </summary>
        /// <param name="col">The color.</param>
        private static uint ColorToUint(float4 col)
        {
            uint packedR = (uint)(col.r * 255);
            uint packedG = (uint)(col.g * 255) << 8;
            uint packedB = (uint)(col.b * 255) << 16;
            uint packedA = (uint)(col.a * 255) << 24;

            return packedR + packedG + packedB + packedA;
        }

        #endregion
    }
}