using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using Fusee.Math;

namespace Fusee.Engine
{
    public class Generic6DofConstraintImp : IGeneric6DofConstraintImp
    {
        internal Generic6DofConstraint _g6dofci;
        internal Translater Translater = new Translater();

        public float3 AngularLowerLimit
        {
            get
            {
                var retval = Translater.BtVector3ToFloat3(_g6dofci.AngularLowerLimit);
                return retval;
            }
            set
            {
                var o = (Generic6DofConstraintImp) _g6dofci.UserObject;
                o._g6dofci.AngularLowerLimit = Translater.Float3ToBtVector3(value);
            }
        }
        public float3 AngularUpperLimit
        {
            get
            {
                var retval = Translater.BtVector3ToFloat3(_g6dofci.AngularUpperLimit);
                return retval;
            }
            set
            {
                var o = (Generic6DofConstraintImp)_g6dofci.UserObject;
                o._g6dofci.AngularUpperLimit = Translater.Float3ToBtVector3(value);
            }
        }

        public void CalcAnchorPos()
        {
            _g6dofci.CalcAnchorPos();
        }

        public void CalculateTransforms()
        {
            _g6dofci.CalculateTransforms();
        }
        public void CalculateTransforms(float4x4 transA, float4x4 transB)
        {
            _g6dofci.CalculateTransforms(Translater.Float4X4ToBtMatrix(transA), Translater.Float4X4ToBtMatrix(transB));
        }
        public float4x4 CalculatedTransformA
        {
            get
            {
                var retval =  Translater.BtMatrixToFloat4X4(_g6dofci.CalculatedTransformA);
                return retval;
            }
        }
        public float4x4 CalculatedTransformB
        {
            get
            {
                var retval = Translater.BtMatrixToFloat4X4(_g6dofci.CalculatedTransformB);
                return retval;
            }
        }

        public float4x4 FrameOffsetA
        {
            get
            {
                var retval = Translater.BtMatrixToFloat4X4(_g6dofci.FrameOffsetA);
                return retval;
            }
            set
            {
                var o = (Generic6DofConstraintImp) _g6dofci.UserObject;
                o._g6dofci.FrameOffsetA = Translater.Float4X4ToBtMatrix(value);
            }
        }
        public float4x4 FrameOffsetB
        {
            get
            {
                var retval = Translater.BtMatrixToFloat4X4(_g6dofci.FrameOffsetB);
                return retval;
            }
            set
            {
                var o = (Generic6DofConstraintImp)_g6dofci.UserObject;
                o._g6dofci.FrameOffsetB = Translater.Float4X4ToBtMatrix(value);
            }
        }

        public float GetAngle(int axisIndex)
        {
            var retval = _g6dofci.GetAngle(axisIndex);
            return retval;
        }
        public float3 GetAxis(int axisIndex)
        {
            var retval = Translater.BtVector3ToFloat3(_g6dofci.GetAxis(axisIndex));
            return retval;
        }

        public float GetRelativePivotPosition(int axisIndex)
        {
            var retval =  _g6dofci.GetRelativePivotPosition(axisIndex);
            return retval;
        }

        public bool IsLimited(int limitIndex)
        {
            var retval = _g6dofci.IsLimited(limitIndex);
            return retval;
        }
        public float3 LinearLowerLimit
        {
            get
            {
                var retval = Translater.BtVector3ToFloat3(_g6dofci.LinearLowerLimit);
                return retval;
            }
            set
            {
                var o = (Generic6DofConstraintImp)_g6dofci.UserObject;
                o._g6dofci.LinearLowerLimit = Translater.Float3ToBtVector3(value);
            }
        }
        public float3 LinearUpperLimit
        {
            get
            {
                var retval = Translater.BtVector3ToFloat3(_g6dofci.LinearUpperLimit);
                return retval;
            }
            set
            {
                var o = (Generic6DofConstraintImp)_g6dofci.UserObject;
                o._g6dofci.LinearUpperLimit = Translater.Float3ToBtVector3(value);
            }
        }

        public void SetAxis(float3 axis1, float3 axis2)
        {
            _g6dofci.SetAxis(Translater.Float3ToBtVector3(axis1), Translater.Float3ToBtVector3(axis2));
        }
        public void SetFrames(float4x4 frameA, float4x4 frameB)
        {
            _g6dofci.SetFrames(Translater.Float4X4ToBtMatrix(frameA), Translater.Float4X4ToBtMatrix(frameB));
        }
        public void SetLimit(int axis, float lo, float hi)
        {
            _g6dofci.SetLimit(axis, lo, hi);
        }

        public bool TestAngularLimitMotor(int axisIndex)
        {
            var retval = _g6dofci.TestAngularLimitMotor(axisIndex);
            return retval;
        }

        public bool UseFrameOffset
        {
            get
            {
                var retval = _g6dofci.UseFrameOffset;
                return retval;
            }
            set
            {
                var o = (Generic6DofConstraintImp) _g6dofci.UserObject;
                o._g6dofci.UseFrameOffset = value;
            }
        }

        public void UpdateRhs(float timeStep)
        {
            _g6dofci.UpdateRHS(timeStep);
        }


        public IRigidBodyImp RigidBodyA
        {
            get
            {
                var retval = _g6dofci.RigidBodyA;
                return (RigidBodyImp)retval.UserObject;
            }
        }
        public IRigidBodyImp RigidBodyB
        {
            get
            {
                var retval = _g6dofci.RigidBodyB;
                return (RigidBodyImp)retval.UserObject;
            }
        }
        public int GetUid()
        {
            var retval = _g6dofci.Uid;
            return retval;
        }
        private object _userObject;
        public object UserObject
        {
            get { return _userObject; }
            set { _userObject = value; }
        }
    }
}
