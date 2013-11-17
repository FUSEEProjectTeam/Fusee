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

        public void SetParam(float value, int num = 1, int axis = -1)
        {

            var o = (Point2PointConstraintImp)_p2pci.UserObject;
            ConstraintParam constraintParam;
            switch (num)
            {
                case 1:
                    constraintParam = ConstraintParam.Erp;
                    break;
                case 2:
                    constraintParam = ConstraintParam.StopErp;
                    break;
                case 3:
                    constraintParam = ConstraintParam.Cfm;
                    break;
                case 4:
                    constraintParam = ConstraintParam.StopCfm;
                    break;
                default:
                    constraintParam = ConstraintParam.Cfm;
                    break;

            }

            o._p2pci.SetParam(constraintParam, value, axis);
        }

        public float GetParam(int num, int axis = -1)
        {
            var retval = _p2pci.GetParam(num, axis);
            return retval;
        }

        public IRigidBodyImp RigidBodyA
        {
            get
            {
                var retval = _p2pci.RigidBodyA;
                return (RigidBodyImp)retval.UserObject;
            }
        }

        public IRigidBodyImp RigidBodyB
        {
            get
            {
                var retval = _p2pci.RigidBodyB;
                return (RigidBodyImp)retval.UserObject;
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
