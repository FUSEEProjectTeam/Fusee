using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance;
using Fusee.Base.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Potree.V2.Data;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.PointCloud.Potree
{

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct LASHeader
    {
        public LASHeader() { }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        internal char[] FileSignature = new char[] { 'L', 'A', 'S', 'F' };

        internal ushort FileSourceID = 0;
        internal ushort GlobalEncoding = 0;

        internal uint GUIDData1 = 0;
        internal ushort GUIDData2 = 0;
        internal ushort GUIDData3 = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        internal byte[] GUIData4 = new byte[8];

        internal byte VersionMajor = 1;
        internal byte VersionMinor = 4;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        internal byte[] SystemIdentifier = new byte[32];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        internal byte[] GeneratingSoftware = new byte[32];

        internal ushort FileCreationDayOfYear = (ushort)DateTime.Now.Day;
        internal ushort FileCreationYear = (ushort)DateTime.Now.Year;
        internal ushort HeaderSize = 375;
        internal uint OffsetToPointData = 375;
        internal uint NumberOfVariableLengthRecords = 0;
        internal byte PointDataRecordFormat = 2;
        internal ushort PointDataRecordLength = 26;
        internal uint LegacyNbrOfPoints = 0;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        internal uint[] LeacyNbrOfPointsByRtn = new uint[5];

        internal double ScaleFactorX = 0;
        internal double ScaleFactorY = 0;
        internal double ScaleFactorZ = 0;

        internal double OffsetX = 0;
        internal double OffsetY = 0;
        internal double OffsetZ = 0;

        internal double MaxX = 0;
        internal double MinX = 0;

        internal double MaxY = 0;
        internal double MinY = 0;

        internal double MaxZ = 0;
        internal double MinZ = 0;

        internal ulong StartOfWaveformPacket = 0;
        internal ulong StartOfFirstExtendend = 0;
        internal uint NbrOfExtendedVariableLength = 0;

        internal ulong NumberOfPtRecords = 0;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
        internal ulong[] NbrOfPointsByReturn = new ulong[15];
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct LASPoint
    {
        public LASPoint() { }

        internal uint X = 0;
        internal uint Y = 0;
        internal uint Z = 0;

        internal ushort Intensity = 0;
        internal byte ReturnNbrOfScanDirAndEdgeByte = 0;

        internal byte Classification = 0;
        internal byte ScanAngleRank = 0;
        internal byte UserData = 0;

        internal ushort PtSrcID = 0;

        internal ushort R = 0;
        internal ushort G = 0;
        internal ushort B = 0;
    }

    /// <summary>
    /// This class provides methods to convert and saves <typeparamref name="PotreePoint"/> clouds to LAS 1.4
    /// </summary>
    public class Potree2LAS<PotreePoint> : IPointWriter<PotreePoint>, IDisposable where PotreePoint : struct
    {
        /// <summary>
        /// Returns the point type.
        /// </summary>
        public PointType PointType => PointType.Raw;

        /// <summary>
        /// The path the file is being saved to
        /// </summary>
        public FileInfo SavePath => _savePath;

        /// <summary>
        /// The necessary metadata information
        /// </summary>
        public IPointWriterMetadata Metadata => _metadata;

        private readonly FileInfo _savePath;
        private readonly Stream _fileStream;
        private readonly IPointWriterMetadata _metadata;

        private bool disposedValue;

        // these information are filled during the first write call
        private LASHeader _header;

        /// <summary>
        /// Generate a writer instance.
        /// Use the write methods to fill the file with points.
        /// </summary>
        /// <param name="savePath">Path to save the file (make sure the extension is .las!)</param>
        /// <param name="metadata">The metadata needed for the header (offset, scale)</param>
        public Potree2LAS(FileInfo savePath/*, IPointWriterMetadata metadata*/)
        {
            Guard.IsNotNull(savePath);
            //Guard.IsNotNull(metadata);
            Guard.IsTrue(savePath.Extension == ".las");
            if (savePath.Exists)
            {
                Diagnostics.Warn($"{savePath.FullName} does already exists. Overwriting ...");
            }

            _savePath = savePath;
            _fileStream = _savePath.OpenWrite();
            //_metadata = metadata;
            ParseAndFillHeader();
        }

        private void ParseAndFillHeader()
        {
            var doy = DateTime.Now.DayOfYear;
            var year = DateTime.Now.Year;
            var size = Marshal.SizeOf<PotreePoint>();

            // make sure we can cast to <see cref="ushort"/> and do not lose information
            Guard.IsLessThan(doy, ushort.MaxValue);
            Guard.IsLessThan(year, ushort.MaxValue);
            Guard.IsLessThan(size, ushort.MaxValue);

            _header = new LASHeader
            {
                PointDataRecordLength = (ushort)size,
                FileCreationDayOfYear = (ushort)doy,
                FileCreationYear = (ushort)year,
                //MaxX = _metadata.BoundingBox.max.x,
                //MaxY = _metadata.BoundingBox.max.y,
                //MaxZ = _metadata.BoundingBox.max.z,
                //MinX = _metadata.BoundingBox.min.x,
                //MinY = _metadata.BoundingBox.min.y,
                //MinZ = _metadata.BoundingBox.min.z,
                //OffsetX = _metadata.Offset.x,
                //OffsetY = _metadata.Offset.y,
                //OffsetZ = _metadata.Offset.z,
                //ScaleFactorX = _metadata.Scale.x,
                //ScaleFactorY = _metadata.Scale.y,
                //ScaleFactorZ = _metadata.Scale.z
            };

            var generatingSoftware = Encoding.UTF8.GetBytes($"POLAR v.{Assembly.GetExecutingAssembly().GetName().Version}");
            Guard.IsLessThan(generatingSoftware.Length, _header.GeneratingSoftware.Length);
            Array.Copy(generatingSoftware, _header.GeneratingSoftware, generatingSoftware.Length);

            // Initialize unmanged memory to hold the struct.
            var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<LASHeader>());
            try
            {
                // Copy the struct to unmanaged memory.
                Marshal.StructureToPtr(_header, ptr, false);
                var dest = new byte[Marshal.SizeOf<LASHeader>()];
                Marshal.Copy(ptr, dest, 0, Marshal.SizeOf<LASHeader>());
                _fileStream.Write(dest);

            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>
        /// This methods takes a list <see cref="PointType"/>s and converts it to the desired output format and appends it to the file
        /// to disk at given <see cref="SavePath"/>
        /// </summary>
        /// <param name="points">The point data as <see cref="ReadOnlySpan{T}"/></param>
        public void WritePointcloudPoints(ReadOnlySpan<PotreePoint> points)
        {
            Guard.IsNotEmpty(points);
        }

        /// <summary>
        /// This methods takes a list <see cref="PointType"/>s and converts it to the desired output format and appends it to the file
        /// to disk at given <see cref="SavePath"/> in an <see langword="async"/> manner.
        /// </summary>
        /// <param name="points">The point data as <see cref="ReadOnlySpan{T}"/></param>
        public Task WritePointcloudPointsAsync(ReadOnlyMemory<PotreePoint> points)
        {
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // close the file stream
                    _fileStream.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Potree2LAS()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        // old code, replace
        //private static void WritePotree2LAS(IEnumerable<PotreePoint> points, PotreeMetadata metadata, FileInfo savePath)
        //{
        //    Guard.IsNotNull(savePath);
        //    Guard.IsNotNull(points);
        //    Guard.IsTrue(savePath.Extension == ".las");

        //    Guard.IsEqualTo(Marshal.SizeOf<LASPoint>(), 26);
        //    Guard.IsEqualTo(Marshal.SizeOf<LASHeader>(), 375);

        //    if (savePath.Exists)
        //        Diagnostics.Warn($"File {savePath.FullName} does exist, overwriting ...");

        //    var scaleFactor = metadata.Scale;

        //    const float maxColorValuePotree = byte.MaxValue;
        //    const short maxIntensityValuePotree = short.MaxValue;
        //    const ushort maxColorAndIntensityValueLAS = ushort.MaxValue;

        //    var invFlipMatrix = Potree2Consts.YZflip.Invert();

        //    convertedData.Add(new LASPoint
        //    {
        //        X = (uint)((ptFlipped.x) / scaleFactor.x),
        //        Y = (uint)((ptFlipped.y) / scaleFactor.y),
        //        Z = (uint)((ptFlipped.z) / scaleFactor.z),
        //        Classification = p.Classification,
        //        //Intensity = (ushort)((p.Intensity / maxIntensityValuePotree) * maxColorAndIntensityValueLAS),
        //        R = (ushort)(p.Color.r / maxColorValuePotree * maxColorAndIntensityValueLAS),
        //        G = (ushort)(p.Color.g / maxColorValuePotree * maxColorAndIntensityValueLAS),
        //        B = (ushort)(p.Color.b / maxColorValuePotree * maxColorAndIntensityValueLAS),
        //    });
        //}

        //    foreach (var p in points)
        //    {
        //        // flipped y/z
        //        var ptFlipped = invFlipMatrix * p.Position;

        //        convertedData.Add(new LASPoint
        //        {
        //            X = (uint)((ptFlipped.x) / scaleFactor.x),
        //            Y = (uint)((ptFlipped.y) / scaleFactor.y),
        //            Z = (uint)((ptFlipped.z) / scaleFactor.z),
        //            Classification = p.Classification,
        //            Intensity = (ushort)((p.Intensity / maxIntensityValuePotree) * maxColorAndIntensityValueLAS),
        //            R = (ushort)(p.Color.r / maxColorValuePotree * maxColorAndIntensityValueLAS),
        //            G = (ushort)(p.Color.g / maxColorValuePotree * maxColorAndIntensityValueLAS),
        //            B = (ushort)(p.Color.b / maxColorValuePotree * maxColorAndIntensityValueLAS),
        //        });
        //    }

        //    var min = metadata.Attributes["position"].Min;
        //    var max = metadata.Attributes["position"].Max;

        //    var header = new LASHeader
        //    {
        //        // flipped y/z
        //        OffsetX = metadata.Offset.x,
        //        OffsetY = metadata.Offset.y,
        //        OffsetZ = metadata.Offset.z,
        //        ScaleFactorX = metadata.Scale.x,
        //        ScaleFactorY = metadata.Scale.y,
        //        ScaleFactorZ = metadata.Scale.z,
        //        NumberOfPtRecords = (ulong)convertedData.Count,
        //        MinX = min.x,
        //        MaxX = max.x,
        //        MinY = min.y,
        //        MaxY = max.y,
        //        MinZ = min.z,
        //        MaxZ = max.z
        //    };

        //    using var fs = savePath.Create();
        //    using var bw = new BinaryWriter(fs);

        //    bw.Write(ToByteArray(header));

        //    foreach (var p in convertedData)
        //    {
        //        bw.Write(ToByteArray(p));
        //    }

        //    bw.Close();
        //    fs.Close();
        //}
    }
}