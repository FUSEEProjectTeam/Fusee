using Fusee.Engine.Common;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;

namespace Fusee.PointCloud.Core
{
    /// <summary>
    /// Provides static methods for creating meshes from lists of point cloud points.
    /// </summary>
    public static class MeshFromPoints
    {
        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        public static Mesh GetMeshPos64<TPoint>(PointAccessor<TPoint> ptAccessor, TPoint[] points, bool doExchangeYZ, float3 translationVector)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var firstPos = ptAccessor.GetPositionFloat3_64(ref points[0]);
            if (doExchangeYZ)
                FuseePointCloudHelper.ExchangeYZ(ref firstPos);

            var firstPosF = (float3)firstPos - translationVector;

            Mesh currentMesh = new()
            {
                Vertices = new float3[numberOfPointsInMesh],
                Triangles = new ushort[numberOfPointsInMesh],
                Colors = new uint[numberOfPointsInMesh],
                MeshType = (int)OpenGLPrimitiveType.Points,
                BoundingBox = new AABBf(firstPosF, firstPosF)
            };

            for (int i = 0; i < points.Length; i++)
            {
                var pos = ptAccessor.GetPositionFloat3_64(ref points[i]);
                if (doExchangeYZ)
                    FuseePointCloudHelper.ExchangeYZ(ref pos);

                var posF = (float3)pos - translationVector;

                currentMesh.Vertices[i] = posF;
                currentMesh.BoundingBox |= currentMesh.Vertices[i];

                currentMesh.Triangles[i] = (ushort)i;

            }
            return currentMesh;
        }


        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64Col32IShort"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        public static Mesh GetMeshPos64Col32IShort<TPoint>(PointAccessor<TPoint> ptAccessor, TPoint[] points, bool doExchangeYZ, float3 translationVector)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var firstPos = ptAccessor.GetPositionFloat3_64(ref points[0]);
            if (doExchangeYZ)
                FuseePointCloudHelper.ExchangeYZ(ref firstPos);

            var firstPosF = (float3)firstPos - translationVector;

            Mesh currentMesh = new()
            {
                Vertices = new float3[numberOfPointsInMesh],
                Triangles = new ushort[numberOfPointsInMesh],
                Colors = new uint[numberOfPointsInMesh],
                MeshType = (int)OpenGLPrimitiveType.Points,
                BoundingBox = new AABBf(firstPosF, firstPosF)
            };

            for (int i = 0; i < points.Length; i++)
            {
                var pos = ptAccessor.GetPositionFloat3_64(ref points[i]);
                if (doExchangeYZ)
                    FuseePointCloudHelper.ExchangeYZ(ref pos);

                var posF = (float3)pos - translationVector;

                currentMesh.Vertices[i] = posF;
                currentMesh.BoundingBox |= currentMesh.Vertices[i];
                currentMesh.Triangles[i] = (ushort)i;

                var col = ptAccessor.GetColorFloat3_32(ref points[i]);
                currentMesh.Colors[i] = FuseePointCloudHelper.ColorToUInt((int)col.r / 256, (int)col.g / 256, (int)col.b / 256, 255);
                var intensity = (int)(ptAccessor.GetIntensityUInt_16(ref points[i]) / 4096f * 256);
                currentMesh.Colors1[i] = FuseePointCloudHelper.ColorToUInt(intensity, intensity, intensity, 255);

            }
            return currentMesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64IShort"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        public static Mesh GetMeshPos64IShort<TPoint>(PointAccessor<TPoint> ptAccessor, TPoint[] points, bool doExchangeYZ, float3 translationVector)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var firstPos = ptAccessor.GetPositionFloat3_64(ref points[0]);
            if (doExchangeYZ)
                FuseePointCloudHelper.ExchangeYZ(ref firstPos);

            var firstPosF = (float3)firstPos - translationVector;

            Mesh currentMesh = new()
            {
                Vertices = new float3[numberOfPointsInMesh],
                Triangles = new ushort[numberOfPointsInMesh],
                Colors = new uint[numberOfPointsInMesh],
                MeshType = (int)OpenGLPrimitiveType.Points,
                BoundingBox = new AABBf(firstPosF, firstPosF)
            };

