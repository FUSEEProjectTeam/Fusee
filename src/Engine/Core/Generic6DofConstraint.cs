using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace Fusee.Engine
{
    public class Generic6DofConstraint
    {
        internal IGeneric6DofConstraintImp _IG6DofConstraintImp;

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
        public float3 AngularUpperLimit
        {
            get
            {
                var retval = _IG6DofConstraintImp.AngularUpperLimit;
                return retval;
            }
            set
            {
                var o = (Generic6DofConstraint) _IG6DofConstraintImp.UserObject;
                o._IG6DofConstraintImp.AngularUpperLimit = value;
            }
        }

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

        public float GetAngle(int axisIndex)
        {
            var retval = _IG6DofConstraintImp.GetAngle(axisIndex);
            return retval;
        }
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

        public RigidBody RigidBodyA
        {
            get
            {
                var retval = _IG6DofConstraintImp.RigidBodyA.UserObject;
                return (RigidBody)retval;
            }
        }
        public RigidBody RigidBodyB
        {
            get
            {
                var retval = _IG6DofConstraintImp.RigidBodyB.UserObject;
                return (RigidBody)retval;
            }
        }
        public int GetUid()
        {
            var retval = _IG6DofConstraintImp.GetUid();
            return retval;
        }
    }
}
