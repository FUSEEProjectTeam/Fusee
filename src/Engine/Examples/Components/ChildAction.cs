
using System.Collections.Generic;
using System.Text;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.Components
{
    class ChildAction : ActionCode
    {
        private float x=0.5f;


        public override void Start()
        {
            transform.LocalPosition = new float3(0, 200, 200);
            //transform.LocalScale = new float3(2,2,2);
            renderer.color= new float4(0,1,0,1);
        }

        public override void Update()
        {
            //transform.LocalScale = new float3(1.1f, 1.1f, 1.1f);
            //transform.Matrix = float4x4.CreateRotationY(x);
            transform.LocalEulerAngles = new float3(x, 0, 0);
            //transform.LocalPosition = new float3(-x*10, x*10, x*10);
            
            x += 0.1f;
            //Console.WriteLine("Local Matrix of Child is: "+transform.Matrix.ToString());

        }
    }
}
