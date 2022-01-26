using Fusee.Engine.Common;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using System;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Provides static methods for creating meshes from lists of point cloud points.
    /// </summary>
    public static class MeshFromPointCloudPoints
    {
        public static GpuMesh GetMesh(IPointAccessor ptAccessor, IPointCloudPoint[] points, bool doExchangeYZ, float3 translationVector, CreateGpuMesh createMesh)
        {
            switch (ptAccessor.PointType)
            {
                default:
                case PointType.Undefined:
                    throw new ArgumentException();
                case PointType.Pos64:
                    return GetMeshPos64((Pos64Accessor)ptAccessor, points, doExchangeYZ, translationVector, createMesh);
                case PointType.Pos64Col32IShort:
                    return GetMeshPos64Col32IShort((Pos64Col32IShortAccessor)ptAccessor, points, doExchangeYZ, translationVector, createMesh);
                case PointType.Pos64IShort:
                    return GetMeshPos64IShort((Pos64IShortAccessor)ptAccessor, points, doExchangeYZ, translationVector, createMesh);
                case PointType.Pos64Col32:
                    return GetMeshPos64Col32((Pos64Col32Accessor)ptAccessor, points, doExchangeYZ, translationVector, createMesh);
                case PointType.Pos64Label8:
                    return GetMeshPos64Label8((Pos64Label8Accessor)ptAccessor, points, doExchangeYZ, translationVector, createMesh);
                case PointType.Pos64Nor32Col32IShort:
                    return GetMeshPos64Nor32Col32IShort((Pos64Nor32Col32IShortAccessor)ptAccessor, points, doExchangeYZ, translationVector, createMesh);
                case PointType.Pos64Nor32IShort:
                    return GetMeshPos64Nor32IShort((Pos64Nor32IShortAccessor)ptAccessor, points, doExchangeYZ, translationVector, createMesh);
                case PointType.Pos64Nor32Col32:
                    return GetMeshPos64Nor32Col32((Pos64Nor32Col32Accessor)ptAccessor, points, doExchangeYZ, translationVector, createMesh);
                case PointType.Position_double__Color_float__Label_byte:
                    return GetMesh___Position_double__Color_float__Label_byte((Position_double__Color_float__Label_byte___Accessor)ptAccessor, points, doExchangeYZ, translationVector, createMesh);
            }
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        /// <param name="createMesh">Function for creating the <see cref="GpuMesh"/>.</param>
        public static GpuMesh GetMeshPos64(Pos64Accessor ptAccessor, IPointCloudPoint[] points, bool doExchangeYZ, float3 translationVector, CreateGpuMesh createMesh)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var firstPoint = (Pos64)points[0];
            var firstPos = ptAccessor.GetPositionFloat3_64(ref firstPoint);

            if (doExchangeYZ)
                ExchangeYZ(ref firstPos);

            var firstPosF = (float3)firstPos - translationVector;
            var vertices = new float3[numberOfPointsInMesh];
            var triangles = new ushort[numberOfPointsInMesh];
            var boundingBox = new AABBf(firstPosF, firstPosF);

            for (int i = 0; i < points.Length; i++)
            {
                var typedPoint = (Pos64)points[i];
                var pos = ptAccessor.GetPositionFloat3_64(ref typedPoint);
                if (doExchangeYZ)
                    ExchangeYZ(ref pos);

                var posF = (float3)pos - translationVector;

                vertices[i] = posF;
                boundingBox |= posF;
                triangles[i] = (ushort)i;
            }
            var mesh = createMesh(PrimitiveType.Points, vertices, triangles);
            mesh.BoundingBox = boundingBox;
            return mesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64Col32IShort"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        /// <param name="createMesh">Function for creating the <see cref="GpuMesh"/>.</param>
        public static GpuMesh GetMeshPos64Col32IShort(Pos64Col32IShortAccessor ptAccessor, IPointCloudPoint[] points, bool doExchangeYZ, float3 translationVector, CreateGpuMesh createMesh)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var firstPoint = (Pos64Col32IShort)points[0];
            var firstPos = ptAccessor.GetPositionFloat3_64(ref firstPoint);
            if (doExchangeYZ)
                ExchangeYZ(ref firstPos);

            var firstPosF = (float3)firstPos - translationVector;
            var vertices = new float3[numberOfPointsInMesh];
            var triangles = new ushort[numberOfPointsInMesh];
            var colors = new uint[numberOfPointsInMesh];
            var colors1 = new uint[numberOfPointsInMesh];
            var boundingBox = new AABBf(firstPosF, firstPosF);

            for (int i = 0; i < points.Length; i++)
            {
                var typedPoint = (Pos64Col32IShort)points[i];
                var pos = ptAccessor.GetPositionFloat3_64(ref typedPoint);
                if (doExchangeYZ)
                    ExchangeYZ(ref pos);

                var posF = (float3)pos - translationVector;

                vertices[i] = posF;
                boundingBox |= vertices[i];
                triangles[i] = (ushort)i;

                var col = ptAccessor.GetColorFloat3_32(ref typedPoint);
                colors[i] = ColorToUInt((int)col.r / 256, (int)col.g / 256, (int)col.b / 256, 255);
                var intensity = (int)(ptAccessor.GetIntensityUInt_16(ref typedPoint) / 4096f * 256);
                colors1[i] = ColorToUInt(intensity, intensity, intensity, 255);

            }
            var mesh = createMesh(PrimitiveType.Points, vertices, triangles, null, colors, colors1);
            mesh.BoundingBox = boundingBox;
            return mesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64IShort"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        /// <param name="createMesh">Function for creating the <see cref="GpuMesh"/>.</param>
        public static GpuMesh GetMeshPos64IShort(Pos64IShortAccessor ptAccessor, IPointCloudPoint[] points, bool doExchangeYZ, float3 translationVector, CreateGpuMesh createMesh)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var typedFirstPoint = (Pos64IShort)points[0];
            var firstPos = ptAccessor.GetPositionFloat3_64(ref typedFirstPoint);
            if (doExchangeYZ)
                ExchangeYZ(ref firstPos);

            var firstPosF = (float3)firstPos - translationVector;
            var vertices = new float3[numberOfPointsInMesh];
            var triangles = new ushort[numberOfPointsInMesh];
            var colors = new uint[numberOfPointsInMesh];
            var boundingBox = new AABBf(firstPosF, firstPosF);

            for (int i = 0; i < points.Length; i++)
            {
                var typedPoint = (Pos64IShort)points[i];
                var pos = ptAccessor.GetPositionFloat3_64(ref typedPoint);
                if (doExchangeYZ)
                    ExchangeYZ(ref pos);

                var posF = (float3)pos - translationVector;

                vertices[i] = posF;
                boundingBox |= vertices[i];
                triangles[i] = (ushort)i;
                var intensity = (int)(ptAccessor.GetIntensityUInt_16(ref typedPoint) / 4096f * 256);
                colors[i] = ColorToUInt(intensity, intensity, intensity, 255);
            }
            var mesh = createMesh(PrimitiveType.Points, vertices, triangles, null, colors);
            mesh.BoundingBox = boundingBox;
            return mesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64Label8"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        /// <param name="createMesh">Function for creating the <see cref="GpuMesh"/>.</param>
        public static GpuMesh GetMeshPos64Label8(Pos64Label8Accessor ptAccessor, IPointCloudPoint[] points, bool doExchangeYZ, float3 translationVector, CreateGpuMesh createMesh)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var typedFirstPoint = (Pos64Label8)points[0];
            var firstPos = ptAccessor.GetPositionFloat3_64(ref typedFirstPoint);
            if (doExchangeYZ)
                ExchangeYZ(ref firstPos);

            var firstPosF = (float3)firstPos - translationVector;
            var vertices = new float3[numberOfPointsInMesh];
            var triangles = new ushort[numberOfPointsInMesh];
            var colors = new uint[numberOfPointsInMesh];
            var boundingBox = new AABBf(firstPosF, firstPosF);

            for (int i = 0; i < points.Length; i++)
            {
                var typedPoint = (Pos64Label8)points[i];
                var pos = ptAccessor.GetPositionFloat3_64(ref typedPoint);
                if (doExchangeYZ)
                    ExchangeYZ(ref pos);

                var posF = (float3)pos - translationVector;

                vertices[i] = posF;
                boundingBox |= vertices[i];
                triangles[i] = (ushort)i;

                var lbl = ptAccessor.GetLabelUInt_8(ref typedPoint);
                var colorScale = new float3[4] { new float3(0, 0, 1), new float3(0, 1, 0), new float3(1, 1, 0), new float3(1, 0, 0) };
                colors[i] = ColorToUint(LabelToColor(colorScale, 32, lbl));

            }
            var mesh = createMesh(PrimitiveType.Points, vertices, triangles, null, colors);
            mesh.BoundingBox = boundingBox;
            return mesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64Label8"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        /// <param name="createMesh">Function for creating the <see cref="GpuMesh"/>.</param>
        public static GpuMesh GetMesh___Position_double__Color_float__Label_byte(Position_double__Color_float__Label_byte___Accessor ptAccessor, IPointCloudPoint[] points, bool doExchangeYZ, float3 translationVector, CreateGpuMesh createMesh)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var typedFirstPoint = (Position_double__Color_float__Label_byte)points[0];
            var firstPos = ptAccessor.GetPositionFloat3_64(ref typedFirstPoint);
            if (doExchangeYZ)
                ExchangeYZ(ref firstPos);

            var firstPosF = (float3)firstPos - translationVector;
            var vertices = new float3[numberOfPointsInMesh];
            var triangles = new ushort[numberOfPointsInMesh];
            var colors = new uint[numberOfPointsInMesh];
            var boundingBox = new AABBf(firstPosF, firstPosF);

            for (int i = 0; i < points.Length; i++)
            {
                var typedPoint = (Position_double__Color_float__Label_byte)points[i];
                var pos = ptAccessor.GetPositionFloat3_64(ref typedPoint);
                if (doExchangeYZ)
                    ExchangeYZ(ref pos);

                var posF = (float3)pos - translationVector;

                vertices[i] = posF;
                boundingBox |= vertices[i];

                triangles[i] = (ushort)i;
                var col = ptAccessor.GetColorFloat3_32(ref typedPoint);
                colors[i] = ColorToUInt((int)col.r, (int)col.g, (int)col.b, 255);

                //TODO: add labels correctly
                var label = ptAccessor.GetLabelUInt_8(ref typedPoint);
            }
            var mesh = createMesh(PrimitiveType.Points, vertices, triangles, null, colors);
            mesh.BoundingBox = boundingBox;
            return mesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64Nor32Col32IShort"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        /// <param name="createMesh">Function for creating the <see cref="GpuMesh"/>.</param>
        public static GpuMesh GetMeshPos64Nor32Col32IShort(Pos64Nor32Col32IShortAccessor ptAccessor, IPointCloudPoint[] points, bool doExchangeYZ, float3 translationVector, CreateGpuMesh createMesh)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var typedFirstPoint = (Pos64Nor32Col32IShort)points[0];
            var firstPos = ptAccessor.GetPositionFloat3_64(ref typedFirstPoint);
            if (doExchangeYZ)
                ExchangeYZ(ref firstPos);

            var firstPosF = (float3)firstPos - translationVector;
            var vertices = new float3[numberOfPointsInMesh];
            var normals = new float3[numberOfPointsInMesh];
            var triangles = new ushort[numberOfPointsInMesh];
            var colors = new uint[numberOfPointsInMesh];
            var colors1 = new uint[numberOfPointsInMesh];
            var boundingBox = new AABBf(firstPosF, firstPosF);

            for (int i = 0; i < points.Length; i++)
            {
                var typedPoint = (Pos64Nor32Col32IShort)points[i];
                var pos = ptAccessor.GetPositionFloat3_64(ref typedPoint);
                if (doExchangeYZ)
                    ExchangeYZ(ref pos);

                var posF = (float3)pos - translationVector;

                vertices[i] = posF;
                boundingBox |= vertices[i];
                triangles[i] = (ushort)i;

                var col = ptAccessor.GetColorFloat3_32(ref typedPoint);
                colors[i] = ColorToUInt((int)col.r / 256, (int)col.g / 256, (int)col.b / 256, 255);
                var intensity = (int)(ptAccessor.GetIntensityUInt_16(ref typedPoint) / 4096f * 256);
                colors1[i] = ColorToUInt(intensity, intensity, intensity, 255);
                normals[i] = ptAccessor.GetNormalFloat3_32(ref typedPoint);

            }
            var mesh = createMesh(PrimitiveType.Points, vertices, triangles, normals, colors, colors1);
            mesh.BoundingBox = boundingBox;
            return mesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64Nor32IShort"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        /// <param name="createMesh">Function for creating the <see cref="GpuMesh"/>.</param>
        public static GpuMesh GetMeshPos64Nor32IShort(Pos64Nor32IShortAccessor ptAccessor, IPointCloudPoint[] points, bool doExchangeYZ, float3 translationVector, CreateGpuMesh createMesh)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var typedFirstPoint = (Pos64Nor32IShort)points[0];
            var firstPos = ptAccessor.GetPositionFloat3_64(ref typedFirstPoint);
            if (doExchangeYZ)
                ExchangeYZ(ref firstPos);

            var firstPosF = (float3)firstPos - translationVector;
            var vertices = new float3[numberOfPointsInMesh];
            var normals = new float3[numberOfPointsInMesh];
            var triangles = new ushort[numberOfPointsInMesh];
            var colors = new uint[numberOfPointsInMesh];
            var boundingBox = new AABBf(firstPosF, firstPosF);

            for (int i = 0; i < points.Length; i++)
            {
                var typedPoint = (Pos64Nor32IShort)points[i];
                var pos = ptAccessor.GetPositionFloat3_64(ref typedPoint);
                if (doExchangeYZ)
                    ExchangeYZ(ref pos);

                var posF = (float3)pos - translationVector;

                vertices[i] = posF;
                boundingBox |= vertices[i];
                triangles[i] = (ushort)i;

                var intensity = (int)(ptAccessor.GetIntensityUInt_16(ref typedPoint) / 4096f * 256);
                colors[i] = ColorToUInt(intensity, intensity, intensity, 255);
                normals[i] = ptAccessor.GetNormalFloat3_32(ref typedPoint);

            }
            var mesh = createMesh(PrimitiveType.Points, vertices, triangles, normals, colors);
            mesh.BoundingBox = boundingBox;
            return mesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64Nor32Col32"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        /// <param name="createMesh">Function for creating the <see cref="GpuMesh"/>.</param>
        public static GpuMesh GetMeshPos64Nor32Col32(Pos64Nor32Col32Accessor ptAccessor, IPointCloudPoint[] points, bool doExchangeYZ, float3 translationVector, CreateGpuMesh createMesh)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var typedFirstPoint = (Pos64Nor32Col32)points[0];
            var firstPos = ptAccessor.GetPositionFloat3_64(ref typedFirstPoint);
            if (doExchangeYZ)
                ExchangeYZ(ref firstPos);

            var firstPosF = (float3)firstPos - translationVector;
            var vertices = new float3[numberOfPointsInMesh];
            var normals = new float3[numberOfPointsInMesh];
            var triangles = new ushort[numberOfPointsInMesh];
            var colors = new uint[numberOfPointsInMesh];
            var boundingBox = new AABBf(firstPosF, firstPosF);

            for (int i = 0; i < points.Length; i++)
            {
                var typedPoint = (Pos64Nor32Col32)points[i];
                var pos = ptAccessor.GetPositionFloat3_64(ref typedPoint);
                if (doExchangeYZ)
                    ExchangeYZ(ref pos);

                var posF = (float3)pos - translationVector;

                vertices[i] = posF;
                boundingBox |= vertices[i];
                triangles[i] = (ushort)i;

                var col = ptAccessor.GetColorFloat3_32(ref typedPoint);
                colors[i] = ColorToUInt((int)col.r / 256, (int)col.g / 256, (int)col.b / 256, 255);
                normals[i] = ptAccessor.GetNormalFloat3_32(ref typedPoint);

            }
            var mesh = createMesh(PrimitiveType.Points, vertices, triangles, normals, colors);
            mesh.BoundingBox = boundingBox;
            return mesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64Col32"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        /// <param name="createMesh">Function for creating the <see cref="GpuMesh"/>.</param>
        public static GpuMesh GetMeshPos64Col32(Pos64Col32Accessor ptAccessor, IPointCloudPoint[] points, bool doExchangeYZ, float3 translationVector, CreateGpuMesh createMesh)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var typedFirstPoint = (Pos64Col32)points[0];
            var firstPos = ptAccessor.GetPositionFloat3_64(ref typedFirstPoint);
            if (doExchangeYZ)
                ExchangeYZ(ref firstPos);

            var firstPosF = (float3)firstPos - translationVector;
            var vertices = new float3[numberOfPointsInMesh];
            var triangles = new ushort[numberOfPointsInMesh];
            var colors = new uint[numberOfPointsInMesh];
            var boundingBox = new AABBf(firstPosF, firstPosF);

            for (int i = 0; i < points.Length; i++)
            {
                var typedPoint = (Pos64Col32)points[i];
                var pos = ptAccessor.GetPositionFloat3_64(ref typedPoint);
                if (doExchangeYZ)
                    ExchangeYZ(ref pos);

                var posF = (float3)pos - translationVector;

                vertices[i] = posF;
                boundingBox |= vertices[i];

                triangles[i] = (ushort)i;
                var col = ptAccessor.GetColorFloat3_32(ref typedPoint);
                colors[i] = ColorToUInt((int)col.r / 256, (int)col.g / 256, (int)col.b / 256, 255);
            }
            var mesh = createMesh(PrimitiveType.Points, vertices, triangles, null, colors);
            mesh.BoundingBox = boundingBox;
            return mesh;
        }

        /// <summary>
        /// Exchanges the Y and Z values of a given position of type float3.
        /// </summary>
        /// <param name="pos">The position.</param>
        private static void ExchangeYZ(ref float3 pos)
        {
            var z = pos.z;
            var y = pos.y;
            pos.z = y;
            pos.y = z;
        }

        /// <summary>
        /// Exchanges the Y and Z values of a given position of type double3.
        /// </summary>
        /// <param name="pos">The position.</param>
        private static void ExchangeYZ(ref double3 pos)
        {
            var z = pos.z;
            var y = pos.y;
            pos.z = y;
            pos.y = z;
        }

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

        /// <summary>
        /// Returns a color for a given label/class (represented by an int).
        /// </summary>
        /// <param name="colors">The given colors.</param>
        /// <param name="numberOfLabels">The number of labels.</param>
        /// <param name="label">THe label.</param>
        /// <returns></returns>
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
    }
}