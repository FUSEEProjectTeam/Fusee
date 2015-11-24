using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusee.Math;
using BulletSharp;

namespace Fusee.Engine
{
    public class StaticPlaneShapeImp : CollisonShapeImp, IStaticPlaneShapeImp
    {
        internal StaticPlaneShape BtStaticPlaneShape;
        internal Translater Translater = new Translater();

        public float PlaneConstant
        {
            get { return BtStaticPlaneShape.PlaneConstant; }
        }

        public float3 PlaneNormal
        {
            get
            {
                var retval = Translater.BtVector3ToFloat3(BtStaticPlaneShape.PlaneNormal);
                return retval;
            }
        }

        //Inherited
        public float Margin
        {
            get
            {
                var retval = BtStaticPlaneShape.Margin;
                return retval;
            }
            set
            {
                var o = (StaticPlaneShapeImp)BtStaticPlaneShape.UserObject;
                o.BtStaticPlaneShape.Margin = value;
            }
        }

        private object _userObject;
        public virtual object UserObject
        {
            get { return _userObject; }
            set { _userObject = value; }
        }
    }
}
