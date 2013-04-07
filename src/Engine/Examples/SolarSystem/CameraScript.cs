using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                Debug.WriteLine("mouse wheel");
                transform.LocalPosition -= new float3(0, 0, (Input.Instance.GetAxis(InputAxis.MouseWheel)*100));
            }

           
        }

    }
}
