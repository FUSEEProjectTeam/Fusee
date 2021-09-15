using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using System.Collections.Generic;

namespace Fusee.PointCloud.PointAccessorCollections
{
    /// <summary>
    /// This class defines a method for every <see cref="PointType"/>, that returns a Mesh which can hold point cloud points.
    /// </summary>
    public static class MeshFromPointList
    {
        /// <summary>
        /// Returns meshes for point clouds that only have position information in double precision.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        public static List<Mesh> GetMeshsForNodePos64(PointAccessor<Pos64> ptAccessor, List<Pos64> points)
        {
            int maxVertCount = ushort.MaxValue - 1;
            var noOfMeshes = (int)System.Math.Ceiling((float)points.Count / maxVertCount);
            List<Mesh> allMeshes = new(noOfMeshes);

            Mesh currentMesh = new();
            int indexCount = 0;

            for (int i = 0; i < points.Count; i++)
            {
                if (i % maxVertCount == 0)
                {
                    int numberOfPointsInMesh;

                    if (noOfMeshes == 1)
                        numberOfPointsInMesh = points.Count;
                    else if (noOfMeshes == allMeshes.Count + 1)
                        numberOfPointsInMesh = points.Count - maxVertCount * allMeshes.Count;
                    else
                        numberOfPointsInMesh = maxVertCount;

                    indexCount = 0;
                    currentMesh = new Mesh
                    {
                        Vertices = new float3[numberOfPointsInMesh],
                        Triangles = new ushort[numberOfPointsInMesh],
                        Colors = new uint[numberOfPointsInMesh]
                    };
                    allMeshes.Add(currentMesh);
                }

                var pt = points[i];
                currentMesh.Vertices[indexCount] = new float3(ptAccessor.GetPositionFloat3_64(ref pt));
                currentMesh.Triangles[indexCount] = (ushort)indexCount;

                indexCount++;
            }

            return allMeshes;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64Col32IShort"/>.
        /// Because we can only save one Color value at a <see cref="Mesh"/> right now we choose the intensity here.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        public static List<Mesh> GetMeshsForNodePos64Col32IShort(PointAccessor<Pos64Col32IShort> ptAccessor, List<Pos64Col32IShort> points)
        {
            int maxVertCount = ushort.MaxValue - 1;
            var noOfMeshes = (int)System.Math.Ceiling((float)points.Count / maxVertCount);
            List<Mesh> allMeshes = new(noOfMeshes);

            Mesh currentMesh = new();
            int indexCount = 0;

            for (int i = 0; i < points.Count; i++)
            {
                if (i % maxVertCount == 0)
                {
                    int numberOfPointsInMesh;

                    if (noOfMeshes == 1)
                        numberOfPointsInMesh = points.Count;
                    else if (noOfMeshes == allMeshes.Count + 1)
                        numberOfPointsInMesh = points.Count - maxVertCount * allMeshes.Count;
                    else
                        numberOfPointsInMesh = maxVertCount;

                    indexCount = 0;
                    currentMesh = new Mesh
                    {
                        Vertices = new float3[numberOfPointsInMesh],
                        Triangles = new ushort[numberOfPointsInMesh],
                        Colors = new uint[numberOfPointsInMesh]
                    };
                    allMeshes.Add(currentMesh);
                }

                var pt = points[i];
                currentMesh.Vertices[indexCount] = new float3(ptAccessor.GetPositionFloat3_64(ref pt));
                currentMesh.Triangles[indexCount] = (ushort)indexCount;
                //var col = ptAccessor.GetColorFloat3_32(ref pt);
                //currentMesh.Colors[indexCount] = ColorToUInt((int)col.r / 256, (int)col.g / 256, (int)col.b / 256);
                var intensity = (int)(pt.Intensity / 4096f * 256);
                currentMesh.Colors[indexCount] = ColorToUInt(intensity, intensity, intensity);

                indexCount++;
            }

            return allMeshes;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64IShort"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        public static List<Mesh> GetMeshsForNodePos64IShort(PointAccessor<Pos64IShort> ptAccessor, List<Pos64IShort> points)
        {
            int maxVertCount = ushort.MaxValue - 1;
            var noOfMeshes = (int)System.Math.Ceiling((float)points.Count / maxVertCount);
            List<Mesh> allMeshes = new(noOfMeshes);

            Mesh currentMesh = new();
            int indexCount = 0;

            for (int i = 0; i < points.Count; i++)
            {
                if (i % maxVertCount == 0)
                {
                    int numberOfPointsInMesh;

                    if (noOfMeshes == 1)
                        numberOfPointsInMesh = points.Count;
                    else if (noOfMeshes == allMeshes.Count + 1)
                        numberOfPointsInMesh = points.Count - maxVertCount * allMeshes.Count;
                    else
                        numberOfPointsInMesh = maxVertCount;

                    indexCount = 0;
                    currentMesh = new Mesh
                    {
                        Vertices = new float3[numberOfPointsInMesh],
                        Triangles = new ushort[numberOfPointsInMesh],
                        Colors = new uint[numberOfPointsInMesh]
                    };
                    allMeshes.Add(currentMesh);
                }

                var pt = points[i];
                currentMesh.Vertices[indexCount] = new float3(ptAccessor.GetPositionFloat3_64(ref pt));
                currentMesh.Triangles[indexCount] = (ushort)indexCount;
                //var col = ptAccessor.GetColorFloat3_32(ref pt);
                //currentMesh.Colors[indexCount] = ColorToUInt((int)col.r / 256, (int)col.g / 256, (int)col.b / 256);
                var intensity = (int)(pt.Intensity / 4096f * 256);
                currentMesh.Colors[indexCount] = ColorToUInt(intensity, intensity, intensity);

                indexCount++;
            }

            return allMeshes;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64Label8"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        public static List<Mesh> GetMeshsForNodePos64Label8(PointAccessor<Pos64Label8> ptAccessor, List<Pos64Label8> points)
        {
            int maxVertCount = ushort.MaxValue - 1;
            var noOfMeshes = (int)System.Math.Ceiling((float)points.Count / maxVertCount);
            List<Mesh> allMeshes = new(noOfMeshes);

            Mesh currentMesh = new();
            int indexCount = 0;

            for (int i = 0; i < points.Count; i++)
            {
                if (i % maxVertCount == 0)
                {
                    int numberOfPointsInMesh;

                    if (noOfMeshes == 1)
                        numberOfPointsInMesh = points.Count;
                    else if (noOfMeshes == allMeshes.Count + 1)
                        numberOfPointsInMesh = points.Count - maxVertCount * allMeshes.Count;
                    else
                        numberOfPointsInMesh = maxVertCount;

                    indexCount = 0;
                    currentMesh = new Mesh
                    {
                        Vertices = new float3[numberOfPointsInMesh],
                        Triangles = new ushort[numberOfPointsInMesh],
                        Colors = new uint[numberOfPointsInMesh]
                    };
                    allMeshes.Add(currentMesh);
                }

                var pt = points[i];
                currentMesh.Vertices[indexCount] = new float3(ptAccessor.GetPositionFloat3_64(ref pt));
                currentMesh.Triangles[indexCount] = (ushort)indexCount;

                var lbl = ptAccessor.GetLabelUInt_8(ref pt);
                var colorScale = new float3[4] { new float3(0, 0, 1), new float3(0, 1, 0), new float3(1, 1, 0), new float3(1, 0, 0) };
                currentMesh.Colors[indexCount] = ColorToUint(LabelToColor(colorScale, 32, lbl));

                indexCount++;
            }

            return allMeshes;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64Col32"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        public static List<Mesh> GetMeshsForNodePos64Col32(PointAccessor<Pos64Col32> ptAccessor, List<Pos64Col32> points)
        {
            int maxVertCount = ushort.MaxValue - 1;
            var noOfMeshes = (int)System.Math.Ceiling((float)points.Count / maxVertCount);
            List<Mesh> allMeshes = new(noOfMeshes);

            Mesh currentMesh = new();
            int indexCount = 0;

            for (int i = 0; i < points.Count; i++)
            {
                if (i % maxVertCount == 0)
                {
                    int numberOfPointsInMesh;

                    if (noOfMeshes == 1)
                        numberOfPointsInMesh = points.Count;
                    else if (noOfMeshes == allMeshes.Count + 1)
                        numberOfPointsInMesh = points.Count - maxVertCount * allMeshes.Count;
                    else
                        numberOfPointsInMesh = maxVertCount;

                    indexCount = 0;
                    currentMesh = new Mesh
                    {
                        Vertices = new float3[numberOfPointsInMesh],
                        Triangles = new ushort[numberOfPointsInMesh],
                        Colors = new uint[numberOfPointsInMesh]
                    };
                    allMeshes.Add(currentMesh);
                }

                var pt = points[i];
                currentMesh.Vertices[indexCount] = new float3(ptAccessor.GetPositionFloat3_64(ref pt));
                currentMesh.Triangles[indexCount] = (ushort)indexCount;
                var col = ptAccessor.GetColorFloat3_32(ref pt);
                currentMesh.Colors[indexCount] = ColorToUInt((int)col.r / 256, (int)col.g / 256, (int)col.b / 256);

                indexCount++;
            }

            return allMeshes;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64Nor32Col32IShort"/>.
        /// Because we can only save one color calue at a <see cref="Mesh"/> the intensity is chosen here.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        public static List<Mesh> GetMeshsForNode_Pos64Nor32Col32IShort(PointAccessor<Pos64Nor32Col32IShort> ptAccessor, List<Pos64Nor32Col32IShort> points)
        {
            int maxVertCount = ushort.MaxValue - 1;
            var noOfMeshes = (int)System.Math.Ceiling((float)points.Count / maxVertCount);
            List<Mesh> allMeshes = new(noOfMeshes);

            Mesh currentMesh = new();
            int indexCount = 0;

            for (int i = 0; i < points.Count; i++)
            {
                if (i % maxVertCount == 0)
                {
                    int numberOfPointsInMesh;

                    if (noOfMeshes == 1)
                        numberOfPointsInMesh = points.Count;
                    else if (noOfMeshes == allMeshes.Count + 1)
                        numberOfPointsInMesh = points.Count - maxVertCount * allMeshes.Count;
                    else
                        numberOfPointsInMesh = maxVertCount;

                    indexCount = 0;
                    currentMesh = new Mesh
                    {
                        Vertices = new float3[numberOfPointsInMesh],
                        Triangles = new ushort[numberOfPointsInMesh],
                        Colors = new uint[numberOfPointsInMesh],
                        Normals = new float3[numberOfPointsInMesh]
                    };
                    allMeshes.Add(currentMesh);
                }

                var pt = points[i];
                currentMesh.Vertices[indexCount] = new float3(ptAccessor.GetPositionFloat3_64(ref pt));
                currentMesh.Triangles[indexCount] = (ushort)indexCount;
                //var col = ptAccessor.GetColorFloat3_32(ref pt);
                //currentMesh.Colors[indexCount] = ColorToUInt((int)col.r / 256, (int)col.g / 256, (int)col.b / 256);
                var intensity = (int)(pt.Intensity / 4096f * 256);
                currentMesh.Colors[indexCount] = ColorToUInt(intensity, intensity, intensity);
                currentMesh.Normals[indexCount] = ptAccessor.GetNormalFloat3_32(ref pt);

                indexCount++;
            }

            return allMeshes;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64Nor32IShort"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        public static List<Mesh> GetMeshsForNode_Pos64Nor32IShort(PointAccessor<Pos64Nor32IShort> ptAccessor, List<Pos64Nor32IShort> points)
        {
            int maxVertCount = ushort.MaxValue - 1;
            var noOfMeshes = (int)System.Math.Ceiling((float)points.Count / maxVertCount);
            List<Mesh> allMeshes = new(noOfMeshes);

            Mesh currentMesh = new();
            int indexCount = 0;

            for (int i = 0; i < points.Count; i++)
            {
                if (i % maxVertCount == 0)
                {
                    int numberOfPointsInMesh;

                    if (noOfMeshes == 1)
                        numberOfPointsInMesh = points.Count;
                    else if (noOfMeshes == allMeshes.Count + 1)
                        numberOfPointsInMesh = points.Count - maxVertCount * allMeshes.Count;
                    else
                        numberOfPointsInMesh = maxVertCount;

                    indexCount = 0;
                    currentMesh = new Mesh
                    {
                        Vertices = new float3[numberOfPointsInMesh],
                        Triangles = new ushort[numberOfPointsInMesh],
                        Colors = new uint[numberOfPointsInMesh],
                        Normals = new float3[numberOfPointsInMesh]
                    };
                    allMeshes.Add(currentMesh);
                }

                var pt = points[i];
                currentMesh.Vertices[indexCount] = new float3(ptAccessor.GetPositionFloat3_64(ref pt));
                currentMesh.Triangles[indexCount] = (ushort)indexCount;
                var col = ptAccessor.GetColorFloat3_32(ref pt);
                currentMesh.Colors[indexCount] = ColorToUInt((int)col.r / 256, (int)col.g / 256, (int)col.b / 256);
                currentMesh.Normals[indexCount] = ptAccessor.GetNormalFloat3_32(ref pt);

                indexCount++;
            }

            return allMeshes;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64Nor32Col32"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        public static List<Mesh> GetMeshsForNode_Pos64Nor32Col32(PointAccessor<Pos64Nor32Col32> ptAccessor, List<Pos64Nor32Col32> points)
        {
            int maxVertCount = ushort.MaxValue - 1;
            var noOfMeshes = (int)System.Math.Ceiling((float)points.Count / maxVertCount);
            List<Mesh> allMeshes = new(noOfMeshes);

            Mesh currentMesh = new();
            int indexCount = 0;

            for (int i = 0; i < points.Count; i++)
            {
                if (i % maxVertCount == 0)
                {
                    int numberOfPointsInMesh;

                    if (noOfMeshes == 1)
                        numberOfPointsInMesh = points.Count;
                    else if (noOfMeshes == allMeshes.Count + 1)
                        numberOfPointsInMesh = points.Count - maxVertCount * allMeshes.Count;
                    else
                        numberOfPointsInMesh = maxVertCount;

                    indexCount = 0;
                    currentMesh = new Mesh
                    {
                        Vertices = new float3[numberOfPointsInMesh],
                        Triangles = new ushort[numberOfPointsInMesh],
                        Colors = new uint[numberOfPointsInMesh],
                        Normals = new float3[numberOfPointsInMesh]
                    };
                    allMeshes.Add(currentMesh);
                }

                var pt = points[i];
                currentMesh.Vertices[indexCount] = new float3(ptAccessor.GetPositionFloat3_64(ref pt));
                currentMesh.Triangles[indexCount] = (ushort)indexCount;
                var col = ptAccessor.GetColorFloat3_32(ref pt);
                currentMesh.Colors[indexCount] = ColorToUInt((int)col.r / 256, (int)col.g / 256, (int)col.b / 256);
                currentMesh.Normals[indexCount] = ptAccessor.GetNormalFloat3_32(ref pt);

                indexCount++;
            }

            return allMeshes;
        }

        private static float3 LabelToColor(float3[] colors, int numberOfLabels, int label)
        {
            var numberOfColors = colors.Length;
            var range = numberOfLabels / (numberOfColors - 1.0f);
            var colArea = label == 0 ? 0 : (int)(label / range);

            var col1 = colors[colArea];
            var col2 = colors[colArea + 1];

            var percent = (((100 / range) * label) % 100) / 100;

            return col1 + percent * (col2 - col1);

        }

        private static uint ColorToUInt(int r, int g, int b)
        {
            return (uint)((b << 16) | (g << 8) | (r << 0));
        }

        private static uint ColorToUint(float3 col)
        {
            uint packedR = (uint)(col.r * 255);
            uint packedG = (uint)(col.g * 255) << 8;
            uint packedB = (uint)(col.b * 255) << 16;

            return packedR + packedG + packedB;
        }
    }
}