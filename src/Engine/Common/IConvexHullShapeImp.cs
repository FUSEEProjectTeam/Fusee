using Fusee.Math.Core;

namespace Fusee.Engine.Common
{
    /// <summary>
    /// Implementation agnostic representation of a convex hull shape.
    /// </summary>
    public interface IConvexHullShapeImp : ICollisionShapeImp
    {
        /// <summary>
        /// Adds the point.
        /// </summary>
        /// <param name="point">The point.</param>
        void AddPoint(float3 point);
        /// <summary>
        /// Gets the scaled point.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        float3 GetScaledPoint(int index);
        /// <summary>
        /// Gets the unscaled points.
        /// </summary>
        /// <returns>An array of unscaled vertices.</returns>
        float3[] GetUnscaledPoints();
        /// <summary>
        /// Gets the number of points.
        /// </summary>
        /// <returns>The number of points.</returns>
        int GetNumPoints();
    }
}
