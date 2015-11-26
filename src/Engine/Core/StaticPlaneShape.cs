using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    public class StaticPlaneShape : CollisionShape
    {
        internal IStaticPlaneShapeImp _staticPlaneShapeImp;

        public float PlaneConstant
        {
            get { return _staticPlaneShapeImp.PlaneConstant; }
        }

        public float3 PlaneNormal
        {
            get
            {
                return _staticPlaneShapeImp.PlaneNormal;
            }
        }

        //Inherited
        public override float Margin
        {

            get
            {
                var retval = _staticPlaneShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (StaticPlaneShape)_staticPlaneShapeImp.UserObject;
                o._staticPlaneShapeImp.Margin = value;
            }
        }
    }
}
