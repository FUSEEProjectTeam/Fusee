using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using Fusee.Math;

namespace Fusee.Engine
{
    public class CylinderShapeImp : CollisonShapeImp, ICylinderShapeImp
    {
        internal CylinderShape BtCylinderShape;
        public float Margin
        {
            get
            {
                var retval = BtCylinderShape.Margin;
                return retval;
            }
            set
            {
                var o = (CylinderShapeImp)BtCylinderShape.UserObject;
                o.BtCylinderShape.Margin = value;
            }
        }

        public float3 LocalScaling
        {
            get
            {
                var retval = Translater.BtVector3ToFloat3(BtCylinderShape.LocalScaling);
                return retval;
            }
            set
            {
                var o = (CylinderShapeImp)BtCylinderShape.UserObject;
                o.BtCylinderShape.LocalScaling = Translater.Float3ToBtVector3(value);
            }
        }

        private object _userObject;
        public virtual object UserObject
        {
            get { return _userObject; }
            set { _userObject = value; }
        }


        public float3 HalfExtents
        {
            get
            {
                var retval = new float3(BtCylinderShape.HalfExtentsWithMargin.X, BtCylinderShape.HalfExtentsWithMargin.Y, BtCylinderShape.HalfExtentsWithMargin.Z);
                return retval;
            }
        }

        public float Radius
        {
            get
            {
                var retval = BtCylinderShape.Radius;
                return retval;
            }
        }

        public int UpAxis
        {
            get
            {
                var retval = BtCylinderShape.UpAxis;
                return retval;
            }
        }
    }
}
