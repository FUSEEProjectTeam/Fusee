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

        /// <summary>
        /// 
        /// </summary>
        public Physic()
        {
            Debug.WriteLine("Physic: Constructor");
            _world = new DynamicWorld();

            //FallingTower();
            Ground();
           // InitPoint2PointConstraint();
           // InitHingeConstraint();
            //InitSliderConstraint();
            //InitGearConstraint();
            //InitDfo6Constraint();
            Tester();
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
                    _world.AddRigidBody(0, pos, new float3(1, 1, 1));
                }
            }

        }


        public void FallingTower()
        {
            Mesh mesh = MeshReader.LoadMesh(@"Assets/Sphere.obj.model");
            
            for (int k = 1; k < 4; k++)
            {
                for (int h = -2; h < 2; h++)
                {
                    for (int j = -2; j < 2; j++)
                    {
                        var pos = new float3(4 * h, 400 + (k * 4), 4 * j);
                        _world.AddRigidBody(1, pos, new float3(1, 1, 1));
                    }
                }
            }
        }

        public void InitPoint2PointConstraint()
        {
            var mesh = MeshReader.LoadMesh(@"Assets/Cube.obj.model");
            var rbA = _world.AddRigidBody(1, new float3(100, 300, 0), new float3(0, 0, 0));
            rbA.LinearFactor = new float3(0,0,0);
            rbA.AngularFactor = new float3(0, 0, 0);
           
            var rbB = _world.AddRigidBody(1, new float3(100, 200, 0), new float3(0, 0, 0));
            var p2p = _world.AddPoint2PointConstraint(rbA, rbB, new float3(-0, -70, 0), new float3(0, 10, 0));
            //var p2p = _world.AddPoint2PointConstraint(rbA, new float3(105, -200, -150));
            p2p.SetParam(PointToPointFlags.PointToPointFlagsCfm, 0.9f);

            var rbC = _world.AddRigidBody(1, new float3(150, 100, 0), new float3(0, 0, 0));
            var p2p1 = _world.AddPoint2PointConstraint(rbB, rbC, new float3(0, -70, 0), new float3(0, 10, 0));
  
        }

        public void InitHingeConstraint()
        {
            var mesh = MeshReader.LoadMesh(@"Assets/Cube.obj.model");
            var rbA = _world.AddRigidBody(1, new float3(400, 500, 0), new float3(0, 0, 0));
            rbA.LinearFactor = new float3(0, 0, 0);
            rbA.AngularFactor = new float3(0, 0, 0);

            var rbB = _world.AddRigidBody(1, new float3(200,500, 0), new float3(0, 0, 0));

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
            var rbA = _world.AddRigidBody(1, new float3(400, 500, 0), new float3(0, 0, 0));
            rbA.LinearFactor = new float3(0, 0, 0);
            rbA.AngularFactor = new float3(0, 0, 0);

            var rbB = _world.AddRigidBody(1, new float3(200, 500, 0), new float3(0, 0, 0));

            var frameInA = float4x4.Identity;
            frameInA.Row3 = new float4(0,1,0,1);
            var frameInB = float4x4.Identity;
            frameInA.Row3 = new float4(0, 0, 0, 1);
            var sc = _world.AddSliderConstraint(rbA, rbB, frameInA, frameInB, true);

        }

        public void InitGearConstraint()
        {
            var mesh = MeshReader.LoadMesh(@"Assets/Cube.obj.model");
            var rbA = _world.AddRigidBody(0, new float3(0, 150, 0), new float3(1, 1, 1));
            //rbA.LinearFactor = new float3(0, 0, 0);
            //rbA.AngularFactor = new float3(0, 0, 0);

            var rbB = _world.AddRigidBody(1, new float3(0, 300, 0), new float3(1, 1, 1));
            //rbB.LinearFactor = new float3(0,0,0);
            ////var axisInB = new float3(0, 1, 0);
            // var gc = _world.AddGearConstraint(rbA, rbB, axisInA, axisInB);
        }

        public void InitDfo6Constraint()
        {
            var mesh = MeshReader.LoadMesh(@"Assets/Cube.obj.model");
            var rbA = _world.AddRigidBody(0, new float3(0, 150, 0), new float3(1, 1, 1));
            //rbA.LinearFactor = new float3(0, 0, 0);
            //rbA.AngularFactor = new float3(0, 0, 0);

            var rbB = _world.AddRigidBody(1, new float3(0, 300, 0), new float3(1, 1, 1));
            _world.AddGeneric6DofConstraint(rbA, rbB, rbA.WorldTransform, rbB.WorldTransform, false);
        }

        public void Tester()
        {
            var rbA = _world.AddRigidBody(1, new float3(0, 150, 0), new float3(1, 1, 1));
            //var box = rbA.AddBoxShape(new float3(14,14,14));
            //var shape = rbA.AddCapsuleShape(2, 8);
            //Debug.WriteLine(shape.Radius);
            var box = _world.AddBoxShape(5);
            Debug.WriteLine(box.HalfExtents);
            var comp = _world.AddCompoundShape(true);
            comp.AddChildShape(float4x4.Identity,box);

        }
    }
}
