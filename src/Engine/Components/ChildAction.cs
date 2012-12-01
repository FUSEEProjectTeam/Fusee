using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace SceneManagement
{
    class ChildAction : Action
    {
        private float x=0.5f;


        public override void Start()
        {
            transform.LocalPosition = new float3(0, 200, 200);
            transform.LocalScale = new float3(2,2,2);
        }

        public override void Update()
        {
            //transform.LocalScale = new float3(1.1f, 1.1f, 1.1f);
            //transform.Matrix = float4x4.CreateRotationY(x);
            transform.LocalEulerAngles = new float3(0, x, 0);
            //transform.LocalPosition = new float3(-x*10, x*10, x*10);
            
            x += 0.01f;
            //Console.WriteLine("Local Matrix of Child is: "+transform.Matrix.ToString());

        }
    }
}
