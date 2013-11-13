using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusee.Math;
using BulletSharp;

namespace Fusee.Engine
{
    public class Point2PointConstraintImp : IPoint2PointConstraintImp
    {
        internal Point2PointConstraint _p2pci;

        public float Test
        {
            get
            {
                var retval = _p2pci.PivotInA.X;
                return retval;
            }
            set
            {
               var o = (Point2PointConstraintImp)_p2pci.UserObject;
               o._p2pci.PivotInA = new Vector3(value, _p2pci.PivotInA.Y, _p2pci.PivotInA.Z);
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
