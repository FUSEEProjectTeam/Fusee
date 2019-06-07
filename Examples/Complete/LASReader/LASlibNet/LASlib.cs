using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace LASlibNet
{
    // TODO: Use the PCL PointT points, update LASlibWrapper after that
    [StructLayout(LayoutKind.Sequential)]
    public struct CS_Point
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

    [StructLayout(LayoutKind.Sequential)]
    public struct CS_Header
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

    public struct PointT
    {
        public double X;
        public double Y;
        public double Z;

        public ushort Intensity;

        public byte Classification;
        public byte UserData;

        public ushort R;
        public ushort G;
        public ushort B;

        public static explicit operator PointT(CS_Point v)
        {
            PointT point;
            point.X = v.X;
            point.Y = v.Y;
            point.Z = v.Z;

            point.Intensity = v.Intensity;

            point.Classification = v.Classification;
            point.UserData = v.UserData;

            point.R = v.R;
            point.G = v.G;
            point.B = v.B;

            return point;
        }

       public static bool operator  ==(PointT left, PointT right)
       {
            return (left.X == right.X && left.Y == right.Y && left.Z == right.Z);
       }

        public static bool operator !=(PointT left, PointT right)
        {
            return !(left == right);
        }
        

        public override bool Equals(object obj)
        {
            return obj is PointT t &&
                   X == t.X &&
                   Y == t.Y &&
                   Z == t.Z;
        }

        public override int GetHashCode()
        {
            var hashCode = 1803061989;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Z.GetHashCode();
            hashCode = hashCode * -1521134295 + Intensity.GetHashCode();
            hashCode = hashCode * -1521134295 + Classification.GetHashCode();
            hashCode = hashCode * -1521134295 + UserData.GetHashCode();
            hashCode = hashCode * -1521134295 + R.GetHashCode();
            hashCode = hashCode * -1521134295 + G.GetHashCode();
            hashCode = hashCode * -1521134295 + B.GetHashCode();
            return hashCode;
        }
    }

/// <summary>
///     Open and read one LAS file
/// </summary>
public class LASReader : IDisposable
{
    /// <summary>
    ///     The filename of the current file
    /// </summary>
    public readonly string Filename;

    /// <summary>
    ///     The header of one file with information about point count, offsets, point type, etc.
    /// </summary>
    public readonly CS_Header Header = new CS_Header();

    /// <summary>
    ///     The global UTM coordinate offset of one file
    /// </summary>
    public double3 PointOffset
    {
        get { return new double3(Header.OffsetX, Header.OffsetY, Header.OffsetZ); }
    }

    /// <summary>
    ///     The number of points in this file
    /// </summary>
    public long PointCount { get { return Header.PointCnt; } }

    /// <summary>
    ///     Get all points of one LAS file
    /// </summary>
    public IEnumerable<PointT> Points
    {
        get
        {
            bool readNextPoint = new bool();
            ReadNextPoint(_ptrToLASClass, ref readNextPoint);

            while (readNextPoint)
            {
                var cs_point = new CS_Point();
                GetPoint(_ptrToLASClass, ref cs_point);
                var point = (PointT)cs_point;

                point.X *= Header.ScaleFactorX;
                point.Y *= Header.ScaleFactorY;
                point.Z *= Header.ScaleFactorZ;

                ReadNextPoint(_ptrToLASClass, ref readNextPoint);

                yield return point;
            }
        }
    }

        /// <summary>
        ///     Get all points of one LAS file with added offset
        /// </summary>
        public IEnumerable<PointT> PointsWithOffset
        {
            get
            {
                bool readNextPoint = new bool();
                ReadNextPoint(_ptrToLASClass, ref readNextPoint);

                while (readNextPoint)
                {
                    var cs_point = new CS_Point();
                    GetPoint(_ptrToLASClass, ref cs_point);
                    var point = (PointT)cs_point;

                    point.X = point.X * Header.ScaleFactorX + Header.OffsetX;
                    point.Y = point.Y * Header.ScaleFactorY + Header.OffsetY;
                    point.Z = point.Z * Header.ScaleFactorZ + Header.OffsetZ;

                    ReadNextPoint(_ptrToLASClass, ref readNextPoint);

                    yield return point;
                }
            }
        }

        

    private IntPtr _ptrToLASClass = new IntPtr();

    private bool _disposed = false;

    /// <summary>
    ///     A LASReader can open point files encoded by the las format v. 1.4 with the following extensions:
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
    public LASReader(string filename)
    {
        Filename = filename;
        OpenLASFile(filename, ref _ptrToLASClass);

        if (_ptrToLASClass == IntPtr.Zero)
            throw new FileNotFoundException($"{filename} not found!");

        GetHeader(_ptrToLASClass, ref Header);
    }

    ~LASReader()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        // This object will be cleaned up by the Dispose method.
        // Therefore, you should call GC.SupressFinalize to
        // take this object off the finalization queue
        // and prevent finalization code for this object
        // from executing a second time.
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Dispose(bool disposing) executes in two distinct scenarios.
    // If disposing equals true, the method has been called directly
    // or indirectly by a user's code. Managed and unmanaged resources
    // can be disposed.
    // If disposing equals false, the method has been called by the
    // runtime from inside the finalizer and you should not reference
    // other objects. Only unmanaged resources can be disposed.
    protected virtual void Dispose(bool disposing)
    {
        // Check to see if Dispose has already been called.
        if (!_disposed)
        {
            // Dispose managed resources here.
            // if (disposing) { }

            // Call the appropriate methods to clean up
            // unmanaged resources here.
            // If disposing is false,
            // only the following code is executed.
            Delete(ref _ptrToLASClass);
            _ptrToLASClass = IntPtr.Zero;

            // Note disposing has been done.
            _disposed = true;
        }
    }

    [DllImport("libLASlib", EntryPoint = "CS_OpenLasFile")]
    static extern IntPtr OpenLASFile(string filename, ref IntPtr lasFileHandle);

    [DllImport("libLASlib", EntryPoint = "CS_GetHeader")]
    static extern void GetHeader(IntPtr lasFileHandle, ref CS_Header header);

    [DllImport("libLASlib", EntryPoint = "CS_ReadNextPoint")]
    static extern void ReadNextPoint(IntPtr lasFileHandle, ref bool nextPoint);

    [DllImport("libLASlib", EntryPoint = "CS_GetPoint")]
    static extern void GetPoint(IntPtr lasFileHandle, ref CS_Point csPoint);

    [DllImport("libLASlib", EntryPoint = "CS_Delete")]
    static extern void Delete(ref IntPtr lasFileHandle);
}
}
