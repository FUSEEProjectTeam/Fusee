using CommunityToolkit.Diagnostics;
using Fusee.Base.Core;
using Fusee.PointCloud.Common;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Fusee.PointCloud.Potree.V2.Data;
using System.IO.MemoryMappedFiles;

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

        internal int X = 0;
        internal int Y = 0;
        internal int Z = 0;

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

    public enum LASPointType : byte
    {
        Zero = 0x0,
        Two = 0x2,
        Seven = 0x7
    }

    /// <summary>
    /// This class provides methods to convert and saves <see cref="LASPoint"/> clouds to LAS 1.4
    /// </summary>
    public class Potree2LAS : IPointWriter, IDisposable
    {
        public FileInfo SavePath { get; private set; }
        public IPointWriterMetadata Metadata { get; private set; }

        private readonly Stream _fileStream;
        private bool disposedValue;
        private LASHeader _header;
        private readonly PotreeData _potreeData;
        private readonly LASPointType _type;

        /// <summary>
        /// Generate a writer instance, pass save path, Potree data and optional the point type to write
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="potreeData"></param>
        /// <param name="ptType"></param>
        public Potree2LAS(FileInfo savePath, PotreeData potreeData, LASPointType ptType = LASPointType.Two)
        {
            Guard.IsNotNull(savePath);
            Guard.IsNotNull(potreeData);
            Guard.IsTrue(savePath.Extension == ".las");

            _type = ptType;

            if (savePath.Exists)
            {
                Diagnostics.Warn($"{savePath.FullName} does already exists. Overwriting ...");
                savePath.Delete();
            }

            SavePath = savePath;
            Metadata = potreeData.Metadata;
            _fileStream = SavePath.OpenWrite();
            _potreeData = potreeData;
            ParseAndFillHeader();
        }

        private void ParseAndFillHeader()
        {
            var doy = DateTime.Now.DayOfYear;
            var year = DateTime.Now.Year;
            var size = Marshal.SizeOf<LASPoint>();

            // make sure we can cast to <see cref="ushort"/> and do not lose information
            Guard.IsLessThan(doy, ushort.MaxValue);
            Guard.IsLessThan(year, ushort.MaxValue);
            Guard.IsLessThan(size, ushort.MaxValue);

            _fileStream.Seek(0, SeekOrigin.Begin);

            // TODO: Parse / generate fitting point type and extra bytes, etc...
            _header = new LASHeader
            {
                PointDataRecordLength = (ushort)size,
                FileCreationDayOfYear = (ushort)doy,
                FileCreationYear = (ushort)year,
                MaxX = Metadata.AABB.max.x,
                MaxY = Metadata.AABB.max.z,
                MaxZ = Metadata.AABB.max.y,
                MinX = Metadata.AABB.min.x,
                MinY = Metadata.AABB.min.z,
                MinZ = Metadata.AABB.min.y,
                OffsetX = Metadata.Offset.x,
                OffsetY = Metadata.Offset.y,
                OffsetZ = Metadata.Offset.z,
                ScaleFactorX = Metadata.Scale.x,
                ScaleFactorY = Metadata.Scale.y,
                ScaleFactorZ = Metadata.Scale.z,
                NumberOfPtRecords = (ulong)Metadata.PointCount,
                LegacyNbrOfPoints = (uint)Metadata.PointCount,
                PointDataRecordFormat = (byte)_type
            };

            //_header.LeacyNbrOfPointsByRtn[0] = (uint)Metadata.PointCount;
            //_header.NbrOfPointsByReturn[0] = (ulong)Metadata.PointCount;

            var generatingSoftware = Encoding.UTF8.GetBytes($"Fusee v.{Assembly.GetExecutingAssembly().GetName().Version}");
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

        delegate void ConvertPointMethod(Span<byte> data, Stream s);
        delegate void WriteStreamChunk(MemoryMappedFile file, long start, long end);

        /// <summary>
        /// This methods starts the LASfile write progress.
        /// </summary>
        /// <param name="progressCallback">This methods returns the current progress [0-100] (per-cent)</param>
        public void Write(Action<int>? progressCallback = null)
        {
            // advance to end of stream
            _fileStream.Seek(0, SeekOrigin.End);

            Guard.IsNotNull(_header);

            using var stream = _potreeData.OctreeMappedFile.CreateViewStream();
            var fileLength = Metadata.PointCount * Metadata.PointSize;

            Span<byte> tmpArry = (int)_type switch
            {
                //0 => stackalloc byte[666], // TODO
                2 => stackalloc byte[26 + 1], // + 1 due to wrong potree bytes
                7 => stackalloc byte[36 + 1],
                _ => throw new NotImplementedException(),
            }; ;

            ConvertPointMethod convertPtMethod = (int)_type switch
            {
                0 => static (Span<byte> pt, Stream s) =>
                {
                    throw new NotImplementedException();
                }
                ,
                2 => static (Span<byte> pt, Stream s) =>
                {
                    s.Write(pt[..15]); // position
                    s.Write(pt.Slice(12, 3)); // intensity, and mixed returns according to las 1.4
                    // skip number of returns! [15,16]
                    s.Write(pt.Slice(16, 11)); // rest
                }
                ,
                7 => static (Span<byte> pt, Stream s) =>
                {
                    s.Write(pt[..12]); // position
                    s.Write(pt.Slice(12, 3)); // intensity, and mixed returns according to las 1.4
                    // skip number of returns! [15,16]
                    s.Write(pt.Slice(16, 21)); // rest
                }
                ,
                _ => throw new NotImplementedException(),
            };

            // DO NOT USE stream.Length as the MemoryMappedStream aligns with the page size
            for (var i = 0; i < fileLength; i += Metadata.PointSize)
            {
                float progress = (100f / Metadata.PointCount) * (i / Metadata.PointSize);
                progressCallback?.Invoke((int)progress);
                // we need to copy each point and shrink it back to 26 (from 27) due to PotreeConvert errors
                stream.Read(tmpArry);
                convertPtMethod(tmpArry, _fileStream);
            }
        }

        /// <summary>
        /// Dispose the <see cref="FileStream"/> of this class.
        /// </summary>
        /// <param name="disposing"></param>
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

        /// <summary>
        /// Dispose
        /// </summary>
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