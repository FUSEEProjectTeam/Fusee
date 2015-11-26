using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
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


        public float3 GetScaledPoint(int index)
        {
            return _convexHullShapeImp.GetScaledPoint(index);
        }

        public float3[] GetUnscaledPoints()
        {
            return _convexHullShapeImp.GetUnscaledPoints();
        }

        public int GetNumPoints()
        {
            return _convexHullShapeImp.GetNumPoints();
        }
    }
}
