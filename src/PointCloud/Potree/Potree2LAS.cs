using CommunityToolkit.Diagnostics;
using Fusee.Base.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Potree.V2.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

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

    /// <summary>
    /// LAS point type
    /// </summary>
    internal enum LASPointType : byte
    {
        /// <summary>
        /// 0
        /// </summary>
        Zero = 0x0,
        /// <summary>
        /// 1
        /// </summary>
        One = 0x1,
        /// <summary>
        /// 2
        /// </summary>
        Two = 0x2,
        /// <summary>
        /// 3
        /// </summary>
        Three = 0x3,
        /// <summary>
        /// 4
        /// </summary>
        Four = 0x4,
        /// <summary>
        /// 5
        /// </summary>
        Five = 0x5,
        /// <summary>
        /// 6
        /// </summary>
        Six = 0x6,
        /// <summary>
        /// 7
        /// </summary>
        Seven = 0x7,
        /// <summary>
        /// 8
        /// </summary>
        Eight = 0x8,
        /// <summary>
        /// 9
        /// </summary>
        Nine = 0x9,
        /// <summary>
        /// 10
        /// </summary>
        Ten = 0x10,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct VariableLengthRecordHeader
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
    internal struct LasExtraBytes
    {
        public LasExtraBytes() { }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved = new byte[2];      // 2 bytes

        public byte data_type = 0;                  // 1 byte

        public byte options = 0;                    // 1 byte

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] name = new byte[32];         // 32 bytes

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] unused = new byte[4];        // 4 bytes

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] no_data = new byte[8];        // 8 bytes anytype

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] deprecated1 = new byte[16];  // 16 bytes

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] min = new byte[8];            // 8 bytes anytype

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] deprecated2 = new byte[16];  // 16 bytes

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] max = new byte[8];            // 8 bytes anytype

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] deprecated3 = new byte[16];  // 16 bytes

        public double scale = 0;                    // 8 bytes

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] deprecated4 = new byte[16];  // 16 bytes

        public double offset = 0;                   // 8 bytes

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] deprecated5 = new byte[16];  // 16 bytes

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] description = new byte[32];  // 32 bytes
    }

    /// <summary>
    /// This class provides methods to convert and saves to LAS 1.4 specification
    /// </summary>
    public class Potree2LAS : IPointWriter, IDisposable
    {
        /// <summary>
        /// Path to save the las file to
        /// </summary>
        public FileInfo SavePath { get; private set; }

        /// <summary>
        /// Metadata (scale, offset)
        /// </summary>
        public IPointWriterMetadata Metadata { get; private set; }

        private readonly Stream _fileStream;
        private bool disposedValue;
        private LASHeader _header;
        private readonly PotreeData _potreeData;

        private readonly List<VariableLengthRecordHeader> _vlrh = new();
        private readonly List<LasExtraBytes> _extraByteDesc = new();

        /// <summary>
        /// Generate a writer instance, pass save path, Potree data and optional the point type to write
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="potreeData"></param>
        /// <param name="ptType"></param>
        public Potree2LAS(FileInfo savePath, PotreeData potreeData)
        {
            Guard.IsNotNull(savePath);
            Guard.IsNotNull(potreeData);
            Guard.IsTrue(savePath.Extension == ".las");

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

            // automatic point guessing
            var ptSize = _potreeData.Metadata.OffsetToExtraBytes == -1 ? size : (_potreeData.Metadata.OffsetToExtraBytes - 1);
            LASPointType ptType = ptSize switch
            {
                20 => LASPointType.Zero,
                28 => LASPointType.One,
                26 => LASPointType.Two,
                34 => LASPointType.Three,
                57 => LASPointType.Four,
                63 => LASPointType.Five,
                30 => LASPointType.Six,
                36 => LASPointType.Seven,
                38 => LASPointType.Eight,
                59 => LASPointType.Nine,
                67 => LASPointType.Ten,
                _ => throw new NotImplementedException($"Potree point type of size {ptSize} not supported."),
            };

            // Note: AABB is not y/z flipped, offset and scale is.
            _header = new LASHeader
            {
                PointDataRecordLength = (ushort)size,
                FileCreationDayOfYear = (ushort)doy,
                FileCreationYear = (ushort)year,
                MaxX = Metadata.AABB.max.x,
                MaxY = Metadata.AABB.max.y,
                MaxZ = Metadata.AABB.max.z,
                MinX = Metadata.AABB.min.x,
                MinY = Metadata.AABB.min.y,
                MinZ = Metadata.AABB.min.z,
                OffsetX = Metadata.Offset.x,
                OffsetY = Metadata.Offset.z,
                OffsetZ = Metadata.Offset.y,
                ScaleFactorX = Metadata.Scale.x,
                ScaleFactorY = Metadata.Scale.z,
                ScaleFactorZ = Metadata.Scale.y,
                NumberOfPtRecords = (ulong)Metadata.PointCount,
                LegacyNbrOfPoints = (uint)Metadata.PointCount,
                PointDataRecordFormat = (byte)ptType
            };

            _header.LeacyNbrOfPointsByRtn[0] = (uint)Metadata.PointCount;
            _header.NbrOfPointsByReturn[0] = (ulong)Metadata.PointCount;

            var generatingSoftware = Encoding.UTF8.GetBytes($"Fusee v.{Assembly.GetExecutingAssembly().GetName().Version}\0");
            Guard.IsLessThan(generatingSoftware.Length, _header.GeneratingSoftware.Length);
            Array.Copy(generatingSoftware, _header.GeneratingSoftware, generatingSoftware.Length);

            if (_potreeData.Metadata.OffsetToExtraBytes != -1)
            {
                var offset = 0;
                var sizeOfExtraBytesAfterHeader = 0; // extra variable for vlr entries
                _header.NumberOfVariableLengthRecords++;

                // we have extra bytes to append to each point
                // check how many and which
                // parse them and add them to the header
                // set something for the write method
                foreach (var attribute in _potreeData.Metadata.AttributesList)
                {
                    if (attribute == null) continue;
                    if (offset >= _potreeData.Metadata.OffsetToExtraBytes)
                    {

                        var desc = Encoding.ASCII.GetBytes(attribute.Description.ToArray());
                        var name = Encoding.ASCII.GetBytes(attribute.Name.ToArray());

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

                        Guard.IsLessThanOrEqualTo(desc.Length, currentExtra.description.Length);
                        Guard.IsLessThanOrEqualTo(name.Length, currentExtra.name.Length);

                        Array.Copy(desc, currentExtra.description, desc.Length);
                        Array.Copy(name, currentExtra.name, name.Length);

                        var extraByteMarshalMin = attribute.Type switch
                        {
                            //see Las Specification
                            "uint8" => MemoryMarshal.AsBytes<sbyte>(attribute.MinList?.Select(x => (sbyte)x).ToArray()),
                            "int8" => MemoryMarshal.AsBytes<byte>(attribute.MinList?.Select(x => (byte)x).ToArray()),
                            "uint16" => MemoryMarshal.AsBytes<ushort>(attribute.MinList?.Select(x => (ushort)x).ToArray()),
                            "int16" => MemoryMarshal.AsBytes<short>(attribute.MinList?.Select(x => (short)x).ToArray()),
                            "uint32" => MemoryMarshal.AsBytes<ulong>(attribute.MinList?.Select(x => (ulong)x).ToArray()),
                            "int32" => MemoryMarshal.AsBytes<long>(attribute.MinList?.Select(x => (long)x).ToArray()),
                            "int64" => MemoryMarshal.AsBytes<long>(attribute.MinList?.Select(x => (long)x).ToArray()),
                            "uint64" => MemoryMarshal.AsBytes<ulong>(attribute.MinList?.Select(x => (ulong)x).ToArray()),
                            "float" => MemoryMarshal.AsBytes<float>(attribute.MinList ?.Select(x => (float)x).ToArray()),
                            "double" => MemoryMarshal.AsBytes<double>(attribute.MinList?.ToArray()),
                            _ => throw new ArgumentException("Invalid data type!")
                        };

                        var extraByteMarshalMax = attribute.Type switch
                        {
                            //see Las Specification
                            "uint8" => MemoryMarshal.AsBytes<sbyte>(attribute.MaxList?.Select(x => (sbyte)x).ToArray()),
                            "int8" => MemoryMarshal.AsBytes<byte>(attribute.MaxList?.Select(x => (byte)x).ToArray()),
                            "uint16" => MemoryMarshal.AsBytes<ushort>(attribute.MaxList?.Select(x => (ushort)x).ToArray()),
                            "int16" => MemoryMarshal.AsBytes<short>(attribute.MaxList?.Select(x => (short)x).ToArray()),
                            "uint32" => MemoryMarshal.AsBytes<ulong>(attribute.MaxList?.Select(x => (ulong)x).ToArray()),
                            "int32" => MemoryMarshal.AsBytes<long>(attribute.MaxList?.Select(x => (long)x).ToArray()),
                            "int64" => MemoryMarshal.AsBytes<long>(attribute.MaxList?.Select(x => (long)x).ToArray()),
                            "uint64" => MemoryMarshal.AsBytes<ulong>(attribute.MaxList?.Select(x => (ulong)x).ToArray()),
                            "float" => MemoryMarshal.AsBytes<float>(attribute.MaxList?.Select(x => (float)x).ToArray()),
                            "double" => MemoryMarshal.AsBytes<double>(attribute.MaxList?.ToArray()),
                            _ => throw new ArgumentException("Invalid data type!")
                        };

                        var min = extraByteMarshalMin.ToArray();
                        var max = extraByteMarshalMax.ToArray();
                        Array.Copy(min, currentExtra.min, min.Length);
                        Array.Copy(max, currentExtra.max, max.Length);

                        _extraByteDesc.Add(currentExtra);

                        _header.OffsetToPointData += 192;
                        // each description is 192 bytes
                        sizeOfExtraBytesAfterHeader += 192;
                    }

                    offset += attribute.Size;
                }

                // add the actual variable record header
                var vlr = new VariableLengthRecordHeader
                {
                    RecordLengthAfterHeader = (ushort)sizeOfExtraBytesAfterHeader, // offset with all extraBytes
                    RecordId = 4
                };

                var description = Encoding.UTF8.GetBytes("Extra Bytes Record\0");
                var userId = Encoding.UTF8.GetBytes("LASF_Spec\0");

                Guard.IsLessThanOrEqualTo(description.Length, vlr.Description.Length);
                Guard.IsLessThanOrEqualTo(userId.Length, vlr.UserId.Length);
                Array.Copy(description, vlr.Description, description.Length);
                Array.Copy(userId, vlr.UserId, userId.Length);

                // this is just for the LAS header, we should check if we can already
                // build an internal list to update later when writing the actual extra bytes
                _vlrh.Add(vlr);

                _header.OffsetToPointData += 54;
                // LAS 1.4 Spec: Each Variable Length Record Header is 54 bytes in length.
                // add, too. Complete size: header + variableLengthRecord + n * extraBytes (192bytes)
            }

            // Initialize unmanged memory to hold the struct.
            var headerPtr = Marshal.AllocHGlobal(Marshal.SizeOf<LASHeader>());
            try
            {
                // Copy the struct to unmanaged memory.
                Marshal.StructureToPtr(_header, headerPtr, false);
                var dest = new byte[Marshal.SizeOf<LASHeader>()];
                Marshal.Copy(headerPtr, dest, 0, Marshal.SizeOf<LASHeader>());
                _fileStream.Write(dest);
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(headerPtr);
            }

            // append all variable length record header
            foreach (var vlr in _vlrh)
            {
                var vlrPtr = Marshal.AllocHGlobal(Marshal.SizeOf<VariableLengthRecordHeader>());
                try
                {
                    // Copy the struct to unmanaged memory.
                    Marshal.StructureToPtr(vlr, vlrPtr, false);
                    var dest = new byte[Marshal.SizeOf<VariableLengthRecordHeader>()];
                    Marshal.Copy(vlrPtr, dest, 0, Marshal.SizeOf<VariableLengthRecordHeader>());
                    _fileStream.Write(dest);
                }
                finally
                {
                    // Free the unmanaged memory.
                    Marshal.FreeHGlobal(vlrPtr);
                }
            }

            foreach (var extraByte in _extraByteDesc)
            {
                var extraBytePtr = Marshal.AllocHGlobal(Marshal.SizeOf<LasExtraBytes>());
                try
                {
                    // Copy the struct to unmanaged memory.
                    Marshal.StructureToPtr(extraByte, extraBytePtr, false);
                    var dest = new byte[Marshal.SizeOf<LasExtraBytes>()];
                    Marshal.Copy(extraBytePtr, dest, 0, Marshal.SizeOf<LasExtraBytes>());
                    _fileStream.Write(dest);
                }
                finally
                {
                    // Free the unmanaged memory.
                    Marshal.FreeHGlobal(extraBytePtr);
                }
            }
        }

        /// <summary>
        /// This methods starts the LASfile write progress.
        /// </summary>
        public void Write()
        {
            Guard.IsNotNull(_header);

            // advance to end of stream
            _fileStream.Seek(0, SeekOrigin.End);
            var fileLength = Metadata.PointCount * (long)Metadata.PointSize;

            // set complete length before writing, this generates the full file
            // writing operations are much faster afterwards
            _fileStream.SetLength(fileLength);

            using var inputStream = _potreeData.OctreeMappedFile.CreateViewStream();

            Span<byte> tmpArray = stackalloc byte[_potreeData.Metadata.PointSize];

            // DO NOT USE stream.Length as the MemoryMappedStream aligns with the page size
            for (long i = 0U; i < fileLength; i += Metadata.PointSize)
            {
                // we need to shrink each point back (skip byte 16)
                // extra bytes and everything is already included
                inputStream.Read(tmpArray);
                _fileStream.Write(tmpArray[..15]); // pos(12) + intensity (2) + returnStuff (1)
                _fileStream.Write(tmpArray[16..Metadata.PointSize]); // skip array pos [15], byte 16
            }

            inputStream.Close();
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

                disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}