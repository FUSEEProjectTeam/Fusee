using Fusee.Engine.Imp.Physics.Common;

namespace Fusee.Engine.Imp.Physics.Core
{
    public class GImpactMeshShape : CollisionShape
    {

        internal IGImpactMeshShapeImp _gImpactMeshShapeImp;

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
