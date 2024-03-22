using CommunityToolkit.Diagnostics;
using Fusee.PointCloud.Potree.V2.Data;
using System.IO.MemoryMappedFiles;

namespace Fusee.PointCloud.Potree.V2
{
    /// <summary>
    /// This is the base class for accessing Potree files
    /// </summary>
    public abstract class Potree2AccessBase
    {
        /// <summary>
        /// The <see cref="Data.PotreeData"/>
        /// </summary>
        public PotreeData? PotreeData { get; set; }

        /// <summary>
        /// Constructs a new instance of Potree2AccessBase
        /// </summary>
        /// <param name="potreeData"></param>
        public Potree2AccessBase(PotreeData potreeData)
        {
            PotreeData = potreeData;
        }

        /// <summary>
        /// Constructs a new instance of Potree2AccessBase
        /// </summary>
        protected Potree2AccessBase() { }

        /// <summary>
        /// Returns the raw data of a <see cref="PotreeNode"/>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal MemoryMappedFile ReadRawNodeData(PotreeNode node)
        {
            Guard.IsLessThanOrEqualTo(node.NumPoints, int.MaxValue);
            Guard.IsNotNull(PotreeData);

            var nodeSize = (int)node.NumPoints * PotreeData.Metadata.PointSize;
            var pointArray = new byte[nodeSize];
            PotreeData.ReadViewAccessor.ReadArray(node.ByteOffset, pointArray, 0, nodeSize);

            var mmf = MemoryMappedFile.CreateNew(null, nodeSize);
            using var accessor = mmf.CreateViewAccessor();
            accessor.WriteArray(0, pointArray, 0, pointArray.Length);

            return mmf;
        }

        #region Metadata caching

        /// <summary>
        /// Save if metadata has already been cached
        /// </summary>
        protected bool _isMetadataCached = false;

        /// <summary>
        /// Offset in bytes to the position value in bytes in raw Potree stream
        /// </summary>
        protected int offsetPosition = -1;
        /// <summary>
        /// Offset in bytes to the intensity value in bytes in raw Potree stream
        /// </summary>
        protected int offsetIntensity = -1;
        /// <summary>
        /// Offset in bytes to the return number value in bytes in raw Potree stream
        /// </summary>
        protected int offsetReturnNumber = -1;
        /// <summary>
        /// Offset in bytes to the number of returns value in bytes in raw Potree stream
        /// </summary>
        protected int offsetNumberOfReturns = -1;
        /// <summary>
        /// Offset in bytes to the classification value in bytes in raw Potree stream
        /// </summary>
        protected int offsetClassification = -1;
        /// <summary>
        /// Offset in bytes to the scan angle rank value in bytes in raw Potree stream
        /// </summary>
        protected int offsetScanAngleRank = -1;
        /// <summary>
        /// Offset in bytes to the user data value in bytes in raw Potree stream
        /// </summary>
        protected int offsetUserData = -1;
        /// <summary>
        /// Offset in bytes to the point source id value in bytes in raw Potree stream
        /// </summary>
        protected int offsetPointSourceId = -1;
        /// <summary>
        /// Offset in bytes to the color value in bytes in raw Potree stream
        /// </summary>
        protected int offsetColor = -1;

        /// <summary>
        /// Read and cache metadata from the metadata.json file
        /// </summary>
        protected void CacheMetadata(bool force = false)
        {
            Guard.IsNotNull(PotreeData);

            if (!_isMetadataCached || force)
            {
                offsetPosition = -1;
                offsetIntensity = -1;
                offsetReturnNumber = -1;
                offsetNumberOfReturns = -1;
                offsetClassification = -1;
                offsetScanAngleRank = -1;
                offsetUserData = -1;
                offsetPointSourceId = -1;
                offsetColor = -1;

                if (PotreeData.Metadata.Attributes.TryGetValue("position", out var position))
                {
                    offsetPosition = position.AttributeOffset;
                }
                if (PotreeData.Metadata.Attributes.TryGetValue("intensity", out var intensity))
                {
                    offsetIntensity = intensity.AttributeOffset;
                }
                if (PotreeData.Metadata.Attributes.TryGetValue("return number", out var returnNumber))
                {
                    offsetReturnNumber = returnNumber.AttributeOffset;
                }
                if (PotreeData.Metadata.Attributes.TryGetValue("number of returns", out var numberOfReturns))
                {
                    offsetNumberOfReturns = numberOfReturns.AttributeOffset;
                }
                if (PotreeData.Metadata.Attributes.TryGetValue("classification", out var classification))
                {
                    offsetClassification = classification.AttributeOffset;
                }
                if (PotreeData.Metadata.Attributes.TryGetValue("scan angle rank", out var scanAngleRank))
                {
                    offsetScanAngleRank = scanAngleRank.AttributeOffset;
                }
                if (PotreeData.Metadata.Attributes.TryGetValue("user data", out var userData))
                {
                    offsetUserData = userData.AttributeOffset;
                }
                if (PotreeData.Metadata.Attributes.TryGetValue("point source id", out var pointSourceId))
                {
                    offsetPointSourceId = pointSourceId.AttributeOffset;
                }
                if (PotreeData.Metadata.Attributes.TryGetValue("rgb", out var rgb))
                {
                    offsetColor = rgb.AttributeOffset;
                }

                int pointSize = 0;

                if (PotreeData.Metadata != null)
                {
                    foreach (var metaAttributeItem in PotreeData.Metadata.AttributesList)
                    {
                        pointSize += metaAttributeItem.Size;
                    }

                    PotreeData.Metadata.PointSize = pointSize;
                }

                _isMetadataCached = true;
            }
        }

        #endregion Metadata caching
    }
}