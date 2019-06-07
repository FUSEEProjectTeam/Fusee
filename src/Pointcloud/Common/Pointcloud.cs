using Fusee.Math.Core;
using System;

namespace Fusee.Pointcloud.Common
{
    /// <summary>
    ///     A pointcloud consists of a point accessor which enables access to the points as well as some information about the point type.
    ///     Furthermore the data itself as well as some meta information like offset information.
    /// </summary>
    /// <typeparam name="TPoint">Point type</typeparam>
    public interface IPointcloud<TPoint>
    {
        PointAccessor<TPoint> Pa { get; }

        Span<TPoint> Points { get; }

        IMeta MetaInfo { get; }
    }

    /// <summary>
    ///     Every pointcloud needs a point accessor
    /// </summary>
    /// <typeparam name="TPoint"></typeparam>
    public class PointAccessor<TPoint>
    {
        // PointXYZ
        public virtual bool HasPositionFloat3_32 => false;
        public virtual bool HasPositionFloat3_64 => false;

        /// PointXYZI
        public virtual bool HasIntensityInt_8 => false;
        public virtual bool HasIntensityInt_16 => false;
        public virtual bool HasIntensityInt_32 => false;
        public virtual bool HasIntensityInt_64 => false;
        public virtual bool HasIntensityUInt_8 => false;
        public virtual bool HasIntensityUInt_16 => false;
        public virtual bool HasIntensityUInt_32 => false;
        public virtual bool HasIntensityUInt_64 => false;
        public virtual bool HasIntensityFloat32 => false;
        public virtual bool HasIntensityFloat64 => false;

        // PointXYZINormal
        public virtual bool HasNormalFloat3_32 => false;
        public virtual bool HasNormalFloat3_64 => false;

        // PointXYZINormalRGB
        public virtual bool HasColorInt_8 => false;
        public virtual bool HasColorInt_16 => false;
        public virtual bool HasColorInt_32 => false;
        public virtual bool HasColorInt_64 => false;
        public virtual bool HasColorUInt_8 => false;
        public virtual bool HasColorUInt_16 => false;
        public virtual bool HasColorUInt_32 => false;
        public virtual bool HasColorUInt_64 => false;
        public virtual bool HasColorFloat32 => false;
        public virtual bool HasColorFloat64 => false;
        public virtual bool HasColorFloat3_32 => false;
        public virtual bool HasColorFloat3_64 => false;

        // PointXYZINormalRGBL


        public virtual ref float3 GetPositionFloat32(TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support PositionFloat32");
        }

        public virtual void SetPositionFloat32(ref TPoint point, float3 val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support PositionFloat64");
        }

        public virtual float3 PositionFloat64(TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support ColorFloat32");
        }

        public virtual float3 NormalFloat32(TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support NormalFloat32");
        }

        public virtual float3 ColorFloat32(TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support ColorFloat32");
        }

        public virtual double3 NormalFloat64(TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support NormalFloat64");
        }

        public virtual double3 ColorFloat64(TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support ColorFloat64");
        }
    }









    // LAZ Reader
    /*
     *  - Öffne File
     *  - Gib PointFormat zurück, was steckt in deiner LAZ-Datei
     *  - Gib mir ein Subset von deinem Vorhandenen zurück
     */

    /*
     * - Anfrage an LAZ-File was kannst du denn und dann
     * - Kreiere Pointcloud mit Accessor-Struktur,
     * - Frage LAZReader ob file alles beinhaltet, wenn ja fülle die Pointcloud
     */

    public interface IPointReader
    {
        PointFormat Format { get; }

        bool ReadNextPoint<TPoint>(ref TPoint point, PointAccessor<TPoint> pa);

        IMeta MetaInfo { get; }

    }

    public class ExamplePointReader : IPointReader
    {
        public PointFormat Format => throw new NotImplementedException();

        public IMeta MetaInfo => throw new NotImplementedException();

        public bool ReadNextPoint<TPoint>(ref TPoint point, PointAccessor<TPoint> pa)
        {
            //var rawPoint = ReadNextRawPoint();

            //if (pa.HasPositionFloat32)
            //    pa.SetPositionFloat32(point, rawPoint.Pos);

            //if (pa.HasNormalFloat32)
            //    pa.SetNormal(point, rawPoint.Pos);

            //if (pa.GetPositionFloat32)
            //    pa.SetPositionFloat32(point, rawPoint.Pos);

            //if (pa.GetPositionFloat32)
            //    pa.SetPositionFloat32(point, rawPoint.Pos);

            return true;
        }
    }

