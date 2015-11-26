using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    public class ConeShape : CollisionShape
    {
        internal IConeShapeImp _coneShapeImp;

        public override float Margin
        {
            get
            {
                var retval = _coneShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (ConeShape)_coneShapeImp.UserObject;
                o._coneShapeImp.Margin = value;
            }
        }

        public override float3 LocalScaling
        {
            get
            {
                var retval = _coneShapeImp.LocalScaling;
                return retval;
            }
            set
            {
                var o = (ConeShape) _coneShapeImp.UserObject;
                o._coneShapeImp.LocalScaling = value;
            }
        }

        public int ConeUpIndex
        {
            get
            {
                var retval = _coneShapeImp.ConeUpIndex;
                return retval;
            }
            set
            {
                var o = (ConeShape) _coneShapeImp.UserObject;
                o._coneShapeImp.ConeUpIndex = value;
            }
        }

        public float Height
        {
            get
            {
                var retval = _coneShapeImp.Height;
                return retval;
            }
        }

        public float Radius
        {
            get
            {
                var retval = _coneShapeImp.Radius;
                return retval;
            }
        }
    }
}