            for (int i = 0; i < points.Length; i++)
            {
                var pos = ptAccessor.GetPositionFloat3_64(ref points[i]);
                if (doExchangeYZ)
                    FuseePointCloudHelper.ExchangeYZ(ref pos);

                var posF = (float3)pos - translationVector;

                currentMesh.Vertices[i] = posF;
                currentMesh.BoundingBox |= currentMesh.Vertices[i];
                currentMesh.Triangles[i] = (ushort)i;
                var intensity = (int)(ptAccessor.GetIntensityUInt_16(ref points[i]) / 4096f * 256);
                currentMesh.Colors[i] = FuseePointCloudHelper.ColorToUInt(intensity, intensity, intensity, 255);
            }
            return currentMesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64Label8"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        public static Mesh GetMeshPos64Label8<TPoint>(PointAccessor<TPoint> ptAccessor, TPoint[] points, bool doExchangeYZ, float3 translationVector)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var firstPos = ptAccessor.GetPositionFloat3_64(ref points[0]);
            if (doExchangeYZ)
                FuseePointCloudHelper.ExchangeYZ(ref firstPos);

            var firstPosF = (float3)firstPos - translationVector;

            Mesh currentMesh = new()
            {
                Vertices = new float3[numberOfPointsInMesh],
                Triangles = new ushort[numberOfPointsInMesh],
                Colors = new uint[numberOfPointsInMesh],
                MeshType = (int)OpenGLPrimitiveType.Points,
                BoundingBox = new AABBf(firstPosF, firstPosF)
            };

            for (int i = 0; i < points.Length; i++)
            {
                var pos = ptAccessor.GetPositionFloat3_64(ref points[i]);
                if (doExchangeYZ)
                    FuseePointCloudHelper.ExchangeYZ(ref pos);

                var posF = (float3)pos - translationVector;

                currentMesh.Vertices[i] = posF;
                currentMesh.BoundingBox |= currentMesh.Vertices[i];
                currentMesh.Triangles[i] = (ushort)i;

                var lbl = ptAccessor.GetLabelUInt_8(ref points[i]);
                var colorScale = new float3[4] { new float3(0, 0, 1), new float3(0, 1, 0), new float3(1, 1, 0), new float3(1, 0, 0) };
                currentMesh.Colors[i] = FuseePointCloudHelper.ColorToUint(FuseePointCloudHelper.LabelToColor(colorScale, 32, lbl));

            }
            return currentMesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64Nor32Col32IShort"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        public static Mesh GetMeshPos64Nor32Col32IShort<TPoint>(PointAccessor<TPoint> ptAccessor, TPoint[] points, bool doExchangeYZ, float3 translationVector)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var firstPos = ptAccessor.GetPositionFloat3_64(ref points[0]);
            if (doExchangeYZ)
                FuseePointCloudHelper.ExchangeYZ(ref firstPos);

            var firstPosF = (float3)firstPos - translationVector;

            Mesh currentMesh = new()
            {
                Vertices = new float3[numberOfPointsInMesh],
                Triangles = new ushort[numberOfPointsInMesh],
                Colors = new uint[numberOfPointsInMesh],
                MeshType = (int)OpenGLPrimitiveType.Points,
                BoundingBox = new AABBf(firstPosF, firstPosF)
            };

            for (int i = 0; i < points.Length; i++)
            {
                var pos = ptAccessor.GetPositionFloat3_64(ref points[i]);
                if (doExchangeYZ)
                    FuseePointCloudHelper.ExchangeYZ(ref pos);

                var posF = (float3)pos - translationVector;

                currentMesh.Vertices[i] = posF;
                currentMesh.BoundingBox |= currentMesh.Vertices[i];
                currentMesh.Triangles[i] = (ushort)i;

                var col = ptAccessor.GetColorFloat3_32(ref points[i]);
                currentMesh.Colors[i] = FuseePointCloudHelper.ColorToUInt((int)col.r / 256, (int)col.g / 256, (int)col.b / 256, 255);
                var intensity = (int)(ptAccessor.GetIntensityUInt_16(ref points[i]) / 4096f * 256);
                currentMesh.Colors1[i] = FuseePointCloudHelper.ColorToUInt(intensity, intensity, intensity, 255);
                currentMesh.Normals[i] = ptAccessor.GetNormalFloat3_32(ref points[i]);

            }
            return currentMesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64Nor32IShort"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        public static Mesh GetMeshPos64Nor32IShort<TPoint>(PointAccessor<TPoint> ptAccessor, TPoint[] points, bool doExchangeYZ, float3 translationVector)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var firstPos = ptAccessor.GetPositionFloat3_64(ref points[0]);
            if (doExchangeYZ)
                FuseePointCloudHelper.ExchangeYZ(ref firstPos);

            var firstPosF = (float3)firstPos - translationVector;

            Mesh currentMesh = new()
            {
                Vertices = new float3[numberOfPointsInMesh],
                Triangles = new ushort[numberOfPointsInMesh],
                Colors = new uint[numberOfPointsInMesh],
                MeshType = (int)OpenGLPrimitiveType.Points,
                BoundingBox = new AABBf(firstPosF, firstPosF)
            };

