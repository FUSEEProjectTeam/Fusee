using Fusee.Engine.Imp.Physics.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Core
{
    /// <summary>
    /// This is the fastest kind of arbitrary shape. 
    /// It is defined by a cloud of vertices but the shape formed is the smallest convex shape that encloses the vertices. 
    /// For making a convex dynamic shape like a TV, this is ideal. 
    /// Make sure to reduce the number of vertices below say 100.
    /// </summary>
    public class ConvexHullShape : CollisionShape
    {
        internal IConvexHullShapeImp _convexHullShapeImp;

        /// <summary>
        /// Add a vertex to the cloud of vertices.
        /// There can't be added any vertices to the Shape once it has been added to a rigidbody
        /// </summary>
        /// <param name="point">The vertex to add.</param>
        public void AddPoint(float3 point)
        {
            var o = (ConvexHullShape)_convexHullShapeImp.UserObject;
            o._convexHullShapeImp.AddPoint(point);
        }

        /// <summary>
        /// Gets the scaled point by index from the cloud of vertices.
        /// </summary>
        /// <param name="index">The index of the point.</param>
        /// <returns></returns>
        public float3 GetScaledPoint(int index)
        {
            return _convexHullShapeImp.GetScaledPoint(index);
        }

        /// <summary>
        /// Gets the unscaled points of the cloud of vertices.
        /// </summary>
        /// <returns>An array of unscaled vertices.</returns>
        public float3[] GetUnscaledPoints()
        {
            return _convexHullShapeImp.GetUnscaledPoints();
        }

        /// <summary>
        /// Gets the number of points of the cloud of vertices.
        /// </summary>
        /// <returns>The number of points.</returns>
        public int GetNumPoints()
        {
            return _convexHullShapeImp.GetNumPoints();
        }
    }
}
