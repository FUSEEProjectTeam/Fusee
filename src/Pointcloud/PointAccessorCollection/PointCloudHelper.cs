using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Reader.LASReader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Fusee.PointCloud.PointAccessorCollections
{
    /// <summary>
    /// Static class that contains helper methods to setup a app that renders point clouds.
    /// </summary>
    public static class PointCloudHelper
    {
        /// <summary>
        /// Reads a given las file into a List.
        /// </summary>
        /// <typeparam name="TPoint">The point type.</typeparam>
        /// <param name="ptAcc">The <see cref="PointAccessor{TPoint}"/></param>
        /// <param name="pathToPc">The path to the las file.</param>
        /// <param name="doExchangeYZ">Determines if the Y and Z components of each point position is exchaned.</param>
        /// <returns></returns>
        public static TPoint[] FromLasToArray<TPoint>(PointAccessor<TPoint> ptAcc, string pathToPc, bool doExchangeYZ) where TPoint : new()
        {
            var reader = new LASPointReader(pathToPc);
            var pointCnt = (MetaInfo)reader.MetaInfo;

            var points = new TPoint[(int)pointCnt.PointCnt];

            for (var i = 0; i < points.Length; i++)
                if (!reader.ReadNextPoint(ref points[i], ptAcc)) break;

            if (ptAcc.HasPositionFloat3_32)
            {
                var firstPoint = ptAcc.GetPositionFloat3_32(ref points[0]);

                for (int i = 0; i < points.Length; i++)
                {
                    var pt = points[i];

                    var pos = ptAcc.GetPositionFloat3_32(ref pt);
                    if (doExchangeYZ)
                        ExchangeYZ(ref pos);
                    pos -= firstPoint;
                    ptAcc.SetPositionFloat3_32(ref pt, pos);

                    points[i] = pt;
                }
            }
            else if (ptAcc.HasPositionFloat3_64)
            {
                var firstPoint = ptAcc.GetPositionFloat3_64(ref points[0]);

                for (int i = 0; i < points.Length; i++)
                {
                    var pt = points[i];

                    var pos = ptAcc.GetPositionFloat3_64(ref pt);
                    if (doExchangeYZ)
                        ExchangeYZ(ref pos);
                    pos -= firstPoint;
                    ptAcc.SetPositionFloat3_64(ref pt, pos);

                    points[i] = pt;
                }
            }
            else
            {
                throw new ArgumentException("Invalid Position type");
            }

            reader.Dispose();
            return points;
        }

        /// <summary>
        /// Reads the point type from the meta.json.
        /// </summary>
        /// <param name="pathToFile">Path to folder of the meta.json file.</param>
        /// <returns></returns>
        public static PointType GetPtTypeFromMetaJson(string pathToFile)
        {
            var pathToMetaJson = pathToFile + "\\meta.json";
            JObject jsonObj;

            using (StreamReader sr = new(pathToMetaJson))
            {
                jsonObj = (JObject)JToken.ReadFrom(new JsonTextReader(sr));
            }

            var jsonPtType = (JValue)jsonObj["pointType"];


            if (Enum.TryParse(jsonPtType.ToString(), out PointType ptType))
                return ptType;
            else
            {
                throw new ArgumentException("Invalid point type!");
            }
        }

        /// <summary>
        /// Exchanges the Y and Z values of a given position of type float3.
        /// </summary>
        /// <param name="pos">The position.</param>
        public static void ExchangeYZ(ref float3 pos)
        {
            var z = pos.z;
            var y = pos.y;
            pos.z = y;
            pos.y = z;
        }

        /// <summary>
        /// Exchanges the Y and Z values of a given position of type double3.
        /// </summary>
        /// <param name="pos">The position.</param>
        public static void ExchangeYZ(ref double3 pos)
        {
            var z = pos.z;
            var y = pos.y;
            pos.z = y;
            pos.y = z;
        }
    }
}