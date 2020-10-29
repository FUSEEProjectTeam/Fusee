using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Fusee.Pointcloud.PointAccessorCollections
{
    public static class AppSetupHelper
    {
        public delegate void AppSetupDelegate();

        public static PointType GetPtType(string pathToFile)
        {
            var pathToMetaJson = pathToFile + "\\meta.json";
            JObject jsonObj;

            using (StreamReader sr = new StreamReader(pathToMetaJson))
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

    }
}