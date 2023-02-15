using Fusee.Math.Core;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.PointCloud.Common
{
    /// <summary>
    /// Information about the hierarchy data of the given point cloud
    /// </summary>
    public interface IPointWriterHierarchy
    {
        /// <summary>
        /// Size of the first chunk
        /// </summary>
        public int FirstChunkSize { get; set; }

        /// <summary>
        /// Hierarchy step size
        /// </summary>
        public int StepSize { get; set; }

        /// <summary>
        /// Depth of the Hierarchy
        /// </summary>
        public int Depth { get; set; }
    }

    /// <summary>
    /// Metadata about the point cloud
    /// the "attributes" which are needed for e. g. the Potree export can be reconstructed via the <see cref="IPointAccessor" />
    /// </summary>
    public interface IPointWriterMetadata
    {
        /// <summary>
        /// Version flag
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Point cloud name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of point cloud
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// How many points does this point cloud contain
        /// </summary>
        public int PointCount { get; set; }

        /// <summary>
        /// A possible projection method
        /// </summary>
        public string Projection { get; set; }

        /// <summary>
        /// The point cloud hierarchy information 
        /// </summary>
        public IPointWriterHierarchy Hierarchy { get; set; }

        /// <summary>
        /// Global offset of each point
        /// </summary>
        public double3 Offset { get; set; }

        /// <summary>
        /// Global scale value. Points are being converted to int 
        /// During load this scale factor is being applied to convert int to double
        /// </summary>
        public double3 Scale { get; set; }

        /// <summary>
        /// The spacing between points (set during the sampling process)
        /// </summary>
        public double Spacing { get; set; }

        /// <summary>
        /// Global <see cref="AABBd"/> of the point cloud
        /// </summary>
        public AABBd BoundingBox { get; set; }

        /// <summary>
        /// The encoding of every point, as we save the point cloud as <see cref="sbyte"/> elements
        /// Default is <see cref="Encoding.Default"/>
        /// </summary>
        public string Encoding { get; set; }

        /// <summary>
        /// The size of one point in bytes (for sanity checks later on, compare with <see cref="IPointAccessor"/> data)
        /// </summary>
        public int PointSize { get; set; }
    }

    /// <summary>
    /// Every point writer (e. g. Potree, LAS, etc.) implements this interface
    /// </summary>
    public interface IPointWriter
    {
        /// <summary>
        /// Returns the point type.
        /// </summary>
        public PointType PointType { get; }

        /// <summary>
        /// This methods takes a list <see cref="PointType"/>s and converts it to the desired output format and writes the file 
        /// to disk at given <paramref name="savePath"/>
        /// </summary>
        /// <param name="savePath">Path to save to</param>
        /// <param name="points">The point data as <see cref="ReadOnlySpan{T}"/></param>
        /// <param name="metadata">Necessary metadata, e. g. global scale, etc.</param>
        public void WritePointcloudPoints(FileInfo savePath, ReadOnlySpan<PointType> points, IPointWriterMetadata metadata);

        /// <summary>
        /// This methods takes a list of <see cref="PointType"/>s and converts it to the desired output format and writes the file 
        /// to disk at given <paramref name="savePath"/> in an async manner
        /// </summary>
        /// <param name="savePath">Path to save to</param>
        /// <param name="points">The point data as <see cref="ReadOnlyMemory{T}"/>, no <see cref="ReadOnlySpan{T}"/> as ref types are not allowed  
        /// during async operations</param>
        /// <param name="metadata">Necessary metadata, e. g. global scale, etc.</param>
        public Task WritePointcloudPointsAsync(FileInfo savePath, ReadOnlyMemory<PointType> points, IPointWriterMetadata metadata);
    }
}