using CommunityToolkit.Diagnostics;
using Fusee.PointCloud.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace Fusee.PointCloud.Potree.V2.Data
{
    /// <summary>
    /// Contains information about the Potree file's meta data and hierarchy/octree.
    /// </summary>
    public class PotreeData : IDisposable
    {
        /// <summary>
        /// The hierarchy as linear list of <see cref="PotreeNode"/>s.
        /// </summary>
        public PotreeHierarchy Hierarchy;

        /// <summary>
        /// The meta data of the file.
        /// </summary>
        public PotreeMetadata Metadata;

        /// <summary>
        /// Returns a reference to the memory mapped file for octree.bin.
        /// </summary>
        internal MemoryMappedFile OctreeMappedFile { get; }

        /// <summary>
        /// Returns a reference to the memory mapped file view accessor for reading.
        /// </summary>
        internal MemoryMappedViewAccessor ReadViewAccessor { get; }

        /// <summary>
        /// Returns a reference to the memory mapped file view accessor for writing.
        /// </summary>
        internal MemoryMappedViewAccessor WriteViewAccessor { get; }


        /// <summary>
        /// Creats a new instance of PotreeData
        /// </summary>
        /// <param name="potreeHierarchy"></param>
        /// <param name="potreeMetadata"></param>
        public PotreeData(PotreeHierarchy potreeHierarchy, PotreeMetadata potreeMetadata)
        {
            Hierarchy = potreeHierarchy;
            Metadata = potreeMetadata;

            var path = Path.Combine(Metadata.FolderPath, Potree2Consts.OctreeFileName);

            OctreeMappedFile = MemoryMappedFile.CreateFromFile(path, FileMode.Open);
            ReadViewAccessor = OctreeMappedFile.CreateViewAccessor();
            WriteViewAccessor = OctreeMappedFile.CreateViewAccessor();

            Guard.IsTrue(CheckAllFileStreamsValidity());
        }

        /// <summary>
        /// Returns the node for a given <see cref="OctantId"/>.
        /// </summary>
        /// <param name="octantId"></param>
        /// <returns></returns>
        public PotreeNode? GetNode(OctantId octantId)
        {
            return Hierarchy.Nodes.Find(n => n.OctantId == octantId);
        }

        /// <summary>
        /// Returns the node for a given name of a <see cref="PotreeNode"/>.
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public PotreeNode? GetNode(string nodeName)
        {
            var octantId = new OctantId(nodeName);

            return GetNode(octantId);
        }

        /// <summary>
        /// Checks if all filestreams are open and valid
        /// </summary>
        /// <returns><see langword="false"/>if any errors are present</returns>
        public bool CheckAllFileStreamsValidity()
        {
            return !OctreeMappedFile.SafeMemoryMappedFileHandle.IsInvalid &&
                !OctreeMappedFile.SafeMemoryMappedFileHandle.IsClosed &&
                ReadViewAccessor.CanRead && WriteViewAccessor.CanWrite;
        }

        #region IDisposable

        private bool disposedValue;

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ReadViewAccessor.Flush();
                    WriteViewAccessor.Flush();
                    ReadViewAccessor.Dispose();
                    WriteViewAccessor.Dispose();
                    OctreeMappedFile.Dispose();
                }

                disposedValue = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable
    }
}