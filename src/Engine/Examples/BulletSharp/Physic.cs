using System;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using Fusee.Engine;
using Fusee.Math;
using BulletSharp;
using BoxShape = BulletSharp.BoxShape;
using CollisionShape = BulletSharp.CollisionShape;
using CompoundShape = BulletSharp.CompoundShape;
using ConeShape = BulletSharp.ConeShape;
using ConeTwistConstraint = BulletSharp.ConeTwistConstraint;
using ConvexHullShape = BulletSharp.ConvexHullShape;
using GearConstraint = BulletSharp.GearConstraint;
using Point2PointConstraint = BulletSharp.Point2PointConstraint;
using RigidBody = BulletSharp.RigidBody;
using SliderConstraint = BulletSharp.SliderConstraint;
using HingeConstraint = BulletSharp.HingeConstraint;
using Generic6DofConstraint = BulletSharp.Generic6DofConstraint;
using MultiSphereShape = BulletSharp.MultiSphereShape;
using SphereShape = BulletSharp.SphereShape;
using StaticPlaneShape = BulletSharp.StaticPlaneShape;

namespace Examples.BulletSharp
{
    class Physic
    {
        private DynamicsWorld _world;

        public DynamicsWorld World
        {
            get { return _world; }
            set { _world = value; }
        }

        protected CollisionConfiguration CollisionConf;
        protected Dispatcher Dispatcher;
        protected BroadphaseInterface Broadphase;
        protected ConstraintSolver Solver;
        public AlignedCollisionShapeArray CollisionShapes { get; private set; }

        public Physic()
        {
            Debug.WriteLine("Physic: Constructor");

            // collision configuration contains default setup for memory, collision setup
            CollisionConf = new DefaultCollisionConfiguration();
            Dispatcher = new CollisionDispatcher(CollisionConf);
            
            Broadphase = new DbvtBroadphase();
            Solver = new SequentialImpulseConstraintSolver();
          
            World = new DiscreteDynamicsWorld(Dispatcher, Broadphase, Solver, CollisionConf)
            {
                Gravity = new Vector3(0, -9.81f *10.0f , 0)
            };
            World.SolverInfo.NumIterations = 8;
            //Ground();
            //Tester();
            Plane();
            FallingTower();
            //Constraints();
        }

        public void Plane()
        {
            var planeShape = new StaticPlaneShape(Vector3.UnitY, 10);
            var pos = new float3(0,0,0);
            var startTransform = Matrix.Translation(pos.x, pos.y, pos.z);
            var myMotionState = new DefaultMotionState(startTransform);


            RigidBodyConstructionInfo rbInfo = new RigidBodyConstructionInfo(0, myMotionState, planeShape);
            RigidBody rigidBodyA = new RigidBody(rbInfo);
            World.AddRigidBody(rigidBodyA);
            rbInfo.Dispose;
        }

        public void Ground()
        {
            //create ground
            int size = 2;
            for (int b = -size; b < size; b++)
            {
                for (int c = -size; c < size; c++)
                {
                   
                    var pos = new float3(b * 10, 0, c * 10);
                    var startTransform = Matrix.Translation(pos.x, pos.y, pos.z);
                    var myMotionState = new DefaultMotionState(startTransform);
                    var rbInfo = new RigidBodyConstructionInfo(0, myMotionState, new BoxShape(2));
                    RigidBody rigidBody = new RigidBody(rbInfo);

                    World.AddRigidBody(rigidBody);
                }
            }
        }


        public void FallingTower()
        {
            Mesh mesh = MeshReader.LoadMesh(@"Assets/Teapot.obj.model");
            
            Vector3[] verts =
            {
                new Vector3(0, -25, -25), new Vector3(25, -25, -25), new Vector3(-25, -25, 25), new Vector3(25, -25, 25),
                new Vector3(0, 25, 0), new Vector3(25, 25, -25), new Vector3(-25, 25, 25), new Vector3(25, 25, 25)
            };
            var btColShape = new ConvexHullShape(verts);
            Debug.WriteLine("Init Falling Tower");
            for (int k = 1; k < 4; k++)
            {
                for (int h = -2; h < 2; h++)
                {
                    for (int j = -2; j < 2; j++)
                    {
                        var pos = new float3(2 * h, 400 + (k * 4), 2 * j);
                       
                        var startTransform = Matrix.Translation(pos.x, pos.y, pos.z);
                        var myMotionState = new DefaultMotionState(startTransform);
                        
                        var rbInfo = new RigidBodyConstructionInfo(1, myMotionState, new BoxShape(25));
                        RigidBody rigidBody = new RigidBody(rbInfo);
                        World.AddRigidBody(rigidBody);
                    }
                }
            }
        }


