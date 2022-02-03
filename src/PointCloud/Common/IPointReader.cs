using System;

namespace Fusee.PointCloud.Common
{
    /// <summary>
    /// Implement this into any Point Cloud Reader.
    /// </summary>
    public interface IPointReader : Engine.Common.IPointReader, IDisposable
    {
        /// <summary>
        /// This is the point description: Which type am I, which variables do I hold?
        /// E.g. PointXYZI with normals
        /// </summary>
        IPointFormat Format { get; }

        /// <summary>
        /// Meta information about the point cloud, usually saved in a header
        /// </summary>
        IPointCloudMetaInfo MetaInfo { get; }

        /// <summary>
        ///     While we have a point read the next with given point accessor
        ///     The "raw" read within this method should happen with one delegate
        /// </summary>
        /// <typeparam name="TPoint">The point type we want to write to</typeparam>
        /// <param name="point">The point we want to write to</param>
        /// <param name="pa">The accessor how to write to the point</param>
        /// <returns></returns>
        bool ReadNextPoint<TPoint>(ref TPoint point, IPointAccessor pa) where TPoint : new();

        /// <summary>
        /// Reads the given amount of points from stream
        /// </summary>
        /// <typeparam name="TPoint"></typeparam>
        /// <param name="n"></param>
        /// <param name="pa"></param>
        /// <returns></returns>
        public TPoint[] ReadNPoints<TPoint>(int n, IPointAccessor pa) where TPoint : new();
    }
}