            for (int i = 0; i < points.Length; i++)
            {
                var pos = ptAccessor.GetPositionFloat3_64(ref points[i]);
                if (doExchangeYZ)
                    FuseePointCloudHelper.ExchangeYZ(ref pos);

                var posF = (float3)pos - translationVector;

                currentMesh.Vertices[i] = posF;
                currentMesh.BoundingBox |= currentMesh.Vertices[i];
                currentMesh.Triangles[i] = (ushort)i;

                var intensity = (int)(ptAccessor.GetIntensityUInt_16(ref points[i]) / 4096f * 256);
                currentMesh.Colors[i] = FuseePointCloudHelper.ColorToUInt(intensity, intensity, intensity, 255);
                currentMesh.Normals[i] = ptAccessor.GetNormalFloat3_32(ref points[i]);

            }
            return currentMesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64Nor32Col32"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        public static Mesh GetMeshPos64Nor32Col32<TPoint>(PointAccessor<TPoint> ptAccessor, TPoint[] points, bool doExchangeYZ, float3 translationVector)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var firstPos = ptAccessor.GetPositionFloat3_64(ref points[0]);
            if (doExchangeYZ)
                FuseePointCloudHelper.ExchangeYZ(ref firstPos);

            var firstPosF = (float3)firstPos - translationVector;

            Mesh currentMesh = new()
            {
                Vertices = new float3[numberOfPointsInMesh],
                Triangles = new ushort[numberOfPointsInMesh],
                Colors = new uint[numberOfPointsInMesh],
                MeshType = (int)OpenGLPrimitiveType.Points,
                BoundingBox = new AABBf(firstPosF, firstPosF)
            };

            for (int i = 0; i < points.Length; i++)
            {
                var pos = ptAccessor.GetPositionFloat3_64(ref points[i]);
                if (doExchangeYZ)
                    FuseePointCloudHelper.ExchangeYZ(ref pos);

                var posF = (float3)pos - translationVector;

                currentMesh.Vertices[i] = posF;
                currentMesh.BoundingBox |= currentMesh.Vertices[i];
                currentMesh.Triangles[i] = (ushort)i;

                var col = ptAccessor.GetColorFloat3_32(ref points[i]);
                currentMesh.Colors[i] = FuseePointCloudHelper.ColorToUInt((int)col.r / 256, (int)col.g / 256, (int)col.b / 256, 255);
                currentMesh.Normals[i] = ptAccessor.GetNormalFloat3_32(ref points[i]);

            }
            return currentMesh;
        }

        /// <summary>
        /// Returns meshes for point clouds of type <see cref="Pos64Col32"/>.
        /// </summary>
        /// <param name="ptAccessor">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="points">The lists of "raw" points.</param>
        /// <param name="doExchangeYZ">Determines if the y and z coordinates of the points need to be exchanged.</param>
        /// <param name="translationVector">Vector that will be subtracted from each point coordinate.</param>
        public static Mesh GetMeshPos64Col32<TPoint>(PointAccessor<TPoint> ptAccessor, TPoint[] points, bool doExchangeYZ, float3 translationVector)
        {
            int numberOfPointsInMesh;
            numberOfPointsInMesh = points.Length;

            var firstPos = ptAccessor.GetPositionFloat3_64(ref points[0]);
            if (doExchangeYZ)
                FuseePointCloudHelper.ExchangeYZ(ref firstPos);

            var firstPosF = (float3)firstPos - translationVector;

            Mesh currentMesh = new()
            {
                Vertices = new float3[numberOfPointsInMesh],
                Triangles = new ushort[numberOfPointsInMesh],
                Colors = new uint[numberOfPointsInMesh],
                MeshType = (int)OpenGLPrimitiveType.Points,
                BoundingBox = new AABBf(firstPosF, firstPosF)
            };

            for (int i = 0; i < points.Length; i++)
            {
                var pos = ptAccessor.GetPositionFloat3_64(ref points[i]);
                if (doExchangeYZ)
                    FuseePointCloudHelper.ExchangeYZ(ref pos);

                var posF = (float3)pos - translationVector;

                currentMesh.Vertices[i] = posF;
                currentMesh.BoundingBox |= currentMesh.Vertices[i];

                currentMesh.Triangles[i] = (ushort)i;
                var col = ptAccessor.GetColorFloat3_32(ref points[i]);
                currentMesh.Colors[i] = FuseePointCloudHelper.ColorToUInt((int)col.r / 256, (int)col.g / 256, (int)col.b / 256, 255);
            }
            return currentMesh;
        }

    }
}