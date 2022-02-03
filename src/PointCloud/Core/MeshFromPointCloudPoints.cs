using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;

namespace Fusee.PointCloud.Core
{
    /// <summary>
    /// Provides static methods for creating meshes from lists of point cloud points.
    /// </summary>
    public static class MeshFromPointCloudPoints
    {
        /// <summary>
        /// Returns meshes for point clouds of type <see cref="PosD3"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        /// <param name="createMesh">Function for creating the <see cref="GpuMesh"/>.</param>
        public static GpuMesh GetMeshPosD3(PosD3Accessor ptAccessor, PosD3[] points, bool doExchangeYZ, float3 translationVector, CreateGpuMesh createMesh)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;
            var firstPos = ptAccessor.GetPositionFloat3_64(ref points[0]);

            if (doExchangeYZ)
                ExchangeYZ(ref firstPos);

            var firstPosF = (float3)firstPos - translationVector;
            var vertices = new float3[numberOfPointsInMesh];
            var triangles = new ushort[numberOfPointsInMesh];
            var boundingBox = new AABBf(firstPosF, firstPosF);

            for (int i = 0; i < points.Length; i++)
            {
                var pos = (float3)ptAccessor.GetPositionFloat3_64(ref points[i]);
                if (doExchangeYZ)
                    ExchangeYZ(ref pos);

                pos -= translationVector;

                vertices[i] = pos;
                boundingBox |= pos;
                triangles[i] = (ushort)i;
            }
            var mesh = createMesh(PrimitiveType.Points, vertices, triangles);
            mesh.BoundingBox = boundingBox;
            return mesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="PosD3ColF3InUs"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        /// <param name="createMesh">Function for creating the <see cref="GpuMesh"/>.</param>
        public static GpuMesh GetMeshPosD3ColF3InUs(PosD3ColF3InUsAccessor ptAccessor, PosD3ColF3InUs[] points, bool doExchangeYZ, float3 translationVector, CreateGpuMesh createMesh)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var firstPos = ptAccessor.GetPositionFloat3_64(ref points[0]);
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
                var pos = (float3)ptAccessor.GetPositionFloat3_64(ref points[i]);
                if (doExchangeYZ)
                    ExchangeYZ(ref pos);

                pos -= translationVector;

                vertices[i] = pos;
                boundingBox |= vertices[i];
                triangles[i] = (ushort)i;

                var col = ptAccessor.GetColorFloat3_32(ref points[i]);
                colors[i] = ColorToUInt((int)col.r / 256, (int)col.g / 256, (int)col.b / 256, 255);
                var intensity = (int)(ptAccessor.GetIntensityUInt_16(ref points[i]) / 4096f * 256);
                colors1[i] = ColorToUInt(intensity, intensity, intensity, 255);

            }
            var mesh = createMesh(PrimitiveType.Points, vertices, triangles, null, colors, colors1);
            mesh.BoundingBox = boundingBox;
            return mesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="PosD3ColF3InUs"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        /// <param name="createMesh">Function for creating the <see cref="GpuMesh"/>.</param>
        public static GpuMesh GetMeshPosD3ColF3InUsLblB(PosD3ColF3InUsLblBAccessor ptAccessor, PosD3ColF3InUsLblB[] points, bool doExchangeYZ, float3 translationVector, CreateGpuMesh createMesh)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var firstPos = ptAccessor.GetPositionFloat3_64(ref points[0]);
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
                var pos = (float3)ptAccessor.GetPositionFloat3_64(ref points[i]);
                if (doExchangeYZ)
                    ExchangeYZ(ref pos);

                pos -= translationVector;

                vertices[i] = pos;
                boundingBox |= vertices[i];
                triangles[i] = (ushort)i;

                var col = ptAccessor.GetColorFloat3_32(ref points[i]);
                colors[i] = ColorToUInt((int)col.r / 256, (int)col.g / 256, (int)col.b / 256, 255);
                var intensity = (int)(ptAccessor.GetIntensityUInt_16(ref points[i]) / 4096f * 256);
                colors1[i] = ColorToUInt(intensity, intensity, intensity, 255);

