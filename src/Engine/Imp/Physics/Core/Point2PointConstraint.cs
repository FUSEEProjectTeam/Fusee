using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Core
{
    /// <summary>
    /// Implements a point to point constraint.
    /// </summary>
    public class Point2PointConstraint
    {
        internal IPoint2PointConstraintImp _iP2PConstraintImp;
        /// <summary>
        /// Gets and sets the first pivot point.
        /// </summary>
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
        /// <summary>
        /// Gets and sets the second pivot point.
        /// </summary>
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

        
        /// <summary>
        /// Returns the first rigid body.
        /// </summary>
        public RigidBody RigidBodyA
        {
            get
            {
                var retval = _iP2PConstraintImp.RigidBodyA.UserObject;
                return (RigidBody)retval;
            }
        }
        /// <summary>
        /// Returns the second rigid body.
        /// </summary>
        public RigidBody RigidBodyB
        {
            get
            {
                var retval = _iP2PConstraintImp.RigidBodyB.UserObject;
                return (RigidBody)retval;
            }
        }
        /// <summary>
        /// Returns the Uid.
        /// </summary>
        /// <returns></returns>
        public int GetUid()
        {
            var retval = _iP2PConstraintImp.GetUid();
            return retval;
        }
    }
}
