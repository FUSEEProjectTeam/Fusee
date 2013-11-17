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
    }
}
