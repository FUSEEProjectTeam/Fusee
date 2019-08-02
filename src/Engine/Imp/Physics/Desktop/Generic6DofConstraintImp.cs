using BulletSharp;
using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Desktop
{
    /// <summary>
    /// Implementation of the <see cref="IGeneric6DofConstraintImp" /> interface using the bullet physics engine.
    /// </summary>
    public class Generic6DofConstraintImp : IGeneric6DofConstraintImp
    {
        internal Generic6DofConstraint _g6dofci;

        /// <summary>
        /// Gets and sets the angular lower limit.
        /// </summary>
        /// <value>
        /// The angular lower limit.
        /// </value>
        public float3 AngularLowerLimit
        {
            get
            {
                var retval = Translator.BtVector3ToFloat3(_g6dofci.AngularLowerLimit);
                return retval;
            }
            set
            {
                var o = (Generic6DofConstraintImp) _g6dofci.UserObject;
                o._g6dofci.AngularLowerLimit = Translator.Float3ToBtVector3(value);
            }
        }
        /// <summary>
        /// Gets and sets the angular upper limit.
        /// </summary>
        /// <value>
        /// The angular upper limit.
        /// </value>
        public float3 AngularUpperLimit
        {
            get
            {
                var retval = Translator.BtVector3ToFloat3(_g6dofci.AngularUpperLimit);
                return retval;
            }
            set
            {
                var o = (Generic6DofConstraintImp)_g6dofci.UserObject;
                o._g6dofci.AngularUpperLimit = Translator.Float3ToBtVector3(value);
            }
        }

        /// <summary>
        /// Calculates the anchor position.
        /// </summary>
        public void CalcAnchorPos()
        {
            _g6dofci.CalcAnchorPos();
        }

        /// <summary>
        /// Calculates the transforms.
        /// </summary>
        public void CalculateTransforms()
        {
            _g6dofci.CalculateTransforms();
        }
        /// <summary>
        /// Calculates the transforms.
        /// </summary>
        /// <param name="transA">The trans a.</param>
        /// <param name="transB">The trans b.</param>
        public void CalculateTransforms(float4x4 transA, float4x4 transB)
        {
            _g6dofci.CalculateTransforms(Translator.Float4X4ToBtMatrix(transA), Translator.Float4X4ToBtMatrix(transB));
        }
        /// <summary>
        /// Gets the calculated transform a.
        /// </summary>
        /// <value>
        /// The calculated transform a.
        /// </value>
        public float4x4 CalculatedTransformA
        {
            get
            {
                var retval =  Translator.BtMatrixToFloat4X4(_g6dofci.CalculatedTransformA);
                return retval;
            }
        }
        /// <summary>
        /// Gets the calculated transform b.
        /// </summary>
        /// <value>
        /// The calculated transform b.
        /// </value>
        public float4x4 CalculatedTransformB
        {
            get
            {
                var retval = Translator.BtMatrixToFloat4X4(_g6dofci.CalculatedTransformB);
                return retval;
            }
        }

        /// <summary>
        /// Gets and sets the frame offset a.
        /// </summary>
        /// <value>
        /// The frame offset a.
        /// </value>
        public float4x4 FrameOffsetA
        {
            get
            {
                var retval = Translator.BtMatrixToFloat4X4(_g6dofci.FrameOffsetA);
                return retval;
            }
            set
            {
                var o = (Generic6DofConstraintImp) _g6dofci.UserObject;
                o._g6dofci.FrameOffsetA = Translator.Float4X4ToBtMatrix(value);
            }
        }
        /// <summary>
        /// Gets and sets the frame offset b.
        /// </summary>
        /// <value>
        /// The frame offset b.
        /// </value>
        public float4x4 FrameOffsetB
        {
            get
            {
                var retval = Translator.BtMatrixToFloat4X4(_g6dofci.FrameOffsetB);
                return retval;
            }
            set
            {
                var o = (Generic6DofConstraintImp)_g6dofci.UserObject;
                o._g6dofci.FrameOffsetB = Translator.Float4X4ToBtMatrix(value);
            }
        }

        /// <summary>
        /// Gets the angle.
        /// </summary>
        /// <param name="axisIndex">Index of the axis.</param>
        /// <returns></returns>
        public float GetAngle(int axisIndex)
        {
            var retval = _g6dofci.GetAngle(axisIndex);
            return retval;
        }
        /// <summary>
        /// Gets the axis.
        /// </summary>
        /// <param name="axisIndex">Index of the axis.</param>
        /// <returns></returns>
        public float3 GetAxis(int axisIndex)
        {
            var retval = Translator.BtVector3ToFloat3(_g6dofci.GetAxis(axisIndex));
            return retval;
        }

        /// <summary>
        /// Gets the relative pivot position.
        /// </summary>
        /// <param name="axisIndex">Index of the axis.</param>
        /// <returns></returns>
        public float GetRelativePivotPosition(int axisIndex)
        {
            var retval =  _g6dofci.GetRelativePivotPosition(axisIndex);
            return retval;
        }

        /// <summary>
        /// Determines whether the specified limit index is limited.
        /// </summary>
        /// <param name="limitIndex">Index of the limit.</param>
        /// <returns></returns>
        public bool IsLimited(int limitIndex)
        {
            var retval = _g6dofci.IsLimited(limitIndex);
            return retval;
        }
        /// <summary>
        /// Gets and sets the linear lower limit.
        /// </summary>
        /// <value>
        /// The linear lower limit.
        /// </value>
        public float3 LinearLowerLimit
        {
            get
            {
                var retval = Translator.BtVector3ToFloat3(_g6dofci.LinearLowerLimit);
                return retval;
            }
            set
            {
                var o = (Generic6DofConstraintImp)_g6dofci.UserObject;
                o._g6dofci.LinearLowerLimit = Translator.Float3ToBtVector3(value);
            }
        }
        /// <summary>
        /// Gets and sets the linear upper limit.
        /// </summary>
        /// <value>
        /// The linear upper limit.
        /// </value>
        public float3 LinearUpperLimit
        {
            get
            {
                var retval = Translator.BtVector3ToFloat3(_g6dofci.LinearUpperLimit);
                return retval;
            }
            set
            {
                var o = (Generic6DofConstraintImp)_g6dofci.UserObject;
                o._g6dofci.LinearUpperLimit = Translator.Float3ToBtVector3(value);
            }
        }

        /// <summary>
        /// Sets the axis.
        /// </summary>
        /// <param name="axis1">The axis1.</param>
        /// <param name="axis2">The axis2.</param>
        public void SetAxis(float3 axis1, float3 axis2)
        {
            _g6dofci.SetAxis(Translator.Float3ToBtVector3(axis1), Translator.Float3ToBtVector3(axis2));
        }
        /// <summary>
        /// Sets the frames.
        /// </summary>
        /// <param name="frameA">The frame a.</param>
        /// <param name="frameB">The frame b.</param>
        public void SetFrames(float4x4 frameA, float4x4 frameB)
        {
            _g6dofci.SetFrames(Translator.Float4X4ToBtMatrix(frameA), Translator.Float4X4ToBtMatrix(frameB));
        }
        /// <summary>
        /// Sets the limit.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="lo">The lo.</param>
        /// <param name="hi">The hi.</param>
        public void SetLimit(int axis, float lo, float hi)
        {
            _g6dofci.SetLimit(axis, lo, hi);
        }

        /// <summary>
        /// Tests the angular limit motor.
        /// </summary>
        /// <param name="axisIndex">Index of the axis.</param>
        /// <returns></returns>
        public bool TestAngularLimitMotor(int axisIndex)
        {
            var retval = _g6dofci.TestAngularLimitMotor(axisIndex);
            return retval;
        }

        /// <summary>
        /// Gets and sets a value indicating whether [use frame offset].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use frame offset]; otherwise, <c>false</c>.
        /// </value>
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

        /// <summary>
        /// Updates the RHS.
        /// </summary>
        /// <param name="timeStep">The time step.</param>
        public void UpdateRhs(float timeStep)
        {
            _g6dofci.UpdateRHS(timeStep);
        }


        /// <summary>
        /// Gets the rigid body a.
        /// </summary>
        /// <value>
        /// The rigid body a.
        /// </value>
        public IRigidBodyImp RigidBodyA
        {
            get
            {
                var retval = _g6dofci.RigidBodyA;
                return (RigidBodyImp)retval.UserObject;
            }
        }
        /// <summary>
        /// Gets the rigid body b.
        /// </summary>
        /// <value>
        /// The rigid body b.
        /// </value>
        public IRigidBodyImp RigidBodyB
        {
            get
            {
                var retval = _g6dofci.RigidBodyB;
                return (RigidBodyImp)retval.UserObject;
            }
        }
        /// <summary>
        /// Gets the uid.
        /// </summary>
        /// <returns></returns>
        public int GetUid()
        {
            var retval = _g6dofci.Uid;
            return retval;
        }
        private object _userObject;
        /// <summary>
        /// Gets and sets the user object.
        /// </summary>
        /// <value>
        /// The user object.
        /// </value>
        public object UserObject
        {
            get { return _userObject; }
            set { _userObject = value; }
        }
    }
}
