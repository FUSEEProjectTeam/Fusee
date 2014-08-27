using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    /// <summary>
    /// This is the fastest kind of arbitrary shape. 
    /// It is defined by a cloud of vertices but the shape formed is the smallest convex shape that encloses the vertices. 
    /// For making a convex dynamic shape like a TV, this is ideal. 
    /// Make sure to reduce the number of vertices below say 100.
    /// </summary>
    public class ConvexHullShape : CollisionShape
    {
        internal IConvexHullShapeImp ConvexHullShapeImp;

        /// <summary>
        /// Add a vertice to the cloud of vertices.
        /// There can't be added any vertices to the Shape once it has been added to a rigidbody
        /// </summary>
        /// <param name="point"></param>
        public void AddPoint(float3 point)
        {
            var o = (ConvexHullShape)ConvexHullShapeImp.UserObject;
            o.ConvexHullShapeImp.AddPoint(point);
        }


        public float3 GetScaledPoint(int index)
        {
            return ConvexHullShapeImp.GetScaledPoint(index);
        }

        public float3[] GetUnscaledPoints()
        {
            return ConvexHullShapeImp.GetUnscaledPoints();
        }

        public int GetNumPoints()
        {
            return ConvexHullShapeImp.GetNumPoints();
        }

        public float Margin
        {
            get
            {
                var retval = ConvexHullShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (ConvexHullShape)ConvexHullShapeImp.UserObject;
                o.ConvexHullShapeImp.Margin = value;
            }
        }



        public float3 LocalScaling
        {
            get { return ConvexHullShapeImp.LocalScaling; }
            set
            {
                var o = (ConvexHullShape)ConvexHullShapeImp.UserObject;
                o.ConvexHullShapeImp.LocalScaling = value;
            }
        }
    }
}
