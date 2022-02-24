using Fusee.Base.Imp.Desktop;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Common.Accessors;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Fusee.PointCloud.Las.Desktop
{
    /// <summary>
    /// A reader for points in LAZ or LAS format
    /// </summary>
    public class LasPointReader : IDisposable, IPointReader
    {
        public IPointAccessor PointAccessor { get; private set; }

        /// <summary>
        /// The point cloud meta data, usually stored in the header of the las file.
        /// </summary>
        public LasMetaInfo MetaInfo { get; private set; }

        private IntPtr _ptrToLASClass = new();
        private string _filename;

        public IPointCloud GetPointCloudComponent(string filename)
        {
            _filename = filename;
            OpenFile(_filename);
            throw new NotImplementedException();
        }

        public IPointCloudOctree GetOctree()
        {
            //convert to potree octree
            throw new NotImplementedException();
        }

        public TPoint[] LoadNodeData<TPoint>(string id) where TPoint : new()
        {
            throw new NotImplementedException();
        }

        private void OpenFile(string filename)
        {
            EmbeddedResourcesDllHandler.LoadEmbeddedDll("libLASlib.dll", "Fusee.PointCloud.Las.Desktop.Natives.libLASlib.dll");

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
                PointCount = header.PointCnt,
                PointDataFormat = header.PointDataFormat,
                ScaleFactorX = header.ScaleFactorX,
                ScaleFactorY = header.ScaleFactorY,
                ScaleFactorZ = header.ScaleFactorZ
            };
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

        /// <summary>
        /// Reads the given amount of points from stream
        /// </summary>
        /// <param name="n"></param>
        /// <param name="pa"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public TPoint[] ReadNPoints<TPoint>(int n, IPointAccessor pa) where TPoint : new()
        {
            if (_ptrToLASClass == IntPtr.Zero)
                throw new FileNotFoundException("No file was specified yet. Call 'OpenFile' first");
            var points = new TPoint[n];
            for (var i = 0; i < points.Length; i++)
            {
                if (!ReadNextPoint(ref points[i], pa)) break;
            }
            return points;
        }

        /// <summary>
        /// Reads the next point and writes it to the given point
        /// </summary>
        /// <param name="point"></param>
        /// <param name="pa"></param>
        /// <returns></returns>
        public bool ReadNextPoint<TPoint>(ref TPoint point, IPointAccessor pa) where TPoint : new()
        {
            if (point == null)
                throw new ArgumentOutOfRangeException("No writable point found!");

            var hasNextPoint = true;
            ReadNextPoint(_ptrToLASClass, ref hasNextPoint);
            if (!hasNextPoint) return false;

            var currentPoint = new LasInternalPoint();
            GetPoint(_ptrToLASClass, ref currentPoint);

            var typedAccessor = (PointAccessor<TPoint>)pa;
            
            if ((MetaInfo.PointDataFormat == 2 || MetaInfo.PointDataFormat == 3) && typedAccessor.ColorType == PointColorType.Float3)
                typedAccessor.SetColorFloat3_32(ref point, new float3(currentPoint.R, currentPoint.G, currentPoint.B));

            //TODO: Complete
            //if (typedAccessor.PositionType == PointPositionType.Double3)
            // -> always the case right now
            typedAccessor.SetPositionFloat3_64(ref point, new double3(currentPoint.X * MetaInfo.ScaleFactorX, currentPoint.Y * MetaInfo.ScaleFactorY, currentPoint.Z * MetaInfo.ScaleFactorZ));

            //if (currentFormat.HasIntensity && typedAccessor.IntensityType == PointIntensityType.UInt_16)
            // -> always true right now!
            typedAccessor.SetIntensityUInt_16(ref point, currentPoint.Intensity);

            // -> never the case right now!
            //if (currentFormat.HasClassification && typedAccessor.LabelType == PointLabelType.UInt_8)
            //{
            //    //TODO: HACK!! label was somehow written to UserData and not to classification
            //    if (currentPoint.Classification != 0)
            //        typedAccessor.SetLabelUInt_8(ref point, currentPoint.Classification);
            //    else
            //        typedAccessor.SetLabelUInt_8(ref point, currentPoint.UserData);
            //}

            return true;
        }

        public PointType GetPointType()
        {
            //TODO: Complete
            switch (MetaInfo.PointDataFormat)
            {
                case 0:
                case 1:
                    return PointType.PosD3InUs;
                case 2:
                case 3:
                    return PointType.PosD3ColF3InUs;
                default:
                    throw new ArgumentException($"Point data format with byte {MetaInfo.PointDataFormat} not recognized!");
            }
        }

        public Task<TPoint[]> LoadPointsForNodeAsync<TPoint>(string guid, IPointAccessor pointAccessor) where TPoint : new()
        {
            throw new NotImplementedException();
        }

        public TPoint[] LoadNodeData<TPoint>(string id, IPointAccessor pointAccessor) where TPoint : new()
        {
            throw new NotImplementedException();
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