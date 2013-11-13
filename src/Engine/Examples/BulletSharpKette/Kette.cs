using System;
using Fusee.Engine;
using Fusee.Math;
using BulletSharp;


namespace Examples.BulletSharpKette
{
    class Kette
    {
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
        protected float shootBoxInitialSpeed = 40;


        void SetupEmptyDynamicsWorld()
        {
       
            TriangleIndexVertexArray t = new TriangleIndexVertexArray(Torus.Indices, Torus.Vertices);
            GImpactMeshShape MyMesh = new GImpactMeshShape(t);
            CollisionConf = new DefaultCollisionConfiguration();
            Dispatcher = new CollisionDispatcher(CollisionConf);
            Broadphase = new DbvtBroadphase();
            Solver = new SequentialImpulseConstraintSolver();
            World = new DiscreteDynamicsWorld(Dispatcher, Broadphase, Solver, CollisionConf);
            World.Gravity = new Vector3(0, -10, 0);
        }
    }
}