                //TODO: add labels correctly
                var label = ptAccessor.GetLabelUInt_8(ref points[i]);

            }
            var mesh = createMesh(PrimitiveType.Points, vertices, triangles, null, colors, colors1);
            mesh.BoundingBox = boundingBox;
            return mesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="PosD3InUs"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        /// <param name="createMesh">Function for creating the <see cref="GpuMesh"/>.</param>
        public static GpuMesh GetMeshPosD3InUs(PosD3InUsAccessor ptAccessor, PosD3InUs[] points, bool doExchangeYZ, float3 translationVector, CreateGpuMesh createMesh)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var firstPos = ptAccessor.GetPositionFloat3_64(ref points[0]);
            if (doExchangeYZ)
                ExchangeYZ(ref firstPos);

            var firstPosF = (float3)firstPos - translationVector;
            var vertices = new float3[numberOfPointsInMesh];
            var triangles = new ushort[numberOfPointsInMesh];
            var colors = new uint[numberOfPointsInMesh];
            var boundingBox = new AABBf(firstPosF, firstPosF);

            for (int i = 0; i < points.Length; i++)
            {
                var pos = (float3)ptAccessor.GetPositionFloat3_64(ref points[i]);
                if (doExchangeYZ)
                    ExchangeYZ(ref pos);

                pos -= translationVector;

                vertices[i] = pos;
                boundingBox |= vertices[i];
                triangles[i] = (ushort)i;
                var intensity = (int)(ptAccessor.GetIntensityUInt_16(ref points[i]) / 4096f * 256);
                colors[i] = ColorToUInt(intensity, intensity, intensity, 255);
            }
            var mesh = createMesh(PrimitiveType.Points, vertices, triangles, null, colors);
            mesh.BoundingBox = boundingBox;
            return mesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="PointCloud.Common.PosD3LblB"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        /// <param name="createMesh">Function for creating the <see cref="GpuMesh"/>.</param>
        public static GpuMesh GetMeshPosD3LblB(PosD3LblBAccessor ptAccessor, PosD3LblB[] points, bool doExchangeYZ, float3 translationVector, CreateGpuMesh createMesh)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var firstPos = ptAccessor.GetPositionFloat3_64(ref points[0]);
            if (doExchangeYZ)
                ExchangeYZ(ref firstPos);

            var firstPosF = (float3)firstPos - translationVector;
            var vertices = new float3[numberOfPointsInMesh];
            var triangles = new ushort[numberOfPointsInMesh];
            var colors = new uint[numberOfPointsInMesh];
            var boundingBox = new AABBf(firstPosF, firstPosF);

            for (int i = 0; i < points.Length; i++)
            {
                var pos = (float3)ptAccessor.GetPositionFloat3_64(ref points[i]);
                if (doExchangeYZ)
                    ExchangeYZ(ref pos);

                pos -= translationVector;

                vertices[i] = pos;
                boundingBox |= vertices[i];
                triangles[i] = (ushort)i;

                var col = ptAccessor.GetColorFloat3_32(ref points[i]);
                colors[i] = ColorToUInt((int)col.r / 256, (int)col.g / 256, (int)col.b / 256, 255);

                //TODO: add labels correctly
                var label = ptAccessor.GetLabelUInt_8(ref points[i]);

            }
            var mesh = createMesh(PrimitiveType.Points, vertices, triangles, null, colors);
            mesh.BoundingBox = boundingBox;
            return mesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="PointCloud.Common.PosD3LblB"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        /// <param name="createMesh">Function for creating the <see cref="GpuMesh"/>.</param>
        public static GpuMesh GetMeshPosD3ColF3LblB(PosD3ColF3LblBAccessor ptAccessor, PosD3ColF3LblB[] points, bool doExchangeYZ, float3 translationVector, CreateGpuMesh createMesh)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var firstPos = ptAccessor.GetPositionFloat3_64(ref points[0]);
            if (doExchangeYZ)
                ExchangeYZ(ref firstPos);

            var firstPosF = (float3)firstPos - translationVector;
            var vertices = new float3[numberOfPointsInMesh];
            var triangles = new ushort[numberOfPointsInMesh];
            var colors = new uint[numberOfPointsInMesh];
            var boundingBox = new AABBf(firstPosF, firstPosF);

            for (int i = 0; i < points.Length; i++)
            {
                var pos = (float3)ptAccessor.GetPositionFloat3_64(ref points[i]);
                if (doExchangeYZ)
                    ExchangeYZ(ref pos);

                pos -= translationVector;

                vertices[i] = pos;
                boundingBox |= vertices[i];

                triangles[i] = (ushort)i;
                var col = ptAccessor.GetColorFloat3_32(ref points[i]);
                colors[i] = ColorToUInt((int)col.r, (int)col.g, (int)col.b, 255);

                //TODO: add labels correctly
                var label = ptAccessor.GetLabelUInt_8(ref points[i]);
            }
            var mesh = createMesh(PrimitiveType.Points, vertices, triangles, null, colors);
            mesh.BoundingBox = boundingBox;
            return mesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="PointCloud.Common.PosD3NorF3ColF3InUs"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        /// <param name="createMesh">Function for creating the <see cref="GpuMesh"/>.</param>
        public static GpuMesh GetMeshPosD3NorF3ColF3InUs(PointCloud.Core.PosD3NorF3ColF3InUsAccessor ptAccessor, PosD3NorF3ColF3InUs[] points, bool doExchangeYZ, float3 translationVector, CreateGpuMesh createMesh)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var firstPos = ptAccessor.GetPositionFloat3_64(ref points[0]);
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
                var pos = (float3)ptAccessor.GetPositionFloat3_64(ref points[i]);
                if (doExchangeYZ)
                    ExchangeYZ(ref pos);

                pos -= translationVector;

                vertices[i] = pos;
                boundingBox |= vertices[i];
                triangles[i] = (ushort)i;

                var col = ptAccessor.GetColorFloat3_32(ref points[i]);
                colors[i] = ColorToUInt((int)col.r / 256, (int)col.g / 256, (int)col.b / 256, 255);
                var intensity = (int)(ptAccessor.GetIntensityUInt_16(ref points[i]) / 4096f * 256);
                colors1[i] = ColorToUInt(intensity, intensity, intensity, 255);
                normals[i] = ptAccessor.GetNormalFloat3_32(ref points[i]);

            }
            var mesh = createMesh(PrimitiveType.Points, vertices, triangles, normals, colors, colors1);
            mesh.BoundingBox = boundingBox;
            return mesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="PointCloud.Common.PosD3NorF3InUs"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        /// <param name="createMesh">Function for creating the <see cref="GpuMesh"/>.</param>
        public static GpuMesh GetMeshPosD3NorF3InUs(PosD3NorF3InUsAccessor ptAccessor, PosD3NorF3InUs[] points, bool doExchangeYZ, float3 translationVector, CreateGpuMesh createMesh)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var firstPos = ptAccessor.GetPositionFloat3_64(ref points[0]);
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
                var pos = (float3)ptAccessor.GetPositionFloat3_64(ref points[i]);
                if (doExchangeYZ)
                    ExchangeYZ(ref pos);

                pos -= translationVector;

                vertices[i] = pos;
                boundingBox |= vertices[i];
                triangles[i] = (ushort)i;

                var intensity = (int)(ptAccessor.GetIntensityUInt_16(ref points[i]) / 4096f * 256);
                colors[i] = ColorToUInt(intensity, intensity, intensity, 255);
                normals[i] = ptAccessor.GetNormalFloat3_32(ref points[i]);

            }
            var mesh = createMesh(PrimitiveType.Points, vertices, triangles, normals, colors);
            mesh.BoundingBox = boundingBox;
            return mesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="PosD3NorF3ColF3"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        /// <param name="createMesh">Function for creating the <see cref="GpuMesh"/>.</param>
        public static GpuMesh GetMeshPosD3NorF3ColF3(PosD3NorF3ColF3Accessor ptAccessor, PosD3NorF3ColF3[] points, bool doExchangeYZ, float3 translationVector, CreateGpuMesh createMesh)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var firstPos = ptAccessor.GetPositionFloat3_64(ref points[0]);
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
                var pos = (float3)ptAccessor.GetPositionFloat3_64(ref points[i]);
                if (doExchangeYZ)
                    ExchangeYZ(ref pos);

                pos -= translationVector;

                vertices[i] = pos;
                boundingBox |= vertices[i];
                triangles[i] = (ushort)i;

                var col = ptAccessor.GetColorFloat3_32(ref points[i]);
                colors[i] = ColorToUInt((int)col.r / 256, (int)col.g / 256, (int)col.b / 256, 255);
                normals[i] = ptAccessor.GetNormalFloat3_32(ref points[i]);

            }
            var mesh = createMesh(PrimitiveType.Points, vertices, triangles, normals, colors);
            mesh.BoundingBox = boundingBox;
            return mesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="PosD3ColF3"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        /// <param name="createMesh">Function for creating the <see cref="GpuMesh"/>.</param>
        public static GpuMesh GetMeshPosD3ColF3(PosD3ColF3Accessor ptAccessor, PosD3ColF3[] points, bool doExchangeYZ, float3 translationVector, CreateGpuMesh createMesh)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;
            var firstPos = ptAccessor.GetPositionFloat3_64(ref points[0]);
            if (doExchangeYZ)
                ExchangeYZ(ref firstPos);

            var firstPosF = (float3)firstPos - translationVector;
            var vertices = new float3[numberOfPointsInMesh];
            var triangles = new ushort[numberOfPointsInMesh];
            var colors = new uint[numberOfPointsInMesh];
            var boundingBox = new AABBf(firstPosF, firstPosF);

            for (int i = 0; i < points.Length; i++)
            {
                var pos = (float3)ptAccessor.GetPositionFloat3_64(ref points[i]);
                if (doExchangeYZ)
                    ExchangeYZ(ref pos);

                pos -= translationVector;

                vertices[i] = pos;
                boundingBox |= vertices[i];

                triangles[i] = (ushort)i;
                var col = ptAccessor.GetColorFloat3_32(ref points[i]);
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