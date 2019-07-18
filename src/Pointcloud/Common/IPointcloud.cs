using System;

namespace Fusee.Pointcloud.Common
{

    /// <summary>
    ///     A point cloud consists of a point accessor which enables access to the 
    ///     as well as some information about the point type.
    ///     Furthermore the data itself as well as some meta information like offset information.
    /// </summary>   
    /// <typeparam name="TPoint">Point type</typeparam>
    public interface IPointcloud<TPoint>
    {
        PointAccessor<TPoint> Pa { get; }

        Span<TPoint> Points { get; }

        IMeta MetaInfo { get; }
    }
}
