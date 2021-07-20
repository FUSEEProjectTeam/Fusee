using Fusee.Engine.Common;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.PointCloud.PointAccessorCollections
{
    /// <summary>
    /// This class defines a method for every <see cref="PointType"/>, that returns a Mesh which can hold point cloud points.
    /// </summary>
    public static class MeshFromOocFile
    {
        /// <summary>
        /// Returns meshes for point clouds that only have position information in double precision.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        public static List<Mesh> GetMeshsForNode_Pos64(PointAccessor<Pos64> ptAccessor, List<Pos64> points)
        {
            var allPoints = new List<double3>();


            for (int i = 0; i < points.Count(); i++)
            {
                var pt = points[i];
                allPoints.Add(ptAccessor.GetPositionFloat3_64(ref pt));
            }

            var allMeshes = new List<Mesh>();

            var maxVertCount = ushort.MaxValue - 1;

            var allPointsSplitted = SplitList(allPoints, maxVertCount).ToList();


            for (int i = 0; i < allPointsSplitted.Count; i++)
            {
                var pointSplit = allPointsSplitted[i];

                var currentMesh = new Mesh
                {
                    Vertices = pointSplit.Select(pt => new float3(pt.xyz)).ToArray(),
                    Triangles = Enumerable.Range(0, pointSplit.Count).Select(num => (ushort)num).ToArray(),
                    MeshType = (int)OpenGLPrimitiveType.Points,
                    Normals = new float3[pointSplit.Count],
                    Colors = new uint[pointSplit.Count]
                };

                allMeshes.Add(currentMesh);
            }

            return allMeshes;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64Col32IShort"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        public static List<Mesh> GetMeshsForNode_Pos64Col32IShort(PointAccessor<Pos64Col32IShort> ptAccessor, List<Pos64Col32IShort> points)
        {
            var allPoints = new List<double3>();
            var allIntensities = points.ToArray().Select(pt => (float)pt.Intensity).ToList();
            var allColors = new List<float3>();

            for (int i = 0; i < points.Count(); i++)
            {
                var pt = points[i];
                allPoints.Add(ptAccessor.GetPositionFloat3_64(ref pt));
                allColors.Add(ptAccessor.GetColorFloat3_32(ref pt));
            }

            var allMeshes = new List<Mesh>();

            var maxVertCount = ushort.MaxValue - 1;

            var allPointsSplitted = SplitList(allPoints, maxVertCount).ToList();
            var allColorsSplitted = SplitList(allColors, maxVertCount).ToList();
            var allIntensitiesSplitted = SplitList(allIntensities, maxVertCount).ToList();

            for (int i = 0; i < allPointsSplitted.Count; i++)
            {
                var pointSplit = allPointsSplitted[i];
                var colorSplit = allColorsSplitted[i];
                var intentsitySplit = allIntensitiesSplitted[i];

                var currentMesh = new Mesh
                {
                    Vertices = pointSplit.Select(pt => new float3(pt.xyz)).ToArray(),
                    Triangles = Enumerable.Range(0, pointSplit.Count).Select(num => (ushort)num).ToArray(),
                    MeshType = (int)OpenGLPrimitiveType.Points,
                    Normals = new float3[pointSplit.Count],
                    //Colors = colorSplit.Select(pt => ColorToUInt((int)pt.r / 256, (int)pt.g / 256, (int)pt.b / 256)).ToArray(),
                    Colors = intentsitySplit.Select(pt => ColorToUInt((int)(pt / 4096f * 256), (int)(pt / 4096f * 256), (int)(pt / 4096f * 256))).ToArray(),
                };

                allMeshes.Add(currentMesh);
            }

            return allMeshes;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64IShort"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        public static List<Mesh> GetMeshsForNode_Pos64IShort(PointAccessor<Pos64IShort> ptAccessor, List<Pos64IShort> points)
        {
            var allPoints = new List<double3>();
            var allIntensities = points.ToArray().Select(pt => (float)pt.Intensity).ToList();

            for (int i = 0; i < points.Count(); i++)
            {
                var pt = points[i];
                allPoints.Add(ptAccessor.GetPositionFloat3_64(ref pt));
            }

            var allMeshes = new List<Mesh>();

            var maxVertCount = ushort.MaxValue - 1;

            var allPointsSplitted = SplitList(allPoints, maxVertCount).ToList();
            var allIntensitiesSplitted = SplitList(allIntensities, maxVertCount).ToList();

            for (int i = 0; i < allPointsSplitted.Count; i++)
            {
                var pointSplit = allPointsSplitted[i];
                var intentsitySplit = allIntensitiesSplitted[i];

                var currentMesh = new Mesh
                {
                    Vertices = pointSplit.Select(pt => new float3(pt.xyz)).ToArray(),
                    Triangles = Enumerable.Range(0, pointSplit.Count).Select(num => (ushort)num).ToArray(),
                    MeshType = (int)OpenGLPrimitiveType.Points,
                    Normals = new float3[pointSplit.Count],
                    Colors = intentsitySplit.Select(pt => ColorToUInt((int)(pt / 4096f * 256), (int)(pt / 4096f * 256), (int)(pt / 4096f * 256))).ToArray(),
                };

                allMeshes.Add(currentMesh);
            }

            return allMeshes;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64Label8"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        public static List<Mesh> GetMeshsForNode_Pos64Label8(PointAccessor<Pos64Label8> ptAccessor, List<Pos64Label8> points)
        {
            var allPoints = new List<double3>();
            var allLabels = new List<float3>();

            for (int i = 0; i < points.Count(); i++)
            {
                var pt = points[i];
                allPoints.Add(ptAccessor.GetPositionFloat3_64(ref pt));

                var lbl = ptAccessor.GetLabelUInt_8(ref pt);
                var colorScale = new float3[4] { new float3(0, 0, 1), new float3(0, 1, 0), new float3(1, 1, 0), new float3(1, 0, 0) };
                var col = LabelToColor(colorScale, 32, lbl);
                allLabels.Add(col);
            }

            var allMeshes = new List<Mesh>();

            var maxVertCount = ushort.MaxValue - 1;

            var allPointsSplitted = SplitList(allPoints, maxVertCount).ToList();
            var allLabelsSplitted = SplitList(allLabels, maxVertCount).ToList();

            for (int i = 0; i < allPointsSplitted.Count; i++)
            {
                var pointSplit = allPointsSplitted[i];
                var labelSplit = allLabelsSplitted[i];

                var currentMesh = new Mesh
                {
                    Vertices = pointSplit.Select(pt => new float3(pt.xyz)).ToArray(),
                    Triangles = Enumerable.Range(0, pointSplit.Count).Select(num => (ushort)num).ToArray(),
                    MeshType = (int)OpenGLPrimitiveType.Points,
                    Normals = new float3[pointSplit.Count],
                    Colors = labelSplit.Select(pt => ColorToUint(pt)).ToArray()
                };

                allMeshes.Add(currentMesh);
            }

            return allMeshes;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64Col32"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        public static List<Mesh> GetMeshsForNode_Pos64Col32(PointAccessor<Pos64Col32> ptAccessor, List<Pos64Col32> points)
        {
            var allPoints = new List<double3>();
            var allColors = new List<float3>();

            for (int i = 0; i < points.Count(); i++)
            {
                var pt = points[i];
                allPoints.Add(ptAccessor.GetPositionFloat3_64(ref pt));
                allColors.Add(ptAccessor.GetColorFloat3_32(ref pt));
            }

            var allMeshes = new List<Mesh>();

            var maxVertCount = ushort.MaxValue - 1;

            var allPointsSplitted = SplitList(allPoints, maxVertCount).ToList();
            var allColorsSplitted = SplitList(allColors, maxVertCount).ToList();

            for (int i = 0; i < allPointsSplitted.Count; i++)
            {
                var pointSplit = allPointsSplitted[i];
                var colorSplit = allColorsSplitted[i]; ;

                var currentMesh = new Mesh
                {
                    Vertices = pointSplit.Select(pt => new float3(pt.xyz)).ToArray(),
                    Triangles = Enumerable.Range(0, pointSplit.Count).Select(num => (ushort)num).ToArray(),
                    MeshType = (int)OpenGLPrimitiveType.Points,
                    Normals = new float3[pointSplit.Count],
                    Colors = colorSplit.Select(pt => ColorToUInt((int)pt.r / 256, (int)pt.g / 256, (int)pt.b / 256)).ToArray()
                };

                allMeshes.Add(currentMesh);
            }

            return allMeshes;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64Nor32Col32IShort"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        public static List<Mesh> GetMeshsForNode_Pos64Nor32Col32IShort(PointAccessor<Pos64Nor32Col32IShort> ptAccessor, List<Pos64Nor32Col32IShort> points)
        {
            var allPoints = new List<double3>();
            var allIntensities = points.ToArray().Select(pt => (float)pt.Intensity).ToList();
            var allColors = new List<float3>();
            var allNormals = new List<float3>();

            for (int i = 0; i < points.Count(); i++)
            {
                var pt = points[i];
                allPoints.Add(ptAccessor.GetPositionFloat3_64(ref pt));
                allColors.Add(ptAccessor.GetColorFloat3_32(ref pt));
                allNormals.Add(ptAccessor.GetNormalFloat3_32(ref pt));
            }

            var allMeshes = new List<Mesh>();
            var maxVertCount = ushort.MaxValue - 1;

            var allPointsSplitted = SplitList(allPoints, maxVertCount).ToList();
            var allColorsSplitted = SplitList(allColors, maxVertCount).ToList();
            var allIntensitiesSplitted = SplitList(allIntensities, maxVertCount).ToList();
            var allNormalsSplitted = SplitList(allNormals, maxVertCount).ToList();

            for (int i = 0; i < allPointsSplitted.Count; i++)
            {
                var pointSplit = allPointsSplitted[i];
                var colorSplit = allColorsSplitted[i];
                var intentsitySplit = allIntensitiesSplitted[i];
                var normalSplitt = allNormalsSplitted[i];

                var currentMesh = new Mesh
                {
                    Vertices = pointSplit.Select(pt => new float3(pt.xyz)).ToArray(),
                    Triangles = Enumerable.Range(0, pointSplit.Count).Select(num => (ushort)num).ToArray(),
                    MeshType = (int)OpenGLPrimitiveType.Points,
                    Normals = normalSplitt.Select(pt => new float3(pt.xyz)).ToArray(),
                    //Colors = colorSplit.Select(pt => ColorToUInt((int)pt.r / 256, (int)pt.g / 256, (int)pt.b / 256)).ToArray(),
                    Colors = intentsitySplit.Select(pt => ColorToUInt((int)(pt / 4096f * 256), (int)(pt / 4096f * 256), (int)(pt / 4096f * 256))).ToArray(),
                };

                allMeshes.Add(currentMesh);
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
            var allPoints = new List<double3>();
            var allIntensities = points.ToArray().Select(pt => (float)pt.Intensity).ToList();
            var allNormals = new List<float3>();

            for (int i = 0; i < points.Count(); i++)
            {
                var pt = points[i];
                allPoints.Add(ptAccessor.GetPositionFloat3_64(ref pt));
                allNormals.Add(ptAccessor.GetNormalFloat3_32(ref pt));
            }

            var allMeshes = new List<Mesh>();

            var maxVertCount = ushort.MaxValue - 1;

            var allPointsSplitted = SplitList(allPoints, maxVertCount).ToList();
            var allIntensitiesSplitted = SplitList(allIntensities, maxVertCount).ToList();
            var allNormalsSplitted = SplitList(allNormals, maxVertCount).ToList();

            for (int i = 0; i < allPointsSplitted.Count; i++)
            {
                var pointSplit = allPointsSplitted[i];
                var intentsitySplit = allIntensitiesSplitted[i];
                var normalSplitt = allNormalsSplitted[i];

                var currentMesh = new Mesh
                {
                    Vertices = pointSplit.Select(pt => new float3(pt.xyz)).ToArray(),
                    Triangles = Enumerable.Range(0, pointSplit.Count).Select(num => (ushort)num).ToArray(),
                    MeshType = (int)OpenGLPrimitiveType.Points,
                    Normals = normalSplitt.Select(pt => new float3(pt.xyz)).ToArray(),
                    Colors = intentsitySplit.Select(pt => ColorToUInt((int)(pt / 4096f * 256), (int)(pt / 4096f * 256), (int)(pt / 4096f * 256))).ToArray(),
                };

                allMeshes.Add(currentMesh);
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
            var allPoints = new List<double3>();
            var allColors = new List<float3>();
            var allNormals = new List<float3>();

            for (int i = 0; i < points.Count(); i++)
            {
                var pt = points[i];
                allPoints.Add(ptAccessor.GetPositionFloat3_64(ref pt));
                allColors.Add(ptAccessor.GetColorFloat3_32(ref pt));
                allNormals.Add(ptAccessor.GetNormalFloat3_32(ref pt));
            }

            var allMeshes = new List<Mesh>();

            var maxVertCount = ushort.MaxValue - 1;

            var allPointsSplitted = SplitList(allPoints, maxVertCount).ToList();
            var allColorsSplitted = SplitList(allColors, maxVertCount).ToList();
            var allNormalsSplitted = SplitList(allNormals, maxVertCount).ToList();

            for (int i = 0; i < allPointsSplitted.Count; i++)
            {
                var pointSplit = allPointsSplitted[i];
                var colorSplit = allColorsSplitted[i];
                var normalSplitt = allNormalsSplitted[i];

                var currentMesh = new Mesh
                {
                    Vertices = pointSplit.Select(pt => new float3(pt.xyz)).ToArray(),
                    Triangles = Enumerable.Range(0, pointSplit.Count).Select(num => (ushort)num).ToArray(),
                    MeshType = (int)OpenGLPrimitiveType.Points,
                    Normals = normalSplitt.Select(pt => new float3(pt.xyz)).ToArray(),
                    Colors = colorSplit.Select(pt => ColorToUInt((int)pt.r / 256, (int)pt.g / 256, (int)pt.b / 256)).ToArray()
                };

                allMeshes.Add(currentMesh);
            }

            return allMeshes;
        }

        private static IEnumerable<List<T>> SplitList<T>(List<T> locations, int nSize = 30)
        {
            for (var i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, M.Min(nSize, locations.Count - i));
            }
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