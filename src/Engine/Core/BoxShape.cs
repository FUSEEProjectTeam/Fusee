using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    public class BoxShape : CollisionShape
    {
        internal IBoxShapeImp _boxShapeImp;

        public override float3 LocalScaling
        {
            get { return _boxShapeImp.LocalScaling; }
            set
            {
                var o = (BoxShape)_boxShapeImp.UserObject;
                o._boxShapeImp.LocalScaling = value;
            }
        }
        public float3 HalfExtents
        {
            get
            {
                var retval = _boxShapeImp.HalfExtents;
                return retval;
            }
        }

        //Inherited
        public override float Margin
        {

            get
            {
                var retval = _boxShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (BoxShape) _boxShapeImp.UserObject;
                o._boxShapeImp.Margin = value;
            }
        }
    }
}
