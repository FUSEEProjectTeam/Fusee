using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using Fusee.Engine;
using Fusee.Math;
using Microsoft.Win32;

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
        internal Mesh BoxMesh, TeaPotMesh;

        internal RigidBody _PRigidBody;
        public RigidBody PRbBody
        {
            get { return _PRigidBody; }
            set { _PRigidBody = value; }
        }
            

        public Physic()
        {
            Debug.WriteLine("Physic: Constructor");
            InitScene1();
        }


        public void InitWorld()
        {
            _world = new DynamicWorld();
            MyBoxCollider = _world.AddBoxShape(2);
            MySphereCollider = _world.AddSphereShape(2);
            BoxMesh = MeshReader.LoadMesh(@"Assets/Cube.obj.model");
            TeaPotMesh = MeshReader.LoadMesh(@"Assets/Teapot.obj.model");
            var vertices = BoxMesh.Vertices;
            /*
             MyConvHull = _world.AddConvexHullShape(vertices);
            float3[] verts =
            {
                new float3(0, -25, -25), new float3(25, -25, -25), new float3(-25, -25, 25), new float3(25, -25, 25),
                new float3(0, 25, 0), new float3(25, 25, -25), new float3(-25, 25, 25), new float3(25, 25, 25)
            };
            Hull = _world.AddConvexHullShape(verts);*/
        }

        public void InitScene1()
        {
            InitWorld();
            GroundPlane(float3.Zero);
            FallingTower1();
        }
        public void InitScene2()
        {
            InitWorld();
            InitPoint2PointConstraint();
            InitHingeConstraint();
        }
        public void InitScene3()
        {
            InitWorld();
            GroundPlane(new float3(0,0,(float)Math.PI/6));
            FallingTower2();
            //InitGImpacShape();
        }

        public void GroundPlane(float3 rot)
        {
            //var plane = _world.AddStaticPlaneShape(float3.UnitY, 1);
            //var groundPlane = _world.AddRigidBody(0, new float3(0,0,0), float3.Zero, plane);
            //groundPlane.Bounciness = 1;
            var groundShape = _world.AddBoxShape(40, 0.1f, 40);
            var ground = _world.AddRigidBody(0, new float3(0, 0, 0), rot, groundShape);
           
            ground.Bounciness = 1f;
            ground.Friction = 1;
        }

        public void Wippe()
        {
            //var groundShape = _world.AddBoxShape(150, 25, 150);
            //var ground = _world.AddRigidBody(0, new float3(0, 0, 0), Quaternion.Identity, groundShape);

            var boxShape = _world.AddBoxShape(30, 1, 10);

            //var brettShape = _world.AddBoxShape(50.0f, 0.1f, 10.0f);
            // var comp = _world.AddCompoundShape(true);
            // var brett = _world.AddRigidBody(1, new float3(0, 55, 0), brettShape, new float3(0, 0, 0));
            Quaternion rotA = new Quaternion(new float3(1, 0, 0), 30);
            Quaternion rotB = new Quaternion(new float3(1, 0, 0), -30);
            var box1 = _world.AddRigidBody(0, new float3(20, 100, 0), float3.Zero, boxShape);
            var box2 = _world.AddRigidBody(1, new float3(0, 250, 0), float3.Zero, MySphereCollider);
            var box3 = _world.AddRigidBody(0, new float3(-20, 50, 0), float3.Zero, boxShape);

        } 

        public void FallingTower1()
        {
            for (int k = 0; k < 4; k++)
            {
                for (int h = -2; h < 3; h++)
                {
                    for (int j = -2; j < 3; j++)
                    {
                        var pos = new float3(4 * h, 50 + (k * 4), 4 * j);

                        var cube = _world.AddRigidBody(1, pos, float3.Zero, MyBoxCollider);
                        //cube.Bounciness = 1f;
                        cube.Friction = 1f;
                    }
                }
            }
        }

        public void FallingTower2()
        {
            for (int k = 0; k < 4; k++)
            {
                for (int h = -2; h < 3; h++)
                {
                    for (int j = -2; j < 3; j++)
                    {
                        var pos = new float3(4 * h, 300 + (k * 4), 4 * j);

                        var sphere = _world.AddRigidBody(1, pos, float3.Zero, MySphereCollider);
                        sphere.Friction = 0.5f;
                        sphere.Bounciness = 0.8f;
                    }
                }
            }
        }

        public void InitPoint2PointConstraint()
        {
            
            var rbA = _world.AddRigidBody(1, new float3(-10, 15, 0), float3.Zero, MyBoxCollider);
            rbA.LinearFactor = new float3(0,0,0);
            rbA.AngularFactor = new float3(0, 0, 0);

            var rbB = _world.AddRigidBody(1, new float3(-11, 10, 0), float3.Zero, MyBoxCollider);
            var p2p = _world.AddPoint2PointConstraint(rbA, rbB, new float3(0, -3f, 0), new float3(0, 2.5f, 0));
            p2p.SetParam(PointToPointFlags.PointToPointFlagsCfm, 0.9f);

            var rbC = _world.AddRigidBody(1, new float3(-11, 5, 2), float3.Zero, MyBoxCollider);
            var p2p1 = _world.AddPoint2PointConstraint(rbB, rbC, new float3(0, -2.5f, 0), new float3(0, 2.5f, 0));
  
        }

        public void InitHingeConstraint()
        {
            var rot = new float3(0, (float) Math.PI/4, 0);
            //var mesh = MeshReader.LoadMesh(@"Assets/Cube.obj.model");
            var rbA = _world.AddRigidBody(1, new float3(10, 10, 0), float3.Zero, MyBoxCollider);
            rbA.LinearFactor = new float3(0, 0, 0);
            rbA.AngularFactor = new float3(0, 0, 0);

            var rbB = _world.AddRigidBody(1, new float3(5, 5, 0), float3.Zero, MyBoxCollider);
            
            var hc = _world.AddHingeConstraint(rbA, rbB, new float3(0, -5, 0), new float3(0, 2, 0), new float3(0, 0, 1), new float3(0, 0, 1), false);

            hc.SetLimit(-(float)Math.PI * 0.25f, (float)Math.PI * 0.25f);
        }

        public void InitSliderConstraint()
        {
            var mesh = MeshReader.LoadMesh(@"Assets/Cube.obj.model");
            var rbA = _world.AddRigidBody(1, new float3(400, 500, 0), float3.Zero, MyBoxCollider);
            rbA.LinearFactor = new float3(0, 0, 0);
            rbA.AngularFactor = new float3(0, 0, 0);

            var rbB = _world.AddRigidBody(1, new float3(200, 500, 0), float3.Zero, MyBoxCollider);

            var frameInA = float4x4.Identity;
            frameInA.Row3 = new float4(0,1,0,1);
            var frameInB = float4x4.Identity;
            frameInA.Row3 = new float4(0, 0, 0, 1);
            var sc = _world.AddSliderConstraint(rbA, rbB, frameInA, frameInB, true);

        }

        public void InitGearConstraint()
        {
            var mesh = MeshReader.LoadMesh(@"Assets/Cube.obj.model");
            var rbA = _world.AddRigidBody(0, new float3(0, 150, 0), float3.Zero, MyBoxCollider);
            //rbA.LinearFactor = new float3(0, 0, 0);
            //rbA.AngularFactor = new float3(0, 0, 0);

            var rbB = _world.AddRigidBody(1, new float3(0, 300, 0), float3.Zero, MyBoxCollider);
            //rbB.LinearFactor = new float3(0,0,0);
            ////var axisInB = new float3(0, 1, 0);
            // var gc = _world.AddGearConstraint(rbA, rbB, axisInA, axisInB);
        }

        public void InitDfo6Constraint()
        {
            var mesh = MeshReader.LoadMesh(@"Assets/Cube.obj.model");
            var rbA = _world.AddRigidBody(0, new float3(0, 150, 0), float3.Zero, MyBoxCollider);
            //rbA.LinearFactor = new float3(0, 0, 0);
            //rbA.AngularFactor = new float3(0, 0, 0);

            var rbB = _world.AddRigidBody(1, new float3(0, 300, 0), float3.Zero, MyBoxCollider);
            _world.AddGeneric6DofConstraint(rbA, rbB, rbA.WorldTransform, rbB.WorldTransform, false);
        }

        public void CompoundShape()
        {
            var compShape = _world.AddCompoundShape(true);
            var box = _world.AddBoxShape(25);
            var sphere = _world.AddBoxShape(25);
            var matrixBox = float4x4.Identity;
            var matrixSphere = new float4x4(1, 0, 0, 2, 0, 1, 0, 2, 0, 0, 1, 2, 0, 0, 0, 1);
            compShape.AddChildShape(matrixBox, box);
            compShape.AddChildShape(matrixSphere, sphere);
            var rb = _world.AddRigidBody(1, new float3(0, 150, 0), float3.Zero, compShape);
        }

        public void InitGImpacShape()
        {
            var gimp = _world.AddGImpactMeshShape(TeaPotMesh);
            var rbB = _world.AddRigidBody(1, new float3(0, 10, 0), float3.Zero, gimp);
        }

        public void Tester()
        {
            //var box = _world.AddBoxShape(25);
            //box.Margin = 0.5f;
            //box.LocalScaling = new float3(0.5f, 0.5f, 0.5f);
            //var shape = rbA.AddCapsuleShape(2, 8);
            //Debug.WriteLine(shape.Radius);
            //var rbA = _world.AddRigidBody(0, new float3(0, 150, 0), sphere, new float3(1, 1, 1));
            Quaternion rotA = new Quaternion(0.739f, -0.204f, 0.587f, 0.257f);
            var btPositions = new float3[3];
            btPositions[0] = new float3(0, 0, 0);
            btPositions[1] = new float3(100, 0, 0);
            btPositions[2] = new float3(0, 0, 100);
            
            
            var btRadi = new float[3];
            btRadi[0] = 50;
            btRadi[1] = 50;
            btRadi[2] = 50;
            var col = _world.AddCapsuleShape(20, 580);
            var gimp = _world.AddGImpactMeshShape(TeaPotMesh);
            //var rbA = _world.AddRigidBody(1, new float3(0, 0, 0), rotA, MyBoxCollider);
            var rbB = _world.AddRigidBody(1, new float3(0, 200, 0), float3.Zero, col);
            //var rbC = _world.AddRigidBody(1, new float3(0, 200, 70), rotA, MyConvHull);
            //var rbD = _world.AddRigidBody(1, new float3(0, 300, -50), rotA, MyBoxCollider);
           // var rbE = _world.AddRigidBody(1, new float3(30, 300, 0), rotA, MyBoxCollider);
        }

        public void MurmelBahn()
        {
            var box = _world.AddBoxShape(70, 25, 25);
            var rot = new float3(0.0f, 0.0f, (float)Math.PI / 4);

            _world.AddRigidBody(0, new float3(50, 150, 0), rot, box);
            _world.AddRigidBody(0, new float3(-50, 50, 0), float3.Zero, MyBoxCollider);
            var sphere = _world.AddRigidBody(1, new float3(0, 300, 0), float3.Zero, MyBoxCollider);
            //sphere.Bounciness = 1;
        }

        public void Shoot(float3 camPos, float3 target)
        {
            var ball = _world.AddRigidBody(1, camPos, float3.Zero, MySphereCollider);
            var impulse = target - camPos;
            impulse.Normalize();
            ball.ApplyCentralImpulse = impulse * 100;
        }


    }
}
