
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Fusee.Engine;
using Fusee.Math;
using Fusee.SceneManagement;

namespace Examples.Components
{
    class ChildAction : ActionCode
    {
        private float speed = 0.1f;
        private float x=0.5f;
        private SceneEntity parenttest;
        private TestBehaviour parentscript;
        public override void Start()
        {
            transform.LocalPosition = new float3(2, 2, 0);
            renderer.color= new float4(0,1,0,1);
            //renderer.material.SwitchTexture();
            //parenttest = SceneEntity.FindSceneEntity("erster");
            //parentscript = parenttest.GetComponent<TestBehaviour>();

            Time.Instance.TimeFlow = speed;

        }

        public override void Update()
        {
            if(Input.Instance.IsButtonDown(MouseButtons.Left))
            {
                //parentscript.Test(x);
                //transform.LocalEulerAngles = new float3(0, x, 0);
                //x -= 0.5f*(float)Time.Instance.DeltaTime;
                //speed += 0.05f;
                transform.LocalPosition += new float3(0,0,0.1f);
            }
            else
            {
                //speed = 0.1f;
            }
            //Time.Instance.TimeFlow = speed;
            Debug.WriteLine("Smooth Framerate: " + Time.Instance.FamePerSecondSmooth);
            Debug.WriteLine("Normal Framerate: " + Time.Instance.FramePerSecond);

        }
    }
}
