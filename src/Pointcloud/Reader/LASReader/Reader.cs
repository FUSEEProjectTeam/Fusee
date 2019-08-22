using Fusee.Math.Core;
using Fusee.Pointcloud.Common;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Fusee.Pointcloud.Reader.LASReader
{
    public struct PointFormat : IPointFormat
    {
        public bool HasIntensity;
        public bool HasClassification;
        public bool HasUserData;
        public bool HasColor;
    }

    public struct MetaInfo : IMeta
    {
        public string Filename;

        public byte PointDataFormat;

        public long PointCnt;

        public double ScaleFactorX;
        public double ScaleFactorY;
        public double ScaleFactorZ;

        public double OffsetX;
        public double OffsetY;
        public double OffsetZ;
    }

    internal struct InternalHeader
    {
        public byte PointDataFormat;

        public long PointCnt;

        public double ScaleFactorX;
        public double ScaleFactorY;
        public double ScaleFactorZ;

        public double OffsetX;
        public double OffsetY;
        public double OffsetZ;
    }

    internal struct InternalPoint
    {
        public int X;
        public int Y;
        public int Z;

        public ushort Intensity;

        public byte Classification;
        public byte UserData;

        public ushort R;
        public ushort G;
        public ushort B;
    }

    /// <summary>
    ///     A reader for points in LAZ or LAS format
    /// </summary>
    public class LASPointReader : IDisposable, IPointReader
    {
        /// <summary>
        ///     The point format description (cast to PointFormat)
        /// </summary>
        public IPointFormat Format { get; }

        /// <summary>
        ///     The point format description (cast to MetaInfo)
        /// </summary>
        public IMeta MetaInfo { get; }
        
        private IntPtr _ptrToLASClass = new IntPtr();

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
        /// <param name="filename">The path to a las encoded file</param>
        public LASPointReader(string filename)
        {
            // Open file
            OpenLASFile(filename, ref _ptrToLASClass);

            if (_ptrToLASClass == IntPtr.Zero)
                throw new FileNotFoundException($"{filename} not found!");

            // Read header
            var header = new InternalHeader();

            GetHeader(_ptrToLASClass, ref header);

            MetaInfo = new MetaInfo
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
            Format = ParsePointDataByteFormatToFormatStruct((MetaInfo)MetaInfo);
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

            var currentPoint = new InternalPoint();
            GetPoint(_ptrToLASClass, ref currentPoint);

            var currentFormat = (PointFormat)Format;
            var currentHeader = (MetaInfo)MetaInfo;

            if (pa.HasPositionFloat3_64)
                pa.SetPositionFloat3_64(ref point, new double3(currentPoint.X * currentHeader.ScaleFactorX, currentPoint.Y * currentHeader.ScaleFactorY, currentPoint.Z * currentHeader.ScaleFactorZ));

            if (currentFormat.HasIntensity && pa.HasIntensityUInt_16)
                pa.SetIntensityUInt_16(ref point, currentPoint.Intensity);

            if (currentFormat.HasClassification && pa.HasLabelUInt_8)
                pa.SetLabelUInt_8(ref point, currentPoint.Classification);

            if (currentFormat.HasColor && pa.HasColorFloat3_32)
                pa.SetColorFloat3_32(ref point, new float3(currentPoint.R, currentPoint.G, currentPoint.B));

            return true;
        }

        PointFormat ParsePointDataByteFormatToFormatStruct(MetaInfo info)
        {
            switch(info.PointDataFormat)
            {
                default:
                    throw new ArgumentException($"Point data format with byte {info.PointDataFormat} not recognized!");
                case 0:
                    return new PointFormat
                    {
                        HasClassification = true,
                        HasColor = false,
                        HasIntensity = true,
                        HasUserData = false
                    };
                case 2:
                    return new PointFormat
                    {
                        HasClassification = true,
                        HasColor = true,
                        HasIntensity = true,
                        HasUserData = false
                    };
            }             
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

        ~LASPointReader()
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
        private static extern void GetHeader(IntPtr lasFileHandle, ref InternalHeader header);

        [DllImport("libLASlib", EntryPoint = "CS_ReadNextPoint")]
        private static extern void ReadNextPoint(IntPtr lasFileHandle, ref bool nextPoint);

        [DllImport("libLASlib", EntryPoint = "CS_GetPoint")]
        private static extern void GetPoint(IntPtr lasFileHandle, ref InternalPoint csPoint);

        [DllImport("libLASlib", EntryPoint = "CS_Delete")]
        private static extern void Delete(ref IntPtr lasFileHandle);
    }
}
