using System;
using System.Collections.Generic;
using System.Diagnostics;
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


        public Physic()
        {
            Debug.WriteLine("Physic: Constructor");
            _world = new DynamicWorld();

            //FallingTower();
            Ground();
            InitPoint2PointConstraint();

        }


        public void Ground()
        {
            //create ground
            Mesh mesh = MeshReader.LoadMesh(@"Assets/Cube.obj.model");
            int size = 200;
            for (int b = -1; b < 2; b++)
            {
                for (int c = -1; c < 2; c++)
                {
                    var pos = new float3(b * size, 0, c * size);
                    _world.AddRigidBody(0, pos, mesh, new float3(1, 1, 1));
                }
            }

            //And Teapot
            Mesh meshTea = MeshReader.LoadMesh(@"Assets/Teapot.obj.model");
            _world.AddRigidBody(1, new float3(0, 200, 0), meshTea, new float3(1, 1, 1));
           
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
                        _world.AddRigidBody(1, pos, mesh, new float3(1, 1, 1));
                    }
                }
            }
        }

        public void InitPoint2PointConstraint()
        {
            var mesh = MeshReader.LoadMesh(@"Assets/Cube.obj.model");
            var rbA = _world.AddRigidBody(1, new float3(400, 500, 0), mesh, new float3(0, 0, 0));
            rbA.LinearFactor = new float3(0,0,0);
            rbA.AngularFactor = new float3(0, 0, 0);
           
            var rbB = _world.AddRigidBody(1, new float3(300, 500, 0), mesh, new float3(0, 0, 0));
            var p2p = _world.AddPoint2PointConstraint(rbA, rbB, new float3(105, -150, -150), new float3(0, 100, 0));
            p2p.SetParam(ConstraintParameter.CONSTRAINT_PARAM_CFM, 0.9f);
            Debug.WriteLine("p2p.GetParam(ConstraintParameter.CONSTRAINT_PARAM_CFM): " + p2p.GetParam(ConstraintParameter.CONSTRAINT_PARAM_CFM));

        }
    }
}
