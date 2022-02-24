using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.PointCloud.Common.Accessors;
using System;
using System.Collections.Generic;

namespace Fusee.PointCloud.Core
{
    public static class MeshMaker
    {
        /// <summary>
        /// Generic method that creates meshes with 65k points maximum.
        /// </summary>
        /// <param name="pointAccessor">The point accessor allows access to the point data without casting to explicit a explicit point type."/></param>
        /// <param name="points">The generic point cloud points.</param>
        /// <param name="createMeshHandler">The method that defines how to create a GpuMesh from the point cloud points.</param>
        /// <returns></returns>
        public static IEnumerable<GpuMesh> CreateMeshes<TPoint>(PointAccessor<TPoint> pointAccessor, TPoint[] points, CreateMesh<TPoint> createMeshHandler)
        {
            List<GpuMesh> meshes;

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
                var mesh = createMeshHandler(pointAccessor, pointsPerMesh, PointCloudImplementor.CreateGpuMesh);

                meshes.Add(mesh);
                meshCnt++;
            }
            return meshes;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="PosD3"/>.
        /// </summary>
        /// /// <param name="pointAccessor">The point accessor allows access to the point data without casting to explicit a explicit point type."/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="createMesh">Function for creating the <see cref="GpuMesh"/>.</param>
        public static GpuMesh CreateMeshPosD3<TPoint>(PointAccessor<TPoint> pointAccessor, TPoint[] points, CreateGpuMesh createMesh)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;
            var firstPos = (float3)pointAccessor.GetPositionFloat3_64(ref points[0]);//points[0].Position;
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
            var mesh = createMesh(PrimitiveType.Points, vertices, triangles);
            mesh.BoundingBox = boundingBox;
            return mesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="PosD3LblB"/>.
        /// </summary>
        /// <param name="pointAccessor">The point accessor allows access to the point data without casting to explicit a explicit point type."/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="createMesh">Function for creating the <see cref="GpuMesh"/>.</param>
        public static GpuMesh CreateMeshPosD3ColF3LblB<TPoint>(PointAccessor<TPoint> pointAccessor, TPoint[] points, CreateGpuMesh createMesh)
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
            var mesh = createMesh(PrimitiveType.Points, vertices, triangles, null, colors);
            mesh.BoundingBox = boundingBox;
            return mesh;
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
