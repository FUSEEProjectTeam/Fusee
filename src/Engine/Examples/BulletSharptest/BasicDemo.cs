using System;
using BulletSharp;
using Fusee.Engine;
using System.Diagnostics;
using System.Collections.Generic;

namespace Examples.BulletSharp
{
    class BasicDemo : RenderCanvas
    {
        
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
        public List<RigidBody> BulletArray = new List<RigidBody>();
        protected SphereShape shootSphereShape;
        protected float shootSphereInitialSpeed = 40;
        RigidBody pickedBody;
        protected TypedConstraint pickConstraint;
        float oldPickingDist;

        #region basicDemo
        // create 125 (5x5x5) dynamic objects
        const int ArraySizeX = 5, ArraySizeY = 5, ArraySizeZ = 5;

        // scaling of the objects (0.1 = 20 centimeter boxes )
        const float StartPosX = -5;
        const float StartPosY = 200;
        const float StartPosZ = -3;

        public RigidBody box;
       
        #endregion basicDemo


        public BasicDemo()
        {
            CollisionShapes = new AlignedCollisionShapeArray();
        }

        public void Run()
        {
            OnInitialize();
            OnInitializePhysics();

            clock.Start();
        }

        protected  void OnInitialize()
        {
            Debug.WriteLine("Auf geht's");
        }

        protected void OnInitializePhysics()
        {
            Debug.WriteLine("OnInitializePhysics");
            // collision configuration contains default setup for memory, collision setup
            CollisionConf = new DefaultCollisionConfiguration();
            Dispatcher = new CollisionDispatcher(CollisionConf);

            Broadphase = new DbvtBroadphase();
            

            World = new DiscreteDynamicsWorld(Dispatcher, Broadphase, null, CollisionConf);
            World.Gravity = new Vector3(0, -20, 0);

            // create the ground
            BoxShape groundShape = new BoxShape(500, 1, 500);
            //groundShape.InitializePolyhedralFeatures();
            //CollisionShape groundShape = new StaticPlaneShape(new Vector3(0,1,0), 50);

            CollisionShapes.Add(groundShape);
            CollisionObject ground = LocalCreateRigidBody(0, Matrix.Identity, groundShape);
            ground.UserObject = "Ground";

            // create a few dynamic rigidbodies
            const float mass = 1.0f;

            //BoxShape colShape = new BoxShape(100);
            
            SphereShape colShape = new SphereShape(50);
            CollisionShapes.Add(colShape);
            Vector3 localInertia = colShape.CalculateLocalInertia(mass);

            const float start_x = StartPosX - ArraySizeX / 2;
            const float start_y = StartPosY;
            const float start_z = StartPosZ - ArraySizeZ / 2;

            int anzahl = 0;
            int k, i, j;
            for (k = 0; k < ArraySizeY; k++)
            {
                for (i = 0; i < ArraySizeX; i++)
                {
                    for (j = 0; j < 1; j++)
                    {
                        Matrix startTransform = Matrix.Translation(
                            2 * i + start_x,
                            2 * k + start_y,
                            2 * j + start_z
                        );

                        // using motionstate is recommended, it provides interpolation capabilities
                        // and only synchronizes 'active' objects
                        DefaultMotionState myMotionState = new DefaultMotionState(startTransform);
                        RigidBodyConstructionInfo rbInfo =
                            new RigidBodyConstructionInfo(mass, myMotionState, colShape, localInertia);
                        RigidBody body = new RigidBody(rbInfo);
                        rbInfo.Dispose();

                        // make it drop from a height
                        body.Translate(new Vector3(0, 350, 0));

                        World.AddRigidBody(body);
                        //var cs = colShape.HalfExtentsWithoutMargin;
                        anzahl++;
                    }
                }
            }

            Debug.WriteLine("Anzahl: " + anzahl);

            #region flyingBox
            /*
            Matrix fBoxTransform = Matrix.Translation(0,300,-1000);
            DefaultMotionState fbMotionState = new DefaultMotionState(fBoxTransform);
            RigidBodyConstructionInfo rbInfo1 =
                new RigidBodyConstructionInfo(0, fbMotionState, colShape, localInertia);
            box = new RigidBody(rbInfo1);
            rbInfo1.Dispose();
            // make it drop from a height
            World.AddRigidBody(box);
            //box.ApplyForce(new Vector3(0, 0, 200), new Vector3(0, 0, 500));
            */
            #endregion flyingBox
        }

        public void ClientResetScene()
        {
            RemovePickingConstraint();
            ExitPhysics();
            OnInitializePhysics();
        }

        public void ExitPhysics()
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

        public void OnUpdate()
        {
            _frameDelta = clock.Update();
            frameAccumulator += _frameDelta;
            ++frameCount;
            if (frameAccumulator >= 1.0f)
            {
                FramesPerSecond = frameCount / frameAccumulator;

                frameAccumulator = 0.0f;
                frameCount = 0;
            }

            _world.StepSimulation(_frameDelta);

           
           /* Matrix t = box.WorldTransform;
            Vector4 pos = t.get_Rows(3);
            t.set_Rows(3, new Vector4(0, 0, 0, 1));
            t += Matrix.Translation(new Vector3(0, 0, 1) * 0.001f);
            //t *= Matrix.RotationYawPitchRoll(0.1f * FrameDelta, 0.05f * FrameDelta, 0);
            t.set_Rows(3, pos);
            box.WorldTransform = t;*/

            
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

        

        public void ShootSphere(Vector3 camPos, Vector3 destination)
        {
            if (_world == null)
                return;

            const float mass = 1.0f;

            if (shootSphereShape == null)
            {
                shootSphereShape = new SphereShape(50);
                CollisionShapes.Add(shootSphereShape);
                //shootBoxShape.InitializePolyhedralFeatures();
            }

            SphereShape bulletShape = new SphereShape(50);
            //groundShape.InitializePolyhedralFeatures();
            //CollisionShape groundShape = new StaticPlaneShape(new Vector3(0,1,0), 50);

            CollisionShapes.Add(bulletShape);
            CollisionObject bullet = LocalCreateRigidBody(1, Matrix.Identity, bulletShape);
           // RigidBody body = LocalCreateRigidBody(1, Matrix.Translation(camPos), shootSphereShape);
            //body.UserObject = "Bullet";
            /*bullet.LinearFactor = new Vector3(1, 1, 1);
            //body.Restitution = 1;

            Vector3 linVel = destination - camPos;
            linVel.Normalize();

            bullet.LinearVelocity = linVel * shootSphereInitialSpeed;
            bullet.CcdMotionThreshold = 0.5f;
            bullet.CcdSweptSphereRadius = 0.9f;*/
            

        }

        public RigidBody LocalCreateRigidBody(float mass, Matrix startTransform, CollisionShape shape)
        {
            Debug.WriteLine("LocalCreateRigidBody");
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
