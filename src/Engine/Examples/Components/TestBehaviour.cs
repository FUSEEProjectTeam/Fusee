using System;
using Fusee.Math;
using Fusee.SceneManagement;
namespace Examples.Components
{
    public class TestBehaviour : ActionCode
    {
        private float x;

        
        public override void Start()
        {
            transform.LocalEulerAngles = new float3(0, 90, 0);
            transform.LocalPosition = new float3(100,20,30);
            
        }

        public override void Update()
        {

            transform.LocalQuaternion = Quaternion.FromAxisAngle(new float3(0, 0, 1), x);
            Quaternion q = Quaternion.MatrixToQuaternion(transform.Matrix);
            x += 0.1F*(float)DeltaTime;
        }
    }
}
