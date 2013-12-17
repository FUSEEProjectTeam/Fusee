using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;

namespace Fusee.Engine
{
    public abstract class CollisonShapeImp : ICollisionShapeImp
    {
        internal CollisionShape BtCollisionShape;

        public virtual float Margin { get; set; }
       

        private object _userObject;
        public virtual object UserObject
        {
            get { return _userObject; }
            set { _userObject = value; }
        }
    }
}
