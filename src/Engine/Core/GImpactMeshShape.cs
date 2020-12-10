using Fusee.Engine.Common;

namespace Fusee.Engine.Core
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