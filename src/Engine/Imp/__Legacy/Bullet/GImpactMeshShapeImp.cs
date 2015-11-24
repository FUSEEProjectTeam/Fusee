using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using Fusee.Math;

namespace Fusee.Engine
{
    public class GImpactMeshShapeImp : CollisonShapeImp , IGImpactMeshShapeImp
    {
        internal GImpactMeshShape BtGImpactMeshShape;

        //Inherited
        public float Margin
        {
            get
            {
                var retval = BtGImpactMeshShape.Margin;
                return retval;
            }
            set
            {
                var o = (GImpactMeshShapeImp)BtGImpactMeshShape.UserObject;
                o.BtGImpactMeshShape.Margin = value;
            }
        }
        public float3 LocalScaling
        {
            get
            {
                var retval = Translater.BtVector3ToFloat3(BtGImpactMeshShape.LocalScaling);
                return retval;
            }
            set
            {
                var o = (GImpactMeshShapeImp)BtGImpactMeshShape.UserObject;
                o.BtGImpactMeshShape.LocalScaling = Translater.Float3ToBtVector3(value);
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
