using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Pointcloud.Common
{
    public interface IPointFormat
    {
    }

    /// <summary>
    ///     This is the meta info about one pointcloud, normally stored in the header of a pointcloud 
    /// </summary>
    public interface IMeta
    {
        // e.g.
        // AABBd AABB { get; }
    }

    public interface IPointReader
    {
        /// <summary>
        ///     This is the point description: Which type am I, which variables do I hold?
        ///     E.g. PointXYZI with normals
        /// </summary>
        IPointFormat Format { get; }

        /// <summary>
        ///     Meta information about the pointcloud, usually saved in a header
        /// </summary>
        IMeta MetaInfo { get; }

        /// <summary>
        ///     While we have a point read the next with given point accessor
        ///     The "raw" read within this method should happen with one delegate
        /// </summary>
        /// <typeparam name="TPoint">The point type we want to write to</typeparam>
        /// <param name="point">The point we want to write to</param>
        /// <param name="pa">The accessor how to write to the point</param>
        /// <returns></returns>
        bool ReadNextPoint<TPoint>(ref TPoint point, PointAccessor<TPoint> pa);
    }
}
