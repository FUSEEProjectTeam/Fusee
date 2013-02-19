using System;
using System.Diagnostics;
using Fusee.Math;
using Fusee.SceneManagement;
namespace Examples.Components
{
    public class TestBehaviour : ActionCode
    {
        //private float x;

        
        public override void Start()
        {
            transform.LocalEulerAngles = new float3(0, 90, 0);
            transform.LocalPosition = new float3(0,0,0);
            renderer.material.SwitchTexture();
            renderer.color = new float4(1,1,1,1);
            Renderer[] test = SceneEntity.GetComponents<Renderer>();
            
            Debug.WriteLine("the first members color is "+test[0].color);
        }

        public override void Update()
        {
            //transform.LocalQuaternion = Quaternion.FromAxisAngle(new float3(0, 0, 1), x);
            //Quaternion q = Quaternion.MatrixToQuaternion(transform.Matrix);
            //transform.LocalEulerAngles = new float3(-180,x,0);
            //x += 0.1F*(float)DeltaTime;
        }

        public void Test(float x)
        {
            transform.LocalEulerAngles = new float3((float)Math.Sin(x), (float)Math.Sin(x), (float)Math.Sin(x));
            //Debug.WriteLine("Hello this is testbehaviour");
        }

    }
}
