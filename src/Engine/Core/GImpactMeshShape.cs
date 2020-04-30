using Fusee.Engine.Common;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// TBD
    /// </summary>
    public class GImpactMeshShape : CollisionShape
    {

        internal IGImpactMeshShapeImp _gImpactMeshShapeImp;

        /// <summary>
        /// TBD
        /// </summary>
        public override float Margin
        {
            get
            {
                var retval = _gImpactMeshShapeImp.Margin;
                return retval;
            }
            set
            {
                var o = (GImpactMeshShape)_gImpactMeshShapeImp.UserObject;
                o._gImpactMeshShapeImp.Margin = value;
            }
        }
    }
}
