using Fusee.Base.Imp.Desktop;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Fusee.PointCloud.FileReader.LasReader
{
    /// <summary>
    /// A reader for points in LAZ or LAS format
    /// </summary>
    public class LasPointReader : IDisposable, IPointReader
    {
        /// <summary>
        /// The point format description.
        /// </summary>
        public IPointFormat Format { get; private set; }

        /// <summary>
        /// The point cloud meta data, usually stored in the header of the las file.
        /// </summary>
        public IPointCloudMetaInfo MetaInfo { get; private set; }

        private IntPtr _ptrToLASClass = new();

        public void OpenFile(string filename)
        {
            EmbeddedResourcesDllHandler.LoadEmbeddedDll("libLASlib.dll", "Fusee.PointCloud.FileReader.LasReader.Natives.libLASlib.dll");

            // Open file
            OpenLASFile(filename, ref _ptrToLASClass);

            if (_ptrToLASClass == IntPtr.Zero)
                throw new FileNotFoundException($"{filename} not found!");

            // Read header
            var header = new LasInternalHeader();

            GetHeader(_ptrToLASClass, ref header);

            MetaInfo = new LasMetaInfo
            {
                Filename = filename,
                OffsetX = header.OffsetX,
                OffsetY = header.OffsetY,
                OffsetZ = header.OffsetZ,
                PointCnt = header.PointCnt,
                PointDataFormat = header.PointDataFormat,
                ScaleFactorX = header.ScaleFactorX,
                ScaleFactorY = header.ScaleFactorY,
                ScaleFactorZ = header.ScaleFactorZ
            };

            // Set format
            Format = ParsePointDataByteFormatToFormatStruct((LasMetaInfo)MetaInfo);
        }

        /// <summary>
        ///     A LASPointReader can open point files encoded by the las format v. 1.4 with the following extensions:
        ///     - *.asc
        ///     - *.bil
        ///     - *.bin
        ///     - *.dtm
        ///     - *.las
        ///     - *.ply
        ///     - *.qfit
        ///     - *.shp
        ///     - *.txt
        ///     - *.laz
        /// </summary>
        /// <param name="filename">The path to a las encoded file.</param>
        public LasPointReader(string filename)
        {
            OpenFile(filename);
        }

        /// <summary>
        ///     A LASPointReader can open point files encoded by the las format v. 1.4 with the following extensions:
        ///     - *.asc
        ///     - *.bil
        ///     - *.bin
        ///     - *.dtm
        ///     - *.las
        ///     - *.ply
        ///     - *.qfit
        ///     - *.shp
        ///     - *.txt
        ///     - *.laz
        /// </summary>
        public LasPointReader() { }

        public TPoint[] ReadNPoints<TPoint>(int n, PointAccessor<TPoint> pa)
        {
            if (_ptrToLASClass == IntPtr.Zero)
                throw new FileNotFoundException("No file was specified yet. Call 'OpenFile' first");
            var points = new TPoint[n];
            for (var i = 0; i < points.Length; i++)
                if (!ReadNextPoint(ref points[i], pa)) break;
            return points;
        }

        /// <summary>
        ///     Reads the next point and writes it to the given point
        /// </summary>
        /// <typeparam name="TPoint"></typeparam>
        /// <param name="point"></param>
        /// <param name="pa"></param>
        /// <returns></returns>
        public bool ReadNextPoint<TPoint>(ref TPoint point, PointAccessor<TPoint> pa)
        {
            if (point == null)
                throw new ArgumentOutOfRangeException("No writable point found!");

            var hasNextPoint = true;
            ReadNextPoint(_ptrToLASClass, ref hasNextPoint);
            if (!hasNextPoint) return false;

            var currentPoint = new LasInternalPoint();
            GetPoint(_ptrToLASClass, ref currentPoint);

            var currentFormat = (LasPointFormat)Format;
            var currentHeader = (LasMetaInfo)MetaInfo;

            if (pa.HasPositionFloat3_64)
                pa.SetPositionFloat3_64(ref point, new double3(currentPoint.X * currentHeader.ScaleFactorX, currentPoint.Y * currentHeader.ScaleFactorY, currentPoint.Z * currentHeader.ScaleFactorZ));

            if (currentFormat.HasIntensity && pa.HasIntensityUInt_16)
                pa.SetIntensityUInt_16(ref point, currentPoint.Intensity);

            if (currentFormat.HasClassification && pa.HasLabelUInt_8)
            {
                //TODO: HACK!! label was somehow written to UserData and not to classification
                if (currentPoint.Classification != 0)
                    pa.SetLabelUInt_8(ref point, currentPoint.Classification);
                else
                    pa.SetLabelUInt_8(ref point, currentPoint.UserData);
            }

            if (currentFormat.HasColor && pa.HasColorFloat3_32)
                pa.SetColorFloat3_32(ref point, new float3(currentPoint.R, currentPoint.G, currentPoint.B));

            return true;
        }

        LasPointFormat ParsePointDataByteFormatToFormatStruct(LasMetaInfo info)
        {
            return info.PointDataFormat switch
            {
                0 => new LasPointFormat
                {
                    HasClassification = true,
                    HasColor = false,
                    HasIntensity = true,
                    HasUserData = false
                },
                2 or 3 => new LasPointFormat
                {
                    HasClassification = true,
                    HasColor = true,
                    HasIntensity = true,
                    HasUserData = false
                },
                _ => throw new ArgumentException($"Point data format with byte {info.PointDataFormat} not recognized!"),
            };
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls    

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                Delete(ref _ptrToLASClass);
                _ptrToLASClass = IntPtr.Zero;

                disposedValue = true;
            }
        }

        ~LasPointReader()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        [DllImport("libLASlib", EntryPoint = "CS_OpenLasFile")]
        private static extern IntPtr OpenLASFile(string filename, ref IntPtr lasFileHandle);

        [DllImport("libLASlib", EntryPoint = "CS_GetHeader")]
        private static extern void GetHeader(IntPtr lasFileHandle, ref LasInternalHeader header);

        [DllImport("libLASlib", EntryPoint = "CS_ReadNextPoint")]
        private static extern void ReadNextPoint(IntPtr lasFileHandle, ref bool nextPoint);

        [DllImport("libLASlib", EntryPoint = "CS_GetPoint")]
        private static extern void GetPoint(IntPtr lasFileHandle, ref LasInternalPoint csPoint);

        [DllImport("libLASlib", EntryPoint = "CS_Delete")]
        private static extern void Delete(ref IntPtr lasFileHandle);
    }
}