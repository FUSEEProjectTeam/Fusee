using BulletSharp;
using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Desktop
{
    /// <summary>
    /// Implementation of the <see cref="IStaticPlaneShapeImp" /> interface using the bullet physics engine.
    /// </summary>
    public class StaticPlaneShapeImp : CollisonShapeImp, IStaticPlaneShapeImp
    {
        internal StaticPlaneShape BtStaticPlaneShape;

        /// <summary>
        /// Gets the plane constant.
        /// </summary>
        /// <value>
        /// The plane constant.
        /// </value>
        public float PlaneConstant
        {
            get { return BtStaticPlaneShape.PlaneConstant; }
        }

        /// <summary>
        /// Gets the plane normal.
        /// </summary>
        /// <value>
        /// The plane normal.
        /// </value>
        public float3 PlaneNormal
        {
            get
            {
                var retval = Translator.BtVector3ToFloat3(BtStaticPlaneShape.PlaneNormal);
                return retval;
            }
        }

        //Inherited
        /// <summary>
        /// Gets or sets the margin.
        /// </summary>
        /// <value>
        /// The margin.
        /// </value>
        public override float Margin
        {
            get
            {
                var retval = BtStaticPlaneShape.Margin;
                return retval;
            }
            set
            {
                var o = (StaticPlaneShapeImp)BtStaticPlaneShape.UserObject;
                o.BtStaticPlaneShape.Margin = value;
            }
        }
    }
}
