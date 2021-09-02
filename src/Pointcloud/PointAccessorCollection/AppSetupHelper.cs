using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Fusee.PointCloud.PointAccessorCollections
{
    /// <summary>
    /// Static class that contains helper methods to setup a app that renders point clouds.
    /// </summary>
    public static class AppSetupHelper
    {
        /// <summary>
        /// Reads the point type from the meta.json.
        /// </summary>
        /// <param name="pathToFile">Path to folder of the meta.json file.</param>
        /// <returns></returns>
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