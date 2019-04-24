﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BulletSharp;
using Fusee.Engine.Common;
using Fusee.Math.Core;


namespace Fusee.Engine.Imp.Physics.Desktop
{

    /// <summary>
    /// Implementation of the <see cref="IDynamicWorldImp" /> interface using the bullet physics engine.
    /// </summary>
    public class DynamicWorldImp : IDynamicWorldImp
    {
        internal DiscreteDynamicsWorld BtWorld;
        internal CollisionConfiguration BtCollisionConf;
        internal CollisionDispatcher BtDispatcher;
        internal BroadphaseInterface BtBroadphase;
        internal ConstraintSolver BtSolver;
        //internal AlignedCollisionShapeArray BtCollisionShapes { get; private set; }
        List<CollisionShape> BtCollisionShapes = new List<CollisionShape>();


        internal DynamicWorldImp()
        {
            //Debug.WriteLine("DynamicWorldImp");

            //Default
            // collision configuration contains default setup for memory, collision setup
            BtCollisionConf = new DefaultCollisionConfiguration();
            BtDispatcher = new CollisionDispatcher(BtCollisionConf);
            BtBroadphase = new DbvtBroadphase();
            BtSolver = new SequentialImpulseConstraintSolver();
           // BtCollisionShapes = new AlignedCollisionShapeArray();
            

           
            BtWorld = new DiscreteDynamicsWorld(BtDispatcher, BtBroadphase, BtSolver, BtCollisionConf)
            {
                Gravity = new Vector3(0, -9.81f, 0)
            };
            
            BtWorld.SolverInfo.NumIterations = 5;

            BtWorld.PerformDiscreteCollisionDetection();
            
            //GImpactCollisionAlgorithm.RegisterAlgorithm(BtDispatcher);
           // BtWorld.SetInternalTickCallback(MyTickCallBack);
            //BtWorld.SetInternalTickCallback(TickTack);

            //ManifoldPoint.ContactAdded += OnContactAdded;
            //PersistentManifold.ContactDestroyed += OnContactDestroyed;
            //PersistentManifold.ContactProcessed += OnContactProcessed;
        }




        /// <summary>
        /// Gets and sets the gravity.
        /// </summary>
        /// <value>
        /// The gravity.
        /// </value>
        public float3 Gravity
        {
            get { return Translator.BtVector3ToFloat3(BtWorld.Gravity); }
            set { BtWorld.Gravity = Translator.Float3ToBtVector3(value); }
        }

        /// <summary>
        /// Called when [contact added].
        /// </summary>
        /// <param name="cp">The cp.</param>
        /// <param name="colObj0Wrap">The col obj0 wrap.</param>
        /// <param name="partId0">The part id0.</param>
        /// <param name="index0">The index0.</param>
        /// <param name="colObj1Wrap">The col obj1 wrap.</param>
        /// <param name="partId1">The part id1.</param>
        /// <param name="index1">The index1.</param>
        private void OnContactAdded(ManifoldPoint cp, CollisionObjectWrapper colObj0Wrap, int partId0, int index0,
            CollisionObjectWrapper colObj1Wrap, int partId1, int index1)
        {
            //Debug.WriteLine("OnContactAdded");
            int numManifolds = BtWorld.Dispatcher.NumManifolds;

            for (int i = 0; i < numManifolds; i++)
            {
                PersistentManifold contactManifold = BtWorld.Dispatcher.GetManifoldByIndexInternal(i);
                int numContacts = contactManifold.NumContacts;
                if (numContacts > 0)
                {
                    cp.UserPersistentData = 1;
                    
                    CollisionObject obA = (CollisionObject) contactManifold.Body0;
                    CollisionObject obB = (CollisionObject) contactManifold.Body1;
                    RigidBody btRigidBodyA = (RigidBody) obA;
                    RigidBody btRigidBodyB = (RigidBody)obB;
                    RigidBodyImp rigidBodyA = new RigidBodyImp();
                    RigidBodyImp rigidBodyB = new RigidBodyImp();
                    rigidBodyA._rbi = btRigidBodyA;
                    rigidBodyB._rbi = btRigidBodyB;
                    rigidBodyA.OnCollision(rigidBodyB);
                }
            }
        }

        /// <summary>
        /// Called when [contact processed].
        /// </summary>
        /// <param name="cp">The cp.</param>
        /// <param name="body0">The body0.</param>
        /// <param name="body1">The body1.</param>
        void OnContactProcessed(ManifoldPoint cp, CollisionObject body0, CollisionObject body1)
        {
           // Debug.WriteLine("OnContactProcessed");
            cp.UserPersistentData = 1;
        }

        /// <summary>
        /// Called when [contact destroyed].
        /// </summary>
        /// <param name="userPersistantData">The user persistant data.</param>
        void OnContactDestroyed(object userPersistantData)
        {
            int numManifolds = BtWorld.Dispatcher.NumManifolds;
            
            for (int i = 0; i < numManifolds; i++)
            {
                PersistentManifold contactManifold = BtWorld.Dispatcher.GetManifoldByIndexInternal(i);
                int numContacts = contactManifold.NumContacts;
                if (numContacts > 0)
                {
                    CollisionObject obA = (CollisionObject)contactManifold.Body0;
                    CollisionObject obB = (CollisionObject)contactManifold.Body1;
                    obA.CollisionFlags = CollisionFlags.CustomMaterialCallback;
                    obB.CollisionFlags = CollisionFlags.CustomMaterialCallback;
                }
            }
           // Debug.WriteLine("OnContactDestroyed");
        }

