using CommunityToolkit.Diagnostics;
using Fusee.Base.Core;
using Fusee.PointCloud.Potree.V2;
using Fusee.PointCloud.Potree.V2.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Fusee.PointCloud.Potree
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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
        internal uint OffsetToPointData = 375; // sizeof(LASHeader)
        internal uint NumberOfVariableLengthRecords = 0;
        internal byte PointDataRecordFormat = 2;
        internal ushort PointDataRecordLength = 26; // sizeof(LASPoint)
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

        internal byte[] ToByteArray()
        {
            var arr = Array.Empty<byte>();
            var ptr = IntPtr.Zero;
            try
            {
                int size = Marshal.SizeOf<LASHeader>();
                arr = new byte[size];
                ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(this, ptr, true);
                Marshal.Copy(ptr, arr, 0, size);
            }
            catch (Exception e)
            {
                Diagnostics.Error(e);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return arr;
        }
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

        internal byte[] ToByteArray()
        {
            var arr = Array.Empty<byte>();
            var ptr = IntPtr.Zero;
            try
            {
                int size = Marshal.SizeOf<LASPoint>();
                arr = new byte[size];
                ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(this, ptr, true);
                Marshal.Copy(ptr, arr, 0, size);
            }
            catch (Exception e)
            {
                Diagnostics.Error(e);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return arr;
        }
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// This class provides methods to convert and save <see cref="PotreePoint"/> clouds to LAS 1.4
    /// </summary>
    public static class Potree2LAS
    {
        /// <summary>
        /// Converts a list of <see cref="PotreePoint"/>s to a LAS 1.4 file and writes it to the disk in an async manner
        /// </summary>
        /// <param name="points"><see cref="PotreePoint"/>s as input</param>
        /// <param name="metadata"><see cref="PotreeMetadata"/> for offset and LAS header writing</param>
        /// <param name="savePath"><see cref="FileInfo"/> where to save the *.las file to, existing file is being overwritten!</param>
        public static async Task WritePotree2LASAsync(IEnumerable<PotreePoint> points, PotreeMetadata metadata, FileInfo savePath)
        {
            await Task.Run(() => WritePotree2LAS(points, metadata, savePath));
        }

        /// <summary>
        /// Converts a list of <see cref="PotreePoint"/>s to a LAS 1.4 file and writes it to the disk
        /// </summary>
        /// <param name="points"><see cref="PotreePoint"/>s as input</param>
        /// <param name="metadata"><see cref="PotreeMetadata"/> for offset and LAS header writing</param>
        /// <param name="savePath"><see cref="FileInfo"/> where to save the *.las file to, existing file is being overwritten!</param>
        public static void WritePotree2LAS(IEnumerable<PotreePoint> points, PotreeMetadata metadata, FileInfo savePath)
        {
            Guard.IsNotNull(savePath);
            Guard.IsNotNull(points);
            Guard.IsTrue(savePath.Extension == ".las");

            Guard.IsEqualTo(Marshal.SizeOf<LASPoint>(), 26);
            Guard.IsEqualTo(Marshal.SizeOf<LASHeader>(), 375);

            if (savePath.Exists)
                Diagnostics.Warn($"File {savePath.FullName} does exist, overwriting ...");

            var scaleFactor = metadata.Scale;

            const float maxColorValuePotree = byte.MaxValue;
            const short maxIntensityValuePotree = short.MaxValue;
            const ushort maxColorAndIntensityValueLAS = ushort.MaxValue;

            var invFlipMatrix = Potree2Consts.YZflip.Invert();

            var convertedData = new List<LASPoint>();

            foreach (var p in points)
            {
                // flipped y/z
                var ptFlipped = invFlipMatrix * p.Position;

                convertedData.Add(new LASPoint
                {
                    X = (uint)((ptFlipped.x) / scaleFactor.x),
                    Y = (uint)((ptFlipped.y) / scaleFactor.y),
                    Z = (uint)((ptFlipped.z) / scaleFactor.z),
                    Classification = p.Classification,
                    Intensity = (ushort)((p.Intensity / maxIntensityValuePotree) * maxColorAndIntensityValueLAS),
                    R = (ushort)(p.Color.r / maxColorValuePotree * maxColorAndIntensityValueLAS),
                    G = (ushort)(p.Color.g / maxColorValuePotree * maxColorAndIntensityValueLAS),
                    B = (ushort)(p.Color.b / maxColorValuePotree * maxColorAndIntensityValueLAS),
                });
            }

            var min = metadata.Attributes["position"].Min;
            var max = metadata.Attributes["position"].Max;

            var header = new LASHeader
            {
                // flipped y/z
                OffsetX = metadata.Offset.x,
                OffsetY = metadata.Offset.y,
                OffsetZ = metadata.Offset.z,
                ScaleFactorX = metadata.Scale.x,
                ScaleFactorY = metadata.Scale.y,
                ScaleFactorZ = metadata.Scale.z,
                NumberOfPtRecords = (ulong)convertedData.Count,
                MinX = min.x,
                MaxX = max.x,
                MinY = min.y,
                MaxY = max.y,
                MinZ = min.z,
                MaxZ = max.z
            };

            using var fs = savePath.Create();
            using var bw = new BinaryWriter(fs);

            bw.Write(header.ToByteArray());

            foreach (var p in convertedData)
            {
                bw.Write(p.ToByteArray());
            }

            bw.Close();
            fs.Close();
        }
    }
}
