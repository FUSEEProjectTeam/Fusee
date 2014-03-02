using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;

namespace Fusee.Engine
{
    public class GImpactMeshShapeImp : CollisonShapeImp , IGImpactMeshShapeImp
    {
        internal GImpactMeshShape BtGImpactMeshShape;

        //Inherited
        public virtual float Margin
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

        private object _userObject;
        public virtual object UserObject
        {
            get { return _userObject; }
            set { _userObject = value; }
        }
    }
}
