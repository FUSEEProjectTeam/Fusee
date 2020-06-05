using BulletSharp;
using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Desktop
{
    /// <summary>
    /// Implementation of the <see cref="IGImpactMeshShapeImp" /> interface using the bullet physics engine.
    /// </summary>
    public class GImpactMeshShapeImp : CollisonShapeImp, IGImpactMeshShapeImp
    {
        internal GImpactMeshShape BtGImpactMeshShape;

        //Inherited
        /// <summary>
        /// Gets and sets the margin.
        /// </summary>
        /// <value>
        /// The margin.
        /// </value>
        public override float Margin
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
        /// <summary>
        /// Gets and sets the local scaling.
        /// </summary>
        /// <value>
        /// The local scaling.
        /// </value>
        public override float3 LocalScaling
        {
            get
            {
                var retval = Translator.BtVector3ToFloat3(BtGImpactMeshShape.LocalScaling);
                return retval;
            }
            set
            {
                var o = (GImpactMeshShapeImp)BtGImpactMeshShape.UserObject;
                o.BtGImpactMeshShape.LocalScaling = Translator.Float3ToBtVector3(value);
            }
        }
    }
}