    public struct PointFormat
    {

    }

    //public struct PointFormat
    //{
    //    private readonly List<PointChannel> PointChannels;

    //    public bool HasCategory(PCCat category)
    //    {
    //        foreach (var pc in PointChannels)
    //        {
    //            if (pc.Category == category)
    //                return true;
    //        }
    //        return false;
    //    }

    //    public PointChannel GetChannel(PCCat category) => PointChannels.First(ch => ch.Category == category);

    //    public int PointSize;
    //}

    //public struct PointChannel
    //{
    //    public string Name;
    //    public int StartByte;
    //    public PCCat ChannelCategory;
    //    public PCType ChannelType;
    //    public int Size {
    //        get
    //        {
    //            switch (ChannelType)
    //            {
    //                case PCType.Float3_32: return Marshal.SizeOf<float3>();
    //            }
    //            return -1;
    //        }
    //    }

    //}

    //public enum PCCat
    //{
    //    Position,
    //    Intensity,
    //    Color,
    //    Normal,
    //    Label,
    //    Curvature,
    //    HitCount,
    //    GPSTime,
    //}

    //public enum PCType
    //{
    //    Int_8,
    //    Int_16,
    //    Int_32,
    //    Int_64,
    //    UInt_8,
    //    UInt_16,
    //    UInt_32,
    //    UInt_64,
    //    Float_32,
    //    Float_64,
    //    Float3_32,
    //    Float3_64,
    //}


    //public class PointCloud
    //{
    //    public PointFormat PointFormat;
    //    public byte[] Data;

    //    public Span<byte> SpanData => Data;





    //}



    //[StructLayout(LayoutKind.Sequential)]
    //struct PointChannelTemplate<TPre, TUse, TPost>
    //{
    //    private TPre PrePadding;
    //    public TUse Value;
    //    private TPost PostPadding;
    //}





    internal class ExamplePoint
    {
        public float3 Position;
        public float3 Normal;
    }

    internal class ExamplePointAccessor : PointAccessor<ExamplePoint>
    {
        public override bool HasNormalFloat32 => true;

        public override float3 NormalFloat32(ExamplePoint point)
        {
            return point.Normal;
        }
    }



    //internal class PointCloudMethods
    //{
    //    public static void MyFirstPointCloudAlgorithm(byte[] points, int nPoints, PointFormat pf)
    //    {
    //        if (!pf.HasCategory(PCCat.Normal))
    //            throw new Exception("Normal required");

    //        // Idee 1: Span-Anlegen und Cast im Schleifenrumpf.
    //        for (int i = 0; i < nPoints; i++)
    //        {
    //            var byteSpan = new Span<byte>(points, i * pf.PointSize + pf.GetChannel(PCCat.Normal).StartByte, pf.GetChannel(PCCat.Normal).Size);
    //            float3 val = MemoryMarshal.Read<float3>(byteSpan);
    //            var normal = float3Span[0];

    //        }

    //        // Idee 2:






    //        foreach(var p in WrapPoints(points, pf))
    //        {
    //            // Top
    //            p.Normal

    //            // Plan B
    //            var normal = Normal(p);

    //            // per extension method
    //            var normal = p.Normal();
    //        }
    //    }

    //    public static void MySecondPointCloudAlgorithm(PointCloud pc)
    //    {


    //    }

    public interface IMeta
    {
        // Header info
    }


    internal class PointCloudMethods
    {
        public void ReadPoints(IPointReader pointReader)
        {
            var pa = new ExamplePointAccessor();

            var points = new ExamplePoint[100];

            for (var i = 0; i < 100; i++)
            {
                pointReader.ReadNextPoint(ref points[i], pa);
            }
        }

        public static void AnotherTry<TPoint>(Span<TPoint> points, PointAccessor<TPoint> pa)
        {
            if (!pa.HasNormalFloat32)
                throw new Exception("Normal required");


            // foreach(var point in points)
            for (var i = 0; i < points.Length; i++)
            {
                // Top
                // p.Normal(pa) ?? 

                var oldpos = pa.GetPositionFloat32(points[i]);
                var newpostion = new float3(1, 2, 3);
                pa.SetPositionFloat32(ref points[i], newpostion);


                var normal = pa.NormalFloat32(points[i]);

            }


        }

    }





}
