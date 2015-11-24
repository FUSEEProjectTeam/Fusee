using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using Fusee.Math;

namespace Fusee.Engine
{
    public class CollisonShapeImp : ICollisionShapeImp
    {
        internal CollisionShape BtCollisionShape;
        internal Translater Translater = new Translater();

        public virtual float Margin
        {
            get
            {
                var retval = BtCollisionShape.Margin;
                return retval;
            }
            set
            {
                var o = (BoxShapeImp)BtCollisionShape.UserObject;
                o.BtBoxShape.Margin = value;
            }
        }

        public virtual float3 LocalScaling
        {
            get
            {
                var retval = Translater.BtVector3ToFloat3(BtCollisionShape.LocalScaling);
                return retval;
            }
            set
            {
                var o = (BoxShapeImp)BtCollisionShape.UserObject;
                o.BtBoxShape.LocalScaling = Translater.Float3ToBtVector3(value);
                //Todo: Update RigidBody Inertia refering to the CollisionPbject
            }
        }


        private object _userObject;
        public object UserObject
        {
            get { return _userObject; }
            set { _userObject = value; }
        }
    }
}
