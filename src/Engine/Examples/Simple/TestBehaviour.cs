
using Fusee.Math;
using Fusee.SceneManagement;
namespace Examples.Simple
{
    public class TestBehaviour : Action
    {
        private float x;

        
        public override void Start()
        {
            transform.LocalEulerAngles = new float3(0, 90, 0);
            transform.LocalPosition = new float3(100,20,30);
            
        }

        public override void Update()
        {
            //transform.Matrix = float4x4.CreateRotationY(x);
            transform.LocalEulerAngles=new float3(0,x,0);
            //transform.LocalPosition+=new float3(-x,x,x);
            //transform.LocalScale = new float3(x,x,x);
            x += 0.01F;
            //Console.WriteLine("Internal: "+transform.Matrix.ToString());
            //Console.WriteLine("World: "+transform.WorldMatrix.ToString());
            //Console.WriteLine(this.ToString()+" is running.");
            //transform.LocalPosition+= new float3(0.01f,0,0);
            //Console.WriteLine(transform.WorldMatrix.ToString());
        }
    }
}
