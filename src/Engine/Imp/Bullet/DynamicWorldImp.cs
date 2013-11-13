using System;
using System.Diagnostics;
using Fusee.Engine;
using Fusee.Math;
using BulletSharp;


namespace Fusee.Engine
{
    public class DynamicWorldImp : IDynamicWorldImp
    {


        internal DynamicsWorld BtWorld;
        internal CollisionConfiguration BtCollisionConf;
        internal Dispatcher BtDispatcher;
        internal BroadphaseInterface BtBroadphase;
        internal ConstraintSolver BtSolver;
        //internal AlignedCollisionShapeArray BtCollisionShapes { get; private set; }

        internal DynamicWorldImp()
        {
            Debug.WriteLine("DynamicWorldImp");

            // collision configuration contains default setup for memory, collision setup
            BtCollisionConf = new DefaultCollisionConfiguration();
            BtDispatcher = new CollisionDispatcher(BtCollisionConf);
            BtBroadphase = new DbvtBroadphase();
            BtSolver = new SequentialImpulseConstraintSolver();
            

            BtWorld = new DiscreteDynamicsWorld(BtDispatcher, BtBroadphase, BtSolver, BtCollisionConf)
            {
                Gravity = new Vector3(0, -9.81f  /* *10.0f */, 0)
            };

            BtWorld.SolverInfo.NumIterations = 8;

        }


        public IRigidBodyImp AddRigidBody(float mass, float3 worldTransform, IMeshImp mesh, /* shape, */ float3 inertia)
        {

            Debug.WriteLine("DynamicWorldImp: AddRigidBody");
           // worldTransform*=10.0f;
            // Use bullet to do what needs to be done:
            var btMatrix = Matrix.Translation(worldTransform.x, worldTransform.y, worldTransform.z);
            var btMotionState = new DefaultMotionState(btMatrix);
            //var btInertia = new Vector3(inertia.x, inertia.y, inertia.z);
            var btColShale = new BoxShape(2.5f /* * 10.0f*/);
            var btInertia = btColShale.CalculateLocalInertia(mass);
           // btInertia *= (10*10);
            //TODO: Replace the static Boxshape by a individual collisionshape
            
            #region define custom collision shape
            /*int vLength = mesh.Vertices.Length;
            float3[] ver = new float3[vLength];
            ver = mesh.Vertices;
            Vector3[] btVer = new Vector3[vLength];
            for (int v = 0; v < vLength; v++)
            {
                btVer[v].X = ver[v].x;
                btVer[v].Y = ver[v].y;
                btVer[v].Z = ver[v].z;
            }

            int iLength = mesh.Triangles.Length;
            int[] ind = new int[iLength];
            //ind = Convert.ToInt32[](_meshTea.Triangles);
            for (int c = 0; c < iLength; c++)
            {
                ind[c] = Convert.ToInt32(mesh.Triangles[c]);
            }

            var indexVerexArray = new TriangleIndexVertexArray(ind, btVer);

            GImpactMeshShape trimesh = new GImpactMeshShape(indexVerexArray);
            trimesh.LocalScaling = new Vector3(1);
            trimesh.Margin = 0;
            trimesh.UpdateBound();
            var btColShape = trimesh;*/
            #endregion

            var btRbcInfo = new RigidBodyConstructionInfo(mass /**10*/, btMotionState, btColShale, btInertia);
            var btRigidBody = new RigidBody(btRbcInfo);
            btRigidBody.ContactProcessingThreshold = 0;
            BtWorld.AddRigidBody(btRigidBody);

            var retval = new RigidBodyImp();
            retval._rbi = btRigidBody;
            btRigidBody.UserObject = retval;
            return retval;
        }

        public int StepSimulation(float timeSteps, int maxSubSteps=1, float fixedTimeSteps=1/60)
        {
            return BtWorld.StepSimulation(timeSteps, maxSubSteps, fixedTimeSteps);
        }

        public IRigidBodyImp GetRigidBody(int i)
        {
            var colisionObject = BtWorld.CollisionObjectArray[i];
            var btRigidBody = (RigidBody) colisionObject;
            return (RigidBodyImp) btRigidBody.UserObject;
        }

        public int NumberRigidBodies()
        {
            var number = BtWorld.CollisionObjectArray.Count;
            return number;
        }

        

        //Constraint Test

       /* public IPoint2PointConstraintImp AddPoint2PointConstrain(IRigidBodyImp rigidBodyA, IRigidBodyImp rigidBodyB, float3 pivotInA, float3 pivotInB)
        {
            var rigidBodyAImp = new RigidBodyImp();
            rigidBodyAImp.UserObject = rigidBodyA.UserObject;
            var btRigidBodyA = rigidBodyAImp._rbi;

            var rigidBodyBImp = new RigidBodyImp();
            rigidBodyBImp.UserObject = rigidBodyB.UserObject;
            var btRigidBodyB = rigidBodyBImp._rbi;

            var btP2PConstraint = new Point2PointConstraint(btRigidBodyA, btRigidBodyB,
                new Vector3(pivotInA.x, pivotInA.y, pivotInA.y), new Vector3(pivotInB.x, pivotInB.y, pivotInB.y));
            BtWorld.AddConstraint(btP2PConstraint);
            var retval = new Point2PointConstraintImp();
            retval._p2pci = btP2PConstraint;
            btP2PConstraint.UserObject = retval;
            return retval;
        }*/
        
    }
}