        /*private void MyTickCallBack(ManifoldPoint cp, CollisionObjectWrapper colobj0wrap, int partid0, int index0, CollisionObjectWrapper colobj1wrap, int partid1, int index1)
        {
            Debug.WriteLine("MyTickCallBack");
            int numManifolds = BtWorld.Dispatcher.NumManifolds;
            RigidBodyImp myRb;
            //Debug.WriteLine("numManifolds: " + numManifolds);
            for (int i = 0; i < numManifolds; i++)
            {
                PersistentManifold contactManifold = BtWorld.Dispatcher.GetManifoldByIndexInternal(i);
                int numContacts = contactManifold.NumContacts;
                if (numContacts > 0)
                {
                    CollisionObject obA = (CollisionObject) contactManifold.Body0;
                    CollisionObject obB = (CollisionObject) contactManifold.Body1;

                   // Debug.WriteLine(numContacts);
                    var pnA = obA.UserObject;

                    for (int j = 0; j < numContacts; j++)
                    {
                        ManifoldPoint pt = contactManifold.GetContactPoint(j);

                    }
                }
            }
        }*/


        /// <summary>
        /// Adds the rigid body.
        /// </summary>
        /// <param name="mass">The mass.</param>
        /// <param name="worldTransform">The world transform.</param>
        /// <param name="orientation">The orientation.</param>
        /// <param name="colShape">The col shape.</param>
        /// <returns></returns>
        public IRigidBodyImp AddRigidBody(float mass, float3 worldTransform, float3 orientation, ICollisionShapeImp colShape/*, float3 intertia*/)
        {
            // Use bullet to do what needs to be done:


             var btMatrix = Matrix.RotationX(orientation.x) 
                                    * Matrix.RotationY(orientation.y) 
                                    * Matrix.RotationZ(orientation.z) 
                                    * Matrix.Translation(worldTransform.x, worldTransform.y, worldTransform.z);
            
             var btMotionState = new DefaultMotionState(btMatrix);
          
            
            var shapeType = colShape.GetType().ToString();
            
            CollisionShape btColShape;

            // var isStatic = false;
            switch (shapeType)
            {
                //Primitives
                case "Fusee.Engine.BoxShapeImp":
                    var box = (BoxShapeImp) colShape;
                    var btBoxHalfExtents = Translator.Float3ToBtVector3(box.HalfExtents);
                    btColShape = new BoxShape(btBoxHalfExtents);
                    break;
                case "Fusee.Engine.CapsuleShapeImp":
                    var capsule = (CapsuleShapeImp) colShape;
                    btColShape = new CapsuleShape(capsule.Radius, capsule.HalfHeight);
                    break;
                case "Fusee.Engine.ConeShapeImp":
                    var cone = (ConeShapeImp) colShape;
                    btColShape = new ConeShape(cone.Radius, cone.Height);
                    break;
                case "Fusee.Engine.CylinderShapeImp":
                    var cylinider = (CylinderShapeImp) colShape;
                    var btCylinderHalfExtents = Translator.Float3ToBtVector3(cylinider.HalfExtents);
                    btColShape = new CylinderShape(btCylinderHalfExtents);
                    break;
                case "Fusee.Engine.MultiSphereShapeImp":
                    var multiSphere = (MultiSphereShapeImp) colShape;
                    var btPositions = new Vector3[multiSphere.SphereCount];
                    var btRadi = new float[multiSphere.SphereCount];
                    for (int i = 0; i < multiSphere.SphereCount; i++)
                    {
                        var pos = Translator.Float3ToBtVector3(multiSphere.GetSpherePosition(i));
                        btPositions[i] = pos;
                        btRadi[i] = multiSphere.GetSphereRadius(i);
                    }
                    btColShape = new MultiSphereShape(btPositions, btRadi);
                    break;
                case "Fusee.Engine.SphereShapeImp":
                    var sphere = (SphereShapeImp) colShape;
                    var btRadius = sphere.Radius;
                    btColShape = new SphereShape(btRadius);
                    break;
                
                //Misc
                case "Fusee.Engine.CompoundShapeImp":
                    var compShape = (CompoundShapeImp) colShape;
                    btColShape = new CompoundShape(true);
                    btColShape = compShape.BtCompoundShape;
                    break;
                case "Fusee.Engine.EmptyShapeImp":
                    btColShape = new EmptyShape();
                    break;

                //Meshes
                case "Fusee.Engine.ConvexHullShapeImp":
                    var convHull = (ConvexHullShapeImp) colShape;
                    var btPoints= new Vector3[convHull.GetNumPoints()];
                    for (int i = 0; i < convHull.GetNumPoints(); i++)
                    {
                        var point = convHull.GetScaledPoint(i);
                        btPoints[i] = Translator.Float3ToBtVector3(point);
                    }
                    btColShape = new ConvexHullShape(btPoints);
                    //btColShape.LocalScaling = new Vector3(3,3,3);
                    break;
                case "Fusee.Engine.StaticPlaneShapeImp":  
                    var staticPlane = (StaticPlaneShapeImp) colShape;
                    Debug.WriteLine("staticplane: " + staticPlane.Margin);
                    var btNormal = Translator.Float3ToBtVector3(staticPlane.PlaneNormal);
                    btColShape = new StaticPlaneShape(btNormal, staticPlane.PlaneConstant);
                    // isStatic = true;
                    //btColShape.Margin = 0.04f;
                    //Debug.WriteLine("btColshape" + btColShape.Margin);
                    break;               
                case "Fusee.Engine.GImpactMeshShapeImp":
                    var gImpMesh = (GImpactMeshShapeImp)colShape;
                    gImpMesh.BtGImpactMeshShape.UpdateBound();
                    var btGimp = new GImpactMeshShape(gImpMesh.BtGImpactMeshShape.MeshInterface);
                    
                    btGimp.UpdateBound();
                    btColShape = btGimp;
                    
                    break;
                //Default
                default:
                    Debug.WriteLine("defaultImp");
                    btColShape = new EmptyShape();
                    break;
            }
            
            var btLocalInertia = btColShape.CalculateLocalInertia(mass);
           // btLocalInertia *= (10.0f*10);
            RigidBodyConstructionInfo btRbcInfo = new RigidBodyConstructionInfo(mass, btMotionState, btColShape,
                btLocalInertia);

            var btRigidBody = new RigidBody(btRbcInfo);
            btRigidBody.Restitution = 0.2f;
            btRigidBody.Friction = 0.2f;
            btRigidBody.CollisionFlags = CollisionFlags.CustomMaterialCallback;
            
            BtWorld.AddRigidBody(btRigidBody);
            btRbcInfo.Dispose();
            var retval = new RigidBodyImp();
            retval._rbi = btRigidBody;
            btRigidBody.UserObject = retval;
            return retval;
        }

