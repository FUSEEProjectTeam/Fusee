using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.BulletTest
{
    class Physic
    {
        private DynamicWorld _world;
        
        public DynamicWorld World
        {
            get { return _world; }
            set { _world = value; }
        }

        internal BoxShape MyBoxCollider;
        internal SphereShape MySphereCollider;
        internal ConvexHullShape MyConvHull;
        internal ConvexHullShape Hull;
        internal Mesh BoxMesh;

        public Physic()
        {
            Debug.WriteLine("Physic: Constructor");
            _world = new DynamicWorld();

            MyBoxCollider = _world.AddBoxShape(25);
            MySphereCollider = _world.AddSphereShape(40);
            BoxMesh = MeshReader.LoadMesh(@"Assets/Cube.obj.model");
            var vertices = BoxMesh.Vertices;
            MyConvHull = _world.AddConvexHullShape(vertices);
            float3[] verts =
            {
                new float3(0, -25, -25), new float3(25, -25, -25), new float3(-25, -25, 25), new float3(25, -25, 25),
                new float3(0, 25, 0), new float3(25, 25, -25), new float3(-25, 25, 25), new float3(25, 25, 25)
            };
            Hull = _world.AddConvexHullShape(verts);
           


            //Tets Scenes
            //GroundPlane();
            GroundPlane();
            FallingTower();
            //Ground();
            
            //InitPoint2PointConstraint();
            //InitHingeConstraint();
            //InitSliderConstraint();
            //InitGearConstraint();
            //InitDfo6Constraint();
            //Tester();

            //Wippe();
        }

        public void GroundPlane()
        {
            var plane = _world.AddStaticPlaneShape(float3.UnitY, 10);
            var groundPlane = _world.AddRigidBody(0, new float3(0,0,0) , plane); 
            //var groundShape = _world.AddBoxShape(400, 1, 400);
            //_world.AddRigidBody(0, new float3(0, 0, 0), groundShape, new float3(1, 1, 1));
        }

        public void Ground()
        {
            //create ground
            Mesh mesh = MeshReader.LoadMesh(@"Assets/Cube.obj.model");
            int size = 52;
            for (int b = -2; b < 2; b++)
            {
                for (int c = -2; c < 2; c++)
                {
                    var pos = new float3(b * size, 0, c * size);
                    _world.AddRigidBody(0, pos, MyBoxCollider);
                }
            }

        }

        public void Wippe()
        {
            var groundShape = _world.AddBoxShape(150, 25, 150);
            var ground = _world.AddRigidBody(0, new float3(0, 0, 0), groundShape);

            var boxShape = _world.AddBoxShape(25);
            var sphere = _world.AddSphereShape(50);
            var brettShape = _world.AddBoxShape(50.0f, 0.1f, 1.0f);
            // var comp = _world.AddCompoundShape(true);
            // var brett = _world.AddRigidBody(1, new float3(0, 55, 0), brettShape, new float3(0, 0, 0));
            var box1 = _world.AddRigidBody(1, new float3(-40, 52, 0), boxShape);
            var box2 = _world.AddRigidBody(1, new float3(80, 102, 0), sphere);
            var box3 = _world.AddRigidBody(1, new float3(-60, 200, 0), boxShape);
            // var cmpRB = _world.AddRigidBody(1, new float3(0, 80, 20), comp, new float3(0, 0, 0));
            box3.CollisionShape = _world.AddBoxShape(8);
            box1.CollisionShape = _world.AddSphereShape(30);
            var shape = (SphereShape)box1.CollisionShape;
            Debug.WriteLine(shape.Radius);
            shape.Radius = 5;
            Debug.WriteLine(shape.Radius);
        } 

        public void FallingTower()
        {
            for (int k = 0; k < 4; k++)
            {
                for (int h = -2; h < 2; h++)
                {
                    for (int j = -2; j < 2; j++)
                    {
                        var pos = new float3(4 * h, 300 + (k * 4), 4 * j);
  
                        var cube = _world.AddRigidBody(1, pos, Hull);
                    }
                }
            }
        }

        public void InitPoint2PointConstraint()
        {
            
            var rbA = _world.AddRigidBody(1, new float3(100, 300, 0), MyBoxCollider);
            rbA.LinearFactor = new float3(0,0,0);
            rbA.AngularFactor = new float3(0, 0, 0);
           
            var rbB = _world.AddRigidBody(1, new float3(100, 200, 0),MyBoxCollider);
            var p2p = _world.AddPoint2PointConstraint(rbA, rbB, new float3(-0, -70, 0), new float3(0, 10, 0));
            //var p2p = _world.AddPoint2PointConstraint(rbA, new float3(105, -200, -150));
            p2p.SetParam(PointToPointFlags.PointToPointFlagsCfm, 0.9f);

            var rbC = _world.AddRigidBody(1, new float3(150, 100, 0),MyBoxCollider);
            var p2p1 = _world.AddPoint2PointConstraint(rbB, rbC, new float3(0, -70, 0), new float3(0, 10, 0));
  
        }

        public void InitHingeConstraint()
        {
            var mesh = MeshReader.LoadMesh(@"Assets/Cube.obj.model");
            var rbA = _world.AddRigidBody(1, new float3(400, 500, 0), MyBoxCollider);
            rbA.LinearFactor = new float3(0, 0, 0);
            rbA.AngularFactor = new float3(0, 0, 0);

            var rbB = _world.AddRigidBody(1, new float3(200,500, 0), MyBoxCollider);

            var frameInA = float4x4.Identity;
            frameInA.Row3 = new float4(0,50,0,1);
            var frameInB = float4x4.Identity;
            frameInA.Row3 = new float4(0, -300, 1, 1);
            
            var hc = _world.AddHingeConstraint(rbA, rbB, new float3(100, -300, 0), new float3(0, 100, 0), new float3(0, 0, 1), new float3(0, 0, 1), false);

            hc.SetLimit(-(float)Math.PI * 0.25f, (float)Math.PI * 0.25f);
        }

        public void InitSliderConstraint()
        {
            var mesh = MeshReader.LoadMesh(@"Assets/Cube.obj.model");
            var rbA = _world.AddRigidBody(1, new float3(400, 500, 0), MyBoxCollider);
            rbA.LinearFactor = new float3(0, 0, 0);
            rbA.AngularFactor = new float3(0, 0, 0);

            var rbB = _world.AddRigidBody(1, new float3(200, 500, 0), MyBoxCollider);

            var frameInA = float4x4.Identity;
            frameInA.Row3 = new float4(0,1,0,1);
            var frameInB = float4x4.Identity;
            frameInA.Row3 = new float4(0, 0, 0, 1);
            var sc = _world.AddSliderConstraint(rbA, rbB, frameInA, frameInB, true);

        }

        public void InitGearConstraint()
        {
            var mesh = MeshReader.LoadMesh(@"Assets/Cube.obj.model");
            var rbA = _world.AddRigidBody(0, new float3(0, 150, 0), MyBoxCollider);
            //rbA.LinearFactor = new float3(0, 0, 0);
            //rbA.AngularFactor = new float3(0, 0, 0);

            var rbB = _world.AddRigidBody(1, new float3(0, 300, 0), MyBoxCollider);
            //rbB.LinearFactor = new float3(0,0,0);
            ////var axisInB = new float3(0, 1, 0);
            // var gc = _world.AddGearConstraint(rbA, rbB, axisInA, axisInB);
        }

        public void InitDfo6Constraint()
        {
            var mesh = MeshReader.LoadMesh(@"Assets/Cube.obj.model");
            var rbA = _world.AddRigidBody(0, new float3(0, 150, 0), MyBoxCollider);
            //rbA.LinearFactor = new float3(0, 0, 0);
            //rbA.AngularFactor = new float3(0, 0, 0);

            var rbB = _world.AddRigidBody(1, new float3(0, 300, 0), MyBoxCollider);
            _world.AddGeneric6DofConstraint(rbA, rbB, rbA.WorldTransform, rbB.WorldTransform, false);
        }

        public void Tester()
        {
            var box = _world.AddBoxShape(25);
            box.Margin = 0.5f;
            //box.LocalScaling = new float3(0.5f, 0.5f, 0.5f);
            //var shape = rbA.AddCapsuleShape(2, 8);
            //Debug.WriteLine(shape.Radius);
            //var rbA = _world.AddRigidBody(0, new float3(0, 150, 0), sphere, new float3(1, 1, 1));

            var gimp = _world.AddGImpactMeshShape(BoxMesh);
            var rbA = _world.AddRigidBody(1, new float3(0, 150, 0), gimp);
            



        }
    }
}
