using BulletSharp;
using Fusee.Engine.Common;
using Fusee.Math.Core;
using Quaternion = Fusee.Math.Core.Quaternion;

namespace Fusee.Engine.Imp.Physics.Desktop
{
    /// <summary>
    /// Implementation of the <see cref="IHingeConstraintImp" /> interface using the bullet physics engine.
    /// </summary>
    public class HingeConstraintImp : IHingeConstraintImp
    {
        internal HingeConstraint _hci;

        /// <summary>
        /// Gets and sets a value indicating whether [angular only].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [angular only]; otherwise, <c>false</c>.
        /// </value>
        public bool AngularOnly
        {
            get
            {
                var retval = _hci.AngularOnly;
                return retval;
            }
            set
            {
                var o = (HingeConstraintImp)_hci.UserObject;
                o._hci.AngularOnly = value;
            }
        }

        /// <summary>
        /// Gets and sets a value indicating whether [enable motor].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable motor]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableMotor
        {
            get
            {
                var retval = _hci.EnableMotor;
                return retval;
            }
            set
            {
                var o = (HingeConstraintImp) _hci.UserObject;
                o._hci.EnableMotor = value;
            }
        }
        /// <summary>
        /// Enables the angular motor.
        /// </summary>
        /// <param name="enableMotor">if set to <c>true</c> [enable motor].</param>
        /// <param name="targetVelocity">The target velocity.</param>
        /// <param name="maxMotorImpulse">The maximum motor impulse.</param>
        public void EnableAngularMotor(bool enableMotor, float targetVelocity, float maxMotorImpulse)
        {
            var o = (HingeConstraintImp) _hci.UserObject;
            o._hci.EnableAngularMotor(enableMotor, targetVelocity, maxMotorImpulse);
        }

        /// <summary>
        /// Sets the motor target.
        /// </summary>
        /// <param name="qAinB">The q ain b.</param>
        /// <param name="dt">The dt.</param>
        public void SetMotorTarget(Quaternion qAinB, float dt)
        {
            var o = (HingeConstraintImp) _hci.UserObject;
            o._hci.SetMotorTarget(Translator.QuaternionToBtQuaternion(qAinB), dt);
        }
        /// <summary>
        /// Sets the motor target.
        /// </summary>
        /// <param name="targetAngle">The target angle.</param>
        /// <param name="dt">The dt.</param>
        public void SetMotorTarget(float targetAngle, float dt)
        {
            var o = (HingeConstraintImp) _hci.UserObject;
            o._hci.SetMotorTarget(targetAngle, dt);
        }

        /// <summary>
        /// Gets and sets the maximum motor impulse.
        /// </summary>
        /// <value>
        /// The maximum motor impulse.
        /// </value>
        public float MaxMotorImpulse
        {
            get
            {
                var retval = _hci.MaxMotorImpulse;
                return retval;
            }
            set
            {
                var o = (HingeConstraintImp) _hci.UserObject;
                o._hci.MaxMotorImpulse = value;
            }
        }
        /// <summary>
        /// Gets the motor target velocity.
        /// </summary>
        /// <value>
        /// The motor target velocity.
        /// </value>
        public float MotorTargetVelocity
        {
            get
            {
                var retval = _hci.MotorTargetVelocity;
                return retval;
            }
        }

        /// <summary>
        /// Gets the frame a.
        /// </summary>
        /// <value>
        /// The frame a.
        /// </value>
        public float4x4 FrameA
        {
            get
            {
                var retval = Translator.BtMatrixToFloat4X4(_hci.AFrame);
                return retval;
            }
        }
        /// <summary>
        /// Gets the frame b.
        /// </summary>
        /// <value>
        /// The frame b.
        /// </value>
        public float4x4 FrameB
        {
            get
            {
                var retval = Translator.BtMatrixToFloat4X4(_hci.BFrame);
                return retval;
            }
        }

        /// <summary>
        /// Gets the frame offset a.
        /// </summary>
        /// <value>
        /// The frame offset a.
        /// </value>
        public float4x4 FrameOffsetA
        {
            get
            {
                var retval = Translator.BtMatrixToFloat4X4(_hci.FrameOffsetA);
                return retval;
            }
        }
        /// <summary>
        /// Gets the frame offset b.
        /// </summary>
        /// <value>
        /// The frame offset b.
        /// </value>
        public float4x4 FrameOffsetB
        {
            get
            {
                var retval = Translator.BtMatrixToFloat4X4(_hci.FrameOffsetB);
                return retval;
            }
        }

        /// <summary>
        /// Sets the axis.
        /// </summary>
        /// <param name="axisInA">The axis in a.</param>
        public void SetAxis(float3 axisInA)
        {
            var o = (HingeConstraintImp) _hci.UserObject;
            _hci.SetAxis(Translator.Float3ToBtVector3(axisInA));
        }

        /// <summary>
        /// Gets the hinge angle.
        /// </summary>
        /// <returns></returns>
        public float GetHingeAngle()
        {
            var retval =  _hci.GetHingeAngle();
            return retval;
        }
        /// <summary>
        /// Gets the hinge angle.
        /// </summary>
        /// <param name="transA">The trans a.</param>
        /// <param name="transB">The trans b.</param>
        /// <returns></returns>
        public float GetHingeAngle(float4x4 transA, float4x4 transB)
        {
            var retval = _hci.GetHingeAngle(Translator.Float4X4ToBtMatrix(transA), Translator.Float4X4ToBtMatrix(transB));
            return retval;
        }

        /// <summary>
        /// Sets the limit.
        /// </summary>
        /// <param name="low">The low.</param>
        /// <param name="high">The high.</param>
        /// <param name="softness">The softness.</param>
        /// <param name="biasFactor">The bias factor.</param>
        /// <param name="relaxationFactor">The relaxation factor.</param>
        public void SetLimit(float low, float high, float softness = 0.9f, float biasFactor = 0.3f, float relaxationFactor = 1)
        {
            _hci.SetLimit(low, high, softness, biasFactor, relaxationFactor);
        }

        /// <summary>
        /// Gets the solver limit.
        /// </summary>
        /// <value>
        /// The solver limit.
        /// </value>
        public int SolverLimit
        {
            get
            {
                var retval = _hci.SolveLimit;
                return retval;
            }
        }
        /// <summary>
        /// Gets the lower limit.
        /// </summary>
        /// <value>
        /// The lower limit.
        /// </value>
        public float LowerLimit
        {
            get
            {
                var retval = _hci.LowerLimit;
                return retval;
            }
        }
        /// <summary>
        /// Gets the upper limit.
        /// </summary>
        /// <value>
        /// The upper limit.
        /// </value>
        public float UpperLimit
        {
            get
            {

                var retval = _hci.UpperLimit;
                return retval;
            }
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
                var retval = _hci.RigidBodyA;
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
                var retval = _hci.RigidBodyB;
                return (RigidBodyImp)retval.UserObject;
            }
        }
        /// <summary>
        /// Gets the uid.
        /// </summary>
        /// <returns></returns>
        public int GetUid()
        {
            var retval = _hci.Uid;
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
