using Fusee.PointCloud.Common;
using Fusee.PointCloud.Common.Accessors;
using Fusee.PointCloud.Core.Accessors;
using Fusee.PointCloud.Potree.V2.Data;
using System.IO;

namespace Fusee.PointCloud.Potree.V2
{
    public abstract class Potree2RwBase
    {
        protected PotreeData _potreeData;

        protected bool cachedMetadata = false;

        protected int offsetPosition = -1;
        protected int offsetIntensity = -1;
        protected int offsetReturnNumber = -1;
        protected int offsetNumberOfReturns = -1;
        protected int offsetClassification = -1;
        protected int offsetScanAngleRank = -1;
        protected int offsetUserData = -1;
        protected int offsetPointSourceId = -1;
        protected int offsetColor = -1;

        protected string OctreeFilePath => Path.Combine(_potreeData.Metadata.FolderPath, Potree2Consts.OctreeFileName);

        public Potree2RwBase(ref PotreeData potreeData)
        {
            _potreeData = potreeData;
            PointAccessor = new PosD3ColF3LblBAccessor();

            CacheMetadata();
        }

        /// <summary>
        /// Returns the point type.
        /// </summary>
        public PointType PointType => PointType.PosD3ColF3LblB;

        /// <summary>
        /// A PointAccessor allows access to the point information (position, color, ect.) without casting it to a specific <see cref="PointType"/>.
        /// </summary>
        public IPointAccessor PointAccessor { get; protected set; }

        protected void CacheMetadata()
        {
            if (!cachedMetadata)
            {
                if (_potreeData.Metadata.Attributes.ContainsKey("position"))
                {
                    offsetPosition = _potreeData.Metadata.Attributes["position"].AttributeOffset;
                }
                if (_potreeData.Metadata.Attributes.ContainsKey("intensity"))
                {
                    offsetIntensity = _potreeData.Metadata.Attributes["intensity"].AttributeOffset;
                }
                if (_potreeData.Metadata.Attributes.ContainsKey("return number"))
                {
                    offsetReturnNumber = _potreeData.Metadata.Attributes["return number"].AttributeOffset;
                }
                if (_potreeData.Metadata.Attributes.ContainsKey("number of returns"))
                {
                    offsetNumberOfReturns = _potreeData.Metadata.Attributes["number of returns"].AttributeOffset;
                }
                if (_potreeData.Metadata.Attributes.ContainsKey("classification"))
                {
                    offsetClassification = _potreeData.Metadata.Attributes["classification"].AttributeOffset;
                }
                if (_potreeData.Metadata.Attributes.ContainsKey("scan angle rank"))
                {
                    offsetScanAngleRank = _potreeData.Metadata.Attributes["scan angle rank"].AttributeOffset;
                }
                if (_potreeData.Metadata.Attributes.ContainsKey("user data"))
                {
                    offsetUserData = _potreeData.Metadata.Attributes["user data"].AttributeOffset;
                }
                if (_potreeData.Metadata.Attributes.ContainsKey("point source id"))
                {
                    offsetPointSourceId = _potreeData.Metadata.Attributes["point source id"].AttributeOffset;
                }
                if (_potreeData.Metadata.Attributes.ContainsKey("rgb"))
                {
                    offsetColor = _potreeData.Metadata.Attributes["rgb"].AttributeOffset;
                }

                int pointSize = 0;

                if (_potreeData.Metadata != null)
                {
                    foreach (var metaAttributeItem in _potreeData.Metadata.AttributesList)
                    {
                        pointSize += metaAttributeItem.Size;
                    }

                    _potreeData.Metadata.PointSize = pointSize;
                }

                cachedMetadata = true;
            }
        }

        public static PotreeNode FindNode(ref PotreeHierarchy potreeHierarchy, OctantId id)
        {
            return potreeHierarchy.Nodes.Find(n => n.Name == OctantId.OctantIdToPotreeName(id));
        }
    }
}