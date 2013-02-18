
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
        private SceneEntity parenttest;
        private TestBehaviour parentscript;
        public override void Start()
        {
            transform.LocalPosition = new float3(2, 2, 0);
            renderer.color= new float4(0,1,0,1);
            //renderer.material.SwitchTexture();
            parenttest = SceneEntity.FindSceneEntity("erster");
            parentscript = parenttest.GetComponent<TestBehaviour>();
        }

        public override void Update()
        {
            if(Input.IsButtonDown(MouseButtons.Left))
            {
                parentscript.Test(x);
                transform.LocalEulerAngles = new float3(0, x, 0);
                x -= 0.5f*(float)DeltaTime;
            }
        }
    }
}