        /// <summary>
        /// Steps the simulation.
        /// </summary>
        /// <param name="timeSteps">The time steps.</param>
        /// <param name="maxSubSteps">The maximum sub steps.</param>
        /// <param name="fixedTimeSteps">The fixed time steps.</param>
        /// <returns></returns>
        public int StepSimulation(float timeSteps, int maxSubSteps, float fixedTimeSteps)
        {
            return BtWorld.StepSimulation(timeSteps, maxSubSteps);//, maxSubSteps, fixedTimeSteps);  
        }


        /// <summary>
        /// Checks the collisions.
        /// </summary>
        public void CheckCollisions()
        {
            Debug.WriteLine("CheckCollisions()");
            int numManifolds = BtWorld.Dispatcher.NumManifolds;
            for (int i = 0; i < numManifolds; i++)
            {
                PersistentManifold contactManifold = BtWorld.Dispatcher.GetManifoldByIndexInternal(i);
                CollisionObject obA = contactManifold.Body0 as CollisionObject;
                CollisionObject obB = contactManifold.Body1 as CollisionObject;

                int numContacts = contactManifold.NumContacts;
                for (int j = 0; j < numContacts; j++)
                {
                    ManifoldPoint pt = contactManifold.GetContactPoint(j);
                    if (pt.Distance < 0.0f)
                    {
                        Vector3 ptA = pt.PositionWorldOnA;
                        Vector3 ptB = pt.PositionWorldOnB;
                        Vector3 normalOnB = pt.NormalWorldOnB;
                    }
                }
            }
            
        }

        /// <summary>
        /// Gets the rigid body.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        public IRigidBodyImp GetRigidBody(int i)
        {
            var colisionObject = BtWorld.CollisionObjectArray[i];
            var btRigidBody = (RigidBody) colisionObject;
            return (RigidBodyImp) btRigidBody.UserObject;
        }

        /// <summary>
        /// Numbers the rigid bodies.
        /// </summary>
        /// <returns></returns>
        public int NumberRigidBodies()
        {
            var number = BtWorld.NumCollisionObjects;
            return number;
        }

        #region Constraints
        //Point2point
        /// <summary>
        /// Adds the point2 point constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="pivotInA">The pivot in a.</param>
        /// <returns></returns>
        public IPoint2PointConstraintImp AddPoint2PointConstraint(IRigidBodyImp rigidBodyA, float3 pivotInA)
        {
            var rigidBodyAImp = (RigidBodyImp)rigidBodyA;
            var btRigidBodyA = rigidBodyAImp._rbi;

            var btP2PConstraint = new Point2PointConstraint(btRigidBodyA, new Vector3(pivotInA.x, pivotInA.y, pivotInA.z));
            BtWorld.AddConstraint(btP2PConstraint);

            var retval = new Point2PointConstraintImp();
            retval._p2pci = btP2PConstraint;
            btP2PConstraint.UserObject = retval;
            return retval;
        }
        /// <summary>
        /// Adds the point2 point constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="rigidBodyB">The rigid body b.</param>
        /// <param name="pivotInA">The pivot in a.</param>
        /// <param name="pivotInB">The pivot in b.</param>
        /// <returns></returns>
        public IPoint2PointConstraintImp AddPoint2PointConstraint(IRigidBodyImp rigidBodyA, IRigidBodyImp rigidBodyB, float3 pivotInA,float3 pivotInB)
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

