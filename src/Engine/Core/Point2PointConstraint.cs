using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public class Point2PointConstraint
    {
        internal IPoint2PointConstraintImp _iP2PConstraintImp;

        public float Test
        {
            get
            {
                return _iP2PConstraintImp.Test;
            }
            set
            {
                _iP2PConstraintImp.Test = value;
            }
        }
    }
}
