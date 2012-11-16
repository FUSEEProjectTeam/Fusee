using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace SceneManagement
{
    class ChildAction : Action
    {
        private float x;


        public override void Start()
        {
            transform.LocalPosition = new float3(1, 2, 3);
        }

        public override void Update()
        {
            //transform.Matrix = float4x4.CreateRotationY(x);
            //transform.LocalEulerAngles = new float3(0.001f, 0.001f, 0.001f);
            transform.LocalPosition -= new float3(-x, x, x);
            x += 0.01F;
            //Console.WriteLine("Internal: "+transform.Matrix.ToString());
            //Console.WriteLine("World: "+transform.WorldMatrix.ToString());
            //Console.WriteLine(this.ToString()+" is running.");
            //transform.LocalPosition+= new float3(0.01f,0,0);
            //Console.WriteLine(transform.WorldMatrix.ToString());
        }
    }
}
