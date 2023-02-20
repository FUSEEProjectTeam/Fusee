using CommunityToolkit.Diagnostics;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Potree.V2.Data;
using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace Fusee.PointCloud.Potree.V2
{
    //TODO: cleanup fields/methods that arent necessary for the writer but are present due to this classes historic relation to the reader.
    /// <summary>
    /// This is the base class for reading and writing <see cref="PotreePoint"/>s.
    /// </summary>
    public abstract class Potree2WriterBase : IDisposable
    {
        /// <summary>
        /// The <see cref="V2.PotreeData"/>
        /// </summary>
        public PotreeData? PotreeData
        {
            get => _potreeData;
            private set
            {
                _potreeData = value;
            }
        }
        private PotreeData? _potreeData;

        /// <summary>
        /// Save if metadata has already been cached
        /// </summary>
        protected bool isMetadataCached = false;

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

        private string? _octreeFilePath;
        private MemoryMappedFile? _octreeMappedFile;
        private MemoryMappedViewAccessor? _octreeViewAccessor;
        private bool disposedValue;

        /// <summary>
        /// The <see cref="MemoryMappedViewAccessor"/> to the underlying octree.bin file
        /// </summary>
        protected MemoryMappedViewAccessor OctreeMappedViewAccessor
        {
            get
            {
                Guard.IsNotNull(_octreeViewAccessor, nameof(_octreeViewAccessor));
                Guard.IsNotNull(_octreeMappedFile, nameof(_octreeMappedFile));

                return _octreeViewAccessor;

            }
        }

        /// <summary>
        /// The path to the octree.bin file
        /// On assign the <see cref="MemoryMappedFile"/> and the <see cref="MemoryMappedViewAccessor"/> is being created.
        /// </summary>
        protected string OctreeFilePath
        {
            set
            {
                Guard.IsNotNullOrEmpty(value, nameof(value));
                Guard.IsTrue(File.Exists(value));

                _octreeFilePath = value;

                _octreeMappedFile?.Dispose();
                _octreeViewAccessor?.Dispose();

                _octreeMappedFile = MemoryMappedFile.CreateFromFile(_octreeFilePath, FileMode.Open);
                _octreeViewAccessor = _octreeMappedFile.CreateViewAccessor();
            }
            get
            {
                Guard.IsNotNullOrEmpty(_octreeFilePath);
                return _octreeFilePath;
            }
        }

        /// <summary>
        /// Ctor for RW base. Reads and chaches metadata and sets the path to the octree.bin file
        /// </summary>
        public Potree2WriterBase(PotreeData potreeData)
        {
            PotreeData = potreeData;
        }

        /// <summary>
        /// Returns the point type.
        /// </summary>
        public PointType PointType => PointType.PosD3ColF3LblB;


        /// <summary>
        /// Read and cache metadata from the metadata.json file
        /// </summary>
        protected void CacheMetadata()
        {
            if (!isMetadataCached)
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

                isMetadataCached = true;
            }
        }

        /// <summary>
        /// Iterate the hierarchy, find the node from the given <see cref="OctantId"/>.
        /// </summary>
        /// <param name="potreeHierarchy"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static PotreeNode FindNode(ref PotreeHierarchy potreeHierarchy, OctantId id)
        {
            return potreeHierarchy.Nodes.Find(n => n.Name == OctantId.OctantIdToPotreeName(id));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _octreeMappedFile?.Dispose();
                    _octreeViewAccessor?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}