using BulletSharp;
using Fusee.Engine.Imp.Physics.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Imp.Physics.Desktop
{
    /// <summary>
    /// Implementation of the <see cref="IPoint2PointConstraintImp" /> interface using the bullet physics engine.
    /// </summary>
    public class Point2PointConstraintImp : IPoint2PointConstraintImp
    {
        internal Point2PointConstraint _p2pci;



        /// <summary>
        /// Gets and sets the pivot in a.
        /// </summary>
        /// <value>
        /// The pivot in a.
        /// </value>
        public float3 PivotInA
        {
            get
            {
                var retval = new float3(_p2pci.PivotInA.X, _p2pci.PivotInA.Y, _p2pci.PivotInA.Z);
                return retval;
            }
            set
            {
                var pivoA = new Vector3(value.x, value.y, value.z);
                var o = (Point2PointConstraintImp)_p2pci.UserObject;
                o._p2pci.PivotInA = pivoA;
            }
        }
        /// <summary>
        /// Gets and sets the pivot in b.
        /// </summary>
        /// <value>
        /// The pivot in b.
        /// </value>
        public float3 PivotInB
        {
            get
            {
                var retval = new float3(_p2pci.PivotInB.X, _p2pci.PivotInB.Y, _p2pci.PivotInB.Z);
                return retval;
            }
            set
            {
                var pivoB = new Vector3(value.x, value.y, value.z);
                var o = (Point2PointConstraintImp)_p2pci.UserObject;
                o._p2pci.PivotInB = pivoB;
            }
            
        }

        /// <summary>
        /// Updates the RHS.
        /// </summary>
        /// <param name="timeStep">The time step.</param>
        public void UpdateRhs(float timeStep)
        {
            var o = (Point2PointConstraintImp)_p2pci.UserObject;
            o._p2pci.UpdateRHS(timeStep);
        }

        /// <summary>
        /// Sets the parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="value">The value.</param>
        /// <param name="axis">The axis.</param>
        public void SetParam(PointToPointFlags param, float value, int axis = -1)
        {
            var o = (Point2PointConstraintImp)_p2pci.UserObject;
            ConstraintParam constraintParam;
            switch (param)
            {
                case PointToPointFlags.PointToPointFlagsErp:
                    constraintParam = ConstraintParam.Erp;
                    break;
                case PointToPointFlags.PointToPointFlagsStopErp:
                    constraintParam = ConstraintParam.StopErp;
                    break;
                case PointToPointFlags.PointToPointFlagsCfm:
                    constraintParam = ConstraintParam.Cfm;
                    break;
                case PointToPointFlags.PointToPointFlagsStopCfm:
                    constraintParam = ConstraintParam.StopCfm;
                    break;
                default:
                    constraintParam = ConstraintParam.Cfm;
                    break;

            }

            o._p2pci.SetParam(constraintParam, value, axis);
        }
        /// <summary>
        /// Gets the parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="axis">The axis.</param>
        /// <returns></returns>
        public float GetParam(PointToPointFlags param, int axis = -1)
        {
            int constraintParam;
            switch (param)
            {
                case PointToPointFlags.PointToPointFlagsErp:
                    constraintParam = 1;
                    break;
                case PointToPointFlags.PointToPointFlagsStopErp:
                    constraintParam = 2;
                    break;
                case PointToPointFlags.PointToPointFlagsCfm:
                    constraintParam = 3;
                    break;
                case PointToPointFlags.PointToPointFlagsStopCfm:
                    constraintParam = 4;
                    break;
                default:
                    constraintParam = 3;
                    break; 
            }
            var retval = _p2pci.GetParam(constraintParam, axis);
            return retval;
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
                var retval = _p2pci.RigidBodyA;
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
                var retval = _p2pci.RigidBodyB;
                return (RigidBodyImp)retval.UserObject;
            }
        }
        /// <summary>
        /// Gets the uid.
        /// </summary>
        /// <returns></returns>
        public int GetUid()
        {
            var retval = _p2pci.Uid;
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
