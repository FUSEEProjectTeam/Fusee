using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;
using Fusee.Engine;
using Fusee.SceneManagement;
namespace Examples.SolarSystem
{
    class CameraScript : ActionCode
    {
        public override void Start()
        {
            //transform.LocalPosition= new float3(0,0,-100);
        }
        public override void Update()
        {
            

            if (Input.Instance.GetAxis(InputAxis.MouseWheel) != 0)
            {
                transform.LocalPosition -=new float3(0,0, (float)(Input.Instance.GetAxis(InputAxis.MouseWheel)*Time.Instance.DeltaTime)*1000);
            }
        }

        void SwitchTarget()
        {

        }
    }
}
