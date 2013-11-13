using System;
using System.Collections.Generic;
using System.Diagnostics;
using BulletSharp;
using BulletSharp.MultiThreaded;
using Fusee.Engine;
using Fusee.Math;
using RigidBody = BulletSharp.RigidBody;

namespace Examples.BulletSharp
{
    class MyDemo
    {
        //Vector3 eye = new Vector3(30, 20, 10);
        //Vector3 target = new Vector3(0, 5, -4);
        private TriangleIndexVertexArray indexVerexArray;
        private CollisionShape customShape;
        private List<float3> meshVertices;
        private List<float2> meshTriangels;
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

        private Mesh _meshTea;
        protected void OnInitialize()
        {
            Debug.WriteLine("OnInitialize()");
        }

        protected void OnInitializePhysics()
        {

            _meshTea = MeshReader.LoadMesh(@"Assets/Teapot.obj.model");
            // collision configuration contains default setup for memory, collision setup
            CollisionConf = new DefaultCollisionConfiguration();
            Dispatcher = new CollisionDispatcher(CollisionConf);

            Broadphase = new DbvtBroadphase();
 
            World = new DiscreteDynamicsWorld(Dispatcher, Broadphase, null, CollisionConf);
            World.Gravity = new Vector3(0, -9.81f, 0);
            

            // create plane
            
            // create the ground
            BoxShape groundShape1 = new BoxShape(9.9f, 1, 9.9f);
            //groundShape.InitializePolyhedralFeatures();
            //CollisionShape groundShape = new StaticPlaneShape(new Vector3(0,1,0), 50);
            BoxShape groundShape2 = new BoxShape(9.9f, 1, 9.9f);
            //groundShape.InitializePolyhedralFeatures();
            //CollisionShape groundShape = new StaticPlaneShape(new Vector3(0,1,0), 50);
            BoxShape groundShape3 = new BoxShape(9.9f, 1, 9.9f);
            //groundShape.InitializePolyhedralFeatures();
            //CollisionShape groundShape = new StaticPlaneShape(new Vector3(0,1,0), 50);
            BoxShape groundShape4 = new BoxShape(9.9f, 1, 9.9f);
            //groundShape.InitializePolyhedralFeatures();
            //CollisionShape groundShape = new StaticPlaneShape(new Vector3(0,1,0), 50);

            CollisionShapes.Add(groundShape1);
            CollisionShapes.Add(groundShape2);
            CollisionShapes.Add(groundShape3);
            CollisionShapes.Add(groundShape4);
            Matrix mGround = Matrix.Translation(-4.9f, 0, 4.9f);
            CollisionObject ground1 = LocalCreateRigidBody(0, mGround, groundShape1);
            mGround = Matrix.Translation(-4.9f, 0, -4.9f);
            CollisionObject ground2 = LocalCreateRigidBody(0, mGround, groundShape2);
            mGround = Matrix.Translation(9.9f, 0, 4.9f);
            CollisionObject ground3 = LocalCreateRigidBody(0, mGround, groundShape3);
            mGround = Matrix.Translation(4.9f, 0, -4.9f);
            CollisionObject ground4 = LocalCreateRigidBody(0, mGround, groundShape4);
           // ground.UserObject = "Ground";

            // create a few dynamic rigidbodies
            const float mass = 1.0f;

            //SphereShape colShape = new SphereShape(35);
            BoxShape colShape = new BoxShape(2);
            CollisionShapes.Add(colShape);
            Vector3 localInertia = colShape.CalculateLocalInertia(mass);
            
            const float start_x = StartPosX - ArraySizeX / 2;
            const float start_y = StartPosY;
            const float start_z = StartPosZ - ArraySizeZ / 2;

            int k, i, j;
            for (k = 0; k < 2; k++)
            {
                for (i = 0; i < 2; i++)
                {
                    for (j = 0; j < 2; j++)
                    {
                        Matrix startTransform = Matrix.Translation(
                            4 * i + start_x,
                            4 * k + start_y,
                            4 * j + start_z
                        );

                        // using motionstate is recommended, it provides interpolation capabilities
                        // and only synchronizes 'active' objects
                        DefaultMotionState myMotionState = new DefaultMotionState(startTransform);
                        RigidBodyConstructionInfo rbInfo =
                            new RigidBodyConstructionInfo(1, myMotionState, colShape, localInertia);
                        RigidBody body = new RigidBody(rbInfo);
                        rbInfo.Dispose();

                        // make it drop from a height
                        body.Translate(new Vector3(0, 200, 0));
                        
                        World.AddRigidBody(body);
                    }
                }
            }

            var model = (RigidBody)World.CollisionObjectArray[2];
            
            model.ApplyForce(new Vector3(10,0,0), new Vector3(0,0,0) );
            //model.AngularVelocity = new Vector3(10,10,10);

            #region myConstraint Test

            TypedConstraint tc = new TypedConstraint(0, true);
            GearConstraint gc = new GearConstraint();
            Point2PointConstraint p2p = new Point2PointConstraint();

            #endregion myConstraint Test

            #region CustomMesh
            /* int vLength = _meshTea.Vertices.Length;
            float3[] ver = new float3[vLength];
            ver = _meshTea.Vertices;
            Vector3[] btVer = new Vector3[vLength];
            for (int v = 0; v < vLength; v++)
            {
                btVer[v].X = ver[v].x;
                btVer[v].Y = ver[v].y;
                btVer[v].Z = ver[v].z;
            }

            int iLength = _meshTea.Triangles.Length;
            int[] ind = new int[iLength];
            //ind = Convert.ToInt32[](_meshTea.Triangles);
            for (int c = 0; c < iLength; c++)
            {
                ind[c] = Convert.ToInt32(_meshTea.Triangles[c]);
            }
            
            indexVerexArray = new TriangleIndexVertexArray(ind, btVer);

            GImpactMeshShape trimesh = new GImpactMeshShape(indexVerexArray);
            trimesh.LocalScaling = new Vector3(1);
            trimesh.Margin = 0;
            trimesh.UpdateBound();
            customShape = trimesh;
            customShape.LocalScaling = new Vector3(0.9f,0.9f,0.9f);
            Matrix trans = Matrix.Translation(new Vector3(0, 0, 0));
            DefaultMotionState ms = new DefaultMotionState(trans);
            RigidBodyConstructionInfo info =
                new RigidBodyConstructionInfo(mass, ms, customShape, localInertia);
            RigidBody myBody = new RigidBody(info);
            info.Dispose();

            // make it drop from a height
            myBody.Translate(new Vector3(0, 50, 0));

            World.AddRigidBody(myBody);*/
            #endregion CustomMesh

        }

        public MyDemo()
        {
            CollisionShapes = new AlignedCollisionShapeArray();
            Run();
        }

        public void Run()
        {
            clock.Start();
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

        public virtual void OnUpdate()
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
                shootBoxShape = new BoxShape(2);
                //shootBoxShape.InitializePolyhedralFeatures();
            }

            RigidBody body = LocalCreateRigidBody(mass, Matrix.Translation(camPos), shootBoxShape);
            body.LinearFactor = new Vector3(1, 1, 1);
            //body.Restitution = 1;

            Vector3 linVel = destination - camPos;
            linVel.Normalize();

            body.LinearVelocity = linVel * shootBoxInitialSpeed;
            body.CcdMotionThreshold = 0.5f;
            body.CcdSweptSphereRadius = 0.9f;
            body.ActivationState= ActivationState.ActiveTag;
             
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
