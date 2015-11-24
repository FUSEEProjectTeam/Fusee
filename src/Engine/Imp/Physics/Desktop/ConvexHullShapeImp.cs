using BulletSharp;
using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Desktop
{
    /// <summary>
    /// Implementation of the <see cref="IConvexHullShapeImp" /> interface using the bullet physics engine.
    /// </summary>
    public class ConvexHullShapeImp : CollisonShapeImp, IConvexHullShapeImp
    {
        internal ConvexHullShape BtConvexHullShape;

        /// <summary>
        /// Gets or sets the margin.
        /// </summary>
        /// <value>
        /// The margin.
        /// </value>
        public override float Margin
        {
            get
            {
                var retval = BtConvexHullShape.Margin;
                return retval;
            }
            set
            {
                var o = (ConvexHullShapeImp)BtConvexHullShape.UserObject;
                o.BtConvexHullShape.Margin = value;
            }
        }

        /// <summary>
        /// Gets or sets the local scaling.
        /// </summary>
        /// <value>
        /// The local scaling.
        /// </value>
        public override float3 LocalScaling
        {
            get
            {
                var retval = Translator.BtVector3ToFloat3(BtConvexHullShape.LocalScaling);
                return retval;
            }
            set
            {
                var o = (ConvexHullShapeImp)BtConvexHullShape.UserObject;
                o.BtConvexHullShape.LocalScaling = Translator.Float3ToBtVector3(value);
            }
        }

        /// <summary>
        /// Adds the point.
        /// </summary>
        /// <param name="point">The point.</param>
        public void AddPoint(float3 point)
        {
            var btPoint = Translator.Float3ToBtVector3(point);
            BtConvexHullShape.AddPoint(btPoint, true);
        }

        /// <summary>
        /// Gets the scaled point.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public float3 GetScaledPoint(int index)
        {
            var retval = Translator.BtVector3ToFloat3(BtConvexHullShape.GetScaledPoint(index));
            return retval;
        }

        /// <summary>
        /// Gets the unscaled points.
        /// </summary>
        /// <returns></returns>
        public float3[] GetUnscaledPoints()
        {
            var retval = new float3[BtConvexHullShape.NumPoints];
            var btPoints = BtConvexHullShape.UnscaledPoints;
            for (int i = 0; i < BtConvexHullShape.NumPoints; i++)
            {
                retval[i] = Translator.BtVector3ToFloat3(btPoints[i]);
            }
            return retval;
        }

        /// <summary>
        /// Gets the number points.
        /// </summary>
        /// <returns></returns>
        public int GetNumPoints()
        {
            return BtConvexHullShape.NumPoints;
        }
    }
}
