using CommunityToolkit.Diagnostics;
using Fusee.Base.Core;
using Fusee.PointCloud.Potree.V2;
using Fusee.PointCloud.Potree.V2.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Fusee.PointCloud.Potree
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LASHeader
    {
        public LASHeader()
        {

        }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] FileSignature = new char[] { 'L', 'A', 'S', 'F' };

        public ushort FileSourceID = 0;
        public ushort GlobalEncoding = 0;

        public uint GUIDData1 = 0;
        public ushort GUIDData2 = 0;
        public ushort GUIDData3 = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] GUIData4 = new byte[8];

        public byte VersionMajor = 1;
        public byte VersionMinor = 4;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] SystemIdentifier = new byte[32];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] GeneratingSoftware = new byte[32];

        public ushort FileCreationDayOfYear = (ushort)DateTime.Now.Day;
        public ushort FileCreationYear = (ushort)DateTime.Now.Year;
        public ushort HeaderSize = 375;
        public uint OffsetToPointData = 375; // sizeof(LASHeader)
        public uint NumberOfVariableLengthRecords = 0;
        public byte PointDataRecordFormat = 2;
        public ushort PointDataRecordLength = 26; // sizeof(LASPoint)
        public uint LegacyNbrOfPoints = 0;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public uint[] LeacyNbrOfPointsByRtn = new uint[5];

        public double ScaleFactorX = 0;
        public double ScaleFactorY = 0;
        public double ScaleFactorZ = 0;

        public double OffsetX = 0;
        public double OffsetY = 0;
        public double OffsetZ = 0;

        public double MaxX = 0;
        public double MinX = 0;

        public double MaxY = 0;
        public double MinY = 0;

        public double MaxZ = 0;
        public double MinZ = 0;

        public ulong StartOfWaveformPacket = 0;
        public ulong StartOfFirstExtendend = 0;
        public uint NbrOfExtendedVariableLength = 0;

        public ulong NumberOfPtRecords = 0;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
        public ulong[] NbrOfPointsByReturn = new ulong[15];

        public byte[] ToByteArray()
        {
            var arr = new byte[] { };
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
    public struct LASPoint
    {
        public LASPoint()
        { }

        public uint X = 0;
        public uint Y = 0;
        public uint Z = 0;

        public ushort Intensity = 0;
        public byte ReturnNbrOfScanDirAndEdgeByte = 0;

        public byte Classification = 0;
        public byte ScanAngleRank = 0;
        public byte UserData = 0;

        public ushort PtSrcID = 0;

        public ushort R = 0;
        public ushort G = 0;
        public ushort B = 0;

        public byte[] ToByteArray()
        {
            var arr = new byte[] { };
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
        /// Converts a list of <see cref="PotreePoint"/>s to a LAS 1.4 file and writes it to the disk
        /// </summary>
        /// <param name="points"><see cref="PotreePoint"/>s as input</param>
        /// <param name="savePath"><see cref="FileInfo"/> where to save the *.las file to, existing file is being overwritten!</param>
        public static void WritePotree2LAS(IEnumerable<PotreePoint> points, PotreeMetadata metadata, FileInfo savePath)
        {
            Guard.IsNotNull(savePath);
            Guard.IsNotNull(points);
            Guard.IsTrue(savePath.Extension == ".las");

            Guard.IsEqualTo(Marshal.SizeOf<LASPoint>(), 26);
            Guard.IsEqualTo(Marshal.SizeOf<LASHeader>(), 375);

            if (savePath.Exists)
                Diagnostics.Warn($"File {savePath.FullName} already existing, overwriting ...");

            var scaleFactor = metadata.Scale;
            var offset = metadata.Offset;

            const float maxColorValuePotree = byte.MaxValue;
            const short maxIntensityValuePotree = short.MaxValue;
            const ushort maxColorAndIntensityValueLAS = ushort.MaxValue;

            var invFlipMatrix = Potree2Consts.YZflip.Invert();

            var convertedData = new ConcurrentBag<LASPoint>();
            //Parallel.ForEach(points, (p) =>
            foreach (var p in points)
            {
                var ptFlipped = invFlipMatrix * p.Position;

                var X = (uint)((ptFlipped.x ) / scaleFactor.x);
                var Y = (uint)((ptFlipped.y ) / scaleFactor.y);
                var Z = (uint)((ptFlipped.z ) / scaleFactor.z);
                var R = (ushort)(p.Color.r / maxColorValuePotree * maxColorAndIntensityValueLAS);
                var G = (ushort)(p.Color.g / maxColorValuePotree * maxColorAndIntensityValueLAS);
                var B = (ushort)(p.Color.b / maxColorValuePotree * maxColorAndIntensityValueLAS);

                convertedData.Add(new LASPoint
                {
                    X = (uint)((ptFlipped.x /* + offset.x*/) / scaleFactor.x),
                    Y = (uint)((ptFlipped.y /* + offset.y*/) / scaleFactor.y), // flipped y/z
                    Z = (uint)((ptFlipped.z /* + offset.z*/) / scaleFactor.z),
                    Classification = p.Classification,
                    Intensity = (ushort)((p.Intensity / maxIntensityValuePotree) * maxColorAndIntensityValueLAS),
                    R = (ushort)(p.Color.r / maxColorValuePotree * maxColorAndIntensityValueLAS),
                    G = (ushort)(p.Color.g / maxColorValuePotree * maxColorAndIntensityValueLAS),
                    B = (ushort)(p.Color.b / maxColorValuePotree * maxColorAndIntensityValueLAS),
                });
            }
            //);

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