        /*//TODO: What about inheritance problems -> should return any constraint type
        public IPoint2PointConstraintImp GetConstraint(int i)
        {
            return (Point2PointConstraintImp)BtWorld.GetConstraint(0).UserObject;
        }*/


        //HingeConstraint
        /// <summary>
        /// Adds the hinge constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="frameInA">The frame in a.</param>
        /// <param name="useReferenceFrameA">if set to <c>true</c> [use reference frame a].</param>
        /// <returns></returns>
        public IHingeConstraintImp AddHingeConstraint(IRigidBodyImp rigidBodyA, float4x4 frameInA, bool useReferenceFrameA)
        {
            var rigidBodyAImp = (RigidBodyImp)rigidBodyA;
            var btRigidBodyA = rigidBodyAImp._rbi;
            var btFframeInA = Translator.Float4X4ToBtMatrix(frameInA);
            var btHingeConstraint = new HingeConstraint(btRigidBodyA, btFframeInA, useReferenceFrameA);
            BtWorld.AddConstraint(btHingeConstraint);

            var retval = new HingeConstraintImp();
            retval._hci = btHingeConstraint;
            btHingeConstraint.UserObject = retval;
            return retval;
        }
        /// <summary>
        /// Adds the hinge constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="pivotInA">The pivot in a.</param>
        /// <param name="axisInA">The axis in a.</param>
        /// <param name="useReferenceFrameA">if set to <c>true</c> [use reference frame a].</param>
        /// <returns></returns>
        public IHingeConstraintImp AddHingeConstraint(IRigidBodyImp rigidBodyA, float3 pivotInA, float3 axisInA, bool useReferenceFrameA)
        {
            var rigidBodyAImp = (RigidBodyImp)rigidBodyA;
            var btRigidBodyA = rigidBodyAImp._rbi;

            var btHingeConstraint = new HingeConstraint(btRigidBodyA, new Vector3(pivotInA.x, pivotInA.y, pivotInA.z), new Vector3(axisInA.x, axisInA.y, axisInA.z), useReferenceFrameA);
            BtWorld.AddConstraint(btHingeConstraint);

            var retval = new HingeConstraintImp();
            retval._hci = btHingeConstraint;
            btHingeConstraint.UserObject = retval;
            return retval;
        }
        /// <summary>
        /// Adds the hinge constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="rigidBodyB">The rigid body b.</param>
        /// <param name="pivotInA">The pivot in a.</param>
        /// <param name="pivotInB">The pivot in b.</param>
        /// <param name="axisInA">The axis in a.</param>
        /// <param name="AxisInB">The axis in b.</param>
        /// <param name="useReferenceFrameA">if set to <c>true</c> [use reference frame a].</param>
        /// <returns></returns>
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
        /// <summary>
        /// Adds the hinge constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="rigidBodyB">The rigid body b.</param>
        /// <param name="brAFrame">The br a frame.</param>
        /// <param name="brBFrame">The br b frame.</param>
        /// <param name="useReferenceFrameA">if set to <c>true</c> [use reference frame a].</param>
        /// <returns></returns>
        public IHingeConstraintImp AddHingeConstraint(IRigidBodyImp rigidBodyA, IRigidBodyImp rigidBodyB, float4x4 brAFrame, float4x4 brBFrame, bool useReferenceFrameA)
        {
            var rigidBodyAImp = (RigidBodyImp)rigidBodyA;
            var btRigidBodyA = rigidBodyAImp._rbi;

            var rigidBodyBImp = (RigidBodyImp)rigidBodyB;
            var btRigidBodyB = rigidBodyBImp._rbi;

            var btRbAFrame = Translator.Float4X4ToBtMatrix(brAFrame);
            var btRbBFrame = Translator.Float4X4ToBtMatrix(brBFrame);

            var btHingeConstraint = new HingeConstraint(btRigidBodyA, btRigidBodyB,btRbAFrame, btRbBFrame, useReferenceFrameA);
            BtWorld.AddConstraint(btHingeConstraint);

            var retval = new HingeConstraintImp();
            retval._hci = btHingeConstraint;
            btHingeConstraint.UserObject = retval;
            return retval;
        }


