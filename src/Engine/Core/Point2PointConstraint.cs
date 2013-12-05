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

        public float3 PivotInA
        {
            get
            {
                return _iP2PConstraintImp.PivotInA;
            }
            set
            {
                var o = (Point2PointConstraint)_iP2PConstraintImp.UserObject;
                o._iP2PConstraintImp.PivotInA = value;
            }
            
        }
        public float3 PivotInB
        {
            get
            {
                return _iP2PConstraintImp.PivotInB;
            }
            set
            {
                var o = (Point2PointConstraint)_iP2PConstraintImp.UserObject;
                o._iP2PConstraintImp.PivotInB = value;
            }
        }

        public void UpdateRhS(float timeStep)
        {
            var o = (Point2PointConstraint)_iP2PConstraintImp.UserObject;
            o._iP2PConstraintImp.UpdateRhs(timeStep);
        }

        public void SetParam(PointToPointFlags param, float value, int axis = -1)
        {
            var o = (Point2PointConstraint) _iP2PConstraintImp.UserObject;
            o._iP2PConstraintImp.SetParam(param, value, axis);
        }
        public float GetParam(PointToPointFlags param, int axis = -1)
        {
            var retval = _iP2PConstraintImp.GetParam(param, axis);
            return retval;
        }

        

        public RigidBody RigidBodyA
        {
            get
            {
                var retval = _iP2PConstraintImp.RigidBodyA.UserObject;
                return (RigidBody)retval;
            }
        }
        public RigidBody RigidBodyB
        {
            get
            {
                var retval = _iP2PConstraintImp.RigidBodyB.UserObject;
                return (RigidBody)retval;
            }
        }
        public int GetUid()
        {
            var retval = _iP2PConstraintImp.GetUid();
            return retval;
        }
    }
}
