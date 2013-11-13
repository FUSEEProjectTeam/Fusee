using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Fusee.Math;


namespace Fusee.Engine
{
    public class DynamicWorld
    {
        internal /*readonly*/ IDynamicWorldImp _dwi;


        public DynamicWorld()
        {
            _dwi = ImpFactory.CreateIDynamicWorldImp();
        }

        public RigidBody AddRigidBody(float mass, float3 worldTransform, Mesh mesh/* shape, */,float3 inertia)
        {
          
            /*Mesh m = mesh;
            IMeshImp imi = m._meshImp;*/
            IRigidBodyImp rbi = _dwi.AddRigidBody(mass, worldTransform, null, /* shape, */inertia);
            var retval = new RigidBody();
            retval._iRigidBodyImp = rbi;
            rbi.UserObject = retval;

            return retval;
        }

        public int StepSimulation(float timeSteps, int maxSubSteps, float fixedTimeSteps)
        {
            return _dwi.StepSimulation(timeSteps, maxSubSteps, fixedTimeSteps);
        }

        public RigidBody GetRigidBody(int i)
        {
            IRigidBodyImp rbi = _dwi.GetRigidBody(i);
            var retval = (RigidBody) rbi.UserObject;
            return retval;
        }

        public int NumberRigidBodies()
        {
            var number = _dwi.NumberRigidBodies();
            return number;
        }

       /* public Point2PointConstraint AddPoint2PointConstraint(IRigidBodyImp rigidBodyA, IRigidBodyImp rigidBodyB, float3 pivotInA, float3 pivotInB)
        {
            IPoint2PointConstraintImp ip2pci = _dwi.AddPoint2PointConstraint(rigidBodyA, rigidBodyB, pivotInA, pivotInB);
            var retval = new Point2PointConstraint();
            retval._iP2PConstraintImp = ip2pci;
            ip2pci.UserObject = retval;
            return retval;
        }*/
    }
}
