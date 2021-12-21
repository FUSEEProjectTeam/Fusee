using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Fusee.PointCloud.PotreeReader.V1
{
    /// <summary>
    /// Static class that provides methods to read the meta data from an Potree 1.0 file.
    /// </summary>
    public static class ReadPotreeMetadata
    {
        /// <summary>
        /// Reads the point count of an octant.
        /// </summary>
        /// <param name="fileFolderPath">Path to the file.</param>
        /// <param name="octant">The octant.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static int GetPtCountFromFile<TPoint>(string fileFolderPath, PtOctantRead<TPoint> octant)
        {
            var pathToFile = $"{fileFolderPath}/Octants/{octant.Guid:N}.node";

            if (!File.Exists(pathToFile))
                throw new ArgumentException("File: " + octant.Guid + ".node does not exist!");

            using BinaryReader br = new(File.Open(pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read));
            // step to stream position
            //br.BaseStream.Position = node.StreamPosition;

            // read number of points
            return br.ReadInt32();
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
    }
}