        //SliderConstraint
        /// <summary>
        /// Adds the slider constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="rigidBodyB">The rigid body b.</param>
        /// <param name="frameInA">The frame in a.</param>
        /// <param name="frameInB">The frame in b.</param>
        /// <param name="useLinearReferenceFrameA">if set to <c>true</c> [use linear reference frame a].</param>
        /// <returns></returns>
        public ISliderConstraintImp AddSliderConstraint(IRigidBodyImp rigidBodyA, IRigidBodyImp rigidBodyB, float4x4 frameInA, float4x4 frameInB, bool useLinearReferenceFrameA = false)
        {
            var rigidBodyAImp = (RigidBodyImp)rigidBodyA;
            var btRigidBodyA = rigidBodyAImp._rbi;

            var rigidBodyBImp = (RigidBodyImp)rigidBodyB;
            var btRigidBodyB = rigidBodyBImp._rbi;

            var btFrameInA = Translator.Float4X4ToBtMatrix(frameInA);
            var btFrameInB = Translator.Float4X4ToBtMatrix(frameInB);

            var btSliderConstraint = new SliderConstraint(btRigidBodyA, btRigidBodyB, btFrameInA, btFrameInB, useLinearReferenceFrameA);

            BtWorld.AddConstraint(btSliderConstraint);

            var retval = new SliderConstraintImp();
            retval._sci = btSliderConstraint;
            btSliderConstraint.UserObject = retval;
            return retval;
        }
        /// <summary>
        /// Adds the slider constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="frameInA">The frame in a.</param>
        /// <param name="useLinearReferenceFrameA">if set to <c>true</c> [use linear reference frame a].</param>
        /// <returns></returns>
        public ISliderConstraintImp AddSliderConstraint(IRigidBodyImp rigidBodyA, float4x4 frameInA, bool useLinearReferenceFrameA)
        {
            var rigidBodyAImp = (RigidBodyImp)rigidBodyA;
            var btRigidBodyA = rigidBodyAImp._rbi;
            var btFrameInA = Translator.Float4X4ToBtMatrix(frameInA);
            var btSliderConstraint = new SliderConstraint(btRigidBodyA, btFrameInA, useLinearReferenceFrameA);
            BtWorld.AddConstraint(btSliderConstraint);

            var retval = new SliderConstraintImp();
            retval._sci = btSliderConstraint;
            btSliderConstraint.UserObject = retval;
            return retval;
        }

        //GearConstraint
        /// <summary>
        /// Adds the gear constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="rigidBodyB">The rigid body b.</param>
        /// <param name="axisInA">The axis in a.</param>
        /// <param name="axisInB">The axis in b.</param>
        /// <param name="ratio">The ratio.</param>
        /// <returns></returns>
        public IGearConstraintImp AddGearConstraint(IRigidBodyImp rigidBodyA, IRigidBodyImp rigidBodyB, float3 axisInA, float3 axisInB, float ratio)
        {
            var rigidBodyAImp = (RigidBodyImp)rigidBodyA;
            var btRigidBodyA = rigidBodyAImp._rbi;

            var rigidBodyBImp = (RigidBodyImp)rigidBodyB;
            var btRigidBodyB = rigidBodyBImp._rbi;

            var btAxisInA = Translator.Float3ToBtVector3(axisInA);
            var btAxisInB = Translator.Float3ToBtVector3(axisInB);

            var btGearConstraint = new GearConstraint(btRigidBodyA, btRigidBodyB, btAxisInA, btAxisInB, ratio);

            BtWorld.AddConstraint(btGearConstraint);

            var retval = new GearConstraintImp();
            retval._gci = btGearConstraint;
            btGearConstraint.UserObject = retval;
            return retval;
        }

        //ConeTwistConstraint
        /// <summary>
        /// Adds the cone twist constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="rbAFrame">The rb a frame.</param>
        /// <returns></returns>
        public IConeTwistConstraintImp AddConeTwistConstraint(IRigidBodyImp rigidBodyA, float4x4 rbAFrame)
        {
            var rigidBodyAImp = (RigidBodyImp)rigidBodyA;
            var btRigidBodyA = rigidBodyAImp._rbi;
            var btRbAFrame = Translator.Float4X4ToBtMatrix(rbAFrame);

            var btCTConstraint = new ConeTwistConstraint(btRigidBodyA, btRbAFrame);
            
            BtWorld.AddConstraint(btCTConstraint);

            var retval = new ConeTwistConstraintImp();
            retval._cti = btCTConstraint;
            btCTConstraint.UserObject = retval;
            return retval;
        }
        /// <summary>
        /// Adds the cone twist constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="rigidBodyB">The rigid body b.</param>
        /// <param name="rbAFrame">The rb a frame.</param>
        /// <param name="rbBFrame">The rb b frame.</param>
        /// <returns></returns>
        public IConeTwistConstraintImp AddConeTwistConstraint(IRigidBodyImp rigidBodyA, IRigidBodyImp rigidBodyB, float4x4 rbAFrame,float4x4 rbBFrame)
        {
            var rigidBodyAImp = (RigidBodyImp)rigidBodyA;
            var btRigidBodyA = rigidBodyAImp._rbi;

            var rigidBodyBImp = (RigidBodyImp)rigidBodyB;
            var btRigidBodyB = rigidBodyBImp._rbi;

            var btRbAFrame = Translator.Float4X4ToBtMatrix(rbAFrame);
            var btRbBFrame = Translator.Float4X4ToBtMatrix(rbBFrame);

            var btCTConstraint = new ConeTwistConstraint(btRigidBodyA, btRigidBodyB, btRbAFrame, btRbBFrame);

            BtWorld.AddConstraint(btCTConstraint);

            var retval = new ConeTwistConstraintImp();
            retval._cti = btCTConstraint;
            btCTConstraint.UserObject = retval;
            return retval;
        }


