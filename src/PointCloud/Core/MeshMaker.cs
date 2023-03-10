﻿using CommunityToolkit.HighPerformance.Buffers;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Potree.V2.Data;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
        /// <param name="points">The generic point cloud points.</param>
        /// <param name="createGpuDataHandler">The method that defines how to create a GpuMesh from the point cloud points.</param>
        /// <returns></returns>
        public static IEnumerable<TGpuData> CreateMeshes<TGpuData, TPoint>(MemoryOwner<TPoint> points, CreateGpuData<TGpuData, TPoint> createGpuDataHandler, OctantId octantId)
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

                MemoryOwner<TPoint> pointsPerMesh;
                if (ptCnt > maxVertCount)
                {
                    pointsPerMesh = MemoryOwner<TPoint>.Allocate(numberOfPointsInMesh);
                    points.Span.Slice(i, numberOfPointsInMesh).CopyTo(pointsPerMesh.Span.Slice(0));
                }
                else
                {
                    pointsPerMesh = points;
                }

                meshes.Add(createGpuDataHandler(pointsPerMesh, octantId));
                meshCnt++;
            }
            return meshes;
        }

        /// <summary>
        /// Returns the instance Data for a given point type by using the provided delegate.
        /// </summary>
        /// <typeparam name="TGpuData">Can be of type <see cref="GpuMesh"/> or <see cref="InstanceData"/>. The latter is used when rendering instanced.</typeparam>
        /// <typeparam name="TPoint">The generic point type.</typeparam>
        /// <param name="points">The generic point cloud points.</param>
        /// <param name="createGpuDataHandler">The method that defines how to create a InstanceData from the point cloud points.</param>
        /// <returns></returns>
        public static IEnumerable<TGpuData> CreateInstanceData<TGpuData, TPoint>(MemoryOwner<TPoint> points, CreateGpuData<TGpuData, TPoint> createGpuDataHandler, OctantId octantId)
        {
            return new List<TGpuData>
            {
                createGpuDataHandler(points, octantId)
            };
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="VisualizationPoint"/>.
        /// </summary>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="octantId">The id of the octant.</param>
        public static GpuMesh CreateMeshVisualizationPoint(MemoryOwner<VisualizationPoint> points, OctantId octantId)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var firstPos = points.Span[0].Position;
            var vertices = new float3[numberOfPointsInMesh];
            var triangles = new uint[numberOfPointsInMesh];
            var colors = new uint[numberOfPointsInMesh];
            var flags = new uint[numberOfPointsInMesh];
            var boundingBox = new AABBf(firstPos, firstPos);

            for (int i = 0; i < points.Length; i++)
            {
                var pos = points.Span[i].Position;

                vertices[i] = pos;
                boundingBox |= vertices[i];

                triangles[i] = (uint)i;
                var col = points.Span[i].Color;
                colors[i] = ColorToUInt((int)col.r, (int)col.g, (int)col.b, 255);
                flags[i] = points.Span[i].Flags;
            }
            var mesh = ModuleExtensionPoint.CreateGpuMesh(PrimitiveType.Points, vertices, triangles, null, colors, null, null, null, null, null, null, null, flags);
            mesh.BoundingBox = boundingBox;
            return mesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="VisualizationPoint"/>.
        /// </summary>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="octantId">The id of the octant.</param>
        public static Mesh CreateDynamicMeshVisualizationPoint(MemoryOwner<VisualizationPoint> points, OctantId octantId)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var firstPos = points.Span[0].Position;
            var vertices = new float3[numberOfPointsInMesh];
            var triangles = new uint[numberOfPointsInMesh];
            var colors = new uint[numberOfPointsInMesh];
            var flags = new uint[numberOfPointsInMesh];
            var boundingBox = new AABBf(firstPos, firstPos);

            for (int i = 0; i < points.Length; i++)
            {
                var pos = points.Span[i].Position;

                vertices[i] = pos;
                boundingBox |= vertices[i];

                triangles[i] = (uint)i;
                var col = points.Span[i].Color;
                colors[i] = ColorToUInt((int)col.r, (int)col.g, (int)col.b, 255);
                flags[i] = points.Span[i].Flags;
            }

            return new Mesh(triangles, vertices, null, null, null, null, null, null, colors, null, null, flags)
            {
                Name = OctantId.OctantIdToPotreeName(octantId),
                MeshType = PrimitiveType.Points
            };
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="PosD3LblB"/>.
        /// </summary>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="octantId">The id of the octant.</param>
        public static InstanceData CreateInstanceDataVisualizationPoint(MemoryOwner<VisualizationPoint> points, OctantId octantId)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var firstPos = points.Span[0].Position;
            var vertices = new float3[numberOfPointsInMesh];
            var triangles = new ushort[numberOfPointsInMesh];
            var colors = new float4[numberOfPointsInMesh];
            var boundingBox = new AABBf(firstPos, firstPos);
            var flags = new uint[numberOfPointsInMesh];

            for (int i = 0; i < points.Length; i++)
            {
                var pos = points.Span[i].Position;

                vertices[i] = pos;
                boundingBox |= vertices[i];

                triangles[i] = (ushort)i;
                colors[i] = points.Span[i].Color;
                flags[i] = points.Span[i].Flags;
            }

            // TODO: Add flags to InstanceData 
            return new InstanceData(points.Length, vertices, null, null, colors)
            {
                Name = octantId.ToString()
            };
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
    