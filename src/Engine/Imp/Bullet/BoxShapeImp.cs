using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusee.Math;
using BulletSharp;

namespace Fusee.Engine
{
    public class BoxShapeImp : CollisonShapeImp, IBoxShapeImp
    {
        internal BoxShape BtBoxShape;
        internal Translater Translater;

        public float3 HalfExtents
        {
            get
            {
                var retval = new float3(BtBoxShape.HalfExtentsWithMargin.X, BtBoxShape.HalfExtentsWithMargin.Y, BtBoxShape.HalfExtentsWithMargin.Z);
                return retval; 
            }
        }


        //Inherited
        public override float Margin
        {
            get
            {
                var retval = BtBoxShape.Margin;
                return retval;
            }
            set
            {
                var o = (BoxShapeImp)BtBoxShape.UserObject;
                o.BtBoxShape.Margin = value;
            }
        }

        private object _userObject;
        public override  object UserObject
        {
            get { return _userObject; }
            set { _userObject = value; }
        }
    }
}