        //GenericoDofConstraint
        /// <summary>
        /// Adds the generic6 dof constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="frameInA">The frame in a.</param>
        /// <param name="useReferenceFrameA">if set to <c>true</c> [use reference frame a].</param>
        /// <returns></returns>
        public IGeneric6DofConstraintImp AddGeneric6DofConstraint(IRigidBodyImp rigidBodyA, float4x4 frameInA, bool useReferenceFrameA)
        {
            var rigidBodyAImp = (RigidBodyImp)rigidBodyA;
            var btRigidBodyA = rigidBodyAImp._rbi;
            var btFframeInA = Translator.Float4X4ToBtMatrix(frameInA);
            var btGeneric6DofConstraint = new Generic6DofConstraint(btRigidBodyA, btFframeInA, useReferenceFrameA);
            BtWorld.AddConstraint(btGeneric6DofConstraint);

            var retval = new Generic6DofConstraintImp();
            retval._g6dofci = btGeneric6DofConstraint;
            btGeneric6DofConstraint.UserObject = retval;
            return retval;
        }
        /// <summary>
        /// Adds the generic6 dof constraint.
        /// </summary>
        /// <param name="rigidBodyA">The rigid body a.</param>
        /// <param name="rigidBodyB">The rigid body b.</param>
        /// <param name="frameInA">The frame in a.</param>
        /// <param name="frameInB">The frame in b.</param>
        /// <param name="useReferenceFrameA">if set to <c>true</c> [use reference frame a].</param>
        /// <returns></returns>
        public IGeneric6DofConstraintImp AddGeneric6DofConstraint(IRigidBodyImp rigidBodyA, IRigidBodyImp rigidBodyB, float4x4 frameInA, float4x4 frameInB, bool useReferenceFrameA = false)
        {
            var rigidBodyAImp = (RigidBodyImp)rigidBodyA;
            var btRigidBodyA = rigidBodyAImp._rbi;
            var rigidBodyBImp = (RigidBodyImp)rigidBodyB;
            var btRigidBodyB = rigidBodyAImp._rbi;

            Matrix matrixA = Translator.Float4X4ToBtMatrix(frameInA);
            Matrix matrixB = Translator.Float4X4ToBtMatrix(frameInB);

            var btGeneric6DofConstraint = new Generic6DofConstraint(btRigidBodyA, btRigidBodyB, matrixA, matrixB, useReferenceFrameA);
            BtWorld.AddConstraint(btGeneric6DofConstraint);


            var retval = new Generic6DofConstraintImp();
            retval._g6dofci = btGeneric6DofConstraint;
            btGeneric6DofConstraint.UserObject = retval;
            return retval;
        }
        #endregion Constraints

        #region CollisionShapes
        //CollisionShapes
        //Primitives
        //BoxShape
        /// <summary>
        /// Adds the box shape.
        /// </summary>
        /// <param name="boxHalfExtents">The box half extents.</param>
        /// <returns></returns>
        public IBoxShapeImp AddBoxShape(float boxHalfExtents)
        {
            var btBoxShape = new BoxShape(boxHalfExtents);
            BtCollisionShapes.Add(btBoxShape);
            
            var retval = new BoxShapeImp();
            retval.BtBoxShape = btBoxShape;
            btBoxShape.UserObject = retval;
            return retval;
        }
        /// <summary>
        /// Adds the box shape.
        /// </summary>
        /// <param name="boxHalfExtents">The box half extents.</param>
        /// <returns></returns>
        public IBoxShapeImp AddBoxShape(float3 boxHalfExtents)
        {
            var btBoxShape = new BoxShape(Translator.Float3ToBtVector3(boxHalfExtents));
            BtCollisionShapes.Add(btBoxShape);

            var retval = new BoxShapeImp();
            retval.BtBoxShape = btBoxShape;
            btBoxShape.UserObject = retval;
            return retval;
        }
        /// <summary>
        /// Adds the box shape.
        /// </summary>
        /// <param name="boxHalfExtentsX">The box half extents x.</param>
        /// <param name="boxHalfExtentsY">The box half extents y.</param>
        /// <param name="boxHalfExtentsZ">The box half extents z.</param>
        /// <returns></returns>
        public IBoxShapeImp AddBoxShape(float boxHalfExtentsX, float boxHalfExtentsY, float boxHalfExtentsZ)
        {
            var btBoxShape = new BoxShape(boxHalfExtentsX, boxHalfExtentsY, boxHalfExtentsZ);
            BtCollisionShapes.Add(btBoxShape);

            var retval = new BoxShapeImp();
            retval.BtBoxShape = btBoxShape;
            btBoxShape.UserObject = retval;
            return retval;
        }

        //SphereShape
        /// <summary>
        /// Adds the sphere shape.
        /// </summary>
        /// <param name="radius">The radius.</param>
        /// <returns></returns>
        public ISphereShapeImp AddSphereShape(float radius)
        {
            var btSphereShape = new SphereShape(radius);
            BtCollisionShapes.Add(btSphereShape);
            
            var retval = new SphereShapeImp();
            retval.BtSphereShape = btSphereShape;
            btSphereShape.UserObject = retval;
            return retval;
        }

        //CapsuleShape
        /// <summary>
        /// Adds the capsule shape.
        /// </summary>
        /// <param name="radius">The radius.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public ICapsuleShapeImp AddCapsuleShape(float radius, float height)
        {
            var btCapsuleShape = new CapsuleShape(radius, height);
            BtCollisionShapes.Add(btCapsuleShape);

            var retval = new CapsuleShapeImp();
            retval.BtCapsuleShape = btCapsuleShape;
            btCapsuleShape.UserObject = retval;
            return retval;
        }

