using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fusee.Engine;
using Fusee.Math;
using BulletSharp;


namespace Fusee.Engine
{
    public class DynamicWorldImp : IDynamicWorldImp
    {
        internal Translater Translater = new Translater();

        internal DynamicsWorld BtWorld;
        internal CollisionConfiguration BtCollisionConf;
        internal CollisionDispatcher BtDispatcher;
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
                Gravity = new Vector3(0, -9.81f   *10.0f , 0)
            };

            BtWorld.SolverInfo.NumIterations = 8;
            GImpactCollisionAlgorithm.RegisterAlgorithm(BtDispatcher);
        }
        
        public IRigidBodyImp AddRigidBody(float mass, float3 worldTransform, int[] meshTriangles, float3[] meshVertices, /* shape, */ float3 intertia)
        {

            Debug.WriteLine("DynamicWorldImp: AddRigidBody");
            // worldTransform*=10.0f;
            // Use bullet to do what needs to be done:
            var btMatrix = Matrix.Translation(worldTransform.x, worldTransform.y, worldTransform.z);
            var btMotionState = new DefaultMotionState(btMatrix);
            
            //TODO:what about other collisionshape types -> what is required from the collision shape and its performance
            Vector3[] btMeshVertecies = new Vector3[meshVertices.Length];
            for (int v = 0; v < meshVertices.Length; v++)
            {
                btMeshVertecies[v].X = meshVertices[v].x;
                btMeshVertecies[v].Y = meshVertices[v].y;
                btMeshVertecies[v].Z = meshVertices[v].z;
            }
            var btTriangleIndexVertexArray = new TriangleIndexVertexArray(meshTriangles, btMeshVertecies);
            //TriangleMeshShape ctms = new BvhTriangleMeshShape(btTriangleIndexVertexArray, false);
            GImpactMeshShape btGimpactMeshShape = new GImpactMeshShape(btTriangleIndexVertexArray);
            btGimpactMeshShape.UpdateBound();//This simulates the GImpact Physics
            CollisionShape btColShape = btGimpactMeshShape;//new ConvexTriangleMeshShape(btTriangleIndexVertexArray);
            btColShape.Margin = 0.01f;
            var btLocalInertia = btColShape.CalculateLocalInertia(mass);
            
            var btRbcInfo = new RigidBodyConstructionInfo(mass * 10, btMotionState, btGimpactMeshShape, btLocalInertia);
            var btRigidBody = new RigidBody(btRbcInfo);

            BtWorld.AddRigidBody(btRigidBody);
            var retval = new RigidBodyImp();
            retval._rbi = btRigidBody;
            btRigidBody.UserObject = retval;
            return retval;
        }

        public int StepSimulation(float timeSteps, int maxSubSteps=1, float fixedTimeSteps=1/60)
        {
            return BtWorld.StepSimulation(timeSteps);//, maxSubSteps, fixedTimeSteps);
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
                new Vector3(pivotInA.x, pivotInA.y, pivotInA.z), new Vector3(pivotInB.x, pivotInB.y, pivotInB.z));
            BtWorld.AddConstraint(btP2PConstraint);
            var retval = new Point2PointConstraintImp();
            retval._p2pci = btP2PConstraint;
            btP2PConstraint.UserObject = retval;
            return retval;
        }

        //TODO: What about inheritance problems -> should return any constraint type
        public IPoint2PointConstraintImp GetConstraint(int i)
        {
            return (Point2PointConstraintImp)BtWorld.GetConstraint(0).UserObject;
        }

        public IHingeConstraintImp AddHingeConstraint(IRigidBodyImp rigidBodyA, IRigidBodyImp rigidBodyB, float3 pivotInA, float3 pivotInB, float3 axisInA, float3 AxisInB, bool useReferenceFrameA)
        {
            var rigidBodyAImp = (RigidBodyImp)rigidBodyA;
            var btRigidBodyA = rigidBodyAImp._rbi;

            var rigidBodyBImp = (RigidBodyImp)rigidBodyB;
            var btRigidBodyB = rigidBodyBImp._rbi;

            var btHingeConstraint = new HingeConstraint(btRigidBodyA, btRigidBodyB, new Vector3(pivotInA.x, pivotInA.y, pivotInA.z), new Vector3(pivotInB.x, pivotInB.y, pivotInB.z), new Vector3(axisInA.x, axisInA.y, axisInA.z), new Vector3(AxisInB.x, AxisInB.y, AxisInB.z), useReferenceFrameA);
            BtWorld.AddConstraint(btHingeConstraint);

            var retval = new HingeConstraintImp();
            retval._hci = btHingeConstraint;
            btHingeConstraint.UserObject = retval;
            return retval;
        }

        public ISliderConstraintImp AddSliderConstraint(IRigidBodyImp rigidBodyA, IRigidBodyImp rigidBodyB, float4x4 frameInA, float4x4 frameInB, bool useLinearReferenceFrameA)
        {
            var rigidBodyAImp = (RigidBodyImp)rigidBodyA;
            var btRigidBodyA = rigidBodyAImp._rbi;

            var rigidBodyBImp = (RigidBodyImp)rigidBodyB;
            var btRigidBodyB = rigidBodyBImp._rbi;

            var btFrameInA = Translater.Float4X4ToBtMatrix(frameInA);
            var btFrameInB = Translater.Float4X4ToBtMatrix(frameInB);

            var btSliderConstraint = new SliderConstraint(btRigidBodyA, btRigidBodyB, btFrameInA, btFrameInB, useLinearReferenceFrameA);

            BtWorld.AddConstraint(btSliderConstraint);

            var retval = new SliderConstraintImp();
            retval._sci = btSliderConstraint;
            btSliderConstraint.UserObject = retval;
            return retval;
        }


        public int NumberConstraints()
        {
            return BtWorld.NumConstraints;
        }
    }
}
