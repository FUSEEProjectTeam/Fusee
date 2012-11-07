using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.Math;

namespace SceneManagement
{
    public class TestBehaviour : Behaviour
    {
        private float x;
        public TestBehaviour(TraversalState _traversalState) : base()
        {
            gameEntity = _traversalState.Owner;
            transform = gameEntity.transform;
            renderer = gameEntity.renderer;
            Start();
        }
        public override void Start()
        {
            transform.LocalPosition = new float3(100,20,30);
        }

        public override void Update()
        {
            //transform.Matrix = float4x4.CreateRotationY(x);
            transform.LocalPosition+=new float3(-x,x,x);
            x += 0.01F;
            
            //Console.WriteLine(this.ToString()+" is running.");
            //transform.LocalPosition+= new float3(0.01f,0,0);
            //Console.WriteLine(transform.WorldMatrix.ToString());
        }
    }
}
