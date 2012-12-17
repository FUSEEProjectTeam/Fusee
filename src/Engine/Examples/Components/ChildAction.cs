
using System.Collections.Generic;
using System.Text;
using Fusee.Engine;
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
            renderer.color= new float4(0,1,0,1);
        }

        public override void Update()
        {
            if(Input.IsButtonDown(MouseButtons.Left))
            {
                transform.LocalEulerAngles = new float3(x, 0, 0);
                x +=3*(float)DeltaTime;
            }
        }
    }
}
