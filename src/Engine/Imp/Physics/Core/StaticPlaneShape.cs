using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Core
{
    /// <summary>
    /// Implements a collision shape in form of a static plane.
    /// </summary>
    public class StaticPlaneShape : CollisionShape
    {
        internal IStaticPlaneShapeImp _staticPlaneShapeImp;
        /// <summary>
        /// Returns the plane constant.
        /// </summary>
        public float PlaneConstant
        {
            get { return _staticPlaneShapeImp.PlaneConstant; }
        }
        /// <summary>
        /// Returns the plane´s normal.
        /// </summary>
        public float3 PlaneNormal
        {
            get
            {
                return _staticPlaneShapeImp.PlaneNormal;
            }
        }

        //Inherited
        /// <summary>
        /// Returns the plane´s Margin
        /// </summary>
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
