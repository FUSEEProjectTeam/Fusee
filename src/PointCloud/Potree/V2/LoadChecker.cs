using CommunityToolkit.Diagnostics;
using Fusee.Base.Core;
using Fusee.PointCloud.Potree.V2.Data;
using Newtonsoft.Json;
using System.IO;

namespace Fusee.PointCloud.Potree.V2
{
    //Todo: implement checks

    /// <summary>
    /// Collection of methods to check files and loading capabilities
    /// </summary>
    public static class LoadChecker
    {
        /// <summary>
        /// Check if a potree folder can be loaded
        /// </summary>
        /// <param name="pathToNodeFileFolder"></param>
        /// <param name="ignoreVersion"></param>
        /// <returns></returns>
        public static bool CanHandleFile(string pathToNodeFileFolder, bool ignoreVersion = false)
        {
            var hierarchyFilePath = Path.Combine(pathToNodeFileFolder, Potree2Consts.HierarchyFileName);
            var metadataFilePath = Path.Combine(pathToNodeFileFolder, Potree2Consts.MetadataFileName);
            var octreeFilePath = Path.Combine(pathToNodeFileFolder, Potree2Consts.OctreeFileName);

            var canHandle = true;

            if (File.Exists(metadataFilePath))
            {
                try
                {
                    var metadata = JsonConvert.DeserializeObject<PotreeMetadata>(File.ReadAllText(metadataFilePath));
                    Guard.IsNotNull(metadata);

                    if (!ignoreVersion && metadata.Version != "2.0")
                    {
                        canHandle = false;
                        Diagnostics.Warn("File metadata indicates unsupported version. Metadata version: " + metadata.Version + ", supportet version: 2.0");
                    }

                    if (!metadata.Encoding.Contains("DEFAULT"))
                    {
                        canHandle = false;
                        Diagnostics.Warn("Non-default encoding is not supported!");
                    }
                }
                catch
                {
                    canHandle = false;
                    Diagnostics.Warn("Cannot parse " + metadataFilePath);
                }
            }
            else
            {
                canHandle = false;
                Diagnostics.Warn("File does not exist " + metadataFilePath);
            }

            if (!File.Exists(hierarchyFilePath))
            {
                canHandle = false;
                Diagnostics.Warn("File does not exist " + hierarchyFilePath);
            }

            if (!File.Exists(octreeFilePath))
            {
                canHandle = false;
                Diagnostics.Warn("File does not exist " + octreeFilePath);
            }

            return canHandle;
        }
    }
}