using System;
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

            FallingTower();
            InitPoint2PointConstraint();

        }

        

        public void FallingTower()
        {
            for (int k = -2; k < 2; k++)
            {
                for (int h = -2; h < 2; h++)
                {
                    for (int j = -2; j < 2; j++)
                    {
                        var pos = new float3(4 * h, 100 + (k * 4), 4 * j);
                        _world.AddRigidBody(1, pos, new float3(1, 1, 1));
                    }
                }
            }
        }

        public void InitPoint2PointConstraint()
        {
            var rbA = _world.AddRigidBody(1, new float3(0, 20, 0), new float3(0, 0, 0));
            rbA.LinearFactor = new float3(0,0,0);
            rbA.AngularFactor = new float3(0, 0, 0);
            var rbB = _world.AddRigidBody(1, new float3(0, 20, 0), new float3(0, 0, 0));
            var p2p = _world.AddPoint2PointConstraint(rbA, rbB, new float3(1, -1, 1), new float3(0, 10, 0));
            p2p.SetParam(0.9f, 1);
            Debug.WriteLine(p2p.GetParam(4));
        }
    }
}