        //CylinderShape
        /// <summary>
        /// Adds the cylinder shape.
        /// </summary>
        /// <param name="halfExtents">The half extents.</param>
        /// <returns></returns>
        public ICylinderShapeImp AddCylinderShape(float halfExtents)
        {
            var btCylinderShape = new CylinderShape(halfExtents);
            BtCollisionShapes.Add(btCylinderShape);

            var retval = new CylinderShapeImp();
            retval.BtCylinderShape = btCylinderShape;
            btCylinderShape.UserObject = retval;
            return retval;
        }
        /// <summary>
        /// Adds the cylinder shape.
        /// </summary>
        /// <param name="halfExtents">The half extents.</param>
        /// <returns></returns>
        public ICylinderShapeImp AddCylinderShape(float3 halfExtents)
        {
            var btCylinderShape = new CylinderShape(Translator.Float3ToBtVector3(halfExtents));
            BtCollisionShapes.Add(btCylinderShape);

            var retval = new CylinderShapeImp();
            retval.BtCylinderShape = btCylinderShape;
            btCylinderShape.UserObject = retval;
            return retval;
        }
        /// <summary>
        /// Adds the cylinder shape.
        /// </summary>
        /// <param name="halfExtentsX">The half extents x.</param>
        /// <param name="halfExtentsY">The half extents y.</param>
        /// <param name="halfExtentsZ">The half extents z.</param>
        /// <returns></returns>
        public ICylinderShapeImp AddCylinderShape(float halfExtentsX, float halfExtentsY, float halfExtentsZ)
        {
            var btCylinderShape = new CylinderShape(halfExtentsX, halfExtentsY, halfExtentsZ);
            BtCollisionShapes.Add(btCylinderShape);

            var retval = new CylinderShapeImp();
            retval.BtCylinderShape = btCylinderShape;
            btCylinderShape.UserObject = retval;
            return retval;
        }


        //ConeShape
        /// <summary>
        /// Adds the cone shape.
        /// </summary>
        /// <param name="radius">The radius.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public IConeShapeImp AddConeShape(float radius, float height)
        {
            var btConeShape = new ConeShape(radius, height);
            BtCollisionShapes.Add(btConeShape);

            var retval = new ConeShapeImp();
            retval.BtConeShape = btConeShape;
            btConeShape.UserObject = retval;
            return retval;
        }

        //MultiSphereShape
        /// <summary>
        /// Adds the multi sphere shape.
        /// </summary>
        /// <param name="positions">The positions.</param>
        /// <param name="radi">The radi.</param>
        /// <returns></returns>
        public IMultiSphereShapeImp AddMultiSphereShape(float3[] positions, float[] radi)
        {
            var btPositions = new Vector3[positions.Length];
            for (int i = 0; i < positions.Length; i++)
            {
                btPositions[i] = new Vector3(positions[i].x, positions[i].y, positions[i].z);
            }
            var btMultiSphereShape = new MultiSphereShape(btPositions, radi);
            BtCollisionShapes.Add(btMultiSphereShape);

            var retval = new MultiSphereShapeImp();
            retval.BtMultiSphereShape = btMultiSphereShape;
            btMultiSphereShape.UserObject = retval;
            return retval;
        }

        /// <summary>
        /// Adds the compound shape.
        /// </summary>
        /// <param name="enableDynamicAabbTree">if set to <c>true</c> [enable dynamic aabb tree].</param>
        /// <returns></returns>
        public ICompoundShapeImp AddCompoundShape(bool enableDynamicAabbTree)
        {
            var btCompoundShape = new CompoundShape(enableDynamicAabbTree);
            BtCollisionShapes.Add(btCompoundShape);

            var retval = new CompoundShapeImp();
            retval.BtCompoundShape = btCompoundShape;
            btCompoundShape.UserObject = retval;
            return retval;
        }

        /// <summary>
        /// Adds the empty shape.
        /// </summary>
        /// <returns></returns>
        public IEmptyShapeImp AddEmptyShape()
        {
            var btEmptyShape = new EmptyShape();
            BtCollisionShapes.Add(btEmptyShape);

            var retval = new EmptyShapeImp();
            retval.BtEmptyShape = btEmptyShape;
            btEmptyShape.UserObject = retval;
            return retval;
        }

        /// <summary>
        /// Adds the convex hull shape.
        /// </summary>
        /// <returns></returns>
        public IConvexHullShapeImp AddConvexHullShape()
        {
            var btConvexHullShape = new ConvexHullShape();
            BtCollisionShapes.Add(btConvexHullShape);

            var retval = new ConvexHullShapeImp();
            retval.BtConvexHullShape = btConvexHullShape;
            btConvexHullShape.UserObject = retval;
            return retval;
        }