        public void Constraints()
        {
            Debug.WriteLine("Init Constraints");

            var posA = new Vector3(-10,30, 0);
            var startTransformA = Matrix.Translation(0,20,0);
            var myMotionStateA = new DefaultMotionState(startTransformA);
            var rbInfoA = new RigidBodyConstructionInfo(1, myMotionStateA, new BoxShape(2));
            var rigidBodyA = new RigidBody(rbInfoA);
            rigidBodyA.LinearFactor = new Vector3(0,0,0); //More or Less Static
            
            World.AddRigidBody(rigidBodyA);
            rbInfoA.Dispose();
            var posB = new Vector3(10,0, 0);
            var startTransformB = Matrix.Translation(0,20,0);
            var myMotionStateB = new DefaultMotionState(startTransformB);
            var rbInfoB = new RigidBodyConstructionInfo(1, myMotionStateB, new BoxShape(2));
            var rigidBodyB = new RigidBody(rbInfoB);
            rbInfoB.Dispose();
            World.AddRigidBody(rigidBodyB);
            
            var pivotInA = new Vector3(0, 20, 0);

            var p2p1 = new Point2PointConstraint(rigidBodyA, rigidBodyB, pivotInA, pivotInA);
            
            World.AddConstraint(p2p1);
            p2p1.SetParam(ConstraintParam.Cfm, 0.9f);
            p2p1.SetParam(ConstraintParam.Erp, 0.1f);
            p2p1.SetParam(1, 0.1f, 1);
            p2p1.Setting.Tau = 0.0001f; 

            var hinge = new HingeConstraint(rigidBodyA, posA, posB);
            
            var slider = new SliderConstraint(rigidBodyA, rigidBodyB, startTransformA, startTransformB, true);
            var sl1 = new SliderConstraint(rigidBodyA, startTransformB, true);
            var coneTwist = new ConeTwistConstraint(rigidBodyA, rigidBodyB, startTransformA, startTransformB);
            var dof6 = new Generic6DofConstraint(rigidBodyA, rigidBodyB, startTransformA, startTransformB, false);
            var gear = new GearConstraint(rigidBodyA, rigidBodyB, pivotInA, pivotInA, 0.5f);  
        }


        public void Tester()
        {
           /* var posA = new Vector3(0,500, 0);
            var startTransformA = Matrix.Translation(0, 350, 0);
            var myMotionStateA = new DefaultMotionState(startTransformA);
            var colShape = new SphereShape(5);
            var rbInfoA = new RigidBodyConstructionInfo(1, myMotionStateA, colShape);
            var rigidBodyA = new RigidBody(rbInfoA);
            _world.AddRigidBody(rigidBodyA);
            */


            var plane = new StaticPlaneShape(Vector3.UnitY, 10);

            var posB = new Vector3(0, 0, 0);
            var startTransformB = Matrix.Translation(0, 0, 0);
            var myMotionStateB = new DefaultMotionState(startTransformB);
            var rbInfoB = new RigidBodyConstructionInfo(0, myMotionStateB, plane);
            var rigidBodyB = new RigidBody(rbInfoB);
            World.AddRigidBody(rigidBodyB);    
        }


        //Manage Memory
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ExitPhysics();
            }
        }

        public virtual void ExitPhysics()
        {
            if (_world != null)
            {
                //remove/dispose constraints
                int i;
                for (i = _world.NumConstraints - 1; i >= 0; i--)
                {
                    TypedConstraint constraint = _world.GetConstraint(i);
                    _world.RemoveConstraint(constraint);
                    constraint.Dispose(); ;
                }

                //remove the rigidbodies from the dynamics world and delete them
                for (i = _world.NumCollisionObjects -1; i >= 0; i--)
                {
                    CollisionObject obj = _world.CollisionObjectArray[i];
                    RigidBody body = obj as RigidBody;
                    if (body != null && body.MotionState != null)
                    {
                        body.MotionState.Dispose();
                    }
                    _world.RemoveCollisionObject(obj);
                    obj.Dispose();
                }

                //delete collision shapes
                foreach (CollisionShape shape in CollisionShapes)
                    shape.Dispose();
                CollisionShapes.Clear();

                _world.Dispose();
                Broadphase.Dispose();
                Dispatcher.Dispose();
                CollisionConf.Dispose();
            }

            if (Broadphase != null)
            {
                Broadphase.Dispose();
            }
            if (Dispatcher != null)
            {
                Dispatcher.Dispose();
            }
            if (CollisionConf != null)
            {
                CollisionConf.Dispose();
            }
        }

        public virtual void ClientResetScene()
        {
            ExitPhysics();

        }
    }
}
