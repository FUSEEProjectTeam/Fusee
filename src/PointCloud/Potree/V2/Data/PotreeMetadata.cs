using Fusee.Math.Core;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Fusee.PointCloud.Potree.V2.Data
{
    public class PotreeSettingsHierarchy
    {
        public int FirstChunkSize { get; set; }
        public int StepSize { get; set; }
        public int Depth { get; set; }
    }

    public class PotreeSettingsBoundingBox
    {
        [JsonProperty(PropertyName = "min")]
        public List<double> MinList { get; set; }
        [JsonIgnore]
        public double3 Min => new(MinList[0], MinList[1], MinList[2]);

        [JsonProperty(PropertyName = "max")]
        public List<double> MaxList { get; set; }
        [JsonIgnore]
        public double3 Max => new(MaxList[0], MaxList[1], MaxList[2]);
    }

    public class PotreeSettingsAttribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Size { get; set; }
        public int NumElements { get; set; }
        public int ElementSize { get; set; }
        public string Type { get; set; }

        [JsonProperty(PropertyName = "min")]
        public List<double> MinList { get; set; }
        [JsonIgnore]
        public double3 Min => new(MinList[0], MinList[1], MinList[2]);

        [JsonProperty(PropertyName = "max")]
        public List<double> MaxList { get; set; }
        [JsonIgnore]
        public double3 Max => new(MaxList[0], MaxList[1], MaxList[2]);
    }

    public class PotreeMetadata
    {
        public string Version { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Points { get; set; }
        public string Projection { get; set; }
        public PotreeSettingsHierarchy Hierarchy { get; set; }

        [JsonProperty(PropertyName = "offset")]
        public List<double> OffsetList { get; set; }
        [JsonIgnore]
        public double3 Offset => new(OffsetList[0], OffsetList[1], OffsetList[2]);

        [JsonProperty(PropertyName = "scale")]
        public List<double> ScaleList { get; set; }
        [JsonIgnore]
        public double3 Scale => new(ScaleList[0], ScaleList[1], ScaleList[2]);

        public double Spacing { get; set; }
        public PotreeSettingsBoundingBox BoundingBox { get; set; }
        public string Encoding { get; set; }
        public List<PotreeSettingsAttribute> Attributes { get; set; }

        [JsonIgnore]
        public int PointSize { get; set; }

        [JsonIgnore]
        public string FolderPath { get; set; }
    }
}
