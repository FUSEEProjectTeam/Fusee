using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    public class StaticPlaneShape : CollisionShape
    {
        internal IStaticPlaneShapeImp StaticPlaneShapeImp;

        public float PlaneConstant
        {
            get { return StaticPlaneShapeImp.PlaneConstant; }
        }

        public float3 PlaneNormal
        {
            get
            {
                return StaticPlaneShapeImp.PlaneNormal;
            }
        }

        //Inherited
        public virtual float Margin
        {

            get
            {
                var retval = StaticPlaneShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (StaticPlaneShape)StaticPlaneShapeImp.UserObject;
                o.StaticPlaneShapeImp.Margin = value;
            }
        }
    }
}
