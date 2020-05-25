using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Constraints for a generic six degrees of freedom device.
    /// </summary>
    public class Generic6DofConstraint
    {
        internal IGeneric6DofConstraintImp _IG6DofConstraintImp;
        /// <summary>
        /// Returns the lower limit for an angle.
        /// </summary>
        public float3 AngularLowerLimit
        {
            get
            {
                var retval = _IG6DofConstraintImp.AngularLowerLimit;
                return retval;
            }
            set
            {
                var o = (Generic6DofConstraint)_IG6DofConstraintImp.UserObject;
                o._IG6DofConstraintImp.AngularLowerLimit = value;
            }
        }
        /// <summary>
        /// Returns the upper limit for an angle.
        /// </summary>
        public float3 AngularUpperLimit
        {
            get
            {
                var retval = _IG6DofConstraintImp.AngularUpperLimit;
                return retval;
            }
            set
            {
                var o = (Generic6DofConstraint)_IG6DofConstraintImp.UserObject;
                o._IG6DofConstraintImp.AngularUpperLimit = value;
            }
        }
        /// <summary>
        /// returns the anchor position.
        /// </summary>
        public void CalcAnchorPos()
        {
            _IG6DofConstraintImp.CalcAnchorPos();
        }

        public void CalculateTransforms()
        {
            _IG6DofConstraintImp.CalculateTransforms();
        }
        public void CalculateTransforms(float4x4 transA, float4x4 transB)
        {
            _IG6DofConstraintImp.CalculateTransforms(transA, transB);
        }
        public float4x4 CalculatedTransformA
        {
            get
            {
                var retval = _IG6DofConstraintImp.CalculatedTransformA;
                return retval;
            }
        }
        public float4x4 CalculatedTransformB
        {
            get
            {
                var retval = _IG6DofConstraintImp.CalculatedTransformB;
                return retval;
            }
        }

        public float4x4 FrameOffsetA
        {
            get
            {
                var retval = _IG6DofConstraintImp.FrameOffsetA;
                return retval;
            }
            set
            {
                var o = (Generic6DofConstraint)_IG6DofConstraintImp.UserObject;
                o._IG6DofConstraintImp.FrameOffsetA = value;
            }
        }
        public float4x4 FrameOffsetB
        {
            get
            {
                var retval = _IG6DofConstraintImp.FrameOffsetB;
                return retval;
            }
            set
            {
                var o = (Generic6DofConstraint)_IG6DofConstraintImp.UserObject;
                o._IG6DofConstraintImp.FrameOffsetB = value;
            }
        }

        /// <summary>
        /// Returns the value for a 6Dof axis.
        /// </summary>
        /// <param name="axisIndex"></param>
        /// <returns></returns>
        public float GetAngle(int axisIndex)
        {
            var retval = _IG6DofConstraintImp.GetAngle(axisIndex);
            return retval;
        }
        /// <summary>
        /// Returns the axis of a 6Dof device.
        /// </summary>
        /// <param name="axisIndex"></param>
        /// <returns></returns>
        public float3 GetAxis(int axisIndex)
        {
            var retval = _IG6DofConstraintImp.GetAxis(axisIndex);
            return retval;
        }

        public float GetRelativePivotPosition(int axisIndex)
        {
            var retval = _IG6DofConstraintImp.GetRelativePivotPosition(axisIndex);
            return retval;
        }

        /// <summary>
        /// Returns if the limit index is limited.
        /// </summary>
        /// <param name="limitIndex"></param>
        /// <returns></returns>
        public bool IsLimited(int limitIndex)
        {
            var retval = _IG6DofConstraintImp.IsLimited(limitIndex);
            return retval;
        }
        public float3 LinearLowerLimit
        {
            get
            {
                var retval = _IG6DofConstraintImp.LinearLowerLimit;
                return retval;
            }
            set
            {
                var o = (Generic6DofConstraint)_IG6DofConstraintImp.UserObject;
                o._IG6DofConstraintImp.LinearLowerLimit = value;
            }
        }
        public float3 LinearUpperLimit
        {
            get
            {
                var retval = _IG6DofConstraintImp.LinearUpperLimit;
                return retval;
            }
            set
            {
                var o = (Generic6DofConstraint)_IG6DofConstraintImp.UserObject;
                o._IG6DofConstraintImp.LinearUpperLimit = value;
            }
        }

        public void SetAxis(float3 axis1, float3 axis2)
        {
            _IG6DofConstraintImp.SetAxis(axis1, axis2);
        }
        public void SetFrames(float4x4 frameA, float4x4 frameB)
        {
            _IG6DofConstraintImp.SetFrames(frameA, frameB);
        }

        /// <summary>
        /// Sets the limits for an axis.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="lo"></param>
        /// <param name="hi"></param>
        public void SetLimit(int axis, float lo, float hi)
        {
            _IG6DofConstraintImp.SetLimit(axis, lo, hi);
        }

        public bool TestAngularLimitMotor(int axisIndex)
        {
            var retval = _IG6DofConstraintImp.TestAngularLimitMotor(axisIndex);
            return retval;
        }

        public bool UseFrameOffset
        {
            get
            {
                var retval = _IG6DofConstraintImp.UseFrameOffset;
                return retval;
            }
            set
            {
                var o = (Generic6DofConstraint)_IG6DofConstraintImp.UserObject;
                o._IG6DofConstraintImp.UseFrameOffset = value;
            }
        }

        public void UpdateRhs(float timeStep)
        {
            _IG6DofConstraintImp.UpdateRhs(timeStep);
        }
        /// <summary>
        /// Returns the rigid body a.
        /// </summary>
        public RigidBody RigidBodyA
        {
            get
            {
                var retval = _IG6DofConstraintImp.RigidBodyA.UserObject;
                return (RigidBody)retval;
            }
        }
        /// <summary>
        /// Returns the rigid body b.
        /// </summary>
        public RigidBody RigidBodyB
        {
            get
            {
                var retval = _IG6DofConstraintImp.RigidBodyB.UserObject;
                return (RigidBody)retval;
            }
        }

        /// <summary>
        /// Returns the Uid.
        /// </summary>
        /// <returns></returns>
        public int GetUid()
        {
            var retval = _IG6DofConstraintImp.GetUid();
            return retval;
        }
    }
}