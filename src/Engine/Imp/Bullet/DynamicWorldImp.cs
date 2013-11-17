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
        
        public IRigidBodyImp AddRigidBody(float mass, float3 worldTransform, /*int[] meshTriangles, float3[] meshVertices, /* shape, */ float3 intertia)
        {

            Debug.WriteLine("DynamicWorldImp: AddRigidBody");
            // worldTransform*=10.0f;
            // Use bullet to do what needs to be done:
            var btMatrix = Matrix.Translation(worldTransform.x, worldTransform.y, worldTransform.z);
            var btMotionState = new DefaultMotionState(btMatrix);
            //TODO: Replace the static Boxshape by an individual collisionshape
            /*
            Vector3[] btMeshVertecies = new Vector3[meshVertices.Length];
            for (int v = 0; v < meshVertices.Length; v++)
            {
                btMeshVertecies[v].X = meshVertices[v].x;
                btMeshVertecies[v].Y = meshVertices[v].y;
                btMeshVertecies[v].Z = meshVertices[v].z;
            }
            var btTriangleIndexVertexArray = new TriangleIndexVertexArray(meshTriangles, btMeshVertecies);
            GImpactMeshShape gImpactMeshShape = new GImpactMeshShape(btTriangleIndexVertexArray);
            gImpactMeshShape.LocalScaling = new Vector3(1);
            gImpactMeshShape.Margin = 0;
            gImpactMeshShape.UpdateBound();
            var btCollisionShape = gImpactMeshShape;
            btCollisionShape.LocalScaling = new Vector3(0.5f);*/
            //TODO Get it working ...
            var btColShape = new BoxShape(2.5f);
            
            //TODO: IF Inertia == NULL ... else ...
            var btInertia = btColShape.CalculateLocalInertia(mass);

            var btRbcInfo = new RigidBodyConstructionInfo(mass /**10*/, btMotionState, btColShape, btInertia);
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
        public IPoint2PointConstraintImp AddPoint2PointConstraint(IRigidBodyImp rigidBodyA, IRigidBodyImp rigidBodyB, float3 pivotInA,
            float3 pivotInB)
        {
            var rigidBodyAImp = (RigidBodyImp) rigidBodyA;
            var btRigidBodyA = rigidBodyAImp._rbi;

            var rigidBodyBImp = (RigidBodyImp) rigidBodyB;
            var btRigidBodyB = rigidBodyBImp._rbi;

            var btP2PConstraint = new Point2PointConstraint(btRigidBodyA, btRigidBodyB,
                new Vector3(pivotInA.x, pivotInA.y, pivotInA.y), new Vector3(pivotInB.x, pivotInB.y, pivotInB.y));
            BtWorld.AddConstraint(btP2PConstraint);
            var retval = new Point2PointConstraintImp();
            retval._p2pci = btP2PConstraint;
            btP2PConstraint.UserObject = retval;
            return retval;
        }

        //TODO: What about inheritance problems -> should return any constraint type
        public IPoint2PointConstraintImp GetConstraint(int i)
        {
            return (Point2PointConstraintImp)BtWorld.GetConstraint(i).UserObject;
        }

        public int NumberConstraints()
        {
            return BtWorld.NumConstraints;
        }
    }
}
