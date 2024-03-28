using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace Fusee.PointCloud.Core
{
    public enum SupportedPositionTypes
    {
        int32,
        float32
    }

    /// <summary>
    /// Delegate for a method that knows how to parse a slice of a point's extra bytes to a valid uint.
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public delegate uint HandleReadExtraBytes(Span<byte> bytes);

    /// <summary>
    /// Meta data needed to create a mesh from a point cloud file.
    /// </summary>
    public struct CreateMeshMetaData
    {
        /// <summary>
        /// Size of one point in byte.
        /// </summary>
        public int PointSize;

        /// <summary>
        /// Byte offset in one point to reach the position value.
        /// </summary>
        public int OffsetToPosValues;

        /// <summary>
        /// Byte offset in one point to reach the color value.
        /// -1 means there is no such value.
        /// </summary>
        public int OffsetColor;

        /// <summary>
        /// Byte offset in one point to reach the intensity value.
        /// -1 means there is no such value.
        /// </summary>
        public int OffsetIntensity;

        /// <summary>
        /// Maximum intensity value.
        /// </summary>
        public double IntensityMax;

        /// <summary>
        /// Minimum intensity value.
        /// </summary>
        public double IntensityMin;

        /// <summary>
        /// The x, y, z values used to scale the point positions.
        /// </summary>
        public double3 Scale;

        /// <summary>
        /// Byte offset in one point to reach the extra bytes.
        /// -1 means there is no such value.
        /// </summary>
        public int OffsetToExtraBytes;

        public SupportedPositionTypes PositionType;

    }

    /// <summary>
    /// Static class that provides generic methods that take point cloud points and return <see cref="GpuMesh"/>s.
    /// </summary>
    public static class MeshMaker
    {
        /// <summary>
        /// Generic method that creates meshes with 65k points maximum.
        /// </summary>
        /// <param name="points">The <see cref="MemoryOwner{T}"/> that contains the points.</param>
        /// <param name="createGpuDataHandler">The method that defines how to create a GpuMesh from the point cloud points.</param>
        /// <returns></returns>
        public static IEnumerable<TGpuData> CreateMeshes<TGpuData>(MemoryOwner<VisualizationPoint> points, CreateGpuData<TGpuData> createGpuDataHandler)
        {
            List<TGpuData> meshes;

            var numberOfPointInNode = points.Length;

            int maxVertCount = ushort.MaxValue - 1;
            var noOfMeshes = (int)System.Math.Ceiling((float)numberOfPointInNode / maxVertCount);
            meshes = new(noOfMeshes);
            int meshCnt = 0;

            for (int i = 0; i < numberOfPointInNode; i += maxVertCount)
            {
                int numberOfPointsInMesh;
                if (noOfMeshes == 1)
                    numberOfPointsInMesh = numberOfPointInNode;
                else if (noOfMeshes == meshCnt + 1)
                    numberOfPointsInMesh = (numberOfPointInNode - maxVertCount * meshCnt);
                else
                    numberOfPointsInMesh = maxVertCount;

                if (numberOfPointInNode > maxVertCount)
                {
                    using MemoryOwner<VisualizationPoint> pointsInMesh = MemoryOwner<VisualizationPoint>.Allocate(numberOfPointsInMesh);
                    points.Span.Slice(i, numberOfPointsInMesh).CopyTo(pointsInMesh.Span);
                    meshes.Add(createGpuDataHandler(pointsInMesh));
                }
                else
                    meshes.Add(createGpuDataHandler(points));

                meshCnt++;
            }
            return meshes;
        }

        /// <summary>
        /// Returns the instance Data for a given point type by using the provided delegate.
        /// </summary>
        /// <typeparam name="TGpuData">Can be of type <see cref="GpuMesh"/> or <see cref="InstanceData"/>. The latter is used when rendering instanced.</typeparam>
        /// <param name="points">The <see cref="MemoryOwner{T}"/> that contains the points.</param>
        /// <param name="createGpuDataHandler">The method that defines how to create a InstanceData from the point cloud points.</param>
        /// <returns></returns>
        public static IEnumerable<TGpuData> CreateInstanceData<TGpuData>(MemoryOwner<VisualizationPoint> points, CreateGpuData<TGpuData> createGpuDataHandler)
        {
            return new List<TGpuData>() { createGpuDataHandler(points) };
        }

        public static MemoryOwner<VisualizationPoint> CreateVisualizationPoints(MemoryMappedFile mmf, int numberOfPoints, HandleReadExtraBytes? handleExtraBytes, CreateMeshMetaData metaData, EventHandler<ErrorEventArgs>? onPointCloudReadError)
        {
            var size = numberOfPoints * metaData.PointSize;
            using var accessor = mmf.CreateViewAccessor();
            var rawPoints = new byte[size];

            accessor.ReadArray(0, rawPoints, 0, size);

            var pointsSpan = rawPoints.AsSpan();
            var numberOfRelevantPoints = 0;

            var visPoints = new List<VisualizationPoint>(65535);

            for (int i = 0; i < numberOfPoints; i++)
            {
                float x;
                float y;
                float z;
                switch (metaData.PositionType)
                {
                    case SupportedPositionTypes.int32:
                        {
                            var byteCountPos = sizeof(int) * 3;
                            var posRaw = pointsSpan.Slice(i * metaData.PointSize + metaData.OffsetToPosValues, byteCountPos);
                            var pos = MemoryMarshal.Cast<byte, int>(posRaw);

                            x = (float)(pos[0] * metaData.Scale.x);
                            y = (float)(pos[1] * metaData.Scale.y);
                            z = (float)(pos[2] * metaData.Scale.z);
                        }
                        break;
                    case SupportedPositionTypes.float32:
                        {
                            var byteCountPos = sizeof(float) * 3;
                            var posRaw = pointsSpan.Slice(i * metaData.PointSize + metaData.OffsetToPosValues, byteCountPos);
                            var pos = MemoryMarshal.Cast<byte, float>(posRaw);

                            x = (float)(pos[0] * metaData.Scale.x);
                            y = (float)(pos[1] * metaData.Scale.y);
                            z = (float)(pos[2] * metaData.Scale.z);
                        }
                        break;
                    default:
                        throw new ArgumentException($"Unsupported pos type: {metaData.PositionType} ");
                }

                float4 color;
                if (metaData.OffsetColor != -1)
                {
                    var byteCountColor = Marshal.SizeOf<ushort>() * 3;
                    var colorRaw = pointsSpan.Slice(i * metaData.PointSize + metaData.OffsetColor, byteCountColor);
                    var rgb = MemoryMarshal.Cast<byte, ushort>(colorRaw);

                    color = float4.Zero;

                    color.r = (byte)(rgb[0] > 255 ? rgb[0] / 256 : rgb[0]);
                    color.g = (byte)(rgb[1] > 255 ? rgb[1] / 256 : rgb[1]);
                    color.b = (byte)(rgb[2] > 255 ? rgb[2] / 256 : rgb[2]);
                    color.a = 1;
                }
                else if (metaData.OffsetIntensity != -1)
                {
                    var byteCountIntensity = Marshal.SizeOf<ushort>();
                    var intensityRaw = pointsSpan.Slice(i * metaData.PointSize + metaData.OffsetIntensity, byteCountIntensity);
                    var rgb = MemoryMarshal.Cast<byte, ushort>(intensityRaw);
                    color = float4.Zero;

                    color.r = (float)((rgb[0] - metaData.IntensityMin) / (metaData.IntensityMax - metaData.IntensityMin) * 1f);
                    color.g = color.r;
                    color.b = color.r;
                    color.a = 1;
                }
                else
                {
                    color = float4.UnitW;
                }

                uint flag = 0;
                Span<byte> extraBytesRaw = new();
                if (metaData.OffsetToExtraBytes != -1 && metaData.OffsetToExtraBytes != 0)
                {
                    var extraByteSize = metaData.PointSize - metaData.OffsetToExtraBytes;
                    extraBytesRaw = pointsSpan.Slice(i * metaData.PointSize + metaData.OffsetToExtraBytes, extraByteSize);
                }
                //handleExtraBytes should also handle the case there aren't any extra bytes -> call in any case.
                if (handleExtraBytes != null)
                {
                    try
                    {
                        flag = handleExtraBytes(extraBytesRaw);
                    }
                    catch (Exception e)
                    {
                        onPointCloudReadError?.Invoke(null, new ErrorEventArgs(e));
                    }
                }

                if (x != 0 || y != 0 || z != 0)
                {
                    var visPoint = new VisualizationPoint
                    {
                        Position = new float3(x, y, z),
                        Color = color,
                        Flags = flag
                    };

                    numberOfRelevantPoints++;
                    visPoints.Add(visPoint);
                }
            }

            MemoryOwner<VisualizationPoint> visPointsMem = MemoryOwner<VisualizationPoint>.Allocate(numberOfRelevantPoints);
            for (int i = 0; i < visPoints.Count; i++)
            {
                visPointsMem.Span[i] = visPoints[i];
            }
            return visPointsMem;
        }

        private static (float3[], uint[], uint[], uint[], AABBf) GetGpuDataContents(MemoryOwner<VisualizationPoint> points)
        {
            var numberOfPointsInMesh = points.Length;
            var vertices = new float3[numberOfPointsInMesh];
            var triangles = new uint[numberOfPointsInMesh];
            var colors = new uint[numberOfPointsInMesh];
            var flags = new uint[numberOfPointsInMesh];
            var boundingBox = new AABBf();
            var pointsSpan = points.Span;

            for (int i = 0; i < numberOfPointsInMesh; i++)
            {
                vertices[i] = pointsSpan[i].Position;
                if (i == 0)
                    boundingBox = new(vertices[i], vertices[i]);
                else
                    boundingBox |= vertices[i];
                triangles[i] = (uint)i;

                var col = pointsSpan[i].Color;
                colors[i] = ColorToUInt((int)col.r, (int)col.g, (int)col.b, 255);

                flags[i] = pointsSpan[i].Flags;
            }

            return (vertices, triangles, colors, flags, boundingBox);
        }

        /// <summary>
        /// Returns meshes for points of type <see cref="VisualizationPoint"/>.
        /// </summary>
        /// <param name="points">The <see cref="MemoryOwner{T}"/> that contains the points.</param>
        public static GpuMesh CreateStaticMesh(MemoryOwner<VisualizationPoint> points)
        {
            var meshData = GetGpuDataContents(points);
            var vertices = meshData.Item1;
            var triangles = meshData.Item2;
            var colors = meshData.Item3;
            var flags = meshData.Item4;
            var boundingBox = meshData.Item5;
            var mesh = ModuleExtensionPoint.CreateGpuMesh(PrimitiveType.Points, vertices, triangles, null, colors, null, null, null, null, null, null, null, flags);
            mesh.BoundingBox = boundingBox;
            return mesh;
        }

        /// <summary>
        /// Returns meshes for points of type <see cref="VisualizationPoint"/>.
        /// </summary>
        /// <param name="points">The <see cref="MemoryOwner{T}"/> that contains the points.</param>
        public static Mesh CreateDynamicMesh(MemoryOwner<VisualizationPoint> points)
        {
            var meshData = GetGpuDataContents(points);
            var vertices = meshData.Item1;
            var triangles = meshData.Item2;
            var colors = meshData.Item3;
            var flags = meshData.Item4;
            //var boundingBox = meshData.Item5;

            return new Mesh(triangles, vertices, null, null, null, null, null, null, colors, null, null, flags)
            {
                MeshType = PrimitiveType.Points
            };
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="VisualizationPoint"/>.
        /// </summary>
        /// <param name="points">The <see cref="MemoryOwner{T}"/> that contains the points.</param>
        public static InstanceData CreateInstanceData(MemoryOwner<VisualizationPoint> points)
        {
            var meshData = GetGpuDataContents(points);
            var vertices = meshData.Item1;
            var colors = meshData.Item3;
            //var flags = meshData.Item4;

            // TODO: Add flags to InstanceData 
            return new InstanceData(vertices.Length, vertices, null, null, colors)
            {
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
        public static uint ColorToUInt(int r, int g, int b, int a)
        {
            return (uint)((b << 16) | (g << 8) | (r << 0) | (a << 24));
        }

        #endregion
    }
}