        /// <summary>
        /// Adds the convex hull shape.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="optimized">if set to <c>true</c> [optimized].</param>
        /// <returns></returns>
        public IConvexHullShapeImp AddConvexHullShape(float3[] points, bool optimized)
        {
            var btPoints = new Vector3[points.Count()];
            for (int i = 0; i < btPoints.Count(); i++)
            {
                var point = Translator.Float3ToBtVector3(points[i]);
                btPoints[i] = point;
            }


            var btConvexHullShape = new ConvexHullShape(btPoints);
            //btConvexHullShape.LocalScaling = new Vector3(3, 3, 3);
            if (optimized == true)
            {
                var btShapeHull = new ShapeHull(btConvexHullShape);
                var margin = btConvexHullShape.Margin;
                btShapeHull.BuildHull(margin);
                ConvexHullShape simplifiedConvexShape = new ConvexHullShape(btShapeHull.Vertices);
               
                BtCollisionShapes.Add(simplifiedConvexShape);
                
                var retval = new ConvexHullShapeImp();
                retval.BtConvexHullShape = simplifiedConvexShape;
                simplifiedConvexShape.UserObject = retval;
                return retval;
            }
            else
            {
                BtCollisionShapes.Add(btConvexHullShape);

                var retval = new ConvexHullShapeImp();
                retval.BtConvexHullShape = btConvexHullShape;
                btConvexHullShape.UserObject = retval;
                return retval;    
            }
            
        }

        /// <summary>
        /// Adds the static plane shape.
        /// </summary>
        /// <param name="planeNormal">The plane normal.</param>
        /// <param name="planeConstant">The plane constant.</param>
        /// <returns></returns>
        public IStaticPlaneShapeImp AddStaticPlaneShape(float3 planeNormal, float planeConstant)
        {
            var btPlaneNormal = Translator.Float3ToBtVector3(planeNormal);
            var btStaticPlaneShape = new StaticPlaneShape(btPlaneNormal, planeConstant);
            btStaticPlaneShape.Margin = 0.04f;
            BtCollisionShapes.Add(btStaticPlaneShape);
            Debug.WriteLine("btStaticPlaneShape.Margin" + btStaticPlaneShape.Margin);
            var retval = new StaticPlaneShapeImp();
            retval.BtStaticPlaneShape = btStaticPlaneShape;
            btStaticPlaneShape.UserObject = retval;
            return retval;
        }

        /// <summary>
        /// Adds the g impact mesh shape.
        /// </summary>
        /// <param name="meshTriangles">The mesh triangles.</param>
        /// <param name="meshVertices">The mesh vertices.</param>
        /// <returns></returns>
        public IGImpactMeshShapeImp AddGImpactMeshShape(int[] meshTriangles, float3[] meshVertices)
        {
            Vector3[] btMeshVertices = new Vector3[meshVertices.Length];
            for (int i = 0; i < meshVertices.Length; i++)
            {
                btMeshVertices[i].X = meshVertices[i].x;
                btMeshVertices[i].Y = meshVertices[i].y;
                btMeshVertices[i].Z = meshVertices[i].z;
            }
            var btTriangleIndexVertexArray = new TriangleIndexVertexArray(meshTriangles, btMeshVertices);
            var btGimpactMeshShape = new GImpactMeshShape(btTriangleIndexVertexArray);
            btGimpactMeshShape.UpdateBound();
            BtCollisionShapes.Add(btGimpactMeshShape);
            var retval = new GImpactMeshShapeImp();
            retval.BtGImpactMeshShape = btGimpactMeshShape;
            btGimpactMeshShape.UserObject = retval;
            return retval;
        }
        #endregion CollisionShapes


        /// <summary>
        /// Numbers the constraints.
        /// </summary>
        /// <returns></returns>
        public int NumberConstraints()
        {
            return BtWorld.NumConstraints;
        }

        /*This Funcion is called at:
         * public static void Main()
           {
               var app = new BulletTest();
                app.Run();
                _physic.World.Dispose();
           }
         * definetly the wrong place!!!!!!!!
         * TODO: call it at the right place
         */
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            if (BtWorld != null)
            {
                
                /* for (int d = 0; d < BtWorld.Dispatcher.NumManifolds; d++)
                {
                    var m = BtWorld.Dispatcher.GetManifoldByIndexInternal(d);
                    BtWorld.Dispatcher.ReleaseManifold(m);
                    ;
                }*/
                

                //remove/dispose constraints
                int i;
                for (i = BtWorld.NumConstraints - 1; i >= 0; i--)
                {
                    TypedConstraint constraint = BtWorld.GetConstraint(i);
                    BtWorld.RemoveConstraint(constraint);
                    constraint.Dispose(); 
                }
               

                //remove the rigidbodies from the dynamics world and delete them
                for (i = BtWorld.NumCollisionObjects - 1; i >= 0; i--)
                {
                    CollisionObject obj = BtWorld.CollisionObjectArray[i];
                    RigidBody body = obj as RigidBody;
                    if (body != null && body.MotionState != null)
                    {
                        body.MotionState.Dispose();
                    }
                    BtWorld.RemoveCollisionObject(obj);
                    obj.Dispose();
                }

                //delete collision shapes
                foreach (CollisionShape shape in BtCollisionShapes)
                    shape.Dispose();
                BtCollisionShapes.Clear();

                

                BtWorld.Dispose();
                BtBroadphase.Dispose();
                BtDispatcher.Dispose();
                BtCollisionConf.Dispose();
            }

            if (BtBroadphase != null)
            {
                BtBroadphase.Dispose();
            }
            if (BtDispatcher != null)
            {
                BtDispatcher.Dispose();
            }
            if (BtCollisionConf != null)
            {
                BtCollisionConf.Dispose();
            }
        }

    }
}
