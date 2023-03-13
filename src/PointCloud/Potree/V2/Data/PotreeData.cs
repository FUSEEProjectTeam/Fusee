using System;
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
        }

        private bool disposedValue;

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    OctreeMappedFile.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}