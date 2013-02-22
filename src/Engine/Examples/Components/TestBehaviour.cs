using System;
using System.Diagnostics;
using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;
namespace Examples.Components
{
    public class TestBehaviour : ActionCode
    {
        //private float x;
        private float speed;
        
        public override void Start()
        {
            //transform.LocalEulerAngles = new float3(0, 90, 0);
            //transform.LocalPosition = new float3(0,0,0);
            //renderer.material.SwitchTexture();
            renderer.color = new float4(1,1,1,1);
            Renderer[] test = SceneEntity.GetComponents<Renderer>();
            
            Debug.WriteLine("the first members color is "+test[0].color);
            speed = 0;
            Time.Instance.TimeFlow  = speed;
        }

        public override void Update()
        {
            Debug.WriteLine("parent position: "+transform.LocalPosition);
            transform.LocalEulerAngles+=new float3(0,0.1f,0);
        }

        public void Test(float x)
        {
            transform.LocalEulerAngles = new float3((float)Math.Sin(x), (float)Math.Sin(x), (float)Math.Sin(x)) * (float)Time.Instance.DeltaTime;
            //Debug.WriteLine("Hello this is testbehaviour");
            speed += 0.1f;
            Time.Instance.TimeFlow = speed;
        }

    }
}
