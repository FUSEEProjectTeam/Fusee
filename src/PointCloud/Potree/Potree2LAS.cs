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
using System.Diagnostics;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.Linq;

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
        internal ushort PointDataRecordLength = 0;
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

    public enum LASPointType : byte
    {
        Zero = 0x0,
        Two = 0x2,
        Seven = 0x7
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VariableLengthRecordHeader
    {
        public VariableLengthRecordHeader() { }

        public ushort Reserved = 0;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] UserId = new byte[16];

        public ushort RecordId = 0;
        public ushort RecordLengthAfterHeader = 0;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] Description = new byte[32];
    }

    internal struct InternalVariableLengthRecord
    {
        public int Type;
        public double[] Max;
        public double[] Min;
        public string Name;
        public string Description;

    }

    /// <summary>
    /// Struct for the Specification Defined VLR "Extra Bytes". This has always 192 bytes.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LasExtraBytes
    {
        public LasExtraBytes() { }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved = new byte[2];      // 2 bytes

        public byte data_type = 0;                     // 1 byte

        public byte options = 0;                       // 1 byte

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] name = new byte[32];         // 32 bytes

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] unused = new byte[4];        // 4 bytes

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] no_data = new byte[8];                     // 8 bytes anytype

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] deprecated1 = new byte[16];  // 16 bytes

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] min = new byte[8];                         // 8 bytes anytype

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] deprecated2 = new byte[16];  // 16 bytes

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] max = new byte[8];                         // 8 bytes anytype

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] deprecated3 = new byte[16];  // 16 bytes

        public double scale = 0;                       // 8 bytes

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] deprecated4 = new byte[16];  // 16 bytes

        public double offset = 0;                      // 8 bytes

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] deprecated5 = new byte[16];  // 16 bytes

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] description = new byte[32];  // 32 bytes
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

        private readonly List<VariableLengthRecordHeader> _vlrh = new();
        private readonly List<LasExtraBytes> _extraByteDesc = new();

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
            var size = Metadata.PointSize - 1;

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

            _header.LeacyNbrOfPointsByRtn[0] = (uint)Metadata.PointCount;
            _header.NbrOfPointsByReturn[0] = (ulong)Metadata.PointCount;

            var generatingSoftware = Encoding.UTF8.GetBytes($"Fusee v.{Assembly.GetExecutingAssembly().GetName().Version}");
            Guard.IsLessThan(generatingSoftware.Length, _header.GeneratingSoftware.Length);
            Array.Copy(generatingSoftware, _header.GeneratingSoftware, generatingSoftware.Length);

            if (_potreeData.Metadata.OffsetToExtraBytes != -1)
            {
                var offset = 0;
                // we have extra bytes to append to each point
                // check how many and which
                // parse them and add them to the header
                // set something for the write method
                foreach (var attribute in _potreeData.Metadata.AttributesList)
                {
                    if (attribute == null) continue;
                    if (offset >= _potreeData.Metadata.OffsetToExtraBytes)
                    {
                        _header.NumberOfVariableLengthRecords++;
                        _header.OffsetToPointData += 54; 
                        // LAS 1.4 Spec: Each Variable Length Record Header is 54 bytes in length.
                      
                        var desc = Encoding.ASCII.GetBytes(attribute.Description.Append('\0').ToArray());
                        var name = Encoding.ASCII.GetBytes(attribute.Name.Append('\0').ToArray());

                        var currentEntry = new VariableLengthRecordHeader
                        {
                            RecordLengthAfterHeader = (ushort)_header.OffsetToPointData, // current offset
                            RecordId = 4
                        };

                        Guard.IsLessThan(desc.Length, currentEntry.Description.Length);
                        Array.Copy(desc, currentEntry.Description, desc.Length);

                        // this is just for the LAS header, we should check if we can already 
                        // build an internal list to update later when writing the actual extra bytes
                        _vlrh.Add(currentEntry);


                        var extraByteType = attribute.Type switch
                        {
                            //see Las Specification
                            "uint8" => 1, // uchar
                            "int8" => 2, // char
                            "uint16" => 3, // ushort
                            "int16" => 4, // short
                            "uint32" => 5, // ulong
                            "int32" => 6, // long
                            "int64" => 7, // longlong
                            "uint64" => 8, // ulonglong,
                            "float" => 9, // float
                            "double" => 10 // double
,
                            _ => throw new ArgumentException("Invalid data type!")
                        };


                        var currentExtra = new LasExtraBytes
                        {
                            data_type = (byte)extraByteType
                        };

                        Guard.IsLessThan(desc.Length, currentExtra.description.Length);
                        Guard.IsLessThan(name.Length, currentExtra.name.Length);

                        Array.Copy(desc, currentExtra.description, desc.Length);
                        Array.Copy(name, currentExtra.name, name.Length);

                        var extraByteMarshalMin = attribute.Type switch
                        {
                            //see Las Specification
                            "uint8" => MemoryMarshal.AsBytes<sbyte>(attribute.MinList.Select(x => (sbyte)x).ToArray()),
                            "int8" => MemoryMarshal.AsBytes<byte>(attribute.MinList.Select(x => (byte)x).ToArray()),
                            "uint16" => MemoryMarshal.AsBytes<ushort>(attribute.MinList.Select(x => (ushort)x).ToArray()),
                            "int16" => MemoryMarshal.AsBytes<short>(attribute.MinList.Select(x => (short)x).ToArray()),
                            "uint32" => MemoryMarshal.AsBytes<ulong>(attribute.MinList.Select(x => (ulong)x).ToArray()),
                            "int32" => MemoryMarshal.AsBytes<long>(attribute.MinList.Select(x => (long)x).ToArray()),
                            "int64" => MemoryMarshal.AsBytes<Int64>(attribute.MinList.Select(x => (Int64)x).ToArray()),
                            "uint64" => MemoryMarshal.AsBytes<UInt64>(attribute.MinList.Select(x => (UInt64)x).ToArray()),
                            "float" => MemoryMarshal.AsBytes<float>(attribute.MinList.Select(x => (float)x).ToArray()),
                            "double" => MemoryMarshal.AsBytes<double>(attribute.MinList.ToArray()),
                            _ => throw new ArgumentException("Invalid data type!")
                        };

                        var extraByteMarshalMax = attribute.Type switch
                        {
                            //see Las Specification
                            "uint8" => MemoryMarshal.AsBytes<sbyte>(attribute.MaxList.Select(x => (sbyte)x).ToArray()),
                            "int8" => MemoryMarshal.AsBytes<byte>(attribute.MaxList.Select(x => (byte)x).ToArray()),
                            "uint16" => MemoryMarshal.AsBytes<ushort>(attribute.MaxList.Select(x => (ushort)x).ToArray()),
                            "int16" => MemoryMarshal.AsBytes<short>(attribute.MaxList.Select(x => (short)x).ToArray()),
                            "uint32" => MemoryMarshal.AsBytes<ulong>(attribute.MaxList.Select(x => (ulong)x).ToArray()),
                            "int32" => MemoryMarshal.AsBytes<long>(attribute.MaxList.Select(x => (long)x).ToArray()),
                            "int64" => MemoryMarshal.AsBytes<Int64>(attribute.MaxList.Select(x => (Int64)x).ToArray()),
                            "uint64" => MemoryMarshal.AsBytes<UInt64>(attribute.MaxList.Select(x => (UInt64)x).ToArray()),
                            "float" => MemoryMarshal.AsBytes<float>(attribute.MaxList.Select(x => (float)x).ToArray()),
                            "double" => MemoryMarshal.AsBytes<double>(attribute.MaxList.ToArray()),
                            _ => throw new ArgumentException("Invalid data type!")
                        };


                        var min = extraByteMarshalMin.ToArray();
                        var max = extraByteMarshalMax.ToArray();
                        Array.Copy(min, currentExtra.min, min.Length);
                        Array.Copy(max, currentExtra.max, max.Length);


                        _extraByteDesc.Add(currentExtra);

                        _header.OffsetToPointData += 192;
                        // each description is 192 bytes

                    }

                    offset += attribute.Size;
                }
            }

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

            // append all variable length record header

            foreach (var vlr in _vlrh)
            {
                var mem = Marshal.AllocHGlobal(Marshal.SizeOf<VariableLengthRecordHeader>());
                try
                {
                    // Copy the struct to unmanaged memory.
                    Marshal.StructureToPtr(vlr, mem, false);
                    var dest = new byte[Marshal.SizeOf<VariableLengthRecordHeader>()];
                    Marshal.Copy(mem, dest, 0, Marshal.SizeOf<VariableLengthRecordHeader>());
                    _fileStream.Write(dest);

                }
                finally
                {
                    // Free the unmanaged memory.
                    Marshal.FreeHGlobal(mem);
                }
            }

            foreach (var extraByte in _extraByteDesc)
            {
                var memExtra = Marshal.AllocHGlobal(Marshal.SizeOf<LasExtraBytes>());
                try
                {
                    // Copy the struct to unmanaged memory.
                    Marshal.StructureToPtr(extraByte, memExtra, false);
                    var dest = new byte[Marshal.SizeOf<LasExtraBytes>()];
                    Marshal.Copy(memExtra, dest, 0, Marshal.SizeOf<LasExtraBytes>());
                    _fileStream.Write(dest);

                }
                finally
                {
                    // Free the unmanaged memory.
                    Marshal.FreeHGlobal(memExtra);
                }
            }
        }

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
            ulong fileLength = (ulong)Metadata.PointCount * (ulong)Metadata.PointSize;

            Span<byte> tmpArray = stackalloc byte[_potreeData.Metadata.PointSize];

            // DO NOT USE stream.Length as the MemoryMappedStream aligns with the page size
            for (ulong i = 0U; i < fileLength; i += (ulong)Metadata.PointSize)
            {
                var strSize = _fileStream.Length;
                float progress = (100f / Metadata.PointCount) * (i / (ulong)Metadata.PointSize);
                progressCallback?.Invoke((int)progress);

                // we need to shrink each point back (skip byte 16)
                // extra bytes and everything is already included
                // TODO: check if extra bytes are already aligned correctely with LAS spec
                // probably not, after conversion
                // TODO: Convert bytes to extra bytes, offset to bytes should be given or inside the VRL
                stream.Read(tmpArray);


                _fileStream.Write(tmpArray[..15]); // pos(12) + intensity (2) + returnStuff (1)               
                _fileStream.Write(tmpArray[16..Metadata.PointSize]); // skip array pos [15], byte 16
                                                                     //Debug.Assert(strSize + 36 == _fileStream.Length);


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