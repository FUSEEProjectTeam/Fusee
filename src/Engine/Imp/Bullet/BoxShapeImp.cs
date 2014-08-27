using BulletSharp;
using Fusee.Math;

namespace Fusee.Engine
{
    public class BoxShapeImp : CollisonShapeImp, IBoxShapeImp
    {
        internal BoxShape BtBoxShape;
        internal Translater Translater = new Translater();


        public float3 LocalScaling
        {
            get
            {
                var retval = Translater.BtVector3ToFloat3(BtBoxShape.LocalScaling);
                return retval;
            }
            set
            {
                var o = (BoxShapeImp) BtBoxShape.UserObject;
                o.BtBoxShape.LocalScaling = Translater.Float3ToBtVector3(value);
                //Todo: Update RigidBody Inertia refering to the CollisionPbject
            }
        }

        public float3 HalfExtents
        {
            get
            {
                var retval = new float3(BtBoxShape.HalfExtentsWithMargin.X, BtBoxShape.HalfExtentsWithMargin.Y,
                    BtBoxShape.HalfExtentsWithMargin.Z);
                return retval;
            }
        }


        //Inherited
        public float Margin
        {
            get
            {
                var retval = BtBoxShape.Margin;
                return retval;
            }
            set
            {
                var o = (BoxShapeImp) BtBoxShape.UserObject;
                o.BtBoxShape.Margin = value;
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
