using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using Fusee.Math;

namespace Fusee.Engine
{
    public class ConvexHullShapeImp : CollisonShapeImp, IConvexHullShapeImp
    {
        internal ConvexHullShape BtConvexHullShape;
        internal Translater Translater = new Translater();

        public float Margin
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

        private object _userObject;
        public virtual object UserObject
        {
            get { return _userObject; }
            set { _userObject = value; }
        }

        public float3 LocalScaling
        {
            get
            {
                var retval = Translater.BtVector3ToFloat3(BtConvexHullShape.LocalScaling);
                return retval;
            }
            set
            {
                var o = (ConvexHullShapeImp)BtConvexHullShape.UserObject;
                o.BtConvexHullShape.LocalScaling = Translater.Float3ToBtVector3(value);
            }
        }

        public void AddPoint(float3 point)
        {
            var btPoint = Translater.Float3ToBtVector3(point);
            BtConvexHullShape.AddPoint(btPoint, true);
        }

        public float3 GetScaledPoint(int index)
        {
            var retval = Translater.BtVector3ToFloat3(BtConvexHullShape.GetScaledPoint(index));
            return retval;
        }

        public float3[] GetUnscaledPoints()
        {
            var retval = new float3[BtConvexHullShape.NumPoints];
            var btPoints = BtConvexHullShape.UnscaledPoints;
            for (int i = 0; i < BtConvexHullShape.NumPoints; i++)
            {
                retval[i] = Translater.BtVector3ToFloat3(btPoints[i]);
            }
            return retval;
        }

        public int GetNumPoints()
        {
            return BtConvexHullShape.NumPoints;
        }
    }
}
