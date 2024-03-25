using Fusee.Math.Core;
using System.IO;
using System.Text;

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
    /// Metadata about the point cloud the "attributes" which are needed for e. g. the LAS export.
    /// </summary>
    public interface IPointWriterMetadata
    {
        /// <summary>
        /// Version flag
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Point cloud name
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// Description of point cloud
        /// </summary>
        public string? Description { get; }

        /// <summary>
        /// How many points does this point cloud contain
        /// </summary>
        public int PointCount { get; }

        /// <summary>
        /// A possible projection method
        /// </summary>
        public string? Projection { get; }

        /// <summary>
        /// The point cloud hierarchy information
        /// </summary>
        public IPointWriterHierarchy? Hierarchy { get; }

        /// <summary>
        /// Global offset of each point
        /// </summary>
        public double3 Offset { get; }

        /// <summary>
        /// Global scale value. Points are being converted to int
        /// During load this scale factor is being applied to convert int to double
        /// </summary>
        public double3 Scale { get; }

        /// <summary>
        /// The spacing between points (set during the sampling process)
        /// </summary>
        public double Spacing { get; }

        /// <summary>
        /// Global <see cref="AABBd"/> of the point cloud
        /// These values are not yet y/z flipped
        /// </summary>
        public AABBd AABB { get; }

        /// <summary>
        /// The encoding of every point, as we save the point cloud as <see cref="sbyte"/> elements
        /// Default is <see cref="Encoding.Default"/>
        /// </summary>
        public string? Encoding { get; }

        /// <summary>
        /// The size of one point in bytes (for sanity checks later on, compare with e. g. LASPoint type data)
        /// </summary>
        public int PointSize { get; }
    }

    /// <summary>
    /// Every point writer (e. g. Potree, LAS, etc.) implements this interface
    /// </summary>
    public interface IPointWriter
    {
        /// <summary>
        /// The necessary metadata
        /// </summary>
        IPointWriterMetadata? Metadata { get; }

        /// <summary>
        /// The file to write to
        /// </summary>
        FileInfo? SavePath { get; }
    }
}