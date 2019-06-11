using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Pointcloud.Common
{
    public interface IPointReader
    {
        PointFormat Format { get; }

        bool ReadNextPoint<TPoint>(ref TPoint point, PointAccessor<TPoint> pa);

        IMeta MetaInfo { get; }
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



}
