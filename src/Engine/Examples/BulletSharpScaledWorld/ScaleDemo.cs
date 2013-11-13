using System;
using System.Diagnostics;
using BulletSharp;
using BulletSharp.MultiThreaded;

namespace Examples.BulletSharpScaledWorld
{
    class ScaleDemo
    {
        //Vector3 eye = new Vector3(30, 20, 10);
        //Vector3 target = new Vector3(0, 5, -4);

        // create 125 (5x5x5) dynamic objects
        const int ArraySizeX = 5, ArraySizeY = 5, ArraySizeZ = 5;

        // scaling of the objects (0.1 = 20 centimeter boxes )
        const float StartPosX = -5;
        const float StartPosY = -5;
        const float StartPosZ = -3;

        // Frame counting
        Clock clock = new Clock();
        float frameAccumulator;
        int frameCount;

        float _frameDelta;
        public float FrameDelta
        {
            get { return _frameDelta; }
        }
        public float FramesPerSecond { get; private set; }

        // Physics
        DynamicsWorld _world;
        public DynamicsWorld World
        {
            get { return _world; }
            protected set { _world = value; }
        }

        protected CollisionConfiguration CollisionConf;
        protected CollisionDispatcher Dispatcher;
        protected BroadphaseInterface Broadphase;
        protected ConstraintSolver Solver;
        public AlignedCollisionShapeArray CollisionShapes { get; private set; }

        protected BoxShape shootBoxShape;
        protected float shootBoxInitialSpeed = 200;
        RigidBody pickedBody;
        protected TypedConstraint pickConstraint;


        protected void OnInitialize()
        {
            Debug.WriteLine("OnInitialize()");
        }

        protected void OnInitializePhysics()
        {
            // collision configuration contains default setup for memory, collision setup
            CollisionConf = new DefaultCollisionConfiguration();
            Dispatcher = new CollisionDispatcher(CollisionConf);

            Broadphase = new DbvtBroadphase();
 
            World = new DiscreteDynamicsWorld(Dispatcher, Broadphase, null, CollisionConf);
            World.Gravity = new Vector3(0, -90.81f, 0);
            

            // create plane
            
            // create the ground
            BoxShape groundShape1 = new BoxShape(500.0f, 1, 500.0f);
  
            CollisionShapes.Add(groundShape1);
           
            Matrix ground = Matrix.Translation(0, 0, 0);
            CollisionObject ground1 = LocalCreateRigidBody(0, ground, groundShape1);
            

            // create a few dynamic rigidbodies
            const float mass = 1.0f;

            //SphereShape colShape = new SphereShape(35);
            BoxShape colShape = new BoxShape(40);
            CollisionShapes.Add(colShape);
            Vector3 localInertia = colShape.CalculateLocalInertia(mass);
            
            const float start_x = StartPosX - ArraySizeX / 2;
            const float start_y = StartPosY;
            const float start_z = StartPosZ - ArraySizeZ / 2;

            int k, i, j;
            for (k = 0; k < ArraySizeY; k++)
            {
                for (i = 0; i < ArraySizeX; i++)
                {
                    for (j = 0; j < ArraySizeZ; j++)
                    {
                        Matrix startTransform = Matrix.Translation(
                            40 * i + start_x,
                            40 * k + start_y,
                            40 * j + start_z
                        );

                        // using motionstate is recommended, it provides interpolation capabilities
                        // and only synchronizes 'active' objects
                        DefaultMotionState myMotionState = new DefaultMotionState(startTransform);
                        RigidBodyConstructionInfo rbInfo =
                            new RigidBodyConstructionInfo(mass, myMotionState, colShape, localInertia);
                        RigidBody body = new RigidBody(rbInfo);
                        rbInfo.Dispose();

                        // make it drop from a height
                        body.Translate(new Vector3(0, 200, 0));

                        World.AddRigidBody(body);
                    }
                }
            }
                
        }

        public ScaleDemo()
        {
            CollisionShapes = new AlignedCollisionShapeArray();
            Run();
        }

        public void Run()
        {

            OnInitialize();
            OnInitializePhysics();
        }

        /*public virtual void ClientResetScene()
        {
            RemovePickingConstraint();
            ExitPhysics();
            OnInitializePhysics();
        }*/

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
                for (i = _world.NumCollisionObjects - 1; i >= 0; i--)
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

        void RemovePickingConstraint()
        {
            if (pickConstraint != null && _world != null)
            {
                _world.RemoveConstraint(pickConstraint);
                pickConstraint.Dispose();
                pickConstraint = null;
                pickedBody.ForceActivationState(ActivationState.ActiveTag);
                pickedBody.DeactivationTime = 0;
                pickedBody = null;
            }
        }


        public virtual void ShootBox(Vector3 camPos, Vector3 destination)
        {
            if (_world == null)
                return;

            const float mass = 1.0f;

            if (shootBoxShape == null)
            {
                shootBoxShape = new BoxShape(40);
                //shootBoxShape.InitializePolyhedralFeatures();
            }

            RigidBody body = LocalCreateRigidBody(mass, Matrix.Translation(camPos), shootBoxShape);
            body.LinearFactor = new Vector3(1, 1, 1);
            //body.Restitution = 1;

            Vector3 linVel = (destination - camPos)*10;
            linVel.Normalize();

            body.LinearVelocity = linVel * shootBoxInitialSpeed;
            body.CcdMotionThreshold = 0.5f;
            body.CcdSweptSphereRadius = 0.9f;
        }

        public virtual RigidBody LocalCreateRigidBody(float mass, Matrix startTransform, CollisionShape shape)
        {
            //rigidbody is dynamic if and only if mass is non zero, otherwise static
            bool isDynamic = (mass != 0.0f);

            Vector3 localInertia = Vector3.Zero;
            if (isDynamic)
                shape.CalculateLocalInertia(mass, out localInertia);

            //using motionstate is recommended, it provides interpolation capabilities, and only synchronizes 'active' objects
            DefaultMotionState myMotionState = new DefaultMotionState(startTransform);

            RigidBodyConstructionInfo rbInfo = new RigidBodyConstructionInfo(mass, myMotionState, shape, localInertia);
            RigidBody body = new RigidBody(rbInfo);
            rbInfo.Dispose();

            _world.AddRigidBody(body);

            return body;
        }
    }